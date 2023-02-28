namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using MEC;
    using PlayerRoles.PlayableScps.HUDs;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Condition;
    using ScriptedEvents.Variables.Handlers;
    using ScriptedEvents.Variables.Interfaces;

    public class HelpAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "HELP";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <inheritdoc/>
        public string Description => "Gets information about a command or a variable, or lists all commands or variables.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("input", typeof(string), "The name of the action/variable, \"LIST\" for all actions, or \"LISTVAR\" for all variables. Case-sensitive.", true),
        };

        /// <summary>
        /// Gets or sets a value indicating whether or not the response will be opened via file.
        /// </summary>
        public bool IsFile { get; set; } = false;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            IsFile = false;

            if (Arguments.Length > 1 && Arguments[1].ToUpper() == "FILE" && script.Sender is ServerConsoleSender) IsFile = true;

            // List Help
            if (Arguments[0].ToUpper() == "LIST")
            {
                StringBuilder sbList = StringBuilderPool.Pool.Get();
                sbList.AppendLine();
                sbList.AppendLine($"List of all actions.");

                List<IAction> temp = ListPool<IAction>.Pool.Get();
                foreach (KeyValuePair<string, Type> kvp in ScriptHelper.ActionTypes)
                {
                    IAction lAction = Activator.CreateInstance(kvp.Value) as IAction;
                    temp.Add(lAction);
                }

                var grouped = temp.GroupBy(a => a.Subgroup);

                foreach (IGrouping<ActionSubgroup, IAction> group in grouped.OrderBy(g => g.Key.Display()))
                {
                    if (group.Count() == 0 || (group.All(act => act is IHiddenAction) && !MainPlugin.Configs.Debug))
                        continue;

                    sbList.AppendLine();
                    sbList.AppendLine($"== {group.Key.Display()} Actions ==");

                    foreach (IAction lAction in group)
                    {
                        IHelpInfo lhelpInfo = lAction as IHelpInfo;

                        if (lAction is IHiddenAction && !MainPlugin.Configs.Debug)
                            continue;

                        sbList.AppendLine($"{lAction.Name} : {lhelpInfo?.Description ?? "No Description"}");
                    }
                }

                ListPool<IAction>.Pool.Return(temp);
                return Display(new(true, StringBuilderPool.Pool.ToStringReturn(sbList)));
            }

            // List Variables
            if (Arguments[0].ToUpper() is "LISTVAR" or "VARLIST")
            {
                var conditionList = ConditionVariables.Groups.Where(g => g.GroupType is VariableGroupType.Condition).OrderBy(group => group.GroupName);
                var playerList = PlayerVariables.Groups.Where(g => g.GroupType is VariableGroupType.Player).OrderBy(group => group.GroupName);

                StringBuilder sbList = StringBuilderPool.Pool.Get();
                sbList.AppendLine();
                sbList.AppendLine("=== CONDITION VARIABLES ===");
                sbList.AppendLine("The following variables can all be used in conditions, such as IF, GOTOIF, WAITUNTIL, etc. Additionally, each RoleType has its own variable (eg. {NTFCAPTAIN}).");

                foreach (IVariableGroup group in conditionList)
                {
                    sbList.AppendLine($"+ {group.GroupName} +");
                    foreach (IVariable variable in group.Variables.OrderBy(v => v.Name))
                    {
                        sbList.AppendLine($"{variable.Name} - {variable.Description}");
                    }

                    sbList.AppendLine();
                }

                sbList.AppendLine("+ User Defined +");
                if (ConditionVariables.DefinedVariables.Count == 0)
                {
                    sbList.AppendLine("None");
                }
                else
                {
                    foreach (var userDefined in ConditionVariables.DefinedVariables.OrderBy(v => v.Key))
                    {
                        sbList.AppendLine($"{userDefined.Value.Name}");
                    }
                }

                sbList.AppendLine();
                sbList.AppendLine("=== PLAYER VARIABLES ===");
                sbList.AppendLine("The following variables can all be used in player parameters, such as SETROLE, TESLA PLAYERS, etc. Additionally, each RoleType has its own variable (eg. {NTFCAPTAIN}).");

                foreach (IVariableGroup group in playerList)
                {
                    sbList.AppendLine($"+ {group.GroupName} +");
                    foreach (IVariable variable in group.Variables.OrderBy(v => v.Name))
                    {
                        sbList.AppendLine($"{variable.Name} - {variable.Description}");
                    }

                    sbList.AppendLine();
                }

                sbList.AppendLine("+ User Defined +");
                if (PlayerVariables.DefinedVariables.Count == 0)
                {
                    sbList.AppendLine("None");
                }
                else
                {
                    foreach (var userDefined in PlayerVariables.DefinedVariables.OrderBy(v => v.Key))
                    {
                        sbList.AppendLine($"{userDefined.Value.Name}");
                    }
                }

                return Display(new(true, StringBuilderPool.Pool.ToStringReturn(sbList)));
            }

            // Action Help
            else if (ScriptHelper.ActionTypes.TryGetValue(Arguments[0], out Type type))
            {
                IAction action = Activator.CreateInstance(type) as IAction;

                if (action is not IHelpInfo helpInfo)
                    return new(false, "The command provided is not supported in the HELP action.");

                StringBuilder sb = StringBuilderPool.Pool.Get();

                if (helpInfo.ExpectedArguments.Length > 0)
                {
                    sb.AppendLine();
                }

                sb.AppendLine($"+ {action.Name} +");
                sb.AppendLine($"{helpInfo.Description}");
                sb.AppendLine($"Action type: {MsgGen.Display(action.Subgroup)}");

                // Usage
                sb.Append($"USAGE: {action.Name}");
                foreach (Argument arg in helpInfo.ExpectedArguments)
                {
                    string[] chars = arg.Required ? new[] { "<", ">" } : new[] { "[", "]" };
                    sb.Append($" {chars[0]}{arg.ArgumentName.ToUpper()}{chars[1]}");
                }

                sb.AppendLine();

                if (helpInfo.ExpectedArguments.Length > 0)
                {
                    sb.AppendLine();
                    sb.Append("Arguments:");
                }

                foreach (Argument arg in helpInfo.ExpectedArguments)
                {
                    string[] chars = arg.Required ? new[] { "<", ">" } : new[] { "[", "]" };
                    sb.AppendLine();
                    sb.AppendLine($"{chars[0]}{arg.ArgumentName}{chars[1]}");
                    sb.AppendLine($"  Required: {(arg.Required ? "YES" : "NO")}");
                    sb.AppendLine($"  Type: {arg.TypeString}");
                    sb.AppendLine($"  {arg.Description}");
                }

                return Display(new(true, StringBuilderPool.Pool.ToStringReturn(sb)));
            }

            // Variable help
            else if (Arguments[0].StartsWith("{") && Arguments[0].EndsWith("}"))
            {
                bool valid = false;

                StringBuilder sb = StringBuilderPool.Pool.Get();
                sb.AppendLine();

                if (ConditionVariables.TryGetVariable(Arguments[0], out IConditionVariable variable, out bool reversed))
                {
                    valid = true;
                    sb.AppendLine("=== CONDITION VARIABLE ===");
                    sb.AppendLine($"Name: {variable.Name}");
                    sb.AppendLine($"Description: {variable.Description}");

                    switch (variable)
                    {
                        case IBoolVariable @bool:
                            bool value = reversed ? !@bool.Value : @bool.Value;
                            sb.AppendLine($"Current Value: {(value ? "TRUE" : "FALSE")}");
                            break;
                        case IFloatVariable @float:
                            sb.AppendLine($"Current Value: {@float.Value}");
                            break;
                        case IStringVariable @string:
                            sb.AppendLine($"Current Value: {@string.Value}");
                            break;
                        case CustomVariable custom:
                            sb.AppendLine($"Current Value: {custom.Value}");
                            break;
                    }

                    if (variable is IArgumentVariable argSupport)
                    {
                        sb.AppendLine();
                        sb.AppendLine("Arguments:");

                        foreach (Argument arg in argSupport.ExpectedArguments)
                        {
                            string[] chars = arg.Required ? new[] { "<", ">" } : new[] { "[", "]" };
                            sb.AppendLine();
                            sb.AppendLine($"{chars[0]}{arg.ArgumentName}{chars[1]}");
                            sb.AppendLine($"  Required: {(arg.Required ? "YES" : "NO")}");
                            sb.AppendLine($"  Type: {arg.TypeString}");
                            sb.AppendLine($"  {arg.Description}");
                        }
                    }

                    sb.AppendLine();
                }

                if (PlayerVariables.TryGetVariable(Arguments[0], out IPlayerVariable playerVariable))
                {
                    valid = true;
                    sb.AppendLine($"=== PLAYER VARIABLE ===");
                    sb.AppendLine($"Name: {playerVariable.Name}");
                    sb.AppendLine($"Description: {playerVariable.Description}");
                    sb.AppendLine($"Current Value: {(playerVariable.Players.Count() == 0 ? "[None]" : string.Join(", ", playerVariable.Players.Select(ply => ply.Nickname)))}");

                    if (variable is IArgumentVariable argSupport)
                    {
                        sb.AppendLine();
                        sb.AppendLine("Arguments:");

                        foreach (Argument arg in argSupport.ExpectedArguments)
                        {
                            string[] chars = arg.Required ? new[] { "<", ">" } : new[] { "[", "]" };
                            sb.AppendLine();
                            sb.AppendLine($"{chars[0]}{arg.ArgumentName}{chars[1]}");
                            sb.AppendLine($"  Required: {(arg.Required ? "YES" : "NO")}");
                            sb.AppendLine($"  Type: {arg.TypeString}");
                            sb.AppendLine($"  {arg.Description}");
                        }
                    }
                }

                if (!valid)
                {
                    return new(false, "Invalid variable provided for the HELP action.");
                }

                return Display(new(true, StringBuilderPool.Pool.ToStringReturn(sb)));
            }

            // Nope
            return new(false, "Invalid argument provided for the HELP action.");
        }

        /// <summary>
        /// Displays the response, either in console or in file.
        /// </summary>
        /// <param name="response">The response of the command execution.</param>
        /// <returns>A new response to show to the user.</returns>
        public ActionResponse Display(ActionResponse response)
        {
            if (IsFile)
            {
                string message = $"!-- HELPRESPONSE\nAuto Generated At: {DateTime.UtcNow:f}\nExpires: {DateTime.UtcNow.AddMinutes(5):f}\n{response.Message}";
                string path = Path.Combine(ScriptHelper.ScriptPath, "HelpCommandResponse.txt");

                if (File.Exists(path))
                    File.Delete(path);

                try
                {
                    File.WriteAllText(path, message);
                }
                catch (UnauthorizedAccessException)
                {
                    Log.Warn($"Unable to create the help file, the plugin does not have permission to access the ScriptedEvents directory!");
                    return new(false, "HELP action error shown in server logs.");
                }
                catch (Exception e)
                {
                    Log.Warn($"Error when writing to file: {e}");
                    return new(false, "HELP action error shown in server logs.");
                }

                // File "expire"
                Timing.CallDelayed(300f, Segment.RealtimeUpdate, () =>
                {
                    if (File.Exists(path) && File.ReadAllText(path) == message)
                    {
                        Log.Debug("Deleting auto-generated help file.");
                        File.Delete(path);
                    }
                });

                // Set file attributes
                FileInfo info = new(path)
                {
                    Attributes = FileAttributes.Temporary,
                };

                try
                {
                    System.Diagnostics.Process.Start(path);
                }
                catch (Exception)
                {
                    return new(true, $"File was created successfully, but an error occurred when opening external text editor (likely due to permissions). File will expire in 5 minutes and is located at: {path}.");
                }

                return new(true, $"Opened help in external text editor. Expires in 5 minutes (if the console is left open). Path: {path}");
            }
            else
            {
                return response;
            }
        }
    }
}
