
namespace DrawingRegisterWeb.Models
{
	//Create lists of elements for seeding data: Project States, Projects, Drawings, Layouts and Documentation
	public class SeedDataRuntime
	{
		//Create the list of standart Project States based of DrawingRegister Id
		public static List<ProjectState> CreateProjectStates(int id)
		{
			var states = new List<ProjectState>
			{
				new ProjectState
				{
					Name = "Defined",
					Description = "The project has been created but the plan has not yet been initiated and team members have no access to the project.",
					DrawingRegisterId = id
				},
				new ProjectState
				{
					Name = "Running",
					Description = "The project has been started and is accessible by team members." +
					" Team members can create, read, update and delete drawings, documentation and layouts.",
					DrawingRegisterId = id
				},
				new ProjectState
				{
					Name = "Canceled",
					Description = "The project has been canceled, but project data is still available to team members." +
					" Team members can only read drawings, documentation and layouts.",
					DrawingRegisterId = id
				},
				new ProjectState
				{
					Name = "Completed",
					Description = "The project set to Completed." +
					" Team members can only read drawings, documentation and layouts.",
					DrawingRegisterId = id
				},
				new ProjectState
				{
					Name = "Customized Project State",
					Description = "It is a customized project state. You can create your own customized project statements, " +
					"to give more details about the progress of the project. Project accessibility will be the same as project state - running.",
					DrawingRegisterId = id
				}
			};
			return states;
		}




		//Create the list of example Projects
		public static List<Project> CreateProjects(ProjectState defineState, ProjectState runningState)
		{
			var projects = new List<Project>
			{
				new Project
				{
					ProjectNubmer = "0001",
					Name = "Roller Conveyor System (Example)",
					Description = "Built for application testing purposes only (Trivial Notation Example)",
					CreateDate = new DateTime(2023, 1, 1),
					DeadlineDate = new DateTime(2023, 3, 1),
					ProjectStateId = runningState.Id
				},
				new Project
				{
					ProjectNubmer = "0002",
					Name = "Project for Defined State (Example)",
					Description = "Built for application testing purposes only (Trivial Notation Example)",
					CreateDate = new DateTime(2023, 3, 1),
					DeadlineDate = new DateTime(2023, 5, 1),
					ProjectStateId = defineState.Id
				}
			};
			return projects;
		}




		//Create the list of example Drawings
		public static List<Drawing> CreateDrawings(Project project0001)
		{
			var drawings = new List<Drawing>
			{
				new Drawing
				{
					DrawingNumber = "1-00000001",
					Name = "Roller Conveyor W1000 L3000",
					Description = "Driven Roller Conveyor, Width - 1000 mm, Length - 3000 mm. Rollers pattern every 200 mm.",
					CreateDate = project0001!.CreateDate,
					ProjectId = project0001.Id
				},
				new Drawing
				{
					DrawingNumber = "1-00001001",
					Name = "Roller Guide L3000",
					Description = "Product support guide for conveyor 1-00000001. Full Length - 3000 mm.",
					CreateDate = new DateTime(2023, 2, 1),
					ProjectId = project0001.Id
				},
				new Drawing
				{
					DrawingNumber = "1-00002001",
					Name = "Adjustable leg",
					Description = "Adjustable leg for conveyor 1-00000001.",
					CreateDate = new DateTime(2023, 2, 10),
					ProjectId = project0001.Id
				}
			};
			return drawings;
		}




