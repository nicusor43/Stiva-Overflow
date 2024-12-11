using ProiectStackOverflow.Data;
using ProiectStackOverflow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ProiectStackOverflow.Controllers
{
	public class CommentsController : Controller
	{
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CommentsController(
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
			Comment comm = db.Comments.Find(id);

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comm);
                db.SaveChanges();
                return Redirect("/Questions/Show/" + comm.QuestionId);
            }
            else
            {
                //TempData["message"] = "Nu aveti drepturi";
                //TempData["messageType"] = "alert-danger";
                return Redirect("/Questions/Show/" + comm.QuestionId);
            }
		}

        [Authorize(Roles = "Admin, User")]
        public IActionResult Edit(int id)
		{
			Comment comm = db.Comments.Find(id);

            if(comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(comm);
            }
            else
            {
                //TempData["message"] = "Nu aveti drepturi";
                //TempData["messageType"] = "alert-danger";
                return Redirect("/Questions/Show/" + comm.QuestionId);
            }
		}

		[HttpPost]
        [Authorize(Roles = "Admin, User")]
        public IActionResult Edit(int id, Comment requestComment)
		{
			Comment comm = db.Comments.Find(id);
            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    comm.Content = requestComment.Content;
                    db.SaveChanges();
                    return Redirect("/Questions/Show/" + comm.QuestionId);
                }
                else
                {
                    return View(requestComment);
                }
            }
            else
            {
                //TempData["message"] = "Nu aveti drepturi";
                //TempData["messageType"] = "alert-danger";
                return Redirect("/Questions/Show/" + comm.QuestionId);
            }

        }
	}
}
