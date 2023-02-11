namespace ScriptedEvents.API.Features.Aliases
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.Loader;

    public class Alias
    {
        public string Command { get; set; } = "BROADCAST";
        public string Execute { get; set; } = "COMMAND /bc";
        // args exist for a documentation or something generator, that could help the user understand the plugin's actions
        // in the future
        //[YamlIgnore]
        //public string[] Arguments { get; set; } = new[] { "DURATION", "MESSAGE" }; 

        public Alias() { }
        public Alias(string command, string execute, params string[] args)
        {
            Command = command;
            Execute = execute;
            //Arguments = args;
        }

        public string Unalias(string usage)
            => usage.Replace(Command, Execute);

        // This adds support for special alias parameters, such as duration, which would be able to add random numbers.
        // Essentially, this would make Aliases much more powerful by essentially making them custom actions.
        // For example, you could make a BROADCAST duration message action, which would look like this
        // BROADCAST 3-5 Hello guys, and automatically 3-5 would parse, because, in the config, the duration argument is passed as "duration"

        // TODO: Test & fix this.
        // This will be implemented later on, because I can't be bothered to.
        public List<string> Unalias(string[] usage)
        {
            List<string> copy = usage.ToList();

            int index = usage.IndexOf(Command);
            if (index > -1)
            {
                string str = usage[index];

                str = Unalias(str);
                string[] split = str.Split(' ');

                copy[index] = str;
                copy.InsertRange(index + 1, split);
            }

            int addedCount = copy.Count - usage.Length;
            for (int i = 0; i < usage.Count() - 1; i++)
            {
                string arg = copy[i + 1 + addedCount];
                switch ("duration") //(Arguments[i])
                {
                    case "duration":
                        string[] range = arg.Split('-');
                        if (range.Length > 1)
                        {
                            if (!int.TryParse(range[0], out int min))
                                throw new InvalidOperationException("Range minimum is not a natural number.");
                            if (!int.TryParse(range[0], out int max))
                                throw new InvalidOperationException("Range maximum is not a natural number.");

                            copy[i + addedCount] = Loader.Random.Next(min, max).ToString();
                            break;
                        }

                        copy[i + addedCount] = range[0];
                        break;
                    default:
                        break;
                }
            }

            return copy;
        }

        public override string ToString()
            => /*$"{Command} [{string.Join("] [", Arguments)}]"*/ Command;
    }
}
