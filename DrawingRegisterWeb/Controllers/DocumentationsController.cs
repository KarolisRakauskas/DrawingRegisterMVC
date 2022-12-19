using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DrawingRegisterWeb.Controllers
{
    public class DocumentationsController : Controller
    {
        private readonly DrawingRegisterContext _context;

        public DocumentationsController(DrawingRegisterContext context)
        {
            _context = context;
        }

        // GET: DocumentationsController
        public async Task<IActionResult> Index(string search, string projects)
        {
            IQueryable<string> ProjectsQuery = from p in _context.Project orderby p.ProjectNubmer select p.ProjectNubmer + " " + p.Name;

            var documentation = from d in _context.Documentation.Include(p => p.Project) select d;

            if (search != null)
            {
                documentation = documentation.Where(d =>
                d.FileName.Contains(search) ||
                d.FileType.Contains(search) ||
                d.Project.Name.Contains(search));
            }

            if (projects != null)
            {
                documentation = documentation.Where(d => d.Project!.ProjectNubmer + " " + d.Project!.Name == projects);
            }

            var documentationVM = new DocumentationVM
            {
                ProjectSelectList = new SelectList(await ProjectsQuery.Distinct().ToListAsync()),
                Documentations = await documentation.ToListAsync()
            };

            return View(documentationVM);
        }

        // GET: DocumentationsController/Details
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DocumentationsController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DocumentationsController/Create
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

        // GET: DocumentationsController/Edit
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DocumentationsController/Edit
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

        // GET: DocumentationsController/Delete
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DocumentationsController/Delete
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
