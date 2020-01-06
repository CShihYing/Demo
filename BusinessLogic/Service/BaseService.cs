using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;
using BusinessLogic.Base;
using BusinessLogic.Infrastructure;
using BusinessLogic.Interface;

namespace BusinessLogic.Service
{
    public class BaseService: ShoppingCarDataBase
    {
        
        /// <summary>
        /// 圖檔路徑
        /// </summary>
        public string ImgPath { get; set; }
        /// <summary>
        /// 圖檔存取服務
        /// </summary>
        private IFile _imageFileService;
        /// <summary>
        /// 圖檔存取服務
        /// </summary>
        public IFile ImageFileService
        {
            get
            {
                if (_imageFileService == null)
                {
                    _imageFileService = new ImageFile();
                    _imageFileService.SetPath(HostingEnvironment.MapPath(ImgPath));
                }

                return _imageFileService;
            }
        }

        /// <summary>
        /// 字串轉為Guid
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        protected Guid ParserStringToGuid(string target)
        {
            Guid outGuid = Guid.Empty;

            if (!string.IsNullOrWhiteSpace(target))
            {
                Guid.TryParse(target, out outGuid);
            }
            return outGuid;
        }
        /// <summary>
        /// 將origin class中相同名稱的參數轉至target class
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        protected TK ItemTrans<T,TK>(T origin, TK target)
        {
            Type originType = origin.GetType();
            Type targetType = target.GetType();
            foreach (var property in originType.GetProperties())
            {
                string propertyName = property.Name;
                if (targetType.GetProperties().Any(x => x.Name== propertyName))
                {
                    PropertyInfo propertyInfo = targetType.GetProperty(propertyName);
                    propertyInfo?.SetValue(target, Convert.ChangeType(property.GetValue(origin), propertyInfo.PropertyType), null);
                }
            }
            return target;
        }
        /// <summary>
        /// 將整個List轉為另外一個class List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TK"></typeparam>
        /// <param name="originList"></param>
        /// <param name="targetList"></param>
        protected void ListTrans<T, TK>(List<T> originList, List<TK> targetList)
        {
            foreach (var origin in originList)
            {
                var target= (TK)Activator.CreateInstance(typeof(TK));
                ItemTrans(origin, target);
                targetList.Add(target);
            }
            
        }
    }
}