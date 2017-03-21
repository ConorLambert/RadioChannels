using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using RadioChannels.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace RadioChannels.DAL
{
    public class RadioContext : IdentityDbContext<User>
    {
        public RadioContext() : base("RadioContext"){}
        
        public DbSet<Favourite> Favourite { get; set; }
        public DbSet<Channel> Channel { get; set; } // MAY NOT EVEN NEED THIS

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);            
        }

        // create a single instance of the Database for the duration of the application
        public static RadioContext Create()
        {
            return new RadioContext();
        }       
    }
}