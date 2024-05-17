using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MakeupShop.Models
{
    //Makeup
    public class Book
    {
        public int Id { get; set; }

        [StringLength(50)]
        [Required]

        //Title
        public string Title { get; set; }

        public string? Description { get; set; }

        [Range(1, 100)]
        [DataType(DataType.Currency)]
        //Price
        public int? YearPublished { get; set; }

        public string? Image { get; set; }

        [StringLength(30)]
        //Category
        public string Genre { get; set; }

        [Display(Name = "Author")]
        //BrandId
        public int AuthorId { get; set; }
        //Brand
        public Author? Author { get; set; }

        public ICollection<Review> Reviews { get; set; }
        //UserMakeup
        public ICollection<UserBook> UserBook { get; set; }

    }
}
