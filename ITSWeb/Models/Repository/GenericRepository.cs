using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using ITSWeb.Interface;

namespace ITSWeb.Models.Repository
{
    /// <summary>
    /// 實作Entity CRUD操作
    /// </summary>
    /// <typeparam name="TEntity">EntityType</typeparam>
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// DbContext
        /// </summary>
        private readonly DbContext _dataContext;

        /// <summary>
        /// disposed
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// 依據IDbContextFactory實作對象(DbContext)
        /// </summary>
        /// <param name="factory"></param>
        public GenericRepository(IDbContextFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException($"DbContextFactory is null.");
            }
            this._dataContext = factory.GetDbContext();
            this._dataContext.Configuration.LazyLoadingEnabled = false;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">EntityType</param>
        /// <returns>新增結果</returns>
        public bool Add(TEntity entity)
        {
            bool success = true;

            using (var dbContextTransaction = this._dataContext.Database.BeginTransaction())
            {
                try
                {
                    this._dataContext.Set<TEntity>().Add(entity);
                    var status = this._dataContext.SaveChanges();
                    dbContextTransaction.Commit();

                    if (status <= 0)
                    {
                        dbContextTransaction.Commit();
                        success = false;
                    }
                }
                catch (EntitySqlException ex)
                {
                    dbContextTransaction.Rollback();
                    success = false;
                    throw new Exception(ex.Message, ex);
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message, ex);
                }
            }
            return success;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">EntityType</param>
        /// <param name="useTransaction"></param>
        /// <returns></returns>
        public bool Add(IEnumerable<TEntity> entity, bool useTransaction = false)
        {
            bool success = true;

            if (useTransaction == false)
            {
                try
                {
                    this._dataContext.Set<TEntity>().AddRange(entity);
                    var status = this._dataContext.SaveChanges();

                    if (status <= 0)
                    {

                        success = false;
                    }
                }
                catch (EntitySqlException ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
            else
            {
                success = this.AddWithTransaction(entity);
            }

            return success;
        }

        /// <summary>
        /// Dispose DbContext
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose DbContext
        /// </summary>
        /// <param name="disposing">disposing</param>
        public virtual void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only                
                if (this._dataContext != null)
                {
                    this._dataContext.Dispose();
                }
            }

            // release any unmanaged objects
            // set the object references to null
            _disposed = true;
        }

        /// <summary>
        /// 查詢其中一筆資料
        /// </summary>
        /// <param name="match">查詢條件</param>
        /// <param name="noTracking">Is Use AsNoTracking</param>
        /// <param name="includes">包含哪些資料表</param>
        /// <returns></returns>
        public TEntity Find(Expression<Func<TEntity, bool>> match, bool noTracking = false, params Expression<Func<TEntity, object>>[] includes)
        {
            TEntity data = null;
            IQueryable<TEntity> filter = null;

            try
            {
                if (noTracking == false)
                {
                    filter = this._dataContext.Set<TEntity>().Where(match).AsQueryable();
                }
                else
                {
                    filter = this._dataContext.Set<TEntity>().Where(match).AsNoTracking().AsQueryable();
                }

                if (includes != null)
                {
                    filter = includes.Aggregate(filter, (current, include) => current.Include(include));
                }

                data = filter.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return data;
        }

        /// <summary>
        /// 依據查詢條件篩選
        /// </summary>
        /// <param name="match">查詢條件</param>
        /// <param name="noTracking">Is Use AsNoTracking</param>
        /// <param name="includes">包含哪些資料表</param>
        /// <returns></returns>
        public IQueryable<TEntity> FindAll(Expression<Func<TEntity, bool>> match = null, bool noTracking = false, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> filter = this._dataContext.Set<TEntity>();

            if (match != null)
            {
                filter = filter.Where(match);
            }

            if (noTracking == true)
            {
                filter = filter.AsNoTracking().AsQueryable();
            }

            if (includes != null)
            {
                filter = includes.Aggregate(filter, (current, include) => current.Include(include));
            }

            return filter;
        }

        /// <summary>
        /// 使用sql語法查詢結果
        /// </summary>
        /// <param name="sql">sql string</param>
        /// <param name="param">參數</param>
        /// <returns></returns>
        public IEnumerable<TEntity> GetQueryData(string sql, List<SqlParameter> param = null)
        {
            IEnumerable<TEntity> list = null;

            try
            {
                if (param != null && param.Any())
                {
                    list = this._dataContext.Database.SqlQuery<TEntity>(sql, param.ToArray()).AsEnumerable();
                }
                else
                {
                    list = this._dataContext.Database.SqlQuery<TEntity>(sql).AsEnumerable();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return list;
        }

        /// <summary>
        /// 執行指令
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool ExcuteSqlCmd(string sql)
        {
            bool success = true;

            try
            {
                var status = this._dataContext.Database.ExecuteSqlCommand(sql);

                if (status <= 0)
                {
                    success = false;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return success;
        }

        /// <summary>
        /// 刪除該筆資料
        /// </summary>
        /// <param name="entity">EntityType</param>
        /// <param name="useTransaction">useTransaction</param>
        /// <returns></returns>
        public bool Remove(TEntity entity, bool useTransaction = false)
        {
            bool success = true;

            if (useTransaction == false)
            {
                try
                {
                    this._dataContext.Entry<TEntity>(entity).State = EntityState.Deleted;
                    var status = this._dataContext.SaveChanges();

                    if (status <= 0)
                    {
                        success = false;
                    }
                }
                catch (EntitySqlException ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
            else
            {
                this.RemoveWithTransaction(entity);
            }

            return success;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">EntityType</param>
        /// <param name="useTransaction">useTransaction</param>
        /// <returns></returns>
        public bool Update(TEntity entity, bool useTransaction = false)
        {
            bool success = true;


            if (useTransaction == false)
            {
                try
                {
                    this._dataContext.Entry<TEntity>(entity).State = EntityState.Modified;
                    var status = this._dataContext.SaveChanges();


                    if (status <= 0)
                    {

                        success = false;
                    }
                }
                catch (EntitySqlException ex)
                {

                    throw new Exception(ex.Message, ex);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
            else
            {
                this.UpdateWithTransaction(entity);
            }

            return success;
        }

        /// <summary>
        /// 新增 Use Transaction
        /// </summary>
        /// <param name="entity">EntityType</param>
        /// <returns></returns>
        private bool AddWithTransaction(IEnumerable<TEntity> entity)
        {
            bool success = true;

            using (var dbContextTransaction = this._dataContext.Database.BeginTransaction())
            {
                try
                {
                    this._dataContext.Set<TEntity>().AddRange(entity);
                    var status = this._dataContext.SaveChanges();
                    dbContextTransaction.Commit();

                    if (status <= 0)
                    {
                        dbContextTransaction.Commit();
                        success = false;
                    }
                }
                catch (EntitySqlException ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message, ex);
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message, ex);
                }
            }

            return success;
        }

        /// <summary>
        /// 刪除該筆資料
        /// </summary>
        /// <param name="entity">EntityType</param>
        /// <returns></returns>
        private bool RemoveWithTransaction(TEntity entity)
        {
            bool success = true;

            using (var dbContextTransaction = this._dataContext.Database.BeginTransaction())
            {
                try
                {
                    this._dataContext.Entry<TEntity>(entity).State = EntityState.Deleted;
                    var status = this._dataContext.SaveChanges();
                    dbContextTransaction.Commit();

                    if (status <= 0)
                    {
                        dbContextTransaction.Commit();
                        success = false;
                    }
                }
                catch (EntitySqlException ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message, ex);
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message, ex);
                }
            }

            return success;
        }

        /// <summary>
        /// 刪除該筆資料
        /// </summary>
        /// <param name="entity">EntityType</param>
        /// <returns></returns>
        private bool UpdateWithTransaction(TEntity entity)
        {
            bool success = true;

            using (var dbContextTransaction = this._dataContext.Database.BeginTransaction())
            {
                try
                {
                    this._dataContext.Entry<TEntity>(entity).State = EntityState.Modified;
                    var status = this._dataContext.SaveChanges();
                    dbContextTransaction.Commit();

                    if (status <= 0)
                    {
                        dbContextTransaction.Commit();
                        success = false;
                    }
                }
                catch (EntitySqlException ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message, ex);
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message, ex);
                }
            }

            return success;
        }
    }
}