using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OfflineReportLog = Iata.IS.Model.OfflineReportLog;

namespace Iata.IS.Business.Reports.OfflineReports
{
	public interface ISearchOfflineReportManager
	{
		List<OfflineReportLog.OfflineReportSearchData> GetOfflineReportLogDetail(OfflineReportLog.OfflineReportSearchCriteria searchCriteria, int temporaryFilesPurgePeriod);
	}
}
