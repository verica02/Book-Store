using System.ComponentModel.DataAnnotations;

namespace MakeupShop.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required]
        [StringLength(450)]
        public string AppUser { get; set; }

        [Required]
        [StringLength(500)]
        public string Comment { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public int BookId { get; set; }
        public Book? Book { get; set; }
    }
}
