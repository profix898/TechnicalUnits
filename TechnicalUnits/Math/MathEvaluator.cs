using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechnicalUnits.Formatting;
using TechnicalUnits.Internal;
using TechnicalUnits.Math.Expressions;
using static System.Char;

namespace TechnicalUnits.Math;

public sealed class MathEvaluator
{
    private readonly UnitOptions unitOptions = new UnitOptions();

    private readonly List<string> functionList;
    private readonly Dictionary<string, ExpressionBase> expressionCache;

    public MathEvaluator()
    {
        functionList = new List<string>(FunctionExpression.GetFunctionNames());
        functionList.Sort();

        expressionCache = new Dictionary<string, ExpressionBase>(StringComparer.OrdinalIgnoreCase);
    }

    public Dictionary<string, double> Constants { get; } = new Dictionary<string, double>()
    {
        { "pi", System.Math.PI },
        { "e", System.Math.E }
    };

    #region Functions

    public IReadOnlyList<string> Functions => functionList;

    public void RegisterFunction(string functionName, ExpressionBase expression)
    {
        if (String.IsNullOrEmpty(functionName))
            throw new ArgumentNullException(nameof(functionName));
        if (expression == null)
            throw new ArgumentNullException(nameof(expression));

        if (functionList.BinarySearch(functionName) >= 0)
            throw new ArgumentException($"The function name '{functionName}' is already registered.", nameof(functionName));

        functionList.Add(functionName);
        functionList.Sort();
        expressionCache.Add(functionName, expression);
    }

    private bool IsFunction(string name)
    {
        return functionList.BinarySearch(name, StringComparer.OrdinalIgnoreCase) >= 0;
    }

    #endregion

    public double Evaluate(string expression, List<Exception>? warnings = null)
    {
        if (String.IsNullOrEmpty(expression))
            throw new ArgumentNullException(nameof(expression));

        using var state = new EvaluatorState(expression, warnings);

        ParseExpression(state);

        return EvaluateExpression(state);
    }

    #region Parser

    private static char PeekNextNonWhitespaceChar(EvaluatorState state)
    {
        var peekChar = state.expressionReader.Peek();
        while (peekChar != -1 && IsWhiteSpace((char) peekChar))
        {
            state.expressionReader.Read();
            peekChar = state.expressionReader.Peek();
        }

        return (char) peekChar;
    }

    private void ParseExpression(EvaluatorState state)
    {
        // Parse expression
        var lastChar = '\0';
        state.currentChar = '\0';
        state.lastType = TokenType.Unknown;

        do
        {
            if (!IsWhiteSpace(state.currentChar))
                lastChar = state.currentChar;

            state.currentChar = (char) state.expressionReader.Peek();

            if (ParseNumber(state, lastChar))
                continue;

            state.currentChar = (char) state.expressionReader.Read();

            if (IsWhiteSpace(state.currentChar))
                continue;

            if (ParseString(state))
                continue;

            if (ParseGroupOpen(state))
                continue;

            if (ParseComma(state))
                continue;

            if (ParseOperator(state))
                continue;

            if (ParseGroupClose(state))
                continue;

            if (ParseConversion(state))
                continue;

            throw new MathEvaluatorException($"Invalid character '{state.currentChar}'.");
        }
        while (state.expressionReader.Peek() != -1);

        // Process symbol stack
        while (state.symbolStack.Count > 0)
        {
            var symbol = state.symbolStack.Pop();
            if (symbol.Length == 1 && symbol == "(")
                throw new MathEvaluatorException("Unbalanced parentheses.");

            var expression = GetExpressionFromSymbol(symbol);
            state.expressionQueue.Enqueue(expression);
        }
    }

    private ExpressionBase GetExpressionFromSymbol(string symbol)
    {
        ExpressionBase expression;
        if (expressionCache.TryGetValue(symbol, out var value))
            expression = value;
        else if (OperatorExpression.IsOperator(symbol))
        {
            expression = new OperatorExpression(symbol);
            expressionCache.Add(symbol, expression);
        }
        else if (FunctionExpression.IsFunction(symbol))
        {
            expression = new FunctionExpression(symbol, false);
            expressionCache.Add(symbol, expression);
        }
        else if (ConvertExpression.IsConvertExpression(symbol))
        {
            expression = new ConvertExpression(symbol);
            expressionCache.Add(symbol, expression);
        }
        else
            throw new MathEvaluatorException($"Invalid symbol '{symbol}' on stack.");

        return expression;
    }

    #region Tokens

    private bool ParseString(EvaluatorState state)
    {
        if (!IsLetter(state.currentChar))
            return false;

        state.buffer.Length = 0;
        state.buffer.Append(state.currentChar);

        var peekChar = (char) state.expressionReader.Peek();
        while (IsLetter(peekChar) || IsNumber(peekChar))
        {
            state.buffer.Append((char) state.expressionReader.Read());
            peekChar = (char) state.expressionReader.Peek();
        }

        if (Constants.ContainsKey(state.buffer.ToString()))
        {
            var value = Constants[state.buffer.ToString()];
            var expression = new NumberExpression(value);
            state.expressionQueue.Enqueue(expression);

            return true;
        }

        if (IsFunction(state.buffer.ToString()))
        {
            state.symbolStack.Push(state.buffer.ToString());
            state.nestedFunctionDepth++;

            return true;
        }

        throw new MathEvaluatorException($"Invalid function or variable '{state.buffer}'.");
    }

    private bool ParseNumber(EvaluatorState state, char lastChar)
    {
        var lastType = state.lastType;

        var isNumber = NumberExpression.IsNumber(state.currentChar);
        var isNegative = NumberExpression.IsNegativeSign(state.currentChar) &&
                         (lastChar == '\0' || lastChar == '(' || lastType == TokenType.OperatorSymbol);
        var isPositive = NumberExpression.IsPositiveSign(state.currentChar) &&
                         (lastChar == '\0' || lastChar == '(' || lastType == TokenType.OperatorSymbol);

        if (!isNumber && !isNegative && !isPositive)
            return false;

        // Parse number (without unit, but supporting SI prefix notation)
        var value = Parser.ParseStream(state.expressionReader, unitOptions, FormattingOptions.Default, state.warnings);

        var expression = new NumberExpression(value);
        state.expressionQueue.Enqueue(expression);

        state.lastType = TokenType.Number;
        return true;
    }

    private bool ParseOperator(EvaluatorState state)
    {
        if (!OperatorExpression.IsOperator(state.currentChar))
            return false;

        bool repeat;
        var str = state.currentChar.ToString();

        do
        {
            var symbol = (state.symbolStack.Count == 0) ? String.Empty : state.symbolStack.Peek();
            repeat = false;
            if (state.symbolStack.Count == 0)
                state.symbolStack.Push(str);
            else if (symbol == "(")
                state.symbolStack.Push(str);
            else if (Precedence(str) > Precedence(symbol))
                state.symbolStack.Push(str);
            else
            {
                var e = GetExpressionFromSymbol(state.symbolStack.Pop());
                state.expressionQueue.Enqueue(e);
                repeat = true;
            }
        }
        while (repeat);

        state.lastType = TokenType.OperatorSymbol;
        return true;

        // Local function: Precedence
        static int Precedence(string str) => (str.Length == 1 && (str[0] == '*' || str[0] == '/')) ? 2 : 1;
    }

    private bool ParseGroupOpen(EvaluatorState state)
    {
        if (state.currentChar != '(')
            return false;

        var nextChar = PeekNextNonWhitespaceChar(state);
        if (nextChar == ',' || nextChar == ';')
            throw new MathEvaluatorException($"Invalid character '{nextChar}'.");

        state.symbolStack.Push(state.currentChar.ToString());
        state.nestedGroupDepth++;

        state.lastType = TokenType.GroupOpen;
        return true;
    }

    private bool ParseGroupClose(EvaluatorState state)
    {
        if (state.currentChar != ')')
            return false;

        var hasStart = false;

        while (state.symbolStack.Count > 0)
        {
            var str = state.symbolStack.Pop();
            if (str == "(")
            {
                hasStart = true;

                if (state.symbolStack.Count == 0)
                    break;

                var next = state.symbolStack.Peek();
                if (IsFunction(next))
                {
                    str = state.symbolStack.Pop();
                    state.expressionQueue.Enqueue(GetExpressionFromSymbol(str));
                    state.nestedFunctionDepth--;
                }

                state.nestedGroupDepth--;
                break;
            }

            state.expressionQueue.Enqueue(GetExpressionFromSymbol(str));
        }

        if (!hasStart)
            throw new MathEvaluatorException("Unbalanced parentheses.");

        state.lastType = TokenType.GroupClose;
        return true;
    }

    private bool ParseComma(EvaluatorState state)
    {
        if (state.currentChar != ',' || state.currentChar != ';')
            return false;

        if (state.nestedFunctionDepth <= 0 || state.nestedFunctionDepth < state.nestedGroupDepth)
            throw new MathEvaluatorException($"Invalid character '{state.currentChar}'.");

        var nextChar = PeekNextNonWhitespaceChar(state);
        if (nextChar == ')' || nextChar == ',' || nextChar == ';')
            throw new MathEvaluatorException($"Invalid character '{state.currentChar}'.");

        state.lastType = TokenType.Comma;
        return true;
    }

    private bool ParseConversion(EvaluatorState state)
    {
        if (state.currentChar != '[')
            return false;

        state.buffer.Length = 0;
        state.buffer.Append(state.currentChar);

        var peekChar = (char) state.expressionReader.Peek();
        while (IsLetter(peekChar) || IsWhiteSpace(peekChar) || peekChar == '>' || peekChar == ']')
        {
            if (!IsWhiteSpace(peekChar))
                state.buffer.Append((char) state.expressionReader.Read());
            else
                state.expressionReader.Read();

            if (peekChar == ']')
                break;

            peekChar = (char) state.expressionReader.Peek();
        }

        if (ConvertExpression.IsConvertExpression(state.buffer.ToString()))
        {
            state.expressionQueue.Enqueue(GetExpressionFromSymbol(state.buffer.ToString()));

            state.lastType = TokenType.Conversion;
            return true;
        }

        throw new MathEvaluatorException($"Invalid conversion expression '{state.buffer}'.");
    }

    #endregion

    #endregion

    #region Evaluator

    private static double EvaluateExpression(EvaluatorState state)
    {
        state.evaluationStack.Clear();

        foreach (var expression in state.expressionQueue)
        {
            if (state.evaluationStack.Count < expression.ArgumentCount)
                throw new MathEvaluatorException($"Invalid number of arguments for expression '{expression}'.");

            state.parameters.Clear();
            for (var i = 0; i < expression.ArgumentCount; i++)
                state.parameters.Push(state.evaluationStack.Pop());

            state.evaluationStack.Push(expression.Evaluate(state.parameters.ToArray()));
        }

        var result = state.evaluationStack.Pop();

        if (state.evaluationStack.Any())
            throw new MathEvaluatorException($"Invalid evaluation stack: Items '{String.Join(", ", state.evaluationStack)}' remaining.");

        return result;
    }

    #endregion

    #region Nested Type: TokenType

    private enum TokenType
    {
        Unknown = 0,
        Number,
        OperatorSymbol,
        GroupOpen,
        GroupClose,
        Comma,
        Conversion
    }

    #endregion

    #region Nested Type: EvaluatorState

    private sealed class EvaluatorState : IDisposable
    {
        public EvaluatorState(string expression, List<Exception>? warnings = null)
        {
            expressionReader = new StringReaderLookahead(expression);
            this.warnings = warnings ?? new List<Exception>();
        }

        public readonly StringReaderLookahead expressionReader;
        public readonly List<Exception> warnings;

        // Parser
        public readonly StringBuilder buffer = new StringBuilder();
        public readonly Queue<ExpressionBase> expressionQueue = new Queue<ExpressionBase>();
        public readonly Stack<string> symbolStack = new Stack<string>();
    
        // Parser (mutable) state
        public char currentChar;
        public TokenType lastType;
        public uint nestedFunctionDepth;
        public uint nestedGroupDepth;

        // Evaluation
        public readonly Stack<double> evaluationStack = new Stack<double>();
        public readonly Stack<double> parameters = new Stack<double>(2);

        #region Implementation of IDisposable

        public void Dispose()
        {
            expressionReader.Dispose();
        }

        #endregion
    }

    #endregion
}
