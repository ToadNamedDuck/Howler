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
        [StringLength(255)]
        public string? ProfilePictureUrl { get; set; }

        public DateTime DateCreated { get; set; }
        [Required]
        [StringLength(28, MinimumLength = 28)]
        public string FirebaseId { get; set; }
        public int? PackId { get; set; }
        public bool IsBanned { get; set; }

        //Probably should add a pack object to the user, so you can get the name of the pack off of user.Pack.Name or something similar.
        public Pack Pack { get; set; }
    }
}
