using System;
using System.Linq;
using Exiled.API.Features;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Features.Exceptions;
using ScriptedEvents.API.Modules;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.RoundRule
{
    public class EventRuleAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "EventRule";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string Description => "Enables or disables a 'IDeniable' Exiled event for specified players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new Option("Enable", "Enables an event."),
                new Option("Disable", "Disables an event.")),
            new Argument("players", typeof(Player[]), "The players to affect.", true),
            new Argument("eventName", typeof(string), "The event name to manage.", true),
        };

        public string LongDescription => $@"This action allows you to enable or disable any player event that is deniable.

Let's say you hate players flipping the coin and you want to disable this ability. How would you do it?

    1. Go to this web page: https://github.com/Exiled-Team/EXILED/blob/dev/Exiled.Events/Handlers/Player.cs
    2. On the right side you will find a window named 'Symbols' and inside it a search bar 'Filter symbols'.
    3. Search for 'coin' or 'flip'. First 2 results should be 'FlippingCoin' and 'OnFlippingCoin'. Click on 'FlippingCoin'.
    4. The highlighted portion is the name of the event.

You can now use 'EventRule Disable *@Players FlippingCoin', which will not allow anyone to flip their coin.

BUT! There is a caveat; not every event can be disabled. How can I check if 'FlippingCoin' event can be disabled?

    1. To the left of the highlighted 'FlippingCoin' you will find '<FlippingCoinEventArgs>'. Click on it.
    2. You will see that the right window now shows definitions of it, click 'class FlippingCoinEventArgs'.
    3. A line similar to this should be highlighted: 'public class FlippingCoinEventArgs : IPlayerEvent, IDeniableEvent, IItemEvent'

You see that 'IDeniableEvent' there? It means that you can enable and disable it. If it is not present then you can't enable and disable it.

It's completely fine if this is too much for you, you can always join our discord server for support: discord.gg/3j54zBnbbD
";

        public ActionResponse Execute(Script script)
        {
            bool disable = Arguments[0]!.ToUpper() == "DISABLE";
            Player[] players = (Player[])Arguments[1]!;
            string key = Arguments[2]!.ToString();

            var rule = EventHandlingModule.Singleton!.GetPlayerDisableEvent(key);

            if (MainPlugin.Modules.FirstOrDefault(mod => mod is EventScriptModule) is not EventScriptModule eventHandler)
            {
                throw new ImpossibleException();
            }
            
            eventHandler.ConnectDynamicExiledEvent(key);

            if (disable)
            {
                if (rule.HasValue)
                {
                    rule.Value.Players.AddRange(players);
                }
                else
                {
                    EventHandlingModule.Singleton!.DisabledPlayerEvents.Add(new(key, players.ToList()));
                }
            }
            else
            {
                if (!rule.HasValue) return new(true);
                
                var inner = players;
                rule.Value.Players.RemoveAll(ply => inner.Contains(ply));
            }

            return new(true);
        }
    }
}
