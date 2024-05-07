namespace ScriptedEvents.API.Features
{
    using System;
    using System.IO;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.Variables.Interfaces;

    /// <summary>
    /// Class to save variables long-term.
    /// </summary>
    public static class VariableStorage
    {
        /// <summary>
        /// Path to the variable storage folder.
        /// </summary>
        public static readonly string DirPath = Path.Combine(MainPlugin.BaseFilePath, MainPlugin.Configs.StorageFoldername);

        /// <summary>
        /// Save variable to variable storage.
        /// </summary>
        /// <param name="var">The variable to save.</param>
        public static void Save(IConditionVariable var)
        {
            InternalSave(var.Name, var.String());
        }

        /// <summary>
        /// Save variable to variable storage.
        /// </summary>
        /// <param name="varName">The name of the variable.</param>
        /// <param name="varValue">The value of the variable.</param>
        public static void Save(string varName, string varValue)
        {
            InternalSave(varName, varValue);
        }

        /// <summary>
        /// Read from the variable storage.
        /// </summary>
        /// <param name="varName">The variable name to read.</param>
        /// <returns>Variable value that has been saved before.</returns>
        public static string Read(string varName)
        {
            try
            {
                varName = StripBrackets(varName);
                using StreamReader reader = new(GetFilePath(varName));
                return reader.ReadLine();
            }
            catch (FileNotFoundException)
            {
                Log.Error($"Trying to read {varName} from storage, but it hasn't been saved in the storage folder.");
                return "INVALID - VARIABLE DOESN'T EXIST";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error trying to read from storage: {ex}");
            }
        }

        private static void InternalSave(string varName, string varValue)
        {
            try
            {
                varName = StripBrackets(varName);
                if (!Directory.Exists(DirPath))
                {
                    Directory.CreateDirectory(DirPath);
                    Log.Warn("Storage folder was absent; a new folder has been created.");
                }

                string filePath = GetFilePath(varName);

                using StreamWriter writer = new(filePath, false, System.Text.Encoding.UTF8);
                writer.WriteLine(varValue);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new ScriptedEventsException(ErrorGen.Get(ErrorCode.IOPermissionError) + $": {ex}");
            }
            catch (Exception ex)
            {
                throw new ScriptedEventsException(ErrorGen.Get(ErrorCode.IOError) + $": {ex}");
            }
        }

        private static string GetFilePath(string varName) => Path.Combine(DirPath, $"{varName}.txt");

        private static string StripBrackets(string varName)
        {
            if (varName.Length >= 2 && varName[0] == '{' && varName[varName.Length - 1] == '}')
                return varName.Substring(1, varName.Length - 2);

            return varName;
        }
    }
}
