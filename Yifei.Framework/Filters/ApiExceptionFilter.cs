using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;
using Yifei.Framework.Exceptions;

namespace Yifei.Framework.Filters
{


    /// <summary>
    /// 异常过滤器
    /// </summary>
    public class ApiExceptionFilter : IExceptionFilter
    {


        public bool AllowMultiple
        {
            get
            {
                return true;
            }
        }
        public Task ExecuteExceptionFilterAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {

            return Task.Run(() =>
            {
                if (actionExecutedContext.Exception is ParameterException)
                {

                    var exception = actionExecutedContext.Exception as ParameterException;
                    var httpError = new HttpError();
                    httpError.Add("message", exception.Message);
                    httpError.Add("parameterName", exception.ParameterName);
                    actionExecutedContext.ActionContext.Response = actionExecutedContext.ActionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpError);
                    return;
                }
                else if (actionExecutedContext.Exception is NotFoundException)
                {
                    var exception = actionExecutedContext.Exception as NotFoundException;
                    var httpError = new HttpError();
                    httpError.Add("message", exception.Message);
                    actionExecutedContext.ActionContext.Response = actionExecutedContext.ActionContext.Request.CreateErrorResponse(HttpStatusCode.NotFound, httpError);
                    return;
                }
                else if (actionExecutedContext.Exception is LoginException)
                {
                    var exception = actionExecutedContext.Exception as LoginException;
                    var httpError = new HttpError();
                    httpError.Add("message", exception.Message);
                    actionExecutedContext.ActionContext.Response = actionExecutedContext.ActionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, httpError);
                    return;
                }
                else if (actionExecutedContext.Exception is PermissionException)
                {
                    var exception = actionExecutedContext.Exception as PermissionException;
                    var httpError = new HttpError();
                    httpError.Add("message", exception.Message);
                    actionExecutedContext.ActionContext.Response = actionExecutedContext.ActionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, httpError);
                    return;
                }
                else if (actionExecutedContext.Exception is MessageException)
                {
                    var exception = actionExecutedContext.Exception as MessageException;
                    var httpError = new HttpError();
                    httpError.Add("message", exception.Message);
                    actionExecutedContext.ActionContext.Response = actionExecutedContext.ActionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, httpError);
                    return;
                }
                else if (actionExecutedContext.Exception is DbEntityValidationException)
                {
                    var exception = actionExecutedContext.Exception as DbEntityValidationException;
                    var httpError = new HttpError();
                    httpError.Add("message", "数据库字段校验错误");

                    foreach (var dbEntityValidationResult in exception.EntityValidationErrors)
                    {
                        foreach (var dbValidationError in dbEntityValidationResult.ValidationErrors)
                        {
                            httpError.Add(dbValidationError.PropertyName, dbValidationError.ErrorMessage);
                        }
                    }

                    actionExecutedContext.ActionContext.Response =
                        actionExecutedContext.ActionContext.Request.CreateErrorResponse(
                            HttpStatusCode.InternalServerError, httpError);

                }
                else
                {
                    // 不能处理的异常 写日志
                   /* TODO
                    var log = GlobalConfiguration.Configuration.DependencyResolver.GetService(
                        typeof(ILogWriter)) as ILogWriter;
                    log?.WriteExceptionLog(actionExecutedContext.Exception);
                    */
                }
            });
        }

    }
}
