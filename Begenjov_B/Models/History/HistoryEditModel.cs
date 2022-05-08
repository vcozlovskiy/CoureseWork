using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Begenjov_B.Models.History
{
    public class HistoryEditModel : Controller
    {
        // GET: HistoryEditModel
        public ActionResult Index()
        {
            return View();
        }

        // GET: HistoryEditModel/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HistoryEditModel/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HistoryEditModel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: HistoryEditModel/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HistoryEditModel/Edit/5
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

        // GET: HistoryEditModel/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HistoryEditModel/Delete/5
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
