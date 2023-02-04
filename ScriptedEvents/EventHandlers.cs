using Exiled.API.Features;
using ScriptedEvents.API.Helpers;
using MEC;
using System.IO;
using ScriptedEvents.API.Features;
using ScriptedEvents.API.Features.Exceptions;
using Exiled.Events.EventArgs.Server;
using System;
using ScriptedEvents.Handlers.Variables;

namespace ScriptedEvents
{
    public class EventHandlers
    {
        public int RespawnWaves = 0;
        public DateTime LastRespawnWave = DateTime.MinValue;

        public TimeSpan TimeSinceWave => DateTime.UtcNow - LastRespawnWave;
        public bool IsRespawning => TimeSinceWave.TotalSeconds < 5;


        public void OnRestarting()
        {
            RespawnWaves = 0;
            ScriptHelper.StopAllScripts();
            ConditionVariables.ClearVariables();
            PlayerVariables.ClearVariables();
        }

        public void OnRoundStarted()
        {
            foreach (string name in MainPlugin.Singleton.Config.AutoRunScripts)
            {
                try
                {
                    Script scr = ScriptHelper.ReadScript(name);
                    ScriptHelper.RunScript(scr);
                }
                catch (DisabledScriptException)
                {
                    Log.Warn($"The '{name}' script is set to run each round, but the script is disabled!");
                }
                catch (FileNotFoundException)
                {
                    Log.Warn($"The '{name}' script is set to run each round, but the script is not found!");
                }
            }
        }

        public void OnRespawningTeam(RespawningTeamEventArgs ev)
        {
            if (ev.IsAllowed) return;

            RespawnWaves++;
            LastRespawnWave = DateTime.UtcNow;

            ConditionVariables.DefineVariable("{LASTRESPAWNTEAM}", ev.NextKnownTeam.ToString());
            PlayerVariables.DefineVariable("{RESPAWNEDPLAYERS}", ev.Players);
        }
    }
}
