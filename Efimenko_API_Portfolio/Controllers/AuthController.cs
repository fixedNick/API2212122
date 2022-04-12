using Efimenko_API_Portfolio.Context;
using Efimenko_API_Portfolio.Data;
using Efimenko_API_Portfolio.Models;
using Efimenko_API_Portfolio.Models.BodyModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Efimenko_API_Portfolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        PersonsContext PersonsDb;
        public AuthController(PersonsContext personsContext)
        {
            PersonsDb = personsContext;
        }

        // register
        // localhost:<port>/api/auth/signup POST
        [Route("signupAdmin")]
        [HttpPost]
        public IActionResult SignupAdmin([FromBody] RegisterData registerData)
        {
            // Проверим, есть ли у вызывающего кука.
            if (Request.Cookies.TryGetValue("ID", out string? uid_str))
                return BadRequest(new { error = "You already authorized" });


            // Where FirstOrDefault вернет либо пользователя, либо null, если делегат не вернет ни разу TRUE
            var user = PersonsDb.Persons.Where(u => u.Email == registerData.Email).FirstOrDefault();

            //Пользователь найден
            if (user != null)
                return Unauthorized(new { error = "user with this email already exists" });

            // Пользователь не найден, регистрируем
            var newUser = Models.Person.SignUp(registerData, isAdmin: true);
            PersonsDb.Persons.Add(newUser);
            PersonsDb.SaveChanges();
            return Ok(new { message = $"User registered with email {newUser.Email}" });
        }

        [Route("signupUser")]
        [HttpPost]
        public IActionResult SignupUser([FromBody] RegisterData registerData)
        {
            // Проверим, есть ли у вызывающего кука.
            if (Request.Cookies.TryGetValue("ID", out string? uid_str))
                return BadRequest(new { error = "You already authorized" });


            // Where FirstOrDefault вернет либо пользователя, либо null, если делегат не вернет ни разу TRUE
            var user = PersonsDb.Persons.Where(u => u.Email == registerData.Email).FirstOrDefault();

            //Пользователь найден
            if (user != null)
                return Unauthorized(new { error = "user with this email already exists" });

            // Пользователь не найден, регистрируем
            var newUser = Models.Person.SignUp(registerData, isAdmin: false);
            PersonsDb.Persons.Add(newUser);
            PersonsDb.SaveChanges();
            return Ok(new { message = $"User registered with email {newUser.Email}" });
        }

        // авторизация
        [Route("signin")]
        [HttpPost]
        public IActionResult SignIn([FromBody] AuthorizationData authorizationData)
        {
            // Для начала найдем пользователя по его Email
            var user = PersonsDb.Persons.Where(u => u.Email == authorizationData.Email).FirstOrDefault();
            if (user == null)
                return Unauthorized(new { error = $"Wrong login or password" });

            // Пользователь найден, начнем процесс идентификации в методе класса User
            // Метод вернет false, если пароль введен неверный.
            var isPasswordCorrect = Models.Person.SignIn(authorizationData.Password, user.RegistrationDate, user.Password);
            if (!isPasswordCorrect)
                return Unauthorized(new { error = $"Wrong login or password" });


            //пользователь найден, выполняем вход
            string role = user.IsAdmin ? "Admin" : "User";
            string response = $"SignIn succesful.Your role is: {role}";
            var token = GenerateJWT(user);

            Response.Cookies.Append("ID", user.Id.ToString());

            return Ok(new { message = response, _token = token });
        }

        [Route("Logout")]
        [HttpPost]
        [Authorize(Roles = "admin, user")]
        public IActionResult Logout()
        {
            if (Request.Cookies.TryGetValue("ID", out string? uid))
                Response.Cookies.Delete("ID");
            else
                return BadRequest(new { error = "You are not authorized" });
            return Ok(new { message = $"User with id {uid} unathorized succeed! " });
        }


        private string GenerateJWT(Person user)
        {
            var secretKey = AuthOptions.GetSymmetricSecurityKey();
            var creds = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.IsAdmin ? "admin" : "user")
            };

            var token = new JwtSecurityToken(
                    AuthOptions.ISSUER,
                    AuthOptions.AUDIENCE,
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(AuthOptions.LIFETIME),
                    signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
