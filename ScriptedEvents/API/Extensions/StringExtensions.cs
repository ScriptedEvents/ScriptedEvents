namespace ScriptedEvents.API.Extensions
{
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    using Exiled.API.Features.Pools;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;

    /// <summary>
    /// Contains useful extensions.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Removes every whitespace character from a string, UNLESS the space is inside of {}.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The new string, without any whitespace characters.</returns>
        public static string RemoveWhitespace(this string input)
        {
            return Regex.Replace(input, @"\s+", string.Empty);
        }

        public static bool IsBool(this string input, out bool value, out ErrorInfo? errorInfo, Script? script = null)
        {
            if (script is not null)
                input = Parser.ReplaceContaminatedValueSyntax(input, script);

            if (string.IsNullOrEmpty(input))
            {
                value = default;
                errorInfo = new(
                    "Empty value cannot be intrpreted as a true/false value",
                    "The provided value is null or empty, which does not qualify for a true/false value.",
                    "IsBool Extension");
                return false;
            }

            if (bool.TryParse(input, out var r))
            {
                value = r;
                errorInfo = null;
                return true;
            }

            switch (input.ToUpper())
            {
                case "YES" or "Y" or "T":
                    value = true;
                    errorInfo = null;
                    return true;
                case "NO" or "N" or "F":
                    value = false;
                    errorInfo = null;
                    return true;
            }

            value = false;
            errorInfo = new(
                $"Value '{input}' cannot be interpreted as a true/false value",
                "The provided value does not match any criteria by which it could be assigned a true/false value.",
                "IsBool Extension");
            return false;
        }

        /// <summary>
        /// Converts a string input to a boolean.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="source">The script source.</param>
        /// <returns>The boolean.</returns>
        public static bool AsBool(this string input, Script? source = null)
        {
            IsBool(input, out var v, out _, source);
            return v;
        }

        /// <summary>
        /// Alternative to <see cref="string.Replace(string, string)"/> which takes an object as the newValue (and ToStrings it automatically).
        /// </summary>
        /// <param name="input">The string to perform the replacement on.</param>
        /// <param name="oldValue">The string to look for.</param>
        /// <param name="newValue">The value to replace it with.</param>
        /// <returns>The modified string.</returns>
        public static string Replace(this string input, string oldValue, object newValue) => input.Replace(oldValue, newValue.ToString());

        public static string String(this object input) => input is string str ? str : input.ToString();

        /// <summary>
        /// Converts an object to a string and uppercases it.
        /// </summary>
        /// <param name="input">The input object.</param>
        /// <returns>The new string.</returns>
        public static string ToUpper(this object input) => input.String().ToUpper();

        /// <summary>
        /// Joins object parameters into a string message.
        /// </summary>
        /// <param name="param">The parameters.</param>
        /// <param name="skipCount">Amount of parameters to skip.</param>
        /// <param name="sep">The separator string.</param>
        /// <returns>The new string.</returns>
        public static string JoinMessage(this object[] param, int skipCount = 0, string sep = " ")
        {
            StringBuilder sb = StringBuilderPool.Pool.Get();
            var list = param.Skip(skipCount);
            var enumerable = list as object[] ?? list.ToArray();
            if (!enumerable.Any()) return string.Empty;

            foreach (var obj in enumerable)
            {
                if (obj is string s)
                    sb.Append(s + sep);
                else
                    sb.Append(obj + sep);
            }

            var str = StringBuilderPool.Pool.ToStringReturn(sb);
            return str.Substring(0, str.Length - sep.Length);
        }

        /// <summary>
        /// Joins object parameters into a string message.
        /// </summary>
        /// <param name="param">The parameters.</param>
        /// <param name="skipCount">Amount of parameters to skip.</param>
        /// <param name="sep">The separator string.</param>
        /// <returns>The new string.</returns>
        public static string JoinMessage(this string[] param, int skipCount = 0, string sep = " ")
        {
            StringBuilder sb = StringBuilderPool.Pool.Get();
            var list = param.Skip(skipCount);
            var enumerable = list as object[] ?? list.ToArray();
            if (!enumerable.Any()) return string.Empty;

            foreach (var obj in enumerable)
            {
                if (obj is string s)
                    sb.Append(s + sep);
                else
                    sb.Append(obj + sep);
            }

            var str = StringBuilderPool.Pool.ToStringReturn(sb);
            return str.Substring(0, str.Length - sep.Length);
        }

        public static int CountOccurrences(this string text, char character)
        {
            int count = 0;

            foreach (char c in text)
            {
                if (c == character) count++;
            }

            return count;
        }
    }
}
