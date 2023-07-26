using System;
using System.ComponentModel.DataAnnotations;

namespace Howler.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [Required]
        public int PostId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        [StringLength(255)]
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public BarrenUser User { get; set; }
    }
}
