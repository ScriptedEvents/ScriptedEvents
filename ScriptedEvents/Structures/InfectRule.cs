using PlayerRoles;

namespace ScriptedEvents.Structures
{
    public readonly struct InfectRule
    {
        public RoleTypeId OldRole { get; }
        public RoleTypeId NewRole { get; }
        public bool MovePlayer { get; }

        public InfectRule(RoleTypeId oldRole, RoleTypeId newRole, bool movePlayer)
        {
            OldRole = oldRole;
            NewRole = newRole;
            MovePlayer = movePlayer;
        }
    }
}
