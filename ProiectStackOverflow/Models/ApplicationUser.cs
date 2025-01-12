using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectStackOverflow.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Answer>? Answers { get; set; }
        public virtual ICollection<Question>? Questions { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Numele de utilizator este obligatoriu")]
        public override string UserName { get; set; }

        [MaxLength(500, ErrorMessage = "Descrierea nu poate sa aibă mai mult de 500 de caractere.")]
        public string? Description { get; set; }

        [NotMapped] public IEnumerable<SelectListItem>? AllRoles { get; set; }
    }
}