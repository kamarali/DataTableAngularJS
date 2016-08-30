using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Data.MemberProfile.Impl
{
  public class ContactConstants
  {

    #region  GetContactUserInformation Constants

    public const string GetContactUserInformationFunctionName = "GetContactUserInformation";
    public const string MemberIdParameterName = "MEMBER_ID_I";
    public const string FirstNameParameterName = "FIRST_NAME_I";
    public const string LastNameParameterName = "LAST_NAME_I";
    public const string EmailIdParameterName = "EMAIL_ADDRESS_I";
    public const string StaffIdParameterName = "STAFF_ID_I";
    public const string UserCategoryIdParameterName = "USER_CATEGORY_ID_I";

    //CMP#655(2.1.3)IS-WEB Display per Location
     public const string UserIdParameterName = "USER_ID_I";
     public const string GetUserContactFunctionName = "GetUserContactForLocation";
     public const string GetUserContactLocFunctionName = "GetUserContactAssignedLocation";
     public const string GetOwnLocFunctionName = "GetOwnAssignedLocation";
     public const string GetContactsForMiscOutputAlertsFunctionName = "GetContactsForMiscOutputAlerts";

    //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues-->
    public const string GetContactMemberInformationFunctionName = "GetContactMemberInformation";
    public const string MemberIdForContactParameterName = "PMEMBER_ID_I";
    public const string ContactTypeIdParameterName = "PCONTACT_TYPE_ID";
    public const string BillingCategoryParameterName = "BILLING_CATEGORY_I";
    public const string MiscLocCodeParameterName = "MISC_LOCATION_CODE_I";
    public const string GetMemberAssociationLocation = "GetMemberAssociationLocation";

    //CMP #666: MISC Legal Archiving Per Location ID
    public const string GetArchivalLocations = "GetArchivalLocations";
    public const string ArchivalType = "ARCHIVAL_TYPE_I";
    #endregion 

      
      
  }
}
