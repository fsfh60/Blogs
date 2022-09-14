using BlogRUs.Models;
using BlogRUs.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace BlogRUs.Controllers
{
    public class BlogToDatabaseController : Controller
    {
        private readonly BlogContext _context;

        public BlogToDatabaseController(BlogContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searching, int? page)
        {
            if (!page.HasValue || page < 1)
            {
                page = 1;
            }

            var query = _context.Blogs.AsQueryable();
            if (!String.IsNullOrEmpty(searching)) //it has something
            {
                ViewBag.Term = searching;
                query = query.Where(x => x.Title != null && x.Title.Contains(searching));
            }

            int numberOfMatchingRecords = await query.CountAsync();
            int pageSize = 2;
            int totalPages = (int)Math.Ceiling((decimal)(numberOfMatchingRecords / pageSize));

            int start = (page.Value - 1) * pageSize;

            var records = await query.OrderByDescending(x => x.Id)
                .Select(x => new DisplayBlogViewModel
                {
                    Id = x.Id,
                    Description = x.Description,
                    Title = x.Title,
                })
                .Skip(start)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new ListBlogViewModel()
            {
                PageNumber = page,
                PageSize = page,
                RecordsCount = numberOfMatchingRecords,
                TotalPages = totalPages,
                Records = records,
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            // Try to get the record from the database or null of none
            Blog blog = await _context.Blogs.FirstOrDefaultAsync(x => x.Id == id);

            if (blog == null)
            {
                // if blog is null we know there is no mathing blog in the database to the given id
                // so we show 404 error aka not found
                return NotFound();
            }

            // create the viewmodel, and pass it to the view
            var viewModel = new BlogViewModel()
            {
                Title = blog.Title,
                Description = blog.Description,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _context.Blogs.FirstAsync(x => x.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, BlogViewModel viewModel)
        {
            var blog = await _context.Blogs.FirstOrDefaultAsync(x => x.Id == id);

            if (blog == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                blog.Title = viewModel.Title;
                blog.Description = viewModel.Description;
                _context.Blogs.Update(blog);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Create(BlogViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // validate blog
                var blog = new Blog
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                };
                _context.Blogs.Add(blog);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(viewModel);
        }
    }
}


