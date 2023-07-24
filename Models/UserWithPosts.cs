﻿using System;
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
        [Required]
        [StringLength(128)]
        public string Email { get; set; }
        [StringLength(255)]
        public string? ProfilePictureUrl { get; set; }

        public DateTime DateCreated { get; set; }
        [Required]
        [StringLength(28, MinimumLength = 28)]
        public string FirebaseId { get; set; }
        public int? PackId { get; set; }
        public bool IsBanned { get; set; }
        public List<Post> Posts { get; set; } = new List<Post>();
    }
}