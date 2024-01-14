namespace ScriptedEvents.Integrations
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;

    public class RueIManager
    {
        private const string RUEINAME = "RueI";
        private const string RUEIMAIN = "RueI.RueIMain";
        private const string ENSUREINIT = "EnsureInit";
        private const string REFLECTIONHELPERS = "RueI.Extensions.ReflectionHelpers";
        private const string GETELEMENTSHOWER = "GetElementShower";

        private Action<ReferenceHub, string, float, TimeSpan> shower;

        public RueIManager()
            => MakeNew();

        private static Assembly RueiAssembly { get; } = Exiled.Loader.Loader.Dependencies.FirstOrDefault(x => x.GetName().Name == RUEINAME);

        public void MakeNew()
        {
            if (RueiAssembly == null)
            {
                return;
            }

            MethodInfo elementShower = RueiAssembly?.GetType(REFLECTIONHELPERS)?.GetMethod(GETELEMENTSHOWER);
            var result = elementShower?.Invoke(null, new object[] { });

            if (result is not Action<ReferenceHub, string, float, TimeSpan> elemShower)
            {
                return;
            }

            MethodInfo init = RueiAssembly?.GetType(RUEIMAIN)?.GetMethod(ENSUREINIT);
            if (init == null)
            {
                return;
            }

            init.Invoke(null, new object[] { });
            shower = elemShower;
        }

        public void ShowHint(Player player, string content, TimeSpan span)
        {
            if (shower != null)
            {
                shower(player.ReferenceHub, content, 0, span);
            }
            else
            {
                player.ShowHint(content, (float)span.TotalSeconds);
            }
        }
    }
}