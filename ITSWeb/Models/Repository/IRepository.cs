using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace ITSWeb.Models.Repository
{
    /// <summary>
    ///  資料庫存取介面
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> : IDisposable
    {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity">EntityType</param>
        /// <returns>新增結果</returns>
        bool Add(T entity);

        /// <summary>
        /// 新增多筆資料。
        /// </summary>
        /// <param name="entity">要新增到的Entity</param>
        /// <param name="useTransaction">是否啟用交易(預設為不啟用)</param>
        bool Add(IEnumerable<T> entity, bool useTransaction = false);

        /// <summary>
        /// 依據查詢條件篩選
        /// </summary>
        /// <param name="match">查詢條件</param>
        /// <param name="noTracking">Is Use AsNoTracking</param>
        /// <param name="includes">包含哪些資料表</param>
        /// <returns>查詢結果</returns>
        IQueryable<T> FindAll(Expression<Func<T, bool>> match = null, bool noTracking = false, params Expression<Func<T, object>>[] includes);

        /// <summary>
        ///  查詢其中一筆資料
        /// </summary>
        /// <param name="match">查詢條件</param>
        /// <param name="noTracking">Is Use AsNoTracking</param>
        /// <param name="includes">包含哪些資料表</param>
        /// <returns>查詢結果</returns>
        T Find(Expression<Func<T, bool>> match, bool noTracking = false, params Expression<Func<T, object>>[] includes);

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="entity">EntityType</param>
        /// <param name="useTransaction"></param>
        /// <returns>刪除結果</returns>
        bool Remove(T entity, bool useTransaction = false);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity">EntityType</param>
        /// <param name="useTransaction"></param>
        /// <returns>修改結果</returns>
        bool Update(T entity, bool useTransaction = false);

        /// <summary>
        /// 使用sql語法查詢資料
        /// </summary>
        /// <param name="sql">sql語法</param>
        /// <param name="param">參數</param>
        /// <returns>查詢結果</returns>
        IEnumerable<T> GetQueryData(string sql, List<SqlParameter> param = null);

        /// <summary>
        /// 執行語法
        /// </summary>
        /// <param name="sql">sql語法</param>
        /// <returns></returns>
        bool ExcuteSqlCmd(string sql);
    }
}