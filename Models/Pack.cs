using System.ComponentModel.DataAnnotations;

namespace Howler.Models
{
    public class Pack
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(255)]
        public string Description { get; set; }
        [Required]
        public int PackLeaderId { get; set; }
        public int? PrimaryBoardId { get; set; }
        public User PackOwner { get; set; }
        //public Board PackBoard { get; set; }
    }
}
