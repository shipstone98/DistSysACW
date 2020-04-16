using System;

namespace DistSysACW.Models
{
    public class LogArchive: Log
    {
        public String ApiKey { get; set; }

        public LogArchive(): base() { }
        public LogArchive(String logString): base(logString) { }

        public LogArchive(Log oldLog, String apiKey): base(oldLog)
        {
            if (apiKey is null)
            {
                throw new ArgumentNullException(nameof (apiKey));
            }

            if (String.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentException(nameof (apiKey));
            }

            this.ApiKey = apiKey;
        }
    }
}