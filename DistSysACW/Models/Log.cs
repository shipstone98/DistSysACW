using System;

namespace DistSysACW.Models
{
    public class Log
    {
        public DateTime LogDateTime { get; set; }
        public String LogId { get; set; }
        public String LogString { get; set; }

        public Log() { }

        public Log(String logString)
        {
            this.LogDateTime = DateTime.Now;
            this.LogString = logString ?? "";
        }

        protected internal Log(Log log)
        {
            this.LogDateTime = log.LogDateTime;
            this.LogId = log.LogId ?? Guid.NewGuid().ToString();
            this.LogString = log.LogString ?? String.Empty;
        }
    }
}