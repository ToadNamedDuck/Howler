﻿using Howler.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Streamish.Repositories;
using System.Collections.Generic;

namespace Howler.Repositories
{
    public class PackRepository : BaseRepository
    {
        public PackRepository(IConfiguration configuration) : base(configuration)
        {

        }

        //Get All Packs /\
        //GetById /\
        //Add pack
        //Edit pack
        //Delete Pack
        //Maybe get by owner Id
        //PackLeader should have a User obj without pack info on it. We can make a User model without a pack on it later. BarrenUser

        public List<Pack> GetAllPacks()
        {
            List<Pack> packs = new();

            using(var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select p.Id as PackId, p.Name, p.Description, p.PackLeaderId, p.PrimaryBoardId,
                                        u.Id as UserId, u.DisplayName, u.ProfilePictureUrl, u.DateCreated, u.PackId, u.IsBanned
                                        from Pack p
                                        join [User] u
                                        on u.Id = p.PackLeaderId";

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        //to-do: put data from reader where it belongs. Also: I am thinking about removing the email field from the user object, because I cant forsee where
                        //it will be useful, and really it's probably a security flaw to be putting it around everywhere, especially if it's only used for login, and it won't
                        //be displayed on the website at all.

                        while (reader.Read())
                        {
                            packs.Add(PackBuilder(reader));
                        }

                    }
                }
            }

            return packs;
        }

        public Pack GetById(int id)
        {
            Pack pack = null;

            using(var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select p.Id as PackId, p.Name, p.Description, p.PackLeaderId, p.PrimaryBoardId,
                                        u.Id as UserId, u.DisplayName, u.ProfilePictureUrl, u.DateCreated, u.PackId, u.IsBanned
                                        from Pack p
                                        join [User] u
                                        on u.Id = p.PackLeaderId
                                        where p.Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pack = PackBuilder(reader);
                        }
                    }
                }
            }
            return pack;
        }

        private Pack PackBuilder(SqlDataReader reader)
        {
            Pack pack = new()
            {
                Id = reader.GetInt32(reader.GetOrdinal("PackId")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                PackLeaderId = reader.GetInt32(reader.GetOrdinal("PackLeaderId")),
            };
            if (reader.IsDBNull(reader.GetOrdinal("PrimaryBoardId")))
            {
                pack.PrimaryBoardId = null;
            }
            else
            {
                pack.PrimaryBoardId = reader.GetInt32(reader.GetOrdinal("PrimaryBoardId"));
            }
            pack.PackLeader = UserBuilder(reader);

            return pack;
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