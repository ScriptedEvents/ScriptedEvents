namespace ScriptedEvents.Structures
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;

    public class ActionReturnValues
    {
        public ActionReturnValues(string value)
        {
            Values = new[] { value };
        }

        public ActionReturnValues(Player player)
        {
            Values = new[] { new[] { player } };
        }

        public ActionReturnValues(IEnumerable<Player> players)
        {
            Values = new[] { players as Player[] ?? players.ToArray() };
        }

        public object[] Values { get; set; }
    }
}