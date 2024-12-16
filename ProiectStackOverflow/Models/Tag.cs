using System.ComponentModel.DataAnnotations;

namespace ProiectStackOverflow.Models
{
	public class Tag
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "Name is mandatory")]
		public string? TagName { get; set; }
		[Required(ErrorMessage = "Description is mandatory")]
		public string? Description { get; set; }
		public virtual ICollection<Question>? Questions { get; set; }
	}
}
