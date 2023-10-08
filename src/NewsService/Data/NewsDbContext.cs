using Microsoft.EntityFrameworkCore;
using NewsService.Models;

namespace NewsService.Data
{
    public class NewsDbContext: DbContext
    {
        public NewsDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<News> News { get; set; }
    }
}
