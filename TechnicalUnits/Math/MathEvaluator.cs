using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using TechnicalUnits.Formatting;
using TechnicalUnits.Internal;
using TechnicalUnits.Math.Expressions;

namespace TechnicalUnits.Math;

public sealed class MathEvaluator : IDisposable
{
    private readonly StringBuilder buffer;
    private readonly Stack<double> calculationStack;
    private readonly Dictionary<string, MathExpressionBase> expressionCache;
    private readonly Queue<MathExpressionBase> expressionQueue;
    private readonly List<string> innerFunctions;
    private readonly Stack<double> parameters;

    //instance scope to optimize reuse
    private readonly Stack<string> symbolStack;
    private readonly UnitOptions unitOptions;
    private char currentChar;
    private StringReaderLookahead? expressionReader;
    private ExpressionAtomType lastType;
    private uint nestedFunctionDepth;
    private uint nestedGroupDepth;

    private List<Exception> warnings;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MathEvaluator" /> class.
    /// </summary>
    public MathEvaluator()
    {
        Variables = new VariableWorkspace(this);
        innerFunctions = new List<string>(FunctionMathExpression.GetFunctionNames());
        innerFunctions.Sort();
        Functions = new ReadOnlyCollection<string>(innerFunctions);
        expressionCache = new Dictionary<string, MathExpressionBase>(StringComparer.OrdinalIgnoreCase);
        symbolStack = new Stack<string>();
        expressionQueue = new Queue<MathExpressionBase>();
        buffer = new StringBuilder();
        calculationStack = new Stack<double>();
        parameters = new Stack<double>(2);
        nestedFunctionDepth = 0;
        nestedGroupDepth = 0;

        unitOptions = new UnitOptions();
    }

    public double Answer
    {
        get { return Variables[VariableWorkspace.AnswerVariable]; }
    }

    public ReadOnlyCollection<string> Functions { get; }

    public VariableWorkspace Variables { get; }

    public double Evaluate(string expression, List<Exception>? warnings = null)
    {
        if (String.IsNullOrEmpty(expression))
            throw new ArgumentNullException(nameof(expression));

        this.warnings = warnings ?? new List<Exception>();

        expressionReader = new StringReaderLookahead(expression);
        symbolStack.Clear();
        nestedFunctionDepth = 0;
        nestedGroupDepth = 0;
        expressionQueue.Clear();

        ParseExpressionToQueue();

        var result = CalculateFromQueue();
        Variables[VariableWorkspace.AnswerVariable] = result;

        return result;
    }

    public void RegisterFunction(string functionName, MathExpressionBase expression)
    {
        if (String.IsNullOrEmpty(functionName))
            throw new ArgumentNullException(nameof(functionName));
        if (expression == null)
            throw new ArgumentNullException(nameof(expression));

        if (innerFunctions.BinarySearch(functionName) >= 0)
        {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "The function name '{0}' is already registered.", functionName), nameof(functionName));
        }

        innerFunctions.Add(functionName);
        innerFunctions.Sort();
        expressionCache.Add(functionName, expression);
    }

    internal bool IsFunction(string name)
    {
        return innerFunctions.BinarySearch(name, StringComparer.OrdinalIgnoreCase) >= 0;
    }

    private void ParseExpressionToQueue()
    {
        var lastChar = '\0';
        currentChar = '\0';
        lastType = ExpressionAtomType.Unknown;

        do
        {
            // last non white space char
            if (!Char.IsWhiteSpace(currentChar))
                lastChar = currentChar;

            currentChar = (char) expressionReader.Peek(); // some checks depend on the expression reader containing the current char as well

            if (TryNumber(lastChar, lastType))
                continue;

            currentChar = (char) expressionReader
                .Read(); // some checks depend on the expression reader _not_ containing the current char and read it from a private variable.

            if (Char.IsWhiteSpace(currentChar))
                continue;

            if (TryString())
                continue;

            if (TryStartGroup())
                continue;

            if (TryComma())
                continue;

            if (TryOperator())
                continue;

            if (TryEndGroup())
                continue;

            if (TryConvert())
                continue;

            throw new MathEvaluatorException("Invalid character: " + currentChar);
        }
        while (expressionReader.Peek() != -1);

        ProcessSymbolStack();
    }

    private bool TryConvert()
    {
        if (currentChar != '[')
            return false;

        buffer.Length = 0;
        buffer.Append(currentChar);

        var p = (char) expressionReader.Peek();
        while (Char.IsLetter(p) || Char.IsWhiteSpace(p) || p == '>' || p == ']')
        {
            if (!Char.IsWhiteSpace(p))
                buffer.Append((char) expressionReader.Read());
            else
                expressionReader.Read();

            if (p == ']')
                break;

            p = (char) expressionReader.Peek();
        }

        if (ConvertMathExpression.IsConvertExpression(buffer.ToString()))
        {
            var e = GetExpressionFromSymbol(buffer.ToString());
            expressionQueue.Enqueue(e);
            lastType = ExpressionAtomType.Convert;
            return true;
        }

        throw new MathEvaluatorException("Invalid convertion expression: " + buffer);
    }

    private bool TryString()
    {
        if (!Char.IsLetter(currentChar))
            return false;

        buffer.Length = 0;
        buffer.Append(currentChar);

        var p = (char) expressionReader.Peek();
        while (Char.IsLetter(p) || Char.IsNumber(p))
        {
            buffer.Append((char) expressionReader.Read());
            p = (char) expressionReader.Peek();
        }

        if (Variables.ContainsKey(buffer.ToString()))
        {
            var value = Variables[buffer.ToString()];
            var expression = new NumberMathExpression(value);
            expressionQueue.Enqueue(expression);

            return true;
        }

        if (IsFunction(buffer.ToString()))
        {
            symbolStack.Push(buffer.ToString());
            nestedFunctionDepth++;
            return true;
        }

        throw new MathEvaluatorException("Invalid function or variable: " + buffer);
    }

    private bool TryStartGroup()
    {
        if (currentChar != '(')
            return false;

        var cnw = PeekNextNonWhitespaceChar();
        if (cnw == ',' || cnw == ';')
            throw new MathEvaluatorException("Invalid character: " + cnw);

        symbolStack.Push(currentChar.ToString());
        nestedGroupDepth++;

        lastType = ExpressionAtomType.GroupOpen;
        return true;
    }

    private bool TryComma()
    {
        if (currentChar != ',' || currentChar != ';')
            return false;

        if (nestedFunctionDepth <= 0 ||
            nestedFunctionDepth < nestedGroupDepth)
            throw new MathEvaluatorException("Invalid character: " + currentChar);

        var nextChar = PeekNextNonWhitespaceChar();
        if (nextChar == ')' || nextChar == ',' || nextChar == ';')
            throw new MathEvaluatorException("Invalid character: " + currentChar);

        lastType = ExpressionAtomType.Comma;
        return true;
    }

    private char PeekNextNonWhitespaceChar()
    {
        var next = expressionReader.Peek();
        while (next != -1 && Char.IsWhiteSpace((char) next))
        {
            expressionReader.Read();
            next = expressionReader.Peek();
        }
        return (char) next;
    }

    private bool TryEndGroup()
    {
        if (currentChar != ')')
            return false;

        var hasStart = false;

        while (symbolStack.Count > 0)
        {
            var p = symbolStack.Pop();
            if (p == "(")
            {
                hasStart = true;

                if (symbolStack.Count == 0)
                    break;

                var n = symbolStack.Peek();
                if (IsFunction(n))
                {
                    p = symbolStack.Pop();
                    var f = GetExpressionFromSymbol(p);
                    expressionQueue.Enqueue(f);
                    nestedFunctionDepth--;
                }

                nestedGroupDepth--;

                break;
            }

            var e = GetExpressionFromSymbol(p);
            expressionQueue.Enqueue(e);
        }

        if (!hasStart)
            throw new MathEvaluatorException("Unbalanced parentheses.");

        lastType = ExpressionAtomType.GroupClose;
        return true;
    }

    private bool TryOperator()
    {
        if (!OperatorMathExpression.IsOperator(currentChar))
            return false;

        bool repeat;
        var s = currentChar.ToString();

        do
        {
            var p = symbolStack.Count == 0 ? String.Empty : symbolStack.Peek();
            repeat = false;
            if (symbolStack.Count == 0)
                symbolStack.Push(s);
            else if (p == "(")
                symbolStack.Push(s);
            else if (Precedence(s) > Precedence(p))
                symbolStack.Push(s);
            else
            {
                var e = GetExpressionFromSymbol(symbolStack.Pop());
                expressionQueue.Enqueue(e);
                repeat = true;
            }
        }
        while (repeat);

        lastType = ExpressionAtomType.OperatorSymbol;
        return true;
    }

    private bool TryNumber(char lastChar, ExpressionAtomType lastType)
    {
        var isNumber = NumberMathExpression.IsNumber(currentChar);

        // only negative when last char is group start or symbol
        var isNegative = NumberMathExpression.IsNegativeSign(currentChar) &&
                         (lastChar == '\0' || lastChar == '(' || lastType == ExpressionAtomType.OperatorSymbol);
        var isPositive = NumberMathExpression.IsPositiveSign(currentChar) &&
                         (lastChar == '\0' || lastChar == '(' || lastType == ExpressionAtomType.OperatorSymbol);

        if (!isNumber && !isNegative && !isPositive)
            return false;

        var lst = new List<Exception>();

        var value = Parser.ParseStream(expressionReader, unitOptions, FormattingOptions.Default, lst);

        if (lst.Count > 0)
            warnings.AddRange(lst);

        var expression = new NumberMathExpression(value);
        expressionQueue.Enqueue(expression);

        this.lastType = ExpressionAtomType.Number;

        return true;
    }

    private void ProcessSymbolStack()
    {
        while (symbolStack.Count > 0)
        {
            var p = symbolStack.Pop();
            if (p.Length == 1 && p == "(")
                throw new MathEvaluatorException("Unbalanced parentheses.");

            var e = GetExpressionFromSymbol(p);
            expressionQueue.Enqueue(e);
        }
    }

    private MathExpressionBase GetExpressionFromSymbol(string p)
    {
        MathExpressionBase e;

        if (expressionCache.ContainsKey(p))
            e = expressionCache[p];
        else if (OperatorMathExpression.IsOperator(p))
        {
            e = new OperatorMathExpression(p);
            expressionCache.Add(p, e);
        }
        else if (FunctionMathExpression.IsFunction(p))
        {
            e = new FunctionMathExpression(p, false);
            expressionCache.Add(p, e);
        }
        else if (ConvertMathExpression.IsConvertExpression(p))
        {
            e = new ConvertMathExpression(p);
            expressionCache.Add(p, e);
        }
        else
            throw new MathEvaluatorException("Invalid symbol on stack: " + p);

        return e;
    }

    private static int Precedence(string c)
    {
        if (c.Length == 1 && (c[0] == '*' || c[0] == '/' || c[0] == '%'))
            return 2;

        return 1;
    }

    private double CalculateFromQueue()
    {
        calculationStack.Clear();

        foreach (var expression in expressionQueue)
        {
            if (calculationStack.Count < expression.ArgumentCount)
                throw new MathEvaluatorException("Invalid number of arguments for expression: " + expression);

            parameters.Clear();
            for (var i = 0; i < expression.ArgumentCount; i++)
                parameters.Push(calculationStack.Pop());

            calculationStack.Push(expression.Evaluate(parameters.ToArray()));
        }

        var result = calculationStack.Pop();

        if (calculationStack.Any())
            throw new MathEvaluatorException($"Invalid symbol on stack: Items '{String.Join(", ", calculationStack)}' were remaining on calculation stack.");

        return result;
    }

    #region Nested Type: ExpressionAtomType

    private enum ExpressionAtomType
    {
        Unknown = 0,
        Number,
        OperatorSymbol,
        GroupOpen,
        GroupClose,
        Comma,
        Convert
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (expressionReader != null)
            {
                expressionReader.Dispose();
                expressionReader = null;
            }
        }
    }

    #endregion
}
