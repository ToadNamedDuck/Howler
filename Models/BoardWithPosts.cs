using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Howler.Models
{
    public class BoardWithPosts
    {
        public int Id { get; set; }
        [Required]
        [StringLength(75)]
        public string Name { get; set; }
        [Required]
        [StringLength(255)]
        public string Topic { get; set; }
        [Required]
        [StringLength(255)]
        public string Description { get; set; }
        [Required]
        public int BoardOwnerId { get; set; }
        public bool IsPackBoard { get; set; }
        public BarrenUser BoardOwner { get; set; }
        public List<Post> Posts { get; set; } 
    }
}
