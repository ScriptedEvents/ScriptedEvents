namespace ScriptedEvents.Variables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using PlayerRoles;
    using Random = UnityEngine.Random;

    /// <summary>
    /// A class used to store and retrieve all non-player variables.
    /// </summary>
    public static class ConditionVariables
    {
        /// <summary>
        /// Gets a <see cref="Dictionary{TKey, TValue}"/> of variables that were defined in run-time.
        /// </summary>
        internal static Dictionary<string, object> DefinedVariables { get; } = new();

        /// <summary>
        /// Defines a variable.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <param name="input">The value of the variable.</param>
        /// <remarks>Curly braces will be added automatically if they are not present already.</remarks>
        public static void DefineVariable(string name, object input)
        {
            if (!DefinedVariables.ContainsKey(name))
            {
                DefinedVariables.Add(name, input);
                return;
            }

            DefinedVariables[name] = input;
        }

        /// <summary>
        /// Removes a previously-defined variable.
        /// </summary>
        /// <param name="name">The name of the variable, with curly braces.</param>
        public static void RemoveVariable(string name)
        {
            if (DefinedVariables.ContainsKey(name))
                DefinedVariables.Remove(name);
        }

        /// <summary>
        /// Removes all defined variables.
        /// </summary>
        public static void ClearVariables()
        {
            DefinedVariables.Clear();
        }

        /// <summary>
        /// Alternative to <see cref="string.Replace(string, string)"/> which takes an object as the newValue (and ToStrings it automatically).
        /// </summary>
        /// <param name="input">The string to perform the replacement on.</param>
        /// <param name="oldValue">The string to look for.</param>
        /// <param name="newValue">The value to replace it with.</param>
        /// <returns>The modified string.</returns>
        public static string Replace(this string input, string oldValue, object newValue) => input.Replace(oldValue, newValue.ToString());

        /// <summary>
        /// Replaces all the occurrences of variables in a string.
        /// </summary>
        /// <param name="input">The string to perform the replacements on.</param>
        /// <returns>The modified string.</returns>
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
                    .Replace("{CI}", Player.Get(Team.ChaosInsurgency).Count())
                    .Replace("{GUARDS}", Player.Get(RoleTypeId.FacilityGuard).Count())
                    .Replace("{MTF}", Player.Get(Team.FoundationForces).Count() - Player.Get(RoleTypeId.FacilityGuard).Count())
                    .Replace("{MTFANDGUARDS}", Player.Get(Team.FoundationForces).Count())
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
