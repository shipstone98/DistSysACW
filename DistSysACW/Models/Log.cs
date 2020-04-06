using System;

namespace DistSysACW.Models
{
    public class Log: ICloneable
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

        public Object Clone()
        {
            return new Log
            {
                LogDateTime = this.LogDateTime,
                LogId = this.LogId,
                LogString = this.LogString
            };
        }
    }
}