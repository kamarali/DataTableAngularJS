using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Web.UIModel.Grid.Profile;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.ErrorDetail;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using Iata.IS.Web.Util.Filters;
using iPayables.UserManagement;
using System.Linq;
using UserCategory = Iata.IS.Model.MemberProfile.Enums.UserCategory;

namespace Iata.IS.Web.Areas.Profile.Controllers
{
  public class PermissionController : ISController
  {

    private readonly IPermissionManager _permissionManager;
    public IUserManagement AuthManager { get; set; }
    private const string SearchResultGridAction = "SearchResultGridData";
    public static string PermissionHelpUrl = "~/help/Editing_User_Information.htm";
  
    public IRepository<Template> Template { get; set; }

    public PermissionController(IPermissionManager permissionManager)
    {
      _permissionManager = permissionManager;
    }

    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Profile.ManageUserPermissionAccess)]
    public ActionResult NewPermissionTemplate(int? Id)
    {
      SetViewDataPageMode(Id != null ? PageMode.Edit : PageMode.View);


      if (Id != null && Id != 0)
      {
        var template = _permissionManager.GetTemplateDetail((int)Id);

        var objPermission = new Permission
                                {
                                  TemplateID = template.SingleOrDefault().Id,
                                  TemplateName = template.SingleOrDefault().TemplateName,
                                  UserCategoryId = template.SingleOrDefault().UserCategoryId
                                };
        ViewData["UserCategory"] = (int)SessionUtil.UserCategory;
        if (TempData["Message"] != null)
        {
             ShowErrorMessage(string.Format(Messages.TemplateDuplicateError,TempData["Message"]) );
        }

        return View(objPermission);
      }
      else
      {
          ViewData["UserCategory"] = (int)SessionUtil.UserCategory;
      }

      return View();

    }

    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Profile.ManageUserPermissionAccess)]
    public ActionResult EditPermissionTemplate(int? Id)
    {
        SetViewDataPageMode(Id != null ? PageMode.Edit : PageMode.View);


        if (Id != null && Id != 0)
        {
            var template = _permissionManager.GetTemplateDetail((int)Id);

            var objPermission = new Permission
            {
                TemplateID = template.SingleOrDefault().Id,
                TemplateName = template.SingleOrDefault().TemplateName,
                UserCategoryId = template.SingleOrDefault().UserCategoryId
            };
            ViewData["UserCategory"] = (int)SessionUtil.UserCategory;
            if (TempData["Message"] != null)
            {
                ShowErrorMessage(string.Format(Messages.TemplateDuplicateError, TempData["Message"]));
            }

            return View("NewPermissionTemplate", objPermission);
        }
        else
        {
            ViewData["UserCategory"] = (int)SessionUtil.UserCategory;
        }

        return View("NewPermissionTemplate");

    }

    [HttpPost]
    [ISAuthorize(Business.Security.Permissions.Profile.ManageUserPermissionAccess)]
    [ValidateAntiForgeryToken]
    public ActionResult NewPermissionTemplate(Permission permission)
    {
      
      try
      {
        var isEditMode = false;
  
        // Remove white space from Template field
        permission.TemplateName =  permission.TemplateName.ToString().Trim();

        if (permission.SelectedIDs != null)
        {
          int? lcMemberId = null;
          if (SessionUtil.MemberId > 0)
          {
            lcMemberId = SessionUtil.MemberId;
          }

          if (lcMemberId != null) permission.MemeberID = (int)lcMemberId;

          //if (SessionUtil.UserCategory != UserCategory.SisOps)
          //{
            permission.UserCategoryId = (int)SessionUtil.UserCategory;
         // }
            if (permission.TemplateID != 0)
            {
                Template objTemp =
                    Template.Single(
                        t =>
                        t.TemplateName.ToLower() == permission.TemplateName.Trim().ToLower() && t.Id == permission.TemplateID);
                if (objTemp == null)
                {
                    Template objTempnew;
                    if(permission.MemeberID.HasValue)
                    {
                        objTempnew =
                       Template.Single(t => t.TemplateName.ToLower() == permission.TemplateName.Trim().ToLower() && t.MemberId == permission.MemeberID);
                    }
                    else
                    {
                         objTempnew =
                       Template.Single(t => t.TemplateName.ToLower() == permission.TemplateName.Trim().ToLower() && t.UserCategoryId == permission.UserCategoryId);
                    }
                   
                    if (objTempnew != null)
                    {
                       
                        TempData["TemplateID"] = permission.TemplateID.ToString();
                        TempData["Message"] = permission.TemplateName;
                        return RedirectToAction("NewPermissionTemplate", new
                                                                             {
                                                                                 Id = permission.TemplateID
                                                                             });
                    }
                }
                else
                {
                  isEditMode = true;
                }
            }
            string templateName = permission.TemplateName;
            permission = _permissionManager.CreateTemplatePermission(permission, Convert.ToInt32(SessionUtil.UserId));
          if (permission == null)
          {
              TempData["Message"] = templateName;
              throw new ISBusinessException(Messages.TemplateDuplicateError);

          }
         


              ShowSuccessMessage("Template Details Saved successfully");
              TempData["TemplateID"] = permission.TemplateID.ToString();

              if (!isEditMode)
              {
                return RedirectToAction("NewPermissionTemplate", new
                {
                  Id = permission.TemplateID
                });
              }
              else
              {
                return RedirectToAction("EditPermissionTemplate", new
                {
                  Id = permission.TemplateID
                });
              }

        }
      }
    
      catch (ISBusinessException exception)
      {

          if (exception.ErrorCode == Messages.TemplateDuplicateError)
          {
              ShowErrorMessage(string.Format(Messages.TemplateDuplicateError, TempData["Message"]));
          }
          else
          {

              ShowErrorMessage(exception.ErrorCode);
          }

      }
      ViewData["UserCategory"] = (int)SessionUtil.UserCategory;
      return View(permission);

    }

    [AcceptVerbs(HttpVerbs.Post)]
    public JsonResult GetPermissionListForTreeView(string userCategory)
    {
      var templateId = (string)TempData["TemplateID"] ?? "0";
      int userCategoryId = userCategory == string.Empty ? 0 : Convert.ToInt32(userCategory);
      if (userCategoryId > 0)
      {

        var dsNodes = _permissionManager.GetPermissionTreeViewForTemplate(Convert.ToInt32(templateId), userCategoryId, Convert.ToInt32(SessionUtil.UserId));
        var tableNodes = dsNodes.Tables[0];
        var parentDr = tableNodes.Rows[0];

        var rootNode = new PermissionTemplateTreeview
                           {
                             attributes = new TreeviewAttribute { id = Convert.ToString(parentDr["ID"]) },
                             data = Convert.ToString(parentDr["Name"])
                           };

        PopulateTree(parentDr, rootNode, tableNodes);

        return Json(rootNode);

      }
      return null;
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public JsonResult GetPermissionListToUser(string userCategory, string userId)
    {

      var dsNodes = _permissionManager.GetPermissionTreeViewForUserAssigned(Convert.ToInt32(userId), Convert.ToInt32(userCategory),SessionUtil.UserId);
      var tableNodes = dsNodes.Tables[0];
      var parentDr = tableNodes.Rows[0];

      var rootNode = new PermissionTemplateTreeview
      {
        attributes = new TreeviewAttribute { id = Convert.ToString(parentDr["ID"]) },
        data = Convert.ToString(parentDr["Name"])
      };

      PopulateTree(parentDr, rootNode, tableNodes);

      return Json(rootNode);



    }

    public void PopulateTree(DataRow dataRow, PermissionTemplateTreeview jsTNode, DataTable tableNodes)
    {
      jsTNode.children = new List<PermissionTemplateTreeview>();
      foreach (DataRow dr in tableNodes.Rows)
      {

        if (dr != null)
        {
          if (Convert.ToInt32(dr["level"]) == Convert.ToInt32(dataRow["level"]) + 1
                      && Convert.ToInt32(dr["ParentID"]) == Convert.ToInt32(dataRow["ID"])
                      )
          {

            var cnode = new PermissionTemplateTreeview
                            {
                              attributes = new TreeviewAttribute
                                               {
                                                 id =
                                                     Convert.ToString(dr["ID"]),
                                                 selected =
                                                     Convert.ToBoolean(
                                                         dr["IsSelected"])
                                               },
                              data = Convert.ToString(dr["Name"])
                            };
            jsTNode.children.Add(cnode);
            PopulateTree(dr, cnode, tableNodes);

          }
        }
      }
    }

    [ISAuthorize(Business.Security.Permissions.Profile.ManageUserPermissionAccess)]
    public ActionResult ManagePermissionTemplate(Permission permission)
    {
      try
      {
        string criteria = permission != null ? new JavaScriptSerializer().Serialize(permission) : string.Empty;

        var permissionSearchGrid = new PermissionSearch(ControlIdConstants.SearchGrid, Url.Action(SearchResultGridAction, new
        {
          area = "Profile",
          criteria
        }));

        ViewData[ViewDataConstants.SearchGrid] = permissionSearchGrid.Instance;

      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);
      }


      return View(permission);
    }

    public JsonResult SearchResultGridData(string criteria)
    {

      var permission = new Permission();
      if (!string.IsNullOrEmpty(criteria))
      {
        permission = new JavaScriptSerializer().Deserialize(criteria, typeof(Permission)) as Permission;
      }


      var permissionSearchGrid = new PermissionSearch(ControlIdConstants.SearchGrid, Url.Action(SearchResultGridAction, new
      {
        area = "Profile",
        permission
      }));

      permission.MemeberID = SessionUtil.MemberId;

     
      permission.UserCategoryId = (int)SessionUtil.UserCategory;
    

      var permissionTemplateList = _permissionManager.GetPermissionList(permission).AsQueryable();

      return permissionSearchGrid.DataBind(permissionTemplateList);

    }

    [HttpGet]
    [ISAuthorize(Business.Security.Permissions.Profile.ManageUserPermissionAccess)] 
    public ActionResult EditTemplate(string id)
    {
      TempData["TemplateID"] = id;
      TempData[ViewDataConstants.PageMode] = PageMode.Edit;

      return RedirectToAction("EditPermissionTemplate", new
      {
        Id = id

      });
    }

    [HttpPost]
    public JsonResult DeleteTemplate(string id)
    {
      UIExceptionDetail details;
      try
      {

        var isDeleted = _permissionManager.DeleteTemplate(int.Parse(id));
        details = isDeleted ?
                               new UIExceptionDetail
                               {
                                 IsFailed = false,
                                 Message = "Template Deleted Successfully"
                               } :
                                   new UIExceptionDetail
                                   {
                                     IsFailed = true,
                                     Message = "Failed To Delete Template "
                                   };
      }
      catch (ISBusinessException)
      {
        details = new UIExceptionDetail
        {
          IsFailed = true,
          Message = "Failed To Delete Template "
        };
      }
      return Json(details);
    }

    [ISAuthorize(Business.Security.Permissions.Profile.ManageUserPermissionAccess)]
    public ActionResult PermissionToUser()
    {
      return View();
    }

    [HttpPost]
    [ISAuthorize(Business.Security.Permissions.Profile.ManageUserPermissionAccess)]
    [ValidateAntiForgeryToken]
    public ActionResult PermissionToUser(PermissionToUser form)
    {

      try
      {

        if(form.UserId == 0 || form.UserName == null)
        {
          ShowErrorMessage("Unable to process assign permission due to invalid user name");
          return View(form);
        }

        if (form.SelectedIDs != null)
        {

          form = _permissionManager.SavePermissionToUser(form);
          ShowSuccessMessage("Assigned Permission to User: " + form.UserName + " successfully");
        }
      }
      catch (ISBusinessException exception)
      {
        ShowErrorMessage(exception.ErrorCode);

      }


      return View(form);
    }

    [ISAuthorize(Business.Security.Permissions.Profile.ManageUserPermissionAccess)]
    public ActionResult AddToPermissionList(int templateId)
    {
      PermissionToUser form;
      form = new PermissionToUser();
      return View(form);

    }

    [HttpPost]
    public JsonResult GetTemplatePermissionList(string permissionSelectedIds, string templateId)
    {
      try
      {
        var permissionList = _permissionManager.GetPermissions(Convert.ToInt32(templateId));
        return Json(permissionList);
      }
      catch (Exception)
      {
        var details = new UIExceptionDetail()
        {
          IsFailed = true,
          Message = "Permission list for given template is not available "
        };

        return Json(details);
      }
    }

    [HttpPost]
    public JsonResult GetUserCategoryIdByUserName(string userId)
    {
      try
      {

        var userByUserMailId = AuthManager.GetUserByUserID(Convert.ToInt32(userId));

        var objPermissionListWithCategory = new PermissionListWithCategory { UserCategoryId = userByUserMailId.CategoryID, UserName = userByUserMailId.Email };

        SessionUtil.UserCategoryOfSelectedUser = userByUserMailId.CategoryID;
        SessionUtil.SelectedUserId = userByUserMailId.UserID;

        return Json(objPermissionListWithCategory);
      }
      catch (Exception)
      {
        var details = new UIExceptionDetail()
        {
          IsFailed = true,
          Message = "Error while processing UserName"
        };

        return Json(details);
      }
    }

    [HttpPost]
    public JsonResult GetTemplateNameByUserCategoryId(string userCategoryId)
    {
      try
      {
          var memberId = 0;
        if(SessionUtil.MemberId != null)
        {
            memberId = SessionUtil.MemberId;
        }
          var CurrentUserCategory = (int)SessionUtil.UserCategory;
          var objTemplateList = _permissionManager.GetTemplateNameByUserCategoryId(Convert.ToInt32(userCategoryId), memberId, CurrentUserCategory);

        return Json(objTemplateList);
      }
      catch (Exception)
      {
        var details = new UIExceptionDetail()
        {
          IsFailed = true,
          Message = "Error while processing UserName"
        };

        return Json(details);
      }
    }

    [HttpPost]
    public JsonResult GetAssignedUserPermissionList(string userId)
    {
      try
      {
        var objUserPermissionList = _permissionManager.GetAssignedUserPermissionList(Convert.ToInt32(userId));

        if (objUserPermissionList.Count() == 0)
        {
          var details = new UIExceptionDetail()
          {
            IsFailed = true,
            Message = "No Permission Defined for Selected User"
          };

          return Json(details);
        }
        return Json(objUserPermissionList);

      }
      catch (Exception)
      {
        var details = new UIExceptionDetail()
        {
          IsFailed = true,
          Message = "Error while processing UserName"
        };

        return Json(details);
      }
    }

    /// <summary>
    /// This method gets list of Users and returns matching list of Users only
    /// </summary>
    /// <param name="searchText"></param>
    /// <returns></returns>
    [HttpGet]
    public ContentResult GetUserList(string q)
    {
      var userList = AuthManager.GetAllUsers().Where(
            isUser =>
      string.Format("{0}", isUser.EMAIL_ID.ToUpper())
        .Contains(q.ToUpper()));


      if (SessionUtil.UserCategory != UserCategory.SisOps)
      {

        if (SessionUtil.MemberId > 0)
        {
          userList = userList.Where(u => u.MEMBER_ID == SessionUtil.MemberId && u.IS_ACTIVE == true && u.USER_TYPE == false && u.USER_CATEGORY_ID == (int)SessionUtil.UserCategory);
        }
        else
        {
            //SCP382879: ICH and ACH user permission
            userList = userList.Where(u => u.IS_ACTIVE == true && u.USER_TYPE == false && u.USER_CATEGORY_ID == (int)SessionUtil.UserCategory);
        }
      }
      else
      {
        // TO FIX SPIRA ISSUE# 4840
        userList = SessionUtil.MemberId > 0 ? userList.Where(u => u.MEMBER_ID == SessionUtil.MemberId && u.IS_ACTIVE == true && u.IS_LOCKED == false && u.USER_CATEGORY_ID > 0) : userList.Where(u => u.IS_ACTIVE == true && u.IS_LOCKED == false && u.USER_CATEGORY_ID > 0); // && u.USER_TYPE == false
      }


      var response = new StringBuilder();

      foreach (var objUser in userList)
      {
        if (!(Object.Equals(objUser.IS_USER_ID, SessionUtil.UserId)))
        {
          response.AppendFormat("{0}|{1}\n", objUser.EMAIL_ID, objUser.IS_USER_ID);
        }
      }

      return new SanitizeResult(response.ToString());
    }


    /// <summary>
    /// This method gets list of Users and returns matching list of Users only
    /// </summary>
    /// <param name="searchText"></param>
    /// <returns></returns>
    [HttpGet]
    public ContentResult GetUserListForCopyPermission(string q)
    {

      var userList = AuthManager.GetAllUsers().Where(
            isUser =>
      string.Format("{0}", isUser.EMAIL_ID.ToUpper())
        .Contains(q.ToUpper()));


      if (SessionUtil.UserCategoryOfSelectedUser > 0)
      {
        if (SessionUtil.SelectedUserId > 0)
        {
            if (SessionUtil.MemberId > 0)
            {
              userList = userList.Where(u => u.IS_ACTIVE == true && u.MEMBER_ID == SessionUtil.MemberId && u.IS_LOCKED == false && u.USER_TYPE == false && u.USER_CATEGORY_ID == SessionUtil.UserCategoryOfSelectedUser && u.IS_USER_ID != SessionUtil.SelectedUserId);    
            }
            else
            {
              userList = userList.Where(u => u.IS_ACTIVE == true && u.IS_LOCKED == false && u.USER_TYPE == false && u.USER_CATEGORY_ID == SessionUtil.UserCategoryOfSelectedUser && u.IS_USER_ID != SessionUtil.SelectedUserId);
            }

        }
          
          
      }
      else
      {
        userList = SessionUtil.MemberId > 0 ? userList.Where(u => u.MEMBER_ID == SessionUtil.MemberId && u.IS_ACTIVE == true && u.IS_LOCKED == false && u.USER_CATEGORY_ID > 0) : userList.Where(u => u.IS_ACTIVE == true && u.IS_LOCKED == false && u.USER_CATEGORY_ID > 0);
      }


      var response = new StringBuilder();

      foreach (var objUser in userList)
      {
        if (!(Object.Equals(objUser.IS_USER_ID, SessionUtil.UserId)))
        {
          response.AppendFormat("{0}|{1}\n", objUser.EMAIL_ID, objUser.IS_USER_ID);
        }
      }

      return new SanitizeResult(response.ToString());
    }
  }
}
