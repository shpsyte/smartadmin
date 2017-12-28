
namespace Smart.Data.Context
{


    //    public class SmartContextIdentity : IdentityDbContext<ApplicationUser>
    //    {

    //        public SmartContextIdentity(DbContextOptions<SmartContextIdentity> options) : base(options)
    //        {

    //        }


    //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //        {

    //            //string conection = "Data Source=idellserver\\sqlexpress;Initial Catalog=SmartAdmin;User Id=iscosistemas;Password=keycode&senh@01;";
    //            // define the database to use
    //            //optionsBuilder.UseSqlServer(conection);



    //        }
    //        protected override void OnModelCreating(ModelBuilder builder)
    //        {
    //            base.OnModelCreating(builder);
    //            // Customize the ASP.NET Identity model and override the defaults if needed.
    //            // For example, you can rename the ASP.NET Identity table names and more.
    //            // Add your customizations after calling base.OnModelCreating(builder);

    //             //builder.Entity<IdentityUser>().ToTable("User").Property(p => p.Id).HasColumnName("UserId");
    //             builder.Entity<ApplicationUser>().ToTable("User");
    //             builder.Entity<ApplicationUser>().HasKey(a => a.Id);
    //             builder.Entity<IdentityUserLogin<string>>().HasKey(a => a.UserId);




    //             builder.Entity<IdentityRole>().ToTable("Role");
    //             builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
    //             builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
    //             builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
    //             builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");
    //             builder.Entity<IdentityUserToken<string>>().ToTable("UserToken");





    //        }

    //    }

}
