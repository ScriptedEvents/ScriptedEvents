namespace ScriptedEvents.API.Extensions
{
    using System.Linq;
    using System.Text;

    using Exiled.API.Features.Pools;

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
        /// Converts a string input to a boolean.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The boolean.</returns>
        public static bool AsBool(this string input)
        {
            if (input is null)
                return false;

            if (bool.TryParse(input, out bool r))
                return r;

            return input.ToUpper() is "Y" or "YES";
        }

        /// <summary>
        /// Alternative to <see cref="string.Replace(string, string)"/> which takes an object as the newValue (and ToStrings it automatically).
        /// </summary>
        /// <param name="input">The string to perform the replacement on.</param>
        /// <param name="oldValue">The string to look for.</param>
        /// <param name="newValue">The value to replace it with.</param>
        /// <returns>The modified string.</returns>
        public static string Replace(this string input, string oldValue, object newValue) => input.Replace(oldValue, newValue.ToString());

        /// <summary>
        /// Converts an object to a string and uppercases it.
        /// </summary>
        /// <param name="input">The input object.</param>
        /// <returns>The new string.</returns>
        public static string ToUpper(this object input) => input.ToString().ToUpper();

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

            foreach (object obj in list)
            {
                if (obj is string s)
                    sb.Append(s + sep);
                else
                    sb.Append(obj.ToString());
            }

            string str = StringBuilderPool.Pool.ToStringReturn(sb);
            return str.TrimEnd();
        }
    }
}
