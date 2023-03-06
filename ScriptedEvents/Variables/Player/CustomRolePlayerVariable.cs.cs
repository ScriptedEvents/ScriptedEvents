namespace ScriptedEvents.Variables.Player.Roles
{
#pragma warning disable SA1402 // File may only contain a single type
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.CustomRoles.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class CustomRolesVariable : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "CustomRoles";

        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Player;

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new CustomRoleTypeVariable(),
        };
    }

    public class CustomRoleTypeVariable : IPlayerVariable
    {
        public CustomRoleTypeVariable()
        {
            CustomRole = null;
        }

        public CustomRoleTypeVariable(CustomRole customRole)
        {
            CustomRole = customRole;
        }

        public CustomRole CustomRole { get; }

        /// <inheritdoc/>
        public string Name => $"{{{CustomRole.Name}}}";

        /// <inheritdoc/>
        public string Description => $"Gets all the alive {CustomRole.Name}.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => CustomRole.TrackedPlayers;

        public static bool TryGetValue(string name, out CustomRoleTypeVariable customRoleTypeVariable)
        {
            customRoleTypeVariable = null;
            if (!CustomRole.TryGet(name, out CustomRole customRole))
                return false;

            customRoleTypeVariable = new(customRole);
            return true;
        }
    }
}
