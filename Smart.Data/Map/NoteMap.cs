using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smart.Core.Domain.Notes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Map
{
    public class NoteMap : IEntityTypeConfiguration<Note>
    {
        public void Map(EntityTypeBuilder<Note> entity)
        {
            entity.ForSqlServerToTable("Note");
            entity.HasKey(p => p.NoteId);

            entity.HasIndex(e => e.CreateDate)
                   .HasName("IX_Note");

            entity.Property(e => e.Active).HasDefaultValueSql("1");

            entity.Property(e => e.BusinessEntityId)
                .IsRequired()
                .HasColumnName("BusinessEntityID")
                .HasMaxLength(450);

            entity.Property(e => e.Comments)
                   .IsRequired()
                   .HasColumnType("varchar(max)");

            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            entity.Property(e => e.Rowguid).HasDefaultValueSql("newid()");

            entity.Property(e => e.UserSettingId)
                .IsRequired()
                .HasMaxLength(450);


            entity.HasOne(d => d.UserSetting)
               .WithMany(p => p.Note)
               .HasForeignKey(d => d.UserSettingId)
               .OnDelete(DeleteBehavior.Restrict);


        }
    }

    public class DealNoteMap : IEntityTypeConfiguration<DealNote>
    {
        public void Map(EntityTypeBuilder<DealNote> entity)
        {
            entity.ForSqlServerToTable("DealNote");
            entity.HasKey(p => p.Id);


            entity.Property(e => e.BusinessEntityId)
                                .IsRequired()
                                .HasColumnName("BusinessEntityID")
                                .HasMaxLength(450);


            entity.HasOne(d => d.Note)
              .WithMany(p => p.DealNote)
              .HasForeignKey(d => d.NoteId)
              .OnDelete(DeleteBehavior.Restrict);



            entity.HasOne(d => d.Deal)
              .WithMany(p => p.Notes)
              .HasForeignKey(d => d.DealId)
              .OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class CompanyNoteMap : IEntityTypeConfiguration<CompanyNote>
    {
        public void Map(EntityTypeBuilder<CompanyNote> entity)
        {
            entity.ForSqlServerToTable("CompanyNote");
            entity.HasKey(p => p.Id);

            

            entity.Property(e => e.BusinessEntityId)
                .IsRequired()
                .HasColumnName("BusinessEntityID")
                .HasMaxLength(450);


            entity.HasOne(d => d.Company)
                .WithMany(p => p.Notes)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_CompanyNote_Company");

            entity.HasOne(d => d.Note)
                .WithMany(p => p.CompanyNote)
                .HasForeignKey(d => d.NoteId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_CompanyNote_Note");
        }
    }

    public class ContactNoteMap : IEntityTypeConfiguration<ContactNote>
    {
        public void Map(EntityTypeBuilder<ContactNote> entity)
        {
            entity.ForSqlServerToTable("ContactNote");
            entity.HasKey(p => p.Id);



            entity.Property(e => e.BusinessEntityId)
                .IsRequired()
                .HasColumnName("BusinessEntityID")
                .HasMaxLength(450);


            entity.HasOne(d => d.Contact)
                .WithMany(p => p.Notes)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_ContactNote_Contact");

            entity.HasOne(d => d.Note)
                .WithMany(p => p.ContactNote)
                .HasForeignKey(d => d.NoteId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_ContactNote_Note");
        }
    }

}
