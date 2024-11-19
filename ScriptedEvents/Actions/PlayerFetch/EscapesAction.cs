using System;
using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Modules;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.PlayerFetch
{
    public class EscapesAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "EscapedPlayers";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.PlayerFetch;

        /// <inheritdoc/>
        public string Description => "Returns players which have escaped the facility.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new OptionsArgument("mode", true,
                    new Option("All", "Scientists and ClassDs which have escaped."),
                    new Option("Scientists", "Scientists which have escaped."),
                    new Option("ClassD", "ClassDs which have escaped.")),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var ret = Arguments[0]!.ToUpper() switch
            {
                "ALL" => EventHandlingModule.Singleton!.Escapes[RoleTypeId.ClassD].Union(EventHandlingModule.Singleton!.Escapes[RoleTypeId.Scientist]).ToArray(),
                "SCIENTISTS" => EventHandlingModule.Singleton!.Escapes[RoleTypeId.Scientist].ToArray(),
                "CLASSD" => EventHandlingModule.Singleton!.Escapes[RoleTypeId.ClassD].ToArray(),
                _ => throw new ArgumentException()
            };

            return new(true, new(ret));
        }
    }
}