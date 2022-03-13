using CDC.Reader.Application;
using CDC.Reader.Infrastructure.Context;
using CDC.Reader.Infrastructure.Context.Interface;
using CDC.Reader.Infrastructure.Repository.Interface;
using CDC.Reader.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CDC.Reader.Infrastructure.Repository
{
    public class CDCProcessLogRepository : ICDCProcessLogRepository
    {
        private readonly ISqlExtension _sqlExtension;
        private readonly IContextFactory contextFactory;
        public CDCProcessLogRepository(ISqlExtension sqlExtension, IContextFactory contextFactory)
        {
            _sqlExtension = sqlExtension;
            this.contextFactory = contextFactory;
        }

        public async Task<CDCProcessLogs> GetCDCProcessLog(string TableName)
        {
            string query = @"Select * from dbo.CDCProcessLogs WHERE TableName = @TableName";
            return await _sqlExtension.Get<CDCProcessLogs>(query, new { TableName = TableName }).ConfigureAwait(false);
        }

        public async Task<int> SaveCDCProcessLog(CDCProcessLogs processLog)
        {
            int result = 0;

            using (var cdcDbContext = contextFactory.CreateCDCDbContext<EmployeeContext>(null))
            {
                await cdcDbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
                {
                    if (processLog.ProcessLogId > 0)
                    {
                        cdcDbContext.Entry(processLog).State = EntityState.Modified;
                    }
                    else
                    {
                        cdcDbContext.Add(processLog);
                    }
                    result = await cdcDbContext.SaveChangesAsync().ConfigureAwait(false);
                }).ConfigureAwait(false);
            }

            return result;
        }
    }
}
