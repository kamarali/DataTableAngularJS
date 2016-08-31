using System.Collections.Generic;
using System.Data;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Business.MemberProfile
{
  /// <summary>
  /// 
  /// </summary>
  public interface IQueryAndDownloadDetailsManager
  {

    /// <summary>
    /// This method will get all contact types
    /// </summary>
    /// <param name="groupIdList">group id list</param>
    /// <param name="subGroupIdList"> sub group id list</param>
    /// <returns>List of contact type</returns>
    List<ContactType> GetContactTypeList(List<int> groupIdList, List<int> subGroupIdList);

    /// <summary>
    /// Get available fields for given user category id and report type.
    /// </summary>
    /// <param name="userCategoryId">user category id</param>
    /// <param name="reportType">report type</param>
    /// <param name="isOwnMember">flag to indivate that own member</param>
    /// <returns>List of available fields</returns>
    List<ProfileMetadataAvailableFields> GetProfileMetadataAvailableFields(int userCategoryId, int reportType, bool isOwnMember);

    /// <summary>
    /// Search the member and contact details as per given search criteria
    /// </summary>
    /// <param name="searchCriteria">search criteria.</param>
    /// <param name="isHierarchical">boolean flag to process data to remove repeated member information for contact reports.</param>
    /// <param name="totalRecordCount">total record count</param>
    /// <param name="isDownload"></param>
    /// <returns></returns>
    DataSet SearchMemberContactDetails(QueryAndDownloadSearchCriteria searchCriteria, bool isHierarchical, out int totalRecordCount,bool isDownload);

    DataSet GetMemberDetails();

    string SearchMemberContactDetails(QueryAndDownloadSearchCriteria searchCriteria, out int totalRecordCount, bool isDownloadCsv);

    /// <summary>
    /// Get the display name list for given member profile meta ids and contact type ids.
    /// </summary>
    /// <param name="metaIdList">comma separated list of member profile meta ids and contact type ids</param>
    /// <returns>comma separated list of display names.</returns>
    string GetMemberProfileFieldNames(string metaIdList);

    /// <summary>
    /// This method is used to Convert DataTable to html string.
    /// </summary>
    /// <param name="searchMemberContactDetails">data set</param>
    /// <returns>output html</returns>
    string ConvertDataTableToHtmlString(DataSet searchMemberContactDetails);

    /// <summary>
    /// This method generates PDF File based on Input parameters,
    /// and returns the path of the generated PDF File
    /// </summary>
    /// <param name="inputHtml">HTML string to be used for generating html file, if not generated earlier</param>
    /// <param name="fileName">Input file name</param>
    /// <param name="htmlToPdfExePath">path of htmlToPdf executable</param>
    /// <returns></returns>
    byte[] ConvertHtmlToPdf(string inputHtml, string fileName, string htmlToPdfExePath);

    /// <summary>
    /// This method is used to Convert DataTable to CSV ( comma separated ) file.
    /// </summary>
    /// <param name="table">data table</param>
    /// <param name="fileName"> output file name</param>
    /// <param name="separateChar"></param>
    /// <returns>output csv file path</returns>
    string ConvertDataTableToCsv(DataTable table, string fileName, string separateChar);
  }
}
