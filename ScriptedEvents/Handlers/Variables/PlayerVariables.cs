using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using ScriptedEvents.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScriptedEvents.Handlers.Variables
{
    public static class PlayerVariables
    {
        private static Dictionary<string, IEnumerable<Player>> definedVariables { get; } = new();
        private static readonly Dictionary<string, RoleTypeId> roleTypeIds = ((RoleTypeId[])Enum.GetValues(typeof(RoleTypeId))).ToDictionary(x => $"{{{x.ToString().ToUpper()}}}", x => x);
        public static void DefineVariable(string name, IEnumerable<Player> input)
        {
            name = name.RemoveWhitespace();

            if (!name.StartsWith("{"))
                name = "{" + name;
            if (!name.EndsWith("}"))
                name = name + "}";
            definedVariables[name] = input;
        }

        public static void ClearVariables()
        {
            definedVariables.Clear();
        }

        public static Dictionary<string, IEnumerable<Player>> Variables { get; } = new()
        {
            // By role
            //{ "{CLASSD}", Player.Get(Team.ClassD) },
            //{ "{SCIENTISTS}", Player.Get(Team.Scientists) },
            { "{GUARDS}", Player.Get(RoleTypeId.FacilityGuard) },
            { "{MTFANDGUARDS}", Player.Get(Team.FoundationForces) },
            { "{SCPS}", Player.Get(Team.SCPs) },
            { "{MTF}", Player.Get(ply => ply.Role.Team is Team.FoundationForces && ply.Role.Type is not RoleTypeId.FacilityGuard) },
            { "{CI}", Player.Get(Team.ChaosInsurgency) },
            { "{SH}", Player.Get(player => player.SessionVariables.ContainsKey("IsSH")) },
            { "{UIU}", Player.Get(player => player.SessionVariables.ContainsKey("IsUIU")) },

            { "{LCZ}", Player.Get(ply => ply.Zone is ZoneType.LightContainment) },
            { "{HCZ}", Player.Get(ply => ply.Zone is ZoneType.HeavyContainment) },
            { "{EZ}", Player.Get(ply => ply.Zone is ZoneType.Entrance) },
            { "{SURFACE}", Player.Get(ply => ply.Zone is ZoneType.Surface) },
            { "{POCKET}", Player.Get(ply => ply.CurrentRoom?.Type is RoomType.Pocket) },
        };

        public static string[] IsolateVariables(string input)
        {
            var result = new List<string>();
            var split = input.Split(' ');

            foreach (string str in split)
            {
                string newStr = str.RemoveWhitespace();
                if (newStr.StartsWith("{") && newStr.EndsWith("}"))
                {
                    result.Add(newStr.ToUpper());
                }
            }

            return result.ToArray();
        }

        public static IEnumerable<Player> Get(string input)
        {
            input = input.RemoveWhitespace();

            if (Variables.TryGetValue(input, out IEnumerable<Player> result))
                return result;

            if (definedVariables.TryGetValue(input, out result))
                return result;

            if (roleTypeIds.TryGetValue(input, out RoleTypeId rt))
                return Player.Get(rt);
            return null;
        }

        public static bool TryGet(string input, out IEnumerable<Player> players)
        {
            players = Get(input);
            return players != null;
        }
    }
}
