namespace ScriptedEvents.API.Features
{
    using System;

    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    /// <summary>
    /// A class used to store and retrieve all variables.
    /// </summary>
    public static class SEParser
    {
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

            if (VariableSystemV2.TryGetVariable(input, source, out VariableResult result, requireBrackets) && result.ProcessorSuccess)
            {
                return Parse(result.String(), source, requireBrackets);
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

            if (VariableSystemV2.TryGetVariable(input, source, out VariableResult result, requireBrackets) && result.ProcessorSuccess)
            {
                return ParseInt(result.String(), source, requireBrackets);
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

            if (VariableSystemV2.TryGetVariable(input, source, out VariableResult result, requireBrackets) && result.ProcessorSuccess)
            {
                return ParseLong(result.String(), source, requireBrackets);
            }

            return int.MinValue;
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

            if (VariableSystemV2.TryGetVariable(input, source, out VariableResult vresult, requireBrackets) && vresult.ProcessorSuccess)
            {
                return TryParse(vresult.String(), out result, source, requireBrackets);
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

            if (VariableSystemV2.TryGetVariable(input, source, out VariableResult vresult, requireBrackets) && vresult.ProcessorSuccess)
            {
                return Parse(vresult.String(), enumType, source, requireBrackets);
            }

            return null;
        }
    }
}