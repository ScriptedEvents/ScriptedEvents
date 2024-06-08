namespace ScriptedEvents.API.Constants
{
    using System;
    using System.Text;

    using Exiled.API.Features.Pools;

    using ScriptedEvents.Structures;

    public static class Constants
    {
        public static readonly string ItsMyBirthday = @$"ScriptedEvents has turned {DateTime.Now.Year - 2023} years old today!";

        public static readonly Contributor[] Contributors =
        {
            new("Thunder", "Lead Developer"),
            new("Elektryk_Andrzej", "Developer, Discord Support"),

            new("Saskyc", "Discord Support"),
            new("Jraylor", "Discord Support"),
            new("YourHolinessVonGustav", "Discord Support"),

            new("Yamato", "Former Developer"),
            new("Johnodon", "Former EasyEvents Developer"),
            new("PintTheDragon", "Former EasyEvents Developer"),
        };

        public static string GenerateContributorList(char? prefixCharacter)
        {
            StringBuilder sb = StringBuilderPool.Pool.Get();
            foreach (Contributor c in Contributors)
            {
                sb.AppendLine((prefixCharacter.HasValue ? $"{prefixCharacter} " : string.Empty) + c.ToString());
            }

            return StringBuilderPool.Pool.ToStringReturn(sb);
        }
    }
}
