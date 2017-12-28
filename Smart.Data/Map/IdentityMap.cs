using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Smart.Core.Domain.Identity;
using Smart.Core.Identity;
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace Smart.Data.Map
{
    public class IdentityMap : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Map(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ForSqlServerToTable("User");
            builder.HasKey(p => p.Id);
        }
    }
    public class IdentityRoleMap : IEntityTypeConfiguration<IdentityRole>
    {
        public void Map(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.ForSqlServerToTable("Role");
            builder.HasKey(p => p.Id);
        }
    }
    public class IdentityUserClaimMap : IEntityTypeConfiguration<IdentityUserClaim<string>>
    {
        public void Map(EntityTypeBuilder<IdentityUserClaim<string>> builder)
        {
            builder.ForSqlServerToTable("UserClaim");
            builder.HasKey(a => a.Id );
        }
    }
    public class IdentityUserRoleMap : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Map(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.ForSqlServerToTable("UserRole");
            builder.HasKey(a => new { a.UserId, a.RoleId });
        }
    }
    public class IdentityUserLoginMap : IEntityTypeConfiguration<IdentityUserLogin<string>>
    {
        public void Map(EntityTypeBuilder<IdentityUserLogin<string>> builder)
        {
            builder.ForSqlServerToTable("UserLogin");
            builder.HasKey(a => new { a.LoginProvider, a.ProviderKey });
        }
    }
    public class IdentityRoleClaimMap : IEntityTypeConfiguration<IdentityRoleClaim<string>>
    {
        public void Map(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        {
            builder.ForSqlServerToTable("RoleClaim");
            builder.HasKey(a => a.Id);
        }
    }
    public class IdentityUserTokenMap : IEntityTypeConfiguration<IdentityUserToken<string>>
    {
        public void Map(EntityTypeBuilder<IdentityUserToken<string>> builder)
        {
            builder.ForSqlServerToTable("UserToken");
            builder.HasKey(a => new { a.UserId, a.LoginProvider, a.Name });
        }
    }
}
