using System;
using BusinessLogic.Base;
using DataAccess.Base;
using DataAccess.SampleDataBase;
using Repository;

namespace BusinessLogic.Service
{
    /// <summary>
    /// 使用者(會員)服務
    /// </summary>
    public class MemberService: BaseService
    {
        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="connName"></param>
        public MemberService(string connName)
        {
            base.MemberRepository = new Repository.GenericRepository<AspNetUsers>(new DataAccess.Base.DbContextFactory(connName));
            if (MemberRepository == null)
            {
                throw new AggregateException($"MemberRepository is null");
            }
        }
        /// <summary>
        /// 查詢名稱
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetName(string id)
        {
            Guid memberId = ParserStringToGuid(id);
            return MemberRepository.Find(x => x.Id == memberId)?.UserName;
        }
    }
}