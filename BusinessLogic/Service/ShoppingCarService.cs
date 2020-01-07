using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using BusinessLogic.Base;
using BusinessLogic.Domain;
using DataAccess.SampleDataBase;
using Newtonsoft.Json;
using Repository;

namespace BusinessLogic.Service
{
    public class ShoppingCarService : ShoppingCarDataBase
    {
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="connName">nameOfConnectionString</param>
        public ShoppingCarService(string connName)
        {
            base.ShoppingCarRepository = new Repository.GenericRepository<ShoppingCart>(new DataAccess.Base.DbContextFactory(connName));
            //memberRepository=new GenericRepository<AspNetUsers>(new DbContextFactory("DefaultConnection"));
            if (ShoppingCarRepository == null)
            {
                _log.Error($"ShoppingCarRepository is null");
                throw new AggregateException($"ShoppingCarRepository is null");
            }
        }
        /// <summary>
        /// 取得購物車內容
        /// </summary>
        /// <param name="id">AccountId</param>
        /// <returns></returns>
        public ShoppingCarModel GetAll(string id)
        {
            var model=ShoppingCarRepository.Find(x => x.AccountID == id);
            var result = new ShoppingCarModel(){MemberId = id};
            try
            {
                if (model == null) result.ProductList = new List<ShoppingCarDetail>();
                else
                {
                    result.ProductList = JsonConvert.DeserializeObject<List<ShoppingCarDetail>>(
                        model.ShoppinMsg);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return result;
        }
        /// <summary>
        /// 新增購物車內容
        /// </summary>
        /// <param name="id">AccountId</param>
        /// <returns></returns>
        public ResponseModel AppendOne(string id, ShoppingCarDetail detail)
        {
            var result = new ResponseModel() { Result = "儲存失敗", Status = false };
            List<ShoppingCarDetail> list;
            try
            {
                var dbModel = ShoppingCarRepository.Find(x => x.AccountID == id);
                if (dbModel != null)
                {
                    list = JsonConvert.DeserializeObject<List<ShoppingCarDetail>>(
                        dbModel.ShoppinMsg);
                    var item = list.Where(x => x.Id == detail.Id).FirstOrDefault();
                    if (item != null)
                    {
                        item.Qty += detail.Qty;
                    }
                    else
                    {
                        list.Add(detail);
                    }
                    dbModel.ShoppinMsg = JsonConvert.SerializeObject(list);
                    result.Status = ShoppingCarRepository.Update(dbModel);
                }
                else
                {
                    list = new List<ShoppingCarDetail>();
                    list.Add(detail);
                    dbModel = new ShoppingCart() { AccountID = id };
                    dbModel.ShoppinMsg = JsonConvert.SerializeObject(list);
                    result.Status = ShoppingCarRepository.Add(dbModel);
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
        /// 移除購物車一項內容
        /// </summary>
        /// <param name="id">AccountId</param>
        /// <param name="productId">商品編號</param>
        /// <returns></returns>
        public ResponseModel Remove(string id, int productId)
        {
            var result = new ResponseModel() { Result = "移除失敗", Status = false };
            List<ShoppingCarDetail> list;
            try
            {
                var dbModel = ShoppingCarRepository.Find(x => x.AccountID == id);
                if (dbModel == null)
                {
                    result.Result = "購物車不存在";
                    return result;
                }
                list = JsonConvert.DeserializeObject<List<ShoppingCarDetail>>(
                    dbModel.ShoppinMsg);
                if (productId == -1)
                {
                    return Delete(id);
                }
                else
                {
                    var item = list.Find(x => x.Id == productId);
                    if (item == null)
                    {
                        result.Result = "商品不存在於購物車";
                        return result;
                    }

                    list.Remove(item);
                    dbModel.ShoppinMsg = JsonConvert.SerializeObject(list);
                    result.Status = ShoppingCarRepository.Update(dbModel);
                }

                if (result.Status)
                {
                    result.Result = "移除成功";
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return result;
        }
        /// <summary>
        /// 登入後將購物車內容一起帶入
        /// </summary>
        /// <param name="ori">原始AccountId</param>
        /// <param name="ta">要轉換成的AccountId</param>
        public void ChangeShoppingCarId(string ori, string ta)
        {
            var model = ShoppingCarRepository.Find(x => x.AccountID == ori);
            try
            {
                if (model != null)
                {
                    Delete(ta);
                    model.AccountID = ta;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }
        /// <summary>
        /// 刪除購物車
        /// </summary>
        /// <param name="id">AccountId</param>
        /// <returns></returns>
        private ResponseModel Delete(string id)
        {
            var result = new ResponseModel() { Result = "刪除失敗", Status = false };
            try
            {
                var dbModel = ShoppingCarRepository.Find(x => x.AccountID == id);
                if (dbModel == null)
                {
                    result.Result = "購物車不存在";
                    return result;
                }

                result.Status = ShoppingCarRepository.Remove(dbModel);
                if (result.Status)
                {
                    result.Result = "刪除成功";
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