namespace ScriptedEvents.API.Modules
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using CommandSystem;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;

    using MEC;
    using RemoteAdmin;

    using ScriptedEvents.Actions;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.API.Interfaces;

    using ScriptedEvents.DemoScripts;

    using ScriptedEvents.Structures;

    using Logger = Features.Logger;
    using LogType = Enums.LogType;

    /// <summary>
    /// A helper class to read and execute scripts, and register actions, as well as providing useful API for individual actions.
    /// </summary>
    public class ScriptModule : SEModule
    {
        /// <summary>
        /// The base path to the script folder.
        /// </summary>
        public static readonly string BasePath = Path.Combine(MainPlugin.BaseFilePath, "Scripts");

        /// <summary>
        /// Gets a dictionary of action names and their respective types.
        /// </summary>
        public Dictionary<ActionNameData, Type> ActionTypes { get; } = new();

        /// <summary>
        /// Gets a dictionary of <see cref="Script"/> that are currently running, and the <see cref="CoroutineHandle"/> that is running them.
        /// </summary>
        public Dictionary<Script, CoroutineHandle> RunningScripts { get; } = new();

        /// <summary>
        /// Gets all defined custom actions.
        /// </summary>
        public Dictionary<string, CustomAction> CustomActions { get; } = new();

        /// <summary>
        /// Gets all registered auto run scripts.
        /// </summary>
        public List<string> AutoRunScripts { get; } = new();

        /// <inheritdoc/>
        public override string Name { get; } = "ScriptModule";

        /// <inheritdoc/>
        public override bool ShouldGenerateFiles
            => !Directory.Exists(BasePath);

        /// <summary>
        /// Tries to run the provided action.
        /// </summary>
        /// <param name="scr">The script in which the actione exists.</param>
        /// <param name="action">The action itself.</param>
        /// <param name="actResp">The action response.</param>
        /// <param name="delay">The delay of the action.</param>
        /// <param name="originalActionArgs">Use this if you want to override the argument info about the action found in the script.</param>
        /// <returns>If running the action went properly.</returns>
        public static bool TryRunAction(Script scr, IAction action, out ActionResponse? actResp, out float? delay, string[]? originalActionArgs = null)
        {
            actResp = null;
            delay = null;

            void Log(string message)
            {
                Logger.Debug("[ScriptModule] [TryRunAction] " + message, scr);
            }

            if (action is NullAction nullAction)
            {
                Log($"Null action type: {nullAction.Type}");
                return true;
            }

            if (scr.IfActionBlocksExecution && action is not IIgnoresIfActionBlock)
            {
                Log("Action was skipped; the IF block resulted in FALSE and action does not terminate IF blocks.");
                return true;
            }

            if (originalActionArgs == null)
            {
                // Process Arguments
                if (scr.OriginalActionArgs.TryGetValue(action, out var xxx))
                {
                    originalActionArgs = xxx;
                }
                else
                {
                    Log("Action does not have any arguments provided.");
                }
            }

            ArgumentProcessResult res = ArgumentProcessor.Process(action.ExpectedArguments, originalActionArgs, action, scr, true);
            if (res.Errored)
            {
                var message = (res.FailedArgument != string.Empty ? $"[Argument: {res.FailedArgument}] " : string.Empty) + res.Message;
                actResp = new(false, message);
                return false;
            }

            if (!res.Success)
            {
                Log("Action will not be ran. " + (res.Message ?? string.Empty));
                return true;
            }

            action.Arguments = res.NewParameters.ToArray();
            action.RawArguments = res.StrippedRawParameters;

            switch (action)
            {
                case ITimingAction timed:
                    Log($"Running {action.Name} action (timed)...");
                    delay = timed.Execute(scr, out actResp);
                    break;
                case IScriptAction scriptAction:
                    Log($"Running {action.Name} action...");
                    actResp = scriptAction.Execute(scr);
                    break;
                default:
                    Log($"Action is not runnable.");
                    return true;
            }

            if (delay.HasValue)
            {
                Log($"Action '{action.Name}' is delaying the script. Length of delay: {delay.Value}");
            }

            if (!actResp.Success)
            {
                actResp = new(false, actResp.Message);
                return false;
            }

            Log($"{action.Name} has successfully executed.");
            return true;
        }

        /// <inheritdoc/>
        public override void GenerateFiles()
        {
            base.GenerateFiles();

            try
            {
                DirectoryInfo info = Directory.CreateDirectory(BasePath);
                DirectoryInfo demoScriptFolder = Directory.CreateDirectory(Path.Combine(info.FullName, "DemoScripts"));
                foreach (IDemoScript demo in MainPlugin.DemoScripts)
                {
                    File.WriteAllText(Path.Combine(demoScriptFolder.FullName, $"{demo.FileName}.txt"), demo.Contents);
                }

                File.WriteAllText(Path.Combine(MainPlugin.BaseFilePath, "README.txt"), new About().Contents);
            }
            catch (Exception e)
            {
                Logger.Error(ErrorGenV2.IOError() + $": {e}");
            }
        }

        /// <inheritdoc/>
        public override void Init()
        {
            base.Init();

            RegisterActions(MainPlugin.Singleton.Assembly);
        }

        /// <inheritdoc/>
        public override void Kill()
        {
            base.Kill();
            StopAllScripts();
            ActionTypes.Clear();
        }

        /// <summary>
        /// Registers and runs all autorun defined scripts and saves their names.
        /// </summary>
        /// <param name="allScripts">All NOT DISPOSED scripts available on startup.</param>
        public void RegisterAutorunScripts(IEnumerable<Script> allScripts, out List<Script> autoRunScripts)
        {
            autoRunScripts = new List<Script>();
            foreach (var scr in allScripts)
            {
                if (scr is null) continue;

                if (!scr.HasFlag("AUTORUN"))
                {
                    continue;
                }

                Logger.Debug($"Script '{scr.ScriptName}' set to run automatically.");

                try
                {
                    if (scr.AdminEvent)
                    {
                        Logger.Warn(ErrorGen.Get(ErrorCode.AutoRun_AdminEvent, scr.ScriptName));
                        continue;
                    }

                    RunScript(scr);
                    autoRunScripts.Add(scr);
                    AutoRunScripts.Add(scr.ScriptName);
                }
                catch (DisabledScriptException)
                {
                    Logger.Warn(ErrorGen.Get(ErrorCode.AutoRun_Disabled, scr.ScriptName));
                }
                catch (FileNotFoundException)
                {
                    Logger.Warn(ErrorGen.Get(ErrorCode.AutoRun_NotFound, scr.ScriptName));
                }
            }

            if (ShouldGenerateFiles)
            {
                Logger.Info($"Thank you for installing Scripted Events! View the README file located at {Path.Combine(MainPlugin.BaseFilePath, "README.txt")} for information on how to use and get the most out of this plugin.");
            }
        }

        /// <summary>
        /// Retrieves a list of all scripts in the server.
        /// </summary>
        /// <param name="sender">Optional sender.</param>
        /// <returns>A list of all scripts.</returns>
        /// <remarks>WARNING: Scripts created through this method are NOT DISPOSED!!! Call <see cref="Script.Dispose"/> when done with them.</remarks>
        public List<Script> ListScripts(ICommandSender? sender = null)
        {
            List<Script> scripts = new();
            string[] files = Directory.GetFiles(BasePath, "*.txt", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                try
                {
                    Script scr = ReadScript(Path.GetFileNameWithoutExtension(file), sender, true);
                    scripts.Add(scr);
                }
                catch (Exception e)
                {
                    Logger.Error(e.ToString());
                }
            }

            return scripts;
        }

        /// <summary>
        /// Returns an action type, if its name or aliases match the input.
        /// </summary>
        /// <param name="name">The name of the action.</param>
        /// <returns>Whether or not the try-get was successful.</returns>
        public Type? TryGetActionType(string name)
        {
            name = name.ToUpper();
            return ActionTypes
                .Where(actionData =>
                    actionData.Key.Name == name ||
                    actionData.Key.Aliases.Contains(name))
                .Select(actionData => actionData.Value)
                .FirstOrDefault();
        }

        /// <summary>
        /// Reads a script line-by-line, converting every line into an appropriate action, flag, label, etc. Fills out all data and returns a <see cref="Script"/> object.
        /// </summary>
        /// <param name="scriptName">The name of the script.</param>
        /// <param name="executor">The CommandSender that ran the script. Can be null.</param>
        /// <param name="suppressWarnings">Do not show warnings in the console.</param>
        /// <returns>The <see cref="Script"/> fully filled out, if the script was found.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the script is not found.</exception>
        public Script ReadScript(string scriptName, ICommandSender? executor, bool suppressWarnings = false)
        {
            var allText = ReadScriptText(scriptName);
            var inMultilineComment = false;
            Script script = new();
            var scriptPath = GetFilePath(scriptName);

            if (scriptPath is null)
            {
                throw new Exception($"Script '{scriptName}' could not be found.");
            }

            // Fill out script data
            if (MainPlugin.Singleton.Config.RequiredPermissions.TryGetValue(scriptName, out var perm2))
            {
                script.ReadPermission += $".{perm2}";
                script.ExecutePermission += $".{perm2}";
            }

            script.ScriptName = scriptName;
            script.RawText = allText;
            script.FilePath = scriptPath;
            script.LastRead = File.GetLastAccessTimeUtc(scriptPath);
            script.LastEdited = File.GetLastWriteTimeUtc(scriptPath);

            script.Context = executor switch
            {
                null => ExecuteContext.Automatic,
                ServerConsoleSender _ => ExecuteContext.ServerConsole,
                PlayerCommandSender _ => ExecuteContext.RemoteAdmin,
                _ => script.Context
            };

            script.Sender = executor;
            var actionList = ListPool<IAction>.Pool.Get();
            var array = allText.Split('\n');

            IAction? lastAction = null;

            for (var currentline = 0; currentline < array.Length; currentline++)
            {
                array[currentline] = array[currentline].Trim().Replace("\n", string.Empty).Replace("\r", string.Empty);

                // no action
                var line = array[currentline];
                if (string.IsNullOrWhiteSpace(line))
                {
                    AddActionNoArgs(new NullAction("BLANK LINE"));
                    continue;
                }

                if (line.StartsWith("##"))
                {
                    inMultilineComment = !inMultilineComment;
                    AddActionNoArgs(new NullAction("MULTI COMMENT"));
                    continue;
                }

                if (line.StartsWith("#") || inMultilineComment)
                {
                    AddActionNoArgs(new NullAction("COMMENT"));
                    continue;
                }

                var structureParts = ListPool<string>.Pool.Get();
                structureParts.AddRange(line.Split(' ').Where(str => !string.IsNullOrWhiteSpace(str)));
                var keyword = structureParts[0].RemoveWhitespace();

                switch (keyword)
                {
                    // function labels
                    case "->" when structureParts.Count < 2:
                        Logger.ScriptError($"A function label syntax has been used, but no name has been provided.", script, printableLine: currentline + 1);
                        continue;
                    case "->":
                    {
                        var labelName = structureParts[1].RemoveWhitespace();

                        if (!script.FunctionLabels.ContainsKey(labelName))
                            script.FunctionLabels.Add(labelName, currentline);
                        else if (!suppressWarnings)
                            Logger.Warn(ErrorGen.Get(ErrorCode.MultipleLabelDefs, labelName, scriptName));

                        AddActionNoArgs(new StartFunctionAction());
                        continue;
                    }

                    // smart args
                    case "//" when actionList.Count == 0 || lastAction is null:
                        Logger.Log("'//' (smart argument) syntax can't be used if there isn't any action above it.", LogType.Warning, script, currentline + 1);
                        continue;
                    case "//":
                    {
                        var value = string.Join(" ", structureParts.Skip(1)).Trim();

                        if (script.SmartArguments.ContainsKey(lastAction))
                        {
                            script.SmartArguments[lastAction] = script.SmartArguments[lastAction].Append(Lambda).ToArray();
                        }
                        else
                        {
                            script.SmartArguments[lastAction] = new Func<Tuple<bool, string>>[] { Lambda };
                        }

                        AddActionNoArgs(new NullAction($"SMART ARG"));
                        continue;

                        Tuple<bool, string> Lambda()
                        {
                            return new(true, Parser.ReplaceContaminatedValueSyntax(value, script));
                        }
                    }

                    case "//::" when actionList.Count == 0 || lastAction is null:
                        Logger.Log("'//::' (smart extractor) syntax can't be used if there isn't any action above it.", LogType.Warning, script, currentline + 1);
                        continue;

                    case "//::":
                    {
                        var actionName = structureParts[1];
                        var actionArgs = structureParts.Skip(2).ToArray();

                        // TODO: implement for external actions
                        var extractorActType = TryGetActionType(actionName);
                        if (extractorActType is null)
                        {
                            Logger.Warn(ErrorGen.Get(ErrorCode.InvalidAction, actionName, scriptName), script);
                            continue;
                        }

                        if (Activator.CreateInstance(extractorActType) is not IAction actionToExtract)
                        {
                            Logger.Warn(ErrorGen.Get(ErrorCode.InvalidAction, actionName, scriptName), script);
                            continue;
                        }

                        if (actionToExtract is ITimingAction)
                        {
                            Logger.Log($"{actionToExtract.Name} is a timing action, which cannot be used with smart extractors.", LogType.Warning, script, currentline + 1);
                            continue;
                        }

                        if (actionToExtract is not IReturnValueAction)
                        {
                            Logger.Log($"{actionToExtract.Name} action does not return any values, therefore can't be used with smart accessors.", LogType.Warning, script, currentline + 1);
                            continue;
                        }

                        if (script.SmartArguments.ContainsKey(lastAction))
                        {
                            script.SmartArguments[lastAction] = script.SmartArguments[lastAction].Append(ActionWrapper).ToArray();
                        }
                        else
                        {
                            script.SmartArguments[lastAction] = new Func<Tuple<bool, string>>[] { ActionWrapper };
                        }

                        AddActionNoArgs(new NullAction("SMART EXTR"));
                        continue;

                        Tuple<bool, string> ActionWrapper()
                        {
                            if (!TryRunAction(script, actionToExtract, out var resp, out _, actionArgs))
                            {
                                return new(false, "[Smart extractor]" + (resp?.Message is null ? string.Empty : " " + resp.Message));
                            }

                            if (resp == null || resp.ResponseVariables.Length == 0)
                            {
                                return new(false, "Action did not return any values to use.");
                            }

                            if (resp.ResponseVariables.Length > 1)
                            {
                                Log("Action returned more than 1 value. Using the first one as default.");
                            }

                            var value = resp.ResponseVariables[0];

                            if (value is not string stringValue)
                            {
                                return new(false, "Action returned a value that is not a string.");
                            }

                            return new(true, stringValue);
                        }
                    }

                    // flags
                    case "!--":
                    {
                        var flag = structureParts[1].Trim();

                        if (!script.HasFlag(flag))
                        {
                            Flag fl = new(flag, structureParts.Skip(2));
                            script.Flags.Add(fl);
                        }
                        else if (!suppressWarnings)
                        {
                            Logger.Warn(ErrorGen.Get(ErrorCode.MultipleFlagDefs, flag, scriptName));
                        }

                        AddActionNoArgs(new NullAction("FLAG DEFINE"));
                        continue;
                    }
                }

                // labels
                if (keyword.EndsWith(":"))
                {
                    var labelName = line.Remove(keyword.Length - 1, 1).RemoveWhitespace();

                    if (!script.Labels.ContainsKey(labelName))
                    {
                        script.Labels.Add(labelName, currentline);
                    }
                    else if (!suppressWarnings)
                    {
                        Logger.Warn(ErrorGen.Get(ErrorCode.MultipleLabelDefs, labelName, scriptName));
                    }

                    AddActionNoArgs(new NullAction($"{labelName} LABEL"));
                    continue;
                }

                // extractor
                var indexOfExtractor = line.IndexOf("::", StringComparison.Ordinal);
                var resultVariableNames = Array.Empty<string>();

                if (indexOfExtractor != -1)
                {
                    var variablesSection = line.Substring(0, indexOfExtractor);
                    var actionSection = line.Substring(indexOfExtractor + 2).TrimStart();

                    Log($"[ExtractorSyntax] Variables section: {variablesSection}");
                    Log($"[ExtractorSyntax] Action section: {actionSection}");

                    resultVariableNames = Parser.IsolateValueSyntax(variablesSection, script, true, false, false).variables.Select(arg => arg.Value).ToArray();
                    if (resultVariableNames.Length != 0)
                    {
                        Log($"[ExtractorSyntax] Variables found before the syntax: {string.Join(", ", resultVariableNames)}");

                        structureParts = actionSection.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(part => part.Trim()).ToList();
                        keyword = structureParts[0];
                    }
                    else
                    {
                        Logger.Warn("The extraction operator `::` has been used, but no variable names were specified to contain extracted values.", script);
                    }
                }

                keyword = keyword.ToUpper();
                var actType = TryGetActionType(keyword);
                if (actType is null)
                {
                    /*
                    if (CustomActions.TryGetValue(keyword, out CustomAction customAction))
                    {
                        List<string> customActArgs = ListPool<string>.Pool.Get();
                        CustomAction customAction1 = new(customAction.Name, customAction.Action);

                        foreach (string part in structureParts.Skip(1))
                        {
                            if (ArgumentProcessor.TryProcessSmartArgument(part, customAction1, script, out string result, false))
                            {
                                customActArgs.Add(result);
                            }
                            else
                            {
                                customActArgs.Add(part);
                            }
                        }

                        customAction1.RawArguments = customActArgs.ToArray();
                        actionList.Add(customAction1);
                        script.ResultVariableNames[customAction1] = resultVariableNames;

                        ListPool<string>.Pool.Return(customActArgs);
                        ListPool<string>.Pool.Return(structureParts);
                        continue;
                    }

                    if (!suppressWarnings)
                        Logger.Warn(ErrorGen.Get(ErrorCode.InvalidAction, keyword.RemoveWhitespace(), scriptName), script);

                    */
                    AddActionNoArgs(new NullAction("ERROR"));
                    continue;
                }

                if (Activator.CreateInstance(actType) is not IAction newAction)
                {
                    continue;
                }

                lastAction = newAction;
                script.OriginalActionArgs[newAction] = structureParts.Skip(1).Select(str => str.RemoveWhitespace()).ToArray();
                script.ResultVariableNames[newAction] = resultVariableNames;

                Log($"Queuing action {keyword}, {string.Join(", ", script.OriginalActionArgs[newAction])}");

                // Obsolete check
                if (newAction.IsObsolete(out var obsoleteReason) && !suppressWarnings && !script.SuppressWarnings)
                {
                    Logger.Warn($"Action {newAction.Name} is obsolete; {obsoleteReason}", script);
                }

                actionList.Add(newAction);
                ListPool<string>.Pool.Return(structureParts);
            }

            script.Actions = ListPool<IAction>.Pool.ToArrayReturn(actionList);

            Log($"Debug script read successfully. Name: {script.ScriptName} | Actions: {script.Actions.Count(act => act is not NullAction)} | Flags: {string.Join(" ", script.Flags)} | Labels: {string.Join(" ", script.Labels)} | Comments: {script.Actions.Count(action => action is NullAction @null && @null.Type is "COMMENT")}");

            return script;

            void AddActionNoArgs(IAction action)
            {
                script.OriginalActionArgs[action] = Array.Empty<string>();
                actionList.Add(action);
            }

            void Log(string message)
            {
                Logger.Debug($"[ScriptModule] [ReadScript] {message}", script);
            }
        }

        /// <summary>
        /// Reads and runs a script.
        /// </summary>
        /// <param name="scriptName">The name of the script.</param>
        /// <param name="executor">The executor that is running the script. Can be null.</param>
        /// <param name="dispose">Whether to dispose of the script as soon as execution is finished.</param>
        /// <exception cref="FileNotFoundException">The script was not found.</exception>
        /// <exception cref="DisabledScriptException">If <see cref="Script.IsDisabled"/> is <see langword="true"/>.</exception>
        public void ReadAndRun(string scriptName, ICommandSender executor, bool dispose = true)
        {
            var scr = ReadScript(scriptName, executor);
            RunScript(scr, dispose);
        }

        /// <summary>
        /// Registers all the actions in the provided assembly.
        /// </summary>
        /// <param name="assembly">The assembly to register actions in.</param>
        public void RegisterActions(Assembly assembly)
        {
            var i = 0;
            foreach (var type in assembly.GetTypes())
            {
                if (!typeof(IAction).IsAssignableFrom(type) || !type.IsClass ||
                    type.GetConstructors().Length <= 0) continue;

                if (type == typeof(CustomAction))
                    continue;

                if (Activator.CreateInstance(type) is not IAction temp) continue;

                Logger.Debug($"Adding Action: {temp.Name} | From Assembly: {assembly.GetName().Name}");
                ActionTypes.Add(new(temp.Name, temp.Aliases), type);
                i++;
            }

            MainPlugin.Info($"Assembly '{assembly.GetName().Name}' has registered {i} actions.");
        }

        /// <summary>
        /// Runs the script and disposes of it as soon as the script execution is complete.
        /// </summary>
        /// <param name="scr">The script to run.</param>
        /// <param name="dispose">If <see langword="true"/>, the script will be disposed after finishing execution.</param>
        /// <exception cref="DisabledScriptException">If <see cref="Script.IsDisabled"/> is <see langword="true"/>.</exception>
        public void RunScript(Script scr, bool dispose = true)
        {
            if (scr.IsDisabled)
                throw new DisabledScriptException(scr.ScriptName);

            CoroutineHandle handle = Timing.RunCoroutine(SafeRunCoroutine(RunScriptInternal(scr, dispose)), $"SCRIPT_{scr.UniqueId}");
            RunningScripts.Add(scr, handle);
        }

        /// <summary>
        /// Immediately stops execution of all scripts.
        /// </summary>
        /// <returns>The amount of scripts that were stopped.</returns>
        public int StopAllScripts()
        {
            var amount = 0;
            foreach (var kvp in RunningScripts)
            {
                amount++;
                Timing.KillCoroutines(kvp.Value);

                kvp.Key.IsRunning = false;
                kvp.Key.Dispose();
            }

            CoroutineHelper.KillAll();

            RunningScripts.Clear();
            return amount;
        }

        /// <summary>
        /// Stops execution of a script.
        /// </summary>
        /// <param name="scr">The script to stop.</param>
        /// <returns>Whether or not stopping was successful.</returns>
        public bool StopScript(Script scr)
        {
            KeyValuePair<Script, CoroutineHandle>? found = RunningScripts.FirstOrDefault(k => k.Key == scr);
            if (!found.HasValue || !found.Value.Value.IsRunning) return false;

            Timing.KillCoroutines(found.Value.Value);

            found.Value.Key.Coroutines.ForEach(data =>
            {
                if (!data.IsKilled)
                    data.Kill();
            });

            found.Value.Key.IsRunning = false;
            found.Value.Key.Dispose();

            return true;
        }

        /// <summary>
        /// Stops execution of all scripts with the matching name.
        /// </summary>
        /// <param name="name">The name of the script.</param>
        /// <returns>The amount of scripts stopped.</returns>
        public int StopScripts(string name)
        {
            return RunningScripts.Count(kvp => kvp.Key.ScriptName == name && StopScript(kvp.Key));
        }

        /// <summary>
        /// Reads a script.
        /// </summary>
        /// <param name="scriptName">The name of the script.</param>
        /// <param name="fileDirectory">The directory of the script, if it is found.</param>
        /// <returns>The contents of the script, if it is found.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the script is not found.</exception>
        private static string InternalRead(string scriptName, out string? fileDirectory)
        {
            fileDirectory = default;
            string? text = default;

            var mainFolderFile = Path.Combine(BasePath, scriptName + ".txt");
            string[] fileNames = Directory.GetFiles(BasePath, $"{scriptName}.txt", SearchOption.AllDirectories);

            if (File.Exists(mainFolderFile))
            {
                fileDirectory = mainFolderFile;
                text = File.ReadAllText(mainFolderFile);
            }
            else if (fileNames.Length == 1)
            {
                var fullFilePath = fileNames.First();

                fileDirectory = fullFilePath;
                text = File.ReadAllText(fullFilePath);
            }

            if (text is not null && fileDirectory is not null)
                return text;

            throw new FileNotFoundException($"Script {scriptName} does not exist.");
        }

        /// <summary>
        /// Returns the file path of a script.
        /// </summary>
        /// <param name="scriptName">The name of the script.</param>
        /// <returns>The directory of the script, if it is found.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the script is not found.</exception>
        private static string? GetFilePath(string scriptName)
        {
            InternalRead(scriptName, out var path);
            return path;
        }

        /// <summary>
        /// Reads and returns the text of a script.
        /// </summary>
        /// <param name="scriptName">The name of the script.</param>
        /// <returns>The contents of the script, if it is found.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the script is not found.</exception>
        private string ReadScriptText(string scriptName) => InternalRead(scriptName, out _);

        /// <summary>
        /// Internal coroutine to run the script.
        /// </summary>
        /// <param name="scr">The script to run.</param>
        /// <returns>Coroutine iterator.</returns>
        public IEnumerator<float> RunScriptInternal(Script scr, bool dispose = true)
        {
            void Log(string message)
            {
                Logger.Debug("[ScriptModule] [RunScriptInternal] " + message, scr);
            }

            MainPlugin.Info($"Started running the '{scr.ScriptName}' script.");

            yield return Timing.WaitForOneFrame;

            scr.IsRunning = true;
            scr.RunDate = DateTime.Now;

            for (; scr.CurrentLine < scr.Actions.Length; scr.NextLine())
            {
                if (!scr.HasFlag("NOSAFETY"))
                {
                    yield return Timing.WaitForOneFrame;
                }

                if (!scr.Actions.TryGet(scr.CurrentLine, out IAction action) || action == null)
                {
                    Log("There is no runnable action on this line. Skipping...");
                    continue;
                }

                Log("-> Running action " + action.Name);

                if (!TryRunAction(scr, action, out var resp, out var delay))
                {
                    Log("Action failed.");
                    if (resp is { Message: { Length: > 0 } })
                    {
                        Logger.ScriptError(resp.Message, scr, true);
                    }

                    continue;
                }

                if (delay.HasValue)
                {
                    yield return delay.Value;
                }

                if (resp == null)
                {
                    continue;
                }

                Log("Action success.");

                if (resp.ResponseFlags.HasFlag(ActionFlags.StopEventExecution))
                    break;

                if (!scr.ResultVariableNames.TryGetValue(action, out var variableNames) || variableNames is null)
                {
                    continue;
                }

                foreach (var zipped in resp.ResponseVariables.Zip(variableNames, (variable, name) => (variable, name)))
                {
                    switch (zipped.variable)
                    {
                        case Player[] plrVar:
                            Log($"Action {action.Name} is adding a player variable as '{zipped.name}'.");
                            scr.AddPlayerVariable(zipped.name, plrVar, false);
                            break;

                        case string strVar:
                            Log($"Action {action.Name} is adding a variable as '{zipped.name}'.");
                            scr.AddLiteralVariable(zipped.name, strVar, false);
                            break;

                        default:
                            Logger.ScriptError($"Action '{action.Name}' returned a value of an illegal type '{zipped.variable.GetType()}', which is not supported. Report this error to the developers.", scr);
                            scr.UniqueLiteralVariables.Add(zipped.name, new(zipped.name, string.Empty, zipped.variable.ToString()));
                            break;
                    }
                }
            }

            Log("-----------");
            Log($"Script concluded!");
            MainPlugin.Info($"Finished running script {scr.ScriptName}.");
            Log("-----------");
            scr.IsRunning = false;

            Log("Removing script from running scripts.");
            RunningScripts.Remove(scr);

            if (dispose)
                scr.Dispose();
        }

        private static void DebugLog(string message, Script source)
        {
            Logger.Debug($"[ScriptModule] {message}", source);
        }

        /// <summary>
        /// A wrapper for coroutines which catches exceptions and logs them.
        /// </summary>
        /// <param name="coroutine">The coroutine to run.</param>
        /// <returns>A coroutine.</returns>
        private IEnumerator<float> SafeRunCoroutine(IEnumerator<float> coroutine)
        {
            // chatgpt made this amazing thing
            while (true)
            {
                object current;
                try
                {
                    // Continue running the original coroutine
                    if (coroutine.MoveNext())
                    {
                        current = coroutine.Current;
                    }
                    else
                    {
                        yield break;
                    }
                }
                catch (Exception e)
                {
                    Logger.Error($"A coroutine error has been caught!\n{e.Message}\n{e.StackTrace}");
                    yield break;
                }

                yield return (float)current;
            }
        }
    }
}
