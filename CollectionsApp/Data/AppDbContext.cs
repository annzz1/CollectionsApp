using CollectionsApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CollectionsApp.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
            Collections = Set<Collection>();
            Items = Set<Item>();
            CustomFields = Set<CustomField>();

        }
        public DbSet<Collection> Collections;
        public DbSet<Item> Items;
        public DbSet<CustomField> CustomFields;


    }
}
