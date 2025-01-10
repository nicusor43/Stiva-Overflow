using ProiectStackOverflow.Data;
using ProiectStackOverflow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace ProiectStackOverflow.Controllers
{
	public class TagsController : Controller
	{
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public TagsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
		{
			var tags = from tag in db.Tags
							 orderby tag.TagName
							 select tag;

			ViewBag.Tags = tags;
			ViewBag.Admin = User.IsInRole("Admin");
			
			var search = "";
			if (Convert.ToString(HttpContext.Request.Query["TagSearch"]) != null)
			{
				search = Convert.ToString(HttpContext.Request.Query["TagSearch"]).Trim();
			}
			List<int> tagIds = db.Tags.Where
			(
				t => t.TagName.Contains(search)
				|| t.Description.Contains(search)
			).Select(tag => tag.Id).ToList();

			tags = db.Tags.Where(tag => tagIds.Contains(tag.Id)).OrderBy(t => t.TagName);

			ViewBag.TagSearchString = search;

			if (search != "")
			{
                ViewBag.Tags = tags;
			}
			return View();
		}

		[Authorize(Roles = "Admin")]
		public ActionResult Show(int id)
        {
            Tag tag = db.Tags.Find(id);
            return View(tag);
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
		[Authorize(Roles = "Admin")]
		public ActionResult New(Tag t)
        {
            if (ModelState.IsValid)
            {
                db.Tags.Add(t);
                db.SaveChanges();
                //TempData["message"] = "Tag adaugat";
                return RedirectToAction("Index");
            }

            else
            {
                return View(t);
            }
        }

		[Authorize(Roles = "Admin")]
		public ActionResult Edit(int id)
        {
            Tag tag = db.Tags.Find(id);
            return View(tag);
        }

        [HttpPost]
		[Authorize(Roles = "Admin")]
		public ActionResult Edit(int id, Tag requestTag)
        {
            Tag tag = db.Tags.Find(id);

            if (ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.Write(error.ErrorMessage);
                }
                tag.TagName = requestTag.TagName;
                tag.Description = requestTag.Description;
                db.SaveChanges();
                //TempData["message"] = "Tag modificat!";
                return RedirectToAction("Index");
            }
            else
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.Write(error.ErrorMessage);
                }
                return View(requestTag);
            }
        }

		[Authorize(Roles = "Admin")]
		public IActionResult Delete(int id)
        {
            Tag tag = db.Tags.Find(id);

            //Tag tag = db.Tags.Include("Questions").Include("Questions.Comments")
              //          .Where(t => t.Id == id).First();

            db.Tags.Remove(tag);
            //TempData["message"] = "Tagul a fost sters";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
	}
}
