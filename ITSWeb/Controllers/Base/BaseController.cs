using System;
using System.ComponentModel.Design.Serialization;
using ITSWeb.Infrastructure;
using ITSWeb.Interface;
using System.Web.Mvc;
using ITSWeb.Models.Service.SampleAccount;
using BusinessLogic.Service;
using Microsoft.AspNet.Identity;
using System.Configuration;

namespace ITSWeb.Controllers.Base
{
    /// <summary>
    /// BaseController Dependency All Service
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// 取得當前使用者資訊
        /// </summary>
        public ICurrentUserSerivce UserSerivce;

        /// <summary>
        /// 會員服務
        /// </summary>
        private MemberService _memberService=null;
        /// <summary>
        /// 商品服務
        /// </summary>
        private ProductService _productService = null;
        /// <summary>
        /// 購物車服務
        /// </summary>
        private ShoppingCarService _shoppingCarService = null;
        /// <summary>
        /// 訂單服務
        /// </summary>
        private OrderService _orderService = null;

        /// <summary>
        /// 會員服務
        /// </summary>
        public MemberService MemberService
        {
            get
            {
                if (_memberService == null)
                {
                    _memberService=new MemberService("ShoppingCarEntities");
                }

                return _memberService;
            }
        }
        /// <summary>
        /// 商品服務
        /// </summary>
        public ProductService ProductService
        {
            get
            {
                if (_productService == null)
                {
                    _productService = new ProductService("ShoppingCarEntities", ConfigurationManager.AppSettings["FullPhotoPath"]);
                }

                return _productService;
            }
        }
        /// <summary>
        /// 購物車服務
        /// </summary>
        public ShoppingCarService ShoppingCarService
        {
            get
            {
                if (_shoppingCarService == null)
                {
                    _shoppingCarService = new ShoppingCarService("ShoppingCarEntities");
                }

                return _shoppingCarService;
            }
        }
        /// <summary>
        /// 訂單服務
        /// </summary>
        public OrderService OrderService
        {
            get
            {
                if (_orderService == null)
                {
                    _orderService = new OrderService("ShoppingCarEntities");
                }

                return _orderService;
            }
        }
        public BaseController()
        {
            UserSerivce = new CurrentUserSerivce();
        }
        /// <summary>
        /// 取得使用者ID
        /// </summary>
        /// <returns></returns>
        public string GetUserId()
        {
            var user = UserSerivce.CurrentUser;
            var id = user.Identity.GetUserId();
            if (string.IsNullOrWhiteSpace(id))
            {
                id = System.Web.HttpContext.Current.Session["RandomId"]?.ToString();
                if (string.IsNullOrWhiteSpace(id))
                {
                    id = Guid.NewGuid().ToString();
                    System.Web.HttpContext.Current.Session["RandomId"] = Guid.NewGuid().ToString();
                }
            } 
            return id;
        }

        /// <summary>
        /// SampleService
        /// </summary>
        protected SampleService SampleService { get; set; }
    }
}