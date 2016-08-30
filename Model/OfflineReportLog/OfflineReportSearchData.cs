using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Model.OfflineReportLog
{
	public class OfflineReportSearchData
	{
		public Guid Id
		{
			get;
			set;
		}
		public String ReportName
		{
			get;
			set;
		}
		public String RequestDatetime
		{
			get;
			set;
		}
		public String SearchCriteria
		{
			get;
			set;
		}
		public Guid? DownloadLinkId
		{
			get;
			set;
		}
		public DateTime LastUpdatedOn
		{
			get;
			set;
		}

		public String Status
		{
			set;
			get;
		} 
	}
}
