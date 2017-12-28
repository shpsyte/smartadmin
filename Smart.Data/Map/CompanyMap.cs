using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smart.Core.Domain.Notes;
using Smart.Core.Domain.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Map
{
    public class CompanyMap : IEntityTypeConfiguration<Company>
    {
        public void Map(EntityTypeBuilder<Company> entity)
        {
            entity.ForSqlServerToTable("Company");
            entity.HasKey(p => p.CompanyId);


            entity.HasIndex(e => new { e.Email, e.FirstName })
                    .HasName("IX_Company");


            entity.Property(e => e.Active).HasDefaultValueSql("1");

            entity.Property(e => e.BusinessEntityId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            entity.Property(e => e.Deleted).HasDefaultValueSql("0");

            entity.Property(e => e.Email).HasMaxLength(250).HasColumnType("varchar(250)");
            

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(120).HasColumnType("varchar(120)");

            entity.Property(e => e.LastName).HasMaxLength(120).HasColumnType("varchar(120)");



            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            entity.Property(e => e.Phone).HasMaxLength(120).HasColumnType("varchar(120)");

            entity.Property(e => e.rowguid)
                .HasColumnName("rowguid")
                .HasDefaultValueSql("newid()");
        }



    }




}
