using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NVelocity;
using Iata.IS.Model.BroadcastMessages;
using Iata.IS.Business.BroadcastMessages;
using Iata.IS.Model.Enums;

namespace Iata.IS.Business.Common.Impl
{
  public class ParsingNotificationManager : IParsingNotificationManager
  {
    public IBroadcastMessagesManager BroadcastMessagesManager { get; set; }

    public void SendSisAdminAlert(string fileName)
    {      
       //Create an object of the nVelocity data dictionary
      var context = new VelocityContext();
      context.Put("FileName", fileName);
   
      const string message = "Parsing and Validation failure for file {0}";
      const string title = "Parsing and validation failure alert";

      String internalTeamAdminEmailAddress = Iata.IS.Core.Configuration.ConnectionString.GetconfigAppSetting("ExceptionEmailNotification");

      var issisOpsAlert = new ISSISOpsAlert
      {
        Message = String.Format(message, fileName),
        AlertDateTime = DateTime.UtcNow,
        IsActive = true,
        EmailAddress = AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail + "; " + internalTeamAdminEmailAddress,
        Title = title
      };

      BroadcastMessagesManager.AddAlert(issisOpsAlert, EmailTemplateId.SISAdminAlertParsingAndValidationFailureNotification, context);
    }
  }
}
