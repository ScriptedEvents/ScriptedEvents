namespace ScriptedEvents
{
    using System;
    using System.Collections.Generic;
    using CommandSystem;
    using Discord;
    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Variables.Condition;

    /// <summary>
    /// Represents a script.
    /// </summary>
    public class Script : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Script"/> class.
        /// Creates a new script and assigns its <see cref="UniqueId"/> to a new <see cref="Guid"/>.
        /// </summary>
        public Script()
        {
            Labels = DictionaryPool<string, int>.Pool.Get();
            Flags = ListPool<string>.Pool.Get();
            UniqueVariables = DictionaryPool<string, CustomVariable>.Pool.Get();
            UniquePlayerVariables = DictionaryPool<string, CustomPlayerVariable>.Pool.Get();
            UniqueId = Guid.NewGuid();

            Log.Debug($"Created new script object | ID: {UniqueId}");
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Script"/> class.
        /// </summary>
        ~Script()
        {
            Dispose();
        }

        /// <summary>
        /// Gets the unique ID referring to this Script instance.
        /// </summary>
        public Guid UniqueId { get; }

        /// <summary>
        /// Gets or sets the name of the script.
        /// </summary>
        public string ScriptName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the permission required to read the script.
        /// </summary>
        public string ReadPermission { get; set; } = "script.read";

        /// <summary>
        /// Gets or sets the permission required to execute the script.
        /// </summary>
        public string ExecutePermission { get; set; } = "script.execute";

        /// <summary>
        /// Gets or sets the path to the script, on the host's computer.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the last time the script was read.
        /// </summary>
        public DateTime LastRead { get; set; }

        /// <summary>
        /// Gets or sets the last time the script was edited.
        /// </summary>
        public DateTime LastEdited { get; set; }

        /// <summary>
        /// Gets or sets the raw text of the script.
        /// </summary>
        public string RawText { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a list of <see cref="IAction"/> of each action.
        /// </summary>
        public IAction[] Actions { get; set; }

        /// <summary>
        /// Gets or sets a list of Labels.
        /// </summary>
        public Dictionary<string, int> Labels { get; set; }

        /// <summary>
        /// Gets the line the script is currently on.
        /// </summary>
        public int CurrentLine { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the script is currently executing.
        /// </summary>
        public bool IsRunning { get; internal set; } = false;

        /// <summary>
        /// Gets or sets a value indicating the time that the script began running.
        /// </summary>
        public DateTime RunDate { get; set; }

        /// <summary>
        /// Gets a list of flags on the script.
        /// </summary>
        public List<string> Flags { get; }

        /// <summary>
        /// Gets a value indicating whether or not the script is enabled.
        /// </summary>
        public bool Disabled => Flags.Contains("DISABLE");

        /// <summary>
        /// Gets a value indicating whether or not the script is running in debug mode.
        /// </summary>
        public bool Debug => Flags.Contains("DEBUG");

        /// <summary>
        /// Gets a value indicating whether or not the script is marked as an admin-event (CedMod compatibility).
        /// </summary>
        public bool AdminEvent => Flags.Contains("ADMINEVENT");

        /// <summary>
        /// Gets a value indicating whether or not warnings are suppressed.
        /// </summary>
        public bool SuppressWarnings => Flags.Contains("SUPPRESSWARNINGS");

        /// <summary>
        /// Gets the context that the script was executed in.
        /// </summary>
        public ExecuteContext Context { get; internal set; }

        /// <summary>
        /// Gets the sender of the user who executed the script.
        /// </summary>
        public ICommandSender Sender { get; internal set; }

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> of variables that are unique to this script.
        /// </summary>
        public Dictionary<string, CustomVariable> UniqueVariables { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> of player variables that are unique to this script.
        /// </summary>
        public Dictionary<string, CustomPlayerVariable> UniquePlayerVariables { get; set; }

        /// <inheritdoc/>
        public void Dispose()
        {
            Log.Debug($"Disposing script object | ID: {UniqueId}");

            Labels = null;
            Sender = null;
            Actions = null;
            RawText = null;
            FilePath = null;

            DictionaryPool<string, int>.Pool.Return(Labels);
            ListPool<string>.Pool.Return(Flags);
            DictionaryPool<string, CustomVariable>.Pool.Return(UniqueVariables);
            DictionaryPool<string, CustomPlayerVariable>.Pool.Return(UniquePlayerVariables);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Moves the <see cref="CurrentLine"/> to the specified line.
        /// </summary>
        /// <param name="line">The line to move to.</param>
        public void Jump(int line)
        {
            CurrentLine = line - 1;
        }

        /// <summary>
        /// Moves the <see cref="CurrentLine"/> to the specified location.
        /// </summary>
        /// <param name="keyword">Keyword (NEXT, START, label, or number).</param>
        /// <returns>Whether or not the jump was successful.</returns>
        public bool Jump(string keyword)
        {
            switch (keyword.ToUpper())
            {
                case "NEXT": // Simply return "true" as a success. It'll go to the next line automatically.
                    return true;
                case "START":
                    CurrentLine = 0;
                    return true;
            }

            if (Labels.TryGetValue(keyword, out int line))
            {
                CurrentLine = line;
                return true;
            }

            if (int.TryParse(keyword, out int n))
            {
                Jump(n);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Moves to the next line.
        /// </summary>
        public void NextLine() => CurrentLine++;

        /// <summary>
        /// Logs a debug message to the console.
        /// </summary>
        /// <param name="input">The input to log.</param>
        public void DebugLog(string input)
        {
            if (!Debug) return;
            Log.Send($"[ScriptedEvents] [Script: {ScriptName}] {input}", LogLevel.Debug, ConsoleColor.Gray);
        }

        /// <summary>
        /// Execute the script.
        /// </summary>
        public void Execute() => ScriptHelper.RunScript(this);

        /// <summary>
        /// Adds a variable.
        /// </summary>
        /// <param name="name">Name of the variable.</param>
        /// <param name="desc">Description of the variable.</param>
        /// <param name="value">The value of the variable.</param>
        public void AddVariable(string name, string desc, object value)
        {
            UniqueVariables.Add(name, new(name, desc, value));
        }

        /// <summary>
        /// Adds a player variable.
        /// </summary>
        /// <param name="name">Name of the variable.</param>
        /// <param name="desc">Description of the variable.</param>
        /// <param name="value">The <see cref="IEnumerable{T}"/> of Players for this variable.</param>
        public void AddPlayerVariable(string name, string desc, IEnumerable<Player> value)
        {
            UniquePlayerVariables.Add(name, new(name, desc, value));
        }
    }
}
