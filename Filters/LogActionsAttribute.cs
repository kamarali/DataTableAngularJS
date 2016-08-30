using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Security.Principal;
using System.Web.Mvc;
using Iata.IS.Core.Network;
using log4net;
using System.Web;

namespace Iata.IS.Web.Util.Filters
{
    /// <summary>
    /// Logs actions from controllers when the action is going to be executed and after the action is executed.
    /// To use this filter decorate the controller with this attribute.
    /// </summary>
    public class LogActionsAttribute : ActionFilterAttribute
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private const string ProcessingStartTime = "ProcessingTime";
        private const string ActionName = "ActionName";
        private const string ControllerName = "ControllerName";
        private const string Log4NetPropController = "controller";
        private const string Log4NetPropAction = "action";
        private const string Log4NetPropMethod = "method";
        private const string Log4NetPropProcessingTime = "processing_time";
        private const string Log4NetPropUserId = "user_id";


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Network.ImpersonateUser();

            // Add the current date time to the http context.
            filterContext.HttpContext.Items.Add(ProcessingStartTime, DateTime.UtcNow);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.HttpContext.Items.Add(ActionName, filterContext.ActionDescriptor.ActionName);
            filterContext.HttpContext.Items.Add(ControllerName, filterContext.ActionDescriptor.ControllerDescriptor.ControllerName);
            Network.UndoImpersonation();
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);

            try
            {
                // Retrieve the date and time from the http context and subtract the current time from the same.
                var processingEndTime = DateTime.UtcNow;
                var processingStartTime = Convert.ToDateTime(filterContext.HttpContext.Items[ProcessingStartTime]);
                var actionName = filterContext.HttpContext.Items[ActionName];
                var controllerName = filterContext.HttpContext.Items[ControllerName];
                
                var userId = 0;
                try
                {
                  userId =(HttpContext.Current.Session["userId"]) == null ? 0 : Convert.ToInt32(HttpContext.Current.Session["userId"]); // (int)SessionUtil.UserId;
                }
                catch (ISSessionExpiredException isSessionExpired)
                {
                    userId = 0;
                }
                // Set the logical thread context of log4net.
                LogicalThreadContext.Properties[Log4NetPropController] = controllerName;
                LogicalThreadContext.Properties[Log4NetPropAction] = actionName;
                LogicalThreadContext.Properties[Log4NetPropMethod] = filterContext.RequestContext.HttpContext.Request.HttpMethod;
                LogicalThreadContext.Properties[Log4NetPropProcessingTime] = (processingEndTime - processingStartTime).TotalMilliseconds;
                LogicalThreadContext.Properties[Log4NetPropUserId] = String.IsNullOrEmpty(userId.ToString()) ? (object) "0" : userId;

                // Log the message. We use the rolling-file appender as well as the ADO appender.
                Logger.InfoFormat(CultureInfo.InvariantCulture, "Processing time: [{3}] for action [{0}] of controller [{1}] using [{2}] by user [{4}]",
                                    actionName,
                                    controllerName,
                                    filterContext.RequestContext.HttpContext.Request.HttpMethod,
                                    (processingEndTime - processingStartTime).TotalMilliseconds,
                                    userId);
                
                
            }
            //FW: Request Id ##281423## 
            catch (ISSessionExpiredException isSessionExpired)
            {
              Logger.Error("Error calculating processing time.", isSessionExpired);
              throw new ISSessionExpiredException("Session expired.");
            }
            catch (Exception exception)
            {
                Network.UndoImpersonation();
                Logger.Error("Error calculating processing time.", exception);
            }
        }
    }
}