using BookManagement.Data;
using BookManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string title = "", string author = "", int? yearFrom = null, int? yearTo = null)
        {
            /*var allBooks = _context.Books.ToList();
            _context.Books.RemoveRange(allBooks);  
            await _context.SaveChangesAsync(); */

            /*var sampleBooks = new List<Book>();
            for (int i = 1; i <= 120; i++)
            {
                sampleBooks.Add(new Book
                {
                    Title = $"Book {i}",
                    Author = $"Author {i}",
                    YearPublished = 2000 + (i % 20)
                });
            }
            await _context.Books.AddRangeAsync(sampleBooks);
            await _context.SaveChangesAsync();*/
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(b => b.Title.ToLower().Contains(title.ToLower()));
            }

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(b => b.Author.ToLower().Contains(author.ToLower()));
            }

            if (yearFrom.HasValue)
            {
                query = query.Where(b => b.YearPublished >= yearFrom.Value);
            }

            if (yearTo.HasValue)
            {
                query = query.Where(b => b.YearPublished <= yearTo.Value);
            }

            var totalBooks = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);
            var books = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Books = books;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalBooks = totalBooks;
            ViewBag.PageSize = pageSize;
            ViewBag.Title = title; // ส่งค่าค้นหาจาก title
            ViewBag.Author = author; // ส่งค่าค้นหาจาก author
            ViewBag.YearFrom = yearFrom; // ส่งค่าค้นหาจาก yearFrom
            ViewBag.YearTo = yearTo; // ส่งค่าค้นหาจาก yearTo

            return View();
        }



        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Title, Author, YearPublished")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id , Title , Author , YearPublished")] Book book)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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
            return View(book);

        }
        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
