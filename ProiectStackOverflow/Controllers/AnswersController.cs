using Microsoft.AspNetCore.Mvc;
using ProiectStackOverflow.Data;
using ProiectStackOverflow.Models;

namespace ProiectStackOverflow.Controllers
{
    public class AnswersController : Controller
    {
        private readonly ApplicationDbContext db;
        public AnswersController(ApplicationDbContext context)
        {
            db = context;
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Answer ans = db.Answers.Find(id);
            db.Answers.Remove(ans);
            db.SaveChanges();
            return Redirect("/Questions/Show/" + ans.QuestionId);
        }
        public IActionResult Edit(int id)
        {
            Answer ans = db.Answers.Find(id);
            return View(ans);
        }

        [HttpPost]
        public IActionResult Edit(int id, Answer requestAnswer)
        {
            Answer ans = db.Answers.Find(id);
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
    }
}
