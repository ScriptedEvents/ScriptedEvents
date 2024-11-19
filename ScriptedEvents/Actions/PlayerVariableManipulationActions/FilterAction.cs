﻿using System;
using System.Linq;
using Exiled.API.Enums;
using Exiled.CustomItems.API.Features;
using PlayerRoles;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Features;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.PlayerVariableManipulationActions
{
    public class FilterAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "Filter";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.PlayerVariableManipualtion;

        /// <inheritdoc/>
        public string Description => "Returns a copy of the provided player reference where players match the criteria in order to get included in the output.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
             new Argument("players", typeof(PlayerCollection), "The players to filter.", true),
             new OptionsArgument("type", true,
                    new Option("Role", "Filters by role. Use a 'RoleTypeId' as the 'input' argument."),
                    new Option("Team", "Filters by team. Use a 'Team' type as the 'input' argument."),
                    new Option("Room", "Filters by room. Use a 'RoomType' as the 'input' argument."),
                    new Option("UserID", "Filters by user id (like steam id). Use a specific user id as the 'input' argument."),
                    new Option("PlayerID", "Filters by player id (id assigned in game). Use a specific player id as the 'input' argument."),
                    new Option("Item", "Filters by item in inventory. Use a 'ItemType' as the 'input' argument."),
                    new Option("HeldItem", "Filters by 'ItemType' in hand. Use a 'ItemType' as the 'input' argument."),
                    new Option("Group", "Filters by group. Use a group name as the 'input' argument."),
                    new Option("IsStaff", "Filters by having RA access. Use a TRUE/FALSE value as the 'input' argument."),
                    new Option("Effect", "Filters by 'EffectType' the player has. Use a 'EffectType' as the 'input' argument.")),
             new Argument("input", typeof(string), "What to use as the filter (look at 'type' argument for guidance)", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var players = ((PlayerCollection)Arguments[0]!).GetArray();
            var input = (string)Arguments[2]!;

            var ret = Arguments[1]!.ToUpper() switch
            {
                "ROLE" when Parser.TryGetEnum(input, out RoleTypeId rt, script, out _) => players.Where(plr => plr.Role.Type == rt),
                "TEAM" when Parser.TryGetEnum(input, out Team team, script, out _) => players.Where(plr => plr.Role.Team == team),
                "ROOM" when Parser.TryGetEnum(input, out RoomType room, script, out _) => players.Where(plr => plr.CurrentRoom?.Type == room),
                "USERID" => players.Where(plr => plr.UserId == input),
                "PLAYERID" => players.Where(plr => plr.Id.ToString() == input),
                "ITEM" when Parser.TryGetEnum(input, out ItemType item, script, out _) => players.Where(plr => plr.Items.Any(i => i.Type == item)),
                "ITEM" when CustomItem.TryGet(input, out CustomItem? customItem) => players.Where(plr => plr.Items.Any(item => CustomItem.TryGet(item, out CustomItem? customItem2) && customItem == customItem2)),
                "HELDITEM" when Parser.TryGetEnum(input, out ItemType item, script, out _) => players.Where(plr => plr.CurrentItem?.Type == item),
                "HELDITEM" when CustomItem.TryGet(input, out CustomItem? customItem) => players.Where(plr => CustomItem.TryGet(plr.CurrentItem, out CustomItem? customItem2) && customItem == customItem2),
                "GROUP" => players.Where(plr => plr.GroupName == input),
                "ISSTAFF" => players.Where(plr => plr.RemoteAdminAccess == input.AsBool()),
                "EFFECT" when Parser.TryGetEnum(input, out EffectType et, script, out _) => players.Where(plr => plr.TryGetEffect(et, out _)),
                _ => throw new ArgumentException($"The provided value '{Arguments[1]}' is not a valid filter method, or the provided input '{input}' is not valid for the specified filter method."),
            };

            return new(true, new(ret.ToArray()));
        }
    }
}