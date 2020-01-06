using System.Data.Entity;

namespace ITSWeb.Interface
{
    /// <summary>
    /// DbContext Factory
    /// </summary>
    public interface IDbContextFactory
    {
        /// <summary>
        /// Get Db Context
        /// </summary>
        /// <returns></returns>
        DbContext GetDbContext();
    }
}