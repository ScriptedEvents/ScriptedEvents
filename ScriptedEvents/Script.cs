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
    using ScriptedEvents.API.Helpers;

    /// <summary>
    /// Represents a script.
    /// </summary>
    public class Script
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Script"/> class.
        /// Creates a new script and assigns its <see cref="UniqueId"/> to a new <see cref="Guid"/>.
        /// </summary>
        public Script()
        {
            Flags = ListPool<string>.Pool.Get();
            UniqueId = Guid.NewGuid();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Script"/> class.
        /// </summary>
        ~Script()
        {
            ListPool<string>.Pool.Return(Flags);
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
        public Dictionary<string, int> Labels { get; set; } = new();

        /// <summary>
        /// Gets the line the script is currently on.
        /// </summary>
        public int CurrentLine { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the script is currently executing.
        /// </summary>
        public bool IsRunning { get; internal set; } = false;

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
        /// Moves the <see cref="CurrentLine"/> to the specified line.
        /// </summary>
        /// <param name="line">The line to move to.</param>
        public void Jump(int line)
        {
            CurrentLine = line - 2;
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
    }
}
