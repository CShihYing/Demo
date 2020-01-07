using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using BusinessLogic.Domain;
using ITSWeb.Controllers.Base;
using ITSWeb.Infrastructure;
using Microsoft.AspNet.Identity;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;

namespace ITSWeb.Controllers
{
    public class ProductController : BaseController
    {
        /// <summary>
        /// 商品首頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.ProductList = ProductService.GetAll(new ProductFilterModel()
            {
                OnlyIsShow = true
            });
            return View();
        }
        /// <summary>
        /// 商品管理頁
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult Manage()
        {
            ViewBag.ProductList = ProductService.GetAll();
            return View();
        }
        /// <summary>
        /// 商品修改
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            ViewBag.Product = ProductService.GetOne(id);
            return View();
        }
        /// <summary>
        /// 新增商品
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult Add()
        {
            return View();
        }
        /// <summary>
        /// 儲存
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult Save(ProductModel model)
        {
            var result=ProductService.Save(model);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 儲存
        /// </summary>
        /// <param name="model">產品模型</param>
        /// <returns></returns>
        public ActionResult BuyProduct(ProductModel model)
        {
            var result = ShoppingCarService.GetAll(GetUserId());
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 查詢
        /// </summary>
        /// <param name="model">查詢模型</param>
        /// <returns></returns>
        public ActionResult GetAll(ProductFilterModel model)
        {
            var result = ProductService.GetAll(model);
            return Json(result);
        }
        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="id">產品編號</param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            var result = ProductService.Delete(id);
            return Json(result, JsonRequestBehavior.DenyGet);
        }

    }
}