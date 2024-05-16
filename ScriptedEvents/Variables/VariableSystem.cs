namespace ScriptedEvents.Variables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    /// <summary>
    /// A class used to store and retrieve all variables.
    /// </summary>
    public static class VariableSystem
    {
        /// <summary>
        /// Try-gets a variable.
        /// </summary>
        /// <param name="name">The input string.</param>
        /// <param name="variable">The variable found, if successful.</param>
        /// <param name="reversed">If the value is a reversed boolean value.</param>
        /// <param name="source">The script source.</param>
        /// <param name="requireBrackets">If brackets are required to parse the variable.</param>
        /// <param name="skipProcessing">If processing is to be skipped.</param>
        /// <returns>Whether or not the try-get was successful.</returns>
        public static bool TryGetVariable(string name, out IConditionVariable variable, out bool reversed, Script source, bool requireBrackets = true, bool skipProcessing = false)
        {
            VariableResult res = GetVariable(name, source, requireBrackets, skipProcessing);

            if (!res.Success && !skipProcessing)
                throw new VariableException(res.Message);

            variable = res.Variable;
            reversed = res.Reversed;

            return variable != null;
        }

        /// <summary>
        /// Attempts to get a <see cref="IEnumerable{T}"/> of <see cref="Player"/>s based on the input variable.
        /// </summary>
        /// <param name="name">The variable.</param>
        /// <param name="players">The players found. Can be <see langword="null"/>.</param>
        /// <param name="source">The source script.</param>
        /// <param name="requireBrackets">If brackets are required to parse variables.</param>
        /// <returns>Whether or not players were found.</returns>
        /// <remarks>This should be used for variables where <paramref name="requireBrackets"/> is <see langword="false"/>. Otherwise, use <see cref="ScriptModule.TryGetPlayers(string, int?, out Structures.PlayerCollection, Script)"/>.</remarks>
        /// <seealso cref="ScriptModule.TryGetPlayers(string, int?, out Structures.PlayerCollection, Script)"/>
        public static bool TryGetPlayers(string name, out PlayerCollection players, Script source, bool requireBrackets = true)
        {
            if (TryGetVariable(name, out IConditionVariable variable, out _, source, requireBrackets))
            {
                if (variable is IPlayerVariable plrVariable)
                {
                    players = new(plrVariable.Players.ToList());
                    return true;
                }
            }

            players = null;
            return false;
        }

        /// <summary>
        /// Attempts to parse a string input into a <see cref="float"/>. Functionally similar to <see cref="float.Parse(string)"/>, but also supports SE variables.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="source">The source script.</param>
        /// <param name="requireBrackets">If brackets are required to parse variables.</param>
        /// <returns>The result of the cast, or <see cref="float.NaN"/> if the cast failed.</returns>
        public static float Parse(string input, Script source, bool requireBrackets = true)
        {
            if (float.TryParse(input, out float fl))
                return fl;

            if (TryGetVariable(input, out IConditionVariable var, out _, source, requireBrackets))
            {
                if (var is IFloatVariable floatVar)
                    return floatVar.Value;
                if (var is ILongVariable longVar)
                    return longVar.Value;
                else if (var is IStringVariable stringVar && float.TryParse(stringVar.Value, out float res))
                    return res;
            }

            return float.NaN;
        }

        /// <summary>
        /// Attempts to parse a string input into a <see cref="int"/>. Functionally similar to <see cref="float.Parse(string)"/>, but also supports SE variables.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="source">The source script.</param>
        /// <param name="requireBrackets">If brackets are required to parse variables.</param>
        /// <returns>The result of the cast, or <see cref="int.MinValue"/> if the cast failed.</returns>
        public static int ParseInt(string input, Script source, bool requireBrackets = true)
        {
            if (int.TryParse(input, out int fl))
                return fl;

            if (TryGetVariable(input, out IConditionVariable var, out _, source, requireBrackets))
            {
                try
                {
                    if (var is IFloatVariable floatVar)
                        return (int)floatVar.Value;
                    if (var is ILongVariable longVar)
                        return (int)longVar.Value;
                    else if (var is IStringVariable stringVar && int.TryParse(stringVar.Value, out int res))
                        return res;
                }
                catch (Exception e)
                {
                    Log.Warn($"[Script: {source?.ScriptName ?? "N/A"}] [L: {source?.CurrentLine.ToString() ?? "N/A"}] {(source?.Debug == true ? e : e.Message)}");
                }
            }

            return int.MinValue;
        }

        /// <summary>
        /// Attempts to parse a string input into a <see cref="long"/>. Functionally similar to <see cref="long.Parse(string)"/>, but also supports SE variables.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="source">The source script.</param>
        /// <param name="requireBrackets">If brackets are required to parse variables.</param>
        /// <returns>The result of the cast, or <see cref="long.MinValue"/> if the cast failed.</returns>
        public static long ParseLong(string input, Script source, bool requireBrackets = true)
        {
            if (long.TryParse(input, out long fl))
                return fl;

            if (TryGetVariable(input, out IConditionVariable var, out _, source, requireBrackets))
            {
                if (var is IFloatVariable floatVar)
                    return (long)floatVar.Value;
                if (var is ILongVariable longVar)
                    return longVar.Value;
                else if (var is IStringVariable stringVar && long.TryParse(stringVar.Value, out long res))
                    return res;
            }

            return int.MinValue;
        }

        /// <summary>
        /// Replaces a string variable, if one is present. Otherwise, returns the same string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="source">Source script.</param>
        /// <param name="requireBrackets">Require brackets to replace?.</param>
        /// <returns>A string.</returns>
        /// <remarks>This is intended for a string that is either a variable entirely or a string entirely - this will not isolate and replace all variables in a string. See <see cref="ReplaceVariables(string, Script)"/>.</remarks>
        public static string ReplaceVariable(string input, Script source, bool requireBrackets = true)
        {
            if (TryGetVariable(input, out IConditionVariable var, out _, source, requireBrackets))
            {
                if (var is IStringVariable str)
                    return str.Value;
            }

            return input;
        }

        /// <summary>
        /// Attempts to parse a string input into a <see cref="float"/>. Functionally similar to <see cref="float.TryParse(string, out float)"/>, but also supports SE variables.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="result">The result of the parse.</param>
        /// <param name="source">The source script.</param>
        /// <param name="requireBrackets">If brackets are required to parse variables.</param>
        /// <returns>Whether or not the parse was successful.</returns>
        public static bool TryParse(string input, out float result, Script source, bool requireBrackets = true)
        {
            result = Parse(input, source, requireBrackets);
            return result != float.NaN && result.ToString() != "NaN"; // Hacky but fixes it?
        }

        /// <summary>
        /// Attempts to parse a string input into a <see cref="int"/>. Functionally similar to <see cref="int.TryParse(string, out int)"/>, but also supports SE variables.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="result">The result of the parse.</param>
        /// <param name="source">The source script.</param>
        /// <param name="requireBrackets">If brackets are required to parse variables.</param>
        /// <returns>Whether or not the parse was successful.</returns>
        public static bool TryParse(string input, out int result, Script source, bool requireBrackets = true)
        {
            result = ParseInt(input, source, requireBrackets);
            return result != int.MinValue;
        }

        /// <summary>
        /// Attempts to parse a string input into a <see cref="long"/>. Functionally similar to <see cref="long.TryParse(string, out int)"/>, but also supports SE variables.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="result">The result of the parse.</param>
        /// <param name="source">The source script.</param>
        /// <param name="requireBrackets">If brackets are required to parse variables.</param>
        /// <returns>Whether or not the parse was successful.</returns>
        public static bool TryParse(string input, out long result, Script source, bool requireBrackets = true)
        {
            result = ParseLong(input, source, requireBrackets);
            return result != long.MinValue;
        }

        /// <summary>
        /// Attempts to parse a string input into <typeparamref name="T"/>, where T is an <see cref="Enum"/>. Functionally similar to <see cref="Enum.TryParse{TEnum}(string, out TEnum)"/>, but also supports SE variables.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="result">The result of the parse.</param>
        /// <param name="source">The source script.</param>
        /// <param name="requireBrackets">If brackets are required to parse variables.</param>
        /// <typeparam name="T">The Enum type to cast to.</typeparam>
        /// <returns>Whether or not the parse was successful.</returns>
        public static bool TryParse<T>(string input, out T result, Script source, bool requireBrackets = true)
            where T : struct, Enum
        {
            input = input.Trim();
            if (Enum.TryParse(input, true, out result))
            {
                return true;
            }

            if (TryGetVariable(input, out IConditionVariable vr, out _, source, requireBrackets))
            {
                if (vr is IStringVariable strVar && Enum.TryParse(strVar.Value, true, out result))
                {
                    return true;
                }
            }

            return false;
        }

        public static object Parse(string input, Type enumType, Script source, bool requireBrackets = true)
        {
            try
            {
                object result = Enum.Parse(enumType, input, true);
                return result;
            }
            catch
            {
            }

            if (TryGetVariable(input, out IConditionVariable vr, out _, source, requireBrackets))
            {
                if (vr is IStringVariable strVar)
                {
                    try
                    {
                        object result = Enum.Parse(enumType, strVar.Value, true);
                        return result;
                    }
                    catch
                    {
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Replaces all the occurrences of variables in a string.
        /// </summary>
        /// <param name="input">The string to perform the replacements on.</param>
        /// <param name="source">The script that is currently running to replace variables. Used only for per-script variables.</param>
        /// <returns>The modified string.</returns>
        /// <remarks>This is intended for strings that contain both regular text and variables. Otherwise, see <see cref="ReplaceVariable(string, Script, bool)"/>.</remarks>
        public static string ReplaceVariables(string input, Script source)
        {
            source.DebugLog($"Replacing variables of input '{input}'");
            string[] variables = IsolateVariables(input, source);

            foreach (var variable in variables)
            {
                source.DebugLog("Isolated variable: " + variable);

                if (!TryGetVariable(variable, out IConditionVariable condition, out bool reversed, source))
                {
                    source.DebugLog("Invalid variable.");
                    continue;
                }

                source.DebugLog("Valid variable.");

                try
                {
                    input = input.Replace(variable, condition.String(source, reversed));
                }
                catch (InvalidCastException e)
                {
                    Log.Warn($"[Script: {source?.ScriptName ?? "N/A"}] [L: {source?.CurrentLine.ToString() ?? "N/A"}] {ErrorGen.Get(ErrorCode.VariableReplaceError, condition.Name, e.Message)}");
                }
                catch (Exception e)
                {
                    Log.Warn($"[Script: {source?.ScriptName ?? "N/A"}] [L: {source?.CurrentLine.ToString() ?? "N/A"}] {ErrorGen.Get(ErrorCode.VariableReplaceError, condition.Name, source?.Debug == true ? e : e.Message)}");
                }
            }

            return input;
        }

        public static string ReplaceVariables(object input, Script source)
            => ReplaceVariables(input.ToString(), source);

        /// <summary>
        /// Isolates all variables from a string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="source">The script source.</param>
        /// <returns>The variables used within the string.</returns>
        public static string[] IsolateVariables(string input, Script source)
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
    }
}