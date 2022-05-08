using Begenjov_B.Models.Theory;
using Begenjov_B.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Begenjov_B.Controllers
{
    public class TheoryController : Controller
    {
        public ActionResult Index()
        {
            var repo = new TheoryRepository();
            var list = repo.GetTheories();
            return View("/Views/Theory/List.cshtml", list);
        }

        // GET: History/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: History/Create
        public ActionResult Create()
        {
            return View("/Views/Theory/Add.cshtml");
        }

        // POST: History/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Theory history)
        {
            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Account");
                }

                var repo = new TheoryRepository();
                var userRepo = new UserRepository();

                if (ModelState.IsValid)
                {
                    history.Id = repo.GetMaxId() + 1;
                    history.PublesherDate = DateTime.Now;
                    history.Ovner = userRepo.Get(HttpContext.User.Identity.Name);

                    repo.Add(history);
                }

                return RedirectToAction("Index", "Theory");
            }
            catch
            {
                return View();
            }
        }

        // GET: History/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: History/Edit/5
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

        // GET: History/Delete/5
        public ActionResult Delete(int id)
        {
            var repo = new TheoryRepository();

            return View(repo.Get(id));
        }

        // POST: History/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var repo = new TheoryRepository();

                repo.Delete(repo.Get(id));

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
