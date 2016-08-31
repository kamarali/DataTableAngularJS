using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Pax;

namespace Iata.IS.Business.Common
{
    public interface IReferenceDataManager
    {
        /// <summary>
        /// This method use to reference data for billed member using location id and member id 
        /// </summary>
        /// <param name="locationId">location code</param>
        /// <param name="billingCategory">PAX/MISC/UATP/CGO</param>
        /// <param name="memberId">member id</param>
        /// <returns></returns>
        MemberReferenceData GetMemberReferenceData(string locationId, BillingCategoryType billingCategory, int memberId);

        /// <summary>
        /// Method use to validate billing and billed combination is valid or not.
        /// Example: if billing has location code then billed must have location code, otherwise error.
        /// </summary>
        /// <param name="memberLocationInformations">invoice member location information</param>
        /// <param name="submissionMethod">ISXML/ISIDEC</param>
        /// <param name="billingLocationId">billing location code</param>
        /// <param name="billedLocationId">billed location code</param>
        /// <param name="billingSource">billing reference data: Supplied/Default/AsperLocation</param>
        /// <param name="billedSource">billed reference data: Supplied/Default/AsperLocation</param>
        /// <returns>valid then true</returns>
        ReferenceDataErrorType IsValidBillingBilledCombination(List<MemberLocationInformation> memberLocationInformations,
                                        SubmissionMethod submissionMethod, string billingLocationId,
                                        string billedLocationId, int billingSource,
                                        int billedSource);

        /// <summary>
        /// Method is use to populate billed member location code. 
        /// This only works when reference data is supplied. 
        /// Due to validation modified for billed member. when billing location null, billed location billing ref data, billed ref data are supplied. 
        /// </summary>
        /// <param name="memberLocationInformations">member location information</param>
        /// <param name="billedMemberId">billed member id</param>
        /// <param name="billedLocationId">billed location code</param>
        /// <param name="billedSource">Supplied</param>
        /// <param name="isValidBilledMemberLocation">true/false</param>
        /// <returns></returns>
        List<MemberLocationInformation> PopulateBilledMemberLocationCode( List<MemberLocationInformation> memberLocationInformations,
                                              int billedMemberId, string billedLocationId, int billedSource,
                                              bool isValidBilledMemberLocation);

        /// <summary>
        /// Method used to validate reference data of billed member and indicate error as per system parameters.
        /// weather they are warning/ error
        /// </summary>
        /// <param name="exceptionDetailsList">exception list to record exception of file</param>
        /// <param name="submissionMethod">ISXML/ISIDEC</param>
        /// <param name="fileName">Name of file</param>
        /// <param name="fileSubmissionDate">Date of file</param>
        /// <param name="paxInvoice">Pax invoice instance</param>
        /// <param name="cgoInvoice">Cargo invoice instance</param>
        /// <param name="miscUatpInvoice">Misc invoice instance</param>
        /// <returns>return expection list</returns>
        IList<IsValidationExceptionDetail> ReferenceDataValidation(IList<IsValidationExceptionDetail> exceptionDetailsList, SubmissionMethod submissionMethod, string fileName, DateTime fileSubmissionDate, PaxInvoice paxInvoice = null, CargoInvoice cgoInvoice = null, MiscUatpInvoice miscUatpInvoice = null);

    }
}
