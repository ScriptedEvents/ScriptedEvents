using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using System;
using System.Linq;
using UnityEngine;

namespace ScriptedEvents.API.Helpers
{
    public static class ConditionVariables
    {
        // Useful method so that we don't have to add .ToString() on the end of literally everything
        public static string Replace(this string input, string oldValue, object newValue) => input.Replace(oldValue, newValue.ToString());

        // Replacer
        public static string ReplaceVariables(string input) => input
            // Bools
            .Replace("CASSIESPEAKING", Cassie.IsSpeaking)
            .Replace("!CASSIESPEAKING", !Cassie.IsSpeaking)

            .Replace("DECONTAMINATED", Map.IsLczDecontaminated)
            .Replace("!DECONTAMINATED", !Map.IsLczDecontaminated)

            .Replace("ROUNDENDED", Round.IsEnded)
            .Replace("!ROUNDENDED", !Round.IsEnded)

            .Replace("ROUNDINPROGRESS", Round.InProgress)
            .Replace("!ROUNDINPROGRESS", !Round.InProgress)

            .Replace("ROUNDSTARTED", Round.IsStarted)
            .Replace("!ROUNDSTARTED", !Round.IsStarted)

            .Replace("WARHEADCOUNTING", Warhead.IsInProgress)
            .Replace("!WARHEADCOUNTING", !Warhead.IsInProgress)

            .Replace("SCP914ACTIVE", Exiled.API.Features.Scp914.IsWorking)
            .Replace("!SCP914ACTIVE", !Exiled.API.Features.Scp914.IsWorking)

            .Replace("WARHEADDETONATED", Warhead.IsDetonated)
            .Replace("!WARHEADDETONATED", !Warhead.IsDetonated)

            // Floats
            //-- CHANCE
            .Replace("CHANCE", UnityEngine.Random.value)

            //-- WORLD TIME
            .Replace("DAYOFWEEK", DateTime.UtcNow.DayOfWeek)
            .Replace("DAYOFMONTH", DateTime.UtcNow.Day)
            .Replace("DAYOFYEAR", DateTime.UtcNow.DayOfYear)
            .Replace("MONTH", DateTime.UtcNow.Month)
            .Replace("YEAR", DateTime.UtcNow.Year)

            //-- ROLE COUNT
            .Replace("CDP", Player.Get(RoleTypeId.ClassD).Count())
            .Replace("CHI", Player.Get(Team.ChaosInsurgency).Count())
            .Replace("MTF", Player.Get(Team.FoundationForces).Count())
            .Replace("SCPS", Player.Get(Side.Scp).Count())

            //-- PLAYER COUNT
            .Replace("PLAYERSALIVE", Player.Get(ply => ply.IsAlive).Count())
            .Replace("PLAYERSDEAD", Player.Get(ply => ply.IsDead).Count())

            //-- ROUND TIME
            .Replace("ROUNDMINUTES", Round.ElapsedTime.TotalMinutes)
            .Replace("ROUNDSECONDS", Round.ElapsedTime.TotalSeconds)

            //-- TICKETS
            .Replace("NTFTICKETS", Respawn.NtfTickets)
            .Replace("CHAOSTICKETS", Respawn.ChaosTickets)
            ;
    }
}
