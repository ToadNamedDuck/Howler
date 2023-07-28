using Howler.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Streamish.Repositories;
using System.ComponentModel;

namespace Howler.Repositories
{
    public class CommentRepository : BaseRepository
    {
        public CommentRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //Honestly, I think since posts have an option to pull a post with comments, I think all this needs are:
        //GetById /\
        //Add /\
        //Update /\
        //Delete
        //Search

        public Comment GetById(int id) //do not make an endpoint for this, use only for validation
        {
            Comment comment = null;
            using(var connection = Connection)
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"Select co.Id as CommentId, co.UserId as commentUserId, co.PostId, co.Content, co.CreatedOn,
                                                    u.Id as cUserId, u.DisplayName, u.DateCreated, u.IsBanned, u.ProfilePictureUrl, u.PackId
                                                    from Comment co
                                                    join [User] u on co.UserId = u.Id
                                                    where co.Id = @id";

                    command.Parameters.AddWithValue("@id", id);

                    using(SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            comment = CommentBuilder(reader);
                        }
                    }
                }
            }
            return comment;
        }

        public void Add(Comment comment)
        {
            using(var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Insert into Comment (UserId, PostId, Content, CreatedOn)
                                        OUTPUT INSERTED.ID
                                        VALUES (@uid, @pid, @content, @co)";

                    cmd.Parameters.AddWithValue("@uid", comment.UserId);
                    cmd.Parameters.AddWithValue("@pid", comment.PostId);
                    cmd.Parameters.AddWithValue("@content", comment.Content);
                    cmd.Parameters.AddWithValue("@co", comment.CreatedOn);

                    comment.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(Comment comment)
        {
            using(var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Update Comment
                                        SET Content = @content
                                        Where Id = @id";
                    cmd.Parameters.AddWithValue("@content", comment.Content);
                    cmd.Parameters.AddWithValue("@id", comment.Id);


                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using(var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Delete from Comment where Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private Comment CommentBuilder(SqlDataReader reader)
        {
            Comment comment = new()
            {
                Id = reader.GetInt32(reader.GetOrdinal("CommentId")),
                PostId = reader.GetInt32(reader.GetOrdinal("PostId")),
                UserId = reader.GetInt32(reader.GetOrdinal("commentUserId")),
                Content = reader.GetString(reader.GetOrdinal("Content")),
                CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                User = new()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("cUserId")),
                    DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                    DateCreated = reader.GetDateTime(reader.GetOrdinal("DateCreated")),
                    IsBanned = reader.GetBoolean(reader.GetOrdinal("IsBanned"))
                }
            };

            if (reader.IsDBNull(reader.GetOrdinal("ProfilePictureUrl")))
            {
                comment.User.ProfilePictureUrl = null;
            }
            else
            {
                comment.User.ProfilePictureUrl = reader.GetString(reader.GetOrdinal("ProfilePictureUrl"));
            }

            if (reader.IsDBNull(reader.GetOrdinal("PackId")))
            {
                comment.User.PackId = null;
            }
            else
            {
                comment.User.PackId = reader.GetInt32(reader.GetOrdinal("PackId"));
            }

            return comment;
        }

    }
}
