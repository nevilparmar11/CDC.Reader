using CDC.Reader.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CDC.Reader.Infrastructure.Repository.Interface
{
    public interface ICDCProcessLogRepository
    {
        public Task<CDCProcessLogs> GetCDCProcessLog(string TableName);
        public Task<int> SaveCDCProcessLog(CDCProcessLogs processLog);
    }
}
