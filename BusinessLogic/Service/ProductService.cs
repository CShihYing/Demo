using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Hosting;
using BusinessLogic.Base;
using BusinessLogic.Domain;
using BusinessLogic.Helps;
using BusinessLogic.Infrastructure;
using BusinessLogic.Interface;
using DataAccess.SampleDataBase;
using Repository;

namespace BusinessLogic.Service
{
    /// <summary>
    /// 商品服務
    /// </summary>
    public class ProductService : ProductDataBase
    {
        /// <summary>
        /// 圖檔存取服務
        /// </summary>
        private readonly IFile _imageFileService;
        /// <summary>
        /// 通用函數
        /// </summary>
        private readonly CommonFunction _common=new CommonFunction();
        /// <summary>
        /// 商品鎖定
        /// </summary>
        private static Dictionary<int,int> _productDictionary=new Dictionary<int, int>();
        /// <summary>
        /// 商品列表
        /// </summary>
        private List<int> _productList=new List<int>();
        /// <summary>
        /// 圖檔存檔資料夾路徑
        /// </summary>
        private readonly string _imgPath;

        private static object _locked=new object();
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="connName">nameOfConnectionString</param>
        /// <param name="imgPath">圖檔存檔資料夾路徑</param>
        public ProductService(string connName,string imgPath)
        {
            _imgPath = imgPath;
            _imageFileService = new ImageFile();
            _imageFileService.SetPath(HostingEnvironment.MapPath(imgPath));
            base.ProductRepository = new Repository.GenericRepository<Product>(new DataAccess.Base.DbContextFactory(connName)); ;
            //memberRepository=new GenericRepository<AspNetUsers>(new DbContextFactory("DefaultConnection"));
            if (ProductRepository == null)
            {
                _log.Error($"ProductRepository is null");
                throw new AggregateException($"ProductRepository is null");
            }
        }

        /// <summary>
        /// 確認商品數量/鎖定商品(購買用)
        /// </summary>
        /// <param name="shoppingCarDetailList">購物車商品列表</param>
        /// <returns></returns>
        public ResponseModel CheckProductQuantity(IEnumerable<ShoppingCarDetail> shoppingCarDetailList)
        {
            var result = new ResponseModel() { Result = "商品數量不足", Status = false };
            lock (_locked)
            {
                foreach (var detail in shoppingCarDetailList)
                {
                    try
                    {
                        int waitTime = 0;
                        var item = ProductRepository.Find(x => x.ProductId == detail.Id);
                        if (item != null)
                        {
                            while (_productDictionary.ContainsKey(item.ProductId))
                            {
                                Thread.Sleep(10);
                                waitTime += 10;
                                if (waitTime == 500)
                                {
                                    result.Result = "伺服器忙碌中，請稍後在試";
                                    return result;
                                }
                            }

                            if (item.ProductQuantity >= detail.Qty)
                            {
                                _productDictionary.Add(item.ProductId, detail.Qty);
                                _productList.Add(item.ProductId);
                                result.Status = true;
                            }
                            if(!result.Status) break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                        return result;
                    }
                }

            }
            return result;
        }
        /// <summary>
        /// 商品全部解除鎖定
        /// </summary>
        public void UnLockProduct()
        {
            try
            {
                foreach (var productId in _productList)
                {
                    if (_productDictionary.ContainsKey(productId))
                    {
                        _productDictionary.Remove(productId);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

        }
        /// <summary>
        /// 儲存商品改變數量
        /// </summary>
        /// <returns></returns>
        public ResponseModel SaveChangeProductQuantity()
        {
            var result = new ResponseModel() { Result = "儲存失敗", Status = false };
            try
            {
                foreach (var productId in _productList)
                {
                    var item = ProductRepository.Find(x => x.ProductId == productId);
                    if (item != null)
                    {
                        item.ProductQuantity -= _productDictionary[productId];
                        result.Status = ProductRepository.Update(item);
                    }
                    _productDictionary.Remove(productId);
                }

                if (result.Status)
                {
                    result.Result = "儲存成功";
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
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
            try
            {
                if (model != null)
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
                _common.ListTrans(result.ToList(), item);
                foreach (var detail in item)
                {
                    detail.MaxQuantity = detail.ProductQuantity;
                    var imgPath = _imgPath.StartsWith("~") ? _imgPath.Substring(1) : _imgPath;
                    detail.ProductImage = imgPath + detail.ProductImage;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
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
            try
            {
                _common.ItemTrans(result, item);
                if (!string.IsNullOrWhiteSpace(item.ProductImage))
                {
                    var imgPath = _imgPath.StartsWith("~") ? _imgPath.Substring(1) : _imgPath;
                    item.ProductImage = imgPath + item.ProductImage;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return item;
        }
        /// <summary>
        /// 儲存商品
        /// </summary>
        /// <param name="model">商品模型</param>
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
                    result.Status = _imageFileService.SaveToPath(model.ImageFile, item.ProductImage);
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
        /// <param name="model">商品模型</param>
        /// <param name="item">商品DB模型</param>
        /// <returns></returns>
        private bool Insert(ProductModel model,Product item)
        {
            try
            {
                _common.ItemTrans(model, item);
                return ProductRepository.Add(item);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return false;
            }
          
        }
        /// <summary>
        /// 設定更新資料
        /// </summary>
        /// <param name="model">商品模型</param>
        /// <param name="item">商品DB模型</param>
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
        /// <param name="id">商品編號</param>
        /// <returns></returns>
        public ResponseModel Delete(int id)
        {
            var result = new ResponseModel() { Result = "刪除失敗", Status = false };
            var item = ProductRepository.Find(x => x.ProductId == id);
            try
            {
                if (item != null)
                {
                    result.Status = ProductRepository.Remove(item);
                    if (result.Status)
                    {
                        result.Status = _imageFileService.DeleteFile(item.ProductImage);
                        if (result.Status)
                        {
                            result.Result = "刪除成功!";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return result;
        }
    }
}