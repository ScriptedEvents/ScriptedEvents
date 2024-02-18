namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class GeneratorRuleAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "GENERATORRULE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Modifies genrator behavior.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode to use. Valid options: ACTIVATIONTIME, DEACTIVATIONTIME, LEVERDELAY, INTERACTIONCOOLDOWN", true),
            new Argument("value", typeof(int), "The value to set as the rule.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            int value = (int)Arguments[1];

            foreach (Generator generator in Generator.List)
            {
                switch (Arguments[0].ToUpper())
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
                    default:
                        return new(MessageType.InvalidOption, this, "mode", Arguments[0], "Valid options: ACTIVATIONTIME, DEACTIVATIONTIME, LEVERDELAY, INTERACTIONCOOLDOWN");
                }
            }

            return new(true);
        }
    }
}
