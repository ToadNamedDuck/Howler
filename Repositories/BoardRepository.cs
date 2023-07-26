using Microsoft.Extensions.Configuration;
using Streamish.Repositories;

namespace Howler.Repositories
{
    public class BoardRepository : BaseRepository
    {
        public BoardRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //Get All Boards (That aren't pack boards)
        //Search Boards ???
        //GetBoardByPackId
        //Add a board - creating a pack should *probably* also create a board that is set to a pack board, and probably shouldn't be deletable.
        //Update Board
        //Delete a board.
        //GetBoardWithPosts - BoardWithPosts
    }
}
