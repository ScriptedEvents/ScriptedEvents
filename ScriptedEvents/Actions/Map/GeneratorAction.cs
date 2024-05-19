namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class GeneratorAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "GENERATORS";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Modifies all genrators.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("OPEN", "Opens all generators."),
                new("CLOSE", "Closes all generators."),
                new("LOCK", "Locks all generators, requiring a keycard to use."),
                new("UNLOCK", "Unlocks all generators, no longer requiring a keycard to use."),
                new("OVERCHARGE", "Engages all generators, causing an overcharge."),
                new("ACTIVATE", "Begins activating all generators."),
                new("DEACTIVATE", "Deactivates all generators.")),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
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
                }
            }

            return new(true);
        }
    }
}
