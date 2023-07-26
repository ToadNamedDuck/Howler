using Howler.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Streamish.Repositories;
using System;
using System.Collections.Generic;

namespace Howler.Repositories
{
    public class BoardRepository : BaseRepository, IBoardRepository
    {
        public BoardRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //Get All Boards (That aren't pack boards)/\
        //GetById /\
        //Search Boards ??? --Can add towards the end of the rest - could also do an exact name match, too, for checking duplicate names. / -Need to add regular search, besides exact name search. lol
        //Add a board /\
        //GeneratePackBoard, that takes a pack as a parameter and generates it a board if the packboardid is null /\ - utilized by PackController
        //Update Board /\
        //Delete a board. -- cannot delete a board that is a pack board. Deleting pack boards should happen from the pack controller, when a pack is deleted. Pack controller needs import this /\
        //GetBoardWithPosts - BoardWithPosts /\

        public List<Board> GetAllBoards()//This does not, in fact, get all boards. But it gets all of the boards that AREN'T pack boards.
                                         //The reason for this is because pack boards are PRIVATE spaces. If you're not in the pack, you should have as little information
                                         //as possible about what goes on in their boards.
        {
            List<Board> boards = new List<Board>();
            using (var connection = Connection)
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select b.Id as bId, b.[Name], b.Topic, b.Description, b.BoardOwnerId, b.IsPackBoard,
                                        u.Id as UserId, u.DisplayName, u.ProfilePictureUrl, u.DateCreated, u.PackId as userPackId, u.IsBanned
                                        from Board b
                                        Join [User] u
                                        On b.BoardOwnerId = u.Id
                                        Where IsPackBoard = 0";
                    using (SqlDataReader reader = cmd.ExecuteReader())
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

        public Board GetById(int id)
        //I kind of don't want to make a whole other method for getting pack boards separately, but I *could*. I don't think it is worth it at the moment.
        //Maybe if pack boards were their own table, and those endpoints were only hit from the pack's page. Idk.
        //Perhaps that can be a stretch goal. But not right now. In the controller though, if it is a pack board, it makes sense to check the user's pack, and then pull the pack (if it's not null)
        //and compare the pack's boardId to the requested id. If the packBoardId DOESN'T match, give a forbidden.
        //If IsPackBoard = true and the user's pack is null, we can short circuit right there.
        //If IsPackBoard = true, and the User is in a pack, pull the user's pack, and check the boardId in the pack (which should be attached to the user obj.)
        //If those don't match, forbidden, if they do match, have a 200 Ok
        {
            Board board = null;

            using (var connection = Connection)
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select b.Id as bId, b.[Name], b.Topic, b.Description, b.BoardOwnerId, b.IsPackBoard,
                                        u.Id as UserId, u.DisplayName, u.ProfilePictureUrl, u.DateCreated, u.PackId as userPackId, u.IsBanned
                                        from Board b
                                        Join [User] u
                                        On b.BoardOwnerId = u.Id
                                        Where b.Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            board = BoardBuilder(reader);
                        }
                    }
                }
            }
            return board;
        }

        public void Add(Board board)
        {
            using (var connection = Connection)
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Insert into Board ([Name], Topic, Description, BoardOwnerId, IsPackBoard)
                                        OUTPUT INSERTED.ID
                                        Values (@name, @topic, @desc, @boid, @ipb)";
                    cmd.Parameters.AddWithValue("@name", board.Name);
                    cmd.Parameters.AddWithValue("@topic", board.Topic);
                    cmd.Parameters.AddWithValue("@desc", board.Description);
                    cmd.Parameters.AddWithValue("@boid", board.BoardOwnerId);
                    cmd.Parameters.AddWithValue("@ipb", board.IsPackBoard);

                    board.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void GeneratePackBoard(Pack pack)
        //Might be able to be ran before the pack is inserted from the pack repository, since classes are a reference type,
        //we can change the pack's primaryBoardId before sending it to be added in pack repository!
        //This means the pack controller needs a copy of this repo so that it can call this. (and also the delete if a pack is deleted :3)
        //Pack primaryBoardId should still be nullable in SQL, though, so if you delete a pack, it can delete the board first ;)
        {
            if (pack.PrimaryBoardId != null)
            {
                return;
            }

            //Just to mess around...
            Random rand = new();
            int defaultNameNumber = rand.Next(1, 7);
            //Done

            using (var connection = Connection)
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Insert into Board ([Name], Topic, Description, BoardOwnerId, IsPackBoard)
                                        OUTPUT INSERTED.ID
                                        Values (@name, @topic, @desc, @boid, @ipb)";

                    switch (defaultNameNumber)//Randomize the default board's name a bit because it is fun :)
                    {
                        case 1:
                            {
                                cmd.Parameters.AddWithValue("@name", $"{pack.Name}'s Board");
                                break;
                            }
                        case 2:
                            {
                                cmd.Parameters.AddWithValue("@name", $"{pack.Name}'s Cave");
                                break;
                            }
                        case 3:
                            {
                                cmd.Parameters.AddWithValue("@name", $"{pack.Name}'s Den");
                                break;
                            }
                        case 4:
                            {
                                cmd.Parameters.AddWithValue("@name", $"{pack.Name}'s Hunting Grounds");
                                break;
                            }
                        case 5:
                            {
                                cmd.Parameters.AddWithValue("@name", $"City of {pack.Name}");
                                break;
                            }
                        case 6:
                            {
                                cmd.Parameters.AddWithValue("@name", $"{pack.Name}'s Meeting Spot");
                                break;
                            }
                    }

                    cmd.Parameters.AddWithValue("@topic", "General discussion for all things related to the pack.");
                    cmd.Parameters.AddWithValue("@desc", "You should edit me and put a fitting description of the expectations you have for your pack discussions! You could put a few basic rules, or define goals for your pack! Really, you could put anything, if you wanted to. However, the maximum length is this.");
                    cmd.Parameters.AddWithValue("@boid", pack.PackLeaderId);
                    cmd.Parameters.AddWithValue("@ipb", 1);

                    pack.PrimaryBoardId = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(Board board)
        {
            //IsPackBoard not changeable, neither is Id
            using (var connection = Connection)
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Update Board
                                        Set Name = @name,
                                        Topic = @topic,
                                        Description = @desc,
                                        BoardOwnerId = @boid
                                        Where Id = @id";

                    cmd.Parameters.AddWithValue("@id", board.Id);
                    cmd.Parameters.AddWithValue("@name", board.Name);
                    cmd.Parameters.AddWithValue("@topic", board.Topic);
                    cmd.Parameters.AddWithValue("@desc", board.Description);
                    cmd.Parameters.AddWithValue("@boid", board.BoardOwnerId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var connection = Connection)
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Delete from Board where Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public BoardWithPosts GetWithPosts(int id)
        {
            BoardWithPosts board = null;
            using (var connection = Connection)
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"select b.Id as bId, b.[Name], b.Topic, b.Description, b.BoardOwnerId, b.IsPackBoard,
                                        u.Id as UserId, u.DisplayName, u.ProfilePictureUrl, u.DateCreated, u.PackId as userPackId, u.IsBanned,

                                        p.Id as pId, p.Title, p.Content, p.UserId as PostUserId, p.BoardId, p.CreatedOn,
                                        pu.Id as PostUserId, pu.DisplayName as PostUserDisplayName, pu.ProfilePictureUrl as PostUserPfp, pu.DateCreated as PostUserDate, pu.PackId as PostUserPackId, pu.IsBanned as PostUserIsBanned
                                        
                                        from Board b
                                        join [User] u on b.BoardOwnerId = u.Id
                                        left join Post p on p.BoardId = b.Id
                                        join [User] pu on p.UserId = pu.Id

                                        Where b.Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (board == null) //If there's data, then there's a board, so if board = null, then we need to intialize it.
                            {
                                board = new()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("bId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    Topic = reader.GetString(reader.GetOrdinal("Topic")),
                                    Description = reader.GetString(reader.GetOrdinal("Description")),
                                    BoardOwnerId = reader.GetInt32(reader.GetOrdinal("BoardOwnerId")),
                                    IsPackBoard = reader.GetBoolean(reader.GetOrdinal("IsPackBoard")),
                                    BoardOwner = UserBuilder(reader),
                                    Posts = new List<Post>()
                                };
                            }

                            Post post = new()//After board is initialized, we add posts to the board's post list. This runs every cycle.
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("pId")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Content = reader.GetString(reader.GetOrdinal("Content")),
                                UserId = reader.GetInt32(reader.GetOrdinal("PostUserId")),
                                BoardId = reader.GetInt32(reader.GetOrdinal("BoardId")),
                                CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                                User = new()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("PostUserId")),
                                    DisplayName = reader.GetString(reader.GetOrdinal("PostUserDisplayName")),
                                    DateCreated = reader.GetDateTime(reader.GetOrdinal("PostUserDate")),
                                    IsBanned = reader.GetBoolean(reader.GetOrdinal("PostUserIsBanned"))

                                }
                            };

                            if (reader.IsDBNull(reader.GetOrdinal("PostUserPfp")))
                            {
                                post.User.ProfilePictureUrl = null;
                            }
                            else
                            {
                                post.User.ProfilePictureUrl = reader.GetString(reader.GetOrdinal("PostUserPfp"));
                            }

                            if (reader.IsDBNull(reader.GetOrdinal("PostUserPackId")))
                            {
                                post.User.PackId = null;
                            }
                            else
                            {
                                post.User.PackId = reader.GetInt32(reader.GetOrdinal("PostUserPackId"));
                            }

                            board.Posts.Add(post);
                        }
                    }
                }
            }
            return board;
        }

        public Board ExactSearch(string q)
        {
            Board board = null;

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
                                        Where b.Name LIKE @q";
                    cmd.Parameters.AddWithValue("@q", q);

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            board = BoardBuilder(reader);
                        }
                    }
                }
            }

            return board;
        }

        public List<Board> Search(string q)
        {
            List<Board> boards = new();

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
                                        Where b.Name LIKE @q OR b.Topic LIKE @q OR b.Description LIKE @q";

                    cmd.Parameters.AddWithValue("@q", $"%{q}%");

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Board board = BoardBuilder(reader);
                            if(!board.IsPackBoard)//Pack boards can't be searched, even by members. Must be viewed from pack page link. lol
                            {
                                boards.Add(board);
                            }
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
