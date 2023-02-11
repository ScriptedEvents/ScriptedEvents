namespace ScriptedEvents.Actions
{
    using System;
    using ScriptedEvents.Actions.Interfaces;

    // Represents a line in a file that does not have any actions.
    public class NullAction : IAction, IHiddenAction, ICustomReadDisplay
    {
        public string Name => "NULL";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Type { get; }

        public NullAction()
        {
            Type = "UNKNOWN";
        }

        public NullAction(string type)
        {
            Type = type;
        }

        public bool Read(out string display)
        {
            display = $"NULL <{Type}>";
            return MainPlugin.Singleton.Config.Debug;
        }
    }
}
