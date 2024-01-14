namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class GeneratorAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "GENERATORS";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Modifies genrators.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode to use. Valid options: OPEN, CLOSE, LOCK, UNLOCK, OVERCHARGE, ACTIVATE, DEACTIVATE", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            foreach (Generator generator in Generator.List)
            {
                switch (Arguments[0].ToUpper())
                {
                    case "OPEN":
                        generator.IsOpen = true;
                        break;
                    case "CLOSE":
                        generator.IsOpen = false;
                        break;
                    case "LOCK":
                        generator.IsUnlocked = false;
                        break;
                    case "UNLOCK":
                        generator.IsUnlocked = true;
                        break;
                    case "OVERCHARGE":
                        generator.IsEngaged = true;
                        break;
                    case "ACTIVATE":
                        generator.IsActivating = true;
                        break;
                    case "DEACTIVATE":
                        generator.IsActivating = false;
                        break;
                    default:
                        return new(MessageType.InvalidOption, this, "mode", Arguments[0], "Valid options: OPEN, CLOSE, LOCK, UNLOCK, OVERCHARGE, ACTIVATE, DEACTIVATE");
                }
            }

            return new(true);
        }
    }
}
