using System;
using ScriptedEvents.Enums;

namespace ScriptedEvents.Structures
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;

    using PlayerRoles;

    public class DamageRule
    {
        // Roles
        public DamageRule(RoleTypeId aRole, RoleTypeId rRole)
        {
            Type = DamageRuleType.RoleToRole;
            AttackerRole = aRole;
            ReceiverRole = rRole;
        }

        public DamageRule(RoleTypeId aRole, IEnumerable<Player> rRole)
        {
            Type = DamageRuleType.RoleToPlayer;
            AttackerRole = aRole;
            ReceiverPlayers = rRole.ToList();
        }

        public DamageRule(IEnumerable<Player> aRole, RoleTypeId rRole)
        {
            Type = DamageRuleType.PlayerToRole;
            AttackerPlayers = aRole.ToList();
            ReceiverRole = rRole;
        }

        // Teams
        public DamageRule(Team aTeam, Team rTeam)
        {
            Type = DamageRuleType.TeamToTeam;
            AttackerTeam = aTeam;
            ReceiverTeam = rTeam;
        }

        public DamageRule(Team aTeam, IEnumerable<Player> rTeam)
        {
            Type = DamageRuleType.TeamToPlayer;
            AttackerTeam = aTeam;
            ReceiverPlayers = rTeam.ToList();
        }

        public DamageRule(IEnumerable<Player> aTeam, Team rTeam)
        {
            Type = DamageRuleType.PlayerToTeam;
            AttackerPlayers = aTeam.ToList();
            AttackerTeam = rTeam;
        }

        // Misc
        public DamageRule(RoleTypeId attacker, Team receiver)
        {
            Type = DamageRuleType.RoleToTeam;
            AttackerRole = attacker;
            ReceiverTeam = receiver;
        }

        public DamageRule(Team attacker, RoleTypeId receiver)
        {
            Type = DamageRuleType.TeamToRole;
            AttackerTeam = attacker;
            ReceiverRole = receiver;
        }

        public DamageRule(IEnumerable<Player> attacker, IEnumerable<Player> receiver)
        {
            Type = DamageRuleType.PlayerToPlayer;
            AttackerPlayers = attacker.ToList();
            ReceiverPlayers = receiver.ToList();
        }

        public DamageRule(object attacker, object receiver)
        {
            switch (attacker, receiver)
            {
                case (RoleTypeId aRole, RoleTypeId rRole):
                    AttackerRole = aRole;
                    ReceiverRole = rRole;
                    Type = DamageRuleType.RoleToRole;
                    break;

                case (RoleTypeId aRole, Team rTeam):
                    AttackerRole = aRole;
                    ReceiverTeam = rTeam;
                    Type = DamageRuleType.RoleToTeam;
                    break;

                case (Team aTeam, RoleTypeId rRole):
                    AttackerTeam = aTeam;
                    ReceiverRole = rRole;
                    Type = DamageRuleType.TeamToRole;
                    break;

                case (Team aTeam, Team rTeam):
                    AttackerTeam = aTeam;
                    ReceiverTeam = rTeam;
                    Type = DamageRuleType.TeamToTeam;
                    break;

                case (Player[] aPlayers, RoleTypeId rRole):
                    AttackerPlayers = aPlayers.ToList();
                    ReceiverRole = rRole;
                    Type = DamageRuleType.PlayerToRole;
                    break;

                case (Player[] aPlayers, Team rTeam):
                    AttackerPlayers = aPlayers.ToList();
                    ReceiverTeam = rTeam;
                    Type = DamageRuleType.PlayerToTeam;
                    break;

                case (Player[] aPlayers, Player[] rPlayers):
                    AttackerPlayers = aPlayers.ToList();
                    ReceiverPlayers = rPlayers.ToList();
                    Type = DamageRuleType.PlayerToPlayer;
                    break;

                default:
                    throw new ArgumentException($"Unsupported attacker and receiver types: {attacker.GetType()}, {receiver.GetType()}");
            }
        }


        public DamageRuleType Type { get; }

        public List<Player> AttackerPlayers { get; }

        public List<Player> ReceiverPlayers { get; }

        public RoleTypeId AttackerRole { get; }

        public RoleTypeId ReceiverRole { get; }

        public Team AttackerTeam { get; }

        public Team ReceiverTeam { get; }

        public float Multiplier { get; set; }

        public float DetermineMultiplier(Player attacker, Player receiver)
        {
            if (Type == DamageRuleType.PlayerToRole && AttackerPlayers.Contains(attacker) && ReceiverRole == receiver.Role)
            {
                return Multiplier;
            }
            else if (Type == DamageRuleType.RoleToPlayer && AttackerRole == attacker.Role && ReceiverPlayers.Contains(receiver))
            {
                return Multiplier;
            }
            else if (Type == DamageRuleType.PlayerToTeam && AttackerPlayers.Contains(attacker) && ReceiverTeam == receiver.Role.Team)
            {
                return Multiplier;
            }
            else if (Type == DamageRuleType.TeamToPlayer && AttackerTeam == attacker.Role.Team && ReceiverPlayers.Contains(receiver))
            {
                return Multiplier;
            }
            else if (Type == DamageRuleType.RoleToRole && AttackerRole == attacker.Role && ReceiverRole == receiver.Role)
            {
                return Multiplier;
            }
            else if (Type == DamageRuleType.TeamToTeam && AttackerTeam == attacker.Role.Team && ReceiverTeam == receiver.Role.Team)
            {
                return Multiplier;
            }
            else if (Type == DamageRuleType.RoleToTeam && AttackerRole == attacker.Role && ReceiverTeam == receiver.Role.Team)
            {
                return Multiplier;
            }
            else if (Type == DamageRuleType.TeamToRole && AttackerTeam == attacker.Role.Team && ReceiverRole == receiver.Role)
            {
                return Multiplier;
            }
            else if (Type == DamageRuleType.PlayerToPlayer && AttackerPlayers.Contains(attacker) && ReceiverPlayers.Contains(receiver))
            {
                return Multiplier;
            }

            return 1f;
        }
    }
}
