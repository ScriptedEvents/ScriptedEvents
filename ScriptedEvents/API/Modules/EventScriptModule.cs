namespace ScriptedEvents.API.Modules
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.Features;
    using Exiled.Loader;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.Structures;

    public class EventScriptModule : SEModule
    {
        /// <summary>
        /// Gets an array of Event "Handler" types defined by Exiled.
        /// </summary>
        public static Type[] HandlerTypes { get; private set; }

        public static EventScriptModule Singleton { get; private set; }

        public override string Name => "EventScriptModule";

        public List<Tuple<PropertyInfo, Delegate>> StoredDelegates { get; } = new();

        public Dictionary<string, List<string>> CurrentEventData { get; set; }

        public Dictionary<string, List<string>> CurrentCustomEventData { get; set; }

        public List<string> DynamicallyConnectedEvents { get; set; } = new();

        // Connection methods
        public static void OnArgumentedEvent<T>(T ev)
            where T : IExiledEvent
        {
            Type evType = typeof(T);
            string evName = evType.Name.Replace("EventArgs", string.Empty);
            Singleton.OnAnyEvent(evName, ev);
        }

        public static void OnNonArgumentedEvent()
        {
            Singleton.OnAnyEvent(new StackFrame(2).GetMethod().Name);
        }

        public override void Init()
        {
            base.Init();
            Singleton = this;

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

            Singleton = null;
        }

        // Methods to make and destroy connections
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
                Logger.Debug("Setting up new 'on' event");
                Logger.Debug($"Event: {ev.Key}");
                Logger.Debug($"Scripts: {string.Join(", ", ev.Value)}");

                ConnectDynamicExiledEvent(ev.Key);
            }
        }

        public void ConnectDynamicExiledEvent(string key)
        {
            if (DynamicallyConnectedEvents.Contains(key)) return;

            DynamicallyConnectedEvents.Add(key);

            bool made = false;
            foreach (Type handler in HandlerTypes)
            {
                // Credit to DevTools & Yamato for below code.
                Delegate @delegate = null;
                PropertyInfo propertyInfo = handler.GetProperty(key);

                if (propertyInfo is null)
                    continue;

                EventInfo eventInfo = propertyInfo.PropertyType.GetEvent("InnerEvent", (BindingFlags)(-1));
                MethodInfo subscribe = propertyInfo.PropertyType.GetMethods().First(x => x.Name is "Subscribe");

                if (propertyInfo.PropertyType == typeof(Event))
                {
                    @delegate = new CustomEventHandler(OnNonArgumentedEvent);
                }
                else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Event<>))
                {
                    @delegate = typeof(EventScriptModule)
                        .GetMethod(nameof(OnArgumentedEvent))
                        .MakeGenericMethod(eventInfo.EventHandlerType.GenericTypeArguments)
                        .CreateDelegate(typeof(CustomEventHandler<>)
                        .MakeGenericType(eventInfo.EventHandlerType.GenericTypeArguments));
                }
                else
                {
                    Logger.Warn(propertyInfo.Name);
                    continue;
                }

                subscribe.Invoke(propertyInfo.GetValue(MainPlugin.Handlers), new object[] { @delegate });
                StoredDelegates.Add(new Tuple<PropertyInfo, Delegate>(propertyInfo, @delegate));

                made = true;
            }

            if (made)
                Logger.Debug($"Dynamic event {key} connected successfully");
            else
                Logger.Debug($"Dynamic event {key} failed to be connected");
        }

        public void TerminateConnections()
        {
            foreach (Tuple<PropertyInfo, Delegate> tuple in StoredDelegates)
            {
                PropertyInfo propertyInfo = tuple.Item1;
                Delegate handler = tuple.Item2;

                Logger.Debug($"Removing dynamic connection for event '{propertyInfo.Name}'");

                EventInfo eventInfo = propertyInfo.PropertyType.GetEvent("InnerEvent", (BindingFlags)(-1));
                MethodInfo unSubscribe = propertyInfo.PropertyType.GetMethods().First(x => x.Name is "Unsubscribe");

                unSubscribe.Invoke(propertyInfo.GetValue(MainPlugin.Handlers), new[] { handler });
                Logger.Debug($"Removed dynamic connection for event '{propertyInfo.Name}'");
            }

            StoredDelegates.Clear();
            CurrentEventData = null;
            CurrentCustomEventData = null;
            DynamicallyConnectedEvents = new();
        }

        // Code to run when connected event is executed
        public void OnAnyEvent(string eventName, IExiledEvent ev = null)
        {
            if (ev == null) return;

            Stopwatch stopwatch = new();

            stopwatch.Start();

            Type eventType = ev.GetType();

            // remove EventArgs from the end
            string name = eventType.Name.Substring(0, eventType.Name.Length - 9);
            Log.Debug($"Event '{name}' is now running.");

            if (ev is IDeniableEvent deniable && ev is IPlayerEvent plr)
            {
                bool playerIsNotNone = plr.Player is not null;

                bool isRegisteredRule = MainPlugin.Handlers.GetPlayerDisableEvent(name, plr.Player).HasValue;

                if (playerIsNotNone && isRegisteredRule)
                {
                    Log.Debug("Event is disabled.");
                    deniable.IsAllowed = false;
                    return;
                }
            }

            if (CurrentEventData is null || !CurrentEventData.TryGetValue(eventName, out List<string> scriptNames))
            {
                return;
            }

            Log.Debug("Scripts connected to this event:");
            List<Script> scripts = new();

            foreach (string scrName in scriptNames)
            {
                try
                {
                    scripts.Add(MainPlugin.ScriptModule.ReadScript(scrName, null));
                    Log.Debug($"- {scrName}.txt");
                }
                catch (DisabledScriptException)
                {
                    Logger.Warn(ErrorGen.Get(ErrorCode.On_DisabledScript, eventName, scrName));
                }
                catch (FileNotFoundException)
                {
                    Logger.Warn(ErrorGen.Get(ErrorCode.On_NotFoundScript, eventName, scrName));
                }
                catch (Exception ex)
                {
                    Logger.Warn(ErrorGen.Get(ErrorCode.On_UnknownError, eventName) + $": {ex}");
                }
            }

            PropertyInfo[] properties = ev.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                Log.Debug($"Managing property {property.Name}");
                void AddVariable(string value)
                {
                    foreach (Script script in scripts)
                    {
                        script.AddVariable("{EV" + property.Name.ToUpper() + "}", string.Empty, value);
                        Log.Debug($"Adding variable {{EV{property.Name.ToUpper()}}} to all scripts above.");
                    }
                }

                var value = property.GetValue(ev);
                if (value is null) continue;

                switch (value)
                {
                    case Player player when player is not null:
                        foreach (Script script in scripts) script.AddPlayerVariable($"{{EV{property.Name.ToUpper()}}}", string.Empty, new[] { player });
                        Log.Debug($"Adding variable {{EV{property.Name.ToUpper()}}} to all scripts above.");
                        break;

                    case Exiled.API.Features.Items.Item item when item is not null:
                        AddVariable(item.Base.ItemSerial.ToString());
                        break;

                    case Exiled.API.Features.Doors.Door door when door is not null:
                        AddVariable(door.Type.ToString());
                        break;

                    case MapGeneration.Distributors.Scp079Generator gen when gen is not null:
                        AddVariable(gen.GetInstanceID().ToString());
                        break;

                    case bool @bool:
                        AddVariable(@bool.ToUpper());
                        break;

                    default:
                        AddVariable(value.ToString());
                        break;
                }
            }

            foreach (Script script in scripts)
                script.Execute();

            stopwatch.Stop();
            Log.Debug($"Handling event '{name}' cost {stopwatch.ElapsedMilliseconds} ms");
        }
    }
}
