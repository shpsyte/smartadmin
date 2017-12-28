using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smart.Core.Domain.Addresss;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart.Data.Map
{

    public class AddressMap : IEntityTypeConfiguration<Address>
    {
        public void Map(EntityTypeBuilder<Address> entity)
        {
            entity.ForSqlServerToTable("Address");
            entity.HasKey(p => p.AddressId);

            entity.Property(e => e.AddressLine1)
                   .IsRequired()
                   .HasMaxLength(250).HasColumnType("varchar(250)");

            entity.Property(e => e.AddressLine2).HasMaxLength(150).HasColumnType("varchar(250)");

            entity.Property(e => e.AddressLine3).HasMaxLength(60).HasColumnType("varchar(60)");

            entity.Property(e => e.BusinessEntityId)
                .IsRequired()
                .HasMaxLength(450);


            entity.Property(e => e.Deleted).HasDefaultValueSql("0");


            entity.Property(e => e.CityId)
                .IsRequired();

            entity.Property(e => e.PostalCode)
                .IsRequired()
                .HasMaxLength(15).HasColumnType("varchar(15)");

            entity.Property(e => e.SpatialLocation).HasMaxLength(150).HasColumnType("varchar(150)");


            entity.HasOne(d => d.City)
                .WithMany(p => p.Address)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("FK_Address_City");
        }



    }


    public class CompanyAddressMap : IEntityTypeConfiguration<CompanyAddress>
    {
        public void Map(EntityTypeBuilder<CompanyAddress> entity)
        {
            entity.ForSqlServerToTable("CompanyAddress");
            entity.HasKey(p => p.Id);

            entity.Property(e => e.BusinessEntityId)
                     .IsRequired()
                     .HasMaxLength(450);

            entity.HasOne(d => d.Address)
                .WithMany(p => p.Companys)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_CompanyAddress_Address");

            entity.HasOne(d => d.Company)
                .WithMany(p => p.Address)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_CompanyAddress_Company");
        }
    }


    public class ContactAddressMap : IEntityTypeConfiguration<ContactAddress>
    {
        public void Map(EntityTypeBuilder<ContactAddress> entity)
        {
            entity.ForSqlServerToTable("ContactAddress");
            entity.HasKey(p => p.Id);

            entity.Property(e => e.BusinessEntityId)
                    .IsRequired()
                    .HasMaxLength(450);

            entity.HasOne(d => d.Address)
                .WithMany(p => p.Contacts)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_ContactAddress_Address");

            entity.HasOne(d => d.Contact)
                .WithMany(p => p.Address)
                .HasForeignKey(d => d.ContactId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_ContactAddress_Contact");
        }
    }




    public class CityMap : IEntityTypeConfiguration<City>
    {
        public void Map(EntityTypeBuilder<City> entity)
        {
            entity.ForSqlServerToTable("City");
            entity.HasKey(p => p.CityId);

            entity.Property(e => e.BusinessEntityId)
                     .IsRequired()
                     .HasMaxLength(450);

            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            entity.Property(e => e.MiddleName).HasMaxLength(8).HasColumnType("varchar(8)");

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(250).HasColumnType("varchar(250)");

            entity.Property(e => e.Rowguid)
                .HasColumnName("rowguid")
                .HasDefaultValueSql("newid()");

            entity.Property(e => e.SpecialCodeRegion).HasMaxLength(120).HasColumnType("varchar(120)");

            entity.HasOne(d => d.StateProvince)
                .WithMany(p => p.City)
                .HasForeignKey(d => d.StateProvinceId)
                .HasConstraintName("FK_City_StateProvince");
        }
    }




    public class StateProvinceMap : IEntityTypeConfiguration<StateProvince>
    {
        public void Map(EntityTypeBuilder<StateProvince> entity)
        {
            entity.ForSqlServerToTable("StateProvince");
            entity.HasKey(p => p.StateProvinceId);

            entity.HasIndex(e => new { e.Name, e.CountryRegionCode, e.StateProvinceCode })
                    .HasName("IX_StateProvince");

            entity.Property(e => e.BusinessEntityId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(e => e.CountryRegionCode)
                .IsRequired()
                .HasMaxLength(3).HasColumnType("varchar(3)");

            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            entity.Property(e => e.IsOnlyStateProvinceFlag).HasDefaultValueSql("1");

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50).HasColumnType("varchar(50)");

            entity.Property(e => e.Rowguid)
                .HasColumnName("rowguid")
                .HasDefaultValueSql("newid()");

            entity.Property(e => e.StateProvinceCode)
                .IsRequired()
                .HasColumnType("nchar(3)");

            entity.HasOne(d => d.Territory)
                .WithMany(p => p.StateProvince)
                .HasForeignKey(d => d.TerritoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_StateProvince_Territory");
        }
    }




    public class TerritoryMap : IEntityTypeConfiguration<Territory>
    {
        public void Map(EntityTypeBuilder<Territory> entity)
        {
            entity.ForSqlServerToTable("Territory");
            entity.HasKey(p => p.TerritoryId);

            entity.HasIndex(e => new { e.TerritoryId, e.MiddleName, e.Name })
                  .HasName("IX_Territory");

            entity.Property(e => e.BusinessEntityId)
                .IsRequired()
                .HasMaxLength(450);

            entity.Property(e => e.CountryRegionCode).HasMaxLength(3).HasColumnType("varchar(3)");

            entity.Property(e => e.CreateDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            entity.Property(e => e.MiddleName).HasMaxLength(8).HasColumnType("varchar(8)");

            entity.Property(e => e.ModifiedDate)
                .HasColumnType("datetime")
                .HasDefaultValueSql("getdate()");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50).HasColumnType("varchar(50)");

            entity.Property(e => e.Rowguid)
                .HasColumnName("rowguid")
                .HasDefaultValueSql("newid()");

            entity.Property(e => e.SpecialCodeRegion).HasColumnType("nchar(10)");
        }
    }

}
