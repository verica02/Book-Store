using MakeupShop.Models;

namespace MakeupShop.ViewModels
{
    public class AuthorTitleViewModel
    {
        public IList<Author> Authors { get; set; }
        public string SearchString { get; set; }
    }
}
