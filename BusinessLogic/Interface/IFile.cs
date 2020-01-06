using BusinessLogic.Infrastructure;
using System.Web;

namespace BusinessLogic.Interface
{
    /// <summary>
    /// 檔案處理
    /// </summary>
    public interface IFile
    {
        /// <summary>
        /// 檔案儲存
        /// </summary>
        /// <param name="file">檔案</param>
        /// <param name="fileName">檔名</param>
        /// <returns></returns>
        bool SaveToPath(HttpPostedFileBase file, string fileName);
        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        bool DeleteFile(string fileName);
        /// <summary>
        /// 設定路徑
        /// </summary>
        /// <param name="path"></param>
        void SetPath(string path);

    }
}