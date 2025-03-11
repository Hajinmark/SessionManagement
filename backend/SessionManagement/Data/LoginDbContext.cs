using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SessionManagement.Models;
using System;

namespace SessionManagement.Data
{
    public class LoginDbContext : DbContext
    {
        public LoginDbContext(DbContextOptions<LoginDbContext> options) : base(options)
        {
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
                .HasOne(e => e.UserDetails)
                .WithOne(e => e.Users)
                .HasForeignKey<UserDetails>(e => e.UserID)
                .IsRequired();

            base.OnModelCreating(modelBuilder);     
        }
    }
}

