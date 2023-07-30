using Howler.Models;
using System.Collections.Generic;

namespace Howler.Repositories
{
    public interface ICommentRepository
    {
        void Add(Comment comment);
        void Delete(int id);
        Comment GetById(int id);
        List<Comment> Search(string q, bool latestFirst);
        void Update(Comment comment);
    }
}