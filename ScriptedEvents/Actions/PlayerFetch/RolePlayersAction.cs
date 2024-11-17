using System;
using System.Linq;
using Exiled.API.Features;
using PlayerRoles;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.PlayerFetch
{
    public class RolePlayersAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "GetByRole";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.PlayerFetch;

        /// <inheritdoc/>
        public string Description => "Returns the players with the specified role.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("role", typeof(RoleTypeId), "The role which the players should be fetched by.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return new(true, new(Player.List.Where(ply => ply.Role == ((RoleTypeId)Arguments[0]!)).ToArray()));
        }
    }
}