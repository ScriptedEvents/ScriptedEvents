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

    using ScriptedEvents.Actions.Samples;
    using ScriptedEvents.Actions.Samples.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
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

            if (script.Sender is ServerConsoleSender) IsFile = true;
            if (Arguments.Length > 1 && Arguments[1].ToUpper() == "NOFILE" && script.Sender is ServerConsoleSender) IsFile = false;

            // List Help
            if (Arguments[0].ToUpper() == "LIST")
            {
                StringBuilder sbList = StringBuilderPool.Pool.Get();
                sbList.AppendLine();
                sbList.AppendLine($"List of all actions. For more information on each action, run the HELP <ACTIONNAME> action (or shelp <ACTIONNAME> in the server console).");

                List<IAction> temp = ListPool<IAction>.Pool.Get();
                foreach (KeyValuePair<ActionNameData, Type> kvp in ScriptHelper.ActionTypes)
                {
                    IAction lAction = Activator.CreateInstance(kvp.Value) as IAction;
                    temp.Add(lAction);
                }

                var grouped = temp.GroupBy(a => a.Subgroup);

                foreach (IGrouping<ActionSubgroup, IAction> group in grouped.OrderBy(g => g.Key.Display()))
                {
                    if (group.Count() == 0 || (group.All(act => act is IHiddenAction || act.IsObsolete(out _)) && !MainPlugin.Configs.Debug))
                        continue;

                    sbList.AppendLine();
                    sbList.AppendLine($"== {group.Key.Display()} Actions ==");

                    foreach (IAction lAction in group)
                    {
                        IHelpInfo lhelpInfo = lAction as IHelpInfo;

                        if ((lAction is IHiddenAction && !MainPlugin.Configs.Debug) || lAction.IsObsolete(out _))
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
                var conditionList = VariableSystem.Groups.OrderBy(group => group.GroupName);

                StringBuilder sbList = StringBuilderPool.Pool.Get();
                sbList.AppendLine();
                sbList.AppendLine("=== VARIABLES ===");
                sbList.AppendLine("The following variables can all be used in conditions, such as IF, GOTOIF, WAITUNTIL, etc. Additionally, each RoleType has its own variable (eg. {NTFCAPTAIN}).");
                sbList.AppendLine("An asterisk [*] before the name of a variable indicates it also stores players, which can be used in numerous actions such as SETROLE.");

                foreach (IVariableGroup group in conditionList)
                {
                    sbList.AppendLine($"+ {group.GroupName} +");
                    foreach (IVariable variable in group.Variables.OrderBy(v => v.Name))
                    {
                        sbList.AppendLine($"{(variable is IPlayerVariable ? "[*] " : string.Empty)}{variable.Name} - {variable.Description}");
                    }

                    sbList.AppendLine();
                }

                sbList.AppendLine("+ Script Defined +");
                sbList.AppendLine("These variables are defined by a script. These variables can be used in any script and are erased when the round restarts.");
                if (VariableSystem.DefinedVariables.Count == 0)
                {
                    sbList.AppendLine("None");
                }
                else
                {
                    foreach (var userDefined in VariableSystem.DefinedVariables.OrderBy(v => v.Key))
                    {
                        sbList.AppendLine($"{userDefined.Value.Name}");
                    }

                    foreach (var userDefined2 in VariableSystem.DefinedPlayerVariables.OrderBy(v => v.Key))
                    {
                        sbList.AppendLine($"[*] {userDefined2.Value.Name}");
                    }
                }

                return Display(new(true, StringBuilderPool.Pool.ToStringReturn(sbList)));
            }

            // Action Help
            else if (ScriptHelper.TryGetActionType(Arguments[0].ToUpper(), out Type type))
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
                sb.Append($"Usage: {action.Name}");
                foreach (Argument arg in helpInfo.ExpectedArguments)
                {
                    string[] chars = arg.Required ? new[] { "<", ">" } : new[] { "[", "]" };
                    sb.Append($" {chars[0]}{arg.ArgumentName.ToUpper()}{chars[1]}");
                }

                sb.AppendLine();

                if (action.IsObsolete(out string reason))
                    sb.AppendLine($"** THIS ACTION IS MARKED AS OBSOLETE AND REASON DIRECTIVES SHOULD BE READ BEFORE USING. REASON: {reason} **");

                if (action is ILongDescription longDescription)
                {
                    sb.AppendLine();
                    sb.AppendLine(longDescription.LongDescription);
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

                if (action is ISampleAction sampleAction)
                {
                    Sample[] samples = sampleAction.Samples.Samples;
                    if (samples.Length > 0)
                    {
                        sb.AppendLine();
                        sb.AppendLine("Examples:");
                        sb.AppendLine();

                        foreach (Sample s in samples)
                        {
                            sb.AppendLine($"=== {s.Title} ===");
                            sb.AppendLine(s.Description);
                            sb.AppendLine();
                            sb.AppendLine("```");
                            sb.AppendLine(s.Code);
                            sb.AppendLine("```");
                            sb.AppendLine();
                        }
                    }
                }

                return Display(new(true, StringBuilderPool.Pool.ToStringReturn(sb)));
            }

            // Variable help
            else if (Arguments[0].StartsWith("{") && Arguments[0].EndsWith("}"))
            {
                bool valid = false;

                StringBuilder sb = StringBuilderPool.Pool.Get();
                sb.AppendLine();

                if (VariableSystem.TryGetVariable(Arguments[0].ToUpper(), out IConditionVariable variable, out bool reversed, script))
                {
                    valid = true;
                    sb.AppendLine("=== VARIABLE ===");
                    sb.AppendLine($"Name: {variable.Name}");
                    sb.AppendLine($"Description: {variable.Description}");
                    sb.AppendLine($"Stores Players: {(variable is IPlayerVariable ? "YES" : "NO")}");

                    if (variable is IArgumentVariable argSupport1)
                    {
                        sb.AppendLine($"Usage: {variable.Name.Substring(0, variable.Name.Length - 1)}:{string.Join(":", argSupport1.ExpectedArguments.Select(arg => arg.ArgumentName.ToUpper()))}}}");
                    }
                    else
                    {
                        sb.AppendLine($"Usage: {variable.Name}");
                    }

                    sb.Append("Variable Type: ");

                    switch (variable)
                    {
                        case IBoolVariable @bool:
                            bool value = reversed ? !@bool.Value : @bool.Value;
                            sb.AppendLine("Boolean (true/false)");

                            // sb.AppendLine($"Current Value: {(value ? "TRUE" : "FALSE")}");
                            break;
                        case ILongVariable @long:
                        case IFloatVariable @float:
                            sb.AppendLine("Numerical");

                            // sb.AppendLine($"Current Value: {@float.Value}");
                            break;
                        case IStringVariable @string:
                            sb.AppendLine("String (Text)");

                            // sb.AppendLine($"Current Value: {@string.Value}");
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

                    if (variable is ILongDescription longDescription)
                    {
                        sb.AppendLine(longDescription.LongDescription);
                    }
                }

                if (!valid)
                {
                    return new(false, "Invalid variable provided for the HELP action.");
                }

                return Display(new(true, StringBuilderPool.Pool.ToStringReturn(sb)));
            }

            // Error Codes
            if (Arguments[0].StartsWith("SE-"))
                Arguments[0] = Arguments[0].Replace("SE-", string.Empty);

            if (int.TryParse(Arguments[0], out int res) && ErrorGen.TryGetError(res, out ErrorInfo info))
            {
                StringBuilder sb = StringBuilderPool.Pool.Get();
                sb.AppendLine();
                sb.AppendLine($"=== ERROR CODE: SE-{res} ===");
                sb.AppendLine($"ID: {info.Id}");
                sb.AppendLine(info.Info);
                sb.AppendLine(info.LongDescription);

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
                    Log.Warn(ErrorGen.Get(114));
                    return new(false, "HELP action error shown in server logs.");
                }
                catch (Exception e)
                {
                    Log.Warn($"{ErrorGen.Get(115)}: {e}");
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
