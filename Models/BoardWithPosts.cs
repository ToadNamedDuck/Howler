using System.Collections.Generic;

namespace Howler.Models
{
    public class BoardWithPosts
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public int BoardOwnerId { get; set; }
        public bool IsPackBoard { get; set; }
        public BarrenUser BoardOwner { get; set; }
        public List<Post> Posts { get; set; } 
    }
}
