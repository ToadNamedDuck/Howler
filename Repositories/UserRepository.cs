using Howler.Models;
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
        //GetById
        //GetByEmail
        //AddUser
        //EditUser
        //GetByFirebaseId
        //GetByIdWithPosts
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
                    cmd.CommandText = @"Select Id, DisplayName, Email, DateCreated, FirebaseId, IsBanned, PackId from [User] where Id = @id";
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
            return user;
        }
    }
}
