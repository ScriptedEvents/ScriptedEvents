using Exiled.API.Features;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Variable
{
    public class PlayerDataAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "PlrData";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "PData" };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Allows manipulation of values tied to specific players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new Option("Set", "Sets the player data."),
                new Option("Remove", "Deletes the player data.")),
            new Argument("players", typeof(PlayerCollection), "The players to affect.", true),
            new Argument("keyName", typeof(string), "The name of the key.", true),
            new Argument("value", typeof(string), "The new value of the key. Not used when using mode 'DELETE'.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[1]!;
            string keyName = (string)Arguments[2]!;
            switch (((string)Arguments[0]!).ToUpper())
            {
                case "SET" when Arguments.Length < 4:
                    var err = new ErrorInfo(
                        "Value not provided",
                        "When using 'Set' mode, value needs to be provided.",
                        Name).ToTrace();
                    return new(false, null, err);
                

                case "SET":
                    foreach (Player ply in players)
                    {
                        ply.PlayerDataVariables()[keyName] = Arguments.JoinMessage(3);
                    }
                    break;
                
                case "DELETE":
                    foreach (Player ply in players)
                    {
                        ply.PlayerDataVariables().Remove(keyName);
                    }
                    break;
            }

            return new(true);
        }
    }
}
