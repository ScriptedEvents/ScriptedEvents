namespace ScriptedEvents.API.Constants
{
    using System;
    using System.Text;

    using Exiled.API.Features.Pools;

    using ScriptedEvents.Structures;

    public static class Constants
    {
        public static readonly string ItsMyBirthday = @$"ScriptedEvents has turned {DateTime.Now.Year - 2023} year old today!";

        public static readonly Contributor[] Contributors =
        {
            new("Thunder", "Lead Programmer"),
            new("Elektryk_Andrzej", "Programmer, SECAS Developer, Discord Support"),
            new("Yamato", "Programmer"),

            new("Saskyc", "Discord Support"),
            new("YourHolinessVonGustav", "Discord Support"),

            new("Rue", "RueI Developer"),
            new("Johnodon", "EasyEvents Developer"),
            new("PintTheDragon", "EasyEvents Developer"),
        };

        public static string GenerateContributorList()
        {
            StringBuilder sb = StringBuilderPool.Pool.Get();
            foreach (Contributor c in Contributors)
            {
                sb.AppendLine(c.ToString());
            }

            return StringBuilderPool.Pool.ToStringReturn(sb);
        }
    }
}
