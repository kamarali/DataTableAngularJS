using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Reports.PayablesReport;

namespace Iata.IS.Model.OfflineReportLog
{
	public class RmBmCmPayableReportModel : PayablesReportModel
	{
		public String SourceCodes
		{
			get;
			set;
		}

		public String SearchCriteriaParams
		{
			get;
			set;
		}

		public String BrowserDateTime
		{
			get;
			set;
		}

		public String FileName
		{
			get;
			set;
		}

		public Guid OfflineReportLogId
		{
			get;
			set;
		}
	}
}
