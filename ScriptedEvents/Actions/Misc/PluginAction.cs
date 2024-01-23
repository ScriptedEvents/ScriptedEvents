﻿namespace ScriptedEvents.Actions
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.Loader;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class PluginAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "PLUGIN";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <inheritdoc/>
        public string Description => "Enables/disables a specific plugin.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode (ENABLE/DISABLE).", true),
            new Argument("plugin", typeof(string), "The plugin to toggle.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            switch (Arguments[0].ToUpper())
            {
                case "ENABLE":

                    string assemblyPath = Path.Combine(Paths.Plugins, $"{string.Join(" ", Arguments.Skip(1))}.dll");
                    Assembly assembly = Loader.LoadAssembly(assemblyPath);
                    if (assembly is null)
                    {
                        return new(false, "Plugin not found.");
                    }

                    if (Loader.Plugins.Any(pl => pl.Assembly == assembly))
                    {
                        return new(false, "Plugin already enabled.");
                    }

                    Loader.Locations[assembly] = assemblyPath;

                    IPlugin<IConfig> plugin = Loader.CreatePlugin(assembly);
                    if (plugin is null)
                    {
                        return new(false, "Plugin is null!");
                    }

                    AssemblyInformationalVersionAttribute attribute = plugin.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                    Log.Info($"Loaded plugin {plugin.Name}@{(plugin.Version is not null ? $"{plugin.Version.Major}.{plugin.Version.Minor}.{plugin.Version.Build}" : attribute is not null ? attribute.InformationalVersion : string.Empty)}");

                    Server.PluginAssemblies.Add(assembly, plugin);
                    if (plugin.Config.Debug)
                        Log.DebugEnabled.Add(assembly);

                    Loader.Plugins.Add(plugin);
                    plugin.OnEnabled();
                    plugin.OnRegisteringCommands();
                    break;
                case "DISABLE":
                    IPlugin<IConfig> plugin2 = Loader.GetPlugin(Arguments[1]);
                    if (plugin2 is null)
                    {
                        return new(false, "Plugin not enabled or not found.");
                    }

                    plugin2.OnUnregisteringCommands();
                    plugin2.OnDisabled();
                    Loader.Plugins.Remove(plugin2);
                    Loader.Locations.Remove(plugin2.Assembly);
                    break;
                default:
                    return new(MessageType.InvalidOption, this, "mode", Arguments[0], "ENABLE/DISABLE");
            }

            return new(true);
        }
    }
}
