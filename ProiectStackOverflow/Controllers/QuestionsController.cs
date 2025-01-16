using ProiectStackOverflow.Data;
using ProiectStackOverflow.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Ganss.Xss;
using System.Text.RegularExpressions;
using ProiectStackOverflow.Helpers;

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

        public IActionResult HomeIndex(int? page, string sortOrder)
        {
            /*var questions = db.Questions.Include("Tag")
                                        .Include("User")
                                .OrderByDescending(q => q.Date)
                                .Take(5);

            ViewBag.Questions = questions;

            return View();*/

            var questions = db.Questions.Include("Tag")
                .Include("User")
                .Include(q => q.Answers)
                .OrderBy(a => a.Date);

            var search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim();
            }

            // Cautare in articol (Title si Content)
            List<int> questionIds = db.Questions.Where
            (
                q => q.Title.Contains(search)
                     || q.Content.Contains(search)
            ).Select(q => q.Id).ToList();

            // Cautare in comentarii (Content)
            List<int> questionIdsOfCommentsWithSearchString =
                db.Comments.Where
                (
                    c => c.Content.Contains(search)
                ).Select(c => (int)c.QuestionId).ToList();

            List<int> questionIdsOfAnswersWithSearchString =
                db.Answers.Where
                (
                    a => a.Content.Contains(search)
                ).Select(a => (int)a.QuestionId).ToList();

            List<int> mergedIds = questionIds.Union(questionIdsOfCommentsWithSearchString)
                                             .Union(questionIdsOfAnswersWithSearchString)
                                             .ToList();

            questions = db.Questions.Where(question =>
                    mergedIds.Contains(question.Id))
                .Include("Tag")
                .Include("User")
                .Include(q => q.Answers)
                .OrderBy(a => a.Date);

            // Sortează întrebările
            switch (sortOrder)
            {
                case "date_asc":
                    questions = questions.OrderBy(q => q.Date);
                    break;
                case "date_desc":
                    questions = questions.OrderByDescending(q => q.Date);
                    break;
                case "answers_asc":
                    questions = questions.OrderBy(q => q.Answers.Count);
                    break;
                case "answers_desc":
                    questions = questions.OrderByDescending(q => q.Answers.Count);
                    break;
            }

            //PAGINATIE
            int totalItems = questions.Count();
            int page_questions = 5;

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            var offset = 0;
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * page_questions;
            }

            var paginatedQuestions = questions.Skip(offset).Take(page_questions);
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)page_questions);
            ViewBag.CurrentPage = currentPage;
            ViewBag.Questions = paginatedQuestions;
            ViewBag.PaginationBaseUrl = $"/Questions/HomeIndex/?page";

            /*if (!string.IsNullOrEmpty(search))
            {
                foreach (var question in questions)
                {
                    // Evidențiază termenul de căutare în titlu
                    question.Title = Regex.Replace(question.Title, Regex.Escape(search),
                        match => $"<mark>{match.Value}</mark>", RegexOptions.IgnoreCase);

                    // Evidențiază termenul de căutare în conținut
                    question.Content = Regex.Replace(question.Content, Regex.Escape(search),
                        match => $"<mark>{match.Value}</mark>", RegexOptions.IgnoreCase);
                }
            }*/


            ViewBag.SearchString = search;
            if (search != "")
            {
                ViewBag.PaginationBaseUrl = "/Questions/HomeIndex/?search="
                                            + search + "&page";
            }
            else
            {
                ViewBag.PaginationBaseUrl = "/Questions/HomeIndex/?page";
            }

            // Setează ViewBag.SortOrder pentru a păstra starea sortării
            ViewBag.SortOrder = sortOrder;
            ViewBag.PaginationBaseUrl = $"/Questions/HomeIndex/?sortOrder={sortOrder}&page";

			ViewBag.UserCurent = User.Identity.Name;

			return View();
        }

        public IActionResult Index(int id, int? page, string sortOrder)
        {
            int page_questions = 5;

            var questions = from question in db.Questions.Include("Tag")
                    .Include("User")
                    .Include(q => q.Answers) // Adăugat pentru a putea sorta după numărul de răspunsuri
                where question.TagId == id
                select question;

            // Sortează întrebările
            switch (sortOrder)
            {
                case "date_asc":
                    questions = questions.OrderBy(q => q.Date);
                    break;
                case "date_desc":
                    questions = questions.OrderByDescending(q => q.Date);
                    break;
                case "answers_asc":
                    questions = questions.OrderBy(q => q.Answers.Count);
                    break;
                case "answers_desc":
                    questions = questions.OrderByDescending(q => q.Answers.Count);
                    break;
                default:
                    questions = questions.OrderByDescending(q => q.Date); // Sortare implicită
                    break;
            }

            int totalItems = questions.Count();

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            var offset = 0;
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * page_questions;
            }

            var paginatedQuestions = questions.Skip(offset).Take(page_questions);
            ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)page_questions);
            ViewBag.CurrentPage = currentPage;

            // Modifică ViewBag.PaginationBaseUrl pentru a include sortOrder
            ViewBag.PaginationBaseUrl = $"/Questions/Index/{id}?sortOrder={sortOrder}&page";

			Tag tag = db.Tags.Where(t => t.Id == id).First();
			ViewBag.Tag = tag;
			ViewBag.Questions = paginatedQuestions;
			ViewBag.Admin = User.IsInRole("Admin");

            // Adaugă ViewBag.SortOrder pentru a menține starea sortării
            ViewBag.SortOrder = sortOrder;

            return View();
        }

        // public IActionResult Index(int id)
        // {
        //     int page_questions = 5;
        //
        //     var questions = from question in db.Questions.Include("Tag")
        //             .Include("User")
        //         where question.TagId == id
        //         select question;
        //
        //     int totalItems = questions.Count();
        //
        //     var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);
        //
        //     var offset = 0;
        //     if (!currentPage.Equals(0))
        //     {
        //         offset = (currentPage - 1) * page_questions;
        //     }
        //
        //     var paginatedQuestions = questions.Skip(offset).Take(page_questions);
        //     ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)page_questions);
        //     ViewBag.CurrentPage = currentPage;
        //     ViewBag.PaginationBaseUrl = $"/Questions/Index/{id}?page";
        //
        //     Tag tag = db.Tags.Where(t => t.Id == id).First();
        //     ViewBag.Tag = tag;
        //     ViewBag.Questions = paginatedQuestions;
        //
        //     return View();
        // }

        public IActionResult Show(int id, string sortOrder)
        {
            Question question = db.Questions.Include("Tag")
                .Include("Comments")
                .Include("Answers")
                .Include("User")
                .Include("Comments.User")
                .Include("Answers.User")
                .Where(q => q.Id == id)
                .First();

            switch (sortOrder)
            {
                case "date_asc":
                    question.Answers = question.Answers.OrderBy(a => a.Date).ToList();
                    break;
                case "date_desc":
                    question.Answers = question.Answers.OrderByDescending(a => a.Date).ToList();
                    break;
                default:
                    question.Answers = question.Answers.OrderByDescending(a => a.Date).ToList(); // Sortare implicită (descrescător după dată)
                    break;
            }
            
            SetAccesRights();
            
            ViewBag.SortOrder = sortOrder;

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
            var sanitizer = new HtmlSanitizer();

            question.Date = DateTime.Now;

            question.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                //question.Content = sanitizer.Sanitize(question.Content);
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

            if (question.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(question);
            }
            else
            {
                question.T = GetAllTags();
                return RedirectToAction("Show", new { id = id });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, User")]
        public IActionResult Edit(int id, Question requestQuestion)
        {
            Question question = db.Questions.Find(id);

            //var sanitizer = new HtmlSanitizer();

            if (Summernote.IsEditorEmpty(requestQuestion.Content, 0))
            {
                ModelState.AddModelError("Content", "Content is required.");
            }

            if (ModelState.IsValid)
            {
                if (question.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    //requestQuestion.Content = sanitizer.Sanitize(requestQuestion.Content);

                    question.Title = requestQuestion.Title;
                    question.Content = requestQuestion.Content;
                    question.TagId = requestQuestion.TagId;

                    db.SaveChanges();
                    return RedirectToAction("Show", new { id = id });
                }
                else
                {
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
            //Question question = db.Questions.Find(id);

            Question question = db.Questions.Include("Tag")
                .Include("Comments")
                .Include("Answers")
                .Include("User")
                .Include("Comments.User")
                .Include("Answers.User")
                .Where(q => q.Id == id)
                .First();

            if (question.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                var tag = question.TagId;
                db.Questions.Remove(question);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = tag });
            }
            else
            {
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
        //[Authorize(Roles = "Admin, User")]
        public IActionResult ShowComm([FromForm] Comment comment)
        {
            comment.Date = DateTime.Now;

            comment.UserId = _userManager.GetUserId(User);

            if (!User.Identity.IsAuthenticated)
            {
                // Setează URL-ul corect pentru redirecționare după login
                return Redirect("/Identity/Account/Login?ReturnUrl=/Questions/Show/" + comment.QuestionId);
            }

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect("/Questions/Show/" + comment.QuestionId);
            }
            else
            {
                SetAccesRights();
                return Redirect("/Questions/Show/" + comment.QuestionId);
            }
        }

        //Pentru raspunsuri
        [HttpPost]
        //[Authorize(Roles = "Admin, User")]
        public IActionResult ShowAns([FromForm] Answer answer)
        {
            answer.Date = DateTime.Now;
            answer.UserId = _userManager.GetUserId(User);

            if (!User.Identity.IsAuthenticated)
            {
                // Setează URL-ul corect pentru redirecționare după login
                return Redirect("/Identity/Account/Login?ReturnUrl=/Questions/Show/" + answer.QuestionId);
            }

            if (ModelState.IsValid)
            {
                db.Answers.Add(answer);
                db.SaveChanges();
                return Redirect("/Questions/Show/" + answer.QuestionId);
            }
            else
            {
                SetAccesRights();
                return Redirect("/Questions/Show/" + answer.QuestionId);
            }
        }
    }
}