using Humanizer.Localisation.TimeToClockNotation;
using MakeupShop.Data;
using MakeupShop.Models;
using MakeupShop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Drawing.Drawing2D;

namespace MakeupShop.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly MakeupShopContext _context;

        public ReviewsController(MakeupShopContext context)
        {
            _context = context;
        }

        public IActionResult Create(int? id)
        {
            var makeup = _context.Book.Where(m => m.Id == id).FirstOrDefault();
            if (makeup != null)
            {
                ViewBag.Message = makeup.Title;
            }
            if (id != null)
            {
                ViewBag.MakeupId = id;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewModel viewmodel)
        {
            await _context.Review.AddAsync(new Review()
            {
                AppUser = viewmodel.Review.AppUser,
                Comment = viewmodel.Review.Comment,
                Rating = viewmodel.Review.Rating,
                BookId = viewmodel.BookId
            });

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Book", new { id = viewmodel.BookId });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Review == null)
            {
                return NotFound();
            }
            var review = await _context.Review.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            var BookId = review.BookId;
            var makeup = _context.Book.Where(m => m.Id == BookId).FirstOrDefault();
            if (makeup != null)
            {
                ViewBag.Message = makeup.Title;
            }
            if (BookId != null)
            {
                ViewBag.BookId = BookId;
            }
            ReviewModel viewmodel = new ReviewModel
            {
                Review = review,
                BookId = BookId
            };
            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReviewModel viewmodel)
        {
            if (id != viewmodel.Review.Id)
            {
                return NotFound();
            }

            try
            {
                var review = _context.Review.Where(r => r.Id == id).First();

                review.Comment = viewmodel.Review.Comment;
                review.Rating = viewmodel.Review.Rating;

                _context.Update(review);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(viewmodel.Review.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Details", "Book", new { id = viewmodel.BookId });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Review == null)
            {
                return NotFound();
            }

            var review = await _context.Review
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }


            return View(review);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Review == null)
            {
                return Problem("Entity set 'MakeupShopContext.Review'  is null.");
            }
            var review = await _context.Review.FindAsync(id);
            int MakeupId = review.BookId;

            if (review != null)
            {
                _context.Review.Remove(review);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Book", new { id = MakeupId });
        }

        private bool ReviewExists(int id)
        {
            return (_context.Review?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
