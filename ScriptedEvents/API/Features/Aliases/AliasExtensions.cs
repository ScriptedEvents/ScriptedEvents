using System.Collections.Generic;
using System.Linq;
using ScriptedEvents.API.Features.Aliases;

namespace ScriptedEvents.API.Features.Aliases
{
    public static class AliasExtensions
    {
        public static Alias Get(this IEnumerable<Alias> aliases, string keyword)
            => aliases.FirstOrDefault(x => x.Command == keyword);
    }
}
