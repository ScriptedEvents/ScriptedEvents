namespace ScriptedEvents.API.Features
{
    using System.Collections.Generic;

    using Exiled.API.Features;

    /// <summary>
    /// Class to save variables long-term.
    /// </summary>
    public static class IndentationHelper
    {
        public static readonly Dictionary<int, int> LineIndetationValue = new();

        public static readonly Dictionary<int, int> IndentationEnd = new();

        /// <summary>
        /// Register all indentation.
        /// </summary>
        /// <param name="script">The script object.</param>
        /// <returns>Whether or not file is formatted correctly.</returns>
        public static bool TryRegister(Script script)
        {
            string[] lines = script.RawText.Split('\n');
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                string line = lines[lineIndex];

                if (string.IsNullOrEmpty(line))
                    continue;

                if (!TryCountTabs(line, out int tabCount))
                    Log.Warn($"[error");

                LineIndetationValue.Add(lineIndex, tabCount);
                Log.Warn($"[{lineIndex}] Tabs: {tabCount} Line {line}");
            }

            return true;
        }

        private static bool TryCountTabs(string line, out int tabCount)
        {
            tabCount = 0;
            foreach (char c in line)
            {
                if (c == '\t')
                {
                    tabCount++;
                    continue;
                }
                else if (c == ' ')
                {
                    return false;
                }

                break;
            }

            return true;
        }
    }
}
