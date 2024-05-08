using BookStore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.ViewModels
{
    public class AuthorTitleViewModel
    {
        public IList<Author> Authors { get; set; }
        public SelectList Nationality { get; set; }
        public string AuthorNationality { get; set; }
        public string SearchString { get; set; }
        public string SearchStringLastName { get; set; }
    }
}
