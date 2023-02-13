﻿namespace ScriptedEvents.API.Helpers
{
    using System;
    using System.Reflection;

    using Exiled.API.Features;
    using ScriptedEvents.Actions;

    /// <summary>
    /// A set of tools for other plugins to add actions to Scripted Events.
    /// </summary>
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

        /// <summary>
        /// Registers a custom action.
        /// </summary>
        /// <param name="name">The name of the action.</param>
        /// <param name="action">The function to execute when the action is used. An array string of parameters is provided, and the action must return a tuple with a bool (successful?) and a string message (optional, can be set to string.Empty).</param>
        /// <returns>A string message representing whether or not the unregister process was successful.</returns>
        public static string RegisterCustomAction(string name, Func<string[], Tuple<bool, string>> action)
        {
            if (name is null || action is null)
            {
                return "Missing arguments: name and action.";
            }

            name = name.ToUpper();

            if (ScriptHelper.CustomActions.ContainsKey(name))
            {
                return "The custom action with the provided name already exists!";
            }

            CustomAction custom = new(name, action);
            ScriptHelper.CustomActions.Add(name, custom);
            Log.Info($"Assembly '{Assembly.GetCallingAssembly().GetName().Name}' has registered custom action: '{name}'.");
            return "Success";
        }

        /// <summary>
        /// Unregisters a previously defined action.
        /// </summary>
        /// <param name="name">The name of the action.</param>
        /// <returns>A string message representing whether or not the unregister process was successful.</returns>
        public static string UnregisterCustomAction(string name)
        {
            if (name is null)
            {
                return "Missing arguments: name.";
            }

            name = name.ToUpper();

            if (!ScriptHelper.CustomActions.ContainsKey(name))
            {
                return "The custom action with the provided name does not exist.";
            }

            ScriptHelper.CustomActions.Remove(name);
            return "Success";
        }
    }
}
