using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Web;
using Castle.Core.Smtp;
using Iata.IS.AdminSystem;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core.DI;
using Iata.IS.Data;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using iPayables;
using iPayables.UserManagement;
using log4net;
using NVelocity;
using Iata.IS.Data.MemberProfile;
using System.Security.Cryptography;
using Iata.IS.Core;
using iPayables.Security;

namespace Iata.IS.Business.MemberProfile.Impl
{
  public class UserManagementManager : IUserManagementManager
  {

    public IUserManagement AuthManager { get; set; }
    public IUserManagementRepository UserManagementData { get; set; }
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public Repository<IS_USER> UserReporsitory { get; set; }

    //###### CMP685: User management  #####################################
    //below methods are updtated from CMP685
    //##########################################################################

    public bool SendUserWelcomeNotification(string firstName, string lastName, string emailId, string urlLink)
    {
      bool flag;
      //create nvelocity data dictionary
      var context = new VelocityContext();
      //get an instance of email settings  repository
      var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
      //get an object of the EmailSender component
      var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
      //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
      var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
            
      try
      {

        context.Put("FirstName", CheckEmptystring(firstName));
        context.Put("LastName", CheckEmptystring(lastName));
        context.Put("EmailAddress", CheckEmptystring(emailId));
        context.Put("SisOpsEmailId", CheckEmptystring(SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail));
        context.Put("LogonURL", CheckEmptystring(urlLink));
        var emailToInvoice = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.UserWelcomeNotification, context);

        Logger.Error("Template Created for User Notification.");

        //Get the eMail settings for user welcome notification
        var emailSettingForUserNotification =
          emailSettingsRepository.Get(esfirdu => esfirdu.Id == (int)EmailTemplateId.UserWelcomeNotification);

        //create a mail object to send mail 
        var msgUserNotification = new MailMessage
        {
          From =
            new MailAddress(emailSettingForUserNotification.SingleOrDefault().FromEmailAddress)
        };
        msgUserNotification.IsBodyHtml = true;

        //loop through the contacts list and add them to To list of mail to be sent

        string emailTo = emailId;


        msgUserNotification.To.Add(new MailAddress(emailTo));

        //set subject of mail (replace special field placeholders with values)
        msgUserNotification.Subject = emailSettingForUserNotification.SingleOrDefault().Subject;

        //set body text of mail
        msgUserNotification.Body = emailToInvoice;

        //send the mail
        emailSender.Send(msgUserNotification);



        //clear nvelocity context data
        context = null;
        flag = true;
      }

      catch (Exception exception)
      {
        Logger.Error("Error occurred occured in Sending mail for User Notification.", exception);
        flag = false;
      }

      return flag;

    }


    public bool SendUserResetPasswordUrlLink(string firstName, string lastName, string emailId, string urlLink, string firstLineContent)
    {


      bool flag;
      //create nvelocity data dictionary
      var context = new VelocityContext();
      //get an instance of email settings  repository
      var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));
      //get an object of the EmailSender component
      var emailSender = Ioc.Resolve<IEmailSender>(typeof(IEmailSender));
      //get an object of the TemplatedTextGenerator that is used to generate body text of email from a nvelocity template
      var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));


      if (String.IsNullOrEmpty(emailId)) return false;

      try
      {

        context.Put("FirstName", CheckEmptystring(firstName));
        context.Put("LastName", CheckEmptystring(lastName));
        context.Put("urlLink", CheckEmptystring(urlLink));
        context.Put("firstLineContent", CheckEmptystring(firstLineContent));
        context.Put("SisOpsEmailId", CheckEmptystring(SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail));        

        var emailTouser = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.UserChangedPasswordNotification, context);

        Logger.Error("Template Created for User changed  password Notification.");

        //Get the eMail settings for user welcome notification
        var emailSettingForUserNotification =
          emailSettingsRepository.Get(esfirdu => esfirdu.Id == (int)EmailTemplateId.UserChangedPasswordNotification);

        //create a mail object to send mail 
        var msgUserNotification = new MailMessage
        {
          From =
            new MailAddress(emailSettingForUserNotification.SingleOrDefault().FromEmailAddress)
        };
        msgUserNotification.IsBodyHtml = true;

        //loop through the contacts list and add them to To list of mail to be sent

        string emailTo = emailId;


        msgUserNotification.To.Add(new MailAddress(emailTo));

        //set subject of mail (replace special field placeholders with values)
        msgUserNotification.Subject = emailSettingForUserNotification.SingleOrDefault().Subject;

        //set body text of mail
        msgUserNotification.Body = emailTouser;

        //send the mail
        emailSender.Send(msgUserNotification);

        //clear nvelocity context data
        context = null;
        flag = true;
      }

      catch (Exception exception)
      {
        Logger.Error("Error occurred occured in Sending mail for User Notification.", exception);
        flag = false;
      }

      return flag;

    }

    /// <summary>
    /// create and send the reset password url link
    /// </summary>
    /// <param name="userId">user id</param>
    /// <param name="logOnUrl">base log on url</param>
    /// <param name="carrier">
    /// In: from where its been called.
    /// Out: Complete reset url link </param>
    /// <returns></returns>
    public bool ResetUserPassword(int userId, string logOnUrl, ref string carrier)
    {
      if (userId <= 0) throw new ArgumentException("UserID");
      var userInformation = (ISUser)AuthManager.GetUserByUserID(userId);

      var flag = false;

      try
      {
        if (userInformation.UserID > 0)
        {
          // create object to insert information in database
          var userpass = new ISUserPassReset
          {
            IsUserId = userInformation.UserID,
            LinkId = Guid.NewGuid(), //use new uniqe GUID here as identifier for each password reset url link
            Used = 0
          };

          if (UserManagementData.UpdateIsUserPassResetData(userpass)) // update DB with entry
          {
            // create reset url link here (also Encrypt the identifier key)
            var incodedquerystring = Crypto.EncryptString(ConvertUtil.ConvertGuidToString(userpass.LinkId));
            string resetUrlLink = logOnUrl + (incodedquerystring);
                       
            
            string firstLineContent = string.Empty;
            switch (carrier)
            {
              case "R":// if called from Reset password 
                firstLineContent = "There has been a request to reset the password in your IS user account.";
                //send user intimation mail on his registered E-Mail address containing dynamic url link  
                SendUserResetPasswordUrlLink(userInformation.FirstName, userInformation.LastName, userInformation.Email, resetUrlLink, firstLineContent);
                break;
              case "F":// if called from Forgot password 
                firstLineContent = "You have received this email since you chose the “Forgot Password” option.";
                //send user intimation mail on his registered E-Mail address containing dynamic url link  
                SendUserResetPasswordUrlLink(userInformation.FirstName, userInformation.LastName, userInformation.Email, resetUrlLink, firstLineContent);
                break;
              case "N":// if called for New User Creation from IS-WEB
                // email will be send from calling method after this method execution (old code was working same so did not change)
                break;
              case "NF":// if called for New User Creation from memberprofile CSV uplaod
                SendUserWelcomeNotification(userInformation.FirstName, userInformation.LastName, userInformation.Email, resetUrlLink);
                break;
            }

            //return the complete url link with carrier
            carrier = resetUrlLink;

            flag = true;
          }

        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error occurred occured in ResetUserPassword().", ex);
        flag = false;
      }
      return flag;
    }

    /// <summary>
    /// Validate the Dynamic URL Link
    /// </summary>
    /// <param name="input"></param>
    /// <param name="timeFrame"></param>
    /// <param name="userid"></param>
    /// <param name="linkid"></param>
    /// <returns></returns>
    public bool ValidateResetPasswordLink(string input,int timeFrame, ref int userid, ref Guid linkid)
    {
      try
      {
        //some time encryption replaces the '+' to ' ' while encryption, here reverse it before Decryption
        var decodedquerystring = Crypto.DecryptString(input.Replace(" ", "+"));

        //validate the identifier key from database
        var passResetData = UserManagementData.GetIsUserPassResetData(ConvertUtil.ConvertStringtoGuid(decodedquerystring), timeFrame);

        if (passResetData != null)
        {
          // return back the identifier key and associated User Id
          linkid = passResetData.LinkId;
          userid = passResetData.IsUserId;
          
          //SIS_SCR_REPORT_23_jun-2016_2: Heap_Inspection
          passResetData = null;
          
          return true;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error occurred occured in ValidateResetPasswordLink().", ex);
        return false;
      }

      return false;
    }

    /// <summary>
    /// Delete the Dynamic Link Data once used
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="linkid"></param>
    /// <returns></returns>
    public bool DeleteResetPasswordLinkDataOnceUsed(int userid, Guid linkid)
    {
      try
      {
        // update the data once dynamic link has been used.
        // here set the "Used" flag as a True and update the record.
        // This type of data needs to be purge by purging process further.
        var userpass = new ISUserPassReset
        {
          IsUserId = userid,
          LinkId = linkid,
          Used = 1
        };

        if (UserManagementData.UpdateIsUserPassResetData(userpass))
        {
          return true;
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error occurred occured in DeleteResetPasswordLinkDataOnceUsed().", ex);
       return false;
      }
      return false;
    }

    /// <summary>
    /// Generate the captcha string of given length
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public string GetCaptchaString(int length)
    {
      string strCaptchaString = "";

      try
      {
        int intZero = '0'; //48
        int intNine = '9'; //57
        int intA = 'A'; //65
        int intZ = 'Z'; //90
        int inta = 'a'; //97
        int intz = 'z'; //122
        int intCount = 0;
        int intRandomNumber = 0;

        while (intCount < length)
        {
          //Get the random value 
          intRandomNumber = GetRandomValue(intZero, intZ);

          //check if random value valid in range
          if (((intRandomNumber >= intZero) && (intRandomNumber <= intNine) ||
                //(intRandomNumber >= inta) && (intRandomNumber <= intz) ||
                (intRandomNumber >= intA) && (intRandomNumber <= intZ)
              ))
          {
            strCaptchaString = strCaptchaString + (char)intRandomNumber;
            intCount = intCount + 1;
          }
        }
      }
      catch (Exception ex)
      {
        Logger.Error("Error occurred occured in GetCaptchaString().", ex);
      }
      return strCaptchaString;
    }

    private Int32 GetRandomValue(Int32 minValue, Int32 maxValue)
    {
      try
      {
        RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();
        if (minValue == maxValue) return minValue;
        Int64 diff = maxValue - minValue;
        while (true)
        {
          byte[] data = new byte[1024];
          _rng.GetBytes(data);
          UInt32 rand = BitConverter.ToUInt32(data, 0);

          Int64 max = (1 + (Int64)UInt32.MaxValue);
          Int64 remainder = max % diff;
          if (rand < max - remainder)
          {
            return (Int32)(minValue + (rand % diff));
          }
        }
      }
      catch (Exception ex)
      {        
        throw ex;
      }
    }
    
    public bool Unlock(int userId)
    {

      if (AuthManager.Unlock(userId)) return true;

      return false;
    }

    private string CheckEmptystring(string objstring)
    {
      string returnValue = string.Empty;
      if (!string.IsNullOrEmpty(objstring))
      {
        returnValue = objstring;
      }

      return returnValue;
    }

    private string GetSalutation(string salutationId)
    {
      string returntype = string.Empty;

      if (!string.IsNullOrEmpty(salutationId))
      {

        returntype = ((Salutation)Convert.ToInt32(salutationId)).ToString();
      }
      if (returntype == "0") returntype = string.Empty;
      return returntype;

    }



  }
}