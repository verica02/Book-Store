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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;

namespace MakeupShop.Controllers
{
    public class BookController : Controller
    {
        private readonly MakeupShopContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public BookController(MakeupShopContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
        }

        // GET: Makeup
        public async Task<IActionResult> Index(string makeupCategory, string searchString)
        {
            IQueryable<Book> makeup = _context.Book.AsQueryable();
            IQueryable<string> categoryQuery = _context.Book.OrderBy(m => m.Genre).Select(m => m.Genre).Distinct();

            if (!string.IsNullOrEmpty(searchString))
            {
                makeup = makeup.Where(s => s.Title.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(makeupCategory))
            {
                makeup = makeup.Where(x => x.Genre == makeupCategory);
            }

            makeup = makeup.Include(m => m.Author).Include(x => x.Reviews);

            var makeupCategoryVM = new BookGenreViewModel
            {
                Genres = new SelectList(await categoryQuery.ToListAsync()),
                Books = await makeup.ToListAsync()
            };

            return View(makeupCategoryVM);
        }

        // GET: Makeup/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var makeup = await _context.Book
                .Include(m => m.Author).Include(x => x.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (makeup == null)
            {
                return NotFound();
            }

            return View(makeup);
        }

        // GET: Makeup/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "Title");
            return View();
        }

        // POST: Makeup/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookModel viewmodel)
        {
            string uniqueFileName = UploadedFile(viewmodel);

            await _context.Book.AddAsync(new Book()
            {
                Title = viewmodel.Book.Title,
                Description = viewmodel.Book.Description,
                YearPublished = viewmodel.Book.YearPublished,
                Image = uniqueFileName,
                Genre = viewmodel.Book.Genre,
                AuthorId = viewmodel.Book.AuthorId
            });
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: Makeup/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var makeup = await _context.Book.FindAsync(id);
            if (makeup == null)
            {
                return NotFound();
            }

            BookModel viewmodel = new BookModel
            {
                Book = makeup
            };

            ViewData["BrandId"] = new SelectList(_context.Set<Author>(), "Id", "Title", makeup.AuthorId);
            return View(viewmodel);
        }

        // POST: Makeup/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookModel viewmodel)
        {
            if (id != viewmodel.Book.Id)
            {
                return NotFound();
            }

            string uniqueFileName = UploadedFile(viewmodel);

                try
                {
                    var makeup = _context.Book.Where(b => b.Id == id).First();
                    makeup.Image = uniqueFileName;
                    makeup.Title = viewmodel.Book.Title;
                    makeup.Description = viewmodel.Book.Description;
                    makeup.YearPublished = viewmodel.Book.YearPublished;
                    makeup.Genre = viewmodel.Book.Genre;
                    makeup.AuthorId = viewmodel.Book.AuthorId;
                    _context.Update(makeup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(viewmodel.Book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));

        }

        // GET: Makeup/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var makeup = await _context.Book
                .Include(m => m.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (makeup == null)
            {
                return NotFound();
            }

            return View(makeup);
        }

        // POST: Makeup/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'MakeupShopContext.Book'  is null.");
            }
            var makeup = await _context.Book.FindAsync(id);
            if (makeup != null)
            {
                string photoPath = Path.Combine(webHostEnvironment.WebRootPath, "images/" + makeup.Image);

                if (System.IO.File.Exists(photoPath))
                {
                    System.IO.File.Delete(photoPath);
                }
                _context.Book.Remove(makeup);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private string UploadedFile(BookModel model)
        {
            string uniqueFileName = null;

            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.Image.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
