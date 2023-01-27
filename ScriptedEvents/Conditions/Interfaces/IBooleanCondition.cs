using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Conditions.Interfaces
{
    public interface IBooleanCondition
    {
        public char Symbol { get; }
        public bool Execute(float left, float right);
    }
}
