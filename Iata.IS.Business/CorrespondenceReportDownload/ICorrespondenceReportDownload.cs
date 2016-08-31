using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.CorrespondenceReportDownload
{
    public interface ICorrespondenceReportDownload
    {
        void GenerateCorrespondenceTrailReportZip(ReportDownloadRequestMessage requestMessage);
    }
}
