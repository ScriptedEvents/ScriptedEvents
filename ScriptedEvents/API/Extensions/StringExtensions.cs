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
    }
}
