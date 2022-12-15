using DrawingRegisterWeb.Data;
using Microsoft.EntityFrameworkCore;

namespace DrawingRegisterWeb.Models;

public static class SeedData
{
	public static void Initialize(IServiceProvider serviceProvider)
	{
		using var context = new DrawingRegisterContext(serviceProvider.GetRequiredService<DbContextOptions<DrawingRegisterContext>>());
		if (context.ProjectState.Any())
		{
			return;
		}

		context.ProjectState.AddRange(
			new ProjectState
			{
				Name = "Defined",
				Description = "The project has been created, but the plan has not started and invitations have not been sent to team members."
			},
			new ProjectState
			{
				Name = "Running",
				Description = "The project has been started and invitations have been sent to team members."
			},
			new ProjectState
			{
				Name = "Suspended",
				Description = "The project has been temporarily removed from the active list."
			},
			new ProjectState
			{
				Name = "Canceled",
				Description = "The project has been permanently removed from the active list, but project data is still available to team members."
			},
			new ProjectState
			{
				Name = "Completed",
				Description = "The project set to Completed."
			},
			new ProjectState
			{
				Name = "Customized Project State",
				Description = "It is a customized project state. You can create your own customized project statements, to give more details about the progress of the project."
			}
			);

		context.SaveChanges();
	}
}

