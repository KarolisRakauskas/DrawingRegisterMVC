using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DrawingRegisterWeb.Models
{
	public class DrawingRegister
	{
		[Key]
		public int Id { get; set; }
		[DisplayName("Create Date")]
		[DataType(DataType.Date)]
		public DateTime CreateDate { get; set; } = DateTime.Now;
	}
}
