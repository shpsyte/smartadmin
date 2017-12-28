using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Smart.Core.Domain.Person;
using Smart.Core.Domain.Flow;
using Smart.Core.Domain.Identity;
using Smart.Core.Identity;
using Smart.Data.Map;
using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata;
using Smart.Core.Domain.Business;
using Smart.Core.Domain.Tasks;
using Smart.Core.Domain.Deals;
using Smart.Core.Fake;

namespace Smart.Data.Context
{
    public class SmartContext : IdentityDbContext<ApplicationUser>
    {

        #region DbSets
        
        public DbSet<UserBusinessEntity> UserBusinessEntity { get; set; }
        
        //public DbSet<ApplicationUser> ApplicationUser { get; set; }
        //public DbSet<UserSetting> UserSetting { get; set; }

        #endregion



        public SmartContext(DbContextOptions<SmartContext> options)
            : base(options)
        {
            Database.EnsureCreated();
            //Database.Migrate();

        }
        
        public SmartContext()
        {

        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
            
            //builder.Entity<ApplicationUser>().ToTable("User");
            //builder.Entity<ApplicationUser>().HasKey(a => a.Id);
            //builder.Entity<IdentityRole>().ToTable("Role");
            //builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
            //builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
            //builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
            //builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");
            //builder.Entity<IdentityUserToken<string>>().ToTable("UserToken");
            //builder.Entity<IdentityUserRole<string>>().HasKey(a => new { a.UserId, a.RoleId });
            //builder.Entity<IdentityUserClaim<string>>().HasKey(a => new { a.UserId, a.Id });
            //builder.Entity<IdentityUserLogin<string>>().HasKey(a => new { a.UserId, a.ProviderKey });
            //builder.Entity<IdentityRoleClaim<string>>().HasKey(a => new { a.RoleId, a.Id });
            //builder.Entity<IdentityUserToken<string>>().HasKey(a => new { a.UserId });
           


            #region AutoMap
            // Interface that all of our Entity maps implement
            var mappingInterface = typeof(IEntityTypeConfiguration<>);
            // Types that do entity mapping
            var mappingTypes = typeof(SmartContext).GetTypeInfo().Assembly.GetTypes()
                .Where(x => x.GetInterfaces().Any(y => y.GetTypeInfo().IsGenericType && y.GetGenericTypeDefinition() == mappingInterface));
            // Get the generic Entity method of the ModelBuilder type
            var entityMethod = typeof(ModelBuilder).GetMethods()
                .Single(x => x.Name == "Entity" &&
                        x.IsGenericMethod &&
                        x.ReturnType.Name == "EntityTypeBuilder`1");
            foreach (var mappingType in mappingTypes)
            {
                // Get the type of entity to be mapped
                var genericTypeArg = mappingType.GetInterfaces().Single().GenericTypeArguments.Single();
                // Get the method builder.Entity<TEntity>
                var genericEntityMethod = entityMethod.MakeGenericMethod(genericTypeArg);
                // Invoke builder.Entity<TEntity> to get a builder for the entity to be mapped
                var entityBuilder = genericEntityMethod.Invoke(builder, null);
                // Create the mapping type and do the mapping
                var mapper = Activator.CreateInstance(mappingType);
                mapper.GetType().GetMethod("Map").Invoke(mapper, new[] { entityBuilder });
            }
            #endregion
            //builder.Entity<ApplicationUser>().HasOne(a => a.UserSetting).WithOne();
        }
        // public DbSet<UserSetting> UserSetting { get; set; }
    }
}
