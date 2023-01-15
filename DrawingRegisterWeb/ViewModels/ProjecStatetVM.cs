using DrawingRegisterWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DrawingRegisterWeb.ViewModels
{
    public class ProjectStateVM
    {
        public List<ProjectState>? ProjectStates { get; set; }
        public DrawingRegister? DrawingRegister { get; set; }
        public string? Search { get; set; }
        public string? States { get; set;}
    }
}
