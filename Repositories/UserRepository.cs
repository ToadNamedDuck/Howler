using Howler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Streamish.Repositories;
using System;

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
        //GetByEmail /\
        //AddUser
        //EditUser
        //GetByFirebaseId
        //GetByIdWithPosts
        //GetByPackId - return List<User>
        //GetByIdWithComments ** stretch, add panel to profile
        //GetByIdWithPostsAndComments ** stretch, add panel to profile
        //Update these to include their profile picture url pls :)
        public User GetById(int id)
        {
            User user = null;
            using (var connection = Connection)
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select Id, DisplayName, Email, ProfilePictureUrl, DateCreated, FirebaseId, IsBanned, PackId
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
                    cmd.CommandText = @"Select Id, DisplayName, Email, ProfilePictureUrl, DateCreated, FirebaseId, IsBanned, PackId
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

        private User UserBuilder(SqlDataReader reader)
        {
            User user = new()
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
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
    }
}
