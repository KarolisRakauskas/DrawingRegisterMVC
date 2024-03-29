﻿#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace DrawingRegisterWeb.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class ResendEmailConfirmationModel : PageModel
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IEmailSender _emailSender;

		public ResendEmailConfirmationModel(UserManager<IdentityUser> userManager, IEmailSender emailSender)
		{
			_userManager = userManager;
			_emailSender = emailSender;
		}




		[BindProperty]
		public InputModel Input { get; set; }

		public class InputModel
		{
			[Required]
			[EmailAddress]
			public string Email { get; set; }
		}




		public void OnGet()
		{
		}




		public async Task<IActionResult> OnPostAsync()
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var user = await _userManager.FindByEmailAsync(Input.Email);

			if (user == null)
			{
				ModelState.AddModelError(string.Empty, $"<p class='text-success'> Verification email sent. Please check your email.</p>");
				return Page();
			}

			// Restrict from editing Spectator user
			if (Input.Email.ToLower() == "spectator@mail.com")
			{
				ModelState.AddModelError(string.Empty, $"<p class='text-danger'>Ops! This is built in application spectator account</p>");
				return Page();
			}

			var userId = await _userManager.GetUserIdAsync(user);
			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
			var callbackUrl = Url.Page(
				"/Account/ConfirmEmail",
				pageHandler: null,
				values: new { userId = userId, code = code },
				protocol: Request.Scheme);
			await _emailSender.SendEmailAsync(
				Input.Email,
				"Confirm your email",
				$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

			// For Real aplication you might wanna change this text without confirmation link
			ModelState.AddModelError(string.Empty, $"<p class='text-success'>Verification email sent. Please check your email.</p>" +
				$"<p class='text-danger'>Since this program using the generic gmail account as email sender - it's a high probability " +
				$"that sent email will be put in your email's spam.</p>" +
				$"<p class='text-danger'>To test the application and set up fake accounts, I've added the confirmation link here: " +
				$"<a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Confirm your email from here</a>.</p>");
			return Page();
		}
	}
}
