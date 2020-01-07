using System.Web;
using BusinessLogic.Interface;

namespace BusinessLogic.Infrastructure
{
    public class ImageFile : DeskFileBase
    {
        /// <summary>
        /// 目錄
        /// </summary>
        private string _path;
        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="fileName">檔名</param>
        /// <returns></returns>
        public override bool DeleteFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return true;
            return Delete(_path + fileName);
        }
        /// <summary>
        /// 儲存
        /// </summary>
        /// <param name="file">檔案</param>
        /// <param name="fileName">檔名</param>
        /// <returns></returns>
        public override bool SaveToPath(HttpPostedFileBase file, string fileName)
        {
            if (file == null)
            {
                return true;
            }
            return Save(_path + fileName, file);
        }
        /// <summary>
        /// 設定路徑
        /// </summary>
        /// <param name="path">路徑</param>
        public override void SetPath(string path)
        {
            _path = path;
        }
    }
}