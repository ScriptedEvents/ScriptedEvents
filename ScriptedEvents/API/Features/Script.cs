using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScriptedEvents;
using ScriptedEvents.API.Features.Actions;

namespace ScriptedEvents.API.Features
{
    public class Script
    {
        public string ScriptName { get; set; } = string.Empty;
        public string RawText { get; set; } = string.Empty;
        public Queue<IAction> Actions { get; set; } = new();
        public bool IsRunning { get; internal set; } = false;
    }
}
