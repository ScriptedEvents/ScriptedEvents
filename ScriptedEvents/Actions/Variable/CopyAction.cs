using System;
using System.Linq;
using ScriptedEvents.API.Features.Exceptions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;
using ScriptedEvents.Variables.Interfaces;

namespace ScriptedEvents.Actions.Variable
{
    public class CopyAction : IScriptAction, IHelpInfo, IReturnValueAction
    {
        /// <inheritdoc/>
        public string Name => "Copy";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "It just copies the variable you give to it.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variable", typeof(IVariable), "The variable to copy.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return (IVariable)Arguments[0]! switch
            {
                IPlayerVariable plrVar => new(true, new(plrVar.Players.ToArray())),
                ILiteralVariable lvVar => new(true, new(lvVar.Value)),
                _ => throw new ImpossibleException()
            };
        }
    }
}