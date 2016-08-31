using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.Core.Smtp;
using Iata.IS.AdminSystem;
using Iata.IS.Business.BroadcastMessages.Impl;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Model.BroadcastMessages;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using NVelocity;
using log4net;
using log4net.Repository.Hierarchy;

namespace Iata.IS.Business.Common.Impl
{
  public class EmailManager : IEmailSender, IEmailManager 
  {
    private UnitOfWork unitOfWork = new UnitOfWork(new ObjectContextAdapter());

    private IRepository<IsEmail> isEmailRepository;

    private IRepository<IsEmailAttachment> isEmailAttachmentRepository;

    public readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public EmailManager()
    {
      isEmailRepository = new Repository<IsEmail>(unitOfWork);
      //Ioc.Resolve<IRepository<IsEmail>>(typeof(IRepository<IsEmail>));
      isEmailAttachmentRepository = new Repository<IsEmailAttachment>(unitOfWork); 
      //Ioc.Resolve<IRepository<IsEmailAttachment>>(typeof(IRepository<IsEmailAttachment>));
    }
    public void Send(IEnumerable<System.Net.Mail.MailMessage> messages)
    {
      foreach(var message in messages)
        this.Send(message);
    }

    public void Send(System.Net.Mail.MailMessage message)
    {
      try
      {
          var smptSetting = Iata.IS.Core.Configuration.ConnectionString.GetSmptSetting();
          var email = new IsEmail
            {
              FromEmailAddress = smptSetting.FromAddress,
              ToEmailAddress = String.Join(";", message.To.Select(to => to.Address).ToList()),
              BccEmailAddress = String.Join(";", message.Bcc.Select(bcc => bcc.Address).ToList()),
              CcEmailAddress = String.Join(";", message.CC.Select(cc => cc.Address).ToList()),
              Subject = message.Subject,
              Body = message.Body,
              IsBodyHtml = message.IsBodyHtml,
              Status = "Q",
              LastUpdatedBy = 0
            };

          //SCP#312874 : No Emails from SIS
          //dont add entry of email in IS_EMAIL table when ToEmailAddress is null or blank
          bool emailAddressIsNull = true;
          if (email.ToEmailAddress != null && !string.IsNullOrWhiteSpace(email.ToEmailAddress.ToString()))
          {
            emailAddressIsNull = false;
          }
          if (emailAddressIsNull && (email.BccEmailAddress != null && !string.IsNullOrWhiteSpace(email.BccEmailAddress.ToString())))
          {
            emailAddressIsNull = false;
          }
          if (emailAddressIsNull && (email.CcEmailAddress != null && !string.IsNullOrWhiteSpace(email.CcEmailAddress.ToString())))
          {
            emailAddressIsNull = false;
          }
          if (emailAddressIsNull)
          {
            return;
          }


        if (isEmailRepository == null)
        {
            Logger.InfoFormat("isEmailRepository is null");
            Ioc.Initialize();
            isEmailRepository = Ioc.Resolve<IRepository<IsEmail>>(typeof(IRepository<IsEmail>));
        }
        isEmailRepository.Add(email);
        //UnitOfWork.CommitDefault();
        CommitChanges();

        Logger.InfoFormat("New Email Added: {0}",email.Id);

        string emailAttachmentFolder = String.Format(@"{0}\{1}\{2}",
                                                     AdminSystem.SystemParameters.Instance.General.TempInvoiceOutputFiles,
                                                     "Email", email.Id.ToString());

        BinaryReader attachmentReader;

        foreach (var attachment in message.Attachments)
        {
          if (!Directory.Exists(emailAttachmentFolder))
            Directory.CreateDirectory(emailAttachmentFolder);

          string attachmentFilePath = String.Format(@"{0}\{1}", emailAttachmentFolder, attachment.Name);
          
          using(attachmentReader = new BinaryReader(attachment.ContentStream))
          {
            File.WriteAllBytes(attachmentFilePath, attachmentReader.ReadBytes(Convert.ToInt32(attachmentReader.BaseStream.Length)));
          }

          isEmailAttachmentRepository.Add(new IsEmailAttachment
                                                              {
                                                                IsEmailId = email.Id,
                                                                AttachmentFilePath = attachmentFilePath
                                                              });

        }

        CommitChanges();
        //UnitOfWork.CommitDefault();

        Logger.InfoFormat("Added attachement for: {0}", email.Id);

        EnqueueEmail(email.Id,0);
      }
      catch(Exception ex)
      {
        Logger.ErrorFormat("Error occurred in saving and enqueuing email in DB. Error Msg:{0}, Stack tress:{1}",ex.Message,ex.StackTrace);
      }
      
    }

    

    public void EnqueueEmail(int isEmailId, int delay)
    {

      Logger.Info("Enqueuing values.IS_EMAIL_ID: " + isEmailId.ToString());

      try
      {
        // enqueue message
        IDictionary<string, string> messages = new Dictionary<string, string> {
                { "IS_EMAIL_ID", isEmailId.ToString() }
            };
        var queueHelper = new QueueHelper(ConfigurationManager.AppSettings["EmailQueueName"].Trim());

        queueHelper.Enqueue(messages,1,delay);

        Logger.Info("Enqueued values.");

      } // end try

      catch (Exception exception)
      {
        Logger.Error("Error occurred while adding message to queue.", exception);
      } // end catch

    }

    public void Send(string from, string to, string subject, string messageText)
    {
      throw new NotImplementedException();
    }

    public IsEmail GetIsEmail(int emailId)
    {
      var email = isEmailRepository.First(e => e.Id == emailId);
      if(email != null)
      {
        email.Attachments = isEmailAttachmentRepository.Get(a => a.IsEmailId == email.Id).ToList();
      }

      return email;
    }

    public void CommitChanges()
    {
      unitOfWork.Commit();
    }

  }
}
