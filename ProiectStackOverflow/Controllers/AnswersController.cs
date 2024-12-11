using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProiectStackOverflow.Data;
using ProiectStackOverflow.Models;

namespace ProiectStackOverflow.Controllers
{
    public class AnswersController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AnswersController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost]
		[Authorize(Roles = "Admin, User")]
		public IActionResult Delete(int id)
        {
            Answer ans = db.Answers.Find(id);
            
			if (ans.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
			{
				db.Answers.Remove(ans);
				db.SaveChanges();
				return Redirect("/Questions/Show/" + ans.QuestionId);
			}
			else
			{
				//TempData["message"] = "Nu aveti drepturi";
				//TempData["messageType"] = "alert-danger";
				return Redirect("/Questions/Show/" + ans.QuestionId);
			}
		}

		[Authorize(Roles = "Admin, User")]
		public IActionResult Edit(int id)
        {
            Answer ans = db.Answers.Find(id);

			if (ans.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
			{
				return View(ans);
			}
			else
			{
				//TempData["message"] = "Nu aveti drepturi";
				//TempData["messageType"] = "alert-danger";
				return Redirect("/Questions/Show/" + ans.QuestionId);
			}
		}

        [HttpPost]
		[Authorize(Roles = "Admin, User")]
		public IActionResult Edit(int id, Answer requestAnswer)
        {
            Answer ans = db.Answers.Find(id);

			if (ans.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
			{
				if (ModelState.IsValid)
				{
					ans.Content = requestAnswer.Content;
					db.SaveChanges();
					return Redirect("/Questions/Show/" + ans.QuestionId);
				}
				else
				{
					return View(requestAnswer);
				}
			}
			else
			{
				//TempData["message"] = "Nu aveti drepturi";
				//TempData["messageType"] = "alert-danger";
				return Redirect("/Questions/Show/" + ans.QuestionId);
			}
		}
    }
}
