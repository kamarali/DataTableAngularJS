using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using Castle.Core.Smtp;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Business.Pax;
using Iata.IS.Business.Reports.MiscUatp;
using Iata.IS.Business.TemplatedTextGenerator;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MiscUatp;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.Pax.Enums;
using NVelocity;
using TransactionType = Iata.IS.Model.Pax.Enums.TransactionType;
using Iata.IS.Model.Common;

namespace Iata.IS.Business.MiscUatp.Impl
{
    public class UatpCorrespondenceManager : IUatpCorrespondenceManager
    {

        /// <summary>
        /// Gets or sets the Uatp correspondence repository.
        /// </summary>
        /// <value>The country repository.</value>
        public IMiscCorrespondenceRepository MiscCorrespondenceRepository { get; set; }

        /// <summary>
        /// Gets or sets the Uatp correspondence Attachment repository.
        /// </summary>
        /// <value>The country repository.</value>
        public IMiscCorrespondenceAttachmentRepository MiscCorrespondenceAttachmentRepository { get; set; }

        /// <summary>
        /// Gets or sets the Uatp correspondence Attachment repository.
        /// </summary>
        /// <value>The country repository.</value>
        public IMiscInvoiceRepository MiscInvoiceRepository { get; set; }

        /// <summary>
        /// Gets or sets the line item repository.
        /// </summary>
        /// <value>The line item repository.</value>
        public ILineItemRepository LineItemRepository { get; set; }

        public IMiscInvoiceManager MiscInvoiceManager { get; set; }

        /// <summary>
        /// Gets or sets the reference manager.
        /// </summary>
        /// <value>The reference manager.</value>
        public IReferenceManager ReferenceManager { get; set; }

        public IMemberManager MemberManager { get; set; }

        /// <summary>
        /// Creates the Uatp correspondence.
        /// </summary>
        /// <param name="miscCorrespondence">The Uatp correspondence.</param>
        /// <returns></returns>
        public MiscCorrespondence AddCorrespondence(MiscCorrespondence miscCorrespondence)
        {
            if (ValidateCorrespondence(miscCorrespondence))
            {
                // Mark the correspondence status as Open.
                miscCorrespondence.CorrespondenceStatus = CorrespondenceStatus.Open;

                // Validation for misc. correspondence.

                MiscCorrespondenceRepository.Add(miscCorrespondence);
                //Added the following for look to increment the correspondence ref number if already present in database
                //TODO: need to change the count to 3 after UAT
                for (var tryCount = 0; tryCount < 25; tryCount++)
                {
                    try
                    {
                        UnitOfWork.CommitDefault();
                        tryCount = 25;
                    }
                    catch (Exception)
                    {
                        if (tryCount == 24 || miscCorrespondence.CorrespondenceStage > 1)
                            throw new ISBusinessException(ErrorCodes.InvalidCorrespondencRefNo);
                        miscCorrespondence.CorrespondenceNumber++;
                    }
                }

            }
            return miscCorrespondence;
        }

        /// <summary>
        /// Updates the Uatp correspondence.
        /// </summary>
        /// <param name="miscCorrespondence">The Uatp invoice.</param>
        /// <returns></returns>
        public MiscCorrespondence UpdateCorrespondence(MiscCorrespondence miscCorrespondence)
        {

            if (ValidateCorrespondence(miscCorrespondence))
            {
                //var miscUatpInvoiceInDb = MiscCorrespondenceRepository.Single(correspondence => correspondence.Id == miscCorrespondence.Id);
                // Call replaced by Load Strategy
                var miscUatpInvoiceInDb = MiscCorrespondenceRepository.Single(correspondenceId: miscCorrespondence.Id);
                //ValidateInvoice(miscUatpInvoice, miscUatpInvoiceInDb);

                miscCorrespondence.CorrespondenceStatus = CorrespondenceStatus.Open;
                var updatedCorrespondenceData = MiscCorrespondenceRepository.Update(miscCorrespondence);

                // Changes to update attachment breakdown records.
                var listToDeleteAttachment = miscUatpInvoiceInDb.Attachments.Where(attachment => miscCorrespondence.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0).ToList();

                var attachmentIdList = (from attachment in miscCorrespondence.Attachments
                                        where miscUatpInvoiceInDb.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0
                                        select attachment.Id).ToList();

                var rmCouponAttachmentInDb = MiscCorrespondenceAttachmentRepository.Get(couponAttachment => attachmentIdList.Contains(couponAttachment.Id));
                foreach (var recordAttachment in rmCouponAttachmentInDb)
                {
                    if (IsDuplicateCorrespondenceAttachmentFileName(recordAttachment.OriginalFileName, miscCorrespondence.Id))
                    {
                        throw new ISBusinessException(ErrorCodes.DuplicateFileName);
                    }

                    recordAttachment.ParentId = miscCorrespondence.Id;
                    MiscCorrespondenceAttachmentRepository.Update(recordAttachment);
                }
                foreach (var rmCouponRecordAttachment in listToDeleteAttachment)
                {
                    MiscCorrespondenceAttachmentRepository.Delete(rmCouponRecordAttachment);
                }

                UnitOfWork.CommitDefault();

                return updatedCorrespondenceData;

            }

            return miscCorrespondence;
        }

