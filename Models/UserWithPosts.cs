using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Howler.Models
{
    public class UserWithPosts
    {
        public int Id { get; set; }
        [Required]
        [StringLength(30)]
        public string DisplayName { get; set; }
        [StringLength(255)]
        #nullable enable
        public string? ProfilePictureUrl { get; set; }
        #nullable disable
        public DateTime DateCreated { get; set; }
        public int? PackId { get; set; }
        public bool IsBanned { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
    }
}