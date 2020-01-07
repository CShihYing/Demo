using System;
using System.Linq;
using BusinessLogic.Base;
using BusinessLogic.Domain;
using DataAccess.SampleDataBase;

namespace BusinessLogic.Service
{
    public class OrderService : OrderDataBase
    {
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="connName">nameOfConnectionString</param>
        public OrderService(string connName)
        {
            base.OrderDetailRepository = new Repository.GenericRepository<OrderDetail>(new DataAccess.Base.DbContextFactory(connName));
            base.OrderRepository = new Repository.GenericRepository<Orders>(new DataAccess.Base.DbContextFactory(connName));
            if (OrderDetailRepository == null || OrderDetailRepository==null)
            {
                _log.Error($"OrderRepository is null");
                throw new AggregateException($"OrderRepository is null");
            }
        }
        /// <summary>
        /// 儲存至訂單
        /// </summary>
        /// <param name="model">購物車模型</param>
        /// <returns></returns>
        public ResponseModel SaveOrder(ShoppingCarModel model)
        {
            var result = new ResponseModel() { Result = "儲存失敗", Status = false };

            try
            {
                if (model.ProductList == null || model.ProductList.Count == 0)
                {
                    return result;
                }
                var master = new Orders();
                master.AccountId = model.MemberId;
                master.TotalAmount = model.ProductList.Sum(x => x.Price * x.Qty);
                result.Status = OrderRepository.Add(master);
                if (result.Status)
                {
                    foreach (var item in model.ProductList)
                    {
                        var detail = new OrderDetail();
                        detail.OrderId = master.OrderId;
                        detail.ProductId = item.Id;
                        detail.Quantity = item.Qty;
                        detail.Amount = item.Price;
                        result.Status = OrderDetailRepository.Add(detail);
                        if (!result.Status)
                        {
                            return result;
                        }
                    }
                }
                if (result.Status)
                {
                    result.Result = "儲存成功!";
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return result;
            }
            return result;
        }

    }
}