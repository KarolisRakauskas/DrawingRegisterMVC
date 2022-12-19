using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<IActionResult> Index(string search, string projects)
        {
            IQueryable<string> ProjectsQuery = from p in _context.Project orderby p.ProjectNubmer select p.ProjectNubmer + " " + p.Name;

            var drawing = from d in _context.Drawing.Include(p => p.Project) select d;

            if (search != null)
            {
                drawing = drawing.Where(d =>
                d.DrawingNumber.Contains(search) ||
                d.Name.Contains(search) ||
                d.Project.Name.Contains(search) ||
                d.Description!.Contains(search));
            }

            if (projects != null)
            {
                drawing = drawing.Where(d => d.Project!.ProjectNubmer + " " + d.Project!.Name == projects);
            }

            var drawingVM = new DrawingVM
            {
                ProjectSelectList = new SelectList(await ProjectsQuery.Distinct().ToListAsync()),
                Drawings = await drawing.ToListAsync()
            };

            return View(drawingVM);
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
