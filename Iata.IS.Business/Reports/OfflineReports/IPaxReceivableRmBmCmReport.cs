using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.Reports.OfflineReports
{
  /// <summary>
  /// This interface is used for generate receivable rmbmcm report.
  /// </summary>
	public interface IPaxReceivableRmBmCmReport
	{
		void GeneratePaxReceivableRmBmCmReport(ReportDownloadRequestMessage message);
	}
}
