using Microsoft.EntityFrameworkCore;
using BlogRUs.Models;

namespace BlogRUs.Models
{
    public class BlogContext : DbContext
    {    
        public  DbSet<Blog> Blogs { get; set; }

        public BlogContext(DbContextOptions options) : base(options)
        {

        }

    }
}
