using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yifei.Framework.Logger
{
    /// <summary>
    /// 写文本文件日志实现 ( 可以单例实现 )
    /// </summary>
    public class FileLogWriter : ILogWriter
    {

        /// <summary>
        /// 消息日志存放根目录
        /// </summary>
        private readonly DirectoryInfo _infoLogBaseDirectory;
        /// <summary>
        /// api调用日志存放根目录
        /// </summary>
        private readonly DirectoryInfo _apiLogBaseDirectory;
        /// <summary>
        /// 异常日志存放根目录
        /// </summary>
        private readonly DirectoryInfo _exceptionLogBaseDirectory;
        /// <summary>
        /// 操作日志存放根目录
        /// </summary>
        private readonly DirectoryInfo _operationLogBaseDirectory;

        public FileLogWriter(DirectoryInfo path)
        {
            if (!path.Exists)
            {
                path.Create();
            }

            _infoLogBaseDirectory = new DirectoryInfo(Path.Combine(path.FullName, "info"));
            _apiLogBaseDirectory = new DirectoryInfo(Path.Combine(path.FullName, "api"));
            _exceptionLogBaseDirectory = new DirectoryInfo(Path.Combine(path.FullName, "error"));
            _operationLogBaseDirectory = new DirectoryInfo(Path.Combine(path.FullName, "operation"));
            if (!_infoLogBaseDirectory.Exists)
            {
                _infoLogBaseDirectory.Create();
            }
            if (!_apiLogBaseDirectory.Exists)
            {
                _apiLogBaseDirectory.Create();
            }

            if (!_exceptionLogBaseDirectory.Exists)
            {
                _exceptionLogBaseDirectory.Create();
            }
            if (!_operationLogBaseDirectory.Exists)
            {
                _operationLogBaseDirectory.Create();
            }

        }

        private string CurrentFilename
        {
            get
            {
                var now = DateTime.Now;
                return now.ToString("yyyy") + now.ToString("MM") + now.ToString("dd") + ".log";
            }
        }

        private void WriteFile(FileInfo file, string message)
        {
            using (var sw = file.AppendText())
            {
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : " + message);
            }
        }


        public void Write(string Log)
        {
            var file = new FileInfo(Path.Combine(_infoLogBaseDirectory.FullName, CurrentFilename));
            WriteFile(file, Log);
        }



        public void WriteApiLog(string HttpMethod, string url, string parameters)
        {
            var file = new FileInfo(Path.Combine(_apiLogBaseDirectory.FullName, CurrentFilename));
            var sb = new StringBuilder("\r\n");
            sb.AppendLine($"    method  :   {HttpMethod}");
            sb.AppendLine($"    url     :   {url}");
            sb.AppendLine($"    param   :   {parameters}");
            WriteFile(file, sb.ToString());
        }

        public void WriteExceptionLog(Exception ex)
        {
            var file = new FileInfo(Path.Combine(_exceptionLogBaseDirectory.FullName, CurrentFilename));
            var sb = new StringBuilder("\r\n");
            sb.AppendLine($"    message :   {ex.Message}");
            sb.AppendLine($"    type    :   {ex.GetType().FullName}");
            sb.AppendLine($"    stack   :   {ex.StackTrace}");
            WriteFile(file, sb.ToString());
        }

        public void WriteOperationLog(OperationType operationType, string section, string title, string message, int userid, string ip)
        {
            var file = new FileInfo(Path.Combine(_operationLogBaseDirectory.FullName, CurrentFilename));
            var sb = new StringBuilder("\r\n");
            var operationStr = "";
            switch (operationType)
            {
                case OperationType.View:
                    operationStr = "查看";
                    break;
                case OperationType.Delete:
                    operationStr = "删除";
                    break;
                case OperationType.Modify:
                    operationStr = "修改";
                    break;
                case OperationType.Add:
                    operationStr = "添加";
                    break;
            }
            sb.AppendLine($"    type    :   {operationStr}");
            sb.AppendLine($"    section :   {section}");
            sb.AppendLine($"    title   :   {title}");
            sb.AppendLine($"    message :   {message}");
            sb.AppendLine($"    userid  :   {userid}");
            sb.AppendLine($"    ip      :   {ip}");
            WriteFile(file, sb.ToString());
        }
    }
}
