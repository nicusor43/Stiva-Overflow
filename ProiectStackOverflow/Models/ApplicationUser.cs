using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ProiectStackOverflow.Models
{
	public class ApplicationUser : IdentityUser
	{
		public virtual ICollection<Comment>? Comments { get; set; }
		public virtual ICollection<Answer>? Answers { get; set; }
		public virtual ICollection<Question>? Questions { get; set; }
		
		[Required(ErrorMessage = "Please enter your first name.")]
		public string? FirstName { get; set; }
		
		[Required(ErrorMessage = "Please enter your last name.")]
		public string? LastName { get; set; }

		[NotMapped] public IEnumerable<SelectListItem>? AllRoles { get; set; }
		
		[StringLength(500, ErrorMessage = "Short Description must be less than 500 characters.")]
		public string? ShortDescription { get; set; }


		public string? PhotoFileName { get; set; } // Store file name instead of byte array	}
	}
}
