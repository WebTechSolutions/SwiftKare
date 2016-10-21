using System.Collections.Generic;

namespace RestAPIs.Interfaces
{
    public interface IRepository<T> where T : new()
    {
        string ConnectionString { get; set; }
        List<T> GetList();
        T Find(object id);
        bool Exists(object id);
        bool Delete(int id);
        bool Save(T t);
    }
}