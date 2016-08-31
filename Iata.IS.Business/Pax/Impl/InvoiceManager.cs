using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Common.Impl;
using Iata.IS.Business.FileCore;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Data.Pax;
using Iata.IS.Data.Reports;
using Iata.IS.Ich.Interface;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Common;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Model.Pax;
using Iata.IS.Model.Pax.AutoBilling;
using Iata.IS.Model.Pax.Base;
using Iata.IS.Model.Pax.BillingHistory;
using Iata.IS.Model.Pax.Common;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Model.Enums;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using System.IO;
using SupportingAttachmentStatus = Iata.IS.Model.Enums.SupportingAttachmentStatus;
using System.Globalization;
using Iata.IS.Business.Common;
using Iata.IS.Business.BroadcastMessages.Impl;
using Iata.IS.Model.BroadcastMessages;
using NVelocity;
using System.Net;
using log4net;
using SearchCriteria = Iata.IS.Model.Pax.SearchCriteria;
using Iata.IS.Data.Common;
using Iata.IS.Business.Security;
using System.Data.Objects;

namespace Iata.IS.Business.Pax.Impl
{
    public class InvoiceManager : InvoiceManagerBase, IInvoiceManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        ///  Invoice TotalVat Repository.
        /// </summary>
        public IInvoiceTotalVatRepository InvoiceTotalVatRepository { get; set; }

        /// <summary>
        /// Calendar Manager, will be injected by the container
        /// </summary>
        /// <value>
        /// The calendar manager repository.
        /// </value>
        //public ICalendarManager CalendarManager { get; set; }

        public ISamplingFormCRepository SamplingFormCRepository { get; set; }

        /// <summary>
        /// Source Code Total Repository.
        /// </summary>
        public ISourceCodeTotalRepository SourceCodeTotalRepository { get; set; }

        /// <summary>
        /// Sampling formE Repository.
        /// </summary>
        public ISamplingFormERepository SamplingFormERep { get; set; }

        /// <summary>
        /// Gets or sets the invoice total repository.
        /// </summary>
        /// <value>The invoice total repository.</value>
        public IRepository<InvoiceTotal> InvoiceTotalRepository { get; set; }

        /// <summary>
        /// InvoiceVat Repository, will be injected by the container.
        /// </summary>
        public IRepository<InvoiceVat> InvoiceVatRepository { get; set; }

        /// <summary>
        /// Gets or sets SourceCodevatTotal repository
        /// </summary>
        public ISourceCodeVatRecordRepository SourceCodeVatTotalRepository { get; set; }

        /// <summary>
        /// Gets or sets the source code repository.
        /// </summary>
        /// <value>The source code repository.</value>
        public IRepository<SourceCode> SourceCodeRepository { get; set; }

        /// <summary>
        /// Gets or sets the source code repository.
        /// </summary>
        /// <value>The source code repository.</value>
        public IRepository<ReasonCode> ReasonCodeRepository { get; set; }

        /// <summary>
        /// Gets or sets the coupon record repository.
        /// </summary>
        /// <value>The coupon record repository.</value>
        public ICouponRecordRepository CouponRecordRepository { get; set; }

        /// <summary>
        /// Gets or sets the rejection record repository.
        /// </summary>
        public IRejectionMemoRecordRepository RejectionRecordRepository { get; set; }

        /// <summary>
        /// Gets or sets the country repository.
        /// </summary>
        /// <value>The country repository.</value>
        public IRepository<Country> CountryRepository { get; set; }

        /// <summary>
        /// Gets or sets the correspondence repository.
        /// </summary>
        /// <value>The correspondence repository.</value>
        public IRepository<Correspondence> CorrespondenceRepository { get; set; }

        /// <summary>
        /// Gets or sets InvoiceBase repository.
        /// </summary>
        public IRepository<InvoiceBase> InvoiceBaseRepository { get; set; }

        public IRepository<RMReasonAcceptableDiff> RmReasonAcceptableDifferenceRepository { get; set; }

        /// <summary>
        /// IsInputFile Repository.
        /// </summary>
        public IRepository<IsInputFile> IsInputFileRepository { get; set; }

        /// <summary>
        /// Gets or sets the rejection memo coupon breakdown repository.
        /// </summary>
        /// <value>The rejection memo coupon breakdown repository.</value>
        public IRMCouponBreakdownRecordRepository RejectionMemoCouponBreakdownRepository { get; set; }

        public IPaxCorrespondenceManager CorrespondenceManager { get; set; }

        /// <summary>
        /// Pax repository, will be injected by container.
        /// </summary>
        /// <value>The misc invoice repository.</value>
        public IInvoiceRepository PaxInvoiceRepository { get; set; }

        /// <summary>
        /// Pax Correspondence repository
        /// </summary>
        public IPaxCorrespondenceRepository PaxCorrespondenceRepository { get; set; }

        public IRemoveInvoiceDupCheck RemoveInvoiceDuplicateCheck { get; set; }

        /// <summary>
        /// Gets or sets the rejection memo record repository.
        /// </summary>
        /// <value>
        /// The rejection memo record repository.
        /// </value>
        public IRejectionMemoRecordRepository RejectionMemoRepository { get; set; }

        /// <summary>
        /// Invoice Deleted Audit Repository, will be injected by the container.
        /// </summary>
        public IRepository<AuditDeletedInvoice> InvoiceDeletedAuditRepository { get; set; }

        /// <summary>
        /// Gets or sets the billing memo repository.
        /// </summary>
        /// <value>The billing memo repository.</value>
        public IBillingMemoRecordRepository BillingMemoRepository {
            get;
            set;
        }

        public INonSamplingInvoiceManager NonSamplingInvoiceManager
        {
          get;
          set;
        }

        public IQueryAndDownloadDetailsManager QueryAndDownloadDetailsManager
        {
          get;
          set;
        }


        private UnitOfWork _unitOfWork = new UnitOfWork(new ObjectContextAdapter());


        private const string ReasonCode6B = "6B";
        private const string ReasonCode6A = "6A";

        private MiscUatpErrorCodes MiscUatpErrorCodes;

        /// <summary>
        /// Dictionary object used to store whether Billed is in Invoice is migrated in SIS.
        /// </summary>
        public Dictionary<string, int> IsBilledMemberMigration = new Dictionary<string, int>();
        


    public InvoiceManager()
    {
    }

    public InvoiceManager(MiscUatpErrorCodes errorCodes)
    {
      MiscUatpErrorCodes = errorCodes;
    }

        /// <summary>
        /// Function to add Header information to the invoice
        /// </summary>
        /// <param name="invoiceHeader">Invoice To be Added.</param>
        /// <returns></returns>
        public virtual PaxInvoice CreateInvoice(PaxInvoice invoiceHeader)
        {
            // Mark the invoice status as Open.
            invoiceHeader.InvoiceStatus = InvoiceStatusType.Open;

            // updating validation status to Pending
            invoiceHeader.ValidationStatus = InvoiceValidationStatus.Pending;
            invoiceHeader.ValidationDate = DateTime.MinValue;

            invoiceHeader.BillingCategory = BillingCategoryType.Pax;

            //fixed for issue id.5713 
            invoiceHeader.SettlementFileStatus = InvoiceProcessStatus.NotSet;

            //CMP602
            if(ValidateInvoiceHeader(invoiceHeader, null))
            {
              SetViewableByClearingHouse(invoiceHeader);
            }

          bool isProvisionalBillingMemberMigrated = false;
            bool isMemberMigratedForFormC = false;
            // Create Invoice Total Object and Form E object depending on invoice type.
            // also injecting the SamplingConstant in total.
            if (invoiceHeader.BillingCode == (int)BillingCode.SamplingFormDE)
            {

                // update the linking related flags.
                isProvisionalBillingMemberMigrated = SetMigrationFlags(invoiceHeader, ref isMemberMigratedForFormC);

                if (invoiceHeader.SamplingFormEDetails == null)
                {
                    invoiceHeader.SamplingFormEDetails = new SamplingFormEDetail { SamplingConstant = invoiceHeader.SamplingConstant };
                }

                if (isMemberMigratedForFormC && SamplingFormCRepository.GetCount(frmC => frmC.FromMemberId == invoiceHeader.BillingMemberId && frmC.ProvisionalBillingMemberId == invoiceHeader.BilledMemberId
                && frmC.ProvisionalBillingMonth == invoiceHeader.ProvisionalBillingMonth && frmC.ProvisionalBillingYear == invoiceHeader.ProvisionalBillingYear) == 0)
                {
                    throw new ISBusinessException(SamplingErrorCodes.MemberMigratedForFormCButDataNotFound);
                }
            }
            else
            {
                // Add an invoice total record with zero values.))
                if (invoiceHeader.InvoiceTotalRecord == null)
                {
                    invoiceHeader.InvoiceTotalRecord = new InvoiceTotal { SamplingConstant = invoiceHeader.SamplingConstant };
                }
            }

            InvoiceRepository.Add(invoiceHeader);
            UnitOfWork.CommitDefault();

            // Remove invoice Details from from DUP_INVOICE_LOG and DUP_INVOICE_TEMP tables (used to enforce duplicate check on invoice).
            try
            {
              RemoveInvoiceDuplicateCheck.RemoveDupCheckForInvoice(invoiceHeader.Id);
            }// End try
            catch (Exception ex)
            {
              Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1}", ex.Message, ex.StackTrace);
            }// End catch
            

            // Duplicate invoice check if save invoice button is clicked simultaneously.
            InvoiceRepository.Refresh(invoiceHeader);
            Logger.InfoFormat("InvoiceStatus:{0}, ValidationStatus:{1}", invoiceHeader.InvoiceStatus, invoiceHeader.ValidationStatus);

            if (invoiceHeader.InvoiceStatus == InvoiceStatusType.ValidationError && invoiceHeader.ValidationStatus == InvoiceValidationStatus.Failed)
            {
              // Delete invoice from DB.
              InvoiceRepository.Delete(invoiceHeader);
              UnitOfWork.CommitDefault();
              Logger.Info("Throw Duplicate Invoice Exception");
              throw new ISBusinessException(ErrorCodes.DuplicateInvoiceFound);
            }

            var billingMember = invoiceHeader.MemberLocationInformation.Where(location => location.IsBillingMember).FirstOrDefault();
            if (billingMember != null)
            {
                billingMember.InvoiceId = invoiceHeader.Id;
                UpdateMemberLocationInformation(billingMember, invoiceHeader, true);
            }

            var billedMember = invoiceHeader.MemberLocationInformation.Where(location => !location.IsBillingMember).FirstOrDefault();
            if (billedMember != null)
            {
                billedMember.InvoiceId = invoiceHeader.Id;
                UpdateMemberLocationInformation(billedMember, invoiceHeader, false);
            }

            if (invoiceHeader.BillingCode == (int)BillingCode.SamplingFormDE)
            {
                if (isProvisionalBillingMemberMigrated || isMemberMigratedForFormC)
                {
                    // Call stored procedure and remove following code
                    SamplingFormERep.UpdateFormEDetails(invoiceHeader.Id);
                }
            }

            return invoiceHeader;
        }

        private bool SetMigrationFlags(PaxInvoice invoiceHeader, ref bool isLinkingSuccessfulForFormC)
        {
            // Check for Form AB migration. Get passenger configuration for billed member of Form DE.
            var passengerConfiguration = MemberManager.GetPassengerConfiguration(invoiceHeader.BilledMemberId);
            bool isProvisionalBillingMemberMigrated = false;
            if (passengerConfiguration != null)
            {
                isProvisionalBillingMemberMigrated = IsProvisionalBillingMemberMigrated(invoiceHeader.ProvisionalBillingMonth, invoiceHeader.ProvisionalBillingYear, passengerConfiguration.SamplingProvIsIdecMigratedDate, passengerConfiguration.SamplingProvIsxmlMigratedDate, passengerConfiguration.SamplingProvIsIdecMigrationStatus, passengerConfiguration.SamplingProvIsxmlMigrationStatus);
            }

            // Check for Form C migration. Get passenger configuration for billing member of Form DE.
            passengerConfiguration = MemberManager.GetPassengerConfiguration(invoiceHeader.BillingMemberId);

            // explicitly checking if form C data exists in database before migration check as Form C can be created 
            // through IS-WEB and can exist when even member has not migrated. Such is not the case for Form A/B.
            var isFormCFound =
              SamplingFormCRepository.GetCount(
                frmC =>
                frmC.FromMemberId == invoiceHeader.BillingMemberId && frmC.ProvisionalBillingMemberId == invoiceHeader.BilledMemberId && frmC.ProvisionalBillingMonth == invoiceHeader.ProvisionalBillingMonth &&
                frmC.ProvisionalBillingYear == invoiceHeader.ProvisionalBillingYear) > 0;

            if (isFormCFound)
            {
                isLinkingSuccessfulForFormC = true;
            }
            else
            {
                if (passengerConfiguration != null)
                {
                    bool isMemberMigratedForFormC = IsProvisionalBillingMemberMigrated(invoiceHeader.ProvisionalBillingMonth,
                                                                                       invoiceHeader.ProvisionalBillingYear,
                                                                                       passengerConfiguration.SampleFormCIsIdecMigratedDate,
                                                                                       passengerConfiguration.SampleFormCIsxmlMigratedDate,
                                                                                       passengerConfiguration.SampleFormCIsIdecMigrationStatus,
                                                                                       passengerConfiguration.SampleFormCIsxmlMigrationStatus);
                    if (isMemberMigratedForFormC)
                    {
                        throw new ISBusinessException(SamplingErrorCodes.MemberMigratedForFormCButDataNotFound);
                    }
                }
            }

            invoiceHeader.IsFormABViaIS = isProvisionalBillingMemberMigrated;
            invoiceHeader.IsFormCViaIS = isLinkingSuccessfulForFormC;

            return isProvisionalBillingMemberMigrated;
        }

        private static bool IsProvisionalBillingMemberMigrated(int provisionalBillingMonth, int provisionalBillingYear, DateTime? isIdecMigrationDate, DateTime? isXmlMigrationDate, MigrationStatus idecMigrationStatus, MigrationStatus xmlMigrationStatus)
        {
            var provisionalBillingPeriod = new BillingPeriod(provisionalBillingYear, provisionalBillingMonth, 1);
            // If User migrated for both IS-Xml and IS-IDEC.
            if ((isIdecMigrationDate.HasValue && idecMigrationStatus == MigrationStatus.Certified)
              && (isXmlMigrationDate.HasValue && xmlMigrationStatus == MigrationStatus.Certified))
            {
                var isIdecMigrationPeriod = new BillingPeriod(isIdecMigrationDate.Value.Year, isIdecMigrationDate.Value.Month, 1);
                var isXmlMigrationPeriod = new BillingPeriod(isXmlMigrationDate.Value.Year, isXmlMigrationDate.Value.Month, 1);

                return (provisionalBillingPeriod >= isIdecMigrationPeriod) || (provisionalBillingPeriod >= isXmlMigrationPeriod);
            }

            // If User migrated for IS-IDEC.
            if (isIdecMigrationDate.HasValue && idecMigrationStatus == MigrationStatus.Certified)
            {
                var isIdecMigrationPeriod = new BillingPeriod(isIdecMigrationDate.Value.Year, isIdecMigrationDate.Value.Month, 1);
                return (provisionalBillingPeriod >= isIdecMigrationPeriod);
            }
            // If User migrated for IS-Xml
            if (isXmlMigrationDate.HasValue && xmlMigrationStatus == MigrationStatus.Certified)
            {
                var isXmlMigrationPeriod = new BillingPeriod(isXmlMigrationDate.Value.Year, isXmlMigrationDate.Value.Month, 1);
                return provisionalBillingPeriod >= isXmlMigrationPeriod;
            }

            return false;
        }

        /// <summary>
        /// Updates the invoice base object
        /// </summary>
        /// <param name="invoiceBaseResository"></param>
        /// <param name="invoiceBase"></param>
        /// <returns></returns>
        public InvoiceBase UpdateInvoiceBase(IRepository<InvoiceBase> invoiceBaseResository, InvoiceBase invoiceBase)
        {
            invoiceBaseResository.Update(invoiceBase);
            UnitOfWork.CommitDefault();
            return invoiceBase;
        }

        /// <summary>
        /// This method will update Header information of the invoice
        /// </summary>
        /// <param name="invoiceHeader">Invoice to be Updated.</param>
        /// <returns></returns>
        public virtual PaxInvoice UpdateInvoice(PaxInvoice invoiceHeader)
        {
            var invoiceHeaderInDb = InvoiceRepository.Single(id: invoiceHeader.Id);
            if (invoiceHeader.BillingCode != (int)BillingCode.SamplingFormDE)
            {
                invoiceHeader.InvoiceTotalRecord = invoiceHeaderInDb.InvoiceTotalRecord;

                // Update net total if user changes Exchange rate. (Earlier check of Listing Currency was failing in case of Bilateral SMI)
                if (CompareUtil.IsDirty(invoiceHeader.ExchangeRate, invoiceHeaderInDb.ExchangeRate))
                { //CMP#648: Convert Exchange rate into nullable field.
                  if (invoiceHeader.ExchangeRate.HasValue && invoiceHeader.ExchangeRate.Value > 0)
                    invoiceHeader.InvoiceTotalRecord.NetBillingAmount = invoiceHeader.InvoiceTotalRecord.NetTotal/
                                                                        invoiceHeader.ExchangeRate.Value;
                  else
                    invoiceHeader.InvoiceTotalRecord.NetBillingAmount = invoiceHeader.InvoiceTotalRecord.NetTotal;
                }
            }
            else
            {
                invoiceHeader.SamplingFormEDetails = invoiceHeaderInDb.SamplingFormEDetails;
            }
            invoiceHeader.BillingCategory = BillingCategoryType.Pax;
            // reassigning the LegalPdfLocation property fetch from Database to avoid
            // loss of information.
            invoiceHeader.LegalPdfLocation = invoiceHeaderInDb.LegalPdfLocation;

            //CMP602
            if(ValidateInvoiceHeader(invoiceHeader, invoiceHeaderInDb))
            {
              SetViewableByClearingHouse(invoiceHeader);
            }

          invoiceHeader.InvoiceStatus = InvoiceStatusType.Open;
            if (invoiceHeader.BillingCode == (int)BillingCode.SamplingFormDE && CompareUtil.IsDirty(invoiceHeader.BilledMemberId, invoiceHeaderInDb.BilledMemberId))
            {
                bool isMemberMigratedForFormC = false;
                // update the linking related flags.
                SetMigrationFlags(invoiceHeader, ref isMemberMigratedForFormC);
            }

            //Update billing and billed member reference data
            var billingMember = invoiceHeader.MemberLocationInformation.Where(location => location.IsBillingMember).FirstOrDefault();
            if (billingMember != null)
            {
                billingMember.InvoiceId = invoiceHeader.Id;
                UpdateMemberLocationInformation(billingMember, invoiceHeader, true, false);
            }

            var billedMember = invoiceHeader.MemberLocationInformation.Where(location => !location.IsBillingMember).FirstOrDefault();
            if (billedMember != null)
            {
                billedMember.InvoiceId = invoiceHeader.Id;
                UpdateMemberLocationInformation(billedMember, invoiceHeader, false, false);
            }

            var updatedInvoiceData = InvoiceRepository.Update(invoiceHeader);
            UnitOfWork.CommitDefault();

            if (invoiceHeader.BillingCode == (int)BillingCode.SamplingFormDE)
            {
                if (invoiceHeader.IsFormABViaIS || invoiceHeader.IsFormCViaIS)
                {
                    // Call stored procedure and remove following code
                    SamplingFormERep.UpdateFormEDetails(invoiceHeader.Id);

                }
            }

            return updatedInvoiceData;
        }

        public IList<PaxInvoice> UpdateInvoiceStatuses(List<Guid> invoiceIdList)
        {
            var invoiceList = invoiceIdList.Select(UpdateInvoiceStatus).ToList();

            return invoiceList.Where(invoice => invoice != null && invoice.InvoiceStatus == InvoiceStatusType.Presented).ToList();
        }

        /// <summary>
        /// Update invoice if the invoice is ready for billing
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="billingMember"></param>
        /// <param name="billedMember"></param>
        public void UpdateInvoiceDetails(PaxInvoice invoice, Member billingMember, Member billedMember)
        {
          // Get Final Parent Details for SMI, Currency, Clearing House abd Suspended Flag validations
          var billingFinalParent = MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BillingMemberId));
          var billedFinalParent = MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BilledMemberId));

          var clearingHouse = ReferenceManager.GetClearingHouseForInvoice(invoice, billingFinalParent, billedFinalParent);

            // Update clearing house of invoice
            invoice.ClearingHouse = clearingHouse;

            // Update DS Required By as per billing member location country and DS Required flag in member profile.
            if (invoice.MemberLocationInformation.Count == 2)
            {
                var isDsRequiredByBillingMember = false;
                var isDsRequiredByBilledMember = false;
                //if (billingMember != null && billingMember.DigitalSignApplication && (invoice.DigitalSignatureRequiredId == (int)DigitalSignatureRequired.Yes || invoice.DigitalSignatureRequiredId == (int)DigitalSignatureRequired.Default))
                // SCPID 28241 : Seperate DSReq : Yes and Default in two different condition.
                if (billingMember != null && billingMember.DigitalSignApplication && (invoice.DigitalSignatureRequiredId == (int)DigitalSignatureRequired.Yes))
                {
                    isDsRequiredByBillingMember = true;


                }
                else if (billingMember != null && billingMember.DigitalSignApplication && (invoice.DigitalSignatureRequiredId == (int)DigitalSignatureRequired.Default))
                {
                    if (IsDigitalSignatureRequiredForTheCountry(invoice, true))
                    {
                        isDsRequiredByBillingMember = true;
                    }
                }
                if (billedMember != null && billedMember.DigitalSignApplication)
                {
                    // Check DS Req for country check for billed point of view as well.
                    if (IsDigitalSignatureRequiredForTheCountry(invoice, false))
                    {
                        isDsRequiredByBilledMember = true;
                    }
                }

                // Update DsRequiredBy of invoice based on digital signature required by billing member and billed member.
                invoice.DsRequirdBy = GetDigitalSignatureRequiredBy(isDsRequiredByBillingMember, isDsRequiredByBilledMember);
                invoice.DsStatus = GetDigitalSignatureStatus(invoice.DsRequirdBy, invoice, billedMember);
            }

            // Update suspended flag according to ach/Ach configuration.
            if (ValidateSuspendedFlag(invoice, billingFinalParent, billedFinalParent))
            {
                invoice.SuspendedInvoiceFlag = true;
            }

            // Set Sponsored By
            // CMP#624 : 2.22-Change#2 - Update of ‘Sponsored By’ flag
            if (billingMember != null)
            {
                var ichConfiguration = billingMember.IchConfiguration;
                if (ichConfiguration != null && ichConfiguration.SponsoredById.HasValue)
                {
                    invoice.SponsoredById = ichConfiguration.SponsoredById;
                }
            }
             
            //The ‘Supporting Attachment Status’ flag should be set to ‘N’ i.e. ‘Not processed’.
            invoice.SupportingAttachmentStatus = SupportingAttachmentStatus.NotProcessed;



            //if (invoice.BillingCode == (int)BillingCode.NonSampling && invoice.InvoiceTypeId == (int)InvoiceType.Invoice)
            //{
            //  var invoiceManager = Ioc.Resolve<INonSamplingInvoiceManager>(typeof(INonSamplingInvoiceManager));

            //  foreach (var billingMemo in invoiceManager.GetBillingMemoList(invoice.Id.ToString()))
            //  {
            //    if (billingMemo.CorrespondenceRefNumber != 0 && (billingMemo.ReasonCode == "6A" || billingMemo.ReasonCode == "6B"))
            //    {
            //      var correspondenceManager = Ioc.Resolve<IPaxCorrespondenceManager>(typeof(IPaxCorrespondenceManager));
            //      var correspondence = correspondenceManager.GetRecentCorrespondenceDetails(billingMemo.CorrespondenceRefNumber);
            //      correspondence.CorrespondenceStatus = CorrespondenceStatus.Closed;
            //      correspondence.CorrespondenceSubStatus = CorrespondenceSubStatus.Billed;
            //      correspondenceManager.UpdateCorrespondence(correspondence);
            //    }
            //  }
            //}
            //UnitOfWork.CommitDefault();

        }

        /// <summary>
        /// Update invoice if the invoice is ready for billing on late submission 
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="billingMember"></param>
        /// <param name="billedMember"></param>
        public void UpdateInvoiceDetailsForLateSubmission(InvoiceBase invoice, Member billingMember, Member billedMember)
        {

         // Get Final Parent Details for SMI, Currency, Clearing House abd Suspended Flag validations
            //SCP:170853 IS-Web response time - ICH Ops 
            //Commented below code..get finalParent details only if member parent id >0
            //var billingFinalParent =  MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BillingMemberId)) ;
            //var billedFinalParent =  MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BilledMemberId));

            var billingFinalParent = billingMember.ParentMemberId > 0 ? MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BillingMemberId)) : billingMember;
            var billedFinalParent = billedMember.ParentMemberId > 0 ? MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BilledMemberId)) : billedMember;

            //var billingMember =  invoice.BillingMember;
            //var billedMember = invoice.BilledMember;

            if (string.IsNullOrEmpty(invoice.ClearingHouse))
            {
              var clearingHouse = ReferenceManager.GetClearingHouseForInvoice(invoice, billingFinalParent, billedFinalParent);
                // Update clearing house of invoice
                invoice.ClearingHouse = clearingHouse;
            }

            // Update DS Required By as per billing member location country and DS Required flag in member profile.
            if (invoice.MemberLocationInformation.Count == 2)
            {
                var isDsRequiredByBillingMember = false;
                var isDsRequiredByBilledMember = false;

                //if (billingMember != null && billingMember.DigitalSignApplication && (invoice.DigitalSignatureRequiredId == (int)DigitalSignatureRequired.Yes || invoice.DigitalSignatureRequiredId == (int)DigitalSignatureRequired.Default))
                //{
                //    if (IsDigitalSignatureRequiredForTheCountry(invoice, true))
                //    {
                //        isDsRequiredByBillingMember = true;
                //    }
                //}

                if (billingMember != null && billingMember.DigitalSignApplication && (invoice.DigitalSignatureRequiredId == (int)DigitalSignatureRequired.Yes))
                {
                    isDsRequiredByBillingMember = true;
                }
                else if (billingMember != null && billingMember.DigitalSignApplication && (invoice.DigitalSignatureRequiredId == (int)DigitalSignatureRequired.Default))
                {
                    if (IsDigitalSignatureRequiredForTheCountry(invoice, true))
                    {
                        isDsRequiredByBillingMember = true;
                    }
                }

                if (billedMember != null && billedMember.DigitalSignApplication)
                {
                    if (IsDigitalSignatureRequiredForTheCountry(invoice, false))
                    {
                        isDsRequiredByBilledMember = true;
                    }
                }

                // Update DsRequiredBy of invoice based on digital signature required by billing member and billed member.
                invoice.DsRequirdBy = GetDigitalSignatureRequiredBy(isDsRequiredByBillingMember, isDsRequiredByBilledMember);
                invoice.DsStatus = GetDigitalSignatureStatus(invoice.DsRequirdBy, invoice, billedMember);
            }

            // Update suspended flag according to ach/Ach configuration.
            if (ValidateSuspendedFlag(invoice, billingFinalParent, billedFinalParent))
            {
                invoice.SuspendedInvoiceFlag = true;
            }

            // Set Sponsored By 
            if (billingMember != null)
            {
                var ichConfiguration = billingMember.IchConfiguration;
                if (ichConfiguration != null && ichConfiguration.SponsoredById.HasValue)
                {
                    invoice.SponsoredById = ichConfiguration.SponsoredById;
                }
            }

            //The ‘Supporting Attachment Status’ flag should be set to ‘N’ i.e. ‘Not processed’.
            invoice.SupportingAttachmentStatus = SupportingAttachmentStatus.NotProcessed;
            invoice.SettlementFileStatus = InvoiceProcessStatus.Pending;
            invoice.SettlementFileStatusId = (int)InvoiceProcessStatus.Pending;
            InvoiceBaseRepository.Update(invoice);
            UnitOfWork.CommitDefault();
        }

        /// <summary>
        /// Updates the parsed invoice status.
        /// </summary>
        /// <param name="paxInvoice">The Pax invoice.</param>
        /// <param name="fileSubmissionDate"></param>
        /// <param name="exceptionDetailsList"></param>
        /// <param name="fileName"></param>
        public void UpdateNonSamplingParsedInvoiceStatus(PaxInvoice paxInvoice, string fileName, DateTime fileSubmissionDate, IList<IsValidationExceptionDetail> exceptionDetailsList)
        {

            //fixed for issue id.5713 
            paxInvoice.SettlementFileStatus = InvoiceProcessStatus.NotSet;

            if (exceptionDetailsList.Count(rec => rec.InvoiceNumber == paxInvoice.InvoiceNumber && Convert.ToInt32(rec.ErrorStatus) == (int)ErrorStatus.Z) > 0)
            {
                //If the invoice is submitted with other errors then it is not eligible for late submission.
                if (paxInvoice.ValidationStatus == InvoiceValidationStatus.ErrorPeriod || paxInvoice.ValidationStatus == InvoiceValidationStatus.FutureSubmission)
                {
                    paxInvoice.ValidationStatus = InvoiceValidationStatus.Failed;
                    paxInvoice.ValidationStatusId = (int)InvoiceValidationStatus.Failed;
                }
                paxInvoice.InvoiceStatus = InvoiceStatusType.ErrorNonCorrectable;
                // For non-correctable error invoice, clear all the transaction data.
                ClearNonSamplingInvoiceTransactionData(paxInvoice);
            }
            else if (exceptionDetailsList.Count(rec => rec.InvoiceNumber == paxInvoice.InvoiceNumber && Convert.ToInt32(rec.ErrorStatus) == (int)ErrorStatus.X) > 0)
            {
                //If the invoice is submitted with other errors then it is not eligible for late submission.

                paxInvoice.ValidationStatus = InvoiceValidationStatus.Failed;
                paxInvoice.ValidationStatusId = (int)InvoiceValidationStatus.Failed;
                paxInvoice.InvoiceStatus = InvoiceStatusType.ErrorNonCorrectable;
                // For non-correctable error invoice, clear all the transaction data.
                ClearNonSamplingInvoiceTransactionData(paxInvoice);
            }
            else if (exceptionDetailsList.Count(rec => rec.InvoiceNumber == paxInvoice.InvoiceNumber && Convert.ToInt32(rec.ErrorStatus) == (int)ErrorStatus.C) > 0)
            {

                paxInvoice.ValidationStatus = InvoiceValidationStatus.Failed;
                paxInvoice.ValidationStatusId = (int)InvoiceValidationStatus.Failed;
                paxInvoice.InvoiceStatus = InvoiceStatusType.ErrorCorrectable;

                var billingMember = MemberManager.GetMember(paxInvoice.BillingMemberId);
                var billedMember = MemberManager.GetMember(paxInvoice.BilledMemberId);


                UpdateInvoiceDetails(paxInvoice, billingMember, billedMember);
            }
            else
            {
                //If the invoice is submitted with no errors then it is eligible for late submission.
                if (paxInvoice.ValidationStatus == InvoiceValidationStatus.ErrorPeriod)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(paxInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Date",
                    string.Format("{0}{1}{2}", paxInvoice.BillingYear, paxInvoice.BillingMonth, paxInvoice.BillingPeriod), paxInvoice, fileName,
                    ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvoiceValidForLateSubmission, ErrorStatus.X);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    // Update status of Invoice as OnHold, invoice status will be updated from SP as ReadyForBilling.
                    paxInvoice.InvoiceStatusId = (int)InvoiceStatusType.ErrorNonCorrectable;
                }
                else
                {
                    if (paxInvoice.ValidationStatus != InvoiceValidationStatus.FutureSubmission)
                    {
                        paxInvoice.ValidationStatus = InvoiceValidationStatus.Completed;
                        paxInvoice.ValidationStatusId = (int)InvoiceValidationStatus.Completed;

                        var billingMember = MemberManager.GetMember(paxInvoice.BillingMemberId);
                        var billedMember = MemberManager.GetMember(paxInvoice.BilledMemberId);


                        UpdateInvoiceDetails(paxInvoice, billingMember, billedMember);
                    }
                    else if (paxInvoice.ValidationStatus == InvoiceValidationStatus.FutureSubmission)
                    {
                      // Get Final Parent Details Clearing House
                      var billingFinalParent = MemberManager.GetMember(MemberManager.GetFinalParentDetails(paxInvoice.BillingMemberId));
                      var billedFinalParent = MemberManager.GetMember(MemberManager.GetFinalParentDetails(paxInvoice.BilledMemberId));

                      var clearingHouse = ReferenceManager.GetClearingHouseForInvoice(paxInvoice, billingFinalParent, billedFinalParent);

                      // Update clearing house of invoice
                      paxInvoice.ClearingHouse = clearingHouse;
                    }
                    // Update status of Invoice as OnHold, invoice status will be updated from SP as ReadyForBilling.
                    paxInvoice.InvoiceStatusId = (int)InvoiceStatusType.OnHold;

                   
                }
            }
        }

        /// <summary>
        /// Updates the parsed invoice status.
        /// </summary>
        /// <param name="paxInvoice">The Pax invoice.</param>
        /// <param name="fileSubmissionDate"></param>
        /// <param name="exceptionDetailsList"></param>
        /// <param name="fileName"></param>
        public void UpdateSamplingParsedInvoiceStatus(PaxInvoice paxInvoice, string fileName, DateTime fileSubmissionDate, IList<IsValidationExceptionDetail> exceptionDetailsList)
        {
            //fixed for issue id.5713 
            paxInvoice.SettlementFileStatus = InvoiceProcessStatus.NotSet;

            if (exceptionDetailsList.Count(rec => rec.InvoiceNumber == paxInvoice.InvoiceNumber && Convert.ToInt32(rec.ErrorStatus) == (int)ErrorStatus.Z) > 0)
            {
                paxInvoice.ValidationStatus = InvoiceValidationStatus.Failed;
                paxInvoice.ValidationStatusId = (int)InvoiceValidationStatus.Failed;
                paxInvoice.InvoiceStatus = InvoiceStatusType.ErrorNonCorrectable;
                // For non-correctable error invoice, clear all the transaction data.
                ClearSamplingInvoiceTransactionData(paxInvoice);
            }
            if (exceptionDetailsList.Count(rec => rec.InvoiceNumber == paxInvoice.InvoiceNumber && Convert.ToInt32(rec.ErrorStatus) == (int)ErrorStatus.X) > 0)
            {
                paxInvoice.ValidationStatus = InvoiceValidationStatus.Failed;
                paxInvoice.ValidationStatusId = (int)InvoiceValidationStatus.Failed;
                paxInvoice.InvoiceStatus = InvoiceStatusType.ErrorNonCorrectable;
                // For non-correctable error invoice, clear all the transaction data.
                ClearSamplingInvoiceTransactionData(paxInvoice);
            }
            else if (exceptionDetailsList.Count(rec => rec.InvoiceNumber == paxInvoice.InvoiceNumber && Convert.ToInt32(rec.ErrorStatus) == (int)ErrorStatus.C) > 0)
            {

                paxInvoice.ValidationStatus = InvoiceValidationStatus.Failed;
                paxInvoice.ValidationStatusId = (int)InvoiceValidationStatus.Failed;
                paxInvoice.InvoiceStatus = InvoiceStatusType.ErrorCorrectable;

                var billingMember = MemberManager.GetMemberDetails(paxInvoice.BillingMemberId);
                var billedMember = MemberManager.GetMemberDetails(paxInvoice.BilledMemberId);

                UpdateInvoiceDetails(paxInvoice, billingMember, billedMember);
            }
            else
            {

                //If the invoice is submitted with no errors then it is eligible for late submission.
                if (paxInvoice.ValidationStatus == InvoiceValidationStatus.ErrorPeriod)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(paxInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Date",
                    string.Format("{0}{1}{2}", paxInvoice.BillingYear, paxInvoice.BillingMonth, paxInvoice.BillingPeriod), paxInvoice, fileName,
                    ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvoiceValidForLateSubmission, ErrorStatus.X);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    // Update status of Invoice as OnHold, invoice status will be updated from SP as ReadyForBilling.
                    paxInvoice.InvoiceStatusId = (int)InvoiceStatusType.ErrorNonCorrectable;
                }
                else
                {
                    paxInvoice.ValidationStatus = InvoiceValidationStatus.Completed;
                    paxInvoice.ValidationStatusId = (int)InvoiceValidationStatus.Completed;
                    // Update status of Invoice as OnHold, invoice status will be updated from SP as ReadyForBilling.
                    paxInvoice.InvoiceStatusId = (int)InvoiceStatusType.OnHold;

                    var billingMember = MemberManager.GetMemberDetails(paxInvoice.BillingMemberId);
                    var billedMember = MemberManager.GetMemberDetails(paxInvoice.BilledMemberId);

                    UpdateInvoiceDetails(paxInvoice, billingMember, billedMember);
                }
            }
        }

        /// <summary>
        /// Clears all non sampling transaction data from invoice object.
        /// </summary>
        /// <param name="paxInvoice"></param>
        private static void ClearNonSamplingInvoiceTransactionData(PaxInvoice paxInvoice)
        {
            int sandboxMode = 0;
            try
            {
                sandboxMode = Convert.ToInt32(ConfigurationManager.AppSettings["SandboxMode"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Error("Handled Error: ", ex);
            }

            if (sandboxMode == 0)
            {
                paxInvoice.CouponDataRecord.Clear();
                paxInvoice.BillingMemoRecord.Clear();
                paxInvoice.CreditMemoRecord.Clear();
                paxInvoice.RejectionMemoRecord.Clear();
                paxInvoice.SourceCodeTotal.Clear();
                //paxInvoice.InvoiceTotalRecord = null;
                paxInvoice.InvoiceTotalVat.Clear();
                paxInvoice.MemberLocationInformation.Clear();
                paxInvoice.ValidationExceptionSummary.Clear();
            }

        }

        /// <summary>
        /// Clears all sampling transaction data from invoice object.
        /// </summary>
        /// <param name="paxInvoice"></param>
        private static void ClearSamplingInvoiceTransactionData(PaxInvoice paxInvoice)
        {
            int sandboxMode = 0;
            try
            {
                sandboxMode = Convert.ToInt32(ConfigurationManager.AppSettings["SandboxMode"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Error("Handled Error: ", ex);
            }

            if (sandboxMode == 0)
            {
                paxInvoice.SamplingFormDRecord.Clear();
                //paxInvoice.SamplingFormEDetails = null;
                paxInvoice.SamplingFormEDetailVat.Clear();
                paxInvoice.SourceCodeTotal.Clear();
                paxInvoice.ProvisionalInvoiceRecordDetails.Clear();
                //paxInvoice.InvoiceTotalRecord = null;
                paxInvoice.InvoiceTotalVat.Clear();
                paxInvoice.MemberLocationInformation.Clear();
                paxInvoice.ValidationExceptionSummary.Clear();
            }
        }

        /// <summary>
        /// Update invoice
        /// </summary>
        public PaxInvoice UpdateInvoiceStatus(Guid invoiceId)
        {
            var invoice = InvoiceRepository.Single(id: invoiceId);

            if (invoice.InvoiceStatus == InvoiceStatusType.Presented)
            {
                return null;
            }

            invoice.InvoiceStatus = InvoiceStatusType.Presented;

            // Update invoice to database.
            var updatedInvoice = InvoiceRepository.Update(invoice);

            UnitOfWork.CommitDefault();

            return updatedInvoice;
        }

        public IQueryable<PaxBillingHistorySearchResult> GetBillingHistorySearchResult(InvoiceSearchCriteria searchCriteria, CorrespondenceSearchCriteria correspondenceSearchCriteria, int? pageNo = null, int? pageSize = null, string sortColumn = null, string sortOrder = null, int rowCountRequired = 1)
        {
          //SCP85039: IS Web Performance Feedback / Billing History & Correspondence / Other issues
          //rowCountRequired parameter added to retrieve the toatl count on conditional basis
          var invoice = InvoiceRepository.GetBillingHistorySearchResult(searchCriteria, correspondenceSearchCriteria, pageSize, pageNo, sortColumn, sortOrder, rowCountRequired);
          return invoice.AsQueryable();
        }

        public IQueryable<PaxBillingHistorySearchResult> GetBillingHistoryCorrSearchResult(CorrespondenceSearchCriteria correspondenceSearchCriteria, int? pageNo = null, int? pageSize = null, string sortColumn = null, string sortOrder = null, int rowCountRequired = 1)
        {
          var invoice = InvoiceRepository.GetBillingHistoryCorrSearchResult(correspondenceSearchCriteria, pageSize, pageNo, sortColumn, sortOrder, rowCountRequired);
            return invoice.AsQueryable();
        }

        public IQueryable<PaxInvoice> GetInvoicesForSamplingBillingHistory(int billingCode, int billedMemberId, int billingMemberId, int provisionalBillingMonth, int provisionalBillingYear, int settlementMethodId)
        {
            //Commented For SCP:ID : 19083
            //var invoiceList = InvoiceRepository.Get(invoice => invoice.BillingCode == billingCode && invoice.BilledMemberId == billedMemberId && invoice.BillingMemberId == billingMemberId && (invoice.InvoiceStatusId == (int)InvoiceStatusType.Open) && invoice.ProvisionalBillingMonth == provisionalBillingMonth && invoice.ProvisionalBillingYear == provisionalBillingYear && invoice.SettlementMethodId == settlementMethodId);

            /* CMP #624: ICH Rewrite-New SMI X
            * Description: SMI X transaction should be avoided for sampling.
            * Refer FRS Section 2.14 PAX/CGO IS-WEB Screens (Part 3) Change #9 and Change #11 */

            var invoiceList =
                InvoiceRepository.Get(
                    invoice =>
                    invoice.BillingCode == billingCode && invoice.BilledMemberId == billedMemberId &&
                    invoice.BillingMemberId == billingMemberId &&
                    (invoice.InvoiceStatusId == (int) InvoiceStatusType.Open) &&
                    invoice.ProvisionalBillingMonth == provisionalBillingMonth &&
                    invoice.ProvisionalBillingYear == provisionalBillingYear &&
                    invoice.SettlementMethodId != (int) SMI.IchSpecialAgreement);
            return invoiceList;
        }

        public IQueryable<PaxInvoice> GetInvoicesForBillingHistory(int billingCode, int billedMemberId, int billingMemberId, int settlementMethodId)
        {
            var currentPeriod = CalendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);
            //Commented For SCP:ID : 19083
            //var invoiceList = InvoiceRepository.Get(invoice => invoice.BillingCode == billingCode && invoice.BilledMemberId == billedMemberId && invoice.BillingMemberId == billingMemberId && (invoice.InvoiceStatusId == (int)InvoiceStatusType.Open || invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForSubmission) && invoice.BillingYear == currentPeriod.Year && invoice.BillingMonth == currentPeriod.Month && invoice.BillingPeriod == currentPeriod.Period && invoice.InvoiceTypeId != (int)InvoiceType.CreditNote && invoice.SubmissionMethodId != (int)SubmissionMethod.AutoBilling && invoice.SettlementMethodId == settlementMethodId);

            /* CMP #624: ICH Rewrite-New SMI X
            * Description: SMI X transaction should be transacted by (rejected, BM) only by SMI X invoice and vice a versa.
            * Refer FRS Section 2.14 PAX/CGO IS-WEB Screens (Part 3) Change #9 and Change #11 */

            var invoiceList = ((int) SMI.IchSpecialAgreement == settlementMethodId)
                                  ? InvoiceRepository.Get(
                                      invoice =>
                                      invoice.BillingCode == billingCode && invoice.BilledMemberId == billedMemberId &&
                                      invoice.BillingMemberId == billingMemberId &&
                                      (invoice.InvoiceStatusId == (int) InvoiceStatusType.Open ||
                                       invoice.InvoiceStatusId == (int) InvoiceStatusType.ReadyForSubmission) &&
                                      invoice.BillingYear == currentPeriod.Year &&
                                      invoice.BillingMonth == currentPeriod.Month &&
                                      invoice.BillingPeriod == currentPeriod.Period &&
                                      invoice.InvoiceTypeId != (int) InvoiceType.CreditNote &&
                                      invoice.SubmissionMethodId != (int) SubmissionMethod.AutoBilling &&
                                      invoice.SettlementMethodId == (int)SMI.IchSpecialAgreement)
                                  : InvoiceRepository.Get(
                                      invoice =>
                                      invoice.BillingCode == billingCode && invoice.BilledMemberId == billedMemberId &&
                                      invoice.BillingMemberId == billingMemberId &&
                                      (invoice.InvoiceStatusId == (int) InvoiceStatusType.Open ||
                                       invoice.InvoiceStatusId == (int) InvoiceStatusType.ReadyForSubmission) &&
                                      invoice.BillingYear == currentPeriod.Year &&
                                      invoice.BillingMonth == currentPeriod.Month &&
                                      invoice.BillingPeriod == currentPeriod.Period &&
                                      invoice.InvoiceTypeId != (int) InvoiceType.CreditNote &&
                                      invoice.SubmissionMethodId != (int)SubmissionMethod.AutoBilling &&
                                      invoice.SettlementMethodId != (int)SMI.IchSpecialAgreement);
            return invoiceList;
        }

        public IQueryable<CorrespondenceTrailSearchResult> GetCorrespondenceTrailSearchResult(CorrespondenceTrailSearchCriteria correspondenceTrailSearchCriteria)
        {
            var correspondences = InvoiceRepository.GetCorrespondenceTrailSearchResult(correspondenceTrailSearchCriteria);
            return correspondences.AsQueryable();
        }

        public IQueryable<AutoBillingPerformanceReportSearchResult> GetAutoBillingPerformanceReportSearchResult(int logInMemberid,int entityId, int currencyId, int clearanceMonth, int clearanceYear)
        {
            var performanceReportSearchResult = InvoiceRepository.GetAutoBillingPerformanceReportData(logInMemberid,entityId, currencyId, clearanceMonth, clearanceYear);
            return performanceReportSearchResult.AsQueryable();
        }
        /// <summary>
        /// Function to retrieve invoice details of the given invoice number
        /// </summary>
        /// <param name="invoiceId">invoice id To Be fetched..</param>
        /// <returns></returns>
        public PaxInvoice GetInvoiceHeaderDetails(string invoiceId)
        {
            try
            {
                var invoiceHeader = InvoiceRepository.Single(id: new Guid(invoiceId));

                invoiceHeader.InvoiceStatusDisplayText = invoiceHeader.DisplayInvoiceStatus = ReferenceManager.GetInvoiceStatusDisplayValue(invoiceHeader.InvoiceStatusId);
                invoiceHeader.SettlementMethodDisplayText = ReferenceManager.GetSettlementMethodDisplayValue(invoiceHeader.SettlementMethodId);
                invoiceHeader.SubmissionMethodDisplayText = ReferenceManager.GetDisplayValue(MiscGroups.FileSubmissionMethod, invoiceHeader.SubmissionMethodId);

                return invoiceHeader;
            }
            catch (Exception ex)
            {
                Logger.Error("Get Invoice Header Details", ex);
                throw ex;
            }
        }
        /// <summary>
        /// Function to retrieve invoice details of the given invoice number
        /// //SCPID 85039 - IS Web Performance Feedback / Billing History & Correspondence / Other issues 
        /// </summary>
        /// <param name="invoiceId">invoice id To Be fetched..</param>
        /// <returns></returns>
        public PaxInvoice GetInvoiceDetailForFileUpload(string invoiceId)
        {
            try
            {
                Guid id = Guid.Parse(invoiceId);
                var invoice = InvoiceRepository.Get(i => i.Id == id).SingleOrDefault();
                return invoice;
            }
            catch (Exception ex)
            {
                Logger.Error("Get Invoice Header Details", ex);
                throw ex;
            }
        }

        /// <summary>
        /// ID : 325377 - File Loading & Web Response Stats ViewInvoice CargoManageInvoice
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public PaxInvoice GetInvoiceDetails(string invoiceId)
        {
            try
            {
                Guid id = Guid.Parse(invoiceId);
                var invoice = InvoiceRepository.Get(i => i.Id == id).SingleOrDefault();
                return invoice;
            }
            catch (Exception ex)
            {
                Logger.Error("GetInvoiceDetails", ex);
                throw ex;
            }
        }


        /// <summary>
        /// Function to perform delete on selected invoice
        /// </summary>
        /// <param name="invoiceId">invoice id To Be Deleted.</param>
        /// <returns>Returns true on success, false otherwise.</returns>
        public bool DeleteInvoice(string invoiceId)
        {
            var invoiceToBeDeleted = InvoiceRepository.Single(id: new Guid(invoiceId));

            if (invoiceToBeDeleted == null) return false;
            InvoiceRepository.Delete(invoiceToBeDeleted);
            UnitOfWork.CommitDefault();

            return true;
        }

        /// <summary>
        /// Validates an invoice, when Validate Invoice button pressed
        /// </summary>
        /// <param name="invoiceId">Invoice to be validated</param>
        /// <param name="isFutureSubmitted">if set to <c>true</c> [is future submitted].</param>
        /// <returns>
        /// True if successfully validated, false otherwise
        /// </returns>
        public virtual PaxInvoice ValidateInvoice(string invoiceId, out bool isFutureSubmitted, int sessionUserId, string logRefId)
        {
            // SCP#401400: SRM: Passenger - Validate Invoices is slow
            var log = ReferenceManager.GetDebugLog(DateTime.Now,
                                                "InvoiceManager.ValidateInvoice",
                                                this.ToString(),
                                                BillingCategorys.Passenger.ToString(),
                                                "Step 4 of 12: " + invoiceId + " InvoiceManager.ValidateInvoice Start",
                                                sessionUserId,
                                                logRefId);
            ReferenceManager.LogDebugData(log);

          isFutureSubmitted = false;

            var webValidationErrors = new List<WebValidationError>();
            var invoiceGuid = invoiceId.ToGuid();
            var invoice = InvoiceRepository.Single(id: new Guid(invoiceId));

            // SCP#401400: SRM: Passenger - Validate Invoices is slow
            log = ReferenceManager.GetDebugLog(DateTime.Now,
                                                "InvoiceManager.ValidateInvoice",
                                                this.ToString(),
                                                BillingCategorys.Passenger.ToString(),
                                                "Step 5 of 12: Id: " + invoiceId + " After InvoiceRepository.Single(id)",
                                                sessionUserId,
                                                logRefId);
            ReferenceManager.LogDebugData(log);

            invoice.ValidationErrors.Clear();

            // Member Location information for both billing and billed member should present.
            var isbillingMemberRefData = invoice.MemberLocationInformation.Any(loc => loc.IsBillingMember && !string.IsNullOrEmpty(loc.CountryCode));
            var isbilledMemberRefData = invoice.MemberLocationInformation.Any(loc => !loc.IsBillingMember && !string.IsNullOrEmpty(loc.CountryCode));

            if ((string.IsNullOrEmpty(invoice.BillingMemberLocationCode) && !isbillingMemberRefData) || (string.IsNullOrEmpty(invoice.BilledMemberLocationCode) && !isbilledMemberRefData))
            {
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceGuid, ErrorCodes.InvalidMemberLocationInformation));
            }

            // TODO: Validation for invoice level VAT
            // Late submissions (where the period is the current open period less 1) will be marked as validation error (Error Non-Correctable); 
            // even if the late submission window for the past period is open in the IS Calendar.
            if (webValidationErrors.Count <= 0 && ReferenceManager.IsValidSmiForLateSubmission(invoice.SettlementMethodId) && IsLateSubmission(invoice))
            {
                invoice.ValidationStatus = InvoiceValidationStatus.ErrorPeriod;
                invoice.ValidationStatusId = (int)InvoiceValidationStatus.ErrorPeriod;
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceGuid, ErrorCodes.InvoiceLateSubmitted));
            }
            else
            {
                // Validate correctness of billing period.
              invoice.IsFutureSubmission = false;
              if (!ValidateBillingPeriodForPax(invoice, out isFutureSubmitted))
                {
                    invoice.ValidationStatus = InvoiceValidationStatus.Failed;
                    invoice.ValidationStatusId = (int)InvoiceValidationStatus.Failed;
                    webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceGuid, ErrorCodes.InvalidBillingPeriod));
                }
            }

            // Invoice total should not be a zero.
            //Commented by Tushar N. Should be moved to Child Manager classes, or put OR condition for SamplingFormE.
            //if (invoice.InvoiceTotalRecord.NetBillingAmount == 0)
            //{
            // throw new ISBusinessException(ErrorCodes.InvalidNetBillingAmount);
            //}

            // Validation for membership status on validate button click
            //var billingMember = MemberManager.GetMember(invoice.BillingMemberId);
            var billingMember = invoice.BillingMember;

            if (!ValidateBillingMembershipStatus(billingMember))
            {
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceGuid, ErrorCodes.InvalidBillingIsMembershipStatus));
            }

            if (!ValidateBilledMemberStatus(invoice.BilledMember))
            {
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceGuid, ErrorCodes.InvalidBilledIsMembershipStatus));
            }

            /* // Blocked by Debtor
            if (CheckBlockedMember(true, invoice.BillingMemberId, invoice.BilledMemberId, true))
            {
            webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceGuid, ErrorCodes.InvalidBillingToMember));
            }

            // Blocked by Creditor
            if (CheckBlockedMember(false, invoice.BilledMemberId, invoice.BillingMemberId, true))
            {
            webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceGuid, ErrorCodes.InvalidBillingFromMember));
            } */

            try
            {
                // Validation for Blocked Airline
                ValidationForBlockedAirline(invoice);
            }
            catch (ISBusinessException exception)
            {
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceGuid, exception.ErrorCode));
            }

            // For Form DE..
            if (invoice.SamplingFormDRecord.Count > 0)
            {
              var samplingFormEVatRepository = Ioc.Resolve<ISamplingFormEVatRepository>(typeof(ISamplingFormEVatRepository));
              var invoiceTotalVatList = samplingFormEVatRepository.Get(record => record.ParentId == invoiceGuid);
              //var invoiceTotalVatList = GetInvoiceLevelVatList(invoiceId);
              double samplingConstant = 0;
              if (invoice.SamplingFormEDetails != null) samplingConstant = invoice.SamplingFormEDetails.SamplingConstant;
              decimal? invoiceTotalVat = Convert.ToDecimal(invoiceTotalVatList.Sum(vat => vat.VatCalculatedAmount));
              decimal samplingFormDTotalVat = 0;
              foreach (var FormDRecord in invoice.SamplingFormDRecord)
              {
                foreach (var vat in FormDRecord.VatBreakdown)
                {
                  samplingFormDTotalVat +=
                    Convert.ToDecimal(Math.Round(Convert.ToDouble(Math.Round(vat.VatBaseAmount * samplingConstant, 2, MidpointRounding.AwayFromZero) * vat.VatPercentage / 100), 2, MidpointRounding.AwayFromZero));
                }
              }
              if (Convert.ToDecimal(invoiceTotalVat) != Convert.ToDecimal(samplingFormDTotalVat))
              {
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(), ErrorCodes.InvalidInvoiceTotalVat));
              }
            }
            // For other invoice types..
            else if (invoice.InvoiceTotalRecord != null)
            {
              var invoiceTotalVatList = GetInvoiceLevelVatList(invoiceId);
              double? invoiceTotalVat = invoiceTotalVatList.Sum(vat => vat.VatCalculatedAmount);

              if (Convert.ToDecimal(invoiceTotalVat) != invoice.InvoiceTotalRecord.TotalVatAmount)
              {
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(),
                ErrorCodes.InvalidInvoiceTotalVat));
              }

              PerformExchangeRateValidation(invoice, webValidationErrors, true);

              // SCP#401400: SRM: Passenger - Validate Invoices is slow
              log = ReferenceManager.GetDebugLog(DateTime.Now,
                                        "InvoiceManager.ValidateInvoice",
                                        this.ToString(),
                                        BillingCategorys.Passenger.ToString(),
                                        "Step 6 of 12: Id: " + invoice.Id + " After PerformExchangeRateValidation()",
                                        sessionUserId,
                                        logRefId);
              ReferenceManager.LogDebugData(log);

              /* SCP# 302117: SRM: ICH Settlement Error - SIS Production - Error Code 21015 
              * Desc: Update Total Amount in Currency of Billing. */
              //CMP#648: Convert Exchange rate into nullable field.
              if (invoice.ExchangeRate.HasValue && invoice.ExchangeRate.Value > 0)
              {
                  invoice.InvoiceTotalRecord.NetBillingAmount =
                      ConvertUtil.Round((invoice.InvoiceTotalRecord.NetTotal / invoice.ExchangeRate.Value), 2);
              }
            }
            //SCP350004: Validating Form D/E
            else if (invoice.SamplingFormEDetails != null)
            {
              PerformExchangeRateValidation(invoice, webValidationErrors, true);

              // SCP#401400: SRM: Passenger - Validate Invoices is slow
              log = ReferenceManager.GetDebugLog(DateTime.Now,
                                        "InvoiceManager.ValidateInvoice",
                                        this.ToString(),
                                        BillingCategorys.Passenger.ToString(),
                                        "Step 6.1 of 6.1 of 12: Id: " + invoice.Id + " After PerformExchangeRateValidation()",
                                        sessionUserId,
                                        logRefId);
              ReferenceManager.LogDebugData(log);

              if (Convert.ToDecimal(invoice.ExchangeRate) != 0)
              {
                invoice.SamplingFormEDetails.NetBilledCreditedAmount = 
                  ConvertUtil.Round(((invoice.SamplingFormEDetails.TotalAmountFormB - invoice.SamplingFormEDetails.NetAmountDue) / invoice.ExchangeRate.Value), 2);
              }
            }

            if (webValidationErrors.Count > 0)
            {
                invoice.ValidationErrors.AddRange(webValidationErrors);

                // SCP#401400: SRM: Passenger - Validate Invoices is slow
                log = ReferenceManager.GetDebugLog(DateTime.Now,
                                        "InvoiceManager.ValidateInvoice",
                                        this.ToString(),
                                        BillingCategorys.Passenger.ToString(),
                                        "Step 7 of 12: Id: " + invoiceId +" After invoice.ValidationErrors.AddRange(webValidationErrors)",
                                        sessionUserId,
                                        logRefId);
                ReferenceManager.LogDebugData(log);
            }
            else
            {
                //SCP85837 :- PAX CGO Sequence Number
               invoice.IsRecordSequenceArranged = PaxInvoiceRepository.UpdateTransSeqNoWithInBatch(invoiceGuid) ? RecordSequence.IsArranged : RecordSequence.NotArranged;

               // SCP#401400: SRM: Passenger - Validate Invoices is slow
               log = ReferenceManager.GetDebugLog(DateTime.Now,
                                      "InvoiceManager.ValidateInvoice",
                                      this.ToString(),
                                      BillingCategorys.Passenger.ToString(),
                                      "Step 7 of 12: Id: " + invoiceId + " After PaxInvoiceRepository.UpdateTransSeqNoWithInBatch(invoiceGuid)",
                                      sessionUserId,
                                      logRefId);
               ReferenceManager.LogDebugData(log);
            }

            return invoice;

        }
        
        // Separate method to check for future submission also.
        private bool ValidateBillingPeriodForPax(PaxInvoice invoice, out bool isFutureSubmitted)
        {
          isFutureSubmitted = false;
          var clearingHouse = ReferenceManager.GetClearingHouseToFetchCurrentBillingPeriod(invoice.SettlementMethodId);

          //SCP321997: Sun Splash Aviation SH-361
          var currentBillingPeriod = CalendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(clearingHouse);
          
          if (invoice.InvoiceBillingPeriod > currentBillingPeriod)
          {
            // Check if future period is allowed for Pax.
            if (invoice.BillingCategory == BillingCategoryType.Pax)
            {
              var passengerConfiguration = MemberManager.GetPassengerConfiguration(invoice.BillingMemberId);

              // SCP364025: Unable to Submit Passenger invoice for future period through ISWEB
              if (passengerConfiguration != null && passengerConfiguration.IsFutureBillingSubmissionsAllowed)
              {
                invoice.IsFutureSubmission = true;
                isFutureSubmitted = true;

                return true;
              }

              return false;
            }

            return false;
          }

          //SCP321997: Sun Splash Aviation SH-361
          currentBillingPeriod = CalendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(clearingHouse);

          if (invoice.InvoiceBillingPeriod != currentBillingPeriod)
          {
            return false;
          }

          return true;   
        }

        /// <summary>
        /// Marks the invoices in the invoice id list as presented.
        /// Note: This is only used for testing - will/should never be used in production.
        /// </summary>
        /// <param name="invoiceIdList">List of invoice ids to be submitted</param>
        /// <returns></returns>
        public IList<PaxInvoice> ProcessingCompleteInvoices(List<string> invoiceIdList)
        {
            var invoiceList = invoiceIdList.Select(ProcessingCompleteInvoice).ToList();

            return invoiceList.Where(invoice => invoice != null && invoice.InvoiceStatus == InvoiceStatusType.ProcessingComplete).ToList();
        }

        public PaxInvoice ProcessingCompleteInvoice(string invoiceId)
        {
            var invoiceGuid = invoiceId.ToGuid();
            var invoice = InvoiceRepository.Single(id: invoiceGuid);

            if (invoice.InvoiceStatus == InvoiceStatusType.ProcessingComplete)
            {
                return null;
            }

            if (invoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
            {
                invoice.InvoiceDate = DateTime.UtcNow;
                invoice.InvoiceStatus = InvoiceStatusType.ProcessingComplete;

                // Update invoice to database.
                var updatedInvoice = InvoiceRepository.Update(invoice);
                UnitOfWork.CommitDefault();

                return updatedInvoice;
            }

            return invoice;
        }

        /// <summary>
        /// Marks the invoices in the invoice id list as presented.
        /// Note: This is only used for testing - will/should never be used in production.
        /// </summary>
        /// <param name="invoiceIdList">List of invoice ids to be submitted</param>
        /// <returns></returns>
        public IList<PaxInvoice> PresentInvoices(List<string> invoiceIdList)
        {
            var invoiceList = invoiceIdList.Select(PresentInvoice).ToList();

            return invoiceList.Where(invoice => invoice != null && invoice.InvoiceStatus == InvoiceStatusType.Presented).ToList();
        }

        /// <summary>
        /// Marks the invoice as presented.
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public PaxInvoice PresentInvoice(string invoiceId)
        {
            var invoiceGuid = invoiceId.ToGuid();
            var invoice = InvoiceRepository.Single(id: invoiceGuid);

            if (invoice.InvoiceStatus == InvoiceStatusType.Presented)
            {
                return null;
            }

            // Allow to mark presented if InvoiceStatus is ReadyForBilling or ProcessingComplete
            if (invoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling || invoice.InvoiceStatus == InvoiceStatusType.ProcessingComplete)
            {
                invoice.InvoiceDate = DateTime.UtcNow;
                invoice.InvoiceStatus = InvoiceStatusType.Presented;

                // Update invoice to database.
                var updatedInvoice = InvoiceRepository.Update(invoice);
                UnitOfWork.CommitDefault();

                return updatedInvoice;
            }

            return invoice;
        }
        
        /// <summary>
        /// Submits invoices
        /// </summary>
        /// <param name="invoiceIdList">List of invoice ids to be submitted</param>
        /// <returns></returns>
        public IList<PaxInvoice> SubmitInvoices(List<string> invoiceIdList)
        {
            var invoiceList = invoiceIdList.Select(SubmitInvoice).ToList();

            return invoiceList.Where(invoice => invoice != null && invoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling).ToList();
        }

        public PaxInvoice SubmitInvoice(string invoiceId)
        {
            var webValidationErrors = new List<WebValidationError>();
            var invoice = InvoiceRepository.Single(id: new Guid(invoiceId));

            // User Id of logged in member
            var userId = invoice.LastUpdatedBy;

            if (invoice.InvoiceStatus != InvoiceStatusType.ReadyForSubmission)
            {
                return invoice;
            }
            if (invoice.SamplingFormEDetails != null)
            {
              invoice.InvoiceType = invoice.SamplingFormEDetails.NetBilledCreditedAmount < 0
                                        ? InvoiceType.CreditNote
                                        : InvoiceType.Invoice;
            }


            // Re-fetch the billing and billed member - since we are getting a stale state (workaround)!
            var billingMember = MemberManager.GetMember(invoice.BillingMemberId);
            var billedMember = MemberManager.GetMember(invoice.BilledMemberId);

            // Get Final Parent Details for SMI, Currency, Clearing House abd Suspended Flag validations
            var billingFinalParent = MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BillingMemberId));
            var billedFinalParent = MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoice.BilledMemberId));


            // Get ValidationErrors for invoice from DB.
            var validationErrorsInDb = ValidationErrorManager.GetValidationErrors(invoiceId);

            // IS Membership validations.
            if (!ValidateBilledMemberStatus(invoice.BilledMember))
            {
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoice.Id, ErrorCodes.InvalidBilledIsMembershipStatus));
            }

            if (!ValidateBillingMembershipStatus(billingMember))
            {
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoice.Id, ErrorCodes.InvalidBillingIsMembershipStatus));
            }
                     
            //SCP219674 : InvalidAmountToBeSettled Validation *********************************************************
            var invoiceManager = Ioc.Resolve<INonSamplingInvoiceManager>(typeof(INonSamplingInvoiceManager));
            var bmList = invoiceManager.GetBillingMemoList(invoice.Id.ToString()).Where(bm => bm.ReasonCode == "6A" || bm.ReasonCode == "6B");
            if (invoice.BillingCode == (int)BillingCode.NonSampling && invoice.InvoiceTypeId == (int)InvoiceType.Invoice)
            {
              foreach (var billingMemo in bmList)
              {
                try
                {
                  ValidateCorrespondenceReference(billingMemo, false, invoice);
                }
                catch (ISBusinessException exception)
                {
                  var error = string.Format(" Billing Memo Number: {0} , Batch Sequence Number: {1}", billingMemo.BillingMemoNumber, billingMemo.BatchSequenceNumber);
                  webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(), exception.ErrorCode, error));
                }
              }
            }            
            //********************************************

            // Late submissions (where the period is the current open period less 1) will be marked as validation error (Error Non-Correctable); 
            // even if the late submission window for the past period is open in the IS Calendar.
            if (webValidationErrors.Count <= 0 && IsLateSubmission(invoice))
            {
                invoice.ValidationStatus = InvoiceValidationStatus.ErrorPeriod;
                invoice.ValidationStatusId = (int)InvoiceValidationStatus.ErrorPeriod;
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoice.Id, ErrorCodes.InvoiceLateSubmitted));
            }
            else
            {
                // Validate correctness of billing period.
                if (!ValidateBillingPeriod(invoice))
                {
                    webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoice.Id, ErrorCodes.InvalidBillingPeriod));
                }
            }

            try
            {
                // Validation for Blocked Airline
                ValidationForBlockedAirline(invoice);
            }
            catch (ISBusinessException exception)
            {
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoice.Id, exception.ErrorCode));
            }

            // Update invoice status in case of error.
            if (webValidationErrors.Count > 0)
            {
                invoice.InvoiceStatus = InvoiceStatusType.ValidationError;
                invoice.ValidationErrors.AddRange(webValidationErrors);

                if (invoice.ValidationStatus != InvoiceValidationStatus.ErrorPeriod)
                    invoice.ValidationStatus = InvoiceValidationStatus.Failed;
            }
            else
            {
                // If Billed or Billing member is suspended update Invoice suspended flag to true. 
              if (ValidateSuspendedFlag(invoice, billingFinalParent, billedFinalParent))
                {
                    invoice.SuspendedInvoiceFlag = true;
                }
                // Every validation is successful. Update invoice status as Ready for billing and invoice date as current date.
                invoice.InvoiceDate = DateTime.UtcNow;
                invoice.InvoiceStatus = InvoiceStatusType.ReadyForBilling;
            }

            // Update validation errors in db.
            ValidationErrorManager.UpdateValidationErrors(invoice.Id, invoice.ValidationErrors, validationErrorsInDb);

            // Update clearing house of invoice
            var clearingHouse = ReferenceManager.GetClearingHouseForInvoice(invoice, billingFinalParent, billedFinalParent);
            invoice.ClearingHouse = clearingHouse;

            // Set Sponsored By 
            var ichConfiguration = MemberManager.GetIchConfig(billingMember.Id);
            if (ichConfiguration != null && ichConfiguration.SponsoredById.HasValue)
            {
                invoice.SponsoredById = ichConfiguration.SponsoredById;
            }

            // Update Duplicate Coupon DU Mark
            PaxInvoiceRepository.UpdateDuplicateCouponByInvoiceId(invoice.Id, invoice.BillingMemberId);


            // Adds member location information.
            UpdateMemberLocationInformation(invoice);

            // Update invoice to database.
            var updatedInvoice = InvoiceRepository.Update(invoice);

            // Update DS Required By as per billing member location country and DS Required flag in member profile.
            SetDigitalSignatureInfo(invoice, billingMember, billedMember);

            // Call UpdateSourceCodeTotalVat() method which will update Invoice SourceCode VAT total
            InvoiceRepository.UpdateSourceCodeTotalVat(invoice.Id);

            if (invoice.BillingCode == (int)BillingCode.NonSampling && invoice.InvoiceTypeId == (int)InvoiceType.Invoice && invoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
            {
              foreach (var billingMemo in bmList)
              {
                if (billingMemo.CorrespondenceRefNumber != 0 && (billingMemo.ReasonCode == "6A" || billingMemo.ReasonCode == "6B"))
                {
                  // Get all correspondence having given correspondence no.
                  var correspondence = PaxCorrespondenceRepository.GetCorr(corr => corr.CorrespondenceNumber == billingMemo.CorrespondenceRefNumber);

                  // SCP61363: Correspondence 0980000091 Closed due to Expiry
                  // Update status of entire correspondence trail to "Closed - Billed" if corresp status and sub status is other than "Open - Saved/ReadyForSubmit".
                  // Also delete correspondence if corresp status and sub status is equal to "Open - Saved/ReadyForSubmit". 
                  foreach (var corr in correspondence)
                  {
                    if (corr.CorrespondenceStatus == CorrespondenceStatus.Open && (corr.CorrespondenceSubStatus == CorrespondenceSubStatus.Saved || corr.CorrespondenceSubStatus == CorrespondenceSubStatus.ReadyForSubmit))
                    {
                      Logger.InfoFormat("Deleting Correspondence in 'Open - Saved/ReadyForSubmit' state. Correspondence No: {0}, Stage: {1}, Status: {2}, Sub-Status: {3}", corr.CorrespondenceNumber, corr.CorrespondenceStage, corr.CorrespondenceStatusId, corr.CorrespondenceSubStatusId);
                      PaxCorrespondenceRepository.Delete(corr);
                    }
                    else
                    {
                      corr.CorrespondenceSubStatus = CorrespondenceSubStatus.Billed;
                      corr.CorrespondenceStatus = CorrespondenceStatus.Closed;
                    }
                  }// End foreach
                  ////SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
                  //// For BM created over correspondence.
                  //// Update expiry period using only lead period for billing memo and all transactions prior to it for purging.
                  //DateTime expiryPeriod = ReferenceManager.GetExpiryDatePeriodForClosedCorrespondence(invoice, BillingCategoryType.Pax, Constants.SamplingIndicatorNo, null);
                  //InvoiceRepository.UpdateExpiryDatePeriod(billingMemo.Id, (int)TransactionType.PasNsBillingMemoDueToAuthorityToBill, expiryPeriod);
                }// End if
                //else
                //{
                //    // For plain Billing memo.
                //    // Get expiry period for purging.
                //    DateTime expiryPeriod = ReferenceManager.GetExpiryDatePeriodMethod(TransactionType.RejectionMemo1, invoice, BillingCategoryType.Pax, Constants.SamplingIndicatorNo, null);

                //    // Update it in database.
                //    InvoiceRepository.UpdateExpiryDatePeriod(billingMemo.Id, (int)TransactionType.BillingMemo, expiryPeriod);
                //}// End else
              }// End foreach
            }

            UnitOfWork.CommitDefault();

            if (updatedInvoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
            {
                Logger.InfoFormat("Update invoice '{0}' info after Invoice status changed as Ready for billing.", ConvertUtil.ConvertGuidToString(updatedInvoice.Id));

                try
                {
                    InvoiceRepository.UpdateInvoiceOnReadyForBilling(updatedInvoice.Id, updatedInvoice.BillingCategoryId, updatedInvoice.BillingMemberId, updatedInvoice.BilledMemberId, updatedInvoice.BillingCode);
                    Logger.InfoFormat("Updated invoice '{0}' details successfully after Invoice status changed to Ready for billing by User '{1}'.", ConvertUtil.ConvertGuidToString(updatedInvoice.Id), userId);
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat("Exception occurred while updating Invoice '{0}' in PROC_UPDINV_ON_READYFORBILLING by User '{1}' Exception: '{2}'", ConvertUtil.ConvertGuidToString(updatedInvoice.Id), userId, ex);
                }

                //try
                //{
                //    InvoiceRepository.UpdateInvoiceAndSetLaParameters(new Guid(invoiceId));
                //    Logger.InfoFormat("Updated invoice '{0}' details for LA and successfully set LA Parameters by User '{1}'.", ConvertUtil.ConvertGuidToString(updatedInvoice.Id), userId);
                //}
                //catch (Exception ex)
                //{
                //    Logger.ErrorFormat("Exception occurred while updating Invoice '{0}' in PROC_SET_INV_ARCPARAMETERS by User '{1}' Exception: '{2}'", ConvertUtil.ConvertGuidToString(updatedInvoice.Id), userId, ex);
                //}
                //SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
                // Update expiry period for purging.
                //UpdateExpiryDatePeriod(updatedInvoice);
            }

            return updatedInvoice;
        }
        //SCP0000: PURGING AND SET EXPIRY DATE : Delete below code (Remove real time set expiry)
       

        /// <summary>
        /// Get Member Location Information.
        /// </summary>
        /// <param name="invoiceId">Invoice whose member locations need to be retrieved.</param>
        /// <param name="isBillingMember">Is it a billing Member.</param>
        /// <param name="locationCode">The location code.</param>
        /// <returns>MemberLocationInformation class object</returns>
        public MemberLocationInformation GetMemberReferenceData(string invoiceId, bool isBillingMember, string locationCode)
        {
            var invoiceGuid = invoiceId.ToGuid();
            MemberLocationInformation memberLocationInformation = null;

            var invoice = InvoiceRepository.Single(id: new Guid(invoiceId));

            if (string.IsNullOrEmpty(locationCode))
            {
                locationCode = isBillingMember ? invoice.BillingMemberLocationCode : invoice.BilledMemberLocationCode;
            }

            if (string.IsNullOrEmpty(locationCode))
            {
                memberLocationInformation = MemberLocationInfoRepository.Single(memLocation => memLocation.IsBillingMember == isBillingMember && memLocation.InvoiceId == invoiceGuid);
            }
            else
            {
                var memberId = isBillingMember ? invoice.BillingMemberId : invoice.BilledMemberId;
                var memberLocation = LocationRepository.Single(location => location.LocationCode == locationCode && location.MemberId == memberId);
                if (memberLocation != null)
                {
                    if (string.IsNullOrEmpty(memberLocation.LegalText))
                    {
                        var eBillingConfig = MemberManager.GetEbillingConfig(memberLocation.MemberId);
                        memberLocation.LegalText = eBillingConfig != null ? eBillingConfig.LegalText : memberLocation.LegalText;
                    }

                    memberLocationInformation = new MemberLocationInformation
                    {
                        CompanyRegistrationId = memberLocation.RegistrationId,
                        AddressLine1 = memberLocation.AddressLine1,
                        AddressLine2 = memberLocation.AddressLine2,
                        AddressLine3 = memberLocation.AddressLine3,
                        CityName = memberLocation.CityName,
                        CountryCode = memberLocation.CountryId,
                        CountryName = memberLocation.Country.Name,
                        DigitalSignatureRequiredId = 1,
                        SubdivisionName = memberLocation.SubDivisionName,
                        SubdivisionCode = memberLocation.SubDivisionCode,
                        LegalText = memberLocation.LegalText,
                        OrganizationName = memberLocation.MemberLegalName,
                        PostalCode = memberLocation.PostalCode,
                        TaxRegistrationId = memberLocation.TaxVatRegistrationNumber,
                        MemberLocationCode = memberLocation.LocationCode,
                        IsBillingMember = isBillingMember,
                        InvoiceId = invoiceGuid,
                        AdditionalTaxVatRegistrationNumber = memberLocation.AdditionalTaxVatRegistrationNumber
                    };
                }
            }

            return memberLocationInformation;
        }

        /// <summary>
        /// To get Invoice level VAT list
        /// </summary>
        /// <param name="invoiceId">invoice id for which vat list to be retrieved..</param>
        /// <returns>List of the invoice level Vat</returns>
        public IList<InvoiceVat> GetInvoiceLevelVatList(string invoiceId)
        {
            var invoiceGuid = invoiceId.ToGuid();
            var invoiceTotalVatlist = InvoiceTotalVatRepository.Get(invtotal => invtotal.ParentId == invoiceGuid).ToList();
            return invoiceTotalVatlist;
        }

        // DerivedVATDetails class is used temporarily.It will be replaced by InvoiceTotalVat class,
        // once InvoiceTotalVat class is used in Repository.
        public IList<DerivedVatDetails> GetInvoiceLevelDerivedVatList(string invoiceId)
        {
            var invoiceGuid = invoiceId.ToGuid();
            IList<DerivedVatDetails> derivedVatRecords = InvoiceRepository.GetDerivedVatDetails(invoiceGuid);
            return derivedVatRecords;
        }

        /// <summary>
        /// Following method is used to Get SourceCode Vat total details to be displayed on Popup
        /// </summary>
        /// <param name="sourceCodeVatTotalId">SourceCode Vat breakdown Id</param>
        /// <returns>SourceCode Vat breakdown record</returns>
        public List<SourceCodeVat> GetSourceCodeVatTotal(string sourceCodeVatTotalId)
        {
            // Convert SourceCodeVatTotal Id to Guid
            var sourceCodeTotalId = sourceCodeVatTotalId.ToGuid();
            // Query repository to get record
            var sourceCodeVatTotalRecord = SourceCodeVatTotalRepository.Get(sct => sct.ParentId == sourceCodeTotalId).ToList();
            // return Record
            return sourceCodeVatTotalRecord;
        }

        /// <summary>
        /// Retrieves list of VAT types which are not applied in the invoice
        /// </summary>
        /// <param name="invoiceId">invoice id for which vat list to be retrieved.</param>
        /// <returns></returns>
        public IList<NonAppliedVatDetails> GetNonAppliedVatList(string invoiceId)
        {
            var nonAppliedVatList = InvoiceRepository.GetNonAppliedVatDetails(invoiceId.ToGuid());
            return nonAppliedVatList;
        }

        /// <summary>
        /// Add invoice level Vat
        /// </summary>
        /// <param name="vatList">The invoice total vat.</param>
        /// <returns></returns>
        public InvoiceVat AddInvoiceLevelVat(InvoiceVat vatList)
        {
            InvoiceVatRepository.Add(vatList);
            //Update Invoice Status to open
            var invoice = InvoiceRepository.Single(id: vatList.ParentId);
            invoice.InvoiceStatusId = (int)InvoiceStatusType.Open;
            InvoiceRepository.Update(invoice);

            UnitOfWork.CommitDefault();
            return vatList;
        }

        /// <summary>
        /// Delete invoice level Vat
        /// </summary>
        /// <param name="vatId"></param>
        /// <returns></returns>
        public bool DeleteInvoiceLevelVat(string vatId)
        {
            Guid vatGuid = vatId.ToGuid();
            InvoiceVat vatRecord = InvoiceVatRepository.Single(vat => vat.Id == vatGuid);
            if (vatRecord == null) return false;
            InvoiceVatRepository.Delete(vatRecord);

            var invoice = InvoiceRepository.Single(id: vatRecord.ParentId);
            invoice.InvoiceStatusId = (int)InvoiceStatusType.Open;
            InvoiceRepository.Update(invoice);

            UnitOfWork.CommitDefault();
            return true;
        }

        /// <summary>
        /// Function to Retrieve Invoice Total
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns>invoiceTotal</returns>
        public InvoiceTotal GetInvoiceTotal(string invoiceId)
        {
            var invoiceGuid = invoiceId.ToGuid();
            var invoiceTotalRepository = new Repository<InvoiceTotal>();
            var invoiceTotal = invoiceTotalRepository.Single(invTotal => invTotal.Id == invoiceGuid);
            return invoiceTotal;
        }

        /// <summary>
        /// Retrieve SourceCode Total
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public IList<SourceCodeTotal> GetSourceCodeList(string invoiceId)
        {
            var invoiceGuid = invoiceId.ToGuid();
            var sourceCodeList = SourceCodeTotalRepository.Get(sc => sc.InvoiceId == invoiceGuid).ToList();
            return sourceCodeList;
        }

        /// <summary>
        /// Set BilledMember property of invoice when there is business exception
        /// </summary>
        /// <param name="billedMemberId">Billed Member Id</param>
        /// <returns></returns>
        public Member GetBilledMember(int billedMemberId)
        {
            return MemberManager.GetMember(billedMemberId);
        }

        /// <summary>
        /// Determines whether billed member migrated for specified billing month and period in invoice header.
        /// </summary>
        /// <param name="invoiceHeader">The invoice header.</param>
        /// <returns>
        /// 	True if member migrated for the specified invoice header; otherwise, false.
        /// </returns>
        public virtual bool IsMemberMigrated(PaxInvoice invoiceHeader)
        {
            return false;
        }

        /// <summary>
        /// Gets the invoice with RM coupons.
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="billingMonth"></param>
        /// <param name="billingYear"></param>
        /// <param name="billingPeriod"></param>
        /// <param name="billingMemberId"></param>
        /// <param name="billedMemberId"></param>
        /// <param name="billingCode"></param>
        /// <param name="couponSearchCriteriaString"></param>
        /// <returns></returns>
        public PaxInvoice GetInvoiceWithBMCoupons(string invoiceNumber, int billingMonth, int billingYear, int billingPeriod, int billingMemberId, int billedMemberId, int? billingCode = null, string couponSearchCriteriaString = null)
        {
            var entities = new[] { LoadStrategy.Entities.BillingMemo, LoadStrategy.Entities.BillingMemoCoupon };

            var invoices = InvoiceRepository.GetInvoiceLS(new LoadStrategy(string.Join(",", entities)), invoiceNumber, billingMonth, billingYear, billingPeriod, billingMemberId, billedMemberId, billingCode, invoiceStatusIds: ((int)InvoiceStatusType.Presented).ToString(), couponSearchCriteriaString: couponSearchCriteriaString);

            PaxInvoice invoice = null;
            if (invoices.Count > 0)
            {
                // TODO: throw exception if invoice count > 1
                invoice = invoices[0];
            }
            return invoice;
        }

        /// <summary>
        /// Gets the invoice with RM coupons.
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="billingMonth"></param>
        /// <param name="billingYear"></param>
        /// <param name="billingPeriod"></param>
        /// <param name="billingMemberId"></param>
        /// <param name="billedMemberId"></param>
        /// <param name="billingCode"></param>
        /// <param name="couponSearchCriteriaString"></param>
        /// <returns></returns>
        public PaxInvoice GetInvoiceWithCMCoupons(string invoiceNumber, int billingMonth, int billingYear, int billingPeriod, int billingMemberId, int billedMemberId, int? billingCode = null, string couponSearchCriteriaString = null)
        {
            var entities = new[] { LoadStrategy.Entities.CreditMemo, LoadStrategy.Entities.CreditMemoCoupon };

            var invoices = InvoiceRepository.GetInvoiceLS(new LoadStrategy(string.Join(",", entities)), invoiceNumber, billingMonth, billingYear, billingPeriod, billingMemberId, billedMemberId, billingCode, invoiceStatusIds: ((int)InvoiceStatusType.Presented).ToString(), couponSearchCriteriaString: couponSearchCriteriaString);

            PaxInvoice invoice = null;
            if (invoices.Count > 0)
            {
                // TODO: throw exception if invoice count > 1
                invoice = invoices[0];
            }
            return invoice;
        }


        /// <summary>
        /// Gets the invoice with RM coupons.
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="billingMonth"></param>
        /// <param name="billingYear"></param>
        /// <param name="billingPeriod"></param>
        /// <param name="billingMemberId"></param>
        /// <param name="billedMemberId"></param>
        /// <param name="billingCode"></param>
        /// <param name="couponSearchCriteriaString"></param>
        /// <returns></returns>
        public PaxInvoice GetInvoiceWithRmBmCmCoupons(string invoiceNumber, int billingMonth, int billingYear, int billingPeriod, int billingMemberId, int billedMemberId, int? billingCode = null, string couponSearchCriteriaString = null)
        {
            var entities = new[] { LoadStrategy.Entities.RejectionMemo, LoadStrategy.Entities.RejectionMemoCoupon, LoadStrategy.Entities.CreditMemo, LoadStrategy.Entities.CreditMemoCoupon, LoadStrategy.Entities.BillingMemo, LoadStrategy.Entities.BillingMemoCoupon };

            var invoices = InvoiceRepository.GetInvoiceLS(new LoadStrategy(string.Join(",", entities)), invoiceNumber, billingMonth, billingYear, billingPeriod, billingMemberId, billedMemberId, billingCode, invoiceStatusIds: ((int)InvoiceStatusType.Presented).ToString(), couponSearchCriteriaString: couponSearchCriteriaString);

            PaxInvoice invoice = null;
            if (invoices.Count > 0)
            {
                // TODO: throw exception if invoice count > 1
                invoice = invoices[0];
            }
            return invoice;
        }

        /// <summary>
        /// Gets the invoice with RM coupons.
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="billingMonth"></param>
        /// <param name="billingYear"></param>
        /// <param name="billingPeriod"></param>
        /// <param name="billingMemberId"></param>
        /// <param name="billedMemberId"></param>
        /// <param name="billingCode"></param>
        /// <param name="couponSearchCriteriaString"></param>
        /// <param name="rejectionMemoNumber">Added the new parameter for SCP51931: File stuck in Production. If value provided then data would be fetched for the provided RM only.</param> 
        /// <returns></returns>
        public PaxInvoice GetInvoiceWithRMCoupons(string invoiceNumber, int billingMonth, int billingYear, int billingPeriod, int billingMemberId, int billedMemberId, int? billingCode = null, string couponSearchCriteriaString = null, string rejectionMemoNumber = null)
        {
            var entities = new[] { LoadStrategy.Entities.RejectionMemo, LoadStrategy.Entities.RejectionMemoCoupon };

            var invoices = InvoiceRepository.GetInvoiceLS(new LoadStrategy(string.Join(",", entities)), invoiceNumber, billingMonth, billingYear, billingPeriod, billingMemberId, billedMemberId, billingCode, invoiceStatusIds: ((int)InvoiceStatusType.Presented).ToString(), couponSearchCriteriaString: couponSearchCriteriaString, rejectionMemoNumber: rejectionMemoNumber);

            PaxInvoice invoice = null;
            if (invoices.Count > 0)
            {
                // TODO: throw exception if invoice count > 1
                invoice = invoices[0];
            }
            return invoice;
        }

        /// <summary>
        /// Gets the invoice with prime for validation of RM coupons.
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="billingMonth"></param>
        /// <param name="billingYear"></param>
        /// <param name="billingPeriod"></param>
        /// <param name="billingMemberId"></param>
        /// <param name="billedMemberId"></param>
        /// <param name="billingCode"></param>
        /// <param name="couponSearchCriteriaString"></param>
        /// <param name="fetchCoupon"></param>
        /// <returns></returns>
        public PaxInvoice GetInvoiceWithCoupons(string invoiceNumber, int billingMonth, int billingYear, int billingPeriod, int billingMemberId, int billedMemberId, int? billingCode = null, string couponSearchCriteriaString = null, bool fetchCoupon = true)
        {
            var entities = fetchCoupon ? new[] { LoadStrategy.Entities.Coupon } : new string[]{};

            var invoices = InvoiceRepository.GetInvoiceLS(new LoadStrategy(string.Join(",", entities)), invoiceNumber, billingMonth, billingYear, billingPeriod, billingMemberId, billedMemberId, billingCode, invoiceStatusIds: ((int)InvoiceStatusType.Presented).ToString(), couponSearchCriteriaString: couponSearchCriteriaString);

            PaxInvoice invoice = null;
            if (invoices.Count > 0)
            {
                // TODO: throw exception if invoice count > 1
                invoice = invoices[0];
            }
            return invoice;
        }

        /// <summary>
        /// Gets the invoice with coupons.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns></returns>
        public PaxInvoice GetInvoiceWithCoupons(string invoiceId)
        {
            var invoices = InvoiceRepository.GetInvoiceLS(new LoadStrategy(LoadStrategy.Entities.Coupon), invoiceId: invoiceId);

            PaxInvoice invoice = null;
            if (invoices.Count > 0)
            {
                invoice = invoices[0];
                // DiplayText Retrieval from miscCodes and setting it to required property - To be moved to sp
                invoice.InvoiceStatusDisplayText = invoice.DisplayInvoiceStatus = ReferenceManager.GetInvoiceStatusDisplayValue(invoice.InvoiceStatusId);
                invoice.SettlementMethodDisplayText = ReferenceManager.GetSettlementMethodDisplayValue(invoice.SettlementMethodId);
                invoice.SubmissionMethodDisplayText = ReferenceManager.GetDisplayValue(MiscGroups.FileSubmissionMethod, invoice.SubmissionMethodId);
            }
            return invoice;
        }

        /// <summary>
        /// Gets the invoice with prime and RM coupons.
        /// </summary>
        /// <param name="invoiceNumber"></param>
        /// <param name="billingMonth"></param>
        /// <param name="billingYear"></param>
        /// <param name="billingPeriod"></param>
        /// <param name="billingMemberId"></param>
        /// <param name="billedMemberId"></param>
        /// <param name="billingCode"></param>
        /// <param name="couponSearchCriteriaString"></param>
        /// <returns></returns>
        public PaxInvoice GetInvoiceWithPrimeAndRMCoupons(string invoiceNumber, int billingMonth, int billingYear, int billingPeriod, int billingMemberId, int billedMemberId, int? billingCode = null, string couponSearchCriteriaString = null)
        {
            var entities = new[] { LoadStrategy.Entities.Coupon, LoadStrategy.Entities.RejectionMemo, LoadStrategy.Entities.RejectionMemoCoupon };

            var invoices = InvoiceRepository.GetInvoiceLS(new LoadStrategy(string.Join(",", entities)), invoiceNumber, billingMonth, billingYear, billingPeriod, billingMemberId, billedMemberId, billingCode, invoiceStatusIds: ((int)InvoiceStatusType.Presented).ToString(), couponSearchCriteriaString: couponSearchCriteriaString);

            PaxInvoice invoice = null;
            if (invoices.Count > 0)
            {
                // TODO: throw exception if invoice count > 1
                invoice = invoices[0];
            }
            return invoice;
        }

        /// <summary>
        /// Use case for listing invoices that meet a certain criteria, along with their child coupon records.
        /// The knowledge of the child entities to be loaded, is known to this use case.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public List<PaxInvoice> GetInvoicesWithCoupons(SearchCriteria criteria)
        {
            //The use case invokes the correct repository to fetch the entities. 
            //The child entities to be loaded are specified in the call.
            var arrEntities = new[] { LoadStrategy.Entities.Coupon,LoadStrategy.Entities.CouponTax,
          LoadStrategy.Entities.CouponVat,LoadStrategy.Entities.MemberLocation,LoadStrategy.Entities.SourceCodeTotal,
          LoadStrategy.Entities.SourceCode,LoadStrategy.Entities.SourceCodeVat,LoadStrategy.Entities.CouponDataVatIdentifier,
          LoadStrategy.Entities.SourceCodeVatIdentifier,LoadStrategy.Entities.InvoiceTotal,
          LoadStrategy.Entities.InvoiceTotalVat,LoadStrategy.Entities.InvoiceTotalVatIdentifier
        };
            var entities = string.Join(",", arrEntities);
            var invoices = InvoiceRepository.GetInvoicesLS(criteria, new LoadStrategy(entities));

            // DiplayText Retrieval from miscCodes and setting it to required property - To be moved to sp
            foreach (var paxInvoice in invoices)
            {
                paxInvoice.InvoiceStatusDisplayText = paxInvoice.DisplayInvoiceStatus = ReferenceManager.GetInvoiceStatusDisplayValue(paxInvoice.InvoiceStatusId);
                paxInvoice.SettlementMethodDisplayText = ReferenceManager.GetSettlementMethodDisplayValue(paxInvoice.SettlementMethodId);
                paxInvoice.SubmissionMethodDisplayText = ReferenceManager.GetDisplayValue(MiscGroups.FileSubmissionMethod, paxInvoice.SubmissionMethodId);
            }
            return invoices;
        }

        private static MemberLocationInformation GetMemberLocationInformation(Location memberLocation, bool isBillingMember, Guid invoiceId)
        {
            return new MemberLocationInformation
            {
                CompanyRegistrationId = memberLocation.RegistrationId,
                AddressLine1 = memberLocation.AddressLine1,
                AddressLine2 = memberLocation.AddressLine2,
                AddressLine3 = memberLocation.AddressLine3,
                CityName = memberLocation.CityName,
                DigitalSignatureRequiredId = 0,
                SubdivisionName = memberLocation.SubDivisionName,
                SubdivisionCode = memberLocation.SubDivisionCode,
                LegalText = memberLocation.LegalText,
                OrganizationName = string.IsNullOrEmpty(memberLocation.MemberLegalName) ? " " : memberLocation.MemberLegalName,
                PostalCode = memberLocation.PostalCode,
                TaxRegistrationId = memberLocation.TaxVatRegistrationNumber,
                MemberLocationCode = memberLocation.LocationCode,
                CountryCode = memberLocation.Country != null ? memberLocation.Country.Id : string.Empty,
                AdditionalTaxVatRegistrationNumber = memberLocation.AdditionalTaxVatRegistrationNumber,
                IsBillingMember = isBillingMember,
                InvoiceId = invoiceId,
                CountryName = memberLocation.Country != null ? memberLocation.Country.Name : string.Empty
            };
        }

        /// <summary>
        /// Used to update member location information for submitted invoices. 
        /// While creating/updating invoices, member location information table stores entries only for blank location codes.
        /// </summary>
        /// <param name="updatedInvoice"></param>
        public void UpdateMemberLocationInformationForLateSub(InvoiceBase updatedInvoice)
        {
            if (!string.IsNullOrEmpty(updatedInvoice.BillingMemberLocationCode))
            {
                var billingMemberLocation = LocationRepository.Single(ml => ml.MemberId == updatedInvoice.BillingMemberId && ml.LocationCode.ToUpper() == updatedInvoice.BillingMemberLocationCode.ToUpper());
                var memberLocationInformation = GetMemberLocationInformation(billingMemberLocation, true, updatedInvoice.Id);

                // If member location information exist for given invoice, fetch it and delete before adding new member location information.
                var memlocationInfo = MemberLocationInfoRepository.Single(memLoc => memLoc.InvoiceId == memberLocationInformation.InvoiceId && memLoc.IsBillingMember);
                if (memlocationInfo != null)
                {
                    MemberLocationInfoRepository.Delete(memlocationInfo);
                }

                MemberLocationInfoRepository.Add(memberLocationInformation);
            }

            if (!string.IsNullOrEmpty(updatedInvoice.BilledMemberLocationCode))
            {
                var billedMemberLocation = LocationRepository.Single(ml => ml.MemberId == updatedInvoice.BilledMemberId && ml.LocationCode.ToUpper() == updatedInvoice.BilledMemberLocationCode.ToUpper());
                var billedMemberLocationInformation = GetMemberLocationInformation(billedMemberLocation, false, updatedInvoice.Id);
                var memlocationInfo = MemberLocationInfoRepository.Single(memLoc => memLoc.InvoiceId == billedMemberLocationInformation.InvoiceId && memLoc.IsBillingMember == false);
                if (memlocationInfo != null)
                {
                    MemberLocationInfoRepository.Delete(memlocationInfo);
                }

                MemberLocationInfoRepository.Add(billedMemberLocationInformation);
            }
        }

        public MemberLocationInformation UpdateMemberLocationInformation(MemberLocationInformation memberLocationInformation, InvoiceBase invoiceHeader, bool isBillingMember, bool? commitChanges = null)
        {
            // var invoiceHeader = invoiceHeaderRepository.Single(id: memberLocationInformation.invoiceHeaderId);



            if (!string.IsNullOrEmpty(memberLocationInformation.MemberLocationCode))
            {
                if (isBillingMember)
                {
                    invoiceHeader.BillingMemberLocationCode = memberLocationInformation.MemberLocationCode;
                }
                else
                {
                    invoiceHeader.BilledMemberLocationCode = memberLocationInformation.MemberLocationCode;
                }

                //InvoiceRepository.Update(invoiceHeader);







                //var memberLocInformation = MemberLocationInfoRepository.Single(memLoc => memLoc.InvoiceId == invoiceHeader.Id && memLoc.IsBillingMember == isBillingMember);





                //if (memberLocInformation != null)
                //{
                //  invoiceHeader.MemberLocationInformation.Remove(memberLocInformation);
                //  MemberLocationInfoRepository.Delete(memberLocInformation);
                //}
            }
            else
            {

                if (isBillingMember)
                {
                    invoiceHeader.BillingMemberLocationCode = null;
                }
                else
                {
                    invoiceHeader.BilledMemberLocationCode = null;
                }
            }

            // InvoiceRepository.Update(invoiceHeader);

            var memberLocationInformationInDb = MemberLocationInfoRepository.Single(memLoc => memLoc.InvoiceId == invoiceHeader.Id && memLoc.IsBillingMember == isBillingMember);

            // Update current member location information if already exits
            if (memberLocationInformationInDb != null)
            {
                memberLocationInformation.Id = memberLocationInformationInDb.Id;

                MemberLocationInfoRepository.Update(memberLocationInformation);
            }
            else
            {
                // Add new member location information 
                MemberLocationInfoRepository.Add(memberLocationInformation);
            }


            if ((!commitChanges.HasValue) || commitChanges.Value)
                UnitOfWork.CommitDefault();

            return memberLocationInformation;

        }

        /// <summary>
        /// Validates the invoice header while creating/updating the Invoice (header).
        /// </summary>
        /// <param name="invoiceHeader">Invoice to be validated.</param>
        /// <param name="invoiceHeaderInDb">Invoice in Db.</param>
        /// <returns>Returns true on success, false otherwise.</returns>
        protected virtual bool ValidateInvoiceHeader(PaxInvoice invoiceHeader, PaxInvoice invoiceHeaderInDb)
        {
            var isUpdateOperation = false;

            //Check whether it's a update operation.
            if (invoiceHeaderInDb != null)
            {
                isUpdateOperation = true;
            }

            // Check whether the invoice can be updated.
            if (isUpdateOperation)
            {
                if (invoiceHeader.InvoiceStatus == InvoiceStatusType.Claimed
                    || invoiceHeader.InvoiceStatus == InvoiceStatusType.Presented
                    || invoiceHeader.InvoiceStatus == InvoiceStatusType.ProcessingComplete
                    || invoiceHeader.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
                {
                    throw new ISBusinessException(ErrorCodes.InvalidInvoiceStatusForUpdate);
                }
            }

            // Make sure billing and billed member are not the same.
            if (invoiceHeader.BilledMemberId == invoiceHeader.BillingMemberId)
            {
                throw new ISBusinessException(ErrorCodes.SameBillingAndBilledMember);
            }

            // Get the details of the billing and billed member.
            var billingMember = MemberManager.GetMember(invoiceHeader.BillingMemberId);
            var billedMember = MemberManager.GetMember(invoiceHeader.BilledMemberId);

            //CMP#624: 2.22 -Change#3 - Update of ‘Parent Member’ information
            // Get Final Parent Details for SMI, Currency, Clearing House abd Suspended Flag validations
            var billingFinalParent = MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoiceHeader.BillingMemberId));
            var billedFinalParent = MemberManager.GetMember(MemberManager.GetFinalParentDetails(invoiceHeader.BilledMemberId));
            // Assign final parent to invoice
            if (billingFinalParent != null && billingFinalParent.Id != billingMember.Id)
            {
                invoiceHeader.BillingParentMemberId = billingFinalParent.Id;
            }
            if (billedFinalParent != null && billedFinalParent.Id != billedMember.Id)
            {
                invoiceHeader.BilledParentMemberId = billedFinalParent.Id;
            }
            
            // Make sure that the billing member is valid.
            if (billingMember == null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidBillingMember);
            }

            // Make sure that the billed member is valid.
            if (billedMember == null)
            {
                throw new ISBusinessException(ErrorCodes.InvalidBilledMember);
            }

            /* CMP #596: Length of Member Accounting Code to be Increased to 12 
               Desc: New validation #MW2 and #MW3. The Member should not be a Type B Member.
               Disallow Type B members to bill or be billed in Pax and Cgo billing categories.
               Ref: FRS Section 3.4 Table 16 Row 1, 2, 4, 5, and 6. Also FRS Section 3.4 Point 22, 27.
       
               As per CMP# 596 FRS document, the term ‘Type B Members’ means  - 
               new SIS Members having an Accounting Code with one of the following attributes:
               a.The length of the code is 3, but alpha characters appear in the second and/or third position (the first position may be alpha or numeric)
               b.The length of the code is 4, but alpha character(s) appear in any position (i.e. it is not purely 4 numeric)
               c.The length of the code ranges from 5 to 12
            */
            if (IsTypeBMember(billingMember.MemberCodeNumeric))
            {
                throw new ISBusinessException(ErrorCodes.InvalidBillingMemberType);
            }

            if (IsTypeBMember(billedMember.MemberCodeNumeric))
            {
                throw new ISBusinessException(ErrorCodes.InvalidMemberType);
            }

            if (billingMember != null)
                billingMember.IchConfiguration = MemberManager.GetIchConfig(billingMember.Id);

            if (billedMember != null)
                billedMember.IchConfiguration = GetIchConfiguration(billedMember.Id);

            // MemberProfile- A member cannot create an invoice/creditNote or Form C when his IS Membership is ‘Basic’, ‘Restricted’ or ‘Terminated’.
            if (!ValidateBillingMembershipStatus(billingMember))
            {
                throw new ISBusinessException(ErrorCodes.InvalidBillingIsMembershipStatus);
            }

            // Validate correctness of billing period.
            if (!ValidateBillingPeriodOnSaveHeader(invoiceHeader))
            {
                throw new ISBusinessException(ErrorCodes.InvalidBillingPeriod);
            }

            // SCP177435 - EXCHANGE RATE 
            //Validate Listing Currency
            //if (!isUpdateOperation || (CompareUtil.IsDirty(invoiceHeaderInDb.BillingCurrencyId, invoiceHeader.BillingCurrencyId)) || (CompareUtil.IsDirty(invoiceHeaderInDb.ListingCurrencyId, invoiceHeader.ListingCurrencyId)))
            //{

              //CMP #553: ACH Requirement for Multiple Currency Handling-FRS-v1.1.doc
              if (!ValidateBillingCurrency(invoiceHeader, billingFinalParent, billedFinalParent))
              {
                throw new ISBusinessException(ErrorCodes.InvalidBillingCurrency);
              }

              if (!ValidateListingCurrency(invoiceHeader, billingFinalParent, billedFinalParent))
              {
                throw new ISBusinessException(ErrorCodes.InvalidListingCurrency);
              }

            //}

          // Update suspended flag according to ach/Ach configuration.
            if (ValidateSuspendedFlag(invoiceHeader, billingFinalParent, billedFinalParent))
            {
                invoiceHeader.SuspendedInvoiceFlag = true;
            }

            // Settlement method validation.
            /*CMP#624: ICH Rewrite-New SMI X , Here validate settlement Method for non X*/
            if (invoiceHeader.SettlementMethodId != (int)SMI.IchSpecialAgreement)
            {
              if (!isUpdateOperation ||
                 (CompareUtil.IsDirty(invoiceHeaderInDb.SettlementMethodId, invoiceHeader.SettlementMethodId) ||
                 CompareUtil.IsDirty(invoiceHeaderInDb.BilledMemberId, invoiceHeader.BilledMemberId)))
              {
                if (!ValidateSettlementMethod(invoiceHeader, billingFinalParent, billedFinalParent))
                {
                  throw new ISBusinessException(ErrorCodes.InvalidSettlementMethod);
                }
              }
            }


            if (!isUpdateOperation || (CompareUtil.IsDirty(invoiceHeaderInDb.ListingCurrencyId, invoiceHeader.ListingCurrencyId)))
            {
                // Currency of listing/evaluation 
                // Billing Code (Element 6) = 3 or 5 or 6 or 7
                ValidateInvoiceListingCurrency(invoiceHeader);
            }

            if (!isUpdateOperation || (CompareUtil.IsDirty(invoiceHeaderInDb.InvoiceNumber, invoiceHeader.InvoiceNumber)))
            {
                // TODO:IS-Calendar validation: Billing month and Period: 
                // 1.It can be equal to a future period if the IS calendar’s “Submissions Open (Future dated submissions)” date of
                // that future period is equal to or less than the system date.
                // 2.Late submissions(i.e. when the period is the current open period less 1) will be marked as validation error; 
                // even if the late submission window for the past period is open in the IS Calendar.

                // Used ValidationManager method to validate invoice number.
                if (!ValidateInvoiceNumber(invoiceHeader.InvoiceNumber, invoiceHeader.BillingYear, invoiceHeader.BillingMemberId))
                {
                    throw new ISBusinessException(ErrorCodes.DuplicateInvoiceFound);
                }
            }

            ////Shambhu Thakur - Commented Digital Signature Validation
            ////Shambhu Thakur(08Sep11) - uncommented member profile digitalsignapplication check
            if (billingMember != null && !billingMember.DigitalSignApplication
             && invoiceHeader.DigitalSignatureRequired == DigitalSignatureRequired.Yes)
            {
                throw new ISBusinessException(ErrorCodes.InvalidDigitalSignatureValue);
            }

            //// i.	  ‘Digital Signature Flag’ in the Invoice Header = “D”; AND
            //// ii.	Profile element of the Billing Member ‘DS Services Required?’ = “Y”; AND
            //// iii. Both countries from the invoice reference data (Billing Member and Billed Member) 
            ////      are not listed in profile element of the Billing Member ‘As a Billing Member, DS Required for Invoices From/To’.
            //if (invoiceHeader.DigitalSignatureRequired == DigitalSignatureRequired.Yes)
            //{
            //  if (!IsDigitalSignatureRequired(invoiceHeader, billingMember, billedMember))
            //  {
            //    throw new ISBusinessException(MiscUatpErrorCodes.InvalidDSFlagAsCountryNotSpecified);
            //  }
            //}

            //Validate invoice Date 
            if (!ValidateInvoiceDate(invoiceHeader.InvoiceDate))
            {
                throw new ISBusinessException(ErrorCodes.InvalidInvoiceDate);
            }

            /* // Blocked by Debtor
            if (CheckBlockedMember(true, invoiceHeader.BillingMemberId, invoiceHeader.BilledMemberId, true))
            {
              throw new ISBusinessException(ErrorCodes.InvalidBillingToMember);
            }

            // Blocked by Creditor
            if (CheckBlockedMember(false, invoiceHeader.BilledMemberId, invoiceHeader.BillingMemberId, true))
            {
              throw new ISBusinessException(ErrorCodes.InvalidBillingFromMember);
            } */

            // Validation for Blocked Airline
            ValidationForBlockedAirline(invoiceHeader);

            /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
            if (!ReferenceManager.IsSmiLikeBilateral(invoiceHeader.SettlementMethodId, false))
            {
              if (!invoiceHeader.ListingCurrencyId.HasValue)
              {
                throw new ISBusinessException(MiscUatpErrorCodes.ListingCurrencyMustHaveValue);
              }

              if (!invoiceHeader.BillingCurrencyId.HasValue)
              {
                throw new ISBusinessException(MiscUatpErrorCodes.BillingCurrencyMustHaveValue);
              }
            }
            //if ((!isUpdateOperation || (CompareUtil.IsDirty(invoiceHeaderInDb.SettlementMethodId, invoiceHeader.SettlementMethodId) || CompareUtil.IsDirty(invoiceHeaderInDb.ListingCurrencyId, invoiceHeader.ListingCurrencyId) || CompareUtil.IsDirty(invoiceHeaderInDb.BillingCurrencyId, invoiceHeader.BillingCurrencyId))) && invoiceHeader.ListingCurrencyId.HasValue && invoiceHeader.BillingCurrency.HasValue)
            //{
            /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
                if (!ReferenceManager.IsSmiLikeBilateral(invoiceHeader.SettlementMethodId, true))
                {//CMP#648: Convert Exchange rate into nullable field.
                    if (
                      !ValidateExchangeRate(invoiceHeader.ExchangeRate.HasValue? invoiceHeader.ExchangeRate.Value : (decimal?) null, invoiceHeader.ListingCurrencyId.Value,
                                            invoiceHeader.BillingCurrency.Value, invoiceHeader.BillingYear,
                                            invoiceHeader.BillingMonth))
                    {
                        if (invoiceHeader.SettlementMethodId != (int)SettlementMethodValues.Bilateral)
                        {
                            var correctExchangeRate = ReferenceManager.GetExchangeRate(invoiceHeader.ListingCurrencyId.Value,
                                                                                 (BillingCurrency)invoiceHeader.BillingCurrencyId.Value,
                                                                                 invoiceHeader.BillingYear,
                                                                                 invoiceHeader.BillingMonth);
                            // Update Exchange rate
                            invoiceHeader.ExchangeRate = Convert.ToDecimal(correctExchangeRate);
                        }
                        else
                        {
                            throw new ISBusinessException(CargoErrorCodes.InvalidExchangeRate);
                        }
                    }
                }
                else
                {
                    /*SCP 270845 - Some validations missing is IS-WEB
                     * Description: Bilateral SMI so exchange rate is allowed to be anything but 0. 
                     * Also enforce that exchange rate has to be 1.00000 in cases when same billing and listing currencies.
                    */
                    if (invoiceHeader.BillingCurrencyId.Value == invoiceHeader.ListingCurrencyId.Value && Convert.ToDecimal(invoiceHeader.ExchangeRate) != 1)
                    {
                        throw new ISBusinessException(ErrorCodes.InvalidListingToBillingRateForSameCurrencies);
                    }
                    if (Convert.ToDecimal(invoiceHeader.ExchangeRate) <= 0)
                    {
                        throw new ISBusinessException(ErrorCodes.InvalidListingToBillingRate);
                    }
                }
           // }

            #region CMP #624: ICH Rewrite-New SMI X
              
                //* CMP #624: ICH Rewrite-New SMI X 
                //* Description: ICH Web Service is called when header is saved
                //* Refer FRS Section: 2.12	PAX/CGO IS-WEB Screens (Part 1). */
          ValidationBeforeSmiXWebServiceCall(invoiceHeader,
                                             null,
                                             invoiceHeader.InvoiceTypeId,
                                             null,
                                             null,
                                             billingFinalParent,
                                             billedFinalParent,
                                             true,
                                             null,
                                             invoiceHeader.BatchSequenceNumber,
                                             invoiceHeader.RecordSequenceWithinBatch);

          if (invoiceHeader.SettlementMethodId == (int)SMI.IchSpecialAgreement && invoiceHeader.GetSmiXPhase1ValidationStatus())
          {

            /* CMP #624: ICH Rewrite-New SMI X 
            * Description: ICH Web Service is called when header is saved
            * Refer FRS Section: 2.13 PAX/CGO IS-WEB Screens (Part 2). */
            return CallSmiXIchWebServiceAndHandleResponse(invoiceHeader, null, invoiceHeader.InvoiceTypeId, null, null, true);
          }

          #endregion

          return true;
        }
        
        private void PerformExchangeRateValidation(PaxInvoice invoice,List<WebValidationError> webValidationErrors = null,bool isValidateInvoice = false)
        {
            /* SCP#342522 - SIS: ICH Settlement Error - SIS Production
             * Perform Validation to update exchange rate, if incorrect*/

            /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
            if (!ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, true))
            {
                if (
                  !ValidateExchangeRate(invoice.ExchangeRate, invoice.ListingCurrencyId.Value,
                                        invoice.BillingCurrency.Value, invoice.BillingYear,
                                        invoice.BillingMonth))
                {
                    if (invoice.SettlementMethodId != (int)SettlementMethodValues.Bilateral)
                    {
                        var correctExchangeRate = ReferenceManager.GetExchangeRate(invoice.ListingCurrencyId.Value,
                                                                             (BillingCurrency)invoice.BillingCurrencyId.Value,
                                                                             invoice.BillingYear,
                                                                             invoice.BillingMonth);
                        // Update Exchange rate
                        invoice.ExchangeRate = Convert.ToDecimal(correctExchangeRate);
                    }
                    else
                    {
                        if (isValidateInvoice)
                        {
                            if (webValidationErrors != null)
                            {
                                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoice.Id, CargoErrorCodes.InvalidExchangeRate));
                            }
                        }
                        else
                        {
                            throw new ISBusinessException(CargoErrorCodes.InvalidExchangeRate);
                        }
                    }
                }
            }
            else
            {
                /*SCP 270845 - Some validations missing is IS-WEB
                 * Description: Bilateral SMI so exchange rate is allowed to be anything but 0. 
                 * Also enforce that exchange rate has to be 1.00000 in cases when same billing and listing currencies.
                */
                if (invoice.BillingCurrencyId.Value == invoice.ListingCurrencyId.Value && invoice.ExchangeRate != 1)
                {
                    if (isValidateInvoice)
                    {
                        if (webValidationErrors != null)
                        {
                            webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoice.Id, ErrorCodes.InvalidListingToBillingRateForSameCurrencies));
                        }
                    }
                    else
                    {
                        throw new ISBusinessException(ErrorCodes.InvalidListingToBillingRateForSameCurrencies);
                    }
                }
                if (invoice.ExchangeRate <= 0)
                {
                    if (isValidateInvoice)
                    {
                        if (webValidationErrors != null)
                        {
                            webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoice.Id, ErrorCodes.InvalidListingToBillingRate));
                        }
                    }
                    else
                    {
                        throw new ISBusinessException(ErrorCodes.InvalidListingToBillingRate);
                    }
                }
            }
        }

        /// <summary>
        /// Validate Invoice Listing Currency.
        /// </summary>
        /// <param name="invoiceHeader">Invoice whose currency fields to be validated.</param>
        public static void ValidateInvoiceListingCurrency(PaxInvoice invoiceHeader)
        {
            if (invoiceHeader.BillingCode == (int)BillingCode.SamplingFormAB || invoiceHeader.BillingCode == (int)BillingCode.SamplingFormDE
                || invoiceHeader.BillingCode == (int)BillingCode.SamplingFormF || invoiceHeader.BillingCode == (int)BillingCode.SamplingFormXF)
            {
                if ((invoiceHeader.InvoiceSmi == SMI.Ach || invoiceHeader.InvoiceSmi == SMI.AchUsingIataRules) && invoiceHeader.ListingCurrencyId != (int)BillingCurrency.USD)
                {
                    throw new ISBusinessException(ErrorCodes.InvalidListingCurrency);
                }
            }
        }

        /// <summary>
        /// Return default value for settlement indicator for given combination of billing and billed members
        /// </summary>
        /// <param name="billingMemberId">Billing Member Id</param>
        /// <param name="billedMemberId">Billed Member Id</param>
        /// <param name="billingCategoryId">The billing category id.</param>
        /// <returns></returns>
        public override SMI GetDefaultSettlementMethodForMembers(int billingMemberId, int billedMemberId, int billingCategoryId)
        {
          // Get Billing-Billed final parent data
          BillingMember = MemberManager.GetMember(MemberManager.GetFinalParentDetails(billingMemberId));
          BilledMember = MemberManager.GetMember(MemberManager.GetFinalParentDetails(billedMemberId));

          var settlementMethod = base.GetDefaultSettlementMethodForMembers(billingMemberId, billedMemberId,
                                                                           billingCategoryId);

          // Validation of the ACH Exception if both member are dual; Both member should be added to ACH Exception (Settle through ICH?). 
          if (IsDualMember(BillingMember) && IsDualMember(BilledMember))
          {
            var achExceptionsBillingMember =
              MemberManager.GetExceptionMembers(BillingMember.Id, billingCategoryId, false).ToList();
            if (achExceptionsBillingMember.Count(ach => ach.ExceptionMemberId == BilledMember.Id) > 0)
            {
              settlementMethod = SMI.Ich;
            }
          }

          return settlementMethod;
        }

        /// <summary>
        /// This function is used to check billing and billed member is ach or dual member.
        /// CMP #553: ACH Requirement for Multiple Currency Handling
        /// </summary>
        /// <param name="billingMemberId"></param>
        /// <param name="billedMemberId"></param>
        /// <returns></returns>
        public bool IsBillingAndBilledAchOrDualMember(int billingMemberId, int billedMemberId)
        {
            // Get Billing-Billed final parent data
            BillingMember = MemberManager.GetMember(MemberManager.GetFinalParentDetails(billingMemberId));
            BilledMember = MemberManager.GetMember(MemberManager.GetFinalParentDetails(billedMemberId));

             if (IsAchOrDualMember(BillingMember) && IsAchOrDualMember(BilledMember))
             {
                 return true;
             }
            return false;
        }

        /// <summary>
        /// Validates the tax.
        /// </summary>
        /// <param name="tax">The tax.</param>
        /// <param name="exceptionDetailsList">The exception details list.</param>
        /// <param name="invoice">The invoice.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="errorLevel">The error level.</param>
        /// <param name="fileSubmissionDate">The file submission date.</param>
        /// <param name="batchNo">The batch no.</param>
        /// <param name="sequenceNo">The sequence no.</param>
        /// <param name="documentNumber">The document number.</param>
        /// <param name="sourceCode">The source code.</param>
        /// <param name="linkedDocumentNumber"></param>
        /// <returns></returns>
        protected bool ValidateParsedTax(Tax tax, IList<IsValidationExceptionDetail> exceptionDetailsList, PaxInvoice invoice, string fileName,
          string errorLevel, DateTime fileSubmissionDate, int batchNo = 0, int sequenceNo = 0,
          string documentNumber = null, int sourceCode = 0, string linkedDocumentNumber = null)
        {
            var isValid = true;

            if (tax.TaxCode != null && !string.IsNullOrEmpty(tax.TaxCode.Trim()))
            {
                if (!ReferenceManager.IsValidTaxCode(invoice, tax.TaxCode))
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(tax.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Tax Code", tax.TaxCode, invoice, fileName, errorLevel, ErrorCodes.InvalidTaxCode, ErrorStatus.C, sourceCode, batchNo, sequenceNo, documentNumber, false, linkedDocumentNumber);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }
            else
            {
                // SCP ID 66980 - Files stuck in production
                // In case TAX Code is not provided or Null, Mark it as Error Non-Correctable
                var validationExceptionDetail = CreateValidationExceptionDetail(tax.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Tax Code", tax.TaxCode, invoice, fileName, errorLevel, ErrorCodes.MissingTaxCode, ErrorStatus.X, sourceCode, batchNo, sequenceNo, documentNumber, false, linkedDocumentNumber);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }
          return isValid;
        }

        /// <summary>
        /// Validates the vat db.
        /// </summary>
        /// <param name="vat">The vat.</param>
        /// <param name="exceptionDetailsList">The exception details list.</param>
        /// <param name="invoice">The invoice.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="errorLevel">The error level.</param>
        /// <param name="fileSubmissionDate"></param>
        /// <param name="batchNo">The batch no.</param>
        /// <param name="sequenceNo">The sequence no.</param>
        /// <param name="documentNumber">The document number.</param>
        /// <param name="sourceCode">The source code.</param>
        /// <param name="isIgnoreValidation"></param>
        /// <param name="isValidateVatLabelAndText"></param>
        /// <returns></returns>
        protected bool ValidateParsedVat(Vat vat, IList<IsValidationExceptionDetail> exceptionDetailsList, PaxInvoice invoice, string fileName,
          string errorLevel, DateTime fileSubmissionDate, int batchNo = 0, int sequenceNo = 0,
          string documentNumber = null, int sourceCode = 0, bool isIgnoreValidation = false, bool isValidateVatLabelAndText = false)
        {
            var isValid = true;

            if (!isIgnoreValidation)
            {
                if (!CompareUtil.Compare(vat.VatCalculatedAmount, (vat.VatBaseAmount * vat.VatPercentage) / 100, invoice.Tolerance.RoundingTolerance, Constants.PaxDecimalPlaces))
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(vat.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Calculated Amount", Convert.ToString(vat.VatCalculatedAmount), invoice, fileName, errorLevel, ErrorCodes.InvalidCalculatedVatAmount, ErrorStatus.X, sourceCode, batchNo, sequenceNo, documentNumber);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }

            if (vat.VatIdentifierId == 0)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(vat.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Identifier", vat.Identifier, invoice, fileName, errorLevel, ErrorCodes.InvalidVatIdentifier, ErrorStatus.X, sourceCode, batchNo, sequenceNo, documentNumber);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            if (isValidateVatLabelAndText)
            {
                if (string.IsNullOrWhiteSpace(vat.VatLabel))
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(vat.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Label", string.Empty, invoice, fileName, errorLevel, ErrorCodes.InvalidVatLabel, ErrorStatus.X, sourceCode, batchNo, sequenceNo, documentNumber);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
                if (string.IsNullOrWhiteSpace(vat.VatText))
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(vat.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Text", string.Empty, invoice, fileName, errorLevel, ErrorCodes.InvalidVatText, ErrorStatus.X, sourceCode, batchNo, sequenceNo, documentNumber);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
                //CMP464
                else if(vat.VatText.Length>50)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(vat.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Vat Text", vat.VatText, invoice, fileName, errorLevel, ErrorCodes.InvalidTaxVatLength, ErrorStatus.X, sourceCode, batchNo, sequenceNo, documentNumber);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }

            return isValid;
        }

        /// <summary>
        /// Validates the coupon breakdown record base.
        /// </summary>
        /// <param name="couponBreakdownRecordBase">The coupon breakdown record base.</param>
        /// <param name="errorLevel">The error level.</param>
        /// <param name="exceptionDetailsList">The exception details list.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="parentCoupon">The parent coupon.</param>
        /// <param name="sourceCode">The source code.</param>
        /// <param name="invoice">The invoice.</param>
        /// <param name="couponRecord">The coupon record.</param>
        /// <returns></returns>
        protected bool ValidateCouponBreakdownRecordBase(MemoCouponBase couponBreakdownRecordBase, string errorLevel,
          IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, MemoBase parentCoupon, int sourceCode, PaxInvoice invoice, PrimeCoupon couponRecord)
        {
            var isValid = true;

            DateTime fileSubmissionDate = DateTime.MinValue;
            var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
            var isInputFile = referenceManager.GetIsInputFile(Path.GetFileName(fileName));
            if (isInputFile != null)
            {
                fileSubmissionDate = isInputFile.FileDate;
            }

            if (couponRecord.FlightDate.HasValue && couponBreakdownRecordBase.FlightDate.HasValue)
            {
                if (DateTime.Compare(couponRecord.FlightDate.Value, couponBreakdownRecordBase.FlightDate.Value) != 0)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(couponBreakdownRecordBase.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Flight Day", Convert.ToString(couponBreakdownRecordBase.FlightDate.Value.ToShortDateString()),
                                                      invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.InvalidFlightDate, ErrorStatus.X, sourceCode, parentCoupon.BatchSequenceNumber, parentCoupon.RecordSequenceWithinBatch, couponBreakdownRecordBase.TicketDocOrFimNumber.ToString());
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }

            if (couponRecord.FlightNumber != couponBreakdownRecordBase.FlightNumber)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(couponBreakdownRecordBase.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Flight Number", Convert.ToString(couponBreakdownRecordBase.FlightNumber),
                                                   invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.InvalidFlightNumber, ErrorStatus.X, sourceCode, parentCoupon.BatchSequenceNumber, parentCoupon.RecordSequenceWithinBatch, couponBreakdownRecordBase.TicketDocOrFimNumber.ToString());
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            if (couponRecord.AirlineFlightDesignator != couponBreakdownRecordBase.AirlineFlightDesignator)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(couponBreakdownRecordBase.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Airline Flight Designator", Convert.ToString(couponBreakdownRecordBase.AirlineFlightDesignator),
                                                  invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.InvalidAirlineCode, ErrorStatus.X, sourceCode, parentCoupon.BatchSequenceNumber, parentCoupon.RecordSequenceWithinBatch, couponBreakdownRecordBase.TicketDocOrFimNumber.ToString());
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            //Validate Airline Flight Designator*
            if (!MemberManager.IsValidAirlineAlphaCode(couponBreakdownRecordBase.AirlineFlightDesignator))
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(couponBreakdownRecordBase.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Airline Flight Designator", Convert.ToString(couponBreakdownRecordBase.AirlineFlightDesignator),
                                                   invoice, fileName, ErrorLevels.ErrorLevelCreditMemoCoupon, ErrorCodes.InvalidAirlineCode, ErrorStatus.X, sourceCode, parentCoupon.BatchSequenceNumber, parentCoupon.RecordSequenceWithinBatch, couponBreakdownRecordBase.TicketDocOrFimNumber.ToString());
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            return isValid;
        }


        /// <summary>
        ///   Validates the flight date.
        /// </summary>
        /// <param name = "flightDateString">The flight date string.</param>
        /// <returns></returns>
        public bool ValidateFlightDate(string flightDateString)
        {
            bool result;
            if (!flightDateString.Substring(0, 2).Equals("00"))
            {
                result = false;
            }
            else
            {
                // Try to parse date string to DateTime to check whether its valid.
                DateTime resultDate;
                var flightDate = string.Format("{0}/{1}/{2}", 2000, flightDateString.Substring(2, 2), flightDateString.Substring(4, 2));

                result = DateTime.TryParse(flightDate, out resultDate);
            }
            return result;
        }

        /// <summary>
        /// To mark suspended billing member's invoices as suspended.
        /// </summary>
        /// <param name="memberId">Member Id.</param>
        /// <param name="defaultSuspensionDate">Default Suspension Date.</param>
        /// <param name="fromSuspensionDate">From Suspension Date.</param>
        /// <param name="clearingHouse">Clearing House.</param>
        public bool UpdateSuspendedBillingMemberInvoices(int memberId, DateTime defaultSuspensionDate, DateTime fromSuspensionDate, string clearingHouse)
        {
            string suspensionMonth;
            var billingInvoiceList = new List<InvoiceBase>();
            InvoiceBaseRepository = Ioc.Resolve<IRepository<InvoiceBase>>();

            if (defaultSuspensionDate.Month.ToString().Length < 2)
                suspensionMonth = defaultSuspensionDate.Year + "0" + defaultSuspensionDate.Month;
            else
                suspensionMonth = defaultSuspensionDate.Year + defaultSuspensionDate.Month.ToString();

            // CMP-409 if member is merged entity then Mark all Child member's invoice as suspended
            var childMemberList = MemberManager.GetChildMembers(memberId);

            foreach (var childMember in childMemberList)
            {
                /* CMP #624: ICH Rewrite-New SMI X 
                 * Description: 2.24 Retroactive Suspension of Members
                 * Change #1: Inclusion of SMI X Invoices/Credit Notes in retroactive suspensions */

              if (clearingHouse.Equals("I") || clearingHouse.Equals("IB"))
                billingInvoiceList =
                 InvoiceBaseRepository.Get(
                   invoice => invoice.BillingMemberId == childMember.MemberId && ((invoice.BillingYear > defaultSuspensionDate.Year) || (invoice.BillingYear == defaultSuspensionDate.Year && invoice.BillingMonth > defaultSuspensionDate.Month) || (invoice.BillingYear == defaultSuspensionDate.Year && invoice.BillingMonth == defaultSuspensionDate.Month && invoice.BillingPeriod >= defaultSuspensionDate.Day)) && invoice.SuspendedInvoiceFlag == false && (invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling || invoice.InvoiceStatusId == (int)InvoiceStatusType.Claimed || invoice.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete || invoice.InvoiceStatusId == (int)InvoiceStatusType.Presented) && (invoice.SettlementMethodId == (int)SMI.Ich || invoice.SettlementMethodId == (int)SMI.IchSpecialAgreement)).ToList();
              else if (clearingHouse.Equals("A") || clearingHouse.Equals("AB"))
                billingInvoiceList =
                 InvoiceBaseRepository.Get(
                   invoice => invoice.BillingMemberId == childMember.MemberId && ((invoice.BillingYear > defaultSuspensionDate.Year) || (invoice.BillingYear == defaultSuspensionDate.Year && invoice.BillingMonth > defaultSuspensionDate.Month) || (invoice.BillingYear == defaultSuspensionDate.Year && invoice.BillingMonth == defaultSuspensionDate.Month && invoice.BillingPeriod >= defaultSuspensionDate.Day)) && invoice.SuspendedInvoiceFlag == false && (invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling || invoice.InvoiceStatusId == (int)InvoiceStatusType.Claimed || invoice.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete || invoice.InvoiceStatusId == (int)InvoiceStatusType.Presented) && (invoice.SettlementMethodId == (int)SMI.Ach || invoice.SettlementMethodId == (int)SMI.AchUsingIataRules)).ToList();
              foreach (var invoiceBase in billingInvoiceList)
              {
                invoiceBase.SuspendedInvoiceFlag = true;
                invoiceBase.SuspensionMonth = Convert.ToInt32(suspensionMonth);
                invoiceBase.SuspensionPeriod = defaultSuspensionDate.Day;
                InvoiceBaseRepository.Update(invoiceBase);

              }
            }
            UnitOfWork.CommitDefault();

            return true;
        }

        /// <summary>
        /// To mark suspended billed member's invoices as suspended.
        /// </summary>
        /// <param name="memberId">Member Id.</param>
        /// <param name="defaultSuspensionDate">Default Suspension Date.</param>
        /// <param name="fromSuspensionDate">From Suspension Date.</param>
        /// <param name="clearingHouse">Clearing House.</param>
        /// <returns></returns>
        public List<InvoiceBase> UpdateSuspendedBilledMemberInvoices(int memberId, DateTime defaultSuspensionDate, DateTime fromSuspensionDate, string clearingHouse)
        {
            string suspensionMonth;
            var billedInvoiceList = new List<InvoiceBase>();
            var finalBilledInvoiceList = new List<InvoiceBase>();

            if (defaultSuspensionDate.Month.ToString().Length < 2)

                suspensionMonth = defaultSuspensionDate.Year + "0" + defaultSuspensionDate.Month;
            else
                suspensionMonth = defaultSuspensionDate.Year + defaultSuspensionDate.Month.ToString();
            InvoiceBaseRepository = Ioc.Resolve<IRepository<InvoiceBase>>();

           // CMP-409 if member is merged entity then Mark all Child member's invoice as suspended
            var childMemberList = MemberManager.GetChildMembers(memberId);

            foreach (var childMember in childMemberList)
            {
                /* CMP #624: ICH Rewrite-New SMI X 
                 * Description: 2.24 Retroactive Suspension of Members
                 * Change #1: Inclusion of SMI X Invoices/Credit Notes in retroactive suspensions */

              if ((clearingHouse.Equals("I")) || (clearingHouse.Equals("A")))
                  billedInvoiceList = InvoiceBaseRepository.Get(invoice => invoice.BilledMemberId == childMember.MemberId && ((invoice.BillingYear > defaultSuspensionDate.Year) || (invoice.BillingYear == defaultSuspensionDate.Year && invoice.BillingMonth > defaultSuspensionDate.Month) || (invoice.BillingYear == defaultSuspensionDate.Year && invoice.BillingMonth == defaultSuspensionDate.Month && invoice.BillingPeriod >= defaultSuspensionDate.Day)) && invoice.SuspendedInvoiceFlag == false && (invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling || invoice.InvoiceStatusId == (int)InvoiceStatusType.Claimed || invoice.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete || invoice.InvoiceStatusId == (int)InvoiceStatusType.Presented) && (invoice.SettlementMethodId == (int)SMI.Ich || invoice.SettlementMethodId == (int)SMI.Ach || invoice.SettlementMethodId == (int)SMI.AchUsingIataRules || invoice.SettlementMethodId == (int)SMI.IchSpecialAgreement)).ToList();
              else if (clearingHouse.Equals("AB"))
                billedInvoiceList = InvoiceBaseRepository.Get(invoice => invoice.BilledMemberId == childMember.MemberId && ((invoice.BillingYear > defaultSuspensionDate.Year) || (invoice.BillingYear == defaultSuspensionDate.Year && invoice.BillingMonth > defaultSuspensionDate.Month) || (invoice.BillingYear == defaultSuspensionDate.Year && invoice.BillingMonth == defaultSuspensionDate.Month && invoice.BillingPeriod >= defaultSuspensionDate.Day)) && invoice.SuspendedInvoiceFlag == false && (invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling || invoice.InvoiceStatusId == (int)InvoiceStatusType.Claimed || invoice.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete || invoice.InvoiceStatusId == (int)InvoiceStatusType.Presented) && (invoice.SettlementMethodId == (int)SMI.Ach || invoice.SettlementMethodId == (int)SMI.AchUsingIataRules)).ToList();
              else if (clearingHouse.Equals("IB"))
                  billedInvoiceList = InvoiceBaseRepository.Get(invoice => invoice.BilledMemberId == childMember.MemberId && ((invoice.BillingYear > defaultSuspensionDate.Year) || (invoice.BillingYear == defaultSuspensionDate.Year && invoice.BillingMonth > defaultSuspensionDate.Month) || (invoice.BillingYear == defaultSuspensionDate.Year && invoice.BillingMonth == defaultSuspensionDate.Month && invoice.BillingPeriod >= defaultSuspensionDate.Day)) && invoice.SuspendedInvoiceFlag == false && (invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling || invoice.InvoiceStatusId == (int)InvoiceStatusType.Claimed || invoice.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete || invoice.InvoiceStatusId == (int)InvoiceStatusType.Presented) && (invoice.SettlementMethodId == (int)SMI.Ich || invoice.SettlementMethodId == (int)SMI.IchSpecialAgreement)).ToList();
              foreach (var invoiceBase in billedInvoiceList)
              {
                invoiceBase.SuspendedInvoiceFlag = true;
                invoiceBase.SuspensionMonth = Convert.ToInt32(suspensionMonth);
                invoiceBase.SuspensionPeriod = defaultSuspensionDate.Day;
                InvoiceBaseRepository.Update(invoiceBase);
                finalBilledInvoiceList.Add(invoiceBase);
              }
            }
            UnitOfWork.CommitDefault();

            return finalBilledInvoiceList.ToList();
        }
        ///<summary>
        /// Update invoice status to 'Processing Complete'
        /// </summary>
        public bool UpdateInvoiceStatusToProcessingComplete()
        {
            //Update status to 'Processing Complete' for all claimed invoices
            ProcessingCompleteClaimedInvoices();
            //Update status to 'Processing Complete' for all suspended invoices having SMI as 'A'
            ProcessingCompleteAchSuspendedInvoices();
            //Update status to 'Processing Complete' for all invoices having SMI as 'B','N','R'
            ProcessingCompleteForSMI_BNRInvoices();
            return true;
        }
        ///<summary>
        /// Update status of all claimed invoices to ProcessingComplete based on certain parameters
        /// (a) INVOICE_STATUS_ID == 4 i.e. 'Claimed'
        /// (b) VALUE_CONFIRM_STATUS == 1 or 3 (1 for 'Not Required; and 3 for 'Completed')
        /// (c) DIGITAL_SIGN_STATUS == 1 or 4 (1 for 'Not Required; and 4 for 'Completed')
        /// (d) Supporting_Attachment_STATUS == Completed i.e.3
        /// </summary>
        void ProcessingCompleteClaimedInvoices()
        {
            //get list of invoices based on above parameters (for all SMI i.e. 'A', 'I')
            //supporting attachment status field needs to be added to invoice structure
            InvoiceBaseRepository = Ioc.Resolve<IRepository<InvoiceBase>>();
            var billingInvoiceList =
                InvoiceBaseRepository.Get(
                    invoice => (invoice.InvoiceStatusId == (int)InvoiceStatusType.Claimed && (invoice.ValueConfirmationStatusId.HasValue && invoice.ValueConfirmationStatusId == (int)ValueConfirmationStatus.Completed)
                                || (invoice.ValueConfirmationStatusId.HasValue && invoice.ValueConfirmationStatusId == (int)ValueConfirmationStatus.NotRequired)) && (invoice.DigitalSignatureStatusId == (int)DigitalSignatureStatus.Completed
                                || invoice.DsStatus == "N") && invoice.SupportingAttachmentStatusId == (int)SupportingAttachmentStatus.Completed);

            //loop for each invoice and update invoice status to Processing Complete
            foreach (var invoiceBase in billingInvoiceList)
            {
                InvoiceBaseRepository.Refresh(invoiceBase);
                invoiceBase.InvoiceStatusId = (int)InvoiceStatusType.ProcessingComplete;
                //InvoiceBaseRepository.Update(invoiceBase);
            }
            UnitOfWork.CommitDefault();
            InvoiceBaseRepository = null;
            return;
        }


        ///<summary>
        /// Update status of all 'Ready for Billing' ACH invoices to ProcessingComplete based on certain parameters
        /// (a) INVOICE_STATUS_ID == 4 i.e. 'Ready for Billing'
        /// (b) VALUE_CONFIRM_STATUS == 1 or 3 (1 for 'Not Required; and 3 for 'Completed')
        /// (c) DIGITAL_SIGN_STATUS == 1 or 4 (1 for 'Not Required; and 4 for 'Completed')
        /// (d) SUPPORTING_ATTACHMENT_STATUS == Completed i.e.3
        /// (e) IS_SUSPENDED == 1 (true)
        /// (f) SETTLEMENT_METHOD_ID = 2 i.e. ACH
        /// </summary>
        void ProcessingCompleteAchSuspendedInvoices()
        {
            //get list of invoices based on above parameters (only ACH invoices)
            //supporting attachment status field needs to be added to invoice structure
            InvoiceBaseRepository = Ioc.Resolve<IRepository<InvoiceBase>>();
            var billingInvoiceList = InvoiceRepository.Get(
                invoice => (invoice.SuspendedInvoiceFlag && invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling && invoice.SettlementMethodId == (int)SMI.Ach && (invoice.ValueConfirmationStatusId.HasValue && invoice.ValueConfirmationStatusId == (int)ValueConfirmationStatus.Completed)
                          || (invoice.ValueConfirmationStatusId.HasValue && invoice.ValueConfirmationStatusId == (int)ValueConfirmationStatus.NotRequired)) && (invoice.DigitalSignatureStatusId == (int)DigitalSignatureStatus.Completed
                          || invoice.DsStatus == "N") && invoice.SupportingAttachmentStatusId == (int)SupportingAttachmentStatus.Completed);

            //InvoiceBaseRepository.Get(
            //    invoice => (invoice.SuspendedInvoiceFlag && invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling && invoice.SettlementMethodId == (int)SMI.Ach && (invoice.ValueConfirmationStatusId.HasValue && invoice.ValueConfirmationStatusId == (int)ValueConfirmationStatus.Completed)
            //              || (invoice.ValueConfirmationStatusId.HasValue && invoice.ValueConfirmationStatusId == (int)ValueConfirmationStatus.NotRequired)) && (invoice.DigitalSignatureStatusId == (int)DigitalSignatureStatus.Completed
            //              || invoice.DigitalSignatureStatusId == (int)DigitalSignatureStatus.NotRequired) && invoice.SupportingAttachmentStatusId == (int)SupportingAttachmentStatus.Completed);

            //loop for each invoice and update invoice status to Processing Complete
            foreach (var invoiceBase in billingInvoiceList)
            {
                InvoiceBaseRepository.Refresh(invoiceBase);
                invoiceBase.InvoiceStatusId = (int)InvoiceStatusType.ProcessingComplete;
                //InvoiceBaseRepository.Update(invoiceBase);
            }
            UnitOfWork.CommitDefault();

            return;
        }

        void ProcessingCompleteForSMI_BNRInvoices()
        {
            //get list of invoices based on above parameters (only ACH invoices)
            //supporting attachment status field needs to be added to invoice structure
            /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
            InvoiceBaseRepository = Ioc.Resolve<IRepository<InvoiceBase>>();
            var billingInvoiceList = InvoiceRepository.Get(
                invoice => (invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling && (ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false) || invoice.SettlementMethodId == (int)SMI.AdjustmentDueToProtest || invoice.SettlementMethodId == (int)SMI.AchUsingIataRules)) && (invoice.ValueConfirmationStatusId == (int)ValueConfirmationStatus.Completed || invoice.ValueConfirmationStatusId == (int)ValueConfirmationStatus.NotRequired) && (invoice.DigitalSignatureStatusId == (int)DigitalSignatureStatus.Completed || invoice.DigitalSignatureStatusId == (int)DigitalSignatureStatus.NotRequired)
                           && invoice.SupportingAttachmentStatusId == (int)SupportingAttachmentStatus.Completed);

            //loop for each invoice and update invoice status to Processing Complete
            foreach (var invoiceBase in billingInvoiceList)
            {
                //InvoiceBaseRepository.Refresh(invoiceBase);
                invoiceBase.InvoiceStatusId = (int)InvoiceStatusType.ProcessingComplete;
                invoiceBase.LastUpdatedOn = DateTime.UtcNow;
                //InvoiceBaseRepository.Update(invoiceBase);
            }
            UnitOfWork.CommitDefault();

            return;
        }



        /// <summary>
        /// Get invoice using load strategy to be used in readonly header
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public PaxInvoice GetInvoiceHeader(string invoiceId)
        {
            Guid invoiceIdGuid = invoiceId.ToGuid();
            return InvoiceRepository.GetInvoiceHeader(invoiceIdGuid);
        }

        /// <summary>
        /// Validates the acceptable differences.
        /// </summary>
        /// <param name="reasonCode">The reason code.</param>
        /// <param name="transactionType">Type of the transaction.</param>
        /// <param name="fareAmountDifference">Gross amount difference.</param>
        /// <param name="taxAmountDifference">Tax amount difference.</param>
        /// <param name="vatAmountDifference">VAT amount difference.</param>
        /// <param name="iscAmountDifference">ISC amount difference.</param>
        /// <param name="uatpAmountDifference">UATP amount difference.</param>
        /// <param name="handlingFeeAmountDifference">Handling fee amount difference.</param>
        /// <param name="otherCommissionAmountDifference">Other commission amount difference.</param>
        /// <returns></returns>
        public string ValidateAcceptableDifferences(string reasonCode, TransactionType transactionType, double fareAmountDifference, double taxAmountDifference,
                                                  double vatAmountDifference, double iscAmountDifference, double uatpAmountDifference,
                                                  double handlingFeeAmountDifference, double otherCommissionAmountDifference, IList<ReasonCode> validReasonCodes = null, IList<RMReasonAcceptableDiff> validRmReasonAcceptableDiff = null)
        {
            ReasonCode reasonCodeObj;
            if (validReasonCodes != null)
            {
                reasonCodeObj =
                  validReasonCodes.FirstOrDefault(
                    rCode =>
                    rCode.Code.Equals(reasonCode, StringComparison.OrdinalIgnoreCase) && rCode.TransactionTypeId == (int)transactionType &&
                    rCode.IsActive);
            }
            else
            {
                reasonCodeObj =
                  ReasonCodeRepository.Single(
                    rCode =>
                    rCode.Code.Equals(reasonCode, StringComparison.OrdinalIgnoreCase) && rCode.TransactionTypeId == (int)transactionType &&
                    rCode.IsActive);
            }
            int reasonCodeId;

            RMReasonAcceptableDiff reasonCodeAmountDifference = null;
            if (reasonCodeObj != null)
            {
                reasonCodeId = reasonCodeObj.Id;

                reasonCodeAmountDifference = validRmReasonAcceptableDiff != null
                                               ? validRmReasonAcceptableDiff.FirstOrDefault(acceptableDiff => acceptableDiff.ReasonCodeId == reasonCodeId && acceptableDiff.IsActive)
                                               : RmReasonAcceptableDifferenceRepository.First(acceptableDiff => acceptableDiff.ReasonCodeId == reasonCodeId && acceptableDiff.IsActive);
            }

            if (reasonCodeAmountDifference == null) return string.Empty;
            var differenceFields = new StringBuilder(string.Empty);

            const string comma = ",";
            if (!reasonCodeAmountDifference.IsFareAmount && fareAmountDifference != 0)
            {
                differenceFields.Append(" Gross").Append(comma);
            }
            if (!reasonCodeAmountDifference.IsHfAmount && handlingFeeAmountDifference != 0)
            {
                differenceFields.Append(" Handling Fee").Append(comma);
            }
            if (!reasonCodeAmountDifference.IsIscAmount && iscAmountDifference != 0)
            {
                differenceFields.Append(" ISC ").Append(comma);
            }
            if (!reasonCodeAmountDifference.IsOcAmount && otherCommissionAmountDifference != 0)
            {
                differenceFields.Append(" Other Commission ").Append(comma);
            }
            if (!reasonCodeAmountDifference.IsTaxAmount && taxAmountDifference != 0)
            {
                differenceFields.Append(" Tax ").Append(comma);
            }
            if (!reasonCodeAmountDifference.IsUatpAmount && uatpAmountDifference != 0)
            {
                differenceFields.Append(" UATP ").Append(comma);
            }
            if (!reasonCodeAmountDifference.IsVatAmount && vatAmountDifference != 0)
            {
                differenceFields.Append(" VAT ").Append(comma);
            }

            return differenceFields.Length > 0 ? differenceFields.Remove(differenceFields.Length - 1, 1).ToString() : differenceFields.ToString();
        }

        /// <summary>
        /// Gets the default currency.
        /// </summary>
        /// <param name="settlementMethodId">The settlement method id.</param>
        /// <param name="billingMemberId">The billing member id.</param>
        /// <param name="billedMemberId">The billed member id.</param>
        /// <returns></returns>
        public int GetDefaultCurrency(int settlementMethodId, int billingMemberId, int billedMemberId)
        {
          // SCP237121: Prevention of Unhandled exception found in IS-WEB Log.
          // Handled the non-zero value in parameters. 
          if(settlementMethodId <= 0 || billingMemberId <= 0 || billedMemberId <= 0) 
          { 
            return -1; 
          } 
            // Get Billing-Billed final parent data
            var billingMember = MemberManager.GetMember(MemberManager.GetFinalParentDetails(billingMemberId));
            var billedMember = MemberManager.GetMember(MemberManager.GetFinalParentDetails(billedMemberId));

            billingMember.IchConfiguration = GetIchConfiguration(billingMember.Id);
            billedMember.IchConfiguration = GetIchConfiguration(billedMember.Id);

            if (settlementMethodId == (int)SMI.Ich && billingMember.IchConfiguration != null)
            {
                switch (billingMember.IchConfiguration.IchZone)
                {
                    case IchZoneType.A:
                        {
                            // Billed airline in zone A.
                            if (billedMember.IchConfiguration != null && billedMember.IchConfiguration.IchZone == IchZoneType.A)
                            {
                                return (int)BillingCurrency.GBP;
                            }
                            // Billed airline in any zone other than A.
                            return (int)BillingCurrency.USD;
                        }
                    case IchZoneType.B:
                    case IchZoneType.C:
                        return (int)BillingCurrency.USD;
                    case IchZoneType.D:
                        // Billed airline in zone A, B, C.
                        if (billedMember.IchConfiguration != null && billedMember.IchConfiguration.IchZone == IchZoneType.D)
                        {
                            return (int)BillingCurrency.EUR;
                        }
                        // Billed airline in any zone other than D.
                        return (int)BillingCurrency.USD;
                }
            }

            // When SMI = A/M, and if only billing member belongs to ACH, lock the dropdown on USD. 
            if ((settlementMethodId == (int)SMI.Ach ||
                 settlementMethodId == (int)SMI.AchUsingIataRules)
                && IsAchMember(billingMember))
            {
                return (int)BillingCurrency.USD;
            }

            // When SMI = A/M, and both members belong to ACH, dropdown should only offer USD/CAD. Default to USD
            if ((settlementMethodId == (int)SMI.Ach ||
                 settlementMethodId == (int)SMI.AchUsingIataRules)
                && IsAchMember(billingMember) && IsAchMember(billedMember))
            {
                return (int)BillingCurrency.USD;
            }

            // If billing member is only ACH then default currency should be USD
            billingMember.AchConfiguration = GetAchConfiguration(billingMemberId);
            if (IsAchMember(billingMember) && !IsIchMember(billingMember))
            {
                return (int)BillingCurrency.USD;
            }

            return -1;
        }

      public PaxAuditTrail GetbillingHistoryAuditTrail(string transactionId, string transactionType)
        {
            var transactionGuid = transactionId.ToGuid();

            var auditTrail = InvoiceRepository.AuditSingle(transactionGuid, transactionType);

            return auditTrail;
        }

        /// <summary>
        /// Ignores the validation in migration period.
        /// </summary>
        /// <param name="invoice">The invoice.</param>
        /// <param name="rejectionMemoRecord"></param>
        /// <param name="transactionType">Type of the transaction.</param>
        /// <returns></returns>
        public bool IgnoreValidationInMigrationPeriodForSamplingFormD(PaxInvoice invoice, SamplingFormDRecord samplingFormDRecord, TransactionType transactionType)
        {

            // Get billing and billed member Cargo configuration. - To Check
            var passengerConfiguration = MemberManager.GetPassengerConfiguration(invoice.BilledMemberId);

            var isXmlfileType = (invoice.SubmissionMethod == (Model.Enums.SubmissionMethod)SubmissionMethod.IsXml) ? true : false;

            if (!IsMemberMigratedForFormDLinking(invoice, samplingFormDRecord, transactionType, isXmlfileType, passengerConfiguration, false))
            {
              return true;
            }

            return false;
        }

        /// <summary>
        /// Ignores the validation in migration period.
        /// </summary>
        /// <param name="invoice">The invoice.</param>
        /// <param name="rejectionMemoRecord"></param>
        /// <param name="transactionType">Type of the transaction.</param>
        /// <returns></returns>
        public bool IgnoreValidationInMigrationPeriod(PaxInvoice invoice, RejectionMemo rejectionMemoRecord, TransactionType transactionType, PassengerConfiguration passengerConfig = null)
        {
          //SCP#48125 : RM Linking
          var passengerConfiguration = passengerConfig ?? MemberManager.GetPassengerConfiguration(invoice.BilledMemberId);

          var isXmlfileType = (invoice.SubmissionMethod == (Model.Enums.SubmissionMethod)SubmissionMethod.IsXml) ? true : false;

          if (!IsMemberMigratedForTransactionLinking(invoice, rejectionMemoRecord, transactionType, isXmlfileType, passengerConfiguration, false))
          {
            return true;
          }
          if (ValidationCache.Instance.IgnoreValidationOnMigrationPeriod)
          {
            return true;
          }
          return false;

        }

        /// <summary>
        /// Ignores the validation in migration period.
        /// </summary>
        /// <param name="invoice">The invoice.</param>
        /// <param name="transactionType">Type of the transaction.</param>
        /// <param name="isXmlFileType">if set to true if validation needs to be ignored.</param>
        /// <param name="passengerConfiguration"></param>
        /// <returns></returns>
        public bool IgnoreValidationInMigrationPeriod(PaxInvoice invoice, TransactionType transactionType, bool isXmlFileType, PassengerConfiguration passengerConfiguration = null)
        {
          //SCP#48125 : RM Linking
          if (!IsMemberMigratedForTransactionLinking(invoice, transactionType, isXmlFileType, passengerConfiguration, false))
          {
            return true;
          }
          if ((transactionType != TransactionType.SamplingFormAB && transactionType != TransactionType.SamplingFormC && transactionType != TransactionType.SamplingFormD) && ValidationCache.Instance.IgnoreValidationOnMigrationPeriod)
          {
            return true;
          }

          return false;
        }

        /// <summary>
        /// The mothod checks memeber migration in case of linking
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="transactionType"></param>
        /// <param name="isXmlFileType"></param>
        /// <param name="passengerConfig"></param>
        /// <param name="isBillingMember"></param>
        /// <returns></returns>
        public bool IsMemberMigratedForTransactionLinking(InvoiceBase invoice, TransactionType transactionType, bool isXmlFileType, PassengerConfiguration passengerConfig = null, bool isBillingMember = true)
        {
          var memberId = isBillingMember ? invoice.BillingMemberId : invoice.BilledMemberId;

          if (isBillingMember && (invoice.BillingMember == null || invoice.BillingMemberId == 0))
          {
            return true;
          }

          if (!isBillingMember && (invoice.BilledMember == null || invoice.BilledMemberId == 0))
          {
            return true;
          }

          var passengerConfiguration = passengerConfig ?? MemberManager.GetPassengerConfiguration(memberId);

          if (passengerConfiguration == null)
          {
            return false;
          }

          var billingPeriod = new BillingPeriod(invoice.BillingYear, invoice.BillingMonth, invoice.BillingPeriod);
          int provisionalBillingMonth = invoice.BillingMonth;
          int provisionalBillingYear = invoice.BillingYear;
          var paxinvoice = invoice as PaxInvoice;
          if(paxinvoice!=null)
          {
            provisionalBillingMonth = paxinvoice.ProvisionalBillingMonth > 0 ? paxinvoice.ProvisionalBillingMonth : invoice.BillingMonth;
            //SCP#456109 : Invalid record in PIDECT-61820160104.DAT (SQ IS-IDEC file)
            provisionalBillingYear = paxinvoice.ProvisionalBillingYear > 0 ? paxinvoice.ProvisionalBillingYear : invoice.BillingYear;
          }
          DateTime? minumOfMigrationDate = null;

          switch (transactionType)
          {
            case TransactionType.Coupon:

              if (passengerConfiguration.NonSamplePrimeBillingIsIdecMigratedDate.HasValue && passengerConfiguration.NonSamplePrimeBillingIsIdecMigrationStatus == MigrationStatus.Certified)
              {
                minumOfMigrationDate = passengerConfiguration.NonSamplePrimeBillingIsIdecMigratedDate;
              }

              if (passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate.HasValue && passengerConfiguration.NonSamplePrimeBillingIsxmlMigrationStatus == MigrationStatus.Certified)
              {
                if (minumOfMigrationDate.HasValue)
                  minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate;
                else
                {
                  minumOfMigrationDate = passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate;
                }

              }

              if (passengerConfiguration.NonSamplePrimeBillingIswebMigratedDate.HasValue)
              {
                if (minumOfMigrationDate.HasValue)
                  minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSamplePrimeBillingIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSamplePrimeBillingIswebMigratedDate;
                else
                {
                  minumOfMigrationDate = passengerConfiguration.NonSamplePrimeBillingIswebMigratedDate;
                }
              }

              if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0 && minumOfMigrationDate.Value.Day != 0)
              {
                var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, minumOfMigrationDate.Value.Day);
                return (billingPeriod >= isMigrationPeriod);
              }
              return false;
              break;

            case TransactionType.RejectionMemo1:

              if (passengerConfiguration.NonSampleRmIsIdecMigratedDate.HasValue && passengerConfiguration.NonSampleRmIsIdecMigrationStatus == MigrationStatus.Certified)
              {
                minumOfMigrationDate = passengerConfiguration.NonSampleRmIsIdecMigratedDate;
              }

              if (passengerConfiguration.NonSampleRmIsXmlMigratedDate.HasValue && passengerConfiguration.NonSampleRmIsXmlMigrationStatus == MigrationStatus.Certified)
              {
                if (minumOfMigrationDate.HasValue)
                  minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSampleRmIsXmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSampleRmIsXmlMigratedDate;
                else
                {
                  minumOfMigrationDate = passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate;


                }

              }

              if (passengerConfiguration.NonSampleRmIswebMigratedDate.HasValue)
              {
                if (minumOfMigrationDate.HasValue)
                  minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSampleRmIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSampleRmIswebMigratedDate;
                else
                {
                  minumOfMigrationDate = passengerConfiguration.NonSampleRmIswebMigratedDate;
                }
              }


              if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0 && minumOfMigrationDate.Value.Day != 0)
              {
                var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, minumOfMigrationDate.Value.Day);
                return (billingPeriod >= isMigrationPeriod);
              }
              return false;
              break;
            case TransactionType.BillingMemo:

              if (passengerConfiguration.NonSampleBmIsIdecMigratedDate.HasValue && passengerConfiguration.NonSampleBmIsIdecMigrationStatus == MigrationStatus.Certified)
              {
                minumOfMigrationDate = passengerConfiguration.NonSampleBmIsIdecMigratedDate;
              }

              if (passengerConfiguration.NonSampleBmIsXmlMigratedDate.HasValue && passengerConfiguration.NonSamplePrimeBillingIsxmlMigrationStatus == MigrationStatus.Certified)
              {
                if (minumOfMigrationDate.HasValue)
                  minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSampleBmIsXmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSampleBmIsXmlMigratedDate;
                else
                {
                  minumOfMigrationDate = passengerConfiguration.NonSampleBmIsXmlMigratedDate;
                }

              }
              if (passengerConfiguration.NonSampleBmIswebMigratedDate.HasValue)
              {
                if (minumOfMigrationDate.HasValue)
                  minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSampleBmIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSampleBmIswebMigratedDate;
                else
                {
                  minumOfMigrationDate = passengerConfiguration.NonSampleBmIswebMigratedDate;
                }
              }

              if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0 && minumOfMigrationDate.Value.Day != 0)
              {
                var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, minumOfMigrationDate.Value.Day);
                return (billingPeriod >= isMigrationPeriod);
              }
              return false;

              break;
            case TransactionType.CreditMemo:

              if (passengerConfiguration.NonSampleCmIsIdecMigratedDate.HasValue && passengerConfiguration.NonSampleCmIsIdecMigrationStatus == MigrationStatus.Certified)
              {
                minumOfMigrationDate = passengerConfiguration.NonSampleCmIsIdecMigratedDate;
              }

              if (passengerConfiguration.NonSampleCmIsXmlMigratedDate.HasValue && passengerConfiguration.NonSampleCmIsXmlMigrationStatus == MigrationStatus.Certified)
              {
                if (minumOfMigrationDate.HasValue)
                  minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSampleCmIsXmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSampleCmIsXmlMigratedDate;
                else
                {
                  minumOfMigrationDate = passengerConfiguration.NonSampleCmIsXmlMigratedDate;
                }

              }

              if (passengerConfiguration.NonSampleCmIswebMigratedDate.HasValue)
              {
                if (minumOfMigrationDate.HasValue)
                  minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSampleCmIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSampleCmIswebMigratedDate;
                else
                {
                  minumOfMigrationDate = passengerConfiguration.NonSampleCmIswebMigratedDate;
                }
              }

              if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0 && minumOfMigrationDate.Value.Day != 0)
              {
                var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, minumOfMigrationDate.Value.Day);
                return (billingPeriod >= isMigrationPeriod);
              }
              return false;

              break;

            case TransactionType.SamplingFormAB:
              
              billingPeriod = new BillingPeriod(provisionalBillingYear, provisionalBillingMonth, 1);
              if (passengerConfiguration.SamplingProvIsIdecMigratedDate.HasValue && passengerConfiguration.SamplingProvIsIdecMigrationStatus == MigrationStatus.Certified)
              {
                minumOfMigrationDate = passengerConfiguration.SamplingProvIsIdecMigratedDate;
              }

              if (passengerConfiguration.SamplingProvIsxmlMigratedDate.HasValue && passengerConfiguration.SamplingProvIsxmlMigrationStatus == MigrationStatus.Certified)
              {
                if (minumOfMigrationDate.HasValue)
                  minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SamplingProvIsxmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.SamplingProvIsxmlMigratedDate;
                else
                {
                  minumOfMigrationDate = passengerConfiguration.SamplingProvIsxmlMigratedDate;

                }

              }


              if (passengerConfiguration.SamplingProvIswebMigratedDate.HasValue)
              {
                if (minumOfMigrationDate.HasValue)
                  minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SamplingProvIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.SamplingProvIswebMigratedDate;
                else
                {
                  minumOfMigrationDate = passengerConfiguration.SamplingProvIswebMigratedDate;
                }
              }

              if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0)
              {
                var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, 1);
                return (billingPeriod >= isMigrationPeriod);
              }
              return false;
              break;

            case TransactionType.SamplingFormC:
              billingPeriod = new BillingPeriod(provisionalBillingYear, provisionalBillingMonth, 1);
              if (passengerConfiguration.SampleFormCIsIdecMigratedDate.HasValue && passengerConfiguration.SampleFormCIsIdecMigrationStatus == MigrationStatus.Certified)
              {
                minumOfMigrationDate = passengerConfiguration.SampleFormCIsIdecMigratedDate;
              }

              if (passengerConfiguration.SampleFormCIsxmlMigratedDate.HasValue && passengerConfiguration.SampleFormCIsxmlMigrationStatus == MigrationStatus.Certified)
              {
                if (minumOfMigrationDate.HasValue)
                  minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SampleFormCIsxmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.SampleFormCIsxmlMigratedDate;
                else
                {
                  minumOfMigrationDate = passengerConfiguration.SampleFormCIsxmlMigratedDate;
                }

              }

              if (passengerConfiguration.SampleFormCIswebMigratedDate.HasValue)
              {
                if (minumOfMigrationDate.HasValue)
                  minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SampleFormCIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.SampleFormCIswebMigratedDate;
                else
                {
                  minumOfMigrationDate = passengerConfiguration.SampleFormCIswebMigratedDate;
                }
              }
              if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0)
              {
                var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, 1);
                return (billingPeriod >= isMigrationPeriod);
              }
              return false;
              break;
            case TransactionType.SamplingFormD:
              billingPeriod = new BillingPeriod(provisionalBillingYear, provisionalBillingMonth, 1);
              if (passengerConfiguration.SampleFormDeIsIdecMigratedDate.HasValue && passengerConfiguration.SampleFormDeIsIdecMigrationStatus == MigrationStatus.Certified)
              {
                minumOfMigrationDate = passengerConfiguration.SampleFormDeIsIdecMigratedDate;
              }


              if (passengerConfiguration.SampleFormDeIsxmlMigratedDate.HasValue && passengerConfiguration.SampleFormDEisxmlMigrationStatus == MigrationStatus.Certified)
              {
                if (minumOfMigrationDate.HasValue)
                  minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SampleFormDeIsxmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.SampleFormDeIsxmlMigratedDate;
                else
                {
                  minumOfMigrationDate = passengerConfiguration.SampleFormDeIsxmlMigratedDate;

                }

              }

              if (passengerConfiguration.SampleFormDeIswebMigratedDate.HasValue)
              {
                if (minumOfMigrationDate.HasValue)
                  minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SampleFormDeIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.SampleFormDeIswebMigratedDate;
                else
                {
                  minumOfMigrationDate = passengerConfiguration.SampleFormDeIswebMigratedDate;



                }
              }

              if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0 && minumOfMigrationDate.Value.Day != 0)
              {
                var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, minumOfMigrationDate.Value.Day);
                return (billingPeriod >= isMigrationPeriod);
              }
              return false;
              break;
            case TransactionType.SamplingFormF:
              billingPeriod = new BillingPeriod(provisionalBillingYear, provisionalBillingMonth, 1);
              if (passengerConfiguration.SampleFormFxfIsIdecMigratedDate.HasValue && passengerConfiguration.SampleFormFxfIsIdecMigrationStatus == MigrationStatus.Certified)
              {
                minumOfMigrationDate = passengerConfiguration.SampleFormFxfIsIdecMigratedDate;
              }

              if (passengerConfiguration.SampleFormFxfIsxmlMigratedDate.HasValue && passengerConfiguration.SampleFormFxfIsxmlMigratedStatus == MigrationStatus.Certified)
              {
                if (minumOfMigrationDate.HasValue)
                  minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SampleFormFxfIsxmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.SampleFormFxfIsxmlMigratedDate;
                else
                {
                  minumOfMigrationDate = passengerConfiguration.SampleFormFxfIsxmlMigratedDate;

                }

              }
              if (passengerConfiguration.SampleFormFxfIswebMigratedDate.HasValue)
              {
                if (minumOfMigrationDate.HasValue)
                  minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SampleFormFxfIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.SampleFormFxfIswebMigratedDate;
                else
                {
                  minumOfMigrationDate = passengerConfiguration.SampleFormFxfIswebMigratedDate;
                }
              }

              if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0 && minumOfMigrationDate.Value.Day != 0)
              {
                var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, minumOfMigrationDate.Value.Day);
                return (billingPeriod >= isMigrationPeriod);
              }
              return false;
              break;
          }

          return false;
        }


        /// <summary>
        /// The method checks member migration in case of linking
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="samplingFormDRecord"></param>
        /// <param name="transactionType"></param>
        /// <param name="isXmlFileType"></param>
        /// <param name="passengerConfig"></param>
        /// <param name="isBillingMember"></param>
        /// <returns></returns>
        public bool IsMemberMigratedForFormDLinking(PaxInvoice invoice, SamplingFormDRecord samplingFormDRecord, TransactionType transactionType, bool isXmlFileType, PassengerConfiguration passengerConfig = null, bool isBillingMember = true)
        {
            var memberId = isBillingMember ? invoice.BillingMemberId : invoice.BilledMemberId;

            if (isBillingMember && (invoice.BillingMember == null || invoice.BillingMemberId == 0))
            {
                return true;
            }

            if (!isBillingMember && (invoice.BilledMember == null || invoice.BilledMemberId == 0))
            {
                return true;
            }

            var passengerConfiguration = passengerConfig ?? MemberManager.GetPassengerConfiguration(memberId);

            if (passengerConfiguration == null)
            {
                return false;
            }

            if (invoice.ProvisionalBillingMonth == 0 || invoice.ProvisionalBillingMonth > 12 || invoice.ProvisionalBillingYear == 0)
            {
                Logger.InfoFormat("Invalid your invoice billing month or year");
                return false;
            }

            var billingPeriod = new BillingPeriod(invoice.ProvisionalBillingYear, invoice.ProvisionalBillingMonth, 1);

            //var billingPeriod = new BillingPeriod(invoice.BillingYear, invoice.BillingMonth, 1);

            DateTime? minumOfMigrationDate = null;

            switch (transactionType)
            {
                case TransactionType.Coupon:

                    if (passengerConfiguration.NonSamplePrimeBillingIsIdecMigratedDate.HasValue && passengerConfiguration.NonSamplePrimeBillingIsIdecMigrationStatus == MigrationStatus.Certified)
                    {
                        minumOfMigrationDate = passengerConfiguration.NonSamplePrimeBillingIsIdecMigratedDate;
                    }

                    if (passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate.HasValue && passengerConfiguration.NonSamplePrimeBillingIsxmlMigrationStatus == MigrationStatus.Certified)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate;
                        }

                    }

                    if (passengerConfiguration.NonSamplePrimeBillingIswebMigratedDate.HasValue)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSamplePrimeBillingIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSamplePrimeBillingIswebMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.NonSamplePrimeBillingIswebMigratedDate;
                        }
                    }

                    if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0 && minumOfMigrationDate.Value.Day != 0)
                    {
                      var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, minumOfMigrationDate.Value.Day);
                        return (billingPeriod >= isMigrationPeriod);
                    }
                    return false;
                    break;

                case TransactionType.RejectionMemo1:

                    if (passengerConfiguration.NonSampleRmIsIdecMigratedDate.HasValue && passengerConfiguration.NonSampleRmIsIdecMigrationStatus == MigrationStatus.Certified)
                    {
                        minumOfMigrationDate = passengerConfiguration.NonSampleRmIsIdecMigratedDate;
                    }

                    if (passengerConfiguration.NonSampleRmIsXmlMigratedDate.HasValue && passengerConfiguration.NonSampleRmIsXmlMigrationStatus == MigrationStatus.Certified)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSampleRmIsXmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSampleRmIsXmlMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate;


                        }

                    }

                    if (passengerConfiguration.NonSampleRmIswebMigratedDate.HasValue)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSampleRmIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSampleRmIswebMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.NonSampleRmIswebMigratedDate;
                        }
                    }


                    if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0 && minumOfMigrationDate.Value.Day != 0)
                    {
                      var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, minumOfMigrationDate.Value.Day);
                        return (billingPeriod >= isMigrationPeriod);
                    }
                    return false;
                    break;
                case TransactionType.BillingMemo:

                    if (passengerConfiguration.NonSampleBmIsIdecMigratedDate.HasValue && passengerConfiguration.NonSampleBmIsIdecMigrationStatus == MigrationStatus.Certified)
                    {
                        minumOfMigrationDate = passengerConfiguration.NonSampleBmIsIdecMigratedDate;
                    }

                    if (passengerConfiguration.NonSampleBmIsXmlMigratedDate.HasValue && passengerConfiguration.NonSamplePrimeBillingIsxmlMigrationStatus == MigrationStatus.Certified)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSampleBmIsXmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSampleBmIsXmlMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.NonSampleBmIsXmlMigratedDate;
                        }

                    }
                    if (passengerConfiguration.NonSampleBmIswebMigratedDate.HasValue)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSampleBmIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSampleBmIswebMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.NonSampleBmIswebMigratedDate;
                        }
                    }

                    if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0 && minumOfMigrationDate.Value.Day != 0)
                    {
                      var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, minumOfMigrationDate.Value.Day);
                        return (billingPeriod >= isMigrationPeriod);
                    }
                    return false;

                    break;
                case TransactionType.CreditMemo:

                    if (passengerConfiguration.NonSampleCmIsIdecMigratedDate.HasValue && passengerConfiguration.NonSampleCmIsIdecMigrationStatus == MigrationStatus.Certified)
                    {
                        minumOfMigrationDate = passengerConfiguration.NonSampleCmIsIdecMigratedDate;
                    }

                    if (passengerConfiguration.NonSampleCmIsXmlMigratedDate.HasValue && passengerConfiguration.NonSampleCmIsXmlMigrationStatus == MigrationStatus.Certified)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSampleCmIsXmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSampleCmIsXmlMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.NonSampleCmIsXmlMigratedDate;
                        }

                    }

                    if (passengerConfiguration.NonSampleCmIswebMigratedDate.HasValue)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSampleCmIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSampleCmIswebMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.NonSampleCmIswebMigratedDate;
                        }
                    }

                    if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0 && minumOfMigrationDate.Value.Day != 0)
                    {
                      var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, minumOfMigrationDate.Value.Day);
                        return (billingPeriod >= isMigrationPeriod);
                    }
                    return false;

                    break;

                case TransactionType.SamplingFormAB:


                    if (passengerConfiguration.SamplingProvIsIdecMigratedDate.HasValue && passengerConfiguration.SamplingProvIsIdecMigrationStatus == MigrationStatus.Certified)
                    {
                        minumOfMigrationDate = passengerConfiguration.SamplingProvIsIdecMigratedDate;
                    }

                    if (passengerConfiguration.SamplingProvIsxmlMigratedDate.HasValue && passengerConfiguration.SamplingProvIsxmlMigrationStatus == MigrationStatus.Certified)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SamplingProvIsxmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.SamplingProvIsxmlMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.SamplingProvIsxmlMigratedDate;

                        }

                    }


                    if (passengerConfiguration.SamplingProvIswebMigratedDate.HasValue)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SamplingProvIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.SamplingProvIswebMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.SamplingProvIswebMigratedDate;
                        }
                    }

                    if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0)
                    {
                        var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, 1);
                        return (billingPeriod >= isMigrationPeriod);
                    }
                    return false;
                    break;

                case TransactionType.SamplingFormC:

                    if (passengerConfiguration.SampleFormCIsIdecMigratedDate.HasValue && passengerConfiguration.SampleFormCIsIdecMigrationStatus == MigrationStatus.Certified)
                    {
                        minumOfMigrationDate = passengerConfiguration.SampleFormCIsIdecMigratedDate;
                    }

                    if (passengerConfiguration.SampleFormCIsxmlMigratedDate.HasValue && passengerConfiguration.SampleFormCIsxmlMigrationStatus == MigrationStatus.Certified)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SampleFormCIsxmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.SampleFormCIsxmlMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.SampleFormCIsxmlMigratedDate;
                        }

                    }

                    if (passengerConfiguration.SampleFormCIswebMigratedDate.HasValue)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SampleFormCIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.SampleFormCIswebMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.SampleFormCIswebMigratedDate;
                        }
                    }
                    if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0)
                    {
                        var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, 1);
                        return (billingPeriod >= isMigrationPeriod);
                    }
                    return false;
                    break;
                case TransactionType.SamplingFormD:

                    if (passengerConfiguration.SampleFormDeIsIdecMigratedDate.HasValue && passengerConfiguration.SampleFormDeIsIdecMigrationStatus == MigrationStatus.Certified)
                    {
                        minumOfMigrationDate = passengerConfiguration.SampleFormDeIsIdecMigratedDate;
                    }


                    if (passengerConfiguration.SampleFormDeIsxmlMigratedDate.HasValue && passengerConfiguration.SampleFormDEisxmlMigrationStatus == MigrationStatus.Certified)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SampleFormDeIsxmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.SampleFormDeIsxmlMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.SampleFormDeIsxmlMigratedDate;

                        }

                    }

                    if (passengerConfiguration.SampleFormDeIswebMigratedDate.HasValue)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SampleFormDeIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.SampleFormDeIswebMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.SampleFormDeIswebMigratedDate;



                        }
                    }

                    if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0 && minumOfMigrationDate.Value.Day != 0)
                    {
                      var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, minumOfMigrationDate.Value.Day);
                        return (billingPeriod >= isMigrationPeriod);
                    }
                    return false;
                    break;
                case TransactionType.SamplingFormF:

                    if (passengerConfiguration.SampleFormFxfIsIdecMigratedDate.HasValue && passengerConfiguration.SampleFormFxfIsIdecMigrationStatus == MigrationStatus.Certified)
                    {
                        minumOfMigrationDate = passengerConfiguration.SampleFormFxfIsIdecMigratedDate;
                    }

                    if (passengerConfiguration.SampleFormFxfIsxmlMigratedDate.HasValue && passengerConfiguration.SampleFormFxfIsxmlMigratedStatus == MigrationStatus.Certified)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SampleFormFxfIsxmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.SampleFormFxfIsxmlMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.SampleFormFxfIsxmlMigratedDate;

                        }

                    }
                    if (passengerConfiguration.SampleFormFxfIswebMigratedDate.HasValue)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SampleFormFxfIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.SampleFormFxfIswebMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.SampleFormFxfIswebMigratedDate;
                        }
                    }

                    if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0 && minumOfMigrationDate.Value.Day != 0)
                    {
                      var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, minumOfMigrationDate.Value.Day);
                        return (billingPeriod >= isMigrationPeriod);
                    }
                    return false;
                    break;
            }

            return false;
        }

        /// <summary>
        /// The method checks member migration in case of linking
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="rejectionMemoRecord"></param>
        /// <param name="transactionType"></param>
        /// <param name="isXmlFileType"></param>
        /// <param name="passengerConfig"></param>
        /// <param name="isBillingMember"></param>
        /// <returns></returns>
        public bool IsMemberMigratedForTransactionLinking(InvoiceBase invoice, RejectionMemo rejectionMemoRecord, TransactionType transactionType, bool isXmlFileType, PassengerConfiguration passengerConfig = null, bool isBillingMember = true)
        {
            var memberId = isBillingMember ? invoice.BillingMemberId : invoice.BilledMemberId;

            if (isBillingMember && (invoice.BillingMember == null || invoice.BillingMemberId == 0))
            {
                return true;
            }

            if (!isBillingMember && (invoice.BilledMember == null || invoice.BilledMemberId == 0))
            {
                return true;
            }

            var passengerConfiguration = passengerConfig ?? MemberManager.GetPassengerConfiguration(memberId);

            if (passengerConfiguration == null)
            {
                return false;
            }

            if (rejectionMemoRecord.YourInvoiceBillingMonth == 0 || rejectionMemoRecord.YourInvoiceBillingMonth > 12 || rejectionMemoRecord.YourInvoiceBillingYear == 0)
            {
                Logger.InfoFormat("Invalid your invoice billing month or year");
                return false;
            }

            //SCP#48125 : RM Linking
            var billingPeriod = new BillingPeriod(rejectionMemoRecord.YourInvoiceBillingYear, rejectionMemoRecord.YourInvoiceBillingMonth, rejectionMemoRecord.YourInvoiceBillingPeriod);
            
           //var billingPeriod = new BillingPeriod(invoice.BillingYear, invoice.BillingMonth, 1);

            DateTime? minumOfMigrationDate = null;

            switch (transactionType)
            {
                case TransactionType.Coupon:

                    if (passengerConfiguration.NonSamplePrimeBillingIsIdecMigratedDate.HasValue && passengerConfiguration.NonSamplePrimeBillingIsIdecMigrationStatus == MigrationStatus.Certified)
                    {
                        minumOfMigrationDate = passengerConfiguration.NonSamplePrimeBillingIsIdecMigratedDate;
                    }

                    if (passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate.HasValue && passengerConfiguration.NonSamplePrimeBillingIsxmlMigrationStatus == MigrationStatus.Certified)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate;
                        }

                    }

                    if (passengerConfiguration.NonSamplePrimeBillingIswebMigratedDate.HasValue)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSamplePrimeBillingIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSamplePrimeBillingIswebMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.NonSamplePrimeBillingIswebMigratedDate;
                        }
                    }

                    if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0 && minumOfMigrationDate.Value.Day != 0)
                    {
                      var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, minumOfMigrationDate.Value.Day);
                        return (billingPeriod >= isMigrationPeriod);
                    }
                    return false;
                    break;

                case TransactionType.RejectionMemo1:

                    if (passengerConfiguration.NonSampleRmIsIdecMigratedDate.HasValue && passengerConfiguration.NonSampleRmIsIdecMigrationStatus == MigrationStatus.Certified)
                    {
                        minumOfMigrationDate = passengerConfiguration.NonSampleRmIsIdecMigratedDate;
                    }

                    if (passengerConfiguration.NonSampleRmIsXmlMigratedDate.HasValue && passengerConfiguration.NonSampleRmIsXmlMigrationStatus == MigrationStatus.Certified)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSampleRmIsXmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSampleRmIsXmlMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate;


                        }

                    }

                    if (passengerConfiguration.NonSampleRmIswebMigratedDate.HasValue)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSampleRmIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSampleRmIswebMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.NonSampleRmIswebMigratedDate;
                        }
                    }


                    if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0 && minumOfMigrationDate.Value.Day != 0)
                    {
                      var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, minumOfMigrationDate.Value.Day);
                        return (billingPeriod >= isMigrationPeriod);
                    }
                    return false;
                    break;
                case TransactionType.BillingMemo:

                    if (passengerConfiguration.NonSampleBmIsIdecMigratedDate.HasValue && passengerConfiguration.NonSampleBmIsIdecMigrationStatus == MigrationStatus.Certified)
                    {
                        minumOfMigrationDate = passengerConfiguration.NonSampleBmIsIdecMigratedDate;
                    }

                    if (passengerConfiguration.NonSampleBmIsXmlMigratedDate.HasValue && passengerConfiguration.NonSamplePrimeBillingIsxmlMigrationStatus == MigrationStatus.Certified)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSampleBmIsXmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSampleBmIsXmlMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.NonSampleBmIsXmlMigratedDate;
                        }

                    }
                    if (passengerConfiguration.NonSampleBmIswebMigratedDate.HasValue)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSampleBmIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSampleBmIswebMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.NonSampleBmIswebMigratedDate;
                        }
                    }

                    if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0 && minumOfMigrationDate.Value.Day != 0)
                    {
                      var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, minumOfMigrationDate.Value.Day);
                        return (billingPeriod >= isMigrationPeriod);
                    }
                    return false;

                    break;
                case TransactionType.CreditMemo:

                    if (passengerConfiguration.NonSampleCmIsIdecMigratedDate.HasValue && passengerConfiguration.NonSampleCmIsIdecMigrationStatus == MigrationStatus.Certified)
                    {
                        minumOfMigrationDate = passengerConfiguration.NonSampleCmIsIdecMigratedDate;
                    }

                    if (passengerConfiguration.NonSampleCmIsXmlMigratedDate.HasValue && passengerConfiguration.NonSampleCmIsXmlMigrationStatus == MigrationStatus.Certified)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSampleCmIsXmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSampleCmIsXmlMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.NonSampleCmIsXmlMigratedDate;
                        }

                    }

                    if (passengerConfiguration.NonSampleCmIswebMigratedDate.HasValue)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.NonSampleCmIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.NonSampleCmIswebMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.NonSampleCmIswebMigratedDate;
                        }
                    }

                    if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0 && minumOfMigrationDate.Value.Day != 0)
                    {
                      var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, minumOfMigrationDate.Value.Day);
                        return (billingPeriod >= isMigrationPeriod);
                    }
                    return false;

                    break;

                case TransactionType.SamplingFormAB:


                    if (passengerConfiguration.SamplingProvIsIdecMigratedDate.HasValue && passengerConfiguration.SamplingProvIsIdecMigrationStatus == MigrationStatus.Certified)
                    {
                        minumOfMigrationDate = passengerConfiguration.SamplingProvIsIdecMigratedDate;
                    }

                    if (passengerConfiguration.SamplingProvIsxmlMigratedDate.HasValue && passengerConfiguration.SamplingProvIsxmlMigrationStatus == MigrationStatus.Certified)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SamplingProvIsxmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.SamplingProvIsxmlMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.SamplingProvIsxmlMigratedDate;

                        }

                    }


                    if (passengerConfiguration.SamplingProvIswebMigratedDate.HasValue)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SamplingProvIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.SamplingProvIswebMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.SamplingProvIswebMigratedDate;
                        }
                    }

                    if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0)
                    {
                        var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, 1);
                        return (billingPeriod >= isMigrationPeriod);
                    }
                    return false;
                    break;

                case TransactionType.SamplingFormC:

                    if (passengerConfiguration.SampleFormCIsIdecMigratedDate.HasValue && passengerConfiguration.SampleFormCIsIdecMigrationStatus == MigrationStatus.Certified)
                    {
                        minumOfMigrationDate = passengerConfiguration.SampleFormCIsIdecMigratedDate;
                    }

                    if (passengerConfiguration.SampleFormCIsxmlMigratedDate.HasValue && passengerConfiguration.SampleFormCIsxmlMigrationStatus == MigrationStatus.Certified)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SampleFormCIsxmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.SampleFormCIsxmlMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.SampleFormCIsxmlMigratedDate;
                        }

                    }

                    if (passengerConfiguration.SampleFormCIswebMigratedDate.HasValue)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SampleFormCIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.SampleFormCIswebMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.SampleFormCIswebMigratedDate;
                        }
                    }
                    if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0)
                    {
                        var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, 1);
                        return (billingPeriod >= isMigrationPeriod);
                    }
                    return false;
                    break;
                case TransactionType.SamplingFormD:

                    if (passengerConfiguration.SampleFormDeIsIdecMigratedDate.HasValue && passengerConfiguration.SampleFormDeIsIdecMigrationStatus == MigrationStatus.Certified)
                    {
                        minumOfMigrationDate = passengerConfiguration.SampleFormDeIsIdecMigratedDate;
                    }


                    if (passengerConfiguration.SampleFormDeIsxmlMigratedDate.HasValue && passengerConfiguration.SampleFormDEisxmlMigrationStatus == MigrationStatus.Certified)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SampleFormDeIsxmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.SampleFormDeIsxmlMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.SampleFormDeIsxmlMigratedDate;

                        }

                    }

                    if (passengerConfiguration.SampleFormDeIswebMigratedDate.HasValue)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SampleFormDeIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.SampleFormDeIswebMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.SampleFormDeIswebMigratedDate;



                        }
                    }

                    if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0 && minumOfMigrationDate.Value.Day != 0)
                    {
                      var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, minumOfMigrationDate.Value.Day);
                        return (billingPeriod >= isMigrationPeriod);
                    }
                    return false;
                    break;
                case TransactionType.SamplingFormF:

                    if (passengerConfiguration.SampleFormFxfIsIdecMigratedDate.HasValue && passengerConfiguration.SampleFormFxfIsIdecMigrationStatus == MigrationStatus.Certified)
                    {
                        minumOfMigrationDate = passengerConfiguration.SampleFormFxfIsIdecMigratedDate;
                    }

                    if (passengerConfiguration.SampleFormFxfIsxmlMigratedDate.HasValue && passengerConfiguration.SampleFormFxfIsxmlMigratedStatus == MigrationStatus.Certified)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SampleFormFxfIsxmlMigratedDate ? minumOfMigrationDate : passengerConfiguration.SampleFormFxfIsxmlMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.SampleFormFxfIsxmlMigratedDate;

                        }

                    }
                    if (passengerConfiguration.SampleFormFxfIswebMigratedDate.HasValue)
                    {
                        if (minumOfMigrationDate.HasValue)
                            minumOfMigrationDate = minumOfMigrationDate < passengerConfiguration.SampleFormFxfIswebMigratedDate ? minumOfMigrationDate : passengerConfiguration.SampleFormFxfIswebMigratedDate;
                        else
                        {
                            minumOfMigrationDate = passengerConfiguration.SampleFormFxfIswebMigratedDate;
                        }
                    }

                    if (minumOfMigrationDate.HasValue && minumOfMigrationDate.Value.Year != 0 && minumOfMigrationDate.Value.Month != 0 && minumOfMigrationDate.Value.Day != 0)
                    {
                      var isMigrationPeriod = new BillingPeriod(minumOfMigrationDate.Value.Year, minumOfMigrationDate.Value.Month, minumOfMigrationDate.Value.Day);
                        return (billingPeriod >= isMigrationPeriod);
                    }
                    return false;
                    break;
            }

            return false;
        }

        /// <summary>
        /// Determines whether billed member migrated for specified billing month and period in invoice header.
        /// </summary>
        /// <param name="invoice">The invoice.</param>
        /// <param name="transactionType">Type of the transaction.</param>
        /// <param name="isXmlFileType">if set to true [is XML file type].</param>
        /// <param name="passengerConfig"></param>
        /// <param name="isBillingMember">if set to true [is billing member].</param>
        /// <returns>
        /// True if member migrated for the specified invoice header; otherwise, false.
        /// </returns>
        public bool IsMemberMigratedForTransaction(InvoiceBase invoice, TransactionType transactionType, bool isXmlFileType, PassengerConfiguration passengerConfig = null, bool isBillingMember = true)
        {
            var memberId = isBillingMember ? invoice.BillingMemberId : invoice.BilledMemberId;
            if (isBillingMember && (invoice.BillingMember == null || invoice.BillingMemberId == 0))
            {
                return true;
            }

            if (!isBillingMember && (invoice.BilledMember == null || invoice.BilledMemberId == 0))
            {
                return true;
            }

            var passengerConfiguration = passengerConfig ?? MemberManager.GetPassengerConfiguration(memberId);

            if (passengerConfiguration == null)
            {
                return false;
            }

            var billingPeriod = new BillingPeriod(invoice.BillingYear, invoice.BillingMonth, 1);
            // If User migrated for both IS-Xml and IS-IDEC.

            //if (invoice.SubmissionMethodId == (int)SubmissionMethod.IsIdec)
            //{

            switch (transactionType)
            {
                case TransactionType.Coupon:
                    if (isXmlFileType)
                    {
                        if (passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate.HasValue && passengerConfiguration.NonSamplePrimeBillingIsxmlMigrationStatus == MigrationStatus.Certified)
                        {
                          if (passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate.Value.Year != 0 && passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate.Value.Month != 0)
                            {
                                var isIdecMigrationPeriod = new BillingPeriod(passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate.Value.Year,
                                                                            passengerConfiguration.NonSamplePrimeBillingIsxmlMigratedDate.Value.Month,
                                                                            1);
                                return (billingPeriod >= isIdecMigrationPeriod);
                            }
                            return false;
                        }
                    }
                    else
                    {
                        if (passengerConfiguration.NonSamplePrimeBillingIsIdecMigratedDate.HasValue && passengerConfiguration.NonSamplePrimeBillingIsIdecMigrationStatus == MigrationStatus.Certified)
                        {
                          if (passengerConfiguration.NonSamplePrimeBillingIsIdecMigratedDate.Value.Year != 0 && passengerConfiguration.NonSamplePrimeBillingIsIdecMigratedDate.Value.Month != 0)
                            {
                              var isIdecMigrationPeriod = new BillingPeriod(passengerConfiguration.NonSamplePrimeBillingIsIdecMigratedDate.Value.Year, passengerConfiguration.NonSamplePrimeBillingIsIdecMigratedDate.Value.Month, 1);
                                return (billingPeriod >= isIdecMigrationPeriod);
                            }
                            return false;
                        }
                    }
                    break;
                case TransactionType.RejectionMemo1:
                    if (isXmlFileType)
                    {
                        if (passengerConfiguration.NonSampleRmIsXmlMigratedDate.HasValue && passengerConfiguration.NonSampleRmIsXmlMigrationStatus == MigrationStatus.Certified)
                        {
                            if (passengerConfiguration.NonSampleRmIsXmlMigratedDate.Value.Year != 0 && passengerConfiguration.NonSampleRmIsXmlMigratedDate.Value.Month != 0)
                            {
                                var isIdecMigrationPeriod = new BillingPeriod(passengerConfiguration.NonSampleRmIsXmlMigratedDate.Value.Year, passengerConfiguration.NonSampleRmIsXmlMigratedDate.Value.Month, 1);
                                return (billingPeriod >= isIdecMigrationPeriod);
                            }
                            return false;
                        }
                    }
                    else
                    {
                        if (passengerConfiguration.NonSampleRmIsIdecMigratedDate.HasValue && passengerConfiguration.NonSampleRmIsIdecMigrationStatus == MigrationStatus.Certified)
                        {
                            if (passengerConfiguration.NonSampleRmIsIdecMigratedDate.Value.Year != 0 && passengerConfiguration.NonSampleRmIsIdecMigratedDate.Value.Month != 0)
                            {
                                var isIdecMigrationPeriod = new BillingPeriod(passengerConfiguration.NonSampleRmIsIdecMigratedDate.Value.Year, passengerConfiguration.NonSampleRmIsIdecMigratedDate.Value.Month, 1);
                                return (billingPeriod >= isIdecMigrationPeriod);
                            }
                            return false;
                        }
                    }
                    break;
                case TransactionType.BillingMemo:
                    if (isXmlFileType)
                    {
                        if (passengerConfiguration.NonSampleBmIsXmlMigratedDate.HasValue && passengerConfiguration.NonSampleBmIsXmlMigrationStatus == MigrationStatus.Certified)
                        {
                            if (passengerConfiguration.NonSampleBmIsXmlMigratedDate.Value.Year != 0 && passengerConfiguration.NonSampleBmIsXmlMigratedDate.Value.Month != 0)
                            {
                                var isIdecMigrationPeriod = new BillingPeriod(passengerConfiguration.NonSampleBmIsXmlMigratedDate.Value.Year, passengerConfiguration.NonSampleBmIsXmlMigratedDate.Value.Month, 1);
                                return (billingPeriod >= isIdecMigrationPeriod);
                            }
                            return false;
                        }
                    }
                    else
                    {
                        if (passengerConfiguration.NonSampleBmIsIdecMigratedDate.HasValue && passengerConfiguration.NonSampleBmIsIdecMigrationStatus == MigrationStatus.Certified)
                        {
                            if (passengerConfiguration.NonSampleBmIsIdecMigratedDate.Value.Year != 0 && passengerConfiguration.NonSampleBmIsIdecMigratedDate.Value.Month != 0)
                            {
                                var isIdecMigrationPeriod = new BillingPeriod(passengerConfiguration.NonSampleBmIsIdecMigratedDate.Value.Year, passengerConfiguration.NonSampleBmIsIdecMigratedDate.Value.Month, 1);
                                return (billingPeriod >= isIdecMigrationPeriod);
                            }
                            return false;
                        }
                    }
                    break;
                case TransactionType.CreditMemo:
                    if (isXmlFileType)
                    {
                        if (passengerConfiguration.NonSampleCmIsXmlMigratedDate.HasValue && passengerConfiguration.NonSampleCmIsXmlMigrationStatus == MigrationStatus.Certified)
                        {
                            if (passengerConfiguration.NonSampleCmIsXmlMigratedDate.Value.Year != 0 && passengerConfiguration.NonSampleCmIsXmlMigratedDate.Value.Month != 0)
                            {
                                var isIdecMigrationPeriod = new BillingPeriod(passengerConfiguration.NonSampleCmIsXmlMigratedDate.Value.Year, passengerConfiguration.NonSampleCmIsXmlMigratedDate.Value.Month, 1);
                                return (billingPeriod >= isIdecMigrationPeriod);
                            }
                            return false;
                        }
                    }
                    else
                    {
                        if (passengerConfiguration.NonSampleCmIsIdecMigratedDate.HasValue && passengerConfiguration.NonSampleCmIsIdecMigrationStatus == MigrationStatus.Certified)
                        {
                            if (passengerConfiguration.NonSampleCmIsIdecMigratedDate.Value.Year != 0 && passengerConfiguration.NonSampleCmIsIdecMigratedDate.Value.Month != 0)
                            {
                                var isIdecMigrationPeriod = new BillingPeriod(passengerConfiguration.NonSampleCmIsIdecMigratedDate.Value.Year, passengerConfiguration.NonSampleCmIsIdecMigratedDate.Value.Month, 1);
                                return (billingPeriod >= isIdecMigrationPeriod);
                            }
                            return false;
                        }
                    }
                    break;
                case TransactionType.SamplingFormAB:
                    if (isXmlFileType)
                    {
                        if (passengerConfiguration.SamplingProvIsxmlMigratedDate.HasValue && passengerConfiguration.SamplingProvIsxmlMigrationStatus == MigrationStatus.Certified)
                        {
                            if (passengerConfiguration.SamplingProvIsxmlMigratedDate.Value.Year != 0 && passengerConfiguration.SamplingProvIsxmlMigratedDate.Value.Month != 0)
                            {
                                var isIdecMigrationPeriod = new BillingPeriod(passengerConfiguration.SamplingProvIsxmlMigratedDate.Value.Year, passengerConfiguration.SamplingProvIsxmlMigratedDate.Value.Month, 1);
                                return (billingPeriod >= isIdecMigrationPeriod);
                            }
                            return false;
                        }
                    }
                    else
                    {
                        if (passengerConfiguration.SamplingProvIsIdecMigratedDate.HasValue && passengerConfiguration.SamplingProvIsIdecMigrationStatus == MigrationStatus.Certified)
                        {
                            if (passengerConfiguration.SamplingProvIsIdecMigratedDate.Value.Year != 0 && passengerConfiguration.SamplingProvIsIdecMigratedDate.Value.Month != 0)
                            {
                                var isIdecMigrationPeriod = new BillingPeriod(passengerConfiguration.SamplingProvIsIdecMigratedDate.Value.Year, passengerConfiguration.SamplingProvIsIdecMigratedDate.Value.Month, 1);
                                return (billingPeriod >= isIdecMigrationPeriod);
                            }
                            return false;
                        }
                    }

                    break;
                case TransactionType.SamplingFormC:
                    if (isXmlFileType)
                    {
                        if (passengerConfiguration.SampleFormCIsxmlMigratedDate.HasValue && passengerConfiguration.SampleFormCIsxmlMigrationStatus == MigrationStatus.Certified)
                        {
                            if (passengerConfiguration.SampleFormCIsxmlMigratedDate.Value.Year != 0 && passengerConfiguration.SampleFormCIsxmlMigratedDate.Value.Month != 0)
                            {
                                var isIdecMigrationPeriod = new BillingPeriod(passengerConfiguration.SampleFormCIsxmlMigratedDate.Value.Year, passengerConfiguration.SampleFormCIsxmlMigratedDate.Value.Month, 1);
                                return (billingPeriod >= isIdecMigrationPeriod);
                            }
                            return false;
                        }
                    }
                    else
                    {
                        if (passengerConfiguration.SampleFormCIsIdecMigratedDate.HasValue && passengerConfiguration.SampleFormCIsIdecMigrationStatus == MigrationStatus.Certified)
                        {
                            if (passengerConfiguration.SampleFormCIsIdecMigratedDate.Value.Year != 0 && passengerConfiguration.SampleFormCIsIdecMigratedDate.Value.Month != 0)
                            {
                                var isIdecMigrationPeriod = new BillingPeriod(passengerConfiguration.SampleFormCIsIdecMigratedDate.Value.Year, passengerConfiguration.SampleFormCIsIdecMigratedDate.Value.Month, 1);
                                return (billingPeriod >= isIdecMigrationPeriod);
                            }
                            return false;
                        }
                    }
                    break;
                case TransactionType.SamplingFormD:
                    if (isXmlFileType)
                    {
                        if (passengerConfiguration.SampleFormDeIsxmlMigratedDate.HasValue && passengerConfiguration.SampleFormDEisxmlMigrationStatus == MigrationStatus.Certified)
                        {
                            if (passengerConfiguration.SampleFormDeIsxmlMigratedDate.Value.Year != 0 && passengerConfiguration.SampleFormDeIsxmlMigratedDate.Value.Month != 0)
                            {
                                var isIdecMigrationPeriod = new BillingPeriod(passengerConfiguration.SampleFormDeIsxmlMigratedDate.Value.Year, passengerConfiguration.SampleFormDeIsxmlMigratedDate.Value.Month, 1);
                                return (billingPeriod >= isIdecMigrationPeriod);
                            }
                            return false;
                        }
                    }
                    else
                    {
                        if (passengerConfiguration.SampleFormDeIsIdecMigratedDate.HasValue && passengerConfiguration.SampleFormDeIsIdecMigrationStatus == MigrationStatus.Certified)
                        {
                            if (passengerConfiguration.SampleFormDeIsIdecMigratedDate.Value.Year != 0 && passengerConfiguration.SampleFormDeIsIdecMigratedDate.Value.Month != 0)
                            {
                                var isIdecMigrationPeriod = new BillingPeriod(passengerConfiguration.SampleFormDeIsIdecMigratedDate.Value.Year, passengerConfiguration.SampleFormDeIsIdecMigratedDate.Value.Month, 1);
                                return (billingPeriod >= isIdecMigrationPeriod);
                            }
                            return false;
                        }
                    }
                    break;
                case TransactionType.SamplingFormF:
                    if (isXmlFileType)
                    {
                        if (passengerConfiguration.SampleFormFxfIsxmlMigratedDate.HasValue && passengerConfiguration.SampleFormFxfIsxmlMigratedStatus == MigrationStatus.Certified)
                        {
                            if (passengerConfiguration.SampleFormFxfIsxmlMigratedDate.Value.Year != 0 && passengerConfiguration.SampleFormFxfIsxmlMigratedDate.Value.Month != 0)
                            {
                                var isIdecMigrationPeriod = new BillingPeriod(passengerConfiguration.SampleFormFxfIsxmlMigratedDate.Value.Year, passengerConfiguration.SampleFormFxfIsxmlMigratedDate.Value.Month, 1);
                                return (billingPeriod >= isIdecMigrationPeriod);
                            }
                            return false;
                        }
                    }
                    else
                    {
                        if (passengerConfiguration.SampleFormFxfIsIdecMigratedDate.HasValue && passengerConfiguration.SampleFormFxfIsIdecMigrationStatus == MigrationStatus.Certified)
                        {
                            if (passengerConfiguration.SampleFormFxfIsIdecMigratedDate.Value.Year != 0 && passengerConfiguration.SampleFormFxfIsIdecMigratedDate.Value.Month != 0)
                            {
                                var isIdecMigrationPeriod = new BillingPeriod(passengerConfiguration.SampleFormFxfIsIdecMigratedDate.Value.Year, passengerConfiguration.SampleFormFxfIsIdecMigratedDate.Value.Month, 1);
                                return (billingPeriod >= isIdecMigrationPeriod);
                            }
                            return false;
                        }
                    }
                    break;
            }

            return false;
        }

        /// <summary>
        /// To populate default location if reference data and locationId are not provided.
        /// </summary>
        public static bool PopulateDefaultLocation(int memberId, MemberLocationInformation memberLocationInformation, string locationCode, PaxInvoice invoice = null)
        {
            //var location = Ioc.Resolve<IMemberManager>(typeof(IMemberManager)).GetMemberDefaultLocation(memberId, locationCode);

            var locationRepository = Ioc.Resolve<ILocationRepository>(typeof(ILocationRepository));

            // Get location query.
            var locationQuery = locationRepository.Get(memberLocation => memberLocation.MemberId == memberId && memberLocation.LocationCode.ToUpper() == locationCode.ToUpper());

            // Get only required properties of the location.
            var locationList = locationQuery.Select(loc => new
            {
                loc.AdditionalTaxVatRegistrationNumber,
                loc.TaxVatRegistrationNumber,
                loc.LocationCode,
                loc.Country,
                loc.RegistrationId,
                loc.MemberLegalName,
                loc.AddressLine1,
                loc.AddressLine2,
                loc.AddressLine3,
                loc.CityName,
                loc.SubDivisionCode,
                loc.SubDivisionName,
                loc.PostalCode,
                loc.LegalText
            }).ToList();

            if (locationList.Count > 0)
            {
                var location = locationList[0];

                memberLocationInformation.AdditionalTaxVatRegistrationNumber = location.AdditionalTaxVatRegistrationNumber;
                memberLocationInformation.TaxRegistrationId = location.TaxVatRegistrationNumber;
                memberLocationInformation.MemberLocationCode = location.LocationCode;
                memberLocationInformation.CompanyRegistrationId = location.RegistrationId;
                memberLocationInformation.OrganizationName = location.MemberLegalName;
                memberLocationInformation.AddressLine1 = location.AddressLine1;
                memberLocationInformation.AddressLine2 = location.AddressLine2;
                memberLocationInformation.AddressLine3 = location.AddressLine3;
                memberLocationInformation.CityName = location.CityName;
                memberLocationInformation.SubdivisionCode = location.SubDivisionCode;
                memberLocationInformation.SubdivisionName = location.SubDivisionName;
                if (invoice != null && string.IsNullOrEmpty(invoice.LegalText))
                {
                    if (string.IsNullOrEmpty(location.LegalText))
                    {
                        var memberManager = Ioc.Resolve<IMemberManager>(typeof(IMemberManager));
                        var eBillingConfig = memberManager.GetEbillingConfig(memberId);
                        invoice.LegalText = eBillingConfig != null && eBillingConfig.LegalText != null ? eBillingConfig.LegalText.Trim().Replace("\r", "").Replace("\n", "") : string.Empty;
                    }
                    else
                    {
                        invoice.LegalText = location.LegalText.Trim().Replace("\r", "").Replace("\n", "");
                    }

                }
                if (location.Country != null)
                {
                    memberLocationInformation.CountryCode = location.Country.Id;
                    memberLocationInformation.CountryCodeIcao = location.Country.CountryCodeIcao;
                    memberLocationInformation.CountryName = location.Country.Name;
                }
                memberLocationInformation.PostalCode = location.PostalCode;
                return true;
            }

            var memberDetails = Ioc.Resolve<IMemberManager>(typeof(IMemberManager)).GetMemberDetails(memberId);
            if (memberDetails != null)
            {
                memberLocationInformation.OrganizationDesignator = memberDetails.MemberCodeAlpha;
            }
            return false;
        }

        /// <summary>
        /// Validates the settlement method.
        /// </summary>
        /// <param name="invoice">The invoice header.</param>
        /// <param name="billingMember">The billing member.</param>
        /// <param name="billedMember">The billed member.</param>
        /// <returns></returns>
        protected string GetInvoiceClearingHouse(PaxInvoice invoice, Member billingMember, Member billedMember)
        {
            BillingMember = billingMember ?? (BillingMember = MemberManager.GetMember(invoice.BillingMemberId));
            BilledMember = billedMember ?? (BilledMember = MemberManager.GetMember(invoice.BilledMemberId));

            /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
            if (ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false)) return string.Empty;

            // if Billing Member and billed member both are dual member
            if (IsDualMember(BillingMember) && IsDualMember(BilledMember))
            {

                // if exception is not present in any of the member list for other member, then SMI should be A or M.
                if (invoice.InvoiceSmi == SMI.Ach || invoice.InvoiceSmi == SMI.AchUsingIataRules)
                {
                    return string.Empty;
                }

            }
            //// Billing member is Dual.
            else if (IsDualMember(BillingMember))
            {
                // and Billed Member is ICH, then SMI should be ICH.
                if (IsIchMember(BilledMember) && (invoice.InvoiceSmi == SMI.Ich)) return string.Empty;
                // and Billed Member is ACH, then SMI should be ACH or ACH using IATA rules.
                if (IsAchMember(BilledMember))
                {
                    if (invoice.InvoiceSmi == SMI.Ach || invoice.InvoiceSmi == SMI.AchUsingIataRules)
                    {
                        return string.Empty;
                    }
                }
            }

            // Billing member is only ICH, then SMI should be ICH, irrespective of BilledMember status.
            else if (IsIchMember(BillingMember))
            {
                if (invoice.InvoiceSmi == SMI.Ich) return string.Empty;
            }

            // Billing member is only ACH.
            else if (IsAchMember(BillingMember))
            {
                // if Billed Member is dual or only ACH, then SMI should be A or M.
                if (IsDualMember(BilledMember) || IsAchMember(BilledMember))
                {
                    if (invoice.InvoiceSmi == SMI.Ach || invoice.InvoiceSmi == SMI.AchUsingIataRules)
                    {
                        return string.Empty;
                    }
                }
                // if Billed Member is only ICH, then SMI should be M.
                else if (IsIchMember(BilledMember))
                {
                    if (invoice.InvoiceSmi == SMI.AchUsingIataRules) return string.Empty;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Validates the settlement method.
        /// </summary>
        /// <param name="invoice">The invoice header.</param>
        /// <param name="billingMember">The billing member.</param>
        /// <param name="billedMember">The billed member.</param>
        /// <returns></returns>
        protected virtual bool ValidatePaxSettlementMethod(PaxInvoice invoice, Member billingMember, Member billedMember)
        {
            if (invoice.InvoiceTypeId == (int)InvoiceType.Invoice && (invoice.SettlementMethodId != (int)SMI.Ich && invoice.SettlementMethodId != (int)SMI.Ach &&
              invoice.SettlementMethodId != (int)SMI.Bilateral && invoice.SettlementMethodId != (int)SMI.AchUsingIataRules))
            {
                return false;
            }

            if (invoice.InvoiceTypeId == (int)InvoiceType.CreditNote && invoice.SettlementMethodId == (int)SMI.AdjustmentDueToProtest)
            {
                return true;
            }

            BillingMember = billingMember ?? (BillingMember = MemberManager.GetMember(invoice.BillingMemberId));
            BilledMember = billedMember ?? (BilledMember = MemberManager.GetMember(invoice.BilledMemberId));

            if (invoice.InvoiceSmi == SMI.Bilateral) return true;

            // if Billing Member and billed member both are dual member
            if (IsDualMember(BillingMember) && IsDualMember(BilledMember))
            {
                // Check for ACH Exception "Settle through ICH",
                // if exception is present in both member, then SMI should be ICH.
                var achExceptionsBillingMember = MemberManager.GetExceptionMembers(BillingMember.Id, (int)invoice.BillingCategory, false).ToList();
                var achExceptionsBilledMember = MemberManager.GetExceptionMembers(BilledMember.Id, (int)invoice.BillingCategory, false).ToList();
                if (achExceptionsBillingMember.Count(ach => ach.ExceptionMemberId == BilledMember.Id) > 0 && achExceptionsBilledMember.Count(ach => ach.ExceptionMemberId == BillingMember.Id) > 0)
                {
                    return invoice.InvoiceSmi == SMI.Ich;
                }

                // this condition is to check if one member has other member in its exception list but not vice versa.
                if (achExceptionsBillingMember.Count(ach => ach.ExceptionMemberId == BilledMember.Id) > 0 || achExceptionsBilledMember.Count(ach => ach.ExceptionMemberId == BillingMember.Id) > 0)
                {
                    return false;
                }

                // if exception is not present in any of the member list for other member, then SMI should be A or M.
                if (invoice.InvoiceSmi == SMI.Ach || invoice.InvoiceSmi == SMI.AchUsingIataRules)
                {
                    return true;
                }

            }
            //// Billing member is Dual.
            else if (IsDualMember(BillingMember))
            {
                // and Billed Member is ICH, then SMI should be ICH.
                if (IsIchMember(BilledMember) && (invoice.InvoiceSmi == SMI.Ich)) return true;
                // and Billed Member is ACH, then SMI should be ACH or ACH using IATA rules.
                if (IsAchMember(BilledMember))
                {
                    if (invoice.InvoiceSmi == SMI.Ach || invoice.InvoiceSmi == SMI.AchUsingIataRules)
                    {
                        return true;
                    }
                }
            }
            // Billing member is only ICH, then SMI should be ICH, irrespective of BilledMember status.
            else if (IsIchMember(BillingMember))
            {
                if (IsIchMember(BilledMember) || IsAchMember(BilledMember))
                {
                    if (invoice.InvoiceSmi == SMI.Ich) return true;
                }
            }

            // Billing member is only ACH.
            else if (IsAchMember(BillingMember))
            {
                // if Billed Member is dual or only ACH, then SMI should be A or M.
                if (IsDualMember(BilledMember) || IsAchMember(BilledMember))
                {
                    if (invoice.InvoiceSmi == SMI.Ach || invoice.InvoiceSmi == SMI.AchUsingIataRules)
                    {
                        return true;
                    }
                }
                // if Billed Member is only ICH, then SMI should be M.
                else if (IsIchMember(BilledMember))
                {
                    if (invoice.InvoiceSmi == SMI.AchUsingIataRules) return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Validates the agreement indicator supplied.
        /// Other than those defined in validations (I J K W V T S) , all other single letter agreement indicators reserved for future use. All double letter agreement indicators are available for bilateral use.
        /// </summary>
        /// <param name="agreementIndicatorSupplied">The agreement indicator supplied.</param>
        /// <returns></returns>
        public bool ValidateAgreementIndicatorSupplied(string agreementIndicatorSupplied)
        {
            var result = true;
            const string regexAgreementIndicatorSupplied = "^[IJKWVTSijkwvts ]$";
            agreementIndicatorSupplied = agreementIndicatorSupplied.Trim();
            if (agreementIndicatorSupplied.Length == 1)
            {
                var regEx = new Regex(regexAgreementIndicatorSupplied);
                if (!regEx.IsMatch(agreementIndicatorSupplied))
                {
                    result = false;
                }
            }
            // No need to perform validation for 2 letter value.
            return result;
        }


        /// <summary>
        /// Validates the original PMI.
        /// </summary>
        /// <param name="originalPmi">The original PMI.</param>
        /// <param name="agreementIndicatorSupplied">The agreement indicator supplied.</param>
        /// <returns></returns>
        public bool ValidateOriginalPmi(string originalPmi, string agreementIndicatorSupplied)
        {
            var result = true;

            if (agreementIndicatorSupplied.Trim().Length > 1) return true;

            agreementIndicatorSupplied = agreementIndicatorSupplied.Trim();
            //Should be blank if field Agreement Indicator Supplied (Element 56) is blank.
            if (!ValidationCache.Instance.IgnoreValidationOnMigrationPeriod)
            {
                if (string.IsNullOrEmpty(agreementIndicatorSupplied) && !string.IsNullOrEmpty(originalPmi.Trim()))
                {
                    result = false;
                }
            }
            //Should be equal to "N" if field Agreement Indicator Supplied (Element 56) = "I", "J" or "K".
            var regex = new Regex("[I-Ki-k]");
            if (regex.IsMatch(agreementIndicatorSupplied) && !originalPmi.Equals("N", StringComparison.OrdinalIgnoreCase))
            {
                result = false;
            }
            //Should be equal to "O" if  field Agreement Indicator Supplied (Element 56) = "W", "V" or "T".
            regex = new Regex("[TVWtvw]");
            if (regex.IsMatch(agreementIndicatorSupplied) && !originalPmi.Equals("O", StringComparison.OrdinalIgnoreCase))
            {
                result = false;
            }
            //Should be equal to "A", "B", "C" or "D" if field Agreement Indicator Supplied (Element 56) = "S"
            regex = new Regex("[A-Da-d]");
            if (agreementIndicatorSupplied.Equals("S", StringComparison.OrdinalIgnoreCase) && !regex.IsMatch(originalPmi.Trim()))
            {
                result = false;
            }
            return result;
        }

        public TransactionDetails GetRejectedTransactionDetails(string transactionIds)
        {
            string[] transactionIdAndType = transactionIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string memoId = null, couponIds = null;
            string couponGuid = null;
            var rejectedTransactionDetails = new TransactionDetails();
            if (transactionIdAndType[1] == "PC")
            {
                couponIds = transactionIdAndType[0];
                string[] coupons = couponIds.Split(new char[] { ',' });

                foreach (var coupon in coupons)
                {
                    couponGuid = ConvertUtil.ConvertGuidToString(coupon.ToGuid()) + ',' + couponGuid;
                }
            }

            else if (transactionIdAndType[1] == "RM") memoId = ConvertUtil.ConvertGuidToString(transactionIdAndType[0].ToGuid());
            else return rejectedTransactionDetails; // For any other transaction type, validation is not needed.

            rejectedTransactionDetails.Transactions = InvoiceRepository.GetRejectedTransactionDetails(memoId, couponGuid);

            return rejectedTransactionDetails;
        }

        /// <summary>
        /// Gets the duplicate RM coupon count.
        /// </summary>
        /// <param name="rejectionMemoCouponBreakdownRecord">The rejection memo coupon breakdown record.</param>
        /// <param name="isUpdateOperation">if set to true [is update operation].</param>
        /// <param name="invoice">The invoice.</param>
        /// <param name="rejectionMemoRecord">The rejection memo record.</param>
        /// <param name="duplicateErrorMessage">The duplicate error message.</param>
        /// <returns></returns>
        protected string GetDuplicateRMCouponCount(RMCoupon rejectionMemoCouponBreakdownRecord, bool isUpdateOperation, InvoiceBase invoice, RejectionMemo rejectionMemoRecord, string duplicateErrorMessage)
        {

            DateTime billingDate;
            var billingYearToCompare = 0;
            var billingMonthToCompare = 0;

            if (DateTime.TryParse(string.Format("{0}/{1}/{2}", invoice.BillingYear.ToString().PadLeft(2, '0'), invoice.BillingMonth.ToString().PadLeft(2, '0'), "01"), out billingDate))
            {
                var billingDateToCompare = billingDate.AddMonths(-12);
                billingYearToCompare = billingDateToCompare.Year;
                billingMonthToCompare = billingDateToCompare.Month;
            }

            var duplicateCouponCount = RejectionMemoCouponBreakdownRepository.GetRMCouponDuplicateCount(rejectionMemoRecord.RejectionStage,
                                                                                                        rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline,
                                                                                                        rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber,
                                                                                                        rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber,
                                                                                                        invoice.BillingMemberId, invoice.BilledMemberId,
                                                                                                        rejectionMemoRecord.YourInvoiceNumber,
                                                                                                        billingMonthToCompare, billingYearToCompare);

            // if ((isUpdateOperation && duplicateCouponCount > 1) || (!isUpdateOperation && duplicateCouponCount > 0))
            if (duplicateCouponCount > 0)
            {
                duplicateErrorMessage = string.Format(Messages.RejectionMemoCouponDuplicateMessage, duplicateCouponCount);
            }
            return duplicateErrorMessage;
        }

        /// <summary>
        /// Generate the pax Invoces old IDEC file.
        /// </summary>
        public void GeneratePaxOldIdec(BillingPeriod billingPeriod)
        {
            Logger.Info(string.Format("Billing Month Period :Y{0}-M{1}-P{2}", billingPeriod.Year, billingPeriod.Month, billingPeriod.Period));
            if (billingPeriod.Period == 4)
            {
                GeneratePaxOldIdecInternal(billingPeriod);
            }
        }

        /// <summary>
        /// Generate the pax Invoces old IDEC file.
        /// </summary>
        public void GeneratePaxOldIdecInternal(BillingPeriod lastBillingMonthPeriod, int regenerateFlag = 0, int billingMemberId = 0)
        {
            try
            {

                StringBuilder stringBuilderPaxIdecHeaderAndCoupons;
                //Get Invoices & coupons for pericular year and month having Value Confurmation Status other than 1 or 3
                var checkentities = new[] { LoadStrategy.Entities.BillingMember, LoadStrategy.Entities.BilledMember };
                List<PaxInvoice> checkPaxInvoicelist = new List<PaxInvoice>();
                if (regenerateFlag == 0)
                {
                    checkPaxInvoicelist = InvoiceRepository.GetPaxOldIdecInvoiceLS(new LoadStrategy(string.Join(",", checkentities)),
                                                                                   billingYear: lastBillingMonthPeriod.Year,
                                                                                   billingMonth: lastBillingMonthPeriod.Month,
                                                                                   checkValueConfurmation: 1);
                }
                if (checkPaxInvoicelist.Count == 0)
                {
                    if (regenerateFlag == 0)
                    {
                        checkPaxInvoicelist = InvoiceRepository.GetPaxOldIdecInvoiceLS(new LoadStrategy(string.Join(",", checkentities)),
                                                                                       billingYear: lastBillingMonthPeriod.Year,
                                                                                       billingMonth: lastBillingMonthPeriod.Month,
                                                                                       checkValueConfurmation: 2);
                    }
                    if (checkPaxInvoicelist.Count == 0)
                    {
                        var paxBillingMemberlist = new List<PaxOldIdecBillingMember>();
                        if (billingMemberId == 0)
                        {
                            paxBillingMemberlist = InvoiceRepository.GetPaxOldIdecBillingMember(null, lastBillingMonthPeriod.Year, lastBillingMonthPeriod.Month);
                        }
                        else
                        {

                            paxBillingMemberlist.Add(new PaxOldIdecBillingMember { BillingMemberId = billingMemberId });
                        }


                        if (regenerateFlag == 1)
                        {
                            foreach (var paxBillingMember in paxBillingMemberlist)
                            {
                                // Call Function for System Monitor
                                SysMonGeneratePaxOldIdec(Convert.ToInt32(paxBillingMember.BillingMemberId),
                                                         Convert.ToInt32(lastBillingMonthPeriod.Year),
                                                         Convert.ToInt32(lastBillingMonthPeriod.Month),
                                                         Convert.ToInt32(lastBillingMonthPeriod.Period));
                            }
                        }
                        else
                        {

                            foreach (var paxBillingMember in paxBillingMemberlist)
                            {

                                // Call single Instance EXE for Normal Process 
                                using (var p = new Process())
                                {
                                    p.StartInfo.FileName = ConfigurationManager.AppSettings["PaxOldIDECSingleInstancePath"].ToString();
                                    Logger.InfoFormat("Calling Single Instance :" + p.StartInfo.FileName);
                                    p.StartInfo.Arguments = string.Format("{0}-{1}-{2}-{3}",
                                                                          Convert.ToInt32(paxBillingMember.BillingMemberId),
                                                                          Convert.ToInt32(lastBillingMonthPeriod.Year),
                                                                          Convert.ToInt32(lastBillingMonthPeriod.Month),
                                                                          Convert.ToInt32(lastBillingMonthPeriod.Period));
                                    Logger.InfoFormat(string.Format("Single Instance Argument [{0}].", p.StartInfo.Arguments));
                                    p.StartInfo.UseShellExecute = false;
                                    p.StartInfo.RedirectStandardOutput = true;
                                    p.StartInfo.RedirectStandardError = true;
                                    p.StartInfo.RedirectStandardInput = true;
                                    p.StartInfo.CreateNoWindow = true;
                                    p.EnableRaisingEvents = false;
                                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                    p.Start();
                                    Logger.Info("Instace Started.......");
                                    p.WaitForExit();
                                    Logger.Info("Instace waiting.......");
                                    p.Close();
                                    Logger.Info("Instace Closed.......");
                                }


                            }

                        }
                    }
                    else
                    {
                        Logger.Info("Send Email to IS Admin for Future Submission invoices pending.");
                        SendEmailToAdminForFutureSubmission(checkPaxInvoicelist);
                    }
                }
                else
                {
                    Logger.Info("Send Email to IS Admin for Value confurmation.");
                    SendEmailToAdminForBvcpending(checkPaxInvoicelist);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error occurred while generating Old-IDEC Downgrade File.", ex);
                var context = new VelocityContext();
                context.Put("ErrorMessage", ex.Message);
                context.Put("BillingMonth", string.Format("{0}-{1}", lastBillingMonthPeriod.Year, CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(lastBillingMonthPeriod.Month).ToUpper()));
                const string message = "Old-IDEC Downgrade Files Generation Failed.";
                const string title = "Old-IDEC Downgrade Files Generation Failed.";
                var issisOpsAlert = new ISSISOpsAlert
                {
                    Message = String.Format(message),
                    AlertDateTime = DateTime.UtcNow,
                    IsActive = true,
                    EmailAddress = SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail,
                    Title = title
                };
                BroadcastMessagesManager.AddAlert(issisOpsAlert, EmailTemplateId.ISAdminOldIdecFailureNotification, context);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkPaxInvoicelist"></param>
        private void SendEmailToAdminForBvcpending(List<PaxInvoice> checkPaxInvoicelist)
        {
            //Alert and Email to IS Admin
            StringBuilder pendingInvoice = new StringBuilder();
            pendingInvoice.Append("<table border=1><tr><td>Billing Airline</td><td>Billed Airline</td><td>   Billing Year/Month/Period</td><td>Invoice Number</td></tr>");
            foreach (var inv in checkPaxInvoicelist)
            {
                pendingInvoice.Append("<tr>");
                pendingInvoice.Append("<td>" + string.Format("{0}/{1}", inv.BillingMember.MemberCodeAlpha, inv.BillingMember.MemberCodeNumeric) + "</td>");
                pendingInvoice.Append("<td>" + string.Format("{0}/{1}", inv.BilledMember.MemberCodeAlpha, inv.BilledMember.MemberCodeNumeric) + "</td>");
                pendingInvoice.Append("<td>" + string.Format("{0}/{1}/{2}", inv.BillingYear, inv.BillingMonth, inv.BillingPeriod) + "</td>");
                pendingInvoice.Append("<td>" + inv.InvoiceNumber + "</td>");
                pendingInvoice.Append("</tr>");
            }
            pendingInvoice.Append("</table>");
            var context = new VelocityContext();
            context.Put("Invoices", pendingInvoice);
            const string message = "Passenger Old-IDEC downgrade process was aborted.";
            const string title = "Passenger Old-IDEC downgraded files generation process was aborted.";
            var issisOpsAlert = new ISSISOpsAlert
                                    {
                                        Message = String.Format(message),
                                        AlertDateTime = DateTime.UtcNow,
                                        IsActive = true,
                                        EmailAddress = SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail,
                                        Title = title
                                    };
            BroadcastMessagesManager.AddAlert(issisOpsAlert, EmailTemplateId.ISAdminPendingBVCProcessesAlert, context);
            Logger.Info("Email send to IS Admin:" + SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkPaxInvoicelist"></param>
        private void SendEmailToAdminForFutureSubmission(List<PaxInvoice> checkPaxInvoicelist)
        {
            //Alert and Email to IS Admin
            StringBuilder pendingInvoice = new StringBuilder();
            pendingInvoice.Append("<table border=1><tr><td>Billing Airline</td><td>Billed Airline</td><td>   Billing Year/Month/Period</td><td>Invoice Number</td></tr>");
            foreach (var inv in checkPaxInvoicelist)
            {
                pendingInvoice.Append("<tr >");
                pendingInvoice.Append("<td>" + string.Format("{0}/{1}", inv.BillingMember.MemberCodeAlpha, inv.BillingMember.MemberCodeNumeric) + "</td>");
                pendingInvoice.Append("<td>" + string.Format("{0}/{1}", inv.BilledMember.MemberCodeAlpha, inv.BilledMember.MemberCodeNumeric) + "</td>");
                pendingInvoice.Append("<td>" + string.Format("{0}/{1}/{2}", inv.BillingYear, inv.BillingMonth, inv.BillingPeriod) + "</td>");
                pendingInvoice.Append("<td>" + inv.InvoiceNumber + "</td>");
                pendingInvoice.Append("</tr>");
            }
            pendingInvoice.Append("</table>");
            var context = new VelocityContext();
            context.Put("Invoices", pendingInvoice);
            const string message = "Passenger Old-IDEC downgrade process was aborted.";
            const string title = "Passenger Old-IDEC downgraded files generation process was aborted.";
            var issisOpsAlert = new ISSISOpsAlert
            {
                Message = String.Format(message),
                AlertDateTime = DateTime.UtcNow,
                IsActive = true,
                EmailAddress = SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail,
                Title = title
            };
            BroadcastMessagesManager.AddAlert(issisOpsAlert, EmailTemplateId.ISAdminPendingFutureSubmissionAlert, context);
            Logger.Info("Email send to IS Admin:" + SystemParameters.Instance.SIS_OpsDetails.SisOpsEmail);

        }

        public void SysMonGeneratePaxOldIdec(int billingMemberId, int year, int month, int period)
        {
            try
            {
                var stringBuilderPaxIdecHeaderAndCoupons = new StringBuilder();
                long recordSequenceNumber = 0;
                decimal exchangeRate = 0;
                double fileTotalGrossValue = 0;
                double fileTotalInterlineServiceCharge = 0;
                double fileTotalTax = 0;
                int fileTotalNoCoupons = 0;
                double fileNetTotal = 0;
                double fileTotalHandlingFeeAmount = 0;
                double fileNetBillingAmount = 0;
                //Billing Member
                string billingMemberCodeNumeric = string.Empty;
                string billingMemberCodeAlpha = string.Empty;


                //Get all invoices according to billing Member
                //  var memberPaxInvoicelist = paxInvoicelist.Where(i => i.BillingMemberId == paxBillingMember.BillingMemberId).OrderBy(i => i.BilledMember.MemberCodeNumeric).ThenBy(i => i.BillingPeriod).ThenBy(i => i.InvoiceNumber).ToList();

                Ioc.Initialize();
                var InvoiceRepository = Ioc.Resolve<IInvoiceRepository>(typeof(IInvoiceRepository));

                //Vinod 
                var couponEntities = new[] { LoadStrategy.Entities.Coupon, LoadStrategy.Entities.InvoiceTotal, LoadStrategy.Entities.BillingMember, LoadStrategy.Entities.BilledMember };
                var memberPaxInvoicelist = InvoiceRepository.GetPaxOldIdecInvoiceLS(new LoadStrategy(string.Join(",", couponEntities)),
                                                                                    billingYear: year,
                                                                                    billingMonth: month,
                                                                                    billingMemberId: billingMemberId);
                Logger.Info("Invoices found : " + memberPaxInvoicelist.Count);
                if (memberPaxInvoicelist.Count > 0)
                {
                  Logger.InfoFormat("Writing {0} Invoices in to file.", memberPaxInvoicelist.Count());

                    foreach (PaxInvoice invoice in memberPaxInvoicelist)
                    {
                        billingMemberCodeAlpha = invoice.BillingMember.MemberCodeAlpha;
                        billingMemberCodeNumeric = invoice.BillingMember.MemberCodeNumeric;
                        var billingMember = invoice.BillingMember;
                        var billedMember = invoice.BilledMember;

                        //Billed member numerice code start with 'S' then member code will be '000'
                        if (billedMember.MemberCodeNumeric.StartsWith("S"))
                        {
                            billedMember.MemberCodeNumeric = "000";
                        }
                        else
                        {
                            var firstChar = billedMember.MemberCodeNumeric.Substring(0, 1);
                            var remainigChars = billedMember.MemberCodeNumeric.Substring(1);

                            //if First character of mrmber code is 'A,B....Z' then it will be replaced by '10,11.....35'
                            switch (firstChar)
                            {
                                case "A":
                                    firstChar = "10";
                                    break;
                                case "B":
                                    firstChar = "11";
                                    break;
                                case "C":
                                    firstChar = "12";
                                    break;
                                case "D":
                                    firstChar = "13";
                                    break;
                                case "E":
                                    firstChar = "14";
                                    break;
                                case "F":
                                    firstChar = "15";
                                    break;
                                case "G":
                                    firstChar = "16";
                                    break;
                                case "H":
                                    firstChar = "17";
                                    break;
                                case "I":
                                    firstChar = "18";
                                    break;
                                case "J":
                                    firstChar = "19";
                                    break;
                                case "K":
                                    firstChar = "20";
                                    break;
                                case "L":
                                    firstChar = "21";
                                    break;
                                case "M":
                                    firstChar = "22";
                                    break;
                                case "N":
                                    firstChar = "23";
                                    break;
                                case "O":
                                    firstChar = "24";
                                    break;
                                case "P":
                                    firstChar = "25";
                                    break;
                                case "Q":
                                    firstChar = "26";
                                    break;
                                case "R":
                                    firstChar = "27";
                                    break;
                                case "S":
                                    firstChar = "28";
                                    break;
                                case "T":
                                    firstChar = "29";
                                    break;
                                case "U":
                                    firstChar = "30";
                                    break;
                                case "V":
                                    firstChar = "31";
                                    break;
                                case "W":
                                    firstChar = "32";
                                    break;
                                case "X":
                                    firstChar = "33";
                                    break;
                                case "Y":
                                    firstChar = "34";
                                    break;
                                case "Z":
                                    firstChar = "35";
                                    break;

                            }
                            billedMember.MemberCodeNumeric = firstChar + remainigChars;
                        }

                        #region Invoice Header Details

                        recordSequenceNumber++;

                        //Standerd Messege Identifier Always 'PBD' For Passenger invoices
                        stringBuilderPaxIdecHeaderAndCoupons.Append("PBD");

                        //Record Sequence 8N
                        stringBuilderPaxIdecHeaderAndCoupons.Append(recordSequenceNumber.ToString().PadLeft(8, '0'));

                        //Standerd Field identifier '10' for header details.
                        stringBuilderPaxIdecHeaderAndCoupons.Append("10");

                        //Billing Airline code 
                        stringBuilderPaxIdecHeaderAndCoupons.Append((billingMember.MemberCodeNumeric).PadLeft(4, '0'));

                        //Billed Airline code 
                        stringBuilderPaxIdecHeaderAndCoupons.Append((billedMember.MemberCodeNumeric).PadLeft(4, '0'));

                        //Billing Code
                        stringBuilderPaxIdecHeaderAndCoupons.Append(invoice.BillingCode.ToString().PadLeft(1, '0'));

                        //Invoice Number
                        stringBuilderPaxIdecHeaderAndCoupons.Append(invoice.InvoiceNumber.PadRight(14, ' '));

                        //Page/batch Sequence Number '00000' for header
                        stringBuilderPaxIdecHeaderAndCoupons.Append("00000");

                        //Record Sequence within Page/batch  Number '00000' for header
                        stringBuilderPaxIdecHeaderAndCoupons.Append("00000");

                        //Billing Date in 'YYMM00' format
                        stringBuilderPaxIdecHeaderAndCoupons.Append(Convert.ToString(invoice.BillingYear).Substring(2, 2).PadLeft(2, '0') + Convert.ToString(invoice.BillingMonth.ToString().PadLeft(2, '0')) +
                                                                    "00");

                        //Listing Currency
                        stringBuilderPaxIdecHeaderAndCoupons.Append(Convert.ToString(invoice.ListingCurrencyId).PadLeft(3, '0'));

                        //Billing Currency
                        stringBuilderPaxIdecHeaderAndCoupons.Append(Convert.ToString(invoice.BillingCurrencyId).PadLeft(3, '0'));

                        //Currency Adjustment method 'A2' for header
                        stringBuilderPaxIdecHeaderAndCoupons.Append("A2");

                        //Rate of exchange  (exchange rate converted into multiplicative factor)
                        // In IS format, this is represented as a dividing factor. 
                        // In Old-IDEC, it should be a multiplicative factor.
                        // Formula = 1/ Listing/Evaluation to Billing Rate; rounded (closest) to 5 decimal places.
                        //CMP#648: Convert Exchange rate into nullable field.
                        if (invoice.ExchangeRate.HasValue && invoice.ExchangeRate.Value > 0)
                        {
                            exchangeRate = ConvertUtil.Round((1 / invoice.ExchangeRate.Value), 5);
                            stringBuilderPaxIdecHeaderAndCoupons.Append(
                                Math.Abs(exchangeRate).ToString("N5").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                    CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(8, '0'));
                        }
                        else
                        {
                            stringBuilderPaxIdecHeaderAndCoupons.Append(("0").PadLeft(8, '0'));
                        }

                        //currency Conversion Indicator '9' to indicate down conversion
                        stringBuilderPaxIdecHeaderAndCoupons.Append("9");

                        //Period Number 
                        stringBuilderPaxIdecHeaderAndCoupons.Append(invoice.BillingPeriod.ToString().PadLeft(2, '0'));

                        //filler
                        stringBuilderPaxIdecHeaderAndCoupons.Append(String.Empty.PadLeft(89));

                        stringBuilderPaxIdecHeaderAndCoupons.Append("\r\n");

                        #endregion

                        #region Invoice Coupons and Batch/Page Total Details

                        int batchSequenceNumber = 0;
                        double batchTotalGrossValue = 0;
                        double batchTotalInterlineServiceCharge = 0;
                        double batchTotalTax = 0;
                        int batchTotalNoCoupons = 0;
                        double batchTotalHandlingFeeAmount = 0;
                        //Invoice Variables 
                        double invoiceTotalGrossValue = 0;
                        double invoiceTotalInterlineServiceCharge = 0;
                        double invoiceTotalTax = 0;
                        int invoiceTotalNoCoupons = 0;
                        double invoiceNetTotal = 0;
                        double invoiceTotalHandlingFeeAmount = 0;
                        double invoiceNetBillingAmount = 0;
                        var couponBatches = (from batch in invoice.CouponDataRecord
                                             group batch by batch.BatchSequenceNumber
                                                 into batchGroup
                                                 select new { BatchSequenceNumber = batchGroup.Key, Count = batchGroup.Select(b => b.InvoiceId).Count() });
                        foreach (var couponSequence in couponBatches.OrderBy(s => s.BatchSequenceNumber))
                        {
                            batchSequenceNumber = couponSequence.BatchSequenceNumber;
                            batchTotalGrossValue = 0;
                            batchTotalInterlineServiceCharge = 0;
                            batchTotalTax = 0;
                            batchTotalNoCoupons = 0;
                            batchTotalHandlingFeeAmount = 0;
                            foreach (var coupon in
                                invoice.CouponDataRecord.Where(c => c.BatchSequenceNumber == couponSequence.BatchSequenceNumber).OrderBy(o => o.BatchSequenceNumber).ThenBy(o => o.RecordSequenceWithinBatch))
                            {
                                invoiceTotalNoCoupons++;

                                #region Invoice Coupons Details

                                recordSequenceNumber++;
                                batchTotalNoCoupons++;

                                //Standerd Messege Identifier Always 'PBD' For Passenger invoices
                                stringBuilderPaxIdecHeaderAndCoupons.Append("PBD");

                                //Record Sequence Number
                                stringBuilderPaxIdecHeaderAndCoupons.Append(recordSequenceNumber.ToString().PadLeft(8, '0'));

                                //Standerd Field identifier '20' for coupon details.
                                stringBuilderPaxIdecHeaderAndCoupons.Append("20");

                                //Billing Airline code 
                                stringBuilderPaxIdecHeaderAndCoupons.Append((billingMember.MemberCodeNumeric).PadLeft(4, '0'));

                                //Billed Airline code 
                                stringBuilderPaxIdecHeaderAndCoupons.Append((billedMember.MemberCodeNumeric).PadLeft(4, '0'));

                                //Billing Code
                                stringBuilderPaxIdecHeaderAndCoupons.Append(invoice.BillingCode.ToString().PadLeft(1, '0'));

                                //Invoice Number
                                stringBuilderPaxIdecHeaderAndCoupons.Append(invoice.InvoiceNumber.PadRight(14, ' '));

                                //Page/batch Sequence Number 
                                stringBuilderPaxIdecHeaderAndCoupons.Append(coupon.BatchSequenceNumber.ToString().PadLeft(5, '0'));

                                //Record Sequence Within Batch
                                stringBuilderPaxIdecHeaderAndCoupons.Append(coupon.RecordSequenceWithinBatch.ToString().PadLeft(5, '0'));

                                //Ticket Issuing Airline

                                stringBuilderPaxIdecHeaderAndCoupons.Append((coupon.TicketOrFimIssuingAirline ?? string.Empty).PadLeft(4, '0'));

                                //Coupon Number
                                stringBuilderPaxIdecHeaderAndCoupons.Append(coupon.TicketOrFimCouponNumber.ToString().PadLeft(2, '0'));

                                //Ticket/Document Number
                                stringBuilderPaxIdecHeaderAndCoupons.Append(coupon.TicketDocOrFimNumber.ToString().PadLeft(11, '0'));

                                //Check Digit
                                stringBuilderPaxIdecHeaderAndCoupons.Append(coupon.CheckDigit.ToString().PadLeft(1, '0'));

                                //Coupon Gross Value/Applicable Local Fare : While populating final value, ignore sign
                                batchTotalGrossValue += coupon.CouponGrossValueOrApplicableLocalFare;
                                stringBuilderPaxIdecHeaderAndCoupons.Append(
                                    Math.Abs(coupon.CouponGrossValueOrApplicableLocalFare).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                        CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(11, '0'));

                                //Interline Service Charge (%) : 
                                // (1) If a non-zero value is available in â€˜Other Commission Amountâ€™ or â€˜UATP Amountâ€™ of IS data, 
                                //     then ISC % will be calculated as [â€˜ISC Amountâ€™ + â€˜UATP Amountâ€™ + â€˜Other Commission Amountâ€™ considering signs]
                                //     upon â€˜Coupon Gross Value/Applicable Local Fareâ€™, rounded (closest) to 2 decimals.
                                // (2) When â€˜Other Commission Amountâ€™ and â€˜UATP Amountâ€™ are zero, and just the â€˜Interline Service Charge (%)â€™ 
                                //     needs to be populated, round (closest) the 3 decimal ISC% in the IS format data to 2 decimals before populating
                                // While populating final value, ignore sign
                                double paxIscPercentage = ConvertUtil.Round(coupon.IscPercent, 3);
                                if (coupon.OtherCommissionAmount != 0 || coupon.UatpAmount != 0)
                                {
                                    paxIscPercentage = ConvertUtil.Round(((coupon.IscAmount + coupon.OtherCommissionAmount + coupon.UatpAmount) / coupon.CouponGrossValueOrApplicableLocalFare) * 100, 2);
                                }
                                batchTotalInterlineServiceCharge += ConvertUtil.Round((coupon.IscAmount + coupon.OtherCommissionAmount + coupon.UatpAmount), 2);
                                stringBuilderPaxIdecHeaderAndCoupons.Append(
                                    Math.Abs(paxIscPercentage).ToString("N3").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                        CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(5, '0'));

                                // Coupon Tax Amount : If a non-zero value is available in â€˜VAT Amountâ€™ of IS data, 
                                // then Old-IDEC field â€˜Coupon Tax Amountâ€™ = IS fields [â€˜Coupon Tax Amountâ€™ + â€˜VAT Amountâ€™ considering signs] 
                                // While populating final value, ignore sign
                                double paxCouponTaxAmount = coupon.TaxAmount;
                                if (coupon.VatAmount != 0)
                                {
                                    paxCouponTaxAmount = (coupon.TaxAmount + coupon.VatAmount);
                                }
                                batchTotalTax += paxCouponTaxAmount;
                                stringBuilderPaxIdecHeaderAndCoupons.Append(
                                    Math.Abs(paxCouponTaxAmount).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                        CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(11, '0'));

                                //Currency Adjustment Indicator
                                stringBuilderPaxIdecHeaderAndCoupons.Append((coupon.CurrencyAdjustmentIndicator == null ? " " : coupon.CurrencyAdjustmentIndicator).PadRight(3, ' '));

                                //Refund Code always blank
                                stringBuilderPaxIdecHeaderAndCoupons.Append(" ");

                                //Source Code 
                                //---int SourceCodeIdentifier = SourceCodeRepository.Single(sourceCode => sourceCode.Id == coupon.SourceCodeId).SourceCodeIdentifier;
                                stringBuilderPaxIdecHeaderAndCoupons.Append(coupon.SourceCodeId.ToString().PadLeft(2, '0'));

                                //Type Code : â€˜1â€™ for Excess Baggage coupons (when Source Code = 25)
                                // â€˜0â€™ for Passenger coupons (all other Source Codes)
                                stringBuilderPaxIdecHeaderAndCoupons.Append((coupon.SourceCodeId == 25 ? 1 : 0).ToString());

                                //Electronic Ticket Indicator
                                stringBuilderPaxIdecHeaderAndCoupons.Append((coupon.ElectronicTicketIndicator == true ? 'E' : ' ').ToString());

                                //Sample Pricing Method Indicator : 
                                //When a value of â€˜Validated PMI â€˜is available, it will be inserted and not the â€˜Original PMIâ€™. 
                                //ATPCO will pass these downgraded records to the billed carrier without performing BVC on such coupons.
                                //When â€˜Validated PMIâ€™ is non-blank/not-null: Value of â€˜Validated PMIâ€™ will be populated 
                                //When â€˜Validated PMIâ€™ is blank/null: Value of â€˜Original PMIâ€™ will be populated
                                stringBuilderPaxIdecHeaderAndCoupons.Append((string.IsNullOrEmpty(coupon.ValidatedPmi) ? (coupon.OriginalPmi ?? string.Empty) : coupon.ValidatedPmi).PadRight(1, ' '));


                                //Filler
                                stringBuilderPaxIdecHeaderAndCoupons.Append("  ");

                                //Number of Passengers always '000' for coupons details
                                stringBuilderPaxIdecHeaderAndCoupons.Append("000");

                                //Flight Number
                                stringBuilderPaxIdecHeaderAndCoupons.Append(Convert.ToInt32(coupon.FlightNumber).ToString().PadRight(5, ' '));

                                //Flight date : downgraded to Old-IDEC format  00MMDD 
                                stringBuilderPaxIdecHeaderAndCoupons.Append("00" + Convert.ToDateTime(coupon.FlightDate).ToString("MMdd"));

                                //â€˜Fromâ€™ City of Coupon : â€˜Fromâ€™ Airport of Coupon
                                stringBuilderPaxIdecHeaderAndCoupons.Append((coupon.FromAirportOfCoupon ?? string.Empty).PadRight(4, ' '));

                                //â€˜Toâ€™ City of Coupon : â€˜Toâ€™ Airport of Coupon
                                stringBuilderPaxIdecHeaderAndCoupons.Append((coupon.ToAirportOfCoupon ?? string.Empty).PadRight(4, ' '));

                                //Filing Reference
                                stringBuilderPaxIdecHeaderAndCoupons.Append((coupon.FilingReference ?? string.Empty).PadRight(10, ' '));

                                //Discount Indicator : 
                                // â€˜00â€™ if IS field â€˜UATP Amountâ€™ is equal to zero.
                                // â€˜01â€™ if IS field â€˜UATP Amountâ€™ is non-zero.
                                stringBuilderPaxIdecHeaderAndCoupons.Append(coupon.UatpAmount == 0 ? "00" : "01");

                                //Handling Fee Type
                                stringBuilderPaxIdecHeaderAndCoupons.Append(coupon.HandlingFeeTypeId.PadRight(1, ' '));

                                //Handling Fee Amount : While populating value, ignore sign
                                batchTotalHandlingFeeAmount += coupon.HandlingFeeAmount;
                                stringBuilderPaxIdecHeaderAndCoupons.Append(
                                    Math.Abs(coupon.HandlingFeeAmount).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                        CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(9, '0'));

                                //Settlement Authorisation Code
                                stringBuilderPaxIdecHeaderAndCoupons.Append((coupon.SettlementAuthorizationCode ?? string.Empty).PadRight(14, ' '));
                                stringBuilderPaxIdecHeaderAndCoupons.Append("\r\n");

                                #endregion
                            }

                            #region Batch/Page Total Details

                            recordSequenceNumber++;

                            //Standerd Messege Identifier Always 'PBD' For Passenger invoices
                            stringBuilderPaxIdecHeaderAndCoupons.Append("PBD");

                            //Record Sequence Number
                            stringBuilderPaxIdecHeaderAndCoupons.Append(recordSequenceNumber.ToString().PadLeft(8, '0'));

                            //Standerd Field identifier '40' for Batch/Page Total details.
                            stringBuilderPaxIdecHeaderAndCoupons.Append("40");

                            //Billing Airline code 
                            stringBuilderPaxIdecHeaderAndCoupons.Append((billingMember.MemberCodeNumeric).PadLeft(4, '0'));

                            //Billed Airline code 
                            stringBuilderPaxIdecHeaderAndCoupons.Append((billedMember.MemberCodeNumeric).PadLeft(4, '0'));

                            //Billing Code
                            stringBuilderPaxIdecHeaderAndCoupons.Append(invoice.BillingCode.ToString().PadLeft(1, '0'));

                            //Invoice Number
                            stringBuilderPaxIdecHeaderAndCoupons.Append(invoice.InvoiceNumber.PadRight(14, ' '));


                            //Page/batch Sequence Number 
                            stringBuilderPaxIdecHeaderAndCoupons.Append(batchSequenceNumber.ToString().PadLeft(5, '0'));

                            //Record Sequence Within Batch
                            stringBuilderPaxIdecHeaderAndCoupons.Append("99999");

                            //Filler 
                            stringBuilderPaxIdecHeaderAndCoupons.Append(String.Empty.PadLeft(5));

                            //Total Interline Service Charge Sign : The resultant sign of totals from all coupon record data for this batch ,  
                            // using the following IS format fields considering their signs:
                            // ISC Amount
                            // UATP Amount
                            // Other Commission Amount
                            // When the sign is negative, enter ‘M’, when the sign is null or positive, enter ‘P’
                            invoiceTotalInterlineServiceCharge += batchTotalInterlineServiceCharge;
                            stringBuilderPaxIdecHeaderAndCoupons.Append(batchTotalInterlineServiceCharge < 0 ? "M" : "P");

                            //Total Gross Value :  Sum of all ‘Coupon Gross Value/Applicable Local Fare’ considering the signs from all coupon record data for this batch 
                            // While populating final value, ignore sign
                            invoiceTotalGrossValue += batchTotalGrossValue;
                            stringBuilderPaxIdecHeaderAndCoupons.Append(
                                Math.Abs(batchTotalGrossValue).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                    CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(15, '0'));

                            //Total Interline Service Charge : The resultant value of totals from all coupon record data for this batch ,  
                            // using the following IS format fields considering their signs:
                            // ISC Amount
                            // UATP Amount
                            // Other Commission Amount
                            // Sign for this field will be considered and populated in field â€˜Total Interline Service Charge Signâ€™ above
                            stringBuilderPaxIdecHeaderAndCoupons.Append(
                                Math.Abs(batchTotalInterlineServiceCharge).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                    CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(15, '0'));

                            // Total Tax :  The resultant value of totals from all coupon record data for this batch ,  
                            // using the following IS format fields considering their signs:
                            // Coupon Tax Amount
                            // VAT Amount  
                            // While populating final value, ignore sign
                            invoiceTotalTax += batchTotalTax;
                            stringBuilderPaxIdecHeaderAndCoupons.Append(
                                Math.Abs(batchTotalTax).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                    CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(15, '0'));

                            //Net Total : Always ‘999999999999999’
                            stringBuilderPaxIdecHeaderAndCoupons.Append("999999999999999");

                            //Net Billing Amount : Always â€˜999999999999999â€™
                            stringBuilderPaxIdecHeaderAndCoupons.Append("999999999999999");

                            //Number of Coupons : The count of coupon records that exist in the batch 
                            stringBuilderPaxIdecHeaderAndCoupons.Append(batchTotalNoCoupons.ToString().PadLeft(6, '0'));

                            //Filler
                            stringBuilderPaxIdecHeaderAndCoupons.Append(String.Empty.PadLeft(1));

                            //Handling Fee Amount : Sum of all â€˜Handling Fee Amountâ€™ considering the signs from all coupon record data for this batch
                            invoiceTotalHandlingFeeAmount += batchTotalHandlingFeeAmount;
                            stringBuilderPaxIdecHeaderAndCoupons.Append(
                                Math.Abs(batchTotalHandlingFeeAmount).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                    CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(9, '0'));

                            //Filler
                            stringBuilderPaxIdecHeaderAndCoupons.Append(String.Empty.PadLeft(17));

                            stringBuilderPaxIdecHeaderAndCoupons.Append("\r\n");

                            #endregion
                        }

                        #endregion

                        #region Invoice Total Details

                        recordSequenceNumber++;

                        //Standerd Messege Identifier Always 'PBD' For Passenger invoices
                        stringBuilderPaxIdecHeaderAndCoupons.Append("PBD");

                        //Record Sequence Number
                        stringBuilderPaxIdecHeaderAndCoupons.Append(recordSequenceNumber.ToString().PadLeft(8, '0'));

                        //Standerd Field identifier '40' for Invoice Total details.
                        stringBuilderPaxIdecHeaderAndCoupons.Append("40");

                        //Billing Airline code 
                        stringBuilderPaxIdecHeaderAndCoupons.Append((billingMember.MemberCodeNumeric).PadLeft(4, '0'));

                        //Billed Airline code 
                        stringBuilderPaxIdecHeaderAndCoupons.Append((billedMember.MemberCodeNumeric).PadLeft(4, '0'));

                        //Billing Code
                        stringBuilderPaxIdecHeaderAndCoupons.Append(invoice.BillingCode.ToString().PadLeft(1, '0'));

                        //Invoice Number
                        stringBuilderPaxIdecHeaderAndCoupons.Append(invoice.InvoiceNumber.PadRight(14, ' '));

                        //Page/batch Sequence Number 
                        stringBuilderPaxIdecHeaderAndCoupons.Append("99999");

                        //Record Sequence Within Batch
                        stringBuilderPaxIdecHeaderAndCoupons.Append("99999");

                        //Prov. Adjustment Rate  : 
                        //When Billing Code of the invoice = 0:
                        //Value of â€˜Prov. Adjustment Rateâ€™ (this value will be found with zeroes will be populated as such)
                        //When Billing Code of the invoice = 3:
                        //When â€˜Prov. Adjustment Rateâ€™ is not-blank/not-null, value of â€˜Prov. Adjustment Rateâ€™
                        //When â€˜Prov. Adjustment Rateâ€™ is blank/null, sum of following fields considering their signs:
                        //Fare Absorption %
                        //ISC Absorption %
                        //Tax Absorption %
                        //UATP Absorption %
                        //Handling Fee Absorption %
                        //Other Commission Absorption %
                        //VAT Absorption %
                        var passengerInvoiceTotal = invoice.InvoiceTotalRecord;
                        double provAdjustmentRate = Convert.ToDouble(passengerInvoiceTotal.ProvAdjustmentRate);
                        if (passengerInvoiceTotal.ProvAdjustmentRate == 0 && invoice.BillingCode == (int)BillingCode.SamplingFormAB)
                        {
                            //provAdjustmentRate = Convert.ToDouble(passengerInvoiceTotal.FareAbsorptionPercent) + Convert.ToDouble(passengerInvoiceTotal.IscAbsorptionPercent) +
                            //                     Convert.ToDouble(passengerInvoiceTotal.TaxAbsorptionPercent) + Convert.ToDouble(passengerInvoiceTotal.UatpAbsorptionPercent) +
                            //                     Convert.ToDouble(passengerInvoiceTotal.HandlingFeeAbsorptionPercent) + Convert.ToDouble(passengerInvoiceTotal.OtherCommissionAbsorptionPercent) +
                            //                     Convert.ToDouble(passengerInvoiceTotal.VatAbsorptionPercent);
                            provAdjustmentRate = ((double)Math.Abs(passengerInvoiceTotal.TotalProvisionalAdjustmentAmount) / (double)Math.Abs(passengerInvoiceTotal.TotalGrossValue)) * 100;
                        }
                        stringBuilderPaxIdecHeaderAndCoupons.Append(
                            Math.Abs(provAdjustmentRate).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(5, '0'));
                        //----stringBuilderPaxIDECHeaderAndCoupons.Append(invoice.a);

                        //Total Interline Service Charge/Prov. Adjustment Sign : 
                        //When Billing Code of the invoice = 0:
                        //Add the amounts â€˜Total Interline Service Chargeâ€™, â€˜Total Other Commission Amountâ€™ and â€˜Total UATP Amountâ€™ considering their respective signs. If the resultant sign is negative, populate â€˜Mâ€™. If the resultant sign is positive, populate â€˜Pâ€™.
                        //When Billing Code of the invoice = 3:
                        //Populate the value of â€˜Total Provisional Adjustment Amount Signâ€™.
                        if (invoice.BillingCode == (int)BillingCode.SamplingFormAB)
                        {
                            invoiceTotalInterlineServiceCharge = (double)passengerInvoiceTotal.TotalProvisionalAdjustmentAmount;
                            stringBuilderPaxIdecHeaderAndCoupons.Append(invoiceTotalInterlineServiceCharge < 0 ? "M" : "P");
                        }
                        else
                        {
                            invoiceTotalInterlineServiceCharge = (double)(passengerInvoiceTotal.TotalIscAmount + passengerInvoiceTotal.TotalOtherCommission + passengerInvoiceTotal.TotalUatpAmount);
                            stringBuilderPaxIdecHeaderAndCoupons.Append(invoiceTotalInterlineServiceCharge < 0 ? "M" : "P");
                        }
                        invoiceTotalGrossValue = (double)passengerInvoiceTotal.TotalGrossValue;
                        //Total Gross Value : While populating final value, ignore sign.
                        fileTotalGrossValue += Math.Abs(invoiceTotalGrossValue);
                        stringBuilderPaxIdecHeaderAndCoupons.Append(
                            Math.Abs(invoiceTotalGrossValue).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(15, '0'));

                        //Total Interline Service Charge/Prov. Adjustment Amount : 
                        //When Billing Code = 0:Add the amounts â€˜Total Interline Service Chargeâ€™ + â€˜Total Other Commission Amountâ€™ + â€˜UATP Amountâ€™ considering their respective signs. 
                        //When Billing Code = 3:Populate the value of â€˜Total Provisional Adjustment Amountâ€™.
                        //While populating final value, ignore sign. 

                        if (invoice.BillingCode == (int)BillingCode.SamplingFormAB)
                        {
                            //double provAdjustmentAmount = 0;
                            //provAdjustmentAmount = Convert.ToDouble(passengerInvoiceTotal.FareAbsorptionAmount) + Convert.ToDouble(passengerInvoiceTotal.IscAbsorptionAmount) + Convert.ToDouble(passengerInvoiceTotal.TaxAbsorptionAmount) + Convert.ToDouble(passengerInvoiceTotal.UatpAbsorptionAmount) + Convert.ToDouble(passengerInvoiceTotal.HandlingFeeAbsorptionAmount) + Convert.ToDouble(passengerInvoiceTotal.OtherCommissionAbsorptionAmount) + Convert.ToDouble(passengerInvoiceTotal.VatAbsorptionAmount);

                            fileTotalInterlineServiceCharge += (double)Math.Abs(passengerInvoiceTotal.TotalProvisionalAdjustmentAmount);
                            stringBuilderPaxIdecHeaderAndCoupons.Append(
                                Math.Abs(passengerInvoiceTotal.TotalProvisionalAdjustmentAmount).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                    CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(15, '0'));

                        }
                        else
                        {
                            fileTotalInterlineServiceCharge += Math.Abs(invoiceTotalInterlineServiceCharge);
                            stringBuilderPaxIdecHeaderAndCoupons.Append(
                                Math.Abs(invoiceTotalInterlineServiceCharge).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                    CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(15, '0'));

                        }
                        //Total Tax :  â€˜Total Taxâ€™ + â€˜Total VAT Amountâ€™ considering their respective signs.While populating final value, ignore sign 
                        invoiceTotalTax = (double)(passengerInvoiceTotal.TotalTaxAmount + passengerInvoiceTotal.TotalVatAmount);
                        fileTotalTax += Math.Abs(invoiceTotalTax);
                        stringBuilderPaxIdecHeaderAndCoupons.Append(
                            Math.Abs(invoiceTotalTax).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(15, '0'));

                        //Net Total : While populating value, ignore sign //this value Get from Pax Invoice Total.
                        //invoiceNetTotal = invoiceTotalGrossValue + invoiceTotalInterlineServiceCharge + invoiceTotalTax;
                        invoiceNetTotal = (double)passengerInvoiceTotal.NetTotal;
                        fileNetTotal += Math.Abs(invoiceNetTotal);
                        stringBuilderPaxIdecHeaderAndCoupons.Append(
                            Math.Abs(invoiceNetTotal).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(15, '0'));

                        //Net Billing Amount : Calculated as â€˜Net Totalâ€™ multiplied by â€˜Rate of Exchangeâ€™ populated in the Old-IDEC header record. Round closest to 2 decimals.
                        invoiceNetBillingAmount = ConvertUtil.Round((invoiceNetTotal * Convert.ToDouble(exchangeRate)), 2);
                        fileNetBillingAmount += Math.Abs(invoiceNetBillingAmount);
                        stringBuilderPaxIdecHeaderAndCoupons.Append(
                            Math.Abs(invoiceNetBillingAmount).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(15, '0'));

                        //Number of Coupons : No. of Billing Records //this value Get from Pax Invoice Total.
                        invoiceTotalNoCoupons = passengerInvoiceTotal.NoOfBillingRecords;
                        fileTotalNoCoupons += Math.Abs(invoiceTotalNoCoupons);
                        stringBuilderPaxIdecHeaderAndCoupons.Append(invoiceTotalNoCoupons.ToString().PadLeft(6, '0'));

                        //Filler
                        stringBuilderPaxIdecHeaderAndCoupons.Append(String.Empty.PadLeft(1));

                        //Handling Fee Amount : While populating value, ignore sign
                        fileTotalHandlingFeeAmount += Math.Abs(invoiceTotalHandlingFeeAmount);
                        stringBuilderPaxIdecHeaderAndCoupons.Append(
                            Math.Abs(invoiceTotalHandlingFeeAmount).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                                CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(9, '0'));

                        //Filler
                        stringBuilderPaxIdecHeaderAndCoupons.Append(String.Empty.PadLeft(17));

                        stringBuilderPaxIdecHeaderAndCoupons.Append("\r\n");

                        #endregion
                    }

                    #region File total Details

                    recordSequenceNumber++;

                    //Standerd Messege Identifier Always 'PBD' For Passenger invoices
                    stringBuilderPaxIdecHeaderAndCoupons.Append("PBD");

                    //Record Sequence Number
                    stringBuilderPaxIdecHeaderAndCoupons.Append(recordSequenceNumber.ToString().PadLeft(8, '0'));

                    //Standerd Field identifier '40' for File Total details.
                    stringBuilderPaxIdecHeaderAndCoupons.Append("40");

                    //Billing Airline code : Accounting code of the Billing Airline for which the file is created
                    stringBuilderPaxIdecHeaderAndCoupons.Append((billingMemberCodeNumeric).PadLeft(4, '0'));

                    //Billed Airline code : Always ‘9999’
                    stringBuilderPaxIdecHeaderAndCoupons.Append("9999");

                    //Billing Code : Always ‘9’.
                    stringBuilderPaxIdecHeaderAndCoupons.Append("9");

                    //Invoice Number : 6N portion - always ‘999999’ and 8 A/N portion - always ‘99999999’
                    stringBuilderPaxIdecHeaderAndCoupons.Append("99999999999999");

                    //Page/batch Sequence Number : Always ‘99999’
                    stringBuilderPaxIdecHeaderAndCoupons.Append("99999");

                    //Record Sequence Within Batch : Always ‘99999’
                    stringBuilderPaxIdecHeaderAndCoupons.Append("99999");

                    //Prov. Adjustment Rate : Always ‘99999’
                    stringBuilderPaxIdecHeaderAndCoupons.Append("99999");

                    //Filler
                    stringBuilderPaxIdecHeaderAndCoupons.Append(String.Empty.PadLeft(1));

                    //Total Gross Value : Sum of all ‘Total Gross Value’ from all ‘Invoice Total Records’ of the downgraded Old-IDEC file being created. 
                    //This will be a hash total (signs are anyway not available in the Old-IDEC for this field).
                    stringBuilderPaxIdecHeaderAndCoupons.Append(
                        Math.Abs(fileTotalGrossValue).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                            CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(15, '0'));

                    //Total Interline Service Charge/Prov. Adjustment Amount : Sum of all ‘Total Interline Service Charge/Prov. Adjustment Amount’ from all ‘Invoice Total Records’ of the downgraded Old-IDEC file being created. 
                    //This will be a hash total (sign values available in ‘Total Interline Service Charge/Prov. Adjustment Sign’ will be ignored)
                    stringBuilderPaxIdecHeaderAndCoupons.Append(
                        Math.Abs(fileTotalInterlineServiceCharge).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                            CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(15, '0'));

                    //Total Tax : Sum of all ‘Total Tax’ from all ‘Invoice Total Records’ of the downgraded Old-IDEC file being created. 
                    //This will be a hash total (signs are anyway not available in the Old-IDEC for this field).
                    stringBuilderPaxIdecHeaderAndCoupons.Append(
                        Math.Abs(fileTotalTax).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                            CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(15, '0'));

                    //Net Total : Sum of all ‘Net Total’ from all ‘Invoice Total Records’ of the downgraded Old-IDEC file being created. 
                    //This will be a hash total (signs are anyway not available in the Old-IDEC for this field).
                    stringBuilderPaxIdecHeaderAndCoupons.Append(
                        Math.Abs(fileNetTotal).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                            CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(15, '0'));

                    //Net Billing Amount : Sum of all ‘Net Billing Amount’ from all ‘Invoice Total Records’ of the downgraded Old-IDEC file being created. 
                    //This will be a hash total (Signs are anyway not available in the Old-IDEC for this field. Even if the Currency of Billing values vary among invoices, they will be ignored).
                    stringBuilderPaxIdecHeaderAndCoupons.Append(
                        Math.Abs(fileNetBillingAmount).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                            CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(15, '0'));

                    //Number of Coupons : Sum of all ‘Number of Coupons’ from all ‘Invoice Total Records’ of the downgraded Old-IDEC file being created.
                    stringBuilderPaxIdecHeaderAndCoupons.Append(fileTotalNoCoupons.ToString().PadLeft(6, '0'));

                    //Filler
                    stringBuilderPaxIdecHeaderAndCoupons.Append(String.Empty.PadLeft(1));

                    //Handling Fee Amount : Sum of all ‘Net Total’ from all ‘Invoice Total Records’ of the downgraded Old-IDEC file being created. 
                    //This will be a hash total (signs are anyway not available in the Old-IDEC for this field).
                    stringBuilderPaxIdecHeaderAndCoupons.Append(
                        Math.Abs(fileTotalHandlingFeeAmount).ToString("N2").Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, String.Empty).Replace(
                            CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, String.Empty).PadLeft(9, '0'));

                    //Filler
                    stringBuilderPaxIdecHeaderAndCoupons.Append(String.Empty.PadLeft(17));

                    stringBuilderPaxIdecHeaderAndCoupons.Append("\r\n");

                    #endregion

                    //This portion of code used to create file on tempory location.
                    string fileUploadPath = FileIo.GetATPCOFTPDownloadFolderPath();
                    //fileUploadPath = @"D:\OLDIDEC_FILES\";
                    // If sending non-zipped file:
                    //V16.PROD.XMT@@RCN.IDECP
                    //replace PROD to UTST in both file name
                    string fileName = String.Empty;

                    if (SystemParameters.Instance.Atpco.ApplicationMode.ToUpper() == "PROD".ToUpper())
                    {
                        fileName = "V16.PROD.XMT" + billingMemberCodeAlpha + "RCN.IDECP";
                    }
                    else
                    {
                        fileName = "V16.UTST.XMT" + billingMemberCodeAlpha + "RCN.IDECP";
                    }


                    // If sending zipped file:
                    // V16.PROD.XMT@@RCZ.IDECP @@=Member Alpha Code
                    //string zipFileName = "V16.PROD.XMT" + billingMemberCodeAlpha + "RCZ.IDECP";
                    //Add the single quotes to zip file name
                    string zipFileName = string.Empty;

                    if (SystemParameters.Instance.Atpco.ApplicationMode.ToUpper() == "PROD".ToUpper())
                    {
                        zipFileName = "'V16.PROD.XMT" + billingMemberCodeAlpha + "RCZ.IDECP'";
                    }
                    else
                    {
                        zipFileName = "'V16.UTST.XMT" + billingMemberCodeAlpha + "RCZ.IDECP'";
                    }


                    var streamWriter = File.CreateText(fileUploadPath + fileName);
                    streamWriter.Write(stringBuilderPaxIdecHeaderAndCoupons.ToString().TrimEnd(new char[] { '\r', '\n' }));
                    streamWriter.Close();
                    //Create Zip File.
                    //Add guid in Zip File Name
                    string guidKey = Guid.NewGuid().ToString();
                    zipFileName = zipFileName + "_" + guidKey;
                    FileIo.ZipOutputFile((fileUploadPath + fileName), fileUploadPath, zipFileName);
                    Logger.Info("Zip Output File Generated For Member:" + billingMemberCodeAlpha);
                    Logger.Info("Zip Output File" + zipFileName + " Created at:" + fileUploadPath);

                    var IsInputFileRepository = new Repository<IsInputFile>(_unitOfWork); // Ioc.Resolve<IRepository<IsInputFile>>(typeof(IRepository<IsInputFile>));

                    var isInputFile = new IsInputFile
                                          {
                                              BillingMonth = month,
                                              BillingPeriod = period,
                                              BillingYear = year,
                                              FileDate = DateTime.UtcNow,
                                              FileFormat = FileFormatType.OldIdec,
                                              FileLocation = fileUploadPath,
                                              //File location should not contain file name
                                              FileName = zipFileName,
                                              FileStatus = FileStatusType.AvailableForDownload,
                                              SenderRecieverType = (int)FileSenderRecieverType.ATPCO,
                                              FileVersion = "0.1",
                                              IsIncoming = true,
                                              ReceivedDate = DateTime.UtcNow,
                                              SenderReceiverIP = Dns.GetHostByName(Dns.GetHostName()).AddressList.First().ToString(),
                                              OutputFileDeliveryMethodId = 1
                                          };
                    IsInputFileRepository.Add(isInputFile);
                    _unitOfWork.Commit();
                    Logger.Info("Add Entry in Is File Log for file:" + zipFileName);
                }
                else
                {
                    Logger.Info("No invoices found");
                }
            }
            catch (Exception ex)
            {

            }
        }




        /// <summary>
        /// Get Invoice Legal PDF path 
        /// </summary>
        /// <param name="invoiceId">Invoice Number </param>
        /// <returns> PDF location path </returns>
        public string GetInvoiceLegalPfdPath(Guid invoiceId)
        {

            var invoicepdfpath = InvoiceRepository.GetInvoiceLegalPdfPath(invoiceId);

            return invoicepdfpath;
        }

        /// <summary>
        /// This function will only return the invalid Invoices,
        /// which were not marked as processing complete by supporting
        /// document finalization process.
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        public List<InvoiceBase> GetInvalidInvoicesForOutputfileGeneration(SearchCriteria searchCriteria)
        {
            return
                //CMP#622 : MISC Outputs Split as per Location ID 
              InvoiceBaseRepository.Get(
                i =>
                i.BilledMemberId == searchCriteria.BilledMemberId && i.BillingPeriod == searchCriteria.BillingPeriod && i.BillingMonth == searchCriteria.BillingMonth &&
                i.BillingYear == searchCriteria.BillingYear && (i.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling || i.InvoiceStatusId == (int)InvoiceStatusType.Claimed) && i.MiscBilledMemberLocCode==null).ToList();
        }

        /// <summary>
        /// For system monitor, this function will only return the Invoice bases to be generated in output file
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        public List<InvoiceBase> SystemMonitorGetAllInvoiceBasesForOutputfileGeneration(SearchCriteria searchCriteria)
        {
            return
              InvoiceBaseRepository.Get(
                i =>
                i.BilledMemberId == searchCriteria.BilledMemberId && i.BillingPeriod == searchCriteria.BillingPeriod && i.BillingMonth == searchCriteria.BillingMonth &&
                i.BillingYear == searchCriteria.BillingYear &&
                (i.InvoiceStatusId == (int)InvoiceStatusType.Presented || i.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete ||
                i.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling || i.InvoiceStatusId == (int)InvoiceStatusType.Claimed)).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileLogId">file log id </param>
        /// <param name="billedMemberId">billing memberId</param>
        /// <param name="billingMemberId">billed memberId</param>
        /// <returns></returns>
        public List<InvoiceBase> GetSuspendedMembersInvoicesForIsFile(string fileLogId, int billedMemberId, int billingMemberId)
        {
            var fileLogIdGuid = ConvertUtil.ConvertStringtoGuid(fileLogId);
            return InvoiceBaseRepository.Get(
                invoice => invoice.IsInputFileId == fileLogIdGuid && invoice.BilledMemberId == billedMemberId && invoice.BillingMemberId == billingMemberId).ToList();

        }

        /// <summary>
        /// Check Submit Invoice Permission of user.
        /// ID : 296572 - Submission and Assign permission to user doesn't match !
        /// </summary>
        /// <param name="invIdList">Invoice id list </param>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        public List<string> ChkInvSubmitPermission(List<string> invIdList, int userId)
        {
          var authorizationManager = Ioc.Resolve<IAuthorizationManager>();
          var hasNSSubmitPermit = authorizationManager.IsAuthorized(userId,
                                                                     Security.Permissions.Pax.Receivables.NonSampleInvoice.
                                                                       Submit);
          var hasFormFSubmitPermit = authorizationManager.IsAuthorized(userId,
                                                                     Security.Permissions.Pax.Receivables.SampleFormF.
                                                                       Submit);
          var hasFormXFSubmitPermit = authorizationManager.IsAuthorized(userId,
                                                                     Security.Permissions.Pax.Receivables.SampleFormXF.
                                                                       Submit);
          var hasFromDESubmitPermit = authorizationManager.IsAuthorized(userId,
                                                                     Security.Permissions.Pax.Receivables.SampleFormDE.
                                                                       Submit);
          var hasCreditNoteSubmitPermit = authorizationManager.IsAuthorized(userId,
                                                                            Security.Permissions.Pax.Receivables.NonSampleCreditNote.
                                                                        Submit);

          var invTobeSubmit = new List<string>();
          foreach (var invId in invIdList)
          {
            var invoice = GetInvoiceDetailForFileUpload(invId);
            //Form AB can only come through file
            if (invoice.InvoiceType == InvoiceType.Invoice)
            {
              if (invoice.BillingCode == (int)BillingCode.NonSampling && hasNSSubmitPermit)
              {
                invTobeSubmit.Add(invoice.Id.ToString());
              }
              else if (invoice.BillingCode == (int)BillingCode.SamplingFormDE && hasFromDESubmitPermit)
              {
                invTobeSubmit.Add(invoice.Id.ToString());
              }
              else if (invoice.BillingCode == (int)BillingCode.SamplingFormF && hasFormFSubmitPermit)
              {
                invTobeSubmit.Add(invoice.Id.ToString());
              }
              else if (invoice.BillingCode == (int)BillingCode.SamplingFormXF && hasFormXFSubmitPermit)
              {
                invTobeSubmit.Add(invoice.Id.ToString());
              }
            }
            if (invoice.InvoiceType == InvoiceType.CreditNote && hasCreditNoteSubmitPermit)
            {
              invTobeSubmit.Add(invoice.Id.ToString());
            }
          }

          return invTobeSubmit;
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceId">invoiceId</param>
        /// <returns></returns>
        public InvoiceBase GetInvoice(Guid invoiceId)
        {
          return InvoiceBaseRepository.Single(invoice => invoice.Id == invoiceId);
        }

        /// <summary>
        /// Validate the error on error correction screen.
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="exceptionCode"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public int ValidateForErrorCorrection(string newValue, string exceptionCode, Guid? entityId = null)
        {
            int result = 0;
            if (exceptionCode.CompareTo(ErrorCodes.InvalidFlightDate.ToUpper()) == 0)
            {
                if (CheckValidDate(newValue) == 1)
                {
                    return 1;
                }
                return 0;
            }
            if (exceptionCode.CompareTo(ErrorCodes.InvalidFlightNumber.ToUpper()) == 0)
            {
                try
                {
                    if (Convert.ToInt32(newValue) > 0)
                        return 1;
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            // Check for the valid currency code
            if (exceptionCode.CompareTo(ErrorCodes.InvalidCurrencyAdjustmentInd.ToUpper()) == 0)
            {
                if (ReferenceManager.IsValidCurrencyCode(newValue))
                {
                    return 1;
                }
                return 0;
            }

            //Reference field 2
            if (exceptionCode.CompareTo(ErrorCodes.InvalidReferenceField2ForBMCoupon.ToUpper()) == 0)
            {
                if (entityId == null)
                    return 0;

                var bmCouponBreakdownRepository = Ioc.Resolve<IBillingMemoCouponBreakdownRecordRepository>();
                var bmCoupon = bmCouponBreakdownRepository.First(i => i.Id == entityId.Value);
                if (bmCoupon != null)
                {
                    var isValid = ReferenceManager.IsValidRfiscCode(newValue, bmCoupon.ReferenceField1.Trim());
                    return isValid ? 1 : 0;
                }
                return 0;
            }

            if (exceptionCode.CompareTo(ErrorCodes.InvalidReferenceField2ForCoupon.ToUpper()) == 0)
            {
                if (entityId == null)
                    return 0;
                var couponRecordRepository = Ioc.Resolve<ICouponRecordRepository>();
                var coupon = couponRecordRepository.First(i => i.Id == entityId.Value);
                if (coupon != null)
                {
                    var isValid = ReferenceManager.IsValidRfiscCode(newValue, coupon.ReferenceField1.Trim());
                    return isValid ? 1 : 0;



                }
                return 0;
            }

            return result;
        }

        public void ValidateCorrespondenceReference(BillingMemo billingMemo, bool isUpdateOperation, PaxInvoice billingMemoInvoice)
        {
          var paxCorrespondence =
            PaxCorrespondenceRepository.Get(
              correspondence => correspondence.CorrespondenceNumber == billingMemo.CorrespondenceRefNumber).
              OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();

          if (paxCorrespondence == null)
          {
            throw new ISBusinessException(ErrorCodes.BillingMemoReferenceCorrespondenceDoesNotExist);
          }

          if (paxCorrespondence.CorrespondenceStatus == CorrespondenceStatus.Closed)
          {
            throw new ISBusinessException(ErrorCodes.CorrespondenceStatusIsClosed);
          }


          var transactionType = TransactionType.BillingMemo;

          if (billingMemo.ReasonCode == ReasonCode6A)
          {
            // The Billing Airline should be the recipient of the last correspondence and
            // the Billed Airline should be the respondent of the last correspondence.
            if (paxCorrespondence.FromMemberId != billingMemoInvoice.BilledMemberId ||
                paxCorrespondence.ToMemberId != billingMemoInvoice.BillingMemberId
                || paxCorrespondence.CorrespondenceStatusId != (int) CorrespondenceStatus.Open)
            {
              throw new ISBusinessException(ErrorCodes.InvalidCorrespondenceMembers);
            }

            // The Billed Airline should have provided an Authority to Bill to the Billing Airline.
            if (!paxCorrespondence.AuthorityToBill)
            {
              throw new ISBusinessException(ErrorCodes.AuthorityToBillNotSetForCorrespondence);
            }

            //SCP480619: Correspondence Expiry Notification for Passenger
            transactionType = TransactionType.PasNsBillingMemoDueToAuthorityToBill;
          }
          else if (billingMemo.ReasonCode == ReasonCode6B)
          {
            // The Billing Airline should be the respondent of the last correspondence and
            // the Billed Airline should be the recipient of the last correspondence.
            if (paxCorrespondence.FromMemberId != billingMemoInvoice.BillingMemberId ||
                paxCorrespondence.ToMemberId != billingMemoInvoice.BilledMemberId)
            {
              throw new ISBusinessException(ErrorCodes.InvalidCorrespondenceMembers);
            }

            // The correspondence should be expired for the Billed Airline
            if (paxCorrespondence.CorrespondenceStatus != CorrespondenceStatus.Expired)
            {
              throw new ISBusinessException(ErrorCodes.CorrespondenceStatusNotExpired);
            }

            //SCP480619: Correspondence Expiry Notification for Passenger
            transactionType = TransactionType.PasNsBillingMemoDueToExpiry;
          }
          //CMP#624 : 2.10 - Change#6 : Time Limits
          /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
          if (!ReferenceManager.IsSmiLikeBilateral(billingMemoInvoice.SettlementMethodId, false))
          {
            if (
              !ReferenceManager.IsTransactionInTimeLimitMethodD(transactionType, billingMemoInvoice.SettlementMethodId,
                                                                paxCorrespondence.CorrespondenceDate))
            {
              throw new ISBusinessException(ErrorCodes.TimeLimitExpiryForCorrespondence);
            }
          }
          else
          {
            if (
              !ReferenceManager.IsTransactionInTimeLimitMethodD1(transactionType, Convert.ToInt32(SMI.Bilateral),
                                                                 paxCorrespondence.CorrespondenceDate))
            {
              throw new ISBusinessException(ErrorCodes.TimeLimitExpiryForCorrespondence);
            }
          }
          //if (!(ReferenceManager.IsTransactionInTimeLimitForBm(transactionType, billingMemoInvoice.SettlementMethodId, paxCorrespondence.CorrespondenceDate, billingMemoInvoice)))
          //{
          //    throw new ISBusinessException(ErrorCodes.TimeLimitExpiryForCorrespondence);
          //}

            //SCP219674 : InvalidAmountToBeSettled Validation
            #region Old Code for Validatation of CorrespondenceAmounttobeSettled - To be remove
            /*if (paxCorrespondence.CurrencyId != null && billingMemoInvoice.ListingCurrencyId != null)
          {
            var exchangeRate = ReferenceManager.GetExchangeRate(billingMemoInvoice.ListingCurrencyId.Value,
                                                                (BillingCurrency) paxCorrespondence.CurrencyId.Value,
                                                                billingMemoInvoice.BillingYear,
                                                                billingMemoInvoice.BillingMonth);

                var netAmountBilled = exchangeRate > 0 ? billingMemo.NetAmountBilled / Convert.ToDecimal(exchangeRate) : billingMemo.NetAmountBilled;

                if (ConvertUtil.Round(paxCorrespondence.AmountToBeSettled, Constants.CgoDecimalPlaces) != ConvertUtil.Round(Convert.ToDecimal(netAmountBilled), Constants.CgoDecimalPlaces))
            {
              throw new ISBusinessException(ErrorCodes.InvalidAmountToBeSettled);
            }
            }*/
            #endregion
            #region New Code for Validation of CorrespondenceAmounttobeSettled
            if (paxCorrespondence.CurrencyId != null && billingMemoInvoice.ListingCurrencyId != null)
            {
              // CMP#624 : 2.10-Change#4 - Conditional validation of PAX/CGO 6A/6B BM amounts  
              var corrRmInvoice = InvoiceRepository.GetInvoiceHeader(paxCorrespondence.InvoiceId);
              if(corrRmInvoice!=null)
              {
                if(!ValidateSmiAfterLinking(billingMemoInvoice.SettlementMethodId,corrRmInvoice.SettlementMethodId))
                {
                    /* CMP #624: ICH Rewrite-New SMI X 
                    * Description: Code Fixed regarding CMP 624: ISWEB -  BM saved without checking the SMI of lined rejection invoice. 
                    * Code modified to provide case specific error. */
                    if (corrRmInvoice.SettlementMethodId == (int)SMI.IchSpecialAgreement)
                    {
                        /* ERROR_CODE_O := 'Exception: BPAXNS_10945 - Rejected Invoice/Credit Note was billed using Settlement Method X. 
                        * Invoices/Credit Notes billed using Settlement Method X can be rejected only by an Invoice using Settlement Method X*/
                        throw new ISBusinessException(ErrorCodes.PaxNSRejInvBHLinkingCheckForSmiX);
                    }
                    else
                    {
                        /* Exception: BPAXNS_10943 - Rejected Invoice/Credit Note was billed using a Settlement Method other than X. 
                        * Only Invoices/Credit Notes billed using Settlement Method X can be rejected by an Invoice using Settlement Method X. */
                        throw new ISBusinessException(ErrorCodes.PaxNSRejctionInvoiceLinkingCheckForSmiX);
                    }
                }
              }
              decimal netBilledAmount = billingMemo.NetAmountBilled;
              var isValid = ReferenceManager.ValidateCorrespondenceAmounttobeSettled(billingMemoInvoice,
                                                                                     ref netBilledAmount,
                                                                                     paxCorrespondence.CurrencyId.Value,
                                                                                     paxCorrespondence.AmountToBeSettled, corrRmInvoice);
              if (!isValid)
                throw new ISBusinessException(ErrorCodes.InvalidAmountToBeSettled);
          }
            #endregion
            // Check if BM already exists for the given correspondence ref no.
            CheckDuplicateBillingMemoForCorr(billingMemo, billingMemoInvoice, true);
        }

        /// <summary>
        /// check date format is yyMMdd
        /// </summary>
        /// <returns></returns>
        private static int CheckValidDate(string newValue)
        {
            var cultureInfo = new CultureInfo("en-US");
            DateTime awbDate;

            string strRegex = @"(19|20|21)\d\d[-](0[1-9]|1[012])[-](0[1-9]|[12][0-9]|3[01])";
            if (Regex.IsMatch(newValue, strRegex))
            {
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Get next sequence number for Auto Billing Invoice
        /// </summary>
        /// <returns>Next Invoice Sequnce Number</returns>
        public int GetAutoBillingInvoiceNumberSeq(int billingMemberId, int billingYear)
        {
            return InvoiceRepository.GetAutoBillingInvoiceNumberSeq(billingMemberId, billingYear);
        }


        public bool AuditDeletedInvoice(AuditDeletedInvoice auditDeletedInvoice )
        {
            InvoiceDeletedAuditRepository.Add(auditDeletedInvoice);
            
       //     UnitOfWork.CommitDefault();
            
            return true;
        }

        public List<InvoiceDeletionAuditReport> GetInvoiceDeletionAuditDetails(AuditDeletedInvoice auditDeletedInvoice)
        {
            return InvoiceRepository.GetInvoiceDeletionAuditDetails(auditDeletedInvoice);
        }


        /// <summary>
        /// Validates the amounts in RM.
        /// </summary>
        /// <param name="outcomeOfMismatchOnRmBilledOrAllowedAmounts">if set to <c>true</c> [outcome of mismatch on rm billed or allowed amounts].</param>
        /// <param name="exceptionDetailsList">The exception details list.</param>
        /// <param name="rejectionMemoRecord">The rejection memo record.</param>
        /// <param name="isErrorCorrection">if set to <c>true</c> [is error correction].</param>
        /// <returns></returns>
        public bool ValidateAmountsInRMonMemoLevel(bool outcomeOfMismatchOnRmBilledOrAllowedAmounts, IList<IsValidationExceptionDetail> exceptionDetailsList, RejectionMemo rejectionMemoRecord,bool isErrorCorrection=false)
        {
            bool isValid = true;
            //CMP#459 : Validate Memo level amounts
            if (outcomeOfMismatchOnRmBilledOrAllowedAmounts && Convert.ToBoolean(rejectionMemoRecord.IsLinkingSuccessful))
            {
                RejectionMemoRepository = Ioc.Resolve<IRejectionMemoRecordRepository>(typeof(IRejectionMemoRecordRepository));
                var rejectionMemoInvoice = InvoiceRepository.Single(id: rejectionMemoRecord.InvoiceId);
                //Get Rejection Memo record from DB
                if (!isErrorCorrection)
                {
                    rejectionMemoRecord = RejectionMemoRepository.Single(rejectionMemoId: rejectionMemoRecord.Id);
                }

                //Validate FIM Number is present in the original invoice 
                if (rejectionMemoRecord.FIMBMCMIndicatorId == (int) FIMBMCMIndicator.FIMNumber)
                {
                    if (rejectionMemoRecord.FimBMCMNumber != null && Validators.IsWholeNumber(rejectionMemoRecord.FimBMCMNumber))
                    {
                        if (rejectionMemoRecord.RejectionStage == 1)
                        {
                            var yourInvoice = GetInvoiceWithCoupons(rejectionMemoRecord.YourInvoiceNumber,
                                                                    rejectionMemoRecord.YourInvoiceBillingMonth,
                                                                    rejectionMemoRecord.YourInvoiceBillingYear,
                                                                    rejectionMemoRecord.YourInvoiceBillingPeriod,
                                                                    rejectionMemoInvoice.BilledMemberId,
                                                                    rejectionMemoInvoice.BillingMemberId,
                                                                    (int) BillingCode.NonSampling,
                                                                    string.Format("{0}:{1}:FIM", rejectionMemoRecord.FimCouponNumber, rejectionMemoRecord.FimBMCMNumber));
                            if (yourInvoice != null && yourInvoice.CouponDataRecord != null)
                            {
                                var yourFimRecordList =
                                    yourInvoice.CouponDataRecord.Where(
                                        couponRecord =>
                                        couponRecord.TicketDocOrFimNumber == Convert.ToInt64(rejectionMemoRecord.FimBMCMNumber) &&
                                        couponRecord.TicketOrFimCouponNumber == rejectionMemoRecord.FimCouponNumber).ToList();
                                if (yourFimRecordList.Count() > 0 && yourFimRecordList.FirstOrDefault() != null)
                                {
                                    //CMP#459 : Validate Amount at memo leve in FIM R1
                                    var prevDate = new DateTime(yourInvoice.BillingYear, yourInvoice.BillingMonth, 1);
                                    var currDate = new DateTime(rejectionMemoInvoice.BillingYear, rejectionMemoInvoice.BillingMonth, 1);
                                    var exchangeRateRepository = Ioc.Resolve<IExchangeRateRepository>(typeof(IExchangeRateRepository));
                                    var prevInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == yourInvoice.ListingCurrencyId && ex.EffectiveFromDate <= prevDate && ex.EffectiveToDate >= prevDate).FirstOrDefault();
                                    var currInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == rejectionMemoInvoice.ListingCurrencyId && ex.EffectiveFromDate <= currDate && ex.EffectiveToDate >= currDate).FirstOrDefault();
                                    isValid = ValidateOriginalBillingAmountInRm(outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                                                                prevInvExRate,
                                                                                currInvExRate,
                                                                                rejectionMemoRecord,
                                                                                exceptionDetailsList,
                                                                                rejectionMemoInvoice,
                                                                                yourInvoice,
                                                                                null,
                                                                                yourFimRecordList.ToList(),
                                                                                null,
                                                                                string.Empty,
                                                                                DateTime.UtcNow,
                                                                                true,
                                                                                true,
                                                                                isErrorCorrection);
                                }
                            }
                        }
                        if (rejectionMemoRecord.RejectionStage == 2 || rejectionMemoRecord.RejectionStage == 3)
                        {
                            PaxInvoice yourInvoice = GetInvoiceWithRMCoupons(rejectionMemoRecord.YourInvoiceNumber,
                                                                             rejectionMemoRecord.YourInvoiceBillingMonth,
                                                                             rejectionMemoRecord.YourInvoiceBillingYear,
                                                                             rejectionMemoRecord.YourInvoiceBillingPeriod,
                                                                             rejectionMemoInvoice.BilledMemberId,
                                                                             rejectionMemoInvoice.BillingMemberId,
                                                                             null,
                                                                             null,
                                                                             rejectionMemoRecord.YourRejectionNumber);
                            if (yourInvoice != null && yourInvoice.RejectionMemoRecord != null)
                            {
                                var yourRejectionMemoRecordList =
                                    yourInvoice.RejectionMemoRecord.Where(
                                        rejectionRec =>
                                        rejectionRec.RejectionMemoNumber == rejectionMemoRecord.YourRejectionNumber && rejectionRec.FimBMCMNumber != null &&
                                        rejectionRec.FimBMCMNumber.Trim() == rejectionMemoRecord.FimBMCMNumber && rejectionRec.FimCouponNumber == rejectionMemoRecord.FimCouponNumber).ToList();

                                if (yourRejectionMemoRecordList.Count() > 0 && yourRejectionMemoRecordList.FirstOrDefault() != null)
                                {
                                    var yourRejectionMemoRecord = yourRejectionMemoRecordList.FirstOrDefault();
                                    //CMP#459 : Validate Amount
                                    //All amounts of the rejected BM from the rejected invoice should match with the RM level amounts of the rejecting RM
                                    var prevDate = new DateTime(yourInvoice.BillingYear, yourInvoice.BillingMonth, 1);
                                    var currDate = new DateTime(rejectionMemoInvoice.BillingYear, rejectionMemoInvoice.BillingMonth, 1);
                                    var exchangeRateRepository = Ioc.Resolve<IExchangeRateRepository>(typeof(IExchangeRateRepository));
                                    var prevInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == yourInvoice.ListingCurrencyId && ex.EffectiveFromDate <= prevDate && ex.EffectiveToDate >= prevDate).FirstOrDefault();
                                    var currInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == rejectionMemoInvoice.ListingCurrencyId && ex.EffectiveFromDate <= currDate && ex.EffectiveToDate >= currDate).FirstOrDefault();
                                    isValid = ValidateOriginalBillingAmountInRm(outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                                                                prevInvExRate,
                                                                                currInvExRate,
                                                                                rejectionMemoRecord,
                                                                                exceptionDetailsList,
                                                                                rejectionMemoInvoice,
                                                                                yourInvoice,
                                                                                null,
                                                                                null,
                                                                                null,
                                                                                string.Empty,
                                                                                DateTime.UtcNow,
                                                                                true,
                                                                                true,
                                                                                isErrorCorrection);
                                }
                            }
                        }
                    }
                }
                if (rejectionMemoRecord.FIMBMCMIndicatorId == (int) FIMBMCMIndicator.BMNumber)
                {
                    if (rejectionMemoRecord.FimBMCMNumber != null && !string.IsNullOrEmpty(rejectionMemoRecord.FimBMCMNumber))
                    {
                        if (rejectionMemoRecord.RejectionStage == 1)
                        {

                            // Get rejection invoice with RM and RM Coupon for Rejection Stage 2 and 3.
                            var yourInvoice = GetInvoiceWithBMCoupons(rejectionMemoRecord.YourInvoiceNumber,
                                                                      rejectionMemoRecord.YourInvoiceBillingMonth,
                                                                      rejectionMemoRecord.YourInvoiceBillingYear,
                                                                      rejectionMemoRecord.YourInvoiceBillingPeriod,
                                                                      rejectionMemoInvoice.BilledMemberId,
                                                                      rejectionMemoInvoice.BillingMemberId,
                                                                      (int) BillingCode.NonSampling,
                                                                      null);
                            if (yourInvoice != null && yourInvoice.BillingMemoRecord != null)
                            {
                                var yourBillingMemoRecordList =
                                    yourInvoice.BillingMemoRecord.Where(billingMemo => billingMemo.BillingMemoNumber.Trim().ToUpper() == rejectionMemoRecord.FimBMCMNumber.ToUpper()).ToList();
                                if (yourBillingMemoRecordList.Count() > 0 && yourBillingMemoRecordList.FirstOrDefault() != null)
                                {
                                    var yourBillingMemoRecord = yourBillingMemoRecordList.FirstOrDefault();
                                    //CMP#459 : Validate Amount
                                    //All amounts of the rejected BM from the rejected invoice should match with the RM level amounts of the rejecting RM
                                    var prevDate = new DateTime(yourInvoice.BillingYear, yourInvoice.BillingMonth, 1);
                                    var currDate = new DateTime(rejectionMemoInvoice.BillingYear, rejectionMemoInvoice.BillingMonth, 1);
                                    var exchangeRateRepository = Ioc.Resolve<IExchangeRateRepository>(typeof(IExchangeRateRepository));
                                    var prevInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == yourInvoice.ListingCurrencyId && ex.EffectiveFromDate <= prevDate && ex.EffectiveToDate >= prevDate).FirstOrDefault();
                                    var currInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == rejectionMemoInvoice.ListingCurrencyId && ex.EffectiveFromDate <= currDate && ex.EffectiveToDate >= currDate).FirstOrDefault();
                                    isValid = ValidateOriginalBillingAmountInRm(outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                                                                prevInvExRate,
                                                                                currInvExRate,
                                                                                rejectionMemoRecord,
                                                                                exceptionDetailsList,
                                                                                rejectionMemoInvoice,
                                                                                yourInvoice,
                                                                                null,
                                                                                null,
                                                                                null,
                                                                                string.Empty,
                                                                                DateTime.UtcNow,
                                                                                true,
                                                                                true,
                                                                                isErrorCorrection);
                                }
                            }
                        }
                        else if (rejectionMemoRecord.RejectionStage == 2 || rejectionMemoRecord.RejectionStage == 3)
                        {
                            PaxInvoice yourInvoice = GetInvoiceWithRMCoupons(rejectionMemoRecord.YourInvoiceNumber,
                                                                             rejectionMemoRecord.YourInvoiceBillingMonth,
                                                                             rejectionMemoRecord.YourInvoiceBillingYear,
                                                                             rejectionMemoRecord.YourInvoiceBillingPeriod,
                                                                             rejectionMemoInvoice.BilledMemberId,
                                                                             rejectionMemoInvoice.BillingMemberId,
                                                                             null,
                                                                             null,
                                                                             rejectionMemoRecord.YourRejectionNumber);
                            if (yourInvoice != null && yourInvoice.RejectionMemoRecord != null)
                            {
                                var yourRejectionMemoRecordList =
                                    yourInvoice.RejectionMemoRecord.Where(
                                        rejectionRec =>
                                        rejectionRec.RejectionMemoNumber == rejectionMemoRecord.YourRejectionNumber && rejectionRec.FimBMCMNumber != null &&
                                        rejectionRec.FimBMCMNumber.Trim() == rejectionMemoRecord.FimBMCMNumber).ToList();
                                if (yourRejectionMemoRecordList.Count() > 0 && yourRejectionMemoRecordList.FirstOrDefault() != null)
                                {
                                    var yourRejectionMemoRecord = yourRejectionMemoRecordList.FirstOrDefault();
                                    //CMP#459 : Validate Amount
                                    //All amounts of the rejected BM from the rejected invoice should match with the RM level amounts of the rejecting RM
                                    var prevDate = new DateTime(yourInvoice.BillingYear, yourInvoice.BillingMonth, 1);
                                    var currDate = new DateTime(rejectionMemoInvoice.BillingYear, rejectionMemoInvoice.BillingMonth, 1);
                                    var exchangeRateRepository = Ioc.Resolve<IExchangeRateRepository>(typeof(IExchangeRateRepository));
                                    var prevInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == yourInvoice.ListingCurrencyId && ex.EffectiveFromDate <= prevDate && ex.EffectiveToDate >= prevDate).FirstOrDefault();
                                    var currInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == rejectionMemoInvoice.ListingCurrencyId && ex.EffectiveFromDate <= currDate && ex.EffectiveToDate >= currDate).FirstOrDefault();
                                    isValid = ValidateOriginalBillingAmountInRm(outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                                                                prevInvExRate,
                                                                                currInvExRate,
                                                                                rejectionMemoRecord,
                                                                                exceptionDetailsList,
                                                                                rejectionMemoInvoice,
                                                                                yourInvoice,
                                                                                null,
                                                                                null,
                                                                                null,
                                                                                string.Empty,
                                                                                DateTime.UtcNow,
                                                                                true,
                                                                                true,
                                                                                isErrorCorrection);
                                }
                            }
                        }
                    }
                }
                if (rejectionMemoRecord.FIMBMCMIndicatorId == (int) FIMBMCMIndicator.CMNumber)
                {
                    if (rejectionMemoRecord.FimBMCMNumber != null && !string.IsNullOrEmpty(rejectionMemoRecord.FimBMCMNumber))
                    {
                        if (rejectionMemoRecord.RejectionStage == 1)
                        {
                          var yourInvoice = GetInvoiceWithCMCoupons(rejectionMemoRecord.YourInvoiceNumber,
                                                                      rejectionMemoRecord.YourInvoiceBillingMonth,
                                                                      rejectionMemoRecord.YourInvoiceBillingYear,
                                                                      rejectionMemoRecord.YourInvoiceBillingPeriod,
                                                                      rejectionMemoInvoice.BilledMemberId,
                                                                      rejectionMemoInvoice.BillingMemberId,
                                                                      (int)BillingCode.NonSampling,
                                                                      null);
                            if (yourInvoice != null && yourInvoice.CreditMemoRecord != null)
                            {
                                var yourCreditMemoRecordList =
                                    yourInvoice.CreditMemoRecord.Where(creditMemo => creditMemo.CreditMemoNumber.Trim().ToUpper() == rejectionMemoRecord.FimBMCMNumber.ToUpper()).ToList();
                                if (yourCreditMemoRecordList.Count() > 0 && yourCreditMemoRecordList.FirstOrDefault() != null)
                                {
                                    var yourCreditMemoRecord = yourCreditMemoRecordList.FirstOrDefault();
                                    //CMP#459 : Validate Amount
                                    //All amounts of the rejected CM from the rejected invoice should match with the RM level amounts of the rejecting RM
                                    var prevDate = new DateTime(yourInvoice.BillingYear, yourInvoice.BillingMonth, 1);
                                    var currDate = new DateTime(rejectionMemoInvoice.BillingYear, rejectionMemoInvoice.BillingMonth, 1);
                                    var exchangeRateRepository = Ioc.Resolve<IExchangeRateRepository>(typeof(IExchangeRateRepository));
                                    var prevInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == yourInvoice.ListingCurrencyId && ex.EffectiveFromDate <= prevDate && ex.EffectiveToDate >= prevDate).FirstOrDefault();
                                    var currInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == rejectionMemoInvoice.ListingCurrencyId && ex.EffectiveFromDate <= currDate && ex.EffectiveToDate >= currDate).FirstOrDefault();
                                    isValid = ValidateOriginalBillingAmountInRm(outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                                                                prevInvExRate,
                                                                                currInvExRate,
                                                                                rejectionMemoRecord,
                                                                                exceptionDetailsList,
                                                                                rejectionMemoInvoice,
                                                                                yourInvoice,
                                                                                null,
                                                                                null,
                                                                                null,
                                                                                string.Empty,
                                                                                DateTime.UtcNow,
                                                                                true,
                                                                                true,
                                                                                isErrorCorrection);

                                }
                            }
                        }
                        else if (rejectionMemoRecord.RejectionStage == 2 || rejectionMemoRecord.RejectionStage == 3)
                        {
                            PaxInvoice yourInvoice = GetInvoiceWithRMCoupons(rejectionMemoRecord.YourInvoiceNumber,
                                                                             rejectionMemoRecord.YourInvoiceBillingMonth,
                                                                             rejectionMemoRecord.YourInvoiceBillingYear,
                                                                             rejectionMemoRecord.YourInvoiceBillingPeriod,
                                                                             rejectionMemoInvoice.BilledMemberId,
                                                                             rejectionMemoInvoice.BillingMemberId,
                                                                             null,
                                                                             null,
                                                                             rejectionMemoRecord.YourRejectionNumber);
                            if (yourInvoice != null && yourInvoice.RejectionMemoRecord != null)
                            {
                                var yourRejectionMemoRecordList =
                                    yourInvoice.RejectionMemoRecord.Where(
                                        rejectionRec =>
                                        rejectionRec.RejectionMemoNumber == rejectionMemoRecord.YourRejectionNumber && rejectionRec.FimBMCMNumber != null &&
                                        rejectionRec.FimBMCMNumber.Trim() == rejectionMemoRecord.FimBMCMNumber).ToList();
                                if (yourRejectionMemoRecordList.Count() > 0 && yourRejectionMemoRecordList.FirstOrDefault() != null)
                                {
                                    var yourRejectionMemoRecord = yourRejectionMemoRecordList.FirstOrDefault();
                                    //CMP#459 : Validate Amount RM
                                    //All amounts of the rejected Stage 1 RM from the rejected invoice should match with the RM level amounts of the rejecting RM
                                    var prevDate = new DateTime(yourInvoice.BillingYear, yourInvoice.BillingMonth, 1);
                                    var currDate = new DateTime(rejectionMemoInvoice.BillingYear, rejectionMemoInvoice.BillingMonth, 1);
                                    var exchangeRateRepository = Ioc.Resolve<IExchangeRateRepository>(typeof(IExchangeRateRepository));
                                    var prevInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == yourInvoice.ListingCurrencyId && ex.EffectiveFromDate <= prevDate && ex.EffectiveToDate >= prevDate).FirstOrDefault();
                                    var currInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == rejectionMemoInvoice.ListingCurrencyId && ex.EffectiveFromDate <= currDate && ex.EffectiveToDate >= currDate).FirstOrDefault();
                                    isValid = ValidateOriginalBillingAmountInRm(outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                                                                prevInvExRate,
                                                                                currInvExRate,
                                                                                rejectionMemoRecord,
                                                                                exceptionDetailsList,
                                                                                rejectionMemoInvoice,
                                                                                yourInvoice,
                                                                                null,
                                                                                null,
                                                                                null,
                                                                                string.Empty,
                                                                                DateTime.UtcNow,
                                                                                true,
                                                                                true,
                                                                                isErrorCorrection);
                                }
                            }
                        }
                    }
                }

                //Validate rejection memo record
                if ((rejectionMemoRecord.FIMBMCMIndicatorId == 0 || rejectionMemoRecord.FIMBMCMIndicatorId == (int) FIMBMCMIndicator.None) &&
                    (rejectionMemoRecord.RejectionStage == 2 || rejectionMemoRecord.RejectionStage == 3))
                {
                    PaxInvoice yourInvoice = GetInvoiceWithRMCoupons(rejectionMemoRecord.YourInvoiceNumber,
                                                                     rejectionMemoRecord.YourInvoiceBillingMonth,
                                                                     rejectionMemoRecord.YourInvoiceBillingYear,
                                                                     rejectionMemoRecord.YourInvoiceBillingPeriod,
                                                                     rejectionMemoInvoice.BilledMemberId,
                                                                     rejectionMemoInvoice.BillingMemberId,
                                                                     null,
                                                                     null,
                                                                     rejectionMemoRecord.YourRejectionNumber);
                    if (yourInvoice != null && yourInvoice.RejectionMemoRecord != null)
                    {
                        var yourRejectionMemoRecordList = yourInvoice.RejectionMemoRecord.Where(rejectionRec => rejectionRec.RejectionMemoNumber == rejectionMemoRecord.YourRejectionNumber).ToList();
                        if (yourRejectionMemoRecordList.Count() > 0 && yourRejectionMemoRecordList.FirstOrDefault() != null)
                        {
                            var yourRejectionMemoRecord = yourRejectionMemoRecordList.FirstOrDefault();
                            //CMP#459 : Validate Amount RM
                            //All amounts of the rejected Stage 1 RM from the rejected invoice should match with the RM level amounts of the rejecting RM
                            var prevDate = new DateTime(yourInvoice.BillingYear, yourInvoice.BillingMonth, 1);
                            var currDate = new DateTime(rejectionMemoInvoice.BillingYear, rejectionMemoInvoice.BillingMonth, 1);
                            var exchangeRateRepository = Ioc.Resolve<IExchangeRateRepository>(typeof(IExchangeRateRepository));
                            var prevInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == yourInvoice.ListingCurrencyId && ex.EffectiveFromDate <= prevDate && ex.EffectiveToDate >= prevDate).FirstOrDefault();
                            var currInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == rejectionMemoInvoice.ListingCurrencyId && ex.EffectiveFromDate <= currDate && ex.EffectiveToDate >= currDate).FirstOrDefault();
                            isValid = ValidateOriginalBillingAmountInRm(outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                                                        prevInvExRate,
                                                                        currInvExRate,
                                                                        rejectionMemoRecord,
                                                                        exceptionDetailsList,
                                                                        rejectionMemoInvoice,
                                                                        yourInvoice,
                                                                        null,
                                                                        null,
                                                                        null,
                                                                        string.Empty,
                                                                        DateTime.UtcNow,
                                                                        true,
                                                                        true,
                                                                        isErrorCorrection);
                        }
                    }
                }
            }
            return isValid;
        }

        /// <summary>
        /// Validates the amounts in R mon coupon level.
        /// </summary>
        /// <param name="outcomeOfMismatchOnRmBilledOrAllowedAmounts">if set to <c>true</c> [outcome of mismatch on rm billed or allowed amounts].</param>
        /// <param name="exceptionDetailsList">The exception details list.</param>
        /// <param name="rejectionMemoRecord">The rejection memo record.</param>
        /// <param name="rejectionMemoCouponBreakdownRecord">The rejection memo coupon breakdown record.</param>
        /// <param name="isErrorCorrection">if set to <c>true</c> [is error correction].</param>
        /// <returns></returns>
        public bool ValidateAmountsInRMonCouponLevel(bool outcomeOfMismatchOnRmBilledOrAllowedAmounts, IList<IsValidationExceptionDetail> exceptionDetailsList, RejectionMemo rejectionMemoRecord, RMCoupon rejectionMemoCouponBreakdownRecord, bool isBillingHistory = true, bool isErrorCorrection = false)
        {
            bool isValid = true;
            //CMP#459 : Validate Amount at coupon level.
            if (outcomeOfMismatchOnRmBilledOrAllowedAmounts && Convert.ToBoolean(rejectionMemoRecord.IsLinkingSuccessful))
            {
                var invoice = InvoiceRepository.Single(id: rejectionMemoRecord.InvoiceId);
                if (((rejectionMemoRecord.FIMBMCMIndicatorId == 0 || rejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.None ) && rejectionMemoRecord.RejectionStage == 1 && invoice.BillingCode == (int)BillingCode.NonSampling))
                {
                    PaxInvoice yourInvoice = GetInvoiceWithCoupons(rejectionMemoRecord.YourInvoiceNumber,
                                                                   rejectionMemoRecord.YourInvoiceBillingMonth,
                                                                   rejectionMemoRecord.YourInvoiceBillingYear,
                                                                   rejectionMemoRecord.YourInvoiceBillingPeriod,
                                                                   invoice.BilledMemberId,
                                                                   invoice.BillingMemberId,
                                                                   (int)BillingCode.NonSampling,
                                                                   string.Format("{0}-{1}-{2}", rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline, rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber, rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber));
                    if(yourInvoice!=null && yourInvoice.CouponDataRecord!=null)
                    {
                        var yourCouponRecords =
                        yourInvoice.CouponDataRecord.Where(
                            couponRecord =>
                            couponRecord.TicketOrFimIssuingAirline.Trim() == rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline.Trim() &&
                            couponRecord.TicketDocOrFimNumber == rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber &&
                            couponRecord.TicketOrFimCouponNumber == rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber);
                        if (yourCouponRecords.Count() > 0 && rejectionMemoRecord.FIMBMCMIndicatorId != (int)FIMBMCMIndicator.FIMNumber)
                        {
                            //CMP#459 : Validate Amount
                            //If duplicate rejected FIMs are found in the rejected invoice, all amounts of at least one FIM coupon from the rejected invoice should match the RM level amounts of the rejecting RM
                            var prevDate = new DateTime(yourInvoice.BillingYear, yourInvoice.BillingMonth, 1);
                            var currDate = new DateTime(invoice.BillingYear, invoice.BillingMonth, 1);
                            var exchangeRateRepository = Ioc.Resolve<IExchangeRateRepository>(typeof(IExchangeRateRepository));
                            var prevInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == yourInvoice.ListingCurrencyId && ex.EffectiveFromDate <= prevDate && ex.EffectiveToDate >= prevDate).FirstOrDefault();
                            var currInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == invoice.ListingCurrencyId && ex.EffectiveFromDate <= currDate && ex.EffectiveToDate >= currDate).FirstOrDefault();
                            isValid = ValidateOriginalBillingAmountInRm(outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                                                        prevInvExRate,
                                                                        currInvExRate,
                                                                        rejectionMemoRecord,
                                                                        exceptionDetailsList,
                                                                        invoice,
                                                                        yourInvoice,
                                                                        rejectionMemoCouponBreakdownRecord,
                                                                        yourCouponRecords.ToList(),
                                                                        null,
                                                                        string.Empty,
                                                                        DateTime.UtcNow,
                                                                        true,
                                                                        isBillingHistory,
                                                                        isErrorCorrection);
                        }
                    }
                    
                }
                else if (invoice.BillingCode == (int)BillingCode.SamplingFormF)
                {
                    var yourInvoice = InvoiceRepository.GetInvoiceWithFormDRecord(rejectionMemoRecord.YourInvoiceNumber, rejectionMemoRecord.YourInvoiceBillingMonth, rejectionMemoRecord.YourInvoiceBillingYear, rejectionMemoRecord.YourInvoiceBillingPeriod, invoice.BilledMemberId, invoice.BillingMemberId, (int)BillingCode.SamplingFormDE);
                    if (yourInvoice != null && yourInvoice.SamplingFormDRecord != null && yourInvoice.SamplingFormDRecord.Count > 0)
                    {
                        var yourCouponRecords =
                            yourInvoice.SamplingFormDRecord.Where(
                                formDRecord =>
                                formDRecord.TicketIssuingAirline == rejectionMemoCouponBreakdownRecord.TicketOrFimIssuingAirline &&
                                formDRecord.TicketDocNumber == rejectionMemoCouponBreakdownRecord.TicketDocOrFimNumber &&
                                formDRecord.CouponNumber == rejectionMemoCouponBreakdownRecord.TicketOrFimCouponNumber);

                        if (yourCouponRecords.Count() > 0)
                        {

                            //CMP#459 : Validate Amount 
                            //If a rejected Form D coupon is found more than once in the rejected form D/E invoice, then all amounts of at least one form D coupon from the rejected invoice should match the coupon of the rejecting RM
                            var prevDate = new DateTime(yourInvoice.BillingYear, yourInvoice.BillingMonth, 1);
                            var currDate = new DateTime(invoice.BillingYear, invoice.BillingMonth, 1);
                            var exchangeRateRepository = Ioc.Resolve<IExchangeRateRepository>(typeof(IExchangeRateRepository));
                            var prevInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == yourInvoice.ListingCurrencyId && ex.EffectiveFromDate <= prevDate && ex.EffectiveToDate >= prevDate).FirstOrDefault();
                            var currInvExRate = exchangeRateRepository.Get(ex => ex.CurrencyId == invoice.ListingCurrencyId && ex.EffectiveFromDate <= currDate && ex.EffectiveToDate >= currDate).FirstOrDefault();
                            isValid = ValidateOriginalBillingAmountInRm(outcomeOfMismatchOnRmBilledOrAllowedAmounts,
                                                                        prevInvExRate,
                                                                        currInvExRate,
                                                                        rejectionMemoRecord,
                                                                        exceptionDetailsList,
                                                                        invoice,
                                                                        yourInvoice,
                                                                        rejectionMemoCouponBreakdownRecord,
                                                                        null,
                                                                        yourCouponRecords.ToList(),
                                                                        string.Empty,
                                                                        DateTime.UtcNow,
                                                                        true,
                                                                        true,
                                                                        isErrorCorrection);
                        }
                    }
                }

            }
            return isValid;
        }

        /// <summary>
        /// CMP#459 : Validates the original billing amount in RM.
        /// </summary>
        /// <param name="outcomeOfMismatchOnRmBilledOrAllowedAmounts">if set to <c>true</c> [outcome of mismatch on rm billed or allowed amounts].</param>
        /// <param name="currentRejectionMemoRecord">The current rejection memo record.</param>
        /// <param name="exceptionDetailsList">The exception details list.</param>
        /// <param name="currentInvoice">The current invoice.</param>
        /// <param name="yourInvoice">Your invoice.</param>
        /// <param name="currentRmCoupon">The current rm coupon.</param>
        /// <param name="yourPrimeCouponRecords">Your prime coupon records.</param>
        /// <param name="samplingFormDRecords">The sampling form D records.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileSubmissionDate">The file submission date.</param>
        /// <returns></returns>
        public bool ValidateOriginalBillingAmountInRm(bool outcomeOfMismatchOnRmBilledOrAllowedAmounts,ExchangeRate prevExchangeRate,ExchangeRate currentExchangeRate, RejectionMemo currentRejectionMemoRecord, IList<IsValidationExceptionDetail> exceptionDetailsList, PaxInvoice currentInvoice, PaxInvoice yourInvoice, RMCoupon currentRmCoupon, IList<PrimeCoupon> yourPrimeCouponRecords, IList<SamplingFormDRecord> samplingFormDRecords, string fileName, DateTime fileSubmissionDate, bool isIsWeb = false, bool isBillingHistory = true, bool isErrorCorrection = false)
        {
            bool isValidAmount = true;
            Tolerance currentInvoiceTolerance = new Tolerance();
            var exchangeRateManager = Ioc.Resolve<IExchangeRateManager>(typeof(IExchangeRateManager));
            if (!exchangeRateManager.IsValidAttributeOfRejectedInvoice(yourInvoice.SettlementMethodId, yourInvoice.ListingCurrencyId.Value, yourInvoice.BillingCurrencyId.Value, Convert.ToDouble(yourInvoice.ExchangeRate), yourInvoice.BillingYear, yourInvoice.BillingMonth,prevExchangeRate))
            {
                return isValidAmount;
            }
            if (currentInvoice.Tolerance == null)
            {
                if (currentInvoice.ListingCurrencyId.HasValue)
                {

                    currentInvoiceTolerance = CompareUtil.GetTolerance(BillingCategoryType.Pax, currentInvoice.ListingCurrencyId.Value, currentInvoice, Constants.PaxDecimalPlaces);

                }
                else
                {
                    currentInvoiceTolerance = new Tolerance
                    {
                        ClearingHouse = CompareUtil.GetClearingHouse(currentInvoice.SettlementMethodId),
                        BillingCategoryId = (int)BillingCategoryType.Pax,
                        RoundingTolerance = 0,
                        SummationTolerance = 0
                    };
                }
            }
            else
            {
                currentInvoiceTolerance = currentInvoice.Tolerance;
            }

            string[] errorArgs = new string[] { currentRejectionMemoRecord.RejectionMemoNumber, Convert.ToString(currentRejectionMemoRecord.BatchSequenceNumber), Convert.ToString(currentRejectionMemoRecord.RecordSequenceWithinBatch) };

            if (currentInvoice.BillingCode == (int)BillingCode.NonSampling || currentInvoice.BillingCode == (int)BillingCode.SamplingFormXF)
            {
                if (currentInvoice.BillingCode == (int)BillingCode.NonSampling && currentRejectionMemoRecord.RejectionStage == 1)
                {
                    if ((currentRejectionMemoRecord.FIMBMCMIndicatorId == 0 || currentRejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.None) && (currentRejectionMemoRecord.CouponBreakdownRecord.Count > 0 || !isBillingHistory))
                    {
                        // Coupon Breakdown Level Validation - SC:1
                        #region Coupon Level Validation  - SC:1
                        IList<IsValidationExceptionDetail> amountExceptionDetailsList = new List<IsValidationExceptionDetail>();
                        string[] errorCorrArgs = new string[] { currentRmCoupon.TicketOrFimIssuingAirline, Convert.ToString(currentRmCoupon.TicketDocOrFimNumber), Convert.ToString(currentRmCoupon.TicketOrFimCouponNumber) };
                        foreach (var primeCoupon in yourPrimeCouponRecords)
                        {
                          //239034 - Warning errors in the validation report
                          isValidAmount = true;
                            amountExceptionDetailsList = new List<IsValidationExceptionDetail>();
                            //Validate Gross Amount Billed and Coupon Gross Value/Applicable Local Fare  
                            #region Validate Gross Amount Billed  - SC:1
                            if (IsAmountMismatch(currentInvoice, yourInvoice,prevExchangeRate,currentExchangeRate, Convert.ToDouble(primeCoupon.CouponGrossValueOrApplicableLocalFare),Convert.ToDouble(currentRmCoupon.GrossAmountBilled),currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnGross;
                                if (isIsWeb)
                                {
                                  errorCode = ErrorCodes.IsWebMismatchOnGross1;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnGross;
                                }
                              var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                              exceptionDetailsList.Count() + 1,
                                                                                              fileSubmissionDate,
                                                                                              "Gross Amount Billed",
                                                                                              Convert.ToString(currentRmCoupon.GrossAmountBilled),
                                                                                              currentInvoice,
                                                                                              fileName,
                                                                                              ErrorLevels.ErrorLevelRejectionMemoCoupon,
                                                                                              errorCode,
                                                                                              errorStatus,
                                                                                              currentRejectionMemoRecord,
                                                                                              false,
                                                                                              string.Format("{0}-{1}-{2}",
                                                                                                            currentRmCoupon.TicketOrFimIssuingAirline,
                                                                                                            currentRmCoupon.TicketDocOrFimNumber,
                                                                                                            currentRmCoupon.TicketOrFimCouponNumber));
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion
                            //Validate Tax Amount Billed and Coupon Tax Amount
                            #region Validate Tax Amount Billed  - SC:1

                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(primeCoupon.TaxAmount), Convert.ToDouble(currentRmCoupon.TaxAmountBilled), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnTax;
                                if (isIsWeb)
                                {
                                  errorCode = ErrorCodes.IsWebMismatchOnTax1;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnTax;
                                }
                              var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                              exceptionDetailsList.Count() + 1,
                                                                                              fileSubmissionDate,
                                                                                              "Tax Amount Billed",
                                                                                              Convert.ToString(currentRmCoupon.TaxAmountBilled),
                                                                                              currentInvoice,
                                                                                              fileName,
                                                                                              ErrorLevels.ErrorLevelRejectionMemoCoupon,
                                                                                              errorCode,
                                                                                              errorStatus,
                                                                                              currentRejectionMemoRecord,
                                                                                              false,
                                                                                              string.Format("{0}-{1}-{2}",
                                                                                                            currentRmCoupon.TicketOrFimIssuingAirline,
                                                                                                            currentRmCoupon.TicketDocOrFimNumber,
                                                                                                            currentRmCoupon.TicketOrFimCouponNumber));
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion
                            //Validate ISC Amount Allowed ad ISC Amount
                            #region Validate ISC Amount Allowed  - SC:1
                            
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(primeCoupon.IscAmount), Convert.ToDouble(currentRmCoupon.AllowedIscAmount), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnIsc;
                                if (isIsWeb)
                                {
                                  errorCode = ErrorCodes.IsWebMismatchOnIsc1;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnIsc;
                                }
                              var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                              exceptionDetailsList.Count() + 1,
                                                                                              fileSubmissionDate,
                                                                                              "ISC Amount Allowed",
                                                                                              Convert.ToString(currentRmCoupon.AllowedIscAmount),
                                                                                              currentInvoice,
                                                                                              fileName,
                                                                                              ErrorLevels.ErrorLevelRejectionMemoCoupon,
                                                                                              errorCode,
                                                                                              errorStatus,
                                                                                              currentRejectionMemoRecord,
                                                                                              false,
                                                                                              string.Format("{0}-{1}-{2}",
                                                                                                            currentRmCoupon.TicketOrFimIssuingAirline,
                                                                                                            currentRmCoupon.TicketDocOrFimNumber,
                                                                                                            currentRmCoupon.TicketOrFimCouponNumber));
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion
                            //Validate Other Commission Amount Allowed and Other Commission Amount
                            #region Validate Other Commission Amount Allowed  - SC:1
                            
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(primeCoupon.OtherCommissionAmount), Convert.ToDouble(currentRmCoupon.AllowedOtherCommission), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnOc;
                                if (isIsWeb)
                                {
                                  errorCode = ErrorCodes.IsWebMismatchOnOc1;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnOc;
                                }
                              var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                              exceptionDetailsList.Count() + 1,
                                                                                              fileSubmissionDate,
                                                                                              "Other Commission Amount Allowed",
                                                                                              Convert.ToString(currentRmCoupon.AllowedOtherCommission),
                                                                                              currentInvoice,
                                                                                              fileName,
                                                                                              ErrorLevels.ErrorLevelRejectionMemoCoupon,
                                                                                              errorCode,
                                                                                              errorStatus,
                                                                                              currentRejectionMemoRecord,
                                                                                              false,
                                                                                              string.Format("{0}-{1}-{2}",
                                                                                                            currentRmCoupon.TicketOrFimIssuingAirline,
                                                                                                            currentRmCoupon.TicketDocOrFimNumber,
                                                                                                            currentRmCoupon.TicketOrFimCouponNumber));
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion
                            //Validate Handling Fee Amount Allowed and Handling Fee Amount
                            #region Validate Handling Fee Amount Allowed  - SC:1

                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(primeCoupon.HandlingFeeAmount), Convert.ToDouble(currentRmCoupon.AllowedHandlingFee), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnHandlingFee;
                                if (isIsWeb)
                                {
                                  errorCode = ErrorCodes.IsWebMismatchOnHandlingFee1;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnHandlingFee;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Handling Fee Amount Allowed",
                                                                                                Convert.ToString(currentRmCoupon.AllowedHandlingFee),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemoCoupon,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord,
                                                                                              false,
                                                                                              string.Format("{0}-{1}-{2}",
                                                                                                            currentRmCoupon.TicketOrFimIssuingAirline,
                                                                                                            currentRmCoupon.TicketDocOrFimNumber,
                                                                                                            currentRmCoupon.TicketOrFimCouponNumber));
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion
                            //Validate UATP Amount Allowed and UATP Amount
                            #region Validate UATP Amount Allowed  - SC:1
                            
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(primeCoupon.UatpAmount), Convert.ToDouble(currentRmCoupon.AllowedUatpAmount), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnUatp;
                                if (isIsWeb)
                                {
                                  errorCode = ErrorCodes.IsWebMismatchOnUatp1;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnUatp;
                                }
                              var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                              exceptionDetailsList.Count() + 1,
                                                                                              fileSubmissionDate,
                                                                                              "UATP Amount Allowed",
                                                                                              Convert.ToString(currentRmCoupon.AllowedUatpAmount),
                                                                                              currentInvoice,
                                                                                              fileName,
                                                                                              ErrorLevels.ErrorLevelRejectionMemoCoupon,
                                                                                              errorCode,
                                                                                              errorStatus,
                                                                                              currentRejectionMemoRecord,
                                                                                              false,
                                                                                              string.Format("{0}-{1}-{2}",
                                                                                                            currentRmCoupon.TicketOrFimIssuingAirline,
                                                                                                            currentRmCoupon.TicketDocOrFimNumber,
                                                                                                            currentRmCoupon.TicketOrFimCouponNumber));
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion
                            //Validate VAT Amount Billed and VAT Amount
                            #region Validate VAT Amount Billed  - SC:1

                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(primeCoupon.VatAmount), Convert.ToDouble(currentRmCoupon.VatAmountBilled), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnVat;
                                if (isIsWeb)
                                {
                                  errorCode = ErrorCodes.IsWebMismatchOnVat1;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnVat;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "VAT Amount Billed",
                                                                                                Convert.ToString(currentRmCoupon.VatAmountBilled),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemoCoupon,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord,
                                                                                              false,
                                                                                              string.Format("{0}-{1}-{2}",
                                                                                                            currentRmCoupon.TicketOrFimIssuingAirline,
                                                                                                            currentRmCoupon.TicketDocOrFimNumber,
                                                                                                            currentRmCoupon.TicketOrFimCouponNumber));
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion
                            if (isValidAmount) break;
                        }

                        //239034 - Warning errors in the validation report
                        if (!isValidAmount)
                        {
                          foreach (var isValidationExceptionDetail in amountExceptionDetailsList)
                          {
                            isValidationExceptionDetail.SerialNo = exceptionDetailsList.Count + 1;
                            if (isErrorCorrection)
                              isValidationExceptionDetail.ErrorDescription =
                                string.Format(isValidationExceptionDetail.ErrorDescription, errorCorrArgs);
                            exceptionDetailsList.Add(isValidationExceptionDetail);
                          }
                          if (!outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                          {
                            isValidAmount = true;
                          }
                        }
                      //End Coupon Level Validation  - SC:1
                        #endregion
                    }
                    if (currentRejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.FIMNumber)
                    {
                        //Memo level validation
                        //Validates amounts of current RM with FIM coupons (Prime).
                        #region FIM coupons Level Validation - SC:2
                        IList<IsValidationExceptionDetail> amountExceptionDetailsList = new List<IsValidationExceptionDetail>();
                        foreach (var primeCoupon in yourPrimeCouponRecords)
                        {
                          //239034 - Warning errors in the validation report
                          isValidAmount = true;
                            amountExceptionDetailsList = new List<IsValidationExceptionDetail>();
                            //Validate Total Gross Amount Billed and Coupon Gross Value/Applicable Local Fare
                            #region Validate Total Gross Amount Billed  - SC:2
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(primeCoupon.CouponGrossValueOrApplicableLocalFare), Convert.ToDouble(currentRejectionMemoRecord.TotalGrossAmountBilled), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnGross;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnGross;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnTotalGrossBilled;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Gross Amount Billed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.TotalGrossAmountBilled),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            //Validate Total Tax Amount Billed and Coupon Tax Amount
                            #region Validate Total Tax Amount Billed - SC:2
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(primeCoupon.TaxAmount), Convert.ToDouble(currentRejectionMemoRecord.TotalTaxAmountBilled), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnTax;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnTax;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnTaxBilled;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total Tax Amount Billed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.TotalTaxAmountBilled),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            //Validate Total ISC Amount Allowed and ISC Amount
                            #region Validate Total ISC Amount  - SC:2
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(primeCoupon.IscAmount), Convert.ToDouble(currentRejectionMemoRecord.AllowedIscAmount), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnIsc;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnIsc;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnIscAllowed;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total ISC Amount Allowed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.AllowedIscAmount),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            //Validate Total Other Commission Amount Allowed and Other Commission Amount
                            #region Validate Total Other Commission Amount Allowed  - SC:2
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(primeCoupon.OtherCommissionAmount), Convert.ToDouble(currentRejectionMemoRecord.AllowedOtherCommission), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnOc;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnOc;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnOcAllowed;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total Other Commission Amount Allowed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.AllowedOtherCommission),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            //Validate Total Handling Fee Amount Allowed and Handling Fee Amount
                            #region Validate Total Handling Fee Amount Allowed - SC:2
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(primeCoupon.HandlingFeeAmount), Convert.ToDouble(currentRejectionMemoRecord.AllowedHandlingFee), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnHandlingFee;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnHandlingFee;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnHandlingFeeAllowed;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total Handling Fee Amount Allowed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.AllowedHandlingFee),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            //Validate Total UATP Amount Allowed and UATP Amount
                            #region Validate Total UATP Amount Allowed - SC:2
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(primeCoupon.UatpAmount), Convert.ToDouble(currentRejectionMemoRecord.AllowedUatpAmount), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnUatp;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnUatp;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnUatpAllowed;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total UATP Amount Allowed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.AllowedUatpAmount),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            //Validate Total VAT Amount Billed and VAT Amount
                            #region Validate Total VAT Amount Billed - SC:2
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(primeCoupon.VatAmount), Convert.ToDouble(currentRejectionMemoRecord.TotalVatAmountBilled), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnVat;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnVat;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnVatBilled;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total VAT Amount Billed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.TotalVatAmountBilled),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            if (isValidAmount) break;
                        }
                        
                        //239034 - Warning errors in the validation report
                        if (!isValidAmount)
                        {
                          foreach (var isValidationExceptionDetail in amountExceptionDetailsList)
                          {
                            isValidationExceptionDetail.SerialNo = exceptionDetailsList.Count + 1;
                            if (isIsWeb && !isErrorCorrection)
                              isValidationExceptionDetail.ErrorDescription =
                                string.Format(isValidationExceptionDetail.ErrorDescription, errorArgs);
                            exceptionDetailsList.Add(isValidationExceptionDetail);
                          }
                          if (!outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                          {
                            isValidAmount = true;
                          }
                        }

                      #endregion
                    }
                    if (currentRejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.BMNumber)
                    {
                        //Memo level validation
                        //Validates amounts of current RM with Billing Memo
                        #region Billing Memo level validation - SC:3
                        IList<IsValidationExceptionDetail> amountExceptionDetailsList = new List<IsValidationExceptionDetail>();
                        var billingMemos = yourInvoice.BillingMemoRecord.Where(billingMemo => billingMemo.BillingMemoNumber != null && billingMemo.BillingMemoNumber.Trim().ToUpper() == currentRejectionMemoRecord.FimBMCMNumber.Trim().ToUpper()).ToList();
                        foreach (var bm in billingMemos)
                        {
                          //239034 - Warning errors in the validation report
                          isValidAmount = true;
                          
                          amountExceptionDetailsList = new List<IsValidationExceptionDetail>();
                            //Validate Total Gross Amount Billed and Total Gross Amount Billed / Credited 
                            #region Validate Total Gross Amount Billed   - SC:3
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(bm.TotalGrossAmountBilled), Convert.ToDouble(currentRejectionMemoRecord.TotalGrossAmountBilled), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnTotalGross;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnTotalGross;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnTotalGross;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total Gross Amount Billed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.TotalGrossAmountBilled),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            //Validate Total Tax Amount Billed and Total Tax Amount Billed / Credited
                            #region Validate Total Tax Amount Billed - SC:3
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(bm.TaxAmountBilled), Convert.ToDouble(currentRejectionMemoRecord.TotalTaxAmountBilled), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnTotalTax;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnTotalTax;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnTotalTax;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total Tax Amount Billed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.TotalTaxAmountBilled),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            //Validate Total ISC Amount Allowed and Total ISC Amount Billed / Credited
                            #region Validate Total ISC Amount Allowed - SC:3
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(bm.TotalIscAmountBilled), Convert.ToDouble(currentRejectionMemoRecord.AllowedIscAmount), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnTotalIsc;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnTotalIsc;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnTotalIsc;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total ISC Amount Allowed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.AllowedIscAmount),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            //Validate Total Other Commission Amount Allowed and Total Other Commission Amount Billed / Credited
                            #region Validate Total Other Commission Amount Allowed - SC:3
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(bm.TotalOtherCommissionAmount), Convert.ToDouble(currentRejectionMemoRecord.AllowedOtherCommission), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnTotalOc;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnTotalOc;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnTotalOc;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total Other Commission Amount Allowed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.AllowedOtherCommission),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            //Validate Total Total Handling Fee Amount Allowed and Total Handling Fee Amount Billed / Credited
                            #region Validate Total Total Handling Fee Amount Allowed - SC:3
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(bm.TotalHandlingFeeBilled), Convert.ToDouble(currentRejectionMemoRecord.AllowedHandlingFee), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnTotalHandlingFee;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnTotalHandlingFee;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnTotalHandlingFee;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total Handling Fee Amount Allowed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.AllowedHandlingFee),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            //Validate Total Total UATP Amount Allowed and Total UATP Amount Billed / Credited
                            #region Validate Total Total UATP Amount Allowed - SC:3
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(bm.TotalUatpAmountBilled), Convert.ToDouble(currentRejectionMemoRecord.AllowedUatpAmount), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnTotalUatp;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnTotalUatp;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnTotalUatp;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total UATP Amount Allowed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.AllowedUatpAmount),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            //Validate Total VAT Amount Billed and Total VAT Amount Billed / Credited
                            #region Validate Total VAT Amount Billed - SC:3
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(bm.TotalVatAmountBilled), Convert.ToDouble(currentRejectionMemoRecord.TotalVatAmountBilled), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnTotalVat;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnTotalVat;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnTotalVat;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total VAT Amount Billed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.TotalVatAmountBilled),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            if (isValidAmount) break;
                        }

                        //239034 - Warning errors in the validation report
                        if (!isValidAmount)
                        {
                          foreach (var isValidationExceptionDetail in amountExceptionDetailsList)
                          {
                            isValidationExceptionDetail.SerialNo = exceptionDetailsList.Count + 1;
                            if (isIsWeb)
                              isValidationExceptionDetail.ErrorDescription =
                                string.Format(isValidationExceptionDetail.ErrorDescription, errorArgs);
                            exceptionDetailsList.Add(isValidationExceptionDetail);
                          }
                          if (!outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                          {
                            isValidAmount = true;
                          }
                        }

                      #endregion
                    }
                    if (currentRejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.CMNumber)
                    {
                        //Memo level validation
                        //Validates amounts of current RM with Credit memo
                        #region Credit Memo level validation - SC:4
                        IList<IsValidationExceptionDetail> amountExceptionDetailsList = new List<IsValidationExceptionDetail>();
                        var creditMemos = yourInvoice.CreditMemoRecord.Where(creditMemo => creditMemo.CreditMemoNumber.Trim().ToUpper() == currentRejectionMemoRecord.FimBMCMNumber.ToUpper()).ToList();
                        foreach (var cm in creditMemos)
                        {
                          //239034 - Warning errors in the validation report
                          isValidAmount = true;

                            amountExceptionDetailsList = new List<IsValidationExceptionDetail>();
                            // Coupon Breakdown Level Validation
                            //Validate Total Gross Amount Billed and Total Gross Amount Billed / Credited 
                            #region Validate Total Gross Amount Billed - SC:4
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(cm.TotalGrossAmountCredited), Convert.ToDouble(currentRejectionMemoRecord.TotalGrossAmountBilled), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnTotalGross;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnTotalGross;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnTotalGross;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total Gross Amount Billed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.TotalGrossAmountBilled),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            //Validate Total Tax Amount Billed and Total Tax Amount Billed / Credited
                            #region Validate Total Tax Amount Billed - SC:4
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(cm.TaxAmount), Convert.ToDouble(currentRejectionMemoRecord.TotalTaxAmountBilled), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnTotalTax;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnTotalTax;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnTotalTax;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total Tax Amount Billed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.TotalTaxAmountBilled),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            //Validate Total ISC Amount Allowed and Total ISC Amount Billed / Credited
                            #region Validate Total ISC Amount Allowed - SC:4
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(cm.TotalIscAmountCredited), Convert.ToDouble(currentRejectionMemoRecord.AllowedIscAmount), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnTotalIsc;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnTotalIsc;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnTotalIsc;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total ISC Amount Allowed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.AllowedIscAmount),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            //Validate Total Other Commission Amount Allowed and Total Other Commission Amount Billed / Credited
                            #region Validate Total Other Commission Amount Allowed - SC:4
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(cm.TotalOtherCommissionAmountCredited), Convert.ToDouble(currentRejectionMemoRecord.AllowedOtherCommission), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnTotalOc;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnTotalOc;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnTotalOc;
                                }

                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total Other Commission Amount Allowed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.AllowedOtherCommission),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            //Validate Total Handling Fee Amount Allowed and Total Handling Fee Amount Billed / Credited
                            #region Validate Total Handling Fee Amount Allowed - SC:4
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(cm.TotalHandlingFeeCredited), Convert.ToDouble(currentRejectionMemoRecord.AllowedHandlingFee), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnTotalHandlingFee;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnTotalHandlingFee;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnTotalHandlingFee;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total Handling Fee Amount Allowed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.AllowedHandlingFee),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            //Validate Total UATP Amount Allowed and Total UATP Amount Billed / Credited
                            #region Validate Total UATP Amount Allowed - SC:4
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(cm.TotalUatpAmountCredited), Convert.ToDouble(currentRejectionMemoRecord.AllowedUatpAmount), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnTotalUatp;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnTotalUatp;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnTotalUatp;
                                }

                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total UATP Amount Allowed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.AllowedUatpAmount),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            //Validate Total VAT Amount Billed and Total VAT Amount Billed / Credited
                            #region Validate Total VAT Amount Billed - SC:4
                            if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(cm.VatAmount), Convert.ToDouble(currentRejectionMemoRecord.TotalVatAmountBilled), currentInvoiceTolerance))
                            {
                                isValidAmount = false;
                                ErrorStatus errorStatus = ErrorStatus.W;
                                if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                                {
                                    errorStatus = ErrorStatus.X;
                                }
                                var errorCode = ErrorCodes.MismatchOnTotalVat;
                                if (isIsWeb)
                                {
                                    errorCode = ErrorCodes.IsWebMismatchOnTotalVat;
                                }
                                if (isErrorCorrection)
                                {
                                    errorCode = ErrorCodes.ErrCorrMismatchOnTotalVat;
                                }
                                var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Total VAT Amount Billed",
                                                                                                Convert.ToString(currentRejectionMemoRecord.TotalVatAmountBilled),
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                errorCode,
                                                                                                errorStatus,
                                                                                                currentRejectionMemoRecord);
                                amountExceptionDetailsList.Add(validationExceptionDetail);
                            }

                            #endregion

                            if (isValidAmount) break;
                        }

                        //239034 - Warning errors in the validation report
                        if (!isValidAmount)
                        {
                          foreach (var isValidationExceptionDetail in amountExceptionDetailsList)
                          {
                            isValidationExceptionDetail.SerialNo = exceptionDetailsList.Count + 1;
                            if (isIsWeb)
                              isValidationExceptionDetail.ErrorDescription =
                                string.Format(isValidationExceptionDetail.ErrorDescription, errorArgs);
                            exceptionDetailsList.Add(isValidationExceptionDetail);
                          }
                          if (!outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                          {
                            isValidAmount = true;
                          }
                        }

                      #endregion
                    }
                }
                if (currentRejectionMemoRecord.RejectionStage == 2 || currentRejectionMemoRecord.RejectionStage == 3)
                {
                    //Memo level validation
                    //Validates amounts of current RM with your RM
                    #region Rejection Memo level validation - SC:5/6/8
                    IList<IsValidationExceptionDetail> amountExceptionDetailsList = new List<IsValidationExceptionDetail>();
                    var rejectionMemos = yourInvoice.RejectionMemoRecord.Where(rejectionRec => rejectionRec.RejectionMemoNumber == currentRejectionMemoRecord.YourRejectionNumber).ToList();

                    foreach (var rm in rejectionMemos)
                    {
                      //239034 - Warning errors in the validation report
                      isValidAmount = true;

                        amountExceptionDetailsList = new List<IsValidationExceptionDetail>();
                        // Coupon Breakdown Level Validation
                        //Validate Total Gross Amount Billed and Total Gross Amount Accepted 
                        #region Validate Total Gross Amount Billed - SC:5/6/8
                        if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(rm.TotalGrossAcceptedAmount), Convert.ToDouble(currentRejectionMemoRecord.TotalGrossAmountBilled), currentInvoiceTolerance))
                        {
                            isValidAmount = false;
                            ErrorStatus errorStatus = ErrorStatus.W;
                            if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                            {
                                errorStatus = ErrorStatus.X;
                            }
                            var errorCode = ErrorCodes.MismatchOnAcceptedTotalGross;
                            if (isIsWeb)
                            {
                                errorCode = ErrorCodes.IsWebMismatchOnAcceptedTotalGross;
                            }
                            if (isErrorCorrection)
                            {
                                errorCode = ErrorCodes.ErrCorrMismatchOnAcceptedTotalGross;
                            }
                            var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                            exceptionDetailsList.Count() + 1,
                                                                                            fileSubmissionDate,
                                                                                            "Total Gross Amount Billed",
                                                                                            Convert.ToString(currentRejectionMemoRecord.TotalGrossAmountBilled),
                                                                                            currentInvoice,
                                                                                            fileName,
                                                                                            ErrorLevels.ErrorLevelRejectionMemo,
                                                                                            errorCode,
                                                                                            errorStatus,
                                                                                            currentRejectionMemoRecord);
                            amountExceptionDetailsList.Add(validationExceptionDetail);
                        }

                        #endregion

                        //Validate Total Tax Amount Billed and Total Tax Amount Accepted
                        #region Validate Total Tax Amount Billed - SC:5/6/8
                        if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(rm.TotalTaxAmountAccepted), Convert.ToDouble(currentRejectionMemoRecord.TotalTaxAmountBilled), currentInvoiceTolerance))
                        {
                            isValidAmount = false;
                            ErrorStatus errorStatus = ErrorStatus.W;
                            if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                            {
                                errorStatus = ErrorStatus.X;
                            }
                            var errorCode = ErrorCodes.MismatchOnAcceptedTotalTax;
                            if (isIsWeb)
                            {
                                errorCode = ErrorCodes.IsWebMismatchOnAcceptedTotalTax;
                            }
                            if (isErrorCorrection)
                            {
                                errorCode = ErrorCodes.ErrCorrMismatchOnAcceptedTotalTax;
                            }
                            var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                            exceptionDetailsList.Count() + 1,
                                                                                            fileSubmissionDate,
                                                                                            "Total Tax Amount Billed",
                                                                                            Convert.ToString(currentRejectionMemoRecord.TotalTaxAmountBilled),
                                                                                            currentInvoice,
                                                                                            fileName,
                                                                                            ErrorLevels.ErrorLevelRejectionMemo,
                                                                                            errorCode,
                                                                                            errorStatus,
                                                                                            currentRejectionMemoRecord);
                            amountExceptionDetailsList.Add(validationExceptionDetail);
                        }

                        #endregion

                        //Validate Total ISC Amount Allowed and Total ISC Amount Accepted
                        #region Validate Total ISC Amount Allowed - SC:5/6/8
                        if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(rm.AcceptedIscAmount), Convert.ToDouble(currentRejectionMemoRecord.AllowedIscAmount), currentInvoiceTolerance))
                        {
                            isValidAmount = false;
                            ErrorStatus errorStatus = ErrorStatus.W;
                            if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                            {
                                errorStatus = ErrorStatus.X;
                            }
                            var errorCode = ErrorCodes.MismatchOnAcceptedTotalIsc;
                            if (isIsWeb)
                            {
                                errorCode = ErrorCodes.IsWebMismatchOnAcceptedTotalIsc;
                            }
                            if (isErrorCorrection)
                            {
                                errorCode = ErrorCodes.ErrCorrMismatchOnAcceptedTotalIsc;
                            }
                            var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                            exceptionDetailsList.Count() + 1,
                                                                                            fileSubmissionDate,
                                                                                            "Total ISC Amount Allowed",
                                                                                            Convert.ToString(currentRejectionMemoRecord.AllowedIscAmount),
                                                                                            currentInvoice,
                                                                                            fileName,
                                                                                            ErrorLevels.ErrorLevelRejectionMemo,
                                                                                            errorCode,
                                                                                            errorStatus,
                                                                                            currentRejectionMemoRecord);
                            amountExceptionDetailsList.Add(validationExceptionDetail);

                        }

                        #endregion

                        //Validate Total Other Commission Amount Allowed and Total Other Commission Amount Accepted
                        #region Validate Total Other Commission Amount Allowed - SC:5/6/8
                        if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(rm.AcceptedOtherCommission), Convert.ToDouble(currentRejectionMemoRecord.AllowedOtherCommission), currentInvoiceTolerance))
                        {
                            isValidAmount = false;
                            ErrorStatus errorStatus = ErrorStatus.W;
                            if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                            {
                                errorStatus = ErrorStatus.X;
                            }
                            var errorCode = ErrorCodes.MismatchOnAcceptedTotalOc;
                            if (isIsWeb)
                            {
                                errorCode = ErrorCodes.IsWebMismatchOnAcceptedTotalOc;
                            }
                            if (isErrorCorrection)
                            {
                                errorCode = ErrorCodes.ErrCorrMismatchOnAcceptedTotalOc;
                            }
                            var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                            exceptionDetailsList.Count() + 1,
                                                                                            fileSubmissionDate,
                                                                                            "Total Other Commission Amount Allowed",
                                                                                            Convert.ToString(currentRejectionMemoRecord.AllowedOtherCommission),
                                                                                            currentInvoice,
                                                                                            fileName,
                                                                                            ErrorLevels.ErrorLevelRejectionMemo,
                                                                                            errorCode,
                                                                                            errorStatus,
                                                                                            currentRejectionMemoRecord);
                            amountExceptionDetailsList.Add(validationExceptionDetail);
                        }

                        #endregion

                        //Validate Total Handling Fee Amount Allowed and Total Handling Fee Amount Accepted
                        #region Validate Total Handling Fee Amount Allowed - SC:5/6/8
                        if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(rm.AcceptedHandlingFee), Convert.ToDouble(currentRejectionMemoRecord.AllowedHandlingFee), currentInvoiceTolerance))
                        {
                            isValidAmount = false;
                            ErrorStatus errorStatus = ErrorStatus.W;
                            if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                            {
                                errorStatus = ErrorStatus.X;
                            }
                            var errorCode = ErrorCodes.MismatchOnAcceptedTotalHandlingFee;
                            if (isIsWeb)
                            {
                                errorCode = ErrorCodes.IsWebMismatchOnAcceptedTotalHandlingFee;
                            }
                            if (isErrorCorrection)
                            {
                                errorCode = ErrorCodes.ErrCorrMismatchOnAcceptedTotalHandlingFee;
                            }
                            var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                            exceptionDetailsList.Count() + 1,
                                                                                            fileSubmissionDate,
                                                                                            "Total Handling Fee Amount Allowed",
                                                                                            Convert.ToString(currentRejectionMemoRecord.AllowedHandlingFee),
                                                                                            currentInvoice,
                                                                                            fileName,
                                                                                            ErrorLevels.ErrorLevelRejectionMemo,
                                                                                            errorCode,
                                                                                            errorStatus,
                                                                                            currentRejectionMemoRecord);
                            amountExceptionDetailsList.Add(validationExceptionDetail);
                        }

                        #endregion

                        //Validate Total UATP Amount Allowed and Total UATP Amount Accepted
                        #region Validate Total UATP Amount Allowed - SC:5/6/8
                        if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(rm.AcceptedUatpAmount), Convert.ToDouble(currentRejectionMemoRecord.AllowedUatpAmount), currentInvoiceTolerance))
                        {
                            isValidAmount = false;
                            ErrorStatus errorStatus = ErrorStatus.W;
                            if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                            {
                                errorStatus = ErrorStatus.X;
                            }
                            var errorCode = ErrorCodes.MismatchOnAcceptedTotalUatp;
                            if (isIsWeb)
                            {
                                errorCode = ErrorCodes.IsWebMismatchOnAcceptedTotalUatp;
                            }
                            if (isErrorCorrection)
                            {
                                errorCode = ErrorCodes.ErrCorrMismatchOnAcceptedTotalUatp;
                            }
                            var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                            exceptionDetailsList.Count() + 1,
                                                                                            fileSubmissionDate,
                                                                                            "Total UATP Amount Allowed",
                                                                                            Convert.ToString(currentRejectionMemoRecord.AllowedUatpAmount),
                                                                                            currentInvoice,
                                                                                            fileName,
                                                                                            ErrorLevels.ErrorLevelRejectionMemo,
                                                                                            errorCode,
                                                                                            errorStatus,
                                                                                            currentRejectionMemoRecord);
                            amountExceptionDetailsList.Add(validationExceptionDetail);
                        }

                        #endregion

                        //Validate Total VAT Amount Billed and Total VAT Amount Accepted
                        #region Validate Total VAT Amount Billed - SC:5/6/8
                        if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(rm.TotalVatAmountAccepted), Convert.ToDouble(currentRejectionMemoRecord.TotalVatAmountBilled), currentInvoiceTolerance))
                        {
                            isValidAmount = false;
                            ErrorStatus errorStatus = ErrorStatus.W;
                            if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                            {
                                errorStatus = ErrorStatus.X;
                            }
                            var errorCode = ErrorCodes.MismatchOnAcceptedTotalVat;
                            if (isIsWeb)
                            {
                                errorCode = ErrorCodes.IsWebMismatchOnAcceptedTotalVat;
                            }
                            if (isErrorCorrection)
                            {
                                errorCode = ErrorCodes.ErrCorrMismatchOnAcceptedTotalVat;
                            }
                            var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                            exceptionDetailsList.Count() + 1,
                                                                                            fileSubmissionDate,
                                                                                            "Total VAT Amount Billed",
                                                                                            Convert.ToString(currentRejectionMemoRecord.TotalVatAmountBilled),
                                                                                            currentInvoice,
                                                                                            fileName,
                                                                                            ErrorLevels.ErrorLevelRejectionMemo,
                                                                                            errorCode,
                                                                                            errorStatus,
                                                                                            currentRejectionMemoRecord);
                            amountExceptionDetailsList.Add(validationExceptionDetail);
                        }

                        #endregion

                        if (isValidAmount) break;
                    }

                    //239034 - Warning errors in the validation report
                    if (!isValidAmount)
                    {
                      foreach (var isValidationExceptionDetail in amountExceptionDetailsList)
                      {
                        isValidationExceptionDetail.SerialNo = exceptionDetailsList.Count + 1;
                        if (isIsWeb)
                          isValidationExceptionDetail.ErrorDescription =
                            string.Format(isValidationExceptionDetail.ErrorDescription, errorArgs);
                        exceptionDetailsList.Add(isValidationExceptionDetail);
                      }
                      if (!outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                      {
                        isValidAmount = true;
                      }
                    }

                  #endregion
                }
            }

            //SCP289215 - UA Ticket 618 729 0229461 cpn 1
            //Replace Rejection memo coupon RM Coupon Record with CurrentRmCoupon Record.
            if (currentInvoice.BillingCode == (int)BillingCode.SamplingFormF && currentRmCoupon!=null)
            {
                // Coupon Breakdown Level Validation 
                //Validate amounts of current RM Coupon form D coupons.
                #region Sampling form D coupon level validation - SC:7
                IList<IsValidationExceptionDetail> amountExceptionDetailsList = new List<IsValidationExceptionDetail>();
                string[] errorCorrArgs = new string[] { currentRmCoupon.TicketOrFimIssuingAirline, Convert.ToString(currentRmCoupon.TicketDocOrFimNumber), Convert.ToString(currentRmCoupon.TicketOrFimCouponNumber) };
                foreach (var formDRecord in samplingFormDRecords)
                {
                   //239034 - Warning errors in the validation report
                    isValidAmount = true;

                    amountExceptionDetailsList = new List<IsValidationExceptionDetail>();
                    //Validate Gross Amount Billed and Evaluated Gross Amount
                    #region Validate Gross Amount Billed - SC:7
                    if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(formDRecord.EvaluatedGrossAmount), Convert.ToDouble(currentRmCoupon.GrossAmountBilled), currentInvoiceTolerance))
                    {
                        isValidAmount = false;
                        ErrorStatus errorStatus = ErrorStatus.W;
                        if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                        {
                            errorStatus = ErrorStatus.X;
                        }
                        var errorCode = ErrorCodes.FileMismatchOnEvaluatedGross;
                        if (isIsWeb)
                        {
                          errorCode = ErrorCodes.MismatchOnEvaluatedGross;
                        }
                        if (isErrorCorrection)
                        {
                            errorCode = ErrorCodes.ErrCorrMismatchOnEvaluatedGross;
                        }
                        var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                        exceptionDetailsList.Count() + 1,
                                                                                        fileSubmissionDate,
                                                                                        "Gross Amount Billed",
                                                                                        Convert.ToString(currentRmCoupon.GrossAmountBilled),
                                                                                        currentInvoice,
                                                                                        fileName,
                                                                                        ErrorLevels.ErrorLevelRejectionMemo,
                                                                                        errorCode,
                                                                                        errorStatus,
                                                                                        currentRejectionMemoRecord);
                        amountExceptionDetailsList.Add(validationExceptionDetail);
                    }

                    #endregion

                    //Validate Tax Amount Billed and Evaluated Tax Amount
                    #region Validate Tax Amount Billed - SC:7
                    if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(formDRecord.TaxAmount), Convert.ToDouble(currentRmCoupon.TaxAmountBilled), currentInvoiceTolerance))
                    {
                        isValidAmount = false;
                        ErrorStatus errorStatus = ErrorStatus.W;
                        if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                        {
                            errorStatus = ErrorStatus.X;
                        }
                        var errorCode = ErrorCodes.FileMismatchOnEvaluatedTax;
                        if (isIsWeb)
                        {
                          errorCode = ErrorCodes.MismatchOnEvaluatedTax;
                        }
                        if (isErrorCorrection)
                        {
                            errorCode = ErrorCodes.ErrCorrMismatchOnEvaluatedTax;
                        }
                        var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                        exceptionDetailsList.Count() + 1,
                                                                                        fileSubmissionDate,
                                                                                        "Tax Amount Billed",
                                                                                        Convert.ToString(currentRmCoupon.TaxAmountBilled),
                                                                                        currentInvoice,
                                                                                        fileName,
                                                                                        ErrorLevels.ErrorLevelRejectionMemo,
                                                                                        errorCode,
                                                                                        errorStatus,
                                                                                        currentRejectionMemoRecord);
                        amountExceptionDetailsList.Add(validationExceptionDetail);
                    }

                    #endregion

                    //Validate ISC Amount Allowed	Evaluated and ISC Amount
                    #region Validate ISC Amount Allowed - SC:7
                    if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(formDRecord.IscAmount), Convert.ToDouble(currentRmCoupon.AllowedIscAmount), currentInvoiceTolerance))
                    {
                        isValidAmount = false;
                        ErrorStatus errorStatus = ErrorStatus.W;
                        if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                        {
                            errorStatus = ErrorStatus.X;
                        }
                        var errorCode = ErrorCodes.FileMismatchOnEvaluatedIsc;
                        if (isIsWeb)
                        {
                          errorCode = ErrorCodes.MismatchOnEvaluatedIsc;
                        }
                        if (isErrorCorrection)
                        {
                            errorCode = ErrorCodes.ErrCorrMismatchOnEvaluatedIsc;
                        }
                        var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                        exceptionDetailsList.Count() + 1,
                                                                                        fileSubmissionDate,
                                                                                        "ISC Amount Allowed",
                                                                                        Convert.ToString(currentRmCoupon.AllowedIscAmount),
                                                                                        currentInvoice,
                                                                                        fileName,
                                                                                        ErrorLevels.ErrorLevelRejectionMemo,
                                                                                        errorCode,
                                                                                        errorStatus,
                                                                                        currentRejectionMemoRecord);
                        amountExceptionDetailsList.Add(validationExceptionDetail);
                    }

                    #endregion

                    //Validate Other Commission Amount Allowed and Evaluated Other Commission Amount
                    #region Validate Other Commission Amount Allowed - SC:7
                    if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(formDRecord.OtherCommissionAmount), Convert.ToDouble(currentRmCoupon.AllowedOtherCommission), currentInvoiceTolerance))
                    {
                        isValidAmount = false;
                        ErrorStatus errorStatus = ErrorStatus.W;
                        if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                        {
                            errorStatus = ErrorStatus.X;
                        }
                        var errorCode = ErrorCodes.FileMismatchOnEvaluatedOc;
                        if (isIsWeb)
                        {
                          errorCode = ErrorCodes.MismatchOnEvaluatedOc;
                        }
                        if (isErrorCorrection)
                        {
                            errorCode = ErrorCodes.ErrCorrMismatchOnEvaluatedOc;
                        }

                        var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                        exceptionDetailsList.Count() + 1,
                                                                                        fileSubmissionDate,
                                                                                        "Other Commission Amount Allowed",
                                                                                        Convert.ToString(currentRmCoupon.AllowedOtherCommission),
                                                                                        currentInvoice,
                                                                                        fileName,
                                                                                        ErrorLevels.ErrorLevelRejectionMemo,
                                                                                        errorCode,
                                                                                        errorStatus,
                                                                                        currentRejectionMemoRecord);
                        amountExceptionDetailsList.Add(validationExceptionDetail);
                    }

                    #endregion

                    //Validate Handling Fee Amount Allowed and Evaluated Handling Fee Amount
                    #region Validate Handling Fee Amount Allowed - SC:7
                    if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(formDRecord.HandlingFeeAmount), Convert.ToDouble(currentRmCoupon.AllowedHandlingFee), currentInvoiceTolerance))
                    {
                        isValidAmount = false;
                        ErrorStatus errorStatus = ErrorStatus.W;
                        if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                        {
                            errorStatus = ErrorStatus.X;
                        }
                        var errorCode = ErrorCodes.FileMismatchOnEvaluatedHandlingFee;
                        if (isIsWeb)
                        {
                          errorCode = ErrorCodes.MismatchOnEvaluatedHandlingFee;
                        }
                        if (isErrorCorrection)
                        {
                            errorCode = ErrorCodes.ErrCorrMismatchOnEvaluatedHandlingFee;
                        }

                        var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                        exceptionDetailsList.Count() + 1,
                                                                                        fileSubmissionDate,
                                                                                        "Handling Fee Amount Allowed",
                                                                                        Convert.ToString(currentRmCoupon.AllowedHandlingFee),
                                                                                        currentInvoice,
                                                                                        fileName,
                                                                                        ErrorLevels.ErrorLevelRejectionMemo,
                                                                                        errorCode,
                                                                                        errorStatus,
                                                                                        currentRejectionMemoRecord);
                        amountExceptionDetailsList.Add(validationExceptionDetail);
                    }

                    #endregion

                    //Validate UATP Amount Allowed and Evaluated UATP Amount
                    #region Validate UATP Amount Allowed - SC:7
                    if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(formDRecord.UatpAmount), Convert.ToDouble(currentRmCoupon.AllowedUatpAmount), currentInvoiceTolerance))
                    {
                        isValidAmount = false;
                        ErrorStatus errorStatus = ErrorStatus.W;
                        if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                        {
                            errorStatus = ErrorStatus.X;
                        }
                        var errorCode = ErrorCodes.FileMismatchOnEvaluatedUatp;
                        if (isIsWeb)
                        {
                          errorCode = ErrorCodes.MismatchOnEvaluatedUatp;
                        }
                        if (isErrorCorrection)
                        {
                            errorCode = ErrorCodes.ErrCorrMismatchOnEvaluatedUatp;
                        }
                        var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                        exceptionDetailsList.Count() + 1,
                                                                                        fileSubmissionDate,
                                                                                        "UATP Amount Allowed",
                                                                                        Convert.ToString(currentRmCoupon.AllowedUatpAmount),
                                                                                        currentInvoice,
                                                                                        fileName,
                                                                                        ErrorLevels.ErrorLevelRejectionMemo,
                                                                                        errorCode,
                                                                                        errorStatus,
                                                                                        currentRejectionMemoRecord);
                        amountExceptionDetailsList.Add(validationExceptionDetail);
                    }

                    #endregion

                    //Validate VAT Amount Billed and Evaluated VAT Amount
                    #region Validate VAT Amount Billed - SC:7
                    if (IsAmountMismatch(currentInvoice, yourInvoice, prevExchangeRate, currentExchangeRate, Convert.ToDouble(formDRecord.VatAmount), Convert.ToDouble(currentRmCoupon.VatAmountBilled), currentInvoiceTolerance))
                    {
                        isValidAmount = false;
                        ErrorStatus errorStatus = ErrorStatus.W;
                        if (outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                        {
                            errorStatus = ErrorStatus.X;
                        }
                        var errorCode = ErrorCodes.FileMismatchOnEvaluatedVat;
                        if (isIsWeb)
                        {
                          errorCode = ErrorCodes.MismatchOnEvaluatedVat;
                        }
                        if (isErrorCorrection)
                        {
                            errorCode = ErrorCodes.ErrCorrMismatchOnEvaluatedVat;
                        }
                        var validationExceptionDetail = CreateValidationExceptionDetail(currentRejectionMemoRecord.Id.Value(),
                                                                                        exceptionDetailsList.Count() + 1,
                                                                                        fileSubmissionDate,
                                                                                        "VAT Amount Billed",
                                                                                        Convert.ToString(currentRmCoupon.VatAmountBilled),
                                                                                        currentInvoice,
                                                                                        fileName,
                                                                                        ErrorLevels.ErrorLevelRejectionMemo,
                                                                                        errorCode,
                                                                                        errorStatus,
                                                                                        currentRejectionMemoRecord);
                        amountExceptionDetailsList.Add(validationExceptionDetail);
                    }

                    #endregion

                    if (isValidAmount) break;
                }

                //239034 - Warning errors in the validation report
                if (!isValidAmount)
                {
                  foreach (var isValidationExceptionDetail in amountExceptionDetailsList)
                  {
                    isValidationExceptionDetail.SerialNo = exceptionDetailsList.Count + 1;
                    if (isErrorCorrection)
                      isValidationExceptionDetail.ErrorDescription =
                        string.Format(isValidationExceptionDetail.ErrorDescription, errorCorrArgs);
                    exceptionDetailsList.Add(isValidationExceptionDetail);
                  }
                  if (!outcomeOfMismatchOnRmBilledOrAllowedAmounts)
                  {
                    isValidAmount = true;
                  }
                }

              #endregion
            }

            return isValidAmount;
        }

        /// <summary>
        /// CMP#459 : Gets the currency conversion factor.
        /// </summary>
        /// <param name="currentInvoice">The current invoice.</param>
        /// <param name="yourInvoice">Your invoice.</param>
        /// <param name="prevInvExchangeRate">The prev inv exchange rate.</param>
        /// <param name="currentInvExchangeRate">The current inv exchange rate.</param>
        /// <param name="prevAmount">The prev amount.</param>
        /// <param name="currentAmount">The current amount.</param>
        /// <param name="currentInvoiceTolerance">The current invoice tolerance.</param>
        /// <returns></returns>
        private bool IsAmountMismatch(PaxInvoice currentInvoice, PaxInvoice yourInvoice, ExchangeRate prevInvExchangeRate, ExchangeRate currentInvExchangeRate, double prevAmount, double currentAmount, Tolerance currentInvoiceTolerance)
        {
          var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
          var prevInvListingCurrency = Convert.ToInt32(yourInvoice.ListingCurrencyId.Value);
          var prevInvBillingCurrency = Convert.ToInt32(yourInvoice.BillingCurrencyId.Value);
          var currentInvListingCurrency = Convert.ToInt32(currentInvoice.ListingCurrencyId.Value);
          var prevInvSmi = yourInvoice.SettlementMethodId;
          var currentInvSmi = currentInvoice.SettlementMethodId;
          double convertedAmount = prevAmount;
          if (currentInvExchangeRate != null && prevInvExchangeRate != null)
          {
            if (prevInvListingCurrency == prevInvBillingCurrency && prevInvBillingCurrency == currentInvListingCurrency)
            {
              convertedAmount = prevAmount;
              //SCP203775 : RM is validated when billed amount doesn't match with previous transaction 
              return (currentInvoiceTolerance != null && !CompareUtil.Compare(currentAmount, convertedAmount, currentInvoiceTolerance.RoundingTolerance, Constants.PaxDecimalPlaces));
            }
            // CMP#624 : Change#3 - Conditional validation of PAX/CGO Billed/Allowed amounts 
            if (prevInvSmi != (int) SettlementMethodValues.IchSpecialAgreement && currentInvSmi != (int) SettlementMethodValues.IchSpecialAgreement)
            {

              //SMI is I/A/M
              if (prevInvSmi == (int) SettlementMethodValues.Ich || prevInvSmi == (int) SettlementMethodValues.Ach || prevInvSmi == (int) SettlementMethodValues.AchUsingIATARules)
              {
                //Currency of Billing of previous transaction is USD && SMI I/M/A/B
                if ((prevInvBillingCurrency == (int) BillingCurrency.USD) &&
                    (currentInvSmi == (int) SettlementMethodValues.Ich || currentInvSmi == (int) SettlementMethodValues.Ach || currentInvSmi == (int) SettlementMethodValues.AchUsingIATARules ||
                     currentInvSmi == (int) SettlementMethodValues.Bilateral || referenceManager.IsSmiLikeBilateral(currentInvSmi, false)))
                {
                  convertedAmount = TruncateAmount((prevAmount / prevInvExchangeRate.FiveDayRateUsd));
                  convertedAmount = convertedAmount * currentInvExchangeRate.FiveDayRateUsd;
                  convertedAmount = ConvertUtil.Round(convertedAmount, Constants.PaxDecimalPlaces);
                }
                //Currency of Billing of previous transaction is GBP & SMI I/M/A/B
                if (prevInvSmi == (int) SettlementMethodValues.Ich && prevInvBillingCurrency == (int) BillingCurrency.GBP &&
                    (currentInvSmi == (int) SettlementMethodValues.Ich || currentInvSmi == (int) SettlementMethodValues.Ach || currentInvSmi == (int) SettlementMethodValues.AchUsingIATARules ||
                     currentInvSmi == (int) SettlementMethodValues.Bilateral))
                {
                  convertedAmount = TruncateAmount((prevAmount / prevInvExchangeRate.FiveDayRateGbp));
                  convertedAmount = convertedAmount * currentInvExchangeRate.FiveDayRateGbp;
                  convertedAmount = ConvertUtil.Round(convertedAmount, Constants.PaxDecimalPlaces);
                }
                //Currency of Billing of previous transaction is EUR
                if (prevInvSmi == (int) SettlementMethodValues.Ich && prevInvBillingCurrency == (int) BillingCurrency.EUR &&
                    (currentInvSmi == (int) SettlementMethodValues.Ich || currentInvSmi == (int) SettlementMethodValues.Ach || currentInvSmi == (int) SettlementMethodValues.AchUsingIATARules ||
                     currentInvSmi == (int) SettlementMethodValues.Bilateral || referenceManager.IsSmiLikeBilateral(currentInvSmi, false)))
                {
                  convertedAmount = TruncateAmount((prevAmount / prevInvExchangeRate.FiveDayRateEur));
                  convertedAmount = convertedAmount * currentInvExchangeRate.FiveDayRateEur;
                  convertedAmount = ConvertUtil.Round(convertedAmount, Constants.PaxDecimalPlaces);
                }


                //Currency of Billing of previous transaction is CAD
                if (prevInvSmi == (int) SettlementMethodValues.Ach && prevInvListingCurrency == (int) BillingCurrency.CAD && prevInvBillingCurrency == (int) BillingCurrency.CAD &&
                    (currentInvSmi == (int) SettlementMethodValues.Ich || currentInvSmi == (int) SettlementMethodValues.Ach || currentInvSmi == (int) SettlementMethodValues.AchUsingIATARules ||
                     currentInvSmi == (int) SettlementMethodValues.Bilateral || referenceManager.IsSmiLikeBilateral(currentInvSmi, false)))
                {
                  convertedAmount = TruncateAmount((prevAmount / prevInvExchangeRate.FiveDayRateUsd));
                  convertedAmount = convertedAmount * currentInvExchangeRate.FiveDayRateUsd;
                  convertedAmount = ConvertUtil.Round(convertedAmount, Constants.PaxDecimalPlaces);
                }
              }
              if (prevInvSmi == (int) SettlementMethodValues.Bilateral || referenceManager.IsSmiLikeBilateral(prevInvSmi, false))
              {
                if (currentInvSmi == (int) SettlementMethodValues.Ich || currentInvSmi == (int) SettlementMethodValues.Ach || currentInvSmi == (int) SettlementMethodValues.AchUsingIATARules ||
                    currentInvSmi == (int) SettlementMethodValues.Bilateral || referenceManager.IsSmiLikeBilateral(currentInvSmi, false))
                {
                  if (prevInvBillingCurrency == (int) BillingCurrency.USD)
                  {
                    convertedAmount = TruncateAmount((prevAmount / prevInvExchangeRate.FiveDayRateUsd));
                    convertedAmount = convertedAmount * currentInvExchangeRate.FiveDayRateUsd;
                    convertedAmount = ConvertUtil.Round(convertedAmount, Constants.PaxDecimalPlaces);
                  }

                  if (prevInvBillingCurrency == (int) BillingCurrency.GBP)
                  {
                    convertedAmount = TruncateAmount((prevAmount / prevInvExchangeRate.FiveDayRateGbp));
                    convertedAmount = convertedAmount * currentInvExchangeRate.FiveDayRateGbp;
                    convertedAmount = ConvertUtil.Round(convertedAmount, Constants.PaxDecimalPlaces);
                  }

                  if (prevInvBillingCurrency == (int) BillingCurrency.EUR)
                  {
                    convertedAmount = TruncateAmount((prevAmount / prevInvExchangeRate.FiveDayRateEur));
                    convertedAmount = convertedAmount * currentInvExchangeRate.FiveDayRateEur;
                    convertedAmount = ConvertUtil.Round(convertedAmount, Constants.PaxDecimalPlaces);
                  }
                  if (prevInvBillingCurrency != (int) BillingCurrency.USD && prevInvBillingCurrency != (int) BillingCurrency.GBP && prevInvBillingCurrency != (int) BillingCurrency.EUR)
                  {
                    convertedAmount = TruncateAmount((prevAmount / prevInvExchangeRate.FiveDayRateUsd));
                    convertedAmount = convertedAmount * currentInvExchangeRate.FiveDayRateUsd;
                    convertedAmount = ConvertUtil.Round(convertedAmount, Constants.PaxDecimalPlaces);
                  }
                }
              }
            }
            // CMP#624 : Change#3 - Conditional validation of PAX/CGO Billed/Allowed amounts 
            if ( prevInvSmi == (int) SettlementMethodValues.IchSpecialAgreement && currentInvSmi == (int) SettlementMethodValues.IchSpecialAgreement)
            {
              if(!string.IsNullOrWhiteSpace(yourInvoice.CurrencyRateIndicator) && yourInvoice.CurrencyRateIndicator.Trim().ToUpper() == "F")
              {
                // Validate amounts for SMI X only if
                // When the Currency of Billing of the rejected invoice is USD, GBP or EUR
                //AND
                //Invoice level flag ‘Currency Rate Indicator’ of the rejected invoice is “F” (FDR)
                if (prevInvBillingCurrency == (int)BillingCurrency.USD)
                {
                  convertedAmount = TruncateAmount((prevAmount / prevInvExchangeRate.FiveDayRateUsd));
                  convertedAmount = convertedAmount * currentInvExchangeRate.FiveDayRateUsd;
                  convertedAmount = ConvertUtil.Round(convertedAmount, Constants.PaxDecimalPlaces);
                  return (currentInvoiceTolerance != null && !CompareUtil.Compare(currentAmount, convertedAmount, currentInvoiceTolerance.RoundingTolerance, Constants.PaxDecimalPlaces));
                }
                if (prevInvBillingCurrency == (int)BillingCurrency.GBP)
                {
                  convertedAmount = TruncateAmount((prevAmount / prevInvExchangeRate.FiveDayRateGbp));
                  convertedAmount = convertedAmount * currentInvExchangeRate.FiveDayRateGbp;
                  convertedAmount = ConvertUtil.Round(convertedAmount, Constants.PaxDecimalPlaces);
                  return (currentInvoiceTolerance != null && !CompareUtil.Compare(currentAmount, convertedAmount, currentInvoiceTolerance.RoundingTolerance, Constants.PaxDecimalPlaces));
                }

                if (prevInvBillingCurrency == (int)BillingCurrency.EUR)
                {
                  convertedAmount = TruncateAmount((prevAmount / prevInvExchangeRate.FiveDayRateEur));
                  convertedAmount = convertedAmount * currentInvExchangeRate.FiveDayRateEur;
                  convertedAmount = ConvertUtil.Round(convertedAmount, Constants.PaxDecimalPlaces);
                  return (currentInvoiceTolerance != null && !CompareUtil.Compare(currentAmount, convertedAmount, currentInvoiceTolerance.RoundingTolerance, Constants.PaxDecimalPlaces));
                }
                
                 //for other Billing Currency should not validate amounts
                 return false;
              }
              
                //for currency rate indicator other than "F" amounts should not validated.
              return false;
              
            }

            //CMP #553: ACH Requirement for Multiple Currency Handling-FRS
            //This section will apply when Previous SMI is A OR M
            // CMP #456: section 8.1, 8.2, 8.3
            if (prevInvSmi == (int)SettlementMethodValues.Ach || prevInvSmi == (int)SettlementMethodValues.AchUsingIATARules)
            {
                //Previous billing currency 'GBP' and currenct SMI I/M/A/B/X
                if (prevInvBillingCurrency == (int)BillingCurrency.GBP && (currentInvSmi == (int)SettlementMethodValues.Ich ||
                    currentInvSmi == (int)SettlementMethodValues.Ach || currentInvSmi == (int)SettlementMethodValues.AchUsingIATARules ||
                    currentInvSmi == (int)SettlementMethodValues.Bilateral || referenceManager.IsSmiLikeBilateral(currentInvSmi, true)
                    || currentInvSmi == (int)SettlementMethodValues.IchSpecialAgreement))
                {
                    convertedAmount = TruncateAmount((prevAmount / prevInvExchangeRate.FiveDayRateGbp));
                    convertedAmount = convertedAmount * currentInvExchangeRate.FiveDayRateGbp;
                    convertedAmount = ConvertUtil.Round(convertedAmount, Constants.PaxDecimalPlaces);
                }
                else
                {
                    //Previous billing currency 'EUR"' and currenct SMI I/M/A/B/X
                    if (prevInvBillingCurrency == (int) BillingCurrency.EUR &&
                        (currentInvSmi == (int) SettlementMethodValues.Ich ||
                         currentInvSmi == (int) SettlementMethodValues.Ach ||
                         currentInvSmi == (int) SettlementMethodValues.AchUsingIATARules ||
                         currentInvSmi == (int) SettlementMethodValues.Bilateral ||
                         referenceManager.IsSmiLikeBilateral(currentInvSmi, true)
                         || currentInvSmi == (int) SettlementMethodValues.IchSpecialAgreement))
                    {
                        convertedAmount = TruncateAmount((prevAmount/prevInvExchangeRate.FiveDayRateEur));
                        convertedAmount = convertedAmount*currentInvExchangeRate.FiveDayRateEur;
                        convertedAmount = ConvertUtil.Round(convertedAmount, Constants.PaxDecimalPlaces);
                    }
                    else
                    {
                        //currenct SMI I/M/A/B/X
                        if (currentInvSmi == (int) SettlementMethodValues.Ich ||
                            currentInvSmi == (int) SettlementMethodValues.Ach ||
                            currentInvSmi == (int) SettlementMethodValues.AchUsingIATARules ||
                            currentInvSmi == (int) SettlementMethodValues.Bilateral ||
                            referenceManager.IsSmiLikeBilateral(currentInvSmi, true)
                            || currentInvSmi == (int) SettlementMethodValues.IchSpecialAgreement)
                        {
                            convertedAmount = TruncateAmount((prevAmount/prevInvExchangeRate.FiveDayRateUsd));
                            convertedAmount = convertedAmount*currentInvExchangeRate.FiveDayRateUsd;
                            convertedAmount = ConvertUtil.Round(convertedAmount, Constants.PaxDecimalPlaces);
                        }
                    }
                }
            }
          }
          return (currentInvoiceTolerance != null && !CompareUtil.Compare(currentAmount, convertedAmount, currentInvoiceTolerance.RoundingTolerance, Constants.PaxDecimalPlaces));
        }

        /// <summary>
        /// Truncates the amount.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        private double TruncateAmount(double amount)
        {
            int precision = 10;
            double step = (double) Math.Pow(10, precision);
            double tmpAmount = Math.Truncate(step * amount);
            return (tmpAmount / step);
        }

        /// <summary>
        /// Validates the amountof rm on validation error correction.
        /// </summary>
        /// <param name="exceptionDetailsList">The exception details list.</param>
        /// <param name="errorCorrection">The error correction.</param>
        /// <returns></returns>
        public bool ValidateAmountofRmOnValidationErrorCorrection(IList<IsValidationExceptionDetail> exceptionDetailsList, ValidationErrorCorrection errorCorrection)
        {
            bool isValid = true;
            bool outcomeOfMismatchOnRmBilledOrAllowedAmounts = Convert.ToBoolean(SystemParameters.Instance.ValidationParams.PAXRMBilledAllowedAmounts);
            var rejectionMemoRecord = RejectionMemoRepository.Single(rejectionMemoId: errorCorrection.PkReferenceId);
            if (rejectionMemoRecord != null && outcomeOfMismatchOnRmBilledOrAllowedAmounts)
            {
                var currentInvoice = InvoiceRepository.Single(id: rejectionMemoRecord.InvoiceId);
                rejectionMemoRecord.IsLinkingSuccessful = true;
                rejectionMemoRecord.YourInvoiceBillingPeriod = errorCorrection.YourInvoicePeriod;
                rejectionMemoRecord.YourInvoiceBillingMonth = errorCorrection.YourInvoiceMonth;
                rejectionMemoRecord.YourInvoiceBillingYear = errorCorrection.YourInvoiceYear;
                rejectionMemoRecord.YourInvoiceNumber = errorCorrection.YourInvoiceNo;
                rejectionMemoRecord.YourRejectionNumber = errorCorrection.YourRejectionMemoNo;

                if (currentInvoice.BillingCode == (int)BillingCode.NonSampling || currentInvoice.BillingCode == (int)BillingCode.SamplingFormXF)
                {
                    if (currentInvoice.BillingCode == (int)BillingCode.NonSampling && rejectionMemoRecord.RejectionStage == 1 )
                    {
                        if (rejectionMemoRecord.FIMBMCMIndicatorId == 0 || rejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.None)
                        {
                            //Coupon Level
                            foreach (var rejectionMemoCouponBreakdownRecord in rejectionMemoRecord.CouponBreakdownRecord)
                            {
                                isValid = ValidateAmountsInRMonCouponLevel(outcomeOfMismatchOnRmBilledOrAllowedAmounts, exceptionDetailsList, rejectionMemoRecord, rejectionMemoCouponBreakdownRecord, true, true);
                            }
                        }
                        if (rejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.FIMNumber)
                        {
                            isValid = ValidateAmountsInRMonMemoLevel(outcomeOfMismatchOnRmBilledOrAllowedAmounts, exceptionDetailsList, rejectionMemoRecord, true);
                        }
                        if (rejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.BMNumber || rejectionMemoRecord.FIMBMCMIndicatorId == (int)FIMBMCMIndicator.CMNumber)
                        {
                          isValid = ValidateAmountsInRMonMemoLevel(outcomeOfMismatchOnRmBilledOrAllowedAmounts, exceptionDetailsList, rejectionMemoRecord, true);
                        }
                    }
                    if (rejectionMemoRecord.RejectionStage == 2 || rejectionMemoRecord.RejectionStage == 3)
                    {
                        //Memo Level
                        isValid = ValidateAmountsInRMonMemoLevel(outcomeOfMismatchOnRmBilledOrAllowedAmounts, exceptionDetailsList, rejectionMemoRecord,true);
                    }
                }
                if (currentInvoice.BillingCode == (int)BillingCode.SamplingFormF && rejectionMemoRecord.CouponBreakdownRecord.Count > 0)
                {
                    //Coupon Level
                    foreach (var rejectionMemoCouponBreakdownRecord in rejectionMemoRecord.CouponBreakdownRecord)
                    {
                        isValid = ValidateAmountsInRMonCouponLevel(outcomeOfMismatchOnRmBilledOrAllowedAmounts, exceptionDetailsList, rejectionMemoRecord, rejectionMemoCouponBreakdownRecord, true, true);
                    }
                }
                
                rejectionMemoRecord = RejectionMemoRepository.Single(rejectionMemoId: rejectionMemoRecord.Id);
            }
            return isValid;
        }
      /// <summary>
      /// Delete Invoice from Manahe Invoice Screen
      /// </summary>
      /// <param name="invoiceIdStringInOracleFormat"></param>
      /// <param name="dummyMemberId"></param>
      /// <param name="userId"></param>
      /// <param name="isIsWebInvoice"></param>
      /// <returns></returns>
      public bool DeleteInvoice(string invoiceIdStringInOracleFormat, int dummyMemberId, int userId,int isIsWebInvoice)
      {

        //get an instance of Processing Dashboard repository
        var processingDashboardRepository = Ioc.Resolve<IProcessingDashboardFileStatusRepository>(typeof(IProcessingDashboardFileStatusRepository));

        // Call "DeleteInvoices()" method which will Execute stored procedure and delete selected invoices
        var actionInvoiceList = processingDashboardRepository.DeleteInvoices(invoiceIdStringInOracleFormat, dummyMemberId, userId,isIsWebInvoice);

        if (actionInvoiceList != null && actionInvoiceList.Count > 0)
        {
          if (actionInvoiceList[0].ActionStatus == "1")
          {
            return true;
          }
        }
        return false;
      }

      /// <summary>
      /// Gets the ach invoice count.
      /// </summary>
      /// <param name="billingMemberId">The billing member id.</param>
      /// <param name="billingCategoryId">The billing category id.</param>
      /// <param name="billingYear">The billing year.</param>
      /// <param name="billingMonth">The billing month.</param>
      /// <param name="billingPeriod">The billing period.</param>
      /// <param name="settlementMethodId">The settlement method id.</param>
      /// <param name="clearanceHouse">The clearance house.</param>
      /// <returns></returns>
      public int GetAchInvoiceCount(int billingMemberId, int billingCategoryId, int billingYear, int billingMonth, int billingPeriod, int settlementMethodId, string clearanceHouse)
      {

        var invoiceRepository = Ioc.Resolve<IInvoiceRepository>(typeof(IInvoiceRepository));

        return  invoiceRepository.GetAchInvoiceCount(billingMemberId, billingCategoryId, billingYear, billingMonth, billingPeriod, settlementMethodId, clearanceHouse);
      }

      /// <summary>
      /// Check if BM already exists for the given correspondence ref no.
      /// </summary>
      /// <param name="billingMemo"></param>
      /// <param name="billingMemoInvoice"></param>
      /// <param name="isUpdateOperation"></param>
      /// SCP186155 - Same BM ,RAm5.2.2.5
      public void CheckDuplicateBillingMemoForCorr(BillingMemo billingMemo, PaxInvoice billingMemoInvoice, bool isUpdateOperation)
      {
          BillingMemoRepository = Ioc.Resolve<IBillingMemoRecordRepository>(typeof(IBillingMemoRecordRepository));
          if (billingMemo.CorrespondenceRefNumber != 0)
          {
              //SCP186155 - Same BM ,RAm5.2.2.5
              //Desc: Check if BM against the corr already exists. Existing conditions on invoice status is relaxed now.
              //      Prior to code change invoice status was required to be 3,4,5 or 6. This is now changed to anything but 9.
              //SCP# 304831 - Issue with audit trail. 
              //Desc: Standalone BM having a valid Correspondence reference number and reason code other than 6A/6B, 
              //shows this incorrect correspondence in audit trail. Duplicate BM on Correspondence check is enhanced
              //to consider reason code as well. (corresponding SP PROC_IS_BILLING_MEMO_EXISTS is also updated for this purpose)
              long billingMemoCount;
              if (!isUpdateOperation)
                billingMemoCount =
                    BillingMemoRepository.GetCount(
                        memo =>
                        memo.CorrespondenceRefNumber == billingMemo.CorrespondenceRefNumber && 
                        memo.Invoice.BillingMemberId == billingMemoInvoice.BillingMemberId &&
                        memo.Invoice.BilledMemberId == billingMemoInvoice.BilledMemberId &&
                        (memo.ReasonCode == "6A" || memo.ReasonCode == "6B") &&
                        !(memo.Invoice.InvoiceStatusId == (int)InvoiceStatusType.ErrorNonCorrectable &&
                        memo.Invoice.ValidationStatusId == (int)InvoiceValidationStatus.Failed));
              else
                billingMemoCount =
                    BillingMemoRepository.GetCount(
                        memo =>
                        memo.CorrespondenceRefNumber == billingMemo.CorrespondenceRefNumber && 
                        memo.Invoice.BillingMemberId == billingMemoInvoice.BillingMemberId &&
                        memo.Invoice.BilledMemberId == billingMemoInvoice.BilledMemberId && memo.Id != billingMemo.Id && 
                        (memo.ReasonCode == "6A" || memo.ReasonCode == "6B") &&
                       !(memo.Invoice.InvoiceStatusId == (int)InvoiceStatusType.ErrorNonCorrectable &&
                        memo.Invoice.ValidationStatusId == (int)InvoiceValidationStatus.Failed));

              if (billingMemoCount > 0) throw new ISBusinessException(ErrorCodes.BillingMemoExistsForCorrespondence);
          }
      }

      #region CMP612: Changes to PAX CGO Correspondence Audit Trail Download

      /// <summary>
      /// This function is used to get linked correspondence rejection memo list.
      /// </summary>
      /// <param name="corrRefNo"></param>
      /// <returns></returns>
      //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
      public List<PaxLinkedCorrRejectionSearchData> GetLinkedCorrRejectionSearchResult(Guid correspondenceId)
      {
        //Get stage 3 rejection memo based on correspondence id.
        var filteredList = InvoiceRepository.GetLinkedCorrRejectionSearchResult(correspondenceId);
        return filteredList;
      }
      
      /// <summary>
      /// This function is used for create rejection audit trail pdf.
      /// </summary>
      /// <param name="request"></param>
      /// <param name="memberFtpRMAuditTrailPath"></param>
      /// <returns></returns>
      //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
      public string CreateRejectionAuditTrailPdf(ReportDownloadRequestMessage request, string memberFtpRMAuditTrailPath, int ProcessingUnitNumber)
      {
        //Split file name and rejection memo id from request.
        var inputArray = request.InputData.Split(';');
        var zipFolderName = inputArray[1];
        var rejectionMemoIds = inputArray[0].Split(',');
        var reportPdfPaths = new List<string>();
        var pdfCreationPath = Path.Combine(memberFtpRMAuditTrailPath, Guid.NewGuid().ToString());
        if (!Directory.Exists(pdfCreationPath))
        {
          Logger.InfoFormat("Create temp folder for pdf file. {0}", pdfCreationPath);
          Directory.CreateDirectory(pdfCreationPath);
        }

        foreach (var rejectionMemoId in rejectionMemoIds)
        {
          var fileLocation = string.Empty;
          using (var p = new Process())
          {
            //Get single instance file name.
            p.StartInfo.FileName = ConfigurationManager.AppSettings["RMAuditTrailPDFGenerator"];
            Logger.InfoFormat("Calling Single Instance :" + p.StartInfo.FileName);
           
            //Set argument to single instance for create audit trail rejection pdf.
            string arguments = string.Format("{3} {0} \"{1}\" {2}", rejectionMemoId, pdfCreationPath, (int)BillingCategoryType.Pax, ProcessingUnitNumber);
            p.StartInfo.Arguments = arguments;
            Logger.InfoFormat(string.Format("Single Instance Arguments [{0}].", p.StartInfo.Arguments));

            //Set configuration setting for exe.
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.CreateNoWindow = true;
            p.EnableRaisingEvents = false;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.Start();

            fileLocation = p.StandardOutput.ReadToEnd();
            Logger.Info("Instance Started.......");
            p.WaitForExit();
            Logger.Info("Instance waiting.......");
            p.Close();
            Logger.Info("Instance Closed.......");

            //Exception occurring while generate audit trail PDF for RM, We will throw exception to service so that service can retry.  
            if (fileLocation.Contains("Exception"))
            {
              //Delete temporary folder after use. 
              DeleteTemporaryFolder(pdfCreationPath);

              throw new Exception(fileLocation);
            }
          }
          //Add file path for create zip file.
          reportPdfPaths.Add(fileLocation);
        }

        if (reportPdfPaths.Count > 0)
        {
          //Create zip file for audit trail rejection memo.
          string reportZipFilePath = CreateRejectionAuditTrailZip(memberFtpRMAuditTrailPath, zipFolderName, reportPdfPaths);
          Logger.InfoFormat("Report Zip Created at location {0}", reportZipFilePath);

          //Delete temporary folder after use. 
          DeleteTemporaryFolder(pdfCreationPath);

          return reportZipFilePath;
        }
        else
        {
          Logger.InfoFormat("No PDF found for Rejection Memo");
          return string.Empty;
        }
      }

      /// <summary>
      /// This function is used to delete temporary folder.
      /// </summary>
      /// <param name="pdfCreationPath"></param>
      //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
      private static void DeleteTemporaryFolder(string pdfCreationPath)
      {
        //Delete temporary folder after use. 
        if (Directory.Exists(pdfCreationPath))
        {
          try
          {
            Directory.Delete(pdfCreationPath, true);
            Logger.InfoFormat("Delete temp folder. {0}", pdfCreationPath);
          }
          catch (Exception ex)
          {
            //Eat up exception.
            Logger.ErrorFormat("Handle Exception: Error occured while deleting temp folder. {0}", ex);
          }
        }
      }

      /// <summary>
      /// This function is used to create rejection audit trail zip.
      /// </summary>
      /// <param name="basePath"></param>
      /// <param name="zipFolderName"></param>
      /// <param name="reportPdfPaths"></param>
      /// <returns></returns>
      //CMP612: Changes to PAX CGO Correspondence Audit Trail Download
      private string CreateRejectionAuditTrailZip(string basePath, string zipFolderName, List<string> reportPdfPaths)
      {
        Logger.InfoFormat("Creating Zip ");
        var zipFileName = string.Format("{0}.ZIP", zipFolderName);
        var zipFolder = Path.Combine(basePath, zipFolderName);

        //Create zip file.
        FileIo.ZipOutputFile(zipFolder, string.Empty, zipFileName, reportPdfPaths.ToArray());

        Logger.InfoFormat("Zip Created Location: {0}", Path.Combine(basePath, zipFileName));
        return Path.Combine(basePath, zipFileName);
      }
      
      #endregion

      public void SetViewableByClearingHouseForAutoBilling(InvoiceBase invoice)
      {
        SetViewableByClearingHouse(invoice);
      }

      #region CMP#641
      //CMP#641: Time Limit Validation on Third Stage PAX Rejections

      /// <summary>
      /// Validates the pax stage three rm for time limit.
      /// </summary>
      /// <param name="transactionType">Type of the transaction.</param>
      /// <param name="settlementMethodId">The settlement method id.</param>
      /// <param name="rejectionMemo">The rejection memo.</param>
      /// <param name="currentInvoice">The current invoice.</param>
      /// <param name="isIsWeb">if set to <c>true</c> [is is web].</param>
      /// <param name="isManageInvoice">if set to <c>true</c> [is manage invoice].</param>
      /// <param name="fileName">Name of the file.</param>
      /// <param name="fileSubmissionDate">The file submission date.</param>
      /// <param name="exceptionDetailsList">The exception details list.</param>
      /// <param name="errorCorrection">The error correction.</param>
      /// <param name="isErrorCorrection">if set to <c>true</c> [is error correction].</param>
      /// <param name="isBillingHistory">if set to <c>true</c> [is billing history].</param>
      /// <returns></returns>
      public bool ValidatePaxStageThreeRmForTimeLimit(TransactionType transactionType, int settlementMethodId, RejectionMemo rejectionMemo, PaxInvoice currentInvoice, bool isIsWeb = false, bool isManageInvoice = false, string fileName = null, DateTime? fileSubmissionDate = null, IList<IsValidationExceptionDetail> exceptionDetailsList = null, ValidationErrorCorrection errorCorrection = null, bool isErrorCorrection = false, bool isBillingHistory = false)
      {
          Logger.Info("Time Limit validation Start");

          bool isValid = false;
          string errorDescription = string.Empty;

          Stopwatch mystopwatch = new Stopwatch();
          mystopwatch.Start();
          DateTime yourBillingDate = new DateTime();
          DateTime invoiceBillingDate = new DateTime();
          var cultureInfo = new CultureInfo("en-US");
          cultureInfo.Calendar.TwoDigitYearMax = 2099;
          const string billingDateFormat = "yyyyMMdd";

          if (isErrorCorrection && errorCorrection != null)
          {
              //rejectionMemo = RejectionMemoRepository.Get(rm=>rm.Id== errorCorrection.PkReferenceId).FirstOrDefault();
              if (errorCorrection.RejectionStage != (int)RejectionStage.StageThree)
              {
                  return true;
              }
              currentInvoice = InvoiceRepository.GetInvoiceHeader(errorCorrection.InvoiceID);
             
              if(currentInvoice!=null && currentInvoice.BillingCode == (int) BillingCode.SamplingFormXF)
              {
                  transactionType = TransactionType.SamplingFormXF;
              }
              if (currentInvoice != null )
              {
                  settlementMethodId = currentInvoice.SettlementMethodId;
                  if (
                  !DateTime.TryParseExact(
                      string.Format("{0}{1}{2}",
                                    errorCorrection.YourInvoiceYear.ToString("0000"),
                                    errorCorrection.YourInvoiceMonth.ToString("00"),
                                    errorCorrection.YourInvoicePeriod.ToString("00")),
                      billingDateFormat,
                      cultureInfo,
                      DateTimeStyles.None,
                      out yourBillingDate))
                  {
                      return false;
                  }
                  if (
                      !DateTime.TryParseExact(
                          string.Format("{0}{1}{2}",
                                        currentInvoice.BillingYear.ToString("0000"),
                                        currentInvoice.BillingMonth.ToString("00"),
                                        currentInvoice.BillingPeriod.ToString("00")),
                          billingDateFormat,
                          cultureInfo,
                          DateTimeStyles.None,
                          out invoiceBillingDate))
                  {
                      return false;
                  }
              }
          }
          else if (isBillingHistory)
          {
              if (
                  !DateTime.TryParseExact(
                      string.Format("{0}{1}{2}",
                                    currentInvoice.BillingYear.ToString("0000"),
                                    currentInvoice.BillingMonth.ToString("00"),
                                    currentInvoice.BillingPeriod.ToString("00")),
                      billingDateFormat,
                      cultureInfo,
                      DateTimeStyles.None,
                      out yourBillingDate))
              {
                  return false;
              }
              int expectedSmi;
              ClearingHouse expectedClearingHouse;
              GetExpectedSmiAndClearingHouseForStageThreeRm(currentInvoice, out expectedSmi, out expectedClearingHouse);
              Logger.InfoFormat("Expected SMI for Time Limit [{0}]", expectedSmi);
              Logger.InfoFormat("Expected CH for Time Limit [{0}]", expectedClearingHouse);
              settlementMethodId = expectedSmi;
              var currBillingPeriod = CalendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(expectedClearingHouse);

              if (
                  !DateTime.TryParseExact(
                      string.Format("{0}{1}{2}",
                                    currBillingPeriod.Year.ToString("0000"),
                                    currBillingPeriod.Month.ToString("00"),
                                    currBillingPeriod.Period.ToString("00")),
                      billingDateFormat,
                      cultureInfo,
                      DateTimeStyles.None,
                      out invoiceBillingDate))
              {
                  return false;
              }
          }
          
          if (rejectionMemo != null && currentInvoice != null)
          {
              if (
                  !DateTime.TryParseExact(
                      string.Format("{0}{1}{2}",
                                    rejectionMemo.YourInvoiceBillingYear.ToString("0000"),
                                    rejectionMemo.YourInvoiceBillingMonth.ToString("00"),
                                    rejectionMemo.YourInvoiceBillingPeriod.ToString("00")),
                      billingDateFormat,
                      cultureInfo,
                      DateTimeStyles.None,
                      out yourBillingDate))
              {
                  return false;
              }
              if (
                  !DateTime.TryParseExact(
                      string.Format("{0}{1}{2}",
                                    currentInvoice.BillingYear.ToString("0000"),
                                    currentInvoice.BillingMonth.ToString("00"),
                                    currentInvoice.BillingPeriod.ToString("00")),
                      billingDateFormat,
                      cultureInfo,
                      DateTimeStyles.None,
                      out invoiceBillingDate))
              {
                  return false;
              }
          }

          if (invoiceBillingDate != new DateTime() && yourBillingDate != new DateTime())
          {
              string billingPeriodyymmpp = string.Empty;
              string billingPeriodyyyyMmmPx = string.Empty;

              var currentBillingPeriodyymmpp = string.Format("{0}{1}{2}", invoiceBillingDate.Year.ToString("0000").Substring(2, 2), invoiceBillingDate.Month.ToString("00"), invoiceBillingDate.Day.ToString("00"));

              var timeLimit = ReferenceManager.GetTransactionTimeLimitForPaxRmStageThree(transactionType, settlementMethodId, yourBillingDate);
              if (timeLimit != null)
              {
                  var invoiceBillingPeriod = new BillingPeriod(invoiceBillingDate.Year, invoiceBillingDate.Month, invoiceBillingDate.Day);

                  var startDate = new DateTime(yourBillingDate.Year, yourBillingDate.Month, 1);
                  var endDate = startDate.AddMonths(timeLimit.Limit);

                  var endBillingPeriod = new BillingPeriod(endDate.Year, endDate.Month, 4);

                  billingPeriodyymmpp = string.Format("{0}{1}04", endDate.ToString("yy"), endDate.Month.ToString("00"));

                  billingPeriodyyyyMmmPx = string.Format("{1} {0} P4", endDate.Year.ToString("0000"), endDate.ToString("MMM"));

                  isValid = (invoiceBillingPeriod <= endBillingPeriod);

                  if (isValid)
                  {
                      return isValid;
                  }
                  else
                  {
                      #region Beyond Time Limit
                      
                      if (isIsWeb)
                      {
                          if (isManageInvoice)
                          {
                              //BPAXNS_10968
                              //Validation failed for RM No. <RM No.> Batch No. <Batch No.> Seq. No. <Seq. No.> as it has been billed beyond the Applicable Time Limit - <Mmm YYYY Px>.
                              errorDescription = Messages.ResourceManager.GetString(ErrorCodes.BeyondTimeLimitIsWebValInvoice);
                              if (!string.IsNullOrEmpty(errorDescription))
                              {
                                  errorDescription = string.Format(errorDescription,rejectionMemo.RejectionMemoNumber,rejectionMemo.BatchSequenceNumber,rejectionMemo.RecordSequenceWithinBatch, billingPeriodyyyyMmmPx);
                              }
                             // throw new ISBusinessException(ErrorCodes.BeyondTimeLimitIsWebValInvoice,errorDescription);
                              var validationExceptionDetail = CreateTimeLimitValidationExceptionDetail(rejectionMemo.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Billing Period",
                                                                                                currentBillingPeriodyymmpp,
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                ErrorCodes.BeyondTimeLimitIsWebValInvoice,
                                                                                                ErrorStatus.X,
                                                                                                rejectionMemo,true, exceptionDesc: errorDescription);
                              exceptionDetailsList.Add(validationExceptionDetail);
                          }
                          else
                          {
                              //BPAXNS_10970
                              //Unable to save Rejection Memo as it has been billed beyond the Applicable Time Limit - <Mmm YYYY Px>.
                              errorDescription = Messages.ResourceManager.GetString(ErrorCodes.BeyondTimeLimitIsWeb);
                              if (!string.IsNullOrEmpty(errorDescription))
                              {
                                  errorDescription = string.Format(errorDescription, billingPeriodyyyyMmmPx);
                                  errorDescription = string.Format("{0}-{1}",ErrorCodes.BeyondTimeLimitIsWeb ,errorDescription);
                              }
                              throw new ISBusinessException(errorDescription);
                          }
                          
                      }
                      else if (isErrorCorrection)
                      {
                          //BPAXNS_10971
                          //Unable to perform linking error correction as this Stage 3 Rejection has been billed beyond the Applicable Time Limit - <Mmm YYYY Px>.
                          errorDescription = Messages.ResourceManager.GetString(ErrorCodes.BeyondTimeLimitErrorCorrection);
                          if (!string.IsNullOrEmpty(errorDescription))
                          {
                              
                              errorDescription = string.Format(errorDescription, billingPeriodyyyyMmmPx);
                          }
                          IsValidationExceptionDetail validationExceptionDetail = new IsValidationExceptionDetail();
                          validationExceptionDetail.ErrorDescription = errorDescription;
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                      else if (isBillingHistory)
                      {
                          //BPAXNS_10969
                          //Unable to initiate rejection as the Applicable Time Limit - <Mmm YYYY Px> has passed. In case Late Submission is open for period <Mmm YYYY Px>, the Stage 3 rejection memo against this rejection may be captured from an invoice of that Billing Period.
                          errorDescription = Messages.ResourceManager.GetString(ErrorCodes.BeyondTimeLimitIsWebBillingHistory);
                          if (!string.IsNullOrEmpty(errorDescription) && exceptionDetailsList != null)
                          {
                              errorDescription = string.Format(errorDescription, billingPeriodyyyyMmmPx);
                              IsValidationExceptionDetail validationExceptionDetail = new IsValidationExceptionDetail();
                              validationExceptionDetail.ErrorDescription = errorDescription;
                              exceptionDetailsList.Add(validationExceptionDetail);
                          }
                      }
                      else
                      {
                          //BPAXNS_10972
                          //This Stage 3 Rejection has been billed beyond the Applicable Time Limit - <YYMMPP>.”
                          errorDescription = Messages.ResourceManager.GetString(ErrorCodes.BeyondTimeLimit);
                          if (!string.IsNullOrEmpty(errorDescription))
                          {
                              errorDescription = string.Format(errorDescription, billingPeriodyymmpp);
                          }
                          var validationExceptionDetail = CreateTimeLimitValidationExceptionDetail(rejectionMemo.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Billing Period",
                                                                                                currentBillingPeriodyymmpp,
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                 ErrorCodes.BeyondTimeLimit,
                                                                                                ErrorStatus.X,
                                                                                                rejectionMemo, exceptionDesc: errorDescription);
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                      #endregion
                  }
              }
              else
              {
                  #region Invalid Time Limit
                  
                  if (isIsWeb)
                  {
                      if (isManageInvoice)
                      {
                          //BPAXNS_10964
                          //Validation failed for RM No. <RM No.> Batch No. <Batch No.> Seq. No. <Seq. No.>. The system failed to retrieve Time Limit data from the master while calculating Applicable Time Limit. Please contact SIS Help Desk for resolution.
                          errorDescription = Messages.ResourceManager.GetString(ErrorCodes.InvalidTimeLimitIsWebValInvoice);
                          if (!string.IsNullOrEmpty(errorDescription))
                          {
                              errorDescription = string.Format(errorDescription, rejectionMemo.RejectionMemoNumber, rejectionMemo.BatchSequenceNumber, rejectionMemo.RecordSequenceWithinBatch);
                          }
                          var validationExceptionDetail = CreateTimeLimitValidationExceptionDetail(rejectionMemo.Id.Value(),
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                "Billing Period",
                                                                                                currentBillingPeriodyymmpp,
                                                                                                currentInvoice,
                                                                                                fileName,
                                                                                                ErrorLevels.ErrorLevelRejectionMemo,
                                                                                                ErrorCodes.InvalidTimeLimitIsWebValInvoice,
                                                                                                ErrorStatus.X,
                                                                                                rejectionMemo, true,exceptionDesc: errorDescription);
                          exceptionDetailsList.Add(validationExceptionDetail);
                          //throw new ISBusinessException(ErrorCodes.InvalidTimeLimitIsWebValInvoice);
                      }
                      else
                      {
                          //BPAXNS_10965
                          //Unable to save Rejection Memo as the system failed to retrieve Time Limit data from the master while calculating Applicable Time Limit. Please contact SIS Help Desk for resolution.
                          throw new ISBusinessException(ErrorCodes.InvalidTimeLimitIsWeb);
                      }
                  }
                  else if (isErrorCorrection && exceptionDetailsList!= null)
                  {
                      //BPAXNS_10966
                      //Unable to perform linking error correction as the system failed to retrieve Time Limit data from the master while calculating Applicable Time Limit. Please contact SIS Help Desk for resolution.
                      errorDescription = Messages.ResourceManager.GetString(ErrorCodes.InvalidTimeLimitErrorCorrection);
                      if (!string.IsNullOrEmpty(errorDescription) && exceptionDetailsList != null)
                      {
                          errorDescription = string.Format(errorDescription, billingPeriodyyyyMmmPx);
                          IsValidationExceptionDetail validationExceptionDetail = new IsValidationExceptionDetail();
                          validationExceptionDetail.ErrorDescription = errorDescription;
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                  }
                  else if (isBillingHistory)
                  {
                      //BPAXNS_10963
                      //Unable to initiate rejection as the system failed to retrieve Time Limit data from the master while calculating Applicable Time Limit. Please contact SIS Help Desk for resolution.

                      errorDescription = Messages.ResourceManager.GetString(ErrorCodes.InvalidTimeLimitIsWebPayable);
                      if (!string.IsNullOrEmpty(errorDescription) && exceptionDetailsList != null)
                      {
                          errorDescription = string.Format(errorDescription, billingPeriodyyyyMmmPx);
                          IsValidationExceptionDetail validationExceptionDetail = new IsValidationExceptionDetail();
                          validationExceptionDetail.ErrorDescription = errorDescription;
                          exceptionDetailsList.Add(validationExceptionDetail);
                      }
                  }
                  else if (exceptionDetailsList != null)
                  {
                      //BPAXNS_10967
                      //“The system failed to retrieve Time Limit data from the master while calculating Applicable Time Limit. Please contact SIS Help Desk for resolution.”
                      var validationExceptionDetail = CreateTimeLimitValidationExceptionDetail(rejectionMemo.Id.Value(),
                                                                                               exceptionDetailsList.Count() + 1,
                                                                                               fileSubmissionDate,
                                                                                               "Billing Period",
                                                                                               currentBillingPeriodyymmpp,
                                                                                               currentInvoice,
                                                                                               fileName,
                                                                                               ErrorLevels.ErrorLevelRejectionMemo,
                                                                                               ErrorCodes.InvalidTimeLimit,
                                                                                               ErrorStatus.X,
                                                                                               rejectionMemo);
                      exceptionDetailsList.Add(validationExceptionDetail);
                  }
                  #endregion

              }

              
          }
          mystopwatch.Stop();
          Logger.InfoFormat("Time Limit validation End [{0}]",mystopwatch.Elapsed);
          return isValid;
      }

      /// <summary>
      /// Creates the time limit validation exception detail.
      /// </summary>
      /// <param name="pkId">The pk id.</param>
      /// <param name="serialNumber">The serial number.</param>
      /// <param name="fileSubmissionDate">The file submission date.</param>
      /// <param name="fieldName">Name of the field.</param>
      /// <param name="fieldValue">The field value.</param>
      /// <param name="invoice">The invoice.</param>
      /// <param name="fileName">Name of the file.</param>
      /// <param name="errorLevel">The error level.</param>
      /// <param name="exceptionCode">The exception code.</param>
      /// <param name="errorStatus">The error status.</param>
      /// <param name="rejectionMemoRecord">The rejection memo record.</param>
      /// <param name="isBatchUpdateAllowed">if set to <c>true</c> [is batch update allowed].</param>
      /// <param name="linkedDocumentNumber">The linked document number.</param>
      /// <param name="islinkingError">if set to <c>true</c> [islinking error].</param>
      /// <param name="ticketIssuingAirline">The ticket issuing airline.</param>
      /// <param name="couponNumber">The coupon number.</param>
      /// <param name="exceptionDesc">The exception desc.</param>
      /// <returns></returns>
      protected static IsValidationExceptionDetail CreateTimeLimitValidationExceptionDetail(string pkId, int serialNumber, DateTime? fileSubmissionDate, string fieldName, string fieldValue, PaxInvoice invoice, string fileName, string errorLevel, string exceptionCode, ErrorStatus errorStatus, RejectionMemo rejectionMemoRecord, bool isIsweb = false, bool isBatchUpdateAllowed = false, string linkedDocumentNumber = null, bool islinkingError = false, string ticketIssuingAirline = null, int couponNumber = 0, string exceptionDesc = null)
      {
          if(isIsweb)
          {
              if (String.IsNullOrEmpty(exceptionDesc))
              {
                  if (!string.IsNullOrEmpty(exceptionCode))
                      exceptionDesc = Messages.ResourceManager.GetString(exceptionCode);
              }
              var validationexceptionDetail = new IsValidationExceptionDetail
              {
                  ExceptionCode = exceptionCode,
                  ErrorStatus = ((int)errorStatus).ToString(),
                  ErrorDescription = exceptionDesc,
              };
              return validationexceptionDetail;
          }

          string submissionFormat;
          if (Path.GetExtension(fileName).ToUpper().Equals(".XML"))
          {
              submissionFormat = ((int)SubmissionMethod.IsXml).ToString();// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsXml).ToUpper();
          }
          else
          {
              submissionFormat = ((int)SubmissionMethod.IsIdec).ToString();// Enum.GetName(typeof(SubmissionMethod), SubmissionMethod.IsIdec).ToUpper();
          }
          var errorDescription = string.Empty;
          if (String.IsNullOrEmpty(exceptionDesc))
          {
              if (!string.IsNullOrEmpty(exceptionCode))
                  errorDescription = Messages.ResourceManager.GetString(exceptionCode);
          }
          else
          {
              //CMP #614: Source Code Validation for PAX RMs.
              //Configurable exception description. 
              errorDescription = exceptionDesc;
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
              BillingFileSubmissionDate = fileSubmissionDate != null ? Convert.ToDateTime(fileSubmissionDate).ToString("yyyyMMdd"):string.Empty,
              LinkedDocNo = linkedDocumentNumber,

              ErrorDescription = errorDescription,
              FieldName = fieldName,
              FieldValue = fieldValue,
              BatchUpdateAllowed = isBatchUpdateAllowed,

              InvoiceNumber = invoice.InvoiceNumber,
              DocumentNo = rejectionMemoRecord.RejectionMemoNumber,
              SourceCodeId = rejectionMemoRecord.SourceCodeId.ToString(),
              ErrorLevel = errorLevel,
              ExceptionCode = exceptionCode,
              FileName = Path.GetFileName(fileName),
              LineItemOrBatchNo = rejectionMemoRecord.BatchSequenceNumber,
              LineItemDetailOrSequenceNo = rejectionMemoRecord.RecordSequenceWithinBatch,
              Id = Guid.NewGuid(),
              PkReferenceId = pkId
          };

          if (!string.IsNullOrWhiteSpace(ticketIssuingAirline))
          {
              validationExceptionDetail.IssuingAirline = ticketIssuingAirline;
          }

          if (couponNumber > 0)
          {
              validationExceptionDetail.CouponNo = couponNumber;
          }

          if (islinkingError)
          {
              validationExceptionDetail.YourInvoiceNo = rejectionMemoRecord.YourInvoiceNumber;
              validationExceptionDetail.YourInvoiceBillingMonth = rejectionMemoRecord.YourInvoiceBillingMonth;
              validationExceptionDetail.YourInvoiceBillingYear = rejectionMemoRecord.YourInvoiceBillingYear;
              validationExceptionDetail.YourInvoiceBillingPeriod = rejectionMemoRecord.YourInvoiceBillingPeriod;
              validationExceptionDetail.YourRejectionMemoNo = rejectionMemoRecord.YourRejectionNumber;
              validationExceptionDetail.FimCouponNumber = rejectionMemoRecord.FimCouponNumber > 0 ? rejectionMemoRecord.FimCouponNumber : (int?)null;
              validationExceptionDetail.FimBmCmNumber = rejectionMemoRecord.FimBMCMNumber;
              validationExceptionDetail.TransactionId = 2;
              validationExceptionDetail.RejectionStage = rejectionMemoRecord.RejectionStage;
              validationExceptionDetail.ReasonCode = rejectionMemoRecord.ReasonCode;
              validationExceptionDetail.FimBmCmIndicator = rejectionMemoRecord.FIMBMCMIndicatorId;
          }
          if (errorStatus.Equals(ErrorStatus.X) && !rejectionMemoRecord.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
          {
              rejectionMemoRecord.TransactionStatus = Model.Common.TransactionStatus.ErrorNonCorrectable;
          }

          if (errorStatus.Equals(ErrorStatus.C) && !rejectionMemoRecord.TransactionStatus.Equals(Model.Common.TransactionStatus.ErrorNonCorrectable))
          {
              rejectionMemoRecord.TransactionStatus = Model.Common.TransactionStatus.ErrorCorrectable;
          }

          return validationExceptionDetail;
      }

      /// <summary>
      /// Gets the expected smi and clearing house for stage three rm.
      /// </summary>
      /// <param name="yourInvoice">Your invoice.</param>
      /// <param name="expectedSmi">The expected smi.</param>
      /// <param name="expectedClearingHouse">The expected clearing house.</param>
      public void GetExpectedSmiAndClearingHouseForStageThreeRm(PaxInvoice yourInvoice, out int expectedSmi, out ClearingHouse expectedClearingHouse)
      {
          var billingMember = yourInvoice.BilledMember;
          if (billingMember == null)
          {
              billingMember = MemberManager.GetMember(yourInvoice.BilledMemberId);
          }
          expectedSmi = yourInvoice.SettlementMethodId;
          expectedClearingHouse = ClearingHouse.Ich;
          // 1. A	Any	Any	A	ACH
          if(yourInvoice.SettlementMethodId == (int)SMI.Ach)
          {
              expectedSmi = (int)SMI.Ach;
              expectedClearingHouse = ClearingHouse.Ach;
          }
          //2.	I	‘Live’or ‘Suspended’	Any	I	ICH
          else if( yourInvoice.SettlementMethodId == (int)SMI.Ich && (billingMember.IchMemberStatusId == (int)IchMemberShipStatus.Live || billingMember.IchMemberStatusId == (int)IchMemberShipStatus.Suspended))
          {
              expectedSmi = (int)SMI.Ich;
              expectedClearingHouse = ClearingHouse.Ich;
          }
          //3.	I	‘Terminated’ or ‘Not a Member’	‘Live’ or ‘Suspended’	M	ACH
          else if (yourInvoice.SettlementMethodId == (int)SMI.Ich && (billingMember.IchMemberStatusId == (int)IchMemberShipStatus.Terminated || billingMember.IchMemberStatusId == (int)IchMemberShipStatus.NotAMember) && (billingMember.AchMemberStatusId == (int)AchMembershipStatus.Live || billingMember.AchMemberStatusId == (int)AchMembershipStatus.Suspended))
          {
              expectedSmi = (int)SMI.AchUsingIataRules;
              expectedClearingHouse = ClearingHouse.Ach;
          }
          //4.  M	Any	‘Live’ or ‘Suspended’	M	ACH
          else if (yourInvoice.SettlementMethodId == (int)SMI.AchUsingIataRules && (billingMember.AchMemberStatusId == (int)AchMembershipStatus.Live || billingMember.IchMemberStatusId == (int)IchMemberShipStatus.Suspended))
          {
              expectedSmi = (int)SMI.AchUsingIataRules;
              expectedClearingHouse = ClearingHouse.Ach;
          }
          //5.	M	‘Live’ or ‘Suspended’  ‘Terminated’ or ‘Not a Member’	I	ICH
          else if (yourInvoice.SettlementMethodId == (int)SMI.AchUsingIataRules && (billingMember.IchMemberStatusId == (int)IchMemberShipStatus.Live || billingMember.IchMemberStatusId == (int)IchMemberShipStatus.Suspended) && (billingMember.AchMemberStatusId == (int)AchMembershipStatus.Terminated || billingMember.AchMemberStatusId == (int)AchMembershipStatus.NotAMember))
          {
              expectedSmi = (int)SMI.Ich;
              expectedClearingHouse = ClearingHouse.Ich;
          }
          //6.	X	Any	Any	X	ICH
          else if (yourInvoice.SettlementMethodId == (int)SMI.IchSpecialAgreement)
          {
              expectedSmi = (int)SMI.Ich;
              expectedClearingHouse = ClearingHouse.Ich;
          }
          //7.	Any Bilateral SMI(Other than X)	Any	Any	B	ICH
          else if (ReferenceManager.IsSmiLikeBilateral(yourInvoice.SettlementMethodId,false))
          {
              expectedSmi = (int)SMI.Bilateral;
              expectedClearingHouse = ClearingHouse.Ich;
          }
          //8.	Defaulting condition, in case any condition is not fulfilled in rows 1-7 above	B	ICH
          else
          {
              expectedSmi = (int)SMI.Bilateral;
              expectedClearingHouse = ClearingHouse.Ich;
          }
      }

        #endregion

    }
}


