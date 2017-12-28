using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smart.Core.Domain.Flow;
using Smart.Core.Fake;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Map
{
   

    public class StagedMap : IEntityTypeConfiguration<Stage>
    {
        public void Map(EntityTypeBuilder<Stage> builder)
        {
            builder.ForSqlServerToTable("Stage");

            builder.HasKey(p => p.StageId );


            builder.Property(p => p.Name).IsRequired().HasMaxLength(150).HasColumnType("varchar(150)");



            builder.Property(e => e.CreateDate)
             .HasColumnType("datetime")
             .HasDefaultValueSql("getdate()");

            builder.Property(e => e.ModifiedDate)
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

            builder.Property(e => e.Active).HasDefaultValueSql("1");

            builder.Property(e => e.OrderStage).IsRequired();

            builder.Property(e => e.rowguid)
              .HasColumnName("rowguid")
              .HasDefaultValueSql("newid()");


            builder.HasOne(d => d.Pipeline)
              .WithMany(p => p.Stage)
              .HasForeignKey(d => d.PipelineId)
              .OnDelete(DeleteBehavior.Restrict);

        }
    }



    public class StageUserMap : IEntityTypeConfiguration<StageUser>
    {
        public void Map(EntityTypeBuilder<StageUser> entity)
        {
            entity.ForSqlServerToTable("StageUser");

            entity.HasKey(p => p.Id);

            entity.Property(e => e.BusinessEntityId)
                   .IsRequired()
                   .HasColumnName("BusinessEntityID")
                   .HasMaxLength(450);

            entity.Property(e => e.UserSettingId)
                .IsRequired()
                .HasMaxLength(450);

            entity.HasOne(d => d.Stage)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.StageId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_StageUser_Stage");

            entity.HasOne(d => d.UserSetting)
                .WithMany(p => p.StageUser)
                .HasForeignKey(d => d.UserSettingId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_StageUser_UserSetting");

        }
    }

    //public class TimeStageMap : IEntityTypeConfiguration<TimeStage>
    //{
    //    //public void Map(EntityTypeBuilder<TimeStage> entity)
    //    //{
    //    //    entity.ForSqlServerToTable("TimeStage");

    //    //    entity.HasKey(p => p.Id);

    //    //    entity.Property(e => e.BusinessEntityId)
    //    //           .IsRequired()
    //    //           .HasColumnName("BusinessEntityID")
    //    //           .HasMaxLength(450);
            

    //    //}
    //}

}
