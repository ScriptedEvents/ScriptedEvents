using System;
using Exiled.API.Features;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Map
{
    public class GeneratorAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Generators";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Modifies all genrators.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new Option("Open", "Opens all generators."),
                new Option("Close", "Closes all generators."),
                new Option("Lock", "Locks all generators, requiring a keycard to use."),
                new Option("Unlock", "Unlocks all generators, no longer requiring a keycard to use."),
                new Option("Overcharge", "Engages all generators, causing an overcharge."),
                new Option("Activate", "Begins activating all generators."),
                new Option("Deactivate", "Deactivates all generators.")),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            foreach (Generator generator in Generator.List)
            {
                switch (Arguments[0]!.ToUpper())
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
                }
            }

            return new(true);
        }
    }
}
