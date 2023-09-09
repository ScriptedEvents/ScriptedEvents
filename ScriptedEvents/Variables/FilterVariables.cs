namespace ScriptedEvents.Variables.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using PlayerRoles;
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
            new Filter(),
            new GetByIndex(),
        };
    }

    public class Filter : IFloatVariable, IPlayerVariable, IArgumentVariable, INeedSourceVariable
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
            new Argument("name", typeof(string), "The name of the variable to filter.", true),
            new Argument("type", typeof(string), "The mode to use to filter. Valid modes: ROLE, ZONE, TEAM, ROOM, USERID, INV", true),
            new Argument("input", typeof(string), "What to use as the filter (RoleType, ZoneType, etc)", true),
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

                var conditionVariable = VariableSystem.GetVariable(Arguments[0], Source, false);
                if (conditionVariable.Item1 is not null && conditionVariable.Item1 is IPlayerVariable playerVariable)
                {
                    return Arguments[1].ToString() switch
                    {
                        "ROLE" when VariableSystem.TryParse(Arguments[2], out RoleTypeId rt, Source, false) => Player.List.Where(plr => plr.Role.Type == rt),
                        "TEAM" when VariableSystem.TryParse(Arguments[2], out Team team, Source, false) => Player.List.Where(plr => plr.Role.Team == team),
                        "ZONE" when VariableSystem.TryParse(Arguments[2], out ZoneType zt, Source, false) => Player.List.Where(plr => plr.Zone.HasFlag(zt)),
                        "ROOM" when VariableSystem.TryParse(Arguments[2], out RoomType room, Source, false) => Player.List.Where(plr => plr.CurrentRoom.Type == room),
                        "USERID" => Player.List.Where(plr => plr.UserId == Arguments[2]),
                        "INV" when VariableSystem.TryParse(Arguments[2], out ItemType item, Source, false) => Player.List.Where(plr => plr.Items.Any(i => i.Type == item)),
                        "INV" when CustomItem.TryGet(Arguments[2], out CustomItem customItem) => Player.List.Where(plr => plr.Items.Any(item => CustomItem.TryGet(item, out CustomItem customItem2) && customItem == customItem2)),
                        "ISSTAFF" when Arguments[2].ToUpper() == "TRUE" => Player.List.Where(plr => plr.RemoteAdminAccess),
                        "ISSTAFF" when Arguments[2].ToUpper() == "FALSE" => Player.List.Where(plr => !plr.RemoteAdminAccess),
                        _ => throw new ArgumentException($"The provided value '{Arguments[1]}' is not a valid filter method, or the provided input '{Arguments[2]}' is not valid for the specified filter method."),
                    };
                }

                throw new ArgumentException($"The provided value '{Arguments[0]}' is not a valid variable or has no associated players. [Error Code: SE-131]");
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
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; } = new[]
        {
            new Argument("name", typeof(string), "The name of the variable to index.", true),
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

                if (VariableSystem.TryGetVariable(Arguments[0], out IConditionVariable var, out _, Source, false) && var is IPlayerVariable playerVariable)
                {
                    if (!VariableSystem.TryParse(Arguments[1], out int index, Source))
                    {
                        throw new ArgumentException($"The provided value '{Arguments[1]}' is not a valid integer or variable containing an integer. [Error Code: SE-134]");
                    }

                    if (index > playerVariable.Players.Count() - 1)
                        throw new IndexOutOfRangeException($"The provided index '{index}' is greater than the size of the player collection. [Error Code: SE-135]");

                    return new List<Player>() { playerVariable.Players.ToList()[index] }; // Todo make pretty
                }

                throw new ArgumentException($"The provided value '{Arguments[0]}' is not a valid variable or has no associated players. [Error Code: SE-131]");
            }
        }
    }
}
