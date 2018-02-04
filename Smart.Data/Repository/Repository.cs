using Smart.Core.Domain.Base;
using Smart.Core.Interface.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using Smart.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Smart.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly SmartContext _context;
        private DbSet<T> _entity;

        #region NoAsync

        public Repository(SmartContext context)
        {
            this._context = context;
            _entity = context.Set<T>();
            
        }

        public int Count()
        {
            return this._entity.Count();
        }

        public int Count(Expression<Func<T, bool>> where)
        {

            return this._entity.Count(where);
            
        }

        public void Delete(T entity)
        {
            
            this._entity.Remove(entity);
            this._context.SaveChanges();
        }

        public void Delete(Expression<Func<T, bool>> where)
        {
            this._entity.Where(where).ToList().ForEach(del => _context.Set<T>().Remove(del));
            this._context.SaveChanges();
        }

        public T Find(params object[] key)
        {
            return this._entity.Find(key);
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            return this._entity.Where(where).FirstOrDefault();
        }

        public IEnumerable<T> GetAll()
        {
            return this._entity.ToList();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> where)
        {
            return this._entity.Where(where).ToList();
        }

        public void Add(T entity)
        {
           
                if (entity == null)
                    throw new ArgumentNullException("entity");

                _entity.Add(entity);
                this._context.SaveChanges();
            
        }

        public IQueryable<T> Query()
        {
            return this._entity.AsNoTracking().AsQueryable();
        }

        public IQueryable<T> Query(Expression<Func<T, bool>> where)
        {
            return this._entity.AsNoTracking().Where(where).AsQueryable();
        }

        public void Update(T entity)
        {
            this._context.Entry(entity).State = EntityState.Modified;
            this._context.SaveChanges();

        }




        #endregion



        #region Sync


        public async Task<T> AddAsync(T entity)
        {
            await this._entity.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;

        }

        public async Task<T> UpdateAsync(T entity)
        {
            _context. Entry(entity).CurrentValues.SetValues(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<int> DeleteAsync(T entity)
        {
            this._entity.Remove(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(Expression<Func<T, bool>> where)
        {
            this._entity.Where(where).ToList().ForEach(del => _context.Set<T>().Remove(del));
            return await this._context.SaveChangesAsync();
        }

        public async Task<T> FindAsync(params object[] key)
        {
            return await this._entity.FindAsync(key);
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> where)
        {
            return await this._entity.Where(where).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await this._entity.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> where)
        {
            return await this._entity.Where(where).ToListAsync();
        }
        
        public Task<int> CountAsync()
        {
            return _entity.CountAsync();
        }

        public Task<int> CountAsync(Expression<Func<T, bool>> where)
        {
            return _entity.CountAsync(where);
        }

        #endregion
    }
}
