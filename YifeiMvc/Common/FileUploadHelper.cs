using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Toeye.Web.Common
{
    public class FileUploadHelper
    {
        public static string FileUpload(HttpPostedFileBase filebase, string path)
        {
            try
            {
                if (filebase.ContentLength == 0) return string.Empty;

                string fileEx = Path.GetExtension(filebase.FileName);
                string fileName = Guid.NewGuid().ToString() + fileEx;

                string dirPath = HttpContext.Current.Server.MapPath(path);

                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                string filePath = dirPath + "\\" + fileName;

                filebase.SaveAs(filePath);

                return path.Replace("~/", "/") + "/" + Path.GetFileName(fileName);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}