using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.OfflineReportLog
{
	public class OfflineReportLog : EntityBase<Guid>
	{
		public int OfflineReportId {
			get;
			set;
		}
		public DateTime RequestDateTime {
			get;
			set;
		}
		public int UserId {
			get;
			set;
		}
		public int MemberId {
			get;
			set;
		}
		public string SearchCriterion {
			get;
			set;
		}
		public DateTime? ReportGenerateDateTime {
			get;
			set;
		}
		public int IsDownload {
			get;
			set;
		}
		public DateTime? DownloadDateTime {
			get;
			set;
		}
		public Guid? DownloadLinkId {
			get;
			set;
		}
	}
}
