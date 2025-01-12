using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProiectStackOverflow.Data; // Înlocuiește cu namespace-ul tău
using ProiectStackOverflow.Models; // Înlocuiește cu namespace-ul tău

public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string id)
    {
        ApplicationUser user;

        if (string.IsNullOrEmpty(id))
        {
            // Dacă nu este specificat un ID, afișează profilul utilizatorului curent
            user = await _userManager.GetUserAsync(User);
        }
        else
        {
            // Altfel, caută utilizatorul după ID
            user = await _context.Users.FindAsync(id);
        }

        if (user == null)
        {
            return NotFound();
        }

        // Preia ultimele 5 întrebări
        var recentQuestions = await _context.Questions
            .Where(q => q.UserId == user.Id)
            .OrderByDescending(q => q.Date)
            .Take(5)
            .ToListAsync();

        // Preia ultimele 5 răspunsuri
        var recentAnswers = await _context.Answers
            .Where(a => a.UserId == user.Id)
            .OrderByDescending(a => a.Date)
            .Include(a => a.Question)
            .Take(5)
            .ToListAsync();

        // Crează un ViewModel pentru a trimite datele către View
        var viewModel = new Profile()
        {
            User = user,
            RecentQuestions = recentQuestions,
            RecentAnswers = recentAnswers
        };

        return View(viewModel);
    }
}