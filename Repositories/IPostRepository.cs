using Howler.Models;
using System.Collections.Generic;

namespace Howler.Repositories
{
    public interface IPostRepository
    {
        void Add(Post post);
        void Delete(int id);
        List<Post> GetAllPosts();
        Post GetById(int id);
        PostWithComments GetWithComments(int id);
        List<Post> Search(string q);
        void Update(Post post);
    }
}