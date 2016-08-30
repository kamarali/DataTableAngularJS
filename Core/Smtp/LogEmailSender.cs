using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using Castle.Core.Smtp;
using log4net;

namespace Iata.IS.Core.Smtp
{
  public class LogEmailSender : IEmailSender
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public void Send(string from, string to, string subject, string messageText)
    {
      Logger.Info(string.Format("IEmailSender::Send called. From: [{0}], To: [{1}], Subject: [{2}], Body: [{3}]", from, to, subject, messageText));
    }

    public void Send(MailMessage message)
    {
      if (message == null)
      {
        throw new ArgumentNullException("message");
      }

      Logger.Info(string.Format("IEmailSender::Send called. From: [{0}], To: [{1}], Subject: [{2}], Body: [{3}]", message.From, message.To, message.Subject, message.Body));
    }

    public void Send(IEnumerable<MailMessage> messages)
    {
      var stringBuilder = new StringBuilder();

      if (messages == null)
      {
        throw new ArgumentNullException("messages");
      }

      foreach (var message in messages)
      {
        stringBuilder.AppendFormat("IEmailSender::Send called for multiple messages. From: [{0}], To: [{1}], Subject: [{2}], Body: [{3}]", message.From, message.To, message.Subject, message.Body);
      }

      Logger.Info(stringBuilder);
    }
  }
}
