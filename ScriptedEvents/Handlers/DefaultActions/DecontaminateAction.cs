using Exiled.API.Features;
using ScriptedEvents.API.Features.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Actions
{
    public class DecontaminateAction : IAction
    {
        public string Name => "DECONTAMINATE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            Map.StartDecontamination();
            return new(true);
        }
    }
}