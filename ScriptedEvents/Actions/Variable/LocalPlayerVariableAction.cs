namespace ScriptedEvents.Actions
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;

    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class LocalPlayerVariableAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "LOCALPLAYERVAR";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "LPVAR" };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Allows manipulation of player variables accessible in this script only.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("SET", "Saves a new player variable."),
                new("ADD", "Adds player(s) to an established player variable. If a given variable doesn't exist, a new one will be created."),
                new("REMOVE", "Removes player(s) from an established player variable.")),
            new Argument("variableName", typeof(string), "The name of the variable.", true),
            new Argument("players", typeof(PlayerCollection), "The players to set/add/remove.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string mode = ((string)Arguments[0]).ToUpper();
            string varName = RawArguments[1];
            PlayerCollection players = Arguments.Length > 2 ? (PlayerCollection)Arguments[2] : new(new List<Player>());

            switch (mode)
            {
                case "SET":
                    script.AddPlayerVariable(varName, "Script-defined variable", players.GetInnerList());
                    break;

                case "ADD":
                    if (script.UniquePlayerVariables.TryGetValue(varName, out CustomPlayerVariable var))
                    {
                        var.Add(players.GetArray());
                    }
                    else
                    {
                        script.AddPlayerVariable(varName, "Script-defined variable", players.GetInnerList());
                    }

                    break;

                case "REMOVE":
                    if (!script.UniquePlayerVariables.TryGetValue(varName, out CustomPlayerVariable var2))
                        return new(false, $"'{varName}' is not a valid variable.");

                    var2.Remove(players.GetArray());
                    break;

                default:
                    return new(false);
            }

            return new(true);
        }
    }
}