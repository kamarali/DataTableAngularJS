using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Model.OfflineReportLog;
using Iata.IS.Data.Impl;

namespace Iata.IS.Data.OfflineReport.Imp
{
  /// <summary>
  /// This class is used for fetch data from database based on search criteria.
  /// </summary>
	public class ManageOfflineReportRepository : Repository<OfflineReportLog>, IManageOfflineReportRepository
	{
    /// <summary>
		/// This function is used for fetch data from database based on search criteria.
    /// </summary>
    /// <param name="offlineReportSearchCriteria"></param>
    /// <returns></returns>
		public List<OfflineReportSearchData> GetOfflineReportLog(OfflineReportSearchCriteria offlineReportSearchCriteria,int temporaryFilesPurgePeriod)
    {
    	try
    	{
    		var parameters = new ObjectParameter[5];
    		parameters[0] = new ObjectParameter("OFFLINE_REPORT_ID_I", typeof (int))
    		                	{
    		                		Value = offlineReportSearchCriteria.ReportId
    		                	};
    		parameters[1] = new ObjectParameter("REQUEST_DATETIME_I", typeof (int))
    		                	{
    		                		Value =
    		                			offlineReportSearchCriteria.RequestDateTime == null
    		                				? null
    		                				: offlineReportSearchCriteria.RequestDateTime.Value.ToString("dd-MM-yy")
    		                	};
    		parameters[2] = new ObjectParameter("USER_ID_I", typeof (int))
    		                	{
    		                		Value = offlineReportSearchCriteria.UserId
    		                	};

    		parameters[3] = new ObjectParameter("MEMBER_ID_I", typeof (int))
    		                	{
    		                		Value = offlineReportSearchCriteria.MemberId
    		                	};
    		parameters[4] = new ObjectParameter("RETENTION_PERIOD_I", typeof (int))
    		                	{
    		                		Value = temporaryFilesPurgePeriod
    		                	};
				

    		var returnData = ExecuteStoredFunction<OfflineReportSearchData>("GetOfflineReportLogs", parameters);
    		return returnData.ToList();
    	}
    	catch (Exception ex)
    	{
    		Logger.ErrorFormat(
    			"Error occured while fecting data from database based on search criteria. Execption caught: {0}", ex);
    		return new List<OfflineReportSearchData>();
    	}
    }
	}
}
