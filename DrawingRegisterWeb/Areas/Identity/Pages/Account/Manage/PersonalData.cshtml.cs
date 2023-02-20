using DrawingRegisterWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DrawingRegisterWeb.Areas.Identity.Pages.Account.Manage
{
	[Authorize(Roles = ConstData.Role_Norml_Name)]
	public class PersonalDataModel : PageModel
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly ILogger<PersonalDataModel> _logger;

		public PersonalDataModel(
			UserManager<IdentityUser> userManager,
			ILogger<PersonalDataModel> logger)
		{
			_userManager = userManager;
			_logger = logger;
		}




		public async Task<IActionResult> OnGet()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			return Page();
		}
	}
}
