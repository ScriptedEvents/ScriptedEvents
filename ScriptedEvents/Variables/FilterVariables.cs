namespace ScriptedEvents.Variables.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using PlayerRoles;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;
    using UnityEngine;

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
            new Argument("type", typeof(string), "The mode to use to filter. Valid modes: ROLE, ZONE", true),
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
                // Todo: Throw error, not empty enumerable
                if (Arguments.Length < 3) return Enumerable.Empty<Player>();

                string name = Arguments[0].Replace("{", string.Empty).Replace("}", string.Empty);

                var conditionVariable = VariableSystem.GetVariable($"{{{name}}}", Source);
                if (conditionVariable.Item1 is not null && conditionVariable.Item1 is IPlayerVariable playerVariable)
                {
                    switch (Arguments[1].ToString())
                    {
                        case "ROLE" when Enum.TryParse(Arguments[2], true, out RoleTypeId rt):
                            return Player.List.Where(plr => plr.Role.Type == rt);
                        case "TEAM" when Enum.TryParse(Arguments[2], true, out Team team):
                            return Player.List.Where(plr => plr.Role.Team == team);
                        case "ZONE" when Enum.TryParse(Arguments[2], true, out ZoneType zt):
                            return Player.List.Where(plr => plr.Zone.HasFlag(zt));
                        case "ROOM" when Enum.TryParse(Arguments[2], true, out RoomType room):
                            return Player.List.Where(plr => plr.CurrentRoom.Type == room);
                        case "USERID":
                            return Player.List.Where(plr => plr.UserId == Arguments[2]);
                        default:
                            return Enumerable.Empty<Player>();
                    }
                }

                return Enumerable.Empty<Player>();
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
                // Todo: Throw error, not empty enumerable
                if (Arguments.Length < 2) return Enumerable.Empty<Player>();

                string name = Arguments[0].Replace("{", string.Empty).Replace("}", string.Empty);

                var conditionVariable = VariableSystem.GetVariable($"{{{name}}}", Source);
                if (conditionVariable.Item1 is not null && conditionVariable.Item1 is IPlayerVariable playerVariable)
                {
                    int index = -1;

                    if (int.TryParse(Arguments[1], out index))
                    {
                        // yay
                    }
                    else if (VariableSystem.TryGetVariable(Arguments[1], out IConditionVariable var, out _, Source) && var is IFloatVariable floatVar)
                    {
                        index = (int)floatVar.Value;
                    }
                    else
                    {
                        return Enumerable.Empty<Player>();
                    }

                    if (index > playerVariable.Players.Count() - 1)
                        return Enumerable.Empty<Player>();

                    return new List<Player>() { playerVariable.Players.ToList()[index] }; // Todo make pretty
                }

                return Enumerable.Empty<Player>();
            }
        }
    }
}
