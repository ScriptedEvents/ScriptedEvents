namespace ScriptedEvents.API.Extensions
{
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
    }
}
