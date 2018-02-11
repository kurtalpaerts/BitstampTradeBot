using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace BitstampTradeBot.Data.Repositories
{
    public class SqlRepository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _ctx;

        public SqlRepository(DbContext context)
        {
            _ctx = context;
        }

        public void Add(T newEntity)
        {
            _ctx.Set<T>().Add(newEntity);
        }

        public void Remove(T newEntity)
        {
            _ctx.Set<T>().Remove(newEntity);
        }

        public void Save()
        {
            _ctx.SaveChanges();
        }

        public void Update(T entity)
        {
            DbEntityEntry entityEntry = _ctx.Entry(entity);
            if (entityEntry.State == EntityState.Detached)
            {
                _ctx.Set<T>().Attach(entity);
                entityEntry.State = EntityState.Modified;
            }
        }

        public IEnumerable<T> Where(Func<T, bool> predicate)
        {
            return _ctx.Set<T>().Where(predicate);
        }

        public List<T> ToList()
        {
            return _ctx.Set<T>().ToList();
        }
    }
}
