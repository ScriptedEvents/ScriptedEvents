using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Conditions.Interfaces
{
    public interface IStringCondition : ICondition
    {
        public bool Execute(string left, string right);
    }
}
