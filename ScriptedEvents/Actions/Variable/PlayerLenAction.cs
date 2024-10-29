using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class PlayerLenAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "LEN";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <inheritdoc/>
        public string Description => "Returns a number saying how many players are contained inside the specified variable.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("players", typeof(PlayerCollection), "The players.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script) => new(true, variablesToRet: new[] { ((PlayerCollection)Arguments[0]).Length.ToString() });
    }
}