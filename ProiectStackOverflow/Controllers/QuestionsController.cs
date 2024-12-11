using ProiectStackOverflow.Data;
using ProiectStackOverflow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace ProiectStackOverflow.Controllers
{
	public class QuestionsController : Controller
	{
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public QuestionsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin, User")]
		public IActionResult Index(int id)
		{
			var questions = from question in db.Questions.Include("Tag").Include("User")
							where question.TagId == id
							select question;

			Tag tag = db.Tags.Where(t => t.Id == id).First();

			ViewBag.Questions = questions;
			ViewBag.Tag = tag;

			if (TempData.ContainsKey("message"))
			{
				//ViewBag.Message = TempData["message"];
				//ViewBag.Alert = TempData["messageType"];
			}

			return View();
		}

        [Authorize(Roles = "Admin, User")]
        public IActionResult Show(int id)
		{
			Question question = db.Questions.Include("Tag").Include("Comments")
							  .Include("Answers").Include("User").Include("Comments.User")
                              .Where(q => q.Id == id)
							  .First();

			SetAccesRights();

			return View(question);
		}

        [Authorize(Roles = "Admin, User")]
        public IActionResult New()
		{
			Question question = new Question();

			question.T = GetAllTags();

			return View(question);
		}

        [Authorize(Roles = "Admin, User")]
        [HttpPost]
		public IActionResult New(Question question)
		{
            question.Date = DateTime.Now;

			question.UserId = _userManager.GetUserId(User);

			if (ModelState.IsValid)
			{
				db.Questions.Add(question);
				db.SaveChanges();

				//TempData["message"] = "Articolul a fost adaugat";
				//TempData["messageType"] = "alert-success";
				return RedirectToAction("Index", new { id = question.TagId });
			}
			else
			{
                question.T = GetAllTags();
                return View(question);
			}
		}
		[Authorize(Roles = "Admin, User")]
		public IActionResult Edit(int id)
		{
			Question question = db.Questions.Include("Tag")
										 .Where(q => q.Id == id)
										 .First();

			question.T = GetAllTags();

			if(question.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
			{
				return View(question);
			}
			else
			{
				question.T = GetAllTags();
				//TempData["message"] = "Nu aveti dreptul sa faceti modificari";
				//TempData["messageType"] = "alert-danger";
				return RedirectToAction("Show", new { id = id });
			}
		}

		[HttpPost]
		[Authorize(Roles = "Admin, User")]
		public IActionResult Edit(int id, Question requestQuestion)
		{
			Question question = db.Questions.Find(id);

			if (ModelState.IsValid)
			{
				if (question.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
				{
					question.Title = requestQuestion.Title;
					question.Content = requestQuestion.Content;
					question.TagId = requestQuestion.TagId;
					//TempData["message"] = "Articolul a fost modificat";
					//TempData["messageType"] = "alert-success";
					db.SaveChanges();
					return RedirectToAction("Show", new { id = id });
				}
				else
				{
					//TempData["message"] = "Nu aveti dreptul sa faceti modificari";
					//TempData["messageType"] = "alert-danger";
					return RedirectToAction("Show", new { id = id });
				}
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

			if (question.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
			{
				var tag = question.TagId;
				db.Questions.Remove(question);
				db.SaveChanges();
				//TempData["message"] = "Articolul a fost sters";
				//TempData["messageType"] = "alert-success";
				return RedirectToAction("Index", new { id = tag });
			}
			else
			{
				//TempData["message"] = "Nu aveti dreptul sa stergeti";
				//TempData["messageType"] = "alert-danger";
				return RedirectToAction("Show", new { id = id });
			}
		}

		private void SetAccesRights()
		{
			ViewBag.UserCurent = _userManager.GetUserId(User);
			ViewBag.Admin = User.IsInRole("Admin");
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
        [Authorize(Roles = "Admin, User")]
        public IActionResult ShowComm([FromForm] Comment comment)
		{
			comment.Date = DateTime.Now;

			comment.UserId = _userManager.GetUserId(User);

			if (ModelState.IsValid)
			{
				db.Comments.Add(comment);
				db.SaveChanges();
				return Redirect("/Questions/Show/" + comment.QuestionId);
			}
			else
			{
				Question q = db.Questions.Include("Tag").Include("User")
							.Include("Comments").Include("Answers")
							.Include("Comment.User")
							.Where(q => q.Id == comment.QuestionId).First();
				SetAccesRights();
                return View(q);
            }
		}

        //Pentru raspunsuri
        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public IActionResult ShowAns([FromForm] Answer answer)
        {
            answer.Date = DateTime.Now;
            answer.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Answers.Add(answer);
                db.SaveChanges();
                return Redirect("/Questions/Show/" + answer.QuestionId);
            }
            else
            {
                Question q = db.Questions.Include("Tag").Include("User")
                            .Include("Comments").Include("Answers")
                            .Include("Comment.User")
                            .Where(q => q.Id == answer.QuestionId).First();
                SetAccesRights();
                return View(q);
            }
        }
    }
}
