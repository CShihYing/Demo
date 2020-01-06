using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace BusinessLogic.Domain
{
    /// <summary>
    /// 訂單主檔
    /// </summary>
    public class OrderDomainModel
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 使用者
        /// </summary>
        public Guid AccountId { get; set; }
        /// <summary>
        /// 總金額
        /// </summary>
        public int TotalAmount { get; set; }
        /// <summary>
        /// 訂單明細
        /// </summary>
        public List<OrderDetailModel> DetailList=new List<OrderDetailModel>();
    }
    /// <summary>
    /// 訂單明細
    /// </summary>
    public class OrderDetailModel
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 使用者
        /// </summary>
        public Guid ProductId { get; set; }
        /// <summary>
        /// 數量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 金額
        /// </summary>
        public int Amount { get; set; }
    }
}