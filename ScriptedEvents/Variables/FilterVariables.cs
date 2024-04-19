namespace ScriptedEvents.Variables.Filters
{
#pragma warning disable SA1402 // File may only contain a single type
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CustomPlayerEffects;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using PlayerRoles;

    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class FilterVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Filters";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Max(),
            new Filter(),
            new GetByIndex(),
        };
    }

    public class Max : IFloatVariable, IPlayerVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{MAX}";

        /// <inheritdoc/>
        public string Description => "Gets a specified amount of players from a player variable.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; } = new[]
        {
            new Argument("name", typeof(IPlayerVariable), "The name of the variable to select from.", true),
            new Argument("amount", typeof(int), "The amount of players to select (default: 1)", false),
        };

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public Script Source { get; set; }

        /// <inheritdoc/>
        public IEnumerable<Player> Players
        {
            get
            {
                IEnumerable<Player> players = ((IPlayerVariable)Arguments[0]).Players;

                int max = 1;

                if (Arguments.Length > 1)
                    max = (int)Arguments[1];

                List<Player> list = players.ToList();

                for (int i = 0; i < max; i++)
                {
                    if (list.Count == 0)
                        yield break;

                    yield return list.PullRandomItem();
                }
            }
        }
    }

    public class Filter : IFloatVariable, IPlayerVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{FILTER}";

        /// <inheritdoc/>
        public string Description => "Filters a player variable by a certain type.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; } = new[]
        {
            new Argument("name", typeof(IPlayerVariable), "The name of the variable to filter.", true),
            new OptionsArgument("type", true,
                    new("ROLE", "Filter by 'RoleTypeId'."),
                    new("TEAM", "Filter by 'Team'."),
                    new("ROOM", "Filter by 'Room'."),
                    new("USERID", "Filter by user id. (steam id)"),
                    new("PLAYERID", "Filter by player id. (id in game)"),
                    new("ITEM", "Filter by item in inventory."),
                    new("HELDITEM", "Filter by item in hand."),
                    new("GROUP", "Filter by group."),
                    new("ISSTAFF", "Filter by having RA access."),
                    new("EFFECT", "Filter by effect.")),
            new Argument("input", typeof(string), "What to use as the filter (ROOM, TEAM, etc.)", true),
        };

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public Script Source { get; set; }

        /// <inheritdoc/>
        public IEnumerable<Player> Players
        {
            get
            {
                IEnumerable<Player> players = ((IPlayerVariable)Arguments[0]).Players;
                string input = (string)Arguments[2];

                return Arguments[1].ToString() switch
                {
                    "ROLE" when VariableSystem.TryParse(input, out RoleTypeId rt, Source, false) => players.Where(plr => plr.Role.Type == rt),
                    "TEAM" when VariableSystem.TryParse(input, out Team team, Source, false) => players.Where(plr => plr.Role.Team == team),
                    "ROOM" when VariableSystem.TryParse(input, out RoomType room, Source, false) => players.Where(plr => plr.CurrentRoom?.Type == room),
                    "USERID" => players.Where(plr => plr.UserId == VariableSystem.ReplaceVariable(input, Source, false)),
                    "PLAYERID" => players.Where(plr => plr.Id.ToString() == VariableSystem.ReplaceVariable(input, Source, false)),
                    "ITEM" when VariableSystem.TryParse(input, out ItemType item, Source, false) => players.Where(plr => plr.Items.Any(i => i.Type == item)),
                    "ITEM" when CustomItem.TryGet(input, out CustomItem customItem) => players.Where(plr => plr.Items.Any(item => CustomItem.TryGet(item, out CustomItem customItem2) && customItem == customItem2)),
                    "HELDITEM" when VariableSystem.TryParse(input, out ItemType item, Source, false) => players.Where(plr => plr.CurrentItem?.Type == item),
                    "HELDITEM" when CustomItem.TryGet(input, out CustomItem customItem) => players.Where(plr => CustomItem.TryGet(plr.CurrentItem, out CustomItem customItem2) && customItem == customItem2),
                    "GROUP" => players.Where(plr => plr.GroupName == input),
                    "ISSTAFF" when VariableSystem.ReplaceVariable(input.ToUpper(), Source).AsBool(Source) => players.Where(plr => plr.RemoteAdminAccess),
                    "ISSTAFF" when !VariableSystem.ReplaceVariable(input.ToUpper(), Source).AsBool(Source) => players.Where(plr => !plr.RemoteAdminAccess),
                    "EFFECT" when VariableSystem.TryParse(input, out EffectType et, Source, false) => players.Where(plr => plr.TryGetEffect(et, out StatusEffectBase seb)),
                    _ => throw new ArgumentException($"The provided value '{Arguments[1]}' is not a valid filter method, or the provided input '{input}' is not valid for the specified filter method."),
                };

                throw new ArgumentException(ErrorGen.Get(126));
            }
        }
    }

    public class GetByIndex : IFloatVariable, IPlayerVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{INDEXVAR}";

        /// <inheritdoc/>
        public string Description => "Indexes a player variable and gets ONE player at the specified position.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; } = new[]
        {
            new Argument("name", typeof(IPlayerVariable), "The name of the variable to index.", true),
            new Argument("type", typeof(int), "The index. Number variables can be used (if they are decimal, the decimal portion will be removed)", true),
        };

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public Script Source { get; set; }

        /// <inheritdoc/>
        public IEnumerable<Player> Players
        {
            get
            {
                IEnumerable<Player> players = ((IPlayerVariable)Arguments[0]).Players;
                int index = (int)Arguments[1];

                if (index > players.Count() - 1)
                    throw new IndexOutOfRangeException(ErrorGen.Get(135, index));

                return new List<Player>() { players.ToList()[index] }; // Todo: better solution (yield return didn't work??)
            }
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type