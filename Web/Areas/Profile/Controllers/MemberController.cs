using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;
using Iata.IS.AdminSystem;
using Iata.IS.Business;
using Iata.IS.Core.Exceptions;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Common;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Reports.Enums;
using Iata.IS.Web.UIModel.Grid.Profile;
using Iata.IS.Web.Util;
using Iata.IS.Web.UIModel.ErrorDetail;
using System.Web;
using Iata.IS.Web.Util.ExtensionHelpers;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Business.Security;
using log4net;
using UserCategory = Iata.IS.Model.MemberProfile.Enums.UserCategory;

namespace Iata.IS.Web.Areas.Profile.Controllers
{
    public class MemberController : ISController
    {
        private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private int _userId;
        private UserCategory _userCategory;
        private readonly IMemberManager _memberManager;
        private IAuthorizationManager _authorizationManager;
        private ICalendarManager _calendarManager { get; set; }
        private readonly ISisMemberSubStatusManager _sisMemberSubStatusManager;

        public MemberController(IMemberManager memberManager, IAuthorizationManager authorizationManager, ICalendarManager calendarManager, ISisMemberSubStatusManager sisMemberSubStatusManager)
        {
            _memberManager = memberManager;
            _authorizationManager = authorizationManager;
            _calendarManager = calendarManager;
            _sisMemberSubStatusManager = sisMemberSubStatusManager;
            var billingPeriod = _calendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
            ViewData["CurrentBillingPeriod"] = billingPeriod;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            _userId = SessionUtil.UserId;
            _userCategory = SessionUtil.UserCategory;
            _memberManager.UserId = _userId;
        }

        /// <summary>
        /// Create  Member Profile
        /// </summary>
        [ISAuthorize(Business.Security.Permissions.Profile.CreateOrManageMemberAccess)]
        [HttpGet]
        public ActionResult Create()
        {
            SessionUtil.IsEmailToSend = false;

            //return View(new Member { UserCategory = _userCategory });
            return View(new Member());
        }

        /// <summary>
        /// Create Member Profile
        /// </summary>
        [ISAuthorize(Business.Security.Permissions.Profile.CreateOrManageMemberAccess)]
        [HttpPost]
        public ActionResult Create(Member member)
        {
            try
            {
                if (member.Id > 0 && SessionUtil.MemberId > 0 && member.Id != SessionUtil.MemberId)
                {
                    throw new ISBusinessException("Unathorized Access.");
                }

                // Set 'Last Updated By' to logged in user id for new member.
                member.LastUpdatedBy = SessionUtil.UserId;

                member = _memberManager.CreateISMember(member);

                ShowSuccessMessage("Member details saved successfully");

                return RedirectToAction("ManageMember", new { memberId = member.Id });
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
                return View(member);
            }
        }

        [ISAuthorize(Business.Security.Permissions.Profile.CreateOrManageMemberAccess)]
        [HttpGet]
        public ActionResult ManageMember(int memberId = 0)
        {
            var selectedMemberId = memberId;

            var member = new Member();

            if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
            {
                member = new Member();
            }
            else
            {
                if (_userId > 0)
                {
                    ViewData["RecentUpdateTime"] = _memberManager.GetRecentUpdateDateTimeForFutureUpdate(_userId);
                }
                else
                {
                    ViewData["RecentUpdateTime"] = "0";
                }

                if (selectedMemberId > 0)
                {
                    member = _memberManager.GetMember(selectedMemberId, true);

                    //member.UserCategory = _userCategory;
                    SessionUtil.MemberName = member.DisplayCommercialName;

                    // below view data is used to show next period when future period is blank for add/edit of any future related field
                    //ViewData["NextBillingPeriod"] = _memberManager.GetFormattedNextPeriod(selectedMemberId);
                    //Changes made for, if Late submission window is open
                    BillingPeriod billingPeriod =
                        _calendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich) + 1;
                    string formattedPeriod = string.Empty;
                    if (billingPeriod.Period != 0)
                    {
                        formattedPeriod = billingPeriod.Year + "-" + Enum.GetName(typeof (Month), billingPeriod.Month) +
                                          "-" + "0" + billingPeriod.Period;
                    }
                    ViewData["NextBillingPeriod"] = formattedPeriod;
                }

                if (TempData[ViewDataConstants.SuccessMessage] != null)
                {
                    ShowSuccessMessage("Member details saved successfully.");
                }
            }
            return View("Manage", member);
        }

        [ISAuthorize(Business.Security.Permissions.Profile.ManageMemberAccess)]
        [HttpGet]
        public ActionResult Manage(int memberId = 0)
        {
            var selectedMemberId = memberId;

            if (selectedMemberId == 0 && SessionUtil.MemberId > 0)
            {
                selectedMemberId = SessionUtil.MemberId;
            }

            if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
            {
                return RedirectToAction("Error", "Home", new { title = "Invalid Member", area = "" });
            }

            if (_userId > 0)
            {
                ViewData["RecentUpdateTime"] = _memberManager.GetRecentUpdateDateTimeForFutureUpdate(_userId);
            }
            else
            {
                ViewData["RecentUpdateTime"] = "0";
            }

            var member = new Member();

            if (selectedMemberId > 0)
            {
                member = _memberManager.GetMember(selectedMemberId, true);
                //member.UserCategory = _userCategory;

                // below view data is used to show next period when future period is blank for add/edit of any future related field
                //ViewData["NextBillingPeriod"] = _memberManager.GetFormattedNextPeriod(selectedMemberId);
                //Changes made for, if Late submission window is open
                BillingPeriod billingPeriod = _calendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich) + 1;
                string formattedPeriod = string.Empty;
                if (billingPeriod.Period != 0)
                {
                    formattedPeriod = billingPeriod.Year + "-" + Enum.GetName(typeof(Month), billingPeriod.Month) + "-" + "0" + billingPeriod.Period;
                }
                ViewData["NextBillingPeriod"] = formattedPeriod;

                //CMP #665: User related enhancement [Sec 2.11: Conditional Display of All Tabs in the IS-WEB Member Profile screen]
                //This change is applicable for ‘Member User’ and A ‘SIS Ops User’ performs proxy login as a ‘Member User’ 
                //This change is not applicable for ‘SIS Ops User’, ‘ICH Ops User’ and ‘ACH Ops User’.
                if (SessionUtil.UserCategory == UserCategory.Member)
                {
                    var isMembershipSubStatusId = member.IsMembershipSubStatusId.HasValue;
                    if (isMembershipSubStatusId)
                    {
                        ViewData["SisMemberSubStatusId"] =
                            _sisMemberSubStatusManager.GetSisMemberSubStatusDetails(member.IsMembershipSubStatusId.Value).
                                LimitedMemProfileAccess;
                    }
                }

            }

            if (TempData[ViewDataConstants.SuccessMessage] != null)
            {
                ShowSuccessMessage("Member details saved successfully.");
            }

            return View(member);
        }

        [HttpPost]
        public ActionResult Manage(Member member)
        {
            try
            {
                if (member.Id > 0 && SessionUtil.MemberId > 0 && member.Id != SessionUtil.MemberId)
                {
                    throw new ISBusinessException(ErrorCodes.FutureUpdateErrorMemberDetails);
                }

                member = _memberManager.UpdateMember(member);

                return RedirectToAction("Manage");
            }
            catch (ISBusinessException exception)
            {
                ShowErrorMessage(exception.ErrorCode);
            }

            return View(member);
        }

        /// <summary>
        /// Member Details
        /// </summary>
        [HttpGet]
        [OutputCache(CacheProfile = "donotCache")]
        public ActionResult MemberDetails(int selectedMemberId = 0)
        {
            // Retrieve selectedMemberId value from Session variable and use it across the method

            Location location;
            Member member;

            // to hide/show logo
            ViewData["hide"] = "hidden";
            ViewData["MemberPendingFutureUpdates"] = false;

            if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
            {
                member = new Member();
            }
            else
            {
                if (selectedMemberId != 0)
                {
                    member = _memberManager.GetMember(selectedMemberId, true);
                    // member.UserCategory = _userCategory;

                    location = _memberManager.GetMemberDefaultLocation(selectedMemberId, "Main");

                    if (member != null)
                    {
                        member.DefaultLocation = location;
                    }

                    // if logo is present display image tag else hide it 
                    var logo = _memberManager.GetMemberLogo(selectedMemberId);
                    if (logo.Length > 0)
                    {
                        ViewData["hide"] = "";
                    }
                }
                else
                {
                    location = new Location();
                    //member = new Member { DefaultLocation = location, UserCategory = _userCategory };
                    member = new Member {DefaultLocation = location};
                }

                // Member Status Entry and Termination date values captured
                var memberStatus = _memberManager.GetMemberStatus(selectedMemberId, "MEM");

                if (memberStatus.Count > 0)
                {
                    ViewData["memberStatus"] = memberStatus.ElementAt(memberStatus.Count - 1).MembershipStatusId;

                    // Set Entry Date
                    if (((int) ViewData["memberStatus"]) == (int) MemberStatus.Active)
                        // Review: Please use MemberStatus enum.
                    {
                        if (member != null)
                        {
                            member.EntryDate = memberStatus.ElementAt(memberStatus.Count - 1).StatusChangeDate;
                        }
                    }

                    // Set Termination Date
                    if (((int) ViewData["memberStatus"]) == (int) MemberStatus.Terminated)
                    {
                        if (member != null)
                        {
                            member.TerminationDate = memberStatus.ElementAt(memberStatus.Count - 1).StatusChangeDate;
                        }
                    }
                }
            }
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult MemberDetails(Member member)
        {
            // Retrieve selectedMemberId value from model variable and use it across the method
            var selectedMemberId = member.Id; // 

            //SCP176737: Convert Member Prefix to always have it in Upper Case in DB
            member.MemberCodeNumeric = member.MemberCodeNumeric.ToUpper();

            // Trim the spaces if any for the member legal and commercial names.
            member.DefaultLocation.MemberCommercialName = member.CommercialName = member.CommercialName.Trim();
            member.DefaultLocation.MemberLegalName = member.LegalName = member.LegalName.Trim();
            // CMP597: TFS_Bug_8930 IS WEB -Memebr legal name is not a future update field from SIS ops login.
            // Simultaneous updation of 'Member Legal Name' on 'Member Details' and 'Locations' tabs
            member.DefaultLocation.MemberLegalNameFuturePeriod = member.LegalNameFuturePeriod;
            member.DefaultLocation.MemberLegalNameFutureValue = member.LegalNameFutureValue;
            UIMessageDetail details = null;

            // Validate Inputed char
            if (!ValidateInputParameter(member.DefaultLocation, out details))
            {
                return Json(details);
            }

            try
            {

                if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
                {
                    throw new Exception();
                }

                if (selectedMemberId == 0)
                {
                    // Set 'Last Updated By' to logged in user id for new member.
                    member.LastUpdatedBy = SessionUtil.UserId;

                    //Set display value for currency dropdown
                    member.DefaultLocation.CurrencyIdFutureDisplayValue = member.DefaultLocation.CurrencyId > 0 ? _memberManager.GetCurrencyName(member.DefaultLocation.CurrencyId.Value) : string.Empty;
                    member.DefaultLocation.CurrencyIdDisplayValue = string.Empty;

                    //Set display value for IS Membership sub status dropdown
                    member.IsMembershipSubStatusIdDisplayValue = member.IsMembershipSubStatusId > 0 ? EnumMapper.GetMembershipSubStatusDisplayValue(member.IsMembershipSubStatusId) : "";
                    member.IsMembershipSubStatusIdFutureDisplayValue = string.Empty;

                    //Set display value for country dropdown
                    member.DefaultLocation.CountryIdDisplayValue = string.Empty;
                    member.DefaultLocation.CountryIdFutureDisplayValue = !string.IsNullOrEmpty(member.DefaultLocation.CountryId) ? _memberManager.GetCountryName(member.DefaultLocation.CountryId) : string.Empty;

                    _memberManager.CreateISMember(member);
                    SessionUtil.MemberName = member.MemberCodeNumeric;
                    var memberId = member.Id;

                    // Member Status History details
                    var memberStatus = _memberManager.GetMemberStatus(memberId, "MEM");
                    ViewData["StatusHistory"] = memberStatus;
                    if (memberStatus.Count > 0)
                    {
                        ViewData["memberStatus"] = memberStatus.ElementAt(memberStatus.Count - 1).MembershipStatusId;

                        if (((int)ViewData["memberStatus"]) == 1)
                        {
                            member.EntryDate = memberStatus.ElementAt(memberStatus.Count - 1).StatusChangeDate;
                        }

                        // Set Termination
                        if (((int)ViewData["memberStatus"]) == 2)
                        {
                            member.TerminationDate = memberStatus.ElementAt(memberStatus.Count - 1).StatusChangeDate;
                        }
                    }

                    TempData[ViewDataConstants.SuccessMessage] = "Member details saved successfully.";
                    /*SCP101407: FW: XML Validation Failure for 450-9B - SIS Production
                    Description: Zone and Category are made immediate update fields for first edit now. Redirection after creating new member is stopped.
                    This code change is to stop incorrect redirection to incorrect screen, because memberId (parameter) was not passed */

                    details = new UIMessageDetail
                    {
                        IsFailed = false,
                        Message = "Member details saved successfully.",
                        isRedirect = true,
                        RedirectUrl =
                        _authorizationManager.IsAuthorized(_userId, Business.Security.Permissions.Profile.CreateOrManageMemberAccess)
                          ? Url.Action("ManageMember", "Member", new { memberId = member.Id })
                                      : Url.Action("Manage", "Member", new { memberId = member.Id }),
                        OtherUrl = Url.Action("MemberLogoUpload", "Member", new { selectedMemberId = member.Id })
                    };
                }
                else
                {
                    member.Id = selectedMemberId;

                    var memberInDb = _memberManager.GetMember(selectedMemberId, true);

                    member.DefaultLocation.CurrencyIdFutureDisplayValue = (member.DefaultLocation.CurrencyId != null) && (member.DefaultLocation.CurrencyId > 0)
                                                                            ? _memberManager.GetCurrencyName(member.DefaultLocation.CurrencyId.Value)
                                                                            : string.Empty;
                    member.DefaultLocation.CurrencyIdDisplayValue = (memberInDb.DefaultLocation.CurrencyId != null) && (memberInDb.DefaultLocation.CurrencyId > 0)
                                                                      ? _memberManager.GetCurrencyName(memberInDb.DefaultLocation.CurrencyId.Value)
                                                                      : string.Empty;

                    // Set display value for IS Membership sub status dropdown
                    member.IsMembershipSubStatusIdFutureDisplayValue = (member.IsMembershipSubStatusId != null) && (member.IsMembershipSubStatusId > 0)
                                                                         ? EnumMapper.GetMembershipSubStatusDisplayValue(member.IsMembershipSubStatusId)
                                                                         : string.Empty;

                    member.IsMembershipSubStatusIdDisplayValue = (memberInDb.IsMembershipSubStatusId != null) && (memberInDb.IsMembershipSubStatusId > 0)
                                                                   ? EnumMapper.GetMembershipSubStatusDisplayValue(memberInDb.IsMembershipSubStatusId)
                                                                   : string.Empty;

                    // For disabled controls , get original values from database and assign it to current object
                    ProfileFieldsHelper.PrepareProfileModel(memberInDb, member, _userCategory);
                    ProfileFieldsHelper.PrepareProfileModel(memberInDb.DefaultLocation, member.DefaultLocation, _userCategory);

                    // Set 'Last Updated By' to logged in user id for new member.
                    member.LastUpdatedBy = SessionUtil.UserId;

                    // Reset SISOps tab related properties.
                    member.EnableEInvWgInfoContact = memberInDb.EnableEInvWgInfoContact;
                    member.EnableFirstNFinalInfoContact = memberInDb.EnableFirstNFinalInfoContact;
                    member.EnableFnfAiaInfoContact = memberInDb.EnableFnfAiaInfoContact;
                    member.EnableFnfAsgInfoContact = memberInDb.EnableFnfAsgInfoContact;
                    member.EnableIawgMemInfoContact = memberInDb.EnableIawgMemInfoContact;
                    member.EnableIchPanelInfoContact = memberInDb.EnableIchPanelInfoContact;
                    member.EnableOldIdecScInfoContact = memberInDb.EnableOldIdecScInfoContact;
                    member.EnableRawgMemInfoContact = memberInDb.EnableRawgMemInfoContact;
                    member.EnableSampMemInfoContact = memberInDb.EnableSampMemInfoContact;
                    member.EnableSampQSmartInfoContact = memberInDb.EnableSampQSmartInfoContact;
                    member.EnableSampScMemInfoContact = memberInDb.EnableSampScMemInfoContact;
                    member.EnableSisScInfoContact = memberInDb.EnableSisScInfoContact;
                    member.AllowContactDetailsDownload = memberInDb.AllowContactDetailsDownload;
                    member.IsParticipateInValueConfirmation = memberInDb.IsParticipateInValueConfirmation;
                    member.IsParticipateInValueDetermination = memberInDb.IsParticipateInValueDetermination;
                    member.LegalArchivingRequired = memberInDb.LegalArchivingRequired;
                    member.CdcCompartmentIDforInv = memberInDb.CdcCompartmentIDforInv;
                    member.PaxOldIdecMember = memberInDb.PaxOldIdecMember;
                    member.CgoOldIdecMember = memberInDb.CgoOldIdecMember;
                    member.DigitalSignApplication = memberInDb.DigitalSignApplication;
                    member.DigitalSignVerification = memberInDb.DigitalSignVerification;
                    member.UatpInvoiceHandledbyAtcan = memberInDb.UatpInvoiceHandledbyAtcan;

                    _memberManager.UpdateMember(member);

                    TempData[ViewDataConstants.SuccessMessage] = "Member details saved successfully.";
                    details = new UIMessageDetail
                    {
                        IsFailed = false,
                        Message = "Member details saved successfully.",
                        isRedirect = true,
                        RedirectUrl =
                          _authorizationManager.IsAuthorized(_userId, Business.Security.Permissions.Profile.CreateOrManageMemberAccess)
                            ? Url.Action("ManageMember", "Member", new { memberId = selectedMemberId })
                                                  : Url.Action("Manage", "Member", new { memberId = selectedMemberId }),
                        OtherUrl = Url.Action("MemberLogoUpload", "Member", new { selectedMemberId = member.Id })
                    };
                }
            }
            catch (ISBusinessException exception)
            {
                details = new UIMessageDetail
                {
                    IsFailed = true,
                    Message = string.IsNullOrEmpty(exception.ErrorCode) ? "Error" : GetDisplayMessageWithErrorCode(exception.ErrorCode),
                    ErrorCode = string.IsNullOrEmpty(exception.ErrorCode) ? string.Empty : exception.ErrorCode
                };
            }
            /* SCP205273: Error while modifiying member profile 
             * Desc: Database trigger "TRIG_MEMBER_DETAILS_BEFORE_UPD" prevent the user from updating member code numeric. 
             *      Same application error raised through trigger is handled below to show appropriate error message.
             *      Before this error was not handled and so incorrect message i.e. - session expired used to come on screen.
             */
            catch (Exception handledGenericException)
            {
                // SCP223072: Unable to change Member Code
                // Code is commented because, now this type of exception will be raised as Business exception.
                //if (handledGenericException != null && handledGenericException.Message.Contains("Member Numeric Code cannot be changed."))
                //{
                // details = new UIMessageDetail
                // {
                //                        IsFailed = true,
                //                        Message = "Member Numeric Code cannot be changed"
                // };
                //}
                //else
                if (handledGenericException != null && handledGenericException.InnerException != null &&
                           handledGenericException.InnerException.Message != null && handledGenericException.InnerException.Message.Contains("-20401"))
                {
                    details = new UIMessageDetail
                    {
                        IsFailed = true,
                        Message = Messages.ResourceManager.GetString(ErrorCodes.RecordRestrictFromISWEB)
                    };
                }
                else
                {
                    details = new UIMessageDetail
                    {
                        IsFailed = true,
                        Message = "Unexpected error occurred. please try again later or contact administrator."
                    };
                }
            }

            return Json(details);
        }



        public bool ValidateInputParameter(Location memberLocation, out UIMessageDetail messageDetails)
        {
            // Validate Special Char and word Symbol 
            var validatedInput = true;
            var fieldName = string.Empty;

            if (!ValidateAsciiChar(memberLocation.MemberLegalName) && validatedInput)
            {
                validatedInput = false;
                fieldName = "Member Legal Name";
            }

            if (!ValidateAsciiChar(memberLocation.MemberCommercialName) && validatedInput)
            {
                validatedInput = false;
                fieldName = "Member Commercial Name";
            }
            if (!ValidateAsciiChar(memberLocation.AddressLine1) && validatedInput)
            {
                validatedInput = false;
                fieldName = "Address Line1";
            }

            if (!ValidateAsciiChar(memberLocation.AddressLine2) && validatedInput)
            {
                validatedInput = false;
                fieldName = "Address Line2";
            }
            if (!ValidateAsciiChar(memberLocation.AddressLine3) && validatedInput)
            {
                validatedInput = false;
                fieldName = "Address Line3";
            }
            if (!ValidateAsciiChar(memberLocation.PostalCode) && validatedInput)
            {
                validatedInput = false;
                fieldName = "Postal Code";
            }

            if (!ValidateAsciiChar(memberLocation.CityName) && validatedInput)
            {
                validatedInput = false;
                fieldName = "City Name";
            }

            if (!ValidateAsciiChar(memberLocation.RegistrationId) && validatedInput)
            {
                validatedInput = false;
                fieldName = "Company Registration ID";
            }
            if (!ValidateAsciiChar(memberLocation.TaxVatRegistrationNumber) && validatedInput)
            {
                validatedInput = false;
                fieldName = "Tax/VAT Registration";
            }

            if (!ValidateAsciiChar(memberLocation.AdditionalTaxVatRegistrationNumber) && validatedInput)
            {
                validatedInput = false;
                fieldName = "Add. Tax/VAT Registration";
            }

            if (!ValidateAsciiChar(memberLocation.BankAccountNumber) && validatedInput)
            {
                validatedInput = false;
                fieldName = "Bank Account Number";
            }
            if (!ValidateAsciiChar(memberLocation.BankName) && validatedInput)
            {
                validatedInput = false;
                fieldName = "Bank Name";
            }
            if (!ValidateAsciiChar(memberLocation.BankAccountName) && validatedInput)
            {
                validatedInput = false;
                fieldName = "Bank Account Name";
            }

            if (!ValidateAsciiChar(memberLocation.BranchCode) && validatedInput)
            {
                validatedInput = false;
                fieldName = "Branch Code";
            }

            if (!ValidateAsciiChar(memberLocation.Iban) && validatedInput)
            {
                validatedInput = false;
                fieldName = "IBAN";
            }
            if (!ValidateAsciiChar(memberLocation.Swift) && validatedInput)
            {
                validatedInput = false;
                fieldName = "SWIFT";
            }

            if (!ValidateAsciiChar(memberLocation.LegalText) && validatedInput)
            {
                validatedInput = false;
                fieldName = "Invoice Footer";
            }

            // SCP:31634-Sub division Name is a auto populate field. It has rare chances to insert white space in DB. In-case exist record contains white space, below check should ignore
            if (!string.IsNullOrWhiteSpace(memberLocation.SubDivisionName))
            {
                if (!ValidateAsciiChar(memberLocation.SubDivisionName) && validatedInput)
                {
                    validatedInput = false;
                    fieldName = "Sub Division Name";
                }
            }

            messageDetails = new UIMessageDetail
            {
                IsFailed = true,
                Message = "'" + fieldName + "' Field value contains invalid characters.",
                ErrorCode = ""
            };
            return validatedInput;
        }

        public bool ValidateAsciiChar(string inputStr)
        {
            bool returnType = false;

            if (string.IsNullOrEmpty(inputStr))
            {
                return true;
            }

            char[] ar = inputStr.ToCharArray();
            for (int i = 0; i < ar.Length; i++)
            {
                int t = (int)ar[i];
                if (t >= 32 && t <= 126)
                { returnType = true; }

                else
                {
                    returnType = false;
                    break;
                }

            }
            return returnType;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MemberLogoUpload(string id, int selectedMemberId = 0)
        {
            string logoType = null;
            HttpPostedFileBase uploadedFile;
            FileAttachmentHelper attachmentHelper;
            bool isOversize = false;

            try
            {

                if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
                {
                    throw new Exception();
                }

            // Get numeric member code for member.This is required while generating logo file
            var memberNumericCode = _memberManager.GetMemberCode(selectedMemberId);
            
                foreach (string file in Request.Files)
                {
                    uploadedFile = Request.Files[file];

                    if (uploadedFile != null && uploadedFile.ContentLength > 0)
                    {
                        attachmentHelper = new FileAttachmentHelper { FileToSave = uploadedFile };
                        isOversize = attachmentHelper.IsOversize(50, 150);
                        if (!isOversize)
                        {
                            //SCP# 56826 : Invoice Logo
                            // Added MIME Type for JPG/JPEG, GIF and PNG
                            switch (uploadedFile.ContentType.ToLower())
                            {
                                case "image/gif":
                                    logoType = ".gif";
                                    break;

                                case "image/jpeg":
                                case "image/x-citrix-jpeg":
                                case "image/pjpeg":
                                    logoType = ".jpg";
                                    break;
                                case "image/png":
                                case "image/x-citrix-png":
                                case "image/x-png":
                                    logoType = ".png";
                                    break;

                            }
                            if (logoType != null)
                            {
                                //NII002: Malicious File Upload: Ease of Exploitation
                                if (selectedMemberId != 0)
                                {
                                    _memberManager.UploadMemberLogo(selectedMemberId, attachmentHelper.FileBinaryData);
                                }

                                _logger.DebugFormat("Content Type: {0}", uploadedFile.ContentType);


                                var targetFilePath = Path.Combine(SystemParameters.Instance.General.MemberLogoFileLocation,
                                                                  memberNumericCode + logoType);
                                var deleteTargetFilePath = "";
                                deleteTargetFilePath = Path.Combine(SystemParameters.Instance.General.MemberLogoFileLocation,
                                                                    memberNumericCode + ".gif");

                                if (System.IO.File.Exists(deleteTargetFilePath))
                                {
                                    System.IO.File.Delete(deleteTargetFilePath);
                                }
                                deleteTargetFilePath = Path.Combine(SystemParameters.Instance.General.MemberLogoFileLocation,
                                                                    memberNumericCode + ".jpg");

                                if (System.IO.File.Exists(deleteTargetFilePath))
                                {
                                    System.IO.File.Delete(deleteTargetFilePath);
                                }
                                deleteTargetFilePath = Path.Combine(SystemParameters.Instance.General.MemberLogoFileLocation,
                                                                    memberNumericCode + ".png");

                                if (System.IO.File.Exists(deleteTargetFilePath))
                                {
                                    System.IO.File.Delete(deleteTargetFilePath);
                                }

                                _logger.DebugFormat("Target file path: {0}", targetFilePath);

                                uploadedFile.SaveAs(targetFilePath);
                            }
                            else
                            {
                                ShowErrorMessage("Incorrect Member Logo file format.", true);
                            }

                        }
                    }
                    else
                    {
                        //NII002: Malicious File Upload: Ease of Exploitation
                        _logger.Info("Error during member logo upload, Due to Zero Size File");
                        ShowErrorMessage("Unable to  upload Member Logo", true);
                    }
                }
                
                if (SessionUtil.IsEmailToSend)
                {
                    SessionUtil.IsEmailToSend = false;
                    SendEmail(selectedMemberId);
                }
            }
            catch (Exception exception)
            {  //NII002: Malicious File Upload: Ease of Exploitation
                _logger.Error("Error during member logo upload.", exception);
                ShowErrorMessage("Unable to  upload Member Logo", true);
                // Review: Not good.
            }

            if (isOversize)
            {
                ShowErrorMessage(ErrorCodes.OversizeMemberLogo, true);
            }

            if (_authorizationManager.IsAuthorized(_userId, Business.Security.Permissions.Profile.CreateOrManageMemberAccess))
            {
                return RedirectToAction("ManageMember", "Member", new { memberId = selectedMemberId });
            }

            return RedirectToAction("Manage", "Member", new { memberId = selectedMemberId });
        }

        [HttpGet]
        [OutputCache(CacheProfile = "cache10Mins")]
        public ActionResult GetMemberLogo(int memberId)
        {
            var imageData = new byte[0];
            if (memberId > 0)
            {
                imageData = _memberManager.GetMemberLogo(memberId);
            }

            if (imageData.Length > 0)
            {
                return File(imageData, "image/jpg");
            }

            return File("~/Content/Images/no_member_logo.gif", "image/gif");
        }

        /// <summary>
        /// Get Passenger configurations
        /// </summary>
        [HttpGet]
        [OutputCache(CacheProfile = "donotCache")]
        public ActionResult Pax(int selectedMemberId = 0)
        {
            // Retrieve selectedMemberId value from Session variable and use it across the method

            PassengerConfiguration pax;

            ViewData["paxPendingFutureUpdates"] = false;
            ViewData["TabType"] = "pax";
            if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
            {
                pax = new PassengerConfiguration();
            }
            else
            {
                if (selectedMemberId != 0)
                {
                    pax = _memberManager.GetPassengerConfiguration(selectedMemberId, true) ??
                          new PassengerConfiguration();
                }
                else
                {
                    pax = new PassengerConfiguration();
                }

                if (selectedMemberId > 0)
                {
                    pax.MemberId = selectedMemberId;
                }
                pax.UserCategory = _userCategory;
            }
            Session["helplinkurl"] = "Managing_Passenger_Billing_Configurations";
            return View(pax);
        }

        /// <summary>
        /// Passenger Configuration
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Pax(PassengerConfiguration pax)
        {
            // Retrieve selectedMemberId value from Model variable and use it across the method
            var selectedMemberId = pax.MemberId;   // 

            UIMessageDetail details;
            PassengerConfiguration paxRecordInDB;

            try
            {
                if (pax.MemberId > 0 && SessionUtil.MemberId > 0 && pax.MemberId != SessionUtil.MemberId)
                {
                    throw new Exception();
                }

                if (SessionUtil.UserId > 0)
                {
                    // Read current passenger configuration for selected member to know values of migration fields
                    paxRecordInDB = _memberManager.GetPassengerConfiguration(selectedMemberId, true);

                    if (paxRecordInDB != null)
                    {
                        pax.NonSamplePrimeBillingIsIdecMigrationStatusIdDisplayValue =
                            paxRecordInDB.NonSamplePrimeBillingIsIdecMigrationStatusId > 0
                                ? EnumMapper.GetMigrationStatusDisplayValue(
                                    paxRecordInDB.NonSamplePrimeBillingIsIdecMigrationStatusId)
                                : string.Empty;
                        pax.NonSamplePrimeBillingIsxmlMigrationStatusIdDisplayValue =
                            paxRecordInDB.NonSamplePrimeBillingIsxmlMigrationStatusId > 0
                                ? EnumMapper.GetMigrationStatusDisplayValue(
                                    paxRecordInDB.NonSamplePrimeBillingIsxmlMigrationStatusId)
                                : string.Empty;
                        pax.SamplingProvIsIdecMigrationStatusIdDisplayValue =
                            paxRecordInDB.SamplingProvIsIdecMigrationStatusId > 0
                                ? EnumMapper.GetMigrationStatusDisplayValue(
                                    paxRecordInDB.SamplingProvIsIdecMigrationStatusId)
                                : string.Empty;
                        pax.SamplingProvIsxmlMigrationStatusIdDisplayValue =
                            paxRecordInDB.SamplingProvIsxmlMigrationStatusId > 0
                                ? EnumMapper.GetMigrationStatusDisplayValue(
                                    paxRecordInDB.SamplingProvIsxmlMigrationStatusId)
                                : string.Empty;
                        pax.NonSampleRmIsIdecMigrationStatusIdDisplayValue =
                            paxRecordInDB.NonSampleRmIsIdecMigrationStatusId > 0
                                ? EnumMapper.GetMigrationStatusDisplayValue(
                                    paxRecordInDB.NonSampleRmIsIdecMigrationStatusId)
                                : string.Empty;
                        pax.NonSampleRmIsXmlMigrationStatusIdDisplayValue =
                            paxRecordInDB.NonSampleRmIsXmlMigrationStatusId > 0
                                ? EnumMapper.GetMigrationStatusDisplayValue(
                                    paxRecordInDB.NonSampleRmIsXmlMigrationStatusId)
                                : string.Empty;
                        pax.NonSampleCmIsIdecMigrationStatusIdDisplayValue =
                            paxRecordInDB.NonSampleCmIsIdecMigrationStatusId > 0
                                ? EnumMapper.GetMigrationStatusDisplayValue(
                                    paxRecordInDB.NonSampleCmIsIdecMigrationStatusId)
                                : string.Empty;
                        pax.NonSampleCmIsXmlMigrationStatusIdDisplayValue =
                            paxRecordInDB.NonSampleCmIsXmlMigrationStatusId > 0
                                ? EnumMapper.GetMigrationStatusDisplayValue(
                                    paxRecordInDB.NonSampleCmIsXmlMigrationStatusId)
                                : string.Empty;
                        pax.NonSampleBmIsIdecMigrationStatusIdDisplayValue =
                            paxRecordInDB.NonSampleBmIsIdecMigrationStatusId > 0
                                ? EnumMapper.GetMigrationStatusDisplayValue(
                                    paxRecordInDB.NonSampleBmIsIdecMigrationStatusId)
                                : string.Empty;
                        pax.NonSampleBmIsXmlMigrationStatusIdDisplayValue =
                            paxRecordInDB.NonSampleBmIsXmlMigrationStatusId > 0
                                ? EnumMapper.GetMigrationStatusDisplayValue(
                                    paxRecordInDB.NonSampleBmIsXmlMigrationStatusId)
                                : string.Empty;
                        pax.SampleFormCIsIdecMigrationStatusIdDisplayValue =
                            paxRecordInDB.SampleFormCIsIdecMigrationStatusId > 0
                                ? EnumMapper.GetMigrationStatusDisplayValue(
                                    paxRecordInDB.SampleFormCIsIdecMigrationStatusId)
                                : string.Empty;
                        pax.SampleFormCIsxmlMigrationStatusIdDisplayValue =
                            paxRecordInDB.SampleFormCIsxmlMigrationStatusId > 0
                                ? EnumMapper.GetMigrationStatusDisplayValue(
                                    paxRecordInDB.SampleFormCIsxmlMigrationStatusId)
                                : string.Empty;
                        pax.SampleFormDeIsIdecMigrationStatusIdDisplayValue =
                            paxRecordInDB.SampleFormDeIsIdecMigrationStatusId > 0
                                ? EnumMapper.GetMigrationStatusDisplayValue(
                                    paxRecordInDB.SampleFormDeIsIdecMigrationStatusId)
                                : string.Empty;
                        pax.SampleFormDEisxmlMigrationStatusIdDisplayValue =
                            paxRecordInDB.SampleFormDEisxmlMigrationStatusId > 0
                                ? EnumMapper.GetMigrationStatusDisplayValue(
                                    paxRecordInDB.SampleFormDEisxmlMigrationStatusId)
                                : string.Empty;
                        pax.SampleFormFxfIsIdecMigrationStatusIdDisplayValue =
                            paxRecordInDB.SampleFormFxfIsIdecMigrationStatusId > 0
                                ? EnumMapper.GetMigrationStatusDisplayValue(
                                    paxRecordInDB.SampleFormFxfIsIdecMigrationStatusId)
                                : string.Empty;
                        pax.SampleFormFxfIsxmlMigratedStatusIdDisplayValue =
                            paxRecordInDB.SampleFormFxfIsxmlMigratedStatusId > 0
                                ? EnumMapper.GetMigrationStatusDisplayValue(
                                    paxRecordInDB.SampleFormFxfIsxmlMigratedStatusId)
                                : string.Empty;

                        if (string.IsNullOrEmpty(pax.RejectionOnValidationFailureIdDisplayValue))
                        {
                            pax.RejectionOnValidationFailureIdDisplayValue =
                                paxRecordInDB.RejectionOnValidationFailureId > 0
                                    ? EnumMapper.GetRejectionOnValidationFailureDisplayValue(
                                        paxRecordInDB.RejectionOnValidationFailureId)
                                    : string.Empty;
                        }

                        if (string.IsNullOrEmpty(pax.SamplingCareerTypeIdDisplayValue))
                        {
                            pax.SamplingCareerTypeIdDisplayValue = paxRecordInDB.SamplingCareerTypeId > 0
                                                                       ? EnumMapper.GetSamplingCareerTypeDisplayValue(
                                                                           paxRecordInDB.SamplingCareerTypeId)
                                                                       : string.Empty;
                        }

                        if (string.IsNullOrEmpty(pax.ListingCurrencyIdDisplayValue))
                        {
                            pax.ListingCurrencyIdDisplayValue = paxRecordInDB.ListingCurrencyId > 0
                                                                    ? _memberManager.GetCurrencyName(
                                                                        paxRecordInDB.ListingCurrencyId.Value)
                                                                    : string.Empty;
                        }
                    }

                    ViewData["TabType"] = "pax";

                    pax.NonSamplePrimeBillingIsIdecMigrationStatusIdFutureDisplayValue =
                        pax.NonSamplePrimeBillingIsIdecMigrationStatusId > 0
                            ? EnumMapper.GetMigrationStatusDisplayValue(pax.NonSamplePrimeBillingIsIdecMigrationStatusId)
                            : string.Empty;
                    pax.NonSamplePrimeBillingIsxmlMigrationStatusIdFutureDisplayValue =
                        pax.NonSamplePrimeBillingIsxmlMigrationStatusId > 0
                            ? EnumMapper.GetMigrationStatusDisplayValue(pax.NonSamplePrimeBillingIsxmlMigrationStatusId)
                            : string.Empty;
                    pax.SamplingProvIsIdecMigrationStatusIdFutureDisplayValue =
                        pax.SamplingProvIsIdecMigrationStatusId > 0
                            ? EnumMapper.GetMigrationStatusDisplayValue(pax.SamplingProvIsIdecMigrationStatusId)
                            : string.Empty;
                    pax.SamplingProvIsxmlMigrationStatusIdFutureDisplayValue = pax.SamplingProvIsxmlMigrationStatusId >
                                                                               0
                                                                                   ? EnumMapper.
                                                                                         GetMigrationStatusDisplayValue(
                                                                                             pax.
                                                                                                 SamplingProvIsxmlMigrationStatusId)
                                                                                   : string.Empty;
                    pax.NonSampleRmIsIdecMigrationStatusIdFutureDisplayValue = pax.NonSampleRmIsIdecMigrationStatusId >
                                                                               0
                                                                                   ? EnumMapper.
                                                                                         GetMigrationStatusDisplayValue(
                                                                                             pax.
                                                                                                 NonSampleRmIsIdecMigrationStatusId)
                                                                                   : string.Empty;
                    pax.NonSampleRmIsXmlMigrationStatusIdFutureDisplayValue = pax.NonSampleRmIsXmlMigrationStatusId > 0
                                                                                  ? EnumMapper.
                                                                                        GetMigrationStatusDisplayValue(
                                                                                            pax.
                                                                                                NonSampleRmIsXmlMigrationStatusId)
                                                                                  : string.Empty;
                    pax.NonSampleCmIsIdecMigrationStatusIdFutureDisplayValue = pax.NonSampleCmIsIdecMigrationStatusId >
                                                                               0
                                                                                   ? EnumMapper.
                                                                                         GetMigrationStatusDisplayValue(
                                                                                             pax.
                                                                                                 NonSampleCmIsIdecMigrationStatusId)
                                                                                   : string.Empty;
                    pax.NonSampleCmIsXmlMigrationStatusIdFutureDisplayValue = pax.NonSampleCmIsXmlMigrationStatusId > 0
                                                                                  ? EnumMapper.
                                                                                        GetMigrationStatusDisplayValue(
                                                                                            pax.
                                                                                                NonSampleCmIsXmlMigrationStatusId)
                                                                                  : string.Empty;
                    pax.NonSampleBmIsIdecMigrationStatusIdFutureDisplayValue = pax.NonSampleBmIsIdecMigrationStatusId >
                                                                               0
                                                                                   ? EnumMapper.
                                                                                         GetMigrationStatusDisplayValue(
                                                                                             pax.
                                                                                                 NonSampleBmIsIdecMigrationStatusId)
                                                                                   : string.Empty;
                    pax.NonSampleBmIsXmlMigrationStatusIdFutureDisplayValue = pax.NonSampleBmIsXmlMigrationStatusId > 0
                                                                                  ? EnumMapper.
                                                                                        GetMigrationStatusDisplayValue(
                                                                                            pax.
                                                                                                NonSampleBmIsXmlMigrationStatusId)
                                                                                  : string.Empty;
                    pax.SampleFormCIsIdecMigrationStatusIdFutureDisplayValue = pax.SampleFormCIsIdecMigrationStatusId >
                                                                               0
                                                                                   ? EnumMapper.
                                                                                         GetMigrationStatusDisplayValue(
                                                                                             pax.
                                                                                                 SampleFormCIsIdecMigrationStatusId)
                                                                                   : string.Empty;
                    pax.SampleFormCIsxmlMigrationStatusIdFutureDisplayValue = pax.SampleFormCIsxmlMigrationStatusId > 0
                                                                                  ? EnumMapper.
                                                                                        GetMigrationStatusDisplayValue(
                                                                                            pax.
                                                                                                SampleFormCIsxmlMigrationStatusId)
                                                                                  : string.Empty;
                    pax.SampleFormDeIsIdecMigrationStatusIdFutureDisplayValue =
                        pax.SampleFormDeIsIdecMigrationStatusId > 0
                            ? EnumMapper.GetMigrationStatusDisplayValue(pax.SampleFormDeIsIdecMigrationStatusId)
                            : string.Empty;
                    pax.SampleFormDEisxmlMigrationStatusIdFutureDisplayValue = pax.SampleFormDEisxmlMigrationStatusId >
                                                                               0
                                                                                   ? EnumMapper.
                                                                                         GetMigrationStatusDisplayValue(
                                                                                             pax.
                                                                                                 SampleFormDEisxmlMigrationStatusId)
                                                                                   : string.Empty;
                    pax.SampleFormFxfIsIdecMigrationStatusIdFutureDisplayValue =
                        pax.SampleFormFxfIsIdecMigrationStatusId > 0
                            ? EnumMapper.GetMigrationStatusDisplayValue(pax.SampleFormFxfIsIdecMigrationStatusId)
                            : string.Empty;
                    pax.SampleFormFxfIsxmlMigratedStatusIdFutureDisplayValue = pax.SampleFormFxfIsxmlMigratedStatusId >
                                                                               0
                                                                                   ? EnumMapper.
                                                                                         GetMigrationStatusDisplayValue(
                                                                                             pax.
                                                                                                 SampleFormFxfIsxmlMigratedStatusId)
                                                                                   : string.Empty;

                    if (string.IsNullOrEmpty(pax.RejectionOnValidationFailureIdFutureDisplayValue))
                    {
                        pax.RejectionOnValidationFailureIdFutureDisplayValue = pax.RejectionOnValidationFailureId > 0
                                                                                   ? EnumMapper.
                                                                                         GetRejectionOnValidationFailureDisplayValue
                                                                                         (pax.
                                                                                              RejectionOnValidationFailureId)
                                                                                   : string.Empty;
                    }

                    if (string.IsNullOrEmpty(pax.SamplingCareerTypeIdFutureDisplayValue))
                    {
                        pax.SamplingCareerTypeIdFutureDisplayValue = pax.SamplingCareerTypeId > 0
                                                                         ? EnumMapper.GetSamplingCareerTypeDisplayValue(
                                                                             pax.SamplingCareerTypeId)
                                                                         : string.Empty;
                    }

                    if (string.IsNullOrEmpty(pax.ListingCurrencyIdFutureDisplayValue))
                    {
                        pax.ListingCurrencyIdFutureDisplayValue = pax.ListingCurrencyId > 0
                                                                      ? _memberManager.GetCurrencyName(
                                                                          pax.ListingCurrencyId.Value)
                                                                      : string.Empty;
                    }

                    // For disabled controls, get original values from database and assign it to current object
                    ProfileFieldsHelper.PrepareProfileModel(paxRecordInDB, pax, _userCategory);

                    // if dropdown value not equal to certified then fill null in respective fields 

                    #region // insert null if not certified

                    if (pax.NonSamplePrimeBillingIsIdecMigrationStatusId < 3)
                    {
                        pax.NonSamplePrimeBillingIsIdecCertifiedOn = null;
                        pax.NonSamplePrimeBillingIsIdecMigratedDate = null;
                    }
                    if (pax.NonSamplePrimeBillingIsxmlMigrationStatusId < 3)
                    {
                        pax.NonSamplePrimeBillingIsxmlCertifiedOn = null;
                        pax.NonSamplePrimeBillingIsxmlMigratedDate = null;
                    }
                    if (pax.SamplingProvIsIdecMigrationStatusId < 3)
                    {
                        pax.SamplingProvIsIdecCerfifiedOn = null;
                        pax.SamplingProvIsIdecMigratedDate = null;
                    }
                    if (pax.SamplingProvIsxmlMigrationStatusId < 3)
                    {
                        pax.SamplingProvIsxmlCertifiedOn = null;
                        pax.SamplingProvIsxmlMigratedDate = null;
                    }
                    if (pax.NonSampleRmIsIdecMigrationStatusId < 3)
                    {
                        pax.NonSampleRmIsIdecCertifiedOn = null;
                        pax.NonSampleRmIsIdecMigratedDate = null;
                    }
                    if (pax.NonSampleRmIsXmlMigrationStatusId < 3)
                    {
                        pax.NonSampleRmIsXmlCertifiedOn = null;
                        pax.NonSampleRmIsXmlMigratedDate = null;
                    }
                    if (pax.NonSampleBmIsIdecMigrationStatusId < 3)
                    {
                        pax.NonSampleBmIsIdecCertifiedOn = null;
                        pax.NonSampleBmIsIdecMigratedDate = null;
                    }
                    if (pax.NonSampleBmIsXmlMigrationStatusId < 3)
                    {
                        pax.NonSampleBmIsXmlCertifiedOn = null;
                        pax.NonSampleBmIsXmlMigratedDate = null;
                    }
                    if (pax.NonSampleCmIsIdecMigrationStatusId < 3)
                    {
                        pax.NonSampleCmIsIdecCertifiedOn = null;
                        pax.NonSampleCmIsIdecMigratedDate = null;
                    }
                    if (pax.NonSampleCmIsXmlMigrationStatusId < 3)
                    {
                        pax.NonSampleCmIsXmlCertifiedOn = null;
                        pax.NonSampleCmIsXmlMigratedDate = null;
                    }
                    if (pax.SampleFormCIsIdecMigrationStatusId < 3)
                    {
                        pax.SampleFormCIsIdecCertifiedOn = null;
                        pax.SampleFormCIsIdecMigratedDate = null;
                    }
                    if (pax.SampleFormCIsxmlMigrationStatusId < 3)
                    {
                        pax.SampleFormCIsxmlCertifiedOn = null;
                        pax.SampleFormCIsxmlMigratedDate = null;
                    }
                    if (pax.SampleFormDeIsIdecMigrationStatusId < 3)
                    {
                        pax.SampleFormDeIsIdecCertifiedOn = null;
                        pax.SampleFormDeIsIdecMigratedDate = null;
                    }
                    if (pax.SampleFormDEisxmlMigrationStatusId < 3)
                    {
                        pax.SampleFormDeIsxmlCertifiedOn = null;
                        pax.SampleFormDeIsxmlMigratedDate = null;
                    }
                    if (pax.SampleFormFxfIsIdecMigrationStatusId < 3)
                    {
                        pax.SampleFormFxfIsIdecCertifiedOn = null;
                        pax.SampleFormFxfIsIdecMigratedDate = null;
                    }
                    if (pax.SampleFormFxfIsxmlMigratedStatusId < 3)
                    {
                        pax.SampleFormFxfIsxmlCertifiedOn = null;
                        pax.SampleFormFxfIsxmlMigratedDate = null;
                    }

                    #endregion

                    pax = _memberManager.UpdatePassengerConfiguration(selectedMemberId, pax);
                    details = new CustomUiMessageDetail
                    {
                        IsFailed = false,
                        Message = "Passenger details saved successfully.",
                        isRedirect = false,
                        Id = pax.MemberId,
                        Value =
                            Convert.ToString(pax.IsConsolidatedProvisionalBillingFileRequiredFutureValue)
                    };
                }
                //SCP121394 - SIS: HOP INVOICES NOT RECEIVED FROM OTHER AIRLINES AS FROM MARCH 2013/PERIOD 4
                else
                {
                    ClearSession();
                    details = new UIMessageDetail { IsFailed = true, Message = "Session seems to be expired. Please log in again" };
                    return Json(details);
                }
            }
            catch (ISBusinessException exception)
            {
                _logger.Error("Business exception in passenger tab.", exception);
                details = new UIMessageDetail { IsFailed = true, Message = ShowMessageText(exception.ErrorCode) };
            }
            catch (Exception handledGenericException)
            {
                details = new UIMessageDetail
                  {
                      IsFailed = true,
                      Message = "Unexpected error occurred. please try again later or contact administrator."
                  };
            }

            return Json(details);
        }

        /// <summary>
        /// Details of Cargo
        /// </summary>
        [OutputCache(CacheProfile = "donotCache")]
        public ActionResult Cgo(int selectedMemberId = 0)
        {
            // Retrieve selectedMemberId value from Session variable and use it across the method

            CargoConfiguration cargo;
            ViewData["TabType"] = "cgo";

            if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
            {
                cargo = new CargoConfiguration();
            }
            else
            {

                if (selectedMemberId != 0)
                {
                    cargo = _memberManager.GetCargoConfig(selectedMemberId, true) ?? new CargoConfiguration();
                }
                else
                {
                    cargo = new CargoConfiguration();
                }

                // GetContactAssignments("CGO", "");
                ViewData["ContactsDataRowCount"] = 0;

            }
            Session["helplinkurl"] = "Managing_Cargo";
            cargo.UserCategory = _userCategory;
            if (selectedMemberId > 0)
            {
                cargo.MemberId = selectedMemberId;
            }

            return View(cargo);
        }

        /// <summary>
        /// Save Cargo details.
        /// </summary>
        /// <param name="cargoConfig"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Cgo(CargoConfiguration cargoConfig)
        {
            // Retrieve selectedMemberId value from Model variable and use it across the method
            var selectedMemberId = cargoConfig.MemberId; //

            UIMessageDetail details;
            CargoConfiguration cargoRecordindb;
            try
            {
                if (cargoConfig.MemberId > 0 && SessionUtil.MemberId > 0 && cargoConfig.MemberId != SessionUtil.MemberId)
                {
                    throw new Exception();
                }

                // Read current passenger configuration for selected member to know values of migration fields
                cargoRecordindb = _memberManager.GetCargoConfig(selectedMemberId, true);

                if (cargoRecordindb != null)
                {
                    cargoConfig.PrimeBillingIsIdecMigrationStatusIdDisplayValue = cargoRecordindb.PrimeBillingIsIdecMigrationStatusId > 0
                                                                                    ? EnumMapper.GetMigrationStatusDisplayValue(cargoRecordindb.PrimeBillingIsIdecMigrationStatusId)
                                                                                    : string.Empty;
                    cargoConfig.PrimeBillingIsxmlMigrationStatusIdDisplayValue = cargoRecordindb.PrimeBillingIsxmlMigrationStatusId > 0
                                                                                   ? EnumMapper.GetMigrationStatusDisplayValue(cargoRecordindb.PrimeBillingIsxmlMigrationStatusId)
                                                                                   : string.Empty;
                    cargoConfig.RmIsIdecMigrationStatusIdDisplayValue = cargoRecordindb.RmIsIdecMigrationStatusId > 0
                                                                          ? EnumMapper.GetMigrationStatusDisplayValue(cargoRecordindb.RmIsIdecMigrationStatusId)
                                                                          : string.Empty;
                    cargoConfig.RmIsXmlMigrationStatusIdDisplayValue = cargoRecordindb.RmIsXmlMigrationStatusId > 0
                                                                         ? EnumMapper.GetMigrationStatusDisplayValue(cargoRecordindb.RmIsXmlMigrationStatusId)
                                                                         : string.Empty;
                    cargoConfig.CmIsIdecMigrationStatusIdDisplayValue = cargoRecordindb.CmIsIdecMigrationStatusId > 0
                                                                          ? EnumMapper.GetMigrationStatusDisplayValue(cargoRecordindb.CmIsIdecMigrationStatusId)
                                                                          : string.Empty;
                    cargoConfig.CmIsXmlMigrationStatusIdDisplayValue = cargoRecordindb.CmIsXmlMigrationStatusId > 0
                                                                         ? EnumMapper.GetMigrationStatusDisplayValue(cargoRecordindb.CmIsXmlMigrationStatusId)
                                                                         : string.Empty;
                    cargoConfig.BmIsIdecMigrationStatusIdDisplayValue = cargoRecordindb.BmIsIdecMigrationStatusId > 0
                                                                          ? EnumMapper.GetMigrationStatusDisplayValue(cargoRecordindb.BmIsIdecMigrationStatusId)
                                                                          : string.Empty;
                    cargoConfig.BmIsXmlMigrationStatusIdDisplayValue = cargoRecordindb.BmIsXmlMigrationStatusId > 0
                                                                         ? EnumMapper.GetMigrationStatusDisplayValue(cargoRecordindb.BmIsXmlMigrationStatusId)
                                                                         : string.Empty;
                    if (string.IsNullOrEmpty(cargoConfig.RejectionOnValidationFailureIdDisplayValue))
                    {
                        cargoConfig.RejectionOnValidationFailureIdDisplayValue = cargoRecordindb.RejectionOnValidationFailureId > 0
                                                                                   ? EnumMapper.GetRejectionOnValidationFailureDisplayValue(cargoConfig.RejectionOnValidationFailureId)
                                                                                   : string.Empty;
                    }
                }

                ViewData["TabType"] = "cgo";

                cargoConfig.PrimeBillingIsIdecMigrationStatusIdFutureDisplayValue = cargoConfig.PrimeBillingIsIdecMigrationStatusId > 0
                                                                                      ? EnumMapper.GetMigrationStatusDisplayValue(cargoConfig.PrimeBillingIsIdecMigrationStatusId)
                                                                                      : string.Empty;
                cargoConfig.PrimeBillingIsxmlMigrationStatusIdFutureDisplayValue = cargoConfig.PrimeBillingIsxmlMigrationStatusId > 0
                                                                                     ? EnumMapper.GetMigrationStatusDisplayValue(cargoConfig.PrimeBillingIsxmlMigrationStatusId)
                                                                                     : string.Empty;
                cargoConfig.RmIsIdecMigrationStatusIdFutureDisplayValue = cargoConfig.RmIsIdecMigrationStatusId > 0
                                                                            ? EnumMapper.GetMigrationStatusDisplayValue(cargoConfig.RmIsIdecMigrationStatusId)
                                                                            : string.Empty;
                cargoConfig.RmIsXmlMigrationStatusIdFutureDisplayValue = cargoConfig.RmIsXmlMigrationStatusId > 0
                                                                           ? EnumMapper.GetMigrationStatusDisplayValue(cargoConfig.RmIsXmlMigrationStatusId)
                                                                           : string.Empty;
                cargoConfig.CmIsIdecMigrationStatusIdFutureDisplayValue = cargoConfig.CmIsIdecMigrationStatusId > 0
                                                                            ? EnumMapper.GetMigrationStatusDisplayValue(cargoConfig.CmIsIdecMigrationStatusId)
                                                                            : string.Empty;
                cargoConfig.CmIsXmlMigrationStatusIdFutureDisplayValue = cargoConfig.CmIsXmlMigrationStatusId > 0
                                                                           ? EnumMapper.GetMigrationStatusDisplayValue(cargoConfig.CmIsXmlMigrationStatusId)
                                                                           : string.Empty;
                cargoConfig.BmIsIdecMigrationStatusIdFutureDisplayValue = cargoConfig.BmIsIdecMigrationStatusId > 0
                                                                            ? EnumMapper.GetMigrationStatusDisplayValue(cargoConfig.BmIsIdecMigrationStatusId)
                                                                            : string.Empty;
                cargoConfig.BmIsXmlMigrationStatusIdFutureDisplayValue = cargoConfig.BmIsXmlMigrationStatusId > 0
                                                                           ? EnumMapper.GetMigrationStatusDisplayValue(cargoConfig.BmIsXmlMigrationStatusId)
                                                                           : string.Empty;
                if (string.IsNullOrEmpty(cargoConfig.RejectionOnValidationFailureIdDisplayValue))
                {
                    cargoConfig.RejectionOnValidationFailureIdFutureDisplayValue = cargoConfig.RejectionOnValidationFailureId > 0
                                                                                     ? EnumMapper.GetRejectionOnValidationFailureDisplayValue(cargoConfig.RejectionOnValidationFailureId)
                                                                                     : string.Empty;
                }

                // For disabled controls , get original values from database and assign it to current object
                ProfileFieldsHelper.PrepareProfileModel(cargoRecordindb, cargoConfig, _userCategory);

                // if dropdown value not equal to certified then fill null in respective fields 
                #region // insert null if not certified
                if (cargoConfig.PrimeBillingIsIdecMigrationStatusId < 3)
                {
                    cargoConfig.PrimeBillingIsIdecCertifiedOn = null;
                    cargoConfig.PrimeBillingIsIdecMigratedDate = null;
                }

                if (cargoConfig.PrimeBillingIsxmlMigrationStatusId < 3)
                {
                    cargoConfig.PrimeBillingIsxmlCertifiedOn = null;
                    cargoConfig.PrimeBillingIsxmlMigratedDate = null;
                }

                if (cargoConfig.RmIsIdecMigrationStatusId < 3)
                {
                    cargoConfig.RmIsIdecCertifiedOn = null;
                    cargoConfig.RmIsIdecMigratedDate = null;
                }
                if (cargoConfig.RmIsXmlMigrationStatusId < 3)
                {
                    cargoConfig.RmIsXmlCertifiedOn = null;
                    cargoConfig.RmIsXmlMigratedDate = null;
                }

                if (cargoConfig.CmIsIdecMigrationStatusId < 3)
                {
                    cargoConfig.CmIsIdecCertifiedOn = null;
                    cargoConfig.CmIsIdecMigratedDate = null;
                }
                if (cargoConfig.CmIsXmlMigrationStatusId < 3)
                {
                    cargoConfig.CmIsXmlCertifiedOn = null;
                    cargoConfig.CmIsXmlMigratedDate = null;
                }

                if (cargoConfig.BmIsIdecMigrationStatusId < 3)
                {
                    cargoConfig.BmIsIdecCertifiedOn = null;
                    cargoConfig.BmIsIdecMigratedDate = null;
                }
                if (cargoConfig.BmIsXmlMigrationStatusId < 3)
                {
                    cargoConfig.BmIsXmlMigratedDate = null;
                    cargoConfig.BmIsXmlCertifiedOn = null;
                }
                #endregion

                cargoConfig = _memberManager.UpdateCargoConfiguration(selectedMemberId, cargoConfig);
                details = new UIMessageDetail { IsFailed = false, Message = "Cargo details saved successfully.", isRedirect = false };
            }
            catch (ISBusinessException exception)
            {
                _logger.Error("Error in cargo tab.", exception);
                details = new UIMessageDetail { IsFailed = true, Message = ShowMessageText(exception.ErrorCode) };
            }
            catch (Exception handledGenericException)
            {
                details = new UIMessageDetail
                  {
                      IsFailed = true,
                      Message = "Unexpected error occurred. please try again later or contact administrator."
                  };
            }

            return Json(details);
        }

        /// <summary>
        /// ICH Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [OutputCache(CacheProfile = "donotCache")]
        public ActionResult Ich(int selectedMemberId = 0)
        {
            // Retrieve selectedMemberId value from Session variable and use it across the method

            IchConfiguration ich;

            if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
            {
                ich = new IchConfiguration();
            }
            else
            {

                // Get the list of sponsored members
                IchConfiguration[] futureSponsordMembers =
                    _memberManager.GetSponsoredList(selectedMemberId, true).ToArray();

                // ViewData is used to bind the list box with the existing sponsors
                ViewData["Members"] = new MultiSelectList(futureSponsordMembers, "MemberId", "CommercialName");

                // Get the list of sponsored members
                IchConfiguration[] currentSponsordMembers =
                    _memberManager.GetSponsoredList(selectedMemberId, false).ToArray();

                // ViewData is used to bind the list box with the existing sponsors
                ViewData["currentSponsordMembers"] = new MultiSelectList(currentSponsordMembers, "MemberId",
                                                                         "CommercialName");

                // Get the list of Aggregator members set for future period
                IchConfiguration[] futureAggregatorMembers =
                    _memberManager.GetAggregatorsList(selectedMemberId, true).ToArray();

                // ViewData is used to bind the list box with the future aggregators
                ViewData["Aggregators"] = new MultiSelectList(futureAggregatorMembers, "MemberId", "CommercialName");

                // Get the list of Aggregator members set for future period
                IchConfiguration[] currentAggregatorMembers =
                    _memberManager.GetAggregatorsList(selectedMemberId, false).ToArray();

                // ViewData is used to bind the list box with the existing aggregators
                ViewData["currentAggregatorMembers"] = new MultiSelectList(currentAggregatorMembers, "MemberId",
                                                                           "CommercialName");
                ViewData["ichPendingFutureUpdates"] = false;

                // Get Ich details against memberID passed
                ich = _memberManager.GetIchConfig(selectedMemberId, true) ?? new IchConfiguration();

                // Get status dates for current status
                ich = _memberManager.GetIchMemberStatusList(ich);
                // SCP 49297: Setting the MemberId as it was passing null..
                if (selectedMemberId > 0)
                {
                    ich.MemberId = selectedMemberId;
                }

                if (currentSponsordMembers.Length > 0)
                {
                    // if the member is sponsoring other members, set value of flag to true
                    ich.ISSponsororMember = true;
                }

                if (currentAggregatorMembers.Length > 0)
                {
                    // if the member is sponsoring other members, set value of flag to true
                    ich.ISAggregatorMember = true;
                }

                // Get MemberStatus details 
                var memberStatus = _memberManager.GetMemberStatus(selectedMemberId, "ICH");

                ViewData["memberStatus"] = memberStatus.Count > 0
                                               ? memberStatus.ElementAt(memberStatus.Count - 1).MembershipStatusId
                                               : 0;

                // Used this View data to capture ICH model object 
                var ichConfig = new IchConfiguration();
                ViewData["IchConfig"] = ichConfig;

                //CMP-689-Flexible CH Activation Options
                //pass here the fieldtype of field IchMemberShipStatusId to view by using viewdata
                //if IchMemberShipStatusId having future value as  'Live' from 'Not a Member' or 'Terminated' , set the fieldtype 19 else 16
                //On the basis of its field type, functinality of field IchMemberShipStatusId will differ on View
                ViewData["IchMemberShipStatusIdFieldType"] = ich.IchMemberShipStatusIdFutureValue == 1 &&
                                                         (ich.IchMemberShipStatusId == 3 || ich.IchMemberShipStatusId == 4)
                                                           ? 19
                                                           : 16;
            }

          Session["helplinkurl"] = "Viewing_ICH_Configurations";
            ich.UserCategory = _userCategory;
            return View(ich);
        }

        /// <summary>
        /// ICH Details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Ich(IchConfiguration ichConfig)
        {
            /* SCP88742: ICH Member Profile XB-B41 - Not a Member status issue 
               Date: 02-Mar-2013
               Desc: By default Memebership status resets to 4 i.e. - NotAMember. Overriding this with user input/chnaged status.
               IchMemberShipStatusIdSelectedOnUi = 0 => No change in member status from dropdown on UI.
            */

            UIMessageDetail details;
            IchConfiguration ichRecordInDB;
            try
            {
                if (ichConfig.MemberId > 0 && SessionUtil.MemberId > 0 && ichConfig.MemberId != SessionUtil.MemberId)
                {
                    throw new Exception();
                }

                if (ichConfig.IchMemberShipStatusIdSelectedOnUi != 0)
                {
                    ichConfig.IchMemberShipStatusId = ichConfig.IchMemberShipStatusIdSelectedOnUi;
                    ichConfig.IchMemberShipStatus = ichConfig.IchMemberShipStatusSelectedOnUi;
                }

                // Retrieve selectedMemberId value from Model variable and use it across the method
                var selectedMemberId = ichConfig.MemberId; // 

                if (ichConfig.SponsororFuturePeriod == "YYYY-MMM-PP")
                {
                    ichConfig.SponsororFuturePeriod = null;
                }
                if (ichConfig.AggregatorFuturePeriod == "YYYY-MMM-PP")
                {
                    ichConfig.AggregatorFuturePeriod = null;
                }

                // Get Ich details against memberID passed
                ichRecordInDB = _memberManager.GetIchConfig(selectedMemberId, true);

                if (ichRecordInDB != null)
                {
                    if (string.IsNullOrEmpty(ichConfig.IchMemberShipStatusIdDisplayValue))
                    {
                        ichConfig.IchMemberShipStatusIdDisplayValue = ichRecordInDB.IchMemberShipStatusId > 0
                                                                          ? EnumMapper.
                                                                                GetIchMemberShipStatusDisplayValue(
                                                                                    ichRecordInDB.IchMemberShipStatusId)
                                                                          : string.Empty;
                    }

                    if (string.IsNullOrEmpty(ichConfig.IchZoneIdDisplayValue))
                    {
                        ichConfig.IchZoneIdDisplayValue = ichRecordInDB.IchZoneId > 0
                                                              ? EnumMapper.GetIchZoneDisplayValue(
                                                                  ichRecordInDB.IchZoneId)
                                                              : string.Empty;
                    }

                    if (string.IsNullOrEmpty(ichConfig.IchCategoryIdDisplayValue))
                    {
                        ichConfig.IchCategoryIdDisplayValue = ichRecordInDB.IchCategoryId > 0
                                                                  ? EnumMapper.GetIchCategoryDisplayValue(
                                                                      ichRecordInDB.IchCategoryId)
                                                                  : string.Empty;
                    }

                    if (string.IsNullOrEmpty(ichConfig.AggregatedTypeIdDisplayValue))
                    {
                        ichConfig.AggregatedTypeIdDisplayValue = (ichRecordInDB.AggregatedTypeId > 0 &&
                                                                  ichRecordInDB.AggregatedTypeId.HasValue)
                                                                     ? EnumMapper.GetAggregatedTypeDisplayValue(
                                                                         (AggregatedType)
                                                                         ichRecordInDB.AggregatedTypeId.Value)
                                                                     : string.Empty;
                    }
                }

                if (string.IsNullOrEmpty(ichConfig.IchMemberShipStatusIdFutureDisplayValue))
                {
                    ichConfig.IchMemberShipStatusIdFutureDisplayValue = ichConfig.IchMemberShipStatusId > 0
                                                                            ? EnumMapper.
                                                                                  GetIchMemberShipStatusDisplayValue(
                                                                                      ichConfig.IchMemberShipStatusId)
                                                                            : string.Empty;
                }

                if (string.IsNullOrEmpty(ichConfig.IchZoneIdFutureDisplayValue))
                {
                    ichConfig.IchZoneIdFutureDisplayValue = ichConfig.IchZoneId > 0
                                                                ? EnumMapper.GetIchZoneDisplayValue(ichConfig.IchZoneId)
                                                                : string.Empty;
                }

                if (string.IsNullOrEmpty(ichConfig.IchCategoryIdFutureDisplayValue))
                {
                    ichConfig.IchCategoryIdFutureDisplayValue = ichConfig.IchCategoryId > 0
                                                                    ? EnumMapper.GetIchCategoryDisplayValue(
                                                                        ichConfig.IchCategoryId)
                                                                    : string.Empty;
                }

                if (string.IsNullOrEmpty(ichConfig.AggregatedTypeIdFutureDisplayValue))
                {
                    ichConfig.AggregatedTypeIdFutureDisplayValue = ichConfig.AggregatedTypeId > 0
                                                                       ? EnumMapper.GetAggregatedTypeDisplayValue(
                                                                           (AggregatedType) ichConfig.AggregatedTypeId)
                                                                       : string.Empty;
                }

                // For disabled controls , get original values from database and assign it to current object
                if (ichRecordInDB != null)
                {
                    ProfileFieldsHelper.PrepareProfileModel(ichRecordInDB, ichConfig, _userCategory);
                }
                ichConfig.IchAccountId = ichConfig.IchAccountId != null
                                             ? ichConfig.IchAccountId.Trim()
                                             : ichConfig.IchAccountId;
                ichConfig = _memberManager.UpdateIchConfiguration(selectedMemberId, ichConfig);

                // Get MemberStatus details 
                var memberStatus = _memberManager.GetMemberStatus(selectedMemberId, "ICH");

                if (memberStatus.Count > 0)
                {
                    ViewData["memberStatus"] = memberStatus.ElementAt(memberStatus.Count - 1).MembershipStatusId;
                }
                else
                {
                    ViewData["memberStatus"] = 0;
                }

                // Member Status History details
                ViewData["StatusHistory"] = _memberManager.GetMemberStatus(selectedMemberId, "ICH");
                details = new UIMessageDetail
                              {
                                  IsFailed = false,
                                  Message = "ICH details saved successfully.",
                                  isRedirect = false,
                                  Id = ichConfig.MemberId
                              };
            }
            catch (ISBusinessException)
            {
                details = new UIMessageDetail { IsFailed = true, Message = "Error", Id = ichConfig.MemberId };
            }
            catch (Exception handledGenericException)
            {
                details = new UIMessageDetail
                  {
                      IsFailed = true,
                      Message = "Unexpected error occurred. please try again later or contact administrator."
                  };
            }

            return Json(details);
        }

        /// <summary>
        /// Location
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [OutputCache(CacheProfile = "donotCache")]
        public ActionResult Location(int selectedMemberId = 0)
        {
            Location location;
            if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
            {
                location = new Location();
            }
            else
            {
                  location = new Location {UserCategory = _userCategory};
            }

            Session["helplinkurl"] = "Managing_Location_Details_";
            location.MemberId = selectedMemberId;

            return View(location);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Location(Location location)
        {
            UIMessageDetail details;
            try
            {
                if (location.MemberId > 0 && SessionUtil.MemberId > 0 && location.MemberId != SessionUtil.MemberId)
                {
                    throw new Exception();
                }

                var locationInDb = _memberManager.GetMemberLocation(location.Id);

                string currency = (locationInDb != null && locationInDb.CurrencyId > 0) ? _memberManager.GetCurrencyName(locationInDb.CurrencyId.Value) : string.Empty;
                location.CurrencyIdFutureDisplayValue = location.CurrencyId > 0 ? _memberManager.GetCurrencyName(location.CurrencyId.Value) : string.Empty;
                location.CurrencyIdDisplayValue = currency;

                // For disabled controls , get original values from database and assign it to current object
                ProfileFieldsHelper.PrepareProfileModel(locationInDb, location, _userCategory);

                // Sync the 'Member Name' with 'Member Commercial Name' of Main location.
                var memberDisplayCommercialName = string.Empty;

                //SIT CMP 622- iiNET account id on location tab is saved with spaces(Bug-9518)
                if (location.LociiNetAccountId != null)
                {
                    location.LociiNetAccountId = location.LociiNetAccountId.Trim();
                }// End the White space issue code change

                if (locationInDb != null && !string.IsNullOrEmpty(locationInDb.LocationCode) && locationInDb.LocationCode.ToUpper().Equals("MAIN"))
                {
                    // For "Main" location update the member details CommercialName as well as LegalName.
                    var memberInDb = _memberManager.GetMember(location.MemberId);

                    // Trim the spaces if any for the member legal and commercial names.
                    location.MemberCommercialName = location.MemberCommercialName.Trim();
                    location.MemberLegalName = location.MemberLegalName.Trim();
                    location.LocationCode = locationInDb.LocationCode;
                    location.IsActive = locationInDb.IsActive;
                    memberInDb.DefaultLocation = location;

                    memberInDb = _memberManager.UpdateMember(memberInDb);
                    location = memberInDb.DefaultLocation;

                    memberDisplayCommercialName = memberInDb.DisplayCommercialName;
                }
                else
                {
                    //Author: Sachin Pharande
                    //Date: 18-06-2012
                    //Issue: SCP ID : 23155 - Changing locations becomes adding location 
                    //Reason: This property is used to check, whether clicked on 'Add Loction' button of the Location tab on Member profile screen.
                    if (location.LocationIdFlag == 1)
                    {
                        location.LocationCode = null;
                    }
                    location = _memberManager.UpdateMemberLocation(location.MemberId, location);
                }

                details = new CustomUiLocationMessageDetail
                {
                    IsFailed = false,
                    Message = "Location details saved successfully.",
                    isRedirect = false,
                    Id = location.Id,
                    Value = location.LocationCode,
                    DisplayCommercialName = memberDisplayCommercialName
                };
            }
            catch (ISBusinessException)
            {
                details = new UIMessageDetail
                {
                    IsFailed = true,
                    Message = "Error"
                };
            }
            catch (Exception handledGenericException)
            {
                if (handledGenericException != null && handledGenericException.InnerException != null &&
                    handledGenericException.InnerException.Message != null && handledGenericException.InnerException.Message.Contains("-20401"))
                {
                    details = new UIMessageDetail
                    {
                        IsFailed = true,
                        Message = Messages.ResourceManager.GetString(ErrorCodes.RecordRestrictFromISWEB)
                    };
                }
                else
                {
                    details = new UIMessageDetail
                    {
                        IsFailed = true,
                        Message = "Unexpected error occurred. please try again later or contact administrator."
                    };
                }
            }

            return Json(details);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "donotCache")]
        public ActionResult EBilling(int selectedMemberId = 0)
        {
            // Retrieve selectedMemberId value from Session variable and use it across the method
            bool updateFlag = false;
            EBillingConfiguration ebilling;

            ViewData["eBillingPendingFutureUpdates"] = false;

            if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
            {
                ebilling = new EBillingConfiguration();
            }
            else
            {
                if (selectedMemberId != 0)
                {
                    ebilling = _memberManager.GetEbillingConfig(selectedMemberId, true) ?? new EBillingConfiguration();
                }
                else
                {
                    ebilling = new EBillingConfiguration();
                    if (selectedMemberId > 0)
                    {
                        ebilling.MemberId = selectedMemberId;
                    }
                }


                //CMP#597:Displaying Technical Tab values on Ebilling Tab
                var technical = _memberManager.GetTechnicalConfig(selectedMemberId);

                if (technical != null)
                {
                    ebilling.IinetAccountIdPax = technical.PaxAccountId;
                    ebilling.IinetAccountIdCgo = technical.CgoAccountId;
                    ebilling.IinetAccountIdMisc = technical.MiscAccountId;
                    ebilling.IinetAccountIdUatp = technical.UatpAccountId;
                }


                var billingCountryList = _memberManager.GetDsSupportedByAtosCountryList();
                var billedCountryList = _memberManager.GetDsSupportedByAtosCountryList();

                // Get the list of future countries for billing category
                Country[] futuredsRequiredBillingCountries =
                    _memberManager.GetDsRequiredCountryList(selectedMemberId, (int) BillingTypes.Billing, true).ToArray();

                foreach (var dsRequiredCountry in futuredsRequiredBillingCountries)
                {
                    billingCountryList.RemoveAll(c => c.Id == dsRequiredCountry.Id);
                }

                // Get the list of future countries for billing category
                Country[] futuredsRequiredBilledCountries =
                    _memberManager.GetDsRequiredCountryList(selectedMemberId, (int) BillingTypes.Billed, true).ToArray();

                foreach (var dsRequiredCountry in futuredsRequiredBilledCountries)
                {
                    billedCountryList.RemoveAll(c => c.Id == dsRequiredCountry.Id);
                }

                // ViewData is used to bind the list box with the existing sponsors
                ViewData["FutureBillingDSSupportedCountryTo"] = new MultiSelectList(futuredsRequiredBillingCountries,
                                                                                    "Id", "Name");
                ViewData["FutureBilledDSSupportedCountryTo"] = new MultiSelectList(futuredsRequiredBilledCountries, "Id",
                                                                                   "Name");

                // Get the list of sponsored members
                Country[] currentdsRequiredBillingCountries =
                    _memberManager.GetDsRequiredCountryList(selectedMemberId, (int) BillingTypes.Billing, false).ToArray
                        ();
                Country[] currentdsRequiredBilledCountries =
                    _memberManager.GetDsRequiredCountryList(selectedMemberId, (int) BillingTypes.Billed, false).ToArray();

                // ViewData is used to bind the list box with the existing sponsors
                ViewData["currentdsRequiredBillingCountries"] = new MultiSelectList(currentdsRequiredBillingCountries,
                                                                                    "Id", "Name");
                ViewData["currentdsRequiredBilledCountries"] = new MultiSelectList(currentdsRequiredBilledCountries,
                                                                                   "Id", "Name");

                if (currentdsRequiredBillingCountries.Length > 0)
                {
                    //if the member is sponsoring other members, set value of flag to true
                    ebilling.HasDSReqCountriesAsBilling = true;
                }

                if (currentdsRequiredBilledCountries.Length > 0)
                {
                    //if the member is sponsoring other members, set value of flag to true
                    ebilling.HasDSReqCountriesAsBilled = true;
                }

                ViewData["DSSupportedBillingCountryListFrom"] = new MultiSelectList(billingCountryList.ToArray(), "Id",
                                                                                    "Name");
                ViewData["DSSupportedBilledCountryListFrom"] = new MultiSelectList(billedCountryList.ToArray(), "Id",
                                                                                   "Name");
                Session["helplinkurl"] = "Managing_E-billing_Services";
                ebilling.UserCategory = _userCategory;

                //CMP#666:
                var unAssociatedLocationsLocation = new List<UserContactLocations>();
                var listBoxAssociatedLoc = new List<UserContactLocations>();
                ViewData["UnAssociatedLocation"] = new MultiSelectList(unAssociatedLocationsLocation.ToArray(), "LocationId", "LocationName");
                ViewData["AssociatedLocation"] = new MultiSelectList(listBoxAssociatedLoc.ToArray(), "LocationId", "LocationName");
                ViewData["UnAssociatedPayLocation"] = new MultiSelectList(unAssociatedLocationsLocation.ToArray(), "LocationId", "LocationName");
                ViewData["AssociatedPayLocation"] = new MultiSelectList(listBoxAssociatedLoc.ToArray(), "LocationId", "LocationName");
                //End Code

            }
            return View(ebilling);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult EBilling(EBillingConfiguration eBilling)
        {
            UIMessageDetail details;
            try
            {
                if (eBilling.MemberId > 0 && SessionUtil.MemberId > 0 && eBilling.MemberId != SessionUtil.MemberId)
                {
                    throw new ISBusinessException(ErrorCodes.InvalidIsUatpInvIgnoreFromDsprocError);
                }

                //Get member detail based on member id.
                var memberDetail = _memberManager.GetMemberDetails(eBilling.MemberId);
                //231363: SRM: CDC.
                //Check cdc compartment id has value or not if not then check all ebilling field which should not be true.
                if ((!String.IsNullOrEmpty(memberDetail.CdcCompartmentIDforInv) && memberDetail.LegalArchivingRequired) || CheckEbillingFieldValueStatus(eBilling))
                {
                    var ebillngInDB = _memberManager.GetEbillingConfig(eBilling.MemberId, true);

                    // For disabled controls , get original values from database and assign it to current object
                    ProfileFieldsHelper.PrepareProfileModel(ebillngInDB, eBilling, _userCategory);

                    //Legal Archive server side validation.
                    //These below all fields are future updated field so these fileds should not required to be changed while saving ebilling details.  
                    eBilling.LegalArchRequiredforCgoPayInv = ebillngInDB.LegalArchRequiredforCgoPayInv;
                    eBilling.LegalArchRequiredforCgoRecInv = ebillngInDB.LegalArchRequiredforCgoRecInv;
                    eBilling.LegalArchRequiredforPaxPayInv = ebillngInDB.LegalArchRequiredforPaxPayInv;
                    eBilling.LegalArchRequiredforPaxRecInv = ebillngInDB.LegalArchRequiredforPaxRecInv;
                    eBilling.LegalArchRequiredforMiscPayInv = ebillngInDB.LegalArchRequiredforMiscPayInv;
                    eBilling.LegalArchRequiredforMiscRecInv = ebillngInDB.LegalArchRequiredforMiscRecInv;
                    eBilling.LegalArchRequiredforUatpPayInv = ebillngInDB.LegalArchRequiredforUatpPayInv;
                    eBilling.LegalArchRequiredforUatpRecInv = ebillngInDB.LegalArchRequiredforUatpRecInv;

                    // TFS_Bug8928: CMP597: System displaying value as true in checkbox where account ID is null
                    // Reading latest values of PaxAccountId, CgoAccountId, MiscAccountId and UatpAccountId from Member Technical Configuration
                    // and updating the values in EBilling Configuration accordingly.
                    var technical = _memberManager.GetTechnicalConfig(eBilling.MemberId);

                    if (technical != null)
                    {

                        if (string.IsNullOrEmpty(technical.PaxAccountId))
                        {
                            eBilling.IinetAccountIdPax = string.Empty;
                            eBilling.ChangeInfoRefDataPax = false;
                            eBilling.CompleteRefDataPax = false;
                            eBilling.CompleteContactsDataPax = false;
                        }
                        if (string.IsNullOrEmpty(technical.CgoAccountId))
                        {
                            eBilling.IinetAccountIdCgo = string.Empty;
                            eBilling.ChangeInfoRefDataCgo = false;
                            eBilling.CompleteRefDataCgo = false;
                            eBilling.CompleteContactsDataCgo = false;
                        }
                        if (string.IsNullOrEmpty(technical.MiscAccountId))
                        {
                            eBilling.IinetAccountIdMisc = string.Empty;
                            eBilling.ChangeInfoRefDataMisc = false;
                            eBilling.CompleteRefDataMisc = false;
                            eBilling.CompleteContactsDataMisc = false;
                        }
                        if (string.IsNullOrEmpty(technical.UatpAccountId))
                        {
                            eBilling.IinetAccountIdUatp = string.Empty;
                            eBilling.ChangeInfoRefDataUatp = false;
                            eBilling.CompleteRefDataUatp = false;
                            eBilling.CompleteContactsDataUatp = false;
                        }
                    }
                    eBilling = _memberManager.UpdateEBillingConfiguration(eBilling.MemberId, eBilling);
                    details = new UIMessageDetail { IsFailed = false, Message = "E-Billing details saved successfully.", isRedirect = false, Id = eBilling.MemberId, };
                }
                else
                {
                    details = new UIMessageDetail
                                {
                                    IsFailed = true,
                                    Message = "E-Billing details can not be save because legal archiving is disabled in the SIS-OPS tab.",
                                    isRedirect = false,
                                    Id = eBilling.MemberId
                                };
                }
            }
            catch (ISBusinessException ex)
            {
                if (ex.ErrorCode == ErrorCodes.InvalidIsUatpInvIgnoreFromDsprocError)
                {
                    details = new UIMessageDetail { IsFailed = true, Message = string.Format(Messages.BMEM_10109) };
                }
                else
                {
                    details = new UIMessageDetail { IsFailed = true, Message = "Error" };
                }
            }
            catch (Exception handledGenericException)
            {
                details = new UIMessageDetail
                  {
                      IsFailed = true,
                      Message = "Unexpected error occurred. please try again later or contact administrator."
                  };
            }

            return Json(details);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OutputCache(CacheProfile = "donotCache")]
        public ActionResult Ach(int selectedMemberId = 0)
        {
            AchConfiguration ach;
            if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
            {
                ach = new AchConfiguration();
            }
            else
            {
                // Retrieve selectedMemberId value from Session variable and use it across the method

                // Get the list of exception members for Pax
                AchException[] futureExceptionMemberspax =
                    _memberManager.GetExceptionMembers(selectedMemberId, (int)BillingCategoryType.Pax, true).ToArray();

                // ViewData is used to bind the list box with the existing exception members for pax category
                ViewData["paxexceptionMemberList"] = new MultiSelectList(futureExceptionMemberspax, "ExceptionMemberId",
                                                                         "ExceptionMemberCommercialName");

                // Get the list of sponsored members
                AchException[] currentPaxExceptionMembers =
                    _memberManager.GetExceptionMembers(selectedMemberId, (int)BillingCategoryType.Pax, false).ToArray();

                // ViewData is used to bind the list box with the existing sponsors
                ViewData["currentPaxExceptionMembers"] = new MultiSelectList(currentPaxExceptionMembers, "ExceptionMemberId",
                                                                             "ExceptionMemberCommercialName");

                AchException[] futureExceptionMemberscgo =
                    _memberManager.GetExceptionMembers(selectedMemberId, (int)BillingCategoryType.Cgo, true).ToArray();

                // ViewData is used to bind the list box with the existing exception members for cgo category
                ViewData["cgoexceptionMemberList"] = new MultiSelectList(futureExceptionMemberscgo, "ExceptionMemberId",
                                                                         "ExceptionMemberCommercialName");

                //  Get the list of sponsored members
                AchException[] currentCgoExceptionMembers =
                    _memberManager.GetExceptionMembers(selectedMemberId, (int)BillingCategoryType.Cgo, false).ToArray();

                // ViewData is used to bind the list box with the existing sponsors
                ViewData["currentCgoExceptionMembers"] = new MultiSelectList(currentCgoExceptionMembers, "ExceptionMemberId",
                                                                             "ExceptionMemberCommercialName");

                AchException[] futureExceptionMembersmisc =
                    _memberManager.GetExceptionMembers(selectedMemberId, (int)BillingCategoryType.Misc, true).ToArray();

                // ViewData is used to bind the list box with the existing exception members for cgo category
                ViewData["miscexceptionMemberList"] = new MultiSelectList(futureExceptionMembersmisc, "ExceptionMemberId",
                                                                          "ExceptionMemberCommercialName");

                //  Get the list of sponsored members
                AchException[] currentMiscExceptionMembers =
                    _memberManager.GetExceptionMembers(selectedMemberId, (int)BillingCategoryType.Misc, false).ToArray();

                // ViewData is used to bind the list box with the existing sponsors
                ViewData["currentMiscExceptionMembers"] = new MultiSelectList(currentMiscExceptionMembers,
                                                                              "ExceptionMemberId",
                                                                              "ExceptionMemberCommercialName");

                AchException[] futureExceptionMembersuatp =
                    _memberManager.GetExceptionMembers(selectedMemberId, (int)BillingCategoryType.Uatp, true).ToArray();

                // ViewData is used to bind the list box with the existing exception members for cgo category
                ViewData["uatpexceptionMemberList"] = new MultiSelectList(futureExceptionMembersuatp, "ExceptionMemberId",
                                                                          "ExceptionMemberCommercialName");

                //  Get the list of sponsored members
                AchException[] currentUatpExceptionMembers =
                    _memberManager.GetExceptionMembers(selectedMemberId, (int)BillingCategoryType.Uatp, false).ToArray();

                // ViewData is used to bind the list box with the existing sponsors
                ViewData["currentUatpExceptionMembers"] = new MultiSelectList(currentUatpExceptionMembers,
                                                                              "ExceptionMemberId",
                                                                              "ExceptionMemberCommercialName");

                ach = _memberManager.GetAchConfig(selectedMemberId, true) ?? new AchConfiguration();

                ach = _memberManager.GetAchMemberStatusList(ach);

                // Get MemberStatus details 
                var achMemberStatus = _memberManager.GetMemberStatus(selectedMemberId, "ACH");

                if (achMemberStatus.Count > 0)
                {
                    ViewData["achMemberStatus"] = achMemberStatus.ElementAt(achMemberStatus.Count - 1).MembershipStatusId;
                }
                else
                {
                    ViewData["achMemberStatus"] = 0;
                }

                if ((ach != null) && (currentPaxExceptionMembers.Length > 0))
                {
                    //if the member is sponsoring other members, set value of flag to true
                    ach.HasPaxExceptionMembers = true;
                }

                if ((ach != null) && (currentCgoExceptionMembers.Length > 0))
                {
                    //if the member is sponsoring other members, set value of flag to true
                    ach.HasCgoExceptionMembers = true;
                }
                if ((ach != null) && (currentMiscExceptionMembers.Length > 0))
                {
                    //if the member is sponsoring other members, set value of flag to true
                    ach.HasMiscExceptionMembers = true;
                }
                if ((ach != null) && (currentUatpExceptionMembers.Length > 0))
                {
                    //if the member is sponsoring other members, set value of flag to true
                    ach.HasUatpExceptionMembers = true;
                }
                ViewData["ContactsDataRowCount"] = 0;

                ach.UserCategory = _userCategory;

                if (selectedMemberId > 0)
                {
                    ach.MemberId = selectedMemberId;
                }

                //CMP-689-Flexible CH Activation Options
                //pass here the fieldtype of field AchMembershipStatusId to view by using viewdata
                //if AchMembershipStatusId having future value as  'Live' from 'Not a Member' or 'Terminated' , set the fieldtype 19 else 16
                //On the basis of its field type, functinality of field AchMembershipStatusId will differ on View
                ViewData["AchMembershipStatusIdFieldType"] = ach.AchMembershipStatusIdFutureValue == 1 &&
                                                         (ach.AchMembershipStatusId == 3 || ach.AchMembershipStatusId == 4)
                                                           ? 20
                                                           : 17;
            }
            Session["helplinkurl"] = "Viewing_ACH_Configurations";
            return View(ach);
        }

        /// <summary>
        /// Save Ach details.
        /// </summary>
        /// <param name="achConfig"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Ach(AchConfiguration achConfig)
        {
            // Retrieve selectedMemberId value from Member variable and use it across the method
            var selectedMemberId = achConfig.MemberId; // 
            UIMessageDetail details;
            try
            {
                if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
                {
                    throw new ISBusinessException("Unauthorized Access.");
                }

                if (achConfig.PaxExceptionFuturePeriod == "YYYY-MMM-PP")
                {
                    achConfig.PaxExceptionFuturePeriod = null;
                }
                if (achConfig.CgoExceptionFuturePeriod == "YYYY-MMM-PP")
                {
                    achConfig.CgoExceptionFuturePeriod = null;
                }
                if (achConfig.MiscExceptionFuturePeriod == "YYYY-MMM-PP")
                {
                    achConfig.MiscExceptionFuturePeriod = null;
                }
                if (achConfig.UatpExceptionFuturePeriod == "YYYY-MMM-PP")
                {
                    achConfig.UatpExceptionFuturePeriod = null;
                }


                var achConfigInDB = _memberManager.GetAchConfig(selectedMemberId, true);
                if (achConfigInDB != null)
                {
                    achConfig.AchCategoryIdDisplayValue = achConfigInDB.AchCategoryId > 0 ? EnumMapper.GetachCategoryDisplayValue(achConfigInDB.AchCategoryId) : string.Empty;
                    if (!string.IsNullOrWhiteSpace(achConfigInDB.AchClearanceInvoiceSubmissionPatternPaxId.ToString()))
                        achConfig.AchClearanceInvoiceSubmissionPatternPaxIdDisplayValue = ConvertToBool(achConfigInDB.AchClearanceInvoiceSubmissionPatternPaxId);

                    if (!string.IsNullOrWhiteSpace(achConfigInDB.AchClearanceInvoiceSubmissionPatternCgoId.ToString()))
                        achConfig.AchClearanceInvoiceSubmissionPatternCgoIdDisplayValue = ConvertToBool(achConfigInDB.AchClearanceInvoiceSubmissionPatternCgoId);

                    if (!string.IsNullOrWhiteSpace(achConfigInDB.AchClearanceInvoiceSubmissionPatternMiscId.ToString()))
                        achConfig.AchClearanceInvoiceSubmissionPatternMiscIdDisplayValue =
                            ConvertToBool(achConfigInDB.AchClearanceInvoiceSubmissionPatternMiscId);

                    if (!string.IsNullOrWhiteSpace(achConfigInDB.AchClearanceInvoiceSubmissionPatternUatpId.ToString()))
                        achConfig.AchClearanceInvoiceSubmissionPatternUatpIdDisplayValue =
                            ConvertToBool(achConfigInDB.AchClearanceInvoiceSubmissionPatternUatpId);

                    if (!string.IsNullOrWhiteSpace(achConfigInDB.InterClearanceInvoiceSubmissionPatternPaxId.ToString()))
                        achConfig.InterClearanceInvoiceSubmissionPatternPaxIdDisplayValue =
                            ConvertToBool(achConfigInDB.InterClearanceInvoiceSubmissionPatternPaxId);

                    if (!string.IsNullOrWhiteSpace(achConfigInDB.InterClearanceInvoiceSubmissionPatternCgoId.ToString()))
                        achConfig.InterClearanceInvoiceSubmissionPatternCgoIdDisplayValue =
                            ConvertToBool(achConfigInDB.InterClearanceInvoiceSubmissionPatternCgoId);

                    if (!string.IsNullOrWhiteSpace(achConfigInDB.InterClearanceInvoiceSubmissionPatternMiscId.ToString()))
                        achConfig.InterClearanceInvoiceSubmissionPatternMiscIdDisplayValue =
                            ConvertToBool(achConfigInDB.InterClearanceInvoiceSubmissionPatternMiscId);

                    if (!string.IsNullOrWhiteSpace(achConfigInDB.InterClearanceInvoiceSubmissionPatternUatpId.ToString()))
                        achConfig.InterClearanceInvoiceSubmissionPatternUatpIdDisplayValue =
                            ConvertToBool(achConfigInDB.InterClearanceInvoiceSubmissionPatternUatpId);

                }

                if (!string.IsNullOrWhiteSpace(achConfig.AchClearanceInvoiceSubmissionPatternPaxId.ToString()))
                    achConfig.AchClearanceInvoiceSubmissionPatternPaxIdFutureDisplayValue = ConvertToBool(achConfig.AchClearanceInvoiceSubmissionPatternPaxId);
                achConfig.AchCategoryIdFutureDisplayValue = achConfig.AchCategoryId > 0 ? EnumMapper.GetachCategoryDisplayValue(achConfig.AchCategoryId) : string.Empty;

                if (!string.IsNullOrWhiteSpace(achConfig.AchClearanceInvoiceSubmissionPatternCgoId.ToString()))
                    achConfig.AchClearanceInvoiceSubmissionPatternCgoIdFutureDisplayValue =
                        ConvertToBool(achConfig.AchClearanceInvoiceSubmissionPatternCgoId);

                if (!string.IsNullOrWhiteSpace(achConfig.AchClearanceInvoiceSubmissionPatternMiscId.ToString()))
                    achConfig.AchClearanceInvoiceSubmissionPatternMiscIdFutureDisplayValue =
                        ConvertToBool(achConfig.AchClearanceInvoiceSubmissionPatternMiscId);

                if (!string.IsNullOrWhiteSpace(achConfig.AchClearanceInvoiceSubmissionPatternUatpId.ToString()))
                    achConfig.AchClearanceInvoiceSubmissionPatternUatpIdFutureDisplayValue =
                        ConvertToBool(achConfig.AchClearanceInvoiceSubmissionPatternUatpId);

                if (!string.IsNullOrWhiteSpace(achConfig.InterClearanceInvoiceSubmissionPatternCgoId.ToString()))
                    achConfig.InterClearanceInvoiceSubmissionPatternCgoIdFutureDisplayValue =
                        ConvertToBool(achConfig.InterClearanceInvoiceSubmissionPatternCgoId);

                if (!string.IsNullOrWhiteSpace(achConfig.InterClearanceInvoiceSubmissionPatternPaxId.ToString()))
                    achConfig.InterClearanceInvoiceSubmissionPatternPaxIdFutureDisplayValue =
                        ConvertToBool(achConfig.InterClearanceInvoiceSubmissionPatternPaxId);

                if (!string.IsNullOrWhiteSpace(achConfig.InterClearanceInvoiceSubmissionPatternMiscId.ToString()))
                    achConfig.InterClearanceInvoiceSubmissionPatternMiscIdFutureDisplayValue =
                        ConvertToBool(achConfig.InterClearanceInvoiceSubmissionPatternMiscId);

                if (!string.IsNullOrWhiteSpace(achConfig.InterClearanceInvoiceSubmissionPatternUatpId.ToString()))
                    achConfig.InterClearanceInvoiceSubmissionPatternUatpIdFutureDisplayValue =
                        ConvertToBool(achConfig.InterClearanceInvoiceSubmissionPatternUatpId);

                // For disabled controls , get original values from database and assign it to current object
                if (achConfigInDB != null)
                {
                    ProfileFieldsHelper.PrepareProfileModel(achConfigInDB, achConfig, _userCategory);
                }

                achConfig = _memberManager.UpdateAchConfiguration(selectedMemberId, achConfig);

                // Get MemberStatus details 
                var achMemberStatus = _memberManager.GetMemberStatus(selectedMemberId, "ACH");

                if (achMemberStatus.Count > 0)
                {
                    ViewData["achMemberStatus"] = achMemberStatus.ElementAt(achMemberStatus.Count - 1).MembershipStatusId;
                }
                else
                {
                    ViewData["achMemberStatus"] = 0;
                }

                // Member Status History details
                ViewData["StatusHistory"] = _memberManager.GetMemberStatus(selectedMemberId, "ACH");
                details = new UIMessageDetail { IsFailed = false, Message = "ACH details saved successfully.", isRedirect = false };
            }
            catch (ISBusinessException exception)
            {
                details = new UIMessageDetail { IsFailed = true, Message = ShowMessageText(exception.ErrorCode) };
            }
            catch (Exception handledGenericException)
            {
                details = new UIMessageDetail { IsFailed = true, Message = "Unexpected error occurred. please try again later or contact administrator." };
            }

            return Json(details);
        }


        private string ConvertToBool(int param)
        {

            string pValue = Convert.ToString(param);
            string pFinalValue = string.Empty;
            int originalLength = 4;
            if (pValue != null)
            {
                if (pValue.Length != 4)
                {
                    originalLength = originalLength - pValue.Length;
                    for (int i = 0; i < originalLength; i++)
                    {
                        pValue = "0" + pValue;
                    }
                }


                for (int i = 0; i < pValue.Length; i++)
                {
                    string dValue = pValue.Substring(i, 1);
                    dValue = dValue == "1" ? "Yes" : "No";
                    pFinalValue = pFinalValue + dValue + ",";
                }

                pFinalValue = pFinalValue.Substring(0, (pFinalValue.Length - 1));

            }
            return pFinalValue;
        }
        /// <summary>
        /// Technical Config
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [OutputCache(CacheProfile = "donotCache")]
        public ActionResult Technical(int selectedMemberId = 0)
        {
            TechnicalConfiguration technical;

            if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
            {
                technical = new TechnicalConfiguration();
            }
            else
            {
                technical = _memberManager.GetTechnicalConfig(selectedMemberId);
                if (technical != null)
                {
                }
                else
                {
                    technical = new TechnicalConfiguration();
                }
                technical.UserCategory = _userCategory;

                if (selectedMemberId > 0)
                {
                    technical.MemberId = selectedMemberId;
                }
            }
            return View(technical);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="technicalConfig"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Technical(TechnicalConfiguration technicalConfig)
        {
            // Retrieve selectedMemberId value from Model variable and use it across the method
            var selectedMemberId = technicalConfig.MemberId; // 
            EBillingConfiguration ebilling;
            var updateFlag = false;
            UIMessageDetail details;
            try
            {
                if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
                {
                    throw new ISBusinessException("Unauthorized Access.");
                }

                //CMP597 : Checkbox in ebilling tab open eventhough it contains only space/ white space.
                if (technicalConfig.PaxAccountId != null)
                {
                    technicalConfig.PaxAccountId = technicalConfig.PaxAccountId.Trim();
                }
                if (technicalConfig.CgoAccountId != null)
                {
                    technicalConfig.CgoAccountId = technicalConfig.CgoAccountId.Trim();
                }
                if (technicalConfig.MiscAccountId != null)
                {
                    technicalConfig.MiscAccountId = technicalConfig.MiscAccountId.Trim();
                }
                if (technicalConfig.UatpAccountId != null)
                {
                    technicalConfig.UatpAccountId = technicalConfig.UatpAccountId.Trim();
                }
                // End the White space issue code change

                ebilling = new EBillingConfiguration();
                ebilling = _memberManager.GetEbillingConfig(selectedMemberId);
                //CMP 597
                // If the iiNET Account of a Billing Category of any Member is removed (i.e. a non-blank value is modified to blank),
                //then the value of all three checkboxes linked to that Member’s Account ID in the e-Billing tab should be set to False/No
                if (String.IsNullOrEmpty(technicalConfig.PaxAccountId))
                {
                    if (ebilling.ChangeInfoRefDataPax || ebilling.CompleteContactsDataPax || ebilling.CompleteRefDataPax)
                    {
                        updateFlag = true;
                        ebilling.IinetAccountIdPax = "";
                        ebilling.ChangeInfoRefDataPax = false;
                        ebilling.CompleteRefDataPax = false;
                        ebilling.CompleteContactsDataPax = false;
                    }

                }
                if (String.IsNullOrEmpty(technicalConfig.CgoAccountId))
                {
                    if (ebilling.ChangeInfoRefDataCgo || ebilling.CompleteRefDataCgo || ebilling.CompleteContactsDataCgo)
                    {
                        updateFlag = true;
                        ebilling.IinetAccountIdCgo = "";
                        ebilling.ChangeInfoRefDataCgo = false;
                        ebilling.CompleteRefDataCgo = false;
                        ebilling.CompleteContactsDataCgo = false;
                    }

                }
                if (string.IsNullOrEmpty(technicalConfig.MiscAccountId))
                {
                    if (ebilling.CompleteContactsDataMisc || ebilling.CompleteRefDataMisc || ebilling.ChangeInfoRefDataMisc)
                    {
                        updateFlag = true;
                        ebilling.IinetAccountIdMisc = "";
                        ebilling.ChangeInfoRefDataMisc = false;
                        ebilling.CompleteRefDataMisc = false;
                        ebilling.CompleteContactsDataMisc = false;
                    }
                }
                if (string.IsNullOrEmpty(technicalConfig.UatpAccountId))
                {
                    if (ebilling.ChangeInfoRefDataUatp || ebilling.CompleteContactsDataUatp || ebilling.CompleteRefDataUatp)
                    {
                        updateFlag = true;
                        ebilling.IinetAccountIdUatp = "";
                        ebilling.ChangeInfoRefDataUatp = false;
                        ebilling.CompleteRefDataUatp = false;
                        ebilling.CompleteContactsDataUatp = false;
                    }
                }
                if (updateFlag)
                    _memberManager.UpdateEBillingConfiguration(selectedMemberId, ebilling);

                technicalConfig = _memberManager.UpdateTechnicalConfiguration(selectedMemberId, technicalConfig);

                details = new UIMessageDetail { IsFailed = false, Message = "Technical details saved successfully.", isRedirect = false };
            }
            catch (ISBusinessException exception)
            {
                details = new UIMessageDetail { IsFailed = true, Message = ShowMessageText(exception.ErrorCode) };
            }

            return Json(details);
        }

        /// <summary>
        /// Gets details of miscellaneous configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [OutputCache(CacheProfile = "donotCache")]
        public ActionResult Misc(int selectedMemberId = 0)
        {
            MiscellaneousConfiguration miscellaneous;
            if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
            {
                miscellaneous = new MiscellaneousConfiguration();
            }
            else
            {
                // Retrieve selectedMemberId value from Session variable and use it across the method
                ViewData["miscPendingFutureUpdates"] = false;
                ViewData["TabType"] = "misc";
                if (selectedMemberId != 0)
                {
                    miscellaneous = _memberManager.GetMiscellaneousConfiguration(selectedMemberId, true) ??
                                    new MiscellaneousConfiguration();
                }
                else
                {
                    miscellaneous = new MiscellaneousConfiguration();
                }
                if (selectedMemberId > 0)
                {
                    miscellaneous.MemberId = selectedMemberId;
                }

                ViewData["ContactsDataRowCount"] = 0;

                miscellaneous.UserCategory = _userCategory;
            }
            Session["helplinkurl"] = "Managing_Passenger_Billing_Configurations";
            return View(miscellaneous);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="miscConfig">miscellaneous configuration object</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Misc(MiscellaneousConfiguration miscConfig)
        {
            // Retrieve selectedMemberId value from Model variable and use it across the method
            var selectedMemberId = miscConfig.MemberId; // 
            MiscellaneousConfiguration miscRecordindb;
            UIMessageDetail details;

            try
            {
                if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
                {
                    throw new Exception();
                }

                //Read current passenger configuration for selected member to know values of migration fields
                miscRecordindb = _memberManager.GetMiscellaneousConfiguration(selectedMemberId, true);

                if (miscRecordindb != null)
                {
                    miscConfig.BillingIsXmlMigrationStatusIdDisplayValue = miscRecordindb.BillingIsXmlMigrationStatusId > 0
                                                                             ? EnumMapper.GetMigrationStatusDisplayValue(miscRecordindb.BillingIsXmlMigrationStatusId)
                                                                             : string.Empty;

                    if (string.IsNullOrEmpty(miscConfig.RejectionOnValidationFailureIdDisplayValue))
                    {
                        miscConfig.RejectionOnValidationFailureIdDisplayValue = miscRecordindb.RejectionOnValidationFailureId > 0
                                                                                  ? EnumMapper.GetRejectionOnValidationFailureDisplayValue(miscConfig.RejectionOnValidationFailureId)
                                                                                  : string.Empty;
                    }
                }

                ViewData["TabType"] = "misc";
                //Set text value for migration status id field so that it can be saved to database

                miscConfig.BillingIsXmlMigrationStatusIdFutureDisplayValue = miscConfig.BillingIsXmlMigrationStatusId > 0
                                                                               ? EnumMapper.GetMigrationStatusDisplayValue(miscConfig.BillingIsXmlMigrationStatusId)
                                                                               : string.Empty;

                if (string.IsNullOrEmpty(miscConfig.RejectionOnValidationFailureIdDisplayValue))
                {
                    miscConfig.RejectionOnValidationFailureIdFutureDisplayValue = miscConfig.RejectionOnValidationFailureId > 0
                                                                                    ? EnumMapper.GetRejectionOnValidationFailureDisplayValue(miscConfig.RejectionOnValidationFailureId)
                                                                                    : string.Empty;
                }

                // For disabled controls , get original values from database and assign it to current object
                ProfileFieldsHelper.PrepareProfileModel(miscRecordindb, miscConfig, _userCategory);

                // if dropdown value not equal to certified then fill null in respective fields 
                // insert null if not certified
                #region
                if (miscConfig.BillingIsXmlMigrationStatusId < 3)
                {
                    miscConfig.BillingIsXmlCertifiedOn = null;
                    miscConfig.BillingIsXmlMigrationDate = null;
                }
                #endregion

                miscConfig = _memberManager.UpdateMiscellaneousConfiguration(selectedMemberId, miscConfig);
                details = new UIMessageDetail { IsFailed = false, Message = "Miscellaneous details saved successfully.", isRedirect = false };
            }
            catch (ISBusinessException es)
            {
                _logger.Error("Business exception in MISC tab.", es);

                details = new UIMessageDetail
                {
                    IsFailed = true,
                    Message = es.ErrorCode + "-" + "Error occured while saving Miscellaneous details"
                };
            }
            catch (Exception handledGenericException)
            {
                _logger.Error("Exception occured in MISC tab.", handledGenericException);
                details = new UIMessageDetail { IsFailed = true, Message = "An error occured while saving Miscellaneous details" };
            }

            return Json(details);
        }

        /// <summary>
        /// Gets details of uatp configuration
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [OutputCache(CacheProfile = "donotCache")]
        public ActionResult Uatp(int selectedMemberId = 0)
        {
            // Retrieve selectedmemberId value from Session variable and use it across the method

            UatpConfiguration uatp;
            ViewData["TabType"] = "uatp";
            ViewData["uatpPendingFutureUpdates"] = false;

            if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
            {
                uatp = new UatpConfiguration();
            }
            else
            {
                if (selectedMemberId != 0)
                {
                    uatp = _memberManager.GetUATPConfiguration(selectedMemberId, true) ?? new UatpConfiguration();
                }
                else
                {
                    uatp = new UatpConfiguration();
                }

                uatp.UserCategory = _userCategory;
                if (selectedMemberId > 0)
                {
                    uatp.MemberId = selectedMemberId;
                }
            }
            Session["helplinkurl"] = "Managing_UATP_Billing_Configurations";
            return View(uatp);
        }

        /// <summary>
        /// action method which will get called when misc tab data is posted
        /// </summary>
        /// <param name="uatpConfig">UATP configuration object</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Uatp(UatpConfiguration uatpConfig)
        {
            // Retrieve selectedmemberId value from Model variable and use it across the method
            var selectedMemberId = uatpConfig.MemberId; // 
            UatpConfiguration uatpRecordindb;
            UIMessageDetail details;

            try
            {
                if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
                {
                    throw new Exception();
                }

                //Read current passenger configuration for selected member to know values of migration fields
                uatpRecordindb = _memberManager.GetUATPConfiguration(selectedMemberId, true);

                if (uatpRecordindb != null)
                {
                    uatpConfig.BillingIsXmlMigrationStatusIdDisplayValue = uatpRecordindb.BillingIsXmlMigrationStatusId > 0
                                                                             ? EnumMapper.GetMigrationStatusDisplayValue(uatpRecordindb.BillingIsXmlMigrationStatusId)
                                                                             : string.Empty;

                    if (string.IsNullOrEmpty(uatpConfig.RejectionOnValidationFailureIdDisplayValue))
                    {
                        uatpConfig.RejectionOnValidationFailureIdDisplayValue = uatpRecordindb.RejectionOnValidationFailureId > 0
                                                                                  ? EnumMapper.GetRejectionOnValidationFailureDisplayValue(uatpConfig.RejectionOnValidationFailureId)
                                                                                  : string.Empty;
                    }
                }

                ViewData["TabType"] = "uatp";
                //Set text value for migration status id field so that it can be saved to database
                uatpConfig.BillingIsXmlMigrationStatusIdFutureDisplayValue = uatpConfig.BillingIsXmlMigrationStatusId > 0
                                                                               ? EnumMapper.GetMigrationStatusDisplayValue(uatpConfig.BillingIsXmlMigrationStatusId)
                                                                               : string.Empty;
                if (string.IsNullOrEmpty(uatpConfig.RejectionOnValidationFailureIdDisplayValue))
                {
                    uatpConfig.RejectionOnValidationFailureIdFutureDisplayValue = uatpConfig.RejectionOnValidationFailureId > 0
                                                                                    ? EnumMapper.GetRejectionOnValidationFailureDisplayValue(uatpConfig.RejectionOnValidationFailureId)
                                                                                    : string.Empty;
                }

                // For disabled controls , get original values from database and assign it to current object
                ProfileFieldsHelper.PrepareProfileModel<UatpConfiguration>(uatpRecordindb, uatpConfig, SessionUtil.UserCategory);

                // if dropdown value not equal to certified then fill null in respective fields 
                // insert null if not certified
                #region
                if (uatpConfig.BillingIsXmlMigrationStatusId < 3)
                {
                    uatpConfig.BillingIsXmlCertifiedOn = null;
                    uatpConfig.BillingIsXmlMigrationDate = null;
                }
                #endregion

                uatpConfig = _memberManager.UpdateUatpConfiguration(selectedMemberId, uatpConfig);

                var value = false;
                if (uatpConfig.IsDigitalSignatureRequired)
                {
                    value = uatpConfig.IsDigitalSignatureRequired;
                }
                else if (uatpConfig.IsDigitalSignatureRequiredFutureValue)
                {
                    value = uatpConfig.IsDigitalSignatureRequiredFutureValue;
                }
                var futureFieldValue = false;

                if (uatpConfig.UatpInvoiceHandledbyAtcan)
                {
                    futureFieldValue = uatpConfig.UatpInvoiceHandledbyAtcan;
                }
                else if (uatpConfig.UatpInvoiceHandledbyAtcanFutureValue.HasValue)
                {
                    futureFieldValue = uatpConfig.UatpInvoiceHandledbyAtcanFutureValue.Value;
                }

                details = new CustomUiMessageDetail() { IsFailed = false, Message = "UATP details saved successfully.", isRedirect = false, Value = value.ToString(), FutureFieldValue = futureFieldValue.ToString() };
            }
            catch (ISBusinessException ex)
            {
                if (ex.ErrorCode == ErrorCodes.InvalidIsUatpInvIgnoreFromDsprocError)
                {
                    details = new UIMessageDetail { IsFailed = true, Message = string.Format(Messages.BMEM_10109) };
                }
                else if (ex.ErrorCode == ErrorCodes.InvalidIsUatpInvoiceHandledByAtcanError)
                {
                    details = new UIMessageDetail { IsFailed = true, Message = string.Format(Messages.BMEM_10110) };
                }
                else
                {
                    details = new UIMessageDetail { IsFailed = true, Message = "Error" };
                }
            }
            catch (Exception handledGenericException)
            {
                _logger.Error("Error in UATP tab.", handledGenericException);
                details = new UIMessageDetail { IsFailed = true, Message = "Error" };
            }

            return Json(details);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "donotCache")]
        public ActionResult Contacts(int selectedMemberId = 0)
        {
            const string firstName = "";
            const string lastName = "";
            const string emailAddress = "";
            const string staffId = "";
            var contactsGrid = new Contacts("ContactsGrid", Url.Action("ContactsData", "Member", new { firstName, lastName, emailAddress, staffId, selectedMemberId }));
            ViewData["ContactsGrid"] = contactsGrid.Instance;
            ViewData["ContactsDataRowCount"] = 0;
            Session["helplinkurl"] = "Managing_Contacts_";
            ViewData["SelectedMemberId"] = selectedMemberId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Contacts(Contact contact)
        {
            // Retrieve selectedmemberId value from model and use it across the method
            var selectedMemberId = contact.MemberId; // 

            UIMessageDetail details;

            try
            {
                if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
                {
                    throw new ISBusinessException(ErrorCodes.NotTheUserOfSelectedMember);
                }

                var contactInDb = _memberManager.GetContactDetails(contact.Id);

                contact.CountryIdDisplayValue = (contactInDb != null && !string.IsNullOrEmpty(contactInDb.CountryId)) ? _memberManager.GetCountryName(contactInDb.CountryId) : string.Empty;
                contact.CountryIdFutureDisplayValue = (!string.IsNullOrEmpty(contact.CountryId)) ? _memberManager.GetCountryName(contact.CountryId) : string.Empty;
                contact.LocationIdDisplayValue = (contactInDb != null && contactInDb.LocationId > 0) ? _memberManager.GetMemberLocation(contactInDb.LocationId).LocationCode : string.Empty;
                contact.LocationIdFutureDisplayValue = contact.LocationId > 0 ? _memberManager.GetMemberLocation(contact.LocationId).LocationCode : string.Empty;
                contact.SalutationIdDisplayValue = (contactInDb != null && contactInDb.SalutationId > 0) ? EnumMapper.GetSaluationDisplayValue((Salutation)contactInDb.SalutationId) : string.Empty;
                contact.SalutationIdFutureDisplayValue = contact.SalutationId > 0 ? EnumMapper.GetSaluationDisplayValue((Salutation)contact.SalutationId) : string.Empty;

                //If contact is marked as Inactive then check whether this is the only contact assigned to contact type/s then raise an error
                if (!contact.IsActive)
                {
                    var isOnlyContact = _memberManager.IsOnlyContactAssigned(contact.Id, 0, 0);
                    //SCPIDID : 112924 - Only contact assigned to specific Contact Type marked as Inactive - no error [TC:073575]
                    if (isOnlyContact)
                    {
                        var contactTypeName = GetContactTypeName(contact.Id);
                        details = new UIMessageDetail { IsFailed = true, Message = "Another person needs to be assigned as " + contactTypeName + " before this change can be saved" };
                    }
                    else
                    {
                        _memberManager.RemoveAllContactAssignments(contact.Id);
                        contact = _memberManager.UpdateContact(selectedMemberId, contact);

                        details = new CustomUiMessageDetail
                        {
                            IsFailed = false,

                            isRedirect = false,
                            Id = contact.Id,
                            Value = string.Format("{0} {1}", contact.FirstName, contact.LastName)
                        };

                        // CMP #655: IS-WEB Display per Location ID
                        // Sec: 2.3	NEW CONTACT CREATION USING IS-WEB
                        details.Message = contact.IsContactIsUser
                                              ? "Contact details saved successfully."
                                              : "Contact details saved successfully. This contact has been associated with all Location IDs of his/her organization. Please review associations and modify if required using screen Manage Location Associations.";
                    }
                }
                //If contact status is active but the user is inactive in user profile then raise an error
                else
                {
                    // For a contact who is also an IS user if the Contact status is "Inactive", the status  can be set to "Active" only if the IS User account is "Active" in the IS User Profile
                    var userData = _memberManager.GetUserByEmailId(contact.EmailAddress, selectedMemberId);
                    if ((userData.UserID != 0) && (userData.IsLocked))
                    {
                        details = new UIMessageDetail { IsFailed = true, Message = string.Format(Messages.BMEM_10115), isRedirect = false, Id = contact.Id };
                    }
                    else
                    {
                        contact = _memberManager.UpdateContact(selectedMemberId, contact);
                        details = new CustomUiMessageDetail { IsFailed = false, isRedirect = false, Id = contact.Id, Value = string.Format("{0} {1}", contact.FirstName, contact.LastName) };

                        // CMP #655: IS-WEB Display per Location ID
                        // Sec: 2.3	NEW CONTACT CREATION USING IS-WEB
                        details.Message = contact.IsContactIsUser
                                               ? "Contact details saved successfully."
                                               : "Contact details saved successfully. This contact has been associated with all Location IDs of his/her organization. Please review associations and modify if required using screen Manage Location Associations.";

                    }

                }
            }
            catch (ISBusinessException exception)
            {
                //TODO:Display actual exception message on tab page
                //Raise an exception when there is an error while setting future updates
                if (exception.ErrorCode.Equals(ErrorCodes.NotTheUserOfSelectedMember))
                {
                    details = new UIMessageDetail
                    {
                        IsFailed = true,
                        Message = string.Format(Messages.BMEM_10117),
                        ErrorCode = (exception.ErrorCode == ErrorCodes.NotTheUserOfSelectedMember).ToString()
                    };
                    return Json(details);
                }

                if (exception.ErrorCode.Equals(ErrorCodes.DuplicateEmailIdFound))
                {
                    details = new UIMessageDetail
                    {
                        IsFailed = true,
                        Message = string.Format(Messages.BMEM_10104),
                        ErrorCode = (exception.ErrorCode == ErrorCodes.DuplicateEmailIdFound).ToString()
                    };
                    return Json(details);
                }

                details = new UIMessageDetail
                {
                    IsFailed = true,
                    Message = exception.ErrorCode == ErrorCodes.FutureUpdateErrorEBilling ? GetDisplayMessageWithErrorCode(exception.ErrorCode) : "Error",
                    ErrorCode = exception.ErrorCode == ErrorCodes.FutureUpdateErrorEBilling ? ErrorCodes.FutureUpdateErrorEBilling : string.Empty
                };
            }
            return Json(details);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FetchMemberDetails(Member member)
        {
            if (_authorizationManager.IsAuthorized(_userId, Business.Security.Permissions.Profile.CreateOrManageMemberAccess))
            {
                return RedirectToAction("ManageMember", new { memberId = member.Id });
            }

            return RedirectToAction("Manage", new { memberId = member.Id });
        }

        /// <summary>
        /// Get Member Location Details
        /// </summary>
        [HttpPost]
        public JsonResult GetMemberLocationDetails(int locationId)
        {
            var memberLocationDetails = _memberManager.GetMemberLocationDetails(locationId, true);

            if (SessionUtil.MemberId > 0 && memberLocationDetails.MemberId != SessionUtil.MemberId)
            {
                memberLocationDetails = new Location();
            }

            return Json(memberLocationDetails);
        }

        /// <summary>
        /// This method gets list of members and returns matching list of members only
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetMemberList(string searchText)
        {
            searchText = searchText.ToUpper();

            var selectedList = _memberManager.GetMemberListFromDB().Where(member => string.Format("{0}-{1}-{2}", member.MemberCodeAlpha, member.MemberCodeNumeric, member.CommercialName).Contains(searchText));

            return Json(selectedList.ToList());
        }

        /// <summary>
        /// To fetch data for contacts and display it in the grid
        /// </summary>
        /// <returns></returns>
        public JsonResult ContactsData(string firstName, string lastName, string emailAddress, string staffId, int selectedMemberId = 0)
        {
            try
            {
                if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
                {
                    throw new ISBusinessException("Unauthorized access.");
                }
                //Create grid instance and retrieve data from database
                var contactsGrid = new Contacts("ContactsGrid", Url.Action("ContactsData", "Member", new { firstName, lastName, emailAddress, staffId, selectedMemberId }));
                var contacts = _memberManager.GetMemberContacts(selectedMemberId, firstName, lastName, emailAddress, staffId, (int)SessionUtil.UserCategory);


                return contactsGrid.DataBind(contacts.AsQueryable());
            }
            catch (ISBusinessException be)
            {
                ViewData["errorMessage"] = be.ErrorCode;
                return null;
            }
        }

        [HttpPost]
        public JsonResult GetContactDetails(string contactId)
        {
            var contactDetails = _memberManager.GetContactDetails(int.Parse(contactId));

            if (SessionUtil.MemberId > 0 && contactDetails.MemberId != SessionUtil.MemberId)
            {
                contactDetails = new Contact();
            }

            return Json(contactDetails);
        }


        public ActionResult Copycontacts(int selectedMemberId = 0)
        {
            Contact c = new Contact();
            c.MemberId = selectedMemberId;
            return PartialView("Copycontacts", c);
        }

        [HttpPost]
        public JsonResult Copycontacts(string oldcontactId, string newcontactId)
        {
            var result = _memberManager.CopyContacts(int.Parse(oldcontactId), int.Parse(newcontactId));
            return Json(result);
        }


        public ActionResult Replacecontacts(int selectedMemberId = 0)
        {
            Contact c = new Contact();
            c.MemberId = selectedMemberId;
            return PartialView("ReplaceContacts", c);
        }

        [HttpPost]
        public JsonResult ReplaceContacts(string oldcontactId, string newcontactId)
        {
            var result = _memberManager.ReplaceContacts(int.Parse(oldcontactId), int.Parse(newcontactId));
            return Json(result);

        }

        /// <summary>
        /// Save all contact assignments.
        /// </summary>
        [HttpPost]
        public JsonResult SaveAllContactAssignment(string contactList, string ichContactTypes)
        {
            var result = string.Empty;
            var details = new UIMessageDetail();
            try
            {
                result = _memberManager.UpdateContactContactTypeMatrix(contactList, ichContactTypes);
                if (!string.IsNullOrEmpty(result))
                {
                    var contactTypeName = GetContactTypeName(result);

                    details = new UIMessageDetail { IsFailed = true, Message = "At least one contact needs to be assigned for the following Contact types:\n " + contactTypeName };

                }
                else
                {
                    details = new UIMessageDetail { IsFailed = false, Message = "Contact assignment saved successfully" };
                }
            }
            catch (Exception)
            {
                details = new UIMessageDetail { IsFailed = true, Message = "Contact assignment Failed" };
            }
            return Json(details);
        }

        // TODO : Hard coded values needs to be removed later
        /// <summary>
        /// Email send functionality
        /// 
        /// </summary>
        /// <returns></returns>
        //SCP 000000  : Handled Elmah Issue "The parameters dictionary contains a null entry for parameter 'id' of non-nullable type 'System.Boolean' for method 'System.Web.Mvc.JsonResult MailSender(Boolean, Int32)' in 'Iata.IS.Web.Areas.Profile.Controllers.MemberController'."
        //Description : Incorrect parameters were passed to this action method. Only use of this method is to send email after creation of new member.
        //As a fix parameters were corrected and appropriately passed from caller.
        //Earlier definitaion: public JsonResult MailSender(bool id, int selectedMemberId = 0)
        public JsonResult MailSender(int selectedMemberId = 0)
        {
            var result = true;

            //if (!id)
            if (selectedMemberId > 0)
            {
                result = SendEmail(selectedMemberId);
            }
            else
            {
                SessionUtil.IsEmailToSend = true;
            }

            return Json(result);
        }

        private bool SendEmail(int selectedMemberId)
        {
            var member = _memberManager.GetMember(selectedMemberId);
            var isIchEmailSent =
              _memberManager.SendMemberCreationMailToIchAchOps(
                member.MemberCodeAlpha + "-" + member.MemberCodeNumeric + "-" + member.CommercialName, "ICH");
            var isAchEmailSent =
              _memberManager.SendMemberCreationMailToIchAchOps(
                member.MemberCodeAlpha + "-" + member.MemberCodeNumeric + "-" + member.CommercialName, "ACH");

            return true;
        }

        /// <summary>
        /// Member Controls
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [OutputCache(CacheProfile = "donotCache")]
        public ActionResult MemberControl(int selectedMemberId = 0)
        {
            Member member = _memberManager.GetMember(selectedMemberId, true);

            member.UserCategory = _userCategory;

            return View(member);
        }

        /// <summary>
        /// Member Controls fields update functionality
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult MemberControl(Member member)
        {
            UIMessageDetail details;
            bool isIchZoneMismatch = false;
            const string ichZoneMismatchMessage = "Warning: The ICH zone of this Member is not the same as the ICH zone of the Parent Member";
            try
            {
                // when no merger information 
                if ((member.IsMergedFutureValue == null || member.IsMergedFutureValue == false) &&
                    (member.ParentMemberId == 0) && (member.ParentMemberIdFutureValue == null || member.ParentMemberIdFutureValue == 0) &&
                    string.IsNullOrWhiteSpace(member.ActualMergerDate) &&
                    string.IsNullOrWhiteSpace(member.ActualMergerDateFutureValue))
                {
                    member.ParentMemberIdFutureDisplayValue = null;
                    member.ParentMemberIdDisplayValue = null;
                    member.ParentMemberIdFutureValue = null;
                    member.ActualMergerDate = null;
                    member.ActualMergerDateFutureValue = null;

                    member.ParentMemberIdFuturePeriod = member.IsMergedFuturePeriod;
                    member.ActualMergerDateFuturePeriod = member.IsMergedFuturePeriod;

                    member = _memberManager.UpdateMemberControl(member.Id, member);
                    details = new UIMessageDetail { IsFailed = false, Message = "SIS Ops details Saved successfully.", isRedirect = false };

                }
                // when first time merger info updated
                else if (member.IsMergedFutureValue != null && member.IsMergedFutureValue == true &&
                         (member.ParentMemberId == 0) &&
                         member.ParentMemberIdFutureValue != null && member.ParentMemberIdFutureValue != 0 &&
                          string.IsNullOrWhiteSpace(member.ActualMergerDate) &&
                          !string.IsNullOrWhiteSpace(member.ActualMergerDateFutureValue))
                {
                    if (!member.Id.ToString().Equals(member.ParentMemberIdFutureValue))
                    {
                        //Show warning message if there is any ICH Zone mismatch
                        var selectedMemberIchInfo = _memberManager.GetIchConfig(member.Id);
                        var parentMemberIchInfo = _memberManager.GetIchConfig(member.ParentMemberIdFutureValue.HasValue ? member.ParentMemberIdFutureValue.Value : 0);
                        if (selectedMemberIchInfo != null && parentMemberIchInfo != null)
                        {
                            if (selectedMemberIchInfo.IchZoneId != parentMemberIchInfo.IchZoneId)
                            {
                                isIchZoneMismatch = true;
                            }
                        }

                        member.ParentMemberIdFuturePeriod = member.IsMergedFuturePeriod;
                        member.ActualMergerDateFuturePeriod = member.IsMergedFuturePeriod;

                        member = _memberManager.UpdateMemberControl(member.Id, member);
                        if (isIchZoneMismatch)
                        {
                            details = new UIMessageDetail
                            {
                                IsFailed = false,
                                Message = "SIS Ops details Saved successfully.",
                                isRedirect = false,
                                IsAlert = true,
                                AlertMessage = ichZoneMismatchMessage
                            };
                        }
                        else
                        {
                            details = new UIMessageDetail
                            {
                                IsFailed = false,
                                Message = "SIS Ops details Saved successfully.",
                                isRedirect = false
                            };
                        }

                    }
                    else
                    {
                        details = new UIMessageDetail
                        {
                            IsFailed = true,
                            Message = "Merging Member and Parent Member can not be same!",
                            isRedirect = false
                        };
                    }
                }

                //when merger information updated
                else if (member.IsMergedFutureValue != null && member.IsMergedFutureValue == true &&
                        (member.ParentMemberId != 0) &&
                         member.ParentMemberIdFutureValue != null && member.ParentMemberIdFutureValue != 0 &&
                          !string.IsNullOrWhiteSpace(member.ActualMergerDate) &&
                          !string.IsNullOrWhiteSpace(member.ActualMergerDateFutureValue))
                {
                    if (!member.Id.ToString().Equals(member.ParentMemberIdFutureValue))
                    {
                        //Show warning message if there is any ICH Zone mismatch
                        var selectedMemberIchInfo = _memberManager.GetIchConfig(member.Id);
                        var parentMemberIchInfo = _memberManager.GetIchConfig(member.ParentMemberIdFutureValue.HasValue ? member.ParentMemberIdFutureValue.Value : 0);
                        if (selectedMemberIchInfo != null && parentMemberIchInfo != null)
                        {
                            if (selectedMemberIchInfo.IchZoneId != parentMemberIchInfo.IchZoneId)
                            {
                                isIchZoneMismatch = true;
                            }
                        }

                        member.ParentMemberIdFuturePeriod = member.IsMergedFuturePeriod;
                        member.ActualMergerDateFuturePeriod = member.IsMergedFuturePeriod;
                        member = _memberManager.UpdateMemberControl(member.Id, member);
                        if (isIchZoneMismatch)
                        {
                            details = new UIMessageDetail
                            {
                                IsFailed = false,
                                Message = "SIS Ops details Saved successfully.",
                                isRedirect = false,
                                IsAlert = true,
                                AlertMessage = ichZoneMismatchMessage
                            };
                        }
                        else
                        {
                            details = new UIMessageDetail
                            {
                                IsFailed = false,
                                Message = "SIS Ops details Saved successfully.",
                                isRedirect = false
                            };
                        }

                    }
                    else
                    {
                        details = new UIMessageDetail
                        {
                            IsFailed = true,
                            Message = "Merging Member and Parent Member can not be same!",
                            isRedirect = false
                        };
                    }
                }
                //When Merger information removed or No Change in alreday not present in "Future Merger Info"
                else if ((member.IsMergedFutureValue == null || member.IsMergedFutureValue == false) &&
                         (member.ParentMemberId != 0) && (member.ParentMemberIdFutureValue == null || member.ParentMemberIdFutureValue == 0) &&
                          !string.IsNullOrWhiteSpace(member.ActualMergerDate) &&
                          string.IsNullOrWhiteSpace(member.ActualMergerDateFutureValue))
                {
                    //If No Change in alreday not present in "Future Merger Info"
                    if (member.IsMergedFutureValue == null && member.IsMergedFuturePeriod == null)
                    {
                        member.ParentMemberIdFutureValue = null;
                        member.ParentMemberIdFutureDisplayValue = null;
                        member.ActualMergerDateFutureValue = null;
                        member.ParentMemberIdFuturePeriod = member.IsMergedFuturePeriod;
                        member.ActualMergerDateFuturePeriod = member.IsMergedFuturePeriod;
                    }
                    //If Merger Information Removed
                    else
                    {
                        member.IsMergedFutureValue = false;
                        member.ParentMemberIdFutureDisplayValue = null;
                        member.ParentMemberIdFutureValue = 0;
                        member.ActualMergerDateFutureValue = null;
                        member.ParentMemberIdFuturePeriod = member.IsMergedFuturePeriod;
                        member.ActualMergerDateFuturePeriod = member.IsMergedFuturePeriod;
                    }

                    member = _memberManager.UpdateMemberControl(member.Id, member);
                    details = new UIMessageDetail { IsFailed = false, Message = "SIS Ops details Saved successfully.", isRedirect = false };

                }
                else
                {
                    details = new UIMessageDetail { IsFailed = true, Message = ShowMessageText("All merger information are required!") };
                }

            }
            catch (ISBusinessException exception)
            {
                details = new UIMessageDetail { IsFailed = true, Message = ShowMessageText(exception.ErrorCode) };
            }

            return Json(details);
        }


        [HttpPost]
        public JsonResult DeleteContact(string id, int selectedMemberId = 0)
        {
            UIExceptionDetail details;
            try
            {
                if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
                {
                    throw new ISBusinessException(null);
                }

                var isOnlyContact = _memberManager.IsOnlyContactAssigned(int.Parse(id), 0, 0);
                //SCPIDID : 112924 - Only contact assigned to specific Contact Type marked as Inactive - no error [TC:073575]
                if (isOnlyContact)
                {
                    var contactTypeName = GetContactTypeName(int.Parse(id));
                    details = new UIExceptionDetail { IsFailed = true, Message = "Another person needs to be assigned as " + contactTypeName + "  before this change can be saved" };
                }
                else
                {
                    var isDeleted = _memberManager.DeleteContact(int.Parse(id), selectedMemberId);
                    details = isDeleted
                                ? new UIExceptionDetail { IsFailed = false, Message = string.Format(Messages.BMEM_10105) }
                                : new UIExceptionDetail { IsFailed = true, Message = string.Format(Messages.BMEM_10106) };
                }
            }
            catch (ISBusinessException)
            {
                details = new UIExceptionDetail { IsFailed = true, Message = string.Format(Messages.InvoiceDeleteFailed) };
            }
            return Json(details);
        }

        [HttpPost]
        public JsonResult GetUserByEmailId(string emailId, int selectedMemberId = 0)
        {
            try
            {
                if (selectedMemberId > 0 && SessionUtil.MemberId > 0 && selectedMemberId != SessionUtil.MemberId)
                {
                    throw new ISBusinessException(null);
                }

                var result = _memberManager.GetUserByEmailId(emailId, selectedMemberId);
                if (result.UserID == 0)
                {
                    return null;
                }
                return Json(result);

            }
            catch (ISBusinessException)
            {
                var details = new UIExceptionDetail { IsFailed = true, Message = string.Format(Messages.BMEM_10117) };
                return Json(details);
            }
        }

        [HttpPost]
        public JsonResult GetUserCityNameAndSubDivisionName(int cityId, string subDivisionId)
        {
            var result = _memberManager.GetUserCityNameAndSubDivisionName(cityId, subDivisionId);
            return Json(result);
        }

        /// <summary>
        /// Function to retrieve Ich/Ach Member status History
        /// </summary>
        /// <param name="memberType"></param>
        /// <returns></returns>
        public JsonResult GetIchMemberHistory(string memberType, int selectedMemberId = 0)
        {
            var memberStatusDetailses = _memberManager.GetMemberStatus(selectedMemberId, memberType);
            var table = new DataTable();
            var statusColumn = new DataColumn("MembershipStatus", typeof(string)) { Caption = "Membership Status" };
            table.Columns.Add(statusColumn);
            var dateColumn = new DataColumn("StatusChangeDate", typeof(string)) { Caption = "Status Change Date" };
            table.Columns.Add(dateColumn);

            foreach (var memberStatusDetails in memberStatusDetailses)
            {
                var dataRow = table.NewRow();
                dataRow["MembershipStatus"] = memberStatusDetails.DisplayIchMembershipStatus;
                dataRow["StatusChangeDate"] = memberStatusDetails.StatusChangeDate.ToString(FormatConstants.DateFormat);
                table.Rows.Add(dataRow);
            }
            string jsonString = table.ToJsonString();
            return Json(jsonString);
        }

        /// <summary>
        /// Function to retrive member status history.
        /// </summary>
        /// <param name="memberType"></param>
        /// <returns></returns>
        public JsonResult GetMemberHistory(string memberType, int selectedMemberId = 0)
        {
            var memberStatusDetailses = _memberManager.GetMemberStatus(selectedMemberId, memberType);
            var table = new DataTable();
            var statusColumn = new DataColumn("MembershipStatus", typeof(string)) { Caption = "Membership Status" };
            table.Columns.Add(statusColumn);
            var dateColumn = new DataColumn("StatusChangeDate", typeof(string)) { Caption = "Status Change Date" };
            table.Columns.Add(dateColumn);

            foreach (var memberStatusDetails in memberStatusDetailses)
            {
                var dataRow = table.NewRow();
                dataRow["MembershipStatus"] = memberStatusDetails.MembershipStatus;
                dataRow["StatusChangeDate"] = memberStatusDetails.StatusChangeDate.ToString(FormatConstants.DateFormat);
                table.Rows.Add(dataRow);
            }
            string jsonString = table.ToJsonString();
            return Json(jsonString);
        }

        public JsonResult GetMyGridDataJson(string contactTypeCategory, string groupId, string subGroupId, string typeId, string columns, int selectedMemberId = 0)
        {
            var searchCriteria = new ContactAssignmentSearchCriteria { Columns = columns, ContactTypeCategory = contactTypeCategory, GroupId = groupId, SubGroupId = subGroupId, TypeId = typeId, PageNumber = 0, PageSize = 1, MemberId = selectedMemberId };

            TempData["SearchCriteria"] = searchCriteria;

            string sidx = "Contact_name", sord = "asc";
            int recordCount;
            searchCriteria.PageNumber = 0;
            searchCriteria.PageSize = 10;

            return Json(JsonHelper.JsonForJqgridColumns(GetMyDataTable(sidx, sord, searchCriteria, out recordCount, true, selectedMemberId), 1, recordCount, 0, (int)SessionUtil.UserCategory), "application/json");
        }

        public ActionResult GetMyGridData(string sidx, string sord, int? page, int rows)
        {
            if (TempData["SearchCriteria"] != null)
            {
                var searchCriteria = (ContactAssignmentSearchCriteria)TempData["SearchCriteria"];

                //Restore the search criteria for future use.
                TempData["SearchCriteria"] = searchCriteria;
                int totalRecordCount;

                //SCP0000: Elmah Exceptions log removal
                searchCriteria.PageNumber = page != null ? (int)page : 1;

                searchCriteria.PageSize = rows;
                if (!sidx.Equals("FIRST_NAME"))
                    if (sord.Equals("asc"))
                        sord = "desc";
                    else if (sord.Equals("desc"))
                        sord = "asc";
                if (page != null)
                    return Content(JsonHelper.JsonForJqgrid(GetMyDataTable(sidx, sord, searchCriteria, out totalRecordCount, false, searchCriteria.MemberId), rows, totalRecordCount, (int)page), "application/json");
            }
            // TODO: handle else condition if TempData["SearchCriteria"] is null
            if (page != null)
                return Content(JsonHelper.JsonForJqgrid(new DataTable(), rows, 0, (int)page), "application/json");
            return null;
        }

        public DataTable GetMyDataTable(string sidx, string sord, ContactAssignmentSearchCriteria searchCriteria, out int recordCount, bool isSearch, int selectedMemberId)
        {
            var result = new DataTable();
            recordCount = 0;
            try
            {
                if (isSearch)
                {
                    DataTable dt = _memberManager.GetContactAssignmentData(searchCriteria, selectedMemberId,
                                                                     (int)SessionUtil.UserCategory, out recordCount);
                    DataView dataView = new DataView(dt);
                    if (!String.IsNullOrEmpty(searchCriteria.ContactTypeCategory))
                        dataView.Sort = dt.Columns[2].ColumnName + " " + "desc";
                    result = dataView.ToTable();

                }
                else
                {

                    DataTable dataTable = _memberManager.GetContactAssignmentData(searchCriteria, selectedMemberId,
                                                                       (int)SessionUtil.UserCategory, out recordCount);

                    DataView dataView = new DataView(dataTable);
                    if (!String.IsNullOrEmpty(sidx) && !String.IsNullOrEmpty(sord))
                        dataView.Sort = sidx + " " + sord;
                    else
                    {
                        if (!String.IsNullOrEmpty(searchCriteria.ContactTypeCategory))

                            dataView.Sort = dataTable.Columns[2].ColumnName + " " + "desc";
                    }
                    var sortedDataTable = dataView.ToTable();
                    result = sortedDataTable.Clone();
                    var startRowIndex = (searchCriteria.PageNumber - 1) * (searchCriteria.PageSize);
                    var endRowIndex = searchCriteria.PageNumber * searchCriteria.PageSize;
                    endRowIndex = dataTable.Rows.Count - 1;
                    for (int i = startRowIndex; i <= endRowIndex; i++)
                    {

                        result.ImportRow(sortedDataTable.Rows[i]);
                    }

                    recordCount = dataTable.Rows.Count;
                }
            }
            catch (Exception exception)
            {
                _logger.Error("Error in GetMyDataTable.", exception);

                // TODO: handle if exception occurs.
            }
            return result;
        }



        public JsonResult GetMemberLocationList(int selectedMemberId = 0)
        {
            var locationList = _memberManager.GetMemberLocationList(selectedMemberId);
            return Json(locationList);
        }

        private string GetContactTypeName(int contactId)
        {
            var returnType = string.Empty;
            var contactList = _memberManager.GetRequiredTypeByContactId(contactId);
            if (contactList != null)
            {
                foreach (var requiredContactType in contactList)
                {
                    returnType += requiredContactType.ContactTypeText + ",";
                }
                //SCPIDID : 112924 - Only contact assigned to specific Contact Type marked as Inactive - no error [TC:073575]
                if (returnType.Length > 1)
                    returnType = returnType.Remove(returnType.Length - 1, 1);
            }

            return returnType;

        }

        private string GetContactTypeName(string contactIdList)
        {
            var returnType = string.Empty;
            try
            {
                //Get  missing required Contact Type Ids
                string[] contactTypeIdArray = contactIdList.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                foreach (var contactTypeId in contactTypeIdArray)
                {
                    var contactTypeName = _memberManager.GetContactTypeNameById(int.Parse(contactTypeId));
                    if (!string.IsNullOrEmpty(contactTypeName))
                    {
                        returnType += contactTypeName + "\n";
                    }
                }
                returnType = returnType.Remove(returnType.Length - 1, 1);
            }
            catch (Exception exception)
            {

            }
            return returnType;

        }

        /// <summary>
        /// Check Ebilling field value has been changed or not. 
        /// </summary>
        /// <param name="eBilling">Ebilling update value from UI.</param>
        /// <returns></returns>
        private static bool CheckEbillingFieldValueStatus(EBillingConfiguration eBilling)
        {
            //return false: if any below ebilling value is true, return true if all below ebilling value is false.
            return !(eBilling.IncludeListingsCgoPayArch
                    || eBilling.IncludeListingsCgoRecArch
                    || eBilling.IncludeListingsMiscPayArch
                    || eBilling.IncludeListingsMiscRecArch
                    || eBilling.IncludeListingsPaxPayArch
                    || eBilling.IncludeListingsPaxRecArch
                    || eBilling.IncludeListingsUatpPayArch
                    || eBilling.IncludeListingsUatpRecArch
                    || eBilling.LegalArchRequiredforPaxRecInv
                    || eBilling.LegalArchRequiredforPaxPayInv
                    || eBilling.LegalArchRequiredforCgoRecInv
                    || eBilling.LegalArchRequiredforCgoPayInv
                    || eBilling.LegalArchRequiredforMiscRecInv
                    || eBilling.LegalArchRequiredforMiscPayInv
                    || eBilling.LegalArchRequiredforUatpRecInv
                    || eBilling.LegalArchRequiredforUatpPayInv
                    || (eBilling.LegalArchRequiredforPaxPayInvFutureValue == true)
                    || (eBilling.LegalArchRequiredforPaxRecInvFutureValue == true)
                    || (eBilling.LegalArchRequiredforCgoRecInvFutureValue == true)
                    || (eBilling.LegalArchRequiredforCgoPayInvFutureValue == true)
                    || (eBilling.LegalArchRequiredforMiscRecInvFutureValue == true)
                    || (eBilling.LegalArchRequiredforMiscPayInvFutureValue == true)
                    || (eBilling.LegalArchRequiredforUatpRecInvFutureValue == true)
                    || (eBilling.LegalArchRequiredforUatpPayInvFutureValue == true));
        }
    }
}
