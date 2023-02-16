namespace ScriptedEvents.Variables.Condition
{
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class ServerInfoVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Server Info";

        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Condition;

        /// <inheritdoc/>
        public IVariable[] Variables => new IVariable[]
        {
            new IP(),
            new Port(),
        };
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
}
