using Efimenko_API_Portfolio.Data;
using Efimenko_API_Portfolio.Models.BodyModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace Efimenko_API_Portfolio.Models
{
    public class Article
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public string CreationDate { get; set; }
        [NotMapped]
        public int Actuality 
        { 
            get
            {
                var actuality = 100;
                var daysPastFromCreation = Utils.GetDaysCountFromCreationDate(CreationDate);

                // за каждый день вычитаем по 1 актуальности
                // акутальность не может быть ниже 1

                actuality -= daysPastFromCreation;
                if (actuality < 0)
                    actuality = 1;

                return actuality;
            }
        }

        public static Article Create(int personId, ArticleUpdate articleUpdateData)
            => new Article() { Title = articleUpdateData.Title, Content =  articleUpdateData.Content, PersonId = personId};
    }
}
