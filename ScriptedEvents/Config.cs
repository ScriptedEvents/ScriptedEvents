namespace ScriptedEvents
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Exiled.API.Interfaces;

    using ScriptedEvents.Structures;

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

        [Description("The name of the folder which will store variables long term. This can be a path, but it will start from the ScriptedEvents directory, not root.")]
        public string StorageFoldername { get; set; } = "VariableStorage";

        [Description("The string to use for countdowns.")]
        public string CountdownString { get; set; } = "<size=26><color=#5EB3FF><b>{TEXT}</b></color></size>\\n{TIME}";

        [Description("The amount of times the WAITUNTIL action will update per second. Can cause performace issues if set too high.")]
        public float WaitUntilFrequency { get; set; } = 2f;

        [Description("Define a custom set of permissions used to run a certain script. The provided permission will be added AFTER script.execute (eg. script.execute.examplepermission for the provided example).")]
        public Dictionary<string, string> RequiredPermissions { get; set; } = new()
        {
            { "ExampleScriptNameHere", "examplepermission" },
        };

        [Description("[ADVANCED] Define a custom command to run a script when it is executed.")]
        public List<CustomCommand> Commands { get; set; } = new()
        {
            new CustomCommand()
            {
                Name = "example",
                Enabled = false,
                Description = "An example custom command!",
                Type = API.Enums.CommandType.PlayerConsole,
                Permission = "example",
                DefaultResponse = true,
                Cooldown = -1,
                PlayerCooldown = -1,
                Run = new() { "MyScript1", "MyScript2" },
            },
        };
    }
}
