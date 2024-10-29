using ScriptedEvents.Enums;

namespace ScriptedEvents.Structures
{
    using System.Collections.Generic;

    public struct CustomCommand
    {
        public string Name { get; set; }

        public bool Enabled { get; set; }

        public string Description { get; set; }

        public string Permission { get; set; }

        public CommandType Type { get; set; }

        public bool DefaultResponse { get; set; }

        public int Cooldown { get; set; }

        public int PlayerCooldown { get; set; }

        public List<string> Run { get; set; }
    }
}
