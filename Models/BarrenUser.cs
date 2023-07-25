using System;
using System.ComponentModel.DataAnnotations;

namespace Howler.Models
{
    public class BarrenUser
    {
        public int Id { get; set; }
        [Required]
        [StringLength(30)]
        public string DisplayName { get; set; }
        [StringLength(255)]
        public string? ProfilePictureUrl { get; set; }
        public DateTime DateCreated { get; set; }
        public int? PackId { get; set; }
        public bool IsBanned { get; set; }
    }
}