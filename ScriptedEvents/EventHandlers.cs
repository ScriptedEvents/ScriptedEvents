using Exiled.API.Features;
using MEC;
using System.IO;

namespace ScriptedEvents
{
    public class EventHandlers
    {
        public void OnRestarting()
        {
            foreach (var kvp in ScriptHelper.RunningScripts)
            {
                Timing.KillCoroutines(kvp.Value);
            }
            ScriptHelper.RunningScripts.Clear();
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
                catch (FileNotFoundException _)
                {
                    Log.Warn($"The '{name}' script is set to run each round, but the script is not found!");
                }
            }
        }
    }
}
