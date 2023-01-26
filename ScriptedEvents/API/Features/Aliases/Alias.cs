namespace ScriptedEvents.API.Features.Aliases
{
    public class Alias
    {
        public string Command { get; set; } = "BROADCAST";
        public string Execute { get; set; } = "COMMAND /bc";
        // args exist for a documentation or something generator, that could help the user understand the plugin's actions
        // in the future
        public string[] Arguments { get; set; } = new[] { "DURATION", "MESSAGE" }; 

        public Alias() { }
        public Alias(string command, string execute, params string[] args)
        {
            Command = command;
            Execute = execute;
            Arguments = args;
        }

        public string Unalias(string usage)
            => usage.Replace(Command, Execute);

        public override string ToString()
            => $"{Command} [{string.Join("] [", Arguments)}]";
    }
}
