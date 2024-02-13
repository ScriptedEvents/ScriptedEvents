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
            new HeavilyModded(),
            new IP(),
            new Port(),
            new MaxPlayers(),
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
            new Argument("mode", typeof(string), "The mode (IP/PORT/NAME/MAXPLAYERS)", true),
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
                    _ => throw new ArgumentException("No mode provided")
                };
            }
        }
    }

    public class HeavilyModded : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{HEAVILYMODDED}";

        /// <inheritdoc/>
        public string ReversedName => "{!HEAVILYMODDED}";

        /// <inheritdoc/>
        public string Description => "Whether or not this server is heavily modded.";

        /// <inheritdoc/>
        public bool Value => Server.IsHeavilyModded;
    }

    public class IP : IStringVariable
    {
        /// <inheritdoc/>
        public string Name => "{IP}";

        /// <inheritdoc/>
        public string Description => "This server's IP address.";

        /// <inheritdoc/>
        public string Value => Server.IpAddress;
    }

    public class ServerName : IStringVariable
    {
        /// <inheritdoc/>
        public string Name => "{SERVERNAME}";

        /// <inheritdoc/>
        public string Description => "This server's name";

        /// <inheritdoc/>
        public string Value => Server.Name;
    }

    public class Port : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{PORT}";

        /// <inheritdoc/>
        public string Description => "This server's port.";

        /// <inheritdoc/>
        public float Value => Server.Port;
    }

    public class MaxPlayers : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{MAXPLAYERS}";

        /// <inheritdoc/>
        public string Description => "This server's maximum player count.";

        /// <inheritdoc/>
        public float Value => Server.MaxPlayerCount;
    }

    public class Verified : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{VERIFIED}";

        /// <inheritdoc/>
        public string ReversedName => "{!VERIFIED}";

        /// <inheritdoc/>
        public string Description => "Whether or not this server is verified.";

        /// <inheritdoc/>
        public bool Value => CustomNetworkManager.IsVerified;
    }
#pragma warning restore SA1402 // File may only contain a single type
}
