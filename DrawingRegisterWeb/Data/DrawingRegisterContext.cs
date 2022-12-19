
using Microsoft.EntityFrameworkCore;
using DrawingRegisterWeb.Models;

namespace DrawingRegisterWeb.Data
{
	public class DrawingRegisterContext : DbContext
	{
		public DrawingRegisterContext(DbContextOptions<DrawingRegisterContext> options) : base(options)
		{
		}

		public DbSet<Project> Project { get; set; }

		public DbSet<ProjectState> ProjectState { get; set; }

		public DbSet<Drawing> Drawing { get; set; }
		public DbSet<DrawingFile> DrawingFile { get; set; }
		public DbSet<Documentation> Documentation { get; set; }
		public DbSet<Layout> Layout { get; set; }
	}
}
