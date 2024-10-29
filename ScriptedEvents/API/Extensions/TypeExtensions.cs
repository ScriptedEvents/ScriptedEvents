namespace ScriptedEvents.API.Extensions
{
    using System;

    /// <summary>
    /// Contains useful extensions.
    /// </summary>
    public static class TypeExtensions
    {
        public static T[] ToNotNullArray<T>(this T? input)
        {
            return input is null ? new T[] { } : new[] { input };
        }

        public static object ToNotNullArrayObject<T>(this T input)
        {
            return ToNotNullArray(input);
        }

        public static object ToStringObject<T>(this T input) =>
            input?.ToString()
            ?? throw new Exception("ToStringObject method got null!!!");
    }
}
