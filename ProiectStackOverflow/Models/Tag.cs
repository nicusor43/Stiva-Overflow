using System.ComponentModel.DataAnnotations;

namespace ProiectStackOverflow.Models
{
	public class Tag
	{
		[Key]
		public int Id { get; set; }
		public string? TagName { get; set; }
		public string? Description { get; set; }
		public virtual ICollection<Question>? Questions { get; set; }
	}
}
