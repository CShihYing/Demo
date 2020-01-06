using System;
using System.Data.Entity;
using DataAccess.Interface;

namespace DataAccess.Base
{
    /// <summary>
    /// 實作DbContextFactory
    /// </summary>
    public class DbContextFactory : IDbContextFactory
    {
        /// <summary>
        /// 資料庫連線名稱
        /// </summary>
        private readonly string _nameOfConnectionString;

        /// <summary>
        /// DbContext
        /// </summary>
        private DbContext _dataContext;

        /// <summary>
        /// 給予資料庫連線名稱
        /// </summary>
        /// <param name="nameOfConnectionString">資料庫連線名稱</param>
        public DbContextFactory(string nameOfConnectionString)
        {
            if (string.IsNullOrEmpty(nameOfConnectionString))
            {
                throw new ArgumentNullException($"name of/or connecting string is null.");
            }
            this._nameOfConnectionString = nameOfConnectionString;
        }

        /// <summary>
        /// Get DataContext
        /// </summary>
        /// <returns></returns>
        public DbContext GetDbContext()
        {
            return this.DataContext;
        }

        /// <summary>
        /// Get DataContext
        /// </summary>
        private DbContext DataContext
        {
            get
            {
                if (this._dataContext == null)
                {
                    try
                    {
                        _dataContext = Activator.CreateInstance(typeof(DbContext), _nameOfConnectionString) as DbContext;
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
                return this._dataContext;
            }
        }
    }
}
