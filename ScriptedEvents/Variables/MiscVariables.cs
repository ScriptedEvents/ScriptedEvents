namespace ScriptedEvents.Variables.Misc
{
#pragma warning disable SA1402 // File may only contain a single type
    using System;

    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class MiscVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Miscellaneous";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new This(),
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
             new Argument("variable", typeof(IVariable), "The variable.", true),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                try
                {
                    IPlayerVariable plrVar = (IPlayerVariable)Arguments[0];

                    return $"{plrVar.Name.Trim('{', '}')} = {plrVar.String()}";
                }
                catch (InvalidCastException)
                {
                }

                try
                {
                    IConditionVariable variable = (IConditionVariable)Arguments[0];

                    return $"{variable.Name.Trim('{', '}')} = {variable.String()}";
                }
                catch (InvalidCastException)
                {
                    throw new ScriptedEventsException(ErrorGen.Generate(API.Enums.ErrorCode.InvalidVariable, (string)Arguments[0]));
                }
            }
        }
    }

    public class This : IStringVariable, INeedSourceVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{THIS}";

        /// <inheritdoc/>
        public string Description => "Returns information about the script.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode to use. CALLER is the only one. If not provided, will return script name.", false),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                if (Arguments.Length == 0) return Source.ScriptName;

                string mode = Arguments[0].ToUpper();
                return mode switch
                {
                    "CALLER" => Source.CallerScript is not null ? Source.CallerScript.ScriptName : "NONE",
                    _ => throw new ArgumentException("Invalid mode.", mode)
                };
            }
        }

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Script Source { get; set; }
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
