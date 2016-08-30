using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Iata.IS.Core.Exceptions;
using Iata.IS.Web.Util;
using Iata.IS.Model.OfflineReportLog;
using Iata.IS.Web.UIModel.Grid.Reports;
using Iata.IS.Business.Reports.OfflineReports;
using System.IO;
using Iata.IS.Core;
using Iata.IS.Web.Util.Filters;
using Iata.IS.Business.Reports.OfflineReportManger;
using log4net;
using System.Reflection;
using Iata.IS.AdminSystem;

namespace Iata.IS.Web.Areas.Reports.Controllers
{
	/// <summary>
	/// This class used for display offline report detail.
	/// </summary>
	public class OfflineReportsController : ISController
	{
		private const string SearchResultGridAction = "SearchResultGridData";
		private readonly ISearchOfflineReportManager _searchOfflinereportManager;
		private readonly IOfflineReportLogManager _offlineReportLogManager;
		private readonly ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public OfflineReportsController(ISearchOfflineReportManager searchInvoiceManager, IOfflineReportLogManager offlineReportLogManager)
		{
			_searchOfflinereportManager = searchInvoiceManager;
			_offlineReportLogManager = offlineReportLogManager;
		}

	 
		/// <summary>
		/// This action is used for display offline report log detail.
		/// </summary>
		/// <param name="searchCriteria">search criteria</param>
		/// <returns></returns>
		public ActionResult Index(OfflineReportSearchCriteria searchCriteria)
		{
			try
			{
				string criteria = string.Empty;
				if (searchCriteria != null)
				{
					criteria = new JavaScriptSerializer().Serialize(searchCriteria);
				}

				//Create offline report search gerid instance.
				var offlineReportSearchResultGrid = new OfflineReportSearchGrid(ControlIdConstants.SearchGrid,
				                                                                Url.Action(SearchResultGridAction,
				                                                                           new
				                                                                           	{
				                                                                           		criteria
				                                                                           	}));

				ViewData[ViewDataConstants.SearchGrid] = offlineReportSearchResultGrid.Instance;
				ViewData[ViewDataConstants.ReportId] = searchCriteria.ReportId == 0 ? -1 : searchCriteria.ReportId;
				if (searchCriteria.ReportId == 0)
					ViewData[ViewDataConstants.RequestDateTime] = DateTime.UtcNow.ToString(FormatConstants.DateFormat);
				else
					ViewData[ViewDataConstants.RequestDateTime] = searchCriteria.RequestDateTime != null ? searchCriteria.RequestDateTime.Value.ToString(FormatConstants.DateFormat) : null;
			}
			catch (ISBusinessException exception)
			{
				//Show error message.
				ShowErrorMessage(exception.ErrorCode);
			}

			return View(searchCriteria);
		}

		/// <summary>
		/// Fetch offline report log searched result and display it in grid
		/// </summary>
		/// <returns></returns>
		public JsonResult SearchResultGridData(string criteria)
		{
			var searchCriteria = new OfflineReportSearchCriteria();

			if (!string.IsNullOrEmpty(criteria))
			{
				searchCriteria =
					new JavaScriptSerializer().Deserialize(criteria, typeof (OfflineReportSearchCriteria)) as
					OfflineReportSearchCriteria;
			}

			// Create grid instance and retrieve data from database
			var offlineReportSearchResultGrid = new OfflineReportSearchGrid(ControlIdConstants.SearchGrid,
			                                                                Url.Action(SearchResultGridAction,
			                                                                           new
			                                                                           	{
			                                                                           		searchCriteria
			                                                                           	}));
			searchCriteria.RequestDateTime = searchCriteria.RequestDateTime != null ? searchCriteria.RequestDateTime.Value.ToLocalTime() : searchCriteria.RequestDateTime;
      //Set user Id and Member Id
			searchCriteria.UserId = SessionUtil.UserId;
			searchCriteria.MemberId = SessionUtil.MemberId;
		//	ViewData[ViewDataConstants.ReportId] = searchCriteria.ReportId;
		//	ViewData[ViewDataConstants.RequestDateTime] = searchCriteria.RequestDateTime!= null ? searchCriteria.RequestDateTime.Value.ToString(FormatConstants.DateFormat) : null;

			_logger.InfoFormat("Fetching data from database based on search criteria parameters, params {0}", searchCriteria);
      //Fetch data from database based on search criteria.
			var offlineReportData =
				_searchOfflinereportManager.GetOfflineReportLogDetail(searchCriteria, SystemParameters.Instance.PurgingPeriodDetails.TemporaryFilesPurgePeriod).AsQueryable();

			_logger.InfoFormat("Finished execution of offline report");

			return offlineReportSearchResultGrid.DataBind(offlineReportData);
		}

		/// <summary>
		/// This function is used for download report.
		/// </summary>
		/// <param name="downloadId"></param>
		/// <param name="offlineReportLogId"></param>
		/// <returns></returns>
		[HttpGet]
		public ActionResult DownloadReport(String downloadId,String offlineReportLogId)
		{
			try
			{
				//Get Httpdownlodlink data based on the download id.
				var httpDownloadData =
					_offlineReportLogManager.GetHttpDownloadLinkDetail(downloadId.ToGuid());
        
        //Fetch offline report log data base on offlinereportlog id and update is_download and download datetime columns.
				var offlineReportLogData = _offlineReportLogManager.GetOfflineReportLog(offlineReportLogId.ToGuid());
				if (offlineReportLogData.IsDownload != 1)
				{
					offlineReportLogData.IsDownload = 1;
					offlineReportLogData.DownloadDateTime = DateTime.UtcNow;
					_offlineReportLogManager.UpdateOfflineReportLog(offlineReportLogData);
				}

				return File(httpDownloadData.FilePath, "application/zip", Path.GetFileName(httpDownloadData.FilePath));
			}
			catch(Exception ex)
			{
        _logger.ErrorFormat("Error occured while download zip file, Exception: {0}", ex);
				return null;
			}
		}
	}
}