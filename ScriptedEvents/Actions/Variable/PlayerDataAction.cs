namespace ScriptedEvents.Actions
{
    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    public class PlayerDataAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "PLAYERDATA";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "PDATA" };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Allows manipulation of variables specific to players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("SET", "Sets the player data."),
                new("DELETE", "Deletes the player data.")),
            new Argument("players", typeof(PlayerCollection), "The players to affect.", true),
            new Argument("keyName", typeof(string), "The name of the key.", true),
            new Argument("value", typeof(string), "The new value of the key. Not used when using mode 'DELETE'.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[1];

            switch (((string)Arguments[0]).ToUpper())
            {
                case "SET" when Arguments.Length < 4:
                    return new(false, "guh");

                case "SET":
                    string keyName = (string)Arguments[2];
                    foreach (Player ply in players)
                    {
                        if (ply.SessionVariables.ContainsKey(keyName))
                            ply.SessionVariables[keyName] = Arguments.JoinMessage(3);
                        else
                            ply.SessionVariables.Add(keyName, Arguments.JoinMessage(3));
                    }

                    break;
                case "DELETE":
                    keyName = (string)Arguments[2];
                    foreach (Player ply in players)
                    {
                        if (ply.SessionVariables.ContainsKey(keyName))
                            ply.SessionVariables.Remove(keyName);
                    }

                    break;
            }

            return new(true);
        }
    }
}
