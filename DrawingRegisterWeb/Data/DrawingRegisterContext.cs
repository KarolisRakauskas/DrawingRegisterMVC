
using Microsoft.EntityFrameworkCore;

namespace DrawingRegisterWeb.Data
{
	public class DrawingRegisterContext : DbContext
	{
		public DrawingRegisterContext(DbContextOptions<DrawingRegisterContext> options) : base(options)
		{
		}

		public DbSet<DrawingRegisterWeb.Models.ProjectModel> Projects { get; set; }
	}
}
