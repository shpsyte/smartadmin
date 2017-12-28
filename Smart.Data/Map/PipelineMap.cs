
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smart.Core.Domain.Flow;

namespace Smart.Data.Map
{
    public class PipelineMap : IEntityTypeConfiguration<Pipeline>
    {
        public void Map(EntityTypeBuilder<Pipeline> builder)
        {
            builder.ForSqlServerToTable("Pipeline");
            builder.HasKey(p => p.PipelineId);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(150).HasColumnType("varchar(150)");


            builder.Property(e => e.CreateDate)
             .HasColumnType("datetime")
             .HasDefaultValueSql("getdate()");

            builder.Property(e => e.ModifiedDate)
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

            builder.Property(e => e.Active).HasDefaultValueSql("1");

            builder.Property(e => e.UserSettingId).IsRequired();

            builder.Property(e => e.rowguid)
              .HasColumnName("rowguid")
              .HasDefaultValueSql("newid()");

            builder.HasOne(d => d.UserSetting)
                 .WithMany(p => p.Pipeline)
                 .HasForeignKey(d => d.UserSettingId)
                 .OnDelete(DeleteBehavior.Restrict);



        }
    }
}
