using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MakeupShop.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        public string? Image { get; set; }

        public byte[]? CatalogDownloadUrl { get; set; }

        public ICollection<Book>? Book { get; set; }
    }
}
