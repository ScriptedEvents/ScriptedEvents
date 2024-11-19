using System;
using Exiled.API.Features;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.RoundRule
{
    public class GeneratorRuleAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "GeneratorRule";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string Description => "Modifies generator behavior.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                    new Option("ActivationTime", "The amount of time for generators to activate."),
                    new Option("DeactivationTime", "The amount of time for generators to deactivate."),
                    new Option("LeverDelay", "The delay to change generator levers."),
                    new Option("InteractionCooldown", "The cooldown for interacting with generators.")),
            new Argument("value", typeof(int), "The value to set as the rule.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            int value = (int)Arguments[1]!;

            foreach (Generator generator in Generator.List)
            {
                switch (Arguments[0]!.ToUpper())
                {
                    case "ACTIVATIONTIME":
                        generator.ActivationTime = value;
                        break;
                    case "DEACTIVATIONTIME":
                        generator.DeactivationTime = value;
                        break;
                    case "LEVERDELAY":
                        generator.LeverDelay = value;
                        break;
                    case "INTERACTIONCOOLDOWN":
                        generator.InteractionCooldown = value;
                        break;
                }
            }

            return new(true);
        }
    }
}
