using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace CDC.Reader.Application
{
    public interface ISqlExtension
    {
        public Task<T> Get<T>(string query, object parameters = null, string mirrorConnectionString = "ConnectionStrings:CDCMirror", string connectionString = "ConnectionStrings:CDCMain");

        public Task<IEnumerable<T>> GetList<T>(string sqlQuery, object parameters = null, string mirrorConnectionString = "ConnectionStrings:CDCMirror", string connectionString = "ConnectionStrings:CDCMain");
    }
}
