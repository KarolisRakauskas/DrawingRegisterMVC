using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.Models;
using DrawingRegisterWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DrawingRegisterWeb
{
	// DrawingRegisters - Creates a unique identifier

	// DrawingRegisterUsers - Creates a relationship with DrawingRegisters and AspNetUsers

	// Invitations - Enable AspNetUsers to join DrawingRegisterUsers by mutual agreement between two or more users

	// ProjectState - Defines states for projects, creates a relationship with projects and DrawingRegisters,
	//				  restricts access to DrawingRegisterUsers that share the same DrawingRegister as the current user

	// Project - Holds main data about user project. Seperates and group drawings, documentation.
	//			 Creates the relationship for drawings, documentation and layouts to ProjectStates

	// Drawings - One of the building blocks of the project.
	//			  Holds main data about drawing files that belong to area/equipment/construction and so on.
	//			  Creates the relationship between DrawingFiles and project
	[Authorize]
	public class DrawingRegistersController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public DrawingRegistersController(
			ApplicationDbContext context,
			UserManager<IdentityUser> userManager,
			SignInManager<IdentityUser> signInManager)
		{
			_context = context;
			_userManager = userManager;
			_signInManager = signInManager;
		}




		public async Task<IActionResult> Index()
		{
			// RegisterVM gathers current user's DrawingRegisterUsers (if user has DrawingRegister) and Invitations
			var registerVM = new RegisterVM();
			var user = await _userManager.GetUserAsync(User);
			var userRegister = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(u => u.UserId == user.Id);

			await _signInManager.RefreshSignInAsync(user);

			if (userRegister == null)
			{
				registerVM.Invitations = await _context.Invitations
					.Include(s => s.Status)
					.Include(u => u.IdentityUser)
					.Where(i => i.UserId == user.Id || i.RecipientEmail == user.Email).ToListAsync();

				return View(registerVM);
			}

			registerVM.DrawingRegister = await _context.DrawingRegisters.FirstOrDefaultAsync(d => d.Id == userRegister.DrawingRegisterId);
			registerVM.DrawingRegisterUsers = await _context.DrawingRegisterUsers
				.Include(u => u.IdentityUser)
				.Where(d => d.DrawingRegisterId == userRegister.DrawingRegisterId).ToListAsync();
			registerVM.Invitations = await _context.Invitations
				.Include(s => s.Status)
				.Include(u => u.IdentityUser)
				.Where(i => i.DrawingRegisterId == userRegister.DrawingRegisterId).ToListAsync();

			// DashBoard SelectList
			IQueryable<string> ProjectsQuery;

			// If user is not administrator - eleminate drawings with ProjectState defined
			if (userRegister!.Role != ConstData.Role_Admin_Name)
			{
				ProjectsQuery = from p in _context.Project.Include(s => s.ProjectState)
								where p.ProjectState.DrawingRegisterId == userRegister!.DrawingRegisterId &&
								p.ProjectState.Name != ConstData.State_Defined
								orderby p.ProjectNubmer
								select p.ProjectNubmer;
			}
			else
			{
				ProjectsQuery = from p in _context.Project.Include(s => s.ProjectState)
								where p.ProjectState.DrawingRegisterId == userRegister!.DrawingRegisterId
								orderby p.ProjectNubmer
								select p.ProjectNubmer;
			}

			registerVM.ProjectSelectList = new SelectList(await ProjectsQuery.Distinct().ToListAsync());

			return View(registerVM);
		}




		// Create new DrawingRegister and RegisterUser for current user
		[Authorize(Roles = ConstData.Role_Norml_Name)]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ConstData.Role_Norml_Name)]
		public async Task<IActionResult> Create(DrawingRegister drawingRegister, bool seedData = false)
		{
			var user = await _userManager.GetUserAsync(User);

			// Check if user already has Register
			var existingDrawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(d => d.IdentityUser == user);
			if(existingDrawingRegisterUser != null)
			{
				TempData["CantCreate"] = "You already have Drawing Register";
				return View();
			}

			if (ModelState.IsValid && user != null)
			{
				_context.Add(drawingRegister);
				_context.SaveChanges();

				var drawingRegisterUser = new DrawingRegisterUsers
				{
					DrawingRegisterId = drawingRegister.Id,
					UserId = user.Id,
					Role = ConstData.Role_Admin_Name
				};

				// Seed required data for new Drawingregister - default states
				var states = SeedDataRuntime.CreateProjectStates(drawingRegister.Id);

				await _context.ProjectState.AddRangeAsync(states);
				await _context.SaveChangesAsync();

				// Seed example data for new Drawingregister: projects, drawings, layouts, documentation
				if (seedData)
				{
					var projects = SeedDataRuntime.CreateProjects(states[0], states[1]);

					await _context.Project.AddRangeAsync(projects);
					await _context.SaveChangesAsync();

					var drawings = SeedDataRuntime.CreateDrawings(projects[0]);

					await _context.Drawing.AddRangeAsync(drawings);
					await _context.SaveChangesAsync();

					var drawingFiles = SeedDataRuntime.CreateDrawingFiles(drawings[0], drawings[1], drawings[2]);
					var layouts = SeedDataRuntime.CreateLayouts(projects[0]);
					var documentation = SeedDataRuntime.CreateDocumentation(projects[0]);

					await _context.DrawingFile.AddRangeAsync(drawingFiles);
					await _context.Layout.AddRangeAsync(layouts);
					await _context.Documentation.AddRangeAsync(documentation);
					await _context.SaveChangesAsync();
				}

				await _context.AddAsync(drawingRegisterUser);
				await _userManager.AddToRoleAsync(user, ConstData.Role_Admin_Name);
				await _context.SaveChangesAsync();
				await _signInManager.RefreshSignInAsync(user);

				return RedirectToAction(nameof(Index));
			}

			return View(drawingRegister);
		}




		// Delete DrawingRegister and all DrawingRegisterUsers of current user's DrawingRegister
		[Authorize(Roles = ConstData.Role_Norml_Name)]
		public async Task<IActionResult> Delete(int? id)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(u => u.UserId == user.Id);

			if (id == null || _context.DrawingRegisters == null)
			{
				return NotFound();
			}

			var drawingRegister = await _context.DrawingRegisters
				.Where(d => d.Id == drawingRegisterUser!.DrawingRegisterId)
				.FirstOrDefaultAsync(m => m.Id == id);

			if (drawingRegister == null)
			{
				return NotFound();
			}

			return View(drawingRegister);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ConstData.Role_Admin_Name)]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (_context.DrawingRegisters == null)
			{
				return Problem("Entity set 'DrawingRegisterContext.DrawingRegisters'  is null.");
			}

			var user = await _userManager.GetUserAsync(User);
			var drawingRegister = await _context.DrawingRegisters.FindAsync(id);
			var drawingRegisterUsers = from d in _context.DrawingRegisterUsers where d.DrawingRegisterId == id select d;

			// Check if drawingRegister exsist
			if (drawingRegister == null || drawingRegisterUsers == null)
			{
				return NotFound();
			}

			_context.DrawingRegisters.Remove(drawingRegister);

			// Remove all roles for all users in this current Drawingregister
			var registerUsersList = await drawingRegisterUsers.ToListAsync();
			var allUsers = from u in _userManager.Users select u;

			foreach (var item in registerUsersList)
			{
				var itemUser = await allUsers.FirstOrDefaultAsync(u => u.Id == item.UserId);
				await _userManager.RemoveFromRoleAsync(itemUser!, item.Role);
			}

			await _context.SaveChangesAsync();
			await _signInManager.RefreshSignInAsync(user);

			return RedirectToAction(nameof(Index));
		}




		// Change role of choosen user in current DrawingRegister
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ConstData.Role_Admin_Name)]
		public async Task<IActionResult> ChangeRole(int id, string role)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FindAsync(id);
			var thisUser = await _userManager.FindByIdAsync(drawingRegisterUser!.UserId);
			var adminRegisterUsers = await _context.DrawingRegisterUsers
				.Where(d => d.DrawingRegisterId == drawingRegisterUser.DrawingRegisterId &&
							d.Role == ConstData.Role_Admin_Name).ToListAsync();

			// Make sure not to leave DrawingRegister without Administrator
			if (drawingRegisterUser.Role == ConstData.Role_Admin_Name && adminRegisterUsers.Count == 1)
			{
				TempData["AdminChangeRole"] = "If you are the only one administrator, you can't change your role. " +
					"Please assign someone as administrator.";

				return RedirectToAction(nameof(Index));
			}

			if (drawingRegisterUser != null && role != null && thisUser != null)
			{
				await _userManager.RemoveFromRoleAsync(thisUser!, drawingRegisterUser.Role);
				await _userManager.AddToRoleAsync(thisUser!, role);

				drawingRegisterUser!.Role = role;

				_context.Update(drawingRegisterUser);
				await _context.SaveChangesAsync();
			}

			if (user == thisUser)
			{
				await _signInManager.RefreshSignInAsync(user);
			}

			return RedirectToAction(nameof(Index));
		}




		// Remove choosen user from DrawingRegister
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ConstData.Role_Admin_Name)]
		public async Task<IActionResult> RemoveMember(int id)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FindAsync(id);
			var thisUser = await _userManager.FindByIdAsync(drawingRegisterUser!.UserId);

			// Check for current user not to remove it self
			if(user == thisUser)
			{
				return RedirectToAction(nameof(Index));
			}

			if (drawingRegisterUser != null && thisUser != null)
			{
				await _userManager.RemoveFromRoleAsync(thisUser!, drawingRegisterUser.Role);
				_context.DrawingRegisterUsers.Remove(drawingRegisterUser);
				await _context.SaveChangesAsync();
			}

			return RedirectToAction(nameof(Index));
		}




		// Leave DrawingRegister for current user
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = $"{ConstData.Role_Admin_Name},{ConstData.Role_Mech_Name},{ConstData.Role_Engr_Name}")]
		public async Task<IActionResult> Leave()
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(u => u.UserId == user.Id);

			if (drawingRegisterUser != null)
			{
				var adminRegisterUsers = await _context.DrawingRegisterUsers
				.Where(d => d.DrawingRegisterId == drawingRegisterUser.DrawingRegisterId &&
							d.Role == ConstData.Role_Admin_Name).ToListAsync();

				// Make sure not to leave DrawingRegister without Administrator
				if (drawingRegisterUser.Role == ConstData.Role_Admin_Name && adminRegisterUsers.Count == 1)
				{
					TempData["AdminLeave"] = "If you are the only one administrator, you can't leave the register. " +
						"Please assign someone as administrator.";

					return RedirectToAction(nameof(Index));
				}

				await _userManager.RemoveFromRoleAsync(user, drawingRegisterUser.Role);
				_context.DrawingRegisterUsers.Remove(drawingRegisterUser);
				await _context.SaveChangesAsync();
				await _signInManager.RefreshSignInAsync(user);
			}

			return RedirectToAction(nameof(Index));
		}




		// Create request to join existing DrawingRegister for user that has no DrawingRegister
		[Authorize(Roles = ConstData.Role_Norml_Name)]
		public IActionResult RequestInvitation()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ConstData.Role_Norml_Name)]
		public async Task<IActionResult> RequestInvitation(Invitation invitation)
		{
			var user = await _userManager.GetUserAsync(User);
			var registerUser = await _userManager.Users.
				FirstOrDefaultAsync(u => u.NormalizedEmail == invitation.RecipientEmail.ToUpper());
			var status = await _context.Statuses.FirstOrDefaultAsync(s => s.Name == ConstData.Status_Request);

			// Check if recipient user exist and if recipient user has DrawingRegister
			if (registerUser != null)
			{
				var drawingRegisterUsers = await _context.DrawingRegisterUsers.
					FirstOrDefaultAsync(d => d.UserId == registerUser.Id);

				if (drawingRegisterUsers == null)
				{
					ModelState.AddModelError("NoRegister",
						"This user has no drawing register. Write the email of the user who already owns register.");
				}
				else
				{
					invitation.StatusId = status!.Id;
					invitation.DrawingRegisterId = drawingRegisterUsers.DrawingRegisterId;
				}
			}
			else
			{
				ModelState.AddModelError("NoUser",
					"There is no such user. Please verify that the user's e-mail is correct.");
			}

			// Restrict from sending invitations to built it app accounts
			if(registerUser!.NormalizedEmail == "Spectator@mail.com".ToUpper() ||
				registerUser!.NormalizedEmail == "Admin@mail.com".ToUpper() ||
				registerUser!.NormalizedEmail == "Engineer@mail.com".ToUpper() ||
				registerUser!.NormalizedEmail == "Mechanic@mail.com".ToUpper())
			{
				ModelState.AddModelError("SpectatorRegister",
						"This user is in spectator drawing register. You can't join spectator's register");
			}

			if (ModelState.IsValid)
			{
				_context.Add(invitation);
				_context.SaveChanges();

				return RedirectToAction(nameof(Index));
			}

			return View(invitation);
		}




		// Remove request to join existing DrawingRegister for user that has no DrawingRegister
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ConstData.Role_Norml_Name)]
		public async Task<IActionResult> RemoveRequestInvitation(int id)
		{
			var invitation = await _context.Invitations.FindAsync(id);

			if (invitation != null)
			{
				_context.Invitations.Remove(invitation);
			}

			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}




		// Remove request or invitation which was assign to current user's DrawingRegister
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ConstData.Role_Admin_Name)]
		public async Task<IActionResult> RemoveInvitation(int id)
		{
			var user = await _userManager.GetUserAsync(User);
			var invitation = await _context.Invitations.FindAsync(id);

			if (invitation != null)
			{
				_context.Invitations.Remove(invitation);
			}

			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}




		// Accept request which was assign to current user's DrawingRegister
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ConstData.Role_Admin_Name)]
		public async Task<IActionResult> AcceptRequest(int id)
		{
			var user = await _userManager.GetUserAsync(User);
			var invitation = await _context.Invitations.FindAsync(id);
			var invitationUser = await _context.Users.FindAsync(invitation!.UserId);
			var invitationUserDrawingRegister = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(d => d.UserId == invitation.UserId);

			//Check if request exsist, user exsist and user has no DrawingRegister already
			if (invitation != null && invitationUser != null && invitationUserDrawingRegister == null)
			{
				var drawingRegisterUser = new DrawingRegisterUsers
				{
					DrawingRegisterId = invitation.DrawingRegisterId,
					UserId = invitation.UserId,
					Role = invitation.Role
				};

				_context.Invitations.Remove(invitation);
				_context.DrawingRegisterUsers.Add(drawingRegisterUser);
				await _userManager.AddToRoleAsync(invitationUser, invitation.Role);
			}
			else
			{
				TempData["BadInvitation"] = "Something went wrong... This user or Drawing Register may no longer exist. " +
					"User can have only one Drawing Register.";
			}

			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}




		// Accept invitation which was assign to current user, who has no DrawingRegister
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ConstData.Role_Norml_Name)]
		public async Task<IActionResult> AcceptInvitation(int id)
		{
			var user = await _userManager.GetUserAsync(User);
			var invitation = await _context.Invitations.FindAsync(id);
			var currentUsersdrawingRegister = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(d => d.UserId == user.Id);

			//Check if invitation exsist, user exsist and current user has no DrawingRegister already
			if (invitation != null && user != null && currentUsersdrawingRegister == null)
			{
				var drawingRegisterUser = new DrawingRegisterUsers
				{
					DrawingRegisterId = invitation.DrawingRegisterId,
					UserId = user.Id,
					Role = invitation.Role
				};

				_context.Invitations.Remove(invitation);
				_context.DrawingRegisterUsers.Add(drawingRegisterUser);
				await _userManager.AddToRoleAsync(user, invitation.Role);
			}
			else
			{
				TempData["BadInvitation"] = "Something went wrong... This user or Drawing Register may no longer exist. " +
					"User can have only one Drawing Register.";
			}

			await _context.SaveChangesAsync();
			await _signInManager.RefreshSignInAsync(user!);

			return RedirectToAction(nameof(Index));
		}




		// Create invitation to join current user's DrawingRegister for user that has no DrawingRegister
		[Authorize(Roles = ConstData.Role_Admin_Name)]
		public IActionResult Invitation()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ConstData.Role_Admin_Name)]
		public async Task<IActionResult> Invitation(Invitation invitation)
		{
			var user = await _userManager.GetUserAsync(User);
			var registerUser = await _userManager.Users.
				FirstOrDefaultAsync(u => u.NormalizedEmail == invitation.RecipientEmail.ToUpper());
			var status = await _context.Statuses.FirstOrDefaultAsync(s => s.Name == ConstData.Status_Invitation);

			// Check if reuquest user exist and if reuquest user has no DrawingRegister
			if (registerUser != null)
			{
				var hasDrawingRegisterUsers = await _context.DrawingRegisterUsers.
					FirstOrDefaultAsync(d => d.UserId == registerUser.Id);

				if (hasDrawingRegisterUsers != null)
				{
					ModelState.AddModelError("UserRegister",
						"This user already has drawing register.");
				}
				else
				{
					var thisDrawingRegisterUsers = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(d => d.UserId == user.Id);
					invitation.StatusId = status!.Id;
					invitation.DrawingRegisterId = thisDrawingRegisterUsers!.DrawingRegisterId;
				}
			}
			else
			{
				ModelState.AddModelError("NoUser",
					"There is no such user. Please verify that the user's e-mail is correct.");
			}

			if (ModelState.IsValid)
			{
				_context.Add(invitation);
				_context.SaveChanges();

				return RedirectToAction(nameof(Index));
			}

			return View(invitation);
		}




		// Get Data For DashBoard
		#region API CALL
		[HttpGet]
		public async Task<IActionResult> GetData()
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(dr => dr.UserId == user.Id);
			var projects = await _context.Project
				.Include(s => s.ProjectState)
				.Where(p => p.ProjectState.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.ToListAsync();
			var drawings = await _context.Drawing
				.Include(p => p.Project)
				.Include(s => s.Project.ProjectState)
				.Where(d => d.Project.ProjectState.DrawingRegisterId == drawingRegisterUser!.DrawingRegisterId)
				.ToListAsync();

			// Load DashBoard Canvas if User has Register
			bool drawingRegisterExsist = drawingRegisterUser != null;

			// Create Data for Bar Chart
			var barChartList = new List<BarChart>();

			foreach (var project in projects)
			{
				var projectDrawings = drawings.Where(d => d.ProjectId == project.Id);
				var drawingList = new List<string>();
				var drawingFileCountList = new List<int>();

				foreach (var drawing in projectDrawings)
				{
					var drawingFilesCount = _context.DrawingFile.Where(d => d.DrawingId == drawing.Id).Count();
					drawingList.Add(drawing.DrawingNumber);
					drawingFileCountList.Add(drawingFilesCount);
				}


				// Get Project number, Drawing numbers and DrawingFiles count
				barChartList.Add(new BarChart() 
				{ 
					ProjectNumber = project.ProjectNubmer, DrawingNumber = drawingList, DrawingFilesCount = drawingFileCountList 
				});
			}

			// Create Data for Doughnut Chart
			var projectsUpcomingCount = projects.Where(p => p.ProjectState.Name == ConstData.State_Defined).Count();
			var projectsCanceledCount = projects.Where(p => p.ProjectState.Name == ConstData.State_Canceled).Count();
			var projectsCompletedCount = projects.Where(p => p.ProjectState.Name == ConstData.State_Completed).Count();
			var projectsRunningCount = projects.Where(p => p.ProjectState.Name != ConstData.State_Defined &&
											p.ProjectState.Name != ConstData.State_Canceled &&
											p.ProjectState.Name != ConstData.State_Completed).Count();

			var doughnutChartList = new List<DoughnutChart>()
			{
				new DoughnutChart()
				{
					Config = "Upcoming & Running",
					States = new List<string>() { "Upcoming", ConstData.State_Running },
					ProjectsCount = new List<int>() { projectsUpcomingCount, projectsRunningCount },
					color = new List<string>() { "#0dcaf0", "#0d6efd" }
				},
				new DoughnutChart()
				{
					Config = "Running & Completed",
					States = new List<string>() { ConstData.State_Running, ConstData.State_Completed },
					ProjectsCount = new List<int>() { projectsRunningCount, projectsCompletedCount },
					color = new List<string>() { "#0d6efd", "#198754" }
				},
				new DoughnutChart()
				{
					Config = "Canceled & Completed",
					States = new List<string>() { ConstData.State_Canceled, ConstData.State_Completed },
					ProjectsCount = new List<int>() { projectsCanceledCount, projectsCompletedCount },
					color = new List<string>() { "#dc3545", "#198754" }

				},
				new DoughnutChart()
				{
					Config = "All",
					States = new List<string>() { "Upcoming", ConstData.State_Running, 
						ConstData.State_Canceled, ConstData.State_Completed },
					ProjectsCount = new List<int>() { projectsUpcomingCount, projectsRunningCount, 
						projectsCanceledCount, projectsCompletedCount },
					color = new List<string>() { "#0dcaf0", "#0d6efd", "#dc3545", "#198754" }
				}
			};

			return Json(new 
			{ 
				register = drawingRegisterExsist, 
				myBarChartList = barChartList, 
				myDoughnutChartList = doughnutChartList 
			});
		}
		#endregion
	}
}
