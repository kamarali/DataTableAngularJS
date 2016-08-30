using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.OfflineReportLog;

namespace Iata.IS.Data.OfflineReport
{
	public interface IManageOfflineReportRepository
	{
		List<OfflineReportSearchData> GetOfflineReportLog(OfflineReportSearchCriteria searchCriteria,int temporaryFilesPurgePeriod);
	}
}
