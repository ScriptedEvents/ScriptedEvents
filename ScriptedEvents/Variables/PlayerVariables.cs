namespace ScriptedEvents.Variables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using PlayerRoles;
    using ScriptedEvents.API.Helpers;

    public static class PlayerVariables
    {
        private static readonly Dictionary<string, RoleTypeId> roleTypeIds = ((RoleTypeId[])Enum.GetValues(typeof(RoleTypeId))).ToDictionary(x => $"{{{x.ToString().ToUpper()}}}", x => x);

        public static Dictionary<string, IEnumerable<Player>> Variables { get; } = new()
        {
            // By role
            { "{GUARDS}", Player.Get(RoleTypeId.FacilityGuard) },
            { "{MTFANDGUARDS}", Player.Get(Team.FoundationForces) },
            { "{SCPS}", Player.Get(Team.SCPs) },
            { "{MTF}", Player.Get(ply => ply.Role.Team is Team.FoundationForces && ply.Role.Type is not RoleTypeId.FacilityGuard) },
            { "{CI}", Player.Get(Team.ChaosInsurgency) },
            { "{SH}", Player.Get(player => player.SessionVariables.ContainsKey("IsSH")) },
            { "{UIU}", Player.Get(player => player.SessionVariables.ContainsKey("IsUIU")) },

            // By zone
            { "{LCZ}", Player.Get(ply => ply.Zone is ZoneType.LightContainment) },
            { "{HCZ}", Player.Get(ply => ply.Zone is ZoneType.HeavyContainment) },
            { "{EZ}", Player.Get(ply => ply.Zone is ZoneType.Entrance) },
            { "{SURFACE}", Player.Get(ply => ply.Zone is ZoneType.Surface) },
            { "{POCKET}", Player.Get(ply => ply.CurrentRoom?.Type is RoomType.Pocket) },
        };

        private static Dictionary<string, IEnumerable<Player>> DefinedVariables { get; } = new();

        public static void DefineVariable(string name, IEnumerable<Player> input)
        {
            name = name.RemoveWhitespace();

            if (!name.StartsWith("{"))
                name = "{" + name;
            if (!name.EndsWith("}"))
                name = name + "}";
            DefinedVariables[name] = input;
        }

        public static void ClearVariables()
        {
            DefinedVariables.Clear();
        }

        public static string[] IsolateVariables(string input)
        {
            List<string> result = ListPool<string>.Pool.Get();
            string[] split = input.Split(' ');

            foreach (string str in split)
            {
                string newStr = str.RemoveWhitespace();
                if (newStr.StartsWith("{") && newStr.EndsWith("}"))
                {
                    result.Add(newStr.ToUpper());
                }
            }

            return ListPool<string>.Pool.ToArrayReturn(result);
        }

        public static IEnumerable<Player> Get(string input)
        {
            input = input.RemoveWhitespace();

            if (Variables.TryGetValue(input, out IEnumerable<Player> result))
                return result;

            if (DefinedVariables.TryGetValue(input, out result))
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
