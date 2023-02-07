using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.Models;
using DrawingRegisterWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrawingRegisterWeb
{
	// DrawingRegisters - Creates a unique identifier
	// DrawingRegisterUsers - Creates a relationship with DrawingRegisters and AspNetUsers
	// Invitations - Enable AspNetUsers to join DrawingRegisterUsers by mutual agreement between two or more users

	[Authorize]
	public class DrawingRegistersController : Controller
	{
		private readonly DrawingRegisterContext _context;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public DrawingRegistersController(
			DrawingRegisterContext context,
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

			return View(registerVM);
		}




		// Create new DrawingRegister and RegisterUser for current user
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(DrawingRegister drawingRegister, bool seedData = false)
		{
			var user = await _userManager.GetUserAsync(User);

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
		public IActionResult RequestInvitation()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
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

			if (invitation != null && invitationUser != null)
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

			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}




		// Accept invitation which was assign to current user, who has no DrawingRegister
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AcceptInvitation(int id)
		{
			var user = await _userManager.GetUserAsync(User);
			var invitation = await _context.Invitations.FindAsync(id);

			if (invitation != null && user != null)
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
	}
}
