using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Iata.IS.Business.Common;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Model.Enums;
using log4net;

namespace Iata.IS.Web.Util.Filters
{
    /// <summary>
    /// Following attribute is used to restrict user from updating Invoice, if invoice status is equal to 
    /// "3-ReadyForBilling", "4-Claimed", "5-Processing Complete", "6-Presented" and user has executed CRUD actions 
    /// CMP 400: If invoice has been deleted then it be will restricted.  
    /// </summary>
    public class RestrictInvoiceUpdateAttribute : ActionFilterAttribute
    {

        #region Property

        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Primary Key Identifier of Prime Table(Ex. InvoiceID for INVOICE Table)
        /// </summary>
        public string InvParamName { get; set; }

        /// <summary>
        /// Primary Key Identifier of Related Table with Prime Table(Ex. transactionId for CGO_RM_AWB Table)
        /// </summary>
        public string TransactionParamName { get; set; }

        /// <summary>
        /// Correspondence
        /// </summary>
        public string CorrespondenceParamName { get; set; }

        /// <summary>
        /// Table Name
        /// </summary>
        public TransactionTypeTable TableName { get; set; }

        /// <summary>
        /// Flag for return type of action
        /// </summary>
        public bool IsJson { get; set; }

        /// <summary>
        /// Flag for single or Multiple(','seperated) Invoice Input 
        /// </summary>
        public bool InvList { get; set; }

        /// <summary>
        /// Return Action Name
        /// </summary>
        public string ActionParamName { get; set; }

        /// <summary>
        /// Local property to hold InvoiceID
        /// </summary>
        private Guid? InvoiceId { get; set; }

        /// <summary>
        /// Flag to indicate input InvParamName is Class Type 
        /// </summary>
        private bool _isClassType = false;

        private bool _isCorrespondence = false;
        

        /// <summary>
        /// Flag to indicate output result will be in file type (used for attachment cases)
        /// </summary>
        public bool FileType { get; set; }

        private const string AlreadySubmittedInvCount = "alreadySubmittedInvCount";
        
        private const string AlreadyDeletedInvCount = "alreadyDeletedInvCount";
        
        #endregion

        // CMP 400: isInvRestricted variable can be 0,1,2
        // if isInvRestricted = 0, means invoice/correspondence/transaction can not be edit.
        // if isInvRestricted = 1, means invoice/correspondence/transaction can be edit.
        // if isInvRestricted = 2, means invoice has been deleted.
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                Ioc.Initialize();
                var concurrencyManager = Ioc.Resolve<IConcurrencyManager>();
                int isInvRestricted = 0;
                string invParameterName = string.Empty;
                
                //Case 1: When Invoice and Form C modify by user. 
                if (TableName == TransactionTypeTable.INVOICE || TableName == TransactionTypeTable.PAX_FORM_C)
                {
                    //Case:On Manage screen user can submit multiple invoice at once.
                    //This condition check list of invoices against restricted Invoice status.
                    if (InvList)
                    {
                        var invoiceId = filterContext.ActionParameters[InvParamName].ToString();
                        var invoiceIdList = invoiceId.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        string validInvList = string.Empty;
                        int inValidInvCount = 0;
                        int invDeletedCount = 0;

                        foreach (var invoice in invoiceIdList)
                        {
                            var result = concurrencyManager.IsInvoiceRestricted(new Guid(invoice), null, null, null,
                                                                                TableName.ToString());
                            if ((int)InvoiceEditStatus.NoneEditable == result)
                            {
                                inValidInvCount++;

                            }
                            else if ((int)InvoiceEditStatus.InvoiceDeleted == result)
                            {
                                invDeletedCount++;

                            }
                            else
                            {
                                validInvList += invoice + ",";
                            }
                        }

                        //modify list of invoices if required and return count of restricted invoices.
                        filterContext.ActionParameters[InvParamName] = validInvList;
                        filterContext.ActionParameters[AlreadySubmittedInvCount] = inValidInvCount;
                        filterContext.ActionParameters[AlreadyDeletedInvCount] = invDeletedCount;
                    }
                    else
                    {
                        //Case: get invoice property value, if InvParamName contains className.PropertyName 
                        var invParamCollection = InvParamName.Split('.');
                       
                        if (invParamCollection.Length > 1)
                        {
                            // [0] class name and [1] invoice parameter name
                            var className = invParamCollection[0];
                            invParameterName = invParamCollection[1];

                            Type type = filterContext.ActionParameters[className].GetType();
                            PropertyInfo property = type.GetProperty(invParamCollection[1]);
                            InvoiceId = new Guid(property.GetValue(filterContext.ActionParameters[className], null).ToString());
                            _isClassType = true;
                        }
                        else
                        {
                            InvoiceId = new Guid(filterContext.ActionParameters[InvParamName].ToString());
                        }

                        isInvRestricted = concurrencyManager.IsInvoiceRestricted(InvoiceId, null,null,null, TableName.ToString());
                    }
                }
                 //Case 2: When Correspondence modify by user
                else if (TableName == TransactionTypeTable.PAX_CORRESPONDENCE || TableName == TransactionTypeTable.CGO_CORRESPONDENCE || TableName == TransactionTypeTable.MU_CORRESPONDENCE)
                {
                    var transactionId = Guid.Empty;
                    if (filterContext.ActionParameters.ContainsKey("transactionId"))
                    {
                        transactionId =
                        string.IsNullOrWhiteSpace(filterContext.ActionParameters[TransactionParamName].ToString())
                            ? Guid.Empty
                            : Guid.Parse(filterContext.ActionParameters[TransactionParamName].ToString());
                    }

                    var type = filterContext.ActionParameters[CorrespondenceParamName].GetType();

                    const string corrParameterNameStage = "CorrespondenceStage";
                    var propertyStage = type.GetProperty(corrParameterNameStage);
                    var corrStage = Int32.Parse(propertyStage.GetValue(filterContext.ActionParameters[CorrespondenceParamName], null).ToString());

                    const string corrParameterNameNo = "CorrespondenceNumber";
                    var propertyNo = type.GetProperty(corrParameterNameNo);
                    var corrNo = Int64.Parse(propertyNo.GetValue(filterContext.ActionParameters[CorrespondenceParamName], null).ToString());

                    _isClassType = true;
                    _isCorrespondence = true;
                    
                    //if (!string.IsNullOrWhiteSpace(InvParamName))
                    //InvoiceId =  new Guid(filterContext.ActionParameters[InvParamName].ToString());


                    isInvRestricted = transactionId != Guid.Empty
                                          ? concurrencyManager.IsInvoiceRestricted(null, transactionId, null, null,
                                                                                   TableName.ToString())
                                          : concurrencyManager.IsInvoiceRestricted(null, null, corrNo, corrStage,
                                                                                   TableName.ToString());
                }
                else if (TableName == TransactionTypeTable.PAX_REJECTION_MEMO || TableName == TransactionTypeTable.PAX_COUPON_RECORD || TableName == TransactionTypeTable.PAX_RM_COUPON_BREAKDOWN || TableName == TransactionTypeTable.CGO_AIR_WAY_BILL)
                {
                    //Getting Invoice id from query string for FormXF and FormF to check status of invoice. Earlier transaction id was using to check status, but due to excess load
                    //on table data was fetching very slowly. If no invoiceid found in query string then previous functionality will work in case of PAX_REJECTION_MEMO table.
                    string queryString = filterContext.HttpContext.Request.RawUrl;
                    var transactionId = new Guid(filterContext.ActionParameters[TransactionParamName].ToString());
                    Guid? invoiceId =null;
                    try
                    {
                        string startElement = string.Empty;
                        string endElement = string.Empty;
                        int pos = 0;
                        if (queryString.ToUpper().Contains("FORMXF"))
                        {
                            //Pax/Receivables/FormXF/f740d5d8d123d985e040010a8d022410/RMEdit/0a125dd92619266ae040010a8d02388a/RMCouponEdit/0a125dd9-2819-266a-e040-010a8d02388a
                            startElement = "FormXF";
                            //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
                            //Added  queryString.ToUpper().Contains("RMEDIT") condition to get invoice id 
                            if (queryString.ToUpper().Contains("RMCOUPONEDIT") || queryString.ToUpper().Contains("RMEDIT"))
                                endElement = "RMEdit";
                            else
                                endElement = "RMDelete";
                            pos = 7;
                        }
                        else if (queryString.ToUpper().Contains("FORMF"))
                        {
                            startElement = "FormF";
                            //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues
                            //Added  queryString.ToUpper().Contains("RMEDIT") condition to get invoice id 
                            if (queryString.ToUpper().Contains("RMCOUPONDELETE") || queryString.ToUpper().Contains("RMCOUPONCREATE") || queryString.ToUpper().Contains("RMEDIT"))
                            {
                                endElement = "RMEdit"; 
                            }
                            else
                            {
                                endElement = "RMDelete";
                            }
                            pos = 6;
                        }
                        else if (queryString.ToUpper().Contains("PRIMEBILLINGDELETE"))
                        {
                            startElement = "Invoice";
                            endElement = "PrimeBillingDelete";
                            pos = 8;
                        }
                        else if (queryString.ToUpper().Contains("RMDELETE"))
                        {
                            startElement = "Invoice";
                            endElement = "RMDelete";
                            pos = 8;
                        }
                        else if (queryString.ToUpper().Contains("RMCOUPONDELETE"))
                        {
                            startElement = "Invoice";
                            endElement = "RMEdit";
                            pos = 8;
                        }
                        else if (queryString.ToUpper().Contains("AWBPREPAIDRECORDDELETE"))
                        {
                            startElement = "Invoice";
                            endElement = "AwbPrepaidRecordDelete";
                            pos = 8;
                        }                        
                        if (!string.IsNullOrEmpty(startElement))
                            invoiceId = new Guid(queryString.Substring(queryString.IndexOf(startElement) + pos, (queryString.IndexOf(endElement) - queryString.IndexOf(startElement) - (pos+1))).Replace("-", ""));                        
                    }
                    catch { //do nothing, so old functionality will start working if found any change in the URL.
                    }
                    isInvRestricted = concurrencyManager.IsInvoiceRestricted(invoiceId, transactionId, null, null, TableName.ToString());
                }
                //Case 3: When invoice modify at transaction level by user.
                else
                {
                    //Case: Some time transactionId provided. By Id we get status of invoice. ex Delete coupon. 
                    var transactionId = new Guid(filterContext.ActionParameters[TransactionParamName].ToString()); 
                    isInvRestricted = concurrencyManager.IsInvoiceRestricted(null, transactionId, null, null, TableName.ToString());
                }

