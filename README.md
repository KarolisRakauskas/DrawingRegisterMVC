<a name="readme-top"></a>

# Drawing Register MVC

Practice project for ASP.NET Core (Model-View-Controller) with C# and Razor Pages

<details>
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#about-the-project">About The Project</a></li>
    <li><a href="#dependencies">Dependencies</a></li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>

## About The Project

Web based software to store, manage, change electronic drawing and documentation files

[![Product Name Screen Shot][product-screenshot]](https://example.com)

Built With :

![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![JavaScript](https://img.shields.io/badge/javascript-%23323330.svg?style=for-the-badge&logo=javascript&logoColor=%23F7DF1E)
![MicrosoftSQLServer](https://img.shields.io/badge/Microsoft%20SQL%20Server-CC2927?style=for-the-badge&logo=microsoft%20sql%20server&logoColor=white)
[![Bootstrap][Bootstrap.com]][Bootstrap-url]
![Azure](https://img.shields.io/badge/azure-%230072C6.svg?style=for-the-badge&logo=microsoftazure&logoColor=white)


<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Dependencies

ASP.NET Core 6 + Packages:

* `Microsoft.EntityFrameworkCore (7.0.0)`
* `Microsoft.EntityFrameworkCore.SqlServer (7.0.0)`
* `Microsoft.EntityFrameworkCore.Tools (7.0.0)`
* `Microsoft.EntityFrameworkCore.Identity (2.2.0)`
* `Microsoft.EntityFrameworkCore.Identity.EntityFrameworkCore (6.0.12)`
* `Microsoft.EntityFrameworkCore.Identity.UI (6.0.12)`
* `MailKit (3.5.0)`
* `MimeKit (3.5.0)`
* `Aspose.PDF (22.12.0)`

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Usage
1. Change Server name at Connection Strings in `appsettings.json`

```json
"DefaultConnection": "Server=(localdb)\\{YOUR LOCAL MSSQL DATABASE NAME};Database=DrawingRegisterMVC;Trusted_Connection=True;MultipleActiveResultSets=true"
```

2. No need for `Add-Migrations` or `Update-Database`. Everything is handled automatically by `DbInitializer.cs`

3. Enter your generic gmail email address and app password in `EmailSender.cs`

```C#
emailClient.Authenticate("EnterYourEmail", "EnterYourAppPassword");
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Contact

Karolis Rakauskas - karolisrakausku@gmail.com

Project Link: [https://github.com/KarolisRakauskas/DrawingRegisterMVC](https://github.com/KarolisRakauskas/DrawingRegisterMVC)


[product-screenshot]: /DrawingRegisterWeb/wwwroot/Assets/screenshot.png
[Bootstrap.com]: https://img.shields.io/badge/Bootstrap-563D7C?style=for-the-badge&logo=bootstrap&logoColor=white
[Bootstrap-url]: https://getbootstrap.com
