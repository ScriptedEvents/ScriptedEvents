namespace ScriptedEvents.Variables.Strings
{
#pragma warning disable SA1402 // File may only contain a single type
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class StringVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Strings";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Index(),
            new ReplaceVariable(),
            new StringCountVariable(),
        };
    }

    public class StringCountVariable : IFloatVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{STR-COUNT}";

        /// <inheritdoc/>
        public string Description => "Returns how many occurences of a string are in a given string.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; } = new[]
        {
            new Argument("variable", typeof(IStringVariable), "The variable on which the operation will be performed.", true),
            new Argument("sequence", typeof(string), "The sequence which will be counted in the given string.", true),
        };

        /// <inheritdoc/>
        public float Value
        {
            get
            {
                // c# really has no method for this for fuck sake
                static int CountOccurrences(string text, string substring)
                {
                    int count = 0;
                    int index = 0;

                    while ((index = text.IndexOf(substring, index)) != -1)
                    {
                        count++;
                        index += substring.Length;
                    }

                    return count;
                }

                string processedVar = ((IStringVariable)Arguments[0]).Value;

                return CountOccurrences(processedVar, (string)Arguments[1]);
            }
        }
    }

    public class ReplaceVariable : IStringVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{STR-REPLACE}";

        /// <inheritdoc/>
        public string Description => "Replaces character sequneces.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; } = new[]
        {
            new Argument("variableName", typeof(IStringVariable), "The variable on which the operation will be performed.", true),
            new Argument("targetSequence", typeof(string), "The sequence which will be replaced.", true),
            new Argument("replacingSequence", typeof(string), "The value to replace with.", true),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                IStringVariable processedVar = (IStringVariable)Arguments[0];
                return processedVar.String().Replace(Arguments[1].ToString(), Arguments[2].ToString());
            }
        }
    }

    public class Index : IStringVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{STR-INDEX}";

        /// <inheritdoc/>
        public string Description => "Extract a certain part of a variables value using an index.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("variable", typeof(IStringVariable), "The name of the variable.", true),
             new Argument("index", typeof(int), "The place from which the value should be taken.", true),
             new Argument("listSplitChar", typeof(char), "The character that will split the variable into a list. Must be a 1 character only.", false),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                IStringVariable value = (IStringVariable)Arguments[0];
                int index = (int)Arguments[1];
                string result;

                if (Arguments.Length >= 3)
                {
                    char delimiter = (char)Arguments[2];

                    string[] resultList = value.Value.Split(delimiter);

                    result = resultList[index].Trim();
                }
                else
                {
                    result = value.Value[index].ToString();
                }

                return result;
            }
        }
    }
}
