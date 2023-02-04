using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Handlers.Variables
{
    public static class PlayerVariables
    {
        public static Dictionary<string, IEnumerable<Player>> Variables { get; } = new()
        {
            // By role
            { "{CLASSD}", Player.Get(Team.ClassD) },
            { "{SCIENTISTS}", Player.Get(Team.Scientists) },
            { "{GUARDS}", Player.Get(RoleTypeId.FacilityGuard) },
            { "{SCPS}", Player.Get(Team.SCPs) },
            { "{MTF}", Player.Get(ply => ply.Role.Team is Team.FoundationForces && ply.Role.Type is not RoleTypeId.FacilityGuard) },
            { "{CI}", Player.Get(Team.ChaosInsurgency) },
            { "{SH}", Player.Get(player => player.SessionVariables.ContainsKey("IsSH")) },
            { "{UIU}", Player.Get(player => player.SessionVariables.ContainsKey("IsUIU")) },

            { "{LCZ}", Player.Get(ply => ply.Zone is ZoneType.LightContainment) },
            { "{HCZ}", Player.Get(ply => ply.Zone is ZoneType.HeavyContainment) },
            { "{EZ}", Player.Get(ply => ply.Zone is ZoneType.Entrance) },
            { "{SURFACE}", Player.Get(ply => ply.Zone is ZoneType.Surface) },
        };

        public static string[] IsolateVariables(string input)
        {
            var result = new List<string>();
            var split = input.Split(' ');

            foreach (string str in split)
            {
                if (Variables.Any(kvp => kvp.Key == str))
                    result.Add(str);
            }

            return result.ToArray();
        }

        public static IEnumerable<Player> Get(string input)
        {
            foreach (var variable in Variables)
            {
                if (variable.Key.Equals(input))
                {
                    return variable.Value;
                }
            }

            return null;
        }

        public static bool TryGet(string input, out IEnumerable<Player> players)
        {
            players = Get(input);
            return players != null;
        }
    }
}
