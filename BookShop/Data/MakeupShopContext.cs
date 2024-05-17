using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MakeupShop.Areas.Identity.Data;
using MakeupShop.Models;

namespace MakeupShop.Data
{
    public class MakeupShopContext : IdentityDbContext<MakeupShopUser>
    {
        public MakeupShopContext (DbContextOptions<MakeupShopContext> options)
            : base(options)
        {
        }

        public DbSet<MakeupShop.Models.Book> Book { get; set; } = default!;

        public DbSet<MakeupShop.Models.Author>? Author { get; set; }

        public DbSet<MakeupShop.Models.UserBook>? UserBook { get; set; }

        public DbSet<MakeupShop.Models.Review>? Review { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Book>()
                .HasOne<Author>(p => p.Author)
                .WithMany(p => p.Book)
                .HasForeignKey(p => p.AuthorId);

            builder.Entity<UserBook>()
                .HasOne<Book>(p => p.Book)
                .WithMany(p => p.UserBook)
                .HasForeignKey(p => p.BookId);

            builder.Entity<Review>()
                .HasOne<Book>(p => p.Book)
                .WithMany(p => p.Reviews)
                .HasForeignKey(p => p.BookId);

            base.OnModelCreating(builder);
        }
    }
}
