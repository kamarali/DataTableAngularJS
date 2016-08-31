using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Data;
using iPayables.UserManagement;
using log4net;
using UnitOfWork = Iata.IS.Data.Impl.UnitOfWork;
namespace Iata.IS.Business.MemberProfile.Impl
{
  public class PermissionManager : IPermissionManager
  {
    public IRepository<Permission> PermissionRepository { get; set; }

    public IPermissionRepository PermissionRep { get; set; }

    public IRepository<TemplatePermission> TemplatePermissionRepository { get; set; }

    public IRepository<Template> Template { get; set; }

    public ITemplatePermissionRepository TemplatePermRepository { get; set; }

    public IRepository<PermissionToUser> PermissionToUserRepository { get; set; }

    public IUserManagement AuthManager { get; set; }

    private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    IList<TreeViewPermissions> _permissionList = new List<TreeViewPermissions>();


    public IList<TreeViewPermissions> GetPermissions(int templateId)
    {
      var permissionlist = PermissionRep.GetPermissions(templateId);
      return permissionlist;
    }


    public IList<TreeViewPermissions> GetPermissionWithUserAssigned(int userId)
    {
      var permissionlist = PermissionRep.GetAllPermissionsWithUserIdAssigned(userId);
      return permissionlist;
    }


    public Permission CreateTemplatePermission(Permission permission, int loggedInUserId)
    {
      Stopwatch stopWatch = new Stopwatch();
      stopWatch.Start();

        if (permission.Id == 0)
        {
            if (permission.MemeberID.HasValue)
            {
                Template objTemp =
                    Template.Single(
                        t =>
                        t.TemplateName.ToLower() == permission.TemplateName.Trim().ToLower() &&
                        t.MemberId == permission.MemeberID);
                //if (objTemp != null)
                //{

                //    return null;

                //}
            }
            else
            {
                Template objTemp =
                    Template.Single(
                        t =>
                        t.TemplateName.ToLower() == permission.TemplateName.Trim().ToLower() &&
                        t.UserCategoryId == permission.UserCategoryId);
                //if (objTemp != null)
                //{

                //    return null;

                //}
            }
        }
       

        Template objTemplate = Template.Single(t => t.Id == permission.TemplateID);



            if (objTemplate != null)
            {
                objTemplate.TemplateName = permission.TemplateName;
                objTemplate.SystemDefined = 0;
                objTemplate.UserCategoryId = permission.UserCategoryId;
                if (permission.MemeberID.HasValue)
                {
                    objTemplate.MemberId = permission.MemeberID;
                }

                Template.Update(objTemplate);


            }
            else
            {
                objTemplate = new Template
                                  {
                                      TemplateName = permission.TemplateName,
                                      SystemDefined = 0,
                                      UserCategoryId = permission.UserCategoryId
                                  };
                if (permission.MemeberID.HasValue)
                {
                    objTemplate.MemberId = permission.MemeberID;
                }


                Template.Add(objTemplate);
            }


            UnitOfWork.CommitDefault();
        

        string selectedPermissionIDs = permission.SelectedIDs.Substring(1, permission.SelectedIDs.Length - 1);
      string[] arrayPermissionId = selectedPermissionIDs.Split(',');

      var isUser = AuthManager.GetUserByUserID(loggedInUserId);

      var loggedinUserCategory = isUser.CategoryID;
      var userType = isUser.UserType;

      if (loggedinUserCategory == (int) Model.MemberProfile.Enums.UserCategory.SisOps)
      {
        if (loggedinUserCategory == permission.UserCategoryId && userType == 1)
        {
          DeleteSisOpsTemplatePermission(objTemplate.Id);
        }
        else if (loggedinUserCategory == permission.UserCategoryId && userType == 0)
        {
          DeleteTemplatePermission(objTemplate.Id, loggedInUserId);
        }
        else
        {
          DeleteSisOpsTemplatePermission(objTemplate.Id);
        }

      }
      else
      {
        DeleteTemplatePermission(objTemplate.Id, loggedInUserId);
      }

      foreach (string permissionId in arrayPermissionId)
      {
        if (!IsParentNode(Convert.ToInt32(permissionId), Convert.ToInt32(permission.TemplateID)))
        {
          var templatePermission = new TemplatePermission
                                       {
                                         TemplateId = objTemplate.Id,
                                         PermissionId = Convert.ToInt32(permissionId)
                                       };

          TemplatePermissionRepository.Add(templatePermission);
         
        }

      }
      permission.TemplateID = objTemplate.Id;
      UnitOfWork.CommitDefault();

      stopWatch.Stop();

      // Get the elapsed time as a TimeSpan value.
      TimeSpan ts = stopWatch.Elapsed;

      // Format and display the TimeSpan value.
      string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
          ts.Hours, ts.Minutes, ts.Seconds,
          ts.Milliseconds / 10);

      _logger.InfoFormat("RunTime " + elapsedTime);

      return permission;
    }


    private  bool  DeleteTemplatePermission(int templateId, int loggedinUserId)
    {
      var loggedinUserPermissionList = PermissionRep.GetAllPermissionsWithUserIdAssigned(loggedinUserId);
      var assignedPermission = loggedinUserPermissionList.Where(p => p.Is_Permission_Assigned == 1 );

      foreach (var permission in assignedPermission)
      {
        TreeViewPermissions objPermission = permission;
        var objTemplatePermission =
            TemplatePermissionRepository.Get(t => t.TemplateId == templateId && t.PermissionId == objPermission.PermissionId);

        foreach (var templatePermission in objTemplatePermission)
        {
          TemplatePermissionRepository.Delete(templatePermission);
        }
     }

      UnitOfWork.CommitDefault();

      return true;
    }

    private bool DeleteSisOpsTemplatePermission(int templateId)
    {
      var objTemplatePermission =
          TemplatePermissionRepository.Get(t => t.TemplateId == templateId);

      foreach (var templatePermission in objTemplatePermission)
      {
        TemplatePermissionRepository.Delete(templatePermission);
      }

      UnitOfWork.CommitDefault();
      return true;
    }

    public PermissionToUser SavePermissionToUser(PermissionToUser permissionToUser)
    {
      string selectedPermissionIDs = permissionToUser.SelectedIDs.Substring(1, permissionToUser.SelectedIDs.Length - 1);


      // Call SP to delete
      PermissionRep.SavePermissionsToUser(permissionToUser.UserId, selectedPermissionIDs,
                                            permissionToUser.TemplateId);

      return permissionToUser;
    }


    private bool IsParentNode(int nodeId, int templateId)
    {
      if (_permissionList.Count() == 0)
      {
        _permissionList = GetPermissions(templateId);
      }

      var parentIdList = from p in _permissionList
                         where p.ParentPermissionId == nodeId
                         select p;

      if (parentIdList.Count() > 0)
      {
        return true;
      }
      return false;
    }


    public IQueryable<Template> GetPermissionList(Permission permission)
    {
      var permissionlist = TemplatePermRepository.Get(permission);

      return permissionlist;
    }



    public IQueryable<Template> GetTemplateNameByUserCategoryId(int userCategoryId, int memberId, int CurrentUserCategory)
    {

        var templateDetail = TemplatePermRepository.GetTemplateNameByUserCategoryId(userCategoryId, memberId, CurrentUserCategory);

        return templateDetail;
    }

    public IQueryable<Template> GetTemplateDetail(int id)
    {

      var templateDetail = Template.Get(templateModel => templateModel.Id == id);

      return templateDetail;
    }

    public IList<PermissionToUserList> GetAssignedUserPermissionList(int userId)
    {
      var objPermissionToUser = PermissionRep.GetAssignedUserPermissionList(userId);
      return objPermissionToUser;
    }



    public bool DeleteTemplate(int templateId)
    {
      var isDeleted = false;

      try
      {
        var template = Template.Single(t => t.Id == templateId);
        Template.Delete(template);

        var templatepPermission = TemplatePermissionRepository.Get(t => t.TemplateId == templateId);

        foreach (var objTemplatePermission in templatepPermission)
        {
          TemplatePermissionRepository.Delete(objTemplatePermission);
        }

        UnitOfWork.CommitDefault();
        isDeleted = true;
      }
      catch (Exception exception)
      {
        _logger.Error("Error occurred in Member Creation Notification Email Handler (Send Mails for a Single Member method).", exception);

      }
      return isDeleted;

    }

    public IList<PermissionListWithCategory> GetPermissionWithCategoryId()
    {
      var userPermissionList = PermissionRep.GetPermissionWithCategoryId();

      return userPermissionList.ToList();

    }


    public IList<Template> GetTemplateNameList()
    {

      var templates = Template.GetAll();
      return templates.ToList();
    }


    public IQueryable<TemplatePermission> GetTemplatePermission(int templateId)
    {
      var objTemplatePermission =
          TemplatePermissionRepository.Get(t => t.TemplateId == templateId);
      return objTemplatePermission;

    }

    public IList<PermissionToUserList> GetUserListedPermissions(int userId)
    {
        var userPermissionlist = PermissionRep.GetUserPermissions(userId);
      return userPermissionlist;
    }


    public IQueryable<PermissionToUser> GetUserPermissionByUserId(int userId)
    {

      var userPermissionList = PermissionToUserRepository.Get(m => m.UserId == userId);

      return userPermissionList;
    }

    // this method use to get user's permission it's user id 
    private IList<PermissionToUserList> GetUserPermissionsByUId(int userId)
    {
        //var userPermissionlist = PermissionRep.GetUserPermissions();
        var userPermissionlist = PermissionRep.GetUserPermissions(userId);
        return userPermissionlist;
    }

    public IList<int> GetUserPermissions(int userId)
    {
        var userPermissionList = GetUserListedPermissions(userId);
      var assignedUserPermission = GetUserPermissionByUserId(userId);

      var permissionIdList = new List<int>();

      foreach (var objPermissionToUser in assignedUserPermission)
      {
        var user = objPermissionToUser;
        var filterList = userPermissionList.Where(m => m.PermissionId == user.PermissionId);

        foreach (var permissionItem in filterList)
        {
          if (!permissionIdList.Contains(permissionItem.ParentPermissionId))
          {
            permissionIdList.Add(permissionItem.ParentPermissionId);
          }
          if (!permissionIdList.Contains(permissionItem.PermissionId))
          {
            permissionIdList.Add(permissionItem.PermissionId);
          }
        }
      }

      // To sort Permission List in Ascending order
      var permissionCollection = from c in permissionIdList
                                 orderby c
                                 select c;

      return permissionCollection.ToList();
    }


    public DataSet GetPermissionTreeViewForTemplate(int templateId, int userCategoryId, int loggedInUserId)
    {
      var permissionList = PermissionRep.GetPermissions(templateId);
      var objPermissionStructure = GetPermissionHiearchicalStructure(permissionList, userCategoryId, loggedInUserId);
      return objPermissionStructure;
    }

    public DataSet GetPermissionTreeViewForUserAssigned(int userId, int userCategoryId,int loggedInUserId)
    {
      var permissionList = PermissionRep.GetAllPermissionsWithUserIdAssigned(userId);
      var objPermissionStructure = GetPermissionHiearchicalStructure(permissionList, userCategoryId, loggedInUserId);
      return objPermissionStructure;

    }



    private DataSet GetPermissionHiearchicalStructure(IEnumerable<TreeViewPermissions> permissionList, int userCategoryId, int loggedInUserId)
    {

      var permissionWihCategory = GetPermissionWithCategoryId();

      var isUser = AuthManager.GetUserByUserID(loggedInUserId);

      var loggedinUserCategory = isUser.CategoryID;
      var userType = isUser.UserType;
      IList<TreeViewPermissions> filteredTreeViewPermissionList=null;
      if(loggedinUserCategory == 1 )
      {
         if (loggedinUserCategory == userCategoryId && userType ==1)
         {
           filteredTreeViewPermissionList = GetSisOpsSuperUserPermissionList(permissionWihCategory, permissionList,
                                                                                 userCategoryId);   
         }
         else if (loggedinUserCategory == userCategoryId && userType ==0)
         {
           filteredTreeViewPermissionList = GetLoggedInUserPermissionList(permissionWihCategory, permissionList, loggedInUserId, userCategoryId);
         }
         else
         {
           filteredTreeViewPermissionList = GetSisOpsSuperUserPermissionList(permissionWihCategory, permissionList,userCategoryId);   
         }
        
      }
      else
      {
        filteredTreeViewPermissionList = GetLoggedInUserPermissionList(permissionWihCategory, permissionList, loggedInUserId, userCategoryId);
      }

      

      var finalPermissionList = from p in filteredTreeViewPermissionList
                                orderby p.PermissionId
                                select p;

    

      var objDs = new DataSet();
      var objDt = new DataTable();
      var objIdColumn = new DataColumn("ID", Type.GetType("System.Int32"));
      var objnAmecOlumn = new DataColumn("Name", Type.GetType("System.String"));
      var objParentId = new DataColumn("ParentID", Type.GetType("System.Int32"));
      var objLevel = new DataColumn("level", Type.GetType("System.Int32"));
      var objIsSelected = new DataColumn("IsSelected", Type.GetType("System.Boolean"));

      objDt.Columns.Add(objIdColumn);
      objDt.Columns.Add(objnAmecOlumn);
      objDt.Columns.Add(objParentId);
      objDt.Columns.Add(objLevel);
      objDt.Columns.Add(objIsSelected);

 


      foreach (TreeViewPermissions objPermission in finalPermissionList)
      {
        DataRow objDataRow = objDt.NewRow();
        objDataRow["ID"] = objPermission.PermissionId;
        objDataRow["Name"] = objPermission.PermissionName;
        objDataRow["ParentID"] = objPermission.ParentPermissionId;
        objDataRow["level"] = objPermission.PermissionLevel;
        objDataRow["IsSelected"] = objPermission.Is_Permission_Assigned;
        objDt.Rows.Add(objDataRow);

      }


      objDs.Tables.Add(objDt);
      return objDs;
    }

    public AspNetSessions GetActiveSessions()
    {
      return PermissionRep.GetActiveSessions();
    }


    public IList<TreeViewPermissions> GetSisOpsSuperUserPermissionList(IEnumerable<PermissionListWithCategory> permissionWihCategory,IEnumerable<TreeViewPermissions> permissionList,  int userCategoryId )
    {

      IList<TreeViewPermissions> filteredTreeViewPermissionList = new List<TreeViewPermissions>();

      if (permissionWihCategory != null)
      {
        var categoryInvoice = from p in permissionWihCategory
                              where p.UserCategoryId == userCategoryId
                              select p;

        foreach (var objSinglePermission in categoryInvoice)
        {
          PermissionListWithCategory permission = objSinglePermission;
          IEnumerable<PermissionListWithCategory> filterPermissionId = permissionWihCategory.Where(p => p.PermissionId == permission.PermissionId);

          foreach (var objPermissionListWithCategory in filterPermissionId)
          {
            PermissionListWithCategory category = objPermissionListWithCategory;
            var selectSinglePermission = (from p in permissionList
                                          where p.PermissionId == category.PermissionId
                                          select p).FirstOrDefault();
            var parentParmission = (from p in permissionList
                                    where p.PermissionId == category.ParentPermissionId
                                    select p).FirstOrDefault();

            if (parentParmission != null)
            {
              var checkPermissionExist = from p in filteredTreeViewPermissionList
                                         where p.PermissionId == parentParmission.PermissionId
                                         select p;
              if (checkPermissionExist.Count() == 0)
              {
                filteredTreeViewPermissionList.Add(parentParmission);
              }
            }

            if (selectSinglePermission != null)
            {
              var checkPermissionExist = from p in filteredTreeViewPermissionList
                                         where p.PermissionId == selectSinglePermission.PermissionId
                                         select p;
              if (checkPermissionExist.Count() == 0)
              {
                filteredTreeViewPermissionList.Add(selectSinglePermission);
              }
            }


          }


        }
      }

      return filteredTreeViewPermissionList;
    }


    public IList<TreeViewPermissions> GetLoggedInUserPermissionList(IEnumerable<PermissionListWithCategory> permissionWihCategory,IEnumerable<TreeViewPermissions> permissionList, int loggeduserId, int userCategoryId )
    {
      var loggedinUserPermissionList = PermissionRep.GetAllPermissionsWithUserIdAssigned(loggeduserId);

      var assignePermission = loggedinUserPermissionList.Where(p => p.Is_Permission_Assigned >= 1 && p.UserCategoryId == userCategoryId);
      var assignedPermission = from p in assignePermission
                               orderby p.PermissionId
                               select p;

      IList<TreeViewPermissions> loggedInUserPermissionList = new List<TreeViewPermissions>();

      foreach (var objSinglePermission in assignedPermission)
      {
        TreeViewPermissions permission = objSinglePermission;
        IEnumerable<PermissionListWithCategory> filterPermissionId = permissionWihCategory.Where(p => p.PermissionId == permission.PermissionId);

        foreach (var objPermissionListWithCategory in filterPermissionId)
        {
          PermissionListWithCategory category = objPermissionListWithCategory;
          var selectSinglePermission = (from p in permissionList
                                        where p.PermissionId == category.PermissionId
                                        select p).FirstOrDefault();
          var parentParmission = (from p in permissionList
                                  where p.PermissionId == category.ParentPermissionId
                                  select p).FirstOrDefault();



          if (parentParmission != null)
          {
            var checkPermissionExist = from p in loggedInUserPermissionList
                                       where p.PermissionId == parentParmission.PermissionId
                                       select p;
            if (checkPermissionExist.Count() == 0)
            {
              loggedInUserPermissionList.Add(parentParmission);
            }
          }
          if (selectSinglePermission != null)
          {
            var checkPermissionExist = from p in loggedInUserPermissionList
                                       where p.PermissionId == selectSinglePermission.PermissionId
                                       select p;
            if (checkPermissionExist.Count() == 0)
            {
              loggedInUserPermissionList.Add(selectSinglePermission);
            }
          }
        }
      }

      return loggedInUserPermissionList;
    }

    public bool AssignPermissionToSuperUser(int userId, int userCategory)
    {
      // Get Template details by UserCategoryId
      var template = Template.Get(m => m.UserCategoryId == userCategory && m.UserType == 1);

      foreach (var objtempate in template)
      {
        // Get defined Permission list  in Tempate Table
        Template objtempate1 = objtempate;
        var templatePermission = TemplatePermissionRepository.Get(m => m.TemplateId == objtempate1.Id);

        foreach (TemplatePermission permission in templatePermission)
        {
          var objpermissionToUser = new PermissionToUser
          {
            PermissionId = permission.PermissionId,
            UserId = userId
          };
          PermissionToUserRepository.Add(objpermissionToUser);

        }

      }
      UnitOfWork.CommitDefault();
      return true;
    }


    public bool AssignPermissionToSisOpsNormalUser(int userId, int userCategory)
    {
      // Get Template details by UserCategoryId
      var template = Template.Get(m => m.UserCategoryId == userCategory && m.UserType == 0);

      foreach (var objtempate in template)
      {
        // Get defined Permission list  in Tempate Table
        Template objtempate1 = objtempate;
        var templatePermission = TemplatePermissionRepository.Get(m => m.TemplateId == objtempate1.Id);

        foreach (TemplatePermission permission in templatePermission)
        {
          var objpermissionToUser = new PermissionToUser
          {
            PermissionId = permission.PermissionId,
            UserId = userId
          };
          PermissionToUserRepository.Add(objpermissionToUser);

        }

      }
      UnitOfWork.CommitDefault();
      return true;
    }


    public bool IsUserAuthorized(int userId, int permissionId)
    {
      return PermissionRep.IsUserAuthorized(userId, permissionId);
    }


    /// <summary>
    /// This function is used to get user detail based on either email id or federation id.
    /// case 1: if email id is present then federation id should be null. 
    /// case 2: if federation id is present then email id should be null.
    /// </summary>
    /// <param name="email_Id">email id</param>
    /// <param name="federationId">federation id</param>
    /// <returns>return user detail</returns>
    //CMP-665-User Related Enhancements-FRS-v1.2.doc [sec 3.2: Single Sign-On from ICP to SIS; and Conditional Redirection of Such Member Users]
    public ISLoginUser IsUserLogOn(int timeFrame, string email_Id = null, string federationId = null)
    {
      return PermissionRep.IsUserLogOn(email_Id, federationId, timeFrame);
    }


      //CMP685 - User Management
      public bool UpdateLogOnUser(int userId, string sessionId, int loginStatusId, bool isCloseOtherSessions = false)
      {
        //SCPID : 221797 - Password change policy : System does not prompt the user to change their password
        // Removed Password expiry duration parameter.
        return PermissionRep.UpdateLogOnUser(userId, sessionId, loginStatusId, isCloseOtherSessions);
      }

  }


}