                if ((isInvRestricted == (int)InvoiceEditStatus.NoneEditable || isInvRestricted == (int)InvoiceEditStatus.InvoiceDeleted) && !InvList)
                {
                    var message = string.Empty;

                    if (isInvRestricted == (int)InvoiceEditStatus.NoneEditable)
                    {
                        message = _isCorrespondence
                                      ? "This Correspondence has already been updated by another user, please go back to billing history screen and try again."
                                      : "This Invoice is no more eligible for Add/Update/Delete!";

                        //redirect to view correspondence with invoiceid parameter...solved sendcorrespondence error
                        //code changed by Arjun / Mrugaja (10-Jun-13)
                        if(filterContext.ActionParameters.ContainsKey("transactionId"))
                        {
                          //SCPID : 231381 - Invoice missing from IS-Web
                          try
                          {
                            filterContext.ActionParameters.Add("invoiceId", filterContext.ActionParameters["transactionId"]);
                          }
// ReSharper disable EmptyGeneralCatchClause
                          catch
// ReSharper restore EmptyGeneralCatchClause
                          {
                            
                          }
                        }
                    }
                    else if (isInvRestricted == (int)InvoiceEditStatus.InvoiceDeleted)
                    {
                        message = "This invoice has been deleted.";
                    }

                    //Case: for ajax submit request then runtime will return message in json.
                    if (IsJson)
                    {
                        if (FileType)
                        {
                            filterContext.Result = new FileUploadJsonResult
                                                       {
                                                           Data =
                                                               new
                                                                   {
                                                                       IsFailed = true,
                                                                       Message = message,
                                                                       Attachment = new List<string>(),
                                                                       Length = 0
                                                                   }
                                                       };
                        }
                        else if (TableName == TransactionTypeTable.VALIDATION_EXCEPTION_SUMMARY)
                        {
                              //if message to be change then modify message at ValidationErrorCorrection.js file of PAX,CARGO,MISC,UATP
                               filterContext.Result = new JsonResult() { Data = "InvoiceDeleted" };
                        }
                        else
                        {
                            filterContext.Result = new JsonResult
                                                       {
                                                           Data = new MessageDetail
                                                                      {
                                                                          IsFailed = true,
                                                                          Message = message,
                                                                          ErrorCode = "1",
                                                                          Id = 0,
                                                                          isRedirect = false,
                                                                          LineItemDetailExpected = false,
                                                                          RedirectUrl = string.Empty
                                                                      },
                                                           JsonRequestBehavior = JsonRequestBehavior.AllowGet
                                                       };
                        }
                    }
              
                    else
                    {
                        //Case: for post submit request then runtime will redirect to respective get method.
                        var actionName = ActionParamName ??
                                         filterContext.Controller.ControllerContext.RouteData.Values["Action"];
                        filterContext.Controller.TempData[ViewDataConstants.ErrorMessage] = message;

                        IDictionary<string, object> paramList = new Dictionary<string, object>();

                        paramList.Add("controller", filterContext.Controller.ControllerContext.RouteData.Values["Controller"]);
                        paramList.Add("action", actionName);
                        paramList.Add("area", filterContext.Controller.ControllerContext.RouteData.DataTokens["area"]);

                        if (_isClassType)
                        {
                            if (!string.IsNullOrWhiteSpace(InvParamName))
                            paramList.Add(invParameterName, InvoiceId);

                            if(_isCorrespondence)
                            {
                                foreach (var param in filterContext.ActionParameters)
                                {
                                    paramList.Add(param.Key, param.Value);
                                }
                            }
                        }
                        else
                        {
                            foreach (var param in filterContext.ActionParameters)
                            {
                                paramList.Add(param.Key,param.Value);
                            }
                        }
                        
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(paramList));
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.InfoFormat("Error on RestrictInvoiceUpdateAttribute : {0}", ex);
            }

            base.OnActionExecuting(filterContext);
        }

    }// end RestrictInvoiceUpdateAttribute class


    public class MessageDetail
    {
        public string ErrorCode { get; set; }

        public string Message { get; set; }

        public bool IsFailed { get; set; }

        public string RedirectUrl { get; set; }

        public bool isRedirect { get; set; }

        public int Id { get; set; }

        public bool LineItemDetailExpected { get; set; }
        
    }
}// end Iata.IS.Web.Util.Filters namespace

