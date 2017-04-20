namespace BillsManagementWebApp.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Collections.Generic;

    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext() : base("name=defConnString")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDBContext, BillsManagementWebApp.Migrations.Configuration>("defConnString"));
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<BillEntry> BillEntries { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
    }
}