using System;

namespace Models
{
    public class LogError
    {
        public DateTime ErrorTime { get; set; }
        public string Info { get; set; }
        public string FileName { get; set; }
        public Exception ExceptionThrown { get; set; }
    }
}
