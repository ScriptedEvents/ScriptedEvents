using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;

using Random = UnityEngine.Random;

namespace ScriptedEvents.Variables
{
    public static class ConditionVariables
    {

        internal static Dictionary<string, object> DefinedVariables { get; } = new();

        public static void DefineVariable(string name, object input)
        {
            if (!DefinedVariables.ContainsKey(name))
            {
                DefinedVariables.Add(name, input);
                return;
            }
            DefinedVariables[name] = input;
        }

        public static void RemoveVariable(string name)
        {
            if (DefinedVariables.ContainsKey(name))
                DefinedVariables.Remove(name);
        }

        public static void ClearVariables()
        {
            DefinedVariables.Clear();
        }

        // Useful method so that we don't have to add .ToString() on the end of literally everything
        public static string Replace(this string input, string oldValue, object newValue) => input.Replace(oldValue, newValue.ToString());

        // Replacer
        public static string ReplaceVariables(string input)
        {
            input = input
                    // Bools
                    .Replace("{CASSIESPEAKING}", Cassie.IsSpeaking)
                    .Replace("{!CASSIESPEAKING}", !Cassie.IsSpeaking)

                    .Replace("{DECONTAMINATED}", Map.IsLczDecontaminated)
                    .Replace("{!DECONTAMINATED}", !Map.IsLczDecontaminated)

                    .Replace("{ROUNDENDED}", Round.IsEnded)
                    .Replace("{!ROUNDENDED}", !Round.IsEnded)

                    .Replace("{ROUNDINPROGRESS}", Round.InProgress)
                    .Replace("{!ROUNDINPROGRESS}", !Round.InProgress)

                    .Replace("{ROUNDSTARTED}", Round.IsStarted)
                    .Replace("{!ROUNDSTARTED}", !Round.IsStarted)

                    .Replace("{SCP914ACTIVE}", Exiled.API.Features.Scp914.IsWorking)
                    .Replace("{!SCP914ACTIVE}", !Exiled.API.Features.Scp914.IsWorking)

                    .Replace("{WARHEADCOUNTING}", Warhead.IsInProgress)
                    .Replace("{!WARHEADCOUNTING}", !Warhead.IsInProgress)

                    .Replace("{WARHEADDETONATED}", Warhead.IsDetonated)
                    .Replace("{!WARHEADDETONATED}", !Warhead.IsDetonated)

                    .Replace("{WAVERESPAWNING}", MainPlugin.Handlers.IsRespawning)
                    .Replace("{!WAVERESPAWNING}", !MainPlugin.Handlers.IsRespawning)

                    // Floats
                    //-- CHANCE
                    .Replace("{CHANCE}", Random.value)
                    .Replace("{CHANCE3}", Random.Range(1, 4))
                    .Replace("{CHANCE5}", Random.Range(1, 6))
                    .Replace("{CHANCE10}", Random.Range(1, 11))
                    .Replace("{CHANCE20}", Random.Range(1, 21))
                    .Replace("{CHANCE100}", Random.Range(1, 101))

                    //-- WORLD TIME
                    .Replace("{DAYOFWEEK}", ((int)DateTime.UtcNow.DayOfWeek) + 1)
                    .Replace("{DAYOFMONTH}", DateTime.UtcNow.Day)
                    .Replace("{DAYOFYEAR}", DateTime.UtcNow.DayOfYear)
                    .Replace("{MONTH}", DateTime.UtcNow.Month)
                    .Replace("{YEAR}", DateTime.UtcNow.Year)

                    //-- ZONE COUNT
                    .Replace("{LCZ}", Player.Get(p => p.Zone.HasFlag(ZoneType.LightContainment)).Count())
                    .Replace("{HCZ}", Player.Get(p => p.Zone.HasFlag(ZoneType.HeavyContainment)).Count())
                    .Replace("{EZ}", Player.Get(p => p.Zone.HasFlag(ZoneType.Entrance)).Count())
                    .Replace("{SURFACE}", Player.Get(p => p.Zone.HasFlag(ZoneType.Surface)).Count())
                    .Replace("{POCKET}", Player.Get(p => p.CurrentRoom?.Type is RoomType.Pocket).Count())

                    //-- ROLE COUNT
                    //.Replace("{CLASSD}", Player.Get(RoleTypeId.ClassD).Count())
                    .Replace("{CI}", Player.Get(Team.ChaosInsurgency).Count())
                    .Replace("{GUARDS}", Player.Get(RoleTypeId.FacilityGuard).Count())
                    .Replace("{MTF}", Player.Get(Team.FoundationForces).Count() - Player.Get(RoleTypeId.FacilityGuard).Count())
                    .Replace("{MTFANDGUARDS}", Player.Get(Team.FoundationForces).Count())
                    //.Replace("{SCIENTISTS}", Player.Get(RoleTypeId.Scientist).Count())
                    .Replace("{SCPS}", Player.Get(Side.Scp).Count())
                    .Replace("{SH}", Player.Get(player => player.SessionVariables.ContainsKey("IsSH")))
                    .Replace("{UIU}", Player.Get(player => player.SessionVariables.ContainsKey("IsUIU")))

                    //-- PLAYER COUNT
                    .Replace("{PLAYERSALIVE}", Player.Get(ply => ply.IsAlive).Count())
                    .Replace("{PLAYERSDEAD}", Player.Get(ply => ply.IsDead).Count())
                    .Replace("{PLAYERS}", Player.List.Count())

                    //-- KILLS AND DEATHS
                    .Replace("{KILLS}", Round.Kills)
                    .Replace("{SCPKILLS}", Round.KillsByScp)

                    //-- ESCAPES
                    .Replace("{CLASSDESCAPES}", Round.EscapedDClasses)
                    .Replace("{SCIENTISTSCAPES}", Round.EscapedScientists)
                    .Replace("{ESCAPES}", Round.EscapedDClasses + Round.EscapedScientists)

                    //-- ROUND TIME
                    .Replace("{ROUNDMINUTES}", Round.ElapsedTime.TotalMinutes)
                    .Replace("{ROUNDSECONDS}", Round.ElapsedTime.TotalSeconds)

                    //-- TICKETS & RESPAWNS
                    .Replace("{NTFTICKETS}", Respawn.NtfTickets)
                    .Replace("{CHAOSTICKETS}", Respawn.ChaosTickets)
                    .Replace("{TOTALWAVES}", MainPlugin.Handlers.RespawnWaves)
                    .Replace("{TIMEUNTILNEXTWAVE}", Respawn.TimeUntilSpawnWave.TotalSeconds)
                    .Replace("{TIMESINCELASTWAVE}", MainPlugin.Handlers.TimeSinceWave.TotalSeconds)

                    // Strings
                    .Replace("{NEXTWAVE}", Respawn.NextKnownTeam)
                    ;

            foreach (KeyValuePair<string, object> definedVariable in DefinedVariables)
            {
                input = input.Replace(definedVariable.Key, definedVariable.Value);
            }

            foreach (RoleTypeId rt in (RoleTypeId[])Enum.GetValues(typeof(RoleTypeId)))
            {
                string roleTypeString = rt.ToString().ToUpper();
                input = input.Replace($"{{{roleTypeString}}}", Player.Get(rt).Count());
            }

            return input;
        }
    }
}
