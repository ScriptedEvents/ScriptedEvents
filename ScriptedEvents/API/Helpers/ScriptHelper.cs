using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ScriptedEvents.API.Features;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Features.Aliases;
using ScriptedEvents.API.Features.Exceptions;
using ScriptedEvents.Handlers.Variables;

using Random = UnityEngine.Random;
using ScriptedEvents.Handlers.DefaultActions;

namespace ScriptedEvents.API.Helpers
{
    public static class ScriptHelper
    {
        internal static void RegisterActions(Assembly assembly)
        {
            int i = 0;
            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IAction).IsAssignableFrom(type) && type.IsClass && type.GetConstructors().Length > 0)
                {
                    IAction temp = (IAction)Activator.CreateInstance(type);

                    Log.Debug($"Adding Action: {temp.Name} | From Assembly: {assembly.GetName().Name}");
                    ActionTypes.Add(temp.Name, type);
                    i++;
                }
            }

            MainPlugin.Info($"Assembly '{assembly.GetName().Name}' has registered {i} actions.");
        }

        public static readonly string ScriptPath = Path.Combine(Paths.Configs, "ScriptedEvents");

        public static Dictionary<string, Type> ActionTypes { get; } = new();
        public static Dictionary<Script, CoroutineHandle> RunningScripts { get; } = new();

        public static string ReadScriptText(string scriptName)
            => File.ReadAllText(Path.Combine(ScriptPath, scriptName + ".txt"));

        public static Script ReadScript(string scriptName)
        {
            Script script = new();
            string allText = ReadScriptText(scriptName);

            string[] array = allText.Split('\n');
            for (int currentline = 0; currentline < array.Length; currentline++)
            {
                // NoAction
                string action = array[currentline];
                if (string.IsNullOrWhiteSpace(action) || action.StartsWith("#"))
                {
                    script.Actions.Add(new NullAction());
                    continue;
                }

                if (action.StartsWith("!--"))
                {
                    string flag = action.Substring(3).RemoveWhitespace();
                    script.Flags.Add(flag);

                    script.Actions.Add(new NullAction());
                    continue;
                }

                string[] actionParts = action.Split(' ');
                string keyword = actionParts[0].RemoveWhitespace();

                // Labels
                if (keyword.EndsWith(":"))
                {
                    script.Labels.Add(action.Remove(keyword.Length - 1, 1), currentline);
                    script.Actions.Add(new NullAction());
                    continue;
                }


                var alias = MainPlugin.Singleton.Config.Aliases.Get(keyword);
                if (alias != null)
                {
                    actionParts = alias.Unalias(action).Split(' ');
                    keyword = actionParts[0].RemoveWhitespace();
                }

#if DEBUG
                Log.Debug($"Queuing action {keyword} {string.Join(", ", actionParts.Skip(1))}");
#endif
                ActionTypes.TryGetValue(keyword, out Type actionType);
                if (actionType is null && alias == null)
                {
                    Log.Info($"Invalid action '{keyword.RemoveWhitespace()}' detected in script '{scriptName}'");
                    script.Actions.Add(new NullAction());
                    continue;
                }

                IAction newAction = Activator.CreateInstance(actionType) as IAction;
                newAction.Arguments = actionParts.Skip(1).Select(str => str.RemoveWhitespace()).ToArray();

                script.Actions.Add(newAction);
            }

            script.ScriptName = scriptName;
            script.RawText = allText;
            return script;
        }

        public static void RunScript(Script scr)
        {
            if (scr.Disabled)
                throw new DisabledScriptException(scr.ScriptName);

            CoroutineHandle handle = Timing.RunCoroutine(RunScriptInternal(scr));
            RunningScripts.Add(scr, handle);
        }

        public static void ReadAndRun(string scriptName)
        {
            Script scr = ReadScript(scriptName);
            if (scr is not null)
                RunScript(scr);
        }

        public static IEnumerator<float> RunScriptInternal(Script scr)
        {
            MainPlugin.Info($"Running script {scr.ScriptName}.");
            scr.IsRunning = true;

            for (; scr.CurrentLine < scr.Actions.Count; scr.CurrentLine++)                                
            {
                if (scr.Actions.TryGet(scr.CurrentLine, out IAction action) && action != null)
                {
                    ActionResponse resp;
                    float? delay = null;

                    try
                    {
                        if (action is ITimingAction timed)
                        {
                            Log.Debug($"Running {action.Name} action...");
                            delay = timed.Execute(scr, out resp);
                        }
                        else if (action is IScriptAction scriptAction)
                        {
                            Log.Debug($"Running {action.Name} action...");
                            resp = scriptAction.Execute(scr);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Ran into an error while running {action.Name} action:\n{e}");
                        continue;
                    }

                    if (!resp.Success)
                    {
                        if (resp.ResponseFlags.HasFlag(ActionFlags.FatalError))
                        {
                            Log.Error($"[{action.Name}] Fatal action error! {resp.Message}");
                            break;
                        }
                        else
                        {
                            Log.Warn($"[{action.Name}] Action error! {resp.Message}");
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(resp.Message))
                            Log.Info($"[{action.Name}] Response: {resp.Message}");
                        if (delay.HasValue)
                            yield return delay.Value;
                    }

                    if (resp.ResponseFlags.HasFlag(ActionFlags.StopEventExecution))
                        break;
                }
            }

            MainPlugin.Info($"Finished running script {scr.ScriptName}.");
            scr.IsRunning = false;

            if (MainPlugin.Singleton.Config.LoopScripts.Contains(scr.ScriptName))
            {
                ReadAndRun(scr.ScriptName); // so that it re-reads the content of the text file.
            }

            RunningScripts.Remove(scr);
        }

        // Convert number or number range to a number
        public static bool TryConvertNumber(string number, out float result)
        {
            if (float.TryParse(number, out result))
            {
                return true;
            }

            var dashSplit = number.Split('-');
            if (dashSplit.Length == 2 && float.TryParse(dashSplit[0], out float min) && float.TryParse(dashSplit[1], out float max))
            {
                result = Random.Range(min, max+1);
                return true;
            }

            return false;
        }

        public static bool TryGetPlayers(string input, int? amount, out List<Player> plys)
        {
            plys = new();
            if (input is "*")
            {
                plys = Player.List.ToList();
                return true;
            }
            else
            {
                string[] variables = PlayerVariables.IsolateVariables(input);
                foreach (string variable in variables)
                {
                    if (PlayerVariables.TryGet(variable, out var playersFromVariable))
                    {
                        plys.AddRange(playersFromVariable);
                    }
                }
                if (Player.TryGet(input, out Player ply))
                {
                    plys.Add(ply);
                }
            }

            plys.ShuffleList();
            plys.RemoveAll(p => !p.IsConnected);

            if (amount.HasValue && amount.Value > 0)
            {
                if (amount.Value < plys.Count)
                {
                    for (int i = 0; i < amount.Value; i++)
                    {
                        plys.PullRandomItem();
                    }
                }
            }

            return plys.Count > 0;
        }

        public static bool TryGetDoors(string input, out List<Door> doors)
        {
            doors = new();
            if (input == "*")
            {
                doors = Door.List.ToList();
            }
            else if (Enum.TryParse<ZoneType>(input, true, out ZoneType zt))
            {
                doors = Door.List.Where(d => d.Zone == zt).ToList();
            }
            else if (Enum.TryParse<DoorType>(input, true, out DoorType dt))
            {
                doors = Door.List.Where(d => d.Type == dt).ToList();
            }
            else if (Enum.TryParse<RoomType>(input, true, out RoomType rt))
            {
                doors = Door.List.Where(d => d.Room?.Type == rt).ToList();
            }
            else
            {
                doors = Door.List.Where(d => d.Name.ToLower() == input.ToLower()).ToList();
            }

            doors = doors.Where(d => d.IsElevator is false && d.Type is not DoorType.Scp079First && d.Type is not DoorType.Scp079Second).ToList();
            return doors.Count > 0;
        }

        public static int StopAllScripts()
        {
            int amount = 0;
            foreach (var kvp in RunningScripts)
            {
                amount++;
                kvp.Key.IsRunning = false;
                Timing.KillCoroutines(kvp.Value);
            }

            foreach (string key in Handlers.DefaultActions.WaitUntilAction.Coroutines)
            {
                Timing.KillCoroutines(key);
            }

            foreach (string key in Handlers.DefaultActions.WaitUntilDebugAction.Coroutines)
            {
                Timing.KillCoroutines(key);
            }

            Handlers.DefaultActions.WaitUntilAction.Coroutines.Clear();
            Handlers.DefaultActions.WaitUntilDebugAction.Coroutines.Clear();
            RunningScripts.Clear();
            return amount;
        }
    }
}
