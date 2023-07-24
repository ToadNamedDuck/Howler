using Howler.Models;
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

        //Get All Packs
        //GetById
        //Add pack
        //Edit pack
        //Delete Pack
        //Maybe get by owner Id
        //User repo already has Get Users By Pack Id, but we could do something where we attach a list of users in the Pack object.

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
                    }
                }
            }

            return packs;
        }
    }
}
