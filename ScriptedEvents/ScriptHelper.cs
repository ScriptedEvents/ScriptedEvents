using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using ScriptedEvents.Actions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ScriptedEvents
{
    public static class ScriptHelper
    {
        public static void Setup()
        {
            foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(IAction).IsAssignableFrom(type) && type.IsClass && type.GetConstructors().Length > 0)
                {
                    IAction temp = (IAction)Activator.CreateInstance(type);
                    ActionTypes.Add(temp.Name, type);
                }
            }
        }

        public static readonly string ScriptPath = Path.Combine(Paths.Configs, "ScriptedEvents");

        public static Dictionary<string, Type> ActionTypes { get; } = new();

        public static string ReadScriptText(string scriptName)
            => File.ReadAllText(Path.Combine(ScriptPath, scriptName + ".txt"));

        public static Script ReadScript(string scriptName)
        {
            Script script = new();
            string allText = ReadScriptText(scriptName);
            string[] sections = allText.Split(new string[] { "--" }, StringSplitOptions.RemoveEmptyEntries);

            if (sections.Length < 4)
                throw new InvalidOperationException("Script is invalid. Missing appropriate sections.");

            string configSection = sections[1].Trim();
            string actionsSections = sections[3].Trim();

            foreach (string config in configSection.Split('\n'))
            {
                string[] configParts = config.Split(':');
                if (configParts.Length < 2)
                {
                    Log.Info($"Malformed config '{configParts}' detected in script '{scriptName}'");
                    continue;
                }
                script.Configuration.Add(configParts[0], configParts[1]);
            }

            foreach (string action in actionsSections.Split('\n'))
            {
                string[] actionParts = action.Split(' ');
                ActionTypes.TryGetValue(actionParts[0], out Type actionType);
                if (actionType is null)
                {
                    Log.Info($"Invalid action '{actionParts[0]}' detected in script '{scriptName}'");
                    continue;
                }
                IAction newAction = Activator.CreateInstance(actionType) as IAction;
                newAction.Arguments = actionParts.Skip(1).ToArray();

                script.Actions.Enqueue(newAction);
            }

            script.ScriptName = scriptName;


            return script;
        }

        public static void RunScript(Script scr) => Timing.RunCoroutine(RunScriptInternal(scr));

        public static IEnumerator<float> RunScriptInternal(Script scr)
        {
            Log.Info($"Running script {scr.ScriptName}.");

            while (true)
            {
                if (scr.Actions.TryDequeue(out IAction action))
                {
                    ActionResponse resp;
                    if (action is ITimingAction timed)
                    {
                        yield return timed.GetDelay(out resp);
                    }
                    else
                    {
                        resp = action.Execute();
                    }

                    if (!resp.Success)
                    {
                        if (resp.Fatal)
                        {
                            Log.Error($"Fatal action error! {resp.Message}");
                            break;
                        }
                        else
                        {
                            Log.Warn($"Action error! {resp.Message}");
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            Log.Info($"Finished running script {scr.ScriptName}.");
        }

        // Convert number or number range to a number
        public static bool TryConvertNumber(string number, out int result)
        {
            if (int.TryParse(number, out result))
            {
                return true;
            }

            var dashSplit = number.Split('-');
            if (dashSplit.Length == 2 && int.TryParse(dashSplit[0], out int min) && int.TryParse(dashSplit[1], out int max))
            {
                result = Random.Range(min, max+1);
                return true;
            }

            return false;
        }

        public static bool TryGetPlayers(string input, out List<Player> plys)
        {
            if (input is "*")
            {
                plys = Player.List.ToList();
                return true;
            }

            plys = null;
            return false;
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
            return doors.Count > 0;
        }
    }
}
