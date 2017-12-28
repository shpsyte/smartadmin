using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smart.Core.Domain.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Map
{
    public class ContactMap : IEntityTypeConfiguration<Contact>
    {
        public void Map(EntityTypeBuilder<Contact> entity)
        {
                entity.ForSqlServerToTable("Contact");
                entity.HasKey(p => p.ContactId);


               entity.HasIndex(e => new { e.FirstName, e.LastName, e.CreateDate })
                    .HasName("IX_Contact");

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

                entity.Property(e => e.MiddleName).HasMaxLength(50).HasColumnType("varchar(50)");

                entity.Property(e => e.ModifiedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Phone).HasMaxLength(120).HasColumnType("varchar(120)");

                entity.Property(e => e.rowguid)
                    .HasColumnName("rowguid")
                    .HasDefaultValueSql("newid()");

                entity.Property(e => e.Title).HasMaxLength(8).HasColumnType("varchar(8)");
            entity.Property(e => e.Comments).HasMaxLength(250).HasColumnType("varchar(250)");
            
            entity.HasOne(d => d.Company)
                 .WithMany(p => p.Contacts)
                 .HasForeignKey(d => d.CompanyId)
                 .HasConstraintName("FK_Contact_Company");

        }



    }
}
