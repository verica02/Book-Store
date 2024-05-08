using BookStore.Areas.Identity.Data;
using BookStore.Models;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BookStore.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookStoreContext _context;

        public BooksController(BookStoreContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index(string bookGenre, string searchString)
        {
            IQueryable<Book> books = _context.Book.AsQueryable();
            var genres = _context.Genre.AsEnumerable();
            var bookgenres = _context.BookGenre.AsQueryable();
            int? genreId;
            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(s => s.Title.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(bookGenre))
            {
                foreach (var g in genres)
                {
                    if (g.Id.ToString().Equals(bookGenre))
                    {
                        genreId = g.Id;
                        foreach (var bg in bookgenres)
                        {
                            if (bg.GenreId == genreId)
                            {
                                books = books.Where(x => x.Id == bg.BookId);
                            }
                        }
                    }
                }
            }

            books = books.Include(b => b.Author).Include(x => x.Reviews);

            var bookGenreVM = new BookGenreFilterViewModel
            {
                Genres = new SelectList(genres, "Id", "GenreName"),
                Books = await books.ToListAsync()
            };

            return View(bookGenreVM);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author).Include(x => x.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        
        public IActionResult Create()
        {
            var genres = _context.Genre.AsEnumerable();

            BookGenreViewModel viewmodel = new BookGenreViewModel
            {
                Book = new Book(),
                GenreList = new MultiSelectList(genres, "Id", "GenreName")
            };
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName");
            return View(viewmodel);
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookGenreViewModel viewmodel)
        {
            IEnumerable<int> newGenreList = viewmodel.SelectedGenres;
            //if (!ModelState.IsValid)
            //{
            //    ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName", viewmodel.Book.AuthorId);
            //    return View(viewmodel);
            //}
            //string uniqueFileName = UploadedFile(viewmodel.Book);

            await _context.Book.AddAsync(new Book()
            {
                Title = viewmodel.Book.Title,
                YearPublished = viewmodel.Book.YearPublished,
                NumPages = viewmodel.Book.NumPages,
                Description = viewmodel.Book.Description,
                Publisher = viewmodel.Book.Publisher,
                FrontPage = viewmodel.Book.FrontPage,
                DownloadUrl = viewmodel.Book.DownloadUrl,
                AuthorId = viewmodel.Book.AuthorId
            });

            await _context.SaveChangesAsync();

            var thisBook = await _context.Book.Where(s => s.Title == viewmodel.Book.Title).FirstOrDefaultAsync();

            if (newGenreList != null)
            {
                foreach (int genreId in newGenreList)
                {
                    _context.BookGenre.Add(new BookGenre { GenreId = genreId, BookId = thisBook.Id });
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Edit/5
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = _context.Book.Where(b => b.Id == id).Include(b => b.Genres).First();

            if (book == null)
            {
                return NotFound();
            }

            var genres = _context.Genre.AsEnumerable();
            genres = genres.OrderBy(s => s.GenreName);

            BookGenreViewModel viewmodel = new BookGenreViewModel
            {
                Book = book,
                GenreList = new MultiSelectList(genres, "Id", "GenreName"),
                SelectedGenres = book.Genres.Select(sa => sa.GenreId)
            };
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName", book.AuthorId);
            return View(viewmodel);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookGenreViewModel viewmodel)
        {
            if (id != viewmodel.Book.Id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
            try
            {
                _context.Update(viewmodel.Book);
                await _context.SaveChangesAsync();

                IEnumerable<int> newGenreList = viewmodel.SelectedGenres;
                IEnumerable<int> prevGenreList = _context.BookGenre.Where(s => s.BookId == id).Select(s => s.GenreId);
                IQueryable<BookGenre> toBeRemoved = _context.BookGenre.Where(s => s.BookId == id);

                if (newGenreList != null)
                {
                    toBeRemoved = toBeRemoved.Where(s => !newGenreList.Contains(s.GenreId));

                    foreach (int genreId in newGenreList)
                    {
                        if (!prevGenreList.Any(s => s == genreId))
                        {
                            _context.BookGenre.Add(new BookGenre { GenreId = genreId, BookId = id });
                        }
                    }
                }
                _context.BookGenre.RemoveRange(toBeRemoved);
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
            //}
            //ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName", viewmodel.Book.AuthorId);
            //return View(viewmodel);
        }

        // GET: Books/Delete/5
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'eBooksContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
