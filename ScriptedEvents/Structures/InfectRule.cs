namespace ScriptedEvents.Structures
{
    using PlayerRoles;

    public readonly struct InfectRule
    {
        public InfectRule(RoleTypeId oldRole, RoleTypeId newRole, bool movePlayer)
        {
            OldRole = oldRole;
            NewRole = newRole;
            MovePlayer = movePlayer;
        }

        public RoleTypeId OldRole { get; }

        public RoleTypeId NewRole { get; }

        public bool MovePlayer { get; }
    }
}
