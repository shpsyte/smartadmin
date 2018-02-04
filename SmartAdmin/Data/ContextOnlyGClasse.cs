using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Smart.Core.Domain.Addresss;
using Smart.Core.Domain.Goals;

namespace SmartAdmin.Data
{
    public partial class ContextOnlyGClasse : DbContext
    {
      

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=idellserver\sqlexpress;Initial Catalog=SmartAdmin;User Id=iscosistemas;Password=keycode&senh@01;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
      
        }

        public DbSet<Smart.Core.Domain.Addresss.City> City { get; set; }

        public DbSet<Smart.Core.Domain.Goals.Goal> Goal { get; set; }
        
    }
}