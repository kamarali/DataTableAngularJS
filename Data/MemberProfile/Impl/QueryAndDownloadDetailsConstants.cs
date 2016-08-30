using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Data.MemberProfile.Impl
{
  internal static class QueryAndDownloadDetailsConstants
  {
    #region GetProfileMetadataAvailableFields Constants

    public const string UserCategoryIdParameterName = "user_Category_Id";
    public const string ReportTypeParameterName = "report_Type";
    public const string IsOwnMemberParameterName = "ISOWNMEMBER";
    public const string GetAvailableFieldsFunctionName = "GetAvailableFields";

    #endregion
    
    #region  GetMemberContactDetails Constants

    public const string GetMemberContactDetailsFunctionName = "PROC_GET_MEM_CONTACT_DETAILS";
    //public const string UserCategoryIdParameterName = "USER_CATEGORY_ID";
    //public const string ReportTypeParameterName = "REPORT_TYPE";
    public const string MetaIdParameterName = "METAIDLIST_I";
    public const string ContactTypeMetaIdListParameterName = "CONTACTTYPEMETAIDLIST_I";
    public const string MemberIdParameterName = "MEMBER_ID_I";
    public const string CountryIdParameterName = "COUNTRY_ID_I";
    public const string AchParameterName = "ACH_I";
    public const string IchParameterName = "ICH_I";
    public const string DualParameterName = "DUAL_I";
    public const string NonChParameterName = "NONCH_I";
    public const string IataParameterName = "IATA_I";
    public const string ContactIdParameterName = "CONTACT_ID_I";
    public const string EmailIdParameterName = "EMAIL_I";
    public const string GroupIdListParameterName = "GROUPIDLIST_I";
    public const string SubGroupIdListParameterName = "SUBGROUPIDLIST_I";
    public const string ContactTypeIdListParameterName = "CONTACTTYPEIDLIST_I";
    public const string SortIdListParameterName = "SORTIDLIST_I";
    public const string SortOrderListParameterName = "SORTORDERLIST_I";
    public const string PageNumberParameterName = "PAGE_NO_I";
    public const string PageSizeParameterName = "PAGE_SIZE_I";
    public const string TotalRecordCountParameterName = "RECORD_COUNT_O";
    public const string IsDownLoadParameterName = "ISDOWNLOAD";
    #endregion 

    
    #region GetProfileMetadataAvailableFields Constants

    public const string MetaIdListParameterName = "METAIDLIST_I";
    public const string DisplayFieldNamesParameterName = "FIELDNAMES_O";
    public const string GetMemberProfileFieldNamesFunctionName = "GetMemberProfileFieldNames";

    #endregion 
  }
}
