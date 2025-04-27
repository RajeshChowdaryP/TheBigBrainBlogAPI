using Microsoft.EntityFrameworkCore;
using TheBigBrainBlog.API.Models.Domain;

namespace TheBigBrainBlog.API.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }

        //DbSet is the collection of entites (for each table we will create one entity and with that we will create a DbSet)
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BlogImage> BlogImages { get; set; }

    }
}
