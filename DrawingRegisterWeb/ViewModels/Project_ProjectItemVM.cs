using DrawingRegisterWeb.Models;

namespace DrawingRegisterWeb.ViewModels
{
    public class Project_ProjectItemVM
    {
        public Project? Project { get; set; }
        public List<ProjectItem>? ProjectItems { get; set; }
    }
}
