using System;
using ScriptedEvents.API.Enums;
using ScriptedEvents.API.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Round
{
    public class RoundAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "ROUND";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Round;

        /// <inheritdoc/>
        public string Description => "Manages the round and its settings.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("START", "Starts the round."),
                new("END", "Ends the round"),
                new("LOCK", "Locks the round."),
                new("UNLOCK", "Unlocks the round"),
                new("RESTART", "Restarts the round.")),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script scr)
        {
            switch ((string)Arguments[0])
            {
                case "START":
                    Exiled.API.Features.Round.Start(); break;
                case "END":
                    Exiled.API.Features.Round.EndRound(true); break;
                case "LOCK":
                    Exiled.API.Features.Round.IsLocked = true; break;
                case "UNLOCK":
                    Exiled.API.Features.Round.IsLocked = false; break;
                case "RESTART":
                    Exiled.API.Features.Round.Restart(); break;
            }

            return new(true);
        }
    }
}