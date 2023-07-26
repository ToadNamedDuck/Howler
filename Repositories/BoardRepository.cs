using Howler.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Streamish.Repositories;
using System;
using System.Collections.Generic;

namespace Howler.Repositories
{
    public class BoardRepository : BaseRepository
    {
        public BoardRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //Get All Boards (That aren't pack boards)/\
        //GetById /\
        //Search Boards ??? --Can add towards the end of the rest - could also do an exact name match, too, for checking duplicate names.
        //Add a board /\
        //Add pack Board method, that takes a pack as a parameter and generates it a board if the packboardid is null
        //Update Board
        //Delete a board. -- cannot delete a board that is a pack board. Deleting pack boards should happen from the pack controller, when a pack is deleted. Pack controller needs import this
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
                                        Where Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

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

        public void Add(Board board)
        {
            using(var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
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
            //Might be able to be ran before the insert, since classes are a reference type, we can change the pack's primaryBoardId before sending it to be added in pack repository!
            //Pack primaryBoardId should still be nullable in SQL, though, so if you delete a pack, it can delete the board first ;)
        {
            if(pack.PrimaryBoardId != null)
            {
                return;
            }

            //Just to mess around...
            Random rand = new();
            int defaultNameNumber = rand.Next(1, 7);
            //Done

            using(var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Insert into Board ([Name], Topic, Description, BoardOwnerId, IsPackBoard)
                                        OUTPUT INSERTED.ID
                                        Values (@name, @topic, @desc, @boid, @ipb)";

                    switch(defaultNameNumber)//Randomize the default board's name a bit because it is fun :)
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
