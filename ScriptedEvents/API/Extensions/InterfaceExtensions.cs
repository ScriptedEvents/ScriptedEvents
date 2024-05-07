namespace ScriptedEvents.API.Extensions
{
    using System;
    using System.Linq;

    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Variables.Interfaces;

    public static class InterfaceExtensions
    {
        public static string String(this IVariable variable, bool reversed = false)
        {
            switch (variable)
            {
                case IBoolVariable @bool:
                    bool result = reversed ? !@bool.Value : @bool.Value;
                    return result.ToUpper();
                case IFloatVariable @float:
                    return @float.Value.ToString();
                case ILongVariable @long:
                    return @long.Value.ToString();
                case IStringVariable @string:
                    return @string.Value;
                default: // Shouldn't be possible
                    throw new System.InvalidCastException();
            }
        }

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
