using DrawingRegisterWeb.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DrawingRegisterWeb.Models;

public static class SeedData
{
	public static void Initialize(IServiceProvider serviceProvider)
	{
		using var context = new DrawingRegisterContext(serviceProvider.GetRequiredService<DbContextOptions<DrawingRegisterContext>>());

		if (!context.Roles.Any())
		{
			context.AddRange(
				new IdentityRole() { Name = ConstData.Role_Admin_Name, NormalizedName = ConstData.Role_Admin_NormalizedName},
				new IdentityRole() { Name = ConstData.Role_Engr_Name, NormalizedName = ConstData.Role_Engr_NormalizedName },
				new IdentityRole() { Name = ConstData.Role_Mech_Name, NormalizedName = ConstData.Role_Mech_NormalizedName }
				);
			context.SaveChanges();
		}
		if (!context.Statuses.Any())
		{
			context.AddRange(
				new Status() { Name = ConstData.Status_Approval_pending}
				);
			context.SaveChanges();
		}

		//context.AddRange(

		//	//ProjectNubmer = "0001"
		//	new Drawing
		//	{
		//		DrawingNumber = "1-00000001",
		//		Name = "Roller Conveyor W1000 L3000",
		//		Description = "Driven Roller Conveyor, Width - 1000 mm, Length - 3000 mm. Rollers pattern every 200 mm.",
		//		CreateDate = project0001!.CreateDate,
		//		ProjectId = project0001.Id
		//	},
		//	new Drawing
		//	{
		//		DrawingNumber = "1-00001001",
		//		Name = "Roller Guide L3000",
		//		Description = "Product support guide for conveyor 1-00000001. Full Length - 3000 mm.",
		//		CreateDate = new DateTime(2023, 2, 1),
		//		ProjectId = project0001.Id
		//	},
		//	new Drawing
		//	{
		//		DrawingNumber = "1-00002001",
		//		Name = "Adjustable leg",
		//		Description = "Adjustable leg for conveyor 1-00000001.",
		//		CreateDate = new DateTime(2023, 2, 10),
		//		ProjectId = project0001.Id
		//	}
		//	);

		//context.SaveChanges();

		//var drawings = from d in context.Drawing select d;
		//var drawing00000001 = drawings.FirstOrDefault(d => d.DrawingNumber == "1-00000001");
		//var drawing00001001 = drawings.FirstOrDefault(d => d.DrawingNumber == "1-00001001");
		//var drawing00002001 = drawings.FirstOrDefault(d => d.DrawingNumber == "1-00002001");

		//context.DrawingFile.AddRange(

		//	//1-00000001
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/1-00000001.pdf",
		//		FileName = "1-00000001",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00000001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000002.dxf",
		//		FileName = "2-00000002",
		//		FileType = "dxf",
		//		Revision = null,
		//		CreateDate = drawing00000001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000002.pdf",
		//		FileName = "2-00000002",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00000001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000002_A.dxf",
		//		FileName = "2-00000002_A",
		//		FileType = "dxf",
		//		Revision = "A",
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000002_A.pdf",
		//		FileName = "2-00000002_A",
		//		FileType = "pdf",
		//		Revision = "A",
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000002_B.dxf",
		//		FileName = "2-00000002_B",
		//		FileType = "dxf",
		//		Revision = "B",
		//		CreateDate = drawing00002001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000002_B.pdf",
		//		FileName = "2-00000002_B",
		//		FileType = "pdf",
		//		Revision = "B",
		//		CreateDate = drawing00002001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000003.dxf",
		//		FileName = "2-00000003",
		//		FileType = "dxf",
		//		Revision = null,
		//		CreateDate = drawing00000001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000003.pdf",
		//		FileName = "2-00000003",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00000001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000004.dxf",
		//		FileName = "2-00000004",
		//		FileType = "dxf",
		//		Revision = null,
		//		CreateDate = drawing00000001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000004.pdf",
		//		FileName = "2-00000004",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00000001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000004_A.dxf",
		//		FileName = "2-00000004_A",
		//		FileType = "dxf",
		//		Revision = "A",
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000004_A.pdf",
		//		FileName = "2-00000004_A",
		//		FileType = "pdf",
		//		Revision = "A",
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000004_B.dxf",
		//		FileName = "2-00000004_B",
		//		FileType = "dxf",
		//		Revision = "B",
		//		CreateDate = drawing00002001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000004_B.pdf",
		//		FileName = "2-00000004_B",
		//		FileType = "pdf",
		//		Revision = "B",
		//		CreateDate = drawing00002001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00000001/2-00000005.pdf",
		//		FileName = "2-00000005",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00000001!.CreateDate,
		//		DrawingId = drawing00000001!.Id
		//	},

		//	//1-00001001
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/1-00001001.pdf",
		//		FileName = "1-00001001",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00001001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/1-00001001_A.pdf",
		//		FileName = "1-00001001_A",
		//		FileType = "pdf",
		//		Revision = "A",
		//		CreateDate = new DateTime(2023, 2, 3),
		//		DrawingId = drawing00001001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/2-00001002.dxf",
		//		FileName = "2-00001002",
		//		FileType = "dxf",
		//		Revision = null,
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00001001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/2-00001002.pdf",
		//		FileName = "2-00001002",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00001001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/2-00001003.pdf",
		//		FileName = "2-00001003",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00001001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/2-00001003.step",
		//		FileName = "2-00001003",
		//		FileType = "step",
		//		Revision = null,
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00001001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/2-00001004.dxf",
		//		FileName = "2-00001004",
		//		FileType = "dxf",
		//		Revision = null,
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00001001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/2-00001004.pdf",
		//		FileName = "2-00001004",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00001001!.CreateDate,
		//		DrawingId = drawing00001001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/2-00001004_A.dxf",
		//		FileName = "2-00001004_A",
		//		FileType = "dxf",
		//		Revision = "A",
		//		CreateDate = new DateTime(2023, 2, 3),
		//		DrawingId = drawing00001001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00001001/2-00001004_A.pdf",
		//		FileName = "2-00001004_A",
		//		FileType = "pdf",
		//		Revision = "A",
		//		CreateDate = new DateTime(2023, 2, 3),
		//		DrawingId = drawing00001001!.Id
		//	},

		//	//1-00002001
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00002001/1-00002001.pdf",
		//		FileName = "1-00002001",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00002001!.CreateDate,
		//		DrawingId = drawing00002001!.Id
		//	},
		//	new DrawingFile
		//	{
		//		FileUrl = "/Files/SeededData/1-00002001/2-00002002.pdf",
		//		FileName = "2-00002002",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00002001!.CreateDate,
		//		DrawingId = drawing00002001!.Id
		//	}
		//	);

		//context.Layout.AddRange(
		//	new Layout
		//	{
		//		FileUrl = "/Files/SeededData/Layout/0001-Layout.pdf",
		//		FileName = "0001-Layout",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00002001!.CreateDate,
		//		ProjectId = project0001.Id
		//	},
		//	new Layout
		//	{
		//		FileUrl = "/Files/SeededData/Layout/0001-Layout.dwg",
		//		FileName = "0001-Layout",
		//		FileType = "dwg",
		//		Revision = null,
		//		CreateDate = drawing00002001!.CreateDate,
		//		ProjectId = project0001.Id
		//	}
		//	);

		//context.Documentation.AddRange(
		//	new Documentation
		//	{
		//		FileUrl = "/Files/SeededData/Documentation/Roller Conveyor System Passport.pdf",
		//		FileName = "Roller Conveyor System Passport",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = drawing00002001!.CreateDate,
		//		ProjectId = project0001.Id
		//	},
		//	new Documentation
		//	{
		//		FileUrl = "/Files/SeededData/Documentation/Conveyor System Task.pdf",
		//		FileName = "Conveyor System Task",
		//		FileType = "pdf",
		//		Revision = null,
		//		CreateDate = project0001!.CreateDate,
		//		ProjectId = project0001.Id
		//	}
		//	);

		//context.SaveChanges();
		//}
	}
}
