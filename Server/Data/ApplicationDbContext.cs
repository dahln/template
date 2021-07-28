using ghostlight.Server.Entities;
using ghostlight.Server.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ghostlight.Server.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        public DbSet<Folder> Folders { get; set; }
        public DbSet<AuthorizedUser> AuthorizedUsers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerFile> CustomerFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Customer>()
            //    .HasOne(c => c.User)
            //    .WithMany(c => c.Customers)
            //    .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CustomerFile>()
                .HasOne(c => c.Customer)
                .WithMany(x => x.Files)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
