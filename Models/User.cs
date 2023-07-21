using System;
using System.ComponentModel.DataAnnotations;

namespace Howler.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [StringLength(30)]
        public string DisplayName { get; set; }
        [Required]
        [StringLength(128)]
        public string Email { get; set; }
        public DateTime DateCreated { get; set; }
        [Required]
        [StringLength(28, MinimumLength = 28)]
        public string FirebaseId { get; set; }
        public int? PackId { get; set; }
        public bool IsBanned { get; set; }
    }
}
