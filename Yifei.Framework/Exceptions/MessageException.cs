using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yifei.Framework.Exceptions
{
    /// <summary>
    /// 消息 返回 400
    /// </summary>
    public class MessageException : Exception
    {
        public MessageException(string message) : base(message)
        {

        }
    }
}
