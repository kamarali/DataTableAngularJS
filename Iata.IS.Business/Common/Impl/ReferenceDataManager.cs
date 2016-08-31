using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Iata.IS.AdminSystem;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Data.Common;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Model.Base;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Pax;

namespace Iata.IS.Business.Common.Impl
{
    public class ReferenceDataManager : IReferenceDataManager
    {

        public IReferenceDataRepository ReferenceDataRep { get; set; }
        private const string UATP = "UATP";
        private const string MAIN = "Main";
        private const string orgNameDelimiter = "!!!";

        /// <summary>
        /// This method use to reference data for billed member using location id and member id 
        /// </summary>
        /// <param name="locationId">location code</param>
        /// <param name="billingCategory">PAX/MISC/UATP/CGO</param>
        /// <param name="memberId">member id</param>
        /// <returns></returns>
        public MemberReferenceData GetMemberReferenceData(string locationId, BillingCategoryType billingCategory, int memberId)
        {
            return ReferenceDataRep.GetMemberReferenceData(locationId, (int) billingCategory, memberId);
        }
       
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
        public ReferenceDataErrorType IsValidBillingBilledCombination(List<MemberLocationInformation> memberLocationInformations, SubmissionMethod submissionMethod, string billingLocationId, string billedLocationId, int billingSource, int billedSource)
        {
            var isBilledReferenceDataAvaliable = false;
            var isBillingReferenceDataAvaliable = false;
            var isBillingLocationAvaliable = false;
            var isBilledLocationAvaliable = false;

            if (memberLocationInformations.Count > 0)
            {
               isBillingReferenceDataAvaliable=  IsReferenceAvaliable(memberLocationInformations.SingleOrDefault(mem => mem.IsBillingMember));
               isBilledReferenceDataAvaliable = IsReferenceAvaliable(memberLocationInformations.SingleOrDefault(mem => !mem.IsBillingMember));
            }

            if (submissionMethod.Equals(SubmissionMethod.IsXml))
            {
                isBilledReferenceDataAvaliable = (billedSource == (int) ReferenceDataSource.Supplied);

                isBillingReferenceDataAvaliable = (billingSource == (int) ReferenceDataSource.Supplied);
            }

            isBillingLocationAvaliable = string.IsNullOrEmpty(billingLocationId)
                                            ? false
                                            : true;

            isBilledLocationAvaliable = string.IsNullOrEmpty(billedLocationId)
                                                        ? false
                                                        : true;


            return MemberLocationMatix(isBillingLocationAvaliable, isBilledLocationAvaliable, isBilledReferenceDataAvaliable, isBillingReferenceDataAvaliable);
        }

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
        public List<MemberLocationInformation> PopulateBilledMemberLocationCode(List<MemberLocationInformation> memberLocationInformations, int billedMemberId, string billedLocationId, int billedSource, bool isValidBilledMemberLocation)
        {
            var locationRepository = Ioc.Resolve<ILocationRepository>(typeof (ILocationRepository));
            var location =
                locationRepository.Single(
                    memberLocation =>
                    memberLocation.MemberId == billedMemberId &&
                    memberLocation.LocationCode.ToUpper() == billedLocationId.ToUpper());
            
            if (location != null)
            {
                foreach (var memberLocationInformation in memberLocationInformations)
                {
                    if (!memberLocationInformation.IsBillingMember && billedSource == (int) ReferenceDataSource.Supplied)
                    {
                        memberLocationInformation.MemberLocationCode = location.LocationCode;
                    }
                }
            }

            return memberLocationInformations;
        }

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
        public IList<IsValidationExceptionDetail> ReferenceDataValidation(IList<IsValidationExceptionDetail> exceptionDetailsList,SubmissionMethod submissionMethod, string fileName, DateTime fileSubmissionDate, PaxInvoice paxInvoice = null, CargoInvoice cgoInvoice = null, MiscUatpInvoice miscUatpInvoice = null)
        {
            MemberLocationInformation billedMemberReferenceData = null;
            MemberReferenceData memberReferenceData = null;
            string locationCode = null;

            if(paxInvoice!=null)
            {
                billedMemberReferenceData = paxInvoice.MemberLocationInformation.SingleOrDefault(memType => memType.IsBillingMember == false);
                memberReferenceData = GetMemberReferenceData(paxInvoice.BilledMemberLocationCode, paxInvoice.BillingCategory, paxInvoice.BilledMemberId);
            }
            else if(cgoInvoice!= null)
            {
                billedMemberReferenceData = cgoInvoice.MemberLocationInformation.SingleOrDefault(memType => memType.IsBillingMember == false);
                memberReferenceData =GetMemberReferenceData(cgoInvoice.BilledMemberLocationCode, cgoInvoice.BillingCategory, cgoInvoice.BilledMemberId);
            }
            else if (miscUatpInvoice != null)
            {
                billedMemberReferenceData = miscUatpInvoice.MemberLocationInformation.SingleOrDefault(memType => memType.IsBillingMember == false);
                memberReferenceData = GetMemberReferenceData(miscUatpInvoice.BilledMemberLocationCode, miscUatpInvoice.BillingCategory, miscUatpInvoice.BilledMemberId);
                //CMP #637: Changes to ICH Settlement
                if (memberReferenceData != null && miscUatpInvoice.BillingCategoryId == (int)BillingCategorys.Uatp)
                {
                  miscUatpInvoice.BilledMemberLocationCode = memberReferenceData.LocationCode;
                }
            }

            // read system parameters validation Params 
            var validationParameters = Iata.IS.AdminSystem.SystemParameters.Instance.ValidationParams;

            //validate Billed member only 
            if (billedMemberReferenceData != null && memberReferenceData!=null)
            {
                locationCode = memberReferenceData.LocationCode;
                 
                  #region Record validation errors for reference data.

                //SCP236631 - Question about sanity-check
                //Bug: When we extract Values from ISXML file, system appends separator(!!!) if organization1 is >50 chars. then combined orgName became org1+separator+org2. 
                //when we compare this legal name, it is not match due to separator. 
                //To fix this issue I have modified code and to remove separators just before comparation. 
                if (!IsRefDataValid(memberReferenceData.MemberLegalName, string.IsNullOrEmpty(billedMemberReferenceData.OrganizationName) ? string.Empty : billedMemberReferenceData.OrganizationName.Replace(orgNameDelimiter,string.Empty)))
                  {
                      var fieldName = submissionMethod.Equals(SubmissionMethod.IsXml) ? "OrganizationName1+OrganizationName2" : "Company Legal Name";

                      if (paxInvoice != null)
                      {
                          var validationExceptionDetail =
                            CreateValidationExceptionDetail(paxInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                            fileSubmissionDate,
                                                            fieldName,
                                                            string.IsNullOrEmpty(billedMemberReferenceData.OrganizationName)
                                                                ? string.Empty
                                                                : billedMemberReferenceData.OrganizationName.Replace(orgNameDelimiter, string.Empty), paxInvoice,
                                                            fileName,
                                                            ErrorLevels.ErrorLevelInvoice,locationCode,
                                                            validationParameters.MemberLegalName ==
                                                            (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                                ? ErrorStatus.X
                                                                : ErrorStatus.W, 0, paxInvoice.BatchSequenceNumber,
                                                            paxInvoice.RecordSequenceWithinBatch);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                      else if(cgoInvoice!=null)
                      {
                          exceptionDetailsList.Add(CreateCgoValidationExceptionDetail(cgoInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                        fileSubmissionDate,
                                                        fieldName,
                                                        string.IsNullOrEmpty(
                                                            billedMemberReferenceData.OrganizationName)
                                                            ? string.Empty
                                                            : billedMemberReferenceData.OrganizationName.Replace(orgNameDelimiter, string.Empty), cgoInvoice,
                                                        fileName,
                                                        ErrorLevels.ErrorLevelInvoice,locationCode,
                                                        validationParameters.MemberLegalName ==
                                                        (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                            ? ErrorStatus.X
                                                            : ErrorStatus.W, 0, cgoInvoice.BatchSequenceNumber,
                                                        cgoInvoice.RecordSequenceWithinBatch));
                      }
                      else if (miscUatpInvoice != null)
                      {
                          var validationExceptionDetail =
                              CreateMiscUatpValidatikonExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                       exceptionDetailsList.Count() + 1,
                                                                       fileSubmissionDate, miscUatpInvoice,
                                                                       fieldName,
                                                                       string.IsNullOrEmpty(
                                                                           billedMemberReferenceData.OrganizationName)
                                                                           ? string.Empty
                                                                           : billedMemberReferenceData.OrganizationName.Replace(orgNameDelimiter, string.Empty),
                                                                       fileName, ErrorLevels.ErrorLevelInvoice,locationCode,
                                                                       validationParameters.MemberLegalName ==
                                                                       (int) Iata.IS.Model.Enums.ValidationParams.Error
                                                                           ? ErrorStatus.X
                                                                           : ErrorStatus.W, 0, 0, true);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                  }

                  if (!IsRefDataValid(memberReferenceData.TaxVatRegNo,billedMemberReferenceData.TaxRegistrationId))
                  {
                      var fieldName = submissionMethod.Equals(SubmissionMethod.IsXml) ? "TaxRegistrationID" : "Tax/VAT Registration ID";

                      if (paxInvoice != null)
                      {
                          var validationExceptionDetail =
                            CreateValidationExceptionDetail(paxInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                            fileSubmissionDate,
                                                            fieldName,
                                                            string.IsNullOrEmpty(
                                                                billedMemberReferenceData.TaxRegistrationId)
                                                                ? string.Empty
                                                                : billedMemberReferenceData.TaxRegistrationId, paxInvoice,
                                                            fileName,
                                                            ErrorLevels.ErrorLevelInvoice,locationCode,
                                                            validationParameters.TaxVATRegistration ==
                                                            (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                                ? ErrorStatus.X
                                                                : ErrorStatus.W, 0, paxInvoice.BatchSequenceNumber,
                                                            paxInvoice.RecordSequenceWithinBatch);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                      else if (cgoInvoice != null)
                      {
                          exceptionDetailsList.Add(CreateCgoValidationExceptionDetail(cgoInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                        fileSubmissionDate,
                                                        fieldName,
                                                        string.IsNullOrEmpty(
                                                            billedMemberReferenceData.TaxRegistrationId)
                                                            ? string.Empty
                                                            : billedMemberReferenceData.TaxRegistrationId, cgoInvoice,
                                                        fileName,
                                                        ErrorLevels.ErrorLevelInvoice,locationCode,
                                                        validationParameters.TaxVATRegistration ==
                                                        (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                            ? ErrorStatus.X
                                                            : ErrorStatus.W, 0, cgoInvoice.BatchSequenceNumber,
                                                        cgoInvoice.RecordSequenceWithinBatch));
                      }
                      else if (miscUatpInvoice != null)
                      {
                          var validationExceptionDetail =
                              CreateMiscUatpValidatikonExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                       exceptionDetailsList.Count() + 1,
                                                                       fileSubmissionDate, miscUatpInvoice,
                                                                       fieldName,
                                                                       string.IsNullOrEmpty(
                                                                           billedMemberReferenceData.TaxRegistrationId)
                                                                           ? string.Empty
                                                                           : billedMemberReferenceData.TaxRegistrationId,
                                                                       fileName, ErrorLevels.ErrorLevelInvoice,locationCode,
                                                                       validationParameters.TaxVATRegistration ==
                                                                       (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                                           ? ErrorStatus.X
                                                                           : ErrorStatus.W, 0, 0, true);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                  }

                  if (!IsRefDataValid(memberReferenceData.AddTaxVatRegNo,billedMemberReferenceData.AdditionalTaxVatRegistrationNumber))
                  {
                      var fieldName = submissionMethod.Equals(SubmissionMethod.IsXml) ? "AdditionalTaxRegistrationID" : "Additional Tax/VAT Registration ID";
                      if (paxInvoice != null)
                      {
                          var validationExceptionDetail =
                           CreateValidationExceptionDetail(paxInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                           fileSubmissionDate,
                                                           fieldName,
                                                           string.IsNullOrEmpty(
                                                               billedMemberReferenceData.
                                                                   AdditionalTaxVatRegistrationNumber)
                                                               ? string.Empty
                                                               : billedMemberReferenceData.
                                                                     AdditionalTaxVatRegistrationNumber, paxInvoice,
                                                           fileName,
                                                           ErrorLevels.ErrorLevelInvoice,locationCode,
                                                           validationParameters.AddTaxVATRegistration ==
                                                           (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                               ? ErrorStatus.X
                                                               : ErrorStatus.W, 0, paxInvoice.BatchSequenceNumber,
                                                           paxInvoice.RecordSequenceWithinBatch);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                      else if (cgoInvoice != null)
                      {
                          exceptionDetailsList.Add(CreateCgoValidationExceptionDetail(cgoInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                        fileSubmissionDate,
                                                        fieldName,
                                                        string.IsNullOrEmpty(
                                                            billedMemberReferenceData.AdditionalTaxVatRegistrationNumber)
                                                            ? string.Empty
                                                            : billedMemberReferenceData.AdditionalTaxVatRegistrationNumber, cgoInvoice,
                                                        fileName,
                                                        ErrorLevels.ErrorLevelInvoice,locationCode,
                                                        validationParameters.AddTaxVATRegistration ==
                                                        (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                            ? ErrorStatus.X
                                                            : ErrorStatus.W, 0, cgoInvoice.BatchSequenceNumber,
                                                        cgoInvoice.RecordSequenceWithinBatch));
                      }
                      else if (miscUatpInvoice != null)
                      {
                          var validationExceptionDetail =
                              CreateMiscUatpValidatikonExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                       exceptionDetailsList.Count() + 1,
                                                                       fileSubmissionDate, miscUatpInvoice,
                                                                       fieldName,
                                                                       string.IsNullOrEmpty(
                                                                           billedMemberReferenceData.AdditionalTaxVatRegistrationNumber)
                                                                           ? string.Empty
                                                                           : billedMemberReferenceData.AdditionalTaxVatRegistrationNumber,
                                                                       fileName, ErrorLevels.ErrorLevelInvoice,locationCode,
                                                                       validationParameters.AddTaxVATRegistration ==
                                                                       (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                                           ? ErrorStatus.X
                                                                           : ErrorStatus.W, 0, 0, true);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                  }

                  if (!IsRefDataValid(memberReferenceData.CompanyRegId,billedMemberReferenceData.CompanyRegistrationId))
                  {
                      if (paxInvoice != null)
                      {
                          var validationExceptionDetail =
                           CreateValidationExceptionDetail(paxInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                           fileSubmissionDate,
                                                           "Company Registration ID",
                                                           string.IsNullOrEmpty(
                                                               billedMemberReferenceData.CompanyRegistrationId)
                                                               ? string.Empty
                                                               : billedMemberReferenceData.CompanyRegistrationId,
                                                           paxInvoice,
                                                           fileName,
                                                           ErrorLevels.ErrorLevelInvoice,locationCode,
                                                           validationParameters.CompanyRegistrationID ==
                                                           (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                               ? ErrorStatus.X
                                                               : ErrorStatus.W, 0, paxInvoice.BatchSequenceNumber,
                                                           paxInvoice.RecordSequenceWithinBatch);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                      else if (cgoInvoice != null)
                      {
                          exceptionDetailsList.Add(CreateCgoValidationExceptionDetail(cgoInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                        fileSubmissionDate,
                                                        "Company Registration ID",
                                                        string.IsNullOrEmpty(
                                                            billedMemberReferenceData.CompanyRegistrationId)
                                                            ? string.Empty
                                                            : billedMemberReferenceData.CompanyRegistrationId, cgoInvoice,
                                                        fileName,
                                                        ErrorLevels.ErrorLevelInvoice,locationCode,
                                                        validationParameters.CompanyRegistrationID ==
                                                        (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                            ? ErrorStatus.X
                                                            : ErrorStatus.W, 0, cgoInvoice.BatchSequenceNumber,
                                                        cgoInvoice.RecordSequenceWithinBatch));
                      }
                      else if (miscUatpInvoice != null)
                      {
                          var validationExceptionDetail =
                            CreateMiscUatpValidatikonExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                     exceptionDetailsList.Count() + 1,
                                                                     fileSubmissionDate, miscUatpInvoice,
                                                                     "Company Registration ID",
                                                                     string.IsNullOrEmpty(
                                                                         billedMemberReferenceData.CompanyRegistrationId)
                                                                         ? string.Empty
                                                                         : billedMemberReferenceData.CompanyRegistrationId,
                                                                     fileName, ErrorLevels.ErrorLevelInvoice,locationCode,
                                                                     validationParameters.CompanyRegistrationID ==
                                                                     (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                                         ? ErrorStatus.X
                                                                         : ErrorStatus.W, 0, 0, true);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                  }

                  if (!IsRefDataValid(memberReferenceData.AddressLine1,billedMemberReferenceData.AddressLine1))
                  {
                      if (paxInvoice != null)
                      {
                          var validationExceptionDetail =
                            CreateValidationExceptionDetail(paxInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                            fileSubmissionDate,
                                                            "Address Line 1",
                                                            string.IsNullOrEmpty(billedMemberReferenceData.AddressLine1)
                                                                ? string.Empty
                                                                : billedMemberReferenceData.AddressLine1, paxInvoice,
                                                            fileName,
                                                            ErrorLevels.ErrorLevelInvoice,locationCode,
                                                            validationParameters.AddressLine1 ==
                                                            (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                                ? ErrorStatus.X
                                                                : ErrorStatus.W, 0, paxInvoice.BatchSequenceNumber,
                                                            paxInvoice.RecordSequenceWithinBatch);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                      else if (cgoInvoice != null)
                      {
                          exceptionDetailsList.Add(CreateCgoValidationExceptionDetail(cgoInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                        fileSubmissionDate,
                                                        "Address Line 1",
                                                        string.IsNullOrEmpty(
                                                            billedMemberReferenceData.AddressLine1)
                                                            ? string.Empty
                                                            : billedMemberReferenceData.AddressLine1, cgoInvoice,
                                                        fileName,
                                                        ErrorLevels.ErrorLevelInvoice,locationCode,
                                                        validationParameters.AddressLine1 ==
                                                        (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                            ? ErrorStatus.X
                                                            : ErrorStatus.W, 0, cgoInvoice.BatchSequenceNumber,
                                                        cgoInvoice.RecordSequenceWithinBatch));
                      }
                      else if (miscUatpInvoice != null)
                      {
                          var validationExceptionDetail =
                            CreateMiscUatpValidatikonExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                     exceptionDetailsList.Count() + 1,
                                                                     fileSubmissionDate, miscUatpInvoice,
                                                                     "Address Line 1",
                                                                     string.IsNullOrEmpty(
                                                                         billedMemberReferenceData.AddressLine1)
                                                                         ? string.Empty
                                                                         : billedMemberReferenceData.AddressLine1,
                                                                     fileName, ErrorLevels.ErrorLevelInvoice,locationCode,
                                                                     validationParameters.AddressLine1 ==
                                                                     (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                                         ? ErrorStatus.X
                                                                         : ErrorStatus.W, 0, 0, true);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                  }

                  if (!IsRefDataValid(memberReferenceData.AddressLine2,billedMemberReferenceData.AddressLine2))
                  {
                      if (paxInvoice != null)
                      {
                          var validationExceptionDetail =
                           CreateValidationExceptionDetail(paxInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                           fileSubmissionDate,
                                                           "Address Line 2",
                                                           string.IsNullOrEmpty(billedMemberReferenceData.AddressLine2)
                                                               ? string.Empty
                                                               : billedMemberReferenceData.AddressLine2, paxInvoice,
                                                           fileName,
                                                           ErrorLevels.ErrorLevelInvoice,locationCode,
                                                           validationParameters.AddressLine2 ==
                                                           (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                               ? ErrorStatus.X
                                                               : ErrorStatus.W, 0, paxInvoice.BatchSequenceNumber,
                                                           paxInvoice.RecordSequenceWithinBatch);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                      else if (cgoInvoice != null)
                      {
                          exceptionDetailsList.Add(CreateCgoValidationExceptionDetail(cgoInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                        fileSubmissionDate,
                                                        "Address Line 2",
                                                        string.IsNullOrEmpty(
                                                            billedMemberReferenceData.AddressLine2)
                                                            ? string.Empty
                                                            : billedMemberReferenceData.AddressLine2, cgoInvoice,
                                                        fileName,
                                                        ErrorLevels.ErrorLevelInvoice,locationCode,
                                                        validationParameters.AddressLine2 ==
                                                        (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                            ? ErrorStatus.X
                                                            : ErrorStatus.W, 0, cgoInvoice.BatchSequenceNumber,
                                                        cgoInvoice.RecordSequenceWithinBatch));
                      }
                      else if (miscUatpInvoice != null)
                      {
                          var validationExceptionDetail =
                            CreateMiscUatpValidatikonExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                     exceptionDetailsList.Count() + 1,
                                                                     fileSubmissionDate, miscUatpInvoice,
                                                                     "Address Line 2",
                                                                     string.IsNullOrEmpty(
                                                                         billedMemberReferenceData.AddressLine2)
                                                                         ? string.Empty
                                                                         : billedMemberReferenceData.AddressLine2,
                                                                     fileName, ErrorLevels.ErrorLevelInvoice,locationCode,
                                                                     validationParameters.AddressLine2 ==
                                                                     (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                                         ? ErrorStatus.X
                                                                         : ErrorStatus.W, 0, 0, true);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                  }

                  if (!IsRefDataValid(memberReferenceData.AddressLine3,billedMemberReferenceData.AddressLine3))
                  {
                      if (paxInvoice != null)
                      {
                          var validationExceptionDetail =
                            CreateValidationExceptionDetail(paxInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                            fileSubmissionDate,
                                                            "Address Line 3",
                                                            string.IsNullOrEmpty(billedMemberReferenceData.AddressLine3)
                                                                ? string.Empty
                                                                : billedMemberReferenceData.AddressLine3, paxInvoice,
                                                            fileName,
                                                            ErrorLevels.ErrorLevelInvoice,locationCode,
                                                            validationParameters.AddressLine3 ==
                                                            (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                                ? ErrorStatus.X
                                                                : ErrorStatus.W, 0, paxInvoice.BatchSequenceNumber,
                                                            paxInvoice.RecordSequenceWithinBatch);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                      else if (cgoInvoice != null)
                      {
                          exceptionDetailsList.Add(CreateCgoValidationExceptionDetail(cgoInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                        fileSubmissionDate,
                                                        "Address Line 3",
                                                        string.IsNullOrEmpty(
                                                            billedMemberReferenceData.AddressLine3)
                                                            ? string.Empty
                                                            : billedMemberReferenceData.AddressLine3, cgoInvoice,
                                                        fileName,
                                                        ErrorLevels.ErrorLevelInvoice,locationCode,
                                                        validationParameters.AddressLine3 ==
                                                        (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                            ? ErrorStatus.X
                                                            : ErrorStatus.W, 0, cgoInvoice.BatchSequenceNumber,
                                                        cgoInvoice.RecordSequenceWithinBatch));
                      }
                      else if (miscUatpInvoice != null)
                      {
                          var validationExceptionDetail =
                            CreateMiscUatpValidatikonExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                     exceptionDetailsList.Count() + 1,
                                                                     fileSubmissionDate, miscUatpInvoice,
                                                                     "Address Line 3",
                                                                     string.IsNullOrEmpty(
                                                                         billedMemberReferenceData.AddressLine3)
                                                                         ? string.Empty
                                                                         : billedMemberReferenceData.AddressLine3,
                                                                     fileName, ErrorLevels.ErrorLevelInvoice,locationCode,
                                                                     validationParameters.AddressLine3 ==
                                                                     (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                                         ? ErrorStatus.X
                                                                         : ErrorStatus.W, 0, 0, true);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                  }


                  if (!IsRefDataValid(memberReferenceData.CityName,billedMemberReferenceData.CityName))
                  {
                      if (paxInvoice != null)
                      {
                          var validationExceptionDetail =
                           CreateValidationExceptionDetail(paxInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                           fileSubmissionDate,
                                                           "City Name",
                                                           string.IsNullOrEmpty(billedMemberReferenceData.CityName)
                                                               ? string.Empty
                                                               : billedMemberReferenceData.CityName, paxInvoice,
                                                           fileName,
                                                           ErrorLevels.ErrorLevelInvoice,locationCode,
                                                           validationParameters.CityName ==
                                                           (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                               ? ErrorStatus.X
                                                               : ErrorStatus.W, 0, paxInvoice.BatchSequenceNumber,
                                                           paxInvoice.RecordSequenceWithinBatch);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                      else if (cgoInvoice != null)
                      {
                          exceptionDetailsList.Add(CreateCgoValidationExceptionDetail(cgoInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                        fileSubmissionDate,
                                                        "City Name",
                                                        string.IsNullOrEmpty(
                                                            billedMemberReferenceData.CityName)
                                                            ? string.Empty
                                                            : billedMemberReferenceData.CityName, cgoInvoice,
                                                        fileName,
                                                        ErrorLevels.ErrorLevelInvoice,locationCode,
                                                        validationParameters.CityName ==
                                                        (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                            ? ErrorStatus.X
                                                            : ErrorStatus.W, 0, cgoInvoice.BatchSequenceNumber,
                                                        cgoInvoice.RecordSequenceWithinBatch));
                      }
                      else if (miscUatpInvoice != null)
                      {
                          var validationExceptionDetail =
                           CreateMiscUatpValidatikonExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                    exceptionDetailsList.Count() + 1,
                                                                    fileSubmissionDate, miscUatpInvoice,
                                                                    "City Name",
                                                                    string.IsNullOrEmpty(
                                                                        billedMemberReferenceData.CityName)
                                                                        ? string.Empty
                                                                        : billedMemberReferenceData.CityName,
                                                                    fileName, ErrorLevels.ErrorLevelInvoice,locationCode,
                                                                    validationParameters.CityName ==
                                                                    (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                                        ? ErrorStatus.X
                                                                        : ErrorStatus.W, 0, 0, true);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                  }

                  if (!IsRefDataValid(memberReferenceData.CountryCode,billedMemberReferenceData.CountryCode))
                  {
                      if (paxInvoice != null)
                      {
                          var validationExceptionDetail =
                           CreateValidationExceptionDetail(paxInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                           fileSubmissionDate,
                                                           "Country Code",
                                                           string.IsNullOrEmpty(billedMemberReferenceData.CountryCode)
                                                               ? string.Empty
                                                               : billedMemberReferenceData.CountryCode, paxInvoice,
                                                           fileName,
                                                           ErrorLevels.ErrorLevelInvoice,locationCode,
                                                           validationParameters.CountryCode ==
                                                           (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                               ? ErrorStatus.X
                                                               : ErrorStatus.W, 0, paxInvoice.BatchSequenceNumber,
                                                           paxInvoice.RecordSequenceWithinBatch);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                      else if (cgoInvoice != null)
                      {
                          exceptionDetailsList.Add(CreateCgoValidationExceptionDetail(cgoInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                        fileSubmissionDate,
                                                        "Country Code",
                                                        string.IsNullOrEmpty(
                                                            billedMemberReferenceData.CountryCode)
                                                            ? string.Empty
                                                            : billedMemberReferenceData.CountryCode, cgoInvoice,
                                                        fileName,
                                                        ErrorLevels.ErrorLevelInvoice,locationCode,
                                                        validationParameters.CountryCode ==
                                                        (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                            ? ErrorStatus.X
                                                            : ErrorStatus.W, 0, cgoInvoice.BatchSequenceNumber,
                                                        cgoInvoice.RecordSequenceWithinBatch));
                      }
                      else if (miscUatpInvoice != null)
                      {
                          var validationExceptionDetail =
                           CreateMiscUatpValidatikonExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                    exceptionDetailsList.Count() + 1,
                                                                    fileSubmissionDate, miscUatpInvoice,
                                                                    "Country Code",
                                                                    string.IsNullOrEmpty(
                                                                        billedMemberReferenceData.CountryCode)
                                                                        ? string.Empty
                                                                        : billedMemberReferenceData.CountryCode,
                                                                    fileName, ErrorLevels.ErrorLevelInvoice,
                                                                    locationCode,
                                                                    validationParameters.CountryCode ==
                                                                    (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                                        ? ErrorStatus.X
                                                                        : ErrorStatus.W, 0, 0, true);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                  }

                  if (!IsRefDataValid(memberReferenceData.PostalCode,billedMemberReferenceData.PostalCode))
                  {
                      if (paxInvoice != null)
                      {
                          var validationExceptionDetail =
                            CreateValidationExceptionDetail(paxInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                            fileSubmissionDate,
                                                            "Postal Code",
                                                            string.IsNullOrEmpty(billedMemberReferenceData.PostalCode)
                                                                ? string.Empty
                                                                : billedMemberReferenceData.PostalCode, paxInvoice,
                                                            fileName,
                                                            ErrorLevels.ErrorLevelInvoice, locationCode,
                                                            validationParameters.PostalCode ==
                                                            (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                                ? ErrorStatus.X
                                                                : ErrorStatus.W, 0, paxInvoice.BatchSequenceNumber,
                                                            paxInvoice.RecordSequenceWithinBatch);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                      else if (cgoInvoice != null)
                      {
                          exceptionDetailsList.Add(CreateCgoValidationExceptionDetail(cgoInvoice.Id.Value(), exceptionDetailsList.Count + 1,
                                                        fileSubmissionDate,
                                                        "Postal Code",
                                                        string.IsNullOrEmpty(
                                                            billedMemberReferenceData.PostalCode)
                                                            ? string.Empty
                                                            : billedMemberReferenceData.PostalCode, cgoInvoice,
                                                        fileName,
                                                        ErrorLevels.ErrorLevelInvoice,locationCode,
                                                        validationParameters.PostalCode ==
                                                        (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                            ? ErrorStatus.X
                                                            : ErrorStatus.W, 0, cgoInvoice.BatchSequenceNumber,
                                                        cgoInvoice.RecordSequenceWithinBatch));
                      }
                      else if (miscUatpInvoice != null)
                      {
                          var validationExceptionDetail =
                           CreateMiscUatpValidatikonExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                    exceptionDetailsList.Count() + 1,
                                                                    fileSubmissionDate, miscUatpInvoice,
                                                                    "Postal Code",
                                                                    string.IsNullOrEmpty(
                                                                        billedMemberReferenceData.PostalCode)
                                                                        ? string.Empty
                                                                        : billedMemberReferenceData.PostalCode,
                                                                    fileName, ErrorLevels.ErrorLevelInvoice,locationCode,
                                                                    validationParameters.PostalCode ==
                                                                    (int)Iata.IS.Model.Enums.ValidationParams.Error
                                                                        ? ErrorStatus.X
                                                                        : ErrorStatus.W, 0, 0, true);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                  }
                    

                  #endregion
              }
            return exceptionDetailsList;
        }
        
        /// <summary>
        /// method to check profile data match with member data
        /// </summary>
        /// <param name="profileData"></param>
        /// <param name="memberData"></param>
        /// <returns></returns>
        private bool IsRefDataValid(string profileData, string memberData)
        {
            var result = true;

            if (string.IsNullOrWhiteSpace(profileData) && !string.IsNullOrWhiteSpace(memberData))
            {
                result = false;
            }

            if (!string.IsNullOrWhiteSpace(profileData) && string.IsNullOrWhiteSpace(memberData))
            {
                result = false;
            }

            if (!string.IsNullOrWhiteSpace(profileData) && !string.IsNullOrWhiteSpace(memberData))
            {
                result = profileData.ToLower().Trim() == memberData.ToLower().Trim();
            }

            return result;
        }

        /// <summary>
        /// check weather reference data is provided or not
        /// </summary>
        /// <param name="memberReferenceData">reference data instance</param>
        /// <returns>true/false</returns>
        private bool IsReferenceAvaliable(MemberLocationInformation memberReferenceData)
        {
            if(memberReferenceData!= null)
            return !string.IsNullOrEmpty(memberReferenceData.OrganizationName) ||
                   !string.IsNullOrEmpty(memberReferenceData.TaxRegistrationId) ||
                   !string.IsNullOrEmpty(memberReferenceData.AdditionalTaxVatRegistrationNumber) ||
                   !string.IsNullOrEmpty(memberReferenceData.CompanyRegistrationId) ||
                   !string.IsNullOrEmpty(memberReferenceData.AddressLine1) ||
                   !string.IsNullOrEmpty(memberReferenceData.AddressLine2) ||
                   !string.IsNullOrEmpty(memberReferenceData.AddressLine3) ||
                   !string.IsNullOrEmpty(memberReferenceData.CityName) ||
                   !string.IsNullOrEmpty(memberReferenceData.CountryName) ||
                   !string.IsNullOrEmpty(memberReferenceData.PostalCode);
            else
                return false;
        }

        /// <summary>
        /// Matrix with call possible cases. 
        /// </summary>
        /// <param name="billingLoc">billing location code</param>
        /// <param name="billedLoc">billed location code</param>
        /// <param name="billedRefData">billed ref data</param>
        /// <param name="billingRefData">billing ref data</param>
        /// <returns>true/false</returns>
        private ReferenceDataErrorType MemberLocationMatix(bool billingLoc, bool billedLoc, bool billedRefData, bool billingRefData)
        {
            //rule = billingLoc+billingRefData+billedLoc+billedRefData;
            //P = Pass, F = Fail
            var rules = new Dictionary<string, string>
                            {
                                {"0000", "P"},
                                {"1000", "P"},
                                {"0010", "P"},
                                {"1010", "P"},
                                {"1111", "FS"},
                                {"1101", "FS"},
                                {"1110", "FSG"},
                                {"0011", "FG"},
                                {"1011", "FG"},
                                {"0100", "FG"},
                                {"0111", "P"},
                                {"0101", "P"}
                            };

            var find = string.Format("{0}{1}{2}{3}", billingLoc ? "1" : "0", billingRefData ? "1" : "0", billedLoc ? "1" : "0", billedRefData ? "1" : "0");
            if (rules.ContainsKey(find))
            {
                switch (rules[find])
                {
                    case "P":
                        return ReferenceDataErrorType.NoError;
                    case "FS":
                        return ReferenceDataErrorType.Specific;
                    case "FSG":
                        return ReferenceDataErrorType.Both;
                    default:
                        return ReferenceDataErrorType.General;
                }
            }
            else
            {
                return ReferenceDataErrorType.General;
            }
        }


        /// <summary>
        /// validation expection detail for cargo file
        /// </summary>
        /// <param name="id"></param>
        /// <param name="serialNumber"></param>
        /// <param name="fileSubmissionDate"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <param name="invoice"></param>
        /// <param name="fileName"></param>
        /// <param name="errorLevel"></param>
        /// <param name="locationCode"></param>
        /// <param name="errorStatus"></param>
        /// <param name="billingCode"></param>
        /// <param name="sourceCode"></param>
        /// <param name="batchNo"></param>
        /// <param name="sequenceNo"></param>
        /// <param name="documentNumber"></param>
        /// <param name="isBatchUpdateAllowed"></param>
        /// <param name="linkedDocumentNumber"></param>
        /// <param name="calculatedValue"></param>
        /// <returns></returns>
        private IsValidationExceptionDetail CreateCgoValidationExceptionDetail(string id, int serialNumber, DateTime fileSubmissionDate, string fieldName, string fieldValue, InvoiceBase invoice, string fileName, string errorLevel, string locationCode, ErrorStatus errorStatus, int billingCode, int sourceCode = 0, int batchNo = 0, int sequenceNo = 0, string documentNumber = null, bool isBatchUpdateAllowed = false, string linkedDocumentNumber = null, string calculatedValue = null)
        {

            string submissionFormat;
            if (Path.GetExtension(fileName).ToUpper().Equals(".XML"))
            {
                submissionFormat = ((int)SubmissionMethod.IsXml).ToString(); //"IS-XML";// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsXml).ToUpper();
            }
            else
            {
                submissionFormat = ((int)SubmissionMethod.IsIdec).ToString(); // Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsIdec).ToUpper();
            }

            var errorDescription = string.Empty;
             if (!string.IsNullOrEmpty( ErrorCodes.InvalidMemberReferenceDataInformation))
                errorDescription = string.Format(Messages.ResourceManager.GetString(ErrorCodes.InvalidMemberReferenceDataInformation), fieldName, locationCode ?? "Main");

            if (!string.IsNullOrWhiteSpace(calculatedValue))
            {
                errorDescription = string.Format("{0} The expected value is {1}", errorDescription, calculatedValue);
            }

            var validationExceptionDetail = new IsValidationExceptionDetail
            {
                SerialNo = serialNumber,
                BillingEntityCode = invoice.BillingMember != null ? invoice.BillingMember.MemberCodeNumeric : string.Empty,
                BilledEntityCode = invoice.BilledMember != null ? invoice.BilledMember.MemberCodeNumeric : string.Empty,
                ChargeCategoryOrBillingCode = GetBillingCode(billingCode),
                CategoryOfBilling = invoice.BillingCategoryId.ToString(),
                SubmissionFormat = submissionFormat,
                ErrorStatus = ((int)errorStatus).ToString(),
                ClearanceMonth = invoice.BillingYear + invoice.BillingMonth.ToString().PadLeft(2, '0'),
                PeriodNumber = invoice.BillingPeriod,
                BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd"),
                LinkedDocNo = linkedDocumentNumber,

                ErrorDescription = errorDescription,
                FieldName = fieldName,
                FieldValue = fieldValue,
                BatchUpdateAllowed = isBatchUpdateAllowed,

                InvoiceNumber = invoice.InvoiceNumber,
                DocumentNo = documentNumber,
                SourceCodeId = sourceCode == 0 ? string.Empty : sourceCode.ToString(),
                ErrorLevel = errorLevel,
                ExceptionCode =  ErrorCodes.InvalidMemberReferenceDataInformation,
                FileName = Path.GetFileName(fileName),
                LineItemOrBatchNo = batchNo,
                LineItemDetailOrSequenceNo = sequenceNo,
                Id = Guid.NewGuid(),
                PkReferenceId = id
            };

            return validationExceptionDetail;
        }

        /// <summary>
        /// Dictionary for Cargo BillingCode.
        /// </summary>
        private static readonly Dictionary<int, string> CargoBillingCodeToEnumMap = new Dictionary<int, string> { { 1, "P" }, { 2, "C" }, { 3, "B" }, { 4, "T" }, { 5, "R" } };


        /// <summary>
        /// To return billingCode for billingCodeId.
        /// </summary>
        /// <param name="billingCode"></param>
        /// <returns></returns>
        private static string GetBillingCode(int billingCode)
        {
            var cgoBillingCode = string.Empty;
            if (CargoBillingCodeToEnumMap.ContainsKey(billingCode))
            {
                cgoBillingCode = CargoBillingCodeToEnumMap[billingCode];
            }
            return cgoBillingCode;
        }
        
       /// <summary>
       /// validation expection detail for pax file
       /// </summary>
       /// <param name="pkId"></param>
       /// <param name="serialNumber"></param>
       /// <param name="fileSubmissionDate"></param>
       /// <param name="fieldName"></param>
       /// <param name="fieldValue"></param>
       /// <param name="invoice"></param>
       /// <param name="fileName"></param>
       /// <param name="errorLevel"></param>
       /// <param name="locationCode"></param>
       /// <param name="errorStatus"></param>
       /// <param name="sourceCode"></param>
       /// <param name="batchNo"></param>
       /// <param name="sequenceNo"></param>
       /// <param name="documentNumber"></param>
       /// <param name="isBatchUpdateAllowed"></param>
       /// <param name="linkedDocumentNumber"></param>
       /// <param name="calculatedValue"></param>
       /// <param name="islinkingError"></param>
       /// <returns></returns>
        private IsValidationExceptionDetail CreateValidationExceptionDetail(string pkId, int serialNumber, DateTime fileSubmissionDate, string fieldName, string fieldValue, InvoiceBase invoice, string fileName, string errorLevel, string locationCode, ErrorStatus errorStatus, int sourceCode = 0, int batchNo = 0, int sequenceNo = 0, string documentNumber = null, bool isBatchUpdateAllowed = false, string linkedDocumentNumber = null, string calculatedValue = null, bool islinkingError = false)
        {
            string submissionFormat;
            if (Path.GetExtension(fileName).ToUpper().Equals(".XML"))
            {
                submissionFormat = ((int)SubmissionMethod.IsXml).ToString(); //"IS-XML";// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsXml).ToUpper();
            }
            else
            {
                submissionFormat = ((int)SubmissionMethod.IsIdec).ToString(); // Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsIdec).ToUpper();
            }
            var errorDescription = string.Empty;
            if (!string.IsNullOrEmpty( ErrorCodes.InvalidMemberReferenceDataInformation))
              errorDescription = string.Format(Messages.ResourceManager.GetString(ErrorCodes.InvalidMemberReferenceDataInformation), fieldName, locationCode ?? "Main");

            if (!string.IsNullOrWhiteSpace(calculatedValue))
            {
                errorDescription = string.Format("{0} The expected value is {1}", errorDescription, calculatedValue);
            }

            var validationExceptionDetail = new IsValidationExceptionDetail
            {
                SerialNo = serialNumber,
                BillingEntityCode = invoice.BillingMember != null ? invoice.BillingMember.MemberCodeNumeric : string.Empty,
                BilledEntityCode = invoice.BilledMember != null ? invoice.BilledMember.MemberCodeNumeric : string.Empty,
                ChargeCategoryOrBillingCode = invoice.BillingCode.ToString(),
                CategoryOfBilling = invoice.BillingCategoryId.ToString(),
                SubmissionFormat = submissionFormat,
                ErrorStatus = ((int)errorStatus).ToString(),
                ClearanceMonth = invoice.BillingYear + invoice.BillingMonth.ToString().PadLeft(2, '0'),
                PeriodNumber = invoice.BillingPeriod,
                BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd"),
                LinkedDocNo = linkedDocumentNumber,

                ErrorDescription = errorDescription,
                FieldName = fieldName,
                FieldValue = fieldValue,
                BatchUpdateAllowed = isBatchUpdateAllowed,
                //CouponNo = 
                InvoiceNumber = invoice.InvoiceNumber,
                DocumentNo = documentNumber,
                SourceCodeId = sourceCode.ToString(),
                ErrorLevel = errorLevel,
                ExceptionCode =  ErrorCodes.InvalidMemberReferenceDataInformation,
                FileName = Path.GetFileName(fileName),
                LineItemOrBatchNo = batchNo,
                LineItemDetailOrSequenceNo = sequenceNo,
                Id = Guid.NewGuid(),
                PkReferenceId = pkId
            };

            if (islinkingError)
            {
                validationExceptionDetail.TransactionId = 9; //TODO : Add code for error correction : Form D

            }

            if (documentNumber != null)
            {
                string[] documentNumbers = documentNumber.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                if (documentNumbers.Count() == 3)
                {
                    validationExceptionDetail.CouponNo = Convert.ToInt32(documentNumbers[2]);
                    validationExceptionDetail.IssuingAirline = documentNumbers[0];
                }
            }

            return validationExceptionDetail;
        }

        /// <summary>
        /// validation expection detail for misc/uatp files
        /// </summary>
        /// <param name="pkId"></param>
        /// <param name="serialNumber"></param>
        /// <param name="fileSubmissionDate"></param>
        /// <param name="miscUatpInvoice"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValue"></param>
        /// <param name="fileName"></param>
        /// <param name="errorLevel"></param>
        /// <param name="locationCode"></param>
        /// <param name="errorStatus"></param>
        /// <param name="lineItemNumber"></param>
        /// <param name="lineItemDetailNumber"></param>
        /// <param name="isBatchUpdateAllowed"></param>
        /// <param name="calculatedAmount"></param>
        /// <param name="islinkingError"></param>
        /// <returns></returns>
        private IsValidationExceptionDetail CreateMiscUatpValidatikonExceptionDetail(string pkId, int serialNumber, DateTime fileSubmissionDate, MiscUatpInvoice miscUatpInvoice, string fieldName, string fieldValue, string fileName, string errorLevel, string locationCode, ErrorStatus errorStatus, int lineItemNumber = 0, int lineItemDetailNumber = 0, bool isBatchUpdateAllowed = false, string calculatedAmount = null, bool islinkingError = false)
        {
            string submissionFormat;
            if (Path.GetExtension(fileName).ToUpper().Equals(".XML"))
            {
                submissionFormat = ((int)SubmissionMethod.IsXml).ToString(); // Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsXml).ToUpper();
            }
            else
            {
                submissionFormat = ((int)SubmissionMethod.IsIdec).ToString();// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsIdec).ToUpper();
            }
            var errorDescription = string.Empty;
            if (!string.IsNullOrEmpty( ErrorCodes.InvalidMemberReferenceDataInformation))
              errorDescription = string.Format(Messages.ResourceManager.GetString(ErrorCodes.InvalidMemberReferenceDataInformation), fieldName, locationCode ?? "Main");
            fileName = Path.GetFileName(fileName);

            var chargecodeName = string.Empty;
            if (!lineItemNumber.Equals(0))
            {
                if (miscUatpInvoice.LineItems.Count > lineItemNumber - 1)
                    chargecodeName = miscUatpInvoice.LineItems[lineItemNumber - 1].ChargeCode != null ? miscUatpInvoice.LineItems[lineItemNumber - 1].ChargeCode.Name : string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(calculatedAmount))
            {
                errorDescription = string.Format("{0} The expected value is {1}", errorDescription, calculatedAmount);
            }

            var validationExceptionDetail = new IsValidationExceptionDetail
            {
                SerialNo = serialNumber,
                BillingEntityCode = miscUatpInvoice.BillingMember != null ? miscUatpInvoice.BillingMember.MemberCodeNumeric : string.Empty,
                BilledEntityCode = miscUatpInvoice.BilledMember != null ? miscUatpInvoice.BilledMember.MemberCodeNumeric : string.Empty,
                ChargeCategoryOrBillingCode = miscUatpInvoice.ChargeCategoryDisplayName,
                CategoryOfBilling = miscUatpInvoice.BillingCategoryId.ToString(),
                SubmissionFormat = submissionFormat,
                ErrorStatus = ((int)errorStatus).ToString(),
                ClearanceMonth = miscUatpInvoice.BillingYear + miscUatpInvoice.BillingMonth.ToString().PadLeft(2, '0'),
                PeriodNumber = miscUatpInvoice.BillingPeriod,
                BillingFileSubmissionDate = fileSubmissionDate.ToString("yyyyMMdd"),

                ErrorDescription = errorDescription,
                FieldName = fieldName,
                FieldValue = fieldValue,
                BatchUpdateAllowed = isBatchUpdateAllowed,

                InvoiceNumber = miscUatpInvoice.InvoiceNumber,
                ErrorLevel = errorLevel,
                ExceptionCode =  ErrorCodes.InvalidMemberReferenceDataInformation,
                SourceCodeId = chargecodeName,
                FileName = Path.GetFileName(fileName),
                LineItemDetailOrSequenceNo = lineItemDetailNumber,
                LineItemOrBatchNo = lineItemNumber,
                Id = Guid.NewGuid(),
                PkReferenceId = pkId
            };

            if (islinkingError)
            {
                validationExceptionDetail.YourInvoiceNo = miscUatpInvoice.RejectedInvoiceNumber;
                validationExceptionDetail.YourInvoiceBillingYear = miscUatpInvoice.SettlementYear;
                validationExceptionDetail.YourInvoiceBillingMonth = miscUatpInvoice.SettlementMonth;
                validationExceptionDetail.YourInvoiceBillingPeriod = miscUatpInvoice.SettlementPeriod;
                validationExceptionDetail.CorrespondenceRefNo = miscUatpInvoice.CorrespondenceRefNo.HasValue
                                                                    ? miscUatpInvoice.CorrespondenceRefNo.Value
                                                                    : 0;

                MiscUatpErrorCodes MiscUatpErrorCodes;
                if (miscUatpInvoice.BillingCategory.Equals(BillingCategoryType.Uatp))
                    MiscUatpErrorCodes = new UatpErrorCodes();
                else
                    MiscUatpErrorCodes = new MiscErrorCodes();

                if ( ErrorCodes.InvalidMemberReferenceDataInformation.CompareTo(MiscUatpErrorCodes.RejectionInvoiceNumberNotExist) == 0) //Rm
                    validationExceptionDetail.TransactionId = 38;
                else
                    validationExceptionDetail.TransactionId = 37;
            }

            return validationExceptionDetail;
        }
 

    }
}