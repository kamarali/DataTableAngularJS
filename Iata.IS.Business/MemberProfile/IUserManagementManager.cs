using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.MemberProfile;
using iPayables;

namespace Iata.IS.Business.MemberProfile
{
   public  interface IUserManagementManager
   {     
     //CMP685 - User Management
     bool SendUserWelcomeNotification(string firstName, string lastName, string emailId, string urlLink);
     bool ResetUserPassword(int userId, string logOnUrl, ref string carrier);
     bool ValidateResetPasswordLink(string input,int timeframe, ref int userId, ref Guid linkid);
     bool DeleteResetPasswordLinkDataOnceUsed(int userid, Guid linkid);
     string GetCaptchaString(int length);
     bool Unlock(int userId);     
     bool SendUserResetPasswordUrlLink(string firstName, string lastName, string emailId, string urlLink, string firstLineContent);

   }
}
