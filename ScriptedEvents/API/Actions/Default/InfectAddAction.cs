using PlayerRoles;
using ScriptedEvents.API.Features.Actions;
using System;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class InfectAddAction : IScriptAction, IHelpInfo
    {
        public string Name => "INFECTADD";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Creates a new infection rule.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("oldRole", typeof(RoleTypeId), "The role a player must die as to be infected.", true),
            new Argument("newRole", typeof(RoleTypeId), "The role a player will become.", true),
            new Argument("movePlayer", typeof(bool), "TRUE if the player should be moved to their death position, FALSE (or leave empty) to leave at spawn.", false),
        };

        public ActionResponse Execute(Script script)
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
