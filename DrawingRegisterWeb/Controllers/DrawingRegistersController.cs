using DrawingRegisterWeb.Data;
using DrawingRegisterWeb.Models;
using DrawingRegisterWeb.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrawingRegisterWeb
{
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
			var registerVM = new RegisterVM();
			var user = await _userManager.GetUserAsync(User);
			var userRegister = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(u => u.UserId == user.Id);

			if (userRegister == null)
			{
				registerVM.Invitations = await _context.Invitations.Include(s => s.Status).Where(i => i.UserId == user.Id).ToListAsync();
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




		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(DrawingRegister drawingRegister)
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

				//Seed required data for new drawing register - default states
				var states = SeedDataRuntime.CreateProjectStates(drawingRegister.Id);

				await _context.ProjectState.AddRangeAsync(states);
				await _context.SaveChangesAsync();

				//Seed example data for new drawing register: projects, drawings, layouts
				var projects = SeedDataRuntime.CreateProjects(states[0], states[1], drawingRegister.Id);

				await _context.Project.AddRangeAsync(projects);
				await _context.SaveChangesAsync();

				await _context.AddAsync(drawingRegisterUser);
				await _userManager.AddToRoleAsync(user!, ConstData.Role_Admin_Name);
				await _context.SaveChangesAsync();
				await _signInManager.RefreshSignInAsync(user!);

				return RedirectToAction(nameof(Index));
			}

			return View(drawingRegister);
		}




		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null || _context.DrawingRegisters == null)
			{
				return NotFound();
			}

			var drawingRegister = await _context.DrawingRegisters.FirstOrDefaultAsync(m => m.Id == id);

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

			if (drawingRegister != null)
			{
				_context.DrawingRegisters.Remove(drawingRegister);
			}

			//Remove all roles for all users in this current drawing register
			var drawingRegisterUsers = from d in _context.DrawingRegisterUsers where d.DrawingRegisterId == id select d;
			var registerUsersList = await drawingRegisterUsers.ToListAsync();
			var allUsers = from u in _userManager.Users select u;

			foreach (var item in registerUsersList)
			{
				var itemUser = await allUsers.FirstOrDefaultAsync(u => u.Id == item.UserId);
				await _userManager.RemoveFromRoleAsync(itemUser!, item.Role);
			}

			await _context.SaveChangesAsync();
			await _signInManager.RefreshSignInAsync(user!);

			return RedirectToAction(nameof(Index));
		}




		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ConstData.Role_Admin_Name)]
		public async Task<IActionResult> ChangeRole(int id, string role)
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FindAsync(id);
			var thisUser = await _userManager.FindByIdAsync(drawingRegisterUser!.UserId);

			if (drawingRegisterUser != null && role != null && thisUser != null)
			{
				await _userManager.RemoveFromRoleAsync(thisUser!, drawingRegisterUser.Role);
				await _userManager.AddToRoleAsync(thisUser!, role);

				drawingRegisterUser!.Role = role;

				_context.Update(drawingRegisterUser);
				await _context.SaveChangesAsync();
			}

			if(user == thisUser)
			{
				await _signInManager.RefreshSignInAsync(user);
			}

			return RedirectToAction(nameof(Index));
		}




		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ConstData.Role_Admin_Name)]
		public async Task<IActionResult> RemoveMember(int id)
		{
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FindAsync(id);
			var thisUser = await _userManager.FindByIdAsync(drawingRegisterUser!.UserId);

			if (drawingRegisterUser != null && thisUser != null)
			{
				await _userManager.RemoveFromRoleAsync(thisUser!, drawingRegisterUser.Role);
				_context.DrawingRegisterUsers.Remove(drawingRegisterUser);
				await _context.SaveChangesAsync();
			}

			return RedirectToAction(nameof(Index));
		}




		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Leave()
		{
			var user = await _userManager.GetUserAsync(User);
			var drawingRegisterUser = await _context.DrawingRegisterUsers.FirstOrDefaultAsync(u => u.UserId == user.Id);

			if (drawingRegisterUser != null)
			{
				await _userManager.RemoveFromRoleAsync(user!, drawingRegisterUser.Role);
				_context.DrawingRegisterUsers.Remove(drawingRegisterUser);
				await _context.SaveChangesAsync();
			}

			await _signInManager.RefreshSignInAsync(user!);

			return RedirectToAction(nameof(Index));
		}




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
			var status = await _context.Statuses.FirstOrDefaultAsync(s => s.Name == ConstData.Status_Approval_pending);

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

			var invitationVM = new InvitationVM()
			{
				Invitation = invitation
			};

			return View(invitationVM);
		}




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




		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ConstData.Role_Admin_Name)]
		public async Task<IActionResult> RemoveInvitation(int id)
		{
			var invitation = await _context.Invitations.FindAsync(id);

			if (invitation != null)
			{
				_context.Invitations.Remove(invitation);
			}

			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}




		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ConstData.Role_Admin_Name)]
		public async Task<IActionResult> AcceptRequest(int id)
		{
			var invitation = await _context.Invitations.FindAsync(id);
			var user = await _context.Users.FindAsync(invitation!.UserId);

			if (invitation != null && user != null)
			{
				var drawingRegisterUser = new DrawingRegisterUsers
				{
					DrawingRegisterId = invitation.DrawingRegisterId,
					UserId = invitation.UserId,
					Role = invitation.Role
				};

				_context.Invitations.Remove(invitation);
				_context.DrawingRegisterUsers.Add(drawingRegisterUser);
				await _userManager.AddToRoleAsync(user, invitation.Role);
			}

			await _context.SaveChangesAsync();

			return RedirectToAction(nameof(Index));
		}
	}
}
