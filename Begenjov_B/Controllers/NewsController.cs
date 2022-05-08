using Begenjov_B.Models.News;
using Begenjov_B.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Begenjov_B.Controllers
{
    public class NewsController : Controller
    {
        private ILogger<NewsController> _logger;
        public NewsController(ILogger<NewsController> logger)
        {
            _logger = logger;
        }
        // GET: NewsController
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var newsRepository = new NewsRepository();
            var news = newsRepository.GetNews();

            return View(news);
        }

        // GET: NewsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: NewsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NewsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NewCreatedModel model)
        {
            var userRepo = new UserRepository();
            var newsRepo = new NewsRepository();

            try
            {
                if (!HttpContext.User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Account");
                }

                if (ModelState.IsValid)
                {
                    int id = newsRepo.GetMaxId() + 1;
                    var @new = new New()
                    {
                        Id = newsRepo.GetMaxId() + 1,
                        Text = model.Text,
                        Title = model.Title,
                        PublesherDate = DateTime.Now,
                        Ovner = userRepo.Get(HttpContext.User.Identity.Name)
                    };

                    newsRepo.Add(@new);
                }

                return RedirectToAction("Index", "News");
            }
            catch
            {
                return View();
            }
        }

        // GET: NewsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: NewsController/Edit/5
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

        // GET: NewsController/Delete/5
        [HttpPost]
        public ActionResult Delete(int @new)
        {
            return View();
        }

        // POST: NewsController/Delete/5
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
