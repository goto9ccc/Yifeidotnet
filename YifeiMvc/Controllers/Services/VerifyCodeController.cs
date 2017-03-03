using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Yifei.Framework.Common;
using Yifei.Framework.Exceptions;
using Yifei.Framework.Utility;
/// <summary>
/// 验证码服务
/// </summary>
namespace YifeiMvc.Controllers.Services
{

    [RoutePrefix("Service/verifycode")]
    public class VerifyCodeController : BaseApiController
    {

        [Route("build")]
        [HttpGet]
        public HttpResponseMessage GetCode(int? width, int? height, int? fontSize)
        {

            int widths = width.HasValue ? width.Value : 100;
            int heights = height.HasValue ? height.Value : 40;
            int fontsize = fontSize.HasValue ? fontSize.Value : 20;
            string code = string.Empty;
            byte[] bytes = ValidateCode.CreateValidateGraphic(out code, 4, widths, heights, fontsize);
            HttpContext.Current.Session["verifycode"] = code;
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(bytes);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            return result;
        }

        [Route("check")]
        [HttpGet]
        public string CheckCode(string code)
        {
            if (HttpContext.Current.Session["verifycode"] == null)
            {
                throw new MessageException("请先生成验证码");
            }
            if (string.IsNullOrEmpty(code))
            {
                throw new ParameterException("code", "code 验证码参数不能为空");
            }
            if (HttpContext.Current.Session["verifycode"].ToString().ToLower() != code)
            {
                throw new MessageException("验证码不正确");
            }
            return "ok";
        }
    }
}