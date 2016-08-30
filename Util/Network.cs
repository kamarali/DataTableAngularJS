using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using Iata.IS.Core.Network;
using log4net.Repository.Hierarchy;

namespace Iata.IS.Web.Util
{
    public class Network
    {
        public static WindowsImpersonationContext _impersonateContext;
        /// <summary>
        /// 
        /// </summary>
        public static WindowsImpersonationContext ImpersonateUser()
        {
            HttpContext context = HttpContext.Current;

            /* Get the service provider from the context */
            IServiceProvider iServiceProvider = context as IServiceProvider;

            /*Get a Type which represents an HttpContext */
            Type httpWorkerRequestType = typeof(HttpWorkerRequest);

            /* Get the HttpWorkerRequest service from the service provider 
            NOTE: When trying to get a HttpWorkerRequest type from the 
            HttpContext unmanaged code permission is demanded. */

            HttpWorkerRequest httpWorkerRequest =
            iServiceProvider.GetService(httpWorkerRequestType) as HttpWorkerRequest;

            /* Get the token passed by IIS */
            IntPtr ptrUserToken = httpWorkerRequest.GetUserToken();

            /* Create a WindowsIdentity from the token */
            WindowsIdentity winIdentity = new WindowsIdentity(ptrUserToken);

            /* Impersonate the user */
            _impersonateContext = winIdentity.Impersonate();
            return _impersonateContext;
        }

        public static void UndoImpersonation()
        {
            _impersonateContext.Undo();
        }

    }
}
