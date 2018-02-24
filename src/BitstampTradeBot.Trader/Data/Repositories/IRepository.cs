using System;
using System.Collections.Generic;

namespace BitstampTradeBot.Trader.Data.Repositories
{
    public interface IRepository<T>
    {
        void Add(T newEntity);
        void Remove(T entity);
        void Save();
        void Update(T entity);
        IEnumerable<T> Where(Func<T, bool> predicate);
        T First(Func<T, bool> predicate);
        List<T> ToList();
    }
}