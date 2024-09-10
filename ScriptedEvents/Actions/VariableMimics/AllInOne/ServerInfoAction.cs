namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class ServerInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "SERVERINFO";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting server related information.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("IP", "Returns the IP of the server."),
                new("PORT", "Returns the port of the server."),
                new("NAME", "Returns the name of the server."),
                new("MAXPLAYERS", "Returns the maximum amount of players the server is set to."),
                new("TPS", "Returns the amount of ticks per second the server is running on.")),
        };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.AllInOneInfo;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string ret = Arguments[0].ToUpper() switch
            {
                "IP" => Server.IpAddress,
                "PORT" => Server.Port.ToString(),
                "NAME" => Server.Name,
                "MAXPLAYERS" => Server.MaxPlayerCount.ToString(),
                "TPS" => Server.Tps.ToString(),
                _ => throw new ArgumentException("No mode provided")
            };

            return new(true, variablesToRet: new[] { ret });
        }
    }
}