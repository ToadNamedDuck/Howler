using Howler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Streamish.Repositories;
using System;
using System.Collections.Generic;

namespace Howler.Repositories
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration)
        {

        }

        //I don't think there's a use case for get all users/GetAll.
        //No deletion of users
        //GetById /\
        //GetByEmail /\ - might be totally unnecessary. rip
        //AddUser /\
        //EditUser /\
        //GetByFirebaseId /\
        //GetByIdWithPosts /\
        //GetByPackId - return List<User>
        //GetByIdWithComments ** stretch, add panel to profile
        //GetByIdWithPostsAndComments ** stretch, add panel to profile
        public User GetById(int id)
        {
            User user = null;
            using (var connection = Connection)
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select Id as UserId, DisplayName, Email, ProfilePictureUrl, DateCreated, FirebaseId, IsBanned, PackId
                                        from [User]
                                        where Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = UserBuilder(reader);
                        }
                    }
                }
            }
            return user;
        }

        public User GetByEmail(string email)
        {
            User user = null;
            using (var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select Id as UserId, DisplayName, Email, ProfilePictureUrl, DateCreated, FirebaseId, IsBanned, PackId
                                        from [User]
                                        where Email = @email";
                    cmd.Parameters.AddWithValue("@email", email);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            user = UserBuilder(reader);
                        }
                    }
                }
            }
            return user;
        }

        public void Add(User user)
        {
            using(var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Insert into [User] (DisplayName, Email, ProfilePictureUrl, DateCreated, FirebaseId, IsBanned, PackId)
                                        OUTPUT INSERTED.ID
                                        values (@dpn, @email, @pfp, @dc, @fbid, 0, @packId)";

                    cmd.Parameters.AddWithValue("@dpn", user.DisplayName);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                    cmd.Parameters.AddWithValue("@dc", DateTime.Now);
                    cmd.Parameters.AddWithValue("@fbid", user.FirebaseId);
                    cmd.Parameters.AddWithValue("@packId", DBNull.Value);
                    if(user.ProfilePictureUrl == null)
                    {
                        cmd.Parameters.AddWithValue("@pfp", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@pfp", user.ProfilePictureUrl);
                    }

                    user.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(User user)
        {
            using(var connection = Connection)
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Update [User]
                                        SET DisplayName = @displayName,
                                        ProfilePictureUrl = @pfp
                                        Where Id = @id";

                    cmd.Parameters.AddWithValue("@displayName", user.DisplayName);
                    if (!String.IsNullOrWhiteSpace(user.ProfilePictureUrl))
                    {
                        cmd.Parameters.AddWithValue("@pfp", user.ProfilePictureUrl);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@pfp", DBNull.Value);
                    }
                    cmd.Parameters.AddWithValue("id", user.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public User GetByFirebaseId(string firebaseId)
        {
            User user = null;
            using(var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select Id as UserId, DisplayName, Email, ProfilePictureUrl, DateCreated, FirebaseId, IsBanned, PackId
                                        from [User]
                                        where FirebaseId = @fbid";

                    cmd.Parameters.AddWithValue("@fbid", firebaseId);

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = UserBuilder(reader);
                        }
                    }
                }
            }
            return user;
        }

        public UserWithPosts GetByIdWithPosts (int id)
        {
            UserWithPosts user = null;

            using(var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select u.Id as UserId, u.DisplayName, u.Email, u.ProfilePictureUrl, u.DateCreated as UserDate, u.FirebaseId, u.IsBanned, u.PackId,
                                        p.Id as PostId, p.Title, p.Content, p.CreatedOn, p.UserId, p.BoardId,
                                        bo.IsPackBoard
                                        from [User] u
                                        left join Post p on p.UserId = u.Id
                                        join Board bo on p.BoardId = bo.Id
                                        where u.Id = @userId AND bo.IsPackBoard = 0";

                    cmd.Parameters.AddWithValue("@userId", id);

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                            while (reader.Read())
                            {
                                if(user == null)
                                {
                                    user = new UserWithPosts()
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                                        DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                                        Email = reader.GetString(reader.GetOrdinal("Email")),
                                        DateCreated = reader.GetDateTime(reader.GetOrdinal("UserDate")),
                                        FirebaseId = reader.GetString(reader.GetOrdinal("FirebaseId")),
                                        IsBanned = reader.GetBoolean(reader.GetOrdinal("IsBanned"))
                                    };
                                    if (!reader.IsDBNull(reader.GetOrdinal("PackId")))
                                    {
                                        user.PackId = reader.GetInt32(reader.GetOrdinal("PackId"));
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
                                };
                                Post post = new()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("PostId")),
                                    Title = reader.GetString(reader.GetOrdinal("Title")),
                                    Content = reader.GetString(reader.GetOrdinal("Content")),
                                    CreatedOn = reader.GetDateTime(reader.GetOrdinal("CreatedOn")),
                                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                                    BoardId = reader.GetInt32(reader.GetOrdinal("BoardId"))
                                };
                                user.Posts.Add(post);
                            }
                    }
                }
            }

            return user;
        }

        public List<BarrenUser> GetByPackId (int packId)//
        {
            List<BarrenUser> users = new List<BarrenUser>();

            using(var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select Id as UserId, DisplayName, Email, ProfilePictureUrl, DateCreated, FirebaseId, IsBanned, PackId from [User] where PackId = @id";
                    cmd.Parameters.AddWithValue("@id", packId);

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BarrenUser user = BarrenUserBuilder(reader);
                            users.Add(user);
                        }
                    }
                }
            }
            return users;
        }

        private User UserBuilder(SqlDataReader reader)
        {
            User user = new()
            {
                Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                DateCreated = reader.GetDateTime(reader.GetOrdinal("DateCreated")),
                FirebaseId = reader.GetString(reader.GetOrdinal("FirebaseId")),
                IsBanned = reader.GetBoolean(reader.GetOrdinal("IsBanned"))
            };
            if (!reader.IsDBNull(reader.GetOrdinal("PackId")))
            {
                user.PackId = reader.GetInt32(reader.GetOrdinal("PackId"));
            }
            else
            {
                user.PackId = null;
            }

            if (!reader.IsDBNull(reader.GetOrdinal("ProfilePictureUrl"))){
                user.ProfilePictureUrl = reader.GetString(reader.GetOrdinal("ProfilePictureUrl"));
            }
            else
            {
                user.ProfilePictureUrl = null;
            }

            return user;
        }

        private BarrenUser BarrenUserBuilder(SqlDataReader reader) //Wanting to test to see how much I can make a barren user. Ideally, the only one that needs to be a real user is firebaseId.
        {
            BarrenUser user = new()
            {
                Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                DateCreated = reader.GetDateTime(reader.GetOrdinal("DateCreated")),
                IsBanned = reader.GetBoolean(reader.GetOrdinal("IsBanned"))
            };
            if (!reader.IsDBNull(reader.GetOrdinal("PackId")))
            {
                user.PackId = reader.GetInt32(reader.GetOrdinal("PackId"));
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
