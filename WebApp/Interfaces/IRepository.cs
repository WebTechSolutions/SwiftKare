using System.Collections.Generic;
using System.Linq;

namespace WebApp.Interfaces
{
    public interface IRepository<T> where T : new()
    {
        IQueryable<T> GetList();
        T GetById(long id);
        T Put(long id, T t);
        T Add(T t);
        void Delete(int id);
        T Find(object id);
        bool Exists(object id);
    }
}
