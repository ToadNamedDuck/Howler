using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace Howler.Models
{
    public class PostWithComments
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
        public BarrenUser User { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
