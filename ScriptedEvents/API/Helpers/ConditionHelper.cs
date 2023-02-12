namespace ScriptedEvents.API.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Exiled.API.Features;
    using ScriptedEvents.Conditions.Floats;
    using ScriptedEvents.Conditions.Interfaces;
    using ScriptedEvents.Conditions.Strings;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

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

        public static ReadOnlyCollection<IFloatCondition> FloatConditions { get; } = new List<IFloatCondition>()
        {
            new GreaterThan(),
            new LessThan(),
            new Equal(),
            new NotEqual(),

            new LessThanOrEqualTo(),
            new GreaterThanOrEqualTo(),
        }.AsReadOnly();

        public static ReadOnlyCollection<IStringCondition> StringConditions { get; } = new List<IStringCondition>()
        {
            new StringEqual(),
            new StringNotEqual(),
        }.AsReadOnly();

        // StackOverflow my beloved
        public static float Math(string expression)
        {
            DataTable loDataTable = new DataTable();
            DataColumn loDataColumn = new DataColumn("Eval", typeof(double), expression);
            loDataTable.Columns.Add(loDataColumn);
            loDataTable.Rows.Add(0);
            return (float)(double)loDataTable.Rows[0]["Eval"];
        }

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

        // StackOverflow my beloved
        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }

        public static ConditionResponse Evaluate(string input)
        {
            input = ConditionVariables.ReplaceVariables(input);
            string newWholeString = input;

            MatchCollection matches = Regex.Matches(input, @"\(([^)]*)\)");
            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    ConditionResponse conditionResult = EvaluateAndOr(match.Groups[1].Value);
                    newWholeString = newWholeString.Replace($"({match.Groups[1].Value})", conditionResult.ObjectResult ?? conditionResult.Passed);
                }
            }

            return EvaluateAndOr(newWholeString, true);
        }

        private static ConditionResponse EvaluateAndOr(string input, bool last = false)
        {
            if (!last && TryMath(ConditionVariables.ReplaceVariables(input), out MathResult result))
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
                    ConditionResponse conditionResult = EvaluateInternal(fragOr.RemoveWhitespace());
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

        private static ConditionResponse EvaluateInternal(string input)
        {
            input = ConditionVariables.ReplaceVariables(input.RemoveWhitespace()).Trim(); // Kill all whitespace & replace variables

            // Code for simple checks
            if (input.ToLowerInvariant() is "true" or "1")
                return new(true, true, string.Empty);

            if (input.ToLowerInvariant() is "false" or "0")
                return new(true, false, string.Empty);

            bool doStringCondition = true;

            // Code for conditions with string operator
            IStringCondition conditionString = null;
            foreach (IStringCondition con in StringConditions)
            {
                if (input.Contains(con.Symbol))
                {
                    conditionString = con;
                }
            }

            if (conditionString is not null)
            {
                string[] arrString = input.Split(new[] { conditionString.Symbol }, StringSplitOptions.RemoveEmptyEntries);

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

                    if (splitString.Count != 2)
                        return new(false, false, $"Malformed condition provided! Condition: '{input}'");

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
                return new(false, false, $"Invalid condition operator provided! Condition: '{input}'");

            string[] arr = input.Split(new[] { condition.Symbol }, StringSplitOptions.RemoveEmptyEntries);
            List<string> split = arr.ToList();
            split.RemoveAll(y => string.IsNullOrWhiteSpace(y));

            if (split.Count != 2)
                return new(false, false, $"Malformed condition provided! Condition: '{input}'");

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
