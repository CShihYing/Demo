using System.Collections.Generic;

namespace BusinessLogic.Domain
{
    /// <summary>
    /// 購物車模型
    /// </summary>
    public class ShoppingCarDetail
    {
        /// <summary>
        /// 商品名稱
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 商品Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 數量
        /// </summary>
        public int Qty { get; set; }
        /// <summary>
        /// 價格
        /// </summary>
        public int Price { get; set; }
        /// <summary>
        /// 最高可訂購數量
        /// </summary>
        public int MaxQty { get; set; }
        /// <summary>
        /// 管理
        /// </summary>
        public string Manager { get; set; }
    }
    /// <summary>
    /// 購物車主模型
    /// </summary>
    public class ShoppingCarModel
    {
        /// <summary>
        /// 會員ID
        /// </summary>
        public string MemberId { get; set; }
        /// <summary>
        /// 商品列表
        /// </summary>
        public List<ShoppingCarDetail> ProductList { get; set; }
    }
}