namespace ScriptedEvents.Variables.Strings
{
#pragma warning disable SA1402 // File may only contain a single type
    using System;

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
            new Log(),
            new Index(),
            new ReplaceVariable(),
        };
    }

    public class ReplaceVariable : IStringVariable, IArgumentVariable, INeedSourceVariable
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

        public Script Source { get; set; }
    }

    public class RandomItem : IStringVariable
    {
        /// <inheritdoc/>
        public string Name => "{RANDOMITEM}";

        /// <inheritdoc/>
        public string Description => "Gets the ItemType of a random item.";

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                ItemType[] items = (ItemType[])Enum.GetValues(typeof(ItemType));
                return items[UnityEngine.Random.Range(0, items.Length)].ToString();
            }
        }
    }

    public class Log : IStringVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{LOG}";

        /// <inheritdoc/>
        public string Description => "Shows the name of the variable with its value. Useful for quick debugging.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Script Source { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("variable", typeof(IConditionVariable), "The name of the variable.", true),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                IConditionVariable variable = (IConditionVariable)Arguments[0];

                return variable switch
                {
                    IStringVariable strV => $"{strV.Name} = {strV.Value}",
                    IFloatVariable floatV => $"{floatV.Name} = {floatV.Value}",
                    ILongVariable longV => $"{longV.Name} = {longV.Value}",
                    IBoolVariable boolV => $"{boolV.Name} = {boolV.Value}",
                    _ => $"{variable.Name} = UNKNOWN VALUE",
                };
            }
        }
    }

    public class Index : IStringVariable, IArgumentVariable, INeedSourceVariable
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
        public Script Source { get; set; }

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
