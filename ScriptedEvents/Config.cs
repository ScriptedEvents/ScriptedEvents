namespace ScriptedEvents
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using Exiled.API.Interfaces;
    using ScriptedEvents.API.Features.Aliases;

    public class Config : IConfig
    {
        [Description("Whether or not to enable the Scripted Events plugin.")]
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; } = false;

        [Description("Enable logs for starting/stopping scripts.")]
        public bool EnableLogs { get; set; } = true;

        [Description("List of scripts to run as soon as the round starts.")]
        public List<string> AutoRunScripts { get; set; } = new();

        [Description("List of scripts to automatically re-run as soon as they finish.")]
        public List<string> LoopScripts { get; set; } = new();

        // todo: un-alias door commands, because they dont have duration anymore
        [Description("Define a custom set of actions and the action they run when used.")]
        public List<Alias> Aliases { get; set; } = new()
        {
            new("LOCKDOORBRIEF", "DOOR LOCK * 10"),
        };

        [Description("Define a custom set of permissions used to run a certain script. The provided permission will be added AFTER script.execute (eg. script.execute.examplepermission for the provided example).")]
        public Dictionary<string, string> RequiredPermissions { get; set; } = new()
        {
            { "ExampleScriptNameHere", "examplepermission" },
        };
    }
}
