using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smart.Core.Domain.Files;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Map
{


    public class FileMap : IEntityTypeConfiguration<File>
    {
        public void Map(EntityTypeBuilder<File> entity)
        {
            entity.ForSqlServerToTable("File");
            entity.HasKey(p => p.FileId);

            entity.HasIndex(e => new { e.Name, e.ContentType })
                   .HasName("IX_File");

            entity.Property(e => e.BusinessEntityId)
                .IsRequired()
                .HasColumnName("BusinessEntityID")
                .HasMaxLength(450);

            entity.Property(e => e.ContentType)
                .IsRequired()
                .HasColumnType("varchar(100)");

            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            entity.Property(e => e.Deleted).HasDefaultValueSql("0");

            entity.Property(e => e.DueDate).HasColumnType("datetime");

            entity.Property(e => e.FileData).IsRequired();

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnType("varchar(40)");

            entity.Property(e => e.Rowguid).HasDefaultValueSql("newid()");


            entity.HasOne(d => d.UserSetting)
               .WithMany(p => p.File)
               .HasForeignKey(d => d.UserSettingId)
               .OnDelete(DeleteBehavior.Restrict);

        }
    }



    public class DealFileMap : IEntityTypeConfiguration<DealFile>
    {
        public void Map(EntityTypeBuilder<DealFile> entity)
        {
            entity.ForSqlServerToTable("DealFile");
            entity.HasKey(p => p.Id);

            entity.Property(e => e.BusinessEntityId)
                   .IsRequired()
                   .HasColumnName("BusinessEntityID")
                   .HasMaxLength(450);

            entity.HasOne(d => d.Deal)
                .WithMany(p => p.Files)
                .HasForeignKey(d => d.DealId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_DealFile_Deal");

            entity.HasOne(d => d.File)
                .WithMany(p => p.DealFile)
                .HasForeignKey(d => d.FileId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_DealFile_File");

        }
    }




    public class CompanyFileMap : IEntityTypeConfiguration<CompanyFile>
    {
        public void Map(EntityTypeBuilder<CompanyFile> entity)
        {
            entity.ForSqlServerToTable("CompanyFile");
            entity.HasKey(p => p.Id);


            entity.Property(e => e.BusinessEntityId)
                .IsRequired()
                .HasColumnName("BusinessEntityID")
                .HasMaxLength(450);

            entity.HasOne(d => d.Company)
                .WithMany(p => p.Files)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_CompanyFile_Company");

            entity.HasOne(d => d.File)
                .WithMany(p => p.CompanyFile)
                .HasForeignKey(d => d.FileId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_CompanyFile_File");

        }
    }

    public class ContactFileMap : IEntityTypeConfiguration<ContactFile>
    {
        public void Map(EntityTypeBuilder<ContactFile> entity)
        {
            entity.ForSqlServerToTable("ContactFile");
            entity.HasKey(p => p.Id);


            entity.Property(e => e.BusinessEntityId)
                .IsRequired()
                .HasColumnName("BusinessEntityID")
                .HasMaxLength(450);

            entity.Property(e => e.BusinessEntityId)
                   .IsRequired()
                   .HasColumnName("BusinessEntityID")
                   .HasMaxLength(450);

            entity.HasOne(d => d.Contact)
                .WithMany(p => p.Files)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_ContactFile_Contact");

            entity.HasOne(d => d.File)
                .WithMany(p => p.ContactFile)
                .HasForeignKey(d => d.FileId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_ContactFile_File");

        }
    }
}
