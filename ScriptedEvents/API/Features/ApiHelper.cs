namespace ScriptedEvents.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Exiled.API.Features;
    using ScriptedEvents.Actions;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

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
            MainPlugin.ScriptModule.RegisterActions(assembly);
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

            if (MainPlugin.ScriptModule.CustomActions.ContainsKey(name))
            {
                return "The custom action with the provided name already exists!";
            }

            CustomAction custom = new(name, action);
            MainPlugin.ScriptModule.CustomActions.Add(name, custom);
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

            if (!MainPlugin.ScriptModule.CustomActions.ContainsKey(name))
            {
                return "The custom action with the provided name does not exist.";
            }

            MainPlugin.ScriptModule.CustomActions.Remove(name);
            return "Success";
        }

        /// <summary>
        /// Unregisters multiple registered actions at once.
        /// </summary>
        /// <param name="actionNames">A string array of action names.</param>
        /// <returns>A string message representing whether or not the unregister process was successful.</returns>
        public static string UnregisterCustomActions(string[] actionNames)
        {
            if (actionNames is null)
            {
                return "Missing arguments: actionNames.";
            }

            foreach (string name in actionNames)
            {
                string result = UnregisterCustomAction(name);
                if (result is not "Success")
                {
                    return $"Error unregistering '{name}' custom action: {result}";
                }
            }

            return "Success";
        }

        /// <summary>
        /// Gets a list of players using the input string.
        /// </summary>
        /// <param name="input">Input string.</param>
        /// /// <param name="script">Script object.</param>
        /// <param name="max">Maximum amount of players to get. Leave below zero for unlimited.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of players.</returns>
        public static Player[] GetPlayers(string input, Script script, int max = -1)
        {
            ScriptModule.TryGetPlayers(input, max, out PlayerCollection list, script);
            return list.GetInnerList().ToArray();
        }

        /// <summary>
        /// Evaluates a string math equation, replacing all variables in the string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="script">Script object.</param>
        /// <returns>A tuple indicating success and the value.</returns>
        public static Tuple<bool, float> Math(string input, Script script)
        {
            bool success = ConditionHelperV2.TryMath(VariableSystemV2.ReplaceVariables(input, script), out MathResult result);
            return new(success, result.Result);
        }
    }
}
