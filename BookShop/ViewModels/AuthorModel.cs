using MakeupShop.Models;

namespace MakeupShop.ViewModels
{
    public class AuthorModel
    {
        public Author Author { get; set; }
        public IFormFile Image { get; set; }
    }
}
