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
            CustomFieldsValues = Set<ItemCustomFieldVal>();

        }
        public DbSet<Collection> Collections;
        public DbSet<Item> Items;
        public DbSet<CustomField> CustomFields;
        public DbSet<ItemCustomFieldVal> CustomFieldsValues;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ItemCustomFieldVal>()
                .HasOne(cf => cf.Item)
                .WithMany(i => i.ItemCustomFieldVals)
                .HasForeignKey(cf => cf.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}
