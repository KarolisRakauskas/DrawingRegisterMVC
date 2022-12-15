using DrawingRegisterWeb.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace DrawingRegisterWeb.ViewModels
{
    public class ProjectVM
    {
        public Project Project { get; set; } = null!;
        [ValidateNever]
        public IEnumerable<ProjectState>? ProjectStates { get; set; }
        [ValidateNever]
        public IEnumerable<ProjectItem>? ProjectItems { get; set; }
        [ValidateNever]
        public IEnumerable<Drawing>? Drawings { get; set; }
    }
}
