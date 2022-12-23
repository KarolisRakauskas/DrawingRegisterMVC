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
					" Team members can only read drawings, documentation and layouts."
				},
				new ProjectState
				{
					Name = "Completed",
					Description = "The project set to Completed."+
					" Team members can only read drawings, documentation and layouts."
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
			string testDescription = "Built for application testing purposes only.";

			context.Project.AddRange(

				//Trivial Notation Example
				new Project
				{
					ProjectNubmer = "0001",
					Name = "Project 1 (Trivial Notation Example)",
					Description = testDescription,
					CreateDate = new DateTime(2023, 1, 1),
					DeadlineDate = new DateTime(2023, 3, 1),
					ProjectStateId = stateRunning!.Id
				},
				new Project
				{
					ProjectNubmer = "0002",
					Name = "Project 2 (Trivial Notation Example)",
					Description = testDescription,
					CreateDate = new DateTime(2023, 3, 1),
					DeadlineDate = new DateTime(2023, 5, 1),
					ProjectStateId = stateRunning!.Id
				},
				new Project
				{
					ProjectNubmer = "0003",
					Name = "Project 3 (Trivial Notation Example)",
					Description = testDescription,
					CreateDate = new DateTime(2023, 3, 1),
					DeadlineDate = new DateTime(2023, 5, 1),
					ProjectStateId = stateCompleted!.Id
				},
				new Project
				{
					ProjectNubmer = "0004",
					Name = "Project 4 (Trivial Notation Example)",
					Description = testDescription,
					CreateDate = new DateTime(2023, 5, 1),
					DeadlineDate = new DateTime(2023, 5, 1),
					ProjectStateId = stateDefined!.Id
				},
				new Project
				{
					ProjectNubmer = "0005",
					Name = "Project 5 (Trivial Notation Example)",
					Description = testDescription,
					CreateDate = new DateTime(2023, 3, 1),
					DeadlineDate = new DateTime(2023, 5, 1),
					ProjectStateId = stateCanceled!.Id
				},

				//Meaningful Notation Example
				new Project
				{
					ProjectNubmer = "DR-001",
					Name = "Project 5 (Meaningful Notation Example)",
					Description = testDescription,
					CreateDate = new DateTime(2023, 1, 1),
					DeadlineDate = new DateTime(2023, 3, 1),
					ProjectStateId = stateRunning!.Id
				},
				new Project
				{
					ProjectNubmer = "DR-002",
					Name = "Project 6 (Meaningful Notation Example)",
					Description = testDescription,
					CreateDate = new DateTime(2023, 3, 1),
					DeadlineDate = new DateTime(2023, 5, 1),
					ProjectStateId = stateRunning!.Id
				},
				new Project
				{
					ProjectNubmer = "DR-003",
					Name = "Project 7 (Meaningful Notation Example)",
					Description = testDescription,
					CreateDate = new DateTime(2023, 3, 1),
					DeadlineDate = new DateTime(2023, 5, 1),
					ProjectStateId = stateCompleted!.Id
				},
				new Project
				{
					ProjectNubmer = "DR-004",
					Name = "Project 8 (Meaningful Notation Example)",
					Description = testDescription,
					CreateDate = new DateTime(2022, 5, 1),
					DeadlineDate = new DateTime(2023, 5, 1),
					ProjectStateId = stateDefined!.Id
				},
				new Project
				{
					ProjectNubmer = "DR-005",
					Name = "Project 9 (Meaningful Notation Example)",
					Description = testDescription,
					CreateDate = new DateTime(2022, 3, 1),
					DeadlineDate = new DateTime(2023, 5, 1),
					ProjectStateId = stateCanceled!.Id
				}
				);

			context.SaveChanges();

			var project = from p in context.Project select p;
			var project0001 = project.FirstOrDefault(p => p.ProjectNubmer == "0001");
			var project0002 = project.FirstOrDefault(p => p.ProjectNubmer == "0002");
			var project0003 = project.FirstOrDefault(p => p.ProjectNubmer == "0003");
			var projectDR001 = project.FirstOrDefault(p => p.ProjectNubmer == "DR-001");
			var projectDR002 = project.FirstOrDefault(p => p.ProjectNubmer == "DR-002");
			var projectDR003 = project.FirstOrDefault(p => p.ProjectNubmer == "DR-003");


			context.AddRange(

				//ProjectNubmer = "0001"
				new Drawing
				{
					DrawingNumber = "1-00000001",
					Name = "Test Drawings 1 (Trivial Notation Example)",
					Description = testDescription,
					CreateDate = project0001!.CreateDate,
					ProjectId = project0001.Id
				},
				new Drawing
				{
					DrawingNumber = "1-00001001",
					Name = "Test Drawings 2 (Trivial Notation Example)",
					Description = testDescription,
					CreateDate = project0001!.CreateDate,
					ProjectId = project0001.Id
				},
				new Drawing
				{
					DrawingNumber = "1-00002001",
					Name = "Test Drawings 3 (Trivial Notation Example)",
					Description = testDescription,
					CreateDate = project0001!.CreateDate,
					ProjectId = project0001.Id
				},
				new Drawing
				{
					DrawingNumber = "1-00003001",
					Name = "Test Drawings 4 (Trivial Notation Example)",
					Description = testDescription,
					CreateDate = project0001!.CreateDate,
					ProjectId = project0001.Id
				},

				//ProjectNubmer = "0002"
				new Drawing
				{
					DrawingNumber = "1-00004001",
					Name = "Test Drawings 1 (Trivial Notation Example)",
					Description = testDescription,
					CreateDate = project0002!.CreateDate,
					ProjectId = project0002.Id
				},
				new Drawing
				{
					DrawingNumber = "1-00005001",
					Name = "Test Drawings 2 (Trivial Notation Example)",
					Description = testDescription,
					CreateDate = project0002!.CreateDate,
					ProjectId = project0002.Id
				},
				new Drawing
				{
					DrawingNumber = "1-00006001",
					Name = "Test Drawings 3 (Trivial Notation Example)",
					Description = testDescription,
					CreateDate = project0002!.CreateDate,
					ProjectId = project0002.Id
				},
				new Drawing
				{
					DrawingNumber = "1-00007001",
					Name = "Test Drawings 4 (Trivial Notation Example)",
					Description = testDescription,
					CreateDate = project0002!.CreateDate,
					ProjectId = project0002.Id
				},

				//ProjectNubmer = "0003"
				new Drawing
				{
					DrawingNumber = "1-00008001",
					Name = "Test Drawings 1 (Trivial Notation Example)",
					Description = testDescription,
					CreateDate = project0003!.CreateDate,
					ProjectId = project0003.Id
				},
				new Drawing
				{
					DrawingNumber = "1-00009001",
					Name = "Test Drawings 2 (Trivial Notation Example)",
					Description = testDescription,
					CreateDate = project0003!.CreateDate,
					ProjectId = project0003.Id
				},
				new Drawing
				{
					DrawingNumber = "1-00010001",
					Name = "Test Drawings 3 (Trivial Notation Example)",
					Description = testDescription,
					CreateDate = project0003!.CreateDate,
					ProjectId = project0003.Id
				},
				new Drawing
				{
					DrawingNumber = "1-00011001",
					Name = "Test Drawings 4 (Trivial Notation Example)",
					Description = testDescription,
					CreateDate = project0003!.CreateDate,
					ProjectId = project0003.Id
				},

				//ProjectNubmer = "DR-001"
				new Drawing
				{
					DrawingNumber = "DR-001 01 01.00.00.000",
					Name = "Test Drawings 1 (Meaningful Notation Example)",
					Description = testDescription,
					CreateDate = projectDR001!.CreateDate,
					ProjectId = projectDR001.Id
				},
				new Drawing
				{
					DrawingNumber = "DR-001 02 01.00.00.000",
					Name = "Test Drawings 2 (Meaningful Notation Example)",
					Description = testDescription,
					CreateDate = projectDR001!.CreateDate,
					ProjectId = projectDR001.Id
				},
				new Drawing
				{
					DrawingNumber = "DR-001 03 01.00.00.000",
					Name = "Test Drawings 3 (Meaningful Notation Example)",
					Description = testDescription,
					CreateDate = projectDR001!.CreateDate,
					ProjectId = projectDR001.Id
				},
				new Drawing
				{
					DrawingNumber = "DR-001 04 01.00.00.000",
					Name = "Test Drawings 4 (Meaningful Notation Example)",
					Description = testDescription,
					CreateDate = projectDR001!.CreateDate,
					ProjectId = projectDR001.Id
				},

				//ProjectNubmer = "DR-002"
				new Drawing
				{
					DrawingNumber = "DR-002 01 01.00.00.000",
					Name = "Test Drawings 1 (Meaningful Notation Example)",
					Description = testDescription,
					CreateDate = projectDR002!.CreateDate,
					ProjectId = projectDR002.Id
				},
				new Drawing
				{
					DrawingNumber = "DR-002 02 01.00.00.000",
					Name = "Test Drawings 2 (Meaningful Notation Example)",
					Description = testDescription,
					CreateDate = projectDR002!.CreateDate,
					ProjectId = projectDR002.Id
				},
				new Drawing
				{
					DrawingNumber = "DR-002 03 01.00.00.000",
					Name = "Test Drawings 3 (Meaningful Notation Example)",
					Description = testDescription,
					CreateDate = projectDR002!.CreateDate,
					ProjectId = projectDR002.Id
				},
				new Drawing
				{
					DrawingNumber = "DR-002 04 01.00.00.000",
					Name = "Test Drawings 4 (Meaningful Notation Example)",
					Description = testDescription,
					CreateDate = projectDR002!.CreateDate,
					ProjectId = projectDR002.Id
				},

				//ProjectNubmer = "DR-003"
				new Drawing
				{
					DrawingNumber = "DR-003 01 01.00.00.000",
					Name = "Test Drawings 1 (Meaningful Notation Example)",
					Description = testDescription,
					CreateDate = projectDR003!.CreateDate,
					ProjectId = projectDR003.Id
				},
				new Drawing
				{
					DrawingNumber = "DR-003 02 01.00.00.000",
					Name = "Test Drawings 2 (Meaningful Notation Example)",
					Description = testDescription,
					CreateDate = projectDR003!.CreateDate,
					ProjectId = projectDR003.Id
				},
				new Drawing
				{
					DrawingNumber = "DR-003 03 01.00.00.000",
					Name = "Test Drawings 3 (Meaningful Notation Example)",
					Description = testDescription,
					CreateDate = projectDR003!.CreateDate,
					ProjectId = projectDR003.Id
				},
				new Drawing
				{
					DrawingNumber = "DR-003 04 01.00.00.000",
					Name = "Test Drawings 4 (Meaningful Notation Example)",
					Description = testDescription,
					CreateDate = projectDR003!.CreateDate,
					ProjectId = projectDR003.Id
				}
				);

			context.SaveChanges();
		}
	}
}

