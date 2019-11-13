using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Models.Repository
{
    public sealed class SecurityDbContext : IdentityDbContext<ApplicationUser>
    {
        public SecurityDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserGroupCatalogs>()
                .HasKey(t => new { t.UserGroupId, t.CatalogId });
            builder.Entity<TagResources>()
                .HasKey(t => new { t.TagId, t.ResourceId });
            builder.Entity<Tag>()
                .Property(tag => tag.Id)
                .ValueGeneratedOnAdd();
        }

        public DbSet<Resource> Resources { get; set;  }
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}