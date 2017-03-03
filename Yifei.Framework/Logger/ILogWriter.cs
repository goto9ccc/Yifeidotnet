using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yifei.Framework.Logger
{
    public interface ILogWriter
    {
        /// <summary>
        /// 写入文本日志 ( 信息 )
        /// </summary>
        /// <param name="Log"></param>
        void Write(string Log);

        /// <summary>
        /// 写入 Web 访问日志
        /// </summary>
        /// <param name="HttpMethod">http 方法</param>
        /// <param name="url">url 名称</param>
        /// <param name="parameters">参数</param>
        void WriteApiLog(string HttpMethod, string url, string parameters);


        /// <summary>
        /// 写入 异常 日志
        /// </summary>
        /// <param name="ex">异常</param>
        void WriteExceptionLog(Exception ex);

        /// <summary>
        /// 写入操作日志
        /// </summary>
        /// <param name="operationType">操作类型</param>
        /// <param name="section">模块/章节</param>
        /// <param name="title">标题</param>
        /// <param name="message">详细描述</param>
        /// <param name="userid">操作用户Id</param>
        /// <param name="ip">ip 地址</param>
        void WriteOperationLog(OperationType operationType, string section, string title, string message, int userid, string ip);


    }

    /// <summary>
    /// 操作类型
    /// </summary>
    public enum OperationType
    {
        /// <summary>
        /// 查看
        /// </summary>
        View,
        /// <summary>
        /// 添加
        /// </summary>
        Add,
        /// <summary>
        /// 修改
        /// </summary>
        Modify,
        /// <summary>
        /// 删除
        /// </summary>
        Delete
    }

}
