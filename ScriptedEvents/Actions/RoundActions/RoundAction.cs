using System;
using Exiled.API.Features;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.RoundActions
{
    public class RoundAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Round";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Round;

        /// <inheritdoc/>
        public string Description => "Manages the round and its settings.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new Option("Start", "Starts the round."),
                new Option("End", "Ends the round"),
                new Option("Lock", "Locks the round."),
                new Option("Unlock", "Unlocks the round"),
                new Option("Restart", "Restarts the round.")),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script scr)
        {
            switch (Arguments[0]!.ToUpper())
            {
                case "START":
                    Round.Start(); break;
                case "END":
                    Round.EndRound(true); break;
                case "LOCK":
                    Round.IsLocked = true; break;
                case "UNLOCK":
                    Round.IsLocked = false; break;
                case "RESTART":
                    Round.Restart(); break;
            }

            return new(true);
        }
    }
}