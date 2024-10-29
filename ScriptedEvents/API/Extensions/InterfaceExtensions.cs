using ScriptedEvents.Interfaces;

namespace ScriptedEvents.API.Extensions
{
    using System;
    using System.Linq;

    using ScriptedEvents.API.Features;
    using ScriptedEvents.Variables.Interfaces;

    public static class InterfaceExtensions
    {
        /// <summary>
        /// Determines if an action is obsolete.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="message">A message regarding the obsolete.</param>
        /// <returns>Whether or not the action is obsolete.</returns>
        public static bool IsObsolete(this IAction action, out string message)
        {
            Type t = action.GetType();
            var obsolete = t.CustomAttributes.FirstOrDefault(attr => attr.AttributeType.Name == "ObsoleteAttribute");
            if (obsolete is not null)
            {
                message = obsolete.ConstructorArguments[0].Value.ToString();
                return true;
            }

            message = string.Empty;
            return false;
        }
    }
}
