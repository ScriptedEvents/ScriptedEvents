namespace ScriptedEvents.Actions.AllInOne
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    using ServerMap = Exiled.API.Features.Map;

    public class MapInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "MapInfo";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting map related information.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new OptionValueDepending("Seed", "Map seed.", typeof(int)),
                new OptionValueDepending("IsOvercharged", "Did the overcharge happen.", typeof(bool)),
                new OptionValueDepending("IsLczDecontaminated", "Is Light Containment decontaminated.", typeof(bool)),
                new OptionValueDepending("Is914Active", "Is SCP-914 is currently active.", typeof(bool)),
                new OptionValueDepending("IsCassieSpeaking", "Is C.A.S.S.I.E speaking on the entire map.", typeof(bool))),
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
            string ret = Arguments[0].ToUpper() switch
            {
                "SEED" => ServerMap.Seed.ToString(),
                "ISOVERCHARGED" => Recontainer.IsContainmentSequenceSuccessful.ToUpper(),
                "ISLCZDECONTAMINATED" => ServerMap.IsLczDecontaminated.ToUpper(),
                "IS914ACTIVE" => Exiled.API.Features.Scp914.IsWorking.ToUpper(),
                "ISCASSIESPEAKING" => Cassie.IsSpeaking.ToUpper(),
                _ => throw new ArgumentException()
            };

            return new(true, new(ret));
        }
    }
}