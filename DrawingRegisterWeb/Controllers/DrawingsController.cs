using DrawingRegisterWeb.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrawingRegisterWeb.Controllers
{
    public class DrawingsController : Controller
    {
        private readonly DrawingRegisterContext _context;

        public DrawingsController(DrawingRegisterContext context)
        {
            _context = context;
        }

        // GET: DrawingsController
        public async Task<IActionResult> Index()
        {
            var drawingRegisterContext = _context.Drawing.Include(p => p.File);
            return View(await drawingRegisterContext.ToListAsync());
        }

        // GET: DrawingsController/Details
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DrawingsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DrawingsController/Create
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

        // GET: DrawingsController/Edit
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DrawingsController/Edit
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

        // GET: DrawingsController/Delete
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DrawingsController/Delete
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
