namespace ScriptedEvents.API.Features
{
    using System.Collections.Generic;

    using MEC;

    using ScriptedEvents.Structures;

    public static class CoroutineHelper
    {
        private static readonly Dictionary<string, List<CoroutineData>> Coroutines = new();

        public static void AddCoroutine(string type, CoroutineHandle coroutine, Script source = null)
        {
            CoroutineData data = new(coroutine);
            source?.Coroutines.Add(data);
            if (Coroutines.ContainsKey(type))
                Coroutines[type].Add(data);
            else
                Coroutines.Add(type, new List<CoroutineData>() { data });
        }

        public static void AddCoroutine(string type, string tag, Script source = null)
        {
            CoroutineData data = new(tag);
            source?.Coroutines.Add(data);
            if (Coroutines.ContainsKey(type))
                Coroutines[type].Add(data);
            else
                Coroutines.Add(type, new List<CoroutineData>() { data });
        }

        public static void KillAll()
        {
            foreach (List<CoroutineData> handleList in Coroutines.Values)
            {
                handleList.ForEach(h =>
                {
                    if (!h.IsKilled)
                        h.Kill();
                });
            }

            Coroutines.Clear();
        }
    }
}
