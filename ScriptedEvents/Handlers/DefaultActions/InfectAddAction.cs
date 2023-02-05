using PlayerRoles;
using ScriptedEvents.API.Features.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class InfectAddAction : IAction
    {
        public string Name => "INFECTADD";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            if (Arguments.Length < 3) return new(false, "Missing arguments: oldrole, newrole, moveplayer");

            if (!Enum.TryParse(Arguments[0], true, out RoleTypeId oldRole))
                return new(false, $"Invalid old role '{Arguments[1]}' provided.");

            if (!Enum.TryParse(Arguments[1], true, out RoleTypeId newRole))
                return new(false, $"Invalid new role '{Arguments[1]}' provided.");

            bool movePlayer = (Arguments[2].ToUpper() is "TRUE" or "YES" ? true : false);

            MainPlugin.Handlers.InfectionRules.RemoveAll(rule => rule.OldRole == oldRole);
            MainPlugin.Handlers.InfectionRules.Add(new(oldRole, newRole, movePlayer));

            return new(true);
        }
    }
}
