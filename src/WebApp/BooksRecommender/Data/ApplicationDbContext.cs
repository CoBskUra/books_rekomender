using BooksRecommender.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksRecommender.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<ReadBook> ReadBooks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ApplicationUser user = new();
            user.Email = "user@book.com";
            user.UserName = "user@book.com";
            user.EmailConfirmed = true;
            user.PhoneNumberConfirmed = false;
            user.LockoutEnabled = false;
            user.AccessFailedCount = 0;
            modelBuilder.Entity<ApplicationUser>().HasData(user);
        }
    }
}
