namespace ScriptedEvents.API.Features
{
    using System.Collections.Generic;

    using MEC;

    using ScriptedEvents.Structures;

    public static class CoroutineHelper
    {
        private static Dictionary<string, List<CoroutineData>> coroutines;

        public static void AddCoroutine(string type, CoroutineHandle coroutine)
        {
            if (coroutines.ContainsKey(type))
                coroutines[type].Add(new(coroutine));
            else
                coroutines.Add(type, new List<CoroutineData>() { new(coroutine) });
        }

        public static void AddCoroutine(string type, string tag)
        {
            if (coroutines.ContainsKey(type))
                coroutines[type].Add(new(tag));
            else
                coroutines.Add(type, new List<CoroutineData>() { new(tag) });
        }

        public static void KillAll()
        {
            foreach (List<CoroutineData> handleList in coroutines.Values)
                handleList.ForEach(h => h.Kill());

            coroutines.Clear();
        }
    }
}
