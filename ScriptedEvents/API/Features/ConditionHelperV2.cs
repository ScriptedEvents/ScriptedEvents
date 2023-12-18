using ScriptedEvents.Structures;
using System.Data;
using System;
using ScriptedEvents.Variables;
using ScriptedEvents.Conditions.Floats;
using ScriptedEvents.Conditions.Interfaces;
using ScriptedEvents.Conditions.Strings;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Exiled.API.Features.Pools;
using System.Linq;
using Exiled.API.Features;

namespace ScriptedEvents.API.Features
{
#pragma warning disable SA1600 // Remove this later
    public static class ConditionHelperV2
    {
        // Constants
        public const string AND = " AND ";
        public const string OR = " OR ";

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

        public static List<string> CaptureGroups(string input)
        {
            List<string> ret = new();
            List<string> wip = ListPool<string>.Pool.Get();
            wip.Add(string.Empty);
            foreach (char c in input)
            {
                if (c == '(')
                {
                    wip.Add(string.Empty);
                }
                else if (c == ')')
                {
                    ret.Add(wip[wip.Count - 1]);
                    wip.RemoveAt(wip.Count - 1);
                }
                else
                {
                    wip[wip.Count - 1] += c;
                }
            }

            if (wip.Count > 0)
            {
                ret.AddRange(wip);
            }

            ListPool<string>.Pool.Return(wip);
            ret.RemoveAll(str => string.IsNullOrWhiteSpace(str));
            return ret;
        }

        private static ConditionResponse EvaluateSingleCondition(string input, string raw)
        {
            // Attempt to run math first
            IFloatCondition match = null;
            foreach (var floatCondition in FloatConditions)
            {
                int index = input.IndexOf(floatCondition.Symbol);
                if (index != -1 && char.IsWhiteSpace(input[index - 1]) && char.IsWhiteSpace(input[index + 1]))
                {
                    match = floatCondition;
                    break;
                }
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
                if (index != -1 && char.IsWhiteSpace(input[index - 1]) && char.IsWhiteSpace(input[index + 1]))
                {
                    match2 = stringCondition;
                    break;
                }
            }

            if (match2 is not null)
            {
                string[] split = input.Split(new[] { match.Symbol }, StringSplitOptions.RemoveEmptyEntries);
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
                string convertedInput2 = VariableSystem.ReplaceVariables(group, source);
                ConditionResponse eval = EvaluateSingleCondition(convertedInput2, group);
                if (!eval.Success)
                {
                    return new(false, eval.Passed, eval.Message);
                }

                if (!eval.Passed) // Todo: Replace this with proper AND/OR support
                    return new(true, false, eval.Message);
            }

            return new(true, true, string.Empty);
        }

        public static ConditionResponse Evaluate(string input, Script source = null)
        {
            return EvaluateInternal(input, source);
        }
    }
}
