namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Mirror;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Commands.MainCommand;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class GeneratorAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "GENERATORS";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Modifies genrators.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode to use. Valid options: OPEN, CLOSE, LOCK, UNLOCK, OVERCHARGE, ACTIVATE, DEACTIVATE", true),
            new Argument("amount", typeof(int), "The amount of generators to affect.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            int max = 3;
            if (Arguments.Length > 1)
            {
                if (!int.TryParse(Arguments[1], out max))
                    return new(MessageType.NotANumber, this, "amount", Arguments[1]);
            }

            List<Generator> x = new List<Generator>(Generator.List);

            // c# is dark fckn magic so chatgpt to the resque i guess
            static List<T> ShuffleList<T>(List<T> list, int max)
            {
                Random random = new Random();
                int n = Math.Min(list.Count, max);
                List<T> shuffledList = new List<T>(list);

                for (int i = n - 1; i > 0; i--)
                {
                    int randIndex = random.Next(0, i + 1);
                    T temp = shuffledList[i];
                    shuffledList[i] = shuffledList[randIndex];
                    shuffledList[randIndex] = temp;
                }

                return shuffledList.GetRange(0, n);
            }

            x = ShuffleList(x, max);

            foreach (Generator generator in x)
            {
                switch (Arguments[0].ToUpper())
                {
                    case "OPEN":
                        generator.IsOpen = true;
                        break;
                    case "CLOSE":
                        generator.IsOpen = false;
                        break;
                    case "LOCK":
                        generator.IsUnlocked = false;
                        break;
                    case "UNLOCK":
                        generator.IsUnlocked = true;
                        break;
                    case "OVERCHARGE":
                        generator.IsEngaged = true;
                        break;
                    case "ACTIVATE":
                        generator.IsActivating = true;
                        break;
                    case "DEACTIVATE":
                        generator.IsActivating = false;
                        break;
                    default:
                        return new(MessageType.InvalidOption, this, "mode", Arguments[0], "Valid options: OPEN, CLOSE, LOCK, UNLOCK, OVERCHARGE, ACTIVATE, DEACTIVATE");
                }
            }

            return new(true);
        }
    }
}
