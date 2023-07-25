using Howler.Models;
using System.Collections.Generic;

namespace Howler.Repositories
{
    public interface IPackRepository
    {
        void Add(Pack pack);
        void Delete(int packId);
        void Edit(Pack pack);
        List<Pack> GetAllPacks();
        Pack GetById(int id);
        Pack ExactSearch(string q);
        List<Pack> Search(string q);
    }
}