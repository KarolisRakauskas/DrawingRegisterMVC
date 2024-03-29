﻿#nullable disable

using DrawingRegisterWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace  DrawingRegisterWeb.Areas.Identity.Pages.Account.Manage
{
	[Authorize(Roles = ConstData.Role_Norml_Name)]
	public static class ManageNavPages
	{
		public static string ChangePassword => "ChangePassword";
		public static string DownloadPersonalData => "DownloadPersonalData";
		public static string DeletePersonalData => "DeletePersonalData";
		public static string PersonalData => "PersonalData";




		public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);
		public static string DownloadPersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, DownloadPersonalData);
		public static string DeletePersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, DeletePersonalData);
		public static string PersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, PersonalData);

		public static string PageNavClass(ViewContext viewContext, string page)
		{
			var activePage = viewContext.ViewData["ActivePage"] as string
				?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
			return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
		}
	}
}
