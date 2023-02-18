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

        [Description("If a script encounters an error, broadcast a notice to the person who ran the command, informing of the error. The broadcast ONLY shows to the command executor.")]
        public bool BroadcastIssues { get; set; } = true;

        [Description("If set to true, players with overwatch enabled will not be affected by any commands related to players.")]
        public bool IgnoreOverwatch { get; set; } = true;

        [Description("List of scripts to run as soon as the round starts.")]
        public List<string> AutoRunScripts { get; set; } = new();

        [Description("List of scripts to automatically re-run as soon as they finish.")]
        public List<string> LoopScripts { get; set; } = new();

        [Description("The string to use for countdowns.")]
        public string CountdownString { get; set; } = "<size=26><color=#5EB3FF><b>{TEXT}</b></color></size>\\n{TIME}";

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
