using Exiled.API.Features;
using Exiled.API.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents
{
    public class MainPlugin : Plugin<Config>
    {
        public override void OnEnabled()
        {
            if (!Directory.Exists(ScriptHelper.ScriptPath))
            {
                var info = Directory.CreateDirectory(ScriptHelper.ScriptPath);
                File.WriteAllText(Path.Combine(info.FullName, "DemoScript.txt"), DemoScript.Demo);
            }

            ScriptHelper.Setup();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            ScriptHelper.ActionTypes.Clear();
            base.OnDisabled();
        }
    }

    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
    }
}
