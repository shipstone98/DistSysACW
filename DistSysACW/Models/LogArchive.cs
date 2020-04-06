using System;

namespace DistSysACW.Models
{
    public class LogArchive: Log
    {
        public String ApiKey { get; set; }

        public LogArchive(): base() { }
        public LogArchive(String logString): base(logString) { }
    }
}