using Smart.Core.Domain.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Smart.Core.Interface.Base;
using Smart.Data.Context;
using Smart.Services.Interfaces;
using System.Threading.Tasks;
using AutoMapper;
using System.Reflection;
using Smart.Core.Domain.Business;
using Microsoft.Extensions.Logging;

namespace Smart.Services.Entity
{
    public class Services<T> : IServices<T> where T : BaseEntity
    {
        private IRepository<T> _repository;
        private SmartContext _context;
        private IUser _currentUser;
        private string _currentUserId;
        private string _currentUserEmail;

        private string _businessEntityId;

        private Expression<Func<T, bool>> _expCurrentBusinessEntityId;
        private readonly IMapper _mapper;
        private readonly List<String> _AllBusinessEntityId;
        private readonly ILogger _logger;



        public Services(SmartContext context, IRepository<T> repository, IUser currentUser, IMapper mapper, ILogger<Services<T>> logger)
        {
            this._repository = repository;
            this._context = context;
            this._currentUser = currentUser;
            this._currentUserId = _currentUser.Id();
            this._currentUserEmail = _currentUser.Email();
            this._expCurrentBusinessEntityId = (a => a.BusinessEntityId == _currentUser.GetCurrentBusinessEntityId());
            this._mapper = mapper;
            this._logger = logger;
            this._businessEntityId = _currentUser.GetCurrentBusinessEntityId();
            this._AllBusinessEntityId = _currentUser.GetAllBusinessEntityId(_currentUser.Id());
            //this._ListEntity = this._context.UserBusinessEntity.Where(a => a.UserId == _currentUser.Id()).Select(a => a.BusinessEntityId).ToList();



        }


        public Expression<Func<T, bool>> InjectCurrentBusiness()
        {
            return p => (p.BusinessEntityId == _businessEntityId);

            //if (ListEntity == null)
            //{
            //    return p => (p.BusinessEntityId == businessEntityId);
            //}
            //else
            //{
            //    return p => ListEntity.Contains(p.BusinessEntityId);
            //}
        }


        public int Count()
        {
            return this._repository.Query().Where(InjectCurrentBusiness()).Count();

        }

        public int Count(Expression<Func<T, bool>> where)
        {
            return this._repository.Query().Where(where).Where(InjectCurrentBusiness()).Count();
        }



        public T Find(params object[] key)
        {
            var data = this._repository.Find(key);

            if (data != null)
            {
                if (data.BusinessEntityId != _businessEntityId)
                {
                    data = null;
                }
            }

            //return _mapper.Map<T>(_repository.Find(key));
            return data;
        }

        public T Get(Expression<Func<T, bool>> where)
        {
            return this._repository.Query(where).Where(InjectCurrentBusiness()).FirstOrDefault();
        }

        public T Get()
        {
            return this._repository.Query(InjectCurrentBusiness()).FirstOrDefault();
        }

        public IEnumerable<T> GetAll()
        {
            return this._repository.GetAll(InjectCurrentBusiness());
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> where)
        {
            return this._repository.Query(where).Where(InjectCurrentBusiness()).ToList();
        }

        public IQueryable<T> Query()
        {
            return this._repository.Query().Where(InjectCurrentBusiness());
        }
        public IQueryable<T> Query(Expression<Func<T, bool>> where)
        {
            return this._repository.Query(where).Where(InjectCurrentBusiness());
        }

        public void Add(T entity)
        {
            entity.BusinessEntityId = this._businessEntityId;
            //try
            //{
            //    typeof(T).GetProperty("CreateDate").SetValue(entity, System.DateTime.UtcNow);
            //}
            //catch (Exception)
            //{
            //    ;
            //}

            //try
            //{
            //    typeof(T).GetProperty("ModifiedDate").SetValue(entity, System.DateTime.UtcNow);
            //}
            //catch (Exception)
            //{
            //    ;
            //}

            this._repository.Add(entity);

        }



        public void Update(T entity)
        {
            entity.BusinessEntityId = this._businessEntityId;
            try
            {
                typeof(T).GetProperty("ModifiedDate").SetValue(entity, System.DateTime.UtcNow);
            }
            catch (Exception)
            {
                ;
            }

            this._repository.Update(entity);

        }


        public void Delete(T entity)
        {
            entity.BusinessEntityId = this._businessEntityId;
            this._repository.Delete(entity);
        }

        public void Delete(Expression<Func<T, bool>> where)
        {
            var _del = this._repository
                .Query(where)
                .Where(InjectCurrentBusiness())
                .ToList();
            _del.ForEach(a => this._repository.Delete(a));
        }







        public async Task<int> CountAsync(Expression<Func<T, bool>> where)
        {
            return await Task.Run(() =>
            {
                return _repository.Query(where).Where(a => a.BusinessEntityId == _currentUserId).Count();
            });

        }

        public async Task<int> CountAsync()
        {
            return await Task.Run(() =>
            {
                return _repository.CountAsync(a => a.BusinessEntityId == _currentUserId);
            });
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> where)
        {
            return await Task.Run(() =>
            {
                return GetAll(where);
            });
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync(InjectCurrentBusiness());
        }

        public async Task<T> GetAsync()
        {
            return await Task.Run(() =>
            {
                return Get();
            });




        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> where)
        {
            return await Task.Run(() =>
            {
                return Get(where);
            });
        }

        public async Task<T> FindAsync(params object[] key)
        {
            return await Task.Run(() =>
            {
                return Find(key);
            });
        }

        public async Task<int> DeleteAsync(Expression<Func<T, bool>> where)
        {
            return await Task.Run(() =>
            {
                Delete(where);
                return 0;
            });
        }

        public async Task<int> DeleteAsync(T entity)
        {
            return await Task.Run(() =>
            {
                Delete(entity);
                return 0;
            });
        }

        public async Task<T> AddAsync(T entity)
        {
            entity.BusinessEntityId = _currentUserId;
            return await _repository.AddAsync(entity);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            entity.BusinessEntityId = _currentUserId;
            return await _repository.UpdateAsync(entity);
        }

        public async Task<IQueryable<T>> QueryAsync()
        {
            return await Task.Run(() =>
            {
                return Query();
            });
        }

        public async Task<IQueryable<T>> QueryAsync(Expression<Func<T, bool>> where)
        {
            return await Task.Run(() =>
            {
                return Query(where);
            });
        }
    }


    

        public class LoggingEvents
        {
            public const int GENERATE_ITEMS = 1000;
            public const int LIST_ITEMS = 1001;
            public const int GET_ITEM = 1002;
            public const int INSERT_ITEM = 1003;
            public const int UPDATE_ITEM = 1004;
            public const int DELETE_ITEM = 1005;

            public const int GET_ITEM_NOTFOUND = 4000;
            public const int UPDATE_ITEM_NOTFOUND = 4001;
        }


}
