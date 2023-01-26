using MEC;
using ScriptedEvents.API.Helpers;

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
    }
}
