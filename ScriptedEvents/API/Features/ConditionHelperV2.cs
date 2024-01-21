namespace ScriptedEvents.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Text.RegularExpressions;

    using Exiled.API.Features;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Conditions.Floats;
    using ScriptedEvents.Conditions.Interfaces;
    using ScriptedEvents.Conditions.Strings;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

#pragma warning disable SA1600 // Remove this later
    public static class ConditionHelperV2
    {
        // Constants
        public const string AND = " AND ";
        public const string OR = " OR ";

        public static readonly char[] IgnoreChars = new[]
        {
            '>',
            '<',
            '=',
            '!',
        };

        // Conditions

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

        // Methods

        /// <summary>
        /// Performs a math equation and returns the result.
        /// </summary>
        /// <param name="expression">The string math equation.</param>
        /// <returns>The result of the math equation.</returns>
        public static float Math(string expression)
        {
            // StackOverflow my beloved
            DataTable loDataTable = new();
            DataColumn loDataColumn = new("Eval", typeof(double), expression);
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

        public static List<string> CaptureGroups(string input)
        {
            MatchCollection matches = Regex.Matches(input, @"\(([^)]*)\)");
            if (matches.Count == 0)
                return new(1) { input };

            List<string> ret = new();

            foreach (Match m in matches)
            {
                ret.Add(m.Groups[1].Value);
                input = input.Replace($"({m.Groups[1].Value})", string.Empty);
            }

            ret.Add(input);
            ret.RemoveAll(r => string.IsNullOrWhiteSpace(r));
            return ret;
        }

        public static ConditionResponse Evaluate(string input, Script source = null)
        {
            source?.DebugLog($"Evaluating condition: {input}");
            return EvaluateInternal(input, source);
        }

        private static ConditionResponse EvaluateAndOr(string input)
        {
            string[] andSplit = input.Split(new[] { AND }, StringSplitOptions.RemoveEmptyEntries);
            bool stillGo = true;
            foreach (string fragAnd in andSplit)
            {
                string[] orSplit = fragAnd.Split(new[] { OR }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string fragOr in orSplit)
                {
                    if (bool.TryParse(fragOr, out bool r))
                    {
                        if (r is true)
                        {
                            stillGo = true;
                            break;
                        }
                        else
                        {
                            stillGo = false;
                        }
                    }
                }

                if (!stillGo)
                    break;
            }

            return new(true, stillGo, string.Empty);
        }

        private static ConditionResponse EvaluateSingleCondition(string input, string raw)
        {
            // Goofball checks first
            if (bool.TryParse(input, out bool r))
                return new(true, r, string.Empty);

            // Attempt to run math first
            IFloatCondition match = null;
            foreach (var floatCondition in FloatConditions)
            {
                int index = input.IndexOf(floatCondition.Symbol);

                Log.Debug($"CND: {floatCondition.GetType().FullName}");
                if (index != -1)
                {
                    Log.Debug($"INDEX: " + index);
                    Log.Debug("SYM BEF: " + input[index - 1]);
                    Log.Debug("SYM AFT: " + input[index + floatCondition.Symbol.Length]);
                    if (!IgnoreChars.Contains(input[index - 1]) && !IgnoreChars.Contains(input[index + floatCondition.Symbol.Length]))
                    {
                        Log.Debug("MATCH: TRUE");
                        match = floatCondition;
                        break;
                    }
                }

                Log.Debug("MATCH: FALSE");
            }

            if (match is not null)
            {
                string[] split = input.Split(new[] { match.Symbol }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 2)
                {
                    return new(false, false, $"Malformed condition provided! Condition: {raw}");
                }

                if (TryMath(split[0], out MathResult res1) && TryMath(split[1], out MathResult res2))
                {
                    return new(true, match.Execute(res1.Result, res2.Result), string.Empty);
                }
            }

            // Math failed - compare strings directly
            IStringCondition match2 = null;
            foreach (var stringCondition in StringConditions)
            {
                int index = input.IndexOf(stringCondition.Symbol);

                Log.Debug($"CND: {stringCondition.GetType().FullName}");
                if (index != -1)
                {
                    Log.Debug($"INDEX: " + index);
                    Log.Debug("SYM BEF: " + input[index - 1]);
                    Log.Debug("SYM AFT: " + input[index + stringCondition.Symbol.Length]);
                    if (!IgnoreChars.Contains(input[index - 1]) && !IgnoreChars.Contains(input[index + stringCondition.Symbol.Length]))
                    {
                        Log.Debug("MATCH: TRUE");
                        match2 = stringCondition;
                        break;
                    }
                }

                Log.Debug("MATCH: FALSE");
            }

            if (match2 is not null)
            {
                string[] split = input.Split(new[] { " " + match2.Symbol + " " }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length < 2)
                {
                    return new(false, false, $"Malformed condition provided! Condition: {raw}");
                }

                return new(true, match2.Execute(split[0], split[1]), string.Empty);
            }

            return new(false, false, $"Invalid condition operator provided! Condition: {raw}");
        }

        private static ConditionResponse EvaluateInternal(string input, Script source = null)
        {
            string convertedInput = VariableSystem.ReplaceVariables(input, source);
            if (bool.TryParse(convertedInput, out bool boolResult))
                return new ConditionResponse(true, boolResult, string.Empty);

            List<string> groups = CaptureGroups(input);
            foreach (string group in groups)
            {
                source?.DebugLog($"GROUP: " + group);
                string[] andSplit = group.Split(new[] { AND }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string fragAnd in andSplit)
                {
                    source?.DebugLog($"FRAG [AND]: " + fragAnd);
                    string[] orSplit = fragAnd.Split(new[] { OR }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string fragOr in orSplit)
                    {
                        source?.DebugLog($"FRAG [OR]: " + fragOr);
                        string convertedFrag = VariableSystem.ReplaceVariables(fragOr, source);
                        ConditionResponse eval = EvaluateSingleCondition(convertedFrag, group);
                        if (!eval.Success)
                        {
                            return new(false, eval.Passed, eval.Message);
                        }

                        input = input.Replace($"{fragOr}", eval.Passed.ToString().ToUpper());
                    }
                }
            }

            source?.DebugLog($"CONVERTED INPUT: " + input);

            if (bool.TryParse(input, out bool r))
                return new(true, r, string.Empty);

            MatchCollection matches = Regex.Matches(input, @"\(([^)]*)\)");
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    ConditionResponse conditionResult = EvaluateAndOr(match.Groups[1].Value);
                    input = input.Replace($"({match.Groups[1].Value})", conditionResult.Passed.ToString().ToUpper());
                }
            }

            return EvaluateAndOr(input);
        }
    }
}
