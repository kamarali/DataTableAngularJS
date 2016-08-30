using System.Collections.Generic;
using System.Web.Mvc;
using Iata.IS.Business;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Web.UIModel.Grid.Profile;
using System.Linq;
using System;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Profile.Controllers
{
  public class ManageContactsController : ISController
  {
    private readonly IManageContactsManager _manageContactsManager = null;

    public ManageContactsController(IManageContactsManager manageContactsManager)
    {
      _manageContactsManager = manageContactsManager;
    }

    //
    // GET: /Profile/Default1/

     [ISAuthorize(Business.Security.Permissions.Profile.ContactAdminAccess)]
    public ActionResult Index()
    {
        const int typeId = 0, groupId = 0, subGroupId = 0;
        var ContactTypeGrid = new ContactTypeSearch("SearchContactsGrid", Url.Action("ContactTypeSearchGridData", "ManageContacts", new { typeId, groupId, subGroupId }));
        ViewData["ContactTypeGrid"] = ContactTypeGrid.Instance;
        return View();
    }
    [Authorize]
    [HttpPost]
    [ISAuthorize(Business.Security.Permissions.Profile.ContactAdminAccess)]
    [ValidateAntiForgeryToken]
    public ActionResult Index(ContactType contactType)
    {
        SessionUtil.CurrentPageSelected = 1;
        //Get countries present in database
        var ContactTypeGrid = new ContactTypeSearch("SearchContactsGrid", Url.Action("ContactTypeSearchGridData", "ManageContacts", new { contactType.TypeId, contactType.GroupId, contactType.SubGroupId }));
        ViewData["ContactTypeGrid"] = ContactTypeGrid.Instance;
        //Displaycountries on UI
        return View();
    }

    [Authorize]
    public JsonResult ContactTypeSearchGridData(int typeId,int groupId,int subGroupId )
    {
        var contactTypeGrid = new ContactTypeSearch("SearchContactsGrid", Url.Action("ContactsSearchGridData", new { typeId, groupId, subGroupId }));
        var contactTypes = _manageContactsManager.GetContactTypeList(typeId, groupId, subGroupId);
        try
        {
            return contactTypeGrid.DataBind(contactTypes.AsQueryable());
        }
        catch (ISBusinessException be)
        {
            ViewData["errorMessage"] = be.ErrorCode;
            return null;
        }
    }
    //
    // GET: /Profile/Default1/Details/5

    public ActionResult Details(int id)
    {
      return View();
    }

    //
    // GET: /Profile/Default1/Create

    public ActionResult Create()
    {
        var contactType = new ContactType();
        contactType.TypeId = (int)TypeOfContactType.Informational;
        contactType.IsActive = true;
        return View(contactType);
        //return View();
    }

    //
    // POST: /Profile/Default1/Create

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(ContactType contactType, FormCollection formCollection)
    {
      try
      {
          var value = formCollection["rbContactType"];
          contactType.Member = true;
          switch (value)
          {
              case "Member":
                  contactType.Member = true;
                  break;
              case "Pax":
                  contactType.Pax = true;
                  break;
              case "Cgo":
                  contactType.Cgo = true;
                  break;
              case "Misc":
                  contactType.Misc = true;
                  break;
              case "Uatp":
                  contactType.Uatp = true;
                  break;
              case "Ich":
                  contactType.Ich = true;
                  break;
              case "Ach":
                  contactType.Ach = true;
                  break;
              default:
                  break;
          }
        //Check if instance for manager class is not null and then add contact type to database
        if (_manageContactsManager != null) _manageContactsManager.AddContactType(contactType);
        ShowSuccessMessage("Contact type details saved successfully");
        return RedirectToAction("Index");
      }

      catch (ISBusinessException exception)
      {
        ShowErrorMessage(ErrorCodes.InvalidContactTypeName);
        return (View());
      }
    }

    //
    // GET: /Profile/Default1/Edit/5

    public ActionResult Edit(int id)
    {
      //Get details of existing contact type so that they can be edited
      ContactType contactType = _manageContactsManager.GetContactTypeDetails(id);
      return View(contactType);
    }

    //
    // POST: /Profile/Default1/Edit/5

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(int id, ContactType contactType,FormCollection formCollection)
    {
      try
      {
        var value = formCollection["rbEditContactType"];
          //contactType.Member = true;
          switch (value)
          {
              case "Member": 
                  contactType.Member = true; 
                  break;
              case "Pax": 
                  contactType.Pax = true; 
                  break;
              case "Cgo": 
                  contactType.Cgo = true; 
                  break;
              case "Misc": 
                  contactType.Misc = true; 
                  break;
              case "Uatp": 
                  contactType.Uatp = true; 
                  break;
              case "Ich": 
                  contactType.Ich = true; 
                  break;
              case "Ach": 
                  contactType.Ach = true; 
                  break;
              default:
                  break;
          }

        //Update existing contact type
        if (_manageContactsManager != null) _manageContactsManager.UpdateContactType(contactType);
        ShowSuccessMessage("Contact type details saved successfully");
        return RedirectToAction("Index");
      }
      catch (ISBusinessException exception)
      {
          ShowErrorMessage(ErrorCodes.InvalidContactTypeName);
          return (View());
      }
      catch (Exception ex)
      {

          return View(contactType);
      }
    }

    //
    // GET: /Profile/Default1/Delete/5

    public ActionResult Delete(int id)
    {
      //get data for existing contact type
      ContactType contactType = _manageContactsManager.GetContactTypeDetails(id);
      return View(contactType);
    }

    //
    // POST: /Profile/Default1/Delete/5

    [HttpPost]
    public ActionResult Delete(int id, FormCollection collection)
    {
      try
      {
        //Delete existing contact type
        _manageContactsManager.DeleteContactType(id);

        return RedirectToAction("Index");
      }
      catch
      {
        return View();
      }
    }
  }
}
