namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class RoundlockAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "ROUNDLOCK";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Round;

        /// <inheritdoc/>
        public string Description => "Set server's roundlock.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("lockStatus", typeof(bool), "Whether or not to lock the round.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Round.IsLocked = (bool)Arguments[0];
            return new(true);
        }
    }
}