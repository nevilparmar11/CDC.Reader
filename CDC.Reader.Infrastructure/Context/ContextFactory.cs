using CDC.Reader.Infrastructure.Context.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

namespace CDC.Reader.Infrastructure.Context
{
    public class ContextFactory : IContextFactory
    {
        private readonly IConfiguration configuration;

        public ContextFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public DbContext CreateCDCDbContext<T>(string[] args) where T : DbContext
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<T>()
                               .UseSqlServer(configuration["ConnectionStrings:CDCMain"],
                               sqlServerOptionsAction: sqlOptions =>
                               {
                                   sqlOptions.EnableRetryOnFailure(maxRetryCount: Convert.ToInt32(configuration["MaxRetryCount"]), 
                                       maxRetryDelay: TimeSpan.FromSeconds(Convert.ToInt32(configuration["MaxRetryDelay"])), 
                                       errorNumbersToAdd: null);
                               });
            return (DbContext)Activator.CreateInstance(typeof(T), dbContextOptionsBuilder.Options);
        }

        public DbContext CreateDbContext(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}
