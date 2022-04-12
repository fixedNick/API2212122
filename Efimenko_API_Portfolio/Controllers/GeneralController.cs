using Efimenko_API_Portfolio.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Efimenko_API_Portfolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        // show all articles
        // show all users data
        ArticlesContext ArticlesDatabase { get; set; }
        PersonsContext PersonsDatabase { get; set; }
        public GeneralController(ArticlesContext articlesContext, PersonsContext personsContext)
        {
            ArticlesDatabase = articlesContext;
            PersonsDatabase = personsContext;
        }

        [Route("ShowAllPersonsData")]
        [HttpPost]
        public IActionResult ShowAllPersonsData()
            => Ok(new {
                message = "All persons data",
                data = PersonsDatabase.Persons.Select(p => new { 
                    email = p.Email,
                    isAdmin = p.IsAdmin,
                    registrationDate = p.RegistrationDate,
                    achievments = p.Achievments,
                    country = p.Country,
                    photo = p.ProfilePhoto
                }).ToList()
            });
    }
}
