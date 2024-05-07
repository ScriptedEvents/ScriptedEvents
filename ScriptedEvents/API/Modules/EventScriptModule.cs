namespace ScriptedEvents.API.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.Events.Features;
    using Exiled.Loader;

    using ScriptedEvents.Structures;

    public class EventScriptModule : SEModule
    {
        /// <summary>
        /// Gets an array of Event "Handler" types defined by Exiled.
        /// </summary>
        public static Type[] HandlerTypes { get; private set; }

        public override string Name => "EventScriptModule";

        public List<Tuple<PropertyInfo, Delegate>> StoredDelegates { get; } = new();

        public Dictionary<string, List<string>> CurrentEventData { get; set; }

        public Dictionary<string, List<string>> CurrentCustomEventData { get; set; }

        public override void Init()
        {
            base.Init();

            HandlerTypes = Loader.Plugins.First(plug => plug.Name == "Exiled.Events")
            .Assembly.GetTypes().Where(t => t.FullName.Equals($"Exiled.Events.Handlers.{t.Name}")).ToArray();

            // Events
            Exiled.Events.Handlers.Server.RestartingRound += TerminateConnections;
            Exiled.Events.Handlers.Server.WaitingForPlayers += BeginConnections;
        }

        public override void Kill()
        {
            base.Kill();
            TerminateConnections();

            // Disconnect events
            Exiled.Events.Handlers.Server.RestartingRound -= TerminateConnections;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= BeginConnections;
        }

        public void BeginConnections()
        {
            if (CurrentEventData is not null)
                return;

            CurrentEventData = new();
            CurrentCustomEventData = new();

            foreach (Script scr in MainPlugin.ScriptModule.ListScripts())
            {
                if (scr.HasFlag("EVENT", out Flag f))
                {
                    string evName = f.Arguments[0];
                    if (CurrentEventData.ContainsKey(evName))
                    {
                        CurrentEventData[evName].Add(scr.ScriptName);
                    }
                    else
                    {
                        CurrentEventData.Add(evName, new List<string>() { scr.ScriptName });
                    }
                }

                if (scr.HasFlag("CUSTOMEVENT", out Flag cf))
                {
                    string cEvName = cf.Arguments[0];
                    if (CurrentCustomEventData.ContainsKey(cEvName))
                    {
                        CurrentCustomEventData[cEvName].Add(scr.ScriptName);
                    }
                    else
                    {
                        CurrentCustomEventData.Add(cEvName, new List<string>() { scr.ScriptName });
                    }
                }

                scr.Dispose();
            }

            foreach (KeyValuePair<string, List<string>> ev in CurrentEventData)
            {
                Log.Debug("Setting up new 'on' event");
                Log.Debug($"Event: {ev.Key}");
                Log.Debug($"Scripts: {string.Join(", ", ev.Value)}");
                bool made = false;
                foreach (Type handler in HandlerTypes)
                {
                    // Credit to DevTools & Yamato for below code.
                    Delegate @delegate = null;
                    PropertyInfo propertyInfo = handler.GetProperty(ev.Key);

                    if (propertyInfo is null)
                        continue;

                    EventInfo eventInfo = propertyInfo.PropertyType.GetEvent("InnerEvent", (BindingFlags)(-1));
                    MethodInfo subscribe = propertyInfo.PropertyType.GetMethods().First(x => x.Name is "Subscribe");

                    if (propertyInfo.PropertyType == typeof(Event))
                    {
                        @delegate = new CustomEventHandler(EventHandlingModule.OnNonArgumentedEvent);
                    }
                    else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Event<>))
                    {
                        @delegate = typeof(EventHandlingModule)
                            .GetMethod(nameof(EventHandlingModule.OnArgumentedEvent))
                            .MakeGenericMethod(eventInfo.EventHandlerType.GenericTypeArguments)
                            .CreateDelegate(typeof(CustomEventHandler<>)
                            .MakeGenericType(eventInfo.EventHandlerType.GenericTypeArguments));
                    }
                    else
                    {
                        Log.Warn(propertyInfo.Name);
                        continue;
                    }

                    subscribe.Invoke(propertyInfo.GetValue(MainPlugin.Handlers), new object[] { @delegate });
                    StoredDelegates.Add(new Tuple<PropertyInfo, Delegate>(propertyInfo, @delegate));

                    made = true;
                }

                if (made)
                    Log.Debug($"Dynamic event {ev.Key} connected successfully");
                else
                    Log.Debug($"Dynamic event {ev.Key} failed to be connected");
            }
        }

        public void TerminateConnections()
        {
            foreach (Tuple<PropertyInfo, Delegate> tuple in StoredDelegates)
            {
                PropertyInfo propertyInfo = tuple.Item1;
                Delegate handler = tuple.Item2;

                Log.Debug($"Removing dynamic connection for event '{propertyInfo.Name}'");

                EventInfo eventInfo = propertyInfo.PropertyType.GetEvent("InnerEvent", (BindingFlags)(-1));
                MethodInfo unSubscribe = propertyInfo.PropertyType.GetMethods().First(x => x.Name is "Unsubscribe");

                unSubscribe.Invoke(propertyInfo.GetValue(MainPlugin.Handlers), new[] { handler });
                Log.Debug($"Removed dynamic connection for event '{propertyInfo.Name}'");
            }

            StoredDelegates.Clear();
            CurrentEventData = null;
            CurrentCustomEventData = null;
        }
    }
}
