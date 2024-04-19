namespace ScriptedEvents.Variables.ServerInfo
{
    using System;
#pragma warning disable SA1402 // File may only contain a single type

    using Exiled.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class ServerInfoVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Server Info";

        /// <inheritdoc/>
        public IVariable[] Variables => new IVariable[]
        {
            new DefaultServer(),
        };
    }

    public class DefaultServer : IStringVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{SERVER}";

        /// <inheritdoc/>
        public string Description => "All-in-one variable for server related information.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true, new("IP"), new("PORT"), new("NAME"), new("MAXPLAYERS"), new("TPS"), new("VERIFIED")),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                string mode = (string)Arguments[0];

                return mode.ToUpper() switch
                {
                    "IP" => Server.IpAddress,
                    "PORT" => Server.Port.ToString(),
                    "NAME" => Server.Name,
                    "MAXPLAYERS" => Server.MaxPlayerCount.ToString(),
                    "TPS" => Server.Tps.ToString(),
                    _ => throw new ArgumentException("No mode provided")
                };
            }
        }
    }
#pragma warning restore SA1402 // File may only contain a single type
}
