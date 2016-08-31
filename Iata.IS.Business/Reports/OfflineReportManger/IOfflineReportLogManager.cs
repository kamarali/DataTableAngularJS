using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.OfflineReportLog;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Reports.OfflineReportManger
{
  /// <summary>
	/// Offline Report Log Manager Interface.
	/// Handle insert and update operation.
  /// </summary>
	public interface IOfflineReportLogManager
	{
		void AddOfflineReportLog(OfflineReportLog offlineReportLog);

		void UpdateOfflineReportLog(OfflineReportLog offlineReportLog);

  	IsHttpDownloadLink GetHttpDownloadLinkDetail(Guid httpDownloadId);

  	void AddHttpDownloadLink(IsHttpDownloadLink httpDownloadlink);

  	OfflineReportLog GetOfflineReportLog(Guid offlineReportLogid);

  	List<OfflineReport> GetAllOfflineReport();

  	String GetOfflineReportName(int reportId);

		void DeleteOfflineReportLogRow(Guid offlineReportLogId);
	}
}
