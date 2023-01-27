using Exiled.API.Features;
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
            .Replace("PLAYERSALIVE", Player.Get(ply => ply.IsAlive).Count())
            .Replace("PLAYERSDEAD", Player.Get(ply => ply.IsDead).Count())
            .Replace("CHANCE", Random.value)
            ;
    }
}
