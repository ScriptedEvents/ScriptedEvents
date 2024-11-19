using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;

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
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.DemoScripts;

    using ScriptedEvents.Structures;

    using Logger = ScriptedEvents.API.Features.Logger;
    using LogType = LogType;

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

            if (scr.SingleLineIfStatements.TryGetValue(action, out var predicate))
            {
                if (!predicate())
                {
                    return true;
                }
            }

            if (originalActionArgs is null)
            {
                if (scr.OriginalActionArgs.TryGetValue(action, out var actionArgs) && actionArgs is not null)
                {
                    originalActionArgs = actionArgs;
                }
                else
                {
                    Log("Action does not have any arguments provided.");
                    originalActionArgs = Array.Empty<string>();
                }
            }

            ArgumentProcessResult res = ArgumentProcessor.ProcessActionArguments(action.ExpectedArguments, originalActionArgs, action, scr);
            if (res.Errored)
            {
                res.ErrorTrace!.Append(Error(
                    $"Execution of action '{action.Name}' failed.",
                    "Action argument processing failed, see inner exception for details."));
                actResp = new(false, res.ErrorTrace);
                return false;
            }

            if (!res.ShouldExecute)
            {
                Log("The action will not run, because the 'ShouldExecute' property from 'ArgumentProcessResult' is set to false.");
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
                    throw new VariableException("Unknown action type");
            }

            if (!actResp.Success)
            {
                return false;
            }

            if (delay.HasValue)
            {
                Log($"Action '{action.Name}' is delaying the script. Length of delay: {delay.Value}");
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
                Logger.Error(Error("File generation error", $"There was an error generating the README and demo scripts: {e.Message}").ToTrace());
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

                if (!TryRunScript(scr, out var trace))
                {
                    trace!.Append(Error(
                        "Autorun execution failed",
                        "Provided script was not able to run, see trace below for details."));
                    Logger.Error(trace!);
                }

                autoRunScripts.Add(scr);
                AutoRunScripts.Add(scr.ScriptName);
            }
        }

        /// <summary>
        /// Retrieves a list of all scripts in the server.
        /// </summary>
        /// <returns>A list of all scripts.</returns>
        /// <remarks>WARNING: Scripts created through this method are NOT DISPOSED!!! Call <see cref="Script.Dispose"/> when done with them.</remarks>
        public List<Script> ListScripts(ICommandSender? sender = null)
        {
            List<Script> scripts = new();
            string[] files = Directory.GetFiles(BasePath, "*.txt", SearchOption.AllDirectories);

            foreach (string x in files)
            {
                var file = Path.GetFileName(x);
                if (!TryParseScript(file, sender, out var script, out var trace))
                {
                    Logger.Error(trace!);
                    continue;
                }

                scripts.Add(script!);
            }

            return scripts;
        }

        /// <summary>
        /// Returns an action type, if its name or aliases match the input.
        /// </summary>
        /// <param name="name">The name of the action.</param>
        /// <returns>Whether or not the try-get was successful.</returns>
        public bool TryGetActionType(string name, out IAction? action, out ErrorInfo? errorInfo)
        {
            // TODO: implement for external actions
            action = null;
            errorInfo = null;

            name = name.ToUpper();
            var type = ActionTypes
                .Where(actionData =>
                    actionData.Key.Name == name ||
                    actionData.Key.Aliases.Contains(name))
                .Select(actionData => actionData.Value)
                .FirstOrDefault();

            if (type is null)
            {
                errorInfo = Error(
                    $"Provided action '{name}' does not exist.",
                    "Perhaps you've made a typo?");
                return false;
            }

            if (Activator.CreateInstance(type) is IAction act)
            {
                action = act;
                errorInfo = null;
                return true;
            }

            errorInfo = Error(
                $"Provided action '{name}' does not exist.",
                "Perhaps you've made a typo?");
            return false;
        }

        /// <summary>
        /// Reads a script line-by-line, converting every line into an appropriate action, flag, label, etc. Fills out all data and returns a <see cref="Script"/> object.
        /// </summary>
        /// <param name="scriptName">The name of the script.</param>
        /// <param name="executor">The CommandSender that ran the script. Can be null.</param>
        /// <param name="suppressWarnings">Do not show warnings in the console.</param>
        /// <returns>The <see cref="Script"/> fully filled out, if the script was found.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the script is not found.</exception>
        public bool TryParseScript(string scriptName, ICommandSender? executor, out Script? script, out ErrorTrace? errorTrace, bool suppressWarnings = false)
        {
            script = default;

            if (!InternalTryReadScript(scriptName, out var content, out var path, out var errorInfo))
            {
                errorTrace = errorInfo!.ToTrace().Append(Error(
                    $"Failed to parse the script '{scriptName}'",
                    "See the trace below for details."));
                return false;
            }

            if (content is null || path is null)
                throw new ArgumentNullException();

            Script innerScript = new();
            innerScript.ScriptName = scriptName;
            innerScript.RawText = content;
            innerScript.FilePath = path;
            innerScript.LastRead = File.GetLastAccessTimeUtc(path);
            innerScript.LastEdited = File.GetLastWriteTimeUtc(path);
            innerScript.Sender = executor;
            innerScript.Context = executor switch
            {
                null => ExecuteContext.Automatic,
                ServerConsoleSender _ => ExecuteContext.ServerConsole,
                PlayerCommandSender _ => ExecuteContext.RemoteAdmin,
                _ => innerScript.Context
            };

            // Fill out script data
            if (MainPlugin.Singleton.Config.RequiredPermissions.TryGetValue(scriptName, out var perm2))
            {
                innerScript.ReadPermission += $".{perm2}";
                innerScript.ExecutePermission += $".{perm2}";
            }

            var actionList = ListPool<IAction>.Pool.Get();
            var array = content.Split('\n');
            IAction? lastAction = null;
            Func<bool>? singleLineIfStatement = null;
            var inMultilineComment = false;

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
                        Logger.ScriptError($"A function label syntax has been used, but no name has been provided.", innerScript, printableLine: currentline + 1);
                        continue;
                    case "->":
                    {
                        var labelName = structureParts[1].RemoveWhitespace();

                        if (!innerScript.FunctionLabels.ContainsKey(labelName))
                        {
                            innerScript.FunctionLabels.Add(labelName, currentline);
                        }
                        else if (!suppressWarnings)
                        {
                            Logger.ScriptError(
                                Error(
                                    $"Multiple label definitions in script '{scriptName}'",
                                    $"Label '{labelName}' is already defined")
                                    .ToTrace(),
                                innerScript,
                                printableLine: currentline + 1);
                            continue;
                        }

                        AddActionNoArgs(new StartFunctionAction());
                        continue;
                    }

                    case "??" when actionList.Count < 2:
                        Logger.Log("'??' (single line if statement) syntax can't be used without providing a conditon.", LogType.Warning, innerScript, currentline + 1);
                        continue;
                    case "??":
                    {
                        singleLineIfStatement = () => ConditionHelper.Evaluate(string.Join(" ", structureParts.Skip(1)).Trim(), innerScript).Passed;
                        continue;
                    }

                    // smart args
                    case "//" when actionList.Count == 0 || lastAction is null:
                        Logger.Log("'//' (smart argument) syntax can't be used if there isn't any action above it.", LogType.Warning, innerScript, currentline + 1);
                        continue;
                    case "//":
                    {
                        var value = string.Join(" ", structureParts.Skip(1)).Trim();

                        if (innerScript.SmartArguments.ContainsKey(lastAction))
                        {
                            innerScript.SmartArguments[lastAction] = innerScript.SmartArguments[lastAction].Append(Lambda).ToArray();
                        }
                        else
                        {
                            innerScript.SmartArguments[lastAction] = new Func<Tuple<ErrorTrace?, object?, Type?>>[] { Lambda };
                        }

                        AddActionNoArgs(new NullAction("SMART ARG"));
                        continue;

                        Tuple<ErrorTrace?, object?, Type?> Lambda()
                        {
                            return new(default, Parser.ReplaceContaminatedValueSyntax(value, innerScript), typeof(string));
                        }
                    }

                    case "//::" when actionList.Count == 0 || lastAction is null:
                        Logger.Log("'//::' (smart extractor) syntax can't be used if there isn't any action above it.", LogType.Warning, innerScript, currentline + 1);
                        continue;
                    case "//::":
                    {
                        var actionName = structureParts[1];
                        var actionArgs = structureParts.Skip(2).ToArray();

                        if (!TryGetActionType(actionName, out var actionToExtract, out var info))
                        {
                            var trace = info!.ToTrace().Append(Error(
                                "Failed to parse the smart extractor",
                                "See traceback below for more info"));
                            Logger.ScriptError(trace, innerScript, printableLine: currentline + 1);
                            continue;
                        }

                        switch (actionToExtract)
                        {
                            case null:
                                throw new ArgumentException();

                            case ITimingAction:
                                Logger.ScriptError($"{actionToExtract.Name} is a timing action, which cannot be used with smart extractors.", innerScript, false, currentline + 1);
                                continue;

                            case IReturnValueAction:
                                break;

                            default:
                                Logger.ScriptError($"{actionToExtract.Name} action does not return any values, therefore can't be used with smart accessors.", innerScript, false, currentline + 1);
                                continue;
                        }

                        if (innerScript.SmartArguments.ContainsKey(lastAction))
                        {
                            innerScript.SmartArguments[lastAction] = innerScript.SmartArguments[lastAction].Append(SmartExtractor).ToArray();
                        }
                        else
                        {
                            innerScript.SmartArguments[lastAction] = new Func<Tuple<ErrorTrace?, object?, Type?>>[] { SmartExtractor };
                        }

                        AddActionNoArgs(new NullAction("SMART EXTR"));
                        continue;

                        Tuple<ErrorTrace?, object?, Type?> SmartExtractor()
                        {
                            if (!TryRunAction(innerScript, actionToExtract, out var resp, out _, actionArgs))
                            {
                                resp!.ErrorTrace!.Append(Error(
                                    $"Smart extractor for action '{actionToExtract.Name}' failed",
                                    "Provided action failed while running, see trace below for more info."));
                                return new(resp.ErrorTrace, default, default);
                            }

                            if (resp == null || resp.ResponseVariables.Length == 0)
                            {
                                var err = Error(
                                    $"Smart extractor for action '{actionToExtract.Name}' failed",
                                    $"The action did not return any values for the extractor to use.");
                                return new(err.ToTrace(), default, default);
                            }

                            if (resp.ResponseVariables.Length > 1)
                            {
                                Log("Action returned more than 1 value. Using the first one as default.");
                            }

                            var value = resp.ResponseVariables[0];

                            switch (value)
                            {
                                case string stringValue:
                                    return new(default, stringValue, typeof(string));
                                case Player[] playerValue:
                                    return new(default, playerValue, typeof(Player[]));
                                default:
                                {
                                    var err1 = Error(
                                        $"Smart extractor for action '{actionToExtract.Name}' failed",
                                        $"Provided action returned a value which is of an incompatible type {value.GetType().Name}.");
                                    return new(err1.ToTrace(), default, default);
                                }
                            }
                        }
                    }

                    // flags
                    case "!--":
                    {
                        var flag = structureParts[1].Trim();

                        if (!innerScript.HasFlag(flag))
                        {
                            Flag fl = new(flag, structureParts.Skip(2));
                            innerScript.Flags.Add(fl);
                        }
                        else if (!suppressWarnings)
                        {
                            Logger.ScriptError(
                                Error(
                                    $"Provided flag '{flag}' is already used.",
                                    $"The script '{innerScript.ScriptName}' already uses the '{flag}' flag.")
                                    .ToTrace(),
                                innerScript,
                                false,
                                currentline + 1);
                        }

                        AddActionNoArgs(new NullAction("FLAG DEFINE"));
                        continue;
                    }
                }

                // labels
                if (keyword.EndsWith(":"))
                {
                    var labelName = line.Remove(keyword.Length - 1, 1).RemoveWhitespace();

                    if (!innerScript.Labels.ContainsKey(labelName))
                    {
                        innerScript.Labels.Add(labelName, currentline);
                    }
                    else if (!suppressWarnings)
                    {
                        Logger.ScriptError(
                            Error(
                                    $"Provided label '{labelName}' is already used.",
                                    $"The script '{innerScript.ScriptName}' already uses the '{labelName}' label. Remove or rename the duplicate label.")
                                .ToTrace(),
                            innerScript,
                            false,
                            currentline + 1);
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

                    resultVariableNames = Parser.IsolateValueSyntax(variablesSection, innerScript, true, false, false).variables.Select(arg => arg.Value).ToArray();
                    if (resultVariableNames.Length != 0)
                    {
                        Log($"[ExtractorSyntax] Variables found before the syntax: {string.Join(", ", resultVariableNames)}");

                        structureParts = actionSection.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(part => part.Trim()).ToList();
                        keyword = structureParts[0];
                    }
                    else
                    {
                        Logger.Warn("The extraction operator `::` has been used, but no variable names were specified to contain extracted values.", innerScript);
                    }
                }

                keyword = keyword.ToUpper();
                if (!TryGetActionType(keyword, out var actionType, out var errorMessage))
                {
                    Logger.ScriptError(
                        errorMessage!.ToTrace(),
                        innerScript,
                        false,
                        currentline + 1);
                }

                lastAction = actionType ?? throw new ArgumentException();
                innerScript.OriginalActionArgs[lastAction] = structureParts.Skip(1).Select(str => str.RemoveWhitespace()).ToArray();
                innerScript.ResultVariableNames[lastAction] = resultVariableNames;

                if (singleLineIfStatement is not null)
                {
                    innerScript.SingleLineIfStatements[lastAction] = singleLineIfStatement;
                    singleLineIfStatement = null;
                }

                Log($"Queuing action {keyword}, {string.Join(", ", innerScript.OriginalActionArgs[lastAction])}");

                // Obsolete check
                if (lastAction.IsObsolete(out var obsoleteReason) && !suppressWarnings && !innerScript.SuppressWarnings)
                {
                    Logger.Warn($"Action {lastAction.Name} is obsolete; {obsoleteReason}", innerScript);
                }

                actionList.Add(lastAction);
                ListPool<string>.Pool.Return(structureParts);
            }

            innerScript.Actions = ListPool<IAction>.Pool.ToArrayReturn(actionList);

            Log($"DebugActions script read successfully. Name: {innerScript.ScriptName} | Actions: {innerScript.Actions.Count(act => act is not NullAction)} | Flags: {string.Join(" ", innerScript.Flags)} | Labels: {string.Join(" ", innerScript.Labels)} | Comments: {innerScript.Actions.Count(action => action is NullAction @null && @null.Type is "COMMENT")}");

            script = innerScript;
            errorTrace = null;
            return true;

            void AddActionNoArgs(IAction action)
            {
                innerScript.OriginalActionArgs[action] = Array.Empty<string>();
                actionList.Add(action);
            }

            void Log(string message)
            {
                Logger.Debug($"[ScriptModule] [TryParseScript] {message}", innerScript);
            }
        }

        /// <summary>
        /// Reads and runs a script.
        /// </summary>
        /// <param name="scriptName">The name of the script.</param>
        /// <param name="executor">The executor that is running the script. Can be null.</param>
        /// <param name="dispose">Whether to dispose of the script as soon as execution is finished.</param>
        public bool TryReadAndRun(string scriptName, ICommandSender? executor, out ErrorTrace? errorTrace, bool dispose = true)
        {
            if (!TryParseScript(scriptName, executor, out var parsedScript, out var trace1))
            {
                errorTrace = trace1;
                return false;
            }

            if (!TryRunScript(parsedScript!, out var trace2, out _, dispose))
            {
                errorTrace = trace2;
                return false;
            }

            errorTrace = null;
            return true;
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

        public bool TryRunScript(Script scr, out ErrorTrace? errorTrace, out CoroutineHandle? handle, bool dispose = true)
        {
            if (scr.IsDisabled)
            {
                errorTrace = Error(
                        $"Script '{scr.ScriptName}' is disabled",
                        "This script is probably marked by the '!-- DISABLED' flag.")
                    .ToTrace();
                handle = default;
                return false;
            }

            CoroutineHandle currHandle = Timing.RunCoroutine(SafeRunCoroutine(RunScriptInternal(scr, dispose)), $"SCRIPT_{scr.UniqueId}");
            RunningScripts.Add(scr, currHandle);
            handle = currHandle;
            errorTrace = null;
            return true;
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
        /// <param name="filePath">The directory of the script, if it is found.</param>
        /// <returns>The contents of the script, if it is found.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the script is not found.</exception>
        private static bool InternalTryReadScript(string scriptName, out string? fileContent, out string? filePath, out ErrorInfo? errorInfo)
        {
            filePath = default;
            fileContent = default;
            var fileNames = Directory.GetFiles(BasePath, scriptName.EndsWith(".txt") ? scriptName : $"{scriptName}.txt", SearchOption.AllDirectories);

            switch (fileNames.Length)
            {
                case 0:
                    errorInfo = Error(
                        $"Script '{scriptName}' does not exist",
                        "Search revealed 0 scripts available.");
                    return false;
                case > 1:
                    errorInfo = Error(
                        $"Multiple definitions of script '{scriptName}'",
                        $"Search revealed {fileNames.Length} scripts under the '{scriptName}' name.");
                    return false;
            }

            filePath = fileNames[0];
            fileContent = File.ReadAllText(filePath);
            errorInfo = null;
            return true;
        }

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
                    Logger.ScriptError(resp!.ErrorTrace!, scr);
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
                            scr.LocalLiteralVariables.Add(zipped.name, new(zipped.name, string.Empty, zipped.variable.ToString()));
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
                    Logger.Error($"A coroutine error has been caught!\nError: '{e.Message}'\n{e.StackTrace}");
                    yield break;
                }

                yield return (float)current;
            }
        }

        private static ErrorInfo Error(string name, string desc)
        {
            return new(name, desc, "ScriptModule");
        }
    }
}
