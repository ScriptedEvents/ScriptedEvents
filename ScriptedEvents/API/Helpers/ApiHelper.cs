namespace ScriptedEvents.API.Helpers
{
    using System;
    using System.Reflection;

    public static class ApiHelper
    {
        // e.g.
        // OnEnabled: GetType().RegisterActions();

        /// <summary>
        /// Registers all actions defined in the provided <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly to search through.</param>
        public static void RegisterActions(Assembly assembly)
        {
            ScriptHelper.RegisterActions(assembly);
        }

        /// <summary>
        /// Registers all actions defined in the current working assembly.
        /// </summary>
        public static void RegisterActions() => RegisterActions(Assembly.GetCallingAssembly());

        /// <summary>
        /// Registers all actions defined in the assembly that the provided <see cref="Type"/> is located in.
        /// </summary>
        /// <param name="plugin">The <see cref="Type"/> to get the assembly to search through.</param>
        public static void RegisterActions(this Type plugin)
        {
            RegisterActions(plugin.Assembly);
        }
    }
}
