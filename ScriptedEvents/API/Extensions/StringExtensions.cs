namespace ScriptedEvents.API.Extensions
{
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
    }
}
