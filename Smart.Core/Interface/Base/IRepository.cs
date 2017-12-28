using Smart.Core.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Smart.Core.Interface.Base
{

    public interface IRepository<T> where T : BaseEntity
    {
        #region SyncMethods

        /// <summary>
        /// Add Entity to Framework
        /// </summary>
        /// <param name="entity">Generic Entity</param>
        void Add(T entity);
      


        /// <summary>
        /// Marks an entity as modified
        /// </summary>
        /// <param name="entity">Generic Entity</param>
        void Update(T entity);



        /// <summary>
        /// Marks an entity to be removed
        /// </summary>
        /// <param name="entity">Generic Entity</param>
        void Delete(T entity);
     



        /// <summary>
        /// Marks each entity do removed
        /// </summary>
        /// <param name="where">Lambda Expression</param>
        void Delete(Expression<Func<T, bool>> where);
      

        /// <summary>
        /// Find Entity by Key
        /// </summary>
        /// <param name="key">Array of Primary Key</param>
        /// <returns>Return single object</returns>
        T Find(params object[] key);
      

        /// <summary>
        ///  Get an entity using delegate
        /// </summary>
        /// <param name="where">Lambda Expression</param>
        /// <returns>Return single object</returns>
        T Get(Expression<Func<T, bool>> where);
       


        /// <summary>
        /// Gets all entities of type T
        /// </summary>
        /// <returns>Return IEnumerable fo Entity (List) </returns>
        IEnumerable<T> GetAll();
      
        /// <summary>
        /// Gets entities using delegate
        /// </summary>
        /// <param name="where">Lambda Expression</param>
        /// <returns>Return IEnumerable fo Entity (List) </returns>
        IEnumerable<T> GetAll(Expression<Func<T, bool>> where);
       


        /// <summary>
        /// Get Query Expression for Entity
        /// </summary>
        /// <returns>Return query expression for entity</returns>
        IQueryable<T> Query();


        /// <summary>
        /// Get Query Expression for Entity using delegate
        /// </summary>
        /// <param name="where">Lambda Expression</param>
        /// <returns>Return query expression for entity</returns>
        IQueryable<T> Query(Expression<Func<T, bool>> where);


        /// <summary>
        /// Get Count of Entity
        /// </summary>
        /// <returns>Number of records this entity</returns>
        int Count();
       
        /// <summary>
        /// Get Count of Entity using delegate
        /// </summary>
        /// <param name="where">Lambda Expression</param>
        /// <returns>Number of records this entity</returns>
        int Count(Expression<Func<T, bool>> where);
#endregion

        #region AsyncMethods
            Task<int> CountAsync(Expression<Func<T, bool>> where);
            Task<int> CountAsync();
            Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> where);
            Task<IEnumerable<T>> GetAllAsync();
            Task<T> GetAsync(Expression<Func<T, bool>> where);
            Task<T> FindAsync(params object[] key);
            Task<int> DeleteAsync(Expression<Func<T, bool>> where);
            Task<int> DeleteAsync(T entity);
            Task<T> AddAsync(T entity);
            Task<T> UpdateAsync(T entity);

         
        #endregion
    }
}
