using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Loader;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Features.Exceptions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Server
{
    public class PluginAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Plugin";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Server;

        /// <inheritdoc/>
        public string Description => "Enables/disables a specific plugin.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new Option("Enable", "Enables a plugin. Requires the name of the plugin DLL."),
                new Option("Disable", "Disables a previously-enabled plugin. Requires the name of the plugin.")),
            new Argument("plugin", typeof(string), "The plugin to toggle.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            switch (Arguments[0]!.ToUpper())
            {
                case "ENABLE":
                    string assemblyPath = Path.Combine(Paths.Plugins, $"{Arguments[1]}.dll");
                    Assembly assembly = Loader.LoadAssembly(assemblyPath);
                    if (assembly is null)
                    {
                        var err = new ErrorInfo(
                            "Failed to load plugin.",
                            $"Failed to load assembly found at '{assemblyPath}'",
                            Name).ToTrace();
                        return new(false, null, err);
                    }

                    if (Loader.Plugins.Any(pl => pl.Assembly == assembly))
                    {
                        return new(true);
                    }

                    Loader.Locations[assembly] = assemblyPath;

                    IPlugin<IConfig> plugin = Loader.CreatePlugin(assembly);
                    if (plugin is null)
                    {
                        throw new ImpossibleException();
                    }

                    AssemblyInformationalVersionAttribute attribute = plugin.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                    Log.Info($"Loaded plugin {plugin.Name}@{(plugin.Version is not null ? $"{plugin.Version.Major}.{plugin.Version.Minor}.{plugin.Version.Build}" : attribute is not null ? attribute.InformationalVersion : string.Empty)}");
                    
                    Exiled.API.Features.Server.PluginAssemblies.Add(assembly, plugin);
                    if (plugin.Config.Debug)
                        Log.DebugEnabled.Add(assembly);

                    Loader.Plugins.Add(plugin);
                    plugin.OnEnabled();
                    plugin.OnRegisteringCommands();
                    break;
                case "DISABLE":
                    IPlugin<IConfig> plugin2 = Loader.GetPlugin((string)Arguments[1]!);
                    if (plugin2 is null)
                    {
                        var err = new ErrorInfo(
                            "Failed to find the plugin.",
                            $"Plugin '{Arguments[1]}' is not present in the running plugins dictionary.",
                            Name).ToTrace();
                        return new(false, null, err);
                    }

                    plugin2.OnUnregisteringCommands();
                    plugin2.OnDisabled();
                    Loader.Plugins.Remove(plugin2);
                    Loader.Locations.Remove(plugin2.Assembly);
                    break;
            }

            return new(true);
        }
    }
}
