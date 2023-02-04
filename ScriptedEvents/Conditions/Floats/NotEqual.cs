using ScriptedEvents.Conditions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Conditions.Floats
{
    public class NotEqual : IFloatCondition
    {
        public string Symbol => "!=";

        public bool Execute(float left, float right) => left != right;
    }
}
