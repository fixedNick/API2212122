using Efimenko_API_Portfolio.Context;
using Efimenko_API_Portfolio.Models;
using Efimenko_API_Portfolio.Models.BodyModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Efimenko_API_Portfolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        PersonsContext PersonsDatabase { get; set; }
        ArticlesContext ArticlesDatabase { get; set; }
        AchievmentsContext AchievmentsDatabase { get; set; }
        public AdminController(PersonsContext persContext, AchievmentsContext achContext, ArticlesContext artContext)
        {
            PersonsDatabase = persContext;
            ArticlesDatabase = artContext;
            AchievmentsDatabase = achContext;
        }

        #region Delete/Update Person
        [Route("DeleteUser/{id}")]
        [HttpDelete]
        [Authorize(Roles = "admin")]
        public IActionResult DeleteUser(int Id)
        {
            var person = PersonsDatabase.Persons.Where(u => u.Id == Id).FirstOrDefault();
            if (person == null)
                return NotFound(new { messege = "User is not found" });
            PersonsDatabase.Persons.Remove(person);
            PersonsDatabase.SaveChanges();
            return Ok(new { messege = $"User with id {Id} is deleted" });
        }

        [Route("UpdateUser")]
        [HttpPut]
        [Authorize(Roles = "admin")]
        public IActionResult UpdateUser([FromBody] PersonUpdate personUpdate)
        {
            var person = PersonsDatabase.Persons.Where(u => u.Id == personUpdate.Id).FirstOrDefault();
            if (person == null)
                return NotFound(new { messege = "User is not found" });

            person.Email = personUpdate.Email ?? person.Email;
            person.Password = personUpdate.Password ?? person.Email;
            person.IsAdmin = personUpdate.IsAdmin ?? person.IsAdmin;
            if (personUpdate.Achievments != null)
            {
                person.Achievments = personUpdate.Achievments;
                person.UpdateAchivmentsListFromJson();
            }

            PersonsDatabase.Update(person);
            PersonsDatabase.SaveChanges();
            return Ok(new { messege = $"User with id {personUpdate.Id} is updated" });
        }
        #endregion

        #region Update/Delete Article & GiveArticleCategory

        [Route("UpdateArticle/{id}")]
        [HttpPut]
        [Authorize(Roles = "admin")]
        public IActionResult UpdateArticle(int id, [FromBody] ArticleUpdate articleData)
        {
            // Получаем id, получаем данные для изменения
            // Пытаемся найти Article с таким id
            // => Если не нашли -> NotFound
            // => Если нашли -> Изменяем его в бд

            var article = ArticlesDatabase.Articles.Where(a => a.Id == id).FirstOrDefault();
            if (article == null)
                return NotFound(new { error = $"Article with id {id} not found" });

            article.Title = articleData.Title ?? article.Title;
            article.Category = articleData.Category ?? article.Category;
            article.Content = articleData.Content ?? article.Content;

            ArticlesDatabase.Update(article);
            ArticlesDatabase.SaveChanges();
            return Ok(new { message = $"Article's data with id '{id}' successfully updated" });
        }

        [Route("DeleteArticle/{id}")]
        [HttpDelete]
        [Authorize(Roles = "admin")]
        public IActionResult DeleteArticle(int id)
        {
            var article = ArticlesDatabase.Articles.Where(a => a.Id == id).FirstOrDefault();
            if (article == null)
                return NotFound(new { messege = "Article is not found" });

            ArticlesDatabase.Articles.Remove(article);
            ArticlesDatabase.SaveChanges();

            return Ok(new { messege = $"Article with id '{id}' is deleted" });
        }

        [Route("GiveCategoryToArticle/{id}")]
        [HttpPut]
        [Authorize(Roles = "admin")]
        public IActionResult GiveCategoryToArticle(int id, [FromBody][Required] string category)
        {
            // проверяем есть article с таким id
            // обновляем категорию
            var article = ArticlesDatabase.Articles.Where(a => a.Id == id).FirstOrDefault();
            if (article == null)
                return NotFound(new { error = $"Article with id '{id}' doesn't exists"});

            article.Category = category;
            ArticlesDatabase.Update(article);
            ArticlesDatabase.SaveChanges();

            return Ok();
        }

        #endregion

        #region Create/Update/Delete and GivePerson Achievment
        [Route("GivePersonAnAchievment/{id}")] // person id
        [HttpPut]
        [Authorize(Roles = "admin")]
        // id - person id
        public IActionResult GivePersonAnAchievment(int id, [FromBody] AchievmentData achievmentData)
        {
            if (achievmentData.Id == null)
                return BadRequest(new { error = "Achievment Id field is required for method GivePersonAnAchievment" });
            // проверить существования пользователя с таким id
            // проверить существование ачивки с таким id
            // проверить, что у пользователя уже нет такой ачивки
            // добавить ачивку пользователю
            var person = PersonsDatabase.Persons.Where(p => p.Id == id).FirstOrDefault();
            if (person == null)
                return NotFound(new { error = $"Person wkith id '{id}' not found" });

            var achievment = AchievmentsDatabase.Achievments.Where(a => a.Id == achievmentData.Id).FirstOrDefault();
            if (achievment == null)
                return NotFound(new { error = $"Achievment with id '{achievmentData.Id}' not found" });

            if (person.TryAddNewAchievment(achievment) == false)
                return BadRequest(new { error = $"Person with id '{person.Id}' already have an achievment with id '{achievment.Id}'" });

            person.UpdateAchievmentsJsonFromList();
            PersonsDatabase.SaveChanges();
            return Ok(new
            {
                message = $"Achievment with id '{achievment.Id}' successfully added to person with id '{person.Id}'",
                personData = person
            });
        }
        [Route("CreateAchievment")]
        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult CreateAchievment([FromBody] AchievmentData achievmentData)
        {
            if (achievmentData.Name == null)
                return BadRequest(new { error = "You cannot create achievment w/o field Name" });

            // Проверим есть ли уже в БД ачивка с таким именем
            var achievment = AchievmentsDatabase.Achievments.Where(a => a.Name.ToLower().Equals(achievmentData.Name.ToLower())).FirstOrDefault();
            if (achievment != null)
                return BadRequest(new { error = $"Achievment with name '{achievment.Name}' already exists" });

            achievment = Achievment.Create(achievmentData.Name);
            AchievmentsDatabase.Achievments.Add(achievment);
            AchievmentsDatabase.SaveChanges();
            return Ok(new
            {
                message = "Achievment successfully created.",
                achievmentData = achievment
            });
        }
        [Route("UpdateAchievment")]
        [HttpPut]
        [Authorize(Roles = "admin")]
        public IActionResult UpdateAchievment([FromBody] AchievmentData achievmentData)
        {
            if (achievmentData.Id == null)
                return BadRequest(new { error = "Field 'Id' required to UpdateAchievment method" });

            // проверить есть ли такая ачивка по id
            // заменить полученные поля
            // обновить ачиыку
            var achievment = AchievmentsDatabase.Achievments.Where(a => a.Id == achievmentData.Id).FirstOrDefault();
            if (achievment == null)
                return NotFound(new { error = $"Achievment with id '{achievmentData.Id}' not found!" });

            achievment.Name = achievmentData.Name ?? achievment.Name;
            achievment.AwardDate = achievmentData.AwardDate ?? achievment.AwardDate;

            AchievmentsDatabase.Achievments.Add(achievment);
            AchievmentsDatabase.SaveChanges();

            return Ok(new
            {
                message = "Achievment successfully updated.",
                achievmentData = achievment
            });
        }
        [Route("DeleteAchievment/{id}")]
        [HttpDelete]
        [Authorize(Roles = "admin")]
        public IActionResult DeleteAchievment(int id)
        {
            // проверить наличие ачивки по id
            // удалить ачивку
            var achievment = AchievmentsDatabase.Achievments.Where(a => a.Id == id).FirstOrDefault();
            if (achievment == null)
                return NotFound(new { error = $"Achievment with id '{id}' not found" });

            AchievmentsDatabase.Achievments.Remove(achievment);
            AchievmentsDatabase.SaveChanges();

            return Ok(new { message = $"Ahievment with id '{id}' successfully deleted" });
        }

        #endregion

        #region GetAllPersons / GetAllAchievments / GetAllArticles
        [Route("GetAllPersons")]
        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult GetAllPersons()
            => Ok(new { persons = PersonsDatabase.Persons });
        [Route("GetAllAchievments")]
        [HttpPost]
        [Authorize(Roles = "admin")]

        public IActionResult GetAllAchievments()
            => Ok(new { achievments = AchievmentsDatabase.Achievments });
        [Route("GetAllArticles")]
        [HttpPost]
        [Authorize(Roles = "admin")]

        public IActionResult GetAllArticles()
            => Ok(new { articles = ArticlesDatabase.Articles });
        #endregion
    }
}
