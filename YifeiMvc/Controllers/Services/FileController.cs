using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Yifei.Framework.Common;
using Yifei.Framework.Exceptions;

namespace YifeiMvc.Controllers.Services
{
    [RoutePrefix("Service/file")]
    public class FileController : BaseApiController
    {

        [Route("upload")]
        [HttpPost]
        public List<object> Update()
        {
            if (HttpContext.Current.Request.Files.Count <= 0)
            {
                throw new MessageException("请选择至少一个文件进行上传");
            }

            var basepath = ConfigurationManager.AppSettings["filepath"];
            if (string.IsNullOrEmpty(basepath))
            {
                throw new MessageException("请设置文件存放路径参数");
            }
            var date = DateTime.Now;
            var path = new DirectoryInfo(Path.Combine(basepath, date.ToString("yyyy"),
                date.ToString("MM")));
            if (!path.Exists)
            {
                path.Create();
            }


            var result = new List<object>();

            for (var i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                HttpPostedFile file = HttpContext.Current.Request.Files[i];
                if (file != null)
                {
                    var filename = Guid.NewGuid().ToString("N").ToLower()
                        + Path.GetExtension(file.FileName);
                    var truefilename = file.FileName;
                    var obj = new
                    {
                        Filename = truefilename,
                        Url = "/Service/file/download/"
                        + date.ToString("yyyy") + "/" + date.ToString("MM") + "/" + filename
                    };
                    file.SaveAs(path.FullName + "\\" + filename);
                    result.Add(obj);
                    //FileService.AddFile(obj.Filename, obj.Url);
                }
            }
            return result;

        }


        // <summary>
        // 下载文件接口
        // </summary>
        // <returns></returns>
        [Route("download/{year}/{month}/{filename}")]
        [HttpGet]
        public HttpResponseMessage Download(string year, string month, string filename)
        {
            var basepath = ConfigurationManager.AppSettings["filepath"];
            if (string.IsNullOrEmpty(basepath))
            {
                throw new MessageException("请设置文件存放路径参数");
            }
            var file = new FileInfo(Path.Combine(basepath, year, month, filename));
            if (!file.Exists)
            {
                throw new MessageException("找不到文件");
            }
            HttpResponseMessage result = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            using (var stream = file.OpenRead())
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                result.Content = new ByteArrayContent(bytes);
            }

            var contentType = System.Web.MimeMapping.GetMimeMapping(file.FullName);
            result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            return result;
        }
    }
}