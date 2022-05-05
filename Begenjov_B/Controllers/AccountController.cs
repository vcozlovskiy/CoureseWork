using Begenjov_B.Models;
using Begenjov_B.Models.Account;
using Begenjov_B.Repository;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Begenjov_B.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            var repository = new UserRepository();

            return View(repository.GetAllUsers());
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel userModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userRepository = new UserRepository();

                    userRepository.Add(new User()
                    {
                        Id = Guid.NewGuid(),
                        FirstName = userModel.FirstName,
                        LastName = userModel.LastName,
                        Email = userModel.Email,
                        PasswordHash = PassToHash(userModel.Password),
                        IsActive = true
                    });


                    await Authenticate(userModel.Email);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        public async Task<ActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var repository = new UserRepository();
                User user = repository.Get(model.Email);

                if (user != null)
                {
                    await Authenticate(model.Email); // аутентификация

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        private string PassToHash(string password)
        {
            using var md5Hash = MD5.Create();

            var sourceBytes = Encoding.UTF8.GetBytes(password);
            var hashBytes = md5Hash.ComputeHash(sourceBytes);

            return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
        }

        private async Task Authenticate(string userName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };

            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

    }
}
