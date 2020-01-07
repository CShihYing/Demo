using System;
using BusinessLogic.Base;
using BusinessLogic.Helps;
using DataAccess.Base;
using DataAccess.SampleDataBase;
using NLog.Fluent;
using Repository;

namespace BusinessLogic.Service
{
    /// <summary>
    /// 使用者(會員)服務
    /// </summary>
    public class MemberService: MemberDataBase
    {
        /// <summary>
        /// 通用函數
        /// </summary>
        private readonly CommonFunction _common = new CommonFunction();
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="connName">nameOfConnectionString</param>
        public MemberService(string connName)
        {
            base.MemberRepository = new Repository.GenericRepository<AspNetUsers>(new DataAccess.Base.DbContextFactory(connName));
            if (MemberRepository == null)
            {
                _log.Error($"MemberRepository is null");
                throw new AggregateException($"MemberRepository is null");
            }
        }
        /// <summary>
        /// 查詢名稱
        /// </summary>
        /// <param name="id">AccountId</param>
        /// <returns></returns>
        public string GetName(string id)
        {
            try
            {
                Guid memberId = _common.ParserStringToGuid(id);
                return MemberRepository.Find(x => x.Id == memberId)?.UserName;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return "";
            }
        }
    }
}