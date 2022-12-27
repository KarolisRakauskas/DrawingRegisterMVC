using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DrawingRegisterWeb.Controllers
{
	public class LayoutsController : Controller
	{
		private readonly DrawingRegisterContext _context;

		public LayoutsController(DrawingRegisterContext context)
		{
			_context = context;
		}

		// GET: LayoutsController
		public async Task<IActionResult> Index(string search, string projects)
		{
			IQueryable<string> ProjectsQuery = from p in _context.Project orderby p.ProjectNubmer select p.ProjectNubmer + " " + p.Name;

			var layout = from l in _context.Layout.Include(p => p.Project) select l;

			if (search != null)
			{
				layout = layout.Where(l =>
				l.FileName!.Contains(search) ||
				l.FileType!.Contains(search) ||
				l.Project!.Name.Contains(search));
			}

			if (projects != null)
			{
				layout = layout.Where(d => d.Project!.ProjectNubmer + " " + d.Project!.Name == projects);
			}

			var layoutVM = new LayoutVM
			{
				ProjectSelectList = new SelectList(await ProjectsQuery.Distinct().ToListAsync()),
				Layouts = await layout.ToListAsync()
			};

			return View(layoutVM);
		}

		// GET: LayoutsController/Details
		public ActionResult Details(int id)
		{
			return View();
		}

		// GET: LayoutsController/Create
		public ActionResult Create()
		{
			return View();
		}

		// POST: LayoutsController/Create
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

		// GET: LayoutsController/Edit
		public ActionResult Edit(int id)
		{
			return View();
		}

		// POST: LayoutsController/Edit
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

		// GET: LayoutsController/Delete
		public ActionResult Delete(int id)
		{
			return View();
		}

		// POST: LayoutsController/Delete
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
