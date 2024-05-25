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
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Features;
    using Exiled.Loader;

    using ScriptedEvents.API.Enums;
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

        // Code to run when connected event is executed
        public void OnAnyEvent(string eventName, IExiledEvent ev = null)
        {
            if (CurrentEventData is null || !CurrentEventData.TryGetValue(eventName, out List<string> scripts))
            {
                return;
            }

            foreach (string script in scripts)
            {
                try
                {
                    Script scr = MainPlugin.ScriptModule.ReadScript(script, null);

                    // Add variables based on event.
                    if (ev is IPlayerEvent playerEvent && playerEvent.Player is not null)
                    {
                        scr.AddPlayerVariable("{EVPLAYER}", "The player that is involved with this event.", new[] { playerEvent.Player });
                    }

                    if (ev is IAttackerEvent attackerEvent && attackerEvent.Attacker is not null)
                    {
                        scr.AddPlayerVariable("{EVATTACKER}", "The attacker that is involved with this event.", new[] { attackerEvent.Attacker });
                    }

                    if (ev is IItemEvent item && item.Item is not null)
                    {
                        scr.AddVariable("{EVITEM}", "The Id of the ItemType of the item involved with this event.", item.Item.Base.ItemSerial.ToString());
                    }
                    else if (ev is IPickupEvent pickup && pickup.Pickup is not null)
                    {
                        scr.AddVariable("{EVITEM}", "The Id of the ItemType of the pickup associated with this event.", pickup.Pickup.Serial.ToString());
                    }

                    if (ev is BanningEventArgs ban)
                    {
                        scr.AddPlayerVariable("{EVPLAYER}", "The ban issuer.", new[] { ban.Player });
                        scr.AddVariable("{EVREASON}", "The ban reason.", ban.Reason.ToString());
                        scr.AddVariable("{EVDURATION}", "The ban duration.", ban.Duration.ToString());
                        scr.AddVariable("{EVTARGET}", "The ban target.", ban.Target.Nickname);
                    }

                    if (ev is LocalReportingEventArgs rep)
                    {
                        scr.AddPlayerVariable("{EVPLAYER}", "The report issuer.", new[] { rep.Player });
                        scr.AddVariable("{EVREASON}", "The report reason.", rep.Reason.ToString());
                        scr.AddPlayerVariable("{EVTARGET}", "The report target.", new[] { rep.Target });
                    }

                    if (ev is KickingEventArgs kick)
                    {
                        scr.AddPlayerVariable("{EVPLAYER}", "The issuer.", new[] { kick.Player });
                        scr.AddVariable("{EVREASON}", "The reason.", kick.Reason.ToString());
                        scr.AddPlayerVariable("{EVTARGET}", "The target.", new[] { kick.Target });
                    }

                    if (ev is HurtingEventArgs hurting)
                    {
                        scr.AddVariable("{EVAMMOUNT}", "The amount of damage dealt.", hurting.Amount.ToString());
                    }

                    if (ev is IDoorEvent door && door.Door is not null)
                    {
                        scr.AddVariable("{EVDOOR}", "The door type.", door.Door.Type.ToString());
                    }

                    MainPlugin.ScriptModule.RunScript(scr);
                }
                catch (DisabledScriptException)
                {
                    Log.Warn(ErrorGen.Get(ErrorCode.On_DisabledScript, eventName, script));
                }
                catch (FileNotFoundException)
                {
                    Log.Warn(ErrorGen.Get(ErrorCode.On_NotFoundScript, eventName, script));
                }
                catch (Exception ex)
                {
                    Log.Warn(ErrorGen.Get(ErrorCode.On_UnknownError, eventName) + $": {ex}");
                }
            }
        }
    }
}
