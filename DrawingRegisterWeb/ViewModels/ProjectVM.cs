using DrawingRegisterWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DrawingRegisterWeb.ViewModels
{
    public class ProjectVM
    {
        public Project? Project { get; set; }
        public List<Project>? Projects { get; set; }
        public SelectList? ProjectStates { get; set; }
        public List<Drawing>? Drawings { get; set; }
        public List<Documentation>? Documentations { get; set; }
        public List<Layout>? Layouts { get; set; }
        public string? Search { get; set; }
        public string? States { get; set;}
    }
}
