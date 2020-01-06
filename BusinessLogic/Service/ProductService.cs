using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Base;
using BusinessLogic.Domain;
using DataAccess.SampleDataBase;
using Repository;

namespace BusinessLogic.Service
{
    /// <summary>
    /// 商品服務
    /// </summary>
    public class ProductService : BaseService
    {
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="connName"></param>
        /// <param name="imgPath"></param>
        public ProductService(string connName,string imgPath)
        {
            ImgPath = imgPath;
            base.ProductRepository = new Repository.GenericRepository<Product>(new DataAccess.Base.DbContextFactory(connName)); ;
            //memberRepository=new GenericRepository<AspNetUsers>(new DbContextFactory("DefaultConnection"));
            if (ProductRepository == null)
            {
                throw new AggregateException($"ProductRepository is null");
            }
        }
        /// <summary>
        /// 更改商品數量(購買用)
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="qty"></param>
        /// <returns></returns>
        public ResponseModel ChangeProductQuantity(int productId, int qty)
        {
            var result = new ResponseModel() { Result = "數量不足", Status = false };
            var item = ProductRepository.Find(x => x.ProductId == productId);
            if (item != null)
            {
                if (item.ProductQuantity >= qty)
                {
                    item.ProductQuantity -= qty;
                }
                result.Status=ProductRepository.Update(item);
            }
            if (result.Status)
            {
                result.Result = "儲存成功!";
            }

            return result;
        }
        /// <summary>
        /// 取得所有商品
        /// </summary>
        /// <param name="model">搜尋模型</param>
        /// <returns></returns>
        public List<ProductModel> GetAll(ProductFilterModel model =null)
        {
            var result = ProductRepository.FindAll();
            var item = new List<ProductModel>();
            if (model !=null)
            {
                if (!string.IsNullOrWhiteSpace(model.Key))
                {
                    result = result.Where(x => x.ProductName.Contains(model.Key));
                }

                if (model.OnlyIsShow)
                {
                    result = result.Where(x => x.IsShow);
                }
            }

            var ori = result.ToList();
            ListTrans(result.ToList(), item);
            foreach (var detail in item)
            {
                var imgPath = ImgPath.StartsWith("~") ? ImgPath.Substring(1): ImgPath;
                detail.ProductImage = imgPath + detail.ProductImage;
            }
            return item;
        }
        /// <summary>
        /// 取得所有商品
        /// </summary>
        /// <param name="id">搜尋模型</param>
        /// <returns></returns>
        public ProductModel GetOne(int id)
        {
            var result = ProductRepository.Find(x => x.ProductId == id);
            var item=new ProductModel();

            ItemTrans(result, item);
            if (!string.IsNullOrWhiteSpace(item.ProductImage))
            {
                var imgPath = ImgPath.StartsWith("~") ? ImgPath.Substring(1) : ImgPath;
                item.ProductImage = imgPath + item.ProductImage;
            }
            return item;
        }
        /// <summary>
        /// 儲存商品
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel Save(ProductModel model)
        {
            var result = new ResponseModel(){ Result = "儲存失敗",Status = false};
            try
            {
                var item=ProductRepository.Find(x => x.ProductId == model.ProductId);
                if (item == null)
                { 
                    item=new Product();
                    if (!Insert(model,item))
                    {
                        return result;
                    }
                }
                else
                {
                    item = SetData(model, item);
                }

                if (model.ImageFile != null)
                {
                    var fileName = model.ImageFile.FileName;
                    item.ProductImage = item.ProductId+ fileName.Substring(fileName.LastIndexOf('.'));
                }
                result.Status = ProductRepository.Update(item);
                if (result.Status)
                {
                    result.Status = ImageFileService.SaveToPath(model.ImageFile, item.ProductImage);
                    if (result.Status)
                    {
                        result.Result = "儲存成功!";
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                result.Status = false;
                result.Result = ex.ToString();
            }
            return result;
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool Insert(ProductModel model,Product item)
        {
            ItemTrans(model, item);
            return ProductRepository.Add(item);
        }
        /// <summary>
        /// 設定更新資料
        /// </summary>
        /// <param name="model"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private Product SetData(ProductModel model, Product item)
        {
            item.ProductName = model.ProductName;
            item.IsShow = model.IsShow;
            item.ProductPrice = model.ProductPrice;
            item.ProductQuantity = model.ProductQuantity;
            return item;
        }
        /// <summary>
        /// 刪除商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel Delete(int id)
        {
            var result = new ResponseModel() { Result = "刪除失敗", Status = false };
            var item = ProductRepository.Find(x => x.ProductId == id);
            if (item != null)
            {
                result.Status=ProductRepository.Remove(item);
                if (result.Status)
                {
                    result.Status = ImageFileService.DeleteFile(item.ProductImage);
                    result.Result = "刪除成功!";
                }
            }
            return result;
        }
    }
}