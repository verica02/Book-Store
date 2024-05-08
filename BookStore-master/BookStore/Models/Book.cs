using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BookStore.Models
{
    public class Book
    {
        public int Id { get; set; }

        [StringLength(100)]
        [Required]
        public string Title { get; set; }

        [Display(Name = "Year Published")]
        public int? YearPublished { get; set; }
        public int? NumPages { get; set; }
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Publisher { get; set; }

        public string? FrontPage { get; set; }
        public string? DownloadUrl { get; set; }

        [Display(Name = "Author")]
        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public ICollection<BookGenre> Genres { get; set; }

        public ICollection<Review> Reviews { get; set; }

        public ICollection<UserBooks> Users { get; set; }
    }
}
