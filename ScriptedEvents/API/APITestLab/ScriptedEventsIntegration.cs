using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Exiled.API.Features;
using Exiled.Loader;

namespace ScriptedEvents.API.APITestLab
{
    /// <summary>
    /// The class for Scripted Events custom action integration.
    /// </summary>
    internal static class ScriptedEventsIntegration
    {
        /// <summary>
        /// Gets the Scripted Events API.
        /// </summary>
        internal static Type API => Loader.GetPlugin("ScriptedEvents")?.Assembly?.GetType("ScriptedEvents.API.Features.ApiHelper");

        /// <summary>
        /// Gets a value indicating whether the Scripted Evetns API is available to be used.
        /// </summary>
        internal static bool CanInvoke => API is not null && AddAction is not null && RemoveAction is not null && APIGetPlayersMethod is not null;

        /// <summary>
        /// Gets the MethodInfo for checking if the ScriptModule was loaded.
        /// </summary>
        internal static MethodInfo ModuleLoadedMethod => API?.GetMethod("IsModuleLoaded");

        /// <summary>
        /// Gets a value indicating whether the ScriptModule of Scripted Events is loaded.
        /// </summary>
        internal static bool IsModuleLoaded => (bool)ModuleLoadedMethod.Invoke(null, Array.Empty<object>());

        /// <summary>
        /// Gets the MethodInfo for adding a custom action.
        /// </summary>
        internal static MethodInfo AddAction => API?.GetMethod("RegisterCustomAction");

        /// <summary>
        /// Gets the MethodInfo for removing a custom action.
        /// </summary>
        internal static MethodInfo RemoveAction => API?.GetMethod("UnregisterCustomAction");

        /// <summary>
        /// Gets the MethodInfo for removing a custom action.
        /// </summary>
        internal static MethodInfo APIGetPlayersMethod => API?.GetMethod("GetPlayers");

        /// <summary>
        /// Gets a list of custom actions registered.
        /// </summary>
        internal static List<string> CustomActions { get; } = new();

#pragma warning disable SA1629 // Documentation text should end with a period
        /// <summary>
        /// Registers a custom action.
        /// </summary>
        /// <param name="name">The name of the action.</param>
        /// <param name="action">The action implementation.</param>
        /// <remarks>
        /// Action implementation is Func<string[], Tuple<bool, string, object[]>>, where:
        ///
        ///   Tuple<string[], object> - the action input, where:
        ///     string[]   - The input to the action. Usually represented by single word strings, BUT can also include multiple words in one string.
        ///     object     - The script in which the action was ran.
        ///
        ///   Tuple<bool, string, object[]> - the action result, where:
        ///     bool       - Did action execute without any errors.
        ///     string     - The action response to the console when there was an error..
        ///     object[]   - optional values to return from an action, either strings or Player[]s, anything different will result in an error.
        /// </remarks>
#pragma warning restore SA1629 // Documentation text should end with a period

        public static void RegisterCustomAction(string name, Func<Tuple<string[], object>, Tuple<bool, string, object[]>> action)
        {
            try
            {
                AddAction.Invoke(null, new object[] { name, action });
                CustomActions.Add(name);
            }
            catch (Exception e)
            {
                Log.Error($"{e.Source} - {e.GetType().FullName} error: {e.Message}");
            }
        }

        /// <summary>
        /// Registers custom actions defined in the method.
        /// Used when plugin is enabled.
        /// </summary>
        public static async void RegisterCustomActions()
        {
            if (!CanInvoke)
            {
                Log.Warn("[Scripted Events integration] Scripted Events is either not present or outdated. Ignore this message if you're not using Scripted Events.");
                return;
            }

            int tries = 0;
            while (!IsModuleLoaded)
            {
                tries++;
                Log.Debug("[Scripted Events integration] Scripted Events is present, but ScriptModule is not yet loaded; retrying in 1s");
                await Task.Delay(1000);

                if (tries > 10)
                {
                    Log.Error("[Scripted Events integration] ScriptModule has not initialized in time; custom actions will not be added.");
                    return;
                }
            }

            // actions are defined here
            RegisterCustomAction("GET_ALL_PLAYERS", (Tuple<string[], object> input) =>
            {
                /*
                * true - action executed successfully
                * string.Empty - error response (no error so empty)
                * new[] { Player.List.ToArray() } - object[] containing Player[] containing all players
                */
                return new(true, string.Empty, new[] { Player.List.ToArray() });
            });

            RegisterCustomAction("EXPLODE_RANDOM_PLAYER", (Tuple<string[], object> input) =>
            {
                string[] args = input.Item1;
                object script = input.Item2;

                if (args.Length < 1)
                {
                    /*
                    * false - action failed
                    * "Players to explode were not provided." - error response
                    * null - no return values
                    */
                    return new(false, "Players to explode were not provided.", null);
                }

                Player toExplode = GetPlayers(args[0], script, 1).FirstOrDefault();

                if (toExplode is null)
                {
                    /*
                    * false - action failed
                    * "Invalid player variable provided." - error response
                    * null - no return values
                    */
                    return new(false, "Invalid player variable provided.", null);
                }

                toExplode.Explode();

                /*
                * true - action executed successfully
                * string.Empty - error response (no error so empty)
                * new[] { new[] { toExplode } } - object[] containing Player[] containing one player who got exploded
                */
                return new(true, string.Empty, new[] { new[] { toExplode } });
            });

            RegisterCustomAction("FLIP_A_COIN", (Tuple<string[], object> input) =>
            {
                string value = UnityEngine.Random.Range(1, 3) == 1 ? "HEADS" : "TAILS";
                /*
                * true - action executed successfully
                * string.Empty - error response (no error so empty)
                * new[] { value } - object[] containing string containing the coinflip result
                */
                return new(true, string.Empty, new[] { value });
            });
        }

        /// <summary>
        /// Registers custom actions defined previously.
        /// Used when plugin is disabled.
        /// </summary>
        public static void UnregisterCustomActions()
        {
            if (!CanInvoke) return;

            foreach (string name in CustomActions)
                RemoveAction.Invoke(null, new object[] { name });
        }

        /// <summary>
        /// Gets the MethodInfo for getting the players from a variable.
        /// </summary>
        /// <param name="input">The input to process.</param>
        /// <param name="script">The script as object.</param>
        /// <param name="max">The number of players to return (-1 for unlimited).</param>
        /// <returns>The list of players.</returns>
        internal static Player[] GetPlayers(string input, object script, int max = -1)
        {
            return (Player[])APIGetPlayersMethod.Invoke(null, new[] { input, script, max });
        }
    }
}