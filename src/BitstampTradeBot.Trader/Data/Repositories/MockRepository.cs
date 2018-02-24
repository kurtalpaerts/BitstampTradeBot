using System;
using System.Collections.Generic;
using System.Linq;

namespace BitstampTradeBot.Trader.Data.Repositories
{
    public class MockRepository<T> : IRepository<T> where T : class
    {
        private List<T> _items = new List<T>();

        public void Add(T entity)
        {
            _items.Add(entity);
        }

        public void Remove(T entity)
        {
            _items.Remove(entity);
        }

        public void Save() { }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Where(Func<T, bool> predicate)
        {
            return _items.Where(predicate);
        }

        public T First(Func<T, bool> predicate)
        {
            return _items.First(predicate);
        }

        public List<T> ToList()
        {
            return _items;
        }
    }
}
