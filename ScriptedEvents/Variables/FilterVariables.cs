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

    public class FilterVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Filters";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new RoleFilter(),
        };
    }

    public class RoleFilter : IFloatVariable, IPlayerVariable, IArgumentVariable
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
        public IEnumerable<Player> Players
        {
            get
            {
                // Todo: Throw error, not empty enumerable
                if (Arguments.Length < 3) return Enumerable.Empty<Player>();

                string name = Arguments[0].Replace("{", string.Empty).Replace("}", string.Empty);

                var conditionVariable = VariableSystem.GetVariable($"{{{name}}}", null); // Todo support per script variables
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
                        default:
                            return Enumerable.Empty<Player>();
                    }
                }

                return Enumerable.Empty<Player>();
            }
        }
    }
}
