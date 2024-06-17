namespace ScriptedEvents.Actions
{
    using System.Collections.Generic;
//data:image/jpeg;base64,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

    using Exiled.API.Features;
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
            new Argument("players", typeof(PlayerCollection), "The players to set/add/remove.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string mode = Arguments[0].ToUpper();
            string varName = RawArguments[1];
            PlayerCollection players = Arguments.Length > 2 ? (PlayerCollection)Arguments[2] : new(new List<Player>());

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