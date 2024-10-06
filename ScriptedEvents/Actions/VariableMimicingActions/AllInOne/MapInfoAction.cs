namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class MapInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "MAPINFO";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting map related information.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("Seed", "Returns the map seed."),
                new("IsOvercharged", "Returns a TRUE/FALSE value saying if the overcharge happened."),
                new("IsDecontaminated", "Returns a TRUE/FALSE value saying if the LightContainment is decontaminated."),
                new("Is914Active", "Returns a TRUE/FALSE value saying if SCP-914 is currently active."),
                new("IsCassieSpeaking", "Returns a TRUE/FALSE value saying if the cassie is currently speaking.")),
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
                "SEED" => Map.Seed.ToString(),
                "ISOVERCHARGED" => Recontainer.IsContainmentSequenceDone.ToUpper(),
                "ISDECONTAMINATED" => Map.IsLczDecontaminated.ToUpper(),
                "IS914ACTIVE" => Scp914.IsWorking.ToUpper(),
                "ISCASSIESPEAKING" => Cassie.IsSpeaking.ToUpper(),
                _ => throw new ArgumentException()
            };

            return new(true, variablesToRet: new[] { ret });
        }
    }
}