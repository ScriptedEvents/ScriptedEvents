namespace ScriptedEvents.Actions
{
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;

    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class GlobalPlayerVariableAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "GLOBALPLAYERVAR";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "GPVAR" };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Allows manipulation of player variables.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("SET", "Saves a new player variable."),
                new("ADD", "Adds player(s) to an established player variable. If a given variable doesn't exist, a new one will be created."),
                new("REMOVE", "Removes player(s) from an established player variable.")),
            new Argument("variableName", typeof(string), "The name of the variable.", true),
            new Argument("players", typeof(PlayerCollection), "The players to set/add/remove.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string mode = Arguments[0].ToUpper();
            string varName = RawArguments[1];
            PlayerCollection players = (PlayerCollection)Arguments[2];

            switch (mode)
            {
                case "SET":
                    VariableSystemV2.DefineVariable(varName, "Script-defined variable", players.GetInnerList(), script);
                    break;

                case "ADD":
                    if (VariableSystemV2.DefinedPlayerVariables.TryGetValue(varName, out CustomPlayerVariable var))
                    {
                        var.Add(players.GetArray());
                    }
                    else
                    {
                        VariableSystemV2.DefineVariable(varName, "Script-defined variable", players.GetInnerList(), script);
                    }

                    break;

                case "REMOVE":
                    if (!VariableSystemV2.DefinedPlayerVariables.TryGetValue(varName, out CustomPlayerVariable var2))
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