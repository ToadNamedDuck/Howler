namespace Howler.Models
{
    public class Board
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Topic { get; set; }
        public string Description { get; set; }
        public int BoardOwnerId { get; set; }
        public bool IsPackBoard { get; set; }
        public User BoardOwner { get; set; }
    }
}
