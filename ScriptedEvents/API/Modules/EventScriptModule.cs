namespace ScriptedEvents.API.Modules
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.Features;
    using Exiled.Loader;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using Structures;

    public class EventScriptModule : SEModule
    {
        /// <summary>
        /// Gets an array of Event "Handler" types defined by Exiled.
        /// </summary>
        private static Type[] HandlerTypes { get; set; }

        public static EventScriptModule? Singleton { get; private set; }

        public override string Name => "EventScriptModule";

        private List<Tuple<PropertyInfo, Delegate>> StoredDelegates { get; } = new();

        private Dictionary<string, List<string>> CurrentEventData { get; set; } = new();

        public Dictionary<string, List<string>> CurrentCustomEventData { get; private set; } = new();

        private List<string> DynamicallyConnectedEvents { get; set; } = new();
        
        private void OnArgumentedEvent<T>(T ev)
            where T : IExiledEvent
        {
            Type evType = typeof(T);
            string evName = evType.Name.Replace("EventArgs", string.Empty);
            Singleton!.OnAnyEvent(evName, ev);
        }

        private void OnNonArgumentedEvent()
        {
            Singleton!.OnAnyEvent(new StackFrame(2).GetMethod().Name);
        }

        public override void Init()
        {
            base.Init();
            Singleton = this;

            try
            {
                HandlerTypes = Loader.Plugins.First(plug => plug.Name == "Exiled.Events")
                    .Assembly.GetTypes()
                    .Where(t => t?.FullName != null && t.FullName.Equals($"Exiled.Events.Handlers.{t.Name}")).ToArray();
            }
            catch (Exception)
            {
                Logger.Error($"Fetching HandlerTypes failed! Exiled.Events does not exist in loaded plugins:\n{string.Join(", ", Loader.Plugins.Select(x => x.Name))}");
            }

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
        private void BeginConnections()
        {
            CurrentEventData = new();
            CurrentCustomEventData = new();

            var scripts = ScriptModule.Singleton!.ListScripts().ToArray();
            RegisterEventScripts(scripts, out var eventScripts);

            scripts = scripts.Where(scr => !eventScripts.Contains(scr)).ToArray();

            foreach (var script in eventScripts)
            {
                script.Dispose();
            }

            ScriptModule.Singleton.RegisterAutorunScripts(scripts, out var autoRunScripts);

            foreach (var script in scripts.Where(scr => !autoRunScripts.Contains(scr)))
            {
                script.Dispose();
            }

            if (ShouldGenerateFiles)
            {
                Logger.Info($"Thank you for installing Scripted Events! View the README file located at {Path.Combine(MainPlugin.BaseFilePath, "README.txt")} for information on how to use and get the most out of this plugin.");
            }
        }

        private void RegisterEventScripts(Script[] allScripts, out List<Script> activeScripts)
        {
            activeScripts = new();
            if (CurrentEventData is null || CurrentCustomEventData is null) throw new NullReferenceException();

            foreach (Script scr in allScripts)
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

                    activeScripts.Add(scr);
                    continue;
                }

                if (!scr.HasFlag("CUSTOMEVENT", out Flag cf)) continue;
                
                string cEvName = cf.Arguments[0];
                if (CurrentCustomEventData.ContainsKey(cEvName))
                {
                    CurrentCustomEventData[cEvName].Add(scr.ScriptName);
                }
                else
                {
                    CurrentCustomEventData.Add(cEvName, new List<string>() { scr.ScriptName });
                }

                activeScripts.Add(scr);
            }

            foreach (var ev in CurrentEventData)
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
                Delegate? @delegate;
                PropertyInfo? propertyInfo = handler.GetProperty(key);

                if (propertyInfo is null)
                    continue;

                EventInfo eventInfo = propertyInfo.PropertyType.GetEvent("InnerEvent", (BindingFlags)(-1)) 
                                      ?? throw new InvalidOperationException();
                MethodInfo subscribe = propertyInfo.PropertyType.GetMethods().First(x => x.Name is "Subscribe");

                if (propertyInfo.PropertyType == typeof(Event))
                {
                    @delegate = new CustomEventHandler(OnNonArgumentedEvent);
                }
                else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Event<>))
                {
                    @delegate = typeof(EventScriptModule)
                        .GetMethod(nameof(OnArgumentedEvent))
                        ?.MakeGenericMethod(eventInfo.EventHandlerType.GenericTypeArguments)
                        .CreateDelegate(typeof(CustomEventHandler<>)
                        .MakeGenericType(eventInfo.EventHandlerType.GenericTypeArguments));
                }
                else
                {
                    Logger.Warn(propertyInfo.Name);
                    continue;
                }

                if (@delegate is null)
                    throw new NullReferenceException("delegate is null");
                
                subscribe.Invoke(propertyInfo.GetValue(EventHandlingModule.Singleton), new object[] { @delegate });
                StoredDelegates.Add(new Tuple<PropertyInfo, Delegate>(propertyInfo, @delegate));

                made = true;
            }

            Logger.Debug(made
                ? $"Dynamic event {key} connected successfully"
                : $"Dynamic event {key} failed to be connected");
        }

        private void TerminateConnections()
        {
            foreach (var (propertyInfo, handler) in StoredDelegates)
            {
                Logger.Debug($"Removing dynamic connection for event '{propertyInfo.Name}'");
                MethodInfo unSubscribe = propertyInfo.PropertyType.GetMethods().First(x => x.Name is "Unsubscribe");

                unSubscribe.Invoke(propertyInfo.GetValue(EventHandlingModule.Singleton), new[]
                {
                    handler
                });
                Logger.Debug($"Removed dynamic connection for event '{propertyInfo.Name}'");
            }

            StoredDelegates.Clear();
            CurrentEventData = new();
            CurrentCustomEventData = new();
            DynamicallyConnectedEvents = new();
        }

        // Code to run when connected event is executed
        private void OnAnyEvent(string eventName, IExiledEvent? ev = null)
        {
            if (ev == null) return;

            Stopwatch stopwatch = new();

            stopwatch.Start();

            Type eventType = ev.GetType();

            // remove EventArgs from the end
            string name = eventType.Name.Substring(0, eventType.Name.Length - 9);
            Log.Debug($"Event '{name}' is now running.");

            if (ev is IDeniableEvent deniable and IPlayerEvent plr)
            {
                var playerIsNotNone = plr.Player is not null;
                var isRegisteredRule = EventHandlingModule.Singleton!.GetPlayerDisableEvent(name, plr.Player).HasValue;

                if (playerIsNotNone && isRegisteredRule)
                {
                    Log.Debug("Event is disabled.");
                    deniable.IsAllowed = false;
                    return;
                }
            }

            if (!CurrentEventData.TryGetValue(eventName, out var scriptNames))
            {
                return;
            }

            Log.Debug("Scripts connected to this event:");
            List<Script> scripts = new();

            foreach (string scrName in scriptNames)
            {
                if (!ScriptModule.Singleton!.TryParseScript(scrName, null, out var script, out var error))
                {
                    Logger.Error(error!.Append(Error(
                        "Event script failed to parse",
                        "There was an error while the script was getting ready for executuion, see inner exception.")));
                    continue;
                }

                scripts.Add(script!);
            }

            var properties = ev.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                Log.Debug($"Managing property {property.Name}");

                var value = property.GetValue(ev);
                if (value is null) continue;

                switch (value)
                {
                    case Player player:
                        foreach (Script script in scripts)
                            script.AddPlayerVariable($"EV{property.Name.ToUpper()}", new[] { player }, true);
                        Log.Debug($"Adding player variable @EV{property.Name.ToUpper()} to all scripts above.");
                        break;

                    case Exiled.API.Features.Items.Item item:
                        AddVariable(item.Base.ItemSerial.ToString());
                        break;

                    case Exiled.API.Features.Doors.Door door:
                        AddVariable(door.Type.ToString());
                        break;

                    case MapGeneration.Distributors.Scp079Generator gen:
                        AddVariable(gen.GetInstanceID().ToString());
                        break;

                    case bool @bool:
                        AddVariable(@bool.ToUpper());
                        break;

                    case IEnumerable enumerable:
                        AddVariable(string.Join(",", enumerable.Cast<object>().Select(x => x.ToString())));
                        break;

                    default:
                        AddVariable(value.ToString());
                        break;
                }

                continue;

                void AddVariable(string val)
                {
                    foreach (Script script in scripts)
                    {
                        script.AddLiteralVariable("EV" + property.Name.ToUpper(), val, true);
                        Log.Debug($"Adding variable @EV{property.Name.ToUpper()} to all scripts above.");
                    }
                }
            }

            foreach (Script script in scripts)
            {
                ScriptModule.Singleton!.TryRunScript(script, out var trace, out _);

                if (trace != null) 
                    Logger.ScriptError(trace, script);
            }

            stopwatch.Stop();
            Log.Debug($"Handling event '{name}' cost {stopwatch.ElapsedMilliseconds} ms");
        }

        private ErrorInfo Error(string name, string description) => new(name, description, "Event Script Module");
    }
}
