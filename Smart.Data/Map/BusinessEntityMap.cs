using Smart.Core.Domain.Business;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Smart.Data.Map
{
    public class BusinessEntityMap : IEntityTypeConfiguration<BusinessEntity>
    {
        public void Map(EntityTypeBuilder<BusinessEntity> builder)
        {
            builder.ForSqlServerToTable("BusinessEntity");
            builder.HasKey(p => p.BusinessEntityId);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(80);
            builder.Property(p => p.EmailCreate).HasMaxLength(250).HasColumnType("varchar(250)");
            builder.Property(p => p.ExternalCode).HasMaxLength(40).HasColumnType("varchar(250)");
            builder.Property(e => e.CreateDate)
               .HasColumnType("datetime")
               .HasDefaultValueSql("getdate()");
            builder.Property(e => e.rowguid)
                           .HasColumnName("rowguid")
                           .HasDefaultValueSql("newid()");
            builder.Property(e => e.Active).HasDefaultValueSql("1");
        




    }
}

    public class UserBusinessEntityMap : IEntityTypeConfiguration<UserBusinessEntity>
    {
        public void Map(EntityTypeBuilder<UserBusinessEntity> builder)
        {
            builder.ForSqlServerToTable("UserBusinessEntity");
            builder.HasKey(p => new { p.BusinessEntityId, p.UserSettingId });
            
        }
    }

    public class BusinessEntityConfigMap : IEntityTypeConfiguration<BusinessEntityConfig>
    {
        public void Map(EntityTypeBuilder<BusinessEntityConfig> builder)
        {
            builder.ForSqlServerToTable("BusinessEntityConfig");
            builder.HasKey(p => new { p.BusinessEntityId  });
            builder.Property(p => p.Pop).HasMaxLength(20);
            builder.Property(p => p.Email).IsRequired().HasMaxLength(250);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(250);
            builder.Property(p => p.Smtp).HasMaxLength(250);
            builder.Property(p => p.Password).HasMaxLength(250);
            builder.Property(e => e.CreateDate)
               .HasColumnType("datetime")
               .HasDefaultValueSql("getdate()");
            builder.Property(e => e.ModifiedDate)
               .HasColumnType("datetime")
               .HasDefaultValueSql("getdate()");
            builder.Property(e => e.rowguid)
                           .HasColumnName("rowguid")
                           .HasDefaultValueSql("newid()");
        }
    }


}
