using ProiectStackOverflow.Data;
using ProiectStackOverflow.Models;
using Microsoft.AspNetCore.Mvc;

namespace ProiectStackOverflow.Controllers
{
	public class TagsController : Controller
	{
		private readonly ApplicationDbContext db;
		public TagsController(ApplicationDbContext context)
		{
			db = context;
		}
		public IActionResult Index()
		{
			var tags = from tag in db.Tags
							 orderby tag.TagName
							 select tag;
			ViewBag.Tags = tags;
			return View();
		}
	}
}
