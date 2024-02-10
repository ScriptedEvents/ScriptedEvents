namespace ScriptedEvents.API.Features
{
    using System.Collections.Generic;

    using Exiled.API.Features;

    using MEC;

    using ScriptedEvents.Structures;

    public static class CoroutineHelper
    {
        private static readonly Dictionary<string, List<CoroutineData>> Coroutines = new();

        public static Dictionary<string, List<CoroutineData>> GetAll()
            => Coroutines;

        public static void AddCoroutine(string type, CoroutineHandle coroutine, Script source = null)
        {
            CoroutineData data = new(coroutine);
            source?.Coroutines.Add(data);
            if (Coroutines.ContainsKey(type))
                Coroutines[type].Add(data);
            else
                Coroutines.Add(type, new List<CoroutineData>() { data });

            if (coroutine.Tag is not null)
                data.Key = coroutine.Tag;

            data.Source = source;

            Log.Debug($"Added new coroutine TYPE: {type} TAG: {coroutine.Tag ?? "N/A"} SOURCE: {source?.ScriptName ?? "N/A"} (BY HANDLE)");
        }

        public static void AddCoroutine(string type, string tag, Script source = null)
        {
            CoroutineData data = new(tag);
            source?.Coroutines.Add(data);
            if (Coroutines.ContainsKey(type))
                Coroutines[type].Add(data);
            else
                Coroutines.Add(type, new List<CoroutineData>() { data });

            data.Source = source;

            Log.Debug($"Added new coroutine TYPE: {type} TAG: {tag} SOURCE: {source?.ScriptName ?? "N/A"} (BY TAG)");
        }

        public static void KillAll()
        {
            int amount = 0;
            foreach (List<CoroutineData> handleList in Coroutines.Values)
            {
                handleList.ForEach(h =>
                {
                    if (!h.IsKilled)
                    {
                        h.Kill();
                        amount++;
                    }
                });
            }

            Log.Debug($"Stopped {amount} coroutines.");

            Coroutines.Clear();
        }
    }
}
