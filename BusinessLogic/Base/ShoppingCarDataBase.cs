﻿using BusinessLogic.Infrastructure;
using BusinessLogic.Interface;
using DataAccess.SampleDataBase;
using Repository;
using System;

namespace BusinessLogic.Base
{
    public class ShoppingCarDataBase : IDisposable
    { /// <summary>
        /// log
        /// </summary>
        protected ILog _log = new Log();

        /// <summary>
        /// 會員(使用者)資料表
        /// </summary>
        protected IRepository<AspNetUsers> MemberRepository { get; set; }
        /// <summary>
        /// 商品資料表
        /// </summary>
        protected IRepository<Product> ProductRepository { get; set; }
        /// <summary>
        /// 購物車資料表
        /// </summary>
        protected IRepository<ShoppingCart> ShoppingCarRepository { get; set; }
        /// <summary>
        /// 訂單主檔資料表
        /// </summary>
        protected IRepository<Orders> OrderRepository { get; set; }
        /// <summary>
        /// 訂單細項資料表
        /// </summary>
        protected IRepository<OrderDetail> OrderDetailRepository { get; set; }

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