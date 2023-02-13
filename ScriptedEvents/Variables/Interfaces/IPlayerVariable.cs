namespace ScriptedEvents.Variables.Interfaces
{
    using System.Collections.Generic;
    using Exiled.API.Features;

    public interface IPlayerVariable : IVariable
    {
        public IEnumerable<Player> Players { get; }
    }
}
