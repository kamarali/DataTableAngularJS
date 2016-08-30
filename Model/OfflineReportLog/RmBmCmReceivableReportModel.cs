using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Reports.ReceivablesReport;

namespace Iata.IS.Model.OfflineReportLog
{
	public class RmBmCmReceivableReportModel : ReceivablesReportModel
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

		public string BroweserDateTime
		{
			get;
			set;
		}

		public string FileName
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
