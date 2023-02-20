using Microsoft.AspNetCore.Identity;
using DrawingRegisterWeb.Data;
using Microsoft.EntityFrameworkCore;
using DrawingRegisterWeb.Models;

namespace DrawingRegisterWeb.DbInitializer
{
	public class DbInitializer : IDbInitializer
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ApplicationDbContext _context;

		public DbInitializer(
			UserManager<IdentityUser> userManager, 
			RoleManager<IdentityRole> roleManager, 
			ApplicationDbContext context)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_context = context;
		}




		public void Initialize()
		{
			// Applay Migrations
			try 
			{
				if (_context.Database.GetPendingMigrations().Any())
				{
					_context.Database.Migrate();
				}
			}
			catch (Exception) {}




			// Seed Status
			if (!_context.Statuses.Any())
			{
				_context.AddRangeAsync(
					new Status() { Name = ConstData.Status_Request },
					new Status() { Name = ConstData.Status_Invitation }
					).GetAwaiter().GetResult();

				_context.SaveChangesAsync().GetAwaiter().GetResult();
			}




			// Create Roles
			if (!_context.Roles.Any())
			{
				_roleManager.CreateAsync(new IdentityRole() { Name = ConstData.Role_Norml_Name }).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole() { Name = ConstData.Role_Admin_Name}).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole() { Name = ConstData.Role_Engr_Name }).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole() { Name = ConstData.Role_Mech_Name }).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole() { Name = ConstData.Role_Spect_Name }).GetAwaiter().GetResult();
			}




			// Create Spectator User and it's interface
			if (_context.Users.FirstOrDefault(u => u.Email == "Spectator@mail.com") == null)
			{
				// Spectator User
				_userManager.CreateAsync(
					new IdentityUser() { Email = "Spectator@mail.com", UserName = "Spectator@mail.com", EmailConfirmed = true },
					"Spectator123.").GetAwaiter().GetResult();

				// Generate Random passwords for interface Users
				string passwordAdmin = CreateRandomPassword();
				string passwordEngr = CreateRandomPassword();
				string passwordMech = CreateRandomPassword();
				string passwordExampl = CreateRandomPassword();

				// Interface Users
				_userManager.CreateAsync(
					new IdentityUser() { Email = "Admin@mail.com", UserName = "Admin@mail.com", EmailConfirmed = true },
					passwordAdmin).GetAwaiter().GetResult();

				_userManager.CreateAsync(
					new IdentityUser() { Email = "Engineer@mail.com", UserName = "Engineer@mail.com", EmailConfirmed = true },
					passwordEngr).GetAwaiter().GetResult();

				_userManager.CreateAsync(
					new IdentityUser() { Email = "Mechanic@mail.com", UserName = "Mechanic@mail.com", EmailConfirmed = true },
					passwordMech).GetAwaiter().GetResult();

				_userManager.CreateAsync(
					new IdentityUser() { Email = "InvitationExample@mail.com", UserName = "InvitationExample@mail.com", EmailConfirmed = true },
					passwordExampl).GetAwaiter().GetResult();

				// Get new Users
				var user = _context.Users.FirstOrDefault(u => u.Email == "Spectator@mail.com");
				var admin = _context.Users.FirstOrDefault(u => u.Email == "Admin@mail.com");
				var engr = _context.Users.FirstOrDefault(u => u.Email == "Engineer@mail.com");
				var mech = _context.Users.FirstOrDefault(u => u.Email == "Mechanic@mail.com");
				var exampl = _context.Users.FirstOrDefault(u => u.Email == "InvitationExample@mail.com");


				// Remove Normal Role and assign spectator role - to prevent from editing Spectator user
				_userManager.RemoveFromRoleAsync(user!, ConstData.Role_Norml_Name).GetAwaiter().GetResult();
				_userManager.AddToRoleAsync(user!, ConstData.Role_Spect_Name).GetAwaiter().GetResult();

				// Give interface users roles
				_userManager.AddToRoleAsync(admin!, ConstData.Role_Admin_Name).GetAwaiter().GetResult();
				_userManager.AddToRoleAsync(engr!, ConstData.Role_Engr_Name).GetAwaiter().GetResult();
				_userManager.AddToRoleAsync(mech!, ConstData.Role_Mech_Name).GetAwaiter().GetResult();

				// Create Drawing Register for Spectator User
				_context.AddAsync( new DrawingRegister() { CreateDate = new DateTime(2023, 1, 1) } ).GetAwaiter().GetResult();
				_context.SaveChangesAsync().GetAwaiter().GetResult();

				// Get Drawing Register
				var drawingRegister = _context.DrawingRegisters.FirstOrDefault(d => d.CreateDate == new DateTime(2023, 1, 1));

				// Assing all DrawingRegisterUsers to all users
				_context.AddRangeAsync(
					new DrawingRegisterUsers() { DrawingRegisterId = drawingRegister!.Id, Role = ConstData.Role_Spect_Name, UserId = user!.Id },
					new DrawingRegisterUsers() { DrawingRegisterId = drawingRegister!.Id, Role = ConstData.Role_Admin_Name, UserId = admin!.Id },
					new DrawingRegisterUsers() { DrawingRegisterId = drawingRegister!.Id, Role = ConstData.Role_Engr_Name, UserId = engr!.Id }, 
					new DrawingRegisterUsers() { DrawingRegisterId = drawingRegister!.Id, Role = ConstData.Role_Mech_Name, UserId = mech!.Id}
					).GetAwaiter().GetResult();

				// Create fake invitations for interface
				var statusRequest = _context.Statuses.FirstOrDefaultAsync(r => r.Name == ConstData.Status_Request).GetAwaiter().GetResult();

				_context.AddRangeAsync(
					new Invitation()
					{
						RecipientEmail = user.Email, 
						Role = ConstData.Role_Engr_Name, 
						StatusId = statusRequest!.Id, 
						DrawingRegisterId = drawingRegister!.Id, 
						UserId = exampl!.Id
					},
					new Invitation()
					{
						RecipientEmail = user.Email, 
						Role = ConstData.Role_Mech_Name, 
						StatusId = statusRequest!.Id, 
						DrawingRegisterId = drawingRegister!.Id, 
						UserId = exampl!.Id
					}
					).GetAwaiter().GetResult();

				_context.SaveChangesAsync().GetAwaiter().GetResult();

				// Create seeded Data: ProjectStates
				var states = SeedDataRuntime.CreateProjectStates(drawingRegister.Id);

				_context.ProjectState.AddRangeAsync(states).GetAwaiter().GetResult();
				_context.SaveChangesAsync().GetAwaiter().GetResult();

				// Create seeded Data: Projects
				var projects = SeedDataRuntime.CreateProjects(states[0], states[1]);

				_context.Project.AddRangeAsync(projects).GetAwaiter().GetResult();
				_context.SaveChangesAsync().GetAwaiter().GetResult();

				// Create seeded Data: Drawings
				var drawings = SeedDataRuntime.CreateDrawings(projects[0]);

				_context.Drawing.AddRangeAsync(drawings).GetAwaiter().GetResult();
				_context.SaveChangesAsync().GetAwaiter().GetResult();

				// Create seeded Data: DrawingFiles; Layouts; Documents
				var drawingFiles = SeedDataRuntime.CreateDrawingFiles(drawings[0], drawings[1], drawings[2]);
				var layouts = SeedDataRuntime.CreateLayouts(projects[0]);
				var documentation = SeedDataRuntime.CreateDocumentation(projects[0]);

				_context.DrawingFile.AddRangeAsync(drawingFiles).GetAwaiter().GetResult();
				_context.Layout.AddRangeAsync(layouts).GetAwaiter().GetResult();
				_context.Documentation.AddRangeAsync(documentation).GetAwaiter().GetResult();
				_context.SaveChangesAsync().GetAwaiter().GetResult();
			}

			return;
		}




		// Generate Random Password with 3 lower case letters 3 upper case letters 3 digits and 3 non-alphanumeric chars
		private static string CreateRandomPassword()
		{
			string lettersLowerChars = "abcdefghijklmnopqrstuvwxyz";
			string lettersUpperChars = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
			string numbersChars = "0123456789";
			string nonAlphanumericChars = "*!?.,$#";
			Random random = new();

			char[] chars = new char[13];
			for (int i = 0; i < 4; i++)
			{
				chars[i] = lettersLowerChars[random.Next(0, lettersLowerChars.Length)];
				chars[i+3] = lettersUpperChars[random.Next(0, lettersUpperChars.Length)];
				chars[i+6] = numbersChars[random.Next(0, numbersChars.Length)];
				chars[i+9] = nonAlphanumericChars[random.Next(0, nonAlphanumericChars.Length)];
			}
			return new string(chars);
		}
	}
}
