using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectStackOverflow.Models
{
	public class Comment
	{
		[Key]
		public int Id { get; set; }
		[Required(ErrorMessage = "Content is mandatory")]
		public string Content { get; set; }
		public DateTime Date { get; set; }
		public int? QuestionId { get; set; }
		public virtual Question? Question { get; set; }
	}
}
