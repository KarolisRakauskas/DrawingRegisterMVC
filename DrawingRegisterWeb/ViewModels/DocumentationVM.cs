using DrawingRegisterWeb.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DrawingRegisterWeb.ViewModels
{
    public class DocumentationVM
    {
        public Documentation? Documentation { get; set; }
        public List<Documentation>? Documentations { get; set; }
        public SelectList? ProjectSelectList { get; set; }
        public string? Search { get; set; }
        public string? Projects { get; set; }
    }
}
