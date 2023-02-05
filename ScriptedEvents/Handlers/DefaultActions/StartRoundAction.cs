using Exiled.API.Features;
using ScriptedEvents.API.Features.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Actions
{
    public class StartRoundAction : IAction
    {
        public string Name => "START";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            Round.Start();
            return new(true);
        }
    }
}