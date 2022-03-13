using CDC.Reader.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CDC.Reader.Infrastructure.Context.Configs
{
    public class CDCProcessLogsConfig : IEntityTypeConfiguration<CDCProcessLogs>
    {
        public void Configure(EntityTypeBuilder<CDCProcessLogs> builder)
        {
            builder.ToTable("CDCProcessLogs", "dbo");
            builder.HasKey(x => x.ProcessLogId);
        }
    }
}
