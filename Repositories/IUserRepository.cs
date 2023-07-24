using Howler.Models;

namespace Howler.Repositories
{
    public interface IUserRepository
    {
        public User GetById(int id);
        public User GetByEmail(string email);
        public void Add(User user);
        public void Update(User user);
        public User GetByFirebaseId(string firebaseId);
    }
}