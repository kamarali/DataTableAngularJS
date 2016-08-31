using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Business.Reports.OfflineReports
{
	interface IOfflineReportBase
	{
		void SendEmail(int userId,string reportZipFilePath,string downloadUrl,string fileName);

		void SendReportCreationFailureAlert(string fileName);

		void GenerateZipFile<T>(List<T> csvListModel,String searchCriteria,String browserDateTime,String reportFileName);
	}
}
