using Microsoft.AspNetCore.Identity;

namespace ProiectStackOverflow.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Answer>? Answers { get; set; }
        public virtual ICollection<Question>? Questinos { get; set; }
    }
}
