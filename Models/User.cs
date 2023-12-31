﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Howler.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string DisplayName { get; set; }
        [Required]
        [StringLength(128)]
        public string Email { get; set; }
        [StringLength(255)]
        #nullable enable
        public string? ProfilePictureUrl { get; set; }
        #nullable disable
        public DateTime DateCreated { get; set; }
        [Required]
        [StringLength(28, MinimumLength = 28)]
        public string FirebaseId { get; set; }
        public int? PackId { get; set; }
        public bool IsBanned { get; set; }
        public Pack Pack { get; set; }
    }
}
