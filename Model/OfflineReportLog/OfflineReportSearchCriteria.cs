using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.OfflineReportLog
{
	public class OfflineReportSearchCriteria
	{
		public int ReportId
		{
			get;
			set;
		}
		public DateTime? RequestDateTime
		{
			get;
			set;
		}
		public int UserId
		{
			get;
			set;
		}
		public int MemberId
		{
			get;
			set;
		}
	}
}
