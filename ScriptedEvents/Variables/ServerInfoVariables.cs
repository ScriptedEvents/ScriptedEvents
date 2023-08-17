namespace ScriptedEvents.Variables.ServerInfo
{
#pragma warning disable SA1402 // File may only contain a single type
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
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
        };
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

    public class Port : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{PORT}";

        /// <inheritdoc/>
        public string Description => "This server's PORT.";

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
#pragma warning restore SA1402 // File may only contain a single type
}
