using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.DemoScripts
{
    public interface IDemoScript
    {
        public string FileName { get; }
        public string Contents { get; }
    }
}
