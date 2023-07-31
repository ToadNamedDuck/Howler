using System.Collections.Generic;

namespace Howler.Models
{
    public class OmniSearchResult
    {
        public List<Pack> Packs { get; set; }
        public List<Board> Boards { get; set; }
        public List<Post> Posts { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
