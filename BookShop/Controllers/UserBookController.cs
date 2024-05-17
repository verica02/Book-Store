using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MakeupShop.Data;
using MakeupShop.Models;
using MakeupShop.ViewModels;

namespace MakeupShop.Controllers
{
    public class UserBookController : Controller
    {
        private readonly MakeupShopContext _context;

        public UserBookController(MakeupShopContext context)
        {
            _context = context;
        }

        // GET: UserMakeup
        public async Task<IActionResult> Index()
        {
            var makeupShopContext = _context.UserBook.Where(u => u.AppUser == User.Identity.Name).Include(u => u.Book);
            return View(await makeupShopContext.ToListAsync());
        }

        public async Task<IActionResult> Buy(int? id, UserBookVM viewmodel)
        {
            var makeup = _context.Book.Where(m => m.Id == id).FirstOrDefault();
            ViewBag.MakeupName = makeup.Title;
            ViewBag.MakeupId = id;
            ViewBag.Price = makeup.YearPublished;
            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy(UserBookVM viewmodel)
        {
            await _context.UserBook.AddAsync(new UserBook()
            {
                AppUser = viewmodel.UserBook.AppUser,
                BookId = viewmodel.BookId
            });

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Book");
        }

        private bool UserMakeupExists(int id)
        {
          return (_context.UserBook?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
