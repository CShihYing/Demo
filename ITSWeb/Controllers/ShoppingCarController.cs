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
        // GET: ShoppingCar
        public ActionResult Index()
        {
            ViewBag.ShoppingCarList = ShoppingCarService.GetAll(GetUserId());
            return View();
        }
        /// <summary>
        /// 新增至購物車
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult Addone(ShoppingCarDetail model)
        {
            var result = ShoppingCarService.AppendOne(GetUserId(), model);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 自購物車移除
        /// </summary>
        /// <param name="productId"></param>
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
            var result = new ResponseModel();
            var shoppingCar = ShoppingCarService.GetAll(GetUserId());
            foreach (var product in shoppingCar.ProductList)
            {
                result=ProductService.ChangeProductQuantity(product.Id, product.Qty);
                if(!result.Status) Json(result);
            }
            result = OrderService.SaveOrder(shoppingCar);
            if (result.Status)
            {
                result = ShoppingCarService.Remove(GetUserId(),-1);
            }
            return Json(result);
        }

    }
}