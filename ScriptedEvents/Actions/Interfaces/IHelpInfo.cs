using Exiled.API.Features;
using PlayerRoles;
using ScriptedEvents.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Actions.Interfaces
{
    public interface IHelpInfo
    {
        public string Description { get; }
        public Argument[] ExpectedArguments { get; }
    }
}
