using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.AuditTrailPackageDownload
{
    public interface IAuditTrailPackageDownload
    {
        void GenerateAuditTrailPackage(ReportDownloadRequestMessage message);
    }
}
