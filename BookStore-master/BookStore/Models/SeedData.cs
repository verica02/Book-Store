using Microsoft.AspNetCore.Identity;
using BookStore.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Models
{
    public class SeedData
    {
        

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new BookStoreContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<BookStoreContext>>()))
            {

                if (context.Book.Any() || context.Author.Any() || context.Genre.Any() || context.Review.Any())
                {
                    return;
                }

                context.Author.AddRange(
                    new Author { FirstName = "Don", LastName = "DeLillo", BirthDate = DateTime.Parse("1936-11-20"), Nationality = "American", Gender = "Male" }
                );
                context.SaveChanges();

                context.Genre.AddRange(
                    new Genre { GenreName = "Fiction" },
                    new Genre { GenreName = "Postmodern literature" },
                    new Genre { GenreName = "Humor"}
                );
                context.SaveChanges();

                context.Book.AddRange(
                    new Book
                    {
                        Title = "White Noise",
                        YearPublished = 1985,
                        NumPages = 384,
                        Description = "White Noise is an effortless combination of social satire and metaphysical dilemma in which Don DeLillo exposes our rampant consumerism, media saturation and novelty intellectualism. It captures the particular strangeness of life lived when the fear of death cannot be denied, repressed or obscured and ponders the role of the family in a time when the very meaning of our existence is under threat.",
                        Publisher = "Viking Adult",
                        FrontPage = "img/book01.jpg",
                        DownloadUrl = "http://webdelprofesor.ula.ve/humanidades/cpozzobon/Downloads_files/WHITE%20NOISE.pdf",
                        AuthorId = context.Author.Single(a => a.FirstName == "Don" && a.LastName == "DeLillo").Id
                    }
                );
                context.SaveChanges();

                context.Review.AddRange(
                    new Review
                    {
                        AppUser = "user1",
                        Comment = "I love this book",
                        Rating = 5,
                        BookId = context.Book.Single(b => b.Title == "White Noise").Id
                    }
                );
                context.SaveChanges();

                context.BookGenre.AddRange(
                    new BookGenre { BookId = 1, GenreId = 1 },
                    new BookGenre { BookId = 1, GenreId = 2 },
                    new BookGenre { BookId = 1, GenreId = 3 }
                );
                context.SaveChanges();

                
            }
        }
    }
}
