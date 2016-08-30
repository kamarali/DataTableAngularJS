using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Data.Impl;
using Iata.IS.Model.MemberProfile;

namespace Iata.IS.Data.MemberProfile.Impl
{
  public class QueryAndDownloadDetailsRepository : Repository<ProfileMetadata>, IQueryAndDownloadDetailsRepository
  {
    public List<ProfileMetadataAvailableFields> GetProfileMetadataAvailableFields(int userCategoryId, int reportType, bool isOwnMember)
    {

      var parameters = new ObjectParameter[3];
      parameters[0] = new ObjectParameter(QueryAndDownloadDetailsConstants.UserCategoryIdParameterName, userCategoryId);
      parameters[1] = new ObjectParameter(QueryAndDownloadDetailsConstants.ReportTypeParameterName, reportType);
      parameters[2] = new ObjectParameter(QueryAndDownloadDetailsConstants.IsOwnMemberParameterName, typeof(int) ){Value = isOwnMember};

      var list = ExecuteStoredFunction<ProfileMetadataAvailableFields>(QueryAndDownloadDetailsConstants.GetAvailableFieldsFunctionName, parameters);
      return list.ToList();
    }

    public DataSet SearchMemberContactDetails(QueryAndDownloadSearchCriteria searchCriteria, out int totalRecordCount,bool isDownload)
    {
      var ds = new DataSet();
      using (var connection = new OracleConnection(Core.Configuration.ConnectionString.Instance.ServiceConnectionString))
      {
        var command = new OracleCommand(QueryAndDownloadDetailsConstants.GetMemberContactDetailsFunctionName, connection)
        {
          CommandType = CommandType.StoredProcedure
        };
        int userCategoryId, reportType, memberId, contactId, emailId;
        command.Parameters.Add(QueryAndDownloadDetailsConstants.UserCategoryIdParameterName, (int.TryParse(searchCriteria.UserCategoryId, out userCategoryId) ? (int?)userCategoryId : null));
        command.Parameters[QueryAndDownloadDetailsConstants.UserCategoryIdParameterName].Direction = ParameterDirection.Input;
        command.Parameters.Add(QueryAndDownloadDetailsConstants.ReportTypeParameterName, (int.TryParse(searchCriteria.ReportType, out reportType) ? (int?)reportType : null));
        command.Parameters[QueryAndDownloadDetailsConstants.ReportTypeParameterName].Direction = ParameterDirection.Input;
        command.Parameters.Add(QueryAndDownloadDetailsConstants.IsOwnMemberParameterName, searchCriteria.ISOwnMember);
        command.Parameters[QueryAndDownloadDetailsConstants.IsOwnMemberParameterName].Direction = ParameterDirection.Input;

        command.Parameters.Add(QueryAndDownloadDetailsConstants.MetaIdParameterName, searchCriteria.MetaIdList);
        command.Parameters[QueryAndDownloadDetailsConstants.MetaIdParameterName].Direction = ParameterDirection.Input;

        command.Parameters.Add(QueryAndDownloadDetailsConstants.MemberIdParameterName, (int.TryParse(searchCriteria.MemberId, out memberId) ? (int?)memberId : null));
        command.Parameters[QueryAndDownloadDetailsConstants.MemberIdParameterName].Direction = ParameterDirection.Input;
        command.Parameters.Add(QueryAndDownloadDetailsConstants.CountryIdParameterName, searchCriteria.CountryId);
        command.Parameters[QueryAndDownloadDetailsConstants.CountryIdParameterName].Direction = ParameterDirection.Input;
        command.Parameters.Add(QueryAndDownloadDetailsConstants.AchParameterName, searchCriteria.ISAch);
        command.Parameters[QueryAndDownloadDetailsConstants.AchParameterName].Direction = ParameterDirection.Input;
        command.Parameters.Add(QueryAndDownloadDetailsConstants.IchParameterName, searchCriteria.ISIch);
        command.Parameters[QueryAndDownloadDetailsConstants.IchParameterName].Direction = ParameterDirection.Input;
        command.Parameters.Add(QueryAndDownloadDetailsConstants.DualParameterName, searchCriteria.ISDual);
        command.Parameters[QueryAndDownloadDetailsConstants.DualParameterName].Direction = ParameterDirection.Input;
        command.Parameters.Add(QueryAndDownloadDetailsConstants.NonChParameterName, searchCriteria.ISNonCh);
        command.Parameters[QueryAndDownloadDetailsConstants.NonChParameterName].Direction = ParameterDirection.Input;
        command.Parameters.Add(QueryAndDownloadDetailsConstants.IataParameterName, searchCriteria.ISIata);
        command.Parameters[QueryAndDownloadDetailsConstants.IataParameterName].Direction = ParameterDirection.Input;
        command.Parameters.Add(QueryAndDownloadDetailsConstants.ContactIdParameterName, (int.TryParse(searchCriteria.ContactId, out contactId) ? (int?)contactId : null));
        command.Parameters[QueryAndDownloadDetailsConstants.ContactIdParameterName].Direction = ParameterDirection.Input;
        command.Parameters.Add(QueryAndDownloadDetailsConstants.EmailIdParameterName, (int.TryParse(searchCriteria.EmailId, out emailId) ? (int?)emailId : null));
        command.Parameters[QueryAndDownloadDetailsConstants.EmailIdParameterName].Direction = ParameterDirection.Input;

        command.Parameters.Add(QueryAndDownloadDetailsConstants.GroupIdListParameterName, searchCriteria.GroupIdList);
        command.Parameters[QueryAndDownloadDetailsConstants.GroupIdListParameterName].Direction = ParameterDirection.Input;
        command.Parameters.Add(QueryAndDownloadDetailsConstants.SubGroupIdListParameterName, searchCriteria.SubGroupIdList);
        command.Parameters[QueryAndDownloadDetailsConstants.SubGroupIdListParameterName].Direction = ParameterDirection.Input;
        command.Parameters.Add(QueryAndDownloadDetailsConstants.ContactTypeIdListParameterName, searchCriteria.ContactTypeIdList);
        command.Parameters[QueryAndDownloadDetailsConstants.ContactTypeIdListParameterName].Direction = ParameterDirection.Input;

        command.Parameters.Add(QueryAndDownloadDetailsConstants.SortIdListParameterName, searchCriteria.SortIds);
        command.Parameters[QueryAndDownloadDetailsConstants.SortIdListParameterName].Direction = ParameterDirection.Input;
        command.Parameters.Add(QueryAndDownloadDetailsConstants.SortOrderListParameterName, searchCriteria.SortOrder);
        command.Parameters[QueryAndDownloadDetailsConstants.SortOrderListParameterName].Direction = ParameterDirection.Input;

        command.Parameters.Add(QueryAndDownloadDetailsConstants.IsDownLoadParameterName, isDownload);
        command.Parameters[QueryAndDownloadDetailsConstants.IsDownLoadParameterName].Direction = ParameterDirection.Input;

        int number;
        command.Parameters.Add(QueryAndDownloadDetailsConstants.PageNumberParameterName, (int.TryParse(searchCriteria.PageNumber, out number) ? (int?)number : null));
        command.Parameters[QueryAndDownloadDetailsConstants.PageNumberParameterName].Direction = ParameterDirection.Input;
        command.Parameters.Add(QueryAndDownloadDetailsConstants.PageSizeParameterName, (int.TryParse(searchCriteria.PageSize, out number) ? (int?)number : null));
        command.Parameters[QueryAndDownloadDetailsConstants.PageSizeParameterName].Direction = ParameterDirection.Input;

        //output parameter for total record count.
        command.Parameters.Add(QueryAndDownloadDetailsConstants.TotalRecordCountParameterName, 0);
        command.Parameters[QueryAndDownloadDetailsConstants.TotalRecordCountParameterName].Direction = ParameterDirection.Output;


        var adapter = new OracleDataAdapter(command);

        adapter.Fill(ds, "SearchResult");

        totalRecordCount = 0;
        int.TryParse(command.Parameters[QueryAndDownloadDetailsConstants.TotalRecordCountParameterName].Value.ToString(), out totalRecordCount);

      }
      var contactTypeColumnNameList = new List<string>();
      if (ds.Tables.Count == 2 && searchCriteria.ReportType != "3")
      {
        foreach (DataRow row in ds.Tables[1].Rows)
        {
          if (ds.Tables[0].Columns.Contains(row["ID"].ToString()))
          {
            ds.Tables[0].Columns[row["ID"].ToString()].Caption = row["NAME"].ToString();
            if (row["ID"].ToString().StartsWith("T")) contactTypeColumnNameList.Add(row["ID"].ToString());
          }
        }
      }

      if (searchCriteria.ReportType == "3")
      {
        const string checkedCheckboxHtml = "<input type='checkbox' disabled='disabled' checked='checked' />";
        const string uncheckedCheckboxHtml = "<input type='checkbox' disabled='disabled' />";
        foreach (DataRow dataRow in ds.Tables[0].Rows)
        {
          foreach (var contactTypeColumnName in contactTypeColumnNameList)
          {
            dataRow[contactTypeColumnName] = Convert.ToInt32(dataRow[contactTypeColumnName]) > 0 ? checkedCheckboxHtml : uncheckedCheckboxHtml;
          }
        }
      }

      return ds;
    }

    /// <summary>
    /// Get the display name list for given member profile meta ids and contact type ids.
    /// </summary>
    /// <param name="metaIdList">comma separated list of member profile meta ids and contact type ids</param>
    /// <returns>comma separated list of display names.</returns>
    public string GetMemberProfileFieldNames(string metaIdList)
    {
      var parameters = new ObjectParameter[2];
      parameters[0] = new ObjectParameter(QueryAndDownloadDetailsConstants.MetaIdListParameterName, typeof(string)) { Value = metaIdList };
      parameters[1] = new ObjectParameter(QueryAndDownloadDetailsConstants.DisplayFieldNamesParameterName, typeof(string));
      ExecuteStoredProcedure(QueryAndDownloadDetailsConstants.GetMemberProfileFieldNamesFunctionName, parameters);

      return parameters[1].Value.ToString();

    }

  }
}
