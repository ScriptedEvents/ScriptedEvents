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
    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class HelpAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "HELP";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <inheritdoc/>
        public string Description => "Gets information about a command or a variable, or lists all commands or variables.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("input", typeof(string), "The name of the action/variable, \"LIST\" for all actions, or \"LISTVAR\" for all variables. Case-sensitive.", false),
        };

        /// <summary>
        /// Gets or sets a value indicating whether or not the response will be opened via file.
        /// </summary>
        public bool IsFile { get; set; } = false;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1)
                Arguments = new[] { "USE-WELCOME-TEXT" };

            if (Arguments[0] is "GENERATE")
            {
                if (script.Context != ExecuteContext.ServerConsole)
                    return new(false, "The 'GENERATE' subcommand must be executed from the server console.");
                bool generatorResult = API.Features.ScriptHelpGenerator.Generator.GenerateConfig(out string message);
                return new(generatorResult, message);
            }

            if (Arguments[0] is "GDONE")
            {
                if (script.Context != ExecuteContext.ServerConsole)
                    return new(false, "The 'GDONE' subcommand must be executed from the server console.");
                bool generatorResult = API.Features.ScriptHelpGenerator.Generator.CreateDocumentation(out string message);
                return new(generatorResult, message);
            }

            if (Arguments[0] is "SHOW")
            {
                if (script.Context != ExecuteContext.ServerConsole)
                    return new(false, "The 'SHOW' subcommand must be executed from the server console.");
                bool generatorResult = API.Features.ScriptHelpGenerator.Generator.PromptDocFolder(out string message);
                return new(generatorResult, message);
            }

            if (script.Sender is ServerConsoleSender) IsFile = true;
            if (Arguments.Length > 1 && Arguments[1].ToUpper() == "NOFILE" && script.Sender is ServerConsoleSender) IsFile = false;

            ActionResponse text = GenerateText(Arguments[0].ToUpper(), script, IsFile);
            return Display(text);
        }

        public static ActionResponse GenerateText(string text, Script script = null, bool nofile = false)
        {
            if (text is "USE-WELCOME-TEXT")
            {
                string newText = $@"
Welcome to the HELP command! This command is your one-stop shop for all {MainPlugin.Singleton.Name} documentation and information.

The most powerful feature of this command is the ability to generate all of SE's documentation directly onto your computer.
To do so, run 'shelp GENERATE' (must be done in the server console). This will open a config file. Complete this file and then run 'shelp GDONE' to create documentation.
From there, using 'shelp SHOW' in the future will open the documentation folder directly on your computer!
Whenever SE updates, re-run 'shelp GENERATE' to ensure your documentation remains updated.

If you don't want to generate documentation on your computer, please use one of the following options to continue instead:
- 'LIST' - Lists all available actions.
- 'LISTVAR' - Lists all available variables.
- 'LISTENUM' - Lists all enums that are used in {MainPlugin.Singleton.Name}, and show more info about enums.
- An error code returned from an error message (example: 'SE-101').

Every value returned from those lists can also be used in shelp to retrieve documentation about specific features. For example, 'TESLA' will give you more information about the TESLA action.
Struggling? Join our Discord server for immediate and useful assistance: {MainPlugin.DiscordUrl}
Interested in helping others learn the plugin? Let {MainPlugin.Singleton.Author} know on Discord. We'd be happy to have you on our team of volunteer helpers!
Thanks for using our plugin. <3

Scripted Events Contributors:
{Constants.GenerateContributorList('-')}";
                return new(true, newText);
            }

            // List Help
            if (text is "LIST" or "ACTIONS" or "ACTS")
            {
                StringBuilder sbList = StringBuilderPool.Pool.Get();
                sbList.AppendLine();
                sbList.AppendLine($"List of all actions. For more information on each action, run the HELP <ACTIONNAME> action (or shelp <ACTIONNAME> in the server console).");

                List<IAction> temp = ListPool<IAction>.Pool.Get();
                foreach (KeyValuePair<ActionNameData, Type> kvp in MainPlugin.ScriptModule.ActionTypes)
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
                return new(true, StringBuilderPool.Pool.ToStringReturn(sbList));
            }

            // List Variables
            if (text is "LISTVAR" or "VARLIST" or "VARIABLES" or "VARS")
            {
                var conditionList = VariableSystemV2.Groups.OrderBy(group => group.GroupName);

                StringBuilder sbList = StringBuilderPool.Pool.Get();
                sbList.AppendLine();
                sbList.AppendLine("=== VARIABLES ===");
                sbList.AppendLine("The following variables can all be used in conditions, such as IF, GOTOIF, WAITUNTIL, etc. Additionally, each RoleType has its own variable (eg. {NTFCAPTAIN}).");
                sbList.AppendLine("An asterisk [*] before the name of a variable indicates it also stores players, which can be used in numerous actions such as SETROLE.");
                sbList.AppendLine();

                foreach (IVariableGroup group in conditionList)
                {
                    sbList.AppendLine($"+ {group.GroupName} +");
                    foreach (IVariable variable in group.Variables.OrderBy(v => v.Name))
                    {
                        sbList.AppendLine($"{(variable is IPlayerVariable ? "[*] " : string.Empty)}{variable.Name} - {variable.Description}");
                    }

                    sbList.AppendLine();
                }

                return new(true, StringBuilderPool.Pool.ToStringReturn(sbList));
            }

            // List Enums
            else if (text is "LISTENUM" or "ENUMLIST")
            {
                StringBuilder sbEnumList = StringBuilderPool.Pool.Get();
                sbEnumList.AppendLine();
                sbEnumList.AppendLine("=== ENUMS ===");
                sbEnumList.AppendLine("Enums are, at the basic level, 'options' in code. Different 'Enum' inputs are required for various actions, such as roles, items, etc.");
                sbEnumList.AppendLine("Type 'shelp <enum name>' to see each different enum's valid options.");
                sbEnumList.AppendLine();

                foreach (EnumDefinition def in EnumDefinitions.Definitions)
                {
                    sbEnumList.AppendLine($"{def.EnumType.Name} - {def.Description}");
                }

                return new(true, StringBuilderPool.Pool.ToStringReturn(sbEnumList));
            }

            // Action Help
            else if (MainPlugin.ScriptModule.TryGetActionType(text, out Type type))
            {
                IAction action = Activator.CreateInstance(type) as IAction;

                if (action is not IHelpInfo helpInfo)
                    return new(false, "The command provided is not supported in the HELP action.");

                StringBuilder sb = StringBuilderPool.Pool.Get();

                if (action.ExpectedArguments is not null && action.ExpectedArguments.Length > 0)
                {
                    sb.AppendLine();
                }

                sb.AppendLine($"+ {action.Name} +");
                sb.AppendLine($"{helpInfo.Description}");
                sb.AppendLine($"Action type: {MsgGen.Display(action.Subgroup)}");

                // Usage
                if (action.ExpectedArguments is not null)
                {
                    sb.Append($"Usage: {action.Name}");
                    foreach (Argument arg in action.ExpectedArguments)
                    {
                        string[] chars = arg.Required ? new[] { "<", ">" } : new[] { "[", "]" };
                        sb.Append($" {chars[0]}{arg.ArgumentName.ToUpper()}{chars[1]}");
                    }
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

                if (action.ExpectedArguments is not null)
                {
                    if (action.ExpectedArguments.Length > 0)
                    {
                        sb.AppendLine();
                        sb.Append("Arguments:");
                    }

                    foreach (Argument arg in action.ExpectedArguments)
                    {
                        string[] chars = arg.Required ? new[] { "<", ">" } : new[] { "[", "]" };
                        sb.AppendLine();
                        sb.AppendLine($"{chars[0]}{arg.ArgumentName}{chars[1]}");
                        sb.AppendLine($"  Required: {(arg.Required ? "YES" : "NO")}");
                        sb.AppendLine($"  Type: {arg.TypeString}");
                        sb.AppendLine($"  {arg.Description}");

                        if (arg is OptionsArgument options)
                        {
                            sb.AppendLine($"  Valid options for this argument:");
                            foreach (Option option in options.Options)
                            {
                                if (option.Description is null)
                                    sb.AppendLine($"    - '{option.Name}'");
                                else
                                    sb.AppendLine($"    - '{option.Name}' - {option.Description}");
                            }
                        }
                    }
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

                return new(true, StringBuilderPool.Pool.ToStringReturn(sb));
            }

            // Variable help
            else if (text.StartsWith("{") && text.EndsWith("}"))
            {
                bool valid = false;

                StringBuilder sb = StringBuilderPool.Pool.Get();
                sb.AppendLine();

                if (VariableSystemV2.TryGetVariable(text, script, out VariableResult res2, skipProcessing: true))
                {
                    valid = true;
                    IConditionVariable variable = res2.Variable;

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
                            sb.AppendLine("Boolean (true/false)");
                            break;
                        case ILongVariable @long:
                        case IFloatVariable @float:
                            sb.AppendLine("Numerical");
                            break;
                        case IStringVariable @string:
                            sb.AppendLine("String (Text)");
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

                            if (arg is OptionsArgument options)
                            {
                                sb.AppendLine($"  Valid options for this argument:");
                                foreach (Option option in options.Options)
                                {
                                    if (option.Description is null)
                                        sb.AppendLine($"    - '{option.Name}'");
                                    else
                                        sb.AppendLine($"    - '{option.Name}' - {option.Description}");
                                }
                            }
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

                return new(true, StringBuilderPool.Pool.ToStringReturn(sb));
            }

            // Enum help
            EnumDefinition match = EnumDefinitions.Definitions.FirstOrDefault(def => def.EnumType.Name.ToUpper() == text);
            if (match is not null)
            {
                StringBuilder sbEnum = StringBuilderPool.Pool.Get();
                sbEnum.AppendLine();
                sbEnum.AppendLine($"=== ENUM ===");
                sbEnum.AppendLine($"Name: {match.EnumType.Name}");
                sbEnum.AppendLine(match.Description);
                if (!string.IsNullOrWhiteSpace(match.LongDescription))
                    sbEnum.AppendLine(match.LongDescription);

                sbEnum.AppendLine();
                sbEnum.AppendLine("Options:");
                foreach (object item in match.ObjectItems)
                {
                    int n = Convert.ToInt32(item);
                    sbEnum.AppendLine($"{n} - {item}");
                }

                return new(true, StringBuilderPool.Pool.ToStringReturn(sbEnum));
            }

            // Error Codes
            if (text.StartsWith("SE-"))
                text = text.Replace("SE-", string.Empty);

            if (int.TryParse(text, out int res) && ErrorGen.TryGetError(res, out ErrorInfo info))
            {
                StringBuilder sb = StringBuilderPool.Pool.Get();
                sb.AppendLine();
                sb.AppendLine($"=== ERROR CODE ===");
                sb.AppendLine($"ID: SE-{info.Id}");

                if (script is not null && script.Debug)
                    sb.AppendLine($"Internal ID: {info.Code}");

                sb.AppendLine(info.Info);
                sb.AppendLine(info.LongDescription);

                return new(true, StringBuilderPool.Pool.ToStringReturn(sb));
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
                string message = $"Auto Generated At: {DateTime.UtcNow:f}\nExpires: {DateTime.UtcNow.AddMinutes(5):f}\n{response.Message}";
                string path = Path.Combine(MainPlugin.BaseFilePath, "HelpCommandResponse.txt");

                if (File.Exists(path))
                    File.Delete(path);

                try
                {
                    File.WriteAllText(path, message);
                }
                catch (UnauthorizedAccessException)
                {
                    Log.Warn(ErrorGen.Get(ErrorCode.IOHelpPermissionError));
                    return new(false, "HELP action error shown in server logs.");
                }
                catch (Exception e)
                {
                    Log.Warn($"{ErrorGen.Get(ErrorCode.IOHelpError)}: {e}");
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
                    Attributes = FileAttributes.Temporary | FileAttributes.Hidden,
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
