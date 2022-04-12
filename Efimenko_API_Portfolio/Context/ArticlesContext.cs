using Efimenko_API_Portfolio.Models;
using Microsoft.EntityFrameworkCore;

namespace Efimenko_API_Portfolio.Context
{
    public class ArticlesContext : DbContext
    {
        public ArticlesContext(DbContextOptions<ArticlesContext> opts) : base(opts)
        {

        }

        public DbSet<Article> Articles { get; set; }
    }
}
