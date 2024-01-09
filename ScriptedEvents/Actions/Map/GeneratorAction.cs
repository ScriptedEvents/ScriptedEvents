namespace ScriptedEvents.Actions
{
    using System;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using Exiled.API.Features;

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
        public string Description => "Modifies tesla gates.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode to run. Valid options: PLAYERS, ROLETYPE, DISABLE, ENABLE", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            foreach (Generator generator in Generator.List)
            {
                generator.IsEngaged = false;
                generator.IsOpen = false;
                generator.IsActivating = false;
                generator.IsEngaged = false;
                generator.IsUnlocked = false;
                generator.DenyUnlockAndResetCooldown();
                generator.Base.ServerSetFlag(MapGeneration.Distributors.Scp079Generator.GeneratorFlags.Open, false);
                   
            }

            return new(true);
        }
    }
}
