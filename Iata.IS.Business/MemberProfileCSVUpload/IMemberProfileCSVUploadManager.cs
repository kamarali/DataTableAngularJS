using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax.Common;

namespace Iata.IS.Business.MemberProfileCSVUpload
{
    public interface IMemberProfileCSVUploadManager
    {
        bool PerformSanityAndValidationForMemberProfileCSV(Guid fileId);

        bool PerformFirstLevelValidationsForISWEB(string filename);

        /// <summary>
        /// This will add an entry of File upload in File log table
        /// The Guid returned will be used to add a entry into queue
        /// </summary>
        /// <param name="billingPeriod"></param>
        /// <param name="filePath"></param>
        /// <param name="fileFormatType"></param>
        /// <param name="userId"></param>
        ///  <param name="locationId"></param>
        Guid AddIsFileLogEntry(BillingPeriod billingPeriod, string filePath, FileFormatType fileFormatType, int userId,
                               string locationId = null);
    }
}
