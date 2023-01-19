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
				new IdentityRole() { Name = "Administrator", NormalizedName = "ADMINISTRATOR"},
				new IdentityRole() { Name = "Engineer", NormalizedName = "ENGINEER" },
				new IdentityRole() { Name = "Mechanic", NormalizedName = "MECHANIC" }
				);
			context.SaveChanges();
		}
	


		//if (context.ProjectState.Any())
		//{
		//	return;
		//}
		//else
		//{
		//context.ProjectState.AddRange(
		//	new ProjectState
		//	{
		//		Name = "Defined",
		//		Description = "The project has been created but the plan has not yet been initiated and team members have no access to the project."
		//	},
		//	new ProjectState
		//	{
		//		Name = "Running",
		//		Description = "The project has been started and is accessible by team members." +
		//		" Team members can create, read, update and delete drawings, documentation and layouts."
		//	},
		//	new ProjectState
		//	{
		//		Name = "Canceled",
		//		Description = "The project has been canceled, but project data is still available to team members." +
		//		" Team members can only read drawings, documentation and layouts."
		//	},
		//	new ProjectState
		//	{
		//		Name = "Completed",
		//		Description = "The project set to Completed." +
		//		" Team members can only read drawings, documentation and layouts."
		//	},
		//	new ProjectState
		//	{
		//		Name = "Customized Project State",
		//		Description = "It is a customized project state. You can create your own customized project statements, to give more details about the progress of the project." +
		//		" Project accessibility will be the same as project state - running."
		//	}
		//	);

		//context.SaveChanges();

		//var states = from s in context.ProjectState select s;
		//var stateDefined = states.FirstOrDefault(s => s.Name == "Defined");
		//var stateRunning = states.FirstOrDefault(s => s.Name == "Running");
		//var stateCanceled = states.FirstOrDefault(s => s.Name == "Canceled");
		//var stateCompleted = states.FirstOrDefault(s => s.Name == "Completed");
		//string testDescription = "Built for application testing purposes only.";

		//context.Project.AddRange(

		//	//Trivial Notation Example
		//	new Project
		//	{
		//		ProjectNubmer = "0001",
		//		Name = "Roller Conveyor System (Example)",
		//		Description = $"{testDescription}(Trivial Notation Example)",
		//		CreateDate = new DateTime(2023, 1, 1),
		//		DeadlineDate = new DateTime(2023, 3, 1),
		//		ProjectStateId = stateRunning!.Id,
		//		ModelUrl = "/Files/SeededData/0001.html"
		//	},
		//	new Project
		//	{
		//		ProjectNubmer = "0002",
		//		Name = "Project for Defined State (Example)",
		//		Description = $"{testDescription}(Trivial Notation Example)",
		//		CreateDate = new DateTime(2023, 3, 1),
		//		DeadlineDate = new DateTime(2023, 5, 1),
		//		ProjectStateId = stateDefined!.Id,
		//		ModelUrl = null
		//	}
		//	/*TODO
		//	new Project
		//	{
		//		ProjectNubmer = "DR-001",
		//		Name = "Roller Conveyor System",
		//		Description = testDescription,
		//		CreateDate = new DateTime(2023, 3, 1),
		//		DeadlineDate = new DateTime(2023, 5, 1),
		//		ProjectStateId = stateRunning!.Id
		//	}
		//	*/
		//	);

		//context.SaveChanges();

		//var project = from p in context.Project select p;
		//var project0001 = project.FirstOrDefault(p => p.ProjectNubmer == "0001");
		///*TODO
		//var projectDR001 = project.FirstOrDefault(p => p.ProjectNubmer == "DR-001");
		//*/


		//context.AddRange(

		//	//ProjectNubmer = "0001"
		//	new Drawing
		//	{
		//		DrawingNumber = "1-00000001",
		//		Name = "Roller Conveyor W1000 L3000",
		//		Description = "Driven Roller Conveyor, Width - 1000 mm, Length - 3000 mm. Rollers pattern every 200 mm.",
		//		CreateDate = project0001!.CreateDate,
		//		ProjectId = project0001.Id
		//	},
		//	new Drawing
		//	{
		//		DrawingNumber = "1-00001001",
		//		Name = "Roller Guide L3000",
		//		Description = "Product support guide for conveyor 1-00000001. Full Length - 3000 mm.",
		//		CreateDate = new DateTime(2023, 2, 1),
		//		ProjectId = project0001.Id
		//	},
		//	new Drawing
		//	{
		//		DrawingNumber = "1-00002001",
		//		Name = "Adjustable leg",
		//		Description = "Adjustable leg for conveyor 1-00000001.",
		//		CreateDate = new DateTime(2023, 2, 10),
		//		ProjectId = project0001.Id
		//	}
		//	);

		//context.SaveChanges();

		//var drawings = from d in context.Drawing select d;
		//var drawing00000001 = drawings.FirstOrDefault(d => d.DrawingNumber == "1-00000001");
		//var drawing00001001 = drawings.FirstOrDefault(d => d.DrawingNumber == "1-00001001");
		//var drawing00002001 = drawings.FirstOrDefault(d => d.DrawingNumber == "1-00002001");

		//context.DrawingFile.AddRange(

		//	//1-00000001
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/1-00000001.pdf",
		//		FileName = "1-00000001",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00000001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000002.dxf",
		//		FileName = "2-00000002",
		//		FileType = "dxf",
		//		Revision = null,
		//		CreateDate = drawing00000001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000002.pdf",
		//		FileName = "2-00000002",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00000001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000002_A.dxf",
		//		FileName = "2-00000002_A",
		//		FileType = "dxf",
		//		Revision = "A",
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000002_A.pdf",
		//		FileName = "2-00000002_A",
		//		FileType = "pdf",
		//		Revision = "A",
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000002_B.dxf",
		//		FileName = "2-00000002_B",
		//		FileType = "dxf",
		//		Revision = "B",
		//		CreateDate = drawing00002001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000002_B.pdf",
		//		FileName = "2-00000002_B",
		//		FileType = "pdf",
		//		Revision = "B",
		//		CreateDate = drawing00002001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000003.dxf",
		//		FileName = "2-00000003",
		//		FileType = "dxf",
		//		Revision = null,
		//		CreateDate = drawing00000001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000003.pdf",
		//		FileName = "2-00000003",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00000001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000004.dxf",
		//		FileName = "2-00000004",
		//		FileType = "dxf",
		//		Revision = null,
		//		CreateDate = drawing00000001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000004.pdf",
		//		FileName = "2-00000004",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00000001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000004_A.dxf",
		//		FileName = "2-00000004_A",
		//		FileType = "dxf",
		//		Revision = "A",
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000004_A.pdf",
		//		FileName = "2-00000004_A",
		//		FileType = "pdf",
		//		Revision = "A",
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000004_B.dxf",
		//		FileName = "2-00000004_B",
		//		FileType = "dxf",
		//		Revision = "B",
		//		CreateDate = drawing00002001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000004_B.pdf",
		//		FileName = "2-00000004_B",
		//		FileType = "pdf",
		//		Revision = "B",
		//		CreateDate = drawing00002001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000005.pdf",
		//		FileName = "2-00000005",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00000001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},

		//	//1-00001001
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/1-00001001.pdf",
		//		FileName = "1-00001001",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00001001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/1-00001001_A.pdf",
		//		FileName = "1-00001001_A",
		//		FileType = "pdf",
		//		Revision = "A",
		//		CreateDate = new DateTime(2023, 2, 3),
		//		DrawingId = drawing00001001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/2-00001002.dxf",
		//		FileName = "2-00001002",
		//		FileType = "dxf",
		//		Revision = null,
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00001001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/2-00001002.pdf",
		//		FileName = "2-00001002",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00001001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/2-00001003.pdf",
		//		FileName = "2-00001003",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00001001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/2-00001003.step",
		//		FileName = "2-00001003",
		//		FileType = "step",
		//		Revision = null,
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00001001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/2-00001004.dxf",
		//		FileName = "2-00001004",
		//		FileType = "dxf",
		//		Revision = null,
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00001001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/2-00001004.pdf",
		//		FileName = "2-00001004",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00001001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/2-00001004_A.dxf",
		//		FileName = "2-00001004_A",
		//		FileType = "dxf",
		//		Revision = "A",
		//		CreateDate = new DateTime(2023, 2, 3),
		//		DrawingId = drawing00001001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/2-00001004_A.pdf",
		//		FileName = "2-00001004_A",
		//		FileType = "pdf",
		//		Revision = "A",
		//		CreateDate = new DateTime(2023, 2, 3),
		//		DrawingId = drawing00001001!.Id
		//	},

		//	//1-00002001
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00002001/1-00002001.pdf",
		//		FileName = "1-00002001",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00002001!.CreateDate,
		//		DrawingId = drawing00002001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00002001/2-00002002.pdf",
		//		FileName = "2-00002002",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00002001!.CreateDate,
		//		DrawingId = drawing00002001!.Id
		//	}
		//	);

		//context.Layout.AddRange(
		//	new Layout
		//	{
		//		FileUrl = "/Files/SeededData/Layout/0001-Layout.pdf",
		//		FileName = "0001-Layout",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00002001!.CreateDate,
		//		ProjectId = project0001.Id
		//	},
		//	new Layout
		//	{
		//		FileUrl = "/Files/SeededData/Layout/0001-Layout.dwg",
		//		FileName = "0001-Layout",
		//		FileType = "dwg",
		//		Revision = null,
		//		CreateDate = drawing00002001!.CreateDate,
		//		ProjectId = project0001.Id
		//	}
		//	);

		//context.Documentation.AddRange(
		//	new Documentation
		//	{
		//		FileUrl = "/Files/SeededData/Documentation/Roller Conveyor System Passport.pdf",
		//		FileName = "Roller Conveyor System Passport",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00002001!.CreateDate,
		//		ProjectId = project0001.Id
		//	},
		//	new Documentation
		//	{
		//		FileUrl = "/Files/SeededData/Documentation/Conveyor System Task.pdf",
		//		FileName = "Conveyor System Task",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = project0001!.CreateDate,
		//		ProjectId = project0001.Id
		//	}
		//	);

		//context.SaveChanges();
		//}
	}
}
