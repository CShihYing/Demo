using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using BusinessLogic.Interface;

namespace BusinessLogic.Infrastructure
{
    public abstract class DeskFileBase : IFile
    {
        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="fileName">檔案名稱</param>
        /// <returns></returns>
        public abstract bool DeleteFile(string fileName);
        /// <summary>
        /// 儲存
        /// </summary>
        /// <param name="file">檔案</param>
        /// <param name="fileName">檔名</param>
        /// <returns></returns>
        public abstract bool SaveToPath(HttpPostedFileBase file, string fileName);
        /// <summary>
        /// 設定路徑
        /// </summary>
        /// <param name="path">路徑</param>
        public abstract void SetPath(string path);

        /// <summary>
        /// 刪除檔案
        /// </summary>
        /// <param name="fullPatch">完整路徑(含檔名)</param>
        /// <returns></returns>
        protected bool Delete(string fullPatch)
        {
            if (File.Exists(fullPatch))
            {
                try
                {
                    File.Delete(fullPatch);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            else return false;
            return true;
        }
        /// <summary>
        /// 存檔
        /// </summary>
        /// <param name="fullPatch">完整路徑(含檔名</param>
        /// <param name="file">檔案</param>
        /// <returns></returns>
        protected bool Save(string fullPatch, HttpPostedFileBase file)
        {
            int lastIndex;
            if (string.IsNullOrWhiteSpace(fullPatch))
            {
                return false;
            }
            else
            {
                lastIndex = fullPatch.LastIndexOf(Path.DirectorySeparatorChar.ToString()) + 1;
            }

            if (!Directory.Exists(fullPatch.Substring(0, lastIndex)) && lastIndex!=0)
            {
                Directory.CreateDirectory(fullPatch.Substring(0, lastIndex));
            }
            file.SaveAs(fullPatch);
            return true;
        }
    }
}