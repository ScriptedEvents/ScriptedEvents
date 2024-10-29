namespace ScriptedEvents.Actions.AllInOne
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class ServerInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "ServerInfo";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting server related information.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new OptionValueDepending("IP", "Server IP.", typeof(string)),
                new OptionValueDepending("Port", "Server port.", typeof(ushort)),
                new OptionValueDepending("Name", "Server name.", typeof(string)),
                new OptionValueDepending("MaxPlayers", "Maximum amount of players the server is set to.", typeof(int)),
                new OptionValueDepending("TPS", "Amount of ticks per second the server is currently running on. Changes over time.", typeof(double))),
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

            return new(true, new(ret));
        }
    }
}