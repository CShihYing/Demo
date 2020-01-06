using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Domain
{
    public class ResponseModel
    {
        /// <summary>
        /// 狀態(成功/失敗)
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 內容
        /// </summary>
        public string Result { get; set; }
    }

}
