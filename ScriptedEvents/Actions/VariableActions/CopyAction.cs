using Exiled.API.Features;

namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;

    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class CopyAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "COPY";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Returns the provided variable value.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variable", typeof(IVariable), "The variable to copy.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            object toRet;

            switch ((IVariable)Arguments[0])
            {
                case IPlayerVariable plrVar:
                    toRet = plrVar.Players.ToArray();
                    break;
                case ILiteralVariable lvVar:
                    toRet = lvVar.Value;
                    break;
                default:
                    throw new ArgumentException("Variable is not a valid variable type");
            }

            return new(true, variablesToRet: new[] { toRet });
        }
    }
}