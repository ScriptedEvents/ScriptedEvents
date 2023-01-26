using System;

namespace ScriptedEvents.API.Helpers
{
    public static class ApiHelper
    {
        // e.g.
        // OnEnabled: GetType().RegisterActions();

        // maybe move ScriptHelper to this file?
        public static void RegisterActions(this Type plugin)
        {
            ScriptHelper.RegisterActions(plugin.Assembly);
        }
    }
}
