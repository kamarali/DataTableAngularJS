using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;

using System.Xml;
using Castle.Core.Smtp;
using FileHelpers;
using Iata.IS.AdminSystem;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Data.ValueConfirmation;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.SanityCheck;
using Iata.IS.Model.ValueConfirmation;
using log4net;
using NVelocity;

namespace Iata.IS.Business.ValueConfirmation.Impl
{

  public class ResponseVCF : IResponseVCF
  {

    private readonly IResponseVCFFileHelper _responseVcfFileHelper;
    public IRepository<IsInputFile> IsInputFileReository { get; set; }
    public IRepository<PrimeCoupon> CouponReository { get; set; }
    private IResponseValueConfirmationRepository _responseVcfRepository;

    private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public ResponseVCF(IResponseVCFFileHelper responseVcfFileHelper, IResponseValueConfirmationRepository responseVcfRepository)
    {
      _responseVcfFileHelper = responseVcfFileHelper;
      _responseVcfRepository = responseVcfRepository;
    }

    public void ReadResponseVCF(string filePath, string fileName, string fileId)
    {
 
      var couponString = new StringBuilder();

      var fileIdGuid = Guid.Parse(fileId);

      var couponlist = _responseVcfFileHelper.ReadResponseVCF(filePath);

      if (couponlist == null)
      {
          logger.Info("Coupon list is null in VCF file ");
          return;
      }
      
      // prepare coupon string
      foreach (var rvf in couponlist)
      {

        couponString.Append(rvf.Vast.ToUpper());
        couponString.Append(rvf.PMIValidated + rvf.AgreementIndicatorValidated + rvf.NfpReasionCodeValidated);
        couponString.Append(rvf.ProrateMethodology + rvf.MonthofSale + rvf.YearofSale);
        couponString.Append(rvf.BilledProrateAmount + rvf.ProrateAmount);
        couponString.Append(rvf.BaseCurrencyofDPRO + rvf.BilledTotalTaxAmount + rvf.TotalTaxAmount);

        couponString.Append(rvf.PublishedTaxAmountCurrency1 + rvf.PublishedTaxAmountCurrency2 +
                            rvf.PublishedTaxAmountCurrency3);
        couponString.Append(rvf.PublishedTaxAmountCurrency4 + rvf.IscPercentage + rvf.BilledHandlingFeeAmount);
        couponString.Append(rvf.HandlingFeeAmount + rvf.HandlingFeeBaseCurrency + rvf.UATPPercentage);
        couponString.Append(rvf.ReasonCode);

        couponString.Append(rvf.CouponId);
      }

      // call sp
      logger.Info("Sp Call in ResponseVCF method for response vcf update service");
      _responseVcfRepository.UpdateValueConfirmationData(couponString.ToString(), fileIdGuid);
      logger.Info("Sp Call end ResponseVCF method response vcf update service  ");
      // get the distinct group header having vast == N
      var distinctGroupHeader = couponlist.Where(c => c.Vast == "N").Select(c => c.GroupHeaderLineNo).Distinct();

      // send mail if get any validation failed group header (VAST == N)
      foreach (
        var record in distinctGroupHeader.Select(gh => couponlist.Where(c => c.GroupHeaderLineNo == gh).First()))
      {
        SendMail(record, fileName);
      }
    }

   
    private void SendMail(ResponseVCFRequiredFields validationFailGroupHeader, string fileName)
    {
      try
      {
        //get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

        //get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

        //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));

        //object of the nVelocity data dictionary
        var context = new VelocityContext();

        // if first character is not 0 for billing and billed air line then take 
        // initial two numbers and convert them two ascii equivalent
        var billingAirline = validationFailGroupHeader.BillingAirline;
        if (!validationFailGroupHeader.BillingAirline.StartsWith("0"))
        {
          var num = Convert.ToInt32(billingAirline.Substring(0, 2));

          //SCPID : 124462 - SRM: Mismatch in BVC participation data 0086/8301
          var c = (char)(num + 55);
          billingAirline = c + billingAirline.Substring(2);
        }
        var billedgAirline = validationFailGroupHeader.BilledAirline;
        if (!validationFailGroupHeader.BilledAirline.StartsWith("0"))
        {
            var num = Convert.ToInt32(billedgAirline.Substring(0, 2));

          //SCPID : 124462 - SRM: Mismatch in BVC participation data 0086/8301
          var c = (char)(num + 55);
          billedgAirline = c + billedgAirline.Substring(2);
        }

        //fill nvelocity data dictionary 
        context.Put("BillingAirline", billingAirline);
        context.Put("BilledAirline", billedgAirline);
        context.Put("FileName", fileName);
        context.Put("RecordPosition", validationFailGroupHeader.GroupHeaderLineNo);

        //Get the eMail settings for value confirmation overview 
        var emailSetting = emailSettingsRepository.Get(es => es.Id == (int)EmailTemplateId.ValueConfirmationFailNotification);

        //generate email body text f
        var body = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.ValueConfirmationFailNotification, context);

        //create a mail object to send mail
        var overview = new MailMessage { From = new MailAddress(emailSetting.SingleOrDefault().FromEmailAddress) };
        overview.IsBodyHtml = true;

        //loop through the contacts list and add them to To list of mail to be sent
        string[] toEmailList = SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail.Replace(';',',').Split(',');
      
        foreach (var contact in toEmailList)
        {
          logger.Info("value confirmation validation Email contact: " + contact);
          overview.To.Add(new MailAddress(contact));
        }

        //set subject of mail 
        overview.Subject = emailSetting.SingleOrDefault().Subject.Replace("$BillingMember$", billingAirline);
        overview.Subject = overview.Subject.Replace("$BilledMember$", billedgAirline);

        //set body text of mail
        overview.Body = body;

        logger.Info("Sending mail for value confirmation validation fail: " + body);

        //send the mail
        emailSender.Send(overview);

        //clear nvelocity context data
        context = null;
      }
      catch (Exception ex)
      {
        logger.Error("Exception occured while sending mail to IS-Admin for Value Confirmation failed group header in response VCF file.", ex);
      }

    }

  }
}
