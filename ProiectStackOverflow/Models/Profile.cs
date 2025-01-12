namespace ProiectStackOverflow.Models;

public class Profile
{
    public ApplicationUser User { get; set; }
    public List<Question> RecentQuestions { get; set; }
    public List<Answer> RecentAnswers { get; set; }
}