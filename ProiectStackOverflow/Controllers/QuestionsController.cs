using ProiectStackOverflow.Data;
using ProiectStackOverflow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProiectStackOverflow.Controllers
{
	public class QuestionsController : Controller
	{
		private readonly ApplicationDbContext db;
		public QuestionsController(ApplicationDbContext context)
		{
			db = context;
		}
		public IActionResult Index(int id)
		{
			var questions = from question in db.Questions.Include("Tag")
							where question.TagId == id
							select question;

			Tag tag = db.Tags.Where(t => t.Id == id).First();

			ViewBag.Questions = questions;
			ViewBag.Tag = tag;

			if (TempData.ContainsKey("message"))
			{
				ViewBag.Message = TempData["message"];
			}

			return View();
		}

		public IActionResult Show(int id)
		{
			Question question = db.Questions.Include("Tag").Include("Comments").Include("Answers")
                              .Where(q => q.Id == id)
							  .First();

			return View(question);
		}

		public IActionResult New()
		{
			Question question = new Question();

			question.T = GetAllTags();

			return View(question);
		}

		[HttpPost]
		public IActionResult New(Question question)
		{
            question.Date = DateTime.Now;
            question.T = GetAllTags();

			if (ModelState.IsValid)
			{
				db.Questions.Add(question);
				db.SaveChanges();

				TempData["message"] = "Articolul a fost adaugat";
				return RedirectToAction("Index", new { id = question.TagId });
			}
			else
			{
				return View(question);
			}
		}
		public IActionResult Edit(int id)
		{
			Question question = db.Questions.Include("Tag")
										 .Where(q => q.Id == id)
										 .First();

			question.T = GetAllTags();

			return View(question);
		}

		[HttpPost]
		public IActionResult Edit(int id, Question requestQuestion)
		{
			Question question = db.Questions.Find(id);

			if (ModelState.IsValid)
			{
				question.Title = requestQuestion.Title;
				question.Content = requestQuestion.Content;
				question.TagId = requestQuestion.TagId;
				TempData["message"] = "Articolul a fost modificat";
				db.SaveChanges();
				return RedirectToAction("Show", new { id = id });
			}
			else
			{
				requestQuestion.T = GetAllTags();

				return View(requestQuestion);
			}
		}

		[HttpPost]
		public ActionResult Delete(int id)
		{
			Question question = db.Questions.Find(id);
			var tag = question.TagId;
			db.Questions.Remove(question);
			db.SaveChanges();
			return RedirectToAction("Index", new { id = tag });
		}
		[NonAction]
		public IEnumerable<SelectListItem> GetAllTags()
		{
			var selectList = new List<SelectListItem>();

			var tags = from t in db.Tags select t;

			foreach (var tag in tags)
			{
				selectList.Add(new SelectListItem
				{
					Value = tag.Id.ToString(),
					Text = tag.TagName
				});
			}
			return selectList;
		}

		//Pentru comentarii
		[HttpPost]
		public IActionResult ShowComm([FromForm] Comment comment)
		{
			comment.Date = DateTime.Now;
			if (ModelState.IsValid)
			{
				db.Comments.Add(comment);
				db.SaveChanges();
				return Redirect("/Questions/Show/" + comment.QuestionId);
			}
			else
			{
                return RedirectToAction("Show", new { id = comment.QuestionId });
            }
		}

        //Pentru raspunsuri
        [HttpPost]
        public IActionResult ShowAns([FromForm] Answer answer)
        {
            answer.Date = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Answers.Add(answer);
                db.SaveChanges();
                return Redirect("/Questions/Show/" + answer.QuestionId);
            }
            else
            {
                return RedirectToAction("Show", new { id = answer.QuestionId });
            }
        }
    }
}
