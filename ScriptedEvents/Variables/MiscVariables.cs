namespace ScriptedEvents.Variables.Misc
{
#pragma warning disable SA1402 // File may only contain a single type
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class MiscVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Miscellaneous";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Storage(),
            new Log(),
        };
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
             new Argument("variable", typeof(IConditionVariable), "The variable.", true),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                IConditionVariable variable = (IConditionVariable)Arguments[0];

                return $"{variable.Name.Trim('{', '}')} = {variable.String()}";
            }
        }
    }

    public class Storage : IStringVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{STORAGE}";

        /// <inheritdoc/>
        public string Description => "Retrives a variable from storage.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variableName", typeof(string), "The variable name to retrive.", true),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                return VariableStorage.Read(RawArguments[0]);
            }
        }
    }
#pragma warning restore SA1402 // File may only contain a single type.
}
