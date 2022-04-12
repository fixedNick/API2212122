using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using Efimenko_API_Portfolio.Data;
using System.Text;
using System.Security.Cryptography;
using Efimenko_API_Portfolio.Models.BodyModels;

namespace Efimenko_API_Portfolio.Models
{
    public class Person
    {
        #region Fields
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; } = false;
        public string RegistrationDate { get; set; }
        public string Achievments { get; set; } = "";
        public string Country { get; set; }
        public string ProfilePhoto { get; set; } = DefaultUserPhoto;

        [NotMapped] // не сохранять поле в бд
        public List<int> AchievmentsList { get; set; } = new List<int>();

        [NotMapped]
        public static readonly string DefaultUserPhoto = "default_photo.jfif";
        #endregion

        #region Change Password
        public bool ComparePassword(string enteredOldPass)
        {
            var realPasswordBytes = ASCIIEncoding.ASCII.GetBytes(Password);
            var enteredOldPassBytes = ASCIIEncoding.ASCII.GetBytes(enteredOldPass);

            return CompareHashes(realPasswordBytes, enteredOldPassBytes);
        }

        public void SetupNewPassword(string newPassword)
        {
            var newHashedPassword = GetHashedPassword(newPassword, RegistrationDate);
            Password = newHashedPassword;
        }

        #endregion

        #region Convert From/To Json/List
        //Получаем объект из JSON-строки
        public void UpdateAchivmentsListFromJson()
            => AchievmentsList = JsonConvert.DeserializeObject<List<int>>(Achievments);

        // Создание JSON-строки из объекта
        public void UpdateAchievmentsJsonFromList()
            => Achievments = JsonConvert.SerializeObject(AchievmentsList, Formatting.Indented);

        #endregion

        #region SignIn
        // TRUE - введен верный пароль
        // FALSE - пароли отличаются
        public static bool SignIn(string enteredPassword, string registrationDate, string realHashedPassword)
        {
            var enteredHashedPassword = GetHashedPassword(enteredPassword, registrationDate);
            var enteredHashedPasswordBytes = ASCIIEncoding.ASCII.GetBytes(enteredHashedPassword);
            var realHashedPasswordBytes = ASCIIEncoding.ASCII.GetBytes(realHashedPassword);

            return CompareHashes(enteredHashedPasswordBytes, realHashedPasswordBytes);
        }
        #endregion

        #region SignUp
        public static Person SignUp(RegisterData registerData, bool isAdmin = false)
        {
            var person = new Person();
            var currentDate = Utils.GetCurrentDateAsString();

            person.Email = registerData.Email;
            person.IsAdmin = isAdmin;
            person.RegistrationDate = currentDate;
            person.Country = registerData.Country;

            if (registerData.ProfilePhoto != null)
                person.ProfilePhoto = registerData.ProfilePhoto;

            var hashedPassword = GetHashedPassword(registerData.Password, currentDate);
            person.Password = hashedPassword;

            return person;
        }
        #endregion

        #region Hash Password
        // TRUE - Хэши сходятся
        // FALSE - Хэши разные
        private static bool CompareHashes(byte[] h1, byte[] h2)
        {
            bool isHashesEquals = false;

            if (h1.Length == h2.Length)
            {
                // Проверяем каждый символ хеша на протяжении всей длины одного из них (i < h2.Length)
                int i = 0;
                while ((i < h2.Length) && (h1[i] == h2[i]))
                    ++i;

                if (i == h2.Length)
                    isHashesEquals = true;
            }
            return isHashesEquals;
        }

        private static string GetHashedPassword(string originPassword, string salt)
        {
            // Защитим пароль, но как? 
            // Будем использовать очень простой метод - в поле Password будет храниться:
            // HASH(пароль + некая соль) - мы будем хранить именно хэш
            // Так как мы делаем лишь проект для примера, то с солью заморачиваться не будем -
            // - возьмем дату регистрации аккаунта RegistrationDate
            // Итого получим к примеру: Hash(superpass02/02/2022)
            // Алгоритм связывания между собой соли и пароля уходит на усмотрение разработчика
            // Как и выбор алгоритма хэширования
            // Суть хэширования в том, что мы создадим необратимый набор невнятных символов,
            // с которым будем сравнивать то, что позже нам будет передавать пользователь при авторизации
            // То есть алгоритм примерно таков:
            // Register: Hash(pass + date) -> database store
            // Authorization: 
            // 1. Получаем имя и пароль от пользователя
            // 2. Проверяем в БД наличие пользователя по имени
            // 3. Если пользователь с таким именем найден - берем его пароль из бд ( Тот самый Hash )
            // 4. Так же берем из БД и дату регистрации этого пользователя
            // 5. Соединяем между собой дату регистрации пользователя из бд и переданный при авторизации пароль
            // ... (pass + date)
            // 6. Хэшируем (pass + date )
            // 7. Проверяем получившийся хэш с тем, что лежит в БД
            // 8. Если совпало - успех, авторизуем пользователя
            // 9. А если не совпало - то иди копай всю соль в черном море

            MD5 md5 = MD5.Create();
            var saltedPassword = originPassword + salt; // superpass + 03/03/2022
            byte[] saltedPasswordBytes = ASCIIEncoding.ASCII.GetBytes(saltedPassword); // Просто перевод в массив байт


            byte[] hashedPasswordBytes = md5.ComputeHash(saltedPasswordBytes); // создаем хэш
            var hashedPassword = Utils.ByteArrayToString(hashedPasswordBytes); // переводим массив байт хеша в строку

            return hashedPassword;
        }
        #endregion

        public bool TryAddNewAchievment(Achievment achievment)
        {
            if (this.AchievmentsList.Contains(achievment.Id))
                return false;

            AchievmentsList.Add(achievment.Id);
            return true;
        }

        public void UpdatePersonalData(UpdateProfileData updateProfileData)
        {
            Email = updateProfileData.Email ?? Email;
            Country = updateProfileData.Country ?? Country;
            ProfilePhoto = updateProfileData.ProfilePhoto ?? ProfilePhoto;
        }
    }
}