		//Create the list of example DrawingFile
		public static List<DrawingFile> CreateDrawingFiles(Drawing drawing00000001, Drawing drawing00001001, Drawing drawing00002001)
		{
			var drawingFiles = new List<DrawingFile>
			{
				//For Drawing 1-00000001
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00000001/1-00000001.pdf",
					FileName = "1-00000001",
					FileType = "pdf",
					Revision = null,
					CreateDate = drawing00000001!.CreateDate,
					DrawingId = drawing00000001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00000001/2-00000002.dxf",
					FileName = "2-00000002",
					FileType = "dxf",
					Revision = null,
					CreateDate = drawing00000001!.CreateDate,
					DrawingId = drawing00000001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00000001/2-00000002.pdf",
					FileName = "2-00000002",
					FileType = "pdf",
					Revision = null,
					CreateDate = drawing00000001!.CreateDate,
					DrawingId = drawing00000001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00000001/2-00000002_A.dxf",
					FileName = "2-00000002_A",
					FileType = "dxf",
					Revision = "A",
					CreateDate = drawing00001001!.CreateDate,
					DrawingId = drawing00000001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00000001/2-00000002_A.pdf",
					FileName = "2-00000002_A",
					FileType = "pdf",
					Revision = "A",
					CreateDate = drawing00001001!.CreateDate,
					DrawingId = drawing00000001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00000001/2-00000002_B.dxf",
					FileName = "2-00000002_B",
					FileType = "dxf",
					Revision = "B",
					CreateDate = drawing00002001!.CreateDate,
					DrawingId = drawing00000001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00000001/2-00000002_B.pdf",
					FileName = "2-00000002_B",
					FileType = "pdf",
					Revision = "B",
					CreateDate = drawing00002001!.CreateDate,
					DrawingId = drawing00000001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00000001/2-00000003.dxf",
					FileName = "2-00000003",
					FileType = "dxf",
					Revision = null,
					CreateDate = drawing00000001!.CreateDate,
					DrawingId = drawing00000001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00000001/2-00000003.pdf",
					FileName = "2-00000003",
					FileType = "pdf",
					Revision = null,
					CreateDate = drawing00000001!.CreateDate,
					DrawingId = drawing00000001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00000001/2-00000004.dxf",
					FileName = "2-00000004",
					FileType = "dxf",
					Revision = null,
					CreateDate = drawing00000001!.CreateDate,
					DrawingId = drawing00000001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00000001/2-00000004.pdf",
					FileName = "2-00000004",
					FileType = "pdf",
					Revision = null,
					CreateDate = drawing00000001!.CreateDate,
					DrawingId = drawing00000001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00000001/2-00000004_A.dxf",
					FileName = "2-00000004_A",
					FileType = "dxf",
					Revision = "A",
					CreateDate = drawing00001001!.CreateDate,
					DrawingId = drawing00000001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00000001/2-00000004_A.pdf",
					FileName = "2-00000004_A",
					FileType = "pdf",
					Revision = "A",
					CreateDate = drawing00001001!.CreateDate,
					DrawingId = drawing00000001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00000001/2-00000004_B.dxf",
					FileName = "2-00000004_B",
					FileType = "dxf",
					Revision = "B",
					CreateDate = drawing00002001!.CreateDate,
					DrawingId = drawing00000001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00000001/2-00000004_B.pdf",
					FileName = "2-00000004_B",
					FileType = "pdf",
					Revision = "B",
					CreateDate = drawing00002001!.CreateDate,
					DrawingId = drawing00000001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00000001/2-00000005.pdf",
					FileName = "2-00000005",
					FileType = "pdf",
					Revision = null,
					CreateDate = drawing00000001!.CreateDate,
					DrawingId = drawing00000001!.Id
				},

				//For Drawing 1-00001001
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00001001/1-00001001.pdf",
					FileName = "1-00001001",
					FileType = "pdf",
					Revision = null,
					CreateDate = drawing00001001!.CreateDate,
					DrawingId = drawing00001001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00001001/1-00001001_A.pdf",
					FileName = "1-00001001_A",
					FileType = "pdf",
					Revision = "A",
					CreateDate = new DateTime(2023, 2, 3),
					DrawingId = drawing00001001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00001001/2-00001002.dxf",
					FileName = "2-00001002",
					FileType = "dxf",
					Revision = null,
					CreateDate = drawing00001001!.CreateDate,
					DrawingId = drawing00001001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00001001/2-00001002.pdf",
					FileName = "2-00001002",
					FileType = "pdf",
					Revision = null,
					CreateDate = drawing00001001!.CreateDate,
					DrawingId = drawing00001001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00001001/2-00001003.pdf",
					FileName = "2-00001003",
					FileType = "pdf",
					Revision = null,
					CreateDate = drawing00001001!.CreateDate,
					DrawingId = drawing00001001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00001001/2-00001003.step",
					FileName = "2-00001003",
					FileType = "step",
					Revision = null,
					CreateDate = drawing00001001!.CreateDate,
					DrawingId = drawing00001001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00001001/2-00001004.dxf",
					FileName = "2-00001004",
					FileType = "dxf",
					Revision = null,
					CreateDate = drawing00001001!.CreateDate,
					DrawingId = drawing00001001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00001001/2-00001004.pdf",
					FileName = "2-00001004",
					FileType = "pdf",
					Revision = null,
					CreateDate = drawing00001001!.CreateDate,
					DrawingId = drawing00001001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00001001/2-00001004_A.dxf",
					FileName = "2-00001004_A",
					FileType = "dxf",
					Revision = "A",
					CreateDate = new DateTime(2023, 2, 3),
					DrawingId = drawing00001001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00001001/2-00001004_A.pdf",
					FileName = "2-00001004_A",
					FileType = "pdf",
					Revision = "A",
					CreateDate = new DateTime(2023, 2, 3),
					DrawingId = drawing00001001!.Id
				},

				//For Drawing 1-00002001
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00002001/1-00002001.pdf",
					FileName = "1-00002001",
					FileType = "pdf",
					Revision = null,
					CreateDate = drawing00002001!.CreateDate,
					DrawingId = drawing00002001!.Id
				},
				new DrawingFile
				{
					FileUrl = "/Files/SeededData/1-00002001/2-00002002.pdf",
					FileName = "2-00002002",
					FileType = "pdf",
					Revision = null,
					CreateDate = drawing00002001!.CreateDate,
					DrawingId = drawing00002001!.Id
				}
			};
			return drawingFiles;
		}




		//Create the list of example layouts
		public static List<Layout> CreateLayouts(Project project0001)
		{
			var layouts = new List<Layout>
			{
				new Layout
				{
					FileUrl = "/Files/SeededData/Layout/0001-Layout.pdf",
					FileName = "0001-Layout",
					FileType = "pdf",
					Revision = null,
					CreateDate = new DateTime(2023, 2, 10),
					ProjectId = project0001.Id
				},
				new Layout
				{
					FileUrl = "/Files/SeededData/Layout/0001-Layout.dwg",
					FileName = "0001-Layout",
					FileType = "dwg",
					Revision = null,
					CreateDate = new DateTime(2023, 2, 10),
					ProjectId = project0001.Id
				}
			};
			return layouts;
		}




		//Create the list of example Documentation
		public static List<Documentation> CreateDocumentation(Project project0001)
		{
			var documentation = new List<Documentation>
			{
				new Documentation
				{
					FileUrl = "/Files/SeededData/Documentation/Roller Conveyor System Passport.pdf",
					FileName = "Roller Conveyor System Passport",
					FileType = "pdf",
					Revision = null,
					CreateDate = new DateTime(2023, 2, 10),
					ProjectId = project0001.Id
				},
				new Documentation
				{
					FileUrl = "/Files/SeededData/Documentation/Conveyor System Task.pdf",
					FileName = "Conveyor System Task",
					FileType = "pdf",
					Revision = null,
					CreateDate = project0001!.CreateDate,
					ProjectId = project0001.Id
				}
			};
			return documentation;
		}
	}
}
