using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Xml;
using Castle.Core.Smtp;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Common;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core.Configuration;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.ExternalInterfaces.IATAInterface;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.ExternalInterfaces.IATAInterface;
using log4net;
using NVelocity;

namespace Iata.IS.Business.ExternalInterfaces.IATAInterface.Impl
{
  public class IATAInterfaceHandler: IIATAInterfaceHandler
  {
    #region Private Members

    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    
    #endregion

    #region Public Properties
    
    public IIATAInterfaceRepository IATAInterfaceRepository { get; set; }

    public ICalendarManager CalendarManager { get; set; }

    public IEmailSender EmailSender { get; set; }

    public ITemplatedTextGenerator TemplatedTextGenerator { get; set; }

    public IICHXmlHandler ICHXmlHandler { get; set; }

    public IFileManager FileManager { get; set; }

    private const string rechargeDataFolder = @"App_Data\SchemaFiles\RechargeData.xsd"; 

    /// <summary>
    /// List to store recharge data.
    /// </summary>
    public List<RechargeData> RechargeDataList { get; set; }

    #endregion

    #region IIATAInterfaceHandler Method Implementation

    /// <summary>
    /// 1. Generates and place recharge data file on FTP for download by IATA.
    /// 2. An email is sent to IATA contact informing recharge data file available for download on FTP.
    /// 3. In case of any error email notification is sent to IS Admin informing the error occured.
    /// </summary>
    public void GenerateAndPlaceRechargeDataOnFTP()
    {
      VelocityContext context;
      try
      {
        BillingPeriod billingPeriod = CalendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);

        // Start period filter
        DateTime startDate = new DateTime(billingPeriod.Year, billingPeriod.Month, 1, 0, 0, 0).AddMonths(-1);

        _logger.Debug(String.Format("From Filter- Year: {0}, Month: {1}, Period: {2}", startDate.Year.ToString("0000", CultureInfo.InvariantCulture), startDate.Month.ToString("00", CultureInfo.InvariantCulture), startDate.Day.ToString("00", CultureInfo.InvariantCulture)));

        // End period filter
        DateTime endDate = startDate.AddDays(3);

        _logger.Debug(String.Format("From Filter- Year: {0}, Month: {1}, Period: {2}", endDate.Year.ToString("0000", CultureInfo.InvariantCulture), endDate.Month.ToString("00", CultureInfo.InvariantCulture), endDate.Day.ToString("00", CultureInfo.InvariantCulture)));

        // Get Recharge Data.
        RechargeDataList = IATAInterfaceRepository.GetRechargeData(startDate, endDate, 0);

        // Get Recharge Data XML.
        string rechargeDataXML = ICHXmlHandler.SerializeXml(PopulateClearanceMonths(), typeof(ClearanceMonths));

        _logger.Debug("Recharge Data XML:");
        _logger.Debug(rechargeDataXML);

        var XSDPath = string.Format("{0}{1}",ConnectionString.GetAppSetting("AppSettingPath"), rechargeDataFolder);
        if (string.IsNullOrEmpty(rechargeDataXML) || ICHXmlHandler.Validate(rechargeDataXML, XSDPath) != "OK")
        {
          throw new Exception("Recharge Data XML generated is empty or is not valid xml.");
        }// End if

        _logger.Debug("Initiating copying file to FTP folder");
        // Place file on FTP server.
        //var fileMgr = Ioc.Resolve<IFileManager>(typeof(IFileManager));

        string rechargeDataFileName = String.Format("SIS_RECHARGE_DATA_{0}{1}.xml",
                                                    startDate.Year.ToString("0000", CultureInfo.InvariantCulture),
                                                    startDate.Month.ToString("00", CultureInfo.InvariantCulture));

        // Create and save recharge data xml in IATA FTP Download folder.
        if (FileManager.UploadFileForFTPPull(rechargeDataXML, rechargeDataFileName, FileFormatType.RechargeDataXml))
        {
          context = new VelocityContext();
          context.Put("FileName", rechargeDataFileName);
          context.Put("SISOpsEmailId", SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
          SendNotificationToISAdmin(context, EmailTemplateId.RechargeDataAvailableForDownloadNotification,
                                    SystemParameters.Instance.IataDetails.IataContactEmail);
          return;
        }// End if

        throw new Exception("Error in placing recharge data file on FTP.");
        
      }// End try
      catch (Exception ex)
      {
        context = new VelocityContext();
        context.Put("Error", ex.Message);
        SendNotificationToISAdmin(context, EmailTemplateId.RechargeDataGenerationFailureNotification,
                                  SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
      }// End catch
      
    }// End GenerateAndPlaceRechargeDataOnFTP()

    #endregion

    #region Private Member Methods

    //private void SendInvoiceFailureNotificationToISAdmin(string clearancePeriod)
    private void SendNotificationToISAdmin(VelocityContext context, EmailTemplateId templateId, string receiverEmail)
    {

      try
      {
        //get an object of the EmailSender component
        //var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

        //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
        //var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));

        //Get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

        var emailSettingForISAdminAlert = emailSettingsRepository.Get(esfopu => esfopu.Id == (int)templateId);

        // Verify and log TemplatedTextGenerator for null value.
        _logger.Debug(String.Format("TemplatedTextGenerator instance is: [{0}].", TemplatedTextGenerator == null ? "NULL" : "NOT NULL"));

        // Generate email body text for own profile updates contact type mail
        // ReSharper disable PossibleNullReferenceException
        var emailToISAdminText = TemplatedTextGenerator.GenerateTemplatedText(templateId, context);
        // ReSharper restore PossibleNullReferenceException

        // Create a mail object to send mail
        var msgForISAdminAlert = new MailMessage
        {
          From = new MailAddress(emailSettingForISAdminAlert.SingleOrDefault().FromEmailAddress),
          IsBodyHtml = true
        };
        //loop through the contacts list and add them to To list of mail to be sent
        msgForISAdminAlert.To.Add(receiverEmail); // new MailAddress(ConfigurationManager.AppSettings["ISAdminEmail"])

        //set subject of mail (replace special field placeholders with values)
        var subject = emailSettingForISAdminAlert.SingleOrDefault().Subject;
        msgForISAdminAlert.Subject = subject;

        //set body text of mail
        msgForISAdminAlert.Body = emailToISAdminText;

        // Verify and log EmailSender for null value.
        _logger.Debug(String.Format("EmailSender instance is: [{0}]", EmailSender == null ? "NULL" : "NOT NULL"));

        //send the mail
        // ReSharper disable PossibleNullReferenceException
        EmailSender.Send(msgForISAdminAlert);
        // ReSharper restore PossibleNullReferenceException

      }// End try
      catch (Exception ex)
      {

        _logger.Error("Error occurred while sending Invoice Settlement Failure Notification to IS Administrator", ex);

        throw new ISBusinessException(ErrorCodes.ErrorSendingRechargeDataNotification, "Error ocurred while sending  Notification to IS Administrator");

      }// End catch
    }

    /// <summary>
    /// Create and populate ClearanceMonths object.
    /// </summary>
    /// <returns>ClearanceMonths object</returns>
    private ClearanceMonths PopulateClearanceMonths()
    {
      return new ClearanceMonths
               {
                 ClearanceMonth = RechargeDataList.Select(rd => rd.YearAndMonth).Distinct().ToArray(), 
                 ClearancePeriods = PopulateClearancePeriod().ToArray()
               };
    }// End PopulateClearanceMonths()

    /// <summary>
    /// Create and populate list of ClearancePeriod objects.
    /// </summary>
    /// <returns>List of ClearancePeriod objects</returns>
    private List<ClearancePeriod> PopulateClearancePeriod()
    {
      // Get distinct clearance periods from recharge data.
      var clearancePeriods = RechargeDataList.Select(rd => rd.PeriodNo).Distinct().OrderBy(r => r);

      return clearancePeriods.Select(clearancePeriod => new ClearancePeriod
                                                          {
                                                            ClearancePeriodNo =
                                                              clearancePeriod.ToString("00",
                                                                                       CultureInfo.InvariantCulture),
                                                            MemberStatistics =
                                                              PopulateMemberStatistics(clearancePeriod).ToArray()
                                                          }).ToList();
    }// End PopulateClearancePeriod()

    /// <summary>
    /// Create and populate list of MemberStatistics objects.
    /// </summary>
    /// <param name="clrearancePeriod">Clearance period for which MemberStatistics is populated.</param>
    /// <returns>List of MemberStatistics objects</returns>
    private List<MemberStatistics> PopulateMemberStatistics(int clrearancePeriod)
    {
      // Get distinct member for a given clearance period from recharge data.
      var members =
        RechargeDataList.Where(rd => rd.PeriodNo == clrearancePeriod).Select(rd => new {rd.AlphaCode, rd.NumericCode}).
          Distinct();

      return
        members.Select(
          member =>
          RechargeDataList.Where(
            rd =>
            rd.PeriodNo == clrearancePeriod && rd.AlphaCode == member.AlphaCode && rd.NumericCode == member.NumericCode)
            .FirstOrDefault()).Select(rechargeData => new MemberStatistics
                                                        {
                                                          AlphaCode = rechargeData.AlphaCode,
                                                          NumericCode = rechargeData.NumericCode,
                                                          //CMP605 : Inclusion of new fields in Usage report and IATA recharge XML file.
                                                          //Encoding values for membership status and membership sub status fields as stated in FRS. 
                                                          //SCP282127 - TOU E&F sub status is encoded wrongly in the recharge XML
                                                          //Implicitly SerializeXml function encode the text so explicit encode call should be disable
                                                          ISMembershipStatus = rechargeData.MembershipStatus,//WebUtility.HtmlEncode(rechargeData.MembershipStatus),
                                                          ISMembershipSubStatus = rechargeData.MembershipSubStatus,//WebUtility.HtmlEncode(rechargeData.MembershipSubStatus),
                                                          Statistic =
                                                            PopulateStatistic(clrearancePeriod, rechargeData.AlphaCode,
                                                                              rechargeData.NumericCode).ToArray()
                                                        }).ToList();
    }// End  PopulateMemberStatistics()

    /// <summary>
    /// Create and populate list of Statistic objects.
    /// </summary>
    /// <param name="clrearancePeriod">Clearance Period filter</param>
    /// <param name="alphaCode">Member alpha code filter</param>
    /// <param name="numericCode">Member Numeric Code filter</param>
    /// <returns>List of Statistic objects</returns>
    private List<Statistic> PopulateStatistic(int clrearancePeriod, string alphaCode, string numericCode)
    {
       
      List<Statistic> statisticList = new List<Statistic>();

      // Get distinct ISSubmission for given clearance period, alpha code, numeric code from recharge data.
      var submissionMethods =
        RechargeDataList.Where(
          rd => rd.PeriodNo == clrearancePeriod && rd.AlphaCode == alphaCode && rd.NumericCode == numericCode).Select(
            rd => rd.ISSubmission).Distinct();

      foreach (var submissionMethod in submissionMethods)
      {
        if (submissionMethod != null)
        {
          var rechargeData = RechargeDataList.Where(
            rd =>
            rd.PeriodNo == clrearancePeriod && rd.AlphaCode == alphaCode && rd.NumericCode == numericCode &&
            rd.ISSubmission == submissionMethod).FirstOrDefault();

          if (rechargeData != null)
              statisticList.Add(new Statistic
                                    {
                                        CountDigitalSignatureValidationCGOrcvd =
                                            rechargeData.DigiSignValidCGORcvd.ToString(),
                                        CountDigitalSignatureValidationCGOsent =
                                            rechargeData.DigiSignValidCGOSent.ToString(),
                                        CountDigitalSignatureValidationMISCrcvd =
                                            rechargeData.DigiSignValidMISCRcvd.ToString(),
                                        CountDigitalSignatureValidationMISCsent =
                                            rechargeData.DigiSignValidMISCSent.ToString(),
                                        CountDigitalSignatureValidationPAXrcvd =
                                            rechargeData.DigiSignValidPAXRcvd.ToString(),
                                        CountDigitalSignatureValidationPAXsent =
                                            rechargeData.DigiSignValidPAXSent.ToString(),
                                        CountDigitalSignatureValidationUATPrcvd =
                                            rechargeData.DigiSignValidUATPRcvd.ToString(),
                                        CountDigitalSignatureValidationUATPsent =
                                            rechargeData.DigiSignValidUATPSent.ToString(),
                                        CountEArchivingCGO = rechargeData.EArchivingCGO.ToString(),
                                        CountEArchivingMISC = rechargeData.EArchivingMISC.ToString(),
                                        CountEArchivingPAX = rechargeData.EArchivingPAX.ToString(),
                                        CountEArchivingUATP = rechargeData.EArchivingUATP.ToString(),
                                        TotalSizeEArchivingPAX = rechargeData.TotalSizeEArchivingPAX.ToString(),
                                        TotalSizeEArchivingCGO = rechargeData.TotalSizeEArchivingCGO.ToString(),
                                        TotalSizeEArchivingMISC = rechargeData.TotalSizeEArchivingMISC.ToString(),
                                        TotalSizeEArchivingUATP = rechargeData.TotalSizeEArchivingUATP.ToString(),
                                        CountInvoicesCGO = rechargeData.CGOInvoices.ToString(),
                                        CountInvoicesMISC = rechargeData.MISCInvoices.ToString(),
                                        CountInvoicesPAX = rechargeData.PAXInvoices.ToString(),
                                        CountInvoicesUATP = rechargeData.UATPInvoices.ToString(),
                                        CountSupportingDocumentsCGO = rechargeData.SupportingDocsCGO.ToString(),
                                        CountSupportingDocumentsMISC = rechargeData.SupportingDocsMISC.ToString(),
                                        CountSupportingDocumentsPAX = rechargeData.SupportingDocsPAX.ToString(),
                                        CountSupportingDocumentsUATP = rechargeData.SupportingDocsUATP.ToString(),
                                        CountTransactionsCGOBillingMemo = rechargeData.CGOBillingMemos.ToString(),
                                        CountTransactionsCGOCorrespondence = rechargeData.CGOCorrespondences.ToString(),
                                        CountTransactionsCGOCreditMemo = rechargeData.CGOCreditMemos.ToString(),
                                        CountTransactionsCGOOriginalBillingAWB = rechargeData.CGOOriginalBillingAWBS.ToString(),
                                        CountTransactionsCGORejectionMemo = rechargeData.CGORejectionMemos.ToString(),
                                        CountTransactionsMISCCorrespondence = rechargeData.MISCCorrespondences.ToString(),
                                        CountTransactionsUATPCorrespondence = rechargeData.UATPCorrespondences.ToString(),
                                        CountTransactionsPaxAutoBillingRequest = rechargeData.PAXAutoBillingRequests.ToString(),
                                        CountTransactionsPaxValueDeterminationRequest =  rechargeData.PAXVDeterminationRequests.ToString(),
                                        CountTransactionsPaxBillingMemo = rechargeData.PAXBillingMemos.ToString(),
                                        CountTransactionsPaxCorrespondence = rechargeData.PAXCorrespondences.ToString(),
                                        CountTransactionsPaxCreditMemo = rechargeData.PAXCreditMemos.ToString(),
                                        CountTransactionsPAXPrimeCoupon = rechargeData.PAXPrimeCoupons.ToString(),
                                        CountTransactionsPaxRejectionMemoInclSampling = rechargeData.PAXRejectionMemosInclSampling.ToString(),
                                        CountTransactionsPaxSamplingDigitEvaluationCoupon = rechargeData.PAXSamplingDigitEvalCoupons.ToString(),
                                        CountTransactionsPaxSamplingProvisionInvoiceCoupon = rechargeData.PAXSamplingProvInvoiceCoupons.ToString(),
                                        CountTransactionsPAXSamplingUAFCoupon = rechargeData.PAXSamplingUAFCoupons.ToString(),
                                        ISSubmission = rechargeData.ISSubmission,
                                        //CMP605 : Inclusion of new fields in Usage report and IATA recharge XML file.
                                        ReceivedCountInvoicesPAX = rechargeData.PAXInvoicesPayable.ToString(),
                                        ReceivedCountInvoicesCGO = rechargeData.CGOInvoicesPayable.ToString(),
                                        ReceivedCountInvoicesMISC = rechargeData.MISCInvoicesPayable.ToString(),
                                        ReceivedCountInvoicesUATP = rechargeData.UATPInvoicesPayable.ToString()
                                    }

              );
        }// End if
      }// End foreach

      return statisticList;
    }// End PopulateStatistic()

    #endregion

  }
}
