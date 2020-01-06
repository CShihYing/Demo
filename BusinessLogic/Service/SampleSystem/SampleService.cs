using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Base;
using BusinessLogic.Interface;
using DataAccess.Base;
using DataAccess.Domain;
using DataAccess.SampleDataBase;
using Repository;

namespace BusinessLogic.Service.SampleSystem
{
    /// <summary>
    /// 功能服務(範例)
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
        /// Repository DI
        /// </summary>
        /// <param name="accountRepository">AccountRepository</param>
        public SampleService(IRepository<SampleAspNetUsers> accountRepository)
        {
            accountRepository=new GenericRepository<SampleAspNetUsers>(new DbContextFactory("conn"));
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
            return base.SampleAccountRepository.FindAll().Select(x => new SampleAccountViewModel()
            {
                Name = x.UserName,
                Password = x.PasswordHash
            }).ToList();
        }
    }
}
