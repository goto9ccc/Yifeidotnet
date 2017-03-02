using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yifei.Framework.Exceptions
{
    /// <summary>
    /// 身份认证异常 返回 401
    /// </summary>
    public class LoginException : MessageException
    {
        public LoginException(string message) : base(message)
        {

        }
    }
}
