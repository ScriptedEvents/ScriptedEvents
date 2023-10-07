namespace ScriptedEvents.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Exiled.API.Features.Pools;
    using ScriptedEvents.Conditions.Floats;
    using ScriptedEvents.Conditions.Interfaces;
    using ScriptedEvents.Conditions.Strings;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    /// <summary>
    /// A set of methods and API for handling conditions and math equations.
    /// </summary>
    public static class ConditionHelper
    {
        /// <summary>
        /// The separator used for AND clauses.
        /// </summary>
        public const string AND = "AND";

        /// <summary>
        /// The separator used for OR clauses.
        /// </summary>
        public const string OR = "OR";

        /// <summary>
        /// Gets a <see cref="ReadOnlyCollection{T}"/> of float operators.
        /// </summary>
        public static ReadOnlyCollection<IFloatCondition> FloatConditions { get; } = new List<IFloatCondition>()
        {
            new GreaterThan(),
            new LessThan(),
            new Equal(),
            new NotEqual(),

            new LessThanOrEqualTo(),
            new GreaterThanOrEqualTo(),
        }.AsReadOnly();

        /// <summary>
        /// Gets a <see cref="ReadOnlyCollection{T}"/> of string operators.
        /// </summary>
        public static ReadOnlyCollection<IStringCondition> StringConditions { get; } = new List<IStringCondition>()
        {
            new StringEqual(),
            new StringNotEqual(),
            new StringContains(),
            new StringNotContains(),
        }.AsReadOnly();

        /// <summary>
        /// Performs a math equation and returns the result.
        /// </summary>
        /// <param name="expression">The string math equation.</param>
        /// <returns>The result of the math equation.</returns>
        public static float Math(string expression)
        {
            // StackOverflow my beloved
            DataTable loDataTable = new DataTable();
            DataColumn loDataColumn = new DataColumn("Eval", typeof(double), expression);
            loDataTable.Columns.Add(loDataColumn);
            loDataTable.Rows.Add(0);
            return (float)(double)loDataTable.Rows[0]["Eval"];
        }

        /// <summary>
        /// Tries to perform a math equation.
        /// </summary>
        /// <param name="expression">The string math equation.</param>
        /// <param name="result">A <see cref="MathResult"/> indicating the success, result, and the exception if any.</param>
        /// <returns>Whether or not the math was successful.</returns>
        public static bool TryMath(string expression, out MathResult result)
        {
            try
            {
                float floatResult = Math(expression);

                result = new() { Success = true, Result = floatResult };
            }
            catch (Exception ex)
            {
                result = new() { Success = false, Result = -1, Exception = ex };
            }

            return result.Success;
        }

        /// <summary>
        /// Removes every whitespace character from a string, UNLESS the space is inside of {}.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The new string, without any whitespace characters.</returns>
        public static string RemoveWhitespace(this string input)
        {
            // StackOverflow my beloved
            string newString = string.Empty;
            char[] chars = input.ToCharArray();
            bool isCurrentlyInVariable = false;
            foreach (char c in chars)
            {
                if (c == '{')
                    isCurrentlyInVariable = true;
                else if (c == '}')
                    isCurrentlyInVariable = false;

                if (isCurrentlyInVariable)
                    newString += c;
                else if (!char.IsWhiteSpace(c))
                    newString += c;
            }

            return newString;
        }

        /// <summary>
        /// Isolates all variables from a string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="source">The script source.</param>
        /// <returns>The variables used within the string.</returns>
        public static string[] IsolateVariables(string input, Script source = null)
        {
            source?.DebugLog($"Isolating variables from: {input}");
            List<string> result = ListPool<string>.Pool.Get();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c is '{')
                {
                    int index = input.IndexOf('}', i);
                    source?.DebugLog($"Detected variable opening symbol, char {i}. Closing index {index}. Substring {index - i + 1}.");
                    string variable = input.Substring(i, index - i + 1);
                    source?.DebugLog($"Variable: {variable}");
                    result.Add(variable);
                }
            }

            return ListPool<string>.Pool.ToArrayReturn(result);
        }

        /// <summary>
        /// Evaluates a condition.
        /// </summary>
        /// <param name="input">The condition input.</param>
        /// <param name="source">The source script.</param>
        /// <returns>The result of the condition.</returns>
        public static ConditionResponse Evaluate(string input, Script source = null)
        {
            string newWholeString = input;

            MatchCollection matches = Regex.Matches(input, @"\(([^)]*)\)");
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    ConditionResponse conditionResult = EvaluateAndOr(match.Groups[1].Value, source: source);
                    newWholeString = newWholeString.Replace($"({match.Groups[1].Value})", conditionResult.ObjectResult ?? conditionResult.Passed);
                }
            }

            return EvaluateAndOr(newWholeString, true, source: source);
        }

        private static ConditionResponse EvaluateAndOr(string input, bool last = false, Script source = null)
        {
            if (!last && TryMath(VariableSystem.ReplaceVariables(input, source), out MathResult result))
            {
                float output = (float)result.Result;
                return new(true, true, string.Empty, output);
            }

            string[] andSplit = input.Split(new[] { AND }, StringSplitOptions.RemoveEmptyEntries);
            bool stillGo = true;
            foreach (string fragAnd in andSplit)
            {
                string[] orSplit = fragAnd.Split(new[] { OR }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string fragOr in orSplit)
                {
                    ConditionResponse conditionResult = EvaluateInternal(fragOr.RemoveWhitespace(), source);
                    if (!conditionResult.Success)
                    {
                        return conditionResult; // Throw the problem to the end-user
                    }

                    if (conditionResult.Passed is true)
                    {
                        stillGo = true;
                        break;
                    }
                    else
                    {
                        stillGo = false;
                    }
                }

                if (!stillGo)
                    break;
            }

            return new(true, stillGo, string.Empty);
        }

        private static ConditionResponse EvaluateInternal(string input, Script source = null)
        {
            input = input.RemoveWhitespace().Trim(); // Kill all whitespace
            string converted = VariableSystem.ReplaceVariables(input, source).ToLower();

            // Code for simple checks
            if (converted is "true" or "1")
                return new(true, true, string.Empty);

            if (converted is "false" or "0")
                return new(true, false, string.Empty);

            bool doStringCondition = true;

            // Code for conditions with string operator
            IStringCondition conditionString = null;
            int conditionStringIndex = -1;
            foreach (IStringCondition con in StringConditions)
            {
                if (input.Contains(con.Symbol))
                {
                    conditionString = con;
                    conditionStringIndex = input.IndexOf(con.Symbol);
                }
            }

            if (conditionString is not null)
            {
                // Hacky: Check the character BEFORE the symbol
                // Fix edge cases with > and < being right before an equal sign (stupid)
                if (conditionStringIndex < 1)
                {
                    doStringCondition = false;
                }
                else
                {
                    char charBefore = input[conditionStringIndex - 1];
                    if (charBefore is '>' or '<')
                        doStringCondition = false;
                }

                string[] arrString = converted.Split(new[] { conditionString.Symbol }, StringSplitOptions.RemoveEmptyEntries);

                // Hacky case to skip over string if both sides are computable
                // For cases with similar string/math operators (eg. '=')
                try
                {
                    if (TryMath(arrString[0], out _) && TryMath(arrString[1], out _))
                        doStringCondition = false;
                }
                catch
                {
                }

                if (doStringCondition)
                {
                    List<string> splitString = arrString.ToList();
                    splitString.RemoveAll(y => string.IsNullOrWhiteSpace(y));

                    source?.DebugLog($"String condition fragments length: {splitString.Count}");

                    if (splitString.Count != 2)
                        return new(false, false, $"Malformed string condition provided! Condition: '{input}'");

                    splitString = splitString.ToList();

                    return new(true, conditionString.Execute(splitString[0], splitString[1]), string.Empty);
                }
            }

            // Code for conditions with float operator
            IFloatCondition condition = null;
            foreach (IFloatCondition con in FloatConditions)
            {
                if (input.Contains(con.Symbol))
                {
                    condition = con;
                }
            }

            if (condition is null)
                return new(false, false, $"Invalid number condition operator provided! Condition: '{input}'");

            string[] arr = converted.Split(new[] { condition.Symbol }, StringSplitOptions.RemoveEmptyEntries);
            List<string> split = arr.ToList();
            split.RemoveAll(y => string.IsNullOrWhiteSpace(y));

            source?.DebugLog($"Float condition fragments length: {split.Count}");

            if (split.Count != 2)
                return new(false, false, $"Malformed number condition provided! Condition: '{input}'");

            double left;
            try
            {
                left = Math(split[0]);
            }
            catch (Exception ex)
            {
                return new(false, false, $"Provided expression on the lefthand side is invalid. Lefthand: '{split[0]}' Error type: '{ex.GetType().Name}' Message: '{ex.Message}'.");
            }

            double right;
            try
            {
                right = Math(split[1]);
            }
            catch (Exception ex)
            {
                return new(false, false, $"Provided expression on the righthand side is invalid. Righthand: '{split[1]}' Error type: '{ex.GetType().Name}' Message: '{ex.Message}'.");
            }

            return new(true, condition.Execute((float)left, (float)right), string.Empty);
        }
    }
}
