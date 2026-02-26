using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechnicalUnits.Formatting;
using TechnicalUnits.Internal;
using TechnicalUnits.Math.Expressions;
using static System.Char;

namespace TechnicalUnits.Math;

/// <summary>
/// Evaluates mathematical expressions that may contain SI-prefixed numbers, unit symbols,
/// built-in functions (sin, cos, sqrt, …), named constants (pi, e, c, …), and unit
/// conversion brackets (<c>[°C>K]</c>).
/// </summary>
/// <remarks>
/// The evaluator uses the Shunting-Yard algorithm to convert infix notation into a
/// postfix (Reverse Polish Notation) queue, which is then evaluated left-to-right.
/// <para>
/// This class is <b>not</b> thread-safe. Do not share a single instance across threads
/// without external synchronisation.
/// </para>
/// </remarks>
public sealed class MathEvaluator
{
    private readonly Dictionary<string, ExpressionBase> _expressionCache;
    private readonly List<string> _functionList;

    /// <summary>
    /// Initializes a new <see cref="MathEvaluator" /> with the default built-in functions and constants.
    /// </summary>
    public MathEvaluator()
    {
        _functionList = new List<string>(FunctionExpression.GetFunctionNames());
        _functionList.Sort();

        _expressionCache = new Dictionary<string, ExpressionBase>(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the dictionary of named constants available during evaluation.
    /// Includes mathematical constants (pi, e) and physical constants (c, h, kB, …).
    /// Entries can be added or removed at runtime.
    /// </summary>
    public Dictionary<string, double> Constants { get; } = new Dictionary<string, double>()
    {
        // Math constants
        { "pi", System.Math.PI }, { "e", System.Math.E }, { "sqrt2", System.Math.Sqrt(2) },

        // Physical constants
        { "c", 299792458 }, // Speed of light in m/s
        { "g", 9.80665 }, // Standard gravity in m/s²
        { "h", 6.62607015e-34 }, // Planck's constant in J·s
        { "hbar", 1.054571817e-34 }, // Reduced Planck's constant in J·s
        { "kB", 1.380649e-23 }, // Boltzmann constant in J/K
        { "NA", 6.02214076e23 }, // Avogadro's number
        { "Gc", 6.67430e-11 }, // Gravitational constant
        { "qe", 1.602176634e-19 }, // Elementary charge in C
        { "me", 9.1093837015e-31 }, // Electron mass in kg
        { "mp", 1.67262192369e-27 }, // Proton mass in kg
        { "mn", 1.67492749804e-27 }, // Neutron mass in kg
        { "mu0", 1.25663706212e-6 }, // Vacuum permeability
        { "eps0", 8.8541878128e-12 }, // Vacuum permittivity in F/m
        { "R", 8.314462618 }, // Gas constant in J/(mol·K)
        { "Ry", 2.1798723611035e-18 }, // Rydberg constant in J
        { "F", 96485.33212 }, // Faraday constant in C/mol
        { "Vm", 22.413962 }, // Molar volume of ideal gas at STP in L/mol
        { "atm", 101325 } // Standard atmosphere
    };

    /// <summary>
    /// Parses and evaluates a mathematical expression string.
    /// </summary>
    /// <param name="expression">The infix expression to evaluate (e.g. <c>"2 * sin(pi / 4)"</c>).</param>
    /// <param name="unitOptions">Unit options used for number/unit parsing.</param>
    /// <param name="formattingOptions">Formatting options used for number parsing.</param>
    /// <param name="warnings">An optional list that receives non-fatal parsing warnings.</param>
    /// <returns>The evaluated result as a <see cref="double" />.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expression" /> is <c>null</c> or empty.</exception>
    /// <exception cref="MathEvaluatorException">The expression contains invalid syntax.</exception>
    public double Evaluate(string expression, UnitOptions unitOptions, FormattingOptions formattingOptions, List<Exception>? warnings)
    {
        if (String.IsNullOrEmpty(expression))
            throw new ArgumentNullException(nameof(expression));

        using var state = new EvaluatorState(expression, unitOptions, formattingOptions, warnings);

        ParseExpression(state);

        return EvaluateExpression(state);
    }

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

    #region Nested Type: EvaluatorState

    private sealed class EvaluatorState : IDisposable
    {
        // Parser
        public readonly StringBuilder buffer = new StringBuilder();

        // Evaluation
        public readonly Stack<double> evaluationStack = new Stack<double>();
        public readonly Queue<ExpressionBase> expressionQueue = new Queue<ExpressionBase>();

        public readonly StringReaderLookahead expressionReader;
        public readonly FormattingOptions formattingOptions;
        public readonly Stack<double> parameters = new Stack<double>(2);
        public readonly Stack<string> symbolStack = new Stack<string>();

        // Options
        public readonly UnitOptions unitOptions;
        public readonly List<Exception> warnings;

        // Parser (mutable) state
        public char currentChar;
        public TokenType lastType;
        public uint nestedFunctionDepth;
        public uint nestedGroupDepth;

        public EvaluatorState(string expression, UnitOptions unitOptions, FormattingOptions formattingOptions, List<Exception>? warnings = null)
        {
            expressionReader = new StringReaderLookahead(expression);
            this.warnings = warnings ?? [];

            this.unitOptions = unitOptions;
            this.formattingOptions = formattingOptions;
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            expressionReader.Dispose();
        }

        #endregion
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

    #region Functions

    /// <summary>Gets the sorted list of registered function names.</summary>
    public IReadOnlyList<string> Functions => _functionList;

    /// <summary>
    /// Registers a custom function that can be used in expressions.
    /// </summary>
    /// <param name="functionName">The name of the function (case-insensitive).</param>
    /// <param name="expression">The expression that implements the function logic.</param>
    /// <exception cref="ArgumentNullException"><paramref name="functionName" /> or <paramref name="expression" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">A function with the same name is already registered.</exception>
    public void RegisterFunction(string functionName, ExpressionBase expression)
    {
        if (String.IsNullOrEmpty(functionName))
            throw new ArgumentNullException(nameof(functionName));
        if (expression == null)
            throw new ArgumentNullException(nameof(expression));

        if (_functionList.BinarySearch(functionName) >= 0)
            throw new ArgumentException($"Function name '{functionName}' is already registered.", nameof(functionName));

        _functionList.Add(functionName);
        _functionList.Sort();
        _expressionCache.Add(functionName, expression);
    }

    private bool IsFunction(string name) => _functionList.BinarySearch(name, StringComparer.OrdinalIgnoreCase) >= 0;

    #endregion

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
        if (_expressionCache.TryGetValue(symbol, out var value))
            expression = value;
        else if (OperatorExpression.IsOperator(symbol))
        {
            expression = new OperatorExpression(symbol);
            _expressionCache.Add(symbol, expression);
        }
        else if (FunctionExpression.IsFunction(symbol))
        {
            expression = new FunctionExpression(symbol, false);
            _expressionCache.Add(symbol, expression);
        }
        else if (ConvertExpression.IsConvertExpression(symbol))
        {
            expression = new ConvertExpression(symbol);
            _expressionCache.Add(symbol, expression);
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
        var isNegative = NumberExpression.IsNegativeSign(state.currentChar) && (lastChar == '\0' || lastChar == '(' || lastType == TokenType.OperatorSymbol);
        var isPositive = NumberExpression.IsPositiveSign(state.currentChar) && (lastChar == '\0' || lastChar == '(' || lastType == TokenType.OperatorSymbol);

        if (!isNumber && !isNegative && !isPositive)
            return false;

        // Parse number (without unit, but supporting SI prefix notation)
        var value = Parser.ParseStream(state.expressionReader, state.unitOptions, state.formattingOptions, state.warnings);

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
            var symbol = state.symbolStack.Count == 0 ? String.Empty : state.symbolStack.Peek();
            repeat = false;
            if (state.symbolStack.Count == 0)
                state.symbolStack.Push(str);
            else if (symbol == "(")
                state.symbolStack.Push(str);
            else if (Precedence(str) > Precedence(symbol) || (Precedence(str) == Precedence(symbol) && IsRightAssociative(str)))
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

        // Local functions
        static int Precedence(string str)
        {
            if (str.Length != 1)
                return 1;
            return str[0] switch
            {
                '*' or '/' => 2,
                '^' => 3,
                _ => 1
            };
        }

        static bool IsRightAssociative(string str) => str.Length == 1 && str[0] == '^';
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
        if (state.currentChar != ',' && state.currentChar != ';')
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
}
