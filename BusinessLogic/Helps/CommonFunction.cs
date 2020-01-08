using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BusinessLogic.Helps
{
    public class CommonFunction
    {
        /// <summary>
        /// 字串轉為Guid
        /// </summary>
        /// <param name="target">string guid</param>
        /// <returns></returns>
        public Guid ParserStringToGuid(string target)
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
        /// <param name="origin">原始model</param>
        /// <param name="target">欲轉換為的model</param>
        /// <returns></returns>
        public TK ItemTrans<T, TK>(T origin, TK target)
        {
            Type originType = origin.GetType();
            Type targetType = target.GetType();
            foreach (var property in originType.GetProperties())
            {
                string propertyName = property.Name;
                if (targetType.GetProperties().Any(x => x.Name == propertyName))
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
        /// <param name="originList">原始List</param>
        /// <param name="targetList">欲轉換為的List</param>
        public void ListTrans<T, TK>(List<T> originList, List<TK> targetList)
        {
            foreach (var origin in originList)
            {
                var target = (TK)Activator.CreateInstance(typeof(TK));
                ItemTrans(origin, target);
                targetList.Add(target);
            }

        }
    }
}