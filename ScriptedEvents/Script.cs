using ScriptedEvents.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents
{
    public class Script
    {
        public string ScriptName { get; set; } = "";
        public Queue<IAction> Actions { get; set; } = new();
    }
}
