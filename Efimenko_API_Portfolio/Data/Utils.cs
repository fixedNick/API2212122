using System.Text;

namespace Efimenko_API_Portfolio.Data
{
    public class Utils
    {
        public static string GetCurrentDateAsString()
            => $"{DateTime.Now.Day}/{DateTime.Now.Month}/{DateTime.Now.Year}";

        // Массив байт в строку 
        // Используется для получения строкового вида хэша из последовательности байтов
        public static string ByteArrayToString(byte[] byteArray)
        {
            int i;
            StringBuilder sbResult = new StringBuilder(byteArray.Length);
            for (i=0; i < byteArray.Length; i++)
                sbResult.Append(byteArray[i].ToString("X2"));
            return sbResult.ToString();
        }

        public static int GetDaysCountFromCreationDate(string date)
            => 
            (int) DateTime.Now.Subtract(
            new DateTime(Convert.ToInt32(date.Split('/')[2]), Convert.ToInt32(date.Split('/')[1]), Convert.ToInt32(date.Split('/')[0]))
            ).TotalDays ;
    }
}
