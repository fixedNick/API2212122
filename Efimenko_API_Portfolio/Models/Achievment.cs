using Efimenko_API_Portfolio.Data;

namespace Efimenko_API_Portfolio.Models
{
    public class Achievment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AwardDate { get; set; }

        public static Achievment Create(string name)
        {
            return new Achievment() { Name = name, AwardDate = Utils.GetCurrentDateAsString() };
        }
    }
}
