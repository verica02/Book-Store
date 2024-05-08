using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Genre Name is required.")]
        [StringLength(50)]
        public string GenreName { get; set; }

        public ICollection<BookGenre>? Books { get; set; }
    }
}
