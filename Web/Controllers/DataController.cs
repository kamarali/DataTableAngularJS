using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Iata.IS.Business.Cargo;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.MiscUatp;
using Iata.IS.Business.Pax;
using Iata.IS.Business.Reports;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.MiscUatp.Enums;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Web.UIModel;
using Iata.IS.Web.UIModel.ErrorDetail;
using Iata.IS.Web.Util;
using Iata.IS.Web.Util.Filters;
using UserCategory = Iata.IS.Model.MemberProfile.Enums.UserCategory;
using Iata.IS.Core.DI;
using Iata.IS.Business.Common;
using Iata.IS.Model.SupportingDocuments.Enums;
using iPayables.UserManagement;
using iPayables;
using log4net;
using Group = Iata.IS.Web.Util.DynamicFields.Group;
using TransactionStatus = Iata.IS.Model.Pax.Enums.TransactionStatus;
using TransactionType = Iata.IS.Model.Enums.TransactionType;

namespace Iata.IS.Web.Controllers
{
  [Authorize]
  [LogActions]
  [ElmahHandleError]
  public class DataController : Controller
  {
    private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private readonly IMemberManager _memberManager;
    private readonly IReferenceManager _referenceManager;
    private readonly IUserManagement _iUserManagement;
    private readonly IProcessingDashboardManager _processingDashboardManager;

    public IInvoiceManager InvoiceManager { get; set; }
    public ICargoInvoiceManager CargoInvoiceManager { get; set; }
    public IMiscInvoiceManager MiscInvoiceManager { get; set; }
    public ISamplingFormFManager SamplingFormFManager { get; set; }
    public ISamplingFormXfManager SamplingFormXfManager { get; set; }
    public INonSamplingInvoiceManager NonSamplingInvoiceManager { get; set; }


    public DataController(IMemberManager memberManager, IReferenceManager referenceManager, IUserManagement iUserManagement, IProcessingDashboardManager processingDashboardManager)
    {
      _memberManager = memberManager;
      _referenceManager = referenceManager;
      _iUserManagement = iUserManagement;
      _processingDashboardManager = processingDashboardManager;
    }

    /// <summary>
    /// This method gets list of members, skiping login member, and returns matching list of members only
    /// </summary>
    /// <param name="q"></param>
    [OutputCache(Duration = 1200, VaryByParam = "q")]
    public ContentResult GetMemberList(string q)
    {
      return GetMemberListData(q, memberToSkip: SessionUtil.MemberId, includeTerminatedMember: true, excludeTypeBMember: false);
    }

    /// <summary>
    /// This method gets list of All members and returns matching list of members only
    /// </summary>
    /// <param name="q"></param>
    [OutputCache(Duration = 1200, VaryByParam = "q")]
    public ContentResult GetMemberListForPaxCgoContainsLoginMember(string q)
    {
      return GetMemberListData(q, memberToSkip: 0, includeTerminatedMember: true, excludeTypeBMember: true);
    }

    /// <summary>
    /// GetMemberList Data
    /// SCP368582 - User Access Problem
    /// </summary>
    /// <param name="q"></param>
    /// <param name="memberToSkip"></param>
    /// <param name="includeTerminatedMember"></param>
    /// <param name="excludeTypeBMember"></param>
    /// <returns></returns>
    private ContentResult GetMemberListData(string q, int memberToSkip, bool includeTerminatedMember, bool excludeTypeBMember)
    {
      var response = _memberManager.GetMemberListForUI(q, memberToSkip, includeTerminated: includeTerminatedMember, excludeTypeBMembers: excludeTypeBMember);

      var splitedResponse = response.Split('\n');

      var allMembers = new StringBuilder();

      if (splitedResponse.Length > 0)
      {
        foreach (var t in splitedResponse.Where(t => !string.IsNullOrEmpty(t)))
        {
          allMembers.AppendFormat("{0}", string.Format("{0}{1}", t, '\n'));
        }
      }

      if (!string.IsNullOrEmpty(q) && allMembers.Length == 0)
      {
        var singleMember = q.Split('-');
        if (singleMember.Length > 2)
        {
          var responsesingleMember = _memberManager.GetMemberListForUI(singleMember[1], includeTerminated: includeTerminatedMember, memberIdToSkip: memberToSkip, excludeTypeBMembers: excludeTypeBMember);

          if (responsesingleMember.Length > 0)
          {
            allMembers.AppendFormat("{0}", string.Format("{0}{1}", responsesingleMember, '\n'));
          }
        }
      }

      //return Content(allMembers.ToString());
      var returnList = allMembers.ToString();
      return new SanitizeResult(string.IsNullOrEmpty(returnList) ? "NoItemFound" : returnList);
    }

    /// <summary>
    /// This method gets list of members and returns matching list of members only Which are not merged with any member
    /// </summary>
    /// <param name="q"></param>
    /// <param name="selectedMemberId"></param>
    [OutputCache(Duration = 1200, VaryByParam = "q")]
    public ContentResult GetNonMergedMemberList(string q, int selectedMemberId=0)
    {
        var response = _memberManager.GetMemberListForUI(q, selectedMemberId, includePending: false, includeBasic: false, includeRestricted: false, includeTerminated: false, excludeMergedMember: true);

        var splitedResponse = response.Split('\n');

        var allMembers = new StringBuilder();

        if (splitedResponse.Length > 0)
        {
            foreach (var t in splitedResponse.Where(t => !string.IsNullOrEmpty(t)))
            {
                allMembers.AppendFormat("{0}", string.Format("{0}{1}", t, '\n'));
            }
        }

        if (!string.IsNullOrEmpty(q) && allMembers.Length == 0)
        {
            var singleMember = q.Split('-');
            if (singleMember.Length > 2)
            {
                var responsesingleMember = _memberManager.GetMemberListForUI(singleMember[1], memberIdToSkip: selectedMemberId, includePending: false, includeBasic: false, includeRestricted: false, includeTerminated: false, excludeMergedMember: true);
                
                if (responsesingleMember.Length > 0)
                {
                    allMembers.AppendFormat("{0}", string.Format("{0}{1}", responsesingleMember, '\n'));
                }
            }
        }

        //return Content(allMembers.ToString());
        var returnList = allMembers.ToString();
        return new SanitizeResult(string.IsNullOrEmpty(returnList) ? "NoItemFound" : returnList);
    }
  
      /// <summary>
    /// This method gets list of exception codes specific to Uatp and returns matching list of exception codes only
    /// </summary>
    /// <param name="q"></param>
    [OutputCache(Duration = 1200, VaryByParam = "q")]
    public ContentResult GetUatpExceptionCodeList(string q)
    {
      var response = MiscInvoiceManager.GetExceptionCodeList(q, (int)BillingCategoryType.Uatp);
      return new SanitizeResult(response);
    }

    /// <summary>
    /// This method gets list of exception codes specific to Misc and returns matching list of exception codes only
    /// </summary>
    /// <param name="q"></param>
    [OutputCache(Duration = 1200, VaryByParam = "q")]
    public ContentResult GetMiscExceptionCodeList(string q)
    {
      var response = MiscInvoiceManager.GetExceptionCodeList(q, (int)BillingCategoryType.Misc);
      return new SanitizeResult(response);
    }


    /// <summary>
    /// This method gets list of exception codes specific to Uatp and returns matching list of exception codes only
    /// </summary>
    /// <param name="q"></param>
    [OutputCache(Duration = 1200, VaryByParam = "q")]
    public ContentResult GetCgoExceptionCodeList(string q)
    {
      var response = MiscInvoiceManager.GetExceptionCodeList(q, (int)BillingCategoryType.Cgo);
      return new SanitizeResult(response);
    }

    /// <summary>
    /// This method gets list of exception codes specific to Uatp and returns matching list of exception codes only
    /// </summary>
    /// <param name="q"></param>
    [OutputCache(Duration = 1200, VaryByParam = "q")]
    public ContentResult GetPaxExceptionCodeList(string q)
    {
      var response = MiscInvoiceManager.GetExceptionCodeList(q, (int)BillingCategoryType.Pax);
      return new SanitizeResult(response);
    }

    /// <summary>
    /// This method gets list of members and returns matching list of members only
    /// </summary>
    /// <param name="q"></param>
    /// <param name="dependentValue"></param>
    /// <param name="userCategoryId"></param>
    /// <param name="isFromMemContactRprt"></param>
    [OutputCache(Duration = 1200, VaryByParam = "q;dependentValue;userCategoryId;isFromMemContactRprt")]
    public ContentResult GetAllMemberList(string q, string dependentValue = null, string userCategoryId = null, bool isFromMemContactRprt = false)
    {
      var response = string.Empty;

      if (string.IsNullOrEmpty(dependentValue) || (dependentValue == "4"))
      {
        // SCP186215: Member Code Mismatch between Member and Location Details
        if (!string.IsNullOrEmpty(userCategoryId) && userCategoryId == "4")
        {
          // CMP597: INCLUDE_MEMBER_TYPE = 9 => exclude Members having ‘IS Membership Sub Status’ as “Terminated” (irrespective of ‘IS Membership Status’)
          response = isFromMemContactRprt ? _memberManager.GetMemberListForUI(q, includeTerminated: true, memberIdToSkip: 0, includePending: false, includeMemberType: 9) : 
                                            _memberManager.GetMemberListForUI(q, includeTerminated: true, memberIdToSkip: 0, includePending: false);
        }
        else
        {
         response = _memberManager.GetMemberListForUI(q, includeTerminated: true, memberIdToSkip: 0);
        }
      }

      var splitedResponse = response.Split('\n');

      var allMembers = new StringBuilder();

      if (splitedResponse.Length > 0)
      {
        foreach (var t in splitedResponse.Where(t => !string.IsNullOrEmpty(t)))
        {
          allMembers.AppendFormat("{0}", string.Format("{0}{1}", t, '\n'));
        }
      }

      if (!string.IsNullOrEmpty(q) && allMembers.Length == 0)
      {
        var singleMember = q.Split('-');
        if (singleMember.Length > 2)
        {
          // SCP186215: Member Code Mismatch between Member and Location Details
          var responsesingleMember = string.Empty;
          if (!string.IsNullOrEmpty(userCategoryId) && userCategoryId == "4")
          {
            // CMP597: INCLUDE_MEMBER_TYPE = 9 => exclude Members having ‘IS Membership Sub Status’ as “Terminated” (irrespective of ‘IS Membership Status’)
            responsesingleMember = isFromMemContactRprt ? _memberManager.GetMemberListForUI(singleMember[1], includeTerminated: true, memberIdToSkip: 0, includePending: false, includeMemberType: 9) :
                                                          _memberManager.GetMemberListForUI(singleMember[1], includeTerminated: true, memberIdToSkip: 0, includePending: false);
          }
          else
          {
            responsesingleMember = _memberManager.GetMemberListForUI(singleMember[1], includeTerminated: true, memberIdToSkip: 0);
          }

          if (responsesingleMember.Length > 0)
          {
            allMembers.AppendFormat("{0}", string.Format("{0}{1}", responsesingleMember, '\n'));
          }
        }
      }

      //return Content(allMembers.ToString());
      var returnList = allMembers.ToString();
      return new SanitizeResult(string.IsNullOrEmpty(returnList) ? "NoItemFound" : returnList);
    }

    /// <summary>
    /// This method gets list of members of ich/ach and returns matching list of members only
    /// </summary>
    /// <param name="q"></param>
    /// <param name="menuType"></param>
    /// <returns></returns>
    [OutputCache(Duration = 1200, VaryByParam = "q;menuType")]
    public ContentResult GetMemberListForIchOrAch(string q, string menuType)
    {
      bool includeIch = false;
      bool includeAch = false;
      int includeMemType = 0;

      var category = _iUserManagement.GetUserByUserID(SessionUtil.UserId).CategoryID;

      if (menuType == "onlyAch" && ((category == (int)UserCategory.AchOps) || (category == (int)UserCategory.SisOps)))
      {
          includeAch = true;
          includeMemType = 1;
      }
      else if (menuType == "onlyIch" && (category == (int)UserCategory.IchOps || (category == (int)UserCategory.SisOps)))
      {
          includeIch = true;
          includeMemType = 2;
      }
      else if (menuType == "bothAchIch" && (category == (int)UserCategory.AchOps || (category == (int)UserCategory.SisOps)))
      {
          includeMemType = 3;
      }
      else if (menuType == "ich" && ((category == (int)UserCategory.IchOps) || (category == (int)UserCategory.SisOps)))
      {
          includeIch = true;
      }
      else if (menuType == "ach" && ((category == (int)UserCategory.AchOps) || (category == (int)UserCategory.SisOps)))
      {
          includeAch = true;
      }

      var response = _memberManager.GetMemberListForUI(q, 0, includePending: true, includeBasic: true, includeRestricted: true, includeTerminated: true, includeOnlyAch: includeAch, includeOnlyIch: includeIch, includeMemberType: includeMemType);

      var splitedResponse = response.Split('\n');

      var allMembers = new StringBuilder();

      if (splitedResponse.Length > 0)
      {
        foreach (var t in splitedResponse.Where(t => !string.IsNullOrEmpty(t)))
        {
          allMembers.AppendFormat("{0}", string.Format("{0}{1}", t, '\n'));
        }
      }

      if (!string.IsNullOrEmpty(q) && allMembers.Length == 0)
      {
        var singleMember = q.Split('-');
        if (singleMember.Length > 2)
        {
          //var responsesingleMember = _memberManager.GetMemberListForUI(singleMember[1], includeTerminated: true, memberIdToSkip: 0);
          //SCP# 372434 - Managing Blocks
          var responsesingleMember = _memberManager.GetMemberListForUI(q, 0, includePending: true, includeBasic: true, includeRestricted: true, includeTerminated: true, includeOnlyAch: includeAch, includeOnlyIch: includeIch, includeMemberType: includeMemType);

          if (responsesingleMember.Length > 0)
          {
            allMembers.AppendFormat("{0}", string.Format("{0}{1}", responsesingleMember, '\n'));
          }
        }
      }

      //return Content(allMembers.ToString());
      var returnList = allMembers.ToString();
      return new SanitizeResult(string.IsNullOrEmpty(returnList) ? "NoItemFound" : returnList);
    }

    /// <summary>
    /// This method gets list of members and returns matching list of members only
    /// </summary>
    /// <param name="q"></param>
    /// <param name="dependentValue"></param>
    /// <param name="isBilling"></param>
    public ContentResult GetBVCMemberList(string q, string dependentValue = null, bool isBilling = true)
    {
        //MemberType = 4 means get only BVC members.
        var memberType = 0;

        if(isBilling)
        {
            memberType = 4;
        }

        var response = string.Empty;

        if (string.IsNullOrEmpty(dependentValue) || (dependentValue == "4"))
        {
            response = _memberManager.GetMemberListForUI(q, includeTerminated: false, memberIdToSkip: 0, includeMemberType: memberType,excludeTypeBMembers:true);
        }
        

        var splitedResponse = response.Split('\n');

        var allMembers = new StringBuilder();

        if (splitedResponse.Length > 0)
        {
            foreach (var t in splitedResponse.Where(t => !string.IsNullOrEmpty(t)))
            {
                /* CMP #596: Length of Member Accounting Code to be Increased to 12 
                Desc: Added a check to see if its a type B member, if so need not be a part of auto-complete on UI.
                Ref: FRS Section 3.1 Point 8 Table 6 Rows 3, 4, 5, 6, 7 and 8 */
                if (!GetMemberCodeAndCheckIfItIsTypeB(t))
                {
                    allMembers.AppendFormat("{0}", string.Format("{0}{1}", t, '\n'));
                }
                /* For logical completion 
                else
                {
                     This is a type B member, so do not add this member in auto-complete list, to be sent to UI
                }*/
                //allMembers.AppendFormat("{0}", string.Format("{0}{1}", t, '\n'));
            }
        }

        if (!string.IsNullOrEmpty(q) && allMembers.Length == 0)
        {
            var singleMember = q.Split('-');
            if (singleMember.Length > 2)
            {
                var responsesingleMember = _memberManager.GetMemberListForUI(singleMember[1], includeTerminated: false, memberIdToSkip: 0, includeMemberType: memberType, excludeTypeBMembers: true);

                if (responsesingleMember.Length > 0)
                {
                    allMembers.AppendFormat("{0}", string.Format("{0}{1}", responsesingleMember, '\n'));
                }
            }
        }

         var returnList = allMembers.ToString();
        return new SanitizeResult(string.IsNullOrEmpty(returnList) ? "NoItemFound" : returnList);
    }

    /// <summary>
    /// This method gets list of ich members for given zone id.
    /// </summary>
    /// <param name="q"></param>
    /// <param name="dependentValue"></param>
    [OutputCache(Duration = 1200, VaryByParam = "q;dependentValue")]
    public ContentResult GetIchMemberListForZone(string q, string dependentValue)
    {
      var ichZoneId = 0;
      string response;

      if (!string.IsNullOrEmpty(dependentValue))
      {
        ichZoneId = Convert.ToInt32(dependentValue);
      }

      if (ichZoneId == (int)IchZoneType.C)
      {
        response = _memberManager.GetMemberListForUI(q, 0, includePending: true, includeBasic: true, includeRestricted: true, includeTerminated: true, includeOnlyAch: true, includeOnlyIch: false, ichZone: 0);

      }
      else if (ichZoneId == -1)
      {
        response = _memberManager.GetMemberListForUI(q, SessionUtil.MemberId);
      }
      else
      {
        response = _memberManager.GetMemberListForUI(q, 0, includePending: true, includeBasic: true, includeRestricted: true, includeTerminated: true, includeOnlyAch: false, includeOnlyIch: true, ichZone: ichZoneId);
      }

      var splitedResponse = response.Split('\n');

      var allMembers = new StringBuilder();

      if (splitedResponse.Length > 0)
      {
        foreach (var t in splitedResponse.Where(t => !string.IsNullOrEmpty(t)))
        {
          allMembers.AppendFormat("{0}", string.Format("{0}{1}", t, '\n'));
        }
      }

      if (!string.IsNullOrEmpty(q) && allMembers.Length == 0)
      {
        var singleMember = q.Split('-');
        if (singleMember.Length > 2)
        {
          var responsesingleMember = _memberManager.GetMemberListForUI(singleMember[1], includeTerminated: true, memberIdToSkip: 0);

          if (responsesingleMember.Length > 0)
          {
            allMembers.AppendFormat("{0}", string.Format("{0}{1}", responsesingleMember, '\n'));
          }
        }
      }

      //return Content(allMembers.ToString());
      var returnList = allMembers.ToString();
      return new SanitizeResult(string.IsNullOrEmpty(returnList) ? "NoItemFound" : returnList);
    }

    [HttpPost]
    [OutputCache(Duration = 1200, VaryByParam = "billedMemberId")]
    public JsonResult GetBilledMemberLocationList(int billedMemberId)
    {
      var memberLocationList = _memberManager.GetMemberLocationDetailsForDropdown(billedMemberId, true).Select(loc => new LocationCodeDetails { CityName = loc.CityName, CurrencyCode = loc.Currency == null ? string.Empty : loc.Currency.Code, CountryId = loc.CountryId, LocationCode = loc.LocationCode });

      //SCP # 48139 - Member Location ID incorrect order 
      if (memberLocationList.Count() > 0)
      {
        var integerMemberLocationList = memberLocationList.ToList();
        integerMemberLocationList.Clear();
        var stringMemberLocationList = memberLocationList.ToList();
        stringMemberLocationList.Clear();

        foreach (var mll in memberLocationList.ToList())
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
        return Json(stringMemberLocationList.ToList());
      }
      
      return Json(memberLocationList.ToList());
    }

    [HttpPost]
    public JsonResult GetMiscUatpBilledMemberLocationList(int? billedMemberId, int billingCategory)
    {
        //SCP0000: Elmah Exceptions log removal
        MiscBilledMemberLocation billedMemberLocation = null;

        if (billedMemberId != null)
        {
            billedMemberLocation = new MiscBilledMemberLocation
                                    {
                                      BilledLocations = _memberManager.GetMemberLocationList((int)billedMemberId).Select(
                                              loc => new LocationCodeDetails
                                                       {
                                                        CityName = loc.CityName,
                                                        LocationCode = loc.LocationCode,
                                                        CountryId = loc.CountryId,
                                                        CurrencyCode = loc.Currency == null ? string.Empty : loc.Currency.Code
                                                        }).ToList(),
                                      DefaultLocation = "Main"
                                    };
        }
        if (billedMemberLocation != null)
        {
            if (billingCategory == (int)BillingCategoryType.Uatp && billedMemberLocation.BilledLocations.Any(loc => loc.LocationCode == "UATP"))
            {
             billedMemberLocation.DefaultLocation = "UATP";
            }

            return Json(billedMemberLocation);
        }
        return null;
    }

    /// <summary>
    /// Gets the charge code type list.
    /// </summary>
    /// <param name="chargeCodeId">The charge code id.</param>
    [HttpPost]
    public JsonResult GetChargeCodeTypeList(int? chargeCodeId)
    {
       //SCP0000: Elmah Exceptions log removal
       IList<ChargeCodeType> chargeCodeTypeList = null;
       if (chargeCodeId != null)
         chargeCodeTypeList = _referenceManager.GetChargeCodeType((int)chargeCodeId, true);

       if (chargeCodeTypeList != null)
         return Json(chargeCodeTypeList.ToList());

      return null;
    }

    /// <summary>
    /// Gets the charge code type detail.
    /// </summary>
    /// <param name="chargeCodeId">The charge code id.</param>
    //CMP #636: Standard Update Mobilization
    [HttpPost]
    public JsonResult GetChargeCodeDetail(int chargeCodeId)
    {
      var chargeCodeDetail = _referenceManager.GetChargeCodeDetail(chargeCodeId);
      return Json(new { chargeCodeTypeReq = chargeCodeDetail.IsChargeCodeTypeRequired, isActiveChargeCodeType = chargeCodeDetail.IsActiveChargeCodeType });
    }

    /// <summary>
    /// Get Supporting document billing year month dropdown values as per supporting document type
    /// </summary>
    [HttpPost]
    public JsonResult GetSupportingDocBillingYearMonth(int supportingDocumentTypeId)
    {
      var bilingMonthSelectList = new List<SelectListItem>();

      var currentBillingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);  // GetCurrentBillingPeriod();

      // Convert DateTime format of CurrentBillingPeriod
      var currentBillingInDateTimeFormat = new DateTime(currentBillingPeriod.Year, currentBillingPeriod.Month, currentBillingPeriod.StartDate.Day);

      if (supportingDocumentTypeId == (int)SupportingDocType.InvoiceCreditNote)
      {
        for (var i = 0; i < 2; i++)
        {
          var strmonth = currentBillingInDateTimeFormat.AddMonths(-i).ToString(FormatConstants.MonthNameFormat);
          var stryear = currentBillingInDateTimeFormat.AddMonths(-i).Year.ToString();
          var monthValue = currentBillingInDateTimeFormat.AddMonths(-i).Month.ToString();

          bilingMonthSelectList.Add(new SelectListItem { Text = string.Format("{0}-{1}", stryear, strmonth), Value = string.Format("{0}-{1}", stryear, monthValue) });
        }
      }
      else if (supportingDocumentTypeId == (int)SupportingDocType.FormC)
      {
        for (var i = 0; i < 5; i++)
        {
          var strmonth = currentBillingInDateTimeFormat.AddMonths(-i).ToString(FormatConstants.MonthNameFormat);
          var stryear = currentBillingInDateTimeFormat.AddMonths(-i).Year.ToString();
          var monthValue = currentBillingInDateTimeFormat.AddMonths(-i).Month.ToString();

          bilingMonthSelectList.Add(new SelectListItem { Text = string.Format("{0}-{1}", stryear, strmonth), Value = string.Format("{0}-{1}", stryear, monthValue) });
        }
      }
      bilingMonthSelectList.Insert(0, new SelectListItem
      {
        Value = string.Empty,
        Text = "Please Select"
      });

      return Json(bilingMonthSelectList);
    }
    /// <summary>
    /// Display Rejection stage 1 and 2 only for ACH, for ICH, BIlateral and ACH using IATA rules display rejection stage 1 only.
    /// </summary>
    [HttpPost]
    public JsonResult GetRejectionStageForSmi(int settlementMethod)
    {
      var rejectionMemoSelectList = EnumMapper.GetMiscRejectionMemoStageList();

      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
      if (settlementMethod == (int)SMI.Ich || _referenceManager.IsSmiLikeBilateral(settlementMethod, true) || settlementMethod == (int)SMI.AchUsingIataRules)
      {
        const int stageTwo = (int)RejectionStage.StageTwo;
        rejectionMemoSelectList.Remove(rejectionMemoSelectList.SingleOrDefault(smi => smi.Value == stageTwo.ToString()));
      }

      return Json(rejectionMemoSelectList);
    }

    /// <summary>
    /// Get Payable screens Supporting document billing year month dropdown values as per supporting document type
    /// </summary>
    [HttpPost]
    public JsonResult GetPayableSupportingDocBillingYearMonth(int supportingDocumentTypeId)
    {
      var bilingMonthSelectList = new List<SelectListItem>();


      // var currentBillingPeriod = Ioc.Resolve<ICalendarManager>(typeof(ICalendarManager)).GetCurrentBillingPeriod();

      if (supportingDocumentTypeId == (int)SupportingDocType.InvoiceCreditNote)
      {
        for (var i = 0; i < 12; i++)
        {
          var strmonth = DateTime.UtcNow.AddMonths(-i).ToString(FormatConstants.MonthNameFormat);
          var stryear = DateTime.UtcNow.AddMonths(-i).Year.ToString();
          var monthValue = DateTime.UtcNow.AddMonths(-i).Month.ToString();

          bilingMonthSelectList.Add(new SelectListItem { Text = string.Format("{0}-{1}", stryear, strmonth), Value = string.Format("{0}-{1}", stryear, monthValue) });
        }
      }
      else if (supportingDocumentTypeId == (int)SupportingDocType.FormC)
      {
        for (var i = 0; i < 12; i++)
        {
          var strmonth = DateTime.UtcNow.AddMonths(-i).ToString(FormatConstants.MonthNameFormat);
          var stryear = DateTime.UtcNow.AddMonths(-i).Year.ToString();
          var monthValue = DateTime.UtcNow.AddMonths(-i).Month.ToString();

          bilingMonthSelectList.Add(new SelectListItem { Text = string.Format("{0}-{1}", stryear, strmonth), Value = string.Format("{0}-{1}", stryear, monthValue) });
        }
      }

      bilingMonthSelectList.Insert(0, new SelectListItem { Value = string.Empty, Text = "Please Select" });
      return Json(bilingMonthSelectList);
    }

    [HttpPost]
    [OutputCache(Duration = 1200, VaryByParam = "chargeCodeId;chargeCodeTypeId")]
    public JsonResult IsLineItemDetailsExpected(int chargeCodeId, int chargeCodeTypeId)
    {
      var isFieldMetaDataExists = MiscInvoiceManager.IsFieldMetaDataExists(chargeCodeId, (chargeCodeTypeId == 0 ? (int?)null : chargeCodeTypeId), -1);
      return Json(isFieldMetaDataExists);
    }

    [HttpGet]
    public JsonResult GetMemberLocationDetails(string locationCode, string invoiceId, bool isBillingMember, string memberId = null)
    {
      if (string.IsNullOrEmpty(locationCode))
      {
        return null;
      }

      if (memberId != null)
      {
        if (string.IsNullOrEmpty(locationCode.Trim()))
        {
          // get Details for main location
          locationCode = "Main";
        }

        var location = _memberManager.GetMemberDefaultLocation(Convert.ToInt32(memberId), locationCode);

        if (location!= null && string.IsNullOrEmpty(location.LegalText))
        {
          var eBillingConfig = _memberManager.GetEbillingConfig(location.MemberId);
          location.LegalText = eBillingConfig != null ? eBillingConfig.LegalText : location.LegalText;
        }

        return GetMemberLocationReferenceFromLocation(location);
      }

      var memberLocationDetails = InvoiceManager.GetMemberReferenceData(invoiceId, isBillingMember, locationCode);

      var result = new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = memberLocationDetails };

