namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Handlers;
    using UnityEngine;

    public class EffectPermanentAction : IScriptAction, IHelpInfo
    {
        public string Name => "EFFECTPERM";

        public string[] Aliases => Array.Empty<string>();

        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        public string[] Arguments { get; set; }

        public string Description => "Action for giving/removing permanent player effects.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode (GIVE, REMOVE, CLEAR)", true),
            new Argument("players", typeof(object), "The players to affect, or the RoleType/Team to infect with the role.", true),
            new Argument("effect", typeof(EffectType), "The effect to give or remove.", true),
            new Argument("intensity", typeof(byte), "The intensity of the effect, between 0-255. Math and variables are NOT supported. Defaults to 1", false),
        };

        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 3) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            string mode = Arguments[0].ToUpper();

            if (!Enum.TryParse<EffectType>(Arguments[2], true, out EffectType effect))
                return new(false, "Invalid effect type provided.");

            
        }
    }
}
