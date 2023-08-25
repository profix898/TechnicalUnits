using System;
using System.Collections.Generic;
using System.Text;
using TechnicalUnits.Internal;
using static System.Math;
using static TechnicalUnits.Internal.CharMatchHelper;

namespace TechnicalUnits.Formatting;

public static class Parser
{
    #region Parse

    public static double ParseString(string strValue, UnitOptions unitOptions, FormattingOptions? formattingOptions = null, List<Exception>? warnings = null)
    {
        var state = ParseStringInternal($"{strValue.Trim()} ", unitOptions, formattingOptions ?? FormattingOptions.Default);

        double value;
        if (state.postDecVal != 0)
            value = (double) state.preDecVal + (double) state.postDecVal;
        else
            value = (double) state.preDecVal;

        warnings?.AddRange(state.warnings);

        return state.sign * value * state.unitConvFactor * Pow(10, state.expSign * state.exp);
    }

    private static ParserState ParseStringInternal(string strValue, UnitOptions unitOptions, FormattingOptions formattingOptions)
    {
        var state = new ParserState();

        if (strValue.Length <= 0)
            return state;

        var i = 0;
        var oldState = ParserPartEnum.EndOfStrPart;
        var oldi = i;

        DiscoverUnitAtEndOfString(ref strValue, unitOptions, formattingOptions, state);

        while (state.currentPart != ParserPartEnum.EndOfStrPart && i < strValue.Length)
        {
            var ch = strValue[i];

            if (oldState == state.currentPart && oldi == i)
                throw new Exception("Internal Error: Neither state nor i have changed during a loop run.");

            oldState = state.currentPart;
            oldi = i;

            string str;
            int exp;
            switch (state.currentPart)
            {
                case ParserPartEnum.PreDecPart:
                    if (IsSign(ch))
                    {
                        state.currentPart = ParserPartEnum.PreDecSign;
                        break;
                    }

                    if (IsNumeric(ch))
                    {
                        state.currentPart = ParserPartEnum.PreDecNum;
                        break;
                    }

                    i++; // Ignore unknown character
                    state.warnings.Add(new UnknownCharacterException($"Unknown character '{ch}' ignored in PreDecPart.", ch));
                    break;

                case ParserPartEnum.PreDecSign:
                    if (IsSign(ch))
                    {
                        if (IsNegativeSign(ch))
                            state.sign *= -1;
                        i++;
                        break;
                    }

                    if (IsNumeric(ch))
                    {
                        state.currentPart = ParserPartEnum.PreDecNum;
                        break;
                    }

                    i++;
                    state.warnings.Add(new UnknownCharacterException($"Unknown character '{ch}' ignored in PreDecSign.", ch));
                    break;

                case ParserPartEnum.PreDecNum:
                    if (IsNumeric(ch))
                    {
                        state.decStr.Append(ch); // Append the number to the decStr
                        i++;
                        break;
                    }

                    state.preDecVal = Int32.Parse(state.decStr.ToString());
                    state.decStr.Clear();
                    state.currentPart = ParserPartEnum.DecSepPart;
                    break;

                case ParserPartEnum.DecSepPart:
                    str = state.chrStr.ToString();
                    if (SIPrefixes.IsSIPrefix(str, out exp))
                    {
                        // Check a more 'greedy' variety to avoid mistaking prefix words for single chars
                        FindGreedySIMatch(strValue, state, ref i, ref exp);

                        state.exp += exp;
                        state.chrStr.Clear(); // Reset the appending
                        state.currentPart = ParserPartEnum.PostDecPart;
                        state.siPrefixFound = true;
                        break;
                    }

                    if (SIPrefixes.IsExpPrefix(str))
                    {
                        state.chrStr.Clear(); // Reset the appending
                        state.currentPart = ParserPartEnum.ExpPart;
                        break;
                    }

                    if (IsDecSep(str, formattingOptions))
                    {
                        state.chrStr.Clear(); // Reset the appending
                        state.currentPart = ParserPartEnum.PostDecPart;
                        break;
                    }

                    if (IsUnit(str, unitOptions, ref state.unitConvFactor) > 0)
                    {
                        state.chrStr.Clear(); // Reset the appending
                        state.currentPart = ParserPartEnum.PostDecPart;
                        break;
                    }

                    if (IsNumeric(ch)) // This has to happen after the string is checked, since str is one character behind ch
                    {
                        // Should not happen on first iteration. Should also not happen on later iterations, since DecSep or ParseStreamHandleUnit should have advanced the state before the postDec part begins.
                        state.warnings.Add(new UnexpectedSyntaxException($"Discarding '{state.chrStr}' as it could not be identified as either SI symbol or unit (or a combination of both), followed by a number.", state.currentPart, state.chrStr.ToString()));
                        state.chrStr.Clear(); // Discard unidentifiable decSep 
                        state.currentPart = ParserPartEnum.PostDecPart;
                        break;
                    }

                    if (!IsBlank(ch))
                        state.chrStr.Append(ch); // Append for next run
                    i++;
                    break;

                case ParserPartEnum.PostDecPart:
                    str = state.chrStr.ToString();
                    if (!state.siPrefixFound && SIPrefixes.IsSIPrefix(str, out exp))
                    {
                        // Check a more 'greedy' variety to avoid mistaking prefix words for single chars
                        FindGreedySIMatch(strValue, state, ref i, ref exp);

                        state.exp += exp;
                        state.expSign = 1;
                        state.chrStr.Clear(); // Reset the appending
                        state.currentPart = ParserPartEnum.SuffixPart;
                        state.siPrefixFound = true;
                        break;
                    }

                    if (SIPrefixes.IsExpPrefix(str))
                    {
                        state.chrStr.Clear(); // Reset the appending
                        state.currentPart = ParserPartEnum.ExpPart;
                        break;
                    }

                    if (IsUnit(str, unitOptions, ref state.unitConvFactor) > 0)
                    {
                        state.chrStr.Clear(); // Reset the appending
                        state.currentPart = ParserPartEnum.SuffixPart;
                        break;
                    }

                    if (IsNumeric(ch))
                    {
                        state.currentPart = ParserPartEnum.PostDecNum;
                        break;
                    }

                    if (!IsBlank(ch))
                        state.chrStr.Append(ch); // Append for next run

                    i++;
                    break;

                case ParserPartEnum.PostDecNum:
                    if (IsNumeric(ch))
                    {
                        state.decStr.Append(ch); // Append for next run
                        i++;
                        break;
                    }

                    state.postDecVal = (decimal) Pow(10, -1 * state.decStr.Length) * Decimal.Parse(state.decStr.ToString());
                    state.decStr.Clear();
                    state.currentPart = ParserPartEnum.PostPostDecPart;
                    break;

                case ParserPartEnum.PostPostDecPart:
                    str = state.chrStr.ToString();
                    if (!state.siPrefixFound && SIPrefixes.IsSIPrefix(str, out exp))
                    {
                        // Check a more 'greedy' variety to avoid mistaking prefix words for single chars
                        FindGreedySIMatch(strValue, state, ref i, ref exp);

                        state.exp += exp;
                        state.expSign = 1;
                        state.chrStr.Clear(); // Reset the appending
                        state.currentPart = ParserPartEnum.SuffixPart;
                        state.siPrefixFound = true;
                        break;
                    }

                    if (SIPrefixes.IsExpPrefix(str))
                    {
                        state.chrStr.Clear(); // Reset the appending
                        state.currentPart = ParserPartEnum.ExpPart;
                        break;
                    }

                    if (IsUnit(str, unitOptions, ref state.unitConvFactor) > 0)
                    {
                        state.chrStr.Clear(); // Reset the appending
                        state.currentPart = ParserPartEnum.EndOfStrPart;
                        break;
                    }

                    if (IsNumeric(ch))
                    {
                        state.warnings.Add(new UnexpectedSyntaxException($"Unexpected numeric '{ch}' in PostPostDecPart (unsupported syntax).", state.currentPart, str + ch));
                        state.currentPart = ParserPartEnum.EndOfStrPart;
                        break;
                    }

                    if (!IsBlank(ch))
                        state.chrStr.Append(ch); // Append for next run
                    i++;
                    break;

                case ParserPartEnum.ExpPart:
                    if (IsSign(ch))
                    {
                        state.currentPart = ParserPartEnum.ExpSign;
                        break;
                    }

                    if (IsNumeric(ch))
                    {
                        state.currentPart = ParserPartEnum.ExpNum;
                        break;
                    }

                    i++;
                    state.warnings.Add(new UnknownCharacterException($"Unknown character '{ch}' ignored in ExpPart.", ch));
                    break;

                case ParserPartEnum.ExpSign:
                    if (IsSign(ch))
                    {
                        if (IsNegativeSign(ch))
                            state.expSign *= -1;
                        i++;
                        break;
                    }

                    state.currentPart = ParserPartEnum.ExpNum;
                    break;

                case ParserPartEnum.ExpNum:
                    if (IsNumeric(ch))
                    {
                        state.decStr.Append(ch); // Append for next run
                        i++;
                        break;
                    }

                    state.exp += state.expSign * Int32.Parse(state.decStr.ToString());
                    state.expSign = 1;
                    state.decStr.Clear();
                    state.currentPart = ParserPartEnum.SuffixPart;
                    break;

                case ParserPartEnum.SuffixPart:
                    str = state.chrStr.ToString();
                    if (!state.siPrefixFound && SIPrefixes.IsSIPrefix(str, out exp))
                    {
                        // Check a more 'greedy' variety to avoid mistaking prefix words for single chars
                        FindGreedySIMatch(strValue, state, ref i, ref exp);

                        state.exp += exp;
                        state.expSign = 1;
                        state.chrStr.Clear(); // Reset the appending
                        state.currentPart = ParserPartEnum.UnitPart;
                        state.siPrefixFound = true;
                        break;
                    }

                    if (IsUnit(str, unitOptions, ref state.unitConvFactor) > 0)
                    {
                        state.chrStr.Clear(); // Reset the appending
                        state.currentPart = ParserPartEnum.EndOfStrPart;
                        break;
                    }

                    if (IsNumeric(ch))
                    {
                        state.warnings.Add(new UnexpectedSyntaxException($"Unexpected numeric '{ch}' in Suffix (unsupported syntax).", state.currentPart, str + ch));
                        state.currentPart = ParserPartEnum.EndOfStrPart;
                        break;
                    }

                    if (!IsBlank(ch))
                        state.chrStr.Append(ch); // Append for next run
                    i++;
                    break;

                case ParserPartEnum.UnitPart:
                    str = state.chrStr.ToString();
                    if (IsUnit(str, unitOptions, ref state.unitConvFactor) > 0)
                    {
                        state.chrStr.Clear(); // Reset the appending
                        state.currentPart = ParserPartEnum.EndOfStrPart;
                        break;
                    }

                    if (IsNumeric(ch))
                    {
                        state.warnings.Add(new UnexpectedSyntaxException($"Unexpected numeric '{ch}' in UnitPart (unsupported syntax).", state.currentPart, str + ch));
                        state.currentPart = ParserPartEnum.EndOfStrPart;
                    }

                    if (!IsBlank(ch))
                        state.chrStr.Append(ch); // Append for next run
                    i++;
                    break;

                case ParserPartEnum.EndOfStrPart:
                    // Just wait for the string to finish
                    i++;
                    break;

                default:
                    throw new InvalidOperationException("Internal Error: ParsePart is in an invalid state.");
            }
        }

        if (!state.siPrefixFound && state.alternateUnitWarning != null)
            state.warnings.Add(state.alternateUnitWarning);

        return state;
    }

    #endregion

    #region ParseStream

    internal static double ParseStream(StringReaderLookahead stringReader, UnitOptions unitOptions, FormattingOptions? formattingOptions = null,
                                       List<Exception>? warnings = null)
    {
        var state = ParseStreamInternal(stringReader, unitOptions, formattingOptions ?? FormattingOptions.Default);

        double value;
        if (state.postDecVal != 0)
            value = (double) state.preDecVal + (double) state.postDecVal;
        else
            value = (double) state.preDecVal;

        warnings?.AddRange(state.warnings);

        return state.sign * value * state.unitConvFactor * Pow(10, state.expSign * state.exp);
    }

    // This is a stripped down version of ParseStringStateMachine that does not require look-ahead, at the expense of unit and SI identification completeness
    private static ParserState ParseStreamInternal(StringReaderLookahead stringReader, UnitOptions unitOptions, FormattingOptions formattingOption)
    {
        var state = new ParserState();

        var oldState = ParserPartEnum.EndOfStrPart;
        var advancedStream = true;
        var lastIteration = false;

        while (state.currentPart != ParserPartEnum.EndOfStrPart && !lastIteration)
        {
            char ch;
            if (stringReader.Peek() > -1)
            {
                ch = (char) stringReader.Peek();
                if (IsBlank(ch))
                    lastIteration = true; // Spaces are delimiters in streams
            }
            else
            {
                ch = ' ';
                lastIteration = true; // Allow the state machine to process the previous iteration one last time before coming to a halt. Space is a neutral character and will mean no harm.
            }

            if (oldState == state.currentPart && !advancedStream)
                throw new Exception("Internal Error: Neither state nor i have changed during a loop run.");

            advancedStream = false; // Assume no progress was requested
            oldState = state.currentPart;

            string str;
            bool siFound;
            bool unitFound;
            switch (state.currentPart)
            {
                case ParserPartEnum.PreDecPart:
                    if (IsSign(ch))
                    {
                        state.currentPart = ParserPartEnum.PreDecSign;
                        break;
                    }

                    if (IsNumeric(ch))
                    {
                        state.currentPart = ParserPartEnum.PreDecNum;
                        break;
                    }

                    AdvanceStream(ref stringReader, out advancedStream); // Ignore unknown character
                    state.warnings.Add(new UnknownCharacterException($"Unknown character '{ch}' ignored in PreDecPart.", ch));
                    break;

                case ParserPartEnum.PreDecSign:
                    if (IsSign(ch))
                    {
                        if (IsNegativeSign(ch)) // Only minus sign changes the sign ...
                            state.sign *= -1;
                        AdvanceStream(ref stringReader, out advancedStream);
                        break;
                    }

                    if (IsNumeric(ch))
                    {
                        state.currentPart = ParserPartEnum.PreDecNum;
                        break;
                    }

                    AdvanceStream(ref stringReader, out advancedStream);
                    state.warnings.Add(new UnknownCharacterException($"Unknown character '{ch}' ignored in PreDecSign.", ch));
                    break;

                case ParserPartEnum.PreDecNum:
                    if (IsNumeric(ch))
                    {
                        state.decStr.Append(ch); // Append the number to the decStr
                        AdvanceStream(ref stringReader, out advancedStream);
                        break;
                    }

                    state.preDecVal = Int32.Parse(state.decStr.ToString());
                    state.decStr.Clear();
                    state.currentPart = ParserPartEnum.DecSepPart;
                    break;

                case ParserPartEnum.DecSepPart:
                    str = state.chrStr.ToString();

                    if (SIPrefixes.IsExpPrefix(str) &&
                        (IsNumeric((char) stringReader.Peek(1)) ||
                         IsSign((char) stringReader.Peek(1)) && IsNumeric((char) stringReader.Peek(2))))
                    {
                        // Valid format of exponential notation found
                        state.chrStr.Clear(); // Reset the appending
                        state.currentPart = ParserPartEnum.ExpPart;
                        break;
                    }

                    if (IsDecSep(str, formattingOption))
                    {
                        state.chrStr.Clear(); // Reset the appending
                        state.currentPart = ParserPartEnum.PostDecPart;
                        break;
                    }

                    // Handle SI and unit strings
                    if (ParseStreamHandleUnit(stringReader, unitOptions, formattingOption, state, out _, out _))
                    {
                        state.currentPart = ParserPartEnum.PostDecPart;
                        break;
                    }

                    if (IsNumeric(ch)) // This has to happen after the string is checked, since str is one character behind ch
                    {
                        // Should not happen on first iteration. Should also not happen on later iterations, since DecSep or ParseStreamHandleUnit should have advanced the state before the postDec part begins.
                        state.warnings.Add(new UnexpectedSyntaxException($"Discarding '{state.chrStr}' as it could not be identified as either SI symbol or unit (or a combination of both), followed by a number.", state.currentPart, state.chrStr.ToString()));
                        state.chrStr.Clear(); // Discard unidentifiable decSep 
                        state.currentPart = ParserPartEnum.PostDecPart;
                        break;
                    }

                    if (IsOperator(ch))
                    {
                        state.currentPart = ParserPartEnum.EndOfStrPart; // Abort reading, since we don't handle math
                        break;
                    }

                    if (IsInvalidChar(ch))
                    {
                        state.currentPart = ParserPartEnum.EndOfStrPart; // Abort reading
                        break;
                    }

                    if (!IsBlank(ch))
                        state.chrStr.Append(ch); // Append for next run
                    AdvanceStream(ref stringReader, out advancedStream);
                    break;

                case ParserPartEnum.PostDecPart:
                    str = state.chrStr.ToString();
                    if (SIPrefixes.IsExpPrefix(str) &&
                        (IsNumeric((char) stringReader.Peek(1)) ||
                         IsSign((char) stringReader.Peek(1)) && IsNumeric((char) stringReader.Peek(2))))
                    {
                        // Valid format of exponential notation found
                        state.chrStr.Clear(); // Reset the appending
                        state.currentPart = ParserPartEnum.ExpPart;
                        break;
                    }

                    if (ParseStreamHandleUnit(stringReader, unitOptions, formattingOption, state, out _, out _))
                    {
                        state.currentPart = ParserPartEnum.SuffixPart;
                        break;
                    }

                    if (IsNumeric(ch))
                    {
                        state.currentPart = ParserPartEnum.PostDecNum;
                        break;
                    }

                    if (IsOperator(ch))
                    {
                        state.currentPart = ParserPartEnum.EndOfStrPart; // Abort reading, since we don't handle math
                        break;
                    }

                    if (IsInvalidChar(ch))
                    {
                        state.currentPart = ParserPartEnum.EndOfStrPart; // Abort reading
                        break;
                    }

                    if (!IsBlank(ch))
                        state.chrStr.Append(ch); // Append for next run
                    AdvanceStream(ref stringReader, out advancedStream);
                    break;

                case ParserPartEnum.PostDecNum:
                    if (IsNumeric(ch))
                    {
                        state.decStr.Append(ch); // Append for next run
                        AdvanceStream(ref stringReader, out advancedStream);
                        break;
                    }

                    state.postDecVal = (decimal) Pow(10, -1 * state.decStr.Length) * Decimal.Parse(state.decStr.ToString());
                    state.decStr.Clear();
                    state.currentPart = ParserPartEnum.PostPostDecPart;
                    break;

                case ParserPartEnum.PostPostDecPart:
                    str = state.chrStr.ToString();
                    if (SIPrefixes.IsExpPrefix(str) &&
                        (IsNumeric((char) stringReader.Peek(1)) ||
                         IsSign((char) stringReader.Peek(1)) && IsNumeric((char) stringReader.Peek(2))))
                    {
                        // Valid format of exponential notation found
                        state.chrStr.Clear(); // Reset the appending
                        state.currentPart = ParserPartEnum.ExpPart;
                        break;
                    }

                    if (ParseStreamHandleUnit(stringReader, unitOptions, formattingOption, state, out siFound, out unitFound))
                    {
                        if (siFound)
                            state.currentPart = ParserPartEnum.SuffixPart; // If only SI is found, goto suffix
                        if (unitFound)
                            state.currentPart = ParserPartEnum.EndOfStrPart; // If unit symbol is found, go to eostr
                        break;
                    }

                    if (IsNumeric(ch))
                    {
                        state.warnings.Add(new UnexpectedSyntaxException($"Unexpected numeric '{ch}' in PostPostDecPart (unsupported syntax).", state.currentPart, str + ch));
                        state.currentPart = ParserPartEnum.EndOfStrPart;
                        break;
                    }

                    if (IsOperator(ch))
                    {
                        state.currentPart = ParserPartEnum.EndOfStrPart; // Abort reading, since we don't handle math
                        break;
                    }

                    if (IsInvalidChar(ch))
                    {
                        state.currentPart = ParserPartEnum.EndOfStrPart; // Abort reading
                        break;
                    }

                    if (!IsBlank(ch))
                        state.chrStr.Append(ch); // Append for next run
                    AdvanceStream(ref stringReader, out advancedStream);
                    break;

                case ParserPartEnum.ExpPart:
                    if (IsSign(ch))
                    {
                        state.currentPart = ParserPartEnum.ExpSign;
                        break;
                    }

                    if (IsNumeric(ch))
                    {
                        state.currentPart = ParserPartEnum.ExpNum;
                        break;
                    }

                    AdvanceStream(ref stringReader, out advancedStream);
                    state.warnings.Add(new UnknownCharacterException($"Unknown character '{ch}' ignored in ExpPart.", ch));
                    break;

                case ParserPartEnum.ExpSign:
                    if (IsSign(ch))
                    {
                        if (IsNegativeSign(ch))
                            state.expSign *= -1;
                        AdvanceStream(ref stringReader, out advancedStream);
                        break;
                    }

                    state.currentPart = ParserPartEnum.ExpNum;
                    break;

                case ParserPartEnum.ExpNum:
                    if (IsNumeric(ch))
                    {
                        state.decStr.Append(ch); // Append for next run
                        AdvanceStream(ref stringReader, out advancedStream);
                        break;
                    }

                    state.exp += state.expSign * Int32.Parse(state.decStr.ToString());
                    state.expSign = 1;
                    state.decStr.Clear();
                    state.currentPart = ParserPartEnum.SuffixPart;
                    break;

                case ParserPartEnum.SuffixPart:
                    str = state.chrStr.ToString();
                    if (ParseStreamHandleUnit(stringReader, unitOptions, formattingOption, state, out siFound, out unitFound))
                    {
                        if (siFound)
                            state.currentPart = ParserPartEnum.UnitPart; // If only SI is found, goto suffix
                        if (unitFound)
                            state.currentPart = ParserPartEnum.EndOfStrPart; // If unit symbol is found, go to eostr
                        break;
                    }

                    if (IsNumeric(ch))
                    {
                        state.warnings.Add(new UnexpectedSyntaxException($"Unexpected numeric '{ch}' in Suffix (unsupported syntax).", state.currentPart, str + ch));
                        state.currentPart = ParserPartEnum.EndOfStrPart;
                        break;
                    }

                    if (IsOperator(ch))
                    {
                        state.currentPart = ParserPartEnum.EndOfStrPart; // Abort reading, since we don't handle math
                        break;
                    }

                    if (IsInvalidChar(ch))
                    {
                        state.currentPart = ParserPartEnum.EndOfStrPart; // Abort reading
                        break;
                    }

                    if (!IsBlank(ch))
                        state.chrStr.Append(ch); // Append for next run
                    AdvanceStream(ref stringReader, out advancedStream);
                    break;

                case ParserPartEnum.UnitPart:
                    str = state.chrStr.ToString();
                    if (IsUnit(str, unitOptions, ref state.unitConvFactor) > 0)
                    {
                        state.chrStr.Clear(); // Reset the appending
                        state.currentPart = ParserPartEnum.EndOfStrPart;
                        break;
                    }

                    if (IsNumeric(ch))
                    {
                        state.warnings.Add(new UnexpectedSyntaxException($"Unexpected numeric '{ch}' in UnitPart (unsupported syntax).", state.currentPart, str + ch));
                        state.currentPart = ParserPartEnum.EndOfStrPart;
                        break;
                    }

                    if (IsOperator(ch))
                    {
                        state.currentPart = ParserPartEnum.EndOfStrPart; // Abort reading, since we don't handle math
                        break;
                    }

                    if (IsInvalidChar(ch))
                    {
                        state.currentPart = ParserPartEnum.EndOfStrPart; // Abort reading
                        break;
                    }

                    if (!IsBlank(ch))
                        state.chrStr.Append(ch); // Append for next run
                    AdvanceStream(ref stringReader, out advancedStream);
                    break;

                case ParserPartEnum.EndOfStrPart:
                    // Just wait for the string to finish.
                    AdvanceStream(ref stringReader, out advancedStream);
                    break;

                default:
                    throw new InvalidOperationException($"Internal Error: ParsePart is in an invalid state at '{state.currentPart}'.");
            }
        }

        if (!state.siPrefixFound && state.alternateUnitWarning != null)
            state.warnings.Add(state.alternateUnitWarning);

        return state;
    }

    private static int AdvanceStream(ref StringReaderLookahead streamReader, out bool advancedStream)
    {
        if (streamReader.Peek() <= -1)
        {
            advancedStream = false;
            return -1;
        }

        advancedStream = true;

        return streamReader.Read();
    }

    private static bool ParseStreamHandleUnit(StringReaderLookahead stringReader, UnitOptions unitOptions, FormattingOptions formattingOptions,
                                              ParserState state, out bool siFound, out bool unitFound)
    {
        // Use look-ahead to the next whitespace, number, operand or invalid character
        siFound = false;
        unitFound = false;

        var lookAheadStr = new StringBuilder();

        var iLook = 0;
        var iStr = 0;
        var ciLook = stringReader.Peek(iLook);
        var cLook = (char) ciLook;

        while (ciLook > 0 && !IsInvalidChar(cLook) && !IsOperator(cLook) && !IsBlank(cLook) && !IsNumeric(cLook)
               && (IsValidUnitChar(cLook, unitOptions) || SIPrefixes.IsValidSIChar(cLook)))
        {
            lookAheadStr.Append(cLook);
            iLook++;
            ciLook = stringReader.Peek(iLook);
            cLook = (char) ciLook;
        }

        // Now we have a string that contains all valid parts from unit symbol or SI prefix
        var strValue = lookAheadStr.ToString();

        // check for unit
        var unitCnt = DiscoverUnitAtEndOfString(ref strValue, unitOptions, formattingOptions, state);
        if (unitCnt > 0)
            unitFound = true;

        if (SIPrefixes.IsSIPrefix(strValue, out var exp))
        {
            // Check a more 'greedy' variety to avoid mistaking prefix words for single chars
            FindGreedySIMatch(strValue, state, ref iStr, ref exp);

            state.exp += exp;
            state.chrStr.Clear(); // Reset the appending

            state.siPrefixFound = true;
            siFound = true;
        }

        if (siFound || unitFound)
        {
            // Advance string reader by the peeked amount
            stringReader.ReadStr(iLook);

            return true;
        }

        return false;
    }

    #endregion

    #region SIUnitMatch

    private static int DiscoverUnitAtEndOfString(ref string strValue, UnitOptions unitOptions, FormattingOptions formattingOptions, ParserState state)
    {
        if (strValue.Length == 0)
            return 0;

        state.alternateUnitWarning = null;

        var unitCnt = 0;

        var whiteCnt = 0;
        while (IsBlank(strValue[strValue.Length - whiteCnt - 1]))
            whiteCnt++;

        foreach (var unit in unitOptions.GetSortedUnits()) // For all units symbols (from the longest to the shortest)
        {
            if (!String.IsNullOrEmpty(unit.Symbol) &&
                SIPrefixes.ContainsSIPrefix(unit.Symbol, out var ambPrefix)) // Check whether this unit symbol can be mistaken as an SI prefix
            {
                // Special case: Remove first occurence of unit symbol starting from the end of the string
                var k = strValue.Length - unit.Symbol.Length - whiteCnt;
                while (k >= 0)
                {
                    var unitCandidate = SubstringTolerant(strValue, k, unit.Symbol.Length);
                    if (unitCandidate == unit.Symbol)
                    {
                        // Unit found
                        IsUnit(unitCandidate, unitOptions, ref state.unitConvFactor);

                        unitCnt++;

                        strValue = SubstringTolerant(strValue, 0, k) + SubstringTolerant(strValue, k + unit.Symbol.Length, strValue.Length);
                        state.alternateUnitWarning = new AmbiguousUnitException($"(Alternate) Unit \'{unit.Symbol}\' is ambiguous with a SI prefix \'{ambPrefix}\' and has been ignored. \nPlease enter unit and prefix if this prefix is desired.", unit, ambPrefix);

                        break;
                    }

                    if (!formattingOptions.UnitMustBeAtEnd)
                        k--;
                    else
                        break;
                }
            }
            else
            {
                // Unit does not contain an SI prefix and is safe to remove (when taking unitConvFactor into account)
                if (StrEndsWithPattern(strValue, unit.Symbol))
                {
                    var unitLength = unit.Symbol.Length;
                    state.unitConvFactor = unit.BaseConversionFactor;
                    SubstringTolerant(strValue, strValue.Length - unitLength, unitLength);
                    strValue = SubstringTolerant(strValue, 0, strValue.Length - unitLength);

                    unitCnt++;

                    break;
                }
            }
        }

        return unitCnt;
    }

    private static void FindGreedySIMatch(string strValue, ParserState state, ref int i, ref int exp)
    {
        var k = i;
        var expAlt = 0;
        var lenAlt = 0;

        while (k < strValue.Length && !IsNumeric(strValue[k]))
        {
            state.chrStr.Append(strValue[k]);
            var str = state.chrStr.ToString();

            if (SIPrefixes.IsSIPrefix(str, out var expAltTmp))
            {
                lenAlt = k;
                expAlt = expAltTmp;
            }
            k++;
        }

        if (expAlt != 0)
        {
            exp = expAlt;
            i = lenAlt + 1; // Continue after the greedy match
        }
    }

    #endregion

    #region Nested type: ParserState

    private class ParserState
    {
        public readonly List<Exception> warnings = new List<Exception>();
        public Exception? alternateUnitWarning;

        public readonly StringBuilder chrStr = new StringBuilder();
        public readonly StringBuilder decStr = new StringBuilder();

        public ParserPartEnum currentPart = ParserPartEnum.PreDecPart;

        public decimal preDecVal;
        public decimal postDecVal;

        public int sign = 1;

        public int exp;
        public int expSign = 1;

        public bool siPrefixFound;
        public double unitConvFactor = 1.0;
    }

    #endregion
}