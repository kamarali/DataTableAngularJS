using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Web.Util;
using System.Text;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.Util.Filters;

namespace Iata.IS.Web.Areas.Profile.Controllers
{
    /// <summary>
    /// This controller class would work for CMP #655: IS-WEB Display per Location ID related functionalities
    /// Author : Vinod Patil 
    /// </summary>
    public class LocationAssociationController : ISController
    {
        private readonly IManageContactsManager _manageContactsManager = null;

        private readonly IMemberManager _memberManager = null;

        public LocationAssociationController(IManageContactsManager manageContactsManager, IMemberManager memberManager)
        {
            _manageContactsManager = manageContactsManager;
            _memberManager = memberManager;
        }

        /// <summary>
        ///  CMP #655: IS-WEB Display per Location ID
        ///  Secion : 2.1.3	NEW SCREENS FOR MEMBERS TO MANAGE LOCATION ASSOCIATIONS
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ISAuthorize(Business.Security.Permissions.Profile.LocationAssociationAccess)]
        public ActionResult ManageLocationAssociation()
        {
            return View();
        }

        /// <summary>
        ///  CMP #655: IS-WEB Display per Location ID
        ///  Secion : 2.1.3	NEW SCREENS FOR MEMBERS TO MANAGE LOCATION ASSOCIATIONS
        ///  Desc : Redirect to View Location Association screen with selected inputs
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        [ISAuthorize(Business.Security.Permissions.Profile.LocationAssociationAccess)]
        [ValidateAntiForgeryToken]
        public ActionResult ManageLocationAssociation(LocationAssociation locationAssociation)
        {

            return RedirectToAction("ViewLocationAssociation", new
            {
                userID = locationAssociation.userId, locationAssociation.emailAddress
            });

        }

        /// <summary>
        ///  CMP #655: IS-WEB Display per Location ID
        ///  Secion : 2.1.3	NEW SCREENS FOR MEMBERS TO MANAGE LOCATION ASSOCIATIONS
        ///  Desc : ‘View Location Association’ screen will display and  provide details of the Location Association of that Target User / Non-User Contact
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="emailAddress"></param>
        /// <returns></returns>

        [HttpGet]
        [ISAuthorize(Business.Security.Permissions.Profile.LocationAssociationAccess)]
        public ActionResult ViewLocationAssociation(int userID, string emailAddress)
        {
            var locationAssociation = new LocationAssociation { emailAddress = emailAddress, userId = userID };
            var userConactAssignedLoc = _manageContactsManager.GetUserContactAssignedLocAssociation(userID);
            locationAssociation.grantingType = GetAssociationType(_manageContactsManager.GetUserContactAssignedLocAssociation(SessionUtil.UserId));
            locationAssociation.targetType = GetAssociationType(userConactAssignedLoc);
            ViewData["ViewAssociatedLocation"] = new MultiSelectList(GetSortedLocationCode(userConactAssignedLoc).ToArray(), "LocationId", "LocationName");

            return View(locationAssociation);
        }

        /// <summary>
        /// Determine the Association Type based on database records
        /// </summary>
        /// <param name="listUserContactLocations"></param>
        /// <returns></returns>
        private int GetAssociationType(List<UserContactLocations> listUserContactLocations)
        {
            if (listUserContactLocations.Count >= 1)
            {
                if (listUserContactLocations.Count == 1)
                {
                    if (listUserContactLocations[0].LocationId == (int)Association.None)
                    {
                        return (int)Association.None;
                    }
                    return (int)Association.SpecificLocation;
                }
                return (int)Association.SpecificLocation;
            }
            return (int)Association.AllLocation;
        }

        /// <summary>
        ///  CMP #655: IS-WEB Display per Location ID
        ///  Secion : 2.1.3	NEW SCREENS FOR MEMBERS TO MANAGE LOCATION ASSOCIATIONS
        ///  Desc : Redirect to Modify Location Association screen with selected inputs
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ISAuthorize(Business.Security.Permissions.Profile.LocationAssociationAccess)]
        [ValidateAntiForgeryToken]
        public ActionResult ViewLocationAssociation(LocationAssociation locationAssociation)
        {
            return RedirectToAction("ModifyLocationAssociation", new
                                                                     {
                                                                         userID = locationAssociation.userId,
                                                                         locationAssociation.emailAddress,
                                                                         locationAssociation.targetType,
                                                                         locationAssociation.grantingType
                                                                     });
        }

        /// <summary>
        ///  CMP #655: IS-WEB Display per Location ID
        ///  Secion : 2.1.3	NEW SCREENS FOR MEMBERS TO MANAGE LOCATION ASSOCIATIONS
        ///  Desc : This method will show the existing Location Association of the Target User /Non user Contact 
        /// </summary>
        /// <param name="locationAssociation"></param>
        /// <returns></returns>
        [HttpGet]
        [ISAuthorize(Business.Security.Permissions.Profile.LocationAssociationAccess)]
        public ActionResult ModifyLocationAssociation(LocationAssociation locationAssociation)
        {

            var unAssociatedLocationsLocation = new List<UserContactLocations>();
            var listBoxAssociatedLoc = new List<UserContactLocations>();

            // Populate all locations based on Member 
            if (locationAssociation.targetType == (int)Association.AllLocation)
            {
                var objAllLocation = _memberManager.GetMemberLocationList(SessionUtil.MemberId);
                unAssociatedLocationsLocation.AddRange(objAllLocation.Select(item => new UserContactLocations
                                                                                         {
                                                                                             LocationId = item.Id,
                                                                                             LocationCode = item.LocationCode,
                                                                                             LocationName = string.Format("{0}-{1}-{2}", item.LocationCode, (item.CityName ?? string.Empty), (item.Country != null ? item.Country.Id : string.Empty))
                                                                                         }));
            }
            else
            {
                unAssociatedLocationsLocation = _manageContactsManager.GetUserContactAssignedLocAssociation(SessionUtil.UserId);

                if (unAssociatedLocationsLocation.Count == 0)
                {
                    var objAllLocation = _memberManager.GetMemberLocationList(SessionUtil.MemberId);
                    unAssociatedLocationsLocation.AddRange(from item in objAllLocation
                                                           where item.Id > 0
                                                           select new UserContactLocations
                                                                      {
                                                                          LocationId = item.Id,
                                                                          LocationCode = item.LocationCode,
                                                                          LocationName = string.Format("{0}-{1}-{2}", item.LocationCode, (item.CityName ?? string.Empty), (item.Country != null ? item.Country.Id : string.Empty))
                                                                      });
                }
                else if (unAssociatedLocationsLocation[0].LocationId == 0)
                {
                    // Raise an error, Association Type is NONE
                }

                var associatedLocations = _manageContactsManager.GetUserContactAssignedLocAssociation(locationAssociation.userId);

                listBoxAssociatedLoc = associatedLocations.ToList();

                //Should show Location IDs of that Member (Active or Inactive) with which the Target User / Non-User Contact is NOT associated
                foreach (var item in associatedLocations)
                {
                    var contains = unAssociatedLocationsLocation.SingleOrDefault(a => a.LocationId == item.LocationId);
                    if (contains == null)
                    {
                        listBoxAssociatedLoc.Remove(item);
                        if (item.LocationId > 0)
                            locationAssociation.excludedLocations += ',' + item.LocationId.ToString();
                    }

                    var itemToRemove = unAssociatedLocationsLocation.SingleOrDefault(s => s.LocationId == item.LocationId);
                    if (itemToRemove != null)
                        unAssociatedLocationsLocation.Remove(itemToRemove);
                }

            }

            ViewData["UnAssociatedLocation"] = new MultiSelectList(GetSortedLocationCode(unAssociatedLocationsLocation).ToArray(), "locationId", "LocationName");
            ViewData["AssociatedLocation"] = new MultiSelectList(GetSortedLocationCode(listBoxAssociatedLoc).ToArray(), "locationId", "LocationName");
            return View(locationAssociation);
        }

        /// <summary>
        /// CMP #655: IS-WEB Display per Location ID
        /// Desc: Should save the changes made to the Location Association of the Target User/contact
        /// </summary>
        /// <param name="locationSelectedIds"> Associated Location Ids </param>
        /// <param name="excludedLocIds"> Invisible Location Ids on screen. These should be excluded while saving the record</param>
        /// <param name="userId">Target User/Contact Id </param>
        /// <param name="associtionType"> Association Type</param>
        /// <param name="emailId">Email Id </param>
        /// <param name="memberId">Member</param>
        /// <returns>1/0</returns>
        [HttpPost]
        public JsonResult SaveLocationAssociation(string locationSelectedIds, string excludedLocIds, string userId, string associtionType, string emailId, string memberId)
        {
            try
            {
                // This method is calling from SIS Ops Users to Manage Member Location Associations and Members' Manage Location Associations screen.
                // MemberId = 0 indicate, it is calling from Members' Manage Location Associations screen
                var paramMemberId = memberId == "0" ? SessionUtil.MemberId : Convert.ToInt32(memberId);

                // Remove first "," char from string
                if (locationSelectedIds.Length > 0) locationSelectedIds = locationSelectedIds.Substring(1, locationSelectedIds.Length - 1);
                if (excludedLocIds.Length > 0) excludedLocIds = excludedLocIds.Substring(1, excludedLocIds.Length - 1);

                var result = _manageContactsManager.InsertLocationAssociation(locationSelectedIds, excludedLocIds, Convert.ToInt32(userId), associtionType, emailId, SessionUtil.UserId, paramMemberId);

                if (result)
                    return Json(new UIExceptionDetail { IsFailed = false, Message = "Location Association has been sucessfully saved" });
                else
                    return Json(new UIExceptionDetail { IsFailed = true, Message = "Error occurred while processing location association. Please try again!!" });


            }
            catch (Exception)
            {
                var details = new UIExceptionDetail
                {
                    IsFailed = true,
                    Message = "Session seems to be expired. Please log in again!!"
                };

                return Json(details);
            }
        }

        /// <summary>
        /// CMP #655: IS-WEB Display per Location ID
        /// 2.1.3 NEW SCREENS FOR MEMBERS TO MANAGE LOCATION ASSOCIATIONS
        /// Desc: For Auto Pupulate Field
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        [HttpGet]
        public ContentResult GetUserContactList(string q)
        {
            var emailList = new List<LocationAssociation>();
            if (SessionUtil.MemberId > 0)
            {
                emailList = _manageContactsManager.GetUserContactListForLocAssociation(SessionUtil.UserId, SessionUtil.MemberId);
            }
            emailList = emailList.Where(isEmail => string.Format("{0}", isEmail.emailAddress.ToUpper()).Contains(q.ToUpper())).ToList();


            emailList = (from email in emailList
                                orderby email.emailAddress ascending
                                select email).ToList();

            var response = new StringBuilder();
            foreach (var objemail in emailList)
            {
                response.AppendFormat("{0}|{1}\n", objemail.emailAddress, objemail.userId);
            }

            return new SanitizeResult(string.IsNullOrEmpty(response.ToString()) ? "NoItemFound" : response.ToString());
            
        }

        /// <summary>
        /// CMP #655: IS-WEB Display per Location ID
        /// Section :2.1.4 NEW POPUP FOR SIS OPS USERS TO MANAGE MEMBER LOCATION ASSOCIATIONS
        /// Desc: Action to populate Associated and Unassociated Location List
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetLocationListForSisops(string userId, string memberId)
        {
            var listboxLocationList = new List<UserContactLocations>();

            var objAllLocation = _memberManager.GetMemberLocationList(Convert.ToInt32(memberId));
            var unAssociatedLocationsLocation = objAllLocation.Select(item => new UserContactLocations
                                                                                  {
                                                                                      LocationId = item.Id,
                                                                                      LocationCode = item.LocationCode,
                                                                                      LocationName = string.Format("{0}-{1}-{2}", item.LocationCode, (item.CityName ?? string.Empty), (item.Country != null ? item.Country.Id : string.Empty))
                                                                                  }).ToList();

            var associatedLocations = _manageContactsManager.GetUserContactAssignedLocAssociation(Convert.ToInt32(userId));
            var associationType = GetAssociationType(associatedLocations);

            foreach (var item in associatedLocations)
            {
                var itemToRemove = unAssociatedLocationsLocation.SingleOrDefault(s => s.LocationId == item.LocationId);
                if (itemToRemove != null)
                    unAssociatedLocationsLocation.Remove(itemToRemove);
                if (item.LocationId > 0)
                    listboxLocationList.Add(new UserContactLocations { LocationId = item.LocationId, LocationCode = item.LocationCode, LocationName = item.LocationName, UserContactId = 1, AssociationType = associationType });
            }

            listboxLocationList.AddRange(from item in unAssociatedLocationsLocation
                                         where item.LocationId > 0
                                         select new UserContactLocations
                                                    {
                                                        LocationId = item.LocationId,
                                                        LocationCode = item.LocationCode,
                                                        LocationName = item.LocationName,
                                                        UserContactId = 0, 
                                                        AssociationType = associationType
                                                   });

            return Json(GetSortedLocationCode(listboxLocationList), JsonRequestBehavior.AllowGet);

        }


        /// <summary>
        /// CMP #655: IS-WEB Display per Location ID
        /// Section:2.1.5 NEW OPTION FOR MEMBER USERS TO VIEW ‘OWN LOCATION ASSOCIATION’
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetOwnLocationAssociation()
        {
            var associatedLocations = _manageContactsManager.GetOwnAssignedLocAssociation(SessionUtil.UserId);
            if (associatedLocations.Count> 0)
            {
                if (associatedLocations.Count > 0 && associatedLocations[0].LocationId != (int) Association.None)
                {
                    associatedLocations =
                        GetOwnSortedLocation(_manageContactsManager.GetOwnAssignedLocAssociation(SessionUtil.UserId));
                }
            }

            var associationType = GetAssociationType(associatedLocations);

            return associationType == (int) Association.None || associationType == (int) Association.AllLocation
                       ? Json(associationType, JsonRequestBehavior.AllowGet)
                       : Json(associatedLocations, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Determine the Association Type based on database records
        /// </summary>
        /// <param name="listUserContactLocations"></param>
        /// <returns></returns>
        private int GetAssociationType(List<UserAssignedLocation> listUserContactLocations)
        {
            if (listUserContactLocations.Count >= 1)
            {
                if (listUserContactLocations.Count == 1)
                {
                    if (listUserContactLocations[0].LocationId == (int)Association.None)
                    {
                        return (int)Association.None;
                    }
                    return (int)Association.SpecificLocation;
                }
                return (int)Association.SpecificLocation;
            }
            return (int)Association.AllLocation;
        }

        /// <summary>
        ///  Data should be sorted in ascending order of the Location ID’s numeric value
        ///    b.	Exception: 
        ///    i.	Location IDs ‘Main’ and ‘UATP’ should be shown before numeric Locations. 
        ///    ii.	Location ‘Main’ should be shown before ‘UATP’
        /// </summary>
        /// <param name="listLocationCode"></param>
        /// <returns></returns>
        public List<UserContactLocations> GetSortedLocationCode(List<UserContactLocations> listLocationCode)
        {

            if (listLocationCode.Count() > 0)
            {
                var integerMemberLocationList = listLocationCode.ToList();
                integerMemberLocationList.Clear();
                var stringMemberLocationList = listLocationCode.ToList();
                stringMemberLocationList.Clear();

                foreach (var mll in listLocationCode.Where(mll => mll.LocationId > 0))
                {
                    if (Regex.IsMatch(mll.LocationCode, @"^[0-9]+$"))
                    {
                        integerMemberLocationList.Add(mll);
                    }
                    else stringMemberLocationList.Add(mll);
                }

                if (integerMemberLocationList.Count != 0)
                    integerMemberLocationList = integerMemberLocationList.OrderBy(l => int.Parse(l.LocationCode)).ToList();

                if (stringMemberLocationList.Count != 0)
                    stringMemberLocationList = stringMemberLocationList.OrderBy(l => l.LocationCode).ToList();

                if (integerMemberLocationList.Count != 0)
                    stringMemberLocationList.AddRange(integerMemberLocationList);
                return stringMemberLocationList.ToList();
            }
            return listLocationCode;
        }

        /// <summary>
        ///  Data should be sorted in ascending order of the Location ID’s numeric value
        ///    b.	Exception: 
        ///    i.	Location IDs ‘Main’ and ‘UATP’ should be shown before numeric Locations. 
        ///    ii.	Location ‘Main’ should be shown before ‘UATP’
        /// </summary>
        /// <param name="listLocationCode"></param>
        /// <returns></returns>
        public List<UserAssignedLocation> GetOwnSortedLocation(List<UserAssignedLocation> listLocationCode)
        {

            if (listLocationCode.Count() > 0)
            {
                var integerMemberLocationList = listLocationCode.ToList();
                integerMemberLocationList.Clear();
                var stringMemberLocationList = listLocationCode.ToList();
                stringMemberLocationList.Clear();

                foreach (var mll in listLocationCode.Where(mll => mll.LocationId > 0))
                {
                    if (Regex.IsMatch(mll.LocationCode, @"^[0-9]+$"))
                    {
                        integerMemberLocationList.Add(mll);
                    }
                    else stringMemberLocationList.Add(mll);
                }

                if (integerMemberLocationList.Count != 0)
                    integerMemberLocationList = integerMemberLocationList.OrderBy(l => int.Parse(l.LocationCode)).ToList();

                if (stringMemberLocationList.Count != 0)
                    stringMemberLocationList = stringMemberLocationList.OrderBy(l => l.LocationCode).ToList();

                if (integerMemberLocationList.Count != 0)
                    stringMemberLocationList.AddRange(integerMemberLocationList);
                return stringMemberLocationList.ToList();
            }
            return listLocationCode;
        }

        #region "CMP #666: IS-WEB Display per Location ID"
        
        [HttpGet]
        [OutputCache(CacheProfile = "donotCache")]
        public JsonResult GetArchivalLocations(string memberId, int archivalType)
        {
            var listboxLocationList = new List<ArchivalLocations>();

            var objAllLocation = _memberManager.GetMemberLocationList(Convert.ToInt32(memberId));
            var unAssociatedLocationsLocation = objAllLocation.Select(item => new ArchivalLocations
            {
                LocationId = item.Id,
                LocationCode = item.LocationCode,
                LocationName = string.Format("{0}-{1}-{2}", item.LocationCode, (item.CityName ?? string.Empty), (item.Country != null ? item.Country.Id : string.Empty))
            }).ToList();

            var associatedLocations = _memberManager.GetAssignedArchivalLocations(Convert.ToInt32(memberId), archivalType);
            var associationType = _memberManager.GetAssociationType(associatedLocations);

            foreach (var item in associatedLocations)
            {
                var itemToRemove = unAssociatedLocationsLocation.SingleOrDefault(s => s.LocationId == item.LocationId);
                if (itemToRemove != null)
                    unAssociatedLocationsLocation.Remove(itemToRemove);
                if (item.LocationId > 0)
                    listboxLocationList.Add(new ArchivalLocations() { LocationId = item.LocationId, LocationCode = item.LocationCode, LocationName = item.LocationName, ArchivalLocId = 1, AssociationType = associationType });
            }

            listboxLocationList.AddRange(from item in unAssociatedLocationsLocation
                                         where item.LocationId > 0
                                         select new ArchivalLocations
                                         {
                                             LocationId = item.LocationId,
                                             LocationCode = item.LocationCode,
                                             LocationName = item.LocationName,
                                             AssociationType = associationType
                                         });

            return Json(_memberManager.GetSortedLocationCode(listboxLocationList), JsonRequestBehavior.AllowGet);

        }

        

      //  [HttpPost]
        [OutputCache(CacheProfile = "donotCache")]
        public JsonResult GetArchivalLocsInconsistency(int memberId, string archReqMiscRecInvCurrent, string archReqMiscPayInvCurrent, string archReqMiscRecInvFuture, string archReqMiscPayInvFuture, int recAssociationType, int payAssociationType)
        {
            var laRequiredforRec = 0;
            var laRequiredforPay = 0;
            if ((!string.IsNullOrEmpty(archReqMiscRecInvCurrent) && archReqMiscRecInvCurrent.ToUpper() == "TRUE") || (!string.IsNullOrEmpty(archReqMiscRecInvFuture) && archReqMiscRecInvFuture.ToUpper() == "TRUE"))
            {
                laRequiredforRec = 1;
            }

            if ((!string.IsNullOrEmpty(archReqMiscPayInvCurrent) && archReqMiscPayInvCurrent.ToUpper() == "TRUE") || (!string.IsNullOrEmpty(archReqMiscPayInvFuture) && archReqMiscPayInvFuture.ToUpper() == "TRUE"))
            {
                laRequiredforPay = 1;
            }

            var messageId = _memberManager.GetArchivalLocsInconsistency(memberId,laRequiredforRec, laRequiredforPay, recAssociationType, payAssociationType);

            return Json(messageId, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// If the ‘Location Association’ of the Member user is “None” or “Specific Location IDs” 
        /// The popups should not be editable and can be only viewed in read-only mode else no restrictions on editing data in the popups
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [OutputCache(CacheProfile = "donotCache")]
        public JsonResult GetLoggedinUserLocsAssociation()
        {
            var associatedLocations = _manageContactsManager.GetOwnAssignedLocAssociation(SessionUtil.UserId);

            var associationType = GetAssociationType(associatedLocations);

            return Json(associationType, JsonRequestBehavior.AllowGet);

        }

        #endregion

    }
}