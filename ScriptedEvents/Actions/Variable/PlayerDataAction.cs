namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using ScriptedEvents.Variables.Interfaces;

    public class PlayerDataAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "PLAYERDATA";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => "Allows manipulation of variables specific to players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode to use (GET/SET/DELETE).", true),
            new Argument("players", typeof(Player[]), "The players to affect.", true),
            new Argument("keyName", typeof(string), "The name of the key.", true),
            new Argument("value", typeof(string), "GET - The variable to create containing the value of the accessed key, SET - The new value of the key, DELETE - N/A", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 3) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[1], null, out PlayerCollection players, script))
                return new(false, players.Message);

            switch (Arguments[0].ToUpper())
            {
                case "GET" when Arguments.Length < 4:
                case "SET" when Arguments.Length < 4:
                    return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);
                case "GET":
                    if (players.Length > 1)
                        return new(false, "The 'GET' mode of the PLAYERDATA action only works on variables with exactly one player!");
                    string varName = Arguments[3];
                    Player ply1 = players[0];
                    if (ply1.SessionVariables.ContainsKey(Arguments[2]))
                        VariableSystem.DefineVariable(varName, $"The result of a PLAYERDATA execution using 'GET' on player '{ply1.DisplayNickname}' with key '{varName}'.", ply1.SessionVariables[Arguments[2]].ToString(), script);
                    else
                        VariableSystem.DefineVariable(varName, $"The result of a PLAYERDATA execution using 'GET' on player '{ply1.DisplayNickname}' with key '{varName}'.", "NONE", script);
                    break;
                case "SET":
                    foreach (Player ply in players)
                    {
                        if (ply.SessionVariables.ContainsKey(Arguments[2]))
                            ply.SessionVariables[Arguments[2]] = string.Join(" ", Arguments.Skip(3));
                        else
                            ply.SessionVariables.Add(Arguments[2], string.Join(" ", Arguments.Skip(3)));
                    }

                    break;
                case "DELETE":
                    foreach (Player ply in players)
                    {
                        if (ply.SessionVariables.ContainsKey(Arguments[2]))
                            ply.SessionVariables.Remove(Arguments[2]);
                    }

                    break;
            }

            return new(true);
        }
    }
}
