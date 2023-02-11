using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.API.Features.Actions
{
    public interface ICustomReadDisplay
    {
        public bool Read(out string message);
    }
}
