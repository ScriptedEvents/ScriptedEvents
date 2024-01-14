namespace ScriptedEvents.Variables.Filters
{
#pragma warning disable SA1402 // File may only contain a single type
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using PlayerRoles;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
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
        public string Description => "Filters a player variable and returns random players less than the provided amount.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

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
                if (Arguments.Length < 1)
                {
                    throw new ArgumentException(MsgGen.VariableArgCount(Name, "name", "amount"));
                }

                if (VariableSystem.TryGetPlayers(Arguments[0], out PlayerCollection players, Source, false))
                {
                    int max = 1;
                    if (Arguments.Length > 1 && !VariableSystem.TryParse(Arguments[1], out max, Source, false))
                    {
                        throw new ArgumentException(ErrorGen.Get(134, Arguments[1]));
                    }

                    List<Player> list = players.GetInnerList();

                    for (int i = 0; i < max; i++)
                    {
                        if (list.Count == 0)
                            yield break;

                        yield return list.PullRandomItem();
                    }
                }

                throw new ArgumentException(ErrorGen.Get(131, Arguments[0]));
            }
        }
    }

    public class Filter : IFloatVariable, IPlayerVariable, IArgumentVariable, INeedSourceVariable, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "{FILTER}";

        /// <inheritdoc/>
        public string Description => "Filters a player variable by a certain type.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; } = new[]
        {
            new Argument("name", typeof(IPlayerVariable), "The name of the variable to filter.", true),
            new Argument("type", typeof(string), "The mode to use to filter.", true),
            new Argument("input", typeof(object), "What to use as the filter (RoleType, ZoneType, etc)", true),
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
                if (Arguments.Length < 3)
                {
                    throw new ArgumentException(MsgGen.VariableArgCount(Name, "name", "type", "input"));
                }

                if (VariableSystem.TryGetPlayers(Arguments[0], out PlayerCollection players, Source, false))
                {
                    return Arguments[1].ToString() switch
                    {
                        "ROLE" when VariableSystem.TryParse(Arguments[2], out RoleTypeId rt, Source, false) => players.Where(plr => plr.Role.Type == rt),
                        "TEAM" when VariableSystem.TryParse(Arguments[2], out Team team, Source, false) => players.Where(plr => plr.Role.Team == team),
                        "ZONE" when VariableSystem.TryParse(Arguments[2], out ZoneType zt, Source, false) => players.Where(plr => plr.Zone.HasFlag(zt)),
                        "ROOM" when VariableSystem.TryParse(Arguments[2], out RoomType room, Source, false) => players.Where(plr => plr.CurrentRoom?.Type == room),
                        "USERID" => players.Where(plr => plr.UserId == VariableSystem.ReplaceVariable(Arguments[2], Source, false)),
                        "PLAYERID" => players.Where(plr => plr.Id.ToString() == VariableSystem.ReplaceVariable(Arguments[2], Source, false)),
                        "INV" when VariableSystem.TryParse(Arguments[2], out ItemType item, Source, false) => players.Where(plr => plr.Items.Any(i => i.Type == item)),
                        "INV" when CustomItem.TryGet(Arguments[2], out CustomItem customItem) => players.Where(plr => plr.Items.Any(item => CustomItem.TryGet(item, out CustomItem customItem2) && customItem == customItem2)),
                        "HELDITEM" when VariableSystem.TryParse(Arguments[2], out ItemType item, Source, false) => players.Where(plr => plr.CurrentItem?.Type == item),
                        "HELDITEM" when CustomItem.TryGet(Arguments[2], out CustomItem customItem) => players.Where(plr => CustomItem.TryGet(plr.CurrentItem, out CustomItem customItem2) && customItem == customItem2),

                        "GROUP" => players.Where(plr => plr.GroupName == Arguments[2]),

                        "ISSTAFF" when VariableSystem.ReplaceVariable(Arguments[2].ToUpper()).AsBool() => players.Where(plr => plr.RemoteAdminAccess),
                        "ISSTAFF" when !VariableSystem.ReplaceVariable(Arguments[2].ToUpper()).AsBool() => players.Where(plr => !plr.RemoteAdminAccess),
                        _ => throw new ArgumentException($"The provided value '{Arguments[1]}' is not a valid filter method, or the provided input '{Arguments[2]}' is not valid for the specified filter method."),
                    };
                }

                throw new ArgumentException(ErrorGen.Get(131, Arguments[0]));
            }
        }

        /// <inheritdoc/>
        public string LongDescription => @"The following options are valid mode options:
- USERID
- PLAYERID
- ROLE
- TEAM
- ROOM
- ZONE
- INV
- HELDITEM
- GROUP
- ISSTAFF
Invalid options will result in a script error.";
    }

    public class GetByIndex : IFloatVariable, IPlayerVariable, IArgumentVariable, INeedSourceVariable
    {
        /// <inheritdoc/>
        public string Name => "{INDEXVAR}";

        /// <inheritdoc/>
        public string Description => "Indexes a player variable and gets ONE player at the specified position.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

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
                if (Arguments.Length < 2)
                {
                    throw new ArgumentException(MsgGen.VariableArgCount(Name, "name", "type"));
                }

                if (VariableSystem.TryGetPlayers(Arguments[0], out PlayerCollection players, Source, false))
                {
                    if (!VariableSystem.TryParse(Arguments[1], out int index, Source, false))
                    {
                        throw new ArgumentException(ErrorGen.Get(134, Arguments[1]));
                    }

                    if (index > players.Count() - 1)
                        throw new IndexOutOfRangeException(ErrorGen.Get(135, index));

                    return new List<Player>() { players.ToList()[index] }; // Todo: better solution (yield return didn't work??)
                }

                throw new ArgumentException(ErrorGen.Get(131, Arguments[0]));
            }
        }
    }
}
#pragma warning restore SA1402 // File may only contain a single type