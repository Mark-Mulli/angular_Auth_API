using System;
using angular_auth_API.models;
using Microsoft.EntityFrameworkCore;

namespace angular_auth_API.context
{
	public class addDbContext: DbContext
	{
		public addDbContext(DbContextOptions<addDbContext> options) : base(options)
		{

		}

		public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			modelBuilder.Entity<User>().ToTable("users");
        }
    }
}

