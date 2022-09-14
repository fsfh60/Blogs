using BlogRUs.Api.Models;
using BlogRUs.Api.Models.V1;
using BlogRUs.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogRUs.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : Controller
    {
        private int MaxPageSize = 1000;
        private BlogContext _context;
        public BlogController(BlogContext context)
        {
            _context = context;
        }


        [HttpGet("BlogList")]
        public async Task<IActionResult> Get(string? searching, int? page, int? pageSize)
        {
            if (!page.HasValue || page < 0)
            {
                page = 1;
            }

            if (!pageSize.HasValue || pageSize.Value > MaxPageSize)
            {
                pageSize = MaxPageSize;
            }

            var query = _context.Blogs.AsQueryable();

            if (!String.IsNullOrEmpty(searching)) //it has something
            {
                query = query.Where(x => x.Title != null && x.Title.Contains(searching));
            }

            int totalRecord = await query.CountAsync();
            int start = (page.Value - 1) * pageSize.Value;

            int pageNumber = (totalRecord / pageSize.Value);
            int currentPage = page.Value;

            var records = await query.OrderByDescending(x => x.Id)
                .Skip(start)
                .Take(pageSize.Value)
                .Select(x => new DisplayBlog
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description
                })
                .ToListAsync();

            var pagination = new Pagination<DisplayBlog>
            {
                Count = totalRecord,
                PageSize = pageSize.Value,
                CurrentPage = currentPage,
                TotalPages = (int)Math.Ceiling(decimal.Divide(totalRecord, pageSize.Value)),
                IndexOne = (currentPage - 1) * pageSize.Value + 1,
                IndexTwo = ((currentPage - 1) * pageSize.Value + pageSize.Value) <= totalRecord ? ((currentPage - 1) * pageSize.Value + pageSize.Value) : totalRecord,
                Records = records,
            };

            return Ok(pagination);
        }


        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetSingleBlog([FromRoute] int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                return Ok(new DisplayBlog()
                {
                    Id = blog.Id,
                    Description = blog.Description,
                    Title = blog.Title,
                });
            }
            return NotFound();

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] BlogRequest addBlogRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var blog = new Blog()
            {
                Title = addBlogRequest.Title,
                Description = addBlogRequest.Description,
            };

            await _context.Blogs.AddAsync(blog);
            await _context.SaveChangesAsync();

            return Created("/", new DisplayBlog()
            {
                Id = blog.Id,
                Description = blog.Description,
                Title = blog.Title,
            });
        }
        //[HttpPost]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    Blog editId = _context.Blogs.Single(x => x.Id == id);
        //    await _context.Blogs.Remove(editId);
        //    await _context.SaveChangesAsync();
        //    return Ok(blog);
        //    [HttpPost]
        //    public IActionResult Delete(int id)
        //    {
        //        Blog editId = _context.Blogs.Single(x => x.Id == id);
        //        _context.Blogs.Remove(editId);
        //        _context.SaveChanges();
        //        return Ok();
        //    }
        //}
        [HttpPut]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public async Task<IActionResult> Update(int? id, [FromBody] BlogRequest updateBlogRequest)
        {
            if (!id.HasValue && !ModelState.IsValid)
            {
                return BadRequest();
            }

            var blog = await _context.Blogs.FindAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            blog.Title = updateBlogRequest.Title;
            blog.Description = updateBlogRequest.Description;

            await _context.SaveChangesAsync();

            return Ok(blog);
        }

        [HttpDelete]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }
            var blog = await _context.Blogs.FindAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            _context.Remove(blog);
            await _context.SaveChangesAsync();
            return Ok(blog);
        }



    }
}
