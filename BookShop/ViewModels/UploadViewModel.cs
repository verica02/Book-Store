using Microsoft.AspNetCore.Mvc;

namespace MakeupShop.ViewModels
{
    public class UploadViewModel
    {
        [BindProperty]
        public IFormFile file { get; set; }
        [BindProperty]
        public int? ID { get; set; }
    }
}
