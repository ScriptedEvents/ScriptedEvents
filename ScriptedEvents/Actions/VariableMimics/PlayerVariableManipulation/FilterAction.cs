namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CustomPlayerEffects;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using PlayerRoles;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class FilterAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "LIMIT";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.PlayerVariableManipualtion;

        /// <inheritdoc/>
        public string Description => "Returns a copy of the provided players where players need to match specified criteria in order to get included in the output.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new OptionsArgument("type", true,
                    new("ROLE", "Filters by role. Use a 'RoleTypeId' type as the 'input' argument."),
                    new("TEAM", "Filters by team. Use a 'TeamType' type as the 'input' argument."),
                    new("ROOM", "Filters by room. Use a 'RoomType' type as the 'input' argument."),
                    new("USERID", "Filters by user id (like steam id). Use a specific user id as the 'input' argument."),
                    new("PLAYERID", "Filters by player id (id assigned in game). Use a specific player id as the 'input' argument."),
                    new("ITEM", "Filters by item in inventory. Use a 'ItemType' type as the 'input' argument."),
                    new("HELDITEM", "Filters by 'ItemType' in hand. Use a 'ItemType' type as the 'input' argument."),
                    new("GROUP", "Filters by group. Use a group name as the 'input' argument."),
                    new("ISSTAFF", "Filters by having RA access. Use a TRUE/FALSE value as the 'input' argument."),
                    new("EFFECT", "Filters by 'EffectType' the player has. Use a 'EffectType' type as the 'input' argument.")),
             new Argument("players", typeof(PlayerCollection), "The players to filter.", true),
             new Argument("input", typeof(string), "What to use as the filter (look at 'type' argument for guidance)", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Player[] players = ((PlayerCollection)Arguments[0]).GetArray();
            string input = (string)Arguments[2];

            IEnumerable<Player> ret = Arguments[0].ToUpper() switch
            {
                "ROLE" when SEParser.TryParse(input, out RoleTypeId rt, script, false) => players.Where(plr => plr.Role.Type == rt),
                "TEAM" when SEParser.TryParse(input, out Team team, script, false) => players.Where(plr => plr.Role.Team == team),
                "ROOM" when SEParser.TryParse(input, out RoomType room, script, false) => players.Where(plr => plr.CurrentRoom?.Type == room),
                "USERID" => players.Where(plr => plr.UserId == input),
                "PLAYERID" => players.Where(plr => plr.Id.ToString() == input),
                "ITEM" when SEParser.TryParse(input, out ItemType item, script, false) => players.Where(plr => plr.Items.Any(i => i.Type == item)),
                "ITEM" when CustomItem.TryGet(input, out CustomItem customItem) => players.Where(plr => plr.Items.Any(item => CustomItem.TryGet(item, out CustomItem customItem2) && customItem == customItem2)),
                "HELDITEM" when SEParser.TryParse(input, out ItemType item, script, false) => players.Where(plr => plr.CurrentItem?.Type == item),
                "HELDITEM" when CustomItem.TryGet(input, out CustomItem customItem) => players.Where(plr => CustomItem.TryGet(plr.CurrentItem, out CustomItem customItem2) && customItem == customItem2),
                "GROUP" => players.Where(plr => plr.GroupName == input),
                "ISSTAFF" => players.Where(plr => plr.RemoteAdminAccess == input.AsBool()),
                "EFFECT" when SEParser.TryParse(input, out EffectType et, script, false) => players.Where(plr => plr.TryGetEffect(et, out StatusEffectBase seb)),
                _ => throw new ArgumentException($"The provided value '{Arguments[1]}' is not a valid filter method, or the provided input '{input}' is not valid for the specified filter method."),
            };

            return new(true, variablesToRet: new[] { ret });
        }
    }
}