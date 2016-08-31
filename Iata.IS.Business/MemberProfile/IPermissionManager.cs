using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Business.MemberProfile
{
    /// <summary>
    ///  Description Goes Here
    /// </summary>

  public   interface IPermissionManager
    {

        /// <summary>
        ///  Description Goes Here
        /// </summary>
        /// <returns></returns>

      

      IList<TreeViewPermissions> GetPermissions(int templateId);


      IQueryable<Template> GetPermissionList(Permission permission);

      Permission CreateTemplatePermission(Permission permission, int loggedInUserId);

      IQueryable<Template> GetTemplateDetail(int Id);

      bool DeleteTemplate(int templateId);

      IList<Template> GetTemplateNameList();

      //IList<PermissionToUserList> GetUserPermissions();

      IQueryable<PermissionToUser> GetUserPermissionByUserId(int userId);

      IQueryable<TemplatePermission> GetTemplatePermission(int templateId);

      IList<PermissionListWithCategory> GetPermissionWithCategoryId();

      IQueryable<Template> GetTemplateNameByUserCategoryId(int userCategoryId, int memberId, int CurrentUserCategory);

      IList<PermissionToUserList> GetAssignedUserPermissionList(int userId);

      PermissionToUser SavePermissionToUser(PermissionToUser permissionToUser);

      IList<TreeViewPermissions> GetPermissionWithUserAssigned(int userId);

      DataSet GetPermissionTreeViewForTemplate(int templateId, int userCategoryId,int loggedInUserId);

      DataSet GetPermissionTreeViewForUserAssigned(int userId, int userCategoryId, int loggedInUserId);
      /// <summary>
      /// Returns the effective list of permission ids for the user (including assigned permissions and parent permissions).
      /// </summary>
      /// <param name="userId">Id of the user</param>
      /// <returns>List of permission ids</returns>
      IList<int> GetUserPermissions(int userId);

      bool AssignPermissionToSuperUser(int userId, int userCategory);

      AspNetSessions GetActiveSessions();

      bool IsUserAuthorized(int userId, int permissionId);

      bool AssignPermissionToSisOpsNormalUser(int userId, int userCategory);


        /// <summary>
        /// This function is used to get user detail based on either email id or federation id.
        /// case 1: if email id is present then federation id should be null. 
        /// case 2: if federation id is present then email id should be null.
        /// </summary>
        /// <param name="email_Id">email id</param>
        /// <param name="federationId">federation id</param>
        /// <returns>return user detail</returns>
        //CMP-665-User Related Enhancements-FRS-v1.2.doc [sec 3.2: Single Sign-On from ICP to SIS; and Conditional Redirection of Such Member Users]
        ISLoginUser IsUserLogOn(int timeFrame, string email_Id = null, string federationId = null);


        //CMP685 - User Management
        bool UpdateLogOnUser(int userId, string sessionId, int loginStatusId, bool isCloseOtherSessions = false);
    }
}