        /// <summary>
        /// Creates the correspondence format report in PDF format.
        /// </summary>
        /// <param name="correspondenceId"></param>
        public string CreateCorrespondenceFormatPdf(string correspondenceId)
        {

            var correspondenceDetails = GetCorrespondenceDetails(correspondenceId);
            string correspondenceFormatReportPath = FileIo.GetForlderPath(SFRFolderPath.CorrespondenceFormatReportPath) + correspondenceId.ToGuid() + ".pdf";

            var miscUatpReportManager = Ioc.Resolve<IMiscUatpReportManager>();

            var fromContact = GetContactDetails(correspondenceDetails.FromMemberId, ProcessingContactType.UatpCorrespondence);
            var toContact = GetContactDetails(correspondenceDetails.ToMemberId, ProcessingContactType.UatpCorrespondence);

            Location memberLocation;
            Country country;

            //If location id is provided, then address, country, postal code etc., should be taken for given location id else use same details as for contact
            if (fromContact != null)
            {
                if (fromContact.LocationId > 0)
                {
                    memberLocation = MemberManager.GetMemberLocationDetails(fromContact.LocationId);
                    fromContact.AddressLine1 = memberLocation.AddressLine1;
                    fromContact.AddressLine2 = memberLocation.AddressLine2;
                    fromContact.AddressLine3 = memberLocation.AddressLine3;
                    fromContact.Country = memberLocation.Country;
                    fromContact.PostalCode = memberLocation.PostalCode;
                    fromContact.CityName = memberLocation.CityName;
                }
                //If Country is not populated, populate it explicitly for given id
                if (fromContact.Country == null)
                {
                    var countryList = ReferenceManager.GetCountryList();
                    country = (from c in countryList
                               where c.Id == fromContact.CountryId
                               select c).FirstOrDefault();
                    fromContact.Country = country;
                }
            }

            //If location id is provided, then address, country, postal code etc., should be taken for given location id else use same details as for contact
            if (toContact != null)
            {
                if (toContact.LocationId > 0)
                {
                    memberLocation = MemberManager.GetMemberLocationDetails(toContact.LocationId);
                    toContact.AddressLine1 = memberLocation.AddressLine1;
                    toContact.AddressLine2 = memberLocation.AddressLine2;
                    toContact.AddressLine3 = memberLocation.AddressLine3;
                    toContact.Country = memberLocation.Country;
                    toContact.PostalCode = memberLocation.PostalCode;
                    toContact.CityName = memberLocation.CityName;
                }
                //If Country is not populated, populate it explicitly for given id
                if (toContact.Country == null)
                {
                    var countryList = ReferenceManager.GetCountryList();
                    country = (from c in countryList
                               where c.Id == toContact.CountryId
                               select c).FirstOrDefault();
                    toContact.Country = country;
                }

                miscUatpReportManager.BuildCorrespondenceFormatReport(correspondenceDetails, ref correspondenceFormatReportPath, fromContact, toContact);
            }

            return correspondenceFormatReportPath;
        }

        public Contact GetContactDetails(int memberId, ProcessingContactType processingContact)
        {
            var contactTypeList = MemberManager.GetContactsForContactType(memberId, processingContact);
            var contact = new Contact();
            if (contactTypeList != null)
            {
                contact = contactTypeList.FirstOrDefault();
            }
            return contact;
        }


        /// <summary>
        /// Check for duplicate file name of correspondence attachment
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <param name="miscCorrespondenceId">correspondence Id</param>
        /// <returns></returns>
        public bool IsDuplicateCorrespondenceAttachmentFileName(string fileName, Guid miscCorrespondenceId)
        {
            return MiscCorrespondenceAttachmentRepository.GetCount(attachment => attachment.ParentId == miscCorrespondenceId && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
        }

        /// <summary>
        /// Gets the numeric member code.
        /// </summary>
        /// <param name="memberCode">The member code.</param>
        /// <returns></returns>
        public int GetNumericMemberCode(string memberCode)
        {
            var index = 0;
            int value;

            if (Validators.IsWholeNumber(memberCode))
            {
                return Convert.ToInt32(memberCode);
            }

            var memberCodeAsciiChars = new byte[memberCode.Length];
            Encoding.ASCII.GetBytes(memberCode.ToUpper(), 0, memberCode.Length, memberCodeAsciiChars, 0);
            foreach (var memberCodeAsciiValue in memberCodeAsciiChars)
            {
                if (memberCodeAsciiValue <= 90 && memberCodeAsciiValue >= 65)
                {
                    //To get A = 10, B=11
                    value = memberCodeAsciiValue - 55;
                    string toReplace = memberCode.Substring(index, 1);
                    memberCode = memberCode.Replace(toReplace, value.ToString());
                }
                index++;
            }

            int numericMemberCode;
            int returnValue;
            if (Int32.TryParse(memberCode, out numericMemberCode))
            {
                returnValue = numericMemberCode > 9999 ? 0 : numericMemberCode;
            }
            else
            {
                returnValue = 0;
            }

            return returnValue;
        }

        /// <summary>
        /// Deletes the Uatp correspondence.
        /// </summary>
        /// <param name="correspondenceId">The Uatp correspondence id.</param>
        /// <returns></returns>
        public bool DeleteCorrespondence(string correspondenceId)
        {
            var correspondenceGuid = correspondenceId.ToGuid();
            //var correspondenceToBeDeleted = MiscCorrespondenceRepository.Single(correspondence => correspondence.Id == correspondenceGuid);
            // Call replaced by Load Strategy
            var correspondenceToBeDeleted = MiscCorrespondenceRepository.Single(correspondenceId: correspondenceGuid);
            if (correspondenceToBeDeleted == null) return false;
            MiscCorrespondenceRepository.Delete(correspondenceToBeDeleted);
            UnitOfWork.CommitDefault();
            return true;
        }

        /// <summary>
        /// Determines whether transaction exists for the specified correspondence id
        /// </summary>
        /// <param name="correspondenceId">The correspondence id.</param>
        /// <returns>
        /// 	<c>true</c> if transaction exists for the specified invoice id; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTransactionExists(string correspondenceId)
        {
            var correspondenceGuid = correspondenceId.ToGuid();
            var isTransactionExists = (MiscCorrespondenceRepository.GetCount(correspondence => correspondence.Id == correspondenceGuid) > 0);

            return isTransactionExists;
        }

        /// <summary>
        /// Function to retrieve correspondence details of the given correspondence id
        /// </summary>
        /// <param name="correspondenceId">correspondence id To Be fetched..</param>
        /// <returns></returns>
        public MiscCorrespondence GetCorrespondenceDetails(string correspondenceId)
        {
            var correspondenceGuid = correspondenceId.ToGuid();
            // Call replaced by Load Strategy
            var correspondenceHeader = MiscCorrespondenceRepository.Single(correspondenceId: correspondenceGuid);
            return correspondenceHeader;
        }

        /// <summary>
        /// Function to retrieve correspondence details of the given correspondence id
        /// </summary>
        /// <param name="correspondenceNumber"></param>
        /// <returns></returns>
        public MiscCorrespondence GetCorrespondenceDetails(long? correspondenceNumber)
        {
            //var corrrespondence = MiscCorrespondenceRepository.Get(correspondence => correspondence.CorrespondenceNumber == correspondenceNumber).OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
            // Call replaced by Load Strategy
            var corrrespondence = MiscCorrespondenceRepository.Get(correspondenceNumber: correspondenceNumber).OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
            return corrrespondence;
        }

        /// <summary>
        /// Function to retrieve correspondence details of the given correspondence id for authority to bill
        /// </summary>
        /// <param name="correspondenceNumber"></param>
        /// <returns></returns>
        public MiscCorrespondence GetCorrespondenceForAuthorityToBillDetails(long? correspondenceNumber)
        {
            //var corrrespondence = MiscCorrespondenceRepository.Get(correspondence => correspondence.CorrespondenceNumber == correspondenceNumber && correspondence.CorrespondenceStatusId == 1 && correspondence.AuthorityToBill).OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
            //// Call replaced by Load Strategy
            var corrrespondence =
              MiscCorrespondenceRepository.Get(correspondenceNumber: correspondenceNumber, correspondenceStatusId: 1, authorityToBill: true).OrderByDescending(
                correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
            return corrrespondence;
        }

        /// <summary>
        /// Function to retrieve correspondence details of the given correspondence id - If billing member is To Member of the correspondence then he should not allow to view the Saved and Ready to Submit correspondences of the other member.
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="billingMemberId"></param>
        /// <returns></returns>
        public MiscCorrespondence GetRecentCorrespondenceDetails(string invoiceId, int billingMemberId)
        {
            var invoiceGuid = invoiceId.ToGuid();
            var corrrespondence = MiscCorrespondenceRepository.GetCorr(correspondence => correspondence.InvoiceId == invoiceGuid
              && (correspondence.FromMemberId == billingMemberId || (correspondence.ToMemberId == billingMemberId
              && (correspondence.CorrespondenceStatusId == 2 || correspondence.CorrespondenceStatusId == 3 || (correspondence.CorrespondenceStatusId == 1
              && correspondence.CorrespondenceSubStatusId == 2)))
              )).OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
            return corrrespondence;
        }

        /// <summary>
        /// Function to retrieve correspondence details of the given correspondence id
        /// </summary>
        /// <param name="correspondenceNumber"></param>
        /// <returns></returns>
        public MiscCorrespondence GetOriginalCorrespondenceDetails(long? correspondenceNumber)
        {
            //var corrrespondence = MiscCorrespondenceRepository.Get(correspondence => correspondence.CorrespondenceNumber == correspondenceNumber).OrderBy(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
            // Call replaced by Load Strategy
            var corrrespondence = MiscCorrespondenceRepository.Get(correspondenceNumber: correspondenceNumber).OrderBy(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
            return corrrespondence;
        }


        /// <summary>
        /// Retrieve recent Correspondence Id for provided invoice id. 
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public MiscCorrespondence GetRecentCorrespondenceDetails(string invoiceId)
        {
            var invoiceGuid = invoiceId.ToGuid();
            //var correspondence = MiscCorrespondenceRepository.Get(corr => corr.InvoiceId == invoiceGuid).OrderByDescending(c => c.CorrespondenceStage).FirstOrDefault();
            // Call replace by Load strategy
            var correspondence = MiscCorrespondenceRepository.Get(invoiceId: invoiceGuid).OrderByDescending(c => c.CorrespondenceStage).FirstOrDefault();
            return correspondence;
        }

        public MiscCorrespondence GetFirstCorrespondenceDetails(string invoiceId)
        {
            var invoiceGuid = invoiceId.ToGuid();
            //var correspondence = MiscCorrespondenceRepository.Get(corr => corr.InvoiceId == invoiceGuid).OrderBy(c => c.CorrespondenceStage).FirstOrDefault();
            // Call replaced by load strategy
            var correspondence = MiscCorrespondenceRepository.Get(invoiceId: invoiceGuid).OrderBy(c => c.CorrespondenceStage).FirstOrDefault();
            return correspondence;
        }

        /// <summary>
        /// Function to retrieve correspondence number of the given member id
        /// </summary>
        /// <param name="memberId">Member id .</param>
        /// <returns></returns>
        public long GetCorrespondenceNumber(int memberId)
        {
            //var correspondence = MiscCorrespondenceRepository.Get(c => c.FromMemberId == memberId).OrderByDescending(c => c.CorrespondenceNumber).FirstOrDefault();
            // Call replaced by load strategy
            var correspondence = MiscCorrespondenceRepository.Get(fromMemberId: memberId, correspondenceStage: 1).OrderByDescending(c => c.CorrespondenceNumber).FirstOrDefault();
            long correspondNumber = 0;

            if (correspondence != null && correspondence.CorrespondenceNumber != null)
                correspondNumber = correspondence.CorrespondenceNumber.Value;

            return correspondNumber;
        }


        /// <summary>
        /// Function to retrieve correspondence number of the given member id
        /// </summary>
        /// <param name="memberId">Member id .</param>
        /// <returns></returns>
        public bool IsFirstCorrespondence(int memberId)
        {
            //var correspondence = MiscCorrespondenceRepository.Get(c => c.FromMemberId == memberId && c.CorrespondenceStage == 1).FirstOrDefault();
            // Call replaced by Load Strategy
            var correspondence = MiscCorrespondenceRepository.Get(fromMemberId: memberId, correspondenceStage: 1).FirstOrDefault();
            if (correspondence == null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Function to retrieve invoice details of the given correspondence id
        /// </summary>
        /// <param name="invoiceId">invoice id To Be fetched..</param>
        /// <returns></returns>
        public MiscUatpInvoice GetInvoiceDetail(string invoiceId)
        {
            return MiscInvoiceManager.GetInvoiceDetail(invoiceId);
        }

        /// <summary>
        /// Gets the Uatp Correspondence attachments.
        /// </summary>
        /// <param name="attachmentIds">The attachment ids.</param>
        /// <returns></returns>
        public List<MiscUatpCorrespondenceAttachment> GetMiscCorrespondenceAttachments(List<Guid> attachmentIds)
        {
            return MiscCorrespondenceAttachmentRepository.Get(attachment => attachmentIds.Contains(attachment.Id)).ToList();
        }


        /// <summary>
        /// Add Uatp Correspondence Attachment record
        /// </summary>
        /// <param name="attach">Misc Correspondence Attachment record</param>
        /// <returns></returns>
        public MiscUatpCorrespondenceAttachment AddRejectionMemoAttachment(MiscUatpCorrespondenceAttachment attach)
        {
            MiscCorrespondenceAttachmentRepository.Add(attach);

            UnitOfWork.CommitDefault();

            return attach;
        }

        /// <summary>
        /// Updates the Uatp Correspondence attachment.
        /// </summary>
        /// <param name="attachments">The attachments.</param>
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        public IList<MiscUatpCorrespondenceAttachment> UpdateMiscCorrespondenceAttachment(IList<Guid> attachments, Guid parentId)
        {
            var attachmentInDb = MiscCorrespondenceAttachmentRepository.Get(miscCorrespondence => attachments.Contains(miscCorrespondence.Id));
            foreach (var recordAttachment in attachmentInDb)
            {
                recordAttachment.ParentId = parentId;
                MiscCorrespondenceAttachmentRepository.Update(recordAttachment);
            }
            UnitOfWork.CommitDefault();
            return attachmentInDb.ToList();
        }

        /// <summary>
        /// Gets the Uatp Correspondence attachment detail.
        /// </summary>
        /// <param name="attachmentId">The attachment id.</param>
        /// <returns></returns>
        public MiscUatpCorrespondenceAttachment GetMiscCorrespondenceAttachmentDetail(string attachmentId)
        {
            var attachmentGuid = attachmentId.ToGuid();
            var attachmentRecord = MiscCorrespondenceAttachmentRepository.Single(attachment => attachment.Id == attachmentGuid);

            return attachmentRecord;
        }

        /// <summary>
        /// Determines whether specified file name already exists for given invoice.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="correspondenceId">The correspondence id.</param>
        /// <returns>
        /// true if specified file name found in repository; otherwise, false.
        /// </returns>
        public bool IsDuplicateMiscCorrespondenceAttachmentFileName(string fileName, Guid correspondenceId)
        {
            return MiscCorrespondenceAttachmentRepository.GetCount(attachment => attachment.ParentId == correspondenceId && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
        }

        /// <summary>
        /// Retrieve Correspondence History List
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public IList<MiscCorrespondence> GetCorrespondenceHistoryList(string invoiceId)
        {
            var invoiceGuid = invoiceId.ToGuid();
            //var correspondenceHistoryList = MiscCorrespondenceRepository.Get(corr => corr.InvoiceId == invoiceGuid).ToList();
            // Call replaced by Load Strategy
            var correspondenceHistoryList = MiscCorrespondenceRepository.Get(invoiceId: invoiceGuid).ToList();
            return correspondenceHistoryList;
        }

        /// <summary>
        /// Retrieve Correspondence History List
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="billingMemberId"></param>
        /// <returns></returns>
        public IList<MiscCorrespondence> GetCorrespondenceHistoryList(string invoiceId, int billingMemberId)
        {
            var invoiceGuid = invoiceId.ToGuid();

            var correspondenceHistoryList = MiscCorrespondenceRepository.GetCorr(correspondence => correspondence.InvoiceId == invoiceGuid
              && (correspondence.FromMemberId == billingMemberId || (correspondence.ToMemberId == billingMemberId
              && (correspondence.CorrespondenceStatusId == 2 || correspondence.CorrespondenceStatusId == 3 || (correspondence.CorrespondenceStatusId == 1
              && correspondence.CorrespondenceSubStatusId == 2)))
              )).ToList();

            return correspondenceHistoryList;
        }

        /// <summary>
        /// Retrieve Correspondence Rejection List
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public IList<MiscUatpInvoice> GetCorrespondenceRejectionList(string invoiceId)
        {
            var invoiceGuid = invoiceId.ToGuid();

            var invoiceList = MiscInvoiceRepository.Get(inv => inv.Id == invoiceGuid).ToList();

            return invoiceList;
        }

        /// <summary>
        /// Validates the correspondence.
        /// </summary>
        /// <param name="miscCorrespondence"></param>
        /// <returns></returns>
        public bool ValidateCorrespondence(MiscCorrespondence miscCorrespondence)
        {

            // Validation for CityAirport as it is auto complete field in UI
            var toEmailId = miscCorrespondence.ToEmailId != null ? miscCorrespondence.ToEmailId.Trim() : miscCorrespondence.ToEmailId;
            var additionalEmailId = miscCorrespondence.ToAdditionalEmailIds != null ? miscCorrespondence.ToAdditionalEmailIds.Trim() : miscCorrespondence.ToAdditionalEmailIds;
            if (string.IsNullOrEmpty(toEmailId + additionalEmailId))
            {
                throw new ISBusinessException(MiscUatpErrorCodes.EnterEmailIds);
            }

            // Validation of Correspondence Subject
            if (string.IsNullOrEmpty(miscCorrespondence.Subject))
            {
                throw new ISBusinessException(MiscUatpErrorCodes.InvalidCorrespondenceSubject);
            }

            // Validation for CityAirport as it is auto complete field in UI
            if (miscCorrespondence.AmountToBeSettled < 0)
            {
                throw new ISBusinessException(MiscUatpErrorCodes.InvalidAmountToBeSettled);
            }

            // Validation for CityAirport as it is auto complete field in UI
            if (string.IsNullOrEmpty(miscCorrespondence.CorrespondenceNumber.ToString()))
            {
                throw new ISBusinessException(MiscUatpErrorCodes.InvalidCorrespondenceNumber);
            }

            // Validation for CityAirport as it is auto complete field in UI
            if (miscCorrespondence.CurrencyId == 0)
            {
                throw new ISBusinessException(MiscUatpErrorCodes.InvalidCorrespondenceNumber);
            }
            var invoice = GetInvoiceDetail(miscCorrespondence.InvoiceId.ToString());
            //You cannot send a correspondence if it is expired
            if ((miscCorrespondence.CorrespondenceSubStatus == CorrespondenceSubStatus.Responded))
            {
                var isOutSideTimeLimit = false;

                if (invoice.InvoiceSmi != SMI.Ach)
                {
                    isOutSideTimeLimit = !ReferenceManager.IsTransactionInTimeLimitMethodC(TransactionType.MiscOtherCorrespondence, invoice.SettlementMethodId, miscCorrespondence.CorrespondenceDate);
                }
                else
                {
                    isOutSideTimeLimit = !ReferenceManager.IsTransactionInTimeLimitMethodG(TransactionType.MiscOtherCorrespondence, invoice.SettlementMethodId, miscCorrespondence.CorrespondenceDate);
                }

                if (isOutSideTimeLimit)
                {
                    throw new ISBusinessException(MiscUatpErrorCodes.ExpiredCorrespondence);
                }
            }

            if (!HasValidAuthorityToBill(miscCorrespondence))
            {
                throw new ISBusinessException(MiscUatpErrorCodes.InvalidAuthorityToBill);
            }

            string toEmailIds = miscCorrespondence.ToEmailId + ";" + miscCorrespondence.ToAdditionalEmailIds;

            if (ValidateToEmailIds(toEmailIds) == false)
            {
                throw new ISBusinessException(MiscUatpErrorCodes.InvalidEmailIds);
            }

            // Updates correspondence expiry date.
            UpdateExpiryDate(miscCorrespondence, invoice);

            return true;
        }



        private bool HasValidAuthorityToBill(MiscCorrespondence miscCorrespondence)
        {
            if (miscCorrespondence.AuthorityToBill)
            {
                var correspondence = GetOriginalCorrespondenceDetails(miscCorrespondence.CorrespondenceNumber);
                if (correspondence != null)
                {
                    if (correspondence.FromMemberId == miscCorrespondence.FromMemberId)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Validates the correspondence.
        /// </summary>
        /// <param name="toEmailId"></param>
        /// <returns></returns>
        public bool ValidateToEmailIds(string toEmailId)
        {

            if (toEmailId == null)
            {
                return false;
            }
            toEmailId = toEmailId.Replace("\r", string.Empty).Replace("\n", string.Empty);
            string[] eMailIds = toEmailId.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            if (eMailIds.All(emailId => string.IsNullOrEmpty(emailId.Trim()))) return false;

            if (eMailIds.Any(eMailId => IsValidEmailId(eMailId.Trim()) == false))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the member of provided member Id .
        /// </summary>
        /// <param name="memberId">The Member Id.</param>
        /// <returns></returns>
        public Member GetMember(int memberId)
        {
            return MemberManager.GetMember(memberId);
        }

        /// <summary>
        /// Get the member of provided member Id .
        /// </summary>
        /// <param name="memberId">The Member Id.</param>
        /// <param name="processingContact"></param>
        /// <returns></returns>
        public string GetToEmailIds(int memberId, ProcessingContactType processingContact)
        {
            var toMailIds = new StringBuilder();
            var index = 0;

            List<Contact> contactTypeList = MemberManager.GetContactsForContactType(memberId, processingContact);
            if (contactTypeList != null)
            {
                foreach (var contact in contactTypeList)
                {
                    index += 1;
                    toMailIds.Append(index != contactTypeList.Count ? string.Format("{0}{1}", contact.EmailAddress, ";") : contact.EmailAddress);
                }
            }

            return toMailIds.ToString();
        }

        public static bool IsValidEmailId(string inputEmailId)
        {
            if (inputEmailId != null)
            {
                const string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                var re = new Regex(strRegex);
                if (re.IsMatch(inputEmailId)) return (true);
            }
            return false;
        }

        /// <summary>
        /// Get the charge codes for provided invoice Id .
        /// </summary>
        /// <param name="invoiceId">The invoice Id.</param>
        /// <returns></returns>
        public string GetChargeCodes(string invoiceId)
        {
            var invoiceGuid = invoiceId.ToGuid();
            var lineItemList = LineItemRepository.Get(invoiceGuid).ToList();
            var chargeCodes = string.Empty;

            if (lineItemList.Count > 1)
            {
                chargeCodes = lineItemList.Aggregate(chargeCodes, (current, li) => String.Format("{0},{1}", current, li.DisplayChargeCode));
            }
            else if (lineItemList.Count == 1)
            {
                chargeCodes = lineItemList[0].DisplayChargeCode;
            }

            return chargeCodes;
        }

        /// <summary>
        /// Gets the error description.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="mailId">The mail id.</param>
        /// <returns></returns>
        private static string GetErrorDescription(string errorCode, string mailId)
        {
            var errorDescription = Messages.ResourceManager.GetString(errorCode);

            // Replace place holders in error message with appropriate record names.
            if (!string.IsNullOrEmpty(errorDescription))
                errorDescription = string.Format(errorDescription, mailId);
            return errorDescription;
        }

        /// <summary>
        /// Function to send correspondence email
        /// </summary>
        /// <param name="correspondPageUrl">Correspond Page Url.</param>
        /// <param name="toEmailIds">To email id's</param>
        /// <param name="subject">Email subject</param>
        /// <returns> bool </returns>
        public bool SendCorrespondenceMail(string correspondPageUrl, string toEmailIds, string subject)
        {
            var templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));
            var context = new VelocityContext();

            context.Put("CorrespondenceUrl", correspondPageUrl);
            context.Put("SisOpsEmailId", AdminSystem.SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);
            var messageBody = templatedTextGenerator.GenerateTemplatedText(EmailTemplateId.CorrespondenceResponse, context);

            var emailSender = Ioc.Resolve<IEmailSender>();

            string[] eMailIds = toEmailIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var emailSettingsRepository = Ioc.Resolve<IRepository<EmailTemplate>>(typeof(IRepository<EmailTemplate>));

            var emailSettingForCorrespondence = emailSettingsRepository.Get(es => es.Id == (int)EmailTemplateId.CorrespondenceResponse);
            foreach (var eMailId in eMailIds)
            {
                if (eMailId != null && !string.IsNullOrEmpty(eMailId.Trim()))
                {
                    var mailMessage = new MailMessage(emailSettingForCorrespondence.SingleOrDefault().FromEmailAddress, eMailId.Trim(), subject, messageBody) { IsBodyHtml = true };
                    try
                    {
                        emailSender.Send(mailMessage);
                    }
                    catch (Exception)
                    {
                        throw new ISBusinessException(GetErrorDescription(MiscUatpErrorCodes.FailedToSendMail, eMailId));
                    }
                }
            }

            return true;
        }

        public List<ExpiredCorrespondence> UpdateCorrespondenceStatus()
        {
            return MiscCorrespondenceRepository.UpdateCorrespondenceStatus();
        }

        /// <summary>
        /// Returns true if billing memo created with given correspondence reference number.
        /// </summary>
        /// <param name="billingMemberId"></param>
        /// <param name="correspondenceRefNumber"></param>
        /// <param name="invoiceNumber"></param>
        /// <returns></returns>
        public string IsCorrespondenceInvoiceExistsForCorrespondence(int billingMemberId, long correspondenceRefNumber)
        {
            var miscInvoiceList = MiscInvoiceRepository.Get(
              invoice =>
              invoice.InvoiceTypeId == (int)InvoiceType.CorrespondenceInvoice &&
              invoice.CorrespondenceRefNo == correspondenceRefNumber && invoice.BillingMemberId == billingMemberId).ToList();
            return miscInvoiceList.Count > 0 ? miscInvoiceList[0].InvoiceNumber : null;

            //return MiscInvoiceRepository.GetCount(
            //  invoice => invoice.InvoiceTypeId == (int)InvoiceType.CorrespondenceInvoice && invoice.CorrespondenceRefNo == correspondenceRefNumber && invoice.BillingMemberId == billingMemberId) > 0;
        }

        /// <summary>
        /// Validates, user can create correspondence or not if rejection invoice is out side time limit.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>
        /// true if [is valid correspondence time limit] [the specified misc correspondence]; otherwise, false.
        /// </returns>
        public bool IsCorrespondenceOutSideTimeLimit(string invoiceId)
        {
            var isOutsideTimeLimit = false;
            var invoiceHeader = GetInvoiceDetail(invoiceId);

            if (invoiceHeader.RejectionStage == 1)
            {
                if (invoiceHeader.InvoiceSmi == SMI.Ich || invoiceHeader.InvoiceSmi == SMI.AchUsingIataRules)
                {
                    isOutsideTimeLimit =
                      !ReferenceManager.IsTransactionInTimeLimitMethodB(TransactionType.MiscCorrespondence, invoiceHeader.SettlementMethodId, DateTime.UtcNow, invoiceHeader);
                }
                else if (invoiceHeader.InvoiceSmi == SMI.Bilateral)
                {
                    isOutsideTimeLimit =
                      !ReferenceManager.IsTransactionInTimeLimitMethodB1(TransactionType.MiscCorrespondence, invoiceHeader.SettlementMethodId, DateTime.UtcNow, invoiceHeader);
                }
            }
            else if (invoiceHeader.RejectionStage == 2 && invoiceHeader.InvoiceSmi == SMI.Ach)
            {
                isOutsideTimeLimit = !ReferenceManager.IsTransactionInTimeLimitMethodF(TransactionType.MiscCorrespondence, invoiceHeader.SettlementMethodId, invoiceHeader);
            }

            return isOutsideTimeLimit;
        }

        /// <summary>
        /// Updates the expiry date.
        /// </summary>
        /// <param name="correspondence">The correspondence.</param>
        /// <param name="invoice">The invoice.</param>
        private void UpdateExpiryDate(MiscCorrespondence correspondence, InvoiceBase invoice)
        {
            DateTime expiryDate;
            if (correspondence.CorrespondenceStatus == CorrespondenceStatus.Open && correspondence.AuthorityToBill)
            {
                expiryDate = invoice.InvoiceSmi != SMI.Bilateral
                               ? ReferenceManager.GetTimeLimitMethodExpiryDate(TransactionType.MiscCorrInvoiceDueToAuthorityToBill, invoice.SettlementMethodId, correspondence.CorrespondenceDate)
                               : ReferenceManager.GetTimeLimitMethodD1ExpiryDate(TransactionType.MiscCorrInvoiceDueToAuthorityToBill, invoice.SettlementMethodId, correspondence.CorrespondenceDate);

                if (expiryDate != correspondence.CorrespondenceDate) correspondence.ExpiryDate = expiryDate;
            }
            else
            {
                if (invoice.InvoiceSmi == SMI.Ach)
                {
                    expiryDate = ReferenceManager.GetTimeLimitMethodExpiryDateMethodG(TransactionType.MiscOtherCorrespondence, invoice.SettlementMethodId, correspondence.CorrespondenceDate);
                }
                else
                {
                    expiryDate = ReferenceManager.GetTimeLimitMethodExpiryDate(TransactionType.MiscOtherCorrespondence, invoice.SettlementMethodId, correspondence.CorrespondenceDate);
                }
            }
            if (expiryDate != correspondence.CorrespondenceDate)
            {
                correspondence.ExpiryDate = expiryDate;
            }
        }

    }
}
