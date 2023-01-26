using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Actions
{
    public interface IAction
    {
        string Name { get; }
        string[] Aliases { get; }
        string[] Arguments { get; set; }
        ActionResponse Execute();
    }
}
