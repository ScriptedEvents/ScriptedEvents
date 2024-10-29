using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;
    using UnityEngine;

    public class RaycastAction : IScriptAction, IHelpInfo, IReturnValueAction
    {
        /// <inheritdoc/>
        public string Name => "RAYCAST";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Returns the thing the player is looking at.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("player", typeof(Player), "The player.", true),
            new Argument("maxdistance", typeof(int), "The max distance of the raycast.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Player plr = (Player)Arguments[0];
            int distance = (int)Arguments[1];
            const int mask = ~(1 << 1 | 1 << 13 | 1 << 16 | 1 << 28);

            if (Physics.Raycast(plr.CameraTransform.position, plr.CameraTransform.forward, out RaycastHit raycastHit, distance, mask))
            {
                if (raycastHit.collider.gameObject == null)
                {
                    return new(true, variablesToRet: Array.Empty<object>());
                }

                if (raycastHit.collider.gameObject.name == "Collider")
                {
                    return new(true, variablesToRet: new object[] { raycastHit.collider.gameObject.gameObject.name });
                }

                return new(true, variablesToRet: new object[] { raycastHit.collider.gameObject.name });
            }

            Player raycastedPlayer = Exiled.API.Features.Player.Get(raycastHit.collider);
            if (raycastedPlayer == null)
            {
                return new(true, variablesToRet: new object[] { "NONE" });
            }

            return new(true, variablesToRet: new object[] { new Player[] { raycastedPlayer } });
        }
    }
}