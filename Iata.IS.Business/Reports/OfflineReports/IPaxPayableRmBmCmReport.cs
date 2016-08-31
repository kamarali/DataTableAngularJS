using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Reports.OfflineReports
{
	public interface IPaxPayableRmBmCmReport
	{
		void GeneratePaxPaybleRmBmCmReport(ReportDownloadRequestMessage message);
	}
}
