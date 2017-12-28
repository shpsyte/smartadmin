using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smart.Core.Domain.Deals;

namespace Smart.Data.Map
{
    public class DealMap : IEntityTypeConfiguration<Deal>
    {
        public void Map(EntityTypeBuilder<Deal> entity)
        {
            entity.ForSqlServerToTable("Deal");
            entity.HasKey(p => p.DealId);


            entity.Property(e => e.Win).HasDefaultValueSql("0");
            entity.Property(e => e.Lost).HasDefaultValueSql("0");
            entity.Property(e => e.VisibleAll).HasDefaultValueSql("1");

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

            entity.Property(e => e.Name).HasMaxLength(150).HasColumnType("varchar(150)");


            entity.Property(e => e.Rowguid)
                .HasColumnName("rowguid")
                .HasDefaultValueSql("newid()");

            entity.Property(e => e.UserSettingId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal")
                .HasDefaultValueSql("0");

            entity.Property(e => e.Comments)
                .HasMaxLength(250).HasColumnType("varchar(250)");

            entity.Property(e => e.Name)
               .HasMaxLength(250).HasColumnType("varchar(150)");
            entity.Property(e => e.Phone)
               .HasMaxLength(250).HasColumnType("varchar(250)");
            entity.Property(e => e.Email)
               .HasMaxLength(250).HasColumnType("varchar(250)");



            entity.HasOne(d => d.Contact)
                .WithMany(p => p.Deal)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Company)
                 .WithMany(p => p.Deal)
                 .HasForeignKey(d => d.CompanyId)
                 .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Stage)
                .WithMany(p => p.Deals)
                .HasForeignKey(d => d.StageId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Pipeline)
               .WithMany(p => p.Deal)
               .HasForeignKey(d => d.PipelineId)
               .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.UserSetting)
               .WithMany(p => p.Deal)
               .HasForeignKey(d => d.UserSettingId)
               .OnDelete(DeleteBehavior.Restrict);






        }

    }


    public class DealUserMap : IEntityTypeConfiguration<DealUser>
    {
        public void Map(EntityTypeBuilder<DealUser> entity)
        {
            entity.ForSqlServerToTable("DealUser");
            entity.HasKey(p => p.Id);


            entity.Property(e => e.BusinessEntityId)
                     .IsRequired()
                     .HasColumnName("BusinessEntityID")
                     .HasMaxLength(450);

            entity.Property(e => e.UserSettingId)
                .IsRequired()
                .HasMaxLength(450);

            entity.HasOne(d => d.Deal)
                .WithMany(p => p.Users)
                .HasForeignKey(d => d.DealId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_DealUser_Deal");


            entity.HasOne(d => d.UserSetting)
                .WithMany(p => p.DealUser)
                .HasForeignKey(d => d.UserSettingId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_DealUser_UserSetting");






        }

    }



    public class DealStageMap : IEntityTypeConfiguration<DealStage>
    {
        public void Map(EntityTypeBuilder<DealStage> entity)
        {
            entity.ForSqlServerToTable("DealStage");
            entity.HasKey(p => p.Id);


            entity.Property(e => e.BusinessEntityId)
                .IsRequired()
                .HasColumnName("BusinessEntityID")
                .HasMaxLength(450);

            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            entity.Property(e => e.ExitDate)
                .HasColumnType("datetime");
            

            entity.Property(e => e.Rowguid).HasDefaultValueSql("newid()");

            entity.Property(e => e.UserSettingId)
                .IsRequired()
                .HasMaxLength(450);

            entity.HasOne(d => d.Deal)
                .WithMany(p => p.Stages)
                .HasForeignKey(d => d.Dealid)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_DealStage_Deal");

            entity.HasOne(d => d.Stage)
                .WithMany(p => p.DealStages)
                .HasForeignKey(d => d.StageId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_DealStage_Stage");

            entity.HasOne(d => d.UserSetting)
                .WithMany(p => p.DealStage)
                .HasForeignKey(d => d.UserSettingId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_DealStage_UserSetting");





        }

    }
}
