namespace ScriptedEvents.API.Features.ScriptHelpGenerator
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using ScriptedEvents.Actions;
    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    /// <summary>
    /// Used to generate files via `shelp GENERATE` in the server console.
    /// </summary>
    public static class Generator
    {
        public static readonly string BasePath = Path.Combine(MainPlugin.BaseFilePath, "Documentation");
        public static readonly string ActionPath = Path.Combine(BasePath, "Actions");
        public static readonly string VariablePath = Path.Combine(BasePath, "Variables");
        public static readonly string ErrorsPath = Path.Combine(BasePath, "Errors");
        public static readonly string EnumsPath = Path.Combine(BasePath, "Enums");
        public static readonly string ConfigPath = Path.Combine(BasePath, "generator_config.txt");
        public static readonly Type ConfigType = typeof(GeneratorConfig);

        public static bool GenerateConfig(out string message)
        {
            if (!Directory.Exists(BasePath))
                Directory.CreateDirectory(BasePath);

            if (File.Exists(ConfigPath))
                File.Delete(ConfigPath);

            StringBuilder fileContents = StringBuilderPool.Pool.Get();
            fileContents.AppendLine("# Documentation Generator Configs\n# Please change each 'y' to a 'n' if you do not want the associated files generated. More files = more space taken up on PC.");
            foreach (FieldInfo propInfo in ConfigType.GetFields())
            {
                fileContents.AppendLine();

                DescriptionAttribute desc = propInfo.GetCustomAttribute<DescriptionAttribute>();

                if (desc is not null)
                    fileContents.AppendLine($"# {desc.Description}");

                fileContents.AppendLine(propInfo.Name + ": y");
            }

            File.WriteAllText(ConfigPath, StringBuilderPool.Pool.ToStringReturn(fileContents));

            new FileInfo(ConfigPath)
            {
                Attributes = FileAttributes.Temporary,
            };

            try
            {
                Process.Start(ConfigPath);
            }
            catch (Exception)
            {
                message = $"Config file was created successfully, but an error occurred when opening external text editor (likely due to permissions). File is located at: {ConfigPath}.";
                return false;
            }

            message = "A config file for the documentation generator has been created. Please fill out the generated config file using 'y' for yes and 'n' for no. When done, run the 'shelp GDONE' command.";
            return true;
        }

        public static bool CreateDocumentation(out string message)
        {
            if (!Directory.Exists(BasePath))
                Directory.CreateDirectory(BasePath);

            if (!File.Exists(ConfigPath))
            {
                message = "Documentation generation is not in progress! Please run 'shelp GENERATE' before running this command.";
                return false;
            }

            string rawConfig = File.ReadAllText(ConfigPath);
            GeneratorConfig config = new();

            int i = 0;
            foreach (string line in rawConfig.Split('\n'))
            {
                i++;

                if (line.StartsWith("#") || line.RemoveWhitespace() == string.Empty || line == string.Empty)
                    continue;

                string[] sections = line.RemoveWhitespace().Split(':');
                if (sections.Length < 2)
                {
                    message = $"Malformed file. Line {i} is missing info or has the wrong amount of sections.";
                    return false;
                }

                FieldInfo fieldInfo = ConfigType.GetField(sections[0]);
                if (fieldInfo is null)
                {
                    message = $"Unknown documentation config '{sections[0]}'";
                    return false;
                }

                if (!sections[1].IsBool(out bool doGenerate))
                {
                    message = $"Malformed file. Line {i} does not have proper y/n value.";
                    return false;
                }

                fieldInfo.SetValue(config, doGenerate);
            }

            foreach (FieldInfo fieldInfo in ConfigType.GetFields())
            {
                Log.Debug($"{fieldInfo.Name} = {fieldInfo.GetValue(config)}");
            }

            File.Delete(ConfigPath);

            // Documentation data
            string metaPath = Path.Combine(BasePath, "DocInfo.txt");
            File.WriteAllText(metaPath, $"Documentation Generator\nGenerated at: {DateTime.UtcNow:f}\nSE version: {MainPlugin.Singleton.Version}\nExperimental DLL: {(MainPlugin.IsExperimental ? "YES" : "NO")}\n\n\n-- DO NOT MODIFY BELOW THIS LINE --\n!_V{MainPlugin.Singleton.Version}");

            // Delete old folders
            if (Directory.Exists(ActionPath))
            {
                Log.Info("Old actions documentation exists and has been deleted.");
                Directory.Delete(ActionPath, true);
            }

            if (Directory.Exists(VariablePath))
            {
                Log.Info("Old variable documentation exists and has been deleted.");
                Directory.Delete(VariablePath, true);
            }

            if (Directory.Exists(ErrorsPath))
            {
                Log.Info("Old error documentation exists and has been deleted.");
                Directory.Delete(ErrorsPath, true);
            }

            if (Directory.Exists(EnumsPath))
            {
                Log.Info("Old enum documentation exists and has been deleted.");
                Directory.Delete(EnumsPath, true);
            }

            if (config.generate_actions)
            {
                Directory.CreateDirectory(ActionPath);

                Stopwatch watch = Stopwatch.StartNew();
                foreach (var actionData in MainPlugin.ScriptModule.ActionTypes)
                {
                    IAction action = Activator.CreateInstance(actionData.Value) as IAction;

                    if ((action is IHiddenAction && !MainPlugin.Configs.Debug) || action is not IHelpInfo helpInfo || action.IsObsolete(out _))
                        continue;

                    Log.Debug("Creating documentation for action: " + action.Name);

                    ActionResponse text = HelpAction.GenerateText(action.Name);

                    if (text.Success)
                    {
                        string path = Path.Combine(ActionPath, action.Name + ".txt");
                        File.WriteAllText(path, text.Message);
                    }
                }

                watch.Stop();

                Log.Info($"Completed generating documentation for actions. Elapsed time: {watch.ElapsedMilliseconds}ms");
            }

            if (config.generate_variables)
            {
                Directory.CreateDirectory(VariablePath);

                Stopwatch watch = Stopwatch.StartNew();
                var conditionList = VariableSystemV2.Groups.OrderBy(group => group.GroupName);
                foreach (IVariableGroup group in conditionList)
                {
                    string groupPath = Path.Combine(VariablePath, group.GroupName);
                    Directory.CreateDirectory(groupPath);

                    foreach (IVariable variable in group.Variables)
                    {
                        Log.Debug("Creating documentation for variable: " + variable.Name);

                        ActionResponse text = HelpAction.GenerateText(variable.Name);

                        if (text.Success)
                        {
                            string path = Path.Combine(groupPath, variable.Name.Replace("{", string.Empty).Replace("}", string.Empty) + ".txt");
                            File.WriteAllText(path, text.Message);
                        }
                    }
                }

                watch.Stop();

                Log.Info($"Completed generating documentation for variables. Elapsed time: {watch.ElapsedMilliseconds}ms");
            }

            if (config.generate_enums)
            {
                Directory.CreateDirectory(EnumsPath);

                Stopwatch watch = Stopwatch.StartNew();
                foreach (EnumDefinition def in EnumDefinitions.Definitions)
                {
                    Log.Debug("Creating documentation for enum: " + def.EnumType.Name);

                    ActionResponse text = HelpAction.GenerateText(def.EnumType.Name.ToUpper());

                    if (text.Success)
                    {
                        string path = Path.Combine(EnumsPath, def.EnumType.Name + ".txt");
                        File.WriteAllText(path, text.Message);
                    }
                }

                watch.Stop();

                Log.Info($"Completed generating documentation for enums. Elapsed time: {watch.ElapsedMilliseconds}ms");
            }

            if (config.generate_error_codes)
            {
                Directory.CreateDirectory(ErrorsPath);

                Stopwatch watch = Stopwatch.StartNew();
                foreach (ErrorInfo errorInfo in ErrorList.Errors)
                {
                    Log.Debug("Creating documentation for error: " + errorInfo.Id);

                    ActionResponse text = HelpAction.GenerateText(errorInfo.Id.ToString());

                    if (text.Success)
                    {
                        string path = Path.Combine(ErrorsPath, "SE-" + errorInfo.Id.ToString() + ".txt");
                        File.WriteAllText(path, text.Message);
                    }
                }

                watch.Stop();

                Log.Info($"Completed generating documentation for enums. Elapsed time: {watch.ElapsedMilliseconds}ms");
            }

            try
            {
                Process.Start(BasePath);
            }
            catch
            {
            }

            message = $"Generation complete! The documentation is located at: {BasePath}";
            return true;
        }

        public static bool PromptDocFolder(out string message)
        {
            if (!Directory.Exists(BasePath))
                Directory.CreateDirectory(BasePath);

            try
            {
                Process.Start(BasePath);
                message = "Done.";
                return true;
            }
            catch (UnauthorizedAccessException unauthEx)
            {
                message = $"This console does not have proper permissions to open files. {(MainPlugin.Configs.Debug ? unauthEx : string.Empty)}";
                return false;
            }
            catch (Exception e)
            {
                message = $"Unknown exception while opening file: {(MainPlugin.Configs.Debug ? e : e.Message)}";
                return false;
            }
        }

        public static bool CheckUpdated(out string message)
        {
            if (!Directory.Exists(BasePath))
            {
                message = "SKIP";
                return false;
            }

            string metaPath = Path.Combine(BasePath, "DocInfo.txt");

            if (!File.Exists(metaPath))
            {
                message = "DocInfo file cannot be found in your generated documentation. Consider re-generating your local documentation using the 'shelp GENERATE' command.";
                return false;
            }

            string contents = File.ReadAllText(metaPath);

            try
            {
                int versionIndex = contents.IndexOf("!_V");
                if (versionIndex == -1)
                {
                    message = "DocInfo file does not contain version information regarding documentation. Consider re-generating your local documentation using the 'shelp GENERATE' command.";
                    return false;
                }

                Version version = new(contents.Substring(versionIndex + 3));

                if (version < MainPlugin.Singleton.Version)
                {
                    message = $"Your local ScriptedEvents documentation is NOT UP TO DATE!! Consider re-generating your local documentation using the 'shelp GENERATE' command. Plugin Version: {MainPlugin.Singleton.Version} Doc Version: {version}";
                    return false;
                }

                message = $"Your local ScriptedEvents documentation is up to date. Plugin Version: {MainPlugin.Singleton.Version} Doc Version: {version}";
                return true;
            }
            catch (Exception e)
            {
                message = $"Error when performing documentation generation update checking. {(MainPlugin.Configs.Debug ? e : e.Message)}";
                return false;
            }
        }
    }
}
