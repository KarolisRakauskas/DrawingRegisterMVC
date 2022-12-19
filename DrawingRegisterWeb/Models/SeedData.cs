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
		else
		{
			context.ProjectState.AddRange(
				new ProjectState
				{
					Name = "Defined",
					Description = "The project has been created but the plan has not yet been initiated and team members have no access to the project."
				},
				new ProjectState
				{
					Name = "Running",
					Description = "The project has been started and is accessible by team members."+
					" Team members can create, read, update and delete drawings, documentation and layouts."
				},
				new ProjectState
				{
					Name = "Canceled",
					Description = "The project has been canceled, but project data is still available to team members."+
					" Team members can read drawings, documentation and layouts."
				},
				new ProjectState
				{
					Name = "Completed",
					Description = "The project set to Completed."+
					" Team members can read drawings, documentation and layouts."
				},
				new ProjectState
				{
					Name = "Customized Project State",
					Description = "It is a customized project state. You can create your own customized project statements, to give more details about the progress of the project."+
					" Project accessibility will be the same as project state - running."
				}
				);

			context.SaveChanges();

			var states = from s in context.ProjectState select s;
			var stateDefined = states.FirstOrDefault(s => s.Name == "Defined");
			var stateRunning = states.FirstOrDefault(s => s.Name == "Running");
			var stateCanceled = states.FirstOrDefault(s => s.Name == "Canceled");
			var stateCompleted = states.FirstOrDefault(s => s.Name == "Completed");

			context.Project.AddRange(

				new Project
				{
					ProjectNubmer = "0001",
					Name = "Project 1 (Example)",
					Description = "Built for application testing purposes only.",
					CreateDate = new DateTime(2022, 11, 28),
					DeadlineDate = new DateTime(2023, 1, 28),
					ProjectStateId = stateDefined!.Id
				},
				new Project
				{
					ProjectNubmer = "0002",
					Name = "Project 2 (Example)",
					Description = "Built for application testing purposes only.",
					CreateDate = new DateTime(2022, 12, 26),
					DeadlineDate = new DateTime(2023, 2, 26),
					ProjectStateId = stateRunning!.Id
				},
				new Project
				{
					ProjectNubmer = "0003",
					Name = "Project 3 (Example)",
					Description = "Built for application testing purposes only.",
					CreateDate = new DateTime(2022, 12, 26),
					DeadlineDate = new DateTime(2023, 2, 26),
					ProjectStateId = stateCanceled!.Id
				},
				new Project
				{
					ProjectNubmer = "0004",
					Name = "Project 4 (Example)",
					Description = "Built for application testing purposes only.",
					CreateDate = new DateTime(2022, 12, 12),
					DeadlineDate = new DateTime(2023, 2, 12),
					ProjectStateId = stateCompleted!.Id
				}
				);

			context.SaveChanges();
		}
	}
}

