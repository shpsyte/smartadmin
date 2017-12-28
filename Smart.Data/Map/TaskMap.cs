using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smart.Core.Domain.Notes;
using Smart.Core.Domain.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
namespace Smart.Data.Map
{
    public class TaskMap : IEntityTypeConfiguration<Task>
    {
        public void Map(EntityTypeBuilder<Task> entity)
        {
            entity.ForSqlServerToTable("Task");
            entity.HasKey(p => p.TaskId);
            entity.Property(e => e.Active).HasDefaultValueSql("1");
            entity.Property(e => e.Done).HasDefaultValueSql("0");
            entity.Property(e => e.BusinessEntityId)
                .IsRequired()
                .HasColumnName("BusinessEntityID")
                .HasMaxLength(450);
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");
            entity.Property(e => e.Deleted).HasDefaultValueSql("0");
            
            entity.Property(e => e.DueDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("dateadd(day,(7),getdate())");
            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnType("varchar(40)");
            entity.Property(e => e.Rowguid).HasDefaultValueSql("newid()");
            entity.Property(e => e.UserSettingId).HasMaxLength(450);


            entity.Property(e => e.Required).HasDefaultValueSql("0");



            entity.HasOne(d => d.TaskGroup)
                .WithMany(p => p.Task)
                .HasForeignKey(d => d.TaskGroupId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Task_TaskGroup");
            entity.HasOne(d => d.UserSetting)
               .WithMany(p => p.Task)
               .HasForeignKey(d => d.UserSettingId)
               .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Company)
                   .WithMany(p => p.Tasks)
                   .HasForeignKey(d => d.CompanyId)
                   .HasConstraintName("FK_Task_Company");

            entity.HasOne(d => d.Contact)
                .WithMany(p => p.Task)
                .HasForeignKey(d => d.ContactId)
                .HasConstraintName("FK_Task_Contact");

            entity.HasOne(d => d.Deal)
                .WithMany(p => p.Tasks)
                .HasForeignKey(d => d.DealId)
                .HasConstraintName("FK_Task_Deal");


        }
    }
    public class TaskGroupMap : IEntityTypeConfiguration<TaskGroup>
    {
        public void Map(EntityTypeBuilder<TaskGroup> entity)
        {
            entity.ForSqlServerToTable("TaskGroup");
            entity.HasKey(p => p.TaskGroupId);
            entity.HasIndex(e => e.Name)
                    .HasName("IX_TaskGroup");
            entity.Property(e => e.BusinessEntityId)
                .IsRequired()
                .HasColumnName("BusinessEntityID")
                .HasMaxLength(450);
            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");
            entity.Property(e => e.Deleted).HasDefaultValueSql("0");
            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasColumnType("varchar(40)");
            entity.Property(e => e.Rowguid).HasDefaultValueSql("newid()");
        }
    }
    
}