      return result;
    }

    private JsonResult GetMemberLocationReferenceFromLocation(Location memberLocation)
    {
      if(memberLocation == null)
          return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data =  new MemberLocationInformation() };

      var memberLocationInformation = new MemberLocationInformation
      {
        CompanyRegistrationId = memberLocation.RegistrationId,
        AddressLine1 = memberLocation.AddressLine1,
        AddressLine2 = memberLocation.AddressLine2,
        AddressLine3 = memberLocation.AddressLine3,
        CityName = memberLocation.CityName,
        DigitalSignatureRequiredId = 1,
        SubdivisionName = memberLocation.SubDivisionName,
        SubdivisionCode = memberLocation.SubDivisionCode,
        LegalText = memberLocation.LegalText,
        OrganizationName = memberLocation.MemberLegalName,
        PostalCode = memberLocation.PostalCode,
        TaxRegistrationId = memberLocation.TaxVatRegistrationNumber,
        MemberLocationCode = memberLocation.LocationCode,
        CountryCode = memberLocation.CountryId,
        AdditionalTaxVatRegistrationNumber = memberLocation.AdditionalTaxVatRegistrationNumber
      };

      var result = new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = memberLocationInformation };

      return result;
    }

    public ContentResult GetSubdivisionCodeList(string q, string dependentValue)
    {
      if (string.IsNullOrEmpty(dependentValue))
      {
        return null;
      }

      var countryName = dependentValue;
      var subdivisionCodeList = _memberManager.GetSubDivisionListByCountryName(countryName, q);

      var response = new StringBuilder();

      foreach (var subDivisionCode in subdivisionCodeList)
      {
        response.AppendFormat("{0}|{1}\n", subDivisionCode.Name, subDivisionCode.Id);
      }

      return new SanitizeResult(response.ToString());
    }

    public ContentResult GetCountryList(string q)
    {
      var countryList = (List<MST_COUNTRY>)UserManagementModel.GetAllCountries();
      if (!string.IsNullOrWhiteSpace(q))
      {
        countryList = countryList.Where(c => c.COUNTRY_NAME.ToUpper().Contains(q.ToUpper())).ToList();
      }

      countryList.Sort(CompareCountriesByCountryName);

      var response = new StringBuilder();

      foreach (var mstCountry in countryList)
      {
        response.AppendFormat("{0}|{1}\n", mstCountry.COUNTRY_NAME, mstCountry.COUNTRY_CODE);
      }

      return new SanitizeResult(response.ToString());
    }

    private static int CompareCountriesByCountryName(MST_COUNTRY Country1, MST_COUNTRY Country2)
    {
      return Country1.COUNTRY_NAME.CompareTo(Country2.COUNTRY_NAME);
    }

    /// <summary>
    /// Gets the subdivision name list based on the country code.
    /// </summary>
    /// <param name="q">The term entered in auto-complete textbox</param>
    /// <param name="dependentValue">Country code</param>
    /// <returns>List of subdivision names.</returns>
    [OutputCache(Duration = 86400, VaryByParam = "q;dependentValue")]
    public ContentResult GetSubdivisionNameList(string q, string dependentValue)
    {
      if (string.IsNullOrEmpty(dependentValue))
      {
        return null;
      }

      var subdivisionCodeList = _memberManager.GetSubDivisionList(dependentValue, q);

      var response = new StringBuilder();

      foreach (var subDivisionCode in subdivisionCodeList)
      {
        response.AppendFormat("{0}|{1}\n", subDivisionCode.Name, subDivisionCode.Name);
      }

      return new SanitizeResult(response.ToString());
    }

    public ContentResult GetAdditionalDetails(string q, string extraparam1, string extraparam2)
    {
      var type = (AdditionalDetailType)Enum.Parse(typeof(AdditionalDetailType), extraparam1);
      var level = (AdditionalDetailLevel)Enum.Parse(typeof(AdditionalDetailLevel), extraparam2);
      var additionalDetailList = _referenceManager.GetAdditionalDetailList(type, level).Where(additionalDetail => additionalDetail.Name.Contains(q));

      var response = new StringBuilder();

      foreach (var additionalDetail in additionalDetailList)
      {
        response.AppendFormat("{0}|{1}\n", additionalDetail.Name, additionalDetail.Name);
      }

      //SCP140271 - incorrect data in eInvoice are validated by SI
      return Content(response.ToString().Length == 0 ? "\n" : response.ToString());
    }

    [HttpPost]
    [OutputCache(Duration = 86400, VaryByParam = "listingCurrencyId;billingCurrencyId;billingPeriod")]
    public JsonResult GetExchangeRate(string listingCurrencyId, string billingCurrencyId, string billingPeriod)
    {
      try
      {
        int year;
        int month;
        if (string.IsNullOrEmpty(billingPeriod))
        {
          year = month = 0;
        }
        else
        {
          var billingPeriodTokens = billingPeriod.Split('-');

          year = Convert.ToInt32(billingPeriodTokens[0]);
          month = Convert.ToInt32(billingPeriodTokens[1]);
        }

        int currencyId;
        var billingCurrency = (BillingCurrency)Enum.Parse(typeof(BillingCurrency), billingCurrencyId);
        int.TryParse(listingCurrencyId, out currencyId);
        var exchangeRate = _referenceManager.GetExchangeRate(currencyId, billingCurrency, year, month);

        return Json(exchangeRate.ToString("0.00000"));
      }
      catch (Exception)
      {
        var details = new UIExceptionDetail { IsFailed = true, Message = "Exchange rate for given currencies is not available." };

        return Json(details);
      }
    }

    [HttpGet]
    [OutputCache(Duration = 86400, VaryByParam = "q;extraparam1")]
    public ContentResult GetSourceCodeList(string q, string extraparam1)
    {
      var sourceCodeList =
        _referenceManager.GetSourceCodeList(Convert.ToInt32(extraparam1)).Where(
          code => string.Format("{0}-{1}", code.SourceCodeIdentifier.ToString(), code.SourceCodeDescription.ToUpper()).Contains(q.ToUpper())).OrderBy(soureCode => soureCode.SourceCodeIdentifier).Select(
            sourceCode => sourceCode);

      var response = new StringBuilder();

      foreach (var sourceCode in sourceCodeList)
      {
        response.AppendFormat("{0}|{1}\n", string.Format("{0}-{1}", sourceCode.SourceCodeIdentifier, sourceCode.SourceCodeDescription), sourceCode.SourceCodeIdentifier);
      }

      return new SanitizeResult(response.ToString());
    }

    /// CMP523 - Source Code in RMBMCM Summary Report
    /// <summary>
    /// This method accepts transaction type and returns list of Source code for that transaction type
    /// removing dupicates
    /// </summary>
    /// <param name="transId">TransactionType</param>
    /// <returns></returns>
    [HttpGet]
    [OutputCache(Duration = 86400, VaryByParam = "transId")]
    public ContentResult GetSourceCodesList(int transId)
    {
        var sourceCodes = _referenceManager.GetSourceCodesList(Convert.ToInt32(transId));

        var sourceCodeNonBilateral = sourceCodes.Where(sc => sc.IsBilateralCode == false).Select(sc => new { sc.SourceCodeIdentifier }).Distinct().OrderBy(sci => sci.SourceCodeIdentifier).ToList();
        var sourceCodeBilateral = sourceCodes.Where(sc => sc.IsBilateralCode == true).Select(sc => new { sc.SourceCodeIdentifier }).Distinct().OrderBy(sci => sci.SourceCodeIdentifier).ToList();

        var sourceCodeList = sourceCodeNonBilateral.Union(sourceCodeBilateral);

        var response = new StringBuilder();

        foreach (var sourceCode in sourceCodeList)
        {
            response.AppendFormat("<option value=\"{0}\">{1}</option>",sourceCode.SourceCodeIdentifier, sourceCode.SourceCodeIdentifier);
        }

        return Content(response.ToString());
    }

    //CMP526 - Passenger Correspondence Identifiable by Source Code
    /// <summary>
    /// This Method accepts two transaction types and returns union of Source code for two transaction types 
    /// removing dupicates
    /// </summary>
    /// <param name="q"></param>
    /// <param name="extraparam1">TransactionType1</param>
    /// <param name="extraparam2">TransactionType2</param>
    /// <returns></returns>
    [HttpGet]
    [OutputCache(Duration = 86400, VaryByParam = "q;extraparam1")]
    public ContentResult GetSourceCodeListForCorrespondence(string q, string extraparam1)
    {
        
        var response = new StringBuilder();
        response.Append(Environment.NewLine);
        BillingCategoryType billingCategory = (BillingCategoryType)Enum.Parse(typeof(BillingCategoryType), extraparam1);

        switch(billingCategory)
        {
            case BillingCategoryType.Pax:
                var sourceCodesForNsRmStg3 =
                    _referenceManager.GetSourceCodeList(Convert.ToInt32(TransactionType.RejectionMemo3)).Where(
                        code =>
                        string.Format("{0}-{1}", code.SourceCodeIdentifier.ToString(),
                                      code.SourceCodeDescription.ToUpper()).
                            Contains(q.ToUpper())).OrderBy(soureCode => soureCode.SourceCodeIdentifier).Select(
                                sourceCode => sourceCode);

                var sourceCodesForSFrmXf =
                    _referenceManager.GetSourceCodeList(Convert.ToInt32(TransactionType.SamplingFormXF)).Where(
                        code =>
                        string.Format("{0}-{1}", code.SourceCodeIdentifier.ToString(),
                                      code.SourceCodeDescription.ToUpper()).
                            Contains(q.ToUpper())).OrderBy(soureCode => soureCode.SourceCodeIdentifier).Select(
                                sourceCode => sourceCode);

                var sourceCodeList = sourceCodesForNsRmStg3.Union(sourceCodesForSFrmXf).OrderBy(sourceCode=>sourceCode.SourceCodeIdentifier);//Union removes duplicates

                foreach (var sourceCode in sourceCodeList)
                {
                    response.AppendFormat("{0}|{1}{2}",
                                          string.Format("{0}-{1}", sourceCode.SourceCodeIdentifier,
                                                        sourceCode.SourceCodeDescription),
                                          sourceCode.SourceCodeIdentifier,Environment.NewLine);
                }
                break;
        }

        return new SanitizeResult(response.ToString());
    }

    [HttpGet]
    [OutputCache(Duration = 86400, VaryByParam = "q")]
    public ContentResult GetEntireSourceCodeList(string q)
    {
      var sourceCodeList =
        _referenceManager.GetSourceCodeList().Where(
          sourceCode => string.Format("{0}-{1}", sourceCode.SourceCodeIdentifier.ToString(), sourceCode.SourceCodeDescription != null ? sourceCode.SourceCodeDescription.ToUpper() : string.Empty).Contains(q.ToUpper()));

      sourceCodeList = sourceCodeList.Count() > 0 ? sourceCodeList.OrderBy(soureCode => soureCode.SourceCodeIdentifier).Select(sourceCode => sourceCode) : sourceCodeList;
      var distinctSourceCodeList = sourceCodeList.Distinct(new SourceCodeComparer());

      var response = new StringBuilder();

      foreach (var sourceCode in distinctSourceCodeList)
      {
        response.AppendFormat("{0}|{1}\n", string.Format("{0}-{1}", sourceCode.SourceCodeIdentifier, sourceCode.SourceCodeDescription), sourceCode.SourceCodeIdentifier);
      }

      return new SanitizeResult(response.ToString());
    }

    [HttpGet]
    [OutputCache(Duration = 86400, VaryByParam = "q;extraParam2;dependentValue")]
    public ContentResult GetReasonCodeList(string q, string extraParam2, string dependentValue)
    {
      var reasonCodeList =
        from reasonCode in
          _referenceManager.GetReasonCodeList(Convert.ToInt32(extraParam2)).Where(
            reasonCode => string.Format("{0}-{1}", reasonCode.Code.ToString(), reasonCode.Description.ToUpper()).Contains(q.ToUpper()) && reasonCode.BilateralCode == false).OrderBy(reasonCode => reasonCode.Code)
        select reasonCode;

      var response = new StringBuilder();

      foreach (var reasonCode in reasonCodeList)
      {
        response.AppendFormat("{0}|{1}\n", string.Format("{0}-{1}", reasonCode.Code, reasonCode.Description), reasonCode.Code);
      }

      return new SanitizeResult(response.ToString());
    }

    /// <summary>
    /// Following action is used to retrieve ReasonCodeList for Rejection Memo, CreditMemo. 
    /// Another action is there retrieve reasonCodeList but this action is used because we retrieve ReasonCOde as well as CouponBreakdownMandatory value
    /// </summary>
    [HttpGet]
    [OutputCache(Duration = 86400, VaryByParam = "q;extraParam2;dependentValue")]
    public ContentResult GetReasonCodeListForAutoComplete(string q, string extraParam2, string dependentValue)
    {
      //SCP0000: Elmah Exceptions log removal
      // Declare variable to get transaction type
       int transactionTypeValue = 0;

      // Dependent value parameter is set in case of RejectionMemo, else it is null in case of Billing and Credit memo
      if (!string.IsNullOrEmpty(dependentValue))
      {
        transactionTypeValue = Convert.ToInt32(dependentValue) + 1;
      }
      else
      {
        if (extraParam2 != null)
        transactionTypeValue = Convert.ToInt32(extraParam2);
      }

      // Retrieve ReasonCode list depending on transactionType
      var reasonCodeList =
        from reasonCode in
          _referenceManager.GetReasonCodeList(transactionTypeValue).Where(
          reasonCode => string.Format("{0}-{1}", reasonCode.Code.ToString(), reasonCode.Description.ToUpper()).Contains(q.ToUpper())).OrderBy(reasonCode => reasonCode.BilateralCode)
        select reasonCode;

      // Declare string builder
      var response = new StringBuilder();

      // Iterate through retrieved ReasonCode list and append Coupon Breakdown to ReasonCode
      foreach (var reasonCode in reasonCodeList)
      {
        response.AppendFormat("{0}|{1}\n", string.Format("{0}-{1}", reasonCode.Code, reasonCode.Description), reasonCode.Code + "-" + reasonCode.CouponAwbBreakdownMandatory);
      }

      // Return ContentResult
      return new SanitizeResult(response.ToString());
    }

    //CMP#502 : [3.5] Rejection Reason for MISC Invoices
    /// <summary>
    /// Gets the misc rejection reason code list for auto complete.
    /// </summary>
    /// <param name="q">The q.</param>
    /// <param name="extraParam2">The extra param2.</param>
    /// <param name="dependentValue">The dependent value.</param>
    /// <returns></returns>
    [HttpGet]
    [OutputCache(NoStore = true, Duration = 0)]
    public ContentResult GetMiscRejectionReasonCodeListForAutoComplete(string q, string extraParam2, string dependentValue)
    {
        int transactionTypeValue = 0;

        // Dependent value parameter is set in case of RejectionMemo, else it is null in case of Billing and Credit memo
        if (!string.IsNullOrEmpty(dependentValue))
        {
            transactionTypeValue = Convert.ToInt32(dependentValue) + 1;
        }
        else
        {
            if (extraParam2 != null)
                transactionTypeValue = Convert.ToInt32(extraParam2);
        }

        // Retrieve ReasonCode list depending on transactionType
        var reasonCodeList =
          from reasonCode in
              _referenceManager.GetReasonCodeList(transactionTypeValue).Where(
              reasonCode => string.Format("{0}-{1}", reasonCode.Code.ToString(), !string.IsNullOrWhiteSpace(reasonCode.Description) ? reasonCode.Description.ToUpper():string.Empty).Contains(q.ToUpper())).OrderBy(reasonCode => reasonCode.Code)
          select reasonCode;

        // Declare string builder
        var response = new StringBuilder();

        // Iterate through retrieved ReasonCode list and append Coupon Breakdown to ReasonCode
        foreach (var reasonCode in reasonCodeList)
        {
            response.AppendFormat("{0}|{1}\n", string.Format("{0}-{1}", reasonCode.Code, reasonCode.Description), reasonCode.Code);
        }
        // Return ContentResult
        var returnList = response.ToString();
        return new SanitizeResult(string.IsNullOrEmpty(returnList) ? "NoItemFound" : returnList);
    }

    [HttpGet]
    [OutputCache(Duration = 86400, VaryByParam = "q;extraParam2;dependentValue")]
    public ContentResult GetReasonCodeListForCargo(string q, string extraParam2, string dependentValue)
    {
      // Declare variable to get transaction type
      int transactionTypeValue;

      // Dependent value parameter is set in case of RejectionMemo, else it is null in case of Billing and Credit memo
      if (!string.IsNullOrEmpty(dependentValue))
      {
        // dependentValue is the rejection stage.
        switch (dependentValue)
        {
          case "1":
            transactionTypeValue = Convert.ToInt32(TransactionType.CargoRejectionMemoStage1);
            break;
          case "2":
            transactionTypeValue = Convert.ToInt32(TransactionType.CargoRejectionMemoStage2);
            break;
          case "3":
            transactionTypeValue = Convert.ToInt32(TransactionType.CargoRejectionMemoStage3);
            break;
          default:
            transactionTypeValue = Convert.ToInt32(TransactionType.CargoRejectionMemoStage1);
            break;
        }
      }
      else
      {
        transactionTypeValue = Convert.ToInt32(extraParam2);
      }

      // Retrieve ReasonCode list depending on transactionType
      var reasonCodeList =
        from reasonCode in
          _referenceManager.GetReasonCodeList(transactionTypeValue).Where(
          reasonCode => string.Format("{0}-{1}", reasonCode.Code.ToString(), string.IsNullOrEmpty(reasonCode.Description) ? string.Empty : reasonCode.Description.ToUpper()).Contains(q.ToUpper())).OrderBy(reasonCode => reasonCode.BilateralCode)
        select reasonCode;

      // Declare string builder
      var response = new StringBuilder();

      // Iterate through retrieved ReasonCode list and append Coupon Breakdown to ReasonCode
      foreach (var reasonCode in reasonCodeList)
      {
        response.AppendFormat("{0}|{1}\n", string.Format("{0}-{1}", reasonCode.Code, reasonCode.Description), reasonCode.Code + "-" + reasonCode.CouponAwbBreakdownMandatory);
      }

      // Return ContentResult
      return Content(response.ToString());
    }


    /// <summary>
    /// Following action is used to retrieve ReasonCodeList for BillingMemo. 
    /// </summary>
    [HttpGet]
    [OutputCache(Duration = 86400, VaryByParam = "q")]
    public ContentResult GetReasonCodeListForBillingMemo(string q)
    {
      // Retrieve ReasonCode list depending on transactionType
      var reasonCodeList =
        from reasonCode in
          NonSamplingInvoiceManager.GetReasonCodeListForBillingMemo().Where(
          reasonCode => string.Format("{0}-{1}", reasonCode.Code.ToString(), reasonCode.Description.ToUpper()).Contains(q.ToUpper())).OrderBy(reasonCode => reasonCode.BilateralCode)
        select reasonCode;

      // Declare string builder
      var response = new StringBuilder();

      // Iterate through retrieved ReasonCode list and append Coupon Breakdown to ReasonCode
      foreach (var reasonCode in reasonCodeList)
      {
        response.AppendFormat("{0}|{1}\n", string.Format("{0}-{1}", reasonCode.Code, reasonCode.Description), reasonCode.Code + "-" + reasonCode.CouponAwbBreakdownMandatory);
      }

      // Return ContentResult
      return new SanitizeResult(response.ToString());
    }

    /// <summary>
    /// Following action is used to retrieve ReasonCodeList for BillingMemo. 
    /// </summary>
    [HttpGet]
    [OutputCache(Duration = 86400, VaryByParam = "q")]
    public ContentResult GetReasonCodeListForCargoBillingMemo(string q)
    {
      // Retrieve ReasonCode list depending on transactionType
      var reasonCodeList =
        from reasonCode in
          CargoInvoiceManager.GetReasonCodeListForBillingMemo().Where(
          reasonCode => string.Format("{0}-{1}", reasonCode.Code.ToString(), reasonCode.Description.ToUpper()).Contains(q.ToUpper())).OrderBy(reasonCode => reasonCode.BilateralCode)
        select reasonCode;

      // Declare string builder
      var response = new StringBuilder();

      // Iterate through retrieved ReasonCode list and append Coupon Breakdown to ReasonCode
      foreach (var reasonCode in reasonCodeList)
      {
        response.AppendFormat("{0}|{1}\n", string.Format("{0}-{1}", reasonCode.Code, reasonCode.Description), reasonCode.Code + "-" + reasonCode.CouponAwbBreakdownMandatory);
      }

      // Return ContentResult
      return new SanitizeResult(response.ToString());
    }

    [HttpGet]
    [OutputCache(Duration = 86400, VaryByParam = "q")]
    public ContentResult GetEntireReasonCodeList(string q)
    {
      var reasonCodeList =
        from reasonCode in
          _referenceManager.GetReasonCodeList().Where(
            reasonCode =>
            string.Format("{0}-{1}", reasonCode.Code.ToString(), reasonCode.Description.ToUpper()).Contains(q.ToUpper()))
          .OrderBy(reasonCode => reasonCode.Code)
        select reasonCode;
      var response = new StringBuilder();

      foreach (var reasonCode in reasonCodeList)
      {
        response.AppendFormat("{0}|{1}\n", string.Format("{0}-{1}", reasonCode.Code, reasonCode.Description),
                              reasonCode.Code);

      }

      return Content(response.ToString());
    }

    /// <summary>
    /// SCP 121308 : Reason Codes in PAX billing history screen appear multiple times.
    /// this will specifically use for Pax billing history
    /// </summary>
    /// <param name="q">Search content</param>
    /// <param name="extraParam1">rejection Stage</param>
    /// <param name="extraParam2">billing code</param>
    /// <param name="dependentValue">transaction type</param>
    /// <returns></returns>
    [HttpGet]
    public ContentResult GetPaxReasonCodeListForBillingHistory(string q, string extraParam1, string extraParam2, string dependentValue)
    {
      var rejectionStage = extraParam1;
      var billingCode = extraParam2;
      var transactionType = dependentValue;
      const string blankOrall = "-1";
      const string prime = "1";
      const string rejectionMemo = "4";
      const string billingMemo = "5";
      const string creditMemo = "6";
      const string nonSamplingInvoice = "0";
      const string samplingFormAB = "3";
      const string samplingFormDE = "5";
      const string samplingFormF = "6";
      const string samplingFormXF = "7";

      IEnumerable<ReasonCode> reasonCodes = _referenceManager.GetReasonCodeList(true);

      //case 1: Billing Code = All	Transaction Type = All	Rejection Stage = Blank Fetch Reason codes for Transaction Type IDs	2/3/4/10/11/5/6/29/30
      if (!string.IsNullOrEmpty(billingCode) && billingCode == blankOrall && !string.IsNullOrEmpty(transactionType) && transactionType == blankOrall && string.IsNullOrEmpty(rejectionStage))
      {
        reasonCodes = reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int) TransactionType.RejectionMemo1 ||
                                            reasonCode.TransactionTypeId == (int) TransactionType.RejectionMemo2 ||
                                            reasonCode.TransactionTypeId == (int) TransactionType.RejectionMemo3 ||
                                            reasonCode.TransactionTypeId == (int) TransactionType.BillingMemo ||
                                            reasonCode.TransactionTypeId == (int) TransactionType.CreditMemo ||
                                            reasonCode.TransactionTypeId == (int) TransactionType.SamplingFormF ||
                                            reasonCode.TransactionTypeId == (int) TransactionType.SamplingFormXF ||
                                            reasonCode.TransactionTypeId == (int) TransactionType.PasNsBillingMemoDueToAuthorityToBill ||
                                            reasonCode.TransactionTypeId == (int) TransactionType.PasNsBillingMemoDueToExpiry);
      }

      //case 2: Billing Code = All/samplingFormDE/samplingFormAB;	Transaction Type = Prime;	Rejection Stage = Blank; Reason code = Nothing (user cannot input a Reason Code here)
      if (!string.IsNullOrEmpty(billingCode) && (billingCode == blankOrall || billingCode == samplingFormAB || billingCode == samplingFormDE) && !string.IsNullOrEmpty(transactionType) && transactionType == prime && string.IsNullOrEmpty(rejectionStage))
      {
        reasonCodes = new List<ReasonCode>();
      }

      //case 3: Billing Code = All;	Transaction Type = rejectionMemo;	Rejection Stage = Blank; Fetch Reason codes for Transaction Type IDs	2/3/4/10/11
      if (!string.IsNullOrEmpty(billingCode) && billingCode == blankOrall && !string.IsNullOrEmpty(transactionType) && transactionType == rejectionMemo && string.IsNullOrEmpty(rejectionStage))
      {
        reasonCodes = reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int)TransactionType.RejectionMemo1 ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.RejectionMemo2 ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.RejectionMemo3 ||
                                             reasonCode.TransactionTypeId == (int)TransactionType.SamplingFormF ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.SamplingFormXF );
      }

      //case 4: Billing Code = All;	Transaction Type = rejectionMemo;	Rejection Stage = Blank; Fetch Reason codes for Transaction Type IDs	2
      if (!string.IsNullOrEmpty(billingCode) && billingCode == blankOrall && !string.IsNullOrEmpty(transactionType) && transactionType == rejectionMemo && !string.IsNullOrEmpty(rejectionStage) && rejectionStage == "1")
      {
        reasonCodes = reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int) TransactionType.RejectionMemo1);
      }

      //case 5: Billing Code = All;	Transaction Type = rejectionMemo;	Rejection Stage = Blank; Fetch Reason codes for Transaction Type IDs	3/10
      if (!string.IsNullOrEmpty(billingCode) && billingCode == blankOrall && !string.IsNullOrEmpty(transactionType) && transactionType == rejectionMemo && !string.IsNullOrEmpty(rejectionStage) && rejectionStage == "2")
      {
        reasonCodes = reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int)TransactionType.RejectionMemo2 ||
                                          reasonCode.TransactionTypeId == (int)TransactionType.SamplingFormF);
      }

      //case 6: Billing Code = All;	Transaction Type = rejectionMemo;	Rejection Stage = Blank; Fetch Reason codes for Transaction Type IDs	4/11
      if (!string.IsNullOrEmpty(billingCode) && billingCode == blankOrall && !string.IsNullOrEmpty(transactionType) && transactionType == rejectionMemo && !string.IsNullOrEmpty(rejectionStage) && rejectionStage == "3")
      {
        reasonCodes = reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int)TransactionType.RejectionMemo3 ||
                                          reasonCode.TransactionTypeId == (int)TransactionType.SamplingFormXF);
      }

      //case 7: Billing Code = All;	Transaction Type = billingMemo;	Rejection Stage = not consider; Fetch Reason codes for Transaction Type IDs	5/29/30
      if (!string.IsNullOrEmpty(billingCode) && billingCode == blankOrall && !string.IsNullOrEmpty(transactionType) && transactionType == billingMemo)
      {
        reasonCodes = reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int)TransactionType.BillingMemo ||
                                          reasonCode.TransactionTypeId == (int)TransactionType.PasNsBillingMemoDueToAuthorityToBill ||
                                          reasonCode.TransactionTypeId == (int)TransactionType.PasNsBillingMemoDueToExpiry);
      }

      //case 8: Billing Code = All;	Transaction Type = creditMemo;	Rejection Stage = not consider; Fetch Reason codes for Transaction Type IDs	6
      if (!string.IsNullOrEmpty(billingCode) && billingCode == blankOrall && !string.IsNullOrEmpty(transactionType) && transactionType == creditMemo)
      {
        reasonCodes = reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int)TransactionType.CreditMemo);
      }

      //case 9: Billing Code = All;	Transaction Type = All;	Rejection Stage = not consider; Fetch Reason codes for Transaction Type IDs	2/3/4/5/6/29/30
      if (!string.IsNullOrEmpty(billingCode) && billingCode == nonSamplingInvoice && !string.IsNullOrEmpty(transactionType) && transactionType == blankOrall)
      {
        reasonCodes = reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int)TransactionType.RejectionMemo1 ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.RejectionMemo2 ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.RejectionMemo3 ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.BillingMemo ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.CreditMemo ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.PasNsBillingMemoDueToAuthorityToBill ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.PasNsBillingMemoDueToExpiry);
      }

      //case 10: Billing Code = All;	Transaction Type = rejectionMemo;	Rejection Stage = Blank; Fetch Reason codes for Transaction Type IDs	2/3/4
      if (!string.IsNullOrEmpty(billingCode) && billingCode == nonSamplingInvoice && !string.IsNullOrEmpty(transactionType) && transactionType == rejectionMemo && string.IsNullOrEmpty(rejectionStage))
      {
        reasonCodes = reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int)TransactionType.RejectionMemo1 ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.RejectionMemo2 ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.RejectionMemo3);
      }

      //case 11: Billing Code = nonSamplingInvoice;	Transaction Type = rejectionMemo;	Rejection Stage = 1; Fetch Reason codes for Transaction Type IDs	2
      if (!string.IsNullOrEmpty(billingCode) && billingCode == nonSamplingInvoice && !string.IsNullOrEmpty(transactionType) && transactionType == rejectionMemo && !string.IsNullOrEmpty(rejectionStage) && rejectionStage == "1")
      {
        reasonCodes =
          reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int) TransactionType.RejectionMemo1);
      }

      //case 12: Billing Code = nonSamplingInvoice;	Transaction Type = rejectionMemo;	Rejection Stage = 2; Fetch Reason codes for Transaction Type IDs	3
      if (!string.IsNullOrEmpty(billingCode) && billingCode == nonSamplingInvoice && !string.IsNullOrEmpty(transactionType) && transactionType == rejectionMemo && !string.IsNullOrEmpty(rejectionStage) && rejectionStage == "2")
      {
        reasonCodes =
          reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int)TransactionType.RejectionMemo2);
      }

      //case 13: Billing Code = nonSamplingInvoice;	Transaction Type = rejectionMemo;	Rejection Stage = 3; Fetch Reason codes for Transaction Type IDs	4
      if (!string.IsNullOrEmpty(billingCode) && billingCode == nonSamplingInvoice && !string.IsNullOrEmpty(transactionType) && transactionType == rejectionMemo && !string.IsNullOrEmpty(rejectionStage) && rejectionStage == "3")
      {
        reasonCodes =
          reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int)TransactionType.RejectionMemo3);
      }

      //case 14: Billing Code = samplingFormF;	Transaction Type =  not consider;	Rejection Stage =  not consider; Fetch Reason codes for Transaction Type IDs	10
      if (!string.IsNullOrEmpty(billingCode) && billingCode == samplingFormF)
      {
        reasonCodes =
          reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int)TransactionType.SamplingFormF);
      }

      //case 15: Billing Code = samplingFormXF;	Transaction Type = not consider;	Rejection Stage = not consider; Fetch Reason codes for Transaction Type IDs	11
      if (!string.IsNullOrEmpty(billingCode) && billingCode == samplingFormXF)
      {
        reasonCodes =
          reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int)TransactionType.SamplingFormXF);
      }

      reasonCodes = reasonCodes.AsEnumerable().Distinct(new ReasonCodeComparer());

      var response = new StringBuilder();

      foreach (var reasonCode in reasonCodes)
      {
        response.AppendFormat("{0}|{1}\n", string.Format("{0}-{1}", reasonCode.Code, reasonCode.Description),
                              reasonCode.Code);
      }

      return new SanitizeResult(response.ToString());
    }

    /// <summary>
    /// SCP 121308 : Reason Codes in PAX billing history screen appear multiple times
    /// this will specifically use for cargo billing history screen
    /// </summary>
    /// <param name="q">Search content</param>
    /// <param name="extraParam2">rejection Stage</param>
    /// <param name="dependentValue">transaction type</param>
    /// <returns></returns>
    [HttpGet]
    public ContentResult GetReasonCodeListForCargoBillingHistory(string q, string extraParam2, string dependentValue)
    {
      #region Before code fix
      /*// Declare variable to get transaction type
      int transactionTypeValue;

      // Dependent value parameter is set in case of RejectionMemo, else it is null in case of Billing and Credit memo
      if (!string.IsNullOrEmpty(dependentValue))
      {
        // dependentValue is the rejection stage.
        switch (dependentValue)
        {
          case "1":
            transactionTypeValue = 14;
            break;
          case "5":
            transactionTypeValue = 19;
            break;
          case "6":
            transactionTypeValue = 20;
            break;
          case "4":
            transactionTypeValue = 16;
            break;
          default:
            transactionTypeValue = Convert.ToInt32(TransactionType.CargoRejectionMemoStage1);
            break;
        }
      }
      else
      {
        transactionTypeValue = Convert.ToInt32(dependentValue);
      }

      var reasonCodeList =
        from reasonCode in
          _referenceManager.GetReasonCodeList().Where(reasonCode => string.Format("{0}-{1}", reasonCode.Code.ToString(), reasonCode.Description.ToUpper()).Contains(q.ToUpper())).OrderBy(reasonCode => reasonCode.Code)
        select reasonCode;


      reasonCodeList = reasonCodeList.Where(reasonCode => reasonCode.TransactionTypeId == transactionTypeValue);*/
      #endregion

      var rejectionStage = extraParam2;
      var transactionType = dependentValue;
      const string blankOrall = "-1";
      const string primeAWB = "14";
      const string rejectionMemo = "16";
      const string billingMemo = "19";
      const string creditMemo = "20";
      
      IEnumerable<ReasonCode> reasonCodes = _referenceManager.GetReasonCodeList(true);

      //case 1: Transaction Type = All;	Rejection Stage = Blank; Fetch Reason codes for Transaction Type IDs	16/17/18/19/20/31/32
      if (!string.IsNullOrEmpty(transactionType) && transactionType == blankOrall && string.IsNullOrEmpty(rejectionStage))
      {
        reasonCodes = reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int)TransactionType.CargoRejectionMemoStage1 ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.CargoRejectionMemoStage2 ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.CargoRejectionMemoStage3 ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.CargoBillingMemo ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.CargoCreditMemo ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.CargoBillingMemoDueToAuthorityToBill ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.CargoBillingMemoDueToExpiry);
      }

      //case 2: Transaction Type = primeAWB;	Rejection Stage = not consider; Fetch Reason codes for Transaction Type IDs	16/17/18
      if (!string.IsNullOrEmpty(transactionType) && transactionType == primeAWB)
      {
        reasonCodes = new List<ReasonCode>();
      }

      //case 2: Transaction Type = rejectionMemo;	Rejection Stage = Blank; Fetch Reason codes for Transaction Type IDs	16/17/18
      if (!string.IsNullOrEmpty(transactionType) && transactionType == rejectionMemo && string.IsNullOrEmpty(rejectionStage))
      {
        reasonCodes = reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int)TransactionType.CargoRejectionMemoStage1 ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.CargoRejectionMemoStage2 ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.CargoRejectionMemoStage3);
      }

      //case 3: Transaction Type = rejectionMemo;	Rejection Stage = 1; Fetch Reason codes for Transaction Type IDs	16
      if (!string.IsNullOrEmpty(transactionType) && transactionType == rejectionMemo && !string.IsNullOrEmpty(rejectionStage) && rejectionStage == "1")
      {
        reasonCodes = reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int) TransactionType.CargoRejectionMemoStage1);
      }

      //case 4: Transaction Type = rejectionMemo;	Rejection Stage = 2; Fetch Reason codes for Transaction Type IDs	17
      if (!string.IsNullOrEmpty(transactionType) && transactionType == rejectionMemo && !string.IsNullOrEmpty(rejectionStage) && rejectionStage == "2")
      {
        reasonCodes = reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int)TransactionType.CargoRejectionMemoStage2);
      }

      //case 5: Transaction Type = rejectionMemo;	Rejection Stage = 3; Fetch Reason codes for Transaction Type IDs	18
      if (!string.IsNullOrEmpty(transactionType) && transactionType == rejectionMemo && !string.IsNullOrEmpty(rejectionStage) && rejectionStage == "3")
      {
        reasonCodes = reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int)TransactionType.CargoRejectionMemoStage3);
      }

      //case 6: Transaction Type = billingMemo;	Rejection Stage = not consider; Fetch Reason codes for Transaction Type IDs	19/31/32
      if (!string.IsNullOrEmpty(transactionType) && transactionType == billingMemo)
      {
        reasonCodes = reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int)TransactionType.CargoBillingMemo ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.CargoBillingMemoDueToAuthorityToBill ||
                                            reasonCode.TransactionTypeId == (int)TransactionType.CargoBillingMemoDueToExpiry);
      }

      //case 7: Transaction Type = creditMemo;	Rejection Stage = not consider; Fetch Reason codes for Transaction Type IDs	20
      if (!string.IsNullOrEmpty(transactionType) && transactionType == creditMemo)
      {
        reasonCodes = reasonCodes.Where(reasonCode => reasonCode.TransactionTypeId == (int)TransactionType.CargoCreditMemo);
      }

      reasonCodes = reasonCodes.AsEnumerable().Distinct(new ReasonCodeComparer());

      var response = new StringBuilder();

      foreach (var reasonCode in reasonCodes)
      {
        response.AppendFormat("{0}|{1}\n", string.Format("{0}-{1}", reasonCode.Code, reasonCode.Description), reasonCode.Code);
      }

      return new SanitizeResult(response.ToString());
    }

    //[HttpGet]
    //[OutputCache(Duration = 1200, VaryByParam = "q")]
    //public ContentResult GetTicketIssuingAirlineList(string q)
    //{
       
    //        var memberCodeList =
    //            from airline in
    //                _memberManager.GetMemberListFromDB().Where(
    //                    member => member.MemberCodeNumeric.ToUpper().Contains(q.ToUpper()))
    //            select airline.MemberCodeNumeric.PadLeft(3, '0');

    //        var response = new StringBuilder();
    //        foreach (var membercode in memberCodeList)
    //        {
               
    //                response.AppendFormat("{0}|{1}\n", membercode, membercode);
    //        }


    //        return Content(response.ToString());
     
    //}

    [HttpGet]
    [OutputCache(Duration = 1200, VaryByParam = "q")]
    public ContentResult GetTicketIssuingAirlineList(string q)
    {
        bool resultNumeric = true;
        int ii = 0;
        if (!string.IsNullOrWhiteSpace(q))
            resultNumeric = int.TryParse(q, out ii);

        if (resultNumeric)
        {
            var memberCodeList =
                from airline in
                    _memberManager.GetMemberListFromDB().Where(
                        member => member.MemberCodeNumeric.ToUpper().Contains(q.ToUpper()))
                select airline.MemberCodeNumeric.PadLeft(3, '0');

            var response = new StringBuilder();
            foreach (var membercode in memberCodeList)
            {
                int i = 0;
                bool result = int.TryParse(membercode, out i);
                if (result)
                    response.AppendFormat("{0}|{1}\n", membercode, membercode);
            }


            return new SanitizeResult(response.ToString());


        }
        var responses = new StringBuilder();
        return new SanitizeResult(responses.ToString());
    }

    [HttpGet]
    [OutputCache(Duration = 1200, VaryByParam = "q")]
    public ContentResult GetIssuingAirlineList(string q)
    {
      var memberCodeList = from airline in _memberManager.GetMemberListFromDB().Where(member => member.MemberCodeNumeric.ToUpper().Contains(q.ToUpper()))
                           select airline.MemberCodeAlpha;

      var response = new StringBuilder();
      foreach (var membercode in memberCodeList)
      {
        response.AppendFormat("{0}|{1}\n", membercode, membercode);
      }

      return Content(response.ToString());
    }

    [HttpGet]
    [OutputCache(Duration = 1200, VaryByParam = "q")]
    public ContentResult GetOtherChargeCodeList(string q)
    {
      var otherChargeCodeList = from otherCharge in EnumMapper.OtherChargeDic.Where(otherCharge => string.Format("{0}-{1}", otherCharge.Key.ToUpper(), otherCharge.Value.ToUpper()).Contains(q.ToUpper())) select otherCharge;

      var response = new StringBuilder();

      foreach (var otherCharge in otherChargeCodeList)
      {
        response.AppendFormat("{0}|{1}\n", string.Format("{0}-{1}", otherCharge.Key, otherCharge.Value), otherCharge.Key);
      }

      return new SanitizeResult(response.ToString());
    }

    /// <summary>
    /// Get tax code list for entered value
    /// </summary>
    /// <param name="term"></param>
    /// <returns></returns>
    [HttpGet]
    [OutputCache(Duration = 86400, VaryByParam = "term")]
    public JsonResult GetTaxCodeList(string term)
    {
      var taxCodeList = from taxCode in _referenceManager.GetTaxCodeList().Where(tax => tax.Description.ToUpper().Contains(term.ToUpper()))
                        select taxCode.Description;

      var result = new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = taxCodeList.ToArray() };

      return result;
    }

    [HttpGet]
    [OutputCache(Duration = 86400, VaryByParam = "q")]
    public ContentResult GetTaxCodes(string q)
    {
      var taxCodeList =
        _referenceManager.GetTaxCodeList().Where(
          tax => string.Format("{0}", tax.Id.ToString()).Contains(q.ToUpper())).OrderBy(taxId => taxId.Id).Select(
            taxCode => taxCode);

      var response = new StringBuilder();

      foreach (var taxCode in taxCodeList)
      {
        response.AppendFormat("{0}|{1}\n", string.Format("{0}", taxCode.Id), taxCode.Id);
      }

      return new SanitizeResult(response.ToString());
    }

    /// <summary>
    /// Return default value for settlement indicator for given combination of billing and billed members
    /// </summary>
    /// <param name="billingMemberId">Billing Member Id</param>
    /// <param name="billedMemberId">Billed Member Id</param>
    /// <param name="billingCategoryId">The billing category id.</param>
    [HttpPost]
    public JsonResult GetDefaultSettlementMethod(int billingMemberId, int billedMemberId, int billingCategoryId)
    {
      var defaultSmi = InvoiceManager.GetDefaultSettlementMethodForMembers(billingMemberId, billedMemberId, billingCategoryId);
      var result = new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = Convert.ToInt32(defaultSmi) };

      return result;
    }

    /// <summary>
    /// Return default value for settlement indicator for given combination of billing and billed members
    /// CMP #553: ACH Requirement for Multiple Currency Handling
    /// </summary>
    /// <param name="billingMemberId">Billing Member Id</param>
    /// <param name="billedMemberId">Billed Member Id</param>
    [HttpGet]
    [OutputCache(Duration = 86400, VaryByParam = "billingMemberId;billedMemberId")]
    public JsonResult IsBillingAndBilledAchOrDualMember(int billingMemberId, int billedMemberId)
    {
        var isValidForAchCurrency = InvoiceManager.IsBillingAndBilledAchOrDualMember(billingMemberId, billedMemberId);
        var result = new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = isValidForAchCurrency };

      return result;
    }

    /// <summary>
    /// This function is used to check legal archive process compelted for the period.
    /// </summary>
    /// <param name="billingPeriod"></param>
    /// <returns></returns>
    /// CMP #659: SIS IS-WEB Usage Report.
    [HttpGet]
    public JsonResult IsLegalArchivingProcessCompleted(int billingPeriod)
    {
        var isLegalArchiving = new ReferenceManager().IsLegalArchivingProcessCompleted(billingPeriod);
        var result = new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = isLegalArchiving };

        return result;
    }
      

    [HttpGet]
    public ContentResult GetCityAirportList(string q)
    {
      var cityAirportList =
        from cityAirport in _referenceManager.GetCityAirportList().Where(cityAirport => cityAirport.Name.ToUpper().Contains(q.ToUpper()))
        select cityAirport;

      var response = new StringBuilder();
      foreach (var cityAirport in cityAirportList)
      {
        response.AppendFormat("{0}|{1}\n", string.Format("{0}-{1}", cityAirport.Name, cityAirport.MainCity), cityAirport.Id);
      }

      return Content(response.ToString());
    }

    /// <summary>
    /// Get data for dynamic field of type autocomplete
    /// </summary>
    /// <param name="q">search string</param>
    /// <param name="extraparam1">data source id of field</param>
    [HttpGet]
    public ContentResult GetDynamicFieldAutocompleteData(string q, string extraparam1)
    {
      int dataSourceId = Convert.ToInt32(extraparam1);

      // Added try catch block to fix error thrown by stored procedure when data source id is null
      try
      {
        var dynamicFieldDataList =
          from dataValue in
            MiscInvoiceManager.GetDataSourceValues(dataSourceId).Where(
              dataValues => dataValues.Value.ToUpper().Contains(q.ToUpper()))
          select dataValue.Text;

        var response = new StringBuilder();

        foreach (var dataValue in dynamicFieldDataList)
        {
          response.AppendFormat("{0}|{0}\n", dataValue);
        }

        //CMP #636: Standard Update Mobilization.
        return Content(string.IsNullOrEmpty(response.ToString()) ? "NoItemFound" : response.ToString());
      }
      catch (Exception exception)
      {
        Logger.Error("Error while getting dynamic field auto complete data.", exception);

        return Content("NoItemFound");
      }
    }

    /// <summary>
    /// Returns the member location details for the given location id. Used to display payment detail information on misc. invoice header page. 
    /// </summary>
    /// <returns>Member location details if found for the location id, else location details for main location id</returns>
    [HttpPost]
    public JsonResult GetLocationDetails(int memberId, string locationId)
    {
      if (string.IsNullOrEmpty(locationId.Trim()))
      {
        // get Details for main location
        locationId = "Main";
      }

      var locationDetails = _memberManager.GetMemberDefaultLocation(memberId, locationId);
      // if location not present for the given location id, then get data for Main location id.
      if (locationDetails == null && locationId != "Main")
      {
        locationDetails = _memberManager.GetMemberDefaultLocation(memberId, "Main");
      }

      return Json(locationDetails);
    }

    //CMP#622: MISC Outputs Split as per Location IDs
    /// <summary>
    /// Returns the Location codes in sorted order based on MemberId to display in Autocomplete field on Download File screen
    /// </summary>
    /// <returns>Member's location codes other than Main and Uatp location id</returns>
    [HttpGet]
    //[OutputCache(Duration = 1200, VaryByParam = "q;extraParam1")]
    public ContentResult GetMemberLocationList(string q, int extraParam1)
    {

        var memberLocationList = _memberManager.GetMemberLocationList(extraParam1);

        memberLocationList = memberLocationList.Where(l => l.LocationCode.ToUpper().Contains(q.ToUpper())).ToList();

        if (memberLocationList.Count() > 0)
        {
            var mainLocationList = memberLocationList.Where(l => l.LocationCode.ToUpper() == "MAIN" || l.LocationCode.ToUpper() == "UATP").ToList();

            var filterList = memberLocationList.Except(mainLocationList).Select(l=>l.LocationCode.Trim()).ToList();

             filterList = filterList.OrderBy(l => int.Parse(l)).ToList();

                // Declare string builder
                var response = new StringBuilder();

                // Iterate through retrieved filtered location list and get the desired location code
                foreach (var locationCode in filterList)
                {
                    response.AppendFormat("{0}|{1}\n",  locationCode, locationCode);
                }

                // Return ContentResult
                //return Content(response.ToString());
                return Content(string.IsNullOrEmpty(response.ToString()) ? "NoItemFound" : response.ToString());
        }

        return Content("NoItemFound");
    }

    //CMP#622: MISC Outputs Split as per Location IDs
    /// <summary>
    /// Returns the Location codes in sorted order based on MemberId to display in Autocomplete field on System Monitor
    /// </summary>
    /// <returns>Member's location codes other than Main and Uatp location id</returns>
    //[OutputCache(Duration = 1200, VaryByParam = "q;dependentValue")]
    public ContentResult GetLocationListOfMemberOnSM(string q, int dependentValue)
    {
        var memberLocationList = _memberManager.GetMemberLocationList(dependentValue);

        memberLocationList = memberLocationList.Where(l => l.LocationCode.ToUpper().Contains(q.ToUpper())).ToList();

        if (memberLocationList.Count() > 0)
        {
            var mainLocationList = memberLocationList.Where(l => l.LocationCode.ToUpper() == "MAIN" || l.LocationCode.ToUpper() == "UATP").ToList();

            var filterList = memberLocationList.Except(mainLocationList).Select(l => l.LocationCode.Trim()).ToList();

            filterList = filterList.OrderBy(l => int.Parse(l)).ToList();

            // Declare string builder
            var response = new StringBuilder();

            // Iterate through retrieved filtered location list and get the desired location code
            foreach (var locationCode in filterList)
            {
                response.AppendFormat("{0}|{1}\n", locationCode, locationCode);
            }

            // Return ContentResult
            //return Content(response.ToString());
            return new SanitizeResult(string.IsNullOrEmpty(response.ToString()) ? "NoItemFound" : response.ToString());
        }

        return new SanitizeResult("NoItemFound");
    }

    /// <summary>
    /// Gets the group HTML.
    /// </summary>
    /// <param name="chargeCodeId">The charge code id.</param>
    /// <param name="chargeCodeTypeId">The charge code type id.</param>
    /// <param name="groupId">The group id.</param>
    /// <param name="groupCurrentIndex">Index of the group current.</param>
    /// <param name="isOptionalGroup">Indicates whether this is an optional group.</param>
    [HttpPost]
    public JsonResult GetGroupHtml(int chargeCodeId, int? chargeCodeTypeId, Guid groupId, int groupCurrentIndex, bool isOptionalGroup)
    {
      var group = new Group(null, Url.Action("GetDynamicFieldAutocompleteData", "Data", new { area = "" }), Url.Content("~/Content/Images/"), "FieldTemplates");
      var groupMetadata = MiscInvoiceManager.GetFieldMetadataForGroup(chargeCodeId, chargeCodeTypeId == 0 ? null : chargeCodeTypeId, groupId, isOptionalGroup);
      var groupHtml = group.GetAjaxGroupHtml(groupMetadata, groupCurrentIndex.ToString(), isOptionalGroup, groupMetadata.Id.ToString()).ToString();

      return Json(new { Html = groupHtml, FunctionName = group.InitializeSubFieldsFunctionName, GroupHtmlDivId = group.GroupDivId });
    }

    /// <summary>
    /// Gets list of dual members.
    /// </summary>
    [OutputCache(Duration = 1200, VaryByParam = "q")]
    public ContentResult GetDualMemberList(string q)
    {
      var response = _memberManager.GetMemberListForUI(q, 0, includePending: true, includeBasic: true, includeRestricted: true, includeTerminated: true, includeOnlyAch: true, includeOnlyIch: true);

      var splitedResponse = response.Split('\n');

      var allMembers = new StringBuilder();

      if (splitedResponse.Length > 0)
      {
        foreach (var t in splitedResponse.Where(t => !string.IsNullOrEmpty(t)))
        {
          allMembers.AppendFormat("{0}", string.Format("{0}{1}", t, '\n'));
        }
      }

      if (!string.IsNullOrEmpty(q) && allMembers.Length == 0)
      {
        var singleMember = q.Split('-');
        if (singleMember.Length > 2)
        {
          var responsesingleMember = _memberManager.GetMemberListForUI(singleMember[1], includeTerminated: true, memberIdToSkip: 0);

          if (responsesingleMember.Length > 0)
          {
            allMembers.AppendFormat("{0}", string.Format("{0}{1}", responsesingleMember, '\n'));
          }
        }
      }

      //return Content(allMembers.ToString());
      var returnList = allMembers.ToString();
      return new SanitizeResult(string.IsNullOrEmpty(returnList) ? "NoItemFound" : returnList);
    }

    /// <summary>
    /// Gets list of Ich members only.
    /// </summary>
    [OutputCache(Duration = 1200, VaryByParam = "q")]
    public ContentResult GetIchMemberList(string q)
    {
      var response = new StringBuilder();

      q = q.ToUpper();

      var selectedList =
        _memberManager.GetIchMemberList().Where(
          member => string.Format("{0}-{1}-{2}", member.MemberCodeAlpha, member.MemberCodeNumeric, member.CommercialName).ToUpper().Contains(q) && member.Id != SessionUtil.MemberId);

      foreach (var member in selectedList)
      {
        response.AppendFormat("{0}|{1}\n", string.Format("{0}-{1}-{2}", member.MemberCodeAlpha, member.MemberCodeNumeric, member.CommercialName), member.Id);
      }

      return new SanitizeResult(response.ToString());
    }

    /// <summary>
    /// This method gets list of members and returns matching list of members only
    /// </summary>
    public ContentResult GetAggregatSponsordMemberList(string q)
    {
      // Retrieve MemberId from Session and use it across the method
      var memberId = SessionUtil.MemberId;

      var response = new StringBuilder();
      IEnumerable<Member> selectedList;
      q = q.ToUpper();

      var category = _iUserManagement.GetUserByUserID(SessionUtil.UserId).CategoryID;

      //if user category is member then get only aggregated and sponsored member list 
      // else get all members from db
      //CMP602
      switch (category)
      {
        case (int)UserCategory.Member:
          selectedList =
            _processingDashboardManager.GetAggregatedSponsoredMemberList(memberId).Where(
              member =>
              string.Format("{0}-{1}-{2}", member.MemberCodeAlpha, member.MemberCodeNumeric, member.CommercialName).
                ToUpper().Contains(q));
          break;
        case (int)UserCategory.AchOps:
        case (int)UserCategory.IchOps:
          selectedList =
            _memberManager.GetMembersBasedOnUserCategory(category,true).Where(
              member =>
              string.Format("{0}-{1}-{2}", member.MemberCodeAlpha, member.MemberCodeNumeric, member.CommercialName).
                ToUpper().Contains(q) && member.Id != memberId);
          break;
        default:
          selectedList =
            _memberManager.GetMembersBasedOnUserCategory(category).Where(
              member =>
              string.Format("{0}-{1}-{2}", member.MemberCodeAlpha, member.MemberCodeNumeric, member.CommercialName).
                ToUpper().Contains(q) && member.Id != memberId);
          break;
      }

      foreach (var member in selectedList)
      {
        response.AppendFormat("{0}|{1}\n", string.Format("{0}-{1}-{2}", member.MemberCodeAlpha, member.MemberCodeNumeric, member.CommercialName), member.Id);
      }

      return new SanitizeResult(response.ToString());
    }

    [HttpPost]
    public JsonResult SetPageSize(int? pageSize)
    {
        //SCP0000: Elmah Exceptions log removal
        SessionUtil.PageSizeSelected = pageSize != null ? (int) pageSize : 5;

        return new JsonResult();
    }


      //Added by Ranjit Kumar 06/04/2011
    // This is for set the current page index on session for maintain current page by clicking the jqgrid navigator.
    [HttpPost]
    public JsonResult SetcurrentPageNo(int currentPage)
    {
      SessionUtil.CurrentPageSelected = currentPage;
      return new JsonResult();
    }

    [HttpGet]
    public JsonResult GetDefaultCurrency(int settlementMethodId, int billingMemberId, int? billedMemberId)
    {
      //SCP0000: Elmah Exceptions log removal
      if (billedMemberId != null)
      {
        var defaultCurrency = InvoiceManager.GetDefaultCurrency(settlementMethodId, billingMemberId, (int)billedMemberId);

        var result = new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = defaultCurrency };

        return result;
      }
      return null;
     }


    [HttpPost]
    public JsonResult GetFormDESamplingConstant(int billingMemId, string provisionalBillingMonthYear, int billedMemberId)
    {
      string[] provisionalBillingMonthTokens = provisionalBillingMonthYear.Split(new[] { '-' });
      int provisionalBillingMonth = Convert.ToInt32(provisionalBillingMonthTokens[1]);
      int provisionalBillingYear = Convert.ToInt32(provisionalBillingMonthTokens[0]);

      var samplingConstantDetails = SamplingFormFManager.GetLinkedFormDESamplingConstant(billingMemId, billedMemberId, provisionalBillingMonth, provisionalBillingYear);
      return Json(samplingConstantDetails);
    }

    [HttpPost]
    public JsonResult GetFormFSamplingConstant(int billingMemId, string provisionalBillingMonthYear, int billedMemberId)
    {
      string[] provisionalBillingMonthTokens = provisionalBillingMonthYear.Split(new[] { '-' });
      int provisionalBillingMonth = Convert.ToInt32(provisionalBillingMonthTokens[1]);
      int provisionalBillingYear = Convert.ToInt32(provisionalBillingMonthTokens[0]);

      var samplingConstantDetails = SamplingFormXfManager.GetLinkedFormFSamplingConstant(billingMemId, billedMemberId, provisionalBillingMonth, provisionalBillingYear);
      return Json(samplingConstantDetails);
    }

    protected override void HandleUnknownAction(string actionName)
    {
      // Log the exception.
      Logger.ErrorFormat("Unknown action method [{0}].", actionName);

      base.HandleUnknownAction(actionName);
    }

    [HttpGet]
    public JsonResult GetMemberLocationDetailsForCargo(string locationCode, string invoiceId, bool isBillingMember, string memberId = null)
    {
      if (string.IsNullOrEmpty(locationCode))
      {
        return null;
      }

      if (memberId != null)
      {
        if (string.IsNullOrEmpty(locationCode.Trim()))
        {
          // get Details for main location
          locationCode = "Main";
        }

        var location = _memberManager.GetMemberDefaultLocation(Convert.ToInt32(memberId), locationCode);

        if (string.IsNullOrEmpty(location.LegalText))
        {
          var eBillingConfig = _memberManager.GetEbillingConfig(location.MemberId);
          location.LegalText = eBillingConfig != null ? eBillingConfig.LegalText : location.LegalText;
        }

        return GetMemberLocationReferenceFromLocation(location);
      }

      var memberLocationDetails = CargoInvoiceManager.GetMemberReferenceData(invoiceId, isBillingMember, locationCode);

      var result = new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = memberLocationDetails };

      return result;
    }

    /// <summary>
    /// Display Rejection stage 1 and 2 only for ACH, for ICH, BIlateral and ACH using IATA rules display rejection stage 1 only.
    /// </summary>
    [HttpPost]
    public JsonResult GetRejectionReasonForTransactionType(int transactionTypeId)
    {
      var rejectionReasonCodeList = Ioc.Resolve<IReasonCodeManager>(typeof(IReasonCodeManager)).GetRejectionReasonCodeList(transactionTypeId);
      var rejectionReasonCodeSelectList = rejectionReasonCodeList.Select(reasonCode => new SelectListItem
      {
        Text =
            reasonCode.Description,
        Value =
            reasonCode.Code.
            ToString()

      }).ToList();

      var result = new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = rejectionReasonCodeSelectList.ToArray() };
      return result;
    }

    [HttpGet]
    public JsonResult GetSubdivisionCodesByCountryCode(string countryCode)
    {
      if (!string.IsNullOrEmpty(countryCode))
      {
        var subdivisionCodeList = _referenceManager.GetSubdivisionCodesByCountryCode(countryCode.ToUpper());
        return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = subdivisionCodeList };
      }

      return null;
    }

    #region CMP #596: Length of Member Accounting Code to be Increased to 12

    /// <summary>
    /// CMP #596: Length of Member Accounting Code to be Increased to 12 
    /// Desc: The list of Members shown in the auto-complete should exclude Type B Members.
    /// For more refer FRS Section 3.4 Point 20 - 20.New auto-complete logic #MW1
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
    [OutputCache(Duration = 1200, VaryByParam = "q")]
    public ContentResult GetMemberListForPaxCgo(string q)
    {
      return GetMemberListData(q, memberToSkip : SessionUtil.MemberId, includeTerminatedMember: true, excludeTypeBMember: true);
    }
    
    
    /// <summary>
    /// Checks if input member id Type-B as per CMP# 596 logic.
    /// </summary>
    /// <param name="autocompleteMemberString">Input is in format - 
    /// "MemberAlpha-MemberNumericCode-MemberCommertialName|MemberId" E.g. - "AC-014-AIR CANADA|11"</param>
    /// <returns></returns>
    private bool GetMemberCodeAndCheckIfItIsTypeB(string autocompleteMemberString)
    {
        try
        {
            if (!string.IsNullOrEmpty(autocompleteMemberString))
            {
                var splitedMemberDetails = autocompleteMemberString.Split('-');
                if (splitedMemberDetails != null && splitedMemberDetails.Length >= 1)
                {
                    /* MemberNumericCode is expected to be at index 1 */
                    if (!string.IsNullOrEmpty(splitedMemberDetails[1]))
                    {
                        return InvoiceManager.IsTypeBMember(splitedMemberDetails[1]);
                    } //end of if (!string.IsNullOrEmpty(splitedMemberDetails[1])) i.e. - Member code numeric is obtained for further investigation.
                } //end of if (splitedMemberDetails != null && splitedMemberDetails.Length >= 1) i.e. - input is valid.
            } //end of if (!string.IsNullOrEmpty(autocompleteMemberString)) i.e. - input is not empty.
        }
        catch (Exception exception)
        {
            Logger.Info(string.Format("Error while checking, if {0} is Type-B member or not.", autocompleteMemberString));
            Logger.Error(exception);
        }

        /* By default member will not be treated as a Type-B member */
        return false;
    }

    /// <summary>
    /// CMP #596: Length of Member Accounting Code to be Increased to 12 
    /// Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater 
    /// Ref: FRS Section 3.4 Point 23.
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
    [HttpGet]
    [OutputCache(Duration = 1200, VaryByParam = "q")]
    public ContentResult GetTicketIssuingAirlineListForPaxCgo(string q)
    {
        bool resultNumeric = true;
        int ii = 0;
        if (!string.IsNullOrWhiteSpace(q))
            resultNumeric = int.TryParse(q, out ii);

        if (resultNumeric)
        {
            var memberCodeList =
                from airline in
                    _memberManager.GetMemberListFromDB().Where(
                        member => member.MemberCodeNumeric.ToUpper().Contains(q.ToUpper())
                        && member.MemberCodeNumeric.Length < 5)
                select airline.MemberCodeNumeric.PadLeft(3, '0');

            var response = new StringBuilder();
            foreach (var membercode in memberCodeList)
            {
                int i = 0;
                bool result = int.TryParse(membercode, out i);
                if (result)
                    response.AppendFormat("{0}|{1}\n", membercode, membercode);
            }


            return new SanitizeResult(response.ToString());


        }
        var responses = new StringBuilder();
        return new SanitizeResult(responses.ToString());
    }
    
    #endregion

    /* CMP #675: Progress Status Bar for Processing of Billing Data Files.
    * Desc: SP called to get file procssing progress detail. */
    public JsonResult GetFileProgressDetails(string fileLogId)
    {
        string ProcessName = string.Empty;
        string ProcessState = string.Empty;
        int QueuePosition = 0;

        /* Call "GetFileProgressDetails()" method which will Execute stored procedure PROC_GET_FILE_PROGRESS_DETAILS */
        var _dashboardManager = Ioc.Resolve<IProcessingDashboardManager>(typeof(IProcessingDashboardManager));
        var operationStatus = _dashboardManager.GetFileProgressDetails(Guid.Parse(fileLogId), ref ProcessName, ref ProcessState,
                                                 ref QueuePosition);

        /* Return JsonResult */
        /* Return JsonResult */
        if (operationStatus == true)
        {
            var result =
                new { IsSuccess = true, Process = ProcessName, State = ProcessState, Position = QueuePosition };
            return Json(result);
        }
        else
        {
            var result =
                new { IsSuccess = false, Process = ProcessName, State = ProcessState, Position = QueuePosition };
            return Json(result);
        }
    }
  }
}



