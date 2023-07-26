using Howler.Models;
using System.Collections.Generic;

namespace Howler.Repositories
{
    public interface IBoardRepository
    {
        void Add(Board board);
        void Delete(int id);
        void GeneratePackBoard(Pack pack);
        List<Board> GetAllBoards();
        Board GetById(int id);
        BoardWithPosts GetWithPosts(int id);
        void Update(Board board);
    }
}