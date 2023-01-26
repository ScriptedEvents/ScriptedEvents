using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents
{
    public class ActionResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public bool Fatal { get; set; }

        public ActionResponse(bool success, string message = "", bool fatal = false)
        {
            Success = success;
            Message = message;
            Fatal = fatal;
        }
    }
}
