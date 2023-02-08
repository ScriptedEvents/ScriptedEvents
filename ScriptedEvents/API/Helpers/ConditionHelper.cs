using Exiled.API.Features;
using ScriptedEvents.Conditions.Floats;
using ScriptedEvents.Conditions.Interfaces;
using ScriptedEvents.Conditions.Strings;
using ScriptedEvents.Handlers.Variables;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace ScriptedEvents.API.Helpers
{
    public static class ConditionHelper
    {

        public const string AND = "AND";
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
        public static double Math(string expression)
        {
            var loDataTable = new DataTable();
            var loDataColumn = new DataColumn("Eval", typeof(double), expression);
            loDataTable.Columns.Add(loDataColumn);
            loDataTable.Rows.Add(0);
            return (double)loDataTable.Rows[0]["Eval"];
        }

        // StackOverflow my beloved
        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !char.IsWhiteSpace(c))
                .ToArray());
        }

        private static ConditionResponse EvaluateInternal(string input)
        {
            input = ConditionVariables.ReplaceVariables(input.RemoveWhitespace()).Trim(); // Kill all whitespace & replace variables

            // Code for simple checks
            if (input.ToLowerInvariant() is "true")
                return new(true, true, string.Empty);

            if (input.ToLowerInvariant() is "false")
                return new(true, false, string.Empty);

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
                var splitString = arrString.ToList();
                splitString.RemoveAll(y => string.IsNullOrWhiteSpace(y));

                if (splitString.Count != 2)
                    return new(false, false, $"Malformed condition provided! Condition: '{input}'");

                return new(true, conditionString.Execute(splitString[0], splitString[1]), string.Empty);
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
            var split = arr.ToList();
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

        private static ConditionResponse EvaluateAndOr(string input, bool last = false)
        {
            try
            {
                if (!last)
                {
                    float output = (float)Math(ConditionVariables.ReplaceVariables(input));
                    return new(true, true, string.Empty, output);
                }
            }
            catch { }

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

        public static ConditionResponse Evaluate(string input)
        {
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
    }

    public class ConditionResponse
    {
        public bool Success { get; set; }
        public bool Passed { get; set; }
        public string Message { get; set; }
        public object ObjectResult { get; set; }

        public ConditionResponse(bool success, bool passed, string message, object objectResult = null)
        {
            Success = success;
            Passed = passed;
            Message = message;
            ObjectResult = objectResult;
        }

        public override string ToString()
        {
            return $"SUCCESS: {Success} | PASSED: {Passed} | MESSAGE: {(Message ?? "N/A")}";
        }
    }
}
