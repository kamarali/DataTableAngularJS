using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile
{
    public interface IPermissionRepository : IRepository<Permission>
    {

      List<TreeViewPermissions> GetPermissions(int templateId);

      IList<PermissionToUserList> GetUserPermissions(int userId);

      IList<PermissionListWithCategory> GetPermissionWithCategoryId();
      List<PermissionToUserList> GetAssignedUserPermissionList(int userId);
      List<TreeViewPermissions> GetAllPermissionsWithUserIdAssigned(int userId);
      AspNetSessions GetActiveSessions();
      bool IsUserAuthorized(int userId, int permissionId);

        /// <summary>
        /// This function is used to get user detail based on either email id or federation id.
        /// case 1: if email id is present then federation id should be null. 
        /// case 2: if federation id is present then email id should be null.
        /// </summary>
        /// <param name="email_Id">email id</param>
        /// <param name="federationId">federation id</param>
        /// <returns>return user detail</returns>
        //CMP-665-User Related Enhancements-FRS-v1.2.doc [sec 3.2: Single Sign-On from ICP to SIS; and Conditional Redirection of Such Member Users]
        ISLoginUser IsUserLogOn(string email_Id, string federationId, int timeFrame);

        //CMP685 - User Management
        bool UpdateLogOnUser(int userId, string sessionId, int loginStatusId, bool isCloseOtherSessions = false);

        void SavePermissionsToUser(int userId, string permissionlist, int templateId);

    }
}
