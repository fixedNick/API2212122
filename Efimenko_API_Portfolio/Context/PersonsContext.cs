using Efimenko_API_Portfolio.Models;
using Microsoft.EntityFrameworkCore;

namespace Efimenko_API_Portfolio.Context
{
    public class PersonsContext : DbContext
    {
        public PersonsContext(DbContextOptions<PersonsContext> opts) : base(opts)
        {
            if (Persons != null)
            {
                foreach (var p in Persons)
                {
                    if (p.Achievments != null && p.Achievments.Length > 0)
                        p.UpdateAchivmentsListFromJson();
                }
            }
        }

        public DbSet<Person> Persons { get; set; }
    }
}
