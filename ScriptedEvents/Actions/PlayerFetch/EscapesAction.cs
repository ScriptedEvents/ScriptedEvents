using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;

    using Exiled.API.Features;
    using PlayerRoles;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class EscapesAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "ESCAPES";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.PlayerFetch;

        /// <inheritdoc/>
        public string Description => "Returns players which have escaped the facility.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new OptionsArgument("mode", true,
                    new("ALL", "Scientists and ClassDs which have escaped."),
                    new("SCIENTISTS", "Scientists which have escaped."),
                    new("CLASSD", "ClassDs which have escaped.")),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Player[] ret = Arguments[0].ToUpper() switch
            {
                "ALL" => MainPlugin.Handlers.Escapes[RoleTypeId.ClassD].Union(MainPlugin.Handlers.Escapes[RoleTypeId.Scientist]).ToArray(),
                "SCIENTISTS" => MainPlugin.Handlers.Escapes[RoleTypeId.Scientist].ToArray(),
                "CLASSD" => MainPlugin.Handlers.Escapes[RoleTypeId.ClassD].ToArray(),
                _ => throw new ArgumentException()
            };

            return new(true, variablesToRet: new[] { ret });
        }
    }
}