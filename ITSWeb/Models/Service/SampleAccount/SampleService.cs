using System;
using System.Collections.Generic;
using System.Linq;
using ITSWeb.Models.Base;
using ITSWeb.Models.Domain;
using ITSWeb.Models.Repository;

namespace ITSWeb.Models.Service.SampleAccount
{
    /// <summary>
    /// 會員相關功能
    /// </summary>
    public class SampleService : SampleDataBase
    {
        /// <summary>
        /// SampleService
        /// </summary>
        public SampleService()
        {

        }

        /// <summary>
        /// SampleService
        /// </summary>
        /// <param name="accountRepository">AccountRepository</param>
        public SampleService(IRepository<SampleAccountViewModel> accountRepository)
        {
            base.SampleAccountRepository = accountRepository;

            if (accountRepository == null)
            {
                throw new AggregateException($"AccountRepository is null");
            }
        }

        /// <summary>
        /// 取得全部資料
        /// </summary>
        /// <returns></returns>
        public List<SampleAccountViewModel> GetAll()
        {
            return base.SampleAccountRepository.FindAll().ToList();
        }
    }
}