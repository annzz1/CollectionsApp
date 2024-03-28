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
            Comments = Set<Comment>();
            Likes = Set<Like>();

        }
        public DbSet<Collection> Collections;
        public DbSet<Item> Items;
        public DbSet<CustomField> CustomFields;
        public DbSet<ItemCustomFieldVal> CustomFieldsValues;
        public DbSet<Comment> Comments;
        public DbSet<Like> Likes;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           

            modelBuilder.Entity<ItemCustomFieldVal>()
                .HasOne(cf => cf.Item)
                .WithMany(i => i.ItemCustomFieldVals)
                .HasForeignKey(cf => cf.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Comment>()
            .HasOne(c => c.Item)
            .WithMany(i => i.Comments)
                .HasForeignKey(c => c.itemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Like>()
             .HasOne(c => c.Item)
             .WithMany(i => i.Likes)
              .HasForeignKey(c => c.ItemId)
             .OnDelete(DeleteBehavior.Restrict);
            base.OnModelCreating(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }


    }
}
