namespace ScriptedEvents.Actions.AllInOne
{
    using System;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
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
                new OptionValueDepending("Engaged", "Amount of engaged generators.", typeof(int)),
                new OptionValueDepending("Activating", "Amount of activating generators.", typeof(int)),
                new OptionValueDepending("Unlocked", "Amount of unlocked generators.", typeof(int)),
                new OptionValueDepending("Opened", "Amount of open generators.", typeof(int)),
                new OptionValueDepending("Closed", "Amount of closed generators.", typeof(int))),
        };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.AllInOneInfo;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            int ret = Arguments[0]!.ToUpper() switch
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