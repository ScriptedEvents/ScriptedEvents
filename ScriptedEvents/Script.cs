using System;
using System.Collections.Generic;
using ScriptedEvents.API.Features.Actions;

namespace ScriptedEvents
{
    public class Script
    {
        public Script()
        {
            UniqueId = Guid.NewGuid();
        }

        public Guid UniqueId { get; }
        public string ScriptName { get; set; } = string.Empty;
        public string ReadPermission { get; set; } = "script.read";
        public string ExecutePermission { get; set; } = "script.execute";
        public string FilePath { get; set; }
        public DateTime LastRead { get; set; }
        public DateTime LastEdited { get; set; }
        public string RawText { get; set; } = string.Empty;
        public List<IAction> Actions { get; set; } = new();
        public int CurrentLine { get; set; }
        public bool IsRunning { get; internal set; } = false;
        public List<string> Flags { get; set; } = new();
        public bool Disabled => Flags.Contains("DISABLE");
        public bool AdminEvent => Flags.Contains("ADMINEVENT");

        public void Jump(int line)
        {
            CurrentLine = line - 2;
        }
    }
}
