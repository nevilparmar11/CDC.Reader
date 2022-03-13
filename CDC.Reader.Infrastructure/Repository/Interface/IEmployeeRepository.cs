using CDC.Reader.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CDC.Reader.Infrastructure.Repository.Interface
{
    public interface IEmployeeRepository
    {
        public Task<IEnumerable<EmployeeCT>> GetCDCEmployeeRecords(string LSN);
    }
}
