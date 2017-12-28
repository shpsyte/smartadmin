using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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
    }
}