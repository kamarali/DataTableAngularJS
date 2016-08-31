using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Devart.Data.Oracle;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.Core;
using Iata.IS.Core.Configuration;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data.ExternalInterfaces.ICHInterface;
using Iata.IS.Data.Impl;
using Iata.IS.Data.SetInvoiceStatus;
using Iata.IS.Model.BroadcastMessages;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.ExternalInterfaces.ICHInterface;
using Iata.IS.Model.Fdr;
using log4net;
using System.Linq;
using Iata.IS.Business.SISGatewayService;
using NVelocity;
using Iata.IS.Core.DI;
using Castle.Core.Smtp;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Model.Common;
using Iata.IS.Data;
using Iata.IS.Model.Enums;
using System.Net.Mail;
using System.Xml.Linq;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Common;
using System.Globalization;
using Iata.IS.AdminSystem;

namespace Iata.IS.Business.ExternalInterfaces.ICHInterface.Impl
{
  public class ICHInterfaceHandler : IICHInterfaceHandler
  {
    public IICHInterfaceRepository ICHInterfaceRepository { get; set; }

    public ISetInvoiceStatus IsetInvoiceStatus { get; set; }

    public ICalendarManager CalendarManager { get; set; }

    public IICHXmlHandler ICHXmlHandler { get; set; }

    public IICHUpdateHandler ICHUpdateHandler { get; set; }

    public IEmailSender EmailSender { get; set; }

    public ITemplatedTextGenerator TemplatedTextGenerator { get; set; }

    public IBroadcastMessagesManager BroadcastMessagesManager { get; set; }

    /// <summary>
    /// Gets or sets the exchange rate repository.
    /// </summary>
    /// <value>The exchange rate repository.</value>
    public IRepository<ExchangeRate> ExchangeRateRepository { get; set; }

    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private string _errorCode;

    private readonly StringBuilder _webServiceResponse = new StringBuilder();

    private DateTime _submissionDate;

    private static SISGatewayServiceClient _sisGatewayServiceClient;

    private List<ICHSettlementData> _ichSettlementDataList;

    private string _setInvoiceStatus = string.Empty;

    private const int MultipleBillingPeriods = 1;
    private const int InvalidBillingPeriod = 2;
    private const int ValidBillingPeriod = 3;
    private const string ichSettlementXSDFolder = @"App_Data\SchemaFiles\ICH-Settlement.xsd"; 
     

    //public ICHInterfaceHandler(ISetInvoiceStatus _setinvoicestatus)
    //{
    //  IsetInvoiceStatus = _setinvoicestatus;
    //}
    #region IICHInterfaceHandler interface method implementation
    /// <summary>
    /// Gets invoice data corresponding to billing period passed
    /// </summary>
    /// <param name="billingPeriod">Billing period (Should be curent or previous billing period)</param>
    /// <returns></returns>
    public List<CrossCheckRequestInvoice> GetICHCrossCheckRequestData(string billingPeriod, DateTime startDateTime, DateTime endDateTime)
    {
      try
      {
        //Format billing period value passed
        var billingPeriodPassed = new BillingPeriod(Convert.ToInt16(billingPeriod.Substring(0, 4)), Convert.ToInt16(billingPeriod.Substring(4, 2)), Convert.ToInt16(billingPeriod.Substring(6, 2)));

        ICHInterfaceRepository = Ioc.Resolve<IICHInterfaceRepository>();

        try
        {
          // Call Ich interface repository method which will return invoices corresponding to passed bvilling period
          _logger.Info(string.Format("ICHInterfaceRepository instance is: [{0}]", ICHInterfaceRepository != null ? "NOT NULL" : "NULL"));

          if (ICHInterfaceRepository != null)
          {
            var crossCheckInvoiceData = ICHInterfaceRepository.GetICHCrossCheckRequestData(billingPeriodPassed, startDateTime, endDateTime);
            return crossCheckInvoiceData;
          }
        }
        catch (ISBusinessException ex)
        {
          _logger.Info(string.Format("In catch block {0} - {1}", ex.ErrorCode, ex.InnerException));
        }

      }
      catch (Exception ex)
      {
        _logger.Error("Generic Exception", ex);
      }
      return null;
    }

    /// <summary>
    ///  Get invoice data for invoice IDs passed in the request
    /// </summary>
    /// <param name="invoiceIdList">List of invoice IDs for which data needs to be fetched</param>
    /// <param name="billingPeriod">BillingPeriod corresponding to invoice ids passed</param>
    /// <returns>XML string corresponding invoice data generated for invoice ids passed</returns>
    public string GetICHSettlementDataforResend(List<string> invoiceIdList, string billingPeriod)
    {
      string ichSettlementXml = null;
      // Call business layer method for generating invoice data for invoice ids passed
      var lstICHSettlementData = new List<ICHSettlementData>();

      if (invoiceIdList.Count > 0)
      {
        int count = 0;
        while (count < invoiceIdList.Count)
        {
          var invoiceIds = invoiceIdList.Skip(count).Take(120).ToList();
          count += invoiceIds.Count;
          lstICHSettlementData.AddRange(ICHInterfaceRepository.GetICHSettlementDataForResend(String.Join(",", invoiceIds.ToArray())));
        }

      }

      _ichSettlementDataList = lstICHSettlementData;
      try
      {
        if (lstICHSettlementData.Count > 0)
        {
          // Validate whether all invoices returned either belong to current or previous billing period
          int billingPeriodValidationResult = ValidateBillingPeriodforResendRequest(lstICHSettlementData);

          if (billingPeriodValidationResult == 3)
          {
            // Generate XML corresponding to invoice id list passed
            // Generate ICHSettlementXML
            ichSettlementXml = GenerateICHSettlementXML(billingPeriod).Trim();

            _logger.Info("Settlement XML:");
            _logger.Info(ichSettlementXml);

            if (string.IsNullOrEmpty(ichSettlementXml))
            {
              ichSettlementXml = "Error";
            }
            else
            {
              //Validate generated xml against xsd...Ideally, this code should present inside GenerateCHSettlementXML method
              _logger.Info("Before validating Member profile update XML");

              var XSDPath = string.Format("{0}{1}",ConnectionString.GetAppSetting("AppSettingPath"), ichSettlementXSDFolder);
              string sValResult = ICHXmlHandler.Validate(ichSettlementXml, XSDPath); // ConfigurationManager.AppSettings.Get("ICHSettlementSchemaFile")

              _logger.Info("Validation Result");
              _logger.Info(sValResult);




              // Get details from future update
              if (sValResult != "OK")
              {
                var invalidXml = ichSettlementXml;
                ichSettlementXml = "Error";

                //Send mail to ISAdmin/SIS Support to notify that generated XML has validation errors
                ICHUpdateHandler.SendAlertForXmlValidationFailure(0, invalidXml, "Resend Invoices", sValResult);
              }
            }

          }
          else
          {
            // Indicates current or previous billing Period is not passed or invoices corresponding to 
            //more than one distinct billing periods are present in result set
            ichSettlementXml = "Invalid Billing Period";
          }
        }
      }
      catch (Exception ex)
      {
        _logger.Error("Error occurred while generating ICHSettlement XML for settlement.", ex);
        // throw ex;
      }// End catch
      return ichSettlementXml;
    }// End GetICHSettlementDataforResend()

    /// <summary>
    /// Gets ICHSettlement Data and send it for settlement.
    /// </summary>
    public void GenerateAndSendICHSettlementXML()
    {
        var m = new OracleMonitor { IsActive = true };
        _submissionDate = DateTime.UtcNow;
      try
      {

        // Verify and log CalendarManager for null value.
        _logger.Info(String.Format("CalendarManager instance is: [{0}]", (CalendarManager == null) ? "NULL" : "NOT NULL"));

        // Get current billing period.
        // ReSharper disable PossibleNullReferenceException
        var curBillingPeriod = CalendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
        // ReSharper restore PossibleNullReferenceException

        // Get previous billing period.
        var prevBillingPeriod = CalendarManager.GetLastClosedBillingPeriod(ClearingHouse.Ich);


        // Get Max number of invoice in SettlementXML.
        int maxInvoiceInSettlement = SystemParameters.Instance.ICHDetails.MaxNumberOfInvoicesInIchSettlementFile;

        // Log MaxInvoiceInSettlement value.
        _logger.Info(String.Format("Max number of invoices in settlement xml: {0}.", maxInvoiceInSettlement));

        // Get List of ICHSettlementData.
        _ichSettlementDataList = GetICHSettlementData(curBillingPeriod, prevBillingPeriod, SystemParameters.Instance.ICHDetails.MaxNumberOfInvoicesInIchSettlementFile); // maxInvoiceInSettlement

        // Check that the list has data.
        if (_ichSettlementDataList != null && _ichSettlementDataList.Count() > 0)
        {

          var invoices = _ichSettlementDataList.Select(c => c.UniqueInvoiceNumber).Distinct().ToList();

          // Logic to convert all the UniqueInvoiceNumber in Byte[] to string.
          _setInvoiceStatus = string.Join(",", invoices.Select(invoice => ConvertUtil.ConvertGuidToString(invoice)).ToArray());

          _submissionDate = DateTime.UtcNow;

          IsetInvoiceStatus.SetStatusOfInvoices(_setInvoiceStatus, "P", "ICH", _submissionDate);

          //CMP626: Future Submission for MISC and Provisional Settlement with ICH
          //Get Distinct ClearancePeriods 
          var clearancePeriods =_ichSettlementDataList.Select(c => c.ClearancePeriod).Distinct().Select(cP => DateTime.ParseExact(cP, "yyyyMMdd", CultureInfo.InvariantCulture)).ToList();
          var currentBillingPeriodString = String.Format("{0:0000}", curBillingPeriod.Year) + String.Format("{0:00}", curBillingPeriod.Month) + String.Format("{0:00}", curBillingPeriod.Period);
          
          // Send previous billing period invoices for Settlement.
          // If late submission window for last closed billing period is open, only then send invoices of previous billing period

          _logger.Info(String.Format("Check whether late submission window is open for previously closed billing period: [{0}],[{1}],[{2}]", prevBillingPeriod.Year, prevBillingPeriod.Month, prevBillingPeriod.Period));
          if (CalendarManager.IsLateSubmissionWindowOpen(ClearingHouse.Ich, prevBillingPeriod))
          {
            GenerateAndSendXML(prevBillingPeriod);
          }
          // Send current billing period invoices for Settlement.
          GenerateAndSendXML(curBillingPeriod);

          // Send future billing period invoices for Settlement.
          foreach (var clearancePeriod in clearancePeriods)
          {
            if (clearancePeriod > (DateTime.ParseExact(currentBillingPeriodString, "yyyyMMdd", CultureInfo.InvariantCulture)))
            {
              var futureBillingPeriod = new BillingPeriod
                                          {
                                            ClearingHouse = curBillingPeriod.ClearingHouse,
                                            Year = clearancePeriod.Year,
                                            Month = clearancePeriod.Month,
                                            Period = clearancePeriod.Day
                                          };
              GenerateAndSendXML(futureBillingPeriod);
            }

          }

        }// End if

      }// End try
      catch (Exception ex)
      {
        // Log Error.
        _logger.Error("Error occurred in generating and Sending ICHSettlementXML for settlement.", ex);

        // Check that the list has data.
        if (_ichSettlementDataList != null && _ichSettlementDataList.Count() > 0)
        {
          IsetInvoiceStatus.SetStatusOfInvoices(_setInvoiceStatus, "E", "ICH", _submissionDate);
        }

        // Throw ISBusinessException indicating failure of GenerateAndSendICHSettlementXML.
        throw new ISBusinessException(ErrorCodes.GenerateAndSendICHSettlementXMLFailed, "Error occurred in generating and Sending ICHSettlementXML for settlement.");

      }// End catch


    }// End GenerateAndSendICHSettlementXML()

    public void SisWebserviceErrorNotification()
    {
        _logger.Info("Email send process started.");
        
        // Ioc.Initialize();
        try
        {
            // Get an object of the EmailSender component
            var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
           // var TemplatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
            // Get an instance of email settings repository
            var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
            // Verify and log TemplatedTextGenerator for null value.
            _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

            // Generate email body text for ICH settlement web service failure notification email
            var context = new VelocityContext();
            context.Put("FailureTime", DateTime.UtcNow.ToShortTimeString());
            context.Put("EmailContent", "Authentication failed while invoking SIS web-service ");
            context.Put("Comments", "SIS Webservice Connection failure due to wrong username and password.");
            context.Put("EmailSignature", "This is a system generated message - please do not reply.");
            context.Put("n", "\n");
            // Verify and log TemplatedTextGenerator for null value.
            _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

            var emailSettingForISAdminAlert = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.SisWebserviceErrorNotification);

            // Generate email body text for own profile updates contact type mail
            if (TemplatedTextGenerator != null)
            {
                var emailToISAdminText = TemplatedTextGenerator.GenerateTemplatedText(EmailTemplateId.SisWebserviceErrorNotification, context);
                // Create a mail object to send mail
                var msgForISAdmin = new MailMessage
                {
                    From = new MailAddress(emailSettingForISAdminAlert.SingleOrDefault().FromEmailAddress),
                    IsBodyHtml = true
                };

                var subject = emailSettingForISAdminAlert.SingleOrDefault().Subject;
                msgForISAdmin.Subject = subject;


                // loop through the contacts list and add them to To list of mail to be sent
                if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail))
                {
                    var mailAdressList = ConvertUtil.ConvertToMailAddresses(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

                    foreach (var mailaddr in mailAdressList)
                    {
                        msgForISAdmin.To.Add(mailaddr);
                    }
                }

                if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.ICHDetails.IchOpsEmail))
                {
                    var mailAdressList = ConvertUtil.ConvertToMailAddresses(AdminSystem.SystemParameters.Instance.ICHDetails.IchOpsEmail);

                    foreach (var mailaddr in mailAdressList)
                    {
                        msgForISAdmin.To.Add(mailaddr);
                    }
                }
                //set body text of mail
                msgForISAdmin.Body = emailToISAdminText;

                //send the mail
                emailSender.Send(msgForISAdmin);

            }
        }// End try
        catch (Exception ex)
        {

            _logger.Error("Error occurred while sending SIS Webservice Error Notification to IS Administrator", ex);

            throw new ISBusinessException(ErrorCodes.ICHSettlementErrorNotificationFailed, "Error occurred while sending SIS Webservice Error Notification to IS Administrator");

        }// End catch
    }
    #endregion


    #region private methods for ICH Invoice Settlement

    /// <summary>
    /// Filer the given ICHSettlementData based on the given billing period. 
    /// Generate SettlementXML for the filtered data and send it to ICH for settlement.
    /// </summary>
    /// <param name="billingPeriod">BillingPeriod object used to filter given ICHSettlementData</param>
    private void GenerateAndSendXML(BillingPeriod billingPeriod)
    {
      bool isSuccess = true;
      string invoiceIds;

      // Get string representation of given billing period.
      var clearancePeriod = String.Format("{0:0000}", billingPeriod.Year) + String.Format("{0:00}", billingPeriod.Month) + String.Format("{0:00}", billingPeriod.Period);
      try
      {

        // Generate ICHSettlementXML
        var ichSettlementXml = GenerateICHSettlementXML(clearancePeriod).Trim();

        // Check ICHSettlementXML for null or empty
        if (!string.IsNullOrEmpty(ichSettlementXml))
        {

          // Log the Generated XML.
          _logger.Info("ICHSettlementXML: ");

          _logger.Info(ichSettlementXml);



          // Verify and log ICHXmlHandler for null value.
          _logger.Info(String.Format("ICHXmlHandler instance is: [{0}]", ICHXmlHandler == null ? "NULL" : "NOT NULL"));

          // Validate the xml generated against schema file.
          // ReSharper disable PossibleNullReferenceException
          //string validationResult = ICHXmlHandler.Validate(ichSettlementXml, "ICH-Settlement.xsd");
          var XSDPath = string.Format("{0}{1}", ConnectionString.GetAppSetting("AppSettingPath"), ichSettlementXSDFolder);
          string validationResult = ICHXmlHandler.Validate(ichSettlementXml, XSDPath);
          // ReSharper restore PossibleNullReferenceException

          // If validationResult is "OK" then continue else throw business exception.
          if (validationResult.Trim().ToLower().Equals("ok"))
          {

            // Get max number of retries from config file.
            int maxRetries = SystemParameters.Instance.ICHDetails.MaxNumberOfRetriesToSendIchSettlementFile;

            // Used to store response of InvoiceSettlement API call.
            var serviceCallresponse = string.Empty;
            // Retry logic
            for (var retryCount = 1; retryCount <= maxRetries; retryCount++)
            {
              _webServiceResponse.AppendFormat("Attempt {0}:", retryCount);
              _webServiceResponse.Append(Environment.NewLine);

              // Send ICHSettlementXML to ICH for settlement
              serviceCallresponse = SendDataForSettlement(ichSettlementXml);

              // If response is success then break else retry
              if (serviceCallresponse=="Success" || serviceCallresponse !="Failed")

              {
                _logger.Info(String.Format("Invoices of '{0}' (yyyymmpp) clearance period sent for settlement successfully.", clearancePeriod));
                break;
              }

            }// End for
           

            // Get All UniqueInvoiceNumber  of Invoices  which are sent for settelment  
            var invoices = _ichSettlementDataList.Where(c => c.ClearancePeriod == clearancePeriod).Select(c => c.UniqueInvoiceNumber).Distinct().ToList();
            
            if (serviceCallresponse.Length!=0 && serviceCallresponse =="Success")
            {
              //var invoices = _ichSettlementDataList.Where(c => c.ClearancePeriod == clearancePeriod).Select(c => c.UniqueInvoiceNumber).Distinct().ToList();

                // Logic to convert all the UniqueInvoiceNumber in Byte[] to string.
                invoiceIds = string.Join(",", invoices.Select(invoice => ConvertUtil.ConvertGuidToString(invoice)).ToArray());

              _logger.Info(String.Format("Updating status of InvoiceIds: {0}", invoiceIds));



              // Update Invoice status to "Claimed" and set Resubmission flag to "C" for the invoices which are resubmitted.
              _logger.Info(string.Format("ICHInterfaceRepository instance is: [{0}]", ICHInterfaceRepository != null ? "NOT NULL" : "NULL"));

              // Log Invoice Settlement successful Event.
              _logger.Info("Updating Invoice statuses after successful settlement.");

              // Update invoices status after successful settlement.
              // ReSharper disable PossibleNullReferenceException
              ICHInterfaceRepository.UpdateInvoiceStatusAfterSettlement(invoiceIds);
              // ReSharper restore PossibleNullReferenceException

            }// End if
            else if (serviceCallresponse.Length >= 0 && serviceCallresponse != "Success" && serviceCallresponse != "Failed")
            {

                // Log Invoice Settelement unsuccessful Event.
                _logger.Info("Updating Invoice status after unsuccessful settlement => Response have error in some invoices.");

                _logger.InfoFormat("returnedErrorInvoices:{0}", serviceCallresponse);
                //split serviceCallresponse string for error code 0,1,2,3 and rest from 4 to 18
                    var returnedErrorInvoices = serviceCallresponse.Split('#');
                    
                    var retErrInv0123 = returnedErrorInvoices[0];
                    var retErrInv4To18 = returnedErrorInvoices[1];
                    
                    //if(retErrInv0123.Length !=0)
                    //{
                        //var arrayOfretErrInv0123 = retErrInv0123.Split(',');
                        //// get those UniqueInvoiceNumber in list which have no error response
                        //invoices.RemoveAll(item => arrayOfretErrInv0123.Contains(ConvertUtil.ConvertGuidToString(item)));
                    //}

                    if (retErrInv0123.Length == 0)
                    {
                        if (retErrInv4To18.Length != 0)
                        {
                            var arrayOfretErrInv4To18 = retErrInv4To18.Split(',');
                            // get those UniqueInvoiceNumber in list which have no error response
                            invoices.RemoveAll(
                                item => arrayOfretErrInv4To18.Contains(ConvertUtil.ConvertGuidToString(item)));
                        }


                        // Get string of all UniqueInvoiceNumber which have no error Response
                        var remainingInvioceId = string.Join(",",
                                                             invoices.Select(
                                                                 invoice => ConvertUtil.ConvertGuidToString(invoice)).
                                                                 ToArray());

                        // Updating status of Failed Settelment for No Errored InvoiceIds
                        _logger.Info(
                            String.Format("Updating status of Failed Settelment => For No Errored InvoiceIds: {0}",
                                          remainingInvioceId));

                        // update database for those UniqueInvoiceNumber which have no error response
                        ICHInterfaceRepository.UpdateInvoiceStatusAfterSettlement(remainingInvioceId);

                        // Updating status of Failed Settelment for Errored InvoiceIds
                       // _logger.Info(
                       //     String.Format("Updating status of Failed Settelment => For Errored(0-3) InvoiceIds: {0}",
                         //                 retErrInv0123));

                        // update database for those UniqueInvoiceNumber which have error response and set there ICH_SETTLEMENT_STATUS as NULL
                        //ICHInterfaceRepository.UpdateInvoiceStatusNullAfterFailedSettlement(retErrInv0123);


                        // Updating status of Failed Settelment for Errored InvoiceIds
                        _logger.Info(
                            String.Format("Updating status of Failed Settelment => For Errored(4-18) InvoiceIds: {0}",
                                          retErrInv4To18));

                        // update database for those UniqueInvoiceNumber which have error response and set there ICH_SETTLEMENT_STATUS as "Claim Failed"
                        ICHInterfaceRepository.UpdateInvoiceStatusAfterFailedSettlement(retErrInv4To18);
                    }
                    else
                    {
                        _logger.Info("Exception by ICH Webservice (Errorcodes:21000/21001/21002/21003");
                    }

            }
            else
            {
                isSuccess = false;



                // If ICH web service could not be called / web service was called successfully but it returned error then send email




                // Log Invoice Settlement Unsuccessful Event.
                _logger.Info(String.Format("Settlement of invoices for {0} (yyyymmpp) clearance period was unsuccessful. ErrorCode: {1}", clearancePeriod, _errorCode));

                // Message
                _logger.Info("Sending invoice settlement failure notification to IS Administrator.");

                // Send invoice failure notification to IS Administrator.
                var failureReason = String.Compare(_errorCode, "WSConnectFailure") > 0 ? "ICH web service could not be invoked" : string.Format("ICH web service returned error code {0}", _errorCode);

                SendICHSettlementWsFailureNotification(DateTime.UtcNow.ToShortTimeString(), failureReason, EmailTemplateId.SettlementDataWsUnavailabilityNotification);


            }// End else
          }// End if
          else
          {
            isSuccess = false;

            // Send Xml Validation Failure Alert to IS Administrator.
            ICHUpdateHandler.SendAlertForXmlValidationFailure(0, ichSettlementXml, "Invoice Settlement", validationResult);

            // Throw ISBusinessException indicating XML validation failure.
            throw new ISBusinessException(ErrorCodes.ICHSettlementXMLValidationFailed, validationResult);
          }// End else



        }// End if
      }
      catch (ISBusinessException exBusiness)
      {
        isSuccess = false;
        _logger.Error("Business Error occurred in generating and sending XML.", exBusiness);

      }// End catch
      catch (Exception ex)
      {
        isSuccess = false;
        _logger.Error("Error occurred in generating and sending XML", ex);

      }// End catch



      finally
      {
          if (isSuccess == false)
          {
              var invoices = _ichSettlementDataList.Where(c => c.ClearancePeriod == clearancePeriod).Select(
                      c => c.UniqueInvoiceNumber).Distinct().ToList();

              // Logic to convert all the UniqueInvoiceNumber in Byte[] to string.
              invoiceIds = string.Join(",",
                                       invoices.Select(invoice => ConvertUtil.ConvertGuidToString(invoice)).ToArray());

              IsetInvoiceStatus.SetStatusOfInvoices(invoiceIds, isSuccess == false ? "E" : "S", "ICH", _submissionDate);
          }
      }

    }// End GenerateAndSendXML()


    public void SendNotificationToISAdmin(ISSISOpsAlert alert, VelocityContext context, EmailTemplateId templateId, string filePath = null)
    {
      try
      {
        BroadcastMessagesManager.AddAlert(alert, templateId, context);
      }// End try
      catch (Exception ex)
      {

        _logger.Error("Error occurred while sending Invoice Settlement Failure Notification to IS Administrator", ex);

        throw new ISBusinessException(ErrorCodes.SendInvoiceFailureNotificationToISAdminFailed, "Error occurred while sending Invoice Settlement Failure Notification to IS Administrator");
      }// End catch
    }

    public void SendICHSettlementWsFailureNotification(string failureTime, string failureReason, EmailTemplateId templateId)
    {

      try
      {
        // Get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
        // Get an instance of email settings repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
        // Verify and log TemplatedTextGenerator for null value.
        _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

        // Generate email body text for ICH settlement web service failure notification email
        var context = new VelocityContext();
        context.Put("FailureTime", failureTime);
        context.Put("FailureReason", failureReason);

        // Verify and log TemplatedTextGenerator for null value.
        _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));
        var emailSettingForISAdminAlert = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.SettlementDataWsUnavailabilityNotification);


       // Generate email body text for own profile updates contact type mail
        if (TemplatedTextGenerator != null)
        {
          var emailToISAdminText = TemplatedTextGenerator.GenerateTemplatedText(EmailTemplateId.SettlementDataWsUnavailabilityNotification, context);
          // Create a mail object to send mail
          var msgForISAdmin = new MailMessage
                                     {
                                       From = new MailAddress(emailSettingForISAdminAlert.SingleOrDefault().FromEmailAddress),
                                       IsBodyHtml = true
                                     };

          var subject = emailSettingForISAdminAlert.SingleOrDefault().Subject;
          msgForISAdmin.Subject = subject;

          //loop through the contacts list and add them to To list of mail to be sent
          //msgForISAdmin.To.Add(new MailAddress(SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail));
          _logger.Info("Email SisOps :" + AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
                
          // loop through the contacts list and add them to To list of mail to be sent
          if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail))
          {
            var mailAdressList = ConvertUtil.ConvertToMailAddresses(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

            foreach (var mailaddr in mailAdressList)
            {
              msgForISAdmin.To.Add(mailaddr);
              _logger.Info("Email address :" + mailaddr);
            }
          }

          //set body text of mail
          msgForISAdmin.Body = emailToISAdminText;



          //send the mail
          _logger.Info(" Sending Mail.....");
          emailSender.Send(msgForISAdmin);
          _logger.Info("Sending Mail completed.");

        }
      }// End try
      catch (Exception ex)
      {

        _logger.Error("Error occurred while sending Invoice Settlement Failure Notification to IS Administrator", ex);


        throw new ISBusinessException(ErrorCodes.SendInvoiceFailureNotificationToISAdminFailed, "Error occurred while sending Invoice Settlement Failure Notification to IS Administrator");




      }// End catch
    }

    /// <summary>
    /// Generate ICHSettlementXML for given ICHSettlementData.
    /// </summary>
    /// <param name="billingPeriod">ClearancePeriod for which ICHSettlementXML is being created.</param>
    /// <returns></returns>
    private string GenerateICHSettlementXML(string billingPeriod)
    {

      try
      {

        String xmlizedString = "";
       // Populate ICHSettlement (Business) object.
        ICHSettlement objICHSettlement = PopulateICHSettlement(billingPeriod);

        // Check objICHSettlement for null.
        if (objICHSettlement != null)
        {
          // Verify and log ICHXmlHandler for null value.
          _logger.Info(String.Format("ICHXmlHandler value is: {0}", ICHXmlHandler == null ? "NULL" : "NOT NULL"));

          // Get XML representation of ICHSettlement object.
          // ReSharper disable PossibleNullReferenceException
          xmlizedString = ICHXmlHandler.SerializeXml(objICHSettlement, typeof(ICHSettlement));
          // ReSharper restore PossibleNullReferenceException

        }// End if





        // Return Xml string.
        return xmlizedString;

      }// End try
      catch (Exception ex)
      {
        // Log Error.
        _logger.Error("Error in generating ICH Settlement XML", ex);

        // Throw ISBusiness Exception indicating GenerateICHSettlementXML failure.
        throw new ISBusinessException(ErrorCodes.GenerateICHSettlementXMLFailed, "Error generating ICH Settlement XML.");

      }// End catch

    }// End GenerateICHSettlementXML()


    /// <summary>
    /// Populates and returns ICHSettlement object using the given ICHSettlement Data
    /// </summary>
    /// <param name="clearancePeriod">Clearance Period</param>
    /// <returns>ICHSettlement (Business) object</returns>
    private ICHSettlement PopulateICHSettlement(string clearancePeriod)
    {
      try
      {
        ICHSettlement ichSettlement = null;

        var settlementDataForClearancePeriod =
            _ichSettlementDataList.Where(c => c.ClearancePeriod == clearancePeriod).Select(c => c.ClearancePeriod).FirstOrDefault();

        if (settlementDataForClearancePeriod != null)
        {
          ichSettlement = new ICHSettlement
          {
            ClearancePeriod = clearancePeriod,
            Creditors = PopulateCreditors(clearancePeriod).ToArray()
          };
        }// End if

        return ichSettlement;

      }// End try
      catch (Exception ex)
      {

        _logger.Error("Error in populating ICHSettlement.", ex);

       throw;

      }// End catch


    }// End PopulateICHSettlement()

    /// <summary>
    /// Populate and returns list of Creditor objects using the given filters.
    /// </summary>
    /// <param name="clearancePeriod">ClearancePeriod filter</param>
    /// <returns>List of Creditor</returns>
    private List<Creditor> PopulateCreditors(string clearancePeriod)
    {
      var creditorList = new List<Creditor>();

      try
      {

          // Get a list of anonymous object having distinct combination of CreditorsDesignator, CreditorsCode, CreditorSponsorDesignator, CreditorSponsorCode,  MergeParentCreditorsDesignator, MergeParentCreditorsCode
        var creditorDetails =
            _ichSettlementDataList.Where(data => data.ClearancePeriod == clearancePeriod).Select(
                c => new { CreditorDesignator = c.CreditorsDesignator, CreditorCode = c.CreditorsCode, c.CreditorSponsorDesignator, c.CreditorSponsorCode,c.MergeParentCreditorsDesignator,c.MergeParentCreditorsCode }).Distinct().
                ToList();
        
        // Iterate through the list of anonymous objects obtained in above step
        foreach (var creditor in creditorDetails)
        {
          if (!string.IsNullOrEmpty(creditor.CreditorSponsorDesignator) &&
              !string.IsNullOrEmpty(creditor.CreditorSponsorCode))
          {

            var crDetails = new Creditor
                              {
                                CreditorMember = new DebtorMemberType { Designator = creditor.CreditorDesignator, Code = creditor.CreditorCode, },
                                CreditorSponsorMember = new DebtorMemberType { Designator = creditor.CreditorSponsorDesignator, Code = creditor.CreditorSponsorCode, },
                                ZoneDetails = PopulateZoneDetails(clearancePeriod, creditor.CreditorDesignator, creditor.CreditorCode).ToArray()
                              };
            //This 'MergeParentCreditorMember' field is optional and will be shown (with data) only if the Billing Member is merged
            if (!string.IsNullOrEmpty(creditor.MergeParentCreditorsDesignator) && !string.IsNullOrEmpty(creditor.MergeParentCreditorsCode))
            {
                crDetails.MergeParentCreditorMember = new DebtorMemberType { Designator = creditor.MergeParentCreditorsDesignator, Code = creditor.MergeParentCreditorsCode };
            }
            creditorList.Add(crDetails);

          }
          else
          {
            var crDetails = new Creditor
                              {
                                CreditorMember = new DebtorMemberType { Designator = creditor.CreditorDesignator, Code = creditor.CreditorCode, },
                                ZoneDetails = PopulateZoneDetails(clearancePeriod, creditor.CreditorDesignator, creditor.CreditorCode).ToArray()
                              };
            //This 'MergeParentCreditorMember' field is optional and will be shown (with data) only if the Billed Member is merged
            if (!string.IsNullOrEmpty(creditor.MergeParentCreditorsDesignator) && !string.IsNullOrEmpty(creditor.MergeParentCreditorsCode))
            {
                crDetails.MergeParentCreditorMember = new DebtorMemberType { Designator = creditor.MergeParentCreditorsDesignator, Code = creditor.MergeParentCreditorsCode };
            }
            creditorList.Add(crDetails);
          }
        }
      }// End try
      catch (Exception ex)
      {

        _logger.Error("Error in populating Creditors.", ex);

        throw;

      }// End catch
      return creditorList;
    }// End PopulateCreditors()

/// <summary>
    /// Populate and returns list of ZoneDetail objects using the given filters
    /// </summary>
    /// <param name="clearancePeriod">ClearancePeriod filter</param>
    /// <param name="creditorDesignator">CreditorDesignator filter</param>
    /// <param name="creditorCode">CreditorCode filter</param>
    /// <returns>List of ZoneDetail</returns>
    private List<ZoneDetail> PopulateZoneDetails(string clearancePeriod, string creditorDesignator, string creditorCode)
    {

      try
      {
        // Get a list of anonymous object having distinct combination of ZoneCode and ClearanceCurrencyCode
        var zones =
          _ichSettlementDataList.Where(c => c.ClearancePeriod == clearancePeriod && c.CreditorsDesignator == creditorDesignator && c.CreditorsCode == creditorCode).Select(
            c => new { Zone = c.ZoneCode, c.ClearanceCurrencyCode, }).Distinct().ToList();

        // Iterate through the list of anonymous objects obtained in above step

        // Return List of ZoneDetail objects
        return zones.Select(zone => new ZoneDetail
                                        {
                                          ZoneCode = zone.Zone,
                                          ClearanceCurrencyCode = zone.ClearanceCurrencyCode,
                                          DebtorDetails = PopulateDebtorDetails(clearancePeriod, creditorDesignator, creditorCode, zone.Zone, zone.ClearanceCurrencyCode).ToArray()
                                        }).ToList();

      }// End try
      catch (Exception ex)
      {
        _logger.Error("Error in populating ZoneDetails,", ex);

        throw;
      }// End catch



    }// End PopulateZoneDetails()

    /// <summary>
    /// Populate and returns list of DebtorDetail objects using the given ICHSettlement Data
    /// </summary>
    /// <param name="clearancePeriod">ClearancePeriod filter</param>
    /// <param name="creditorDesignator">CreditorDesignator filter</param>
    /// <param name="creditorCode">CreditorCode filter</param>
    /// <param name="zoneCode">ZoneCode filter</param>
    /// <param name="clearanceCurrencyCode">ClearanceCurrencyCode filter</param>
    /// <returns>List of DebtorDetail</returns>
    private List<DebtorDetail> PopulateDebtorDetails(string clearancePeriod, string creditorDesignator, string creditorCode, string zoneCode, string clearanceCurrencyCode)
    {

      try
      {
        var lstDebtorDetail = new List<DebtorDetail>();

        // Get a list of anonymous object having distinct combination of DebtorsDesignator, DebtorsCode, SponsorDesignator and SponsorCode 
        var detors =
          _ichSettlementDataList.Where(
            c =>
            c.ClearancePeriod == clearancePeriod && c.CreditorsDesignator == creditorDesignator && c.CreditorsCode == creditorCode && c.ZoneCode == zoneCode &&
            c.ClearanceCurrencyCode == clearanceCurrencyCode).Select(c => new { c.DebtorsDesignator, c.DebtorsCode, c.SponsorDesignator, c.SponsorCode,c.MergeParentDebtorsDesignator,c.MergeParentDebtorsCode }).Distinct().ToList();

        // Iterate through the list of anonymous objects obtained in above step
        foreach (var debtor in detors)
        {
          // Create and set DebtorDetail object
          var debtorDetail = new DebtorDetail
                               {
                                 DebtorMember = new DebtorMemberType { Designator = debtor.DebtorsDesignator, Code = debtor.DebtorsCode },
                                 CategoryDetails =
                                   PopulateCategoryDetails(clearancePeriod, creditorDesignator, creditorCode, zoneCode, clearanceCurrencyCode, debtor.DebtorsCode, debtor.SponsorCode).ToArray()
                               };

          if (!string.IsNullOrEmpty(debtor.SponsorDesignator) && !string.IsNullOrEmpty(debtor.SponsorCode))
          {
            debtorDetail.SponsorMember = new DebtorMemberType { Designator = debtor.SponsorDesignator, Code = debtor.SponsorCode };
          }
          //This 'MergeParentDebtorMember' field is optional and will be shown (with data) only if the Billed Member is merged
          if (!string.IsNullOrEmpty(debtor.MergeParentDebtorsDesignator) && !string.IsNullOrEmpty(debtor.MergeParentDebtorsCode))
          {
              debtorDetail.MergeParentDebtorMember = new DebtorMemberType { Designator = debtor.MergeParentDebtorsDesignator, Code = debtor.MergeParentDebtorsCode };
          }

          // Add the above create object to the DebtorDetail list
          lstDebtorDetail.Add(debtorDetail);
        }// End foreach
        // Return List of DebtorDetail objects
        return lstDebtorDetail;
      }// End try
      catch (Exception ex)
      {

        _logger.Error("Error in populating DebtorDetails.", ex);

        throw;
      }// End catch



    }// End PopulateDebtorDetails()



    /// <summary>
    ///  Populate and returns list of CategoryDetail objects using the given ICHSettlement Data
    /// </summary>
    /// <param name="clearancePeriod">ClearancePeriod filter</param>
    /// <param name="creditorDesignator">CreditorDesignator filter</param>
    /// <param name="creditorCode">CreditorCode filter</param>
    /// <param name="zoneCode">ZoneCode filter</param>
    /// <param name="clearanceCurrencyCode">ClearanceCurrencyCode filter</param>
    /// <param name="debtorsCode">DebtorsCode filter</param>
    /// <param name="sponsorCode">SponsorCode filter</param>
    /// <returns>List of CategoryDetail objects</returns>
    private List<CategoryDetail> PopulateCategoryDetails(string clearancePeriod, string creditorDesignator, string creditorCode, string zoneCode, string clearanceCurrencyCode, string debtorsCode, string sponsorCode)
    {
      try
      {
        // Get a list of anonymous object having distinct BillingCategory
        List<string> lstBillingCategory =
          _ichSettlementDataList.Where(
            c =>
            c.ClearancePeriod == clearancePeriod && c.CreditorsDesignator == creditorDesignator && c.CreditorsCode == creditorCode && c.ZoneCode == zoneCode &&
            c.ClearanceCurrencyCode == clearanceCurrencyCode && c.DebtorsCode == debtorsCode && c.SponsorCode == sponsorCode).Select(c => c.BillingCategory).Distinct().ToList();

        // Iterate through the list of anonymous objects obtained in above step

        // Return List of CategoryDetail objects
        return
          lstBillingCategory.Select(
            billingCategory =>
            new CategoryDetail
              {
                BillingCategory = billingCategory,
                InvoiceDetails = PopulateInvoiceDetails(clearancePeriod, creditorDesignator, creditorCode, zoneCode, clearanceCurrencyCode, debtorsCode, sponsorCode, billingCategory).ToArray()
              }).
            ToList();

      }// End try
      catch (Exception ex)
      {

       _logger.Error("Error in populating CategoryDetails.", ex);

       throw;

      }// End catch


    }// End PopulateCategoryDetails()

    /// <summary>
    /// Populate and returns list of InvoiceDetail objects using the given ICHSettlement Data
    /// </summary>
    /// <param name="clearancePeriod">ClearancePeriod filter</param>
    /// <param name="creditorDesignator">CreditorDesignator filter</param>
    /// <param name="creditorCode">CreditorCode filter</param>
    /// <param name="zoneCode">ZoneCode filter</param>
    /// <param name="clearanceCurrencyCode">ClearanceCurrencyCode filter</param>
    /// <param name="debtorsCode">DebtorsCode filter</param>
    /// <param name="sponsorCode">SponsorCode filter</param>
    /// <param name="billingCategory">BillingCategory filter</param>
    /// <returns>List of InvoiceDetail objects</returns>
    private List<InvoiceDetail> PopulateInvoiceDetails(string clearancePeriod, string creditorDesignator, string creditorCode, string zoneCode, string clearanceCurrencyCode, string debtorsCode, string sponsorCode, string billingCategory)
    {

      try
      {

        var lstInvoiceDetails = new List<InvoiceDetail>();

        _submissionDate = DateTime.UtcNow;

        var invoiceDetails =
          _ichSettlementDataList.Where(
            c =>
            c.ClearancePeriod == clearancePeriod && c.CreditorsDesignator == creditorDesignator && c.CreditorsCode == creditorCode && c.ZoneCode == zoneCode &&
            c.ClearanceCurrencyCode == clearanceCurrencyCode && c.DebtorsCode == debtorsCode && c.SponsorCode == sponsorCode && c.BillingCategory == billingCategory).ToList();

        // Iterate through the given list of ICHSettlement (Model class) objects
        foreach (var invoiceDetail in invoiceDetails)
        {
          // Create and set InvoiceDetail object
          var invoice = new InvoiceDetail
                          {
                            UniqueInvoiceNumber = ConvertUtil.ConvertGuidToString(invoiceDetail.UniqueInvoiceNumber),
                            InvoiceSource = invoiceDetail.InvoiceSource,
                            InvoiceNumber = invoiceDetail.InvoiceNumber,
                            InvoiceDate =
                              (String.Format("{0:0000}", invoiceDetail.InvoiceDate.Year) + String.Format("{0:00}", invoiceDetail.InvoiceDate.Month) +
                               String.Format("{0:00}", invoiceDetail.InvoiceDate.Day)),
                            LocalCurrencyCode = invoiceDetail.LocalCurrencyCode,
                            ExchangeRate = invoiceDetail.ExchangeRate,
                            DebitOrCredit = invoiceDetail.DebitOrCredit,
                            SettlementMethodIndicator = invoiceDetail.SettlementMethodIndicator,
                            SuspendedInvoiceIndicator = invoiceDetail.SuspendedInvoiceIndicator,
                            LateSubmission = invoiceDetail.LateSubmission,
                            Total = new Total { LocalCurrency = invoiceDetail.TotalLocalCurrency, ClearanceCurrency = invoiceDetail.TotalClearanceCurrency },
                            SubmissionTime = ((invoiceDetail.SubmissionTime > DateTime.MinValue) ? invoiceDetail.SubmissionTime : _submissionDate),
                            IsResubmitted = invoiceDetail.IsResubmitted
                          };

          // Set Transmitter member details if TransmitterDesignator and TransmitterCode is present.
          if (!string.IsNullOrEmpty(invoiceDetail.TransmitterDesignator) && !string.IsNullOrEmpty(invoiceDetail.TransmitterCode))
          {
            invoice.TransmitterMember = new DebtorMemberType { Designator = invoiceDetail.TransmitterDesignator, Code = invoiceDetail.TransmitterCode };
          }
          //CMP #637: Changes to ICH Settlement 
          //populate CHAgreementIndicator
          if (!string.IsNullOrWhiteSpace(invoiceDetail.CHAgreementIndicator))
          {
            invoice.CHAgreementIndicator = invoiceDetail.CHAgreementIndicator;
          }
          //populate CHDueDate
          if (!string.IsNullOrWhiteSpace(invoiceDetail.CHDueDate))
          {
            invoice.CHDueDate = invoiceDetail.CHDueDate;
          }
          //populate SalesOrderNumber
          if (!string.IsNullOrWhiteSpace(invoiceDetail.SalesOrderNumber))
          {
            invoice.SalesOrderNumber = invoiceDetail.SalesOrderNumber;
          }
          //populate CreditorLocationID
          if (!string.IsNullOrWhiteSpace(invoiceDetail.CreditorLocationID))
          {
            invoice.CreditorLocationID = invoiceDetail.CreditorLocationID;
          }
          //populate DebtorLocationID
          if (!string.IsNullOrWhiteSpace(invoiceDetail.DebtorLocationID))
          {
            invoice.DebtorLocationID = invoiceDetail.DebtorLocationID;
          }

          // Add the above create object to the InvoiceDetail list
          lstInvoiceDetails.Add(invoice);

        }// End foreach

        // Return List of InvoiceDetail objects
        return lstInvoiceDetails;
      }// End try
      catch (Exception ex)
      {
        _logger.Error("Error occurred in creating and setting InvoiceDetail", ex);
        throw;
      }// End catch

    }// End PopulateInvoiceDetails()


    /// <summary>
    ///  Get ICHSettlement data containing invoices for the given current billing period and late submitted invoices for the given previous billing period.
    /// </summary>
    /// <param name="curBillingPeriod">Current Billing Period</param>
    /// <param name="prevBillingPeriod">Previous Billing Period</param>
    /// <param name="maxRecords">Max number of records to get</param>
    /// <returns>List of ICHSettlement (Model class) objects</returns>
    private List<ICHSettlementData> GetICHSettlementData(BillingPeriod curBillingPeriod, BillingPeriod prevBillingPeriod, int maxRecords)
    {
      // Return ICHSettlement Data
      return ICHInterfaceRepository.GetICHSettlementData(curBillingPeriod, prevBillingPeriod, maxRecords);

    }// End GetICHSettlementData()



    /// <summary>
    /// Send ICHSettlementXML for settlement.
    /// </summary>
    /// <param name="ichSettlementXML"></param>
    /// <returns>String for Success or Error</returns>
    private string SendDataForSettlement(string ichSettlementXML)
    {
        string responseString = string.Empty;
        string returnString = string.Empty;
      string UnhandledErrorDetails = string.Empty;
      try
      {
        // Get SISGatewayService client
        SISGatewayServiceClient sisGatewayClient = CreateSISGatewayServiceClient();

         responseString = sisGatewayClient.InvoiceSettlement(ichSettlementXML);
           
          //  For Testing
       // responseString = "<Root><Error ErrorCode='21001' ErrorShortDescription='Invoice Detail already in ICH' ErrorLongDescription='Invoice Detail with UniqueInvoiceNumber already in ICH ' UniqueInvoiceNumber='A3EFC567877963FDE040007F01004AB1' ></Error><Error ErrorCode='21001' ErrorShortDescription='Invoice Detail already in ICH' ErrorLongDescription='Invoice Detail with UniqueInvoiceNumber already in ICH ' UniqueInvoiceNumber='A3EFC567877963FDE04H007F01004AB1' ></Error><Error ErrorCode='21001' ErrorShortDescription='Invoice Detail already in ICH' ErrorLongDescription='Invoice Detail with UniqueInvoiceNumber already in ICH ' UniqueInvoiceNumber='A3EFC567877963FDE040U07F01004AB1'></Error><Error ErrorCode='21020' ErrorShortDescription='Invoice Detail already received' ErrorLongDescription='Invoice Detail with UniqueInvoiceNumber already received and will be processed shortly if not already processed in ICH' UniqueInvoiceNumber='A3F0C88365267D7DE040007F01004FF7' ></Error></Root>";

         //responseString = "<Root><Error ErrorCode='21000' ErrorShortDescription='Exception in the SIS Gateway Controller: ' ErrorLongDescription='A system error as occured. ICH technical support is looking into the problem.' /></Root>";
         
          // Parse the response received as XElement.
        XElement response = XElement.Parse(responseString);
        XElement innerNode = response.Element("Success");

        _logger.InfoFormat("ICH Web Service Response: {0}", responseString);
        var responseDoc = new XmlDocument();
        responseDoc.LoadXml(responseString);

        // Get Success node from response if it is present.
        var sucessNode = responseDoc.SelectSingleNode("/Root/Success");

        // Return true if Success response is received else return false)
        if (innerNode != null && innerNode.Value.ToLower().Contains("true"))
        {
            returnString = "Success";
        }// End if

        // Check innerNode for null. if true then fo for Error node from the response.
        if (innerNode == null)
        {
            innerNode = response.Element("Error");
           
            if (innerNode != null)
            {
               
                IEnumerable<XElement> innerNodeError =
                                                   from el in response.Elements("Error")
                                                   select el;
                

                var forErrorCode0123 = string.Empty;
                var forErrorCode4To18 = string.Empty;
           
                foreach (XElement el in innerNodeError)
                {
                  try
                  {
                    // check for ErrorCode 21019 & 21020 as they do not require re-submission as they mean that the information is already in the ICH system or that it will soon be
                    if (!(el.Attribute("ErrorCode").Value.Equals("21019") || el.Attribute("ErrorCode").Value.Equals("21020")))
                    {

                      if (el.Attribute("ErrorCode").Value.Equals("21000") || el.Attribute("ErrorCode").Value.Equals("21001") || el.Attribute("ErrorCode").Value.Equals("21002") ||
                          el.Attribute("ErrorCode").Value.Equals("21003"))
                      {
                          if (el.Attribute("ErrorCode").Value.Equals("21000"))
                          {
                              UnhandledErrorDetails = UnhandledErrorDetails + "Unhandled error from ICH on Invoice Settlement File transmission. This error needs to be escalated to IATA Application Support. " + " " + "ErrorCode=" +
                                                      el.Attribute("ErrorCode").Value + " " + "ErrorShortDescription=" +
                                                      " " + el.Attribute("ErrorShortDescription").Value + " " +
                                                      "ErrorLongDescription=" + " " +
                                                      el.Attribute("ErrorLongDescription").Value;
                          }
                          else
                          {
                              UnhandledErrorDetails = UnhandledErrorDetails + "Invoice Settlement File error detected by ICH System. This error need to be escalated to Kale Application Support." + " " + "ErrorCode=" +
                                                      el.Attribute("ErrorCode").Value + " " + "ErrorShortDescription=" +
                                                      " " + el.Attribute("ErrorShortDescription").Value + " " +
                                                      "ErrorLongDescription=" + " " +
                                                      el.Attribute("ErrorLongDescription").Value;
                          }
                          forErrorCode0123 +="Exception" + ",";
                      }
                      else
                      {
                        forErrorCode4To18 += el.Attribute("UniqueInvoiceNumber").Value + ",";
                      }

                    }
                  }
                  catch(Exception ex)
                  {
                    //This catch block handles errors specific to a error tag.Incase of error the error details would be appended to below string and next error response would be parsed.
                    UnhandledErrorDetails = UnhandledErrorDetails + " " + "ErrorCode=" + el.Attribute("ErrorCode").Value + " " + "ErrorShortDescription=" + " " + el.Attribute("ErrorShortDescription").Value + " " + "ErrorLongDescription=" + " " + el.Attribute("ErrorLongDescription").Value;
                  }
                }

                //Remove last  ","  from string
                if (forErrorCode0123.Length != 0)
                {
                    forErrorCode0123 = forErrorCode0123.Substring(0, forErrorCode0123.Length - 1);
                }
                if (forErrorCode4To18.Length != 0)
                {
                    forErrorCode4To18 = forErrorCode4To18.Substring(0, forErrorCode4To18.Length - 1);
                }
               
                //In case of ErrorCode 21019 & 21020 we will get error but have to consider it as Success case
                if (forErrorCode0123.Length == 0 && forErrorCode4To18.Length == 0 && UnhandledErrorDetails.Length == 0)
                {
                    returnString = "Success";
                }
                else
                {
                    // Create return string with # seperated which will later used to split both
                returnString = forErrorCode0123 + "#" + forErrorCode4To18 ;

                }




                // Send Email on the basis of Error Code
                  SendEmail(response);
            }
            else
            {
                _logger.Error("Service Response does not have 'Success' or 'Error' Node in XML!");
                _errorCode = "WSConnectFailure";
                returnString = "Failed";
            }
        }
         
        _webServiceResponse.AppendLine(responseString);

        // In case there was an error while parsing specific error node, then send mail to sis ops.
        if (!string.IsNullOrEmpty(UnhandledErrorDetails))
        {
          _logger.Info("Sending invoice settlement failure notification to IS Administrator for unhandled exception.");

          // Send invoice failure notification to IS Administrator.
          var failureReason = UnhandledErrorDetails;

          SendICHSettlementWsFailureNotification(DateTime.UtcNow.ToShortTimeString(), failureReason, EmailTemplateId.SettlementDataWsUnavailabilityNotification);
        }

      }// End try
      catch (Exception ex)
      {

        // Log Error.
        _logger.Error("Error in sending invoices for settlement", ex);


        //When ICH web service could not be connected, Set error code so that it can be accessed in caller method
        _errorCode = "WSConnectFailure";
        returnString = "Failed";

      }// End catch




      return returnString;

    }

      private void SendEmail(XElement response)
      {
          try
          {
              string emailContent = string.Empty;
              string errorCodeAndCount = string.Empty;
             // List<KeyValuePair<string, int>> listErrorCodeAndCount = new List<KeyValuePair<string, int>>();
              List<List<string>> listErrorCodeCountInvoiceIds = new List<List<string>>();


              //iterate thru response to get error code and there count
              int i = 0;
              while (i < 24)
              {
                  if ((i < 10) && (i>3))
                  {
                      
                      var errorElements = response.Elements("Error").Where(d => d.Attribute("ErrorCode").Value.Equals("2100" + i));
                      if (errorElements.Count() != 0)
                      {
                      //    listErrorCodeAndCount.Add(new KeyValuePair<string, int>
                      //                                  (errorElements.FirstOrDefault().Attribute("ErrorCode").Value,
                      //                                   errorElements.Count()
                      //                                  ));

                          List<string> test2 = new List<string>();
                          var InvoiceIDs = string.Empty;
                        
                          foreach (var errorElement in errorElements)
                          {
                              var invioceID = errorElement.Attribute("UniqueInvoiceNumber").Value;
                              InvoiceIDs += invioceID + ",";
                          }
                          InvoiceIDs = InvoiceIDs.Substring(0, InvoiceIDs.Length - 1);
           
                          test2.Add(errorElements.FirstOrDefault().Attribute("ErrorCode").Value);
                          test2.Add(errorElements.Count().ToString());
                          test2.Add(InvoiceIDs);

                          listErrorCodeCountInvoiceIds.Add(test2);
                      }
                  }
                  else
                  {
                      var errorElements = response.Elements("Error").Where(d => d.Attribute("ErrorCode").Value.Equals("210" + i));
                      if (errorElements.Count() != 0)
                      {
                          //listErrorCodeAndCount.Add(new KeyValuePair<string, int>
                          //                              (errorElements.FirstOrDefault().Attribute("ErrorCode").Value,
                          //                               errorElements.Count()
                          //                              ));

                          List<string> test2 = new List<string>();
                          var InvoiceIDs = string.Empty;

                          foreach (var errorElement in errorElements)
                          {
                              var invioceID = errorElement.Attribute("UniqueInvoiceNumber").Value;
                              InvoiceIDs += invioceID + ",";
                          }
                          InvoiceIDs = InvoiceIDs.Substring(0, InvoiceIDs.Length - 1);
           
                          test2.Add(errorElements.FirstOrDefault().Attribute("ErrorCode").Value);
                          test2.Add(errorElements.Count().ToString());
                          test2.Add(InvoiceIDs);

                          listErrorCodeCountInvoiceIds.Add(test2);
                      }
                  }

                  i++;
              }

              // Create string of Error Code and  Count
              //foreach (var keyValuePair in listErrorCodeAndCount)
              //{
              //    errorCodeAndCount += "[Error Code " + keyValuePair.Key + " : " + keyValuePair.Value + " invoices] ";
              //}

              foreach (var listErrorCodeCountInvoiceId in listErrorCodeCountInvoiceIds)
              {
                  errorCodeAndCount += "Error Code " + listErrorCodeCountInvoiceId[0] + " : " + listErrorCodeCountInvoiceId[1] + " invoices (" + listErrorCodeCountInvoiceId[2] + " ) <BR/>";
              }


              _logger.InfoFormat("Email String : {0}", errorCodeAndCount);

              // Set Email Content on the basis of Error Code
              //It is assumed here that If it fails on 2 accounts, ICH will only return the first error trapped
              foreach (var listErrorCodeCountInvoiceId in listErrorCodeCountInvoiceIds)
              {
                  if (listErrorCodeCountInvoiceId[0].Equals("21000"))
                  {
                      // if error code 21000
                      emailContent = "Unhandled error from ICH on Invoice Settlement File transmission. This error needs to be escalated to IATA Application Support.";
                      break;

                  }
                  if (listErrorCodeCountInvoiceId[0].Equals("21001") || listErrorCodeCountInvoiceId[0].Equals("21002") || listErrorCodeCountInvoiceId[0].Equals("21003"))
                  {
                      // if error code 21001,21002,21003
                      emailContent = "Invoice Settlement File error detected by ICH System. This error need to be escalated to Kale Application Support.";
                      break;
                  }
                  if (!(listErrorCodeCountInvoiceId[0].Equals("21000") || listErrorCodeCountInvoiceId[0].Equals("21001") || listErrorCodeCountInvoiceId[0].Equals("21002") || listErrorCodeCountInvoiceId[0].Equals("21003")))
                  {
                      // if error code from 21004 to 21020
                      emailContent = "Invoice Settlement File errors detected by ICH System. These errors need to be investigated by ICH OPS using IS Processing Dashboard.";
                      break;
                  }
              }

              //send the Email
              if (!string.IsNullOrEmpty(errorCodeAndCount))
              {
                  SendICHSettlementErrorNotification(DateTime.UtcNow.ToShortTimeString(), emailContent,
                                                     errorCodeAndCount,
                                                     EmailTemplateId.SettlementDataWsUnavailabilityNotification);
              }
          }
          catch (Exception ex)
          {

              _logger.Error("Error occurred while sending ICH Settlement Error Notification to IS Administrator", ex);

          }    
      }

      public void SendICHSettlementErrorNotification(string failureTime, string emailContent, string errorCodeAndCount, EmailTemplateId templateId)
      {

          try
          {
              // Get an object of the EmailSender component
              var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
              // Get an instance of email settings repository
              var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
              // Verify and log TemplatedTextGenerator for null value.
              _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

              // Generate email body text for ICH settlement web service failure notification email
              var context = new VelocityContext();
              context.Put("FailureTime", failureTime);
              context.Put("EmailContent", emailContent);
              context.Put("ErrorCodeAndCount", errorCodeAndCount);
              context.Put("n", "\n");
              // Verify and log TemplatedTextGenerator for null value.
              _logger.Info(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

              var emailSettingForISAdminAlert = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)EmailTemplateId.ICHSettlementErrorNotification);

              // Generate email body text for own profile updates contact type mail
              if (TemplatedTextGenerator != null)
              {
                  var emailToISAdminText = TemplatedTextGenerator.GenerateTemplatedText(EmailTemplateId.ICHSettlementErrorNotification, context);
                  // Create a mail object to send mail
                  var msgForISAdmin = new MailMessage
                  {
                      From = new MailAddress(emailSettingForISAdminAlert.SingleOrDefault().FromEmailAddress),
                      IsBodyHtml = true
                  };

                  var subject = emailSettingForISAdminAlert.SingleOrDefault().Subject;
                  msgForISAdmin.Subject = subject;


                  _logger.Info("Email SisOps :" +  AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
                  _logger.Info("Email IchOps:" + AdminSystem.SystemParameters.Instance.ICHDetails.IchOpsEmail);


                  // loop through the contacts list and add them to To list of mail to be sent
                  if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail))
                  {
                      var mailAdressList = ConvertUtil.ConvertToMailAddresses(AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

                      foreach (var mailaddr in mailAdressList)
                      {
                          msgForISAdmin.To.Add(mailaddr);
                          _logger.Info("Email address :" + mailaddr);
                 
                      }
                  }

                  if (!string.IsNullOrEmpty(AdminSystem.SystemParameters.Instance.ICHDetails.IchOpsEmail))
                  {
                      var mailAdressList = ConvertUtil.ConvertToMailAddresses(AdminSystem.SystemParameters.Instance.ICHDetails.IchOpsEmail);

                      foreach (var mailaddr in mailAdressList)
                      {
                          msgForISAdmin.To.Add(mailaddr);
                          _logger.Info("Email address :" + mailaddr);
                      }
                  }
                  //set body text of mail
                  msgForISAdmin.Body = emailToISAdminText;

                  //send the mail
                  _logger.Info(" Sending Mail.....");
                  emailSender.Send(msgForISAdmin);
                  _logger.Info("Sending Mail completed.");

              }
          }// End try
          catch (Exception ex)
          {

              _logger.Error("Error occurred while sending ICH Settlement Error Notification to IS Administrator", ex);

              throw new ISBusinessException(ErrorCodes.ICHSettlementErrorNotificationFailed, "Error occurred while sending ICH Settlement Error Notification to IS Administrator");

          }// End catch
      }

// End SendDataForSettlement()

    /// <summary>
    /// This method validates whether there are more than one distinct billing periods present 
    /// in list of invoice ids passed
    /// </summary>
    /// <param name="invoiceData"></param>
    /// <returns>
    /// 1 when invoice list contains more than one distinct billing periods
    /// 2 when invoice list contains one billing period but the billing period is not current period or not previous period
    /// 3 when invoice list contains only one billing period (either current or previous)
    /// </returns>
    public int ValidateBillingPeriodforResendRequest(List<ICHSettlementData> invoiceData)
    {
      int validationResult = InvalidBillingPeriod;

      // Check whether invoices corresponding to invoiceId list passed belong to same or different billing periods
      string clearancePeriods = string.Join(",", invoiceData.Select(ichData => ichData.ClearancePeriod).Distinct().ToArray());

      // Call business layer method for generating invoice data for invoice ids passed
      string[] clearancePeriodsSplit = clearancePeriods.Split(',');


      if (clearancePeriodsSplit.Length > 1)
      {
        // Indicates invoice list contains more than one distinct billing periods
        validationResult = MultipleBillingPeriods;
      }
      else if (clearancePeriodsSplit.Length == 1)
      {
        // Indicates invoice list contains one billing period but the billing period is not current period or not previous period

        // Get current ICH billing period
        var billingPeriod = CalendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
        var currentOrPrevbillingPeriod = billingPeriod.Year + billingPeriod.Month.ToString().PadLeft(2, '0') + billingPeriod.Period.ToString().PadLeft(2, '0');

        if (clearancePeriodsSplit[0] == currentOrPrevbillingPeriod)
        {
          validationResult = ValidBillingPeriod;
        }
        else
        {
          // Get current ICH billing period
          billingPeriod = CalendarManager.GetLastClosedBillingPeriod(ClearingHouse.Ich);

          currentOrPrevbillingPeriod = billingPeriod.Year + billingPeriod.Month.ToString().PadLeft(2, '0') + billingPeriod.Period.ToString().PadLeft(2, '0');

          validationResult = clearancePeriodsSplit[0] == currentOrPrevbillingPeriod ? ValidBillingPeriod : InvalidBillingPeriod;
        }
      }

      return validationResult;
    }

    /// <summary>
    /// Create and return SISGatewayService client
    /// </summary>
    /// <returns>SISGatewayService client object</returns>
    private static SISGatewayServiceClient CreateSISGatewayServiceClient()
    {
      if (_sisGatewayServiceClient == null)
      {
         System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender1, certificate, chain, sslPolicyErrors) => true);
        // Create SISGatewayService client
        _sisGatewayServiceClient = new SISGatewayServiceClient();

        // Set client credentials
        _sisGatewayServiceClient.ClientCredentials.UserName.UserName = SystemParameters.Instance.ICHDetails.SisGatewayServiceUserName; // ConfigurationManager.AppSettings.Get("SISGatewayServiceUserName");
        _sisGatewayServiceClient.ClientCredentials.UserName.Password = SystemParameters.Instance.ICHDetails.SisGatewayServiceUsersPassword; // ConfigurationManager.AppSettings.Get("SISGatewayServicePassword");
      }
      // Return SISGatewayService client
      return _sisGatewayServiceClient;
    }// End CreateSISGatewayServiceClient()

    #endregion

    #region Methods for FiveDayRates File


    /// <summary>
    /// Process FiveDayRates File:
    /// 1. Parse file.
    /// 2. Delete if ExchangeRate already exists in database for the calculated effective year and month(from).
    /// 3. Add ExchangeRates received through the FDR file to database for the calculated effective dates(from to).
    /// </summary>
    /// <param name="fiveDayRate"></param>
    /// <param name="fdrFileParsingErrors"></param>
    /// <returns></returns>
    public void ProcessFiveDayRatesFile(FiveDayRate fiveDayRate, List<ISFileParsingException> fdrFileParsingErrors)
    {
      try
      {
        // Verify fiveDayRate is null.
        if (fiveDayRate == null)
        {
          // Send Parsing and Validation failure notification to IS Admin.
          var context = new VelocityContext();
          context.Put("Month", DateTime.UtcNow.Month);
          context.Put("Year", DateTime.UtcNow.Year);
          context.Put("Errors", fdrFileParsingErrors);

          var alert = new ISSISOpsAlert
                        {
                          AlertDateTime = DateTime.UtcNow,
                          EmailAddress = SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail,
                          IsActive = true,
                          Title = "FDR file parsing and validation failure notification."
                        };

          var sb = new StringBuilder();
          const char separator = '\t';
          if (fdrFileParsingErrors.Count > 0)
          {
            sb.Append(String.Format("Please note that Five Day Rate file containing exchange rate records for {0} {1} could not be processed due to following errors:",
                                    DateTime.UtcNow.Month,
                                    DateTime.UtcNow.Year));
            sb.Append(Environment.NewLine);

            sb.Append("Error Code");
            sb.Append(separator);
            sb.Append("Error Description");
            sb.Append(Environment.NewLine);

            foreach (var isFileParsingException in fdrFileParsingErrors)
          {
              sb.Append(isFileParsingException.ErrorCode);
              sb.Append(separator);
              sb.Append(isFileParsingException.Message);
              sb.Append(Environment.NewLine);
            }

          }
          alert.Message = sb.ToString();

          SendNotificationToISAdmin(alert, context, EmailTemplateId.FdrParsingAndValidationFailureNotification);
          return;
        }// End if.




        // Effective From Date.
        var effectiveStartDate = new DateTime(fiveDayRate.Header.FdrDate.Year, fiveDayRate.Header.FdrDate.Month, 1, 0, 0, 0).AddMonths(1);
        
        // Effective To Date.
        var effectiveEndDate = effectiveStartDate.AddMonths(1).AddSeconds(-1);

        // Get ExchangeRates if ExchangeRate exist for effective year and month.
        var exchangeRatesToDelete = ExchangeRateRepository.Get(er => er.EffectiveFromDate.Year == effectiveStartDate.Year && er.EffectiveFromDate.Month == effectiveStartDate.Month);

        // Delete ExchangeRate from database.
        foreach (var exchangeRate in exchangeRatesToDelete)
        {
          ExchangeRateRepository.Delete(exchangeRate);
        }// End foreach.

        // Add ExchangeRate to database.
        foreach (var exchangeRate in fiveDayRate.LstExchangeRate)
        {
          if (exchangeRate != null)
          {
            ExchangeRateRepository.Add(new ExchangeRate
                                         {
                                           CurrencyId = exchangeRate.NumericCurrencyCode,
                                           EffectiveFromDate = effectiveStartDate,
                                           EffectiveToDate = effectiveEndDate,
                                           FiveDayRateEur =
                                             Convert.ToDouble(String.Format("{0}.{1}", exchangeRate.EuroToCurrencyCodeExchangeRateInteger, exchangeRate.EuroToCurrencyCodeExchangeRateFraction),
                                                              CultureInfo.InvariantCulture),
                                           FiveDayRateGbp =
                                             Convert.ToDouble(
                                               String.Format("{0}.{1}", exchangeRate.PoundSterlingToCurrencyCodeExchangeRateInteger, exchangeRate.PoundSterlingToCurrencyCodeExchangeRateFraction),
                                               CultureInfo.InvariantCulture),
                                           FiveDayRateUsd =
                                             Convert.ToDouble(
                                               String.Format("{0}.{1}", exchangeRate.UsDollarToCurrencyCodeExchangeRateInteger, exchangeRate.UsDollarToCurrencyCodeExchangeRateFraction),
                                               CultureInfo.InvariantCulture),
                                           IsActive = true,
                                           LastUpdatedBy = 0,
                                           LastUpdatedOn = DateTime.UtcNow
                                         });
          }

        }// End foreach.

        // Commit changes.
        UnitOfWork.CommitDefault();

      }// End try.
      catch (Exception exception)
      {
        _logger.Error("Error in ProcessFiveDayRatesFile", exception);
        throw;
      }// End catch.
    }// End ProcessFiveDayRatesFile()

    /// <summary>
    /// Send FDR not received notification to IS Admin if FDR file is not received by 4 PM of every month.
    /// </summary>
    public void SendFdrNotReceivedNotification()
    {
      // Effective From Date.
      var effectiveStartDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0).AddMonths(1);

      // Check if FDR File for next month was received or not. If not then send notification.
      if (ExchangeRateRepository.Get(
                  er =>
                  er.EffectiveFromDate.Year == effectiveStartDate.Year &&
                  er.EffectiveFromDate.Month == effectiveStartDate.Month).Count() <= 0)
      {
        var context = new VelocityContext();

        context.Put("Month", effectiveStartDate.ToString("MMM"));
        context.Put("Year", effectiveStartDate.ToString("yyyy"));
        var alert = new ISSISOpsAlert
                      {
                        AlertDateTime = DateTime.UtcNow,
                        EmailAddress = SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail,
                        IsActive = true,
                        Title = "FDR file not received notification",
                        Message =
                          String.Format("Exchange rate for the month of {0} {1} is not yet received. Please take note of the same. Please resolve it ASAP.", effectiveStartDate.ToString("MMM"), effectiveStartDate.ToString("yyyy"))
                      };

        SendNotificationToISAdmin(alert, context, EmailTemplateId.FdrNotReceivedNotification);
      }
    }







    /// <summary>
    /// This method checks whether passed billing period is current or previous billing period
    /// </summary>
    /// <param name="billingPeriod">BillingPeriod value to be validated</param>
    /// <returns>true, if billing period passed is current or previous billing period, false, otherwise</returns>
    public bool ISCurrentorPreviousBillingPeriod(string billingPeriod)
    {
      // Get current billing period value
      _logger.Info(string.Format("CalendarManager instance is: [{0}]", CalendarManager != null ? "NOT NULL" : "NULL"));

      if (CalendarManager == null)
      {
        CalendarManager = Ioc.Resolve<ICalendarManager>();
        _logger.Info(string.Format("CalendarManager instance is: [{0}]", CalendarManager != null ? "NOT NULL" : "NULL"));
      }

      var currbillingPeriod = CalendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(ClearingHouse.Ich);
      _logger.Info(string.Format("Current Billing Period:{0}-{1}-{2}", currbillingPeriod.Year,currbillingPeriod.Month,currbillingPeriod.Period));
      var prevbillingPeriod = CalendarManager.GetLastClosedBillingPeriod(ClearingHouse.Ich);
      _logger.Info(string.Format("Previous Billing Period:{0}-{1}-{2}", prevbillingPeriod.Year, prevbillingPeriod.Month, prevbillingPeriod.Period));
      // If billing period value passed does not matches with current billing period then verify whether previous billing period is passed
      // If billing period passed is not current and not previous billing period then return error
      if ((billingPeriod != currbillingPeriod.Year + currbillingPeriod.Month.ToString().PadLeft(2, '0') + currbillingPeriod.Period.ToString().PadLeft(2, '0')) && (billingPeriod != prevbillingPeriod.Year + prevbillingPeriod.Month.ToString().PadLeft(2, '0') + prevbillingPeriod.Period.ToString().PadLeft(2, '0')))
      {
        _logger.Info("Period Mismatch");
        return false;
      }

      return true;
    }


    public bool ValidateBillingPeriodFormat(string billingPeriod)
    {
      var billingPeriodFormat = new Regex(@"[0-9]{4}(0[1-9]|1[012])[0]{1}[1-4]{1}");

      // If billing period value passed is not in expected format (YYYYMMPP), then return error code 10100
      if (!billingPeriodFormat.IsMatch(billingPeriod))
      {
        return false;
      }

      return true;
    }

    public bool ValidateStartEndDateTime(string inputDateTime, out DateTime? outputDateTime)
    {
      bool isValid = true;

      outputDateTime = null;

      try
      {
        var dateTimeFormat = new Regex(@"^(?=\d)(?:(?!(?:(?:0?[5-9]|1[0-4])(?:-)10(?:-)(?:1582))|(?:(?:0?[3-9]|1[0-3])(?:-)0?9(?:-)(?:1752)))(31(?!(?:-)(?:0?[2469]|11))|30(?!(?:-)0?2)|(?:29(?:(?!(?:-)0?2(?:-))|(?=\D0?2\D(?:(?!000[04]|(?:(?:1[^0-6]|[2468][^048]|[3579][^26])00))(?:(?:(?:\d\d)(?:[02468][048]|[13579][26])(?!\x20BC))|(?:00(?:42|3[0369]|2[147]|1[258]|09)\x20BC))))))|2[0-8]|1\d|0?[1-9])([-])(1[012]|(?:0?[1-9]))\2((?=(?:00(?:4[0-5]|[0-3]?\d)\x20BC)|(?:\d{4}(?:$|(?=\x20\d)\x20)))\d{4}(?:\x20BC)?)(?:$|(?=\x20\d)\x20))?((?:(?:0?[1-9]|1[012])(?::[0-5]\d){0,2}(?:\x20[aApP][mM]))|(?:[01]\d|2[0-3])(?::[0-5]\d){1,2})?$");

        if (!string.IsNullOrWhiteSpace(inputDateTime) && !dateTimeFormat.IsMatch(inputDateTime))
        {
          isValid = false;
        }

        if (isValid)
        {
          outputDateTime = DateTime.ParseExact(inputDateTime, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
        }
        
      }
      catch (Exception)
      {
        isValid = false;
      }

      return isValid;
    }

    #endregion
  }
}

