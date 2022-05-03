using Begenjov_B.Models;
using Begenjov_B.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace Begenjov_B.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            var repository = new UserRepository();

            return View(repository.GetAllUsers());
        }

        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User userModel)
        {
            try
            {
                if (userModel.IsValid())
                {
                    var repo = new UserRepository();
                    string hash;

                    using (var md5Hash = MD5.Create())
                    {
                        var sourceBytes = Encoding.UTF8.GetBytes(userModel.PasswordHash);
                        var hashBytes = md5Hash.ComputeHash(sourceBytes);
                        hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                    }

                    userModel.PasswordHash = hash;

                    repo.Add(userModel);

                    return View();
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

        public ActionResult Edit(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
