using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CDC.Reader.Infrastructure.Context.Interface
{
    public interface IContextFactory : IDesignTimeDbContextFactory<DbContext>
    {
        DbContext CreateCDCDbContext<T>(string[] args) where T : DbContext;
    }
}
