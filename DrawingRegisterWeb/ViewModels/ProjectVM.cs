using DrawingRegisterWeb.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DrawingRegisterWeb.ViewModels
{
    public class ProjectVM
    {
        public List<Project>? Projects { get; set; }
        public SelectList? ProjectStates { get; set; }
        public string? Search { get; set; }
        public string? States { get; set;}
    }
}
