namespace ScriptedEvents.Actions.AllInOne
{
    using System;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class GeneratorInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "GeneratorInfo";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting generator related information.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("Engaged", "Returns the amount of engaged generators."),
                new("Activating", "Returns the amount of activating generators."),
                new("Unlocked", "Returns the amount of unlocked generators."),
                new("Opened", "Returns the amount of open generators."),
                new("Closed", "Returns the amount of closed generators.")),
        };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.AllInOneInfo;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            int ret = Arguments[0].ToUpper() switch
            {
                "ENGAGED" => Generator.Get(GeneratorState.Engaged).Count(),
                "ACTIVATING" => Generator.Get(GeneratorState.Activating).Count(),
                "UNLOCKED" => Generator.Get(GeneratorState.Unlocked).Count(),
                "OPENED" => Generator.Get(GeneratorState.Open).Count(),
                "CLOSED" => Generator.Get(gen => gen.IsOpen is false).Count(),
                _ => throw new ArgumentException()
            };

            return new(true, new(ret.ToString()));
        }
    }
}