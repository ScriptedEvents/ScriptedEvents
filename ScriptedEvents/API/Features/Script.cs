using System;
using System.Collections.Generic;
using ScriptedEvents.API.Features.Actions;

namespace ScriptedEvents.API.Features
{
    public class Script
    {
        public Script()
        {
            UniqueId = Guid.NewGuid();
        }

        public Guid UniqueId { get; }
        public string ScriptName { get; set; } = string.Empty;
        public string RawText { get; set; } = string.Empty;
        public Queue<IAction> Actions { get; set; } = new();
        public bool IsRunning { get; internal set; } = false;
    }
}
