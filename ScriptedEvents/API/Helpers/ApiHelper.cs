using System;

namespace ScriptedEvents.API.Helpers
{
    public static class ApiHelper
    {
        // e.g.
        // OnEnabled: GetType().RegisterActions();

        // maybe move this to ScriptHelper
        public static void RegisterActions(this Type plugin)
        {
            ScriptHelper.RegisterActions(plugin.Assembly);
        }
    }
}
