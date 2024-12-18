using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NETCore.Encrypt.Extensions;
using System.Security.AccessControl;
using System.Security.Claims;
using TelegramBot.Entities;
using TelegramBot.Models;

namespace TelegramBot.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IConfiguration _configuration;

        public AccountController(DatabaseContext databaseContext , IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _configuration = configuration;
        }
        
        public IActionResult Login() { 
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                string MD5Crypto = _configuration.GetValue<string>("AppSettings:MD5Crypto");
                string cryptoPassword = loginVM.Password + MD5Crypto;
                string hashedPassword = cryptoPassword.MD5();

                UserDB user = _databaseContext.Users.SingleOrDefault(x => x.userEmail == loginVM.Email && x.password == hashedPassword);

                if (user != null)
                {
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    claims.Add(new Claim("name", user.userName));
                    claims.Add(new Claim(ClaimTypes.Role, user.role));
                    claims.Add(new Claim("email", user.userEmail));

                    ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");

                }
                else if (!_databaseContext.Users.Any(x => x.userEmail == loginVM.Email))
                {
                    ModelState.AddModelError("", "Böyle kullanıcı adına ait bir hesap bulunamadı");
                }

                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı ya da parola hatalı!");
                }

                return View();
            }
          
            return View(loginVM);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterVM registerVM)
        {
            if(ModelState.IsValid)
            {
                string MD5Crypto = _configuration.GetValue<string>("AppSettings:MD5Crypto");
                string cryptoPassword = registerVM.password + MD5Crypto;
                string hashedPassword = cryptoPassword.MD5();

                UserDB user = new UserDB
                {
                    userName = registerVM.firstName,
                    userSurname = registerVM.lastName,
                    userEmail = registerVM.email,
                    phone = registerVM.phone,
                    password = hashedPassword,

                };
                _databaseContext.Users.Add(user);
                _databaseContext.SaveChanges();
                return View(nameof(Login));
            }

            return View(registerVM);
        }

        [Authorize]
        public IActionResult Profile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int userIdInt = Convert.ToInt32(userId);

            UserDB user = _databaseContext.Users.Find(userIdInt);

            UserVM userVM = new()
            {
                Id  = userIdInt,
                Name = user.userName,
                Surname = user.userSurname,
                Email = user.userEmail,
                phone = user.phone,
                RegisterDate = user.RegisterDate,
                role = user.role,
            };

            return View(userVM);
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
