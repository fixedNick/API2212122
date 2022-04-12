using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Efimenko_API_Portfolio.Data
{
    public class AuthOptions
    {
        public const string ISSUER = "authServer"; // издатель токена
        public const string AUDIENCE = "resourceServer"; // потребитель токена
        const string KEY = "superscretkey123456!123";   // ключ для шифрации
        public const int LIFETIME = 10800; // время жизни токена - 7 дней
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
