using CDC.Reader.Application;
using CDC.Reader.Infrastructure.Repository.Interface;
using CDC.Reader.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CDC.Reader.Infrastructure.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ISqlExtension _sqlExtension;

        public EmployeeRepository(ISqlExtension sqlExtension)
        {
            _sqlExtension = sqlExtension;
        }

        public async Task<IEnumerable<EmployeeCT>> GetCDCEmployeeRecords(string LSN)
        {
            if (!string.IsNullOrEmpty(LSN))
            {
                string query = @"Select 
                                        GlobalEmployeeId,
                                        EmployeeId,
                                        FirstName,
                                        LastName,
                                        PhoneNumber,
                                        Email,
                                        Date AS HiredDate,
                                        __$operation AS OperationType,
                                        __$start_lsn AS Start_LSN
                                        from cdc.dbo_Employees_CT where __$start_lsn > " + LSN;
                return await _sqlExtension.GetList<EmployeeCT>(query).ConfigureAwait(false);
            }

            string query2 = @"Select 
                                        GlobalEmployeeId,
                                        EmployeeId,
                                        FirstName,
                                        LastName,
                                        PhoneNumber,
                                        Email,
                                        Date AS HiredDate,
                                        __$operation AS OperationType,
                                        __$start_lsn AS Start_LSN
                                        from cdc.dbo_Employees_CT";
            return await _sqlExtension.GetList<EmployeeCT>(query2, new { }).ConfigureAwait(false);
        }
    }
}
