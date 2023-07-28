using Howler.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Howler.Repositories
{
    public interface IUserRepository
    {
        public User GetById(int id);
        public User GetByEmail(string email);
        public void Add(User user);
        public void Update(User user);
        public User GetByFirebaseId(string firebaseId);
        public UserWithPosts GetByIdWithPosts(int id);
        public List<BarrenUser> GetByPackId(int packId);
    }
}