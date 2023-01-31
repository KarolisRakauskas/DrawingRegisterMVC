using DrawingRegisterWeb.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DrawingRegisterWeb.Models;

public static class SeedData
{
	public static void Initialize(IServiceProvider serviceProvider)
	{
		using var context = new DrawingRegisterContext(serviceProvider.GetRequiredService<DbContextOptions<DrawingRegisterContext>>());

		if (!context.Roles.Any())
		{
			context.AddRange(
				new IdentityRole() { Name = ConstData.Role_Admin_Name, NormalizedName = ConstData.Role_Admin_NormalizedName},
				new IdentityRole() { Name = ConstData.Role_Engr_Name, NormalizedName = ConstData.Role_Engr_NormalizedName },
				new IdentityRole() { Name = ConstData.Role_Mech_Name, NormalizedName = ConstData.Role_Mech_NormalizedName }
				);

			context.SaveChanges();
		}

		if (!context.Statuses.Any())
		{
			context.AddRange(
				new Status() { Name = ConstData.Status_Request},
				new Status() { Name = ConstData.Status_Invitation}
				);

			context.SaveChanges();
		}

	}
}
