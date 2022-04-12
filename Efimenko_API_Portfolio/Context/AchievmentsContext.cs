using Efimenko_API_Portfolio.Models;
using Microsoft.EntityFrameworkCore;

namespace Efimenko_API_Portfolio.Context
{
    public class AchievmentsContext : DbContext
    {
        public AchievmentsContext(DbContextOptions<AchievmentsContext> opts) : base(opts)
        {

        }

        public DbSet<Achievment> Achievments { get; set; }
    }
}
