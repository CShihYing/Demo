using System;
using BusinessLogic.Infrastructure;
using BusinessLogic.Interface;
using DataAccess.SampleDataBase;
using Repository;

namespace BusinessLogic.Base
{
    public class ProductDataBase
    {
        /// <summary>
        /// log
        /// </summary>
        protected ILog _log = new Log();
        /// <summary>
        /// 商品資料表
        /// </summary>
        protected IRepository<Product> ProductRepository { get; set; }
        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        public virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only                
            }

            // release any unmanaged objects
            // set the object references to null
            _disposed = true;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// The disposed
        /// </summary>
        private bool _disposed = false;
    }
}