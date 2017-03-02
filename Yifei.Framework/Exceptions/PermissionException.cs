using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yifei.Framework.Exceptions
{
    /// <summary>
    /// 权限 返回 403
    /// </summary>
    public class PermissionException : MessageException
    {
        public PermissionException(string message) : base(message)
        {

        }
    }
}
