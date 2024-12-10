using ProiectStackOverflow.Data;
using ProiectStackOverflow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProiectStackOverflow.Controllers
{
	public class CommentsController : Controller
	{
		private readonly ApplicationDbContext db;
		public CommentsController(ApplicationDbContext context)
		{
			db = context;
		}

		[HttpPost]
		public IActionResult Delete(int id)
		{
			Comment comm = db.Comments.Find(id);
			db.Comments.Remove(comm);
			db.SaveChanges();
			return Redirect("/Questions/Show/" + comm.QuestionId);
		}
		public IActionResult Edit(int id)
		{
			Comment comm = db.Comments.Find(id);
			return View(comm);
		}

		[HttpPost]
		public IActionResult Edit(int id, Comment requestComment)
		{
			Comment comm = db.Comments.Find(id);
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
	}
}
