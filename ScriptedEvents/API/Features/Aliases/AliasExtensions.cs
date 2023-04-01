namespace ScriptedEvents.API.Features.Aliases
{
    using System.Collections.Generic;
    using System.Linq;

    public static class AliasExtensions
    {
        public static Alias Get(this IEnumerable<Alias> aliases, string keyword)
        {
            if (aliases is null || keyword is null)
                return null;

            return aliases.FirstOrDefault(x => x.Command == keyword);
        }
    }
}
