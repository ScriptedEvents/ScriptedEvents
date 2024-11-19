using System.Collections.Generic;
using Exiled.API.Features;
using ScriptedEvents.API.Modules;

namespace ScriptedEvents.API.Extensions
{
    public static class PlayerExtensions
    {
        public static Dictionary<string, string> PlayerDataVariables(this Player plr)
        {
            if (VariableSystem.PlayerDataVariables.TryGetValue(plr, out var value))
            {
                return value;
            }

            var dict = new Dictionary<string, string>();
            VariableSystem.PlayerDataVariables.Add(plr, dict);
            return dict;
        }
    }
}
