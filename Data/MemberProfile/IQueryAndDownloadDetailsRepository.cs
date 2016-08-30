using System.Collections.Generic;
using System.Data;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile
{
  public interface IQueryAndDownloadDetailsRepository : IRepository<ProfileMetadata>
  {
    List<ProfileMetadataAvailableFields> GetProfileMetadataAvailableFields(int userCategoryId, int reportType, bool isOwnMember);

    DataSet SearchMemberContactDetails(QueryAndDownloadSearchCriteria searchCriteria, out int totalRecordCount, bool isDownloadCsv);

    /// <summary>
    /// Get the display name list for given member profile meta ids and contact type ids.
    /// </summary>
    /// <param name="metaIdList">comma separated list of member profile meta ids and contact type ids</param>
    /// <returns>comma separated list of display names.</returns>
    string GetMemberProfileFieldNames(string metaIdList);
  }

}
