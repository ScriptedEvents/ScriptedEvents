namespace ScriptedEvents.Actions
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using ScriptedEvents.Variables.Interfaces;

    public class StartLoopAction : IScriptAction, IHiddenAction
    {
        /// <inheritdoc/>
        public string Name => "LOOP";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "[" };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("FOR", true, new Option("FOR")),
            new Argument("variableName", typeof(string), "The variable name to assign to.", true),
            new OptionsArgument("IN", true, new Option("IN")),
            new Argument("valueToLoopThrough", typeof(string), "The value to loop through, depending on the mode.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string varName = RawArguments[1];
            string loopingThrough = RawArguments[3];

            if (VariableSystem.TryGetPlayers(loopingThrough, out PlayerCollection plrs, script))
            {
                return PlayerLoop(varName, plrs.GetInnerList(), script);
            }

            return RangeLoop(varName, loopingThrough, script);
        }

        private static ActionResponse PlayerLoop(string varName, List<Player> players, Script script)
        {
            if (!script.IsInsideLoopStatement)
            {
                script.IsInsideLoopStatement = true;
                script.LoopStatementPlayersToLoopThrough = players;
            }

            Player plr = script.LoopStatementPlayersToLoopThrough.FirstOrDefault();

            script.LoopStatementPlayersToLoopThrough.Remove(plr);

            script.AddPlayerVariable(varName, string.Empty, new Player[] { plr });

            if (script.LoopStatementPlayersToLoopThrough.Count == 0)
            {
                script.IsInsideLoopStatement = false;
            }

            return new(true);
        }

        private static ActionResponse RangeLoop(string varName, string range, Script script)
        {
            range = VariableSystem.ReplaceVariables(range, script);

            if (CountOccurrences(range, "#") != 2)
            {
                return new(false, $"LOOP ERROR 1 - Provided range '{range}' does not contain exactly 2 '#' characters. The correct format is 'startIndex#endIndex#step'.");
            }

            string[] splitRange = range.Split('#');

            for (int i = 0; i < splitRange.Length; i++)
            {
                if (splitRange[i] == string.Empty)
                    continue;

                if (!ConditionHelperV2.TryMath(splitRange[i], out MathResult result))
                    continue;

                if (result.Success)
                {
                    splitRange[i] = result.Result.ToString();
                }
            }

            string startIndex = "0";
            float endIndex;
            float step = 1;

            if (splitRange[0] != string.Empty)
            {
                if (!float.TryParse(splitRange[0], out _))
                {
                    return new(false, $"LOOP ERROR 2 - Provided startIndex '{splitRange[0]}' cant be converted to float.");
                }

                startIndex = splitRange[0];
            }

            if (splitRange[1] != string.Empty)
            {
                if (!float.TryParse(splitRange[1], out endIndex))
                {
                    return new(false, $"LOOP ERROR 3 - Provided endIndex '{splitRange[1]}' cant be converted to float.");
                }
            }
            else
            {
                return new(false, "LOOP ERROR 4 - End index was not provided.");
            }

            if (splitRange[2] != string.Empty)
            {
                if (!float.TryParse(splitRange[2], out step))
                {
                    return new(false, $"LOOP ERROR 5 - Provided step '{splitRange[2]}' cant be converted to float.");
                }
            }

            if (!script.IsInsideLoopStatement)
            {
                script.IsInsideLoopStatement = true;
                script.LoopStatementStart = script.CurrentLine;
                script.AddVariable(varName, string.Empty, startIndex);
                return new(true);
            }

            if (!script.UniqueVariables.TryGetValue(varName, out CustomVariable indexVariable))
            {
                return new(false, "LOOP ERROR 6 - Index variable doesnt exist or was removed.", ActionFlags.FatalError);
            }

            if (!float.TryParse(indexVariable.Value, out float newValue))
            {
                return new(false, "LOOP ERROR 7 - Index variable was modified and cannot be casted into an integer.", ActionFlags.FatalError);
            }

            newValue += step;

            script.AddVariable(varName, string.Empty, newValue.ToString());

            if (step > 0)
            {
                if (newValue <= endIndex - step)
                {
                    return new(true);
                }
            }
            else
            {
                if (newValue >= endIndex - step)
                {
                    return new(true);
                }
            }

            script.IsInsideLoopStatement = false;
            return new(true);
        }

        private static int CountOccurrences(string text, string substring)
        {
            int count = 0;
            int index = 0;

            while ((index = text.IndexOf(substring, index)) != -1)
            {
                count++;
                index += substring.Length;
            }

            return count;
        }
    }
}
