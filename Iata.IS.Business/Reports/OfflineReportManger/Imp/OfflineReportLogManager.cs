using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data;
using Iata.IS.Model.OfflineReportLog;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Reports.OfflineReportManger.Imp
{
	/// <summary>
	/// Offline report Log manager.
	/// </summary>
	public class OfflineReportLogManager : IOfflineReportLogManager
	{
		public IRepository<OfflineReportLog> OffileReportLogRepository {
			get;
			set;
		}

		public IRepository<IsHttpDownloadLink> IsHttpDownloadLinkRepository
		{
			get;
			set;
		}

		public IRepository<OfflineReport> OfflineReportRepository { get; set; }

		/// <summary>
		/// This function is used to insert data into offline report log table.
		/// </summary>
		/// <param name="offlineReportLog"></param>
		public void AddOfflineReportLog(OfflineReportLog offlineReportLog)
		{
	    //insert new object in offline report log table.
			OffileReportLogRepository.Add(offlineReportLog);

			//Commit offline report log detail.
			UnitOfWork.CommitDefault();
		}

		/// <summary>
		/// This function is used to get HttpDownload link detail.
		/// </summary>
		/// <param name="httpDownloadId"></param>
		public IsHttpDownloadLink GetHttpDownloadLinkDetail(Guid httpDownloadId)
		{
      //return http download link data based on httpdownload id.
			return IsHttpDownloadLinkRepository.First(hd => hd.Id == httpDownloadId);
		}

		/// <summary>
		/// This function is used to insert data into HttpDownload link table.
		/// </summary>
		/// <param name="httpDownloadlink"></param>
		public void AddHttpDownloadLink(IsHttpDownloadLink httpDownloadlink)
		{
			//Insert new object in http download link table.
			IsHttpDownloadLinkRepository.Add(httpDownloadlink);

			//Commit offline report log detail.
			UnitOfWork.CommitDefault();
		}
	
		/// <summary>
		/// This function is used to update data in the offline report log table.
		/// </summary>
		/// <param name="offlineReportLog"></param>
		public void UpdateOfflineReportLog(OfflineReportLog offlineReportLog)
    {
    	// Update offline report log data.
    	OffileReportLogRepository.Update(offlineReportLog);

			//Commit offline report log detail.
			UnitOfWork.CommitDefault();
    }

		/// <summary>
		/// This function is used to delete data from the offline report log table.
		/// </summary>
		/// <param name="offlineReportLogId"></param>
		public void DeleteOfflineReportLogRow(Guid offlineReportLogId)
		{
			// Delete offline report log data.
			OffileReportLogRepository.Delete(GetOfflineReportLog(offlineReportLogId));

			//Commit offline report log detail.
			UnitOfWork.CommitDefault();
		}

		/// <summary>
		/// Get offline record log data based on offlinereport log id.
		/// </summary>
		/// <param name="offlineReportLogid"></param>
		/// <returns></returns>
		public OfflineReportLog GetOfflineReportLog(Guid offlineReportLogid)
		{
      //Return offline report log data based on offline report log id.
			return OffileReportLogRepository.Get(orlr => orlr.Id == offlineReportLogid).FirstOrDefault();
		}

   /// <summary>
   /// This function is used for get all offline report details.
   /// </summary>
   /// <returns></returns>
   public List<OfflineReport> GetAllOfflineReport()
   {
    //return all row from offline report table.
   	return OfflineReportRepository.GetAll().ToList();
   }

		/// <summary>
	 /// This function is used for get offlline report name based on report id.
	 /// </summary>
	 /// <returns></returns>
	 public String GetOfflineReportName(int reportId)
	 {
    //Return offline_report name based on report id.
	 	return OfflineReportRepository.Get(or => or.Id == reportId).Select(or => or.Name).FirstOrDefault();
	 }
	}
}
