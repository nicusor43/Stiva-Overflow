using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProiectStackOverflow.Models;

namespace ProiectStackOverflow.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options)
            : base(options)
		{
		}

		public DbSet<ApplicationUser> User { get; set; }
		public DbSet<Question> Questions { get; set; }
		public DbSet<Tag> Tags { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Answer> Answers { get; set; }
	}
}
