using System.ComponentModel.DataAnnotations;

namespace MakeupShop.Models
{
    public class UserBook
    {
        public int Id { get; set; }

        [StringLength(450)]
        public string AppUser { get; set; }

        public int BookId { get; set; }
        public Book? Book { get; set; }
    }
}
