using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smart.Core.Domain.Addresss;
using Smart.Core.Domain.Goals;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Map
{

    public class GoalMap : IEntityTypeConfiguration<Goal>
    {
        public void Map(EntityTypeBuilder<Goal> entity)
        {
            entity.ForSqlServerToTable("Goal");
            entity.HasKey(p => p.Id);


            entity.Property(e => e.Active).HasDefaultValueSql("1");
            entity.Property(e => e.Measure).HasDefaultValueSql("0");
            

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
                .HasColumnType("varchar(150)");

            entity.Property(e => e.Rowguid).HasDefaultValueSql("newid()");

            entity.Property(e => e.UserSettingId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(e => e.Value).HasColumnType("decimal");

            entity.HasOne(d => d.Pipeline)
                .WithMany(p => p.Goal)
                .HasForeignKey(d => d.PipelineId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Goal_Pipeline");

            entity.HasOne(d => d.Stage)
                .WithMany(p => p.Goals)
                .HasForeignKey(d => d.StageId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Goal_Stage");

            entity.HasOne(d => d.UserSetting)
                .WithMany(p => p.Goal)
                .HasForeignKey(d => d.UserSettingId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Goal_UserSetting");


        }



    }
    
}
