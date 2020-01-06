using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using BusinessLogic.Base;
using BusinessLogic.Domain;
using DataAccess.SampleDataBase;
using Newtonsoft.Json;
using Repository;

namespace BusinessLogic.Service
{
    public class ShoppingCarService : BaseService
    {
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="connName"></param>
        public ShoppingCarService(string connName)
        {
            base.ShoppingCarRepository = new Repository.GenericRepository<ShoppingCart>(new DataAccess.Base.DbContextFactory(connName));
            //memberRepository=new GenericRepository<AspNetUsers>(new DbContextFactory("DefaultConnection"));
            if (ShoppingCarRepository == null)
            {
                throw new AggregateException($"ShoppingCarRepository is null");
            }
        }
        /// <summary>
        /// 取得購物車內容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ShoppingCarModel GetAll(string id)
        {
            var model=ShoppingCarRepository.Find(x => x.AccountID == id);
            var result = new ShoppingCarModel(){MemberId = id};
            if(model==null) result.ProductList=new List<ShoppingCarDetail>();
            else
            {
                result.ProductList = JsonConvert.DeserializeObject<List<ShoppingCarDetail>>(
                    model.ShoppinMsg);
            }

            return result;
        }
        /// <summary>
        /// 新增購物車內容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel AppendOne(string id, ShoppingCarDetail detail)
        {
            var result = new ResponseModel() { Result = "儲存失敗", Status = false };
            List<ShoppingCarDetail> list;
            var dbModel= ShoppingCarRepository.Find(x => x.AccountID == id);
            if (dbModel != null)
            {
                list = JsonConvert.DeserializeObject<List<ShoppingCarDetail>>(
                    dbModel.ShoppinMsg);
                list.Add(detail);
                dbModel.ShoppinMsg = JsonConvert.SerializeObject(list);
                result.Status = ShoppingCarRepository.Update(dbModel);
            }
            else
            {
                list=new List<ShoppingCarDetail>();
                list.Add(detail);
                dbModel=new ShoppingCart(){AccountID = id};
                dbModel.ShoppinMsg = JsonConvert.SerializeObject(list);
                result.Status = ShoppingCarRepository.Add(dbModel);

            }

            if (result.Status)
            {
                result.Result = "儲存成功";
            }

            return result;
        }
        /// <summary>
        /// 移除購物車一項內容
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public ResponseModel Remove(string id, int productId)
        {
            var result = new ResponseModel() { Result = "移除失敗", Status = false };
            List<ShoppingCarDetail> list;
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

            return result;
        }
        /// <summary>
        /// 刪除購物車
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        private ResponseModel Delete(string id)
        {
            var result = new ResponseModel() { Result = "刪除失敗", Status = false };
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

            return result;
        }

    }
}