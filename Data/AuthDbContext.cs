using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TheBigBrainBlog.API.Data
{
    public class AuthDbContext: IdentityDbContext<IdentityUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Call the base method to ensure the default Identity model is created
            // Create the default Identity model
            base.OnModelCreating(builder);

            // Customize the Identity model if needed
            // Create unique IDs for roles
            //var readerRoleID = Guid.NewGuid().ToString();
            //var writerRoleID = Guid.NewGuid().ToString();

            var readerRoleID = "36cd2a2a-6aba-4e50-9150-93c0f7cf8ec2";
            var writerRoleID = "7c94ed92-705d-42cf-9b48-a62277dab155";



            // Create Readers and Writers roles
            var roles = new List<IdentityRole>
            {
                new IdentityRole { 
                    Id = readerRoleID,
                    Name = "Reader", 
                    NormalizedName = "READER",
                    ConcurrencyStamp = readerRoleID
                },
                new IdentityRole {
                    Id = writerRoleID,
                    Name = "Writer", 
                    NormalizedName = "WRITER",
                    ConcurrencyStamp = writerRoleID
                }
            };

            // Seeding roles into the database
            builder.Entity<IdentityRole>().HasData(roles);

            // Create default admin user
            var adminUserID = Guid.NewGuid().ToString();
            var admin = new IdentityUser()
            {
                Id = adminUserID,
                UserName = "admin@thebigbrainblog.com",
                Email = "admin@thebigbrainblog.com",
                NormalizedEmail = "admin@thebigbrainblog.com".ToUpper(),
                NormalizedUserName = "admin@thebigbrainblog.com".ToUpper()
            };

            // Hash the password (generate a password hash)
            admin.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(admin, "Admin@123");

            // Seeding admin user into the database
            builder.Entity<IdentityUser>().HasData(admin);

            // Assign roles to the admin user
            var adminRole = new List<IdentityUserRole<string>>()
            {
                new IdentityUserRole<string>()
                {
                    UserId = adminUserID,
                    RoleId = writerRoleID
                },
                new IdentityUserRole<string>()
                {
                    UserId = adminUserID,
                    RoleId = readerRoleID
                }
            };

            // Seeding user roles into the database
            builder.Entity<IdentityUserRole<string>>().HasData(adminRole);


        }
    }
}
