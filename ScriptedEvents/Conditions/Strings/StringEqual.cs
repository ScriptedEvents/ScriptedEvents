using ScriptedEvents.Conditions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Conditions.Strings
{
    public class StringEqual : IStringCondition
    {
        public string Symbol => "=";

        public bool Execute(string left, string right)
        {
            return left.Equals(right);
        }
    }
}
