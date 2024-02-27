namespace ScriptedEvents.API.Features
{
    using System;
    using System.IO;

    using Exiled.API.Features;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Variables.Interfaces;

    /// <summary>
    /// Class to save variables long-term.
    /// </summary>
    public static class VariableStorage
    {
        public static readonly string DirPath = Path.Combine(ScriptHelper.ScriptPath, MainPlugin.Configs.StorageFoldername);

        public static void Save(IConditionVariable var)
        {
            try
            {
                if (!Directory.Exists(DirPath))
                {
                    Directory.CreateDirectory(DirPath);
                    Log.Warn("Variable storage folder was absent; a new folder was created.");
                }

                string filePath = Path.Combine(DirPath, $"{var.Name}.txt");

                using StreamWriter writer = new(filePath, false, System.Text.Encoding.UTF8);
                writer.WriteLine(var.String());
            }
            catch (Exception ex)
            {
                Log.Error($"Error when saving variable to long term storage: {ex}");
            }
        }

        public static void Save(string varName, string varValue)
        {
            try
            {
                if (!Directory.Exists(DirPath))
                {
                    Directory.CreateDirectory(DirPath);
                    Log.Warn("Variable storage folder was absent; a new folder was created.");
                }

                string filePath = Path.Combine(DirPath, $"{varName}.txt");

                using StreamWriter writer = new(filePath, false, System.Text.Encoding.UTF8);
                writer.WriteLine(varValue);
            }
            catch (Exception ex)
            {
                Log.Error($"Error when saving variable to long term storage: {ex}");
            }
        }

        public static string Read(string varName)
        {
            string filePath = Path.Combine(DirPath, $"{varName}.txt");

            using StreamReader reader = new(filePath);
            return reader.ReadLine();
        }
    }
}
