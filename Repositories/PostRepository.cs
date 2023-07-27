using Howler.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Streamish.Repositories;
using System.Collections.Generic;

namespace Howler.Repositories
{
    public class PostRepository : BaseRepository
    {
        public PostRepository(IConfiguration configuration) : base(configuration)
        {
        }

        //GetAllPosts -- mostly for the sake of having it. I can't imagine it would be necessary for anything at all. Doesn't return posts from a pack board. /\
        //GetById -- Front end should only really use GetWithComments endpoint. This is mostly for Getting the board Id off the post for authorization purposes. /\
                    //Also for I guess the CreatedAtAction. Will add it to the api for sake of completeness. The controller needs to use the board id to determine
                    //whether or not the post is on a pack board to determine if the user should be able to access the post or not. This only returns the post. It doesn't care.
        //Add /\
        //Update -- should only be editable by the person who wrote the post. /\
        //Delete -- Posts should be deletable by board owners and also by the member who wrote the post. /\
        //GetWithComments -- Needs a new model /\
        //Search - no need for exact search, like other repositories - I'm not going to enforce a name restriction on posts, in case a pack board has a post /\
                    //called "Do sheep taste good?" and someone makes a post named "Do sheep taste good?" in a public board. However, it will only return results
                    //from public boards. Search functionality for pack posts can be handled by react on the front end using state and array method filtering in js/jsx.
        //GetByBoardId -- List<Post> -- may not be necessary, since boards already get boards with posts on them. If necessary, I'll add this later.

        public List<Post> GetAllPosts()
        {
            List<Post> posts = new();

            using(var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select po.Id as postId, po.Title, po.Content, po.UserId as PostUserId, po.BoardId as PostBoardId, po.CreatedOn as PostDate,
                                                pu.Id as userId, pu.DisplayName, pu.DateCreated as userDate, pu.IsBanned, pu.ProfilePictureUrl, pu.PackId,
                                                bo.IsPackBoard

                                                from Post po
                                                join [User] pu on po.UserId = pu.Id
                                                join Board bo on po.BoardId = bo.Id";

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if(reader.GetBoolean(reader.GetOrdinal("IsPackBoard")) != true)//makes sure the pack board field isnt true, and then adds the post. doesnt add posts on pack board
                            {
                                posts.Add(PostBuilder(reader));
                            }
                        }
                    }
                }
            }
            return posts;
        }

        public Post GetById(int id)
        {
            Post post = null;

            using(var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select po.Id as postId, po.Title, po.Content, po.UserId as PostUserId, po.BoardId as PostBoardId, po.CreatedOn as PostDate,
                                                pu.Id as userId, pu.DisplayName, pu.DateCreated as userDate, pu.IsBanned, pu.ProfilePictureUrl, pu.PackId

                                                from Post po
                                                join [User] pu on po.UserId = pu.Id
                                                where po.Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            post = PostBuilder(reader);
                        }
                    }
                }
            }

            return post;
        }

        public void Add(Post post)
        {
            using(var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Insert into Post (Title, Content, UserId, BoardId, CreatedOn)
                                        OUTPUT INSERTED ID
                                        Values (@title, @content, @userId, @boardId, @createdOn)";

                    cmd.Parameters.AddWithValue("@title", post.Title);
                    cmd.Parameters.AddWithValue("@content", post.Content);
                    cmd.Parameters.AddWithValue("@userId", post.UserId);
                    cmd.Parameters.AddWithValue("@boardId", post.BoardId);
                    cmd.Parameters.AddWithValue("createdOn", post.CreatedOn);

                    post.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(Post post)
        {
            using(var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Post
                                        Set Title = @title,
                                        Content = @content
                                        Where Id = @id";

                    cmd.Parameters.AddWithValue("@title", post.Title);
                    cmd.Parameters.AddWithValue("@content", post.Content);
                    cmd.Parameters.AddWithValue("@id", post.Id);

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
                    cmd.CommandText = @"Delete from Post where Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public PostWithComments GetWithComments(int id)
        {
            PostWithComments post = null;

            using(var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select po.Id as postId, po.Title, po.Content, po.UserId as PostUserId, po.BoardId as PostBoardId, po.CreatedOn as PostDate,
                                                pu.Id as userId, pu.DisplayName, pu.DateCreated as userDate, pu.IsBanned, pu.ProfilePictureUrl, pu.PackId,

                                                co.Id as commentId, co.Content as commentContent, co.PostId as commentPostId, co.UserId as commentUserId,
                                                co.CreatedOn as commentDate,
                                                cu.Id as commentUserId, cu.DisplayName as commentUserDPN, cu.DateCreated as commentUserDate, cu.IsBanned as commentUIB,
                                                cu.ProfilePictureUrl as commentUPFP, cu.PackId as commentPackId

                                                from Post po
                                                join [User] pu on po.UserId = pu.Id
                                                Left Join Comment co on co.PostId = po.Id
                                                join [User] cu on co.UserId = cu.Id
                                                where po.Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if(post == null)
                            {
                                Post noCommentPost = PostBuilder(reader);
                                List<Comment> comments = new List<Comment>();
                                post = new()
                                {
                                    Id = noCommentPost.Id,
                                    Title = noCommentPost.Title,
                                    Content = noCommentPost.Content,
                                    UserId = noCommentPost.UserId,
                                    BoardId = noCommentPost.BoardId,
                                    CreatedOn = noCommentPost.CreatedOn,
                                    Comments = comments,
                                    User = noCommentPost.User
                                };
                            }

                            Comment comment = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("commentId")),
                                Content = reader.GetString(reader.GetOrdinal("commentContent")),
                                PostId = reader.GetInt32(reader.GetOrdinal("commentPostId")),
                                UserId = reader.GetInt32(reader.GetOrdinal("commentUserId")),
                                CreatedOn = reader.GetDateTime(reader.GetOrdinal("commentDate")),
                                User = new()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("commentUserId")),
                                    DisplayName = reader.GetString(reader.GetOrdinal("commentUserDPN")),
                                    DateCreated = reader.GetDateTime(reader.GetOrdinal("commentUserDate")),
                                    IsBanned = reader.GetBoolean(reader.GetOrdinal("commentUIB"))
                                }
                            };

                            if (reader.IsDBNull(reader.GetOrdinal("commentUPFP")))
                            {
                                comment.User.ProfilePictureUrl = null;
                            }
                            else
                            {
                                comment.User.ProfilePictureUrl = reader.GetString(reader.GetOrdinal("commentUPFP"));
                            }

                            if (reader.IsDBNull(reader.GetOrdinal("commentPackId")))
                            {
                                comment.User.ProfilePictureUrl = null;
                            }
                            else
                            {
                                comment.User.ProfilePictureUrl = reader.GetString(reader.GetOrdinal("commentPackId"));
                            }

                            post.Comments.Add(comment);
                        }
                    }
                }
            }
            return post;
        }

        public List<Post> Search(string q)//doesn't return posts from pack boards. :)
        {
            List<Post> posts = new();

            using(var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select po.Id as postId, po.Title, po.Content, po.UserId as PostUserId, po.BoardId as PostBoardId, po.CreatedOn as PostDate,
                                                pu.Id as userId, pu.DisplayName, pu.DateCreated as userDate, pu.IsBanned, pu.ProfilePictureUrl, pu.PackId,
                                                bo.IsPackBoard

                                                from Post po
                                                join [User] pu on po.UserId = pu.Id
                                                join Board bo on po.BoardId = bo.Id
                                                where Title like @q AND bo.IsPackBoard = 0
                                                OR Content like @q AND bo.IsPackBoard = 0";

                    cmd.Parameters.AddWithValue("@q", $"%{q}%");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            posts.Add(PostBuilder(reader));
                        }
                    }
                }
            }
            return posts;
        }

        private Post PostBuilder(SqlDataReader reader)
        {
            Post post = new()
            {
                Id = reader.GetInt32(reader.GetOrdinal("postId")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Content = reader.GetString(reader.GetOrdinal("Content")),
                UserId = reader.GetInt32(reader.GetOrdinal("PostUserId")),
                BoardId = reader.GetInt32(reader.GetOrdinal("PostBoardId")),
                CreatedOn = reader.GetDateTime(reader.GetOrdinal("PostDate")),
                User = new()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("userId")),
                    DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                    DateCreated = reader.GetDateTime(reader.GetOrdinal("userDate")),
                    IsBanned = reader.GetBoolean(reader.GetOrdinal("IsBanned"))
                }
            };

            if (reader.IsDBNull(reader.GetOrdinal("ProfilePictureUrl")))
            {
                post.User.ProfilePictureUrl = null;
            }
            else
            {
                post.User.ProfilePictureUrl = reader.GetString(reader.GetOrdinal("ProfilePictureUrl"));
            }

            if (reader.IsDBNull(reader.GetOrdinal("PackId")))
            {
                post.User.PackId = null;
            }
            else
            {
                post.User.PackId = reader.GetInt32(reader.GetOrdinal("PackId"));
            }

            return post;
        }
    }
}
