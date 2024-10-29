using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    public class EventAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "EVENT";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string Description => "Enables or disables an Exiled event for specified players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("ENABLE", "Enables an event."),
                new("DISABLE", "Disables an event.")),
            new Argument("players", typeof(PlayerCollection), "The players to affect.", true),
            new Argument("eventName", typeof(string), "The event name to manage.", true),
        };

        public string LongDescription => $@"This action allows you to enable or disable any player event that is deniable.

Let's say you hate players flipping the coin and you want to disable this ability. How would you do it?

1. Go to this web page: https://github.com/Exiled-Team/EXILED/blob/dev/Exiled.Events/Handlers/Player.cs
2. On the right side you will find a window named 'Symbols' and inside it a search bar 'Filter symbols'.
3. Search for 'coin' or 'flip'. First 2 results should be 'FlippingCoin' and 'OnFlippingCoin'. Click on 'FlippingCoin'.
4. The highlighted portion is the name of the event.

You can now use 'EVENT DISABLE * FlippingCoin', which will not allow anyone to flip their coin.

BUT! There is a caveat; not every event can be disabled. How can I check if 'FlippingCoin' event can be disabled?

1. To the left of the highlighted 'FlippingCoin' you will find '<FlippingCoinEventArgs>'. Click on it.
2. You will see that the right window now shows definitions of it, click 'class FlippingCoinEventArgs'.
3. A line similar to this should be highlighted: 'public class FlippingCoinEventArgs : IPlayerEvent, IDeniableEvent, IItemEvent'

You see that 'IDeniableEvent' there? It means that you can enable and disable it. If it is not present then you can't enable and disable it.

It's completely fine if this is too much for you, you can always join our discord server for support: discord.gg/3j54zBnbbD
";

        public ActionResponse Execute(Script script)
        {
            bool disable = Arguments[0].ToUpper() == "DISABLE";
            PlayerCollection players = (PlayerCollection)Arguments[1];
            string key = Arguments[2].ToString();

            PlayerDisable? rule = MainPlugin.Handlers.GetPlayerDisableEvent(key);

            EventScriptModule eventHandler = MainPlugin.Modules.Where(mod => mod is EventScriptModule).FirstOrDefault() as EventScriptModule;
            eventHandler.ConnectDynamicExiledEvent(key);

            if (disable)
            {
                if (rule.HasValue)
                {
                    rule.Value.Players.AddRange(players.GetInnerList());
                }
                else
                {
                    MainPlugin.Handlers.DisabledPlayerEvents.Add(new(key, players.GetInnerList()));
                }
            }
            else
            {
                if (rule.HasValue)
                {
                    var inner = players.GetInnerList();
                    rule.Value.Players.RemoveAll(ply => inner.Contains(ply));
                }
            }

            return new(true);
        }
    }
}
