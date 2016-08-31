using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using Castle.Core.Smtp;
using Iata.IS.AdminSystem;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core.Configuration;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Common;
using log4net;
using NVelocity;

namespace Iata.IS.Business.ValueConfirmation.Impl
{
  public class TrackAtpcoResponseVCF : ITrackAtpcoResponseVCF
  {
    public IRepository<IsInputFile> IsInputFileReository { get; set; }
    private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public void TrackAtpcoResponse()
    {
      // get all  vcf files for which response vcf have not been genertaed
      //TODO: check expected response time condition 
      var files =
        IsInputFileReository.Get(
          f =>
          f.FileFormatId == (int) FileFormatType.ValueConfirmation &&
          f.FileStatusId == (int) FileStatusType.PushedToDestination && f.IsResponseRecieved == 0 &&
          System.Data.Objects.EntityFunctions.AddMinutes(f.ReceivedDate, f.ExpectedResponseTime) < DateTime.UtcNow);

      if (files.Count() > 0)
        SendMail(files);
    }

    private void SendMail(IQueryable<IsInputFile> files)
    {
      try
      {
        //get an object of the EmailSender component
        var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));

        //get an instance of email settings  repository
        var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

        //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
        var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));

        foreach (var file in files)
        {

          //object of the nVelocity data dictionary
          var context = new VelocityContext();

          //fill nvelocity data dictionary 
          context.Put("key", file.FileKey);
          context.Put("date", file.ReceivedDate.ToString("dd-MMM-yyyy",  CultureInfo.CreateSpecificCulture("en-US")));
          context.Put("time", file.ReceivedDate.ToString("HH:mm"));

          //Get the eMail settings for track value confirmation  
          var emailSetting =
            emailSettingsRepository.Get(es => es.Id == (int) EmailTemplateId.AtpcoResponseVcfNotification);

          //generate email body text f
          var body = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.AtpcoResponseVcfNotification, context);

          //create a mail object to send mail
          var overview = new MailMessage {From = new MailAddress(emailSetting.SingleOrDefault().FromEmailAddress)};
          overview.IsBodyHtml = true;

          //loop through the contacts list and add them to To list of mail to be sent
          string[] toEmailList = SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail.Split(new char[] { ',', ';' });

          foreach (var contact in toEmailList)
          {
            overview.To.Add(new MailAddress(contact));
          }

          //set subject of mail 
          overview.Subject = emailSetting.SingleOrDefault().Subject.Replace("$key$", file.FileKey.ToString());

          //set body text of mail
          overview.Body = body;

          logger.Info("Sending mail for request VCF for which response vcf have not been generated: " + body);

          //send the mail
          emailSender.Send(overview);

          //clear nvelocity context data
          context = null;
        }
      }
      catch (Exception ex)
      {
        logger.Error("Exception occured while Sending mail for request VCF for which response vcf have not been generated.", ex);
      }

    }
  }
}
