using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yifei.Framework.Exceptions
{
    /// <summary>
    /// 找不到 返回 404
    /// </summary>
    public class NotFoundException : MessageException
    {
        public NotFoundException(string message) : base(message)

        {

        }
    }
}
