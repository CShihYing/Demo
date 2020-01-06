using System.Web;

namespace BusinessLogic.Domain
{
    /// <summary>
    /// 商品模組
    /// </summary>
    public class ProductDomainModel
    {
        
    }
    /// <summary>
    /// 商品搜尋模型
    /// </summary>
    public class ProductFilterModel
    {
        /// <summary>
        /// 關鍵字
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 是否只要上架商品
        /// </summary>
        public bool OnlyIsShow { get; set; }
    }
    /// <summary>
    /// 商品模型
    /// </summary>
    public class ProductModel
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 商品名稱
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 商品價格
        /// </summary>
        public int ProductPrice { get; set; }
        /// <summary>
        /// 商品數量
        /// </summary>
        public int ProductQuantity { get; set; }
        /// <summary>
        /// 商品圖片路徑
        /// </summary>
        public string ProductImage { get; set; }
        /// <summary>
        /// 是否上架
        /// </summary>
        public bool IsShow { get; set; }
        /// <summary>
        /// 商品管理(for 前端用)
        /// </summary>
        public HttpPostedFileBase ImageFile { get; set; }
    }
}