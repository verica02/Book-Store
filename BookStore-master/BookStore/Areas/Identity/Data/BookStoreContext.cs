using BookStore.Areas.Identity.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookStore.Areas.Identity.Data;

public class BookStoreContext : IdentityDbContext<BookStoreUser>
{
    public BookStoreContext(DbContextOptions<BookStoreContext> options)
        : base(options)
    {
    }

    public DbSet<BookStore.Models.Author> Author { get; set; } = default!;

    public DbSet<BookStore.Models.Genre>? Genre { get; set; }

    public DbSet<BookStore.Models.Review>? Review { get; set; }

    public DbSet<BookStore.Models.Book>? Book { get; set; }

    public DbSet<BookGenre> BookGenre { get; set; }

    public DbSet<UserBooks> UserBooks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());

        builder.Entity<BookGenre>()
                .HasOne(b => b.Book)
                .WithMany(bg => bg.Genres)
                .HasForeignKey(b => b.BookId);

        builder.Entity<BookGenre>()
            .HasOne(g => g.Genre)
            .WithMany(bg => bg.Books)
            .HasForeignKey(g => g.GenreId);

        builder.Entity<Book>()
            .HasOne<Author>(p => p.Author)
            .WithMany(p => p.Books)
            .HasForeignKey(p => p.AuthorId);
    }

    public DbSet<BookStore.Models.ProjectRole>? ProjectRole { get; set; }
}

public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<BookStoreUser>
{
    public void Configure(EntityTypeBuilder<BookStoreUser> builder)
    {
        builder.Property(u => u.FirstName).HasMaxLength(50);
        builder.Property(u => u.LastName).HasMaxLength(50);
    }
}