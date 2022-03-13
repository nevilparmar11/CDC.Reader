using System;
using System.Collections.Generic;
using System.Text;

namespace CDC.Reader.Models
{
    public class CDCProcessLogs
    {
        public int ProcessLogId { get; set; }
        public byte[] LSN { get; set; }
        public string TableName { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
