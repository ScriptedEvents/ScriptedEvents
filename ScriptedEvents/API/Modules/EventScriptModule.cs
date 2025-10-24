using Exiled.API.Enums;
using PlayerRoles;

namespace ScriptedEvents.API.Modules
{
#nullable enable
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.Features;
    using Exiled.Loader;
    using MapGeneration.Distributors;
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

        public List<Tuple<IExiledEvent, Delegate>> StoredEvents { get; } = new();

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
            Singleton.OnAnyEvent(new StackFrame(3).GetMethod().Name.Replace("On", string.Empty));
        }

        public override void Init()
        {
            base.Init();
            Singleton = this;

            try
            {
                HandlerTypes = Loader.Plugins.First(plug => plug.Name == "Exiled.Events")
                    .Assembly.GetTypes()
                    .Where(t => t.FullName?.Equals($"Exiled.Events.Handlers.{t.Name}") is true).ToArray();
            }
            catch (Exception ex)
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
                        CurrentEventData[evName].Add(scr.Name);
                    }
                    else
                    {
                        CurrentEventData.Add(evName, new List<string>() { scr.Name });
                    }
                }

                if (scr.HasFlag("CUSTOMEVENT", out Flag cf))
                {
                    string cEvName = cf.Arguments[0];
                    if (CurrentCustomEventData.ContainsKey(cEvName))
                    {
                        CurrentCustomEventData[cEvName].Add(scr.Name);
                    }
                    else
                    {
                        CurrentCustomEventData.Add(cEvName, new List<string>() { scr.Name });
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
            var eventsAssembly = Exiled.Loader.Loader.Plugins.FirstOrDefault(x => x.Name == "Exiled.Events");

			if (eventsAssembly == null)
			{
				Log.Error("Exiled.Events library was not found. Is your EXILED install corrupted?");
				return;
			}
            
            if (DynamicallyConnectedEvents.Contains(key)) return;

            DynamicallyConnectedEvents.Add(key);

            var propertyInfo = eventsAssembly.Assembly
                .GetTypes()
                .Where(t => t.Namespace == "Exiled.Events.Handlers")
                .Select(t => t.GetProperty(key))
                .FirstOrDefault(p => p != null);

            if (propertyInfo is null)
            {
                Log.Error($"There is no EXILED event named '{key}'");
                return;
            }
            
            Delegate handler;
            if (propertyInfo.GetValue(null) is not IExiledEvent @event)
            {
                Log.Warn($"Properety find inside the events class but is not an event: {propertyInfo.Name}");
                return;
            }

            if (@event is Event simpleEvent)
			{
                // No idea if you can do a cast like (CustomEventHandler)MessageHandlerForEmptyArgs
                CustomEventHandler customEvent = OnNonArgumentedEvent;
                simpleEvent.Subscribe(customEvent);
				handler = customEvent;
            }
			else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Event<>))
			{
                // Need to use reflection, no non-genreic interface existe to subsribe non-generic event handler
                handler = typeof(EventScriptModule)
                    .GetMethod(nameof(OnArgumentedEvent))!
                    .MakeGenericMethod(propertyInfo.PropertyType.GenericTypeArguments)
                    .CreateDelegate(typeof(CustomEventHandler<>)
                    .MakeGenericType(propertyInfo.PropertyType.GenericTypeArguments));

                MethodInfo? subscribeMethod = propertyInfo.PropertyType.GetMethod(
					nameof(Event</*dummy type*/int>.Subscribe),
					new[] { handler.GetType() });

                if (subscribeMethod is null)
                {
                    Log.Error($"Failed to create a valid subscribe method for event {propertyInfo.Name}.");
                    return;
                }

                subscribeMethod.Invoke(@event, new[] { handler });
			}
			else
			{
				Log.Warn($"Unknown type of event: {propertyInfo.Name}");
				return;
			}

            StoredEvents.Add(new(@event, handler));
        }

        public void TerminateConnections()
        {
            foreach ((IExiledEvent @event, Delegate handler) in StoredEvents)
            {
                if (@event is Event simpleEvent)
                {
                    simpleEvent.Unsubscribe(handler as CustomEventHandler);
                }
                else
                {
                    MethodInfo unsubscribeMethod = @event.GetType().GetMethod(
                        nameof(Event</*dummy type*/int>.Unsubscribe),
                        new Type[] { handler.GetType() })!;
                    unsubscribeMethod.Invoke(@event, new[] { handler });
                }
            }
           
            StoredEvents.Clear();
            CurrentEventData = null;
            CurrentCustomEventData = null;
            DynamicallyConnectedEvents = new();
        }

        // Code to run when connected event is executed
        public void OnAnyEvent(string eventName, IExiledEvent? ev = null)
        {
            Stopwatch stopwatch = new();

            stopwatch.Start();

            if (ev is IDeniableEvent deniable and IPlayerEvent playerEvent)
            {
                bool playerIsNotNone = playerEvent.Player is not null;

                bool isRegisteredRule = MainPlugin.Handlers.GetPlayerDisableEvent(eventName, playerEvent.Player).HasValue;

                if (playerIsNotNone && isRegisteredRule)
                {
                    Log.Debug("Event is disabled.");
                    deniable.IsAllowed = false;
                    return;
                }
            }

            if (CurrentEventData is null || !CurrentEventData.TryGetValue(eventName, out var scriptNames))
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

            var properties = (
                from prop in ev?.GetType().GetProperties() ?? new PropertyInfo[] {}
                let value = prop.GetValue(ev) 
                where value is not null 
                select new Tuple<object, string>(value, prop.Name)).ToList();

            IPlayerEvent? reference = ev as IPlayerEvent;
            switch (eventName)
            {
                case "Left":
                case "ChangingRole":
                    LastPlayerState state = new(reference!.Player);
                    properties.Add(new(state, "ShouldNotHappen"));
                    break;
            }
            
            foreach (var (propValue, propName) in properties)
            {
                switch (propValue)
                {
                    case Player player:
                        if (player is Npc) continue;

                        foreach (var script in scripts)
                            script.AddPlayerVariable($"{{EV{propName.ToUpper()}}}", string.Empty, new[] { player });

                        Log.Debug($"Adding variable {{EV{propName.ToUpper()}}} to all scripts above.");
                        break;

                    case Item item:
                        AddVariable(item.Base.ItemSerial.ToString());
                        break;

                    case Door door:
                        AddVariable(door.Type.ToString());
                        break;

                    case Scp079Generator gen:
                        AddVariable(gen.GetInstanceID().ToString());
                        break;
                    
                    case Role role:
                        AddVariable(role.Type.ToString());
                        break;
                    
                    case Enum anyEnum:
                        AddVariable(anyEnum.ToString());
                        break;

                    case bool @bool:
                        AddVariable(@bool.ToUpper());
                        break;
                    
                    case LastPlayerState lastPlayerState:
                        foreach (FieldInfo field in typeof(LastPlayerState).GetFields(BindingFlags.Public | BindingFlags.Instance))
                        {
                            string fieldName = field.Name;
                            object value = field.GetValue(lastPlayerState);
                        
                            foreach (Script script in scripts)
                            {
                                script.AddVariable($"{{EV{fieldName.ToUpper()}}}", string.Empty, value.ToString());
                                Log.Debug($"Adding variable {{EV{fieldName.ToUpper()}}} to all scripts above.");
                            }
                        }
                        break;

                    default:
                        AddVariable(propValue.ToString());
                        break;
                }

                continue;

                void AddVariable(string varValue)
                {
                    foreach (Script script in scripts)
                    {
                        script.AddVariable($"{{EV{propName.ToUpper()}}}", string.Empty, varValue);
                        Log.Debug($"Adding variable {{EV{propName.ToUpper()}}} to all scripts above.");
                    }
                }
            }

            foreach (Script script in scripts)
                script.Execute();

            stopwatch.Stop();
            Log.Debug($"Handling event '{eventName}' cost {stopwatch.ElapsedMilliseconds} ms");
        }

        private class LastPlayerState
        {
            public string LastName;
            public string LastUserId;
            public RoleTypeId LastRole;
            public Team LastTeam;
            public ZoneType LastZone;
            public RoomType LastRoom;

            public LastPlayerState(Player player)
            {
                LastName = player.Nickname;
                LastUserId = player.UserId;
                LastRole = player.Role.Type;
                LastTeam = player.Role.Team;
                LastZone = player.Zone;
                LastRoom = player.CurrentRoom?.Type ?? RoomType.Unknown;
            }
        }
    }
}
