using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.API.Features.Actions
{
    public interface IHelpInfo
    {
        public string Description { get; }
        public Argument[] ExpectedArguments { get; }
    }

    public class Argument
    {
        public Argument(string argumentName, Type type, string description, bool required)        {
            ArgumentName = argumentName;
            Type = type;
            Description = description;
            Required = required;
        }

        public string ArgumentName { get; }
        public Type Type { get; }
        public string Description { get; }
        public bool Required { get; }

        public string TypeString => Type.Name;
    }
}
