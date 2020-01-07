using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogic.Domain;
using ITSWeb.Controllers.Base;

namespace ITSWeb.Controllers
{
    public class ShoppingCarController : BaseController
    {
        /// <summary>
        /// 購物車頁面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.ShoppingCarList = ShoppingCarService.GetAll(GetUserId());
            return View();
        }
        /// <summary>
        /// 新增至購物車
        /// </summary>
        /// <param name="model">購物車商品</param>
        /// <returns></returns>
        public ActionResult AddOne(ShoppingCarDetail model)
        {
            var result = ShoppingCarService.AppendOne(GetUserId(), model);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 自購物車移除
        /// </summary>
        /// <param name="productId">商品編號</param>
        /// <returns></returns>
        public ActionResult Remove( int productId)
        {
            var result = ShoppingCarService.Remove(GetUserId(), productId);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 結帳
        /// </summary>
        /// <returns></returns>
        public ActionResult Save()
        {
            ResponseModel result=new ResponseModel(){Status = false, Result ="查無商品!"};
            try
            {
                var shoppingCar = ShoppingCarService.GetAll(GetUserId());
                if (!(shoppingCar == null || shoppingCar.ProductList == null || shoppingCar.ProductList.Count == 0))
                {
                    result = ProductService.CheckProductQuantity(shoppingCar.ProductList);
                }

                if (result.Status)
                {
                    result = OrderService.SaveOrder(shoppingCar);
                    if (result.Status)
                    {
                        result = ProductService.SaveChangeProductQuantity();
                        if (!result.Status) return Json(result);
                        result = ShoppingCarService.Remove(GetUserId(), -1);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ProductService.UnLockProduct();
            }

            return Json(result);
        }

    }
}