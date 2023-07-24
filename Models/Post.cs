using System;
using System.ComponentModel.DataAnnotations;

namespace Howler.Models
{
    public class Post
    {
        public int Id { get; set; }
        [Required]
        [StringLength(75)]
        public string Title { get; set; }
        [Required]
        [StringLength(1000)]
        public string Content { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int BoardId { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
