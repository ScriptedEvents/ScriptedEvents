namespace ScriptedEvents.API.Extensions
{
    using System;

    /// <summary>
    /// Contains useful extensions.
    /// </summary>
    public static class TimeSpanExtensions
    {
        public static T ToSeconds<T>(this TimeSpan input)
            where T : struct, IConvertible 
        {
            try
            {
                return (T)Convert.ChangeType(input.Seconds, typeof(T));
            }
            catch
            {
                throw new InvalidCastException(
                    $"TimeSpan extension `ToSeconds` failed. Provided type {nameof(T)} is not valid.");
            }
        }
    }
}
