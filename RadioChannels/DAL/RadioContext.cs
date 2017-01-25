using RadioChannels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace RadioChannels.DAL
{
	public class RadioContext : DbContext
	{
        public RadioContext() : base("RadioContext")
        {
        }

        public DbSet<Favourite> Favourite { get; set; }
        public DbSet<Channel> Channel { get; set; }
        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}