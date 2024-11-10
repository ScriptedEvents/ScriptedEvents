namespace ScriptedEvents.Structures
{
    using System;
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
        
        public ActionReturnValues(IEnumerable<object> objects)
        {
            var objectsAsArray = objects as object[] ?? objects.ToArray();
            if (objectsAsArray.Any(value => value is not Exiled.Events.Handlers.Player or Player[] or string))
            {
                throw new Exception();
            }

            Values = objectsAsArray;
        }

        public object[] Values { get; set; }
    }
}