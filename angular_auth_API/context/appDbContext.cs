using System;
using angular_auth_API.models;
using Microsoft.EntityFrameworkCore;

namespace angular_auth_API.context
{
	public class appDbContext: DbContext
	{
        public appDbContext(DbContextOptions<appDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
        }
    }
}

