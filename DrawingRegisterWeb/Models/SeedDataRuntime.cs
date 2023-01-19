
namespace DrawingRegisterWeb.Models
{
	//Create Lists of elements for seeding data: Project States, Projects, Drawings, Layouts and Documentation
	public class SeedDataRuntime
	{
		//Creat The List of Standart Project states based of Drawing Register Id
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
					Description = "It is a customized project state. You can create your own customized project statements, to give more details about the progress of the project." +
					" Project accessibility will be the same as project state - running.",
					DrawingRegisterId = id
				}
			};
			return states;
		}


	}
}
