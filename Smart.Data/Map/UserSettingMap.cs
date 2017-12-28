using Smart.Core.Domain.Business;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Smart.Core.Domain.Identity;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Smart.Data.Map
{
    public class UserSettingMap : IEntityTypeConfiguration<UserSetting>
    {

        
        public void Map(EntityTypeBuilder<UserSetting> entity)
        {
            entity.ForSqlServerToTable("UserSetting");
            entity.HasKey(p => p.UserSettingId  );




            entity.HasIndex(e => new { e.FirstName, e.LastName, e.CreateDate })
                 .HasName("IX_UserSetting");

          

            entity.Property(e => e.BusinessEntityId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

           

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(120).HasColumnType("varchar(120)");

            entity.Property(e => e.LastName).HasMaxLength(120).HasColumnType("varchar(120)");

            entity.Property(e => e.MiddleName).HasMaxLength(50).HasColumnType("varchar(50)");

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");


            entity.Property(e => e.rowguid)
                .HasColumnName("rowguid")
                .HasDefaultValueSql("newid()");

            entity.Property(e => e.Title).HasMaxLength(8).HasColumnType("varchar(8)");

            //entity.HasOne(d => d.ApplicationUser)
            //     .WithOne(p => p.UserSetting)
            //     .HasForeignKey<UserSetting>(d => d.UserSettingId)
            //     .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
