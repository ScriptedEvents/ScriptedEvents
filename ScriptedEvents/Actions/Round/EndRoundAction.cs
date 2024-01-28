namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class EndRoundAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "ENDROUND";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Round;

        /// <inheritdoc/>
        public string Description => "Ends the round.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => Array.Empty<Argument>();

        /// <inheritdoc/>
        public ActionResponse Execute(Script scr)
        {
            Round.EndRound(true);
            return new(true);
        }
    }
}