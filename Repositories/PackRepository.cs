﻿using Howler.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Streamish.Repositories;
using System;
using System.Collections.Generic;

namespace Howler.Repositories
{
    public class PackRepository : BaseRepository, IPackRepository
    {
        public PackRepository(IConfiguration configuration) : base(configuration)
        {

        }

        //Get All Packs /\
        //GetById /\
        //Add pack /\
        //Edit pack /\
        //Delete Pack /\
        //Search pack //\\ - Exact matches work, and regular searches this hit the name and the description.
        //PackLeader should have a User obj without pack info on it. We can make a User model without a pack on it later. -- BarrenUser

        public List<Pack> GetAllPacks()
        {
            List<Pack> packs = new();

            using (var connection = Connection)
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select p.Id as PackId, p.Name, p.Description, p.PackLeaderId, p.PrimaryBoardId,
                                        u.Id as UserId, u.DisplayName, u.ProfilePictureUrl, u.DateCreated, u.PackId as userPackId, u.IsBanned
                                        from Pack p
                                        join [User] u
                                        on u.Id = p.PackLeaderId
                                        Order by p.Name asc";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        //to-do: put data from reader where it belongs. Also: I am thinking about removing the email field from the user object, because I cant forsee where
                        //it will be useful, and really it's probably a security flaw to be putting it around everywhere, especially if it's only used for login, and it won't
                        //be displayed on the website at all.

                        //I did add the BarrenUser Type, I'm going to test and see how it rolls out, and test slowly adding it into places and if I can just entirely remove email from the main
                        //User model. Barren User is still useful if a user would be attached to something like a comment, board, or in this case, a pack, so I intend on keeping that.
                        //BUT if I don't need like.... any of the extra stuff I removed, then it would be nice to not broadcast all of people's info over internet packets.

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

            using (var connection = Connection)
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select p.Id as PackId, p.Name, p.Description, p.PackLeaderId, p.PrimaryBoardId,
                                        u.Id as UserId, u.DisplayName, u.ProfilePictureUrl, u.DateCreated, u.PackId as userPackId, u.IsBanned
                                        from Pack p
                                        join [User] u
                                        on u.Id = p.PackLeaderId
                                        where p.Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
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

        public void Add(Pack pack)
        {
            using (var connection = Connection)
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Insert into Pack ([Name], Description, PackLeaderId, PrimaryBoardId)
                                        OUTPUT INSERTED.ID
                                        values (@name, @desc, @plid, @pbid)";
                    //We also want to set the pack leader's packId on his user to the newly created pack. lol
                    cmd.Parameters.AddWithValue("@name", pack.Name);
                    cmd.Parameters.AddWithValue("@desc", pack.Description);
                    cmd.Parameters.AddWithValue("@plid", pack.PackLeaderId);
                    if(pack.PrimaryBoardId == null)
                    {
                        cmd.Parameters.AddWithValue("@pbid", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@pbid", pack.PrimaryBoardId);
                    }

                    pack.Id = (int)cmd.ExecuteScalar();

                    //Now we set the user's PackId to the newly created Pack's Id.
                    cmd.Parameters.Clear();
                    cmd.CommandText = @"Update [User]
                                        Set PackId = @pid
                                        where Id = @uid";
                    cmd.Parameters.AddWithValue("@pid", pack.Id);
                    cmd.Parameters.AddWithValue("@uid", pack.PackLeaderId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Edit(Pack pack)
        {
            using (var connection = Connection)
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Update Pack
                                        SET [Name] = @name,
                                        Description = @desc,
                                        PackLeaderId = @plid,
                                        PrimaryBoardId = @pbid
                                        Where Id = @id";

                    cmd.Parameters.AddWithValue("@name", pack.Name);
                    cmd.Parameters.AddWithValue("@desc", pack.Description);
                    cmd.Parameters.AddWithValue("@plid", pack.PackLeaderId);
                    if (pack.PrimaryBoardId == null)
                    {
                        cmd.Parameters.AddWithValue("@pbid", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@pbid", pack.PrimaryBoardId);
                    }
                    cmd.Parameters.AddWithValue("@id", pack.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int packId)
        {
            using (var connection = Connection)
            {
                connection.Open();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Update [User]
                                        Set PackId = @nullValue
                                        Where PackId = @id;
                                        
                                        Delete from Pack where Id = @id;";

                    cmd.Parameters.AddWithValue("@id", packId);
                    cmd.Parameters.AddWithValue("@nullValue", DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Pack ExactSearch(string q)
        {
            Pack pack = null;
            using (var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select p.Id as PackId, p.Name, p.Description, p.PackLeaderId, p.PrimaryBoardId,
                                        u.Id as UserId, u.DisplayName, u.ProfilePictureUrl, u.DateCreated, u.PackId as userPackId, u.IsBanned
                                        from Pack p
                                        join [User] u
                                        on u.Id = p.PackLeaderId
                                        where [Name] LIKE @q";
                    cmd.Parameters.AddWithValue("@q", q);

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

        public List<Pack> Search(string q)
        {
            List<Pack> packs = new List<Pack>();
            using(var connection = Connection)
            {
                connection.Open();

                using(var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = @"Select p.Id as PackId, p.Name, p.Description, p.PackLeaderId, p.PrimaryBoardId,
                                        u.Id as UserId, u.DisplayName, u.ProfilePictureUrl, u.DateCreated, u.PackId as userPackId, u.IsBanned
                                        from Pack p
                                        join [User] u
                                        on u.Id = p.PackLeaderId
                                        where [Name] LIKE @q or Description LIKE @q
                                        Order by p.Name asc";
                    cmd.Parameters.AddWithValue("@q", $"%{q}%");

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            packs.Add(PackBuilder(reader));
                        }
                    }
                }
            }
            return packs;
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
