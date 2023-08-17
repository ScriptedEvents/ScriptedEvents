namespace ScriptedEvents.Variables.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
        public string Name => "{ROLEFILTER}";

        /// <inheritdoc/>
        public string Description => "Filters a player variable by a certain RoleType.";

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments { get; } = new[]
        {
            new Argument("name", typeof(string), "The name of the variable to filter.", true),
            new Argument("roleType", typeof(RoleTypeId), "The RoleTypeId to sort.", true),
        };

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players
        {
            get
            {
                if (Arguments.Length < 2) return Enumerable.Empty<Player>();
                if (!Enum.TryParse(Arguments[1], out RoleTypeId roleType)) return Enumerable.Empty<Player>();

                string name = Arguments[0].Replace("{", string.Empty).Replace("}", string.Empty);

                var conditionVariable = VariableSystem.GetVariable($"{{{name}}}", null); // Todo support per script variables
                if (conditionVariable.Item1 is not null && conditionVariable.Item1 is IPlayerVariable playerVariable)
                {
                    return playerVariable.Players.Where(ply => ply.Role.Type == roleType);
                }

                return Enumerable.Empty<Player>();
            }
        }
    }
}
