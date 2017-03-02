using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yifei.Framework.Exceptions
{
    /// <summary>
    /// 参数异常 返回 400
    /// </summary>
    public class ParameterException : MessageException
    {

        public string ParameterName { get; set; }

        public ParameterException(string parameterName, string message) : base(message)
        {
            ParameterName = parameterName;
        }
    }
}
