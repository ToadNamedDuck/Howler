using Howler.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Streamish.Repositories;
using System.Collections.Generic;

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

        public List<Board> GetAllBoards()//This does not, in fact, get all boards. But it gets all of the boards that AREN'T pack boards.
            //The reason for this is because pack boards are PRIVATE spaces. If you're not in the pack, you should have as little information
            //as possible about what goes on in their boards.
        {
            List<Board> boards = new List<Board>();
            using(var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select b.Id as bId, b.[Name], b.Topic, b.Description, b.BoardOwnerId, b.IsPackBoard,
                                        u.Id as UserId, u.DisplayName, u.ProfilePictureUrl, u.DateCreated, u.PackId as userPackId, u.IsBanned
                                        from Board b
                                        Join [User] u
                                        On b.BoardOwnerId = u.Id
                                        Where IsPackBoard = 0";
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            boards.Add(BoardBuilder(reader));
                        }
                    }
                }
            }
            return boards;
        }
        private Board BoardBuilder(SqlDataReader reader)
        {
            Board board = new()
            {
                Id = reader.GetInt32(reader.GetOrdinal("bId")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Topic = reader.GetString(reader.GetOrdinal("Topic")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                BoardOwnerId = reader.GetInt32(reader.GetOrdinal("BoardOwnerId")),
                IsPackBoard = reader.GetBoolean(reader.GetOrdinal("IsPackBoard")),
                BoardOwner = UserBuilder(reader)
            };
            return board;
        }
        private BarrenUser UserBuilder(SqlDataReader reader)
        {
            BarrenUser user = new()
            {
                Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                DateCreated = reader.GetDateTime(reader.GetOrdinal("DateCreated")),
                IsBanned = reader.GetBoolean(reader.GetOrdinal("IsBanned"))
            };
            if (!reader.IsDBNull(reader.GetOrdinal("userPackId")))
            {
                user.PackId = reader.GetInt32(reader.GetOrdinal("userPackId"));
            }
            else
            {
                user.PackId = null;
            }

            if (!reader.IsDBNull(reader.GetOrdinal("ProfilePictureUrl")))
            {
                user.ProfilePictureUrl = reader.GetString(reader.GetOrdinal("ProfilePictureUrl"));
            }
            else
            {
                user.ProfilePictureUrl = null;
            }

            return user;
        }
    }
}
