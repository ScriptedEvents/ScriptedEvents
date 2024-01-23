namespace ScriptedEvents.API.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ScriptedEvents.Structures;

    public static class ListExtensions
    {
        public static bool Add(this List<Flag> flagList, string key)
        {
            if (!flagList.Contains(key))
            {
                flagList.Add(new(key, Array.Empty<string>()));
                return true;
            }

            return false;
        }

        public static bool Contains(this List<Flag> flagList, string key)
        {
            return flagList.Any(fl => fl.Key == key);
        }

        public static bool TryGet(this List<Flag> flagList, string key, out Flag flag)
        {
            flag = flagList.FirstOrDefault(fl => fl.Key == key);
            return flag.Key is not null;
        }
    }
}
