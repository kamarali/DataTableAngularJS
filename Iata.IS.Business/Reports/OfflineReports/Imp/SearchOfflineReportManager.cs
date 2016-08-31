using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.OfflineReport.Imp;
using OfflineReportLog = Iata.IS.Model.OfflineReportLog;
using Iata.IS.Data.OfflineReport;
using Iata.IS.AdminSystem;

namespace Iata.IS.Business.Reports.OfflineReports.Imp
{
  /// <summary>
  /// Search offline report manager.
  /// </summary>
	public class SearchOfflineReportManager : ISearchOfflineReportManager
	{
		private readonly IManageOfflineReportRepository _manageOfflineReportRepository;

		/// <summary>
		/// Intialize manage offlinereport repository.
		/// </summary>
		/// <param name="manageOfflineReportRepository"></param>
		public SearchOfflineReportManager(IManageOfflineReportRepository manageOfflineReportRepository)
		{
			_manageOfflineReportRepository = manageOfflineReportRepository;
		}

	  /// <summary>
	  /// This function is used for fetch data from database based on search criteria.
	  /// </summary>
	  /// <param name="searchCriteria"></param>
	  /// <returns></returns>
		public List<OfflineReportLog.OfflineReportSearchData> GetOfflineReportLogDetail(OfflineReportLog.OfflineReportSearchCriteria searchCriteria, int temporaryFilesPurgePeriod)
		{
		var filteredList = _manageOfflineReportRepository.GetOfflineReportLog(searchCriteria, temporaryFilesPurgePeriod);
			return filteredList;
		}
	}
}
