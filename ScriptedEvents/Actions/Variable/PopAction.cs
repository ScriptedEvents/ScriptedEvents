using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class PopAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "POP";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Deletes the provided local variable and returns its value.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variable", typeof(IVariable), "The variable to delete.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            object toRet;

            switch ((IVariable)Arguments[0])
            {
                case IPlayerVariable plrVar:
                    script.RemoveVariable(plrVar);
                    toRet = plrVar.GetPlayers().ToArray();
                    break;
                case ILiteralVariable lvVar:
                    script.RemoveVariable(lvVar);
                    toRet = lvVar.Value;
                    break;
                default:
                    throw new ArgumentException("Variable is not a valid variable type");
            }

            return new(true, variablesToRet: new[] { toRet });
        }
    }
}