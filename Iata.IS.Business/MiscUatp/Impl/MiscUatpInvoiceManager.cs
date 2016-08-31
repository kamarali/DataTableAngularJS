using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Iata.IS.AdminSystem;
using Iata.IS.Business.Common;
using Iata.IS.Business.MiscUatp.ServerValidation;
using Iata.IS.Business.Security;
using Iata.IS.Core;
using Iata.IS.Core.DI;
using Iata.IS.Core.Exceptions;
using Iata.IS.Data;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MiscUatp;
using Iata.IS.Model.Base;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MiscUatp;
using Iata.IS.Model.MiscUatp.Common;
using Iata.IS.Model.MiscUatp.Enums;
using Iata.IS.Model.Pax.Enums;
using log4net;
using TransactionType = Iata.IS.Model.Enums.TransactionType;
using Iata.IS.Model.MiscUatp.Base;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Model.MiscUatp.BillingHistory;
using Iata.IS.Business.TemplatedTextGenerator;
using NVelocity;
using Iata.IS.Business.SupportingDocuments;
using Iata.IS.Data.Common;


namespace Iata.IS.Business.MiscUatp.Impl
{
    /// <summary>
    /// Implementation of business functionality for MISC.
    /// </summary>
    public class MiscUatpInvoiceManager : InvoiceManagerBase, IMiscUatpInvoiceManager
    {
        protected const string ExchangeRateValidationFlag = "EX";
        private const string Comma = ",";
        private const string ErrorCodeSaparator = " - ";
        private const string TimeLimitFlag = "TL";
        private const string ValidationFlagDelimeter = ",";
        private const string MiscBillingHistoryAuditTrailTemplateResourceName = "Iata.IS.Business.App_Data.Templates.MiscBillingHistoryAuditTrailPdf.vm";
        private const int MaxDynamicFieldCount = 5;

        private static readonly IReferenceManager StaticReferenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));

        private ITemplatedTextGenerator _templatedTextGenerator;
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// MiscInvoiceRepository, will be injected by container.
        /// </summary>
        /// <value>The misc invoice repository.</value>
        public IMiscInvoiceRepository MiscInvoiceRepository { get; set; }

        /// <summary>
        /// Gets or sets the type of the transaction.
        /// </summary>
        /// <value>The type of the transaction.</value>
        protected TransactionType TransactionType { get; set; }

        protected TransactionType CorrInvoiceDABTransactionType { get; set; }

        /// <summary>
        /// Gets or sets the parent billing code.
        /// </summary>
        /// <value>The parent billing code.</value>
        protected BillingCategoryType BillingCategory { get; set; }

        /// <summary>
        /// Gets or sets the reason billing code.
        /// </summary>
        /// <value>The reason billing code.</value>
        protected BillingCode BillingCode { get; set; }

        /// <summary>
        /// Gets or sets the type of the invoice.
        /// </summary>
        /// <value>The type of the invoice.</value>
        protected InvoiceType InvoiceType { get; set; }

        /// <summary>
        /// Gets or sets the Ach configuration repository.
        /// </summary>
        /// <value>The Ach configuration repository.</value>
        public IRepository<AchConfiguration> AchRepository { get; set; }

        /// <summary>
        ///  Field MetaData Repository.
        /// </summary>
        public IFieldMetaDataRepository FieldMetaDataRepository { get; set; }

        /// <summary>
        /// Data Source Repository
        /// </summary>
        public IDataSourceRepository DataSourceRepository { get; set; }

        /// <summary>
        /// Misc. repository, will be injected by container.
        /// </summary>
        /// <value>The misc invoice repository.</value>
        public IMiscInvoiceRepository MiscUatpInvoiceRepository { get; set; }

        public IRepository<OtherOrganizationInformation> OtherOrganizationInformationRepository { get; set; }

        /// <summary>
        /// Gets or sets the rejection memo Coupon Attachment repository.
        /// </summary>
        public IMiscUatpInvoiceAttachmentRepository MiscUatpInvoiceAttachmentRepository { get; set; }

        /// <summary>
        /// Gets or sets the line item repository.
        /// </summary>
        /// <value>The line item repository.</value>
        public ILineItemRepository LineItemRepository { get; set; }

        /// <summary>
        /// Gets or sets the correspondence repository.
        /// </summary>
        /// <value>The correspondence repository.</value>
        public IMiscCorrespondenceRepository CorrespondenceRepository { get; set; }

        /// <summary>
        /// Gets or sets the line item detail repository.
        /// </summary>
        /// <value>The line item detail repository.</value>
        public ILineItemDetailRepository LineItemDetailRepository { get; set; }

        public IRepository<FieldValue> FieldValueRepository { get; set; }

        /// <summary>
        /// ContactInformation Repository, will be injected by the container.
        /// </summary>
        public IRepository<ContactInformation> ContactInformationRepository { get; set; }

        /// <summary>
        /// Gets or sets the country repository.
        /// </summary>
        /// <value>The country repository.</value>
        public IRepository<Country> CountryRepository { get; set; }

        /// <summary>
        /// Gets or sets the Currency repository
        /// </summary>
        public IRepository<Currency> CurrencyRepository { get; set; }

        /// <summary>
        /// Misc Correspondence manager that will be injected by the container.
        /// </summary>
        public IMiscCorrespondenceManager CorrespondenceManager { get; set; }

        /// <summary>
        /// Gets or sets the ChargeCategory repository
        /// </summary>
        public IRepository<ChargeCategory> ChargeCategoryRepository { get; set; }

        /// <summary>
        /// Gets or sets the ChargeCode repository
        /// </summary>
        public IRepository<ChargeCode> ChargeCodeRepository { get; set; }

        /// <summary>
        /// Gets or sets the ChargeCodeTyep repository
        /// </summary>
        public IRepository<ChargeCodeType> ChargeCodeTypeRepository { get; set; }

        /// <summary>
        /// Gets or sets the UomCode repository
        /// </summary>
        public IRepository<UomCode> UomCodeRepository { get; set; }

        /// <summary>
        ///  Gets or sets the FieldMetaData repository
        /// </summary>
        public IRepository<FieldMetaData> FRepository { get; set; }

        /// <summary>
        /// FieldChargeCodeMappingRepository, injected through container.
        /// </summary>
        /// <value>The field charge code mapping repository.</value>
        public IRepository<FieldChargeCodeMapping> FieldChargeCodeMappingRepository { get; set; }

        /// <summary>
        /// MiscUatpInvoiceTaxRepository, injected through container.
        /// </summary>
        /// <value>The MISC/UATP invoice tax repository.</value>
        public IRepository<MiscUatpInvoiceTax> MiscUatpInvoiceTaxRepository { get; set; }

        /// <summary>
        /// Gets or sets the Misc/UATP invoice add on charge repository.
        /// </summary>
        /// <value>The Misc/UATP invoice add on charge repository.</value>
        public IRepository<InvoiceAddOnCharge> MiscUatpInvoiceAddOnChargeRepository { get; set; }

        /// <summary>
        /// LineItemTaxRepository, injected through container.
        /// </summary>
        /// <value>The line item tax repository.</value>
        public IRepository<LineItemTax> LineItemTaxRepository { get; set; }

        /// <summary>
        /// Gets or sets the exchange rate repository.
        /// </summary>
        /// <value>The exchange rate repository.</value>
        public IRepository<ExchangeRate> ExchangeRateRepository { get; set; }

        /// <summary>
        /// LineItemDetailTaxRepository, injected through container.
        /// </summary>
        /// <value>The line item tax repository.</value>
        public IRepository<LineItemDetailTax> LineItemDetailTaxRepository { get; set; }

        /// <summary>
        /// Gets or sets the line item detail add on charge repository.
        /// </summary>
        /// <value>The line item detail add on charge repository.</value>
        public IRepository<LineItemDetailAddOnCharge> LineItemDetailAddOnChargeRepository { get; set; }

        /// <summary>
        /// Gets or sets the line item add on charge repository.
        /// </summary>
        /// <value>The line item add on charge repository.</value>
        public IRepository<LineItemAddOnCharge> LineItemAddOnChargeRepository { get; set; }

        /// <summary>
        /// Gets or sets the misc correspondence repository.
        /// </summary>
        /// <value>The misc correspondence repository.</value>
        public IMiscCorrespondenceRepository MiscCorrespondenceRepository { get; set; }

        /// <summary>
        /// Gets or sets the line item additional detail repository.
        /// </summary>
        /// <value>The line item additional detail repository.</value>
        public IRepository<LineItemAdditionalDetail> LineItemAdditionalDetailRepository { get; set; }

        /// <summary>
        /// Gets or sets the line item detail additional detail repository.
        /// </summary>
        /// <value>The line item detail additional detail repository.</value>
        public IRepository<LineItemDetailAdditionalDetail> LineItemDetailAdditionalDetailRepository { get; set; }

        /// <summary>
        /// Gets or sets Misc/UATP invoice additional detail repository.
        /// </summary>
        /// <value>The Misc/UATP invoice additional detail repository.</value>
        public IRepository<MiscUatpInvoiceAdditionalDetail> MiscUatpInvoiceAdditionalDetailRepository { get; set; }

        ///// <summary>
        ///// Gets or sets the web validation error repository.
        ///// </summary>
        ///// <value>The web validation error repository.</value>
        //public IRepository<WebValidationError> WebValidationErrorRepository { get; set; }

        /// <summary>
        /// Gets or sets the invoice summary repository.
        /// </summary>
        /// <value>The invoice summary repository.</value>
        public IRepository<InvoiceSummary> InvoiceSummaryRepository { get; set; }

        /// <summary>
        /// Gets or sets the payment detail repository.
        /// </summary>
        /// <value>The payment detail repository.</value>
        public IRepository<PaymentDetail> PaymentDetailRepository { get; set; }

        /// <summary>
        /// FieldChargeCodeMapping List.
        /// </summary>
        private List<FieldChargeCodeMapping> _fieldChargeCodeMappings;

        /// <summary>
        /// FieldMetaData List.
        /// </summary>
        private IList<FieldMetaData> _fieldMetaDatas;

        public IOnBehalfInvoiceSetupManager _validationonBehalfManager { get; set; }

        /// <summary>
        /// Gets or sets the tax sub type repository.
        /// </summary>
        /// <value>The tax sub type repository.</value>
        public IRepository<TaxSubType> TaxSubTypeRepository { get; set; }

        /// <summary>
        /// Gets or sets the server validator repository.
        /// </summary>
        /// <value>The server validator repository.</value>
        public IRepository<ServerValidator> ServerValidatorRepository { get; set; }

         

        public IBlockingRulesRepository BlockingRulesRepository { get; set; }

        public ISupportingDocumentManager SupportingDocumentManager { get; set; }
        
        /// <summary>
        /// Gets or sets the misc code repository.
        /// </summary>
        /// <value>The misc code repository.</value>
        public IMiscCodeRepository MiscCodeRepository { get; set; }

        public IReasonCodeRepository ReasonCodeRepository { get; set; }

        public IRemoveInvoiceDupCheck RemoveInvoiceDuplicateCheck { get; set; }

        public IMiscUatpInvoiceManager _invoiceManager { get; set; }
      
        private MiscUatpErrorCodes MiscUatpErrorCodes;

        /// <summary>
        /// TimeLimit Repository.
        /// </summary>
        public IRepository<TimeLimit> TimeLimitRepository { get; set; }

        public MiscUatpInvoiceManager()
        {
        }

        public MiscUatpInvoiceManager(MiscUatpErrorCodes errorCodes)
        {
            MiscUatpErrorCodes = errorCodes;            
        }
        

        public IQueryable<MiscInvoiceSearchDetails> SearchInvoiceMisc(MiscSearchCriteria searchCriteria, int pageNo, int pageSize, string sortColumn, string sortOrder)
        {
            var filteredList = MiscInvoiceRepository.GetMiscManageInvoices(searchCriteria, pageSize, pageNo, sortColumn, sortOrder);
            return filteredList.AsQueryable();
        }
        /// <summary>
        /// Searches the invoice.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <returns></returns>
        // public IQueryable<MiscInvoiceSearch> SearchInvoice(MiscSearchCriteria searchCriteria) 
        public IList<MiscInvoiceSearch> SearchInvoice(MiscSearchCriteria searchCriteria)
        {
            //SCP425230 - PAYABLES OPTION
            MiscInvoiceRepository = Ioc.Resolve<IMiscInvoiceRepository>();
            var invoices = MiscInvoiceRepository.SearchMiscInvoiceRecords(searchCriteria, isPayableScreen: false);
            return invoices;

        }

        /// <summary>
        /// Searches the payable invoices.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <returns></returns>
        public IList<MiscInvoiceSearch> SearchPayableInvoices(MiscSearchCriteria searchCriteria)
        {
             //SCP425230 - PAYABLES OPTION
          MiscInvoiceRepository = Ioc.Resolve<IMiscInvoiceRepository>();
          searchCriteria.InvoiceStatusId = (int) InvoiceStatusType.Presented;
          var invoices = MiscInvoiceRepository.SearchMiscInvoiceRecords(searchCriteria, isPayableScreen: true);
          return invoices;

        }

        //CMP529 : Daily Output Generation for MISC Bilateral Invoices
        /// <summary>
        /// Searches the payable invoices.
        /// </summary>
        /// <param name="searchCriteria">The search criteria.</param>
        /// <returns></returns>
        //SCP382334: Daily Bilateral screen is not loading
        public List<MUDailyPayableResultData> SearchDailyPayableInvoices(MiscSearchCriteria searchCriteria)
        {
          MiscInvoiceRepository = Ioc.Resolve<IMiscInvoiceRepository>();

          var dailyPayableInvoices=  MiscInvoiceRepository.SearchDailyPayableInvoices(searchCriteria);

          return dailyPayableInvoices;
        }


        /// <summary>
        /// Creates the invoice.
        /// </summary>
        /// <param name="miscUatpInvoice">The misc invoice.</param>
        /// <returns></returns>
        public MiscUatpInvoice CreateInvoice(MiscUatpInvoice miscUatpInvoice)
        {
            // Mark the invoice status as Open.
            miscUatpInvoice.InvoiceStatus = InvoiceStatusType.Open;
            // MiscUatpInvoice.TransactionStatus = Iata.IS.Model.Pax.Enums.TransactionStatus.Open;
            miscUatpInvoice.ValidationStatus = InvoiceValidationStatus.Pending;
            miscUatpInvoice.ValidationDate = DateTime.MinValue;

            //fixed for issue id.5713 
            miscUatpInvoice.SettlementFileStatus = InvoiceProcessStatus.NotSet;

            // Validation for misc. invoice.
            
            //CMP602
            if(ValidateInvoiceHeader(miscUatpInvoice, null))
            {
              SetViewableByClearingHouse(miscUatpInvoice);
            }

            #region 279473 - Misc credit note- inconsistency between ISWEB and File behavior
            if (miscUatpInvoice.InvoiceType == InvoiceType.CreditNote)
            {
              if (!ReferenceManager.IsSmiLikeBilateral(miscUatpInvoice.SettlementMethodId,true) && (miscUatpInvoice.ListingCurrencyId != miscUatpInvoice.BillingCurrencyId))
              {
                miscUatpInvoice.IsValidationFlag = ExchangeRateValidationFlag;
              }
              else
              {
                miscUatpInvoice.IsValidationFlag = null;
              }
            }
            #endregion

            #region SCP# 414515 - Inquiry about Exchange Rate
            // After discussion - Decision was taken to preserve these internal fileds for Correspondence invoice.
            // This is required for displaying correct Ex. Rate while Editing Corr Invoice.
            // Desc: Reset settlement details for Correspondence Invoice before saving the invoice in DB.*/
            //// Explicitly set the settlement month to 0 in case of Correspondence Invoice as set and used in CreateBillingMemo.
            //if (miscUatpInvoice.InvoiceType == InvoiceType.CorrespondenceInvoice)
            //{
            //    miscUatpInvoice.SettlementYear = 0;
            //    miscUatpInvoice.SettlementMonth = 0;
            //    miscUatpInvoice.SettlementPeriod = 0;
            //} 
            #endregion

            MiscUatpInvoiceRepository.Add(miscUatpInvoice);
            UnitOfWork.CommitDefault();

            // Remove invoice Details from from DUP_INVOICE_LOG and DUP_INVOICE_TEMP tables (used to enforce duplicate check on invoice).
            try
            {
                RemoveInvoiceDuplicateCheck.RemoveDupCheckForInvoice(miscUatpInvoice.Id);
            }// End try
            catch (Exception ex)
            {
                Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1}", ex.Message, ex.StackTrace);
            }// End catch

            // If (string.IsNullOrEmpty(miscUatpInvoice.BillingMemberLocationCode))
            //{
            var billingMember = miscUatpInvoice.MemberLocationInformation.Where(location => location.IsBillingMember).FirstOrDefault();
            if (billingMember != null)
            {
                billingMember.InvoiceId = miscUatpInvoice.Id;
                UpdateMemberLocationInformation(billingMember, true, miscUatpInvoice);
            }
            //}

            // If (string.IsNullOrEmpty(miscUatpInvoice.BilledMemberLocationCode))
            //{
            var billedMember = miscUatpInvoice.MemberLocationInformation.Where(location => !location.IsBillingMember).FirstOrDefault();
            if (billedMember != null)
            {
                billedMember.InvoiceId = miscUatpInvoice.Id;
                UpdateMemberLocationInformation(billedMember, false, miscUatpInvoice);
            }
            //}
            return miscUatpInvoice;
        }

				/// <summary>
				/// This function is used to update bank detail from location to Other MU_OTHER_ORGANIZATION_INFO table.
				/// this information will use in the preview PDF  
				/// </summary>
				/// <param name="invoiceId"></param>
				/// <param name="memberId"></param>
				/// <param name="locationId"></param>
				/// <param name="settlementMethodId"></param>
				private void UpdateBankDetails(Guid invoiceId, int memberId, string locationId, int settlementMethodId)
				{
					//SCP491860 - KAL:SIS-Staging - bank detail info missing from Preview Invoice option
					var otherOrgInformation =
							OtherOrganizationInformationRepository.Get(o => o.InvoiceId == invoiceId).FirstOrDefault();
					if (otherOrgInformation != null)
					{
						OtherOrganizationInformationRepository.Delete(otherOrgInformation);

					}

					if (settlementMethodId == (int)SettlementMethodValues.Bilateral)
					{
						if (string.IsNullOrEmpty(locationId))
						{
							// get Details for main location
							locationId = "Main";
						}

						var locationDetails = MemberManager.GetMemberDefaultLocation(memberId, locationId);
						// if location not present for the given location id, then get data for Main location id.
						if (locationDetails == null && locationId != "Main")
						{
							locationDetails = MemberManager.GetMemberDefaultLocation(memberId, "Main");
						}

						if (locationDetails != null && !(string.IsNullOrEmpty(locationDetails.BankCode) && string.IsNullOrEmpty(locationDetails.BankAccountName) &&
											string.IsNullOrEmpty(locationDetails.BranchCode) &&
											string.IsNullOrEmpty(locationDetails.BankAccountNumber) &&
							string.IsNullOrEmpty(locationDetails.Iban) && string.IsNullOrEmpty(locationDetails.Swift)))
						{

							var otherOrganizationInformation = new OtherOrganizationInformation()
							{
								// SCP84533 - Bilateral UATP invoice not including bank details 
								// Implemented code to set OrganizationId to Location's BankName
								OtherOrganizationType = OtherOrganizationType.RemitTo,
								//Set OtherOrganizationType to Remit to for Misc
								BankCode = locationDetails.BankCode,
								BankName = locationDetails.BankAccountName,
								BranchCode = locationDetails.BranchCode,
								BankAccountNumber = locationDetails.BankAccountNumber,
								BankAccountName = locationDetails.BankAccountName,
								CurrencyId = locationDetails.CurrencyId,
								Iban = locationDetails.Iban,
								Swift = locationDetails.Swift,
								InvoiceId = invoiceId
							};

							OtherOrganizationInformationRepository.Add(otherOrganizationInformation);
						}
					}
				}

        public MiscUatpInvoice CreateBHRejectionInvoice(MiscUatpInvoice miscUatpInvoice, string lineItemIds)
        {
            var lineItemList = new List<string>();
            //CMP#502: [3.6] IS-WEB: Save of Invoice Header of Rejection Invoices
            if (miscUatpInvoice.InvoiceType == InvoiceType.RejectionInvoice && miscUatpInvoice.BillingCategory==BillingCategoryType.Misc)
            {
                var transactionTypeId = (int)TransactionType.MiscRejection1;
                if (miscUatpInvoice.RejectionStage == 2)
                {
                    transactionTypeId = (int)TransactionType.MiscRejection2;
                }
                //Validate Reason Code with Master "MST_REASON_CODE" 
                var reasonCode =
                    ReasonCodeRepository.Get(
                        reasonCodeRecoord =>
                        reasonCodeRecoord.IsActive && reasonCodeRecoord.Code.Equals(miscUatpInvoice.RejectionReasonCode) && reasonCodeRecoord.TransactionTypeId == transactionTypeId).FirstOrDefault();
                if (reasonCode == null)
                {
                    throw new ISBusinessException(MiscErrorCodes.InvalidRejReasonCodeProvidedIsWeb);
                }
            }
            CreateInvoice(miscUatpInvoice);
            foreach (var id in lineItemIds.Split(','))
            {
                if (string.IsNullOrEmpty(id))
                    continue;

                lineItemList.Add(id);
            }
            string ids = null;
            foreach (var lineItem in lineItemList)
            {
                ids = !string.IsNullOrEmpty(lineItem) ? string.IsNullOrEmpty(ids) ? lineItem : string.Format("{0},{1}", lineItem, ids) : ids;
            }

            // Replaced with LoadStrategy call
      //SCP119970 - We are unable to do rejection invoices via ISWEB
      //Logic: Fetching multiple records using billing member and invoice number only. 
      //Fix: In search cretria I have added Settlement month, period and year also.
      var oldInvoice = MiscUatpInvoiceRepository.Single(billingMemberId: miscUatpInvoice.BilledMemberId,
                                                        invoiceNumber: miscUatpInvoice.RejectedInvoiceNumber,
                                                        billingPeriod: miscUatpInvoice.SettlementPeriod,
                                                        billingMonth: miscUatpInvoice.SettlementMonth,
                                                        billingYear: miscUatpInvoice.SettlementYear,
                                                        invoiceStatusId: (int) InvoiceStatusType.Presented);
      //CMP#502: [3.6] IS-WEB: Save of Invoice Header of Rejection Invoices
      if (oldInvoice != null) MiscUatpInvoiceRepository.UpdateBHInvoice(miscUatpInvoice.Id, oldInvoice.Id, ids, miscUatpInvoice.RejectionReasonCode);

            return miscUatpInvoice;
        }

        /// <summary>
        /// Updates the invoice.
        /// </summary>
        /// <param name="miscUatpInvoice">The misc invoice.</param>
        /// <returns></returns>
        public MiscUatpInvoice UpdateInvoice(MiscUatpInvoice miscUatpInvoice)
        {
            var uatpInvoice = miscUatpInvoice;
            // Replaced with LoadStrategy call
            var miscUatpInvoiceInDb = MiscUatpInvoiceRepository.Single(uatpInvoice.Id);
            //CMP#624 : 2.17 : Change #17:Disallow change of SMI from a non-X value to X
            if (miscUatpInvoice.InvoiceType == InvoiceType.RejectionInvoice || miscUatpInvoice.InvoiceType == InvoiceType.CorrespondenceInvoice)
            {
              if (!ValidateSmiAfterLinking(miscUatpInvoice.SettlementMethodId, miscUatpInvoiceInDb.SettlementMethodId))
              {
                if (miscUatpInvoice.SettlementMethodId == (int) SMI.IchSpecialAgreement)
                {
                  throw new ISBusinessException(MiscUatpErrorCodes.MuInvoiceSmiChangeOtherSmiToX);
                }
                else
                {
                  throw new ISBusinessException(MiscUatpErrorCodes.MuInvoiceSmiChangeXtoOtherSmi );
                }
              }
            }
          //CMP602
          if(ValidateInvoiceHeader(miscUatpInvoice, miscUatpInvoiceInDb))
          {
            SetViewableByClearingHouse(miscUatpInvoice);
          }

          // Changes to update tax breakdown records
            UpdateTaxBreakdown(miscUatpInvoice, miscUatpInvoiceInDb);

            // Changes to update tax breakdown records
            UpdateVatBreakdown(miscUatpInvoice, miscUatpInvoiceInDb);

            // Changes to update tax breakdown records
            UpdateAddOnCharge(miscUatpInvoice, miscUatpInvoiceInDb);

            // Changes to update attachment breakdown records
            // UpdateAttachments(miscUatpInvoice, miscUatpInvoiceInDb);

            // Changes to update contact ContactInformation records
            UpdateMemberContact(miscUatpInvoice, miscUatpInvoiceInDb);

            // Update invoice additional detail.
            UpdateInvoiceAddionalDetail(miscUatpInvoice, miscUatpInvoiceInDb);

            // re-assigning legalPdfLocation property from database to avoid loss of 
            // data during update.
            miscUatpInvoice.LegalPdfLocation = miscUatpInvoiceInDb.LegalPdfLocation;

            // Change invoice status to 'Open' as modification done and update in DB.
            miscUatpInvoice.InvoiceStatus = InvoiceStatusType.Open;
            miscUatpInvoice.ValidationStatus = InvoiceValidationStatus.Pending;
            miscUatpInvoice.ValidationDate = DateTime.MinValue;

            miscUatpInvoice = MiscUatpInvoiceRepository.Update(miscUatpInvoice);

            //This table will update by procedure PROC_MU_UPDATE_INV_SUMMARY.
            //InvoiceSummaryRepository.Update(miscUatpInvoice.InvoiceSummary);
            PaymentDetailRepository.Update(miscUatpInvoice.PaymentDetail);

            var billingMember = miscUatpInvoice.MemberLocationInformation.Where(location => location.IsBillingMember).FirstOrDefault();
            if (billingMember != null)
            {
                billingMember.InvoiceId = miscUatpInvoice.Id;
                UpdateMemberLocationInformation(billingMember, true, miscUatpInvoice, false);
            }

            var billedMember = miscUatpInvoice.MemberLocationInformation.Where(location => !location.IsBillingMember).FirstOrDefault();
            if (billedMember != null)
            {
                billedMember.InvoiceId = miscUatpInvoice.Id;
                UpdateMemberLocationInformation(billedMember, false, miscUatpInvoice, false);
            }

            //CMP #648: Clearance Information in MISC Invoice PDFs
            if (miscUatpInvoice.ExchangeRate.HasValue)
            {
              if (miscUatpInvoice.ExchangeRate.Value > 0)
              {
                // Update TotalAmountInClearanceCurrency Amount
                miscUatpInvoice.ClearanceAmount = miscUatpInvoice.BillingAmount /
                                                Convert.ToDecimal(miscUatpInvoice.ExchangeRate.Value);
              }
              else
              {
                miscUatpInvoice.ClearanceAmount = 0.0M;
              }
            }
            else
            {
              miscUatpInvoice.ClearanceAmount = (decimal?)null;
            }

            UnitOfWork.CommitDefault();

            //SCP345230: SRM: SIS: ICH Settlement Error - SIS Production
            //SCP324672: Wrong amount invoice
            MiscUatpInvoiceRepository.UpdateMUInvoiceSummary(miscUatpInvoice.Id, miscUatpInvoice.InvoiceSummary.TotalTaxAmount, miscUatpInvoice.InvoiceSummary.TotalVatAmount, miscUatpInvoice.InvoiceSummary.TotalAddOnChargeAmount);

            return miscUatpInvoice;
        }

        /// <summary>
        /// Updates the attachments.
        /// </summary>
        /// <param name="miscUatpInvoice">The Misc/UATP invoice.</param>
        /// <param name="miscUatpInvoiceInDb">The Misc/UATP invoice in db.</param>
        private void UpdateAttachments(MiscUatpInvoice miscUatpInvoice, MiscUatpInvoice miscUatpInvoiceInDb)
        {
            var listToDeleteAttachment = miscUatpInvoiceInDb.Attachments.Where(attachment => miscUatpInvoice.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0).ToList();

            var attachmentIdList = (from attachment in miscUatpInvoice.Attachments
                                    where miscUatpInvoiceInDb.Attachments.Count(attachmentRecord => attachmentRecord.Id == attachment.Id) == 0
                                    select attachment.Id).ToList();

            var attachmentInDb = MiscUatpInvoiceAttachmentRepository.Get(miscUatpAttachment => attachmentIdList.Contains(miscUatpAttachment.Id));

            foreach (var recordAttachment in attachmentInDb)
            {
                if (IsDuplicateInvoiceAttachmentFileName(recordAttachment.OriginalFileName, miscUatpInvoiceInDb.Id))
                {
                    throw new ISBusinessException(MiscUatpErrorCodes.DuplicateFileName);
                }

                recordAttachment.ParentId = miscUatpInvoice.Id;
                MiscUatpInvoiceAttachmentRepository.Update(recordAttachment);
            }

            foreach (var couponRecordAttachment in listToDeleteAttachment)
            {
                MiscUatpInvoiceAttachmentRepository.Delete(couponRecordAttachment);
            }
        }

        /// <summary>
        /// Gets attachments for an invoice.
        /// </summary>
        /// <param name="invoiceId">Invoice Id.</param>
        /// <returns></returns>
        public IList<MiscUatpAttachment> GetAttachments(string invoiceId)
        {
            Guid invoiceGuid = invoiceId.ToGuid();
            var attachments = MiscUatpInvoiceAttachmentRepository.Get(attachment => attachment.ParentId == invoiceGuid);
            return attachments.ToList();
        }

        /// <summary>
        /// Update Sampling Form D attachment record parent id
        /// </summary>
        /// <param name="attachments">list of attachment</param>
        /// <param name="parentId">billing memo Id</param>
        /// <returns></returns>
        public IQueryable<MiscUatpAttachment> UpdateAttachments(IList<Guid> attachments, Guid parentId)
        {
            var attachmentInDb = MiscUatpInvoiceAttachmentRepository.Get(miscUatpAttachment => miscUatpAttachment.ParentId == parentId);
            var listToDeleteAttachment = attachmentInDb.Where(attachment => attachments.Count(attachmentRecord => attachmentRecord == attachment.Id) == 0).ToList();

            var samplingFormDAttachmentInDb = MiscUatpInvoiceAttachmentRepository.Get(record => attachments.Contains(record.Id));

            foreach (var recordAttachment in samplingFormDAttachmentInDb)
            {
                recordAttachment.ParentId = parentId;
                MiscUatpInvoiceAttachmentRepository.Update(recordAttachment);
            }

            foreach (var attachment in listToDeleteAttachment)
            {
                MiscUatpInvoiceAttachmentRepository.Delete(attachment);
            }

            UnitOfWork.CommitDefault();

            return samplingFormDAttachmentInDb.ToList().AsQueryable();
        }

        /// <summary>
        /// Updates the member contact.
        /// </summary>
        /// <param name="miscUatpInvoice">Misc/UATP invoice.</param>
        /// <param name="miscUatpInvoiceInDb">Misc/UATP invoice in db.</param>
        private void UpdateMemberContact(MiscUatpInvoice miscUatpInvoice, MiscUatpInvoice miscUatpInvoiceInDb)
        {
            var listToDeleteContact = miscUatpInvoiceInDb.MemberContacts.Where(contact => miscUatpInvoice.MemberContacts.Count(contactRecord => contactRecord.Id == contact.Id) == 0).ToList();

            foreach (var contactInformation in miscUatpInvoice.MemberContacts.Where(contact => contact.Id.CompareTo(new Guid()) == 0))
            {
                ContactInformationRepository.Add(contactInformation);
            }

            foreach (var contactInformation in listToDeleteContact)
            {
                ContactInformationRepository.Delete(contactInformation);
            }
        }

        /// <summary>
        /// Updates the add on charge.
        /// </summary>
        /// <param name="miscUatpInvoice">Misc/UATPinvoice.</param>
        /// <param name="miscUatpInvoiceInDb">Misc/UATP invoice in db.</param>
        private void UpdateAddOnCharge(MiscUatpInvoice miscUatpInvoice, MiscUatpInvoice miscUatpInvoiceInDb)
        {
            var listToDeleteAddOnCharge = miscUatpInvoiceInDb.AddOnCharges.Where(invoiceAddOnCharge => miscUatpInvoice.AddOnCharges.Count(addOnCharge => addOnCharge.Id == invoiceAddOnCharge.Id) == 0).ToList();

            foreach (var addOnCharge in miscUatpInvoice.AddOnCharges.Where(addOnCharge => addOnCharge.Id.CompareTo(new Guid()) == 0))
            {
                addOnCharge.ParentId = miscUatpInvoice.Id;
                MiscUatpInvoiceAddOnChargeRepository.Add(addOnCharge);
            }

            foreach (var addOnCharge in listToDeleteAddOnCharge)
            {
                MiscUatpInvoiceAddOnChargeRepository.Delete(addOnCharge);
            }
        }

        /// <summary>
        /// Updates the vat breakdown.
        /// </summary>
        /// <param name="miscUatpInvoice">Misc/UATP invoice.</param>
        /// <param name="miscUatpInvoiceInDb">Misc/UATP invoice in db.</param>
        private void UpdateVatBreakdown(MiscUatpInvoice miscUatpInvoice, MiscUatpInvoice miscUatpInvoiceInDb)
        {
            var listToDeleteVat = miscUatpInvoiceInDb.TaxBreakdown.Where(vat => miscUatpInvoice.TaxBreakdown.Count(vatRecord => vatRecord.Id == vat.Id) == 0 && vat.Type.ToUpper() == "VAT").ToList();

            foreach (var vat in miscUatpInvoice.TaxBreakdown.Where(vat => vat.Id.CompareTo(new Guid()) == 0 && vat.Type.ToUpper() == "VAT"))
            {
                vat.ParentId = miscUatpInvoice.Id;
                MiscUatpInvoiceTaxRepository.Add(vat);
            }

            foreach (var miscUatpInvoiceVat in listToDeleteVat)
            {
                MiscUatpInvoiceTaxRepository.Delete(miscUatpInvoiceVat);
            }
        }

        /// <summary>
        /// Updates the tax breakdown.
        /// </summary>
        /// <param name="miscUatpInvoice">Misc/UATP invoice.</param>
        /// <param name="miscUatpInvoiceInDb">Misc/UATP invoice in db.</param>
        private void UpdateTaxBreakdown(MiscUatpInvoice miscUatpInvoice, MiscUatpInvoice miscUatpInvoiceInDb)
        {
            var listToDeleteTax = miscUatpInvoiceInDb.TaxBreakdown.Where(tax => miscUatpInvoice.TaxBreakdown.Count(taxRecord => taxRecord.Id == tax.Id) == 0 && tax.Type.ToUpper() == "TAX").ToList();

            foreach (var tax in miscUatpInvoice.TaxBreakdown.Where(tax => tax.Id.CompareTo(new Guid()) == 0 && tax.Type.ToUpper() == "TAX"))
            {
                tax.ParentId = miscUatpInvoice.Id;
                MiscUatpInvoiceTaxRepository.Add(tax);
            }

            foreach (var miscUatpInvoiceTax in listToDeleteTax)
            {
                MiscUatpInvoiceTaxRepository.Delete(miscUatpInvoiceTax);
            }
        }

        /// <summary>
        /// Deletes the invoice.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns></returns>
        public bool DeleteInvoice(string invoiceId)
        {
            var invoiceGuid = invoiceId.ToGuid();

            // Replaced with LoadStrategy call
            var invoiceToBeDeleted = MiscUatpInvoiceRepository.Single(invoiceGuid);
            if (invoiceToBeDeleted == null) return false;

            if (invoiceToBeDeleted.InvoiceStatus == InvoiceStatusType.ErrorCorrectable
              || invoiceToBeDeleted.InvoiceStatus == InvoiceStatusType.ErrorNonCorrectable)
            {
                var miscellaneousConfiguration = MemberManager.GetMiscellaneousConfiguration(invoiceToBeDeleted.BillingMemberId);
                if (miscellaneousConfiguration != null &&
                    miscellaneousConfiguration.RejectionOnValidationFailure == RejectionOnValidationFailure.RejectFileInError)
                {
                    // Individual invoice deletion not allowed and that the entire file should be deleted from the Processing Dashboard.
                    throw new ISBusinessException(MiscUatpErrorCodes.InvalidRejectionOnValidationFailureStatus);
                }
            }

            MiscUatpInvoiceRepository.Delete(invoiceToBeDeleted);
            UnitOfWork.CommitDefault();
            return true;
        }

        /// <summary>
        /// Gets the invoice attachments.
        /// </summary>
        /// <param name="attachmentIds">The attachment ids.</param>
        /// <returns></returns>
        public IList<MiscUatpAttachment> GetInvoiceAttachments(List<Guid> attachmentIds)
        {
            return new List<MiscUatpAttachment>(MiscUatpInvoiceAttachmentRepository.Get(attachment => attachmentIds.Contains(attachment.Id)).ToList());
        }

        /// <summary>
        /// Updates the invoice attachment.
        /// </summary>
        /// <param name="attachments">The attachments.</param>
        /// <param name="parentId">The parent id.</param>
        /// <returns></returns>
        public IList<MiscUatpAttachment> UpdateInvoiceAttachment(IList<Guid> attachments, Guid parentId)
        {
            var attachmentInDb = MiscUatpInvoiceAttachmentRepository.Get(invoiceAttachment => attachments.Contains(invoiceAttachment.Id));
            foreach (var recordAttachment in attachmentInDb)
            {
                recordAttachment.ParentId = parentId;
                MiscUatpInvoiceAttachmentRepository.Update(recordAttachment);
            }
            UnitOfWork.CommitDefault();
            return attachmentInDb.ToList();
        }

        /// <summary>
        /// Gets the invoice attachment detail.
        /// </summary>
        /// <param name="attachmentId">The attachment id.</param>
        /// <returns></returns>
        public MiscUatpAttachment GetInvoiceAttachmentDetail(string attachmentId)
        {
            var attachmentGuid = attachmentId.ToGuid();
            var attachmentRecord = MiscUatpInvoiceAttachmentRepository.Single(attachment => attachment.Id == attachmentGuid);

            return attachmentRecord;
        }

        /// <summary>
        /// Determines whether specified file name already exists for given invoice.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>
        /// true if specified file name found in repository; otherwise, false.
        /// </returns>
        public bool IsDuplicateInvoiceAttachmentFileName(string fileName, Guid invoiceId)
        {
            return MiscUatpInvoiceAttachmentRepository.GetCount(attachment => attachment.ParentId == invoiceId && attachment.OriginalFileName.ToUpper() == fileName.ToUpper()) > 0;
        }

        /// <summary>
        /// Validates the invoice.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns></returns>
        public virtual MiscUatpInvoice ValidateInvoice(string invoiceId)
        {
            var webValidationErrors = new List<WebValidationError>();
            var miscUatpInvoiceGuid = invoiceId.ToGuid();

            // Replaced with LoadStrategy call
            var miscUatpInvoice = MiscUatpInvoiceRepository.Single(miscUatpInvoiceGuid);

            miscUatpInvoice.ValidationErrors.Clear();
            // Late submissions (where the period is the current open period less 1) will be marked as validation error (Error Non-Correctable); 
            // even if the late submission window for the past period is open in the IS Calendar.
            if (ReferenceManager.IsValidSmiForLateSubmission(miscUatpInvoice.SettlementMethodId) && IsLateSubmission(miscUatpInvoice))
            {
                miscUatpInvoice.ValidationStatus = InvoiceValidationStatus.ErrorPeriod;
                miscUatpInvoice.ValidationStatusId = (int)InvoiceValidationStatus.ErrorPeriod;
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoiceGuid,
                                                                                     MiscUatpErrorCodes.InvoiceLateSubmitted));
            }
            else
            {
              // Validate correctness of billing period.              
              if (!ValidateBillingPeriodForMisc(miscUatpInvoice))
              {
                miscUatpInvoice.ValidationStatus = InvoiceValidationStatus.Failed;
                miscUatpInvoice.ValidationStatusId = (int)InvoiceValidationStatus.Failed;
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoiceGuid,
                                                                                         MiscUatpErrorCodes.InvalidBillingPeriod));
              }
            }

            var billingMember = MemberManager.GetMember(miscUatpInvoice.BillingMemberId);

            if (!ValidateBillingMembershipStatus(billingMember))
            {
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoiceGuid,
                                                                                     MiscUatpErrorCodes.InvalidBillingIsMembershipStatus));
            }

            if (!ValidateBilledMemberStatus(miscUatpInvoice.BilledMember))
            {
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoiceGuid,
                                                                                     MiscUatpErrorCodes.InvalidBilledIsMembershipStatus));
            }


            // At least one transaction should exist.
            var lineItemCount = LineItemRepository.GetCount(lineItem => lineItem.InvoiceId == miscUatpInvoice.Id);
            if (lineItemCount <= 0)
            {
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoiceGuid,
                                                                                     MiscUatpErrorCodes.TransactionLineItemNotAvailable));
            }

            //CMP #636: Standard Update Mobilization
            ValidateChargeCodeType(miscUatpInvoice, ref webValidationErrors);   

            // Commenting this code as we are rolling up the line item detail data.
            // If (!ValidateLineTotal(miscUatpInvoice))
            //{
            //  webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoiceGuid, MiscUatpErrorCodes.InvalidLineTotal));
            //}

            // If invoice type is credit note, then total amount value should be negative
            // else check the total amount as per amount min max table.
            var totalInvoiceAmount = GetInvoiceTotalAmount(miscUatpInvoice);
            if (miscUatpInvoice.InvoiceType == InvoiceType.CreditNote)
            {
                if (totalInvoiceAmount >= 0)
                {
                    webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoiceGuid,
                                                                                         MiscUatpErrorCodes.
                                                                                           InvalidCreditNoteTotalAmount));
                }
            }
            else
            {
                // for all other invoices check if Total amount is greater than or equal to zero.
                // otherwise validate for min max limits as per transaction type.
                if (totalInvoiceAmount < 0)
                {
                    webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoiceGuid,
                                                                                         MiscUatpErrorCodes.
                                                                                           InvoiceTotalAmountNegative));
                }
                else if (!ValidateInvoiceTotalAmount(miscUatpInvoice))
                {
                    webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoiceGuid,
                                                                                         MiscUatpErrorCodes.
                                                                                           InvalidTotalAmountOutsideLimit));
                }
            }

            if (miscUatpInvoice.InvoiceSummary != null)
            {
                /* SCP# 302117: SRM: ICH Settlement Error - SIS Production - Error Code 21015 
                 * Desc: Update Total Amount in Currency of Billing. */
              //CMP#648: Convert Exchange rate into nullable field.
                if (miscUatpInvoice.ExchangeRate.HasValue &&  miscUatpInvoice.ExchangeRate.Value > 0)
                {
                   miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency =
                        ConvertUtil.Round((miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate.Value), 3);
                }
            }

            // Member Location information for both billing and billed member should present.
            var isbillingMemberRefData =
              miscUatpInvoice.MemberLocationInformation.Any(
                loc => loc.IsBillingMember && !string.IsNullOrEmpty(loc.CountryCode));
            var isbilledMemberRefData =
              miscUatpInvoice.MemberLocationInformation.Any(
                loc => !loc.IsBillingMember && !string.IsNullOrEmpty(loc.CountryCode));

            if ((string.IsNullOrEmpty(miscUatpInvoice.BillingMemberLocationCode) && !isbillingMemberRefData) ||
                (string.IsNullOrEmpty(miscUatpInvoice.BilledMemberLocationCode) && !isbilledMemberRefData))
            {
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoiceGuid,
                                                                                     MiscUatpErrorCodes.
                                                                                       InvalidMemberLocationInformation));
            }


            // Validation of location code on Invoice/Credit Note is Update for CMP#515
            if ((miscUatpInvoice.InvoiceType == InvoiceType.Invoice || miscUatpInvoice.InvoiceType == InvoiceType.CreditNote) && miscUatpInvoice.BillingCategory == BillingCategoryType.Misc)
            {
                // Validates location code required at Invoice/LineItem level.
                var locResult = MiscUatpInvoiceRepository.ValidateMiscUatpInvoiceLocation(miscUatpInvoice.Id);

                switch (locResult)
                {
                    // Location Code is always mandatory at either Invoice level or Line Item level
                    case 1:
                        webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoiceGuid, MiscUatpErrorCodes.LocationCodeRequiredAtInvoiceOrLineItem));
                        break;
                    //If Location Code is defined at Invoice level, it cannot be defined for any Line Item. 
                    //If Location Code is not defined at Invoice level, then it should be defined for every Line Item.
                    case 2:
                        webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoiceGuid, MiscUatpErrorCodes.LocationCodeRequiredAtLineItem));
                        break;
                }

                // Check if line item detail is present for invoice where line item detail is mandatory. If not, throw validation error.
                // This check is for Original Invoice only (as per comment from Rena)
                //SCP124490 - RE: AEF-GAJ-MAS-201302-1-0 (May-13 P2 SIS)
                if (miscUatpInvoice.InvoiceType == InvoiceType.Invoice)
                {
                    int lineItemNumber;
                    if (IsMandatoryLineItemDetailNotFound(miscUatpInvoiceGuid, miscUatpInvoice.BillingCategoryId, out lineItemNumber))
                    {
                        webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoiceGuid, MiscUatpErrorCodes.LineItemDetailExpectedButNotFound, lineItemNumber.ToString()));
                    } 
                }
            }

            if (miscUatpInvoice.InvoiceType == InvoiceType.CorrespondenceInvoice)
            {
                if (!string.IsNullOrEmpty(miscUatpInvoice.CorrespondenceRefNo.ToString()))
                {
                  try
                  {
                    ValidateCorrespondenceReference(miscUatpInvoice);
                  }
                  catch (ISBusinessException exception)
                  {
                    webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoiceGuid, exception.ErrorCode));
                  }
                }
            }

            /* // Blocked by Debtor
            if (CheckBlockedMember(true, miscUatpInvoice.BillingMemberId, miscUatpInvoice.BilledMemberId, isMisc: true))
            {
              webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoiceGuid, MiscUatpErrorCodes.InvalidBillingToMember));
            }

            // Blocked by Creditor
            if (CheckBlockedMember(false, miscUatpInvoice.BilledMemberId, miscUatpInvoice.BillingMemberId, isMisc: true))
            {
              webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoiceGuid, MiscUatpErrorCodes.InvalidBillingFromMember));
            } */

            try
            {
                // Validation for Blocked Airline
                ValidationForBlockedAirline(miscUatpInvoice);
            }
            catch (ISBusinessException exception)
            {
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoiceGuid, exception.ErrorCode));
            }

            //SCP277476: Validate MISC TAX, VAT and Add on Charge Total amount against its breakdown total

            /* SCP# 373159 - Zero totals in LA to JL MISC CN 62843, 2015-May-P3 
            Desc: From Existing SP (PROC_INV_VAL_FOR_MISC_BRDOWN) called from here, Hooked a call to SP PROC_MU_UPDATE_INV_SUMMARY, 
            to update Amounts in MU_INVOICE_TOTAL table at the time of invoice validation. */
            var validationResult = MiscUatpInvoiceRepository.ValidateMiscInvoiceTotalAndBreakdownAmount(miscUatpInvoiceGuid);

						//SCP491860 - KAL:SIS-Staging - bank detail info missing from Preview Invoice option
						// Adds member location information.
						UpdateMemberLocationInformation(miscUatpInvoice);

						UpdateBankDetails(miscUatpInvoice.Id, miscUatpInvoice.BillingMemberId, miscUatpInvoice.BillingMemberLocationCode, miscUatpInvoice.SettlementMethodId);
						UnitOfWork.CommitDefault();

            if (!validationResult.Equals("PASS"))
            {
              webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoiceGuid, MiscUatpErrorCodes.TotalAmountAndBreakdownMisMatch, validationResult));
            }

            if (webValidationErrors.Count > 0)
            {
                miscUatpInvoice.ValidationErrors.AddRange(webValidationErrors);
            }

          return miscUatpInvoice;
        }

        /// <summary>
        /// This function is used to validate charge code type for line item for rejection invoices.
        /// </summary>
        /// <param name="miscUatpInvoice"></param>
        /// <param name="webValidationErrors"></param>
        /// CMP #636: Standard Update Mobilization
        private void ValidateChargeCodeType(MiscUatpInvoice miscUatpInvoice, ref List<WebValidationError> webValidationErrors)
        {
            /*
             * If charge code type has been defined for combination of charge category and charge code.
             *    If charge code type requirement is not active for combination of charge category and charge code.
             *       If charge code type provided in the file then we will raise error for that scenario.
             *    Else 
             *        If charge code type is provided in file
             *             If active charge code type is not exist in the master 
             *               then raise error.
             *             Else
             *               If charge code type is mismatch with DB charge code type
             *                 then raise error.
             *       ELSE IF(charge code type requirement is mandatory for combination of charge category and charge code.)
             *              raise error as non correctable because charge code type is not  provided in the file.
             * Else (charge code type provided in the file then we will raise error for that scenario.)
            */  
          //This validate will apply when invoice type must be rejection invoice and billing category must be MISC.
          if (miscUatpInvoice.InvoiceType == InvoiceType.RejectionInvoice && miscUatpInvoice.BillingCategory == BillingCategoryType.Misc)
          {
            //Get line item based on invoice id,
            var lineItems = GetLineItemList(miscUatpInvoice.Id.ToString());
            foreach (var lineItem in lineItems)
            {
              //Get Charge Code detail based on charge code Id.
              var chargeCode = ReferenceManager.GetChargeCodeDetail(lineItem.ChargeCodeId);

              if (chargeCode.IsChargeCodeTypeRequired != null)
              {
                if (!chargeCode.IsActiveChargeCodeType)
                {
                  if (lineItem.ChargeCodeType != null)
                  {
                    //Add Error
                    webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(MiscErrorCodes.ChargeCodeTypeNotApplicableOfLineItem, miscUatpInvoice.Id, lineItem.LineItemNumber.ToString()));
                  }
                }
                else
                {
                  if (lineItem.ChargeCodeType != null)
                  {
                    var chargeCodeType = ReferenceManager.GetChargeCodeType(lineItem.ChargeCodeId, true);
                    if (chargeCodeType.Count == 0)
                    {
                      //Add Error
                      webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(MiscErrorCodes.InvalidChargeCodeTypeOfLineItem, miscUatpInvoice.Id, lineItem.LineItemNumber.ToString()));
                    }
                    else
                    {
                      var chargeCodeTypeCount = chargeCodeType.Count(c => c.Name.ToUpper() == lineItem.ChargeCodeType.Name.ToUpper());
                      if (chargeCodeTypeCount == 0)
                      {
                        //Add Error
                        webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(MiscErrorCodes.InvalidChargeCodeTypeOfLineItem, miscUatpInvoice.Id, lineItem.LineItemNumber.ToString()));
                      }
                    }
                  }
                  else if (chargeCode.IsChargeCodeTypeRequired.Value)
                  {
                    //Add Error
                    webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(MiscErrorCodes.MissingChargeCodeTypeOfLineItem, miscUatpInvoice.Id, lineItem.LineItemNumber.ToString()));
                  }
                }
              }
              else
              {
                if (lineItem.ChargeCodeType != null)
                {
                  //Add Error
                  webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(MiscErrorCodes.ChargeCodeTypeNotApplicableOfLineItem, miscUatpInvoice.Id, lineItem.LineItemNumber.ToString()));
                }
              }
            }
          }
        }

        //SCP219674 : InvalidAmountToBeSettled Validation   
        private void ValidateCorrespondenceReference(MiscUatpInvoice miscUatpInvoice)
        {
          var correspondence = CorrespondenceManager.GetCorrespondenceDetails(miscUatpInvoice.CorrespondenceRefNo);
          if (correspondence == null)
          {
            throw new ISBusinessException(MiscUatpErrorCodes.InvalidCorrRefNo);            
          }
          else
          {
            if (correspondence.CorrespondenceStatus == CorrespondenceStatus.Closed)
            {
              throw new ISBusinessException(MiscUatpErrorCodes.CorrRefNoClosed);
            }

            if (!GetIsValidCorrAmountToBeSettled(miscUatpInvoice, correspondence))
            {
              throw new ISBusinessException(CargoErrorCodes.CgoInvalidAmountToBeSettled);
            }
          }
        }

      private bool GetIsValidCorrAmountToBeSettled(MiscUatpInvoice miscUatpInvoice, MiscCorrespondence correspondence)
      {
        var isValid = true;
        decimal invoiceAmountToBeCompared;

        var miscCorrespondence =
            MiscCorrespondenceRepository.Get(miscUatpInvoice.CorrespondenceRefNo).OrderByDescending(
              corr => corr.CorrespondenceStage).FirstOrDefault();
        // Get the original Invoice .
        var invoice = GetOriginalInvoice(miscUatpInvoice, miscCorrespondence);

        var originalInvoice = new MiscUatpInvoice()
                                                {
                                                  BillingPeriod = invoice.BillingPeriod == 0 ? 1 : invoice.BillingPeriod,
                                                  BillingMonth = invoice.BillingMonth,
                                                  BillingYear = invoice.BillingYear,
                                                  SettlementMethodId = miscUatpInvoice.SettlementMethodId
                                                };

        // Case 1: If Currency of Correspondence and Currency of Invoice are the same
        if (correspondence.CurrencyId == miscUatpInvoice.ListingCurrencyId)
        {
          invoiceAmountToBeCompared = miscUatpInvoice.InvoiceSummary.TotalAmount;

          var tolerance = CompareUtil.GetTolerance(BillingCategoryType.Misc, miscUatpInvoice.ListingCurrencyId.Value, originalInvoice,
                                                                                                Constants.MiscDecimalPlaces);

          isValid = Iata.IS.Business.Common.Impl.ReferenceManager.ValidateAmounts(invoiceAmountToBeCompared, correspondence.AmountToBeSettled, tolerance, Constants.MiscDecimalPlaces);
         
        }
          // Case 2: If Currency of Correspondence and Currency of Clearance are the same
        else if (correspondence.CurrencyId == miscUatpInvoice.BillingCurrencyId)
        {
          invoiceAmountToBeCompared = miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.Value;

          var tolerance = CompareUtil.GetTolerance(BillingCategoryType.Misc, miscUatpInvoice.BillingCurrencyId.Value, originalInvoice,
                                                                                                Constants.MiscDecimalPlaces);

          isValid = Iata.IS.Business.Common.Impl.ReferenceManager.ValidateAmounts(invoiceAmountToBeCompared, correspondence.AmountToBeSettled, tolerance, Constants.MiscDecimalPlaces);
        }
        else // If Currency of Correspondence doesn’t match with either currencies of the Correspondence Invoice
        {         

          decimal netBilledAmount = miscUatpInvoice.InvoiceSummary.TotalAmount;
          isValid = ReferenceManager.ValidateCorrespondenceAmounttobeSettled(miscUatpInvoice,
                                                                             ref netBilledAmount,
                                                                             correspondence.CurrencyId.Value,
                                                                             correspondence.AmountToBeSettled, originalInvoice);
        }
        return isValid;
      }


        public bool validateVoidPeriod(MiscUatpInvoice invoice)
        {
            var clearingHouse = ReferenceManager.GetClearingHouseToFetchCurrentBillingPeriod(invoice.SettlementMethodId);
            // var tt = CalendarManager.GetCurrentBillingPeriod(clearingHouse);
            BillingPeriod currentPeriod;

            try
            {

                // Try to get the current billing period.
                currentPeriod = CalendarManager.GetCurrentBillingPeriod(clearingHouse); //GetCurrentBillingPeriod(clearingHouse);
            }
            catch (ISCalendarDataNotFoundException)
            {
                // Current billing period not found, try to get the next billing period.
                var previousBillingPeriod = CalendarManager.GetLastClosedBillingPeriod(clearingHouse);

                if (!CalendarManager.IsLateSubmissionWindowOpen(clearingHouse, previousBillingPeriod))
                {
                    return false;
                }
                else
                {
                    return true;
                }

                return false;
            }
            return true;

        }
        /// <summary>
        /// Get the billing month of original invoice. Used for calculating exchange rate for correspondence invoice.
        /// </summary>
        /// <param name="correspondenceInvoice"></param>
        /// <param name="miscCorrespondence"></param>
        /// <returns></returns>
        public MiscUatpInvoice GetOriginalInvoice(MiscUatpInvoice correspondenceInvoice, MiscCorrespondence miscCorrespondence)
        {
            MiscUatpInvoice originalInvoice = null;
            MiscUatpInvoice rejectedInvoice1 = null;

            if (miscCorrespondence != null)
            {
                rejectedInvoice1 = MiscUatpInvoiceRepository.Single(miscCorrespondence.InvoiceId,
                                                                    billingCategoryId: (int)BillingCategory);

                if (rejectedInvoice1 == null)
                    return correspondenceInvoice;
                if (rejectedInvoice1.RejectionStage == 1)
                {
                    // Fetch original invoice
                    // SCP287408: Problem validating a correspondence invoice
                    // Added Settlement Year-Month-Period check
                    originalInvoice = MiscUatpInvoiceRepository.Single(invoiceNumber: rejectedInvoice1.RejectedInvoiceNumber,
                                                                       billingMemberId: rejectedInvoice1.BilledMemberId,
                                                                       billedMemberId: rejectedInvoice1.BillingMemberId,
                                                                       billingCategoryId: rejectedInvoice1.BillingCategoryId,
                                                                       invoiceStatusId: (int)InvoiceStatusType.Presented,
                                                                       billingPeriod: rejectedInvoice1.SettlementPeriod,
                                                                       billingMonth:rejectedInvoice1.SettlementMonth,
                                                                       billingYear:rejectedInvoice1.SettlementYear);
                    if (originalInvoice != null)
                        return originalInvoice;

                    // Original invoice not found.
                    return new MiscUatpInvoice
                    {
                        BillingMonth = rejectedInvoice1.SettlementMonth,
                        BillingYear = rejectedInvoice1.SettlementYear,
                        BillingPeriod = rejectedInvoice1.SettlementPeriod
                    };
                }

                if (rejectedInvoice1.RejectionStage == 2) // If rejection stage is 2 
                {
                    // Get rejected invoice from invoice number from rejection details.
                    // SCP287408: Problem validating a correspondence invoice
                    // Added Settlement Year-Month-Period check
                    rejectedInvoice1 = MiscUatpInvoiceRepository.Single(invoiceNumber: rejectedInvoice1.RejectedInvoiceNumber,
                                                                        billingMemberId: rejectedInvoice1.BilledMemberId,
                                                                        billedMemberId: rejectedInvoice1.BillingMemberId,
                                                                        billingCategoryId: rejectedInvoice1.BillingCategoryId,
                                                                        invoiceStatusId: (int)InvoiceStatusType.Presented,
                                                                        billingPeriod: rejectedInvoice1.SettlementPeriod,
                                                                        billingMonth: rejectedInvoice1.SettlementMonth,
                                                                        billingYear: rejectedInvoice1.SettlementYear);

                    if (rejectedInvoice1 != null)
                    {

                        // Fetch original invoice
                        // SCP287408: Problem validating a correspondence invoice
                        // Added Settlement Year-Month-Period check
                        originalInvoice = MiscUatpInvoiceRepository.Single(invoiceNumber: rejectedInvoice1.RejectedInvoiceNumber,
                                                                           billingMemberId: rejectedInvoice1.BilledMemberId,
                                                                           billedMemberId: rejectedInvoice1.BillingMemberId,
                                                                           billingCategoryId: rejectedInvoice1.BillingCategoryId,
                                                                           invoiceStatusId: (int)InvoiceStatusType.Presented,
                                                                           billingPeriod: rejectedInvoice1.SettlementPeriod,
                                                                           billingMonth: rejectedInvoice1.SettlementMonth,
                                                                           billingYear: rejectedInvoice1.SettlementYear);
                        // Original invoice not found.
                        if (originalInvoice == null)
                            // Original invoice not found.
                            return new MiscUatpInvoice
                            {
                                BillingMonth = rejectedInvoice1.SettlementMonth,
                                BillingYear = rejectedInvoice1.SettlementYear,
                                BillingPeriod = rejectedInvoice1.SettlementPeriod
                            };

                        return originalInvoice;
                    }

                    // Stage 1 rejection not found, original invoice not found, then use billing month of correspondence invoice.
                    return correspondenceInvoice;
                }
            }

            return originalInvoice;
        }

        /// <summary>
        /// Validates the line total is equals to sum of all line item details line total.
        /// </summary>
        /// <param name="miscUatpInvoice">The MISC/UATP invoice.</param>
        /// <returns></returns>
        private bool ValidateLineTotal(MiscUatpInvoice miscUatpInvoice)
        {
            return !(from lineItem in miscUatpInvoice.LineItems
                     where IsFieldMetaDataExists(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, miscUatpInvoice)
                     let item = lineItem
                     let lineItemDetails = LineItemDetailRepository.Get(lineItemId: item.Id)
                     let lineItemDetailChargeAmountSum = lineItemDetails.Sum(lineItemDetail => lineItemDetail.ChargeAmount)
                     where ConvertUtil.Round(lineItem.ChargeAmount, Constants.MiscDecimalPlaces) != ConvertUtil.Round(lineItemDetailChargeAmountSum, Constants.MiscDecimalPlaces)
                     select lineItem).Any();
        }

        /// <summary>
        /// Marks the invoices in the invoice id list as presented.
        /// Note: This is only used for testing - will/should never be used in production.
        /// </summary>
        /// <param name="invoiceIdList">List of invoice ids to be submitted</param>
        /// <returns></returns>
        public IList<MiscUatpInvoice> ProcessingCompleteInvoices(List<string> invoiceIdList)
        {
            var invoiceList = invoiceIdList.Select(ProcessingCompleteInvoice).ToList();

            return invoiceList.Where(invoice => invoice != null && invoice.InvoiceStatus == InvoiceStatusType.ProcessingComplete).ToList();
        }

        public MiscUatpInvoice ProcessingCompleteInvoice(string invoiceId)
        {
            var invoiceGuid = invoiceId.ToGuid();
            var invoice = MiscUatpInvoiceRepository.Single(invoiceGuid);

            if (invoice.InvoiceStatus == InvoiceStatusType.ProcessingComplete)
            {
                return null;
            }

            // Allow to mark invoice as ProcessingComplete if invoice status is ReadyForBilling
            if (invoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
            {
                invoice.InvoiceDate = DateTime.UtcNow;
                invoice.InvoiceStatus = InvoiceStatusType.ProcessingComplete;

                // Update invoice to database.
                var updatedInvoice = MiscUatpInvoiceRepository.Update(invoice);
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
        public IList<MiscUatpInvoice> PresentInvoices(List<string> invoiceIdList)
        {
            var invoiceList = invoiceIdList.Select(PresentInvoice).ToList();

            return invoiceList.Where(invoice => invoice != null && invoice.InvoiceStatus == InvoiceStatusType.Presented).ToList();
        }

        /// <summary>
        /// Marks the invoice as presented.
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public MiscUatpInvoice PresentInvoice(string invoiceId)
        {
            var invoiceGuid = invoiceId.ToGuid();
            var invoice = MiscUatpInvoiceRepository.Single(invoiceGuid);

            if (invoice.InvoiceStatus == InvoiceStatusType.Presented)
            {
                return null;
            }

            // Allow to mark invoice as presented if invoice status is ReadyForBilling or ProcessingComplete
            if (invoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling || invoice.InvoiceStatus == InvoiceStatusType.ProcessingComplete)
            {
                invoice.InvoiceDate = DateTime.UtcNow;
                invoice.InvoiceStatus = InvoiceStatusType.Presented;

                // Update invoice to database.
                var updatedInvoice = MiscUatpInvoiceRepository.Update(invoice);
                UnitOfWork.CommitDefault();

                return updatedInvoice;
            }

            return invoice;
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
          var hasInvSubmitPermit = authorizationManager.IsAuthorized(userId,
                                                                     Security.Permissions.Misc.Receivables.Invoice.
                                                                       Submit);
          var hasCreditNoteSubmitPermit = authorizationManager.IsAuthorized(userId,
                                                                            Security.Permissions.Misc.Receivables.
                                                                              CreditNote.Submit);

          var invTobeSubmit = new List<string>();
          foreach (var invId in invIdList)
          {
            var invoice = _invoiceManager.GetInvoiceDetail(invId);

            if ((invoice.InvoiceType == InvoiceType.Invoice || invoice.InvoiceType == InvoiceType.CorrespondenceInvoice || invoice.InvoiceType == InvoiceType.RejectionInvoice) && hasInvSubmitPermit)
            {
              invTobeSubmit.Add(invoice.Id.ToString());
            }
            if (invoice.InvoiceType == InvoiceType.CreditNote && hasCreditNoteSubmitPermit)
            {
              invTobeSubmit.Add(invoice.Id.ToString());
            }
          }

          return invTobeSubmit;
        }

        /// <summary>
        /// Submits the invoice.
        /// </summary>
        /// <param name="invoiceIdList">List of invoice id.</param>
        /// <returns></returns>
        public IList<MiscUatpInvoice> SubmitInvoices(List<string> invoiceIdList)
        {
          var invoiceList = invoiceIdList.Select(SubmitInvoice).ToList();
          //CMP #626: Future Submission for MISC category
          return invoiceList.Where(invoice => invoice != null && (invoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling || invoice.InvoiceStatus == InvoiceStatusType.FutureSubmitted)).ToList();
        }

        /// <summary>
        /// Submits the invoice.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns></returns>
        public MiscUatpInvoice SubmitInvoice(string invoiceId)
        {
            var webValidationErrors = new List<WebValidationError>();
            var miscUatpInvoiceGuid = invoiceId.ToGuid();

            // Replaced with LoadStrategy call
            var miscUatpInvoice = MiscUatpInvoiceRepository.Single(invoiceId: miscUatpInvoiceGuid);

            // User Id of logged in member
            var userId = miscUatpInvoice.LastUpdatedBy;

            // Get ValidationErrors for invoice from DB.
            var validationErrorsInDb = ValidationErrorManager.GetValidationErrors(invoiceId);
            miscUatpInvoice.ValidationErrors.Clear();


            if (miscUatpInvoice.InvoiceStatus != InvoiceStatusType.ReadyForSubmission)
            {
                return miscUatpInvoice;
            }

            // Re-fetch the billing and billed member - since we are getting a stale state (workaround)!
            //   miscUatpInvoice.BillingMember = MemberManager.GetMember(miscUatpInvoice.BillingMemberId);
            //   miscUatpInvoice.BilledMember = MemberManager.GetMember(miscUatpInvoice.BilledMemberId);
            var billingMember = MemberManager.GetMember(miscUatpInvoice.BillingMemberId);
            var billedMember = MemberManager.GetMember(miscUatpInvoice.BilledMemberId);

            // Get Final Parent Details for SMI, Currency, Clearing House abd Suspended Flag validations
            var billingFinalParent = MemberManager.GetMember(MemberManager.GetFinalParentDetails(miscUatpInvoice.BillingMemberId));
            var billedFinalParent = MemberManager.GetMember(MemberManager.GetFinalParentDetails(miscUatpInvoice.BilledMemberId));

            // IS Membership validations.
            if (!ValidateBilledMemberStatus(miscUatpInvoice.BilledMember))
            {
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoice.Id, MiscUatpErrorCodes.InvalidBilledIsMembershipStatus));
            }

            if (!ValidateBillingMembershipStatus(miscUatpInvoice.BillingMember))
            {
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoice.Id, MiscUatpErrorCodes.InvalidBillingIsMembershipStatus));
            }

            try
            {
                // Validation for Blocked Airline
                ValidationForBlockedAirline(miscUatpInvoice);
            }
            catch (ISBusinessException exception)
            {
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoice.Id, exception.ErrorCode));
            }

            //SCP219674 : InvalidAmountToBeSettled Validation   ***************************************
            if (miscUatpInvoice.InvoiceType == InvoiceType.CorrespondenceInvoice)
            {
              if (!string.IsNullOrEmpty(miscUatpInvoice.CorrespondenceRefNo.ToString()))
              {
                try
                {
                  ValidateCorrespondenceReference(miscUatpInvoice);
                }
                catch (ISBusinessException exception)
                {
                  webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(invoiceId.ToGuid(), exception.ErrorCode));
                }
              }
            }
           //******************************************

            // Late submissions (where the period is the current open period less 1) will be marked as validation error (Error Non-Correctable); 
            // even if the late submission window for the past period is open in the IS Calendar.
            if (webValidationErrors.Count <= 0 && IsLateSubmission(miscUatpInvoice))
            {
                miscUatpInvoice.ValidationStatus = InvoiceValidationStatus.ErrorPeriod;
                miscUatpInvoice.ValidationStatusId = (int)InvoiceValidationStatus.ErrorPeriod;
                webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoice.Id, MiscUatpErrorCodes.InvoiceLateSubmitted));
            }
            else
            {
                // Validate correctness of billing period.
              if (!ValidateBillingPeriodForMisc(miscUatpInvoice))
                {
                    webValidationErrors.Add(ValidationErrorManager.GetWebValidationError(miscUatpInvoice.Id, MiscUatpErrorCodes.InvalidBillingPeriod));
                }
            }

            // Update invoice status in case of error.
            if (webValidationErrors.Count > 0)
            {
                miscUatpInvoice.InvoiceStatus = InvoiceStatusType.ValidationError;
                miscUatpInvoice.ValidationErrors.AddRange(webValidationErrors);

                if (miscUatpInvoice.ValidationStatus != InvoiceValidationStatus.ErrorPeriod)
                    miscUatpInvoice.ValidationStatus = InvoiceValidationStatus.Failed;
            }
            else
            {
                // Every validation is successful. Update invoice status as Ready for billing and invoice date as current date.
                // removed following line to fix #3996
                //miscUatpInvoice.InvoiceDate = DateTime.UtcNow;
                // If Billed or Billing member are suspended update Invoice suspended flag to true. 
                if (ValidateSuspendedFlag(miscUatpInvoice, billingFinalParent, billedFinalParent))
                {
                    miscUatpInvoice.SuspendedInvoiceFlag = true;
                }

                if (miscUatpInvoice.BillingCategory == BillingCategoryType.Misc)
                {
                  miscUatpInvoice.InvoiceStatus = miscUatpInvoice.IsFutureSubmission
                                    ? InvoiceStatusType.FutureSubmitted
                                    : InvoiceStatusType.ReadyForBilling;
                }
                else
                {
                  miscUatpInvoice.InvoiceStatus = InvoiceStatusType.ReadyForBilling;
                }
            }

            // Update validation errors in db.
            ValidationErrorManager.UpdateValidationErrors(miscUatpInvoice.Id, miscUatpInvoice.ValidationErrors, validationErrorsInDb);

            // Update clearing house of invoice
            var clearingHouse = ReferenceManager.GetClearingHouseForInvoice(miscUatpInvoice, billingFinalParent, billedFinalParent);
            miscUatpInvoice.ClearingHouse = clearingHouse;

            // Set Sponsored By 
            var ichConfiguration = MemberManager.GetIchConfig(miscUatpInvoice.BillingMember.Id);
            if (ichConfiguration != null && ichConfiguration.SponsoredById.HasValue)
            {
                miscUatpInvoice.SponsoredById = ichConfiguration.SponsoredById;
            }

            // Adds member location information.
            UpdateMemberLocationInformation(miscUatpInvoice);

            // Update invoice to database.
            var updatedInvoice = MiscUatpInvoiceRepository.Update(miscUatpInvoice);

						if (miscUatpInvoice.SettlementMethodId == (int)SettlementMethodValues.Bilateral)
							UpdateBankDetails(miscUatpInvoice.Id, miscUatpInvoice.BillingMemberId, miscUatpInvoice.BillingMemberLocationCode, miscUatpInvoice.SettlementMethodId);

            // Update DS Required By as per billing member location country and DS Required flag in member profile.
            SetDigitalSignatureInfo(miscUatpInvoice, billingMember, billedMember);

            // On submitting correspondence invoice, update status of correspondence as Closed and sub status as Billed
            if (miscUatpInvoice.InvoiceType == InvoiceType.CorrespondenceInvoice)
            {
              if (!string.IsNullOrEmpty(miscUatpInvoice.CorrespondenceRefNo.ToString()))
              {
                // Get all correspondence having given correspondence no.
                var correspondence = CorrespondenceRepository.GetCorr(corr => corr.CorrespondenceNumber == miscUatpInvoice.CorrespondenceRefNo);

                // SCP61363: Correspondence 0980000091 Closed due to Expiry
                // Update status of entire correspondence trail to "Closed - Billed" if corresp status and sub status is other than "Open - Saved/ReadyForSubmit".
                // Also delete correspondence if corresp status and sub status is equal to "Open - Saved/ReadyForSubmit".
                foreach (var corr in correspondence)
                {
                  // Delete correspondence.
                  if (corr.CorrespondenceStatus == CorrespondenceStatus.Open && (corr.CorrespondenceSubStatus == CorrespondenceSubStatus.Saved || corr.CorrespondenceSubStatus == CorrespondenceSubStatus.ReadyForSubmit))
                  {
                    Logger.InfoFormat("Deleting Correspondence in 'Open - Saved/ReadyForSubmit' state. Correspondence No: {0}, Stage: {1}, Status: {2}, Sub-Status: {3}", corr.CorrespondenceNumber, corr.CorrespondenceStage, corr.CorrespondenceStatusId, corr.CorrespondenceSubStatusId);
                    CorrespondenceRepository.Delete(corr);
                  }// End if
                  else
                  {
                    corr.CorrespondenceSubStatus = CorrespondenceSubStatus.Billed;
                    corr.CorrespondenceStatus = CorrespondenceStatus.Closed;
                  }// End else

                }// End foreach

              }// End if
              //SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
              // Update expiry period using only lead period for correspondence invoice and all transactions prior to it for purging.
              //DateTime expiryPeriod = ReferenceManager.GetExpiryDatePeriodForClosedCorrespondence(miscUatpInvoice, BillingCategory, Constants.SamplingIndicatorNo, null);
              //MiscUatpInvoiceRepository.UpdateExpiryDatePeriod(miscUatpInvoice.Id, (int)CorrInvoiceDABTransactionType, expiryPeriod);
            }// End if
            //else
            //{
            //  TransactionType currentTransactionType;
            //  // Update expiry period for purging - Original invoice/credit note/rejection invoice.

            //  DateTime expiryPeriod = GetExpiryDatePeriod(miscUatpInvoice, null, out currentTransactionType);
            //  MiscUatpInvoiceRepository.UpdateExpiryDatePeriod(miscUatpInvoice.Id, (int) currentTransactionType, expiryPeriod);

            //} // End else

          UnitOfWork.CommitDefault();

            if (updatedInvoice.InvoiceStatus == InvoiceStatusType.ReadyForBilling)
            {
                Logger.InfoFormat("Update invoice '{0}' info after Invoice status changed as Ready for billing.", ConvertUtil.ConvertGuidToString(updatedInvoice.Id));
                try
                {
                    InvoiceRepository.UpdateInvoiceOnReadyForBilling(updatedInvoice.Id, updatedInvoice.BillingCategoryId,
                                                                 updatedInvoice.BillingMemberId,
                                                                 updatedInvoice.BilledMemberId, updatedInvoice.BillingCode);
                    Logger.InfoFormat("Updated invoice '{0}' details successfully after Invoice status changed to Ready for billing by User '{1}'.", ConvertUtil.ConvertGuidToString(updatedInvoice.Id), userId);
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat("Exception occurred while updating Invoice '{0}' in PROC_UPDINV_ON_READYFORBILLING by User '{1}' Exception: '{2}'", ConvertUtil.ConvertGuidToString(updatedInvoice.Id), userId, ex);
                }

                //try
                //{
                //    MiscUatpInvoiceRepository.UpdateInvoiceAndSetLaParameters(new Guid(invoiceId));
                //    Logger.InfoFormat("Updated invoice '{0}' details for LA and successfully set LA Parameters by User '{1}'.", ConvertUtil.ConvertGuidToString(updatedInvoice.Id), userId);
                //}
                //catch (Exception ex)
                //{
                //    Logger.ErrorFormat("Exception occurred while updating Invoice '{0}' in PROC_SET_INV_ARCPARAMETERS by User '{1}' Exception: '{2}'", ConvertUtil.ConvertGuidToString(updatedInvoice.Id), userId, ex);
                //}

            }

            return updatedInvoice;
        }

        /// <summary>
        /// Get Dynamic field metadata for given combination of Charge code, Charge code type and billing category
        /// </summary>
        //CMP #636: Standard Update Mobilization
        public IList<FieldMetaData> GetFieldMetadata(int chargeCodeId, int? chargeCodeTypeId, Guid? lineItemDetailId, Int32 billingCategoryId)
        {
          return FieldMetaDataRepository.GetFieldMetadata(chargeCodeId, chargeCodeTypeId, lineItemDetailId, billingCategoryId);
        }

        /// <summary>
        /// Gets the field metadata for group.
        /// </summary>
        /// <param name="chargeCodeId">The charge code id.</param>
        /// <param name="chargeCodeTypeId">The charge code type id.</param>
        /// <param name="groupId">The group id.</param>
        /// <param name="isOptionalGroup"></param>
        /// <returns></returns>
        public FieldMetaData GetFieldMetadataForGroup(int chargeCodeId, int? chargeCodeTypeId, Guid groupId, bool isOptionalGroup)
        {
            return isOptionalGroup
                     ? FieldMetaDataRepository.GetOptionalFieldMetadataForGroup(chargeCodeId, chargeCodeTypeId, groupId)
                     : FieldMetaDataRepository.GetFieldMetadataForGroup(chargeCodeId, chargeCodeTypeId, groupId);
        }

        /// <summary>
        /// Get list of dictionary based values for field of type dropdown 
        /// </summary>
        /// <param name="dataSourceId"></param>
        /// <returns></returns>
        public IList<DropdownDataValue> GetDataSourceValues(int dataSourceId)
        {
            return DataSourceRepository.GetDataSourceValues(dataSourceId);
        }

        /// <summary>
        /// Gets the invoice detail.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns></returns>
        public MiscUatpInvoice GetInvoiceDetail(string invoiceId)
        {
            var invoiceGuid = invoiceId.ToGuid();

            // Replaced with LoadStrategy call
            var invoiceHeader = MiscUatpInvoiceRepository.Single(invoiceId: invoiceGuid);
            if (invoiceHeader.InvoiceStatus == InvoiceStatusType.ValidationError)
            {
                invoiceHeader.ValidationErrors = ValidationErrorManager.GetValidationErrors(invoiceId).ToList();
            }

            // reverse additional details starting from 2nd additional detail.
            if (invoiceHeader.AdditionalDetails != null && invoiceHeader.AdditionalDetails.Count > 2)
            {
                var listToReverse = invoiceHeader.AdditionalDetails.ToList();
                listToReverse.Reverse(1, invoiceHeader.AdditionalDetails.Count - 1);
                invoiceHeader.AdditionalDetails = listToReverse;
            }

            // DiplayText Retrieval from Table and setting it to required property - To be moved to sp
            invoiceHeader.InvoiceStatusDisplayText = ReferenceManager.GetInvoiceStatusDisplayValue(invoiceHeader.InvoiceStatusId);
            invoiceHeader.SettlementMethodDisplayText = ReferenceManager.GetSettlementMethodDisplayValue(invoiceHeader.SettlementMethodId);
            invoiceHeader.SubmissionMethodDisplayText = ReferenceManager.GetDisplayValue(MiscGroups.FileSubmissionMethod, invoiceHeader.SubmissionMethodId);

            return invoiceHeader;
        }

        public MiscUatpInvoice GetInvoiceHeader(string invoiceId)
        {
            var invoiceHeader = MiscUatpInvoiceRepository.GetLsInvoiceHeaderInformation(invoiceId.ToGuid());

            // DiplayText Retrieval from Table and setting it to required property - To be moved to sp
            invoiceHeader.InvoiceStatusDisplayText = ReferenceManager.GetInvoiceStatusDisplayValue(invoiceHeader.InvoiceStatusId);
            invoiceHeader.SettlementMethodDisplayText = ReferenceManager.GetSettlementMethodDisplayValue(invoiceHeader.SettlementMethodId);
            invoiceHeader.SubmissionMethodDisplayText = ReferenceManager.GetDisplayValue(MiscGroups.FileSubmissionMethod, invoiceHeader.SubmissionMethodId);

            return invoiceHeader;
        }

        /// <summary>
        /// Get Invoice Header only based on invoice Id
        /// ID : 325374 - File Loading & Web Response Stats -PayablesInvoiceSearch
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public MiscUatpInvoice GetInvoiceHeaderForManageScreen(string invoiceId)
        {
            try
            {
                Guid id = Guid.Parse(invoiceId);
                var invoice = MiscUatpInvoiceRepository.Get(i => i.Id == id).SingleOrDefault();
                return invoice;
            }
            catch (Exception ex)
            {
                Logger.Error("GetInvoiceHeaderForManageScreen", ex);
                throw ex;
            }
        }


        public bool IsRejectionInvoiceExist(string invoiceId)
        {
            var invoiceGuid = invoiceId.ToGuid();
            // Replaced with LoadStrategy call
            var invoiceHeader = MiscUatpInvoiceRepository.Single(invoiceGuid);

            var invoiceCount =
              MiscUatpInvoiceRepository.GetCount(
                invoice =>
                invoice.RejectedInvoiceNumber.ToUpper() == invoiceHeader.InvoiceNumber.ToUpper() && invoice.BillingMemberId == invoiceHeader.BilledMemberId && invoice.BilledMemberId == invoiceHeader.BillingMemberId &&
                invoiceHeader.BillingCategoryId == invoice.BillingCategoryId && invoice.InvoiceStatusId == (int)InvoiceStatusType.Presented);

            return invoiceCount > 0;
        }

        /// <summary>
        /// Determines whether RejectionInvoice exists with any Status
        /// </summary>
        /// <param name="invoiceId">InvoiceID</param>
        /// <returns>True if Rejection Invoice exists else false</returns>
        public bool IsRejectionInvoiceExistsWithAnyStatus(string invoiceId)
        {
            // Convert InvoiceId string to Guid
            var invoiceGuid = invoiceId.ToGuid();
            // Retrieve Invoice header
            var invoiceHeader = MiscUatpInvoiceRepository.Single(invoiceGuid);
            // Check whether RejectionInvoice exists with Presented status
          var invoiceCountWithPresentedStatus =
            MiscUatpInvoiceRepository.GetCount(
              invoice =>
              invoice.RejectedInvoiceNumber.ToUpper() == invoiceHeader.InvoiceNumber.ToUpper() &&
              invoice.BillingMemberId == invoiceHeader.BilledMemberId &&
              invoice.BilledMemberId == invoiceHeader.BillingMemberId &&
              invoiceHeader.BillingCategoryId == invoice.BillingCategoryId &&
              invoice.SettlementYear == invoiceHeader.BillingYear &&
              invoice.SettlementMonth == invoiceHeader.BillingMonth &&
              invoice.SettlementPeriod == invoiceHeader.BillingPeriod &&
              invoice.InvoiceStatusId == (int) InvoiceStatusType.Presented);
            // Check whether RejectionInvoice exists with status other than Presented
          var invoiceCountWithStatusOtherThanPresented =
            MiscUatpInvoiceRepository.GetCount(
              invoice =>
              invoice.RejectedInvoiceNumber.ToUpper() == invoiceHeader.InvoiceNumber.ToUpper() &&
              invoice.BillingMemberId == invoiceHeader.BilledMemberId &&
              invoice.BilledMemberId == invoiceHeader.BillingMemberId &&
              invoiceHeader.BillingCategoryId == invoice.BillingCategoryId &&
              invoice.SettlementYear == invoiceHeader.BillingYear &&
              invoice.SettlementMonth == invoiceHeader.BillingMonth &&
              invoice.SettlementPeriod == invoiceHeader.BillingPeriod &&
              invoice.InvoiceStatusId != (int) InvoiceStatusType.Presented &&
              invoice.InvoiceStatusId != (int) InvoiceStatusType.ErrorNonCorrectable);

            return (invoiceCountWithPresentedStatus > 0 || invoiceCountWithStatusOtherThanPresented > 0);
        }

        /// <summary>
        /// Gets the rejection error message to be displayed when user initiates rejection through Payables.
        /// </summary>
        /// <param name="invoiceId">Invoice id of invoice being rejected.</param>
        /// <param name="isCreditNoteRejection"></param>
        /// <returns>The error/warning message.</returns>
        public string GetRejectionErrorMessage(string invoiceId, out bool isCreditNoteRejection)
        {
            var invoiceGuid = invoiceId.ToGuid();
            // Replaced with LoadStrategy call
            var invoiceHeader = MiscUatpInvoiceRepository.Single(invoiceGuid);
            isCreditNoteRejection = false;

            //SCP0000:Impact on MISC/UATP rejection linking due to purging
            if (invoiceHeader != null)
            {
              DateTime purgeDate = new DateTime(1973, 01, 01);
              if (invoiceHeader.ExpiryDatePeriod != null && invoiceHeader.ExpiryDatePeriod == purgeDate)
              {
                return Messages.ResourceManager.GetString(MiscUatpErrorCodes.InvoicePurged);
              }
            }

            //SCPID : 117317 - question about same invoice No
            // Check for presented invoices first.
            //SCP251726: Two reject invoices for same original invoice number
            long invoiceCount =
              MiscUatpInvoiceRepository.GetCount(
                invoice =>
                invoice.RejectedInvoiceNumber.ToUpper() == invoiceHeader.InvoiceNumber.ToUpper() &&
                invoice.BillingMemberId == invoiceHeader.BilledMemberId &&
                invoice.BilledMemberId == invoiceHeader.BillingMemberId &&
                invoiceHeader.BillingCategoryId == invoice.BillingCategoryId &&
                invoiceHeader.BillingYear == invoice.SettlementYear&& 
                !(invoice.InvoiceStatusId == (int)InvoiceStatusType.ErrorNonCorrectable && invoice.ValidationStatusId == (int)InvoiceValidationStatus.Failed));
            if (invoiceCount > 0)
            {
                return Messages.ResourceManager.GetString(MiscUatpErrorCodes.InvoiceAlreadyRejectedForPayables);
            }

          //  // Check for invoices with other status.
          //invoiceCount =
          //  MiscUatpInvoiceRepository.GetCount(
          //    invoice =>
          //    invoice.RejectedInvoiceNumber.ToUpper() == invoiceHeader.InvoiceNumber.ToUpper() &&
          //    invoice.BillingMemberId == invoiceHeader.BilledMemberId &&
          //    invoice.BilledMemberId == invoiceHeader.BillingMemberId &&
          //    invoice.SettlementYear == invoiceHeader.BillingYear &&
          //    invoice.SettlementMonth == invoiceHeader.BillingMonth &&
          //    invoice.SettlementPeriod == invoiceHeader.BillingPeriod &&
          //    invoiceHeader.BillingCategoryId == invoice.BillingCategoryId);

          //  if (invoiceCount > 0)
          //  {
          //      return Messages.ResourceManager.GetString(MiscUatpErrorCodes.InvoiceRejectedForPayablesAndStatusIsPresented);
          //  }

            // Check for credit note after previous validations have passed.
            if (invoiceHeader.InvoiceType == InvoiceType.CreditNote)
            {
                isCreditNoteRejection = true;
                return Messages.ResourceManager.GetString(MiscUatpErrorCodes.CreditNoteRejectionMessage);
            }

            #region CMP #678: Time Limit Validation on Last Stage MISC Rejections
            /*Payable Screen*/
            var rmInTimeLimtMsg = ValidateMiscLastStageRmForTimeLimit(invoiceHeader,
                                                                     null,
                                                                     RmValidationType.IsWebPayableInvoice);
            if (!string.IsNullOrWhiteSpace(rmInTimeLimtMsg))
            {
                return rmInTimeLimtMsg;
            }
            #endregion
            
            return null;
        }

        /// <summary>
        /// Adds the line item.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <returns></returns>
        public LineItem AddLineItem(LineItem lineItem)
        {
          //SCP99417:Is wwb performace.
          
          // Replaced with LoadStrategy call
          //var miscUatpInvoice = MiscUatpInvoiceRepository.Single(invoiceId: lineItem.InvoiceId);
          //Rertieve  the invoice details using get method of repository
         var miscUatpInvoice = MiscUatpInvoiceRepository.Get(i => i.Id == lineItem.InvoiceId).SingleOrDefault();

            // Validate LineItem before creating.
            ValidateLineItem(lineItem, null);

            //CMP#502 : Rejection Reason for MISC Invoices
            if (miscUatpInvoice!= null && miscUatpInvoice.BillingCategory == BillingCategoryType.Misc)
            {
                ValidateRejectionReasonCode(lineItem, null, string.Empty, miscUatpInvoice, DateTime.UtcNow, true);
            }

            //Assign lineitemNumber again to fix issue of LineItemNumber saved as 0
            if (lineItem.LineItemNumber == 0)
                lineItem.LineItemNumber = LineItemRepository.GetMaxLineItemNumber(lineItem.InvoiceId) + 1;

            if (miscUatpInvoice.InvoiceStatus != InvoiceStatusType.Open)
            {
                miscUatpInvoice.InvoiceStatus = InvoiceStatusType.Open;
                miscUatpInvoice.ValidationStatus = InvoiceValidationStatus.Pending;
                miscUatpInvoice.ValidationDate = DateTime.MinValue;
                MiscUatpInvoiceRepository.Update(miscUatpInvoice);
            }

            LineItemRepository.Add(lineItem);
            UnitOfWork.CommitDefault();

            MiscUatpInvoiceRepository.UpdateInvoiceTotal(lineItem.InvoiceId, lineItem.Id, 1);

            return lineItem;
        }

        /// <summary>
        /// Updates the line item.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <returns></returns>
        public LineItem UpdateLineItem(LineItem lineItem)
        {
            // Replaced with LoadStrategy call
            var lineItemInDb = LineItemRepository.Single(lineItemId: lineItem.Id);

            // Validate LineItem before creating.
            ValidateLineItem(lineItem, lineItemInDb);
            //CMP#502 : Rejection Reason for MISC Invoices
            if (lineItemInDb.Invoice != null && lineItemInDb.Invoice.BillingCategory == BillingCategoryType.Misc)
            {
                ValidateRejectionReasonCode(lineItem, null, string.Empty, lineItemInDb.Invoice, DateTime.UtcNow, true);
            }
            // Changes to update tax breakdown records
            UpdateTaxBreakdown(lineItem, lineItemInDb);

            // Changes to update tax breakdown records
            UpdateVatBreakdown(lineItem, lineItemInDb);

            // Changes to update add-on change records
            UpdateAddOnCharge(lineItem, lineItemInDb);

            // Additional detail update in edit mode.
            UpdateLineItemAdditionalDetail(lineItem, lineItemInDb);

            // Validation for Line Item service date range is out side of date range given for Line Item Details.
            // If (CompareUtil.IsDirty(lineItem.StartDate, lineItemInDb.StartDate)
            //  || CompareUtil.IsDirty(lineItem.EndDate, lineItemInDb.EndDate))
            //{
            //  if (!IsValidServiceDate(lineItem))
            //  {
            //    throw new ISBusinessException(MiscUatpErrorCodes.InvalidServiceDateRange);
            //  }
            //}

            // Validations for service date at line item and line item detail level.
            ValidateServiceDate(lineItem, lineItemInDb);

            if (lineItem.Invoice.InvoiceStatus != InvoiceStatusType.Open)
            {
                lineItem.Invoice.InvoiceStatus = InvoiceStatusType.Open;
                lineItem.Invoice.ValidationStatus = InvoiceValidationStatus.Pending;
                lineItem.Invoice.ValidationDate = DateTime.MinValue;
                MiscUatpInvoiceRepository.Update(lineItem.Invoice);
            }

            //Assign lineitemNumber again to fix issue of LineItemNumber saved as 0
            if (lineItem.LineItemNumber == 0)
                lineItem.LineItemNumber = lineItemInDb.LineItemNumber;

            var updatedLineItem = LineItemRepository.Update(lineItem);
            UnitOfWork.CommitDefault();

            // Update invoice total.
            MiscUatpInvoiceRepository.UpdateInvoiceTotal(lineItem.InvoiceId, lineItem.Id, 1);

            return updatedLineItem;
        }

        /// <summary>
        /// Validates the service date.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <param name="lineItemInDb">The line item in db.</param>
        /// <returns></returns>
        private bool ValidateServiceDate(LineItem lineItem, LineItem lineItemInDb)
        {
            // Get line item detail for given line item.
            var lineItemDetails = LineItemDetailRepository.Get(lineItemId: lineItem.Id);

            var hasStartDate = lineItemDetails.Any(lineitemDetail => lineitemDetail.StartDate != null);

            if (lineItem.StartDate != null && (CompareUtil.IsDirty(lineItem.StartDate, lineItemInDb.StartDate) || CompareUtil.IsDirty(lineItem.EndDate, lineItemInDb.EndDate)))
            {
                // 1. Date range specified in line item level and user updates line item date range. 
                // If (hasStartDate && !lineItemDetails.All(lineItemDetail =>lineItem.StartDate <= lineItemDetail.StartDate && lineItem.EndDate >= lineItemDetail.EndDate))
                if ((hasStartDate && lineItemDetails.Any(lineItemDetail => lineItem.StartDate > lineItemDetail.StartDate)) || lineItemDetails.Any(lineItemDetail => lineItem.EndDate < lineItemDetail.EndDate))
                {
                    throw new ISBusinessException(MiscUatpErrorCodes.InvalidServiceDateRange);
                }
            }

            // 2. Only end date is specified in line item (and line item detail exists for line item) then update end date of line item details.
            if (lineItem.StartDate == null && lineItemDetails.Count() > 0)
            {
                // 3. Do not allow user to delete start date if, start date exist for any of line item detail.

                if (hasStartDate)
                {
                    throw new ISBusinessException(MiscUatpErrorCodes.InvalidStartDateForLineItemDetailWithStartDate);
                }

                // 4. if user removes start date from line item detail while lineItem details exists for it, then allow if all line item detail having same End Date.
                var isEndDateSame = lineItemDetails.GroupBy(lineDetail => lineDetail.EndDate).Count() == 1;

                if (!isEndDateSame)
                {
                    throw new ISBusinessException(MiscUatpErrorCodes.InvalidStartDateForLineItemDetailEndDateNotSame);
                }

                // Update line item detail service end date with line item service start date. Stored procedure call.
                LineItemDetailRepository.UpdateLineItemDetailEndDate(lineItem.Id, lineItem.EndDate);
            }

            return true;
        }

        /// <summary>
        /// Updates the add on charge.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <param name="lineItemInDb">The line item in db.</param>
        private void UpdateAddOnCharge(LineItem lineItem, LineItem lineItemInDb)
        {
            if (lineItem.AddOnCharges != null)
            {
                var listToDeleteAddOnCharges = lineItemInDb.AddOnCharges.Where(addOnCharge => lineItem.AddOnCharges.Count(addOnChargeRecord => addOnChargeRecord.Id == addOnCharge.Id) == 0).ToList();

                foreach (var addOnCharge in lineItem.AddOnCharges.Where(addOnCharge => addOnCharge.Id.CompareTo(new Guid()) == 0))
                {
                    addOnCharge.ParentId = lineItem.Id;
                    LineItemAddOnChargeRepository.Add(addOnCharge);
                }

                foreach (var lineItemAddOnCharge in listToDeleteAddOnCharges)
                {
                    LineItemAddOnChargeRepository.Delete(lineItemAddOnCharge);
                }
            }
        }

        /// <summary>
        /// Updates the vat breakdown.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <param name="lineItemInDb">The line item in db.</param>
        private void UpdateVatBreakdown(LineItem lineItem, LineItem lineItemInDb)
        {
            if (lineItem.TaxBreakdown != null)
            {
                var listToDeleteVat = lineItemInDb.TaxBreakdown.Where(vat => lineItem.TaxBreakdown.Count(vatRecord => vatRecord.Id == vat.Id) == 0 && vat.Type.ToUpper() == "VAT").ToList();

                foreach (var vat in lineItem.TaxBreakdown.Where(vat => vat.Id.CompareTo(new Guid()) == 0 && vat.Type.ToUpper() == "VAT"))
                {
                    vat.ParentId = lineItem.Id;
                    LineItemTaxRepository.Add(vat);
                }

                foreach (var lineItemVat in listToDeleteVat)
                {
                    LineItemTaxRepository.Delete(lineItemVat);
                }
            }
        }

        /// <summary>
        /// Updates the tax breakdown.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <param name="lineItemInDb">The line item in db.</param>
        private void UpdateTaxBreakdown(LineItem lineItem, LineItem lineItemInDb)
        {
            if (lineItem.TaxBreakdown != null)
            {
                var listToDeleteTax = lineItemInDb.TaxBreakdown.Where(tax => lineItem.TaxBreakdown.Count(taxRecord => taxRecord.Id == tax.Id) == 0 && tax.Type.ToUpper() == "TAX").ToList();

                foreach (var tax in lineItem.TaxBreakdown.Where(tax => tax.Id.CompareTo(new Guid()) == 0 && tax.Type.ToUpper() == "TAX"))
                {
                    tax.ParentId = lineItem.Id;
                    LineItemTaxRepository.Add(tax);
                }

                foreach (var lineItemTax in listToDeleteTax)
                {
                    LineItemTaxRepository.Delete(lineItemTax);
                }
            }
        }

        /// <summary>
        /// Updates the line item additional detail.
        /// </summary>
        /// <param name="invoice">The invoice.</param>
        /// <param name="invoiceInDb">The invoice in db.</param>
        private void UpdateInvoiceAddionalDetail(MiscUatpInvoice invoice, MiscUatpInvoice invoiceInDb)
        {
            var listToDeleteAdditionalDetail = invoiceInDb.AdditionalDetails.Where(additionalDetail => invoice.AdditionalDetails.Count(detail => detail.Id == additionalDetail.Id) == 0).ToList();

            foreach (var additionalDetail in invoice.AdditionalDetails.Where(additionalDetail => additionalDetail.Id.CompareTo(new Guid()) == 0))
            {
                additionalDetail.InvoiceId = invoice.Id;
                MiscUatpInvoiceAdditionalDetailRepository.Add(additionalDetail);
            }

            foreach (var additionalDetail in listToDeleteAdditionalDetail)
            {
                MiscUatpInvoiceAdditionalDetailRepository.Delete(additionalDetail);
            }
        }

        /// <summary>
        /// Updates the line item additional detail.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <param name="lineItemInDb">The line item in db.</param>
        private void UpdateLineItemAdditionalDetail(LineItem lineItem, LineItem lineItemInDb)
        {
            var listToDeleteAdditionalDetail = lineItemInDb.LineItemAdditionalDetails.Where(additionalDetail => lineItem.LineItemAdditionalDetails.Count(lineItemAdditionalDetail => lineItemAdditionalDetail.Id == additionalDetail.Id) == 0).ToList();

            foreach (var addtionalDetail in lineItem.LineItemAdditionalDetails.Where(additionalDetail => additionalDetail.Id.CompareTo(new Guid()) == 0))
            {
                addtionalDetail.LineItemId = lineItem.Id;
                LineItemAdditionalDetailRepository.Add(addtionalDetail);
            }

            foreach (var additionalDetail in listToDeleteAdditionalDetail)
            {
                LineItemAdditionalDetailRepository.Delete(additionalDetail);
            }
        }

        /// <summary>
        /// Updates the line item detail additional detail.
        /// </summary>
        /// <param name="lineItemDetail">The line item detail.</param>
        /// <param name="lineItemDetailInDb">The line item detail in db.</param>
        private void UpdateLineItemDetailAdditionalDetail(LineItemDetail lineItemDetail, LineItemDetail lineItemDetailInDb)
        {
            var listToDeleteAdditionalDetail = lineItemDetailInDb.LineItemDetailAdditionalDetails.Where(additionalDetail => lineItemDetail.LineItemDetailAdditionalDetails.Count(lineItemDetailAdditionalDetail => lineItemDetailAdditionalDetail.Id == additionalDetail.Id) == 0).ToList();

            foreach (var addtionalDetail in lineItemDetail.LineItemDetailAdditionalDetails.Where(additionalDetail => additionalDetail.Id.CompareTo(new Guid()) == 0))
            {
                addtionalDetail.LineItemDetailId = lineItemDetail.Id;
                LineItemDetailAdditionalDetailRepository.Add(addtionalDetail);
            }

            foreach (var additionalDetail in listToDeleteAdditionalDetail)
            {
                LineItemDetailAdditionalDetailRepository.Delete(additionalDetail);
            }
        }

        /// <summary>
        /// Line Item service date range is out side of date range given for Line Item Details.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <returns>
        /// true if valid service date for specified line item; otherwise, false.
        /// </returns>
        private bool IsValidServiceDate(LineItem lineItem)
        {
            var lineItemDetails = LineItemDetailRepository.Get(lineItemId: lineItem.Id);
            return lineItemDetails.Count() <= 0 || lineItemDetails.All(lineItemDetail => lineItem.StartDate <= lineItemDetail.StartDate && lineItem.EndDate >= lineItemDetail.EndDate);
        }

        /// <summary>
        /// Line Item service date range is out side of date range given for Line Item Details.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <returns>
        /// true if valid service date for specified line item; otherwise, false.
        /// </returns>
        private bool IsValidParsedServiceDate(LineItem lineItem)
        {
            var lineItemDetails = lineItem.LineItemDetails;
            if (lineItem.StartDate != null) //when start-end date at line item are given
            {
                foreach (var lineItemDetail in lineItemDetails)
                {
                    if (lineItemDetail.StartDate == null)
                    {
                        if (lineItem.EndDate < lineItemDetail.EndDate)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (lineItem.StartDate > lineItemDetail.StartDate || lineItem.EndDate < lineItemDetail.EndDate)
                        {
                            return false;
                        }
                    }
                }
            }
            else if (lineItem.StartDate == null) //when start date is not given only end date is given
            {
                var nLineItemDetailWithoutStartDate = lineItemDetails.Count(rec => rec.StartDate == null && rec.EndDate == lineItem.EndDate);
                if (nLineItemDetailWithoutStartDate != lineItemDetails.Count)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the line item information.
        /// </summary>
        /// <param name="lineItemId">The line item id.</param>
        /// <returns></returns>
        public LineItem GetLineItemInformation(string lineItemId)
        {
            var lineItemGuid = lineItemId.ToGuid();

            // Replaced with LoadStrategy implementation call
            var lineItemObj = LineItemRepository.Single(lineItemId: lineItemGuid);
            if (lineItemObj.LineItemDetails.Count > 0)
            {
                var navigationList = MiscUatpInvoiceRepository.GetLineItemDetailNavigation(new Guid(), lineItemObj.Id, 1);

                if (navigationList != null && navigationList[0] != null) lineItemObj.NavigationDetails = navigationList[0];
            }

            return lineItemObj;
        }

        public LineItem GetLineItemHeaderInformation(string lineItemId)
        {
            var lineItemGuid = lineItemId.ToGuid();
            var lineItemObj = LineItemRepository.GetLineItemHeaderInformation(lineItemGuid);

            var uomCodeTypeList = MiscCodeRepository.GetMiscCodes(MiscGroups.UomCodeType);
            var uomCodeTypeBase = uomCodeTypeList.Single(code => code.Description.ToLower().Contains("base"));
            int uomCodeTypeBaseName = Convert.ToInt32(uomCodeTypeBase.Name);
            lineItemObj.UomCode = UomCodeRepository.Single(code => code.Id == lineItemObj.UomCodeId && code.Type == uomCodeTypeBaseName);


            if (LineItemDetailRepository.GetCount(ld => ld.LineItemId == lineItemGuid) > 0)
            {
                var navigationList = MiscUatpInvoiceRepository.GetLineItemDetailNavigation(new Guid(), lineItemObj.Id, 1);

                if (navigationList != null && navigationList[0] != null) { lineItemObj.NavigationDetails = navigationList[0]; }
            }
            return lineItemObj;
        }

        /// <summary>
        /// Gets the line item list.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns></returns>
        public IList<LineItem> GetLineItemList(string invoiceId)
        {
            var invoiceGuid = invoiceId.ToGuid();
            return LineItemRepository.Get(invoiceGuid).OrderBy(lineitem => lineitem.LineItemNumber).ToList();
            // Return LineItemRepository.Get(lineItem => lineItem.InvoiceId == invoiceGuid).OrderBy(lineitem => lineitem.LineItemNumber).ToList();
        }

        /// <summary>
        /// Deletes the line item.
        /// </summary>
        /// <param name="lineItemId">The line item id.</param>
        /// <returns></returns>
        public bool DeleteLineItem(string lineItemId)
        {
            MiscUatpInvoiceRepository.DeleteLineItem(lineItemId.ToGuid());

            return true;
        }

        /// <summary>
        /// Adds the line item detail.
        /// </summary>
        /// <param name="lineItemDetail">The line item detail.</param>
        /// <param name="fieldMetaData">The field metadata.</param>
        /// <returns></returns>
        public LineItemDetail AddLineItemDetail(LineItemDetail lineItemDetail, IList<FieldMetaData> fieldMetaData)
        {


            // Replaced with LoadStrategy implementation call
            var lineItem = LineItemRepository.Single(lineItemId: lineItemDetail.LineItemId);
            var miscUatpInvoice = MiscUatpInvoiceRepository.Single(invoiceId: lineItem.InvoiceId);
            //Assign LineItemNumber to LineItemNumber property of LineItemDetail, this property is used in output generation
            lineItemDetail.LineItemNumber = lineItem.LineItemNumber;

            if (miscUatpInvoice.InvoiceStatus != InvoiceStatusType.Open)
            {
                miscUatpInvoice.InvoiceStatus = InvoiceStatusType.Open;
                miscUatpInvoice.ValidationStatus = InvoiceValidationStatus.Pending;
                miscUatpInvoice.ValidationDate = DateTime.MinValue;
                MiscUatpInvoiceRepository.Update(miscUatpInvoice);
            }

            // Update service start and end date of line item, if line item details service start and end date is not in range.
            UpdateLineItemServiceDate(lineItem, lineItemDetail);
            //   var fieldMetaData = GetFieldMetadata(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, lineItemDetail.Id);
      
      //SCPID 109752 - Issue in incoming MISC Invoice from March4P to Iberia
      //Prevent following code getting executed simultaneously to avoid same LineItemdetail number.
      lock (lineItemDetail)
      {
          ValidateLineItemDetail(lineItemDetail, null);
          LineItemDetailRepository.Add(lineItemDetail);
          lineItemDetail.DynamicFieldsSummary = GetDynamicFieldSummary(lineItemDetail);

          ValidateDynamicFields(lineItemDetail, lineItem, fieldMetaData);

          UnitOfWork.CommitDefault();
      }


            MiscUatpInvoiceRepository.UpdateInvoiceTotal(lineItem.InvoiceId, lineItem.Id, 1);

            // Refresh lineItem object in Context as it will be updated by above Stored Procedure call in database.
            LineItemRepository.Refresh(lineItem);

            return lineItemDetail;
        }

        private string GetDynamicFieldSummary(LineItemDetail lineItemDetail)
        {
            int itemCount = 0;
            var dynamicFieldSummary = new StringBuilder();
            foreach (var fieldValue in lineItemDetail.FieldValues)
            {
                itemCount = GetItemCount(fieldValue, dynamicFieldSummary, ref itemCount);

                if (itemCount == MaxDynamicFieldCount) break;
            }

            if (dynamicFieldSummary.Length > 0)
            {
                // To remove the ', ' at the end.
                dynamicFieldSummary = dynamicFieldSummary.Remove(dynamicFieldSummary.Length - 2, 2);
                if (dynamicFieldSummary.Length > 500)
                {
                    // If length exceeds 500 characters remove the extra characters + more 3 end characters
                    // and append '...'
                    dynamicFieldSummary = dynamicFieldSummary.Remove(500 - 3, dynamicFieldSummary.Length - 500 + 3);
                    dynamicFieldSummary.Append("...");
                }
            }

            return dynamicFieldSummary.ToString();
        }

        private int GetItemCount(FieldValue fieldValue, StringBuilder dynamicFieldSummary, ref int itemCount)
        {
            foreach (var attributeValue in fieldValue.AttributeValues)
            {
                // Recursively iterate through attributes.
                GetItemCount(attributeValue, dynamicFieldSummary, ref itemCount);
                if (string.IsNullOrEmpty(attributeValue.Value))
                {
                    continue;
                }

                if (attributeValue.FieldMetaData != null)
                    dynamicFieldSummary.AppendFormat("{0}: {1}, ", attributeValue.FieldMetaData.DisplayText, attributeValue.Value);
                else
                {
                    var fieldMetadata = FieldMetaDataRepository.Single(fieldMetaData => fieldMetaData.Id == attributeValue.FieldMetaDataId);
                    dynamicFieldSummary.AppendFormat("{0}: {1}, ", fieldMetadata.DisplayText, attributeValue.Value);
                }

                itemCount++;
                if (itemCount == MaxDynamicFieldCount) return itemCount;
            }
            return itemCount;
        }

        private void ValidateDynamicFields(LineItemDetail lineItemDetail, LineItem lineItem, IList<FieldMetaData> fieldMetaData)
        {
            IList<DynamicValidationError> dynamicValidationErrors;
            PerformDynamicFieldValidation(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, fieldMetaData, out dynamicValidationErrors);

            if (dynamicValidationErrors.Count > 0)
            {
                var errors = new StringBuilder();
                foreach (var dynamicValidationError in dynamicValidationErrors.Where(error => error != null))
                {
                    errors.Append(dynamicValidationError.ErrorCode + ErrorCodeSaparator + dynamicValidationError.ErrorDescription).Append(Environment.NewLine);
                }
                // remove newly added line item detail from line item.
                lineItem.LineItemDetails.Remove(lineItemDetail);
                throw new ISBusinessException(errors.ToString());
            }
        }

        /// <summary>
        /// Updates the line item detail.
        /// </summary>
        /// <param name="lineItemDetail">The line item detail.</param>
        /// <param name="fieldMetaData">Field metadata.</param>
        /// <returns></returns>
        public LineItemDetail UpdateLineItemDetail(LineItemDetail lineItemDetail, IList<FieldMetaData> fieldMetaData)
        {
            var lineItemDetailInDb = LineItemDetailRepository.Single(lineItemDetailId: lineItemDetail.Id);

            ValidateLineItemDetail(lineItemDetail, lineItemDetailInDb);

            //Assign LineItemNumber to LineItemNumber property of LineItemDetail, this property is used in output generation
            if (lineItemDetail.LineItemNumber == 0)
                lineItemDetail.LineItemNumber = lineItemDetailInDb.LineItemNumber;

            var fieldValueList = new List<FieldValue>(lineItemDetailInDb.FieldValues);
            foreach (var fieldValue in fieldValueList)
            {
                FieldValueRepository.Delete(fieldValue);
            }

            foreach (var fieldValue in lineItemDetail.FieldValues)
            {
                FieldValueRepository.Add(fieldValue);
            }

            // Changes to update tax breakdown records.
            UpdateTaxBreakdown(lineItemDetail, lineItemDetailInDb);

            // Changes to update tax breakdown records.
            UpdateVatBreakdown(lineItemDetail, lineItemDetailInDb);

            // Changes to update add-on charge records.
            UpdateAddOnCharge(lineItemDetail, lineItemDetailInDb);

            // Update LineItemDetail additional details.
            UpdateLineItemDetailAdditionalDetail(lineItemDetail, lineItemDetailInDb);

            // Replaced with LoadStrategy implementation call
            var lineItem = LineItemRepository.Single(lineItemId: lineItemDetail.LineItemId);
            var miscUatpInvoice = MiscUatpInvoiceRepository.Single(invoiceId: lineItem.InvoiceId);
            //   var fieldMetaData = GetFieldMetadata(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, lineItemDetail.Id);

            ValidateDynamicFields(lineItemDetail, lineItem, fieldMetaData);

            // Update invoice status as data updated for invoice.
            if (miscUatpInvoice.InvoiceStatus != InvoiceStatusType.Open)
            {
                miscUatpInvoice.InvoiceStatus = InvoiceStatusType.Open;
                miscUatpInvoice.ValidationStatus = InvoiceValidationStatus.Pending;
                miscUatpInvoice.ValidationDate = DateTime.MinValue;
                MiscUatpInvoiceRepository.Update(miscUatpInvoice);
            }

            // Update service start and end date of line item, if line item details service start and end date is not in range.
            UpdateLineItemServiceDate(lineItem, lineItemDetail);

            lineItemDetail.DynamicFieldsSummary = GetDynamicFieldSummary(lineItemDetail);
            var updatedLineItem = LineItemDetailRepository.Update(lineItemDetail);

            UnitOfWork.CommitDefault();

            MiscUatpInvoiceRepository.UpdateInvoiceTotal(lineItem.InvoiceId, lineItem.Id, 1);

            // Refresh lineItem object in Context as it will be updated by above Stored Procedure call in database.
            LineItemRepository.Refresh(lineItem);

            return updatedLineItem;
        }

        /// <summary>
        /// Updates the add on charge.
        /// </summary>
        /// <param name="lineItemDetail">The line item detail.</param>
        /// <param name="lineItemDetailInDb">The line item detail in db.</param>
        private void UpdateAddOnCharge(LineItemDetail lineItemDetail, LineItemDetail lineItemDetailInDb)
        {
            var listToDeleteAddOnCharges = lineItemDetailInDb.AddOnCharges.Where(lineItemDetailAddOnCharge => lineItemDetail.AddOnCharges.Count(addOnChargeRecord => addOnChargeRecord.Id == lineItemDetailAddOnCharge.Id) == 0).ToList();

            foreach (var addOnCharge in lineItemDetail.AddOnCharges.Where(addOnCharge => addOnCharge.Id.CompareTo(new Guid()) == 0))
            {
                addOnCharge.ParentId = lineItemDetail.Id;
                LineItemDetailAddOnChargeRepository.Add(addOnCharge);
            }

            foreach (var lineItemDetailAddOnCharge in listToDeleteAddOnCharges)
            {
                LineItemDetailAddOnChargeRepository.Delete(lineItemDetailAddOnCharge);
            }
        }

        /// <summary>
        /// Updates the vat breakdown.
        /// </summary>
        /// <param name="lineItemDetail">The line item detail.</param>
        /// <param name="lineItemDetailInDb">The line item detail in db.</param>
        private void UpdateVatBreakdown(LineItemDetail lineItemDetail, LineItemDetail lineItemDetailInDb)
        {
            var listToDeleteVat = lineItemDetailInDb.TaxBreakdown.Where(vat => lineItemDetail.TaxBreakdown.Count(vatRecord => vatRecord.Id == vat.Id) == 0 && vat.Type.ToUpper() == "VAT").ToList();

            foreach (var vat in lineItemDetail.TaxBreakdown.Where(vat => vat.Id.CompareTo(new Guid()) == 0 && vat.Type.ToUpper() == "VAT"))
            {
                vat.ParentId = lineItemDetail.Id;
                LineItemDetailTaxRepository.Add(vat);
            }

            foreach (var lineItemDetailVat in listToDeleteVat)
            {
                LineItemDetailTaxRepository.Delete(lineItemDetailVat);
            }
        }

        /// <summary>
        /// Updates the tax breakdown.
        /// </summary>
        /// <param name="lineItemDetail">The line item detail.</param>
        /// <param name="lineItemDetailInDb">The line item detail in db.</param>
        private void UpdateTaxBreakdown(LineItemDetail lineItemDetail, LineItemDetail lineItemDetailInDb)
        {
            var listToDeleteTax = lineItemDetailInDb.TaxBreakdown.Where(tax => lineItemDetail.TaxBreakdown.Count(taxRecord => taxRecord.Id == tax.Id) == 0 && tax.Type.ToUpper() == "TAX").ToList();

            foreach (var tax in lineItemDetail.TaxBreakdown.Where(tax => tax.Id.CompareTo(new Guid()) == 0 && tax.Type.ToUpper() == "TAX"))
            {
                tax.ParentId = lineItemDetail.Id;
                LineItemDetailTaxRepository.Add(tax);
            }

            foreach (var lineItemDetailTax in listToDeleteTax)
            {
                LineItemDetailTaxRepository.Delete(lineItemDetailTax);
            }
        }

        /// <summary>
        /// Gets the line item detail information.
        /// </summary>
        /// <param name="lineItemDetailId">The line item detail id.</param>
        /// <returns></returns>
        public LineItemDetail GetLineItemDetailInformation(string lineItemDetailId)
        {
            var lineItemDetailGuid = lineItemDetailId.ToGuid();
            var lineItemDetailObj = LineItemDetailRepository.Single(lineItemDetailId: lineItemDetailGuid);
            var navigationList = MiscUatpInvoiceRepository.GetLineItemDetailNavigation(lineItemDetailObj.Id, lineItemDetailObj.LineItemId, 0);

            if (navigationList != null && navigationList[0] != null)
                lineItemDetailObj.NavigationDetails = navigationList[0];

            return lineItemDetailObj;
        }

        public LineItemDetail GetLineItemDetailHeaderInformation(Guid lineItemId, int detailNumber)
        {
            return LineItemDetailRepository.GetLineItemDetailHeaderInformation(ld => ld.DetailNumber == detailNumber && ld.LineItemId == lineItemId);
        }

        public NavigationDetails GetNavigationDetails(string lineItemDetailId, string lineItemId)
        {
            var lineItemDetailGuid = lineItemDetailId.ToGuid();
            var lineItemGuid = lineItemId.ToGuid();

            var navigationList = MiscUatpInvoiceRepository.GetLineItemDetailNavigation(lineItemDetailGuid, lineItemGuid, 0);

            //SCP239761 - Issue with ISXML - JAN P2 (20140102)
            if (navigationList != null && navigationList.Count>0 && navigationList[0] != null)
                return navigationList[0];

            return null;
        }

        /// <summary>
        /// Deletes the line item detail.
        /// </summary>
        /// <param name="lineItemDetailId">The line item detail id.</param>
        /// <returns></returns>
        public bool DeleteLineItemDetail(string lineItemDetailId)
        {
            MiscUatpInvoiceRepository.DeleteLineItemDetail(lineItemDetailId.ToGuid());

            return true;
        }

        /// <summary>
        /// Gets the line item detail list.
        /// </summary>
        /// <param name="lineItemId">The line item id.</param>
        /// <returns></returns>
        public IList<LineItemDetail> GetLineItemDetailList(string lineItemId)
        {
            var lineItemGuid = lineItemId.ToGuid();
            // Return LineItemDetailRepository.Get(lineItemDetail => lineItemDetail.LineItemId == lineItemGuid).OrderBy(lineItemDetail => lineItemDetail.DetailNumber).ToList();
            return LineItemDetailRepository.Get(lineItemId: lineItemGuid).OrderBy(lineItemDetail => lineItemDetail.DetailNumber).ToList();
        }

        // TODO: Following 2 methods can be moved to one common manager used in InvoiceManager also.
        /// <summary>
        /// Gets the member reference data.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="isBillingMember">if set to true it is billing member</param>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        public MemberLocationInformation GetMemberReferenceData(string invoiceId, bool isBillingMember, string locationCode)
        {
            var invoiceGuid = invoiceId.ToGuid();
            MemberLocationInformation memberLocationInformation = null;
            // Replaced with LoadStrategy call
            var invoice = MiscUatpInvoiceRepository.Single(invoiceId: invoiceGuid);

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
                    memberLocationInformation = new MemberLocationInformation
                    {
                        CompanyRegistrationId = memberLocation.RegistrationId,
                        AddressLine1 = memberLocation.AddressLine1,
                        AddressLine2 = memberLocation.AddressLine2,
                        AddressLine3 = memberLocation.AddressLine3,
                        CityName = memberLocation.CityName,
                        CountryCode = memberLocation.CountryId,
                        CountryName = memberLocation.Country == null ? string.Empty : memberLocation.Country.Name,
                        DigitalSignatureRequiredId = 1,
                        SubdivisionName = memberLocation.SubDivisionName,
                        SubdivisionCode = memberLocation.SubDivisionCode,
                        LegalText = memberLocation.LegalText,
                        OrganizationName = memberLocation.MemberLegalName,
                        PostalCode = memberLocation.PostalCode,
                        TaxRegistrationId = memberLocation.TaxVatRegistrationNumber,
                        MemberLocationCode = memberLocation.LocationCode,
                        IsBillingMember = isBillingMember,
                        InvoiceId = invoiceGuid
                    };
                }
            }

            return memberLocationInformation;
        }

        /// <summary>
        /// Updates the member location information.
        /// </summary>
        /// <param name="memberLocationInformation">The member location information.</param>
        /// <param name="isBillingMember">if set to <c>true</c> [is billing member].</param>
        /// <param name="invoice"></param>
        /// <returns></returns>
        public MemberLocationInformation UpdateMemberLocationInformation(MemberLocationInformation memberLocationInformation, bool isBillingMember, MiscUatpInvoice invoice, bool? commitChanges = null)
        {
            if (!string.IsNullOrEmpty(memberLocationInformation.MemberLocationCode))
            {
                if (isBillingMember)
                {
                    invoice.BillingMemberLocationCode = memberLocationInformation.MemberLocationCode;
                }
                else
                {
                    invoice.BilledMemberLocationCode = memberLocationInformation.MemberLocationCode;
                }

                //MiscUatpInvoiceRepository.Update(invoice);

                //var memberLocInformation = MemberLocationInfoRepository.Single(memLoc => memLoc.InvoiceId == invoice.Id && memLoc.IsBillingMember == isBillingMember);

                //if (memberLocInformation != null)
                //  MemberLocationInfoRepository.Delete(memberLocInformation);
            }
            else
            {
                if (isBillingMember)
                {
                    invoice.BillingMemberLocationCode = null;
                }
                else
                {
                    invoice.BilledMemberLocationCode = null;
                }
            }

            MiscUatpInvoiceRepository.Update(invoice);

            var memberLocationInformationInDb = MemberLocationInfoRepository.Single(memLoc => memLoc.InvoiceId == invoice.Id && memLoc.IsBillingMember == isBillingMember);

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
        /// Common validations for Misc./UATP invoice.
        /// </summary>
        /// <param name="invoiceHeader">The invoice header.</param>
        /// <param name="invoiceHeaderInDb">The invoice header in db.</param>
        /// <exception cref="ISBusinessException"></exception>
        /// <returns></returns>
        protected virtual bool ValidateInvoiceHeader(MiscUatpInvoice invoiceHeader, MiscUatpInvoice invoiceHeaderInDb)
        {
            var isUpdateOperation = false;

            // Check whether it's a update operation.
            if (invoiceHeaderInDb != null)
            {
                isUpdateOperation = true;
            }

            // var memcon = new Dictionary<int, string>(ReferenceManager.GetMiscCode(MiscGroups.ContactType));
            var misccontact = MiscCodeRepository.GetMiscCodes(MiscGroups.ContactType);
            var miscCodesList = misccontact.ToDictionary(miscCode => Convert.ToInt32(miscCode.Name), miscCode => miscCode.Description);
            var typecnt = Convert.ToInt32(invoiceHeader.MemberContacts.Count());
            for (int i = 0; i < typecnt; i++)
            {
                if (invoiceHeader.MemberContacts[i].MemberType == MemberType.Billing)
                {

                    if (!miscCodesList.ContainsValue(invoiceHeader.MemberContacts[i].Type))
                    {
                        throw new ISBusinessException("Invalid Billing Contact Type");
                    }
                    //SCP#55918: In below condition we checked if Member Contact Value is Null, then exception will throw.
                    //because Member Contact Value field is a required field
                    if (string.IsNullOrWhiteSpace(invoiceHeader.MemberContacts[i].Value))
                    {
                        throw new ISBusinessException("Contact value required");
                    }
                }
                if (invoiceHeader.MemberContacts[i].MemberType == MemberType.Billed)
                {

                    if (!miscCodesList.ContainsValue(invoiceHeader.MemberContacts[i].Type))
                    {
                        throw new ISBusinessException("Invalid Billed Contact Type");
                    }
                    //SCP#55918: In below condition we checked if Member Contact Value is Null, then exception will throw.
                    //because Member Contact Value field is a required field
                    if (string.IsNullOrWhiteSpace(invoiceHeader.MemberContacts[i].Value))
                    {
                        throw new ISBusinessException("Contact value required");
                    }
                }
            }




            // Make sure billing and billed member are not the same.
            if (invoiceHeader.BilledMemberId == invoiceHeader.BillingMemberId)
            {
                throw new ISBusinessException(MiscUatpErrorCodes.SameBillingAndBilledMember);
            }

            // Get the details of the billing and billed member.
            var billingMember = MemberManager.GetMember(invoiceHeader.BillingMemberId);
            var billedMember = MemberManager.GetMember(invoiceHeader.BilledMemberId);

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
            ////Shambhu Thakur - Commented Digital Signature Validation 
            ////Shambhu Thakur(08Sep11) - uncommented member profile digitalsignapplication check
            if (billingMember != null && !billingMember.DigitalSignApplication
              && invoiceHeader.DigitalSignatureRequired == DigitalSignatureRequired.Yes)
            {
                throw new ISBusinessException(MiscUatpErrorCodes.InvalidDigitalSignatureValue);
            }

            // i.	  'Digital Signature Flag' in the Invoice Header = “D'; AND
            // ii.	Profile element of the Billing Member 'DS Services Required?' = “Y'; AND
            // iii. Both countries from the invoice reference data (Billing Member and Billed Member) 
            //      are not listed in profile element of the Billing Member 'As a Billing Member, DS Required for Invoices From/To'.
            //if (invoiceHeader.DigitalSignatureRequired == DigitalSignatureRequired.Yes)
            //{
            //  if (!IsDigitalSignatureRequired(invoiceHeader, billingMember, billedMember))
            //  {
            //    throw new ISBusinessException(MiscUatpErrorCodes.InvalidDSFlagAsCountryNotSpecified);
            //  }
            //}

            //Validate invoice Date 
            if (invoiceHeader.BillingCategory == BillingCategoryType.Misc)
            {
              // Invoice date should not be greater than Billing Period mentioned for the Invoice/Credit Note (which may be a past, current or future Billing Period)
              var bPeriod = CalendarManager.GetBillingPeriod(ClearingHouse.Ich, invoiceHeader.InvoiceBillingPeriod.Year, invoiceHeader.InvoiceBillingPeriod.Month, invoiceHeader.InvoiceBillingPeriod.Period);  //
              if (invoiceHeader.InvoiceDate.Date > bPeriod.EndDate)
              {
                throw new ISBusinessException(MiscUatpErrorCodes.InvalidInvoiceDate);
              }
            }
            else
            {
              if (!ValidateInvoiceDate(invoiceHeader.InvoiceDate))
              {
                throw new ISBusinessException(MiscUatpErrorCodes.InvalidInvoiceDate);
              }
            }

            // Update suspended flag according to ach/Ach configuration.
            if (ValidateSuspendedFlag(invoiceHeader, billingFinalParent, billedFinalParent))
            {
                invoiceHeader.SuspendedInvoiceFlag = true;
            }

            // Make sure that the billing member is valid.
            if (billingMember == null)
            {
                throw new ISBusinessException(MiscUatpErrorCodes.InvalidBillingMember);
            }

            // Make sure that the billed member is valid.
            if (billedMember == null)
            {
                throw new ISBusinessException(MiscUatpErrorCodes.InvalidBilledMember);
            }

            // MemberProfile- A member cannot create an invoice/creditNote or Form C when his IS Membership is 'Basic', 'Restricted' or 'Terminated'.
            if (!ValidateBillingMembershipStatus(billingMember))
            {
                throw new ISBusinessException(MiscUatpErrorCodes.InvalidBillingIsMembershipStatus);
            }

            // Validation of the Billed Member will fail if the IS Membership Status of the Billed Member is 'Terminated'. 
            // Refer to profile element 'IS Membership Status'. This will be a non-correctable error.
            if (!ValidateBilledMemberStatus(billedMember))
            {
                throw new ISBusinessException(MiscUatpErrorCodes.InvalidBilledIsMembershipStatus);
            }

            var currentBillingPeriod = CalendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ClearingHouse.Ich);  //GetCurrentBillingPeriod();
            
            // Settlement method validation.
            /*CMP#624: ICH Rewrite-New SMI X , Here validate settlement Method for non X*/
            if (invoiceHeader.SettlementMethodId != (int)SMI.IchSpecialAgreement)
            {
              if (!isUpdateOperation ||
                  (CompareUtil.IsDirty(invoiceHeaderInDb.SettlementMethodId, invoiceHeader.SettlementMethodId) || CompareUtil.IsDirty(invoiceHeaderInDb.BilledMemberId, invoiceHeader.BilledMemberId)))
              {
                if (!ValidateSettlementMethod(invoiceHeader, billingFinalParent, billedFinalParent))
                {
                  throw new ISBusinessException(MiscUatpErrorCodes.InvalidSettlementMethod);
                }
              }
            }
          // Billing period validation.
            if (!isUpdateOperation || CompareUtil.IsDirty(invoiceHeader.InvoiceBillingPeriod, invoiceHeaderInDb.InvoiceBillingPeriod))
            {
                if (!ValidateBillingPeriodOnSaveHeader(invoiceHeader))
                {
                    throw new ISBusinessException(MiscUatpErrorCodes.InvalidBillingPeriod);
                }
            }

            // Invoice Number validation.
            if (!isUpdateOperation || (CompareUtil.IsDirty(invoiceHeaderInDb.InvoiceNumber.ToUpper(), invoiceHeader.InvoiceNumber.ToUpper())))
            {
                // Used ValidationManager method to validate invoice number.
                if (!ValidateInvoiceNumber(invoiceHeader.InvoiceNumber, invoiceHeader.BillingYear, invoiceHeader.BillingMemberId))
                {
                    throw new ISBusinessException(ErrorCodes.DuplicateInvoiceFound);
                }
            }

            // Currency of clearance validation.
            //CMP #553: ACH Requirement for Multiple Currency Handling-FRS-v1.1.doc
            if (!ValidateBillingCurrency(invoiceHeader, billingFinalParent, billedFinalParent))
            {
                throw new ISBusinessException(MiscUatpErrorCodes.InvalidClearanceCurrency);
            }

            // SCP177435 - EXCHANGE RATE 
            // Currency of Billing(listing) validation.
            if (!ValidateListingCurrency(invoiceHeader, billingFinalParent, billedFinalParent))
            {
              throw new ISBusinessException(MiscUatpErrorCodes.InvalidBillingCurrency);
            }

            //// Validate Exchange Rate if and set IS Validation flag.
            //if (invoiceHeader.InvoiceType != InvoiceType.RejectionInvoice)
            //{
            //  if (!IsValidExchangeRate(invoiceHeader, null)) invoiceHeader.IsValidationFlag = ExchangeRateValidationFlag;
            //}

            // if Exchange rate not found for given billing and listing currency.
            // Exchange rate not considered if settlement method is bilateral
            /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
            /*CMP#648: exclude Settlement method id = R*/
            if (!ReferenceManager.IsSmiLikeBilateral(invoiceHeader.SettlementMethodId, false) && Convert.ToDecimal(invoiceHeader.ExchangeRate) <= 0)
            {
              throw new ISBusinessException(MiscUatpErrorCodes.ExchangeRateZero);
            }

            /* // Blocked by Debtor
            if (CheckBlockedMember(true, invoiceHeader.BillingMemberId, invoiceHeader.BilledMemberId, isMisc: true))
            {
              throw new ISBusinessException(MiscUatpErrorCodes.InvalidBillingToMember);
            }

            // Blocked by Creditor
            if (CheckBlockedMember(false, invoiceHeader.BilledMemberId, invoiceHeader.BillingMemberId, isMisc: true))
            {
              throw new ISBusinessException(MiscUatpErrorCodes.InvalidBillingFromMember);
            } */

            // Validation for Blocked Airline
            ValidationForBlockedAirline(invoiceHeader);
            /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
            /*CMP#648: exclude Settlement method id = R*/
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
            // ISValidation flag 

            // Validation for CityAirport as it is auto complete field in UI
            if (!string.IsNullOrEmpty(invoiceHeader.LocationCode) && !ReferenceManager.IsValidCityAirport(invoiceHeader.LocationCode))
            {
                throw new ISBusinessException(MiscUatpErrorCodes.InvalidCityAirport);
            }

            MiscUatpInvoice originalInvoice = null;
            #region RejectionInvoice
            
            
            // Validation for rejection invoice.
            if (invoiceHeader.InvoiceType == InvoiceType.RejectionInvoice)
            {
                if (invoiceHeaderInDb == null || (CompareUtil.IsDirty(invoiceHeader.RejectedInvoiceNumber, invoiceHeaderInDb.RejectedInvoiceNumber)))
                {
                    if (IsInvoiceAlreadyRejected(invoiceHeader))
                    {
                      throw new ISBusinessException(MiscUatpErrorCodes.InvoiceAlreadyRejectedForPayables);
                    }
                    //else if (IsRejectionInvoiceExistsWithInvoiceStatusOtherThanPresented(invoiceHeader))
                    //{
                    //    throw new ISBusinessException(MiscUatpErrorCodes.InvoiceRejectedForPayablesAndStatusIsPresented);
                    //}
                }

                var linkedInvoice = GetMUInvoicePriviousTransanction(invoiceHeader);
                originalInvoice = linkedInvoice;
                // Correspondence invoice cannot be rejected.
                // Check if the rejected invoice number is not that of a correspondence invoice.
                if (linkedInvoice != null)
                {
                    if (linkedInvoice.InvoiceTypeId == (int) InvoiceType.CorrespondenceInvoice)
                    {
                        throw new ISBusinessException(MiscUatpErrorCodes.CorrespondenceInvoiceCannotBeRejected);
                    }

                    if (linkedInvoice.InvoiceStatus == InvoiceStatusType.Open)
                    {
                        throw new ISBusinessException(MiscUatpErrorCodes.OpenInvoiceCannotBeRejected);
                    }
                    //SCP0000:Impact on MISC/UATP rejection linking due to purging
                    if (IsLinkedInvoicePurged(linkedInvoice))
                    {
                        throw new ISBusinessException(MiscUatpErrorCodes.RejectionInvoiceNumberNotExist);
                    }
                    //CMP#624 :  New Validation #9:SMI Match Check for MISC/UATP RM Invoices 
                    if (!ValidateSmiAfterLinking(invoiceHeader.SettlementMethodId, linkedInvoice.SettlementMethodId))
                    {
                        if (invoiceHeader.SettlementMethodId == (int) SMI.IchSpecialAgreement)
                        {
                            throw new ISBusinessException(MiscUatpErrorCodes.MuRejctionInvoiceSmiCheckForSmiX);
                        }
                        else
                        {
                            throw new ISBusinessException(MiscUatpErrorCodes.MuRejctionInvoiceSmiCheckForSmiOtherThanX);
                        }

                    }
                    // CMP#678: Time Limit Validation on Last Stage MISC Rejections

                    #region CMP #678: Time Limit Validation on Last Stage MISC Rejections

                    if (invoiceHeader.BillingCategoryId == (int)BillingCategoryType.Misc)
                    {
                        /*Is Web Standalone Screen*/
                        var rmInTimeLimtMsg = ValidateMiscLastStageRmForTimeLimit(linkedInvoice,
                                                                                  invoiceHeader,
                                                                                  RmValidationType.IsWebStandAlone);
                        if (!string.IsNullOrWhiteSpace(rmInTimeLimtMsg))
                        {
                            throw new ISBusinessException(rmInTimeLimtMsg);
                        }
                    }

                    #endregion
                }
                else
                {
                    /* CMP#624 : 
                     * FRS Section 2.8 New Validation #5 Error message  Original Invoice details are not found. 
                     * Successful Billing History linking is required for Rejection Invoices using Settlement Method X 
                     */
                    if (invoiceHeader.SettlementMethodId == (int)SMI.IchSpecialAgreement)
                    {
                        throw new ISBusinessException(MiscUatpErrorCodes.MuRejctionInvoiceLinkingCheckForSmiX);
                    }
                    // Linking process validation if Rejection Flag is "Y".
                    // Billed member has migrated but could not find the data of the rejected invoice, throw an error.
                    if (!SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod)
                    {
                        if (IsMemberMigrated(invoiceHeader))
                        {
                            throw new ISBusinessException(MiscUatpErrorCodes.InvalidBilledMemberMigrationStatusForRejectedInvoice);
                        }
                    }

                    #region CMP#678: Time Limit Validation on Last Stage MISC Rejections
                    if (invoiceHeader.BillingCategoryId == (int)BillingCategoryType.Misc)
                    {
                        /*Is Web Standalone Screen*/
                        var rejectedInvoiceDetails = new MiscUatpInvoice();
                        rejectedInvoiceDetails.BillingYear = invoiceHeader.SettlementYear;
                        rejectedInvoiceDetails.BillingMonth = invoiceHeader.SettlementMonth;
                        rejectedInvoiceDetails.BillingPeriod = invoiceHeader.SettlementPeriod;

                        var rmInTimeLimtMsg = ValidateMiscLastStageRmForTimeLimit(rejectedInvoiceDetails,
                                                                                  invoiceHeader,
                                                                                  RmValidationType.IsWebStandAlone);
                        if (!string.IsNullOrWhiteSpace(rmInTimeLimtMsg))
                        {
                            throw new ISBusinessException(rmInTimeLimtMsg);
                        }
                    }

                    #endregion
                }

                // Rejection stage 2 not allowed for ICH/Bilateral settlement methods.
                /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
                if ((invoiceHeader.InvoiceSmi == SMI.Ich || ReferenceManager.IsSmiLikeBilateral(invoiceHeader.SettlementMethodId, true) || invoiceHeader.InvoiceSmi == SMI.AchUsingIataRules) && invoiceHeader.RejectionStage == 2)
                {
                    throw new ISBusinessException(MiscUatpErrorCodes.InvalidRejectionStage);
                }

                var settlementMonthPeriod = new BillingPeriod
                {
                    Period = invoiceHeader.SettlementPeriod,
                    Month = invoiceHeader.SettlementMonth,
                    Year = invoiceHeader.SettlementYear
                };

                int settlementMethodId;
                if (linkedInvoice != null)
                {
                    settlementMethodId = linkedInvoice.SettlementMethodId;
                }
                else
                {
                    settlementMethodId = invoiceHeader.SettlementMethodId;
                }

                currentBillingPeriod =
                  CalendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ReferenceManager.GetClearingHouseToFetchCurrentBillingPeriod(settlementMethodId));
                if (settlementMonthPeriod > currentBillingPeriod)
                {
                    throw new ISBusinessException(MiscUatpErrorCodes.InvalidSettlementMonthPeriod);
                }

                //// Invoice number and rejected invoice number can not be same. 
                //if (invoiceHeader.InvoiceNumber.ToLower().Equals(invoiceHeader.RejectedInvoiceNumber))
                //{
                //  throw new ISBusinessException(MiscUatpErrorCodes.InvalidRejectionInvoiceNumber);
                //}

                invoiceHeader.ValidationDate = new DateTime(invoiceHeader.SettlementYear, invoiceHeader.SettlementMonth, invoiceHeader.SettlementPeriod);
                // Transaction out side time limit validation.
                if (IsTransactionOutSideTimeLimit(invoiceHeader))
                {
                    if (invoiceHeader.IsValidationFlag == null || (!string.IsNullOrEmpty(invoiceHeader.IsValidationFlag) && !invoiceHeader.IsValidationFlag.Contains(TimeLimitFlag)))
                        invoiceHeader.IsValidationFlag += string.IsNullOrEmpty(invoiceHeader.IsValidationFlag) ? TimeLimitFlag : ValidationFlagDelimeter + TimeLimitFlag;
                }
            }
            #endregion

            #region CorrespondenceInvoice
            
            if (invoiceHeader.InvoiceType == InvoiceType.CorrespondenceInvoice)
            {
                var isCorrespondenceInvoicePresent = CheckDuplicateCorrespondenceInvoice(invoiceHeader, (invoiceHeaderInDb != null));
                if (isCorrespondenceInvoicePresent)
                {
                    throw new ISBusinessException(MiscUatpErrorCodes.DuplicateInvoiceForCorrespondenceRefNo);
                }

                // Validation for correspondence reference number given has valid for time limit.
                if (ValidateCorrespondenceTimeLimit(invoiceHeader))
                {
                    throw new ISBusinessException(MiscUatpErrorCodes.TimeLimitExpiryForCorrespondence);
                }
                var miscCorrespondence =
                    CorrespondenceRepository.Get(invoiceHeader.CorrespondenceRefNo).
                        OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();

                if (miscCorrespondence != null)
                {
                    /* getting Last stage rejection invoice */
                    var rejectedInvoice = miscUatpInvoiceRepository.Single(miscCorrespondence.InvoiceId,
                                                                       billingCategoryId:
                                                                           (int)invoiceHeader.BillingCategory);
                  originalInvoice = rejectedInvoice;
                  if(rejectedInvoice != null)
                  {
                    //CMP#624 :  New Validation #9:SMI Match Check for MISC/UATP Correspondence Invoices 
                    if (!ValidateSmiAfterLinking(invoiceHeader.SettlementMethodId, rejectedInvoice.SettlementMethodId))
                    {
                      if (invoiceHeader.SettlementMethodId == (int)SMI.IchSpecialAgreement)
                      {
                        throw new ISBusinessException(MiscUatpErrorCodes.MuCorrespondenceInvoiceSmiCheckForSmiX);
                      }
                      else
                      {
                        throw new ISBusinessException(MiscUatpErrorCodes.MuCorrespondenceInvoiceLinkingCheckForSmiOtherThanX);
                      }

                    }
                  }
                  /* CMP#624 : 
                   * FRS Section 2.8 New Validation #6 Error message  Original Invoice details are not found. 
                   * Successful Billing History linking is required for Correspondence Invoices using Settlement Method X 
                   */
                  else if(invoiceHeader.SettlementMethodId ==(int)SMI.IchSpecialAgreement)
                  {
                    throw new ISBusinessException(MiscUatpErrorCodes.MuCorrespondenceInvoiceLinkingCheckForSmiX);
                  }
                }
            }
            #endregion

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

            // if ((!isUpdateOperation || (CompareUtil.IsDirty(invoiceHeaderInDb.SettlementMethodId, invoiceHeader.SettlementMethodId) || CompareUtil.IsDirty(invoiceHeaderInDb.BillingMonth, invoiceHeader.BillingMonth) || CompareUtil.IsDirty(invoiceHeaderInDb.BillingYear, invoiceHeader.BillingYear) || CompareUtil.IsDirty(invoiceHeaderInDb.ListingCurrencyId, invoiceHeader.ListingCurrencyId) || CompareUtil.IsDirty(invoiceHeaderInDb.BillingCurrencyId, invoiceHeader.BillingCurrencyId))) && invoiceHeader.ListingCurrencyId.HasValue && invoiceHeader.BillingCurrency.HasValue)
            // {
            /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
            if (!ReferenceManager.IsSmiLikeBilateral(invoiceHeader.SettlementMethodId, true))
            {
                if (!ValidateInvoiceExchangeRate(invoiceHeader))
                {
                    if (invoiceHeader.SettlementMethodId != (int)SettlementMethodValues.Bilateral)
                    {
                        var correctExchangeRate = ReferenceManager.GetExchangeRate(invoiceHeader.ListingCurrencyId.Value,
                                                                             (BillingCurrency)invoiceHeader.BillingCurrencyId.Value,
                                                                             invoiceHeader.BillingYear,
                                                                             invoiceHeader.BillingMonth);
                        // Update Exchange rate
                        invoiceHeader.ExchangeRate = Convert.ToDecimal(correctExchangeRate);
                        // Update Clearence Amount
                        invoiceHeader.ClearanceAmount = invoiceHeader.BillingAmount / Convert.ToDecimal(correctExchangeRate);
                        // Update TotalAmountInClearanceCurrency Amount
                        invoiceHeader.InvoiceSummary.TotalAmountInClearanceCurrency = invoiceHeader.BillingAmount / Convert.ToDecimal(correctExchangeRate);
                    }
                    else
                    {
                        throw new ISBusinessException(MiscUatpErrorCodes.InvalidExchangeRate);
                    }
                }
            }
            //CMP#624 : Exchange Rate Validation
            if(invoiceHeader.SettlementMethodId == (int)SMI.IchSpecialAgreement)
            {
              if (invoiceHeader.BillingCurrencyId == invoiceHeader.ListingCurrencyId && Convert.ToDecimal(invoiceHeader.ExchangeRate) != 1)
              {
                throw new ISBusinessException(ErrorCodes.InvalidListingToBillingRateForSameCurrencies);
              }
              if (Convert.ToDecimal(invoiceHeader.ExchangeRate) > 0)
              {
                // Update Clearence Amount
                invoiceHeader.ClearanceAmount = invoiceHeader.BillingAmount / invoiceHeader.ExchangeRate.Value;
                // Update TotalAmountInClearanceCurrency Amount
                invoiceHeader.InvoiceSummary.TotalAmountInClearanceCurrency = invoiceHeader.BillingAmount / invoiceHeader.ExchangeRate.Value;
              }
            }
            //SCP277476: Validate Total amount of TAX, VAT and Add-on Charge against its respective breakdown

            if (invoiceHeaderInDb != null)
            {
              decimal sumofTaxAmountbreakdown = 0M;
              decimal sumofVatAmountbreakdown = 0M;
              decimal sumofAddOnChargeAmountbreakdown = 0M;

              if (invoiceHeader.TaxBreakdown.Count > 0)
              {
                sumofTaxAmountbreakdown = ConvertUtil.Round(invoiceHeader.TaxBreakdown.Where(tax => tax.Type.ToUpper() == "TAX" && tax.CalculatedAmount.HasValue).Sum(invoiceTax => invoiceTax.CalculatedAmount.Value), Constants.MiscDecimalPlaces);

                sumofVatAmountbreakdown = invoiceHeader.TaxBreakdown.Where(tax => tax.Type.ToUpper() == "VAT" && tax.CalculatedAmount.HasValue).Sum(invoiceTax => invoiceTax.CalculatedAmount.Value);
              }

              if (invoiceHeader.AddOnCharges.Count > 0)
              {
                sumofAddOnChargeAmountbreakdown = invoiceHeader.AddOnCharges.Sum(invoiceTax => invoiceTax.Amount);
              }

              var validationResult = ValidateMiscInvoiceBreakDownCaptured(invoiceHeader.Id, 1,
                                                              invoiceHeader.InvoiceSummary.TotalTaxAmount ?? 0,
                                                              sumofTaxAmountbreakdown, sumofVatAmountbreakdown,
                                                              invoiceHeader.InvoiceSummary.TotalVatAmount ?? 0,
                                                              invoiceHeader.InvoiceSummary.TotalAddOnChargeAmount ?? 0,
                                                              sumofAddOnChargeAmountbreakdown);


              if (!string.IsNullOrEmpty(validationResult))
              {
                validationResult = validationResult.Remove(validationResult.Length - 1);

                throw new ISBusinessException("The '" + validationResult + "' of this invoice has already been updated by another user ,please get a latest version of this invoice.");
              }
            }
          
            /* CMP #624: ICH Rewrite-New SMI X 
            * Description: ICH Web Service is called when header is saved
            * Refer FRS Section: 2.17 MISC IS-WEB Screens (Part 1). */
            /* Get original invoice from input invoice */
            if (invoiceHeader.InvoiceTypeId == (int)InvoiceType.CorrespondenceInvoice && invoiceHeader.SettlementMethodId == (int)SMI.IchSpecialAgreement)
            {
              MiscUatpInvoice linkedRm1Invoice;
              MiscUatpInvoice linkedRm2Invoice;
              MiscCorrespondence linkedCorrespondence;
              originalInvoice = GetLinkedMUOriginalInvoice(originalInvoice, out linkedRm1Invoice, out linkedRm2Invoice, out linkedCorrespondence);
            }

          /* CMP #624: ICH Rewrite-New SMI X 
            * Description: ICH Web Service is called when header is saved
            * Refer FRS Section: 2.8	Detailed Validation of IS-IDEC and IS-XML Files (Part 1). */
            bool smiXValidationsPhase1Result = ValidationBeforeSmiXWebServiceCall(invoiceHeader, null,
                                                                                  invoiceHeader.InvoiceTypeId,
                                                                                  null, null,
                                                                                  billingFinalParent, billedFinalParent,
                                                                                  true, originalInvoice, 0, 0);
            

          /* CMP #624: ICH Rewrite-New SMI X 
            * Description: ICH Web Service is called when header is saved
            * Refer FRS Section: 2.18 MISC IS-WEB Screens (Part 2). */
            /*CMP#624: ICH Rewrite-New SMI X , Here validate settlement Method for non X*/
            if (invoiceHeader.SettlementMethodId == (int)SMI.IchSpecialAgreement && invoiceHeader.GetSmiXPhase1ValidationStatus())
            {
              //Populate Net due date in payment details if valid CHDueDate is provided
              if (invoiceHeader.ChDueDate.HasValue)
              {
                if (invoiceHeader.PaymentDetail == null)
                {
                  invoiceHeader.PaymentDetail = new PaymentDetail();
                }
                invoiceHeader.PaymentDetail.NetDueDate = invoiceHeader.ChDueDate;
              }
              else
              {
                if (invoiceHeader.PaymentDetail != null)
                  invoiceHeader.PaymentDetail.NetDueDate = null;
              }
              return CallSmiXIchWebServiceAndHandleResponse(invoiceHeader, null, invoiceHeader.InvoiceTypeId, null, null, true, 0, 0, originalInvoice);
            }

          return true;
        }

        private bool ValidateInvoiceExchangeRate(MiscUatpInvoice invoiceHeader)
        {
            bool flag = false;
            switch (invoiceHeader.InvoiceTypeId)
            {
                case (int)Model.Enums.InvoiceType.Invoice:
                //CMP#648: Convert Exchange rate into nullable field.
                    flag = ValidateExchangeRate(invoiceHeader.ExchangeRate.HasValue? invoiceHeader.ExchangeRate.Value: (decimal?)null,
                                                invoiceHeader.ListingCurrencyId.Value,
                                                invoiceHeader.BillingCurrency.Value,
                                                invoiceHeader.BillingYear,
                                                invoiceHeader.BillingMonth);

                    break;
                //SCP279473 - Misc credit note- inconsistency between ISWEB and File behavior
                //Exchange rate for Misc Credit note, should behave like a Bileatral. So it's exchange rate must be same and no correction should be done. 
                case (int)Model.Enums.InvoiceType.CreditNote:
                    //CMP#648: Convert Exchange rate into nullable field.
                    if (invoiceHeader.BillingCurrencyId.HasValue)
                    {
                      if (invoiceHeader.ListingCurrencyId.Value == invoiceHeader.BillingCurrencyId.Value)
                        invoiceHeader.ExchangeRate = Convert.ToDecimal(1.00000);
                    }
                    flag = true;
                    break;
                case (int)Model.Enums.InvoiceType.RejectionInvoice:
                case (int)Model.Enums.InvoiceType.CorrespondenceInvoice:
                    //CMP#648: Convert Exchange rate into nullable field.
                    flag = ValidateExchangeRate(invoiceHeader.ExchangeRate.HasValue? invoiceHeader.ExchangeRate.Value: (decimal?) null,
                                                invoiceHeader.ListingCurrencyId.Value,
                                                invoiceHeader.BillingCurrency.Value,
                                                invoiceHeader.SettlementYear,
                                                invoiceHeader.SettlementMonth);

                    break;
                default:
                    break;
            }

            return flag;
        }
        /// <summary>
        /// Validates the correspondence time limit.
        /// </summary>
        /// <param name="invoiceHeader">The invoice header.</param>
        public bool ValidateCorrespondenceTimeLimit(MiscUatpInvoice invoiceHeader)
        {
            var miscCorrespondence =
              MiscCorrespondenceRepository.Get(invoiceHeader.CorrespondenceRefNo, (int)CorrespondenceSubStatus.Responded).OrderByDescending(correspondence => correspondence.CorrespondenceStage).
                FirstOrDefault();

            var isOutsideTimeLimit = false;

            if (miscCorrespondence != null)
            {
                var transactionType = TransactionType.MiscCorrespondence;

                if (miscCorrespondence.CorrespondenceStatus == CorrespondenceStatus.Expired)
                {
                    transactionType = TransactionType.MiscCorrInvoiceDueToExpiry;
                }
                else if (miscCorrespondence.CorrespondenceStatus == CorrespondenceStatus.Open && miscCorrespondence.AuthorityToBill)
                {
                    transactionType = TransactionType.MiscCorrInvoiceDueToAuthorityToBill;
                }
                //CMP#624 : 2.10 - Change#6 : Time Limits
                // While calculating time limit for SMI X it should behave like SMI I.
                isOutsideTimeLimit = miscCorrespondence.BMExpiryPeriod.HasValue
                                       ? new DateTime(invoiceHeader.BillingYear, invoiceHeader.BillingMonth,
                                                      invoiceHeader.BillingPeriod) > miscCorrespondence.BMExpiryPeriod.Value
                                       : /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
                                       (!ReferenceManager.IsSmiLikeBilateral(invoiceHeader.SettlementMethodId, false))
                                           ? !ReferenceManager.IsTransactionInTimeLimitMethodD(transactionType,
                                                                                               invoiceHeader.SettlementMethodId,
                                                                                               miscCorrespondence.
                                                                                                 CorrespondenceDate)
                                           : !ReferenceManager.IsTransactionInTimeLimitMethodD1(transactionType,
                                                                                                Convert.ToInt32(SMI.Bilateral),
                                                                                                miscCorrespondence.
                                                                                                  CorrespondenceDate);
            }

            return isOutsideTimeLimit;
        }

        /// <summary>
        /// Checks the duplicate correspondence invoice exists
        /// for the billing and billed member combination.
        /// </summary>
        /// <param name="invoiceHeader">The invoice header.</param>
        /// <param name="isUpdateOperation">if set to true [is update operation].</param>
        /// <returns></returns>
        protected bool CheckDuplicateCorrespondenceInvoice(MiscUatpInvoice invoiceHeader, bool isUpdateOperation)
        {
            const int correspodenceInvoiceEnumId = (int)InvoiceType.CorrespondenceInvoice;
            // check if correspondence ref number is not specific of any invoice
            if (isUpdateOperation)
            {
                // in case of update operation, exclude current invoice from the 
                // fetch criteria.
                // SCP-20517: Added the checks of Invoice Status other than Error Non-Correctable
                //SCP186155: Same BM ,RAm5.2.2.5
                return MiscUatpInvoiceRepository.GetCount(invoice => invoice.InvoiceTypeId == correspodenceInvoiceEnumId
                                                    && invoice.BilledMemberId == invoiceHeader.BilledMemberId
                                                    && invoice.BillingMemberId == invoiceHeader.BillingMemberId
                                                    && invoice.CorrespondenceRefNo == invoiceHeader.CorrespondenceRefNo
                                                    && invoice.Id != invoiceHeader.Id
                                                    && !(invoice.InvoiceStatusId == (int)InvoiceStatusType.ErrorNonCorrectable && invoice.ValidationStatusId == (int)InvoiceValidationStatus.Failed)) > 0;
            }

            return MiscUatpInvoiceRepository.GetCount(invoice => invoice.InvoiceTypeId == correspodenceInvoiceEnumId
                                                  && invoice.BilledMemberId == invoiceHeader.BilledMemberId
                                                  && invoice.BillingMemberId == invoiceHeader.BillingMemberId
                                                  && invoice.CorrespondenceRefNo == invoiceHeader.CorrespondenceRefNo
                                                  && !(invoice.InvoiceStatusId == (int)InvoiceStatusType.ErrorNonCorrectable && invoice.ValidationStatusId == (int)InvoiceValidationStatus.Failed)) > 0;
        }

        /// <summary>
        /// Determines whether [is invoice already rejected] [the specified invoice].
        /// </summary>
        /// <param name="miscUatpInvoice">The misc UATP invoice.</param>
        /// <returns>
        /// 	<c>true</c> if [is invoice already rejected] [the specified invoice]; otherwise, <c>false</c>.
        /// </returns>
    /* SCP84368 - We Cannot Save One Invoice Header at All 
     * Date: 14-Nar-2013
     * Desc: Added billing and billed member checks in below LINQ query.
     */
        private bool IsInvoiceAlreadyRejected(MiscUatpInvoice miscUatpInvoice)
        {
        //SCPID : 117317 - question about same invoice No, billingMember condition was cheked twicely, instead of Billed Member
            var alreadyRejectedInvoicesCount =
              MiscUatpInvoiceRepository.GetCount(
                invoice =>
                invoice.RejectedInvoiceNumber.ToUpper() == miscUatpInvoice.RejectedInvoiceNumber.ToUpper() &&
                invoice.SettlementYear == miscUatpInvoice.SettlementYear &&
                invoice.InvoiceNumber.ToUpper() != miscUatpInvoice.InvoiceNumber.ToUpper() 
                && invoice.BillingMemberId == miscUatpInvoice.BillingMemberId &&
                invoice.BilledMemberId == miscUatpInvoice.BilledMemberId
                && !(invoice.InvoiceStatusId == (int)InvoiceStatusType.ErrorNonCorrectable && invoice.ValidationStatusId == (int)InvoiceValidationStatus.Failed));
            return alreadyRejectedInvoicesCount > 0;
        }

    //    /// <summary>
    //    /// Determines whether Invoice is already rejected and RejectedInvoice's status is not equal to Presented if yes returns true else false
    //    /// </summary>
    //    /// <param name="miscUatpInvoice">Invoice object</param>
    //    /// <returns>Returns true if RejectionInvoice exists and Status is Presented else returns False</returns>
    ///* SCP84368 - We Cannot Save One Invoice Header at All 
    // * Date: 14-Mar-2013
    // * Desc: Added billing and billed member conditions in LINQ query below.
    // */
    //    private bool IsRejectionInvoiceExistsWithInvoiceStatusOtherThanPresented(MiscUatpInvoice miscUatpInvoice)
    //    {
    //        var alreadyRejectedInvoicesCount =
    //          MiscUatpInvoiceRepository.GetCount(
    //            invoice =>
    //            invoice.RejectedInvoiceNumber.ToUpper() == miscUatpInvoice.RejectedInvoiceNumber.ToUpper() && invoice.SettlementYear == miscUatpInvoice.SettlementYear &&
    //      invoice.InvoiceNumber.ToUpper() != miscUatpInvoice.InvoiceNumber.ToUpper() && invoice.InvoiceStatusId != (int)InvoiceStatusType.Presented
    //      && invoice.BillingMemberId == miscUatpInvoice.BillingMemberId && invoice.BilledMemberId == miscUatpInvoice.BilledMemberId);
    //        return alreadyRejectedInvoicesCount > 0;
    //    }


        /// <summary>
        /// Validates the invoice total amount.
        /// </summary>
        /// <param name="invoiceHeader">The invoice header.</param>
        /// <returns></returns>
        protected bool ValidateInvoiceTotalAmount(MiscUatpInvoice invoiceHeader)
        {
            //var totalInvoiceAmount = GetInvoiceTotalAmount(invoiceHeader);

            var transactionType = TransactionType.MiscOriginal;
            switch (invoiceHeader.BillingCategory)
            {
                case BillingCategoryType.Misc:
                    switch (invoiceHeader.InvoiceType)
                    {
                        case InvoiceType.Invoice:
                            transactionType = TransactionType.MiscOriginal;
                            break;
                        case InvoiceType.CreditNote:
                            transactionType = TransactionType.MiscOriginal;
                            break;
                        case InvoiceType.RejectionInvoice:
                            switch (invoiceHeader.RejectionStage)
                            {
                                case 1:
                                    transactionType = TransactionType.MiscRejection1;
                                    break;
                                case 2:
                                    transactionType = TransactionType.MiscRejection2;
                                    break;
                            }
                            break;
                        case InvoiceType.CorrespondenceInvoice:
                            transactionType = invoiceHeader.IsAuthorityToBill ? TransactionType.MiscCorrInvoiceDueToAuthorityToBill : TransactionType.MiscCorrInvoiceDueToExpiry;
                            break;
                    }
                    break;

                case BillingCategoryType.Uatp:
                    switch (invoiceHeader.InvoiceType)
                    {
                        case InvoiceType.Invoice:
                            transactionType = TransactionType.UatpOriginal;
                            break;
                        case InvoiceType.CreditNote:
                            transactionType = TransactionType.UatpOriginal;
                            break;
                        case InvoiceType.RejectionInvoice:
                            switch (invoiceHeader.RejectionStage)
                            {
                                case 1:
                                    transactionType = TransactionType.UatpRejection1;
                                    break;
                                case 2:
                                    transactionType = TransactionType.UatpRejection2;
                                    break;
                            }
                            break;
                        case InvoiceType.CorrespondenceInvoice:
                            transactionType = invoiceHeader.IsAuthorityToBill ? TransactionType.UatpCorrInvoiceDueToAuthorityToBill : TransactionType.UatpCorrInvoiceDueToExpiry;
                            break;
                    }

                    break;
            }

            return ReferenceManager.IsValidNetAmount(Convert.ToDouble(invoiceHeader.InvoiceSummary == null ? 0 : invoiceHeader.InvoiceSummary.TotalAmount), transactionType, invoiceHeader.ListingCurrencyId, invoiceHeader, applicableMinimumField: ApplicableMinimumField.TotalAmount);
        }


        protected static decimal? GetInvoiceTotalAmount(MiscUatpInvoice invoiceHeader)
        {
            /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
            return StaticReferenceManager.IsSmiLikeBilateral(invoiceHeader.SettlementMethodId, false) ? invoiceHeader.InvoiceSummary.TotalAmount : invoiceHeader.InvoiceSummary.TotalAmountInClearanceCurrency;
        }

        /// <summary>
        /// This will return the countryId of the country matching given country code
        /// </summary>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        public string GetCountryId(string countryCode)
        {
            Country country = null;
            if (countryCode.Length > 0)
            {
                country = CountryRepository.First(i => i.Id == countryCode);
            }
            return country != null ? country.Id : string.Empty;
        }

        /// <summary>
        /// This will return the currency object matching given currency code
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <returns></returns>
        public Currency GetCurrency(string currencyCode)
        {
            var currency = CurrencyRepository.First(i => i.IsActive && i.Code == currencyCode);
            return currency;
        }

        /// <summary>
        /// Determines whether member migrated for specified invoice header.
        /// </summary>
        /// <param name="invoiceHeader">The invoice header.</param>
        /// <returns>
        /// true if member migrated for specified invoice header; otherwise, false.
        /// </returns>
        public virtual bool IsMemberMigrated(MiscUatpInvoice invoiceHeader)
        {
            return true;
        }

        /// <summary>
        /// Determines whether member migrated for specified invoice header.
        /// </summary>
        /// <param name="invoiceHeader">The invoice header.</param>
        /// <returns>
        /// true if member migrated for specified invoice header; otherwise, false.
        /// </returns>
        public virtual bool IsBillingMemberMigrated(MiscUatpInvoice invoiceHeader)
        {
            return true;
        }

        public virtual bool IsOnBehalfBillingMemberMigrated(int issuingOrganizationId, BillingPeriod invoiceBillingPeriod)
        {
            return true;
        }

        /// <summary>
        /// This will return ChargeCategory matching with chargeCategoryName
        /// </summary>
        /// <param name="chargeCategoryName"></param>
        /// <param name="billingCategoryId"></param>
        /// <returns></returns>
        public ChargeCategory GetChargeCategory(string chargeCategoryName, int billingCategoryId)
        {
            if (!string.IsNullOrEmpty(chargeCategoryName))
            {
                var chargeCategory = ChargeCategoryRepository.First(i => i.IsActive && i.Name == chargeCategoryName && i.BillingCategoryId == billingCategoryId);
                return chargeCategory;
            }
            return null;
        }

        /// <summary>
        /// This will return All charge code types of corresponding charge code id
        /// </summary>
        /// <param name="chargeCodeId"></param>
        /// <returns></returns>
        public List<ChargeCodeType> GetChargeCodeTypeList(int chargeCodeId)
        {
            var chargeCodeTypeCollection = ChargeCodeTypeRepository.Get(i => i.IsActive && i.ChargeCodeId == chargeCodeId).ToList();
            return chargeCodeTypeCollection;
        }

        // CMP # 533: RAM A13 New Validations and New Charge Code [Start]
        /// <summary>
        /// This will return ChargeCodeType matching with given Charge code type name and chargeCodeId
        /// </summary>
        /// <param name="chargeCodeTypeName"></param>
        /// <param name="chargeCodeId"></param>
        /// <returns> Charge Code Type for given valid Charge Code Type Name and valid Charge Code Id or null </returns>
        public ChargeCodeType GetChargeCodeTypeOnChargeCodeId(string chargeCodeTypeName, int? chargeCodeId)
        {
            // check for valid input values.
            if (!string.IsNullOrEmpty(chargeCodeTypeName) && chargeCodeId != null)
            {
                // Get the ChargeCodeTypes from the database.
                var chargeCodeType = ChargeCodeTypeRepository.First(i => i.IsActive && i.Name.ToUpper() == chargeCodeTypeName.ToUpper() && i.ChargeCodeId == chargeCodeId);

                // return Charge Code Type.
                return chargeCodeType;
            } // End if

            // returns null when input parameters are invalid.
            return null;
        } // End GetChargeCodeTypeOnChargeCodeId.
        // CMP # 533: RAM A13 New Validations and New Charge Code [End]

        /// <summary>
        /// This will return ChargeCode matching with given chargeCodeName
        /// </summary>
        /// <param name="chargeCodeName"></param>
        /// <param name="chargeCategoryId"></param>
        /// <returns></returns>
        public ChargeCode GetChargeCode(string chargeCodeName)
        {
            if (!string.IsNullOrEmpty(chargeCodeName))
            {
                var chargeCode = ChargeCodeRepository.First(i => i.IsActive && i.Name.ToUpper() == chargeCodeName.ToUpper());
                return chargeCode;
            }
            return null;
        }

        /// <summary>
        /// This will return ChargeCode matching with given chargeCodeId
        /// </summary>
        /// <param name="chargeCodeName"></param>
        /// <param name="chargeCategoryId"></param>
        /// <returns></returns>
        public ChargeCode GetChargeCode(int chargeCodeId)
        {
          if (chargeCodeId > 0)
          {
            var chargeCode = ChargeCodeRepository.First(i => i.IsActive && i.Id == chargeCodeId);
            return chargeCode;
          }
          return null;
        }

        /// <summary>
        /// This will return ChargeCode matching with given chargeCodeName
        /// </summary>
        /// <param name="chargeCodeName"></param>
        /// <param name="chargeCategoryId"></param>
        /// <returns></returns>
        public ChargeCode GetChargeCode(string chargeCodeName, int chargeCategoryId)
        {
            if (!string.IsNullOrEmpty(chargeCodeName))
            {
                var chargeCode = ChargeCodeRepository.First(i => i.IsActive && i.Name.ToUpper() == chargeCodeName.ToUpper() && i.ChargeCategoryId == chargeCategoryId);
                return chargeCode;
            }
            return null;
        }

        /// <summary>
        /// This will return ChargeCodeType matching with given chargeCodeTypeName
        /// </summary>
        /// <param name="chargeCodeTypeName"></param>
        /// <returns></returns>
        public ChargeCodeType GetChargeCodeType(string chargeCodeTypeName)
        {
            if (!string.IsNullOrEmpty(chargeCodeTypeName))
            {
                var chargeCodeType = ChargeCodeTypeRepository.First(i => i.IsActive && i.Name.ToUpper() == chargeCodeTypeName.ToUpper());
                return chargeCodeType;
            }
            return null;
        }

        /// <summary>
        /// Get UomCode with given UomCode name
        /// </summary>
        /// <param name="uomCodeName"></param>
        /// <returns></returns>
        public UomCode GetUomCode(string uomCodeName)
        {
            if (!string.IsNullOrEmpty(uomCodeName))
            {
                var uomCode = UomCodeRepository.First(i => i.IsActive && i.Id == uomCodeName);
                return uomCode;
            }
            return null;
        }

        /// <summary>
        /// Determines whether FieldMetaData exists for given charge code and charge code type.
        /// </summary>
        /// <param name="chargeCodeid">ChargeCode Id.</param>
        /// <param name="chargeCodeTypeId">ChargeCodeType Id.</param>
        /// <returns>
        /// true if FieldMetaData exists given charge code and change code type; otherwise, false.
        /// </returns>
        public bool IsFieldMetaDataExists(int chargeCodeid, int? chargeCodeTypeId, int billingCategoryId)
        {
            //CMP #636: Standard Update Mobilization.
            if (billingCategoryId == -1)
            {
              billingCategoryId = (from chCode in ChargeCodeRepository.GetAll()
                                   join chCat in ChargeCategoryRepository.GetAll() on
                                   chCode.ChargeCategoryId equals chCat.Id
                                   where chCode.Id == chargeCodeid
                                   select chCat.BillingCategoryId).FirstOrDefault();
            }

            //CMP #636: Standard Update Mobilization.
            if (chargeCodeTypeId.HasValue)
            {
              return FieldChargeCodeMappingRepository.GetCount(fmdChargeCode => fmdChargeCode.ChargeCodeId == chargeCodeid &&  (billingCategoryId == (int)BillingCategoryType.Misc ? true : fmdChargeCode.ChargeCodeTypeId == chargeCodeTypeId)) > 0;
            }
            // Note: Explicitly passing null when charge code type is null as the query translate to 'IS NULL' instead of '= NULL'
            return FieldChargeCodeMappingRepository.GetCount(fmdChargeCode => fmdChargeCode.ChargeCodeId == chargeCodeid && fmdChargeCode.ChargeCodeTypeId == null) > 0;
        }

        private bool IsMandatoryLineItemDetailNotFound(Guid invoiceId, int billingCategoryId,  out int lineItemNumber)
        {
            bool isLineItemDetailExpected;
            MiscUatpInvoiceRepository.IsLineItemDetailExpected(invoiceId, billingCategoryId, out isLineItemDetailExpected, out lineItemNumber);

            return isLineItemDetailExpected;
        }

        /// <summary>
        /// Determines whether FieldMetaData exists for given charge code and charge code type.
        /// Note : This is a oveload method created to use the Field metadata stored in the validation cache during Parsing 
        /// </summary>
        /// <param name="chargeCodeid">ChargeCode Id.</param>
        /// <param name="chargeCodeTypeId">ChargeCodeType Id.</param>
        /// <param name="miscUatpInvoice">miscUatpInvoice</param>
        /// <returns>
        /// true if FieldMetaData exists given charge code and change code type; otherwise, false.
        /// </returns>
        public bool IsFieldMetaDataExists(int chargeCodeid, int? chargeCodeTypeId, MiscUatpInvoice miscUatpInvoice)
        {
            //CMP #636: Standard Update Mobilization
            if (miscUatpInvoice != null && miscUatpInvoice.ValidFieldChargeCodeMapping != null && miscUatpInvoice.ValidFieldChargeCodeMapping.Count > 0)
            {
                //Get it from validation cache
                if (chargeCodeTypeId.HasValue)
                  return miscUatpInvoice.ValidFieldChargeCodeMapping.Count(fmdChargeCode => fmdChargeCode.ChargeCodeId == chargeCodeid && (miscUatpInvoice.BillingCategory == BillingCategoryType.Misc ? true : fmdChargeCode.ChargeCodeTypeId == chargeCodeTypeId)) > 0;

                return miscUatpInvoice.ValidFieldChargeCodeMapping.Count(fmdChargeCode => fmdChargeCode.ChargeCodeId == chargeCodeid && fmdChargeCode.ChargeCodeTypeId == null) > 0;
            }
            //Get it from db
            if (chargeCodeTypeId.HasValue)
            {
              return FieldChargeCodeMappingRepository.GetCount(fmdChargeCode => fmdChargeCode.ChargeCodeId == chargeCodeid && (miscUatpInvoice.BillingCategory == BillingCategoryType.Misc ? true : fmdChargeCode.ChargeCodeTypeId == chargeCodeTypeId)) > 0;
            }
            // Note: Explicitly passing null when charge code type is null as the query translate to 'IS NULL' instead of '= NULL'
            return FieldChargeCodeMappingRepository.GetCount(fmdChargeCode => fmdChargeCode.ChargeCodeId == chargeCodeid && fmdChargeCode.ChargeCodeTypeId == null) > 0;
        }

        /// <summary>
        /// Validates the line item.
        /// </summary>
        /// <param name="lineItem">LineItem</param>
        /// <param name="lineItemInDb">LineItem in db.</param>
        protected virtual void ValidateLineItem(LineItem lineItem, LineItem lineItemInDb)
        {
            var isUpdateOperation = false;

            if (lineItemInDb != null)
            {
                isUpdateOperation = true;
            }

            if (!isUpdateOperation)
            {
                // If Line Item with same line item number exists in a database then update line item number to Max line item number + 1.
                if (LineItemRepository.GetCount(lineItemRec => lineItemRec.InvoiceId == lineItem.InvoiceId && lineItemRec.LineItemNumber == lineItem.LineItemNumber) > 0)
                {
                    // Line Item Detail number will be set to Max line item detail number + 1.
                    //lineItem.LineItemNumber = LineItemRepository.Get(lineItemRec => lineItemRec.InvoiceId == lineItem.InvoiceId).Max(lineItemRec => lineItemRec.LineItemNumber) + 1;
                    lineItem.LineItemNumber = LineItemRepository.Get(lineItem.InvoiceId).Max(lineItemRec => lineItemRec.LineItemNumber) + 1;
                }
            }

            //if (lineItem.TaxBreakdown.Count > 0)
      //{
      //  var lineItemTotalTaxAmount = lineItem.TaxBreakdown.Where(tax => tax.Type.ToUpper() == "TAX" && tax.CalculatedAmount.HasValue).Sum(invoiceTax => invoiceTax.CalculatedAmount.Value);

      //  decimal lineItemDetailTotalTaxAmountSum = 0;
      //  decimal linteItemDetailTotalVatAmountSum = 0;

      //  if (lineItemInDb != null && lineItemInDb.LineItemDetails.Count > 0)
      //  {
      //    lineItemDetailTotalTaxAmountSum = lineItemInDb.LineItemDetails.Sum(lineItemDetail => lineItemDetail.TotalTaxAmount.HasValue ? lineItemDetail.TotalTaxAmount.Value : 0);
      //    linteItemDetailTotalVatAmountSum = lineItemInDb.LineItemDetails.Sum(lineItemDetail => lineItemDetail.TotalVatAmount.HasValue ? lineItemDetail.TotalVatAmount.Value : 0);
      //  }

      //  // TotalTaxAmount should be equal to sum of Calculated amount in TaxBreakdown
      //  if (lineItem.TotalTaxAmount != null)
      //    if (ConvertUtil.Round(lineItem.TotalTaxAmount.Value, Constants.MiscDecimalPlaces) != ConvertUtil.Round(lineItemTotalTaxAmount + lineItemDetailTotalTaxAmountSum, Constants.MiscDecimalPlaces))
      //    {
      //      throw new ISBusinessException(MiscUatpErrorCodes.InvalidTotalTaxAmount);
      //    }

      //  var lineItemTotalVatAmount = lineItem.TaxBreakdown.Where(tax => tax.Type.ToUpper() == "VAT" && tax.CalculatedAmount.HasValue).Sum(invoiceTax => invoiceTax.CalculatedAmount.Value);

      //  //// TotalVatAmount should be equal to sum of Calculated amount in VatBreakdown
      //  if (lineItem.TotalVatAmount != null)
      //    if (ConvertUtil.Round(lineItem.TotalVatAmount.Value, Constants.MiscDecimalPlaces) != ConvertUtil.Round(lineItemTotalVatAmount + linteItemDetailTotalVatAmountSum, Constants.MiscDecimalPlaces))
      //    {
      //      throw new ISBusinessException(MiscUatpErrorCodes.InvalidTotalVatAmount);
      //    }
      //}

      //// TotalVatAmount should be equal to sum of Calculated amount in VatBreakdown
      //if (lineItem.AddOnCharges.Count > 0)
      //{
      //  var lineItemAddonChargeTotalSum = lineItem.AddOnCharges.Sum(invoiceTax => invoiceTax.Amount);
      //  decimal lineItemDetailTotalAddOnChargeAmountSum = 0;

      //  if (lineItemInDb != null)
      //  {
      //    if (lineItemInDb.LineItemDetails.Count > 0)
      //    {
      //      lineItemDetailTotalAddOnChargeAmountSum = lineItemInDb.LineItemDetails.Sum(lineItemDetail => lineItemDetail.TotalAddOnChargeAmount.HasValue ? lineItemDetail.TotalAddOnChargeAmount.Value : 0);
      //    }
      //  }

      //  if (lineItem.TotalAddOnChargeAmount != null)
      //    if (ConvertUtil.Round(lineItem.TotalAddOnChargeAmount.Value, Constants.MiscDecimalPlaces) != ConvertUtil.Round(lineItemAddonChargeTotalSum + lineItemDetailTotalAddOnChargeAmountSum, Constants.MiscDecimalPlaces))
      //    {
      //      throw new ISBusinessException(MiscUatpErrorCodes.InvalidTotalAddOnChargeAmount);
      //    }
      //}

      // TODO: Validation for CityAirport needs to be change after confirmation: as per IS-XML it is correctable.
      // Validates whether CityAirport exist in master.
      if (!isUpdateOperation || CompareUtil.IsDirty(lineItemInDb.LocationCode, lineItem.LocationCode))
      {
        if (!string.IsNullOrEmpty(lineItem.LocationCode) && !ReferenceManager.IsValidCityAirport(lineItem.LocationCode))
        {
          throw new ISBusinessException(MiscUatpErrorCodes.InvalidCityAirport);
        }
      }


      //if (lineItemInDb != null)
      //{
      //  // Check invoice has updated by other user based on last update On datetime.
      //  if (lineItemInDb.LastUpdatedOn.Ticks > lineItem.RequestStartDateTime.Ticks &&
      //      lineItemInDb.LastUpdatedOn.Ticks < DateTime.UtcNow.Ticks)
      //  {
      //    throw new ISBusinessException(MiscUatpErrorCodes.TransactionUdatedByOtherUser);
      //  }

      //}

      //SCP277476: Validate MISC TAX, VAT and Add on Charge Total amount against its breakdown total
      if (lineItemInDb != null)
      {
        decimal sumofTaxAmountbreakdown = 0M;
        decimal sumofVatAmountbreakdown = 0M;
        decimal sumofAddOnChargeAmountbreakdown = 0M;

        if (lineItem.TaxBreakdown.Count > 0)
        {
          sumofTaxAmountbreakdown = ConvertUtil.Round(lineItem.TaxBreakdown.Where(tax => tax.Type.ToUpper() == "TAX" && tax.CalculatedAmount.HasValue).Sum(invoiceTax => invoiceTax.CalculatedAmount.Value), Constants.MiscDecimalPlaces);

          sumofVatAmountbreakdown = lineItem.TaxBreakdown.Where(tax => tax.Type.ToUpper() == "VAT" && tax.CalculatedAmount.HasValue).Sum(invoiceTax => invoiceTax.CalculatedAmount.Value);
        }

        if (lineItem.AddOnCharges.Count > 0)
        {
          sumofAddOnChargeAmountbreakdown = lineItem.AddOnCharges.Sum(invoiceTax => invoiceTax.Amount);
        }

        var validationResult = ValidateMiscInvoiceBreakDownCaptured(lineItem.Id, 2,
                                                        lineItem.TotalTaxAmount ?? 0,
                                                        sumofTaxAmountbreakdown, sumofVatAmountbreakdown,
                                                        lineItem.TotalVatAmount ?? 0,
                                                        lineItem.TotalAddOnChargeAmount ?? 0,
                                                        sumofAddOnChargeAmountbreakdown);


        if (!string.IsNullOrEmpty(validationResult))
        {
          validationResult = validationResult.Remove(validationResult.Length - 1);

          throw new ISBusinessException("The '" + validationResult + "' of this invoice has already been updated by another user ,please get a latest version of this invoice.");
        }

      }

      // SCP306449: Line Item Detail with Zero charge amount
      if (!lineItem.MinimumQuantityFlag)
      {
        lineItem.ChargeAmount = lineItem.ScalingFactor == 0
                                        ? ConvertUtil.Round(Convert.ToDecimal(0), Constants.MiscDecimalPlaces)
                                        : ConvertUtil.Round(
                                          ((Convert.ToDecimal(lineItem.Quantity) * Convert.ToDecimal(lineItem.UnitPrice)) / Convert.ToDecimal(lineItem.ScalingFactor)),
                                          Constants.MiscDecimalPlaces);
      }
      lineItem.TotalNetAmount = ConvertUtil.Round(
                                      Convert.ToDecimal(Convert.ToDecimal(lineItem.ChargeAmount) + Convert.ToDecimal(lineItem.TotalTaxAmount) +
                                                        Convert.ToDecimal(lineItem.TotalVatAmount) + Convert.ToDecimal(lineItem.TotalAddOnChargeAmount)),
                                      Constants.MiscDecimalPlaces);
     }

        /// <summary>
        /// Validates the line item detail.
        /// </summary>
        /// <param name="lineItemDetail">The line item detail.</param>
        /// <param name="lineItemDetailInDb">The line item detail in db.</param>
        protected virtual void ValidateLineItemDetail(LineItemDetail lineItemDetail, LineItemDetail lineItemDetailInDb)
        {
            if (lineItemDetailInDb == null)
            {
                // If Line Item Detail with same line item detail number exists in a database then update detail number to Max detail number + 1.
        //SCPID 109752 - Issue in incoming MISC Invoice from March4P to Iberia
                if (LineItemDetailRepository.GetCount(itemDetail => itemDetail.LineItemId == lineItemDetail.LineItemId && itemDetail.DetailNumber == lineItemDetail.DetailNumber) > 0)
                {
                    // Line Item Detail number will be set to Max line item detail number + 1.
                    lineItemDetail.DetailNumber = LineItemDetailRepository.Get(lineItemId: lineItemDetail.LineItemId).Max(itemDetail => itemDetail.DetailNumber) + 1;
                }
        else if (lineItemDetail.DetailNumber == 0)
        {
            lineItemDetail.DetailNumber = GetMaxLineItemDetailNumber(lineItemDetail.LineItemId) + 1;
        }
            }

            //if (lineItemDetail.TaxBreakdown.Count > 0)
      //{
      //  // TotalTaxAmount should be equal to sum of Calculated amount in TaxBreakdown
      //  if (lineItemDetail.TotalTaxAmount != null)
      //    if (ConvertUtil.Round(lineItemDetail.TotalTaxAmount.Value, Constants.MiscDecimalPlaces) != ConvertUtil.Round(lineItemDetail.TaxBreakdown.Where(tax => tax.Type.ToUpper() == "TAX" && tax.CalculatedAmount.HasValue).Sum(invoiceTax => invoiceTax.CalculatedAmount.Value), Constants.MiscDecimalPlaces))
      //    {
      //      throw new ISBusinessException(MiscUatpErrorCodes.InvalidTotalTaxAmount);
      //    }

      //  // TotalVatAmount should be equal to sum of Calculated amount in VatBreakdown
      //  if (lineItemDetail.TotalVatAmount != null)
      //    if (ConvertUtil.Round(lineItemDetail.TotalVatAmount.Value, Constants.MiscDecimalPlaces) != ConvertUtil.Round(lineItemDetail.TaxBreakdown.Where(tax => tax.Type.ToUpper() == "VAT" && tax.CalculatedAmount.HasValue).Sum(invoiceTax => invoiceTax.CalculatedAmount.Value), Constants.MiscDecimalPlaces))
      //    {
      //      throw new ISBusinessException(MiscUatpErrorCodes.InvalidTotalVatAmount);
      //    }
      //}

      //// TotalVatAmount should be equal to sum of Calculated amount in VatBreakdown
      //if (lineItemDetail.AddOnCharges.Count > 0)
      //{
      //  if (lineItemDetail.TotalAddOnChargeAmount != null)
      //    if (ConvertUtil.Round(lineItemDetail.TotalAddOnChargeAmount.Value, Constants.MiscDecimalPlaces) != ConvertUtil.Round(lineItemDetail.AddOnCharges.Sum(addonCharge => addonCharge.Amount), Constants.MiscDecimalPlaces))
      //    {
      //      throw new ISBusinessException(MiscUatpErrorCodes.InvalidTotalAddOnChargeAmount);
      //    }
      //}

     //SCP277476: Validate MISC TAX, VAT and Add on Charge Total amount against its breakdown total
     if (lineItemDetailInDb != null)
     {
       decimal sumofTaxAmountbreakdown = 0M;
       decimal sumofVatAmountbreakdown = 0M;
       decimal sumofAddOnChargeAmountbreakdown = 0M;

       if (lineItemDetail.TaxBreakdown.Count > 0)
       {
         sumofTaxAmountbreakdown =
           ConvertUtil.Round(
             lineItemDetail.TaxBreakdown.Where(tax => tax.Type.ToUpper() == "TAX" && tax.CalculatedAmount.HasValue).Sum(
               invoiceTax => invoiceTax.CalculatedAmount.Value), Constants.MiscDecimalPlaces);

         sumofVatAmountbreakdown =
           lineItemDetail.TaxBreakdown.Where(tax => tax.Type.ToUpper() == "VAT" && tax.CalculatedAmount.HasValue).Sum(
             invoiceTax => invoiceTax.CalculatedAmount.Value);
       }

       if (lineItemDetail.AddOnCharges.Count > 0)
       {
         sumofAddOnChargeAmountbreakdown = lineItemDetail.AddOnCharges.Sum(invoiceTax => invoiceTax.Amount);
       }

       var validationResult = ValidateMiscInvoiceBreakDownCaptured(lineItemDetail.Id, 3,
                                                                   lineItemDetail.TotalTaxAmount ?? 0,
                                                                   sumofTaxAmountbreakdown, sumofVatAmountbreakdown,
                                                                   lineItemDetail.TotalVatAmount ?? 0,
                                                                   lineItemDetail.TotalAddOnChargeAmount ?? 0,
                                                                   sumofAddOnChargeAmountbreakdown);


       if (!string.IsNullOrEmpty(validationResult))
       {
         validationResult = validationResult.Remove(validationResult.Length - 1);

         throw new ISBusinessException("The '" + validationResult +
                                       "' of this invoice has already been updated by another user ,please get a latest version of this invoice.");
       }

     }

     // SCP306449: Line Item Detail with Zero charge amount
     if (!lineItemDetail.MinimumQuantityFlag)
     {
       lineItemDetail.ChargeAmount = lineItemDetail.ScalingFactor == 0
                                       ? ConvertUtil.Round(Convert.ToDecimal(0), Constants.MiscDecimalPlaces)
                                       : ConvertUtil.Round(
                                         ((Convert.ToDecimal(lineItemDetail.Quantity) * Convert.ToDecimal(lineItemDetail.UnitPrice)) / Convert.ToDecimal(lineItemDetail.ScalingFactor)),
                                         Constants.MiscDecimalPlaces);
     }
     lineItemDetail.TotalNetAmount = ConvertUtil.Round(
                                     Convert.ToDecimal(Convert.ToDecimal(lineItemDetail.ChargeAmount) + Convert.ToDecimal(lineItemDetail.TotalTaxAmount) +
                                                       Convert.ToDecimal(lineItemDetail.TotalVatAmount) + Convert.ToDecimal(lineItemDetail.TotalAddOnChargeAmount)),
                                     Constants.MiscDecimalPlaces);
    }

        /// <summary>
        /// This will return the all the metadata in the database 
        /// </summary>
        /// <returns></returns>
        public List<FieldMetaData> GetFieldMetadata()
        {
            return FRepository.GetAll().ToList();
        }

        /// <summary>
        /// Gets the billing period.
        /// </summary>
        /// <param name="invoiceNumber">The invoice number.</param>
        /// <param name="billingMemberId">The billing member id.</param>
        /// <param name="billedMemberId">The billed member id.</param>
        /// <returns></returns>
        public BillingPeriod GetInvoiceBillingPeriod(string invoiceNumber, int billingMemberId, int billedMemberId)
        {
            // Replaced with LoadStrategy call
            var invoice = MiscUatpInvoiceRepository.Single(invoiceNumber: invoiceNumber, billingMemberId: billingMemberId, billedMemberId: billedMemberId);
            //    var invoice = MiscUatpInvoiceRepository.Single(inv => inv.InvoiceNumber == invoiceNumber
            //                                                   && inv.BillingMemberId == billingMemberId && inv.BilledMemberId == billedMemberId);

            return invoice.InvoiceBillingPeriod;
        }

        /// <summary>
        /// Retrieves Rejected invoice number for the billed member and validates against settlement method.
        /// </summary>
        /// <param name="invoiceNumber">The invoice number.</param>
        /// <param name="smi">The settlement method.</param>
        /// <param name="billingMemberId">The billing member id.</param>
        /// <param name="billedMemberId">The billed member id.</param>
        /// <param name="settlementMonth">Settlement Month.</param>
        /// <param name="settlementYear">Settlement Year</param>
        /// <param name="settlementPeriod">Settlement Period</param>
        /// <returns>
        /// Billing rejected invoice billing period if found
        /// </returns>
        public RejectedInvoiceDetails GetRejectedInvoiceDetails(string invoiceNumber, int smi,
                                                       int billingMemberId, int billedMemberId, int? settlementMonth, int? settlementYear, int? settlementPeriod)
        {
            var rejectedInvoiceDetails = new RejectedInvoiceDetails
            {
                RejectionStage = 0
            };

            // Replaced with LoadStrategy call
            var rejectedInvoice = MiscUatpInvoiceRepository.Single(invoiceNumber: invoiceNumber,
                                                                   billingMemberId: billingMemberId,
                                                                   billedMemberId: billedMemberId,
                                                                   billingCategoryId: (int)BillingCategory,
                                                                   billingMonth: settlementMonth,
                                                                   billingPeriod: settlementPeriod,
                                                                   billingYear: settlementYear,
                                                                   invoiceStatusId: (int)InvoiceStatusType.Presented);

            /*  var rejectedInvoice = MiscUatpInvoiceRepository.Single(
                rejectionInvoice =>
                rejectionInvoice.InvoiceNumber == invoiceNumber
                && rejectionInvoice.BillingMemberId == billingMemberId
                && rejectionInvoice.BilledMemberId == billedMemberId
                && rejectionInvoice.BillingCategoryId == (int)BillingCategory
                && rejectionInvoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForBilling);*/

            // Rejection invoice not found and original invoice not found.
            if (rejectedInvoice == null)
            {
                rejectedInvoiceDetails.DisableBillingCurrency = false;
                return rejectedInvoiceDetails;
            }

            rejectedInvoiceDetails.BillingPeriod = rejectedInvoice.InvoiceBillingPeriod;

            if (rejectedInvoice.InvoiceType == InvoiceType.CorrespondenceInvoice)
            {
                rejectedInvoiceDetails.ErrorMessage = Messages.ResourceManager.GetString(MiscUatpErrorCodes.CorrespondenceInvoiceCannotBeRejected);
                return rejectedInvoiceDetails;
            }

            if (rejectedInvoice.InvoiceType == InvoiceType.CreditNote)
            {
                rejectedInvoiceDetails.AlertMessage = Messages.ResourceManager.GetString(MiscUatpErrorCodes.CreditNoteRejectionMessage);
            }

            // Rejection Stage #1
            if (rejectedInvoice.RejectionStage == 0)
            {
                rejectedInvoiceDetails.CurrentBilledIn = rejectedInvoice.BillingYear + "-" + rejectedInvoice.BillingMonth;
                if (rejectedInvoice.BillingCurrency != null)
                {
                    rejectedInvoiceDetails.CurrentBillingCurrencyCode = rejectedInvoice.ListingCurrency.Id;
                    rejectedInvoiceDetails.DisableBillingCurrency = true;
                }
            }

            // Stage #2 rejection.
            if (!string.IsNullOrEmpty(rejectedInvoice.RejectedInvoiceNumber) && rejectedInvoice.RejectionStage == 1)
            {
                // Replaced with LoadStrategy call
                var originalInvoice =
                  MiscUatpInvoiceRepository.Single(invoiceNumber: rejectedInvoice.RejectedInvoiceNumber, billingMemberId: billedMemberId, billedMemberId: billingMemberId, billingCategoryId: (int)BillingCategory);

                /* var originalInvoice =
                   MiscUatpInvoiceRepository.Single(
                     invoice =>
                     invoice.InvoiceNumber == rejectedInvoice.RejectedInvoiceNumber && invoice.BillingMemberId == billedMemberId && invoice.BilledMemberId == billingMemberId &&
                     invoice.BillingCategoryId == (int)BillingCategory);*/

                // Rejection invoice found and original invoice also found 
                if (originalInvoice != null)
                {
                    rejectedInvoiceDetails.CurrentBilledIn = originalInvoice.BillingYear + "-" + originalInvoice.BillingMonth;
                    if (originalInvoice.BillingCurrency != null)
                    {
                        rejectedInvoiceDetails.CurrentBillingCurrencyCode = originalInvoice.ListingCurrency.Id;
                        rejectedInvoiceDetails.DisableBillingCurrency = true;
                    }
                }
                else //Original Invoice not found in database but Rejection 1 exists- Stage 2 Rejection
                {
                    // FDR based rejection invoice BillingIn
                    rejectedInvoiceDetails.CurrentBilledIn = rejectedInvoice.SettlementYear + "-" + rejectedInvoice.SettlementMonth;
                    if (rejectedInvoice.BillingCurrency != null)
                    {
                        // Billing currency of rejection invoice billing currency and editable.
                        rejectedInvoiceDetails.CurrentBillingCurrencyCode = rejectedInvoice.ListingCurrency.Id;
                    }
                }
            }

            //SMI M will have rules same as ICH
            /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
            if ((smi == Convert.ToInt32(SMI.Ich) || ReferenceManager.IsSmiLikeBilateral(smi, true) || smi == Convert.ToInt32(SMI.AchUsingIataRules)) && rejectedInvoice.InvoiceType != InvoiceType.RejectionInvoice) //allowed invoice types - original, correspondence invoices
            {
                rejectedInvoiceDetails.RejectionStage = 1;
                rejectedInvoiceDetails.CurrentBilledIn = rejectedInvoice.BillingYear + "-" + rejectedInvoice.BillingMonth;
            }
            else if ((smi == Convert.ToInt32(SMI.Ich) || smi == Convert.ToInt32(SMI.AdjustmentDueToProtest) || ReferenceManager.IsSmiLikeBilateral(smi, true) || smi == Convert.ToInt32(SMI.AchUsingIataRules)) && rejectedInvoice.InvoiceType == InvoiceType.RejectionInvoice)
            {
                rejectedInvoiceDetails.ErrorMessage = "You can not reject this invoice further.";
                return rejectedInvoiceDetails;
            }
            else if (smi == Convert.ToInt32(SMI.Ach) && rejectedInvoice.InvoiceType != InvoiceType.RejectionInvoice)
            {
                rejectedInvoiceDetails.RejectionStage = 1;
                rejectedInvoiceDetails.CurrentBilledIn = rejectedInvoice.BillingYear + "-" + rejectedInvoice.BillingMonth;
            }
            else if (smi == Convert.ToInt32(SMI.Ach) && rejectedInvoice.InvoiceType == InvoiceType.RejectionInvoice && rejectedInvoice.RejectionStage == (int)RejectionStage.StageOne)
            {
                rejectedInvoiceDetails.RejectionStage = 2;
            }
            else if (smi == Convert.ToInt32(SMI.Ach) && rejectedInvoice.InvoiceType == InvoiceType.RejectionInvoice && rejectedInvoice.RejectionStage == (int)RejectionStage.StageTwo)
            {
                rejectedInvoiceDetails.ErrorMessage = "You can not reject this invoice further.";
                return rejectedInvoiceDetails;
            }


            return rejectedInvoiceDetails;
        }

        #region Old function ValidateParsedExchangeRate - Commented

        /// <summary>
        /// Validates the parsed exchange rate.
        /// </summary>
        /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
        /// <param name="exceptionDetailsList">The exception details list.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileSubmissionDate">The file submission date.</param>
        /// <param name="clearingHouseEnum"></param>
        /// <param name="currentBillingPeriod"></param>
        //private void ValidateParsedExchangeRate(MiscUatpInvoice miscUatpInvoice, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate, ClearingHouse clearingHouseEnum, BillingPeriod currentBillingPeriod)
        //{

        //    /* CMP #624: Validate exchange rate for SMi X */
        //    if (miscUatpInvoice.SettlementMethodId == (int)SMI.IchSpecialAgreement)
        //    {
        //        if (miscUatpInvoice.ListingCurrencyId == miscUatpInvoice.BillingCurrencyId && miscUatpInvoice.ExchangeRate == 1)
        //        {
        //            // do not require any processing
        //        }
        //        else if (miscUatpInvoice.ListingCurrencyId == miscUatpInvoice.BillingCurrencyId && miscUatpInvoice.ExchangeRate != 1)
        //        {
        //            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1,
        //                                                                            fileSubmissionDate,
        //                                                                            miscUatpInvoice,
        //                                                                            "Exchange Rate",
        //                                                                            miscUatpInvoice.ExchangeRate.ToString(),
        //                                                                            fileName,
        //                                                                            ErrorLevels.ErrorLevelInvoice,
        //                                                                            ErrorCodes.InvalidListingToBillingRateForSameCurrencies,
        //                                                                            ErrorStatus.X,
        //                                                                            0,
        //                                                                            0);
        //            exceptionDetailsList.Add(validationExceptionDetail);
        //        }
        //        // Update Amount in clearance currency
        //        if (miscUatpInvoice.InvoiceSummary != null && !miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //        {
        //            miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //        }
        //        return;
        //    }

        //    //var currentBillingPeriod = CalendarManager.GetCurrentBillingPeriod();

        //    if (miscUatpInvoice.InvoiceType == InvoiceType.Invoice)
        //    {
        //        /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
        //        if (!ReferenceManager.IsSmiLikeBilateral(miscUatpInvoice.SettlementMethodId, true))
        //        {
        //            if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
        //            {
        //                if (miscUatpInvoice.ListingCurrencyId == miscUatpInvoice.BillingCurrencyId && miscUatpInvoice.ExchangeRate == 1)
        //                {
        //                    // do not require any processing
        //                }
        //                else if (miscUatpInvoice.ListingCurrencyId == miscUatpInvoice.BillingCurrencyId && miscUatpInvoice.ExchangeRate != 1)
        //                {
        //                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1,
        //                                                                                    fileSubmissionDate,
        //                                                                                    miscUatpInvoice,
        //                                                                                    "Exchange Rate",
        //                                                                                    miscUatpInvoice.ExchangeRate.ToString(),
        //                                                                                    fileName,
        //                                                                                    ErrorLevels.ErrorLevelInvoice,
        //                                                                                    ErrorCodes.InvalidListingToBillingRateForSameCurrencies,
        //                                                                                    ErrorStatus.X,
        //                                                                                    0,
        //                                                                                    0);
        //                    exceptionDetailsList.Add(validationExceptionDetail);
        //                }
        //                else if (!IsValidExchangeRate(miscUatpInvoice, exceptionDetailsList))
        //                {
        //                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1,
        //                                                                                    fileSubmissionDate,
        //                                                                                    miscUatpInvoice,
        //                                                                                    "Exchange Rate",
        //                                                                                    Convert.ToString(miscUatpInvoice.ExchangeRate),
        //                                                                                    fileName,
        //                                                                                    ErrorLevels.ErrorLevelInvoice,
        //                                                                                    ErrorCodes.InvalidListingToBillingRate,
        //                                                                                    ErrorStatus.X,
        //                                                                                    0,
        //                                                                                    0);
        //                    exceptionDetailsList.Add(validationExceptionDetail);
        //                }
        //            }
        //            else // Exchange Rate not given in input file.
        //            {
        //                if (miscUatpInvoice.ListingCurrencyId != null)
        //                {
        //                    if (miscUatpInvoice.BillingCurrency != null)
        //                    {
        //                        var exchangeRate = GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value, miscUatpInvoice.BillingCurrency.Value, miscUatpInvoice.BillingYear, miscUatpInvoice.BillingMonth);
        //                        miscUatpInvoice.ExchangeRate = Convert.ToDecimal(exchangeRate);
        //                    }
        //                }

        //            }

        //            // Update Amount in clearance currency
        //            if (miscUatpInvoice.InvoiceSummary != null && !miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //            {
        //                miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //            }
        //        }
        //    }

        //    else if (miscUatpInvoice.InvoiceType == InvoiceType.CreditNote)
        //    {
        //        // Exchange rate for credit note.
        //        decimal exchangeRate = 0;
        //        /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateral */
        //        if (!ReferenceManager.IsSmiLikeBilateral(miscUatpInvoice.SettlementMethodId, true))
        //        {
        //            if (miscUatpInvoice.ListingCurrencyId != null)
        //            {
        //                if (miscUatpInvoice.BillingCurrency != null)
        //                {
        //                    exchangeRate =
        //                      Convert.ToDecimal(GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value, miscUatpInvoice.BillingCurrency.Value, miscUatpInvoice.BillingYear, miscUatpInvoice.BillingMonth));
        //                }
        //            }
        //            if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
        //            {

        //                if (miscUatpInvoice.ListingCurrencyId == miscUatpInvoice.BillingCurrencyId && miscUatpInvoice.ExchangeRate != 1)
        //                {
        //                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1,
        //                                                                                    fileSubmissionDate,
        //                                                                                    miscUatpInvoice,
        //                                                                                    "Exchange Rate",
        //                                                                                    miscUatpInvoice.ExchangeRate.ToString(),
        //                                                                                    fileName,
        //                                                                                    ErrorLevels.ErrorLevelInvoice,
        //                                                                                    ErrorCodes.InvalidListingToBillingRateForSameCurrencies,
        //                                                                                    ErrorStatus.X,
        //                                                                                    0,
        //                                                                                    0);
        //                    exceptionDetailsList.Add(validationExceptionDetail);
        //                }

        //                miscUatpInvoice.IsValidationFlag = ExchangeRateValidationFlag;
        //            }
        //            else
        //            {
        //                miscUatpInvoice.ExchangeRate = Convert.ToDecimal(exchangeRate);
        //                miscUatpInvoice.IsValidationFlag = ExchangeRateValidationFlag;
        //            }

        //            // Update Amount in clearance currency
        //            if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //            {
        //                miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //            }
        //        }
        //    }
        //    else if (miscUatpInvoice.InvoiceType == InvoiceType.RejectionInvoice)
        //    {
        //        MiscUatpInvoice originalInvoice;
        //        MiscUatpInvoice rejectedInvoice1;

        //        if (miscUatpInvoice.RejectionStage == 1)
        //        {
        //            //Fetch original invoice
        //            originalInvoice = MiscUatpInvoiceRepository.Single(invoiceNumber: miscUatpInvoice.RejectedInvoiceNumber, billingMemberId: miscUatpInvoice.BilledMemberId, billedMemberId: miscUatpInvoice.BillingMemberId,
        //            billingCategoryId: miscUatpInvoice.BillingCategoryId, invoiceStatusId: (int)InvoiceStatusType.Presented, billingPeriod: miscUatpInvoice.SettlementPeriod, billingMonth: miscUatpInvoice.SettlementMonth, billingYear: miscUatpInvoice.SettlementYear);

        //            // If original invoice found , then use its billing month and year for exchange rate fetch.
        //            if (originalInvoice != null)
        //            {
        //                if (miscUatpInvoice.ListingCurrencyId != null && miscUatpInvoice.BillingCurrency != null)
        //                {

        //                    var exchangeRate =
        //                      Convert.ToDecimal(GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value, miscUatpInvoice.BillingCurrency.Value, originalInvoice.BillingYear, originalInvoice.BillingMonth));

        //                    // Rejection invoice found and exchange rate given in input file.
        //                    if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
        //                    {
        //                        if (!CompareUtil.Compare(exchangeRate, miscUatpInvoice.ExchangeRate, 0D, Constants.ExchangeRateDecimalPlaces))
        //                        {
        //                            //miscUatpInvoice.IsValidationFlag = ExchangeRateValidationFlag;

        //                            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate",
        //                                                                                          Convert.ToString(miscUatpInvoice.ExchangeRate),
        //                                                                                          fileName,
        //                                                                                          ErrorLevels.ErrorLevelInvoice,
        //                                                                                          ErrorCodes.InvalidListingToBillingRate,
        //                                                                                          ErrorStatus.X);
        //                            exceptionDetailsList.Add(validationExceptionDetail);
        //                        }
        //                        else
        //                        {
        //                            // Update Amount in clearance currency
        //                            if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                            {
        //                                miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                            }
        //                        }
        //                    }
        //                    else // Exchange rate not given in input file.
        //                    {
        //                        miscUatpInvoice.ExchangeRate = exchangeRate;

        //                        // Update Amount in clearance currency
        //                        if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                        {
        //                            miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                        }
        //                    }
        //                }
        //            }
        //            else  // If original invoice not found.
        //            {
        //                // Use settlement year , month in rejection details.
        //                if (miscUatpInvoice.ListingCurrencyId != null && miscUatpInvoice.BillingCurrency != null)
        //                {
        //                    var exchangeRate = Convert.ToDecimal(GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value, miscUatpInvoice.BillingCurrency.Value, miscUatpInvoice.SettlementYear, miscUatpInvoice.SettlementMonth));

        //                    // Rejection invoice found and exchange rate given in input file.
        //                    if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
        //                    {
        //                        if (!CompareUtil.Compare(exchangeRate, miscUatpInvoice.ExchangeRate, 0D, Constants.ExchangeRateDecimalPlaces))
        //                        {
        //                            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate", Convert.ToString(miscUatpInvoice.ExchangeRate), fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidListingToBillingRate, ErrorStatus.X);
        //                            exceptionDetailsList.Add(validationExceptionDetail);
        //                        }
        //                        else
        //                        {
        //                            // Update Amount in clearance currency
        //                            if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                            {
        //                                miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                            }
        //                        }
        //                    }
        //                    else // Exchange rate not given in input file.
        //                    {
        //                        miscUatpInvoice.ExchangeRate = exchangeRate;

        //                        // Update Amount in clearance currency
        //                        if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                        {
        //                            miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                        }
        //                    }

        //                    // Update Amount in clearance currency
        //                    if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                    {
        //                        miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                    }
        //                }
        //            }
        //        }
        //        else if (miscUatpInvoice.RejectionStage == 2)  // If rejection stage is 2 
        //        {
        //            // Get rejected invoice from invoice number from rejection details.
        //            // Added Settlement Period search parameter in additional to fetch the rejected invoice
        //            // Author : Vinod Patil
        //            // SCPID : 50966
        //            rejectedInvoice1 = MiscUatpInvoiceRepository.Single(invoiceNumber: miscUatpInvoice.RejectedInvoiceNumber,
        //                                                                billingMemberId: miscUatpInvoice.BilledMemberId,
        //                                                                billedMemberId: miscUatpInvoice.BillingMemberId,
        //                                                                billingCategoryId: miscUatpInvoice.BillingCategoryId,
        //                                                                invoiceStatusId: (int)InvoiceStatusType.Presented,
        //                                                                billingPeriod: miscUatpInvoice.SettlementPeriod,
        //                                                                billingMonth: miscUatpInvoice.SettlementMonth,
        //                                                                billingYear: miscUatpInvoice.SettlementYear);

        //            if (rejectedInvoice1 != null)
        //            {

        //                //Fetch original invoice
        //                originalInvoice = MiscUatpInvoiceRepository.Single(invoiceNumber: rejectedInvoice1.RejectedInvoiceNumber,
        //                                                               billingMemberId: rejectedInvoice1.BilledMemberId,
        //                                                               billedMemberId: rejectedInvoice1.BillingMemberId,
        //                                                               billingCategoryId: rejectedInvoice1.BillingCategoryId,
        //                                                               invoiceStatusId: (int)InvoiceStatusType.Presented);


        //                // If original invoice found , then use its billing month and year for exchange rate fetch
        //                if (originalInvoice != null)
        //                {
        //                    var exchangeRate =
        //                      Convert.ToDecimal(GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value, miscUatpInvoice.BillingCurrency.Value, originalInvoice.BillingYear, originalInvoice.BillingMonth));

        //                    // Rejection invoice found and exchange rate given.
        //                    if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
        //                    {
        //                        if (!CompareUtil.Compare(exchangeRate, miscUatpInvoice.ExchangeRate, 0D, Constants.ExchangeRateDecimalPlaces))
        //                        {
        //                            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate",
        //                                                                                          Convert.ToString(miscUatpInvoice.ExchangeRate),
        //                                                                                          fileName,
        //                                                                                          ErrorLevels.ErrorLevelInvoice,
        //                                                                                          ErrorCodes.InvalidListingToBillingRate,
        //                                                                                          ErrorStatus.X);
        //                            exceptionDetailsList.Add(validationExceptionDetail);
        //                        }
        //                        else
        //                        {
        //                            // Update Amount in clearance currency
        //                            if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                            {
        //                                miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        miscUatpInvoice.ExchangeRate = exchangeRate;

        //                        // Update Amount in clearance currency
        //                        if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                        {
        //                            miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                        }
        //                    }
        //                }
        //                else //// If original invoice  not found 
        //                {
        //                    if (miscUatpInvoice.ListingCurrencyId != null && miscUatpInvoice.BillingCurrency != null)
        //                    {
        //                        var exchangeRate =
        //                          Convert.ToDecimal(GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value,
        //                                                                   miscUatpInvoice.BillingCurrency.Value,
        //                                                                   miscUatpInvoice.BillingYear,
        //                                                                   miscUatpInvoice.BillingMonth));


        //                        // Rejection invoice found and exchange rate given.
        //                        if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
        //                        {
        //                            if (!CompareUtil.Compare(exchangeRate, miscUatpInvoice.ExchangeRate, 0D, Constants.ExchangeRateDecimalPlaces))
        //                            {
        //                                var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate",
        //                                                                                              Convert.ToString(miscUatpInvoice.ExchangeRate),
        //                                                                                              fileName,
        //                                                                                              ErrorLevels.ErrorLevelInvoice,
        //                                                                                              ErrorCodes.InvalidListingToBillingRate,
        //                                                                                              ErrorStatus.X);
        //                                exceptionDetailsList.Add(validationExceptionDetail);
        //                            }
        //                            else
        //                            {
        //                                // Update Amount in clearance currency
        //                                if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                                {
        //                                    miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            miscUatpInvoice.ExchangeRate = exchangeRate;

        //                            // Update Amount in clearance currency
        //                            if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                            {
        //                                miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency =
        //                                  miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            else // R1 invoice not found , then use current fdr exchange rate will be fetched.
        //            {
        //                if (miscUatpInvoice.ListingCurrencyId != null && miscUatpInvoice.BillingCurrency != null)
        //                {
        //                    // Rejection invoice found and exchange rate given.
        //                    if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
        //                    {
        //                        miscUatpInvoice.IsValidationFlag = ExchangeRateValidationFlag;
        //                        // Update Amount in clearance currency
        //                        if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                        {
        //                            miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency =
        //                              miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                        }
        //                    }
        //                    else
        //                    {

        //                        miscUatpInvoice.IsValidationFlag = ExchangeRateValidationFlag;

        //                        var exchangeRate =
        //                        Convert.ToDecimal(GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value,
        //                                                                 miscUatpInvoice.BillingCurrency.Value,
        //                                                                 miscUatpInvoice.BillingYear, miscUatpInvoice.BillingMonth));

        //                        miscUatpInvoice.ExchangeRate = exchangeRate;

        //                        // Update Amount in clearance currency
        //                        if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                        {
        //                            miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency =
        //                              miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else if (miscUatpInvoice.InvoiceType == InvoiceType.CorrespondenceInvoice && miscUatpInvoice.CorrespondenceRefNo != null)
        //    {
        //        MiscUatpInvoice originalInvoice;
        //        MiscUatpInvoice rejectedInvoice1;

        //        var miscCorrespondence =
        //          MiscCorrespondenceRepository.Get(correspondenceNumber: miscUatpInvoice.CorrespondenceRefNo).OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();

        //        if (miscCorrespondence != null)
        //        {

        //            rejectedInvoice1 = MiscUatpInvoiceRepository.Single(invoiceId: miscCorrespondence.InvoiceId, billingCategoryId: (int)BillingCategory);

        //            if (rejectedInvoice1.RejectionStage == 1)
        //            {
        //                //Fetch original invoice
        //                //Fetch original invoice
        //                originalInvoice = MiscUatpInvoiceRepository.Single(invoiceNumber: rejectedInvoice1.RejectedInvoiceNumber, billingMemberId: rejectedInvoice1.BilledMemberId, billedMemberId: rejectedInvoice1.BillingMemberId, billingCategoryId: miscUatpInvoice.BillingCategoryId, invoiceStatusId: (int)InvoiceStatusType.Presented);

        //                // If original invoice found , then use its billing month and year for exchange rate fetch.
        //                if (originalInvoice != null)
        //                {

        //                    var exchangeRate = Convert.ToDecimal(GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value, miscUatpInvoice.BillingCurrency.Value, originalInvoice.BillingYear, originalInvoice.BillingMonth));

        //                    // Rejection invoice found and exchange rate given in input file.
        //                    if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
        //                    {
        //                        if (!CompareUtil.Compare(exchangeRate, miscUatpInvoice.ExchangeRate, 0D, Constants.ExchangeRateDecimalPlaces))
        //                        {

        //                            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate",
        //                                                                                            Convert.ToString(miscUatpInvoice.ExchangeRate),
        //                                                                                            fileName,
        //                                                                                            ErrorLevels.ErrorLevelInvoice,
        //                                                                                            ErrorCodes.InvalidListingToBillingRate,
        //                                                                                            ErrorStatus.X);
        //                            exceptionDetailsList.Add(validationExceptionDetail);
        //                        }
        //                        else
        //                        {
        //                            // Update Amount in clearance currency
        //                            if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                            {
        //                                miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency =
        //                                  miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                            }
        //                        }
        //                    }
        //                    else // Exchange rate not given in input file.
        //                    {
        //                        miscUatpInvoice.ExchangeRate = exchangeRate;

        //                        // Update Amount in clearance currency
        //                        if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                        {
        //                            miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                        }
        //                    }
        //                }
        //                else // If original invoice not found.
        //                {
        //                    // Use settlement year , month in rejection details.
        //                    var exchangeRate = Convert.ToDecimal(GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value, miscUatpInvoice.BillingCurrency.Value, rejectedInvoice1.SettlementYear, rejectedInvoice1.SettlementMonth));


        //                    // Rejection invoice found and exchange rate given in input file.
        //                    if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
        //                    {
        //                        if (!CompareUtil.Compare(exchangeRate, miscUatpInvoice.ExchangeRate, 0D, Constants.ExchangeRateDecimalPlaces))
        //                        {

        //                            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate",
        //                                                                                            Convert.ToString(miscUatpInvoice.ExchangeRate),
        //                                                                                            fileName,
        //                                                                                            ErrorLevels.ErrorLevelInvoice,
        //                                                                                            ErrorCodes.InvalidListingToBillingRate,
        //                                                                                            ErrorStatus.X);
        //                            exceptionDetailsList.Add(validationExceptionDetail);
        //                        }
        //                        else
        //                        {
        //                            // Update Amount in clearance currency
        //                            if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                            {
        //                                miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency =
        //                                  miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                            }
        //                        }
        //                    }
        //                    else // Exchange rate not given in input file.
        //                    {
        //                        miscUatpInvoice.ExchangeRate = exchangeRate;

        //                        // Update Amount in clearance currency
        //                        if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                        {
        //                            miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                        }
        //                    }
        //                }
        //            }
        //            else if (rejectedInvoice1.RejectionStage == 2) // If rejection stage is 2 
        //            {
        //                // Get rejected invoice from invoice number from rejection details.
        //                MiscUatpInvoice rejectionInvoiceStageOne;
        //                rejectionInvoiceStageOne = MiscUatpInvoiceRepository.Single(invoiceNumber: miscUatpInvoice.RejectedInvoiceNumber, billingMemberId: rejectedInvoice1.BilledMemberId, billedMemberId: rejectedInvoice1.BillingMemberId, billingCategoryId: miscUatpInvoice.BillingCategoryId, invoiceStatusId: (int)InvoiceStatusType.Presented);

        //                if (rejectionInvoiceStageOne != null)
        //                {

        //                    //Fetch original invoice
        //                    originalInvoice = MiscUatpInvoiceRepository.Single(invoiceNumber: rejectionInvoiceStageOne.RejectedInvoiceNumber, billingMemberId: rejectionInvoiceStageOne.BilledMemberId, billedMemberId: rejectionInvoiceStageOne.BillingMemberId, billingCategoryId: rejectionInvoiceStageOne.BillingCategoryId, invoiceStatusId: (int)InvoiceStatusType.Presented);

        //                    // If original invoice found , then use its billing month and year for exchange rate fetch
        //                    if (originalInvoice != null)
        //                    {

        //                        var exchangeRate = Convert.ToDecimal(GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value, miscUatpInvoice.BillingCurrency.Value, originalInvoice.BillingYear, originalInvoice.BillingMonth));

        //                        // Rejection invoice found and exchange rate given.
        //                        if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
        //                        {
        //                            if (!CompareUtil.Compare(exchangeRate, miscUatpInvoice.ExchangeRate, 0D, Constants.ExchangeRateDecimalPlaces))
        //                            {
        //                                var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate",
        //                                                                                              Convert.ToString(miscUatpInvoice.ExchangeRate),
        //                                                                                              fileName,
        //                                                                                              ErrorLevels.ErrorLevelInvoice,
        //                                                                                              ErrorCodes.InvalidListingToBillingRate,
        //                                                                                              ErrorStatus.X);
        //                                exceptionDetailsList.Add(validationExceptionDetail);
        //                            }
        //                            else
        //                            {
        //                                // Update Amount in clearance currency
        //                                if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                                {
        //                                    miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency =
        //                                      miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            miscUatpInvoice.ExchangeRate = exchangeRate;

        //                            // Update Amount in clearance currency
        //                            if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                            {
        //                                miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                            }
        //                        }
        //                    }
        //                    else //// If original invoice not found 
        //                    {
        //                        var exchangeRate = Convert.ToDecimal(GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value, miscUatpInvoice.BillingCurrency.Value, miscUatpInvoice.BillingYear, miscUatpInvoice.BillingMonth));


        //                        // Rejection invoice found and exchange rate given.
        //                        if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
        //                        {
        //                            if (!CompareUtil.Compare(exchangeRate, miscUatpInvoice.ExchangeRate, 0D, Constants.ExchangeRateDecimalPlaces))
        //                            {
        //                                var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate",
        //                                                                                              Convert.ToString(miscUatpInvoice.ExchangeRate),
        //                                                                                              fileName,
        //                                                                                              ErrorLevels.ErrorLevelInvoice,
        //                                                                                              ErrorCodes.InvalidListingToBillingRate,
        //                                                                                              ErrorStatus.X);
        //                                exceptionDetailsList.Add(validationExceptionDetail);
        //                            }
        //                            else
        //                            {
        //                                // Update Amount in clearance currency
        //                                if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                                {
        //                                    miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency =
        //                                      miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            miscUatpInvoice.ExchangeRate = exchangeRate;

        //                            // Update Amount in clearance currency
        //                            if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                            {
        //                                miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                            }
        //                        }
        //                    }
        //                }
        //                else // R1 invoice not found , then use current fdr exchange rate will be fetched.
        //                {

        //                    // Rejection invoice found and exchange rate given.
        //                    if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
        //                    {
        //                        miscUatpInvoice.IsValidationFlag = ExchangeRateValidationFlag;
        //                        // Update Amount in clearance currency
        //                        if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                        {
        //                            miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency =
        //                              miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        miscUatpInvoice.IsValidationFlag = ExchangeRateValidationFlag;

        //                        var exchangeRate = Convert.ToDecimal(GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value, miscUatpInvoice.BillingCurrency.Value, miscUatpInvoice.BillingYear, miscUatpInvoice.BillingMonth));
        //                        miscUatpInvoice.ExchangeRate = exchangeRate;

        //                        // Update Amount in clearance currency
        //                        if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate != 0)
        //                        {
        //                            miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        #endregion

        

        public BillingPeriod GetRejectedInvoiceDetails(string invoiceNumber, int smi,
                                                       int billingMemberId, int billedMemberId, out int rejectionStage, out bool isRejectionAllowed)
        {
            isRejectionAllowed = false;
            var billingPeriod = new BillingPeriod();
            rejectionStage = 0;

            // Replaced with LoadStrategy call
            var rejectedInvoice = MiscUatpInvoiceRepository.Single(invoiceNumber: invoiceNumber, billingMemberId: billingMemberId, billedMemberId: billedMemberId,
            billingCategoryId: (int)BillingCategory, invoiceStatusId: (int)InvoiceStatusType.Presented);

            /*  var rejectedInvoice = MiscUatpInvoiceRepository.Single(
                rejectionInvoice =>
                rejectionInvoice.InvoiceNumber == invoiceNumber
                && rejectionInvoice.BillingMemberId == billingMemberId
                && rejectionInvoice.BilledMemberId == billedMemberId
                && rejectionInvoice.BillingCategoryId == (int)BillingCategory);*/


            if (rejectedInvoice == null) return billingPeriod;

            billingPeriod = rejectedInvoice.InvoiceBillingPeriod;
            //SMI M will have same rules as ICH
            /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH or Bilateral (anything but ACH) */
            if ((smi == Convert.ToInt32(SMI.Ich) || ReferenceManager.IsSmiLikeBilateral(smi, true) || smi == Convert.ToInt32(SMI.AchUsingIataRules)) && rejectedInvoice.InvoiceType != InvoiceType.RejectionInvoice) //allowed invoice types - original, correspondence invoices
            {
                rejectionStage = 1;
                isRejectionAllowed = true;
            }
            // Else if (settlementMethod == SettlementMethod.Ich
            //  && rejectedInvoice.InvoiceType == InvoiceType.RejectionInvoice)
            //{
            //  return billingPeriod;
            //}
            else if ((smi == Convert.ToInt32(SMI.Ach))
                     && rejectedInvoice.InvoiceType != InvoiceType.RejectionInvoice)
            {
                rejectionStage = 1;
                isRejectionAllowed = true;
            }
            else if ((smi == Convert.ToInt32(SMI.Ach))
                     && rejectedInvoice.InvoiceType == InvoiceType.RejectionInvoice && rejectedInvoice.RejectionStage == (int)RejectionStage.StageOne)
            {
                rejectionStage = 2;
                isRejectionAllowed = true;
            }
            // Else if ((settlementMethod == SettlementMethod.Ach || settlementMethod == SettlementMethod.AchUsingIataRules)
            //  && rejectedInvoice.InvoiceType == InvoiceType.RejectionInvoice && rejectedInvoice.RejectionStage == (int)RejectionStage.StageTwo)
            //{
            //  return billingPeriod;
            //}

            return billingPeriod;
        }


        /// <summary>
        /// Gets the rejection correspondence detail.
        /// </summary>
        /// <param name="correspondenceRefNo">The correspondence ref no.</param>
        /// <param name="billingMemberId">The billing member id.</param>
        /// <param name="billedMemberId">The billed member id.</param>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="isUpdateOperation">True if update operation, false otherwise.</param>
        /// <returns></returns>
        public CorrespondenceInvoiceDetails GetRejectionCorrespondenceDetail(long correspondenceRefNo, int billingMemberId, int billedMemberId, Guid invoiceId, bool isUpdateOperation)
        {
            var miscCorrespondence =
                // MiscCorrespondenceRepository.Get(correspondence => correspondence.CorrespondenceNumber == correspondenceRefNo).OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
                // Call replaced by Load strategy
              MiscCorrespondenceRepository.Get(correspondenceNumber: correspondenceRefNo).OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();


            // Details not found for given correspondence reference number.
            // If (miscCorrespondence == null) return correspondenceInvoiceDetails;

            var correspondenceInvoiceDetails = ValidateCorrespondenceReferenceNumber(miscCorrespondence, billedMemberId, billingMemberId, invoiceId, isUpdateOperation);

            if (!string.IsNullOrEmpty(correspondenceInvoiceDetails.ErrorMessage)) return correspondenceInvoiceDetails;

            // Fetch the invoice specified in the correspondence.
            // Replaced with LoadStrategy call
            var rejectedInvoice = MiscUatpInvoiceRepository.Single(invoiceId: miscCorrespondence.InvoiceId, billingCategoryId: (int)BillingCategory);
            if (rejectedInvoice == null)
            {
                correspondenceInvoiceDetails.ErrorMessage = Messages.RejInvNotFoundForCorrRefNo;
                return correspondenceInvoiceDetails;
            }

            correspondenceInvoiceDetails.RejectedInvoiceNumber = rejectedInvoice.InvoiceNumber;

            // if Authority to Bill flag is set in the correspondence
            if (miscCorrespondence.AuthorityToBill)
            {
                if (miscCorrespondence.CorrespondenceSubStatus != CorrespondenceSubStatus.Responded)
                {
                    correspondenceInvoiceDetails.ErrorMessage = Messages.CorrRefNoOpen;
                    return correspondenceInvoiceDetails;
                }

                // check if From member is equal billed member and to member is equal to billing member.
                if (miscCorrespondence.FromMemberId != billedMemberId || miscCorrespondence.ToMemberId != billingMemberId)
                {
                    correspondenceInvoiceDetails.ErrorMessage = Messages.CorrRefNoInvalidMembers;
                    return correspondenceInvoiceDetails;
                }
                correspondenceInvoiceDetails = PopulateExchangeRateDetails(correspondenceInvoiceDetails, rejectedInvoice);

                // Note this is valid scenario, where BilledMember has sent correspondence with Authority Bill flag set.
                correspondenceInvoiceDetails.IsAuthorityToBill = miscCorrespondence.AuthorityToBill;
                return correspondenceInvoiceDetails;
            }

            if (miscCorrespondence.CorrespondenceStatus == CorrespondenceStatus.Expired)
            {
                // in case of Expired Correspondence, check if to member is equal billed member and from member is equal to billing member.
                if (miscCorrespondence.ToMemberId != billedMemberId || miscCorrespondence.FromMemberId != billingMemberId)
                {
                    correspondenceInvoiceDetails.ErrorMessage = Messages.CorrRefNoInvalidMembers;
                    return correspondenceInvoiceDetails;
                }
                correspondenceInvoiceDetails = PopulateExchangeRateDetails(correspondenceInvoiceDetails, rejectedInvoice);
                // Note this is valid scenario, where BilledMember has failed to respond to Billing member's correspondence
                // and correspondence got expired.
                correspondenceInvoiceDetails.IsAuthorityToBill = false;
                return correspondenceInvoiceDetails;
            }

            // correspondence not yet expired with no authority to bill flag set, 
            // can not create correspondence invoice
            if (miscCorrespondence.CorrespondenceStatus == CorrespondenceStatus.Open)
            {
                correspondenceInvoiceDetails.ErrorMessage = Messages.CorrRefNoOpen;
                return correspondenceInvoiceDetails;
            }

            correspondenceInvoiceDetails.ErrorMessage = Messages.CorrRefNoGenError;

            return correspondenceInvoiceDetails;
        }

        /// <summary>
        /// Validates the correspondence reference number.
        /// </summary>
        /// <param name="miscCorrespondence">The misc correspondence.</param>
        /// <param name="billedMemberId">The billed member id.</param>
        /// <param name="billingMemberId">The billing member id.</param>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="isUpdateOperation">set to true for update operation.</param>
        /// <returns></returns>
        private CorrespondenceInvoiceDetails ValidateCorrespondenceReferenceNumber(MiscCorrespondence miscCorrespondence, int billedMemberId, int billingMemberId, Guid invoiceId, bool isUpdateOperation)
        {
            var correspondenceInvoiceDetails = new CorrespondenceInvoiceDetails();

            // if correspondence is not present.
            if (miscCorrespondence == null)
            {
                correspondenceInvoiceDetails.ErrorMessage = Messages.InvalidCorrRefNo;
                return correspondenceInvoiceDetails;
            }

            // if correspondence is in closed state.
            if (miscCorrespondence.CorrespondenceStatus == CorrespondenceStatus.Closed)
            {
                correspondenceInvoiceDetails.ErrorMessage = Messages.CorrRefNoClosed;
                return correspondenceInvoiceDetails;
            }

            if (miscCorrespondence.CorrespondenceNumber != null)
            {
                // check if invoice is already created for the correspondence ref number.
                var duplicateInvoiceExists = CheckDuplicateCorrespondenceInvoice(billedMemberId, billingMemberId, miscCorrespondence.CorrespondenceNumber.Value, invoiceId, isUpdateOperation);
                if (duplicateInvoiceExists)
                {
                    correspondenceInvoiceDetails.ErrorMessage = Messages.BMISC_10185;
                    return correspondenceInvoiceDetails;
                }
            }

            return correspondenceInvoiceDetails;
        }

        /// <summary>
        /// Populates the exchange rate details.
        /// </summary>
        /// <param name="correspondenceInvoiceDetails">The correspondence invoice details.</param>
        /// <param name="rejectedInvoice">The rejected invoice.</param>
        /// <returns></returns>
        private CorrespondenceInvoiceDetails PopulateExchangeRateDetails(CorrespondenceInvoiceDetails correspondenceInvoiceDetails, MiscUatpInvoice rejectedInvoice)
        {
            if (string.IsNullOrEmpty(correspondenceInvoiceDetails.RejectedInvoiceNumber)) return correspondenceInvoiceDetails;

            MiscUatpInvoice rejectionStageOneInvoice = new MiscUatpInvoice();
            if (rejectedInvoice.RejectionStage == (int)MiscRejectionStage.StageTwo)
            {
                // Replaced with LoadStrategy call
                rejectionStageOneInvoice =
                 MiscUatpInvoiceRepository.Single(invoiceNumber: rejectedInvoice.RejectedInvoiceNumber,
                                                  billingMemberId: rejectedInvoice.BilledMemberId,
                                                  billedMemberId: rejectedInvoice.BillingMemberId,
                                                  rejectionStage: (int)MiscRejectionStage.StageOne,
                                                  invoiceStatusId: (int)InvoiceStatusType.Presented);
            }
            else if (rejectedInvoice.RejectionStage == (int)MiscRejectionStage.StageOne)
            {
                rejectionStageOneInvoice = rejectedInvoice;
            }
            else
            {
                rejectionStageOneInvoice = null;
            }

            /* var rejectionStageOneInvoice =
                 MiscUatpInvoiceRepository.Single(
                   invoice =>
                   invoice.InvoiceNumber == rejectedInvoice.RejectedInvoiceNumber
                   && invoice.BillingMemberId == rejectedInvoice.BilledMemberId
                   && invoice.BilledMemberId == rejectedInvoice.BillingMemberId
                   && invoice.RejectionStage == (int)MiscRejectionStage.StageOne);*/

            // Rejection stage one invoice not found. 
            if (rejectionStageOneInvoice == null)
            {
                correspondenceInvoiceDetails.EnableExchangeRate = true;
                correspondenceInvoiceDetails.DisableBillingCurrency = false;
                return correspondenceInvoiceDetails;
            }

            // Replaced with LoadStrategy call
            var originalInvoice = MiscUatpInvoiceRepository.Single(
            invoiceNumber: rejectionStageOneInvoice.RejectedInvoiceNumber,
            billingMemberId: rejectedInvoice.BillingMemberId,
            billedMemberId: rejectedInvoice.BilledMemberId);
            /*
            var originalInvoice = MiscUatpInvoiceRepository.Single(
                invoice =>
                invoice.InvoiceNumber == rejectionStageOneInvoice.RejectedInvoiceNumber
                && invoice.BillingMemberId == rejectedInvoice.BillingMemberId
                && invoice.BilledMemberId == rejectedInvoice.BilledMemberId);*/

            // if rejection stage one invoice found but original invoice not found.
            if (originalInvoice == null)
            {
                // Current Billed in should be rejected stage1 invoice BilledIn
                correspondenceInvoiceDetails.CurrentBilledIn = rejectionStageOneInvoice.SettlementYear + "-" + rejectionStageOneInvoice.SettlementMonth;
                correspondenceInvoiceDetails.CurrentBillingCurrencyCode = rejectionStageOneInvoice.ListingCurrency.Id;
                correspondenceInvoiceDetails.DisableBillingCurrency = true;
                return correspondenceInvoiceDetails;
            }

            // Rejection one invoice and original invoice found.
            correspondenceInvoiceDetails.CurrentBillingCurrencyCode = originalInvoice.ListingCurrency.Id;
            correspondenceInvoiceDetails.CurrentBilledIn = originalInvoice.BillingYear + "-" + originalInvoice.BillingMonth;
            correspondenceInvoiceDetails.DisableBillingCurrency = true;


            return correspondenceInvoiceDetails;
        }

        /// <summary>
        /// Checks if any duplicate Correspondence Invoice Exists for the Correspondence Ref. No.
        /// for the Billing and Billed Member Combination.
        /// </summary>
        /// <param name="billedMemberId">Billed Member</param>
        /// <param name="billingMemberId">Billing Member.</param>
        /// <param name="correspondenceRefNo">Corr Ref Number.</param>
        /// <param name="invoiceId">Invoice ID.</param>
        /// <param name="isUpdateOperation">if set to true [its update operation].</param>
        protected bool CheckDuplicateCorrespondenceInvoice(int billedMemberId, int billingMemberId, long correspondenceRefNo, Guid invoiceId, bool isUpdateOperation)
        {
            const int correspodenceInvoiceEnumId = (int)InvoiceType.CorrespondenceInvoice;
            // check if correspondence ref number is not specific of any invoice
            if (isUpdateOperation)
            {
                // in case of update operation, exclude current invoice from the 
                // fetch criteria.
                // SCP-20517: Added the checks of Invoice Status other than Error Non-Correctable
                return MiscUatpInvoiceRepository.GetCount(invoice => invoice.InvoiceTypeId == correspodenceInvoiceEnumId
                                                    && invoice.BilledMemberId == billedMemberId
                                                    && invoice.BillingMemberId == billingMemberId
                                                    && invoice.CorrespondenceRefNo == correspondenceRefNo
                                                    && invoice.Id != invoiceId
                                                    && (invoice.InvoiceStatusId != (int)InvoiceStatusType.ErrorNonCorrectable && invoice.ValidationStatusId != (int)InvoiceValidationStatus.Failed)) > 0;
            }
            return MiscUatpInvoiceRepository.GetCount(invoice => invoice.InvoiceTypeId == correspodenceInvoiceEnumId
                                                  && invoice.BilledMemberId == billedMemberId
                                                  && invoice.BillingMemberId == billingMemberId
                                                  && invoice.CorrespondenceRefNo == correspondenceRefNo
                                                  && (invoice.InvoiceStatusId != (int)InvoiceStatusType.ErrorNonCorrectable && invoice.ValidationStatusId != (int)InvoiceValidationStatus.Failed)) > 0;
        }

        /// <summary>
        /// Update service start and end date of line item, if line item details service start and end date is not in range.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <param name="lineItemDetail">The line item detail.</param>
        private void UpdateLineItemServiceDate(LineItem lineItem, LineItemDetail lineItemDetail)
        {
            if (lineItemDetail.StartDate != null && lineItemDetail.StartDate < lineItem.StartDate)
            {
                lineItem.StartDate = lineItemDetail.StartDate;
            }
            else if (lineItemDetail.StartDate == null)
            {
                // If start date is not specified in line item level and line item detail end date is less then line item start date, update it with line item detail start date.
                if (lineItemDetail.EndDate < lineItem.StartDate) lineItem.StartDate = lineItemDetail.EndDate;
            }

            if (lineItemDetail.EndDate > lineItem.EndDate) lineItem.EndDate = lineItemDetail.EndDate;

            LineItemRepository.Update(lineItem);
        }

        /// <summary>
        /// Adds the invoice attachment.
        /// </summary>
        /// <param name="attachment"></param>
        /// <param name="invoice"></param>
        /// <param name="isUpdateAttachmentIndOrig"></param>
        /// <returns></returns>
        public MiscUatpAttachment AddInvoiceAttachment(MiscUatpAttachment attachment, MiscUatpInvoice invoice, bool isUpdateAttachmentIndOrig)
        {
            try
            {
                string suppDocOfflineCollectionPath = SupportingDocumentManager.GetSupportingDocFolderPath(invoice);

                if (!string.IsNullOrWhiteSpace(suppDocOfflineCollectionPath))
                {
                    FileServer fileServer = ReferenceManager.GetActiveAttachmentServer();

                    SupportingDocumentManager.CopyAttachments(attachment, fileServer.BasePath, suppDocOfflineCollectionPath);

                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Handled Error. Error Message: {0}, Stack Trace: {1}", ex.Message, ex.StackTrace);
            }

            MiscUatpInvoiceAttachmentRepository.Add(attachment);

            // Update the invoice only if the attachment indicator is not set from before.
            if (invoice.AttachmentIndicatorOriginal == 0 && isUpdateAttachmentIndOrig)
            {
                invoice.AttachmentIndicatorOriginal = 1;
                MiscUatpInvoiceRepository.Update(invoice);
            }

            UnitOfWork.CommitDefault();
            attachment = MiscUatpInvoiceAttachmentRepository.Single(a => a.Id == attachment.Id);
            return attachment;
        }

        /// <summary>
        /// Deletes the attachment with the given ID.
        /// </summary>
        /// <param name="attachmentId">ID of the attachment to delete.</param>
        /// <param name="isSupportingDoc">Is set to true if request is to delete Supporting Doc else set to false.</param>
        /// <returns>Flag indicating the success of delete operation.</returns>
        public bool DeleteAttachment(string attachmentId, bool isSupportingDoc)
        {
            try
            {
                var attachmentGuid = attachmentId.ToGuid();

                // Get Attachment entity from the list
                var invoiceAttachment =
                    MiscUatpInvoiceAttachmentRepository.Single(attachment => attachment.Id == attachmentGuid);

                if (invoiceAttachment == null) return false;

            var invoice = MiscUatpInvoiceRepository.Single(invoiceAttachment.ParentId);
            
                DateTime eventTime =
                    CalendarManager.GetCalendarEventTime(CalendarConstants.SupportingDocumentsLinkingDeadlineColumn,
                                                         invoice.BillingYear, invoice.BillingMonth,
                                                         invoice.BillingPeriod);

                if (DateTime.UtcNow > eventTime)
                {
                    throw new ISBusinessException(Messages.SupportingDocDeadline);
                }

                // Delete Attachment from physical path if isfullpath is true.
                SupportingDocumentManager.DeleteAttachement(invoiceAttachment);

                // Delete Attachment record from DB.
                MiscUatpInvoiceAttachmentRepository.Delete(invoiceAttachment);

                // If request is to delete Supporting document donot set AttachmentIndicatorOriginal flag to false if attachment count equals 0.   
                if (!isSupportingDoc)
                {
                    // If Invoice Attachment count == 0, set AttachmentIndicatorOriginal flag to false))
                    if (invoice.Attachments.Count == 0)
                    {
                        invoice.AttachmentIndicatorOriginal = 0;
                        MiscUatpInvoiceRepository.Update(invoice);
                    }
                }
                else
                {
                    if (invoice.Attachments.Count == 0 && invoice.SubmissionMethodId == (int)SubmissionMethod.IsWeb)
                    {
                        invoice.AttachmentIndicatorOriginal = 0;
                        MiscUatpInvoiceRepository.Update(invoice);
                    }
                }

                UnitOfWork.CommitDefault();
            }
            catch (ISBusinessException bex)
            {
                throw bex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        /// <summary>
        /// Return default value for settlement indicator for given combination of billing and billed members
        /// </summary>
        /// <param name="billingMemberId">Billing Member Id</param>
        /// <param name="billedMemberId">Billed Member Id</param>
        /// <param name="billingCategoryId"></param>
        /// <returns></returns>
        public override SMI GetDefaultSettlementMethodForMembers(int billingMemberId, int billedMemberId, int billingCategoryId)
        {
            BillingMember = MemberManager.GetMember(billingMemberId);
            BilledMember = MemberManager.GetMember(billedMemberId);

            var settlementMethod = base.GetDefaultSettlementMethodForMembers(billingMemberId, billedMemberId, billingCategoryId);

            return settlementMethod;
        }

        /// <summary>
        /// Determines whether line item exists for specified invoice id].
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>
        /// true if line item exists for the specified invoice id; otherwise, false.
        /// </returns>
        public bool IsLineItemExists(string invoiceId)
        {
            var invoiceGuid = invoiceId.ToGuid();
            return LineItemRepository.GetCount(lineItem => lineItem.InvoiceId == invoiceGuid) > 0;
        }

        /// <summary>
        /// Determines whether [is line item detail exists] [the specified line item id].
        /// </summary>
        /// <param name="lineItemId">The line item id.</param>
        /// <returns>
        /// 	<c>true</c> if [is line item detail exists] [the specified line item id]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLineItemDetailExists(string lineItemId)
        {
            var lineItemGuid = lineItemId.ToGuid();
            return LineItemDetailRepository.GetCount(lineItemDetail => lineItemDetail.LineItemId == lineItemGuid) > 0;
        }

        public long GetLineItemDetailsCount(string lineItemId)
        {
            var lineItemGuid = lineItemId.ToGuid();
            return LineItemDetailRepository.GetCount(lineItemDetail => lineItemDetail.LineItemId == lineItemGuid);
        }

        /// <summary>
        /// Performs the dynamic field validation.
        /// </summary>
        /// <param name="chargeCodeId">The charge code id.</param>
        /// <param name="chargeCodeTypeId">The charge code type id.</param>
        /// <param name="fieldInfo">The field info.</param>
        /// <param name="errors">The errors.</param>
        /// <returns></returns>
        private bool PerformDynamicFieldValidation(int chargeCodeId, int? chargeCodeTypeId, IList<FieldMetaData> fieldInfo, out IList<DynamicValidationError> errors)
        {
            errors = new List<DynamicValidationError>();
            var isValidationSuccess = true;

            // Get Server validators for given charge code and charge code type id.
            IList<ServerValidator> serverValidators;
            if (ValidationCache.Instance.DynamicFieldServerValidators != null)
            {
                serverValidators = chargeCodeTypeId.HasValue
                                     ? ValidationCache.Instance.DynamicFieldServerValidators.Where(
                                       serverValidator =>
                                       (serverValidator.ChargeCodeId == chargeCodeId || serverValidator.ChargeCodeId == null) &&
                                       (serverValidator.ChargeCodeTypeId == chargeCodeTypeId.Value || serverValidator.ChargeCodeTypeId == null)).ToList()
                                     : ValidationCache.Instance.DynamicFieldServerValidators.Where(
                                       serverValidator =>
                                       (serverValidator.ChargeCodeId == chargeCodeId || serverValidator.ChargeCodeId == null) &&
                                       serverValidator.ChargeCodeTypeId == null).ToList();
            }
            else
            {
                serverValidators = chargeCodeTypeId.HasValue
                                     ? ServerValidatorRepository.Get(
                                       serverValidator =>
                                       (serverValidator.ChargeCodeId == chargeCodeId || serverValidator.ChargeCodeId == null) &&
                                       (serverValidator.ChargeCodeTypeId == chargeCodeTypeId.Value || serverValidator.ChargeCodeTypeId == null)).ToList()
                                     : ServerValidatorRepository.Get(
                                       serverValidator =>
                                       (serverValidator.ChargeCodeId == chargeCodeId || serverValidator.ChargeCodeId == null) &&
                                       serverValidator.ChargeCodeTypeId == null).ToList();

            }

            foreach (var serverValidator in serverValidators)
            {
                var validator = GetServerValidatorObject(serverValidator);
                // If validator object is not created.
                if (validator == null) continue;

                DynamicValidationError valdationError;
                if (validator.Validate(fieldInfo, MiscUatpErrorCodes, out valdationError))
                {
                    continue;
                }
                isValidationSuccess = false;
                errors.Add(valdationError);
            }

            return isValidationSuccess;
        }

        /// <summary>
        /// Gets the server validator object.
        /// </summary>
        /// <param name="serverValidator">The server validator.</param>
        /// <returns></returns>
        private IServerValidator GetServerValidatorObject(ServerValidator serverValidator)
        {
            Type type;
            try
            {
                type = Type.GetType(serverValidator.ValidationClassName);
                // If type not found.
                if (type == null) return null;
            }
            catch
            {
                return null;
            }

            return (IServerValidator)Activator.CreateInstance(type);
        }

        /// <summary>
        /// Validates the exchange rate.
        /// </summary>
        /// <param name="miscUatpInvoice">The Misc/UATP invoice.</param>
        /// <returns>
        /// true if  valid exchange rate for specified Misc/UATP invoice; otherwise, false.
        /// </returns>
        protected bool IsValidExchangeRate(MiscUatpInvoice miscUatpInvoice, IList<IsValidationExceptionDetail> exceptionDetailsList)
        {
            if (miscUatpInvoice.BillingCurrency != null && miscUatpInvoice.ListingCurrencyId != null)
            {
                try
                {
                    // This function will be merged later with exsting function.
                    var exchangeRate = GetExchangeRate(miscUatpInvoice.ListingCurrencyId.Value, miscUatpInvoice.BillingCurrency.Value, miscUatpInvoice.BillingYear, miscUatpInvoice.BillingMonth);
                    // SCP234490 : Inquiry about Exchange Rate
                   //CMP#648: Convert Exchange rate into nullable field.
                    if (ConvertUtil.Round(miscUatpInvoice.ExchangeRate.HasValue? miscUatpInvoice.ExchangeRate.Value: 0.0M, Constants.ExchangeRateDecimalPlaces) != (decimal)ConvertUtil.Round(exchangeRate, Constants.ExchangeRateDecimalPlaces))
                    {
                        return false;
                    }
                }
                catch (ISBusinessException)
                {
                    if (exceptionDetailsList == null)
                    {
                        return false;
                    }

                    throw;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the exchange rate.    
        /// </summary>
        /// <param name="listingCurrencyId">The listing currency id.</param>
        /// <param name="billingCurrency">The billing currency enum.</param>
        /// <param name="billingYear">Invoice billing year</param>
        /// <param name="billingMonth">Invoice billing month</param>
        /// <returns>exchange rate</returns>
        private double GetExchangeRate(int listingCurrencyId, BillingCurrency billingCurrency, int billingYear, int billingMonth)
        {
            var exchangeRateValue = 0d;

            // Get exchange rate for given billing year/month, first date of billing year/month.
            var billingDate = new DateTime(billingYear, billingMonth, 1);

            // Get exchange rate for given listing currency id and first date of billing year/month between exchange rates effective from date and effective to date.
            var exchangeRate = ExchangeRateRepository.Single(exchangeRates => exchangeRates.CurrencyId == listingCurrencyId
                                && exchangeRates.EffectiveFromDate <= billingDate && exchangeRates.EffectiveToDate >= billingDate);

            // Check if exchange rate for given listing currency is valid.
            if (exchangeRate == null)
            {
                return exchangeRateValue;
            }

            if (listingCurrencyId == (int)billingCurrency) exchangeRateValue = 1;

            // Get five days exchange rate value for given Billing Currency.
            switch (billingCurrency)
            {
                case BillingCurrency.USD:
                    exchangeRateValue = exchangeRate.FiveDayRateUsd;
                    break;

                case BillingCurrency.EUR:
                    exchangeRateValue = exchangeRate.FiveDayRateEur;
                    break;

                case BillingCurrency.GBP:
                    exchangeRateValue = exchangeRate.FiveDayRateGbp;
                    break;

                default:
                    break;
            }

            return exchangeRateValue;
        }

        /// <summary>
        /// Get dynamic field values in hierarchy of group-fields-attributes to save in DB
        /// </summary>
        /// <param name="uiFieldValues"></param>
        /// <param name="chargeCodeId"></param>
        /// <param name="chargeCodeTypeId"></param>
        /// <returns></returns>
        public List<FieldValue> SetFieldValueForLineItemDetail(List<FieldValue> uiFieldValues, int chargeCodeId, Nullable<int> chargeCodeTypeId, Guid lineItemDetailId)
        {
            //Assign lineItemDetailId for all records
            foreach (var fieldValue in uiFieldValues)
                fieldValue.LineItemDetailId = lineItemDetailId;

            var fieldValues = new List<FieldValue>();

            // Get field metadata for given charge code and Charge code type combination
            //This is dead function so putting hard code value for resolve conflict.
            //CMP #636: Standard Update Mobilization
            var fieldMetadata = FieldMetaDataRepository.GetFieldMetadata(chargeCodeId, chargeCodeTypeId, null, (int)BillingCategoryType.Misc);
            foreach (var field in fieldMetadata)
            {
                var fieldValueForNode = new List<FieldValue>();
                bool isValueEntered = false;
                // Get field values for entered on UI for field
                if (field.FieldType == FieldType.Field)
                {
                    fieldValueForNode.AddRange(GetFieldValuesForFieldsInGroup(field, uiFieldValues, ref isValueEntered, lineItemDetailId));
                }
                else if (field.FieldType == FieldType.Group)
                {
                    fieldValueForNode.AddRange(GetFieldValuesForGroup(field, uiFieldValues, ref isValueEntered, lineItemDetailId));
                }
                if (isValueEntered)
                    fieldValues.AddRange(fieldValueForNode);
            }
            return fieldValues;
        }

        /// <summary>
        /// This function returns fieldValue array for fields in a group
        /// </summary>
        /// <param name="field"></param>
        /// <param name="uiFieldValues"></param>
        /// <param name="isValueEntered"></param>
        /// <param name="lineItemDetailId"></param>
        /// <returns></returns>
        private List<FieldValue> GetFieldValuesForGroup(FieldMetaData field, List<FieldValue> uiFieldValues, ref bool isValueEntered, Guid lineItemDetailId)
        {
            var fieldValueForNode = new List<FieldValue>();

            // Get list of values entered for subfields on field
            var fieldValuesforGroup = new List<FieldValue>();
            var childNodeMetadataIdsLevel1 = field.SubFields.Select(c => c.Id);

            var childNodesLevel1 = uiFieldValues.Where(f => childNodeMetadataIdsLevel1.Contains(f.FieldMetaDataId));
            fieldValuesforGroup.AddRange(childNodesLevel1);

            var childNodesLevel2 = new List<FieldValue>();
            var childNodesLevel3 = new List<FieldValue>();

            // Get values entered on UI for every subfield of subfields [level 2] and values for subfields of level 2 fields
            foreach (var subfield in field.SubFields)
            {
                var childNodeMetadataIds = subfield.SubFields.Select(s => s.Id);
                childNodesLevel2.AddRange(uiFieldValues.Where(u => childNodeMetadataIds.Contains(u.FieldMetaDataId)));

                childNodeMetadataIds = new List<Guid>();
                foreach (var childSubfield in subfield.SubFields)
                {
                    childNodeMetadataIds = childSubfield.SubFields.Select(s => s.Id);
                    childNodesLevel3.AddRange(uiFieldValues.Where(u => childNodeMetadataIds.Contains(u.FieldMetaDataId)));
                }

            }
            fieldValuesforGroup.AddRange(childNodesLevel2);
            fieldValuesforGroup.AddRange(childNodesLevel3);

            // If value is entered for any fields/subfields of group
            if (fieldValuesforGroup.Count() > 0)
            {

                var groupCount = fieldValuesforGroup.Select(s => s.ControlIdGroupCount).Distinct();
                foreach (var count in groupCount)
                {

                    var fieldValuesForGroupCount = fieldValuesforGroup.Where(s => s.ControlIdGroupCount == count).ToList();
                    // Create new object for field value for Group
                    var groupValue = new FieldValue() { FieldMetaDataId = field.Id, Value = "", LineItemDetailId = lineItemDetailId };
                    foreach (var subfield in field.SubFields)
                    {
                        //Add list of values for subfields of group
                        if (subfield.FieldType == FieldType.Field)
                        {
                            groupValue.AttributeValues.AddRange(GetFieldValuesForFieldsInGroup(subfield, fieldValuesForGroupCount,
                                                                                               ref isValueEntered, lineItemDetailId));
                        }
                        else if (subfield.FieldType == FieldType.Group)
                        {
                            var subGroupNode = new FieldValue() { FieldMetaDataId = subfield.Id, Value = "", LineItemDetailId = lineItemDetailId };
                            foreach (var childSubField in subfield.SubFields)
                            {
                                subGroupNode.AttributeValues.AddRange(GetFieldValuesForFieldsInGroup(childSubField, fieldValuesForGroupCount,
                                                                                                     ref isValueEntered,
                                                                                                     lineItemDetailId));
                            }
                            groupValue.AttributeValues.Add(subGroupNode);
                        }
                    }
                    if (isValueEntered)
                        fieldValueForNode.Add(groupValue);
                }

                //}

            }
            return fieldValueForNode;
        }

        /// <summary>
        /// Get list of field value for field 
        /// </summary>
        /// <param name="field"></param>
        /// <param name="uiFieldValues"></param>
        /// <param name="isValueEntered"></param>
        /// <param name="lineItemDetailId"></param>
        /// <returns></returns>
        private List<FieldValue> GetFieldValuesForFieldsInGroup(FieldMetaData field, List<FieldValue> uiFieldValues, ref bool isValueEntered, Guid lineItemDetailId)
        {
            var fieldValueForNode = new List<FieldValue>();
            if (uiFieldValues.Select(u => u.FieldMetaDataId).Contains(field.Id))
            {
                fieldValueForNode.AddRange(uiFieldValues.Where(f => f.FieldMetaDataId == field.Id));

                // If field data type is numeric with specified decimal places then round the input value to specified number of decimal places.
                if (field.DataType == DataType.Numeric)
                {
                    if (field.DataLength.Contains(Comma))
                    {
                        var noOfDecimalPlaces = field.DataLength.Split(new[] { Comma }, StringSplitOptions.RemoveEmptyEntries);
                        if (noOfDecimalPlaces.Length == 2)
                        {
                            RoundDecimalFieldValues(fieldValueForNode, noOfDecimalPlaces[1]);
                        }
                    }
                    else
                    {
                        // Used try parse to check if number has 0 at start of string

                        for (var i = 0; i < fieldValueForNode.Count; i++)
                        {
                            int fieldValue;
                            int.TryParse(fieldValueForNode[i].Value, out fieldValue);
                            fieldValueForNode[i].Value = fieldValue.ToString();
                        }
                    }

                }
                isValueEntered = true;
            }
            var subFieldValues = uiFieldValues.Where(s => field.SubFields.Select(c => c.Id).Contains(s.FieldMetaDataId));
            var groupCount = subFieldValues.Select(s => s.ControlIdCount).Distinct();
            foreach (var count in groupCount)
            {
                if (fieldValueForNode.Where(node => node.ControlIdCount == count).Count() == 1)
                {
                    var node = fieldValueForNode.Single(n => n.ControlIdCount == count);
                    var subFieldwithFieldCount = subFieldValues.Where(s => s.ControlIdCount == count);
                    node.AttributeValues.AddRange(subFieldwithFieldCount);
                    isValueEntered = true;
                }
                else
                {
                    var node = new FieldValue() { FieldMetaDataId = field.Id, Value = "", LineItemDetailId = lineItemDetailId };
                    var subFieldwithFieldCount = subFieldValues.Where(s => s.ControlIdCount == count);
                    node.AttributeValues.AddRange(subFieldwithFieldCount);
                    fieldValueForNode.Add(node);
                    isValueEntered = true;
                }
            }
            return fieldValueForNode;
        }

        /// <summary>
        /// Rounds the decimal field values.
        /// </summary>
        /// <param name="fieldValueForNode">The field value for node.</param>
        /// <param name="noOfDecimalPlaces">The no of decimal places.</param>
        private static void RoundDecimalFieldValues(IList<FieldValue> fieldValueForNode, string noOfDecimalPlaces)
        {
            var noOfValues = fieldValueForNode.Count;
            int decimalPlacesInValue;
            // Used try parse to check if number of decimal 
            if (int.TryParse(noOfDecimalPlaces, out decimalPlacesInValue))
            {
                for (var i = 0; i < noOfValues; i++)
                {
                    var fieldValue = decimal.Parse(fieldValueForNode[i].Value);
                    fieldValueForNode[i].Value = decimal.Round(fieldValue, decimalPlacesInValue).ToString();
                }
            }
        }

        /// <summary>
        /// Validates the parsed line item.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <param name="exceptionDetailsList">The exception details list.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
        /// <returns></returns>
        protected bool ValidateParsedLineItem(LineItem lineItem, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, MiscUatpInvoice miscUatpInvoice, DateTime fileSubmissionDate)
        {
            var chargeCodeId = lineItem.ChargeCodeId;
            var chargeCodeTypeId = lineItem.ChargeCodeTypeId;
            var isLineItemDetailsManadatory = false;

            double summationTolerance = 0;
            double roundingTolerance = 0;
            if (miscUatpInvoice.Tolerance != null)
            {
                summationTolerance = miscUatpInvoice.Tolerance.SummationTolerance;
                roundingTolerance = miscUatpInvoice.Tolerance.RoundingTolerance;
            }

            var isValid = true;

            // Validate if line item details are mandatory 
            // This check will be done only when Invoice Type is invoice.
            //CMP #636
            if (miscUatpInvoice.InvoiceType == InvoiceType.Invoice)
            {
                var requiredGroupFields = (from fieldMetaData in _fieldMetaDatas
                                           join fieldChargeCodeMapp in _fieldChargeCodeMappings on fieldMetaData.Id equals fieldChargeCodeMapp.FieldMetaDataId
                                           where
                                             fieldChargeCodeMapp.ChargeCodeId == chargeCodeId && fieldMetaData.FieldTypeId == (int)FieldType.Group &&
                                             fieldChargeCodeMapp.RequiredTypeId == (int)RequiredType.Mandatory
                                              && (miscUatpInvoice.BillingCategoryId == (int)BillingCategoryType.Misc ? true : fieldChargeCodeMapp.ChargeCodeTypeId == chargeCodeTypeId)
                                           select fieldMetaData).ToList();
                if (requiredGroupFields.Count > 0)
                {
                    isLineItemDetailsManadatory = true;
                }
            }

            // Validation for LineItemNumber   
            if (lineItem.LineItemNumber <= 0)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "LineItem Number", lineItem.LineItemNumber.ToString(), fileName, ErrorLevels.ErrorLevelLineItem, MiscUatpErrorCodes.InvalidLineItemNumber, ErrorStatus.X, lineItem.LineItemNumber, 0);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            //SCP220346: Inward Billing-XML file mandatory field(scaling factor can not be '0').
            //Validated scaling factor value.
            if (lineItem.ScalingFactor.HasValue && lineItem.ScalingFactor.Value == 0)
            {
              IsValidationExceptionDetail validationExceptionDetail;
              if (miscUatpInvoice.BillingCategoryId == (int) BillingCategoryType.Misc)
              {
                validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate, miscUatpInvoice,
                                                                            "Scaling Factor",
                                                                            lineItem.ScalingFactor.Value.
                                                                              ToString(), fileName,
                                                                            ErrorLevels.ErrorLevelLineItem,
                                                                            new MiscErrorCodes().
                                                                              InvalidScalingFactorValue,
                                                                            ErrorStatus.X,
                                                                            lineItem.LineItemNumber,
                                                                            0);
              }
              else
              {
                validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate, miscUatpInvoice,
                                                                            "Scaling Factor",
                                                                            lineItem.ScalingFactor.Value.
                                                                              ToString(), fileName,
                                                                            ErrorLevels.ErrorLevelLineItem,
                                                                            new UatpErrorCodes().
                                                                              InvalidScalingFactorValue,
                                                                            ErrorStatus.X,
                                                                            lineItem.LineItemNumber,
                                                                            0);
              }

              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }

          // Validation for POLineItemNumber   
            if ((miscUatpInvoice.BillingCategory == BillingCategoryType.Misc) && lineItem.POLineItemNumber <= 0)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "PO LineItem Number", lineItem.POLineItemNumber.ToString(), fileName, ErrorLevels.ErrorLevelLineItem, MiscUatpErrorCodes.InvalidPOLineItemNumber, ErrorStatus.X, lineItem.LineItemNumber, 0);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            // Validation for ChargeCode   
            if (lineItem.ChargeCode != null && miscUatpInvoice.ChargeCategory != null)
            {
                ChargeCategory chargeCategory;
                if (miscUatpInvoice.ValidChargeCategoryList != null)
                {
                    //Get it from the validation cache
                    chargeCategory = miscUatpInvoice.ValidChargeCategoryList.First(i => i.IsActive && i.Name == miscUatpInvoice.ChargeCategory.Name && i.BillingCategoryId == miscUatpInvoice.BillingCategoryId);
                }
                else
                {
                    //Search it in the database
                    chargeCategory = GetChargeCategory(miscUatpInvoice.ChargeCategory.Name, miscUatpInvoice.BillingCategoryId);
                }
                if (chargeCategory != null && chargeCategory.ChargeCodes != null && !chargeCategory.ChargeCodes.Exists(i => i.Name.Equals(lineItem.ChargeCode.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    var validationExceptionDetail = new IsValidationExceptionDetail();
                    if (miscUatpInvoice.BillingCategory == BillingCategoryType.Uatp)
                        validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Charge Code", lineItem.ChargeCode.Name, fileName, ErrorLevels.ErrorLevelLineItem, UatpErrorCodes.InvalidChargeCode, ErrorStatus.X, lineItem.LineItemNumber, 0);
                    else
                        validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Charge Code", lineItem.ChargeCode.Name, fileName, ErrorLevels.ErrorLevelLineItem, MiscErrorCodes.InvalidChargeCode, ErrorStatus.X, lineItem.LineItemNumber, 0);

                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }

            // Start Date - End Date validation 
            if (lineItem.StartDate >= lineItem.EndDate)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Start Date", Convert.ToString(lineItem.StartDate),
                                                                         fileName, ErrorLevels.ErrorLevelLineItem,
                                                                         MiscUatpErrorCodes.InvalidStartDate, ErrorStatus.X, lineItem.LineItemNumber, 0);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            // UOM code validation
            /* Previous statement :  if (!string.IsNullOrEmpty(lineItem.UomCodeId) && lineItem.UomCode != null)
             * Althought, it found right UOM Code, it was logging validation error
             * so removed !(NOT) from this statement */
            if (string.IsNullOrEmpty(lineItem.UomCodeId) && lineItem.UomCode != null)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "UOM Code", lineItem.UomCode.Name,
                                                                          fileName, ErrorLevels.ErrorLevelLineItem,
                                                                          MiscUatpErrorCodes.InvalidUomCode, ErrorStatus.X, lineItem.LineItemNumber, 0);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            // Line Item service date range is out side of date range given for Line Item Details.
            if (!IsValidParsedServiceDate(lineItem))
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Start Date, End Date", Convert.ToString(lineItem.StartDate) + "," + Convert.ToString(lineItem.EndDate),
                                                                fileName, ErrorLevels.ErrorLevelLineItem,
                                                                MiscUatpErrorCodes.InvalidServiceDateRange, ErrorStatus.X, lineItem.LineItemNumber, 0);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            // Validation for ChargeAmount
            // NOTE : This validation will not be performed if the value of the field 'MinimumQuantityFlag' is 'Y'
            // var calculatedChargeAmount = (lineItem.Quantity * lineItem.UnitPrice) / lineItem.ScalingFactor.Value;
            // UG3320 3.1 'ChargeAmount' at Line Item Level should be equal to 'ChargeAmount' of the linked line item detail levels. 
            // It should also equal the Quantity * Unit Price / Scaling factor at that level.
            if (isLineItemDetailsManadatory && lineItem.LineItemDetails.Count == 0)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "LineItemDetail",
                                                                                 string.Empty,
                                                                                 fileName,
                                                                                 ErrorLevels.ErrorLevelLineItem,
                                                                                 MiscUatpErrorCodes.MandatoryLineItemDetail,
                                                                                 ErrorStatus.X,
                                                                                 lineItem.LineItemNumber,
                                                                                 0);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            if (isLineItemDetailsManadatory || lineItem.LineItemDetails.Count > 0)
            {
                var calculatedLineItemDetailChargeAmount = lineItem.LineItemDetails.Sum(lineItemDetailChargeAmt => lineItemDetailChargeAmt.ChargeAmount);
                if (!CompareUtil.Compare(ConvertUtil.Round(calculatedLineItemDetailChargeAmount, Constants.MiscDecimalPlaces), lineItem.ChargeAmount, summationTolerance, Constants.MiscDecimalPlaces))
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Charge Amount",
                                                                                    lineItem.ChargeAmount.ToString(),
                                                                                    fileName,
                                                                                    ErrorLevels.ErrorLevelLineItem,
                                                                                    MiscUatpErrorCodes.InvalidCharAmountForLinkedLineItemDetail,
                                                                                    ErrorStatus.X,
                                                                                    lineItem.LineItemNumber,
                                                                                    0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }


            if (!lineItem.MinimumQuantityFlag || miscUatpInvoice.BillingCategory == BillingCategoryType.Uatp)
            {
                if (lineItem.ScalingFactor.HasValue && lineItem.ScalingFactor.Value != 0)
                {
                    var calculatedChargeAmount = (lineItem.Quantity * lineItem.UnitPrice) / lineItem.ScalingFactor.Value;
                    if (!CompareUtil.Compare(ConvertUtil.Round(calculatedChargeAmount, Constants.MiscDecimalPlaces), lineItem.ChargeAmount, roundingTolerance, Constants.MiscDecimalPlaces))
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Charge Amount",
                                                                                        lineItem.ChargeAmount.ToString(),
                                                                                        fileName, ErrorLevels.ErrorLevelLineItem,
                                                                                        MiscUatpErrorCodes.InvalidChargeAmount,
                                                                                        ErrorStatus.X,
                                                                                        lineItem.LineItemNumber, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                }
            }

            // Vat Validations
            foreach (var tax in lineItem.TaxBreakdown)
            {
                if (tax.Type.ToLower() == "vat")
                {
                    ValidateParsedTax(tax, exceptionDetailsList, fileName, miscUatpInvoice, fileSubmissionDate, ErrorLevels.ErrorLevelLineItemVat, roundingTolerance, lineItem);
                }
                else
                {
                    ValidateParsedTax(tax, exceptionDetailsList, fileName, miscUatpInvoice, fileSubmissionDate, ErrorLevels.ErrorLevelLineItemTax, roundingTolerance, lineItem);
                }
            }

            // Validate Add on Charge 
            foreach (var addoncharge in lineItem.AddOnCharges)
            {
                ValidateParsedAddonCharge(addoncharge, exceptionDetailsList, fileName, miscUatpInvoice, fileSubmissionDate, ErrorLevels.ErrorLevelLineItemAddOnCharge, roundingTolerance, lineItem);
            }

            decimal calculatedTotalTaxAmount = 0, calculatedTotalVatAmount = 0, calculatedTotalAddOnChargeAmount = 0, calculatedTotalNetAmount = 0;
            GetLineItemTotalFields(lineItem, ref calculatedTotalTaxAmount, ref calculatedTotalVatAmount, ref calculatedTotalAddOnChargeAmount, ref calculatedTotalNetAmount);

            // Validate TotalTaxAmount
            if (!CompareUtil.Compare(ConvertUtil.Round(calculatedTotalTaxAmount, Constants.MiscDecimalPlaces), lineItem.TotalTaxAmount.HasValue ? lineItem.TotalTaxAmount.Value : 0, summationTolerance, Constants.MiscDecimalPlaces))
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Total Tax Amount", lineItem.TotalTaxAmount.ToString(),
                                                                        fileName, ErrorLevels.ErrorLevelLineItem,
                                                                        MiscUatpErrorCodes.InvalidTotalTaxAmount, ErrorStatus.X, lineItem.LineItemNumber, 0);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            // Validate TotalVatAmount
            if (!CompareUtil.Compare(ConvertUtil.Round(calculatedTotalVatAmount, Constants.MiscDecimalPlaces), lineItem.TotalVatAmount.HasValue ? lineItem.TotalVatAmount.Value : 0, summationTolerance, Constants.MiscDecimalPlaces))
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Total Vat Amount", lineItem.TotalVatAmount.ToString(),
                                                                        fileName, ErrorLevels.ErrorLevelLineItem,
                                                                        MiscUatpErrorCodes.InvalidTotalVatAmount, ErrorStatus.X, lineItem.LineItemNumber, 0
                                                                         );
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            // Validate TotalAddOnChargeAmount
            if (!CompareUtil.Compare(ConvertUtil.Round(calculatedTotalAddOnChargeAmount, Constants.MiscDecimalPlaces), lineItem.TotalAddOnChargeAmount.HasValue ? lineItem.TotalAddOnChargeAmount.Value : 0, summationTolerance, Constants.MiscDecimalPlaces))
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Total AddOn Charge Amount", lineItem.TotalAddOnChargeAmount.ToString(),
                                                                        fileName, ErrorLevels.ErrorLevelLineItem,
                                                                        MiscUatpErrorCodes.InvalidTotalAddOnChargeAmount, ErrorStatus.X, lineItem.LineItemNumber, 0);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            // Validate TotalNetAmount
            if (!CompareUtil.Compare(ConvertUtil.Round(calculatedTotalNetAmount, Constants.MiscDecimalPlaces), lineItem.TotalNetAmount, summationTolerance, Constants.MiscDecimalPlaces))
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Total Net Amount", lineItem.TotalNetAmount.ToString(),
                                                                        fileName, ErrorLevels.ErrorLevelLineItem,
                                                                        MiscUatpErrorCodes.InvalidTotalNetAmount, ErrorStatus.X, lineItem.LineItemNumber, 0);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            // Validate OriginalLineItemNumber
            if (lineItem.OriginalLineItemNumber <= 0 && miscUatpInvoice.BillingCategory == BillingCategoryType.Misc)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Original LineItem Number", lineItem.OriginalLineItemNumber.ToString(), fileName, ErrorLevels.ErrorLevelLineItem, MiscUatpErrorCodes.InvalidOriginalLineItemNumber, ErrorStatus.X, lineItem.LineItemNumber, 0);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            // Web specification
            // LocationCode validation
            // New updated Logic for CMP#515
            // Location Code is always mandatory at either Invoice level or Line Item level
            if (string.IsNullOrEmpty(lineItem.LocationCode) && string.IsNullOrEmpty(miscUatpInvoice.LocationCode) && miscUatpInvoice.BillingCategory == BillingCategoryType.Misc)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Line Item LocationCode", lineItem.LocationCode, fileName, ErrorLevels.ErrorLevelLineItem, MiscUatpErrorCodes.LocationCodeRequiredAtInvoiceOrLineItem, ErrorStatus.X, lineItem.LineItemNumber, 0, true);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }
            // If Location Code is defined at Invoice level, it cannot be defined for any Line Item. Violation of this rule will result in Error-Non-Correctable 
            // If Location Code is not defined at Invoice level, then it should be defined for every Line Item. Violation of this rule will result in Error-Non-Correctable
            if (!string.IsNullOrEmpty(lineItem.LocationCode) && !string.IsNullOrEmpty(miscUatpInvoice.LocationCode))
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Line Item LocationCode", lineItem.LocationCode, fileName, ErrorLevels.ErrorLevelLineItem, MiscUatpErrorCodes.LocationCodeRequiredAtLineItem, ErrorStatus.X, lineItem.LineItemNumber, 0, true);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            // Charge Code Type Validation
            // Charge Code Type Mandatory for Airport-Fees and Miscellaneous-Adjustments Charge Category- Charge Code combinations
            //CMP #636
            if (miscUatpInvoice.BillingCategory == BillingCategoryType.Uatp)
            {
              if (lineItem.ChargeCode != null && (lineItem.ChargeCode.IsChargeCodeTypeRequired != null ? lineItem.ChargeCode.IsChargeCodeTypeRequired.Value : false))
              {
                if (lineItem.ChargeCodeType == null)
                {
                  var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Charge Code type", string.Empty, fileName, ErrorLevels.ErrorLevelLineItem, MiscUatpErrorCodes.RequiredChargeCodeType, ErrorStatus.X, lineItem.LineItemNumber, 0, true);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
                }
              }
            }


            // Validation exclusively for Misc Type Invoice
            // Validation for CityAirport as it is auto complete field in UI Is-Web specification 9.1.1.5)
            if (lineItem.LocationCode != null && !string.IsNullOrEmpty(lineItem.LocationCode.Trim()) && !IsValidCityAirportCode(lineItem.LocationCode))
            {
                if (!IsValidUnlocCode(lineItem.LocationCode))
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Line Item LocationCode", lineItem.LocationCode, fileName, ErrorLevels.ErrorLevelLineItem, MiscUatpErrorCodes.InvalidIataAirPrtCityLocationCode, ErrorStatus.X, lineItem.LineItemNumber, 0, true);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }



            if (!String.IsNullOrEmpty(lineItem.LocationCodeIcao))
            {
                var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
                if (!referenceManager.IsValidLocationIcaoCode(lineItem.LocationCodeIcao))
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Line Item LocationCodeIcao", lineItem.LocationCodeIcao, fileName, ErrorLevels.ErrorLevelLineItem, MiscUatpErrorCodes.InvalidLocationCodeIcao, ErrorStatus.X, 0, 0, true);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
                if (!string.IsNullOrEmpty(miscUatpInvoice.LocationCodeIcao) && miscUatpInvoice.LocationCodeIcao.CompareTo(lineItem.LocationCodeIcao) != 0)
                {
                    if (miscUatpInvoice.BillingCategory == BillingCategoryType.Misc)
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Location Code Icao", lineItem.LocationCodeIcao, fileName, ErrorLevels.ErrorLevelLineItem, MiscUatpErrorCodes.InvalidLineItemLocationCode, ErrorStatus.X, lineItem.LineItemNumber, 0, true);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                }
            }

            // Web specification 2.e
            // POLineItemNumber validation
            // Should allow entry in this field only if a PO Number has been populated in the invoice header.
            if ((lineItem.POLineItemNumber.HasValue && !string.IsNullOrEmpty(lineItem.POLineItemNumber.Value.ToString())) && string.IsNullOrEmpty(miscUatpInvoice.PONumber))
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "PO LineItem Number", lineItem.POLineItemNumber.Value.ToString(),
                                                                          fileName, ErrorLevels.ErrorLevelLineItem,
                                                                          MiscUatpErrorCodes.InvalidLineItemPOLineItemNumber, ErrorStatus.X, lineItem.LineItemNumber, 0);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }


            //CMP #636: Standard Update Mobilization
            //These validation will apply only for misc category. And charge category and charge code should not be null.
            //Logic:
            /*
             * If charge code type has been defined for combination of charge category and charge code.
             *    If charge code type requirement is not active for combination of charge category and charge code.
             *       If charge code type provided in the file then we will raise error for that scenario.
             *    Else 
             *        If charge code type is provided in file
             *             If active charge code type is not exist in the master 
             *               then raise error.
             *             Else
             *               If charge code type is mismatch with DB charge code type
             *                 then raise error.
             *       ELSE IF(charge code type requirement is mandatory for combination of charge category and charge code.)
             *              raise error as non correctable because charge code type is not  provided in the file.
             * Else (charge code type provided in the file then we will raise error for that scenario.)
            */
            if (miscUatpInvoice.BillingCategory == BillingCategoryType.Misc && miscUatpInvoice.ChargeCategory != null && lineItem.ChargeCode != null)
            {
              var chargeCode = ValidationCache.Instance.ChargeCodeList.Where(i => i.IsActive && i.Name.ToUpper() == lineItem.ChargeCode.Name.ToUpper() && i.ChargeCategoryId == miscUatpInvoice.ChargeCategoryId).FirstOrDefault(); ;
              if (chargeCode != null)
              {
               // Logger.InfoFormat("Charge Code Name: {0}, IsActive: {1}, Mandatory: {2}", chargeCode.Name, chargeCode.IsActiveChargeCodeType, chargeCode.IsChargeCodeTypeRequired);
            
              if (chargeCode.IsChargeCodeTypeRequired != null)
              {
                if (!chargeCode.IsActiveChargeCodeType)
                {
                  if (lineItem.ChargeCodeType != null)
                  {
                    var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Charge Code Type", lineItem.ChargeCodeType.Name,
                                                                                           fileName, ErrorLevels.ErrorLevelLineItem,
                                                                                           MiscUatpErrorCodes.InvalidChargeCodeTypeForChargeCode, ErrorStatus.X, lineItem.LineItemNumber, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                  }
                }
                else
                {
                  if (lineItem.ChargeCodeType != null)
                  {
                    var chargeCodeType = ValidationCache.Instance.ChargeCodeTypeList.Where(i => i.IsActive && i.ChargeCodeId == lineItem.ChargeCodeId).ToList();
                    if (chargeCodeType.Count == 0)
                    {
                      var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Charge Code Type", lineItem.ChargeCodeType.Name,
                                                                                         fileName, ErrorLevels.ErrorLevelLineItem,
                                                                                         MiscUatpErrorCodes.InvalidChargeCodeType, ErrorStatus.X, lineItem.LineItemNumber, 0);
                      exceptionDetailsList.Add(validationExceptionDetail);
                      isValid = false;
                    }
                    else
                    {
                      var chargeCodeTypeCount = chargeCodeType.Count(c => c.Name.ToUpper() == lineItem.ChargeCodeType.Name.ToUpper());
                      if (chargeCodeTypeCount == 0)
                      {
                        var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Charge Code Type", lineItem.ChargeCodeType.Name,
                                                                                         fileName, ErrorLevels.ErrorLevelLineItem,
                                                                                         MiscUatpErrorCodes.InvalidChargeCodeType, ErrorStatus.X, lineItem.LineItemNumber, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                      }
                    }
                  }
                  else if (chargeCode.IsChargeCodeTypeRequired.Value)
                  {
                    var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Charge Code Type", String.Empty,
                                                                                        fileName, ErrorLevels.ErrorLevelLineItem,
                                                                                       MiscErrorCodes.RequiredChargeCodeType, ErrorStatus.X, lineItem.LineItemNumber, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                  }
                }
              }
              else
              {
                if (lineItem.ChargeCodeType != null)
                {
                  var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Charge Code Type", lineItem.ChargeCodeType.Name,
                                                                                           fileName, ErrorLevels.ErrorLevelLineItem,
                                                                                           MiscUatpErrorCodes.InvalidChargeCodeTypeForChargeCode, ErrorStatus.X, lineItem.LineItemNumber, 0);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
                }
              }
              }
            }

            //CMP #636: Standard Update Mobilization
            if (miscUatpInvoice.BillingCategory == BillingCategoryType.Uatp && !lineItem.ChargeCodeTypeId.HasValue && lineItem.ChargeCodeType != null)
            {
              // Invalid charge code type validation
              var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Charge Code Type", lineItem.ChargeCodeType.Name,
                                                                         fileName, ErrorLevels.ErrorLevelLineItem,
                                                                         MiscUatpErrorCodes.InvalidChargeCodeType, ErrorStatus.X, lineItem.LineItemNumber, 0);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }

            // Web specification 2.i
            // Original Line Item Number validation
            // This should be displayed only for a Rejection Invoice
            if ((miscUatpInvoice.InvoiceType != InvoiceType.RejectionInvoice) && lineItem.OriginalLineItemNumber.HasValue)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Original Line Item Number", lineItem.POLineItemNumber.ToString(),
                                                                           fileName, ErrorLevels.ErrorLevelLineItem,
                                                                           MiscUatpErrorCodes.InvalidLineItemOriginalLineItemNumber, ErrorStatus.X, lineItem.LineItemNumber, 0);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }
            //CMP#502 : Rejection Reason for MISC Invoices
            if (miscUatpInvoice.BillingCategory == BillingCategoryType.Misc)
            {
                ValidateRejectionReasonCode(lineItem, exceptionDetailsList, fileName, miscUatpInvoice, fileSubmissionDate);
            }

            // Web specification 2.m
            bool isUomCodeSame = true, isUnitPriceSame = true, isScalingFactorSame = true, isMinimumQuantityFlagFalse = true;
            GetLineItemDetailsUnitsEquality(lineItem, ref isUomCodeSame, ref isUnitPriceSame, ref isScalingFactorSame, ref isMinimumQuantityFlagFalse);

            // This will be derived from the line item detail level.
            if (lineItem.LineItemDetails.Count > 0)
            {
                if (IsFieldMetaDataExists(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, miscUatpInvoice))
                {

                    var calcQuantity = ConvertUtil.Round(lineItem.LineItemDetails.Sum(i => i.Quantity), Constants.MiscDecimalPlaces);
                    var calcTotal = lineItem.LineItemDetails.Sum(i => i.ChargeAmount);

                    // Total validation
                    //if (lineItem.ChargeAmount != calcTotal)
                    if (!CompareUtil.Compare(lineItem.ChargeAmount, calcTotal, summationTolerance, Constants.MiscDecimalPlaces))
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Total", lineItem.ChargeAmount.ToString(),
                                                                                fileName, ErrorLevels.ErrorLevelLineItem,
                                                                                MiscUatpErrorCodes.InvalidLineItemTotal, ErrorStatus.X, lineItem.LineItemNumber, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                }
                else
                {
                    // If there is no line item detail expected for the Charge Category Charge Code and user captures a line item detail 
                    // then the system will validate if the Line Total is equal to the sum of the related Line Detail Totals.
                    var total = lineItem.LineItemDetails.Sum(i => i.ChargeAmount);

                    // Total validation
                    //if (lineItem.ChargeAmount != total)
                    if (!CompareUtil.Compare(lineItem.ChargeAmount, total, summationTolerance, Constants.MiscDecimalPlaces))
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Total", lineItem.ChargeAmount.ToString(),
                                                                                fileName, ErrorLevels.ErrorLevelLineItem,
                                                                                MiscUatpErrorCodes.InvalidLineItemTotal, ErrorStatus.X, lineItem.LineItemNumber, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                }
            }

            // LineItem Detail Validations inside this lineItem
            foreach (var lineItemDetail in lineItem.LineItemDetails)
            {
                ValidateParsedLineItemDetail(lineItemDetail, exceptionDetailsList, fileName, lineItem, miscUatpInvoice, fileSubmissionDate);
            }

            UpdateParsedLineItemStatus(lineItem, miscUatpInvoice, exceptionDetailsList);

            return isValid;
        }

        //CMP#502 : [3.4] Rejection Reason for MISC Invoices
        /// <summary>
        /// Validates the rejection reason code.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <param name="exceptionDetailsList">The exception details list.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
        /// <param name="fileSubmissionDate">The file submission date.</param>
        /// <param name="isIsWebReq">if set to <c>true</c> [is is web req].</param>
        /// <returns></returns>
        public bool ValidateRejectionReasonCode(LineItem lineItem, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, MiscUatpInvoice miscUatpInvoice, DateTime fileSubmissionDate, bool isIsWebReq = false)
        {
            bool isValid = true;
            if (miscUatpInvoice.BillingCategory != BillingCategoryType.Misc)
            {
                return isValid;
            }
            //Invoice Type Rejection
            if (miscUatpInvoice.InvoiceType == InvoiceType.RejectionInvoice)
            {
                //RejectionReasonCode is Provided
                if (!string.IsNullOrWhiteSpace(lineItem.RejectionReasonCode))
                {
                    var transactionTypeId = (int)TransactionType.MiscRejection1;
                    if(miscUatpInvoice.RejectionStage==2)
                    {
                        transactionTypeId = (int)TransactionType.MiscRejection2;
                    }
                    //Validate Reason Code with Master "MST_REASON_CODE" 
                    var reasonCode =
                        ReasonCodeRepository.Get(
                            reasonCodeRecoord =>
                            reasonCodeRecoord.IsActive && reasonCodeRecoord.Code.Equals(lineItem.RejectionReasonCode) && reasonCodeRecoord.TransactionTypeId == transactionTypeId).FirstOrDefault();
                    if (reasonCode != null)
                    {
                        lineItem.RejReasonCodeDescription = reasonCode.Description;
                    }
                    else
                    {
                        if(isIsWebReq)
                        {
                            throw new ISBusinessException(MiscErrorCodes.InvalidRejReasonCodeProvidedIsWeb);
                        }
                       var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(),
                                                                           exceptionDetailsList.Count() + 1,
                                                                           fileSubmissionDate, miscUatpInvoice,
                                                                           "Rejection Reason Code",
                                                                           lineItem.RejectionReasonCode, 
                                                                           fileName,
                                                                           ErrorLevels.ErrorLevelLineItem,
                                                                           MiscErrorCodes.InvalidRejReasonCodeProvided,
                                                                           ErrorStatus.X,
                                                                           lineItem.LineItemNumber,
                                                                           0);
                       exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                }
                //RejectionReasonCode is NOT Provided
                else
                {
                    if (isIsWebReq)
                    {
                        throw new ISBusinessException(MiscErrorCodes.RejReasonCodeNotProvidedRejInvoice);
                    }
                    var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(),
                                                                          exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate, miscUatpInvoice,
                                                                          "Rejection Reason Code",
                                                                           lineItem.RejectionReasonCode,
                                                                           fileName,
                                                                          ErrorLevels.ErrorLevelLineItem,
                                                                          MiscErrorCodes.RejReasonCodeNotProvidedRejInvoice,
                                                                          ErrorStatus.X,
                                                                          lineItem.LineItemNumber,
                                                                          0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }
            //Invoice Type Original or CreditNode or Correspondence Invoice
            else
            {
                //RejectionReasonCode is Provided
                if (!string.IsNullOrWhiteSpace(lineItem.RejectionReasonCode))
                {
                    if (isIsWebReq)
                    {
                        throw new ISBusinessException(MiscErrorCodes.RejReasonCodeProvidedNonRejInvoice);
                    }
                    var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(),
                                                                          exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate, miscUatpInvoice,
                                                                          "Rejection Reason Code",
                                                                           lineItem.RejectionReasonCode,
                                                                           fileName,
                                                                          ErrorLevels.ErrorLevelLineItem,
                                                                          MiscErrorCodes.RejReasonCodeProvidedNonRejInvoice,
                                                                          ErrorStatus.X,
                                                                          lineItem.LineItemNumber,
                                                                          0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }
            return isValid;
        }

        /// <summary>
        /// This method will validate MiscUatp Bm linking process
        /// </summary>
        /// <param name="invoiceHeader">MiscUatpInvoice</param>
        /// <returns></returns>
        public bool ValidateMiscUatCorrespondenceLinking(MiscUatpInvoice invoiceHeader)
        {
            var isCorrespondenceInvoicePresent = CheckDuplicateCorrespondenceInvoice(invoiceHeader, true);
            if (isCorrespondenceInvoicePresent)
            {
                throw new ISBusinessException(MiscUatpErrorCodes.DuplicateInvoiceForCorrespondenceRefNo);
            }

            // Validation for correspondence reference number given has valid for time limit.
            if (ValidateCorrespondenceTimeLimit(invoiceHeader))
            {
                throw new ISBusinessException(MiscUatpErrorCodes.TimeLimitExpiryForCorrespondence);
            }

            ValidateMiscUatpCorrespondenceDetails(invoiceHeader);

            return true;
        }

        /// <summary>
        /// Validates the Correspondence amount to be settled,CorrespondenceStatus and CorrespondenceSubStatus
        /// </summary>
        /// <param name="invoice"></param>
        /// <returns></returns>
        private bool ValidateMiscUatpCorrespondenceDetails(MiscUatpInvoice invoice)
        {
            var miscCorrespondence =
                // MiscCorrespondenceRepository.Get(correspondence => correspondence.CorrespondenceNumber == correspondenceRefNo).OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();
                // Call replaced by Load strategy
              MiscCorrespondenceRepository.Get(correspondenceNumber: invoice.CorrespondenceRefNo).OrderByDescending(correspondence => correspondence.CorrespondenceStage).FirstOrDefault();



            // if correspondence is not present.
            if (miscCorrespondence == null)
            {
                throw new ISBusinessException(MiscUatpErrorCodes.InvalidCorrRefNo);
            }

            // if correspondence is in closed state.
            if (miscCorrespondence.CorrespondenceStatus == CorrespondenceStatus.Closed)
            {
                throw new ISBusinessException(MiscUatpErrorCodes.CorrRefNoClosed);
            }

            // Fetch the invoice specified in the correspondence.
            // Replaced with LoadStrategy call
            var rejectedInvoice = MiscUatpInvoiceRepository.Single(invoiceId: miscCorrespondence.InvoiceId, billingCategoryId: (int)invoice.BillingCategoryId);

            if (rejectedInvoice == null || rejectedInvoice.InvoiceNumber.CompareTo(invoice.RejectedInvoiceNumber) != 0)
            {
                throw new ISBusinessException(MiscUatpErrorCodes.RejInvNotFoundForCorrRefNo);
            }
            //CMP#624 : 2.20 MISC/UATP IS-WEB Error Correction Screens.
            //Change # 3,4
            if (!ValidateSmiAfterLinking(invoice.SettlementMethodId, rejectedInvoice.SettlementMethodId))
            {
              if (invoice.SettlementMethodId == (int)SMI.IchSpecialAgreement)
              {
                throw new ISBusinessException(MiscUatpErrorCodes.MuCorrespondenceInvoiceSmiCheckForSmiX);
              }
              else
              {
                throw new ISBusinessException(MiscUatpErrorCodes.MuCorrespondenceInvoiceLinkingCheckForSmiOtherThanX);
              }
            }

            // if Authority to Bill flag is set in the correspondence
            if (miscCorrespondence.AuthorityToBill)
            {
                if (miscCorrespondence.CorrespondenceSubStatus != CorrespondenceSubStatus.Responded)
                {
                    throw new ISBusinessException(MiscUatpErrorCodes.CorrRefNoOpen);
                }

                // check if From member is equal billed member and to member is equal to billing member.
                if (miscCorrespondence.FromMemberId != invoice.BilledMemberId || miscCorrespondence.ToMemberId != invoice.BillingMemberId)
                {
                    throw new ISBusinessException(MiscUatpErrorCodes.CorrRefNoInvalidMembers);
                }
            }
            if (miscCorrespondence.CorrespondenceStatus == CorrespondenceStatus.Expired)
            {
                // in case of Expired Correspondence, check if to member is equal billed member and from member is equal to billing member.
                if (miscCorrespondence.ToMemberId != invoice.BilledMemberId || miscCorrespondence.FromMemberId != invoice.BillingMemberId)
                {
                    throw new ISBusinessException(MiscUatpErrorCodes.CorrRefNoInvalidMembers);
                }
            }

            // correspondence not yet expired with no authority to bill flag set, 
            // can not create correspondence invoice
            if (miscCorrespondence.CorrespondenceStatus == CorrespondenceStatus.Open && !miscCorrespondence.AuthorityToBill)
            {
                throw new ISBusinessException(MiscUatpErrorCodes.CorrRefNoOpen);
            }

            //SCP219674 : InvalidAmountToBeSettled Validation
            #region Old Code for Validatation of CorrespondenceAmounttobeSettled
           /* if (!ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId))
            {
                var invoiceSummary = InvoiceSummaryRepository.First(i => i.InvoiceId == invoice.Id);
                if (invoiceSummary != null)
                {
                    decimal invoiceAmountToBeCompared;
                    // Case 1: When listing currency and correspondence currency match.
                    if (miscCorrespondence.CurrencyId == invoice.ListingCurrencyId)
                    {
                        invoiceAmountToBeCompared = invoiceSummary.TotalAmount;
                        if (ConvertUtil.Round(miscCorrespondence.AmountToBeSettled, Constants.MiscDecimalPlaces) !=
                            ConvertUtil.Round(invoiceAmountToBeCompared, Constants.MiscDecimalPlaces))
                        {
                            throw new ISBusinessException(MiscUatpErrorCodes.InvalidAmountToBeBilled);
                        }
                    }
                    // Case 2: When clearance currency and correspondence currency match.
                    else if (miscCorrespondence.CurrencyId == invoice.BillingCurrencyId)
                    {
                        if (invoiceSummary.TotalAmountInClearanceCurrency.HasValue)
                        {
                            invoiceAmountToBeCompared = invoiceSummary.TotalAmountInClearanceCurrency.Value;
                            if (ConvertUtil.Round(miscCorrespondence.AmountToBeSettled, Constants.MiscDecimalPlaces) !=
                                ConvertUtil.Round(invoiceAmountToBeCompared, Constants.MiscDecimalPlaces))
                            {
                                throw new ISBusinessException(MiscUatpErrorCodes.InvalidAmountToBeBilled);
                            }
                        }
                    }
                    else // Currency Conversion from Correspondence Currency to Clearance currency
                    {

                        var originalInvoice = GetOriginalInvoice(invoice, miscCorrespondence);

                        if (invoice.BillingCurrencyId.HasValue && miscCorrespondence.CurrencyId.HasValue && invoiceSummary.TotalAmountInClearanceCurrency.HasValue)
                        {
                            double exchangeRate = 0;
                            if (originalInvoice != null)
                            {
                                exchangeRate = ReferenceManager.GetExchangeRate(invoice.BillingCurrencyId.Value,
                                                                                (BillingCurrency)miscCorrespondence.CurrencyId.Value,
                                                                                originalInvoice.BillingYear,
                                                                                originalInvoice.BillingMonth);
                            }

                            // Convert Correspondence amount to Clearance currency.
                            var amountInClearanceCurrency = exchangeRate > 0
                                                              ? miscCorrespondence.AmountToBeSettled * Convert.ToDecimal(exchangeRate)
                                                              : miscCorrespondence.AmountToBeSettled;

                            if (
                              ConvertUtil.Round(invoiceSummary.TotalAmountInClearanceCurrency.Value, Constants.MiscDecimalPlaces) !=
                              ConvertUtil.Round(amountInClearanceCurrency, Constants.MiscDecimalPlaces))
                            {
                                throw new ISBusinessException(MiscUatpErrorCodes.InvalidAmountToBeBilled);
                            }
                        }
                    }
                }
            } */
            #endregion
            #region New Code for Validatation of CorrespondenceAmounttobeSettled
            if (!GetIsValidCorrAmountToBeSettled(invoice, miscCorrespondence))
            {
              throw new ISBusinessException(MiscUatpErrorCodes.InvalidAmountToBeBilled);
            }
            #endregion
            return true;
        }

        /// <summary>
        /// This method will validate MiscUatp Rm linking process
        /// </summary>
        /// <param name="invoiceHeader">MiscUatpInvoice</param>
        /// <returns></returns>
        public bool ValidateMiscUatpRmLinking(MiscUatpInvoice invoiceHeader)
        {
            if (invoiceHeader != null)
            {
                if (IsInvoiceAlreadyRejected(invoiceHeader))
                {
                    throw new ISBusinessException(MiscUatpErrorCodes.InvoiceAlreadyRejected);
                }
                //else if (IsRejectionInvoiceExistsWithInvoiceStatusOtherThanPresented(invoiceHeader))
                //{
                //    throw new ISBusinessException(MiscUatpErrorCodes.InvoiceRejectedForPayablesAndStatusIsPresented);
                //}


                var linkedInvoice = GetMUInvoicePriviousTransanction(invoiceHeader);
                // Correspondence invoice cannot be rejected.
                // Check if the rejected invoice number is not that of a correspondence invoice.
                if (linkedInvoice != null)
                {
                    if (linkedInvoice.InvoiceTypeId == (int) InvoiceType.CorrespondenceInvoice)
                    {
                        throw new ISBusinessException(MiscUatpErrorCodes.CorrespondenceInvoiceCannotBeRejected);
                    }

                    if (linkedInvoice.InvoiceStatus == InvoiceStatusType.Open)
                    {
                        throw new ISBusinessException(MiscUatpErrorCodes.OpenInvoiceCannotBeRejected);
                    }
                    //SCP0000:Impact on MISC/UATP rejection linking due to purging
                    if (IsLinkedInvoicePurged(linkedInvoice))
                    {
                        throw new ISBusinessException(MiscUatpErrorCodes.RejectionInvoiceNumberNotExist);
                    }
                    //CMP#624 : 2.20 SMI Check after linking
                    //Change # 1,2
                    if (!ValidateSmiAfterLinking(invoiceHeader.SettlementMethodId, linkedInvoice.SettlementMethodId))
                    {
                        if (invoiceHeader.SettlementMethodId == (int) SMI.IchSpecialAgreement)
                        {
                            throw new ISBusinessException(MiscUatpErrorCodes.MuRejctionInvoiceSmiCheckForSmiX);
                        }
                        else
                        {
                            throw new ISBusinessException(MiscUatpErrorCodes.MuRejctionInvoiceSmiCheckForSmiOtherThanX);
                        }
                    }

                    // CMP#678: Time Limit Validation on Last Stage MISC Rejections

                    #region CMP #678: Time Limit Validation on Last Stage MISC Rejections

                    if (invoiceHeader.BillingCategoryId == (int)BillingCategoryType.Misc)
                    {
                        /*Online Error Correction Screen*/
                        var rmInTimeLimtMsg = ValidateMiscLastStageRmForTimeLimit(linkedInvoice,
                                                                                  invoiceHeader,
                                                                                  RmValidationType.ErrorCorrectionScreen);
                        if (!string.IsNullOrWhiteSpace(rmInTimeLimtMsg))
                        {
                            throw new ISBusinessException(null, rmInTimeLimtMsg);
                        }
                    }

                    #endregion

                }
                else
                {
                    // Linking process validation if Rejection Flag is "Y".
                    // Billed member has migrated but could not find the data of the rejected invoice, throw an error.
                    if (!SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod)
                    {
                        if (IsMemberMigrated(invoiceHeader))
                        {
                            throw new ISBusinessException(MiscUatpErrorCodes.InvalidBilledMemberMigrationStatusForRejectedInvoice);
                        }
                    }

                    #region CMP#678: Time Limit Validation on Last Stage MISC Rejections

                    if (invoiceHeader.BillingCategoryId == (int)BillingCategoryType.Misc)
                    {
                        /*Online Error Correction Screen*/
                        var rejectedInvoiceDetails = new MiscUatpInvoice();
                        rejectedInvoiceDetails.BillingYear = invoiceHeader.SettlementYear;
                        rejectedInvoiceDetails.BillingMonth = invoiceHeader.SettlementMonth;
                        rejectedInvoiceDetails.BillingPeriod = invoiceHeader.SettlementPeriod;

                        var rmInTimeLimtMsg = ValidateMiscLastStageRmForTimeLimit(rejectedInvoiceDetails,
                                                                                  invoiceHeader,
                                                                                  RmValidationType.ErrorCorrectionScreen);
                        if (!string.IsNullOrWhiteSpace(rmInTimeLimtMsg))
                        {
                            throw new ISBusinessException(null, rmInTimeLimtMsg);
                        }
                    }

                    #endregion
                }

                // Rejection stage 2 not allowed for ICH/Bilateral settlement methods.
                /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like Bilateranl or ICH (anything but ACH) */
                if ((invoiceHeader.InvoiceSmi == SMI.Ich || ReferenceManager.IsSmiLikeBilateral(invoiceHeader.SettlementMethodId, true) ||
                     invoiceHeader.InvoiceSmi == SMI.AchUsingIataRules) && invoiceHeader.RejectionStage == 2)
                {
                    throw new ISBusinessException(MiscUatpErrorCodes.InvalidRejectionStage);
                }

                var settlementMonthPeriod = new BillingPeriod
                {
                    Period = invoiceHeader.SettlementPeriod,
                    Month = invoiceHeader.SettlementMonth,
                    Year = invoiceHeader.SettlementYear
                };

                int settlementMethodId;
                if (linkedInvoice != null)
                {
                    settlementMethodId = linkedInvoice.SettlementMethodId;
                }
                else
                {
                    settlementMethodId = invoiceHeader.SettlementMethodId;
                }

                var currentBillingPeriod =
                  CalendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(
                    ReferenceManager.GetClearingHouseToFetchCurrentBillingPeriod(settlementMethodId));
                if (settlementMonthPeriod > currentBillingPeriod)
                {
                    throw new ISBusinessException(MiscUatpErrorCodes.InvalidSettlementMonthPeriod);
                }

                //// Invoice number and rejected invoice number can not be same. 
                //if (invoiceHeader.InvoiceNumber.ToLower().Equals(invoiceHeader.RejectedInvoiceNumber))
                //{
                //  throw new ISBusinessException(MiscUatpErrorCodes.InvalidRejectionInvoiceNumber);
                //}

                invoiceHeader.ValidationDate = new DateTime(invoiceHeader.SettlementYear, invoiceHeader.SettlementMonth, invoiceHeader.SettlementPeriod);
                // Transaction out side time limit validation.
                if (IsTransactionOutSideTimeLimit(invoiceHeader))
                {
                    if (invoiceHeader.IsValidationFlag == null || (!string.IsNullOrEmpty(invoiceHeader.IsValidationFlag) && !invoiceHeader.IsValidationFlag.Contains(TimeLimitFlag)))
                        invoiceHeader.IsValidationFlag += string.IsNullOrEmpty(invoiceHeader.IsValidationFlag)
                                                          ? TimeLimitFlag
                                                          : ValidationFlagDelimeter + TimeLimitFlag;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// This will check whether corresponding values are same for each lineItemdetail of a lineItem
        /// </summary>
        /// <param name="lineItem">LineItem</param>
        /// <param name="uomCode">will be false if it is different for any lineItemDetail under a given lineItem</param>
        /// <param name="unitPrice">will be false if it is different for any lineItemDetail under a given lineItem</param>
        /// <param name="scalingFactor">will be false if it is different for any lineItemDetail under a given lineItem</param>
        /// <param name="minimumQuantityFlag">This will be true if all lineItemDetail minimumQuantityFlag are false</param>
        private void GetLineItemDetailsUnitsEquality(LineItem lineItem, ref bool uomCode, ref bool unitPrice, ref bool scalingFactor, ref bool minimumQuantityFlag)
        {
            string uomCodeId = string.Empty;
            decimal unitPriceVal = 0;
            int sfVal = 0;
            int linenumber = 1;

            foreach (var lineItemDetail in lineItem.LineItemDetails)
            {
                if (linenumber != 1)
                {
                    int thisSF = lineItemDetail.ScalingFactor.HasValue ? lineItemDetail.ScalingFactor.Value : 0;
                    string thisUomCodeId = !string.IsNullOrEmpty(lineItemDetail.UomCodeId) ? lineItemDetail.UomCodeId : string.Empty;

                    if (unitPriceVal != ConvertUtil.Round(lineItemDetail.UnitPrice, Constants.MiscDecimalPlaces))
                    {
                        unitPrice = false;
                    }

                    if (sfVal != thisSF)
                    {
                        scalingFactor = false;
                    }

                    if (uomCodeId != thisUomCodeId)
                    {
                        uomCode = false;
                    }

                    if (lineItemDetail.MinimumQuantityFlag)
                    {
                        minimumQuantityFlag = false;
                    }
                }

                unitPriceVal = ConvertUtil.Round(lineItemDetail.UnitPrice, Constants.MiscDecimalPlaces);
                if (lineItemDetail.ScalingFactor.HasValue)
                {
                    sfVal = lineItemDetail.ScalingFactor.Value;
                }

                if (!string.IsNullOrEmpty(lineItemDetail.UomCodeId))
                {
                    uomCodeId = lineItemDetail.UomCodeId;
                }

                linenumber++;
            }
        }

        /// <summary>
        /// This method calculates and assigns TotalTaxAmount,TotalVATAmount,TotalAddOnChargeAmount,TotalNetAmount
        /// </summary>
        /// <param name="lineItem">Line Item</param>
        /// <param name="totalTaxAmount"></param>
        /// <param name="totalVatAmount"></param>
        /// <param name="totalAddOnChargeAmount"></param>
        /// <param name="totalNetAmount"></param>
        private void GetLineItemTotalFields(LineItem lineItem, ref decimal totalTaxAmount, ref decimal totalVatAmount, ref decimal totalAddOnChargeAmount, ref decimal totalNetAmount)
        {
            if (lineItem != null)
            {
                // Logic : totalTaxAmount/totalVatAmount :- Summation of TaxAmount at the Line Item Detail level and Line Item Level       
                totalTaxAmount = lineItem.TaxBreakdown.Where(tax => tax.Type.ToUpper() == "TAX" && tax.CalculatedAmount.HasValue).Sum(lineItemTax => lineItemTax.CalculatedAmount.Value) + lineItem.LineItemDetails.Sum(lineItemDetail => lineItemDetail.TotalTaxAmount.HasValue ? lineItemDetail.TotalTaxAmount.Value : 0);
                totalVatAmount = lineItem.TaxBreakdown.Where(tax => tax.Type.ToUpper() == "VAT" && tax.CalculatedAmount.HasValue).Sum(lineItemTax => lineItemTax.CalculatedAmount.Value) + lineItem.LineItemDetails.Sum(lineItemDetail => lineItemDetail.TotalVatAmount.HasValue ? lineItemDetail.TotalVatAmount.Value : 0);

                // Logic : totalAddOnChargeAmount :- Should equal the Summation of AddOnChargeAmount at the Line Item Detail level and Line Item Level
                totalAddOnChargeAmount = lineItem.AddOnCharges.Sum(lineItemAddOnCharge => lineItemAddOnCharge.Amount) +
                lineItem.LineItemDetails.SelectMany(l => l.AddOnCharges).Aggregate<LineItemDetailAddOnCharge, decimal>(0, (current, variable) => current + variable.Amount);

                // Logic : totalNetAmount :- "Should be equal to ChargeAmount+TotalTaxAmount+TotalVATAmount+TotalAddOnChargeAmount."
                totalNetAmount = lineItem.ChargeAmount + totalTaxAmount + totalVatAmount + totalAddOnChargeAmount;
            }
        }


        /// <summary>
        /// Gets the line item detail total fields.
        /// </summary>
        /// <param name="lineItemDetail">The line item detail.</param>
        /// <param name="totalTaxAmount">The total tax amount.</param>
        /// <param name="totalVatAmount">The total vat amount.</param>
        /// <param name="totalAddOnChargeAmount">The total add on charge amount.</param>
        /// <param name="totalNetAmount">The total net amount.</param>
        private void GetLineItemDetailTotalFields(LineItemDetail lineItemDetail, ref decimal totalTaxAmount, ref decimal totalVatAmount, ref decimal totalAddOnChargeAmount, ref decimal totalNetAmount)
        {
            if (lineItemDetail != null)
            {
                // Logic : totalTaxAmount/totalVatAmount :- Summation of TaxAmount at the Line Item Detail level and Line Item Level       
                totalTaxAmount = lineItemDetail.TaxBreakdown.Where(detailTax => detailTax.Type.ToUpper() == "TAX").Aggregate(totalTaxAmount, (current, detailTax) => current + (detailTax.CalculatedAmount.HasValue ? detailTax.CalculatedAmount.Value : 0));
                totalVatAmount = lineItemDetail.TaxBreakdown.Where(detailTax => detailTax.Type.ToUpper() == "VAT").Aggregate(totalVatAmount, (current, detailTax) => current + (detailTax.CalculatedAmount.HasValue ? detailTax.CalculatedAmount.Value : 0));

                // Logic : totalAddOnChargeAmount :- Should equal the Summation of AddOnChargeAmount at the Line Item Detail level and Line Item Level
                totalAddOnChargeAmount = lineItemDetail.AddOnCharges.Aggregate(totalAddOnChargeAmount, (current, addOnCharge) => current + addOnCharge.Amount);

                // Logic : totalNetAmount :- "Should be equal to ChargeAmount+TotalTaxAmount+TotalVATAmount+TotalAddOnChargeAmount."
                totalNetAmount = lineItemDetail.ChargeAmount + totalTaxAmount + totalVatAmount + totalAddOnChargeAmount;
            }
        }

        /// <summary>
        /// Validates the parsed line item detail.
        /// </summary>
        /// <param name="lineItemDetail">The line item detail.</param>
        /// <param name="exceptionDetailsList">The exception details list.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="lineItem">The line item.</param>
        /// <param name="miscUatpInvoice"></param>
        /// <returns></returns>
        protected bool ValidateParsedLineItemDetail(LineItemDetail lineItemDetail, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, LineItem lineItem, MiscUatpInvoice miscUatpInvoice, DateTime fileSubmissionDate)
        {
            double summationTolerance = 0;
            double roundingTolerance = 0;
            if (miscUatpInvoice.Tolerance != null)
            {
                summationTolerance = miscUatpInvoice.Tolerance.SummationTolerance;
                roundingTolerance = miscUatpInvoice.Tolerance.RoundingTolerance;
            }

            // TODO: LineItem Detail validations
            var isValid = true;

            //CMP-539:If lineitemdetail number is <=0 then add exception 
            // Validation for LineItemDetailNumber   
            if (lineItemDetail.DetailNumber <= 0)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(lineItemDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "LineItemDetail Number", lineItemDetail.DetailNumber.ToString(), fileName, ErrorLevels.ErrorLevelLineItemDetail, MiscErrorCodes.InvalidLineItemDetailNumber, ErrorStatus.X, lineItemDetail.LineItemNumber, lineItemDetail.DetailNumber);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            //SCP220346: Inward Billing-XML file mandatory field.
            //Validated scaling factor value.
            if (lineItemDetail.ScalingFactor.HasValue && lineItemDetail.ScalingFactor.Value == 0)
            {
              IsValidationExceptionDetail validationExceptionDetail;

              if (miscUatpInvoice.BillingCategoryId == (int) BillingCategoryType.Misc)
              {
                validationExceptionDetail = CreateValidationExceptionDetail(lineItemDetail.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate, miscUatpInvoice,
                                                                            "Scaling Factor",
                                                                            lineItemDetail.ScalingFactor.Value.
                                                                              ToString(), fileName,
                                                                            ErrorLevels.ErrorLevelLineItemDetail,
                                                                            new MiscErrorCodes().
                                                                              InvalidScalingFactorValue,
                                                                            ErrorStatus.X,
                                                                            lineItemDetail.LineItemNumber,
                                                                            lineItemDetail.DetailNumber);
              }
              else
              {
                validationExceptionDetail = CreateValidationExceptionDetail(lineItemDetail.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate, miscUatpInvoice,
                                                                            "Scaling Factor",
                                                                            lineItemDetail.ScalingFactor.Value.
                                                                              ToString(), fileName,
                                                                            ErrorLevels.ErrorLevelLineItemDetail,
                                                                            new UatpErrorCodes().
                                                                              InvalidScalingFactorValue,
                                                                            ErrorStatus.X,
                                                                            lineItemDetail.LineItemNumber,
                                                                            lineItemDetail.DetailNumber);

              }
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }

          // Start Date - End Date validation 
            if (lineItem.StartDate != null)
            {
                if (lineItemDetail.StartDate >= lineItemDetail.EndDate)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(lineItemDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Start Date", Convert.ToString(lineItemDetail.StartDate),
                                                                             fileName, ErrorLevels.ErrorLevelLineItemDetail,
                                                                             MiscUatpErrorCodes.InvalidStartDate, ErrorStatus.X, lineItemDetail.LineItemNumber, lineItemDetail.DetailNumber
                                                                             );
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }
            else
            {
                if (lineItemDetail.StartDate != null)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(lineItemDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Start Date", Convert.ToString(lineItemDetail.StartDate),
                                                                             fileName, ErrorLevels.ErrorLevelLineItemDetail,
                                                                             MiscUatpErrorCodes.InvalidLineItemDetailStartDate, ErrorStatus.X, lineItemDetail.LineItemNumber, lineItemDetail.DetailNumber
                                                                             );
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }

            // CMP # 533: RAM A13 New Validations and New Charge Code [Start]
            // 'Product ID' Validation at LineItemDetails level in case of Billing Category is 'Misc' and Charge Category is 'Service Provider' and Charge Code is 'GDS'
            if (miscUatpInvoice.BillingCategoryId == (int)BillingCategoryType.Misc &&
                miscUatpInvoice.ChargeCategoryDisplayName.ToLower().Equals(Constants.ServiceProvider) && lineItem.DisplayChargeCode.ToLower().Equals(Constants.Gds))
            {
                // Get the valid Product ID's from enum into list.
                var productIdList = Enum.GetNames(typeof(ProductId)).ToList();

                // check for the valid Product ID value.
                if (!string.IsNullOrEmpty(lineItemDetail.ProductId) && !productIdList.Contains(lineItemDetail.ProductId))
                {
                    // Create validation details.
                    var validationExceptionDetail = CreateValidationExceptionDetail(lineItemDetail.Id.Value(),
                                                                                    exceptionDetailsList.Count() + 1,
                                                                                    fileSubmissionDate,
                                                                                    miscUatpInvoice,
                                                                                    "Product ID",
                                                                                    Convert.ToString(lineItemDetail.ProductId),
                                                                                    fileName,
                                                                                    ErrorLevels.ErrorLevelLineItemDetail,
                                                                                    MiscErrorCodes.InvalidProductId,
                                                                                    ErrorStatus.X,
                                                                                    lineItemDetail.LineItemNumber,
                                                                                    lineItemDetail.DetailNumber
                                                                                   );
                    // Add vlidation details to the exception details list.
                    exceptionDetailsList.Add(validationExceptionDetail);

                    // make isValid flag false.
                    isValid = false;
                } // End if
            } // End if
            // CMP # 533: RAM A13 New Validations and New Charge Code [End]

            // if Minimum quantity flag is false then only perform charge amount validation
            if (!lineItemDetail.MinimumQuantityFlag || miscUatpInvoice.BillingCategory == BillingCategoryType.Uatp)
            {
                var calculatedChargeAmount = (lineItemDetail.UnitPrice * lineItemDetail.Quantity);

                if (lineItemDetail.ScalingFactor.HasValue && lineItemDetail.ScalingFactor.Value != 0)
                {
                    calculatedChargeAmount = Convert.ToDecimal(calculatedChargeAmount / lineItemDetail.ScalingFactor);
                }
                if (!CompareUtil.Compare(ConvertUtil.Round(calculatedChargeAmount, Constants.MiscDecimalPlaces), lineItemDetail.ChargeAmount, roundingTolerance, Constants.MiscDecimalPlaces))
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(lineItemDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Charge Amount", Convert.ToString(lineItemDetail.ChargeAmount),
                                                                   fileName, ErrorLevels.ErrorLevelLineItemDetail,
                                                                   MiscUatpErrorCodes.InvalidChargeAmount, ErrorStatus.X, lineItemDetail.LineItemNumber, lineItemDetail.DetailNumber);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }

            // Validate UOM Code
            if (string.IsNullOrEmpty(lineItemDetail.UomCodeId) && lineItem.UomCode != null)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(lineItemDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "UOM Code", lineItem.UomCode.Id,
                fileName, ErrorLevels.ErrorLevelLineItemDetail,
                MiscUatpErrorCodes.InvalidUomCode, ErrorStatus.X, lineItem.LineItemNumber, lineItemDetail.DetailNumber);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }


            // Validate Line Item Detail Total
            decimal calculatedTotalTaxAmount = 0, calculatedTotalVatAmount = 0, calculatedTotalAddOnChargeAmount = 0, calculatedTotalNetAmount = 0;
            GetLineItemDetailTotalFields(lineItemDetail, ref calculatedTotalTaxAmount, ref calculatedTotalVatAmount, ref calculatedTotalAddOnChargeAmount, ref calculatedTotalNetAmount);

            if (!CompareUtil.Compare(calculatedTotalNetAmount, lineItemDetail.TotalNetAmount, summationTolerance, Constants.MiscDecimalPlaces))
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(lineItemDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Total Net Amount", Convert.ToString(lineItemDetail.TotalNetAmount),
                                                               fileName, ErrorLevels.ErrorLevelLineItemDetail,
                                                               MiscUatpErrorCodes.InvalidTotalAmount, ErrorStatus.X, lineItemDetail.LineItemNumber, lineItemDetail.DetailNumber
                                                               );
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            // Validate Add on Charge 
            foreach (var addoncharge in lineItemDetail.AddOnCharges)
            {
                ValidateParsedAddonCharge(addoncharge, exceptionDetailsList, fileName, miscUatpInvoice, fileSubmissionDate, ErrorLevels.ErrorLevelLineItemDetailAddOnCharge, roundingTolerance, lineItem, lineItemDetail);
            }

            // Vat-tax Validations
            foreach (var tax in lineItemDetail.TaxBreakdown)
            {
                if (tax.Type.ToLower() == "vat")
                {
                    ValidateParsedTax(tax, exceptionDetailsList, fileName, miscUatpInvoice, fileSubmissionDate, ErrorLevels.ErrorLevelLineItemDetailVat, roundingTolerance, lineItem, lineItemDetail);
                }
                else
                {
                    ValidateParsedTax(tax, exceptionDetailsList, fileName, miscUatpInvoice, fileSubmissionDate, ErrorLevels.ErrorLevelLineItemDetailTax, roundingTolerance, lineItem, lineItemDetail);
                }
            }

            ValidateParsedDynamicData(lineItemDetail.FieldValues, exceptionDetailsList, fileName, miscUatpInvoice, ErrorLevels.ErrorLevelLineItemDetailTax, lineItem, lineItemDetail, fileSubmissionDate);

            return isValid;
        }

        /// <summary>
        /// Validates the parsed addon charge.
        /// </summary>
        /// <param name="addOnCharge">The add on charge.</param>
        /// <param name="exceptionDetailsList">The exception details list.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
        /// <param name="errorLevel">Error Level</param>
        /// <param name="roundingTolerance"></param>
        /// <param name="lineItem">The line item.</param>
        /// <param name="lineItemDetail">The line item detail.</param>
        /// <returns></returns>
        protected bool ValidateParsedAddonCharge(AddOnCharge addOnCharge, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, MiscUatpInvoice miscUatpInvoice, DateTime fileSubmissionDate, string errorLevel, double roundingTolerance, LineItem lineItem = null, LineItemDetail lineItemDetail = null)
        {
            var isValid = true;

            if (addOnCharge.Percentage.HasValue && addOnCharge.ChargeableAmount.HasValue)
            {
                var lineItemNumber = (lineItem != null) ? lineItem.LineItemNumber : 0;
                var lineItemDetailNumber = (lineItemDetail != null) ? lineItemDetail.DetailNumber : 0;

                var calculatedAmount = addOnCharge.ChargeableAmount.Value * (Convert.ToDecimal(addOnCharge.Percentage) / 100);

                if (!CompareUtil.Compare(calculatedAmount, addOnCharge.Amount, roundingTolerance, Constants.MiscDecimalPlaces))
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(addOnCharge.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "AddOn Charge Amount", Convert.ToString(addOnCharge.Amount),
                                                                   fileName, errorLevel,
                                                                   MiscUatpErrorCodes.InvalidAddOnChargeAmount, ErrorStatus.X, lineItemNumber, lineItemDetailNumber);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }

            return isValid;
        }

        /// <summary>
        /// Validates the parsed tax.
        /// </summary>
        /// <param name="miscUatpTax">The misc uatp tax.</param>
        /// <param name="exceptionDetailsList">The exception details list.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
        /// <param name="errorLevel">errorLevel.</param>
        /// <param name="roundingTolerance">The rounding tolerance.</param>
        /// <param name="lineItem">The line item.</param>
        /// <param name="lineItemDetail">The line item detail.</param>
        /// <returns></returns>
        protected bool ValidateParsedTax(MiscUatpTax miscUatpTax, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, MiscUatpInvoice miscUatpInvoice, DateTime fileSubmissionDate, string errorLevel, double roundingTolerance, LineItem lineItem = null, LineItemDetail lineItemDetail = null)
        {
            var isValid = true;
            var lineItemNumber = (lineItem != null) ? lineItem.LineItemNumber : 0;
            var lineItemDetailNumber = (lineItemDetail != null) ? lineItemDetail.DetailNumber : 0;

            if (miscUatpTax.Type.ToLower() == "vat")
            {
                // 5091 - KALEQA  IS-XML  It should not give any error message for TAX SUB TYPE where as TAX SUB TYPE is not a mandatory fields
                // If TAX SUB TYPE is NOT there in the XML then by pass the validation
                if (miscUatpTax.SubType != null)
                {
                    // added taxSubTypes filter for active/inactive status of Tax Sub Type.
                    // CMP 534: TFS Bugs 8723 & 8724.
                    //var taxSubTypes = TaxSubTypeRepository.GetCount(rec => rec.SubType == miscUatpTax.SubType && rec.Type == "V");
                    var taxSubTypes = TaxSubTypeRepository.GetCount(rec => rec.IsActive && rec.SubType == miscUatpTax.SubType && rec.Type == "V");
                    if (taxSubTypes == 0)
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpTax.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Tax Sub Type",
                                                                                        miscUatpTax.SubType,
                                                                                        fileName,
                                                                                        errorLevel,
                                                                                        MiscUatpErrorCodes.InvalidTaxSubType,
                                                                                        ErrorStatus.X,
                                                                                        lineItemNumber,
                                                                                        lineItemDetailNumber);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                }
            }

            // CMP #534: Tax Issues in MISC and UATP Invoices. [Start]
            // Validate TaxSubType field when Tax type is Tax
            if (miscUatpTax.Type.ToLower() == "tax")
            {
              if (miscUatpTax.SubType != null)
              {
                // Check the TaxSubType value with database values.
                var taxSubTypes = TaxSubTypeRepository.GetCount(rec => rec.IsActive && rec.SubType == miscUatpTax.SubType && rec.Type == "T");
                if (taxSubTypes == 0)
                {
                  // Read system parameters validation Params 
                  var validationParameters = Iata.IS.AdminSystem.SystemParameters.Instance.ValidationParams;

                  var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpTax.Id.Value(),
                                                  exceptionDetailsList.Count() + 1,
                                                  fileSubmissionDate, miscUatpInvoice,
                                                  "Tax Sub Type",
                                                  miscUatpTax.SubType,
                                                  fileName,
                                                  errorLevel,
                                                  MiscUatpErrorCodes.InvalidTaxSubType,
                                                  validationParameters.TaxSubTypeOfTaxForMiscUatp ==
                                                  (int)Model.Enums.ValidationParams.Error ? ErrorStatus.X : ErrorStatus.W,
                                                  lineItemNumber,
                                                  lineItemDetailNumber);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
                } // End if
              } // End if
            } // End if
            // CMP #534: Tax Issues in MISC and UATP Invoices. [End]

            // If TAX SUB TYPE is VAT there in the XML then by pass the validation
            if (miscUatpTax.Type.ToLower() == "vat" && miscUatpInvoice.BillingCategoryId == (int)BillingCategoryType.Misc)
            {
                // On the basis of assumption that TaxCategory value will always be dictionary defined value, removing CategoryId check as 
                // for MiscUatp TaxCategoy enum because Tax Category 'Standard' will be mapped to zero (0). In this case below check will fail.
                if (string.IsNullOrEmpty(miscUatpTax.CategoryCode))// || miscUatpTax.CategoryId == 0)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpTax.Id.Value(),
                                                                                    exceptionDetailsList.Count() + 1,
                                                                                    fileSubmissionDate, miscUatpInvoice,
                                                                                    "Tax Category",
                                                                                    miscUatpTax.CategoryCode,
                                                                                    fileName,
                                                                                    errorLevel,
                                                                                    MiscUatpErrorCodes.InvalidTaxCategory,
                                                                                    ErrorStatus.X,
                                                                                    lineItemNumber,
                                                                                    lineItemDetailNumber);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }

            //SCP257502 - VAT NODE MISSING
            if (miscUatpTax.Type.ToLower() == "tax" || (miscUatpTax.Type.ToLower() == "vat" && (miscUatpTax.CategoryId != (int)TaxCategory.Exempt && miscUatpTax.CategoryId != (int)TaxCategory.ReverseCharge)))
            {
              if (miscUatpTax.Percentage.HasValue && miscUatpTax.Amount.HasValue && miscUatpTax.CalculatedAmount == null)
              {
                var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpTax.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Tax Amount",
                                                                                string.Empty,
                                                                                fileName,
                                                                                errorLevel,
                                                                                MiscUatpErrorCodes.MandatoryFieldAttributeMissing,
                                                                                ErrorStatus.X,
                                                                                lineItemNumber,
                                                                                lineItemDetailNumber);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
              }
            }

            // Validation for TaxAmount
            // This validation is not to be done when TaxCategory = 'Exempt' or 'Reverse Charge'
            if ((miscUatpTax.Percentage.HasValue && miscUatpTax.Amount.HasValue && miscUatpTax.Percentage > 0) && (!(miscUatpTax.Type.ToLower() == "vat" && (miscUatpTax.CategoryId == (int)TaxCategory.Exempt || miscUatpTax.CategoryId == (int)TaxCategory.ReverseCharge))))
            {
                var calculatedAmount = Convert.ToDecimal(miscUatpTax.Percentage) * miscUatpTax.Amount.Value / 100;

                // TaxAmount Should be equal to the TaxPercent into TaxableAmount if TaxPercent is provided 
                if (miscUatpTax.CalculatedAmount.HasValue)
                {
                    if (!CompareUtil.Compare(calculatedAmount, miscUatpTax.CalculatedAmount.Value, roundingTolerance, Constants.MiscDecimalPlaces))
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpTax.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Tax Amount",
                                                                                        miscUatpTax.CalculatedAmount.ToString(),
                                                                                        fileName,
                                                                                        errorLevel,
                                                                                        MiscUatpErrorCodes.InvalidTaxAmount,
                                                                                        ErrorStatus.X,
                                                                                        lineItemNumber,
                                                                                        lineItemDetailNumber);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                }
            }
            return isValid;
        }


        /// <summary>
        /// Validates the parsed dynamic structure.
        /// </summary>
        /// <param name="fieldValues">The field values.</param>
        /// <param name="exceptionDetailsList">The exception details list.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
        /// <param name="errorLevel">The error level.</param>
        /// <param name="lineItem">The line item.</param>
        /// <param name="lineItemDetail">The line item detail.</param>
        /// <returns></returns>

        protected bool ValidateParsedDynamicData(List<FieldValue> fieldValues, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, MiscUatpInvoice miscUatpInvoice, string errorLevel, LineItem lineItem, LineItemDetail lineItemDetail, DateTime fileSubmissionDate)
        {
            bool isValid = ValidateParsedDynamicGroups(fieldValues, exceptionDetailsList, fileName, miscUatpInvoice, "", lineItem, lineItemDetail, fileSubmissionDate);

            return isValid;
        }
        /// <summary>
        /// Validates the parsed charge code structure.
        /// </summary>
        /// <param name="fieldValues">The field values.</param>
        /// <param name="exceptionDetailsList">The exception details list.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
        /// <param name="errorLevel">The error level.</param>
        /// <param name="lineItem">The line item.</param>
        /// <param name="lineItemDetail">The line item detail.</param>
        /// <param name="fileSubmissionDate"></param>
        /// <returns></returns>
        private bool ValidateParsedDynamicGroups(List<FieldValue> fieldValues, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, MiscUatpInvoice miscUatpInvoice, string errorLevel, LineItem lineItem, LineItemDetail lineItemDetail, DateTime fileSubmissionDate)
        {
            var isValid = true;
            var zeroArray = new[] { "0", "00", "000" };

            // Mandatory group validations.
            var chargeCodeId = lineItem.ChargeCodeId;
            var chargeCodeTypeId = lineItem.ChargeCodeTypeId;

            var requiredGroupFields = (from fieldMetaData in _fieldMetaDatas
                                       join fieldChargeCodeMapp in _fieldChargeCodeMappings on fieldMetaData.Id equals fieldChargeCodeMapp.FieldMetaDataId
                                       where fieldChargeCodeMapp.ChargeCodeId == chargeCodeId && fieldMetaData.FieldTypeId == (int)FieldType.Group &&
                                       fieldChargeCodeMapp.RequiredTypeId == (int)RequiredType.Mandatory 
                                       && (miscUatpInvoice.BillingCategoryId == (int)BillingCategoryType.Misc ? true : fieldChargeCodeMapp.ChargeCodeTypeId == chargeCodeTypeId)
                                       select fieldMetaData).ToList();

          #region CMP609: MISC Changes Required as per ISW2. (Required Groups)
          // If the Original Billing Month is equal to or earlier than 2014-Apr-P4,
          // then validation should be ignored even if Line Item Details have been provided for the combination of Charge Category/Charge Code
          var checkBillingPeriod = new BillingPeriod(2014, 04, 04);
          // If Billing Category is 'Miscelleneous' and is Rejection Invoice and is Original Billing Month is equal to or earlier than 2014-Apr-P4
          if (miscUatpInvoice.BillingCategoryId == (int)BillingCategoryType.Misc && miscUatpInvoice.InvoiceTypeId == (int)InvoiceType.RejectionInvoice && miscUatpInvoice.SettlementBillingPeriod <= checkBillingPeriod)
          {
            // If Charge Category is 'ATC' (1)
            if(miscUatpInvoice.ChargeCategoryId == 1)
            {
              // If Charge Code is 'Approach'(1) or 'En-Route'(3) or 'Over-flight'(7)
              if (lineItem.ChargeCodeId == 1 || lineItem.ChargeCodeId == 3 || lineItem.ChargeCodeId == 7)
              {
                var ignoreFieldMetaDataItem = requiredGroupFields.Find(groupField => groupField.FieldName.Equals("FlightDetails"));
                if (ignoreFieldMetaDataItem != null)
                {
                  requiredGroupFields.Remove(ignoreFieldMetaDataItem);
                }
              }
              // If Charge Code is 'Over-flight'(7)
              if (lineItem.ChargeCodeId == 7)
              {
                var ignoreFieldMetaDataItem = requiredGroupFields.Find(groupField => groupField.FieldName.Equals("RouteDetails"));
                if (ignoreFieldMetaDataItem != null)
                {
                  requiredGroupFields.Remove(ignoreFieldMetaDataItem);
                }
              }
            }
            
            // If Charge Category is 'Ground Handling'
            if (miscUatpInvoice.ChargeCategoryId == 6)
            {
              // If Charge Code is 'Mishandling Baggage'
              if (lineItem.ChargeCodeId == 45)
              {
                var ignoreFieldMetaDataItem = requiredGroupFields.Find(groupField => groupField.FieldName.Equals("LineItemDetail"));
                if (ignoreFieldMetaDataItem != null)
                {
                  requiredGroupFields.Remove(ignoreFieldMetaDataItem);
                }
              }

              // TFS9089: CMP609: System giving error for mandatory field in Ground Handling-Lounge
              // If Charge Code is 'Lounge'
              if (lineItem.ChargeCodeId == 43)
              {
                var ignoreFieldMetaDataItem = requiredGroupFields.Find(groupField => groupField.FieldName.Equals("PassengerDetails"));
                if (ignoreFieldMetaDataItem != null)
                {
                  requiredGroupFields.Remove(ignoreFieldMetaDataItem);
                }
              }
            }

            // If Charge Category is 'Service Provider'
            if (miscUatpInvoice.ChargeCategoryId == 12)
            {
              // If Charge Code is 'Meetings and Conferences'(84) or 'Training'(86)
              if (lineItem.ChargeCodeId == 84 || lineItem.ChargeCodeId == 86)
              {
                var ignoreFieldMetaDataItem = requiredGroupFields.Find(groupField => groupField.FieldName.Equals("EmployeeDetails"));
                if (ignoreFieldMetaDataItem != null)
                {
                  requiredGroupFields.Remove(ignoreFieldMetaDataItem);
                }
              }
            }
          }
          #endregion
          
          foreach (var requiredGroupField in requiredGroupFields)
            {

                bool requiredFieldGroupPresent = false;

                FieldMetaData field = requiredGroupField;
                foreach (var fieldValue in fieldValues.Where(fieldValue => fieldValue.FieldMetaDataId == field.Id))
                {
                    requiredFieldGroupPresent = true;
                }

                if (requiredFieldGroupPresent == false)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(lineItemDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Line Item Detail-Groups", requiredGroupField.DisplayText, fileName, ErrorLevels.ErrorLevelLineItemDetail, MiscUatpErrorCodes.MandatoryRecGroupsMissing, ErrorStatus.X, lineItemDetail.LineItemNumber, lineItemDetail.DetailNumber);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }

            }
            //if (!requiredGroupFields.All(requiredGroupFieldRec => fieldValues.Select(fieldValuesRec => fieldValuesRec.FieldMetaDataId).Contains(requiredGroupFieldRec.Id)))
            //{
            //  var validationExceptionDetail = CreateValidationExceptionDetail(exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Line Item Detail-Groups", "",
            //                                             fileName, ErrorLevels.ErrorLevelLineItemDetail,
            //                                             MiscUatpErrorCodes.MandatoryRecGroupsMissing,
            //                                             ErrorStatus.X,
            //                                             lineItemDetail.LineItemNumber,
            //                                             lineItemDetail.DetailNumber);
            //  exceptionDetailsList.Add(validationExceptionDetail);
            //  isValid = false;
            //}

            // Get the field metadata for only available fieldValues and perform server validation on it.

            const string dynamicGroupCode1 = "UATPDetails";
            const string dynamicFieldName1 = "SignedForCurrencyCode";

            IList<FieldMetaData> dynamicFieldMetaData = new List<FieldMetaData>();

            // This code will get the collection of parsed value.
            var parsedFiledValue = new List<FieldValue>();
            foreach (var fValue in fieldValues)
            {
                foreach (var attributeValue in fValue.AttributeValues)
                {
                    parsedFiledValue.Add(attributeValue);
                }
            }

            // This code snippet is used to set parsed value to subfileds values.
            foreach (var fdValue in fieldValues)
            {
                fdValue.FieldMetaData.FieldValues.Clear();
                fdValue.FieldMetaData.FieldValues.Add(fdValue);
                foreach (var subfieldvalues in fdValue.FieldMetaData.SubFields)
                {
                    var actfldValue = parsedFiledValue.FindAll(i => i.FieldMetaDataId == subfieldvalues.Id).ToList();
                    if (actfldValue.Count > 0)
                    {
                        subfieldvalues.FieldValues.Clear();
                        subfieldvalues.FieldValues.AddRange(actfldValue);
                    }
                }
            }


            foreach (var fldValue in fieldValues)
            {
                if (fldValue != null)
                {
                    if (fldValue.FieldMetaData != null)
                    {
                        dynamicFieldMetaData.Add(fldValue.FieldMetaData);

                        if (fldValue.FieldMetaData.FieldName == dynamicGroupCode1)
                        {
                            isValid = IsValidDynamicField(lineItemDetail, exceptionDetailsList, fileSubmissionDate, miscUatpInvoice, fileName, fldValue, dynamicFieldName1, isValid, zeroArray);
                            }
                        else if (fldValue.FieldMetaData.FieldName == "SettlementDetails")
                        {
                            //Validate Local currency code and settlement currency code.
                            //SCP172860: SIS Misc Payable Invoice - MXMLT contains historical currency code ECV.
                            isValid = (IsValidDynamicField(lineItemDetail, exceptionDetailsList, fileSubmissionDate, miscUatpInvoice, fileName, fldValue, "LocalCurrencyCode", isValid, zeroArray) &&
                                       IsValidDynamicField(lineItemDetail, exceptionDetailsList, fileSubmissionDate, miscUatpInvoice, fileName, fldValue, "SettlementCurrencyCode", isValid, zeroArray));
                        }
                    }

                    //var individualFieldMetaData = new List<FieldMetaData>();
                    //foreach(var vdata in _fieldMetaDatas)
                    //{
                    //  if(vdata.FieldName == fldValue.FieldMetaData.FieldName)
                    //  {
                    //    individualFieldMetaData.Add(vdata);
                    //  }
                    //}

                    //var individualFieldMetaData = from v in _fieldMetaDatas
                    //                              where v.FieldName == fldValue.FieldMetaData.FieldName
                    //                              select v;
                    //foreach (var fldMetaData in individualFieldMetaData.ToList())
                    //{
                    //  fldMetaData.FieldValues.Clear();
                    //  fldMetaData.FieldValues.Add(fldValue);
                    //  dynamicFieldMetaData.Add(fldMetaData);

                    //}
                }
            }


            // Server Validation

            IList<DynamicValidationError> dynamicValidationErrors;
            PerformDynamicFieldValidation(lineItem.ChargeCodeId, lineItem.ChargeCodeTypeId, dynamicFieldMetaData, out dynamicValidationErrors);
            if (dynamicValidationErrors != null && dynamicValidationErrors.Count > 0)
            {

                foreach (var dynamicValidationError in dynamicValidationErrors.Where(i => i.ErrorStatus == ErrorStatus.C).ToList())
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(dynamicValidationError.RecordId,
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                miscUatpInvoice, "Dynamic Field",
                                                                                                Convert.ToString(dynamicValidationError.FieldValue), fileName,
                                                                                                ErrorLevels.
                                                                                                  ErrorLevelLineItemDetail,
                                                                                                dynamicValidationError.ErrorCode,
                                                                                                ErrorStatus.C,
                                                                                                lineItemDetail.LineItemNumber,
                                                                                                lineItemDetail.DetailNumber);

                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }

                foreach (var dynamicValidationError in dynamicValidationErrors.Where(i => i.ErrorStatus != ErrorStatus.C).ToList())
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(dynamicValidationError.RecordId,
                                                                                                exceptionDetailsList.Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                miscUatpInvoice, "Dynamic Field",
                                                                                                Convert.ToString(dynamicValidationError.FieldValue), fileName,
                                                                                                ErrorLevels.
                                                                                                  ErrorLevelLineItemDetail,
                                                                                                dynamicValidationError.ErrorCode,
                                                                                                ErrorStatus.X,
                                                                                                lineItemDetail.LineItemNumber,
                                                                                                lineItemDetail.DetailNumber);

                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }


                //foreach (var validationExceptionDetail in
                //  dynamicValidationErrors.Select(dynamicValidationError => CreateValidationExceptionDetail(lineItemDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Dynamic Field", dynamicValidationError.ErrorDescription, fileName, ErrorLevels.ErrorLevelLineItemDetail, dynamicValidationError.ErrorCode, ErrorStatus.C, lineItemDetail.LineItemNumber, lineItemDetail.DetailNumber)))
                //{
                //  exceptionDetailsList.Add(validationExceptionDetail);
                //  isValid = false;
                //}
            }

            // Call the Group Structure Validation
            foreach (var fieldValue in fieldValues)
            {
                if (fieldValue.FieldMetaData != null && fieldValue.FieldMetaData.FieldValues != null)
                {
                    fieldValue.FieldMetaData.FieldValues.Clear();
                }

                ValidateParsedDynamicFields(fieldValue, exceptionDetailsList, fileName, miscUatpInvoice, "", lineItem, lineItemDetail, fileSubmissionDate);
            }

            return isValid;
        }

        /// <summary>
        /// This function is used to  check dynamic valid or not.
        /// </summary>
        /// <param name="lineItemDetail"></param>
        /// <param name="exceptionDetailsList"></param>
        /// <param name="fileSubmissionDate"></param>
        /// <param name="miscUatpInvoice"></param>
        /// <param name="fileName"></param>
        /// <param name="fldValue"></param>
        /// <param name="dynamicFieldName1"></param>
        /// <param name="isValid"></param>
        /// <returns></returns>
        private bool IsValidDynamicField(LineItemDetail lineItemDetail, IList<IsValidationExceptionDetail> exceptionDetailsList, DateTime fileSubmissionDate, MiscUatpInvoice miscUatpInvoice, string fileName, FieldValue fldValue, string dynamicFieldName1, bool isValid, string[] zeroArray)
        {
            foreach (var fieldMetaData in fldValue.FieldMetaData.SubFields.Where(subFields => subFields.FieldName == dynamicFieldName1))
            {
                foreach (var fldvalues in fieldMetaData.FieldValues)
                {
                    if (ValidationCache.Instance.ValidCurrencyCodes != null)
                    {
                        if (fldvalues.Value != null && (!ValidationCache.Instance.ValidCurrencyCodes.ContainsKey(fldvalues.Value.Trim()) && !zeroArray.Contains(fldvalues.Value.Trim())))
                        {
                            var validationExceptionDetail = CreateValidationExceptionDetail(fldvalues.RecordId, exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Dynamic Field", fldvalues.Value, fileName, ErrorLevels.ErrorLevelLineItemDetail, MiscUatpErrorCodes.InvalidCurrency, ErrorStatus.X, lineItemDetail.LineItemNumber, lineItemDetail.DetailNumber);

                            exceptionDetailsList.Add(validationExceptionDetail);
                            isValid = false;
                        }
                    }
                }
            }
            return isValid;
        }

        /// <summary>
        /// Validates the parsed dynamic field data.
        /// </summary>
        /// <param name="fieldValue">The field value.</param>
        /// <param name="exceptionDetailsList">The exception details list.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
        /// <param name="errorLevel">The error level.</param>
        /// <param name="lineItem">The line item.</param>
        /// <param name="lineItemDetail">The line item detail.</param>
        /// <param name="fileSubmissionDate">The file submission date.</param>
        /// <returns></returns>
        private bool ValidateParsedDynamicFieldData(FieldValue fieldValue, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, MiscUatpInvoice miscUatpInvoice, string errorLevel, LineItem lineItem, LineItemDetail lineItemDetail, DateTime fileSubmissionDate)
        {
            var isValid = true;
            var fieldMetaData = _fieldMetaDatas.Single(field => field.Id == fieldValue.FieldMetaDataId);
            if (fieldMetaData.FieldType == FieldType.Group) return true;

            switch (fieldMetaData.DataType)
            {
                case DataType.Alphabetic:
                    if (!CompareUtil.IsAlpha(fieldValue.Value))
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(lineItemDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Dynamic Field", "Field Name:" + fieldMetaData.FieldName + ", Field Value :" + fieldValue.Value,
                                                       fileName, ErrorLevels.ErrorLevelLineItemDetail,
                                                       MiscUatpErrorCodes.InvalidADataValue,
                                                       ErrorStatus.X,
                                                       lineItemDetail.LineItemNumber,
                                                       lineItemDetail.DetailNumber);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                    break;

                case DataType.Numeric:
                    if (!CompareUtil.IsNumeric(fieldValue.Value))
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(lineItemDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Dynamic Field", "Field Name:" + fieldMetaData.FieldName + ", Field Value :" + fieldValue.Value,
                                                       fileName, ErrorLevels.ErrorLevelLineItemDetail,
                                                       MiscUatpErrorCodes.InvalidNDataValue,
                                                       ErrorStatus.X,
                                                       lineItemDetail.LineItemNumber,
                                                       lineItemDetail.DetailNumber);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                    break;

                case DataType.DateTime:
                    if (!CompareUtil.IsDate(fieldValue.Value))
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(lineItemDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Dynamic Field", "Field Name:" + fieldMetaData.FieldName + ", Field Value :" + fieldValue.Value,
                                                       fileName, ErrorLevels.ErrorLevelLineItemDetail,
                                                       MiscUatpErrorCodes.InvalidDateDataValue,
                                                       ErrorStatus.X,
                                                       lineItemDetail.LineItemNumber,
                                                       lineItemDetail.DetailNumber);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                    break;

                case DataType.PositiveNumber:
                    if (!CompareUtil.IsNumeric(fieldValue.Value) && Convert.ToDouble(fieldValue.Value) < 0)
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(lineItemDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Dynamic Field", "Field Name:" + fieldMetaData.FieldName + ", Field Value :" + fieldValue.Value,
                                                       fileName, ErrorLevels.ErrorLevelLineItemDetail,
                                                       MiscUatpErrorCodes.InvalidPnDataValue,
                                                       ErrorStatus.X,
                                                       lineItemDetail.LineItemNumber,
                                                       lineItemDetail.DetailNumber);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                    break;
            }
            return isValid;
        }

        /// <summary>
        /// Validates the parsed group structure.
        /// </summary>
        /// <param name="fieldValue">The field value.</param>
        /// <param name="exceptionDetailsList">The exception details list.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
        /// <param name="errorLevel">The error level.</param>
        /// <param name="lineItem">The line item.</param>
        /// <param name="lineItemDetail">The line item detail.</param>
        /// <param name="fileSubmissionDate"></param>
        /// <returns></returns>
        protected bool ValidateParsedDynamicFields(FieldValue fieldValue, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, MiscUatpInvoice miscUatpInvoice, string errorLevel, LineItem lineItem, LineItemDetail lineItemDetail, DateTime fileSubmissionDate)
        {
            var isValid = true;

            if (fieldValue.FieldMetaData != null && fieldValue.FieldMetaData.FieldValues != null)
            {
                fieldValue.FieldMetaData.FieldValues.Clear();
            }
            var chargeCodeId = lineItem.ChargeCodeId;
            var chargeCodeTypeId = lineItem.ChargeCodeTypeId;

            var requiredFields = (from fieldMetaData in _fieldMetaDatas
                                  join fieldChargeCodeMapp in _fieldChargeCodeMappings on fieldMetaData.Id equals fieldChargeCodeMapp.FieldMetaDataId
                                  where fieldChargeCodeMapp.ChargeCodeId == chargeCodeId && fieldMetaData.ParentId == fieldValue.FieldMetaDataId &&
                                  (fieldMetaData.FieldTypeId == (int)FieldType.Field || fieldMetaData.FieldTypeId == (int)FieldType.Attribute) &&
                                  fieldChargeCodeMapp.RequiredTypeId == (int)RequiredType.Mandatory
                                  && (miscUatpInvoice.BillingCategoryId == (int)BillingCategoryType.Misc ? true : fieldChargeCodeMapp.ChargeCodeTypeId == chargeCodeTypeId)
                                  select fieldMetaData).ToList();

            #region CMP609: MISC Changes Required as per ISW2. (Required fields)
            // If the Original Billing Month is equal to or earlier than 2014-Apr-P4,
            // then validation should be ignored even if Line Item Details have been provided for the combination of Charge Category/Charge Code
            var checkBillingPeriod = new BillingPeriod(2014, 04, 04);
            // If Billing Category is 'Miscelleneous' and is Rejection Invoice and is Original Billing Month is equal to or earlier than 2014-Apr-P4
            if (miscUatpInvoice.BillingCategoryId == (int)BillingCategoryType.Misc && miscUatpInvoice.InvoiceTypeId == (int)InvoiceType.RejectionInvoice && miscUatpInvoice.SettlementBillingPeriod <= checkBillingPeriod)
            {
              // If Charge Category is 'ATC'(1)
              if (miscUatpInvoice.ChargeCategoryId == 1)
              {
                // If Charge Code is 'Approach'(1) or 'En-Route'(3) or 'Over-flight'(7)
                if (lineItem.ChargeCodeId == 1 || lineItem.ChargeCodeId == 3 || lineItem.ChargeCodeId == 7)
                {
                  var ignoreFieldMetaDataItemFlightNo = requiredFields.Find(field => field.FieldName.Equals("FlightNo"));
                  if (ignoreFieldMetaDataItemFlightNo != null)
                  {
                    requiredFields.Remove(ignoreFieldMetaDataItemFlightNo);
                  }

                  var ignoreFieldMetaDataItemFlightDateTime = requiredFields.Find(field => field.FieldName.Equals("FlightDateTime"));
                  if (ignoreFieldMetaDataItemFlightDateTime != null)
                  {
                    requiredFields.Remove(ignoreFieldMetaDataItemFlightDateTime);
                  }

                  // If Charge Code is 'Over-flight'(7)
                  if (lineItem.ChargeCodeId == 7)
                  {
                    var ignoreFieldMetaDataItem = requiredFields.Find(field => field.FieldName.Equals("LocationCode_ICAO"));
                    if (ignoreFieldMetaDataItem != null)
                    {
                      requiredFields.Remove(ignoreFieldMetaDataItem);
                    }
                  }
                }
              }

              // If Charge Category is 'Ground Handling'(6)
              if (miscUatpInvoice.ChargeCategoryId == 6)
              {
                // If Charge Code is 'Baggage'(31) or 'Catering'(34) or 'Crew Accommodation'(37) or 'Crew Transportation'(38) or 'Passenger Transportation'(49)
                if (lineItem.ChargeCodeId == 31 || lineItem.ChargeCodeId == 34 || lineItem.ChargeCodeId == 37 || lineItem.ChargeCodeId == 38 || lineItem.ChargeCodeId == 49)
                {
                  var ignoreFieldMetaDataItem = requiredFields.Find(field => field.FieldName.Equals("FlightDateTime"));
                  if (ignoreFieldMetaDataItem != null)
                  {
                    requiredFields.Remove(ignoreFieldMetaDataItem);
                  }
                }

                // If Charge Code is 'Lounge'(43)
                if (lineItem.ChargeCodeId == 43)
                {
                  var ignoreFieldMetaDataItem = requiredFields.Find(field => field.FieldName.Equals("PassengerName"));
                  if (ignoreFieldMetaDataItem != null)
                  {
                    requiredFields.Remove(ignoreFieldMetaDataItem);
                  }
                }

                // If Charge Code is 'Mishandling Baggage'(45)
                if (lineItem.ChargeCodeId == 45)
                {
                  var ignoreFieldMetaDataItem = requiredFields.Find(field => field.FieldName.Equals("ReferenceNo"));
                  if (ignoreFieldMetaDataItem != null)
                  {
                    requiredFields.Remove(ignoreFieldMetaDataItem);
                  }
                }
              }

              // If Charge Category is 'Service Provider'(12)
              if (miscUatpInvoice.ChargeCategoryId == 12)
              {
                // If Charge Code is 'Meetings and Conferences'(84) or 'Training'(86)
                if (lineItem.ChargeCodeId == 31 || lineItem.ChargeCodeId == 34)
                {
                  var ignoreFieldMetaDataItem = requiredFields.Find(field => field.FieldName.Equals("StaffName"));
                  if (ignoreFieldMetaDataItem != null)
                  {
                    requiredFields.Remove(ignoreFieldMetaDataItem);
                  }
                }
              }
            }
            #endregion


            foreach (var requiredField in requiredFields)
            {
                bool requiredFieldPresent = false;
                foreach (var attributeValue in fieldValue.AttributeValues)
                {
                    if (attributeValue.FieldMetaDataId == requiredField.Id)
                    {
                        requiredFieldPresent = true;
                    }
                }
                if (requiredFieldPresent == false)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(lineItemDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Dynamic Field", requiredField.FieldName,
                                                             fileName, ErrorLevels.ErrorLevelLineItemDetail,
                                                             MiscUatpErrorCodes.MandatoryFieldAttributeMissing,
                                                             ErrorStatus.X,
                                                             lineItemDetail.LineItemNumber,
                                                             lineItemDetail.DetailNumber);
                    exceptionDetailsList.Add(validationExceptionDetail);
                }
            }

            //CMP609 :file passed when mandatory field is not provided [IN:009211]
            if (miscUatpInvoice.BillingCategoryId == (int)BillingCategoryType.Misc)
            {
              foreach (var requiredField in requiredFields)
              {
                // If Charge Code is 'Lounge'(43)  
                if (lineItem.ChargeCodeId == 43)
                {
                  if (requiredField.FieldName.ToLower().Equals("passengername"))
                  {
                    if (requiredField.DataType == DataType.AlphaNumeric)
                    {
                      foreach (var fieldVal in fieldValue.AttributeValues)
                      {
                        if (fieldVal.FieldMetaData.FieldName.ToLower() == "passengername")
                        {
                          if (string.IsNullOrWhiteSpace(fieldVal.Value))
                          {
                            var validationExceptionDetail = CreateValidationExceptionDetail(lineItemDetail.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Dynamic Field", requiredField.FieldName,
                                                             fileName, ErrorLevels.ErrorLevelLineItemDetail,
                                                             MiscUatpErrorCodes.MandatoryFieldAttributeMissing,
                                                             ErrorStatus.X,
                                                             lineItemDetail.LineItemNumber,
                                                             lineItemDetail.DetailNumber);
                            exceptionDetailsList.Add(validationExceptionDetail);
                            isValid = false;
                          }
                        }
                      }
                    }
                  }
                }

              }
            }


            //if (!requiredFields.All(requiredGroupFieldRec => fieldValue.AttributeValues.Select(fieldValuesRec => fieldValuesRec.FieldMetaDataId).Contains(requiredGroupFieldRec.Id)))
            //{
            //  var errorParentFieldMetaData = _fieldMetaDatas.Single(field => field.Id == fieldValue.FieldMetaDataId);
            //  var validationExceptionDetail = CreateValidationExceptionDetail(exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Dynamic Field",fieldValue.Value,
            //                                             fileName, ErrorLevels.ErrorLevelLineItemDetail,
            //                                             MiscUatpErrorCodes.MandatoryFieldAttributeMissing,
            //                                             ErrorStatus.X,
            //                                             lineItemDetail.LineItemNumber,
            //                                             lineItemDetail.DetailNumber);
            //  exceptionDetailsList.Add(validationExceptionDetail);
            //}

            // Data Type given in DB and value give in field value should be valid.
            isValid = ValidateParsedDynamicFieldData(fieldValue, exceptionDetailsList, fileName, miscUatpInvoice, "", lineItem, lineItemDetail, fileSubmissionDate);

         
            foreach (var fieldVal in fieldValue.AttributeValues)
            {
                if (fieldVal.FieldMetaData != null && fieldVal.FieldMetaData.FieldValues != null)
                {
                    fieldVal.FieldMetaData.FieldValues.Clear();
                }
                if (fieldVal.AttributeValues.Count > 0)
                {
                    isValid = ValidateParsedDynamicFields(fieldVal, exceptionDetailsList, fileName, miscUatpInvoice, "", lineItem, lineItemDetail, fileSubmissionDate);
                }
            }

            return isValid;
        }

        /// <summary>
        /// Gets the clearance currency.
        /// </summary>
        /// <param name="billingMember">The billing member.</param>
        /// <param name="billedMember">The billed member.</param>
        /// <param name="invoiceHeader">The invoice header.</param>
        /// <returns></returns>
        void GetClearanceCurrency(Member billingMember, Member billedMember, InvoiceBase invoiceHeader)
        {
            if (billingMember != null)
                billingMember.IchConfiguration = GetIchConfiguration(billingMember.Id);
            if (billedMember != null)
                billedMember.IchConfiguration = GetIchConfiguration(billedMember.Id);

            if (invoiceHeader.InvoiceSmi == SMI.Ich && billingMember.IchConfiguration != null)
            {
                switch (billingMember.IchConfiguration.IchZone)
                {
                    case IchZoneType.A:
                        {
                            // Billed airline in zone A.
                            if (billedMember.IchConfiguration != null && billedMember.IchConfiguration.IchZone == IchZoneType.A)
                            {
                                invoiceHeader.BillingCurrency = BillingCurrency.GBP;
                                return;
                            }
                            // Billed airline in any zone other than A.
                            invoiceHeader.BillingCurrency = BillingCurrency.USD;
                            return;
                        }

                    case IchZoneType.B:
                    case IchZoneType.C:
                        // Billed airline in any zone.
                        invoiceHeader.BillingCurrency = BillingCurrency.USD;
                        break;
                    case IchZoneType.D:
                        // Billed airline in zone A, B, C.
                        if (billedMember.IchConfiguration != null && billedMember.IchConfiguration.IchZone == IchZoneType.D)
                        {
                            invoiceHeader.BillingCurrency = BillingCurrency.EUR;
                            return;
                        }
                        // Billed airline in any zone other than D.
                        invoiceHeader.BillingCurrency = BillingCurrency.USD;
                        return;
                }
            }

            // When SMI = A/M, and if only billing member belongs to ACH, lock the dropdown on USD. 
            if ((invoiceHeader.InvoiceSmi == SMI.Ach || invoiceHeader.InvoiceSmi == SMI.AchUsingIataRules) && IsAchMember(billingMember) && !IsAchMember(billedMember))
            {
                invoiceHeader.BillingCurrency = BillingCurrency.USD;
                return;
            }

            // When SMI = A/M, and both members belong to ACH, dropdown should only offer USD/CAD. Default to USD
            if ((invoiceHeader.InvoiceSmi == SMI.Ach || invoiceHeader.InvoiceSmi == SMI.AchUsingIataRules) && IsAchMember(billingMember) && IsAchMember(billedMember))
            {
                invoiceHeader.BillingCurrency = BillingCurrency.USD;
            }
        }

        /// <summary>
        /// Validates the parsed invoice.
        /// </summary>
        /// <param name="miscUatpInvoice"></param>
        /// <param name="exceptionDetailsList"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public virtual bool ValidateParsedInvoice(MiscUatpInvoice miscUatpInvoice, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate, string issuingOrgMemberCode)
        {
            var isValid = true;

            //CMP496: initialize repository for reference data validation.
            var referenceDataRepository = Ioc.Resolve<IReferenceDataManager>(typeof(IReferenceDataManager));

            #region CMP #624: ICH Rewrite-New SMI X

            /* Description: Detailed Validation of IS-IDEC and IS-XML Files (Part 1) */
            // Get the details of the billing and billed member.
            var billingMember = (miscUatpInvoice.BillingMemberId == 0 ? null : MemberManager.GetMember(miscUatpInvoice.BillingMemberId));
            var billedMember = (miscUatpInvoice.BilledMemberId == 0 ? null : MemberManager.GetMember(miscUatpInvoice.BilledMemberId));

            // Get Final Parent Details for SMI, Currency, Clearing House abd Suspended Flag validations
            var billingFinalParent = (miscUatpInvoice.BillingMemberId == 0 ? null : MemberManager.GetMember(MemberManager.GetFinalParentDetails(miscUatpInvoice.BillingMemberId)));
            var billedFinalParent = (miscUatpInvoice.BilledMemberId == 0 ? null : MemberManager.GetMember(MemberManager.GetFinalParentDetails(miscUatpInvoice.BilledMemberId)));

            // Assign final parent to invoice
            if (billingFinalParent != null && billingFinalParent.Id != billingMember.Id)
            {
                miscUatpInvoice.BillingParentMemberId = billingFinalParent.Id;
            }
            if (billedFinalParent != null && billedFinalParent.Id != billedMember.Id)
            {
                miscUatpInvoice.BilledParentMemberId = billedFinalParent.Id;
            }

            /* CMP #624: ICH Rewrite-New SMI X  : Get original invoice from input invoice */
            MiscUatpInvoice linkedRm1Invoice;
            MiscUatpInvoice linkedRm2Invoice;
            MiscCorrespondence linkedCorrespondence;

            //if (miscUatpInvoice.SettlementMethodId == (int)SMI.IchSpecialAgreement)
            //{
            //  originalInvoice = GetLinkedMUOriginalInvoice(miscUatpInvoice);
            //}
            // Get all stages linking details up to the original invoice for All SMI's.
            MiscUatpInvoice originalInvoice = GetLinkedMUOriginalInvoice(miscUatpInvoice, out linkedRm1Invoice, out linkedRm2Invoice, out linkedCorrespondence);

          /* CMP #624: ICH Rewrite-New SMI X 
            * Description: ICH Web Service is called when header is saved
            * Refer FRS Section: 2.8	Detailed Validation of IS-IDEC and IS-XML Files (Part 1). */
            bool smiXValidationsPhase1Result = ValidationBeforeSmiXWebServiceCall(miscUatpInvoice, exceptionDetailsList,
                                                                                  miscUatpInvoice.InvoiceTypeId,
                                                                                  fileSubmissionDate, fileName,
                                                                                  billingFinalParent, billedFinalParent,
                                                                                  false, originalInvoice, 0, 0);

            if (miscUatpInvoice.SettlementMethodId == (int)SMI.IchSpecialAgreement && miscUatpInvoice.GetSmiXPhase1ValidationStatus())
            {
                /* CMP #624: ICH Rewrite-New SMI X 
                * Description: ICH Web Service is called when header is saved
                * Refer FRS Section: 2.9	Detailed Validation of IS-IDEC and IS-XML Files (Part 2). */
                CallSmiXIchWebServiceAndHandleResponse(miscUatpInvoice, exceptionDetailsList,
                                                       miscUatpInvoice.InvoiceTypeId, fileSubmissionDate, fileName,
                                                       false, 0, 0, originalInvoice);
            }

            #endregion

            // check whether issuingOrgMemberCode is matching with bilingMemberId.
            List<OnBehalfInvoiceSetup> onBehalfOfMemberList = null;
            var isOnbehalf = false;
            int issuingOrganizationId = 0;
            if (ValidationCache.Instance.OnBehalfOfMemberList != null)
            {
                onBehalfOfMemberList = ValidationCache.Instance.OnBehalfOfMemberList;
                if (onBehalfOfMemberList.Count(type => type.TransmitterCode == issuingOrgMemberCode && issuingOrgMemberCode != miscUatpInvoice.BillingMember.MemberCodeNumeric) > 0)
                {
                    isOnbehalf = true;
                }
            }
            if (!isOnbehalf)
            {
                if (!string.IsNullOrEmpty(issuingOrgMemberCode))
                {
                    if (!issuingOrgMemberCode.Equals(miscUatpInvoice.BillingMember.MemberCodeNumeric))
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                        exceptionDetailsList.Count() + 1,
                                                                                        fileSubmissionDate, miscUatpInvoice,
                                                                                        "Billing Member - issuingOrgMemberCode",
                                                                                        miscUatpInvoice.BillingMember != null
                                                                                            ? miscUatpInvoice.BillingMember.
                                                                                                  MemberCodeNumeric + "-" +
                                                                                              issuingOrgMemberCode
                                                                                            : string.Empty + "-" +
                                                                                              issuingOrgMemberCode, fileName,
                                                                                        ErrorLevels.ErrorLevelInvoice,
                                                                                        ErrorCodes.
                                                                                            InvalidIssuingOrganizationId,
                                                                                        ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                }

            }

            bool isSystemMultilingual = ValidationCache.Instance != null ? ValidationCache.Instance.IsSystemMultilingual : SystemParameters.Instance.General.IsMultilingualAllowed;
            if (!string.IsNullOrEmpty(miscUatpInvoice.InvTemplateLanguage) && isSystemMultilingual)
            {
                if (ValidationCache.Instance.Languages != null)
                {
                    if (!ValidationCache.Instance.Languages.ContainsKey(miscUatpInvoice.InvTemplateLanguage.Trim()))
                    {
                        //Add exception
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                        exceptionDetailsList.Count() + 1,
                                                                                        fileSubmissionDate, miscUatpInvoice,
                                                                                        "Invoice Template Language",
                                                                                        miscUatpInvoice.InvTemplateLanguage,
                                                                                        fileName, ErrorLevels.ErrorLevelInvoice,
                                                                                        MiscUatpErrorCodes.InvalidLanguage,
                                                                                        ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                }
                else
                {
                    //Fetch from db
                    var langaugesRepository = Ioc.Resolve<IRepository<Language>>();
                    if (
                      langaugesRepository.Single(
                        i =>
                        i.Language_Code.ToLower().CompareTo(miscUatpInvoice.InvTemplateLanguage.ToLower()) == 0 && i.IsReqForPdf) ==
                      null)
                    {
                        //Add exception
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                         exceptionDetailsList.Count() + 1,
                                                                                         fileSubmissionDate, miscUatpInvoice,
                                                                                         "Invoice Template Language",
                                                                                         miscUatpInvoice.InvTemplateLanguage,
                                                                                         fileName, ErrorLevels.ErrorLevelInvoice,
                                                                                         MiscUatpErrorCodes.InvalidLanguage,
                                                                                         ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                }
            }

            double summationTolerance = 0;
            double roundingTolerance = 0;
            if (miscUatpInvoice.Tolerance != null)
            {
                summationTolerance = miscUatpInvoice.Tolerance.SummationTolerance;
                roundingTolerance = miscUatpInvoice.Tolerance.RoundingTolerance;
            }

            if (!isOnbehalf)
            {
                if (miscUatpInvoice.BilledMemberId != 0 && miscUatpInvoice.BillingMemberId != 0)
                {
                    if ((IsBillingMemberMigrated(miscUatpInvoice) == false))
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Billing Member", string.Empty, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.MemberIsNotMigrated, ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                }
            }
            else
            {
                issuingOrganizationId = MemberManager.GetMemberId(issuingOrgMemberCode);
                if ((IsOnBehalfBillingMemberMigrated(issuingOrganizationId, miscUatpInvoice.InvoiceBillingPeriod) == false))
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Issuing Organization Code", issuingOrgMemberCode, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.MemberIsNotMigrated, ErrorStatus.X, 0, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }

            var regEx = new Regex("^[a-zA-Z0-9]+$");
            if (miscUatpInvoice.InvoiceNumber != null && !regEx.IsMatch(miscUatpInvoice.InvoiceNumber))
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Invoice Number", miscUatpInvoice.InvoiceNumber, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.SpecialCharactersAreNotAllowedInInvoiceNumber, ErrorStatus.X, 0, 0);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            // Validation for InvoiceNumber.   
            if (miscUatpInvoice.InvoiceNumber != null)
            {
                string currentfilename = string.Empty;
                //478879 - Loading IS-XML from IS-WEB not working Duplicate File Issue.
                var filetime = fileSubmissionDate.ToString("yyyyMMddHHMMss");

                if (ParsedInvoiceList.ContainsKey(miscUatpInvoice.InvoiceNumber))
                {
                    ParsedInvoiceList.TryGetValue(miscUatpInvoice.InvoiceNumber, out currentfilename);
                }

                var msg = string.Format("FN:{0},CFN:{1},FNE:{2},Inv:{3},BY:{4},MId:{5}", fileName + filetime, currentfilename, (!string.IsNullOrEmpty(currentfilename) && currentfilename.Equals(fileName + filetime)), miscUatpInvoice.InvoiceNumber.Trim(), miscUatpInvoice.BillingYear, miscUatpInvoice.BillingMemberId);
                Logger.InfoFormat("Validating duplicate invoice : [{0}]", msg);

                if (!ValidateInvoiceNumber(miscUatpInvoice.InvoiceNumber, miscUatpInvoice.BillingYear, miscUatpInvoice.BillingMemberId) ||
                    (!string.IsNullOrEmpty(currentfilename) && currentfilename.Equals(fileName + filetime)))
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Invoice Number", miscUatpInvoice.InvoiceNumber, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.DuplicateInvoiceFound, ErrorStatus.X, 0, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
                else if (!ParsedInvoiceList.ContainsKey(miscUatpInvoice.InvoiceNumber))
                {
                    ParsedInvoiceList.Add(miscUatpInvoice.InvoiceNumber, fileName + filetime);
                }
            }
            //Validation for clearence currency amount
            if (miscUatpInvoice.BillingCurrency.HasValue && Convert.ToDecimal(miscUatpInvoice.ExchangeRate) != 0)
            {
                if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.ToString(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Total Amount in clearence currency", miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.RequiredTotalAmountInBillingCurrency, ErrorStatus.X, 0, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }

            //CMP#648: Clearance Information in MISC Invoice PDFs 
            //Desc:Table 1: Current and Expected validations for MISC Bilateral Invoices/Credit Notes with respect to Clearance Information
            if (miscUatpInvoice.BillingCategory == BillingCategoryType.Misc && (ReferenceManager.IsSmiLikeBilateral(miscUatpInvoice.SettlementMethodId, false) || (miscUatpInvoice.InvoiceType == Model.Enums.InvoiceType.CreditNote && miscUatpInvoice.SettlementMethodId == (int) SMI.AdjustmentDueToProtest)))
            {
              if (!miscUatpInvoice.BillingCurrency.HasValue && !miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && Convert.ToDecimal(miscUatpInvoice.ExchangeRate) != 0)
              {
                var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.ToString(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Clearance Currency", string.Empty, fileName, ErrorLevels.ErrorLevelInvoice, MiscErrorCodes.TotalAmtSuppliedButNotExRateAndCCurrency, ErrorStatus.X, 0, 0);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
              }

              if (!miscUatpInvoice.BillingCurrency.HasValue && miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && Convert.ToDecimal(miscUatpInvoice.ExchangeRate) != 0)
              {
                var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.ToString(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Clearance Currency", string.Empty, fileName, ErrorLevels.ErrorLevelInvoice, MiscErrorCodes.TotalAmtExRateSuppliedButNotCCurrency, ErrorStatus.X, 0, 0);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
              }
            }


          var clearingHouseEnum = ReferenceManager.GetClearingHouseToFetchCurrentBillingPeriod(miscUatpInvoice.SettlementMethodId);

            // Validation for InvoicePeriod    
            #region CMP626 : Future Submission for MISC and Provisional Settlement with ICH
            
            //Created new logic for future submission
            BillingPeriod billingPeriod;
            BillingPeriod CurrOrPrevbillingPeriod;
            bool isValidBillingPeriod = false;
            var invBillingPeriod = new BillingPeriod
            {
              ClearingHouse = clearingHouseEnum,
              Month = miscUatpInvoice.BillingMonth,
              Year = miscUatpInvoice.BillingYear,
              Period = miscUatpInvoice.BillingPeriod
            };

            try
            {
                billingPeriod = CalendarManager.GetBillingPeriod(fileSubmissionDate, clearingHouseEnum);              
                isValidBillingPeriod = ValidateBillingPeriod(miscUatpInvoice, billingPeriod, clearingHouseEnum);
                if (!isValidBillingPeriod)
                {
                  //If current open period not greater then invoice period then check for late submission of last close period 
                  if (billingPeriod > invBillingPeriod)
                  {
                    billingPeriod = CalendarManager.GetLastClosedBillingPeriod(fileSubmissionDate, clearingHouseEnum);
                  }
                }
            }
            catch (ISCalendarDataNotFoundException)
            {
                billingPeriod = CalendarManager.GetLastClosedBillingPeriod(fileSubmissionDate, clearingHouseEnum);
            }
          CurrOrPrevbillingPeriod = billingPeriod;
            if (!isValidBillingPeriod)
            {
              // Check for the future submission of invoices
              // If it is future submitted update validation status of invoices to future submitted status
              // Set is future submission flag to true
              if ((billingPeriod < invBillingPeriod) && miscUatpInvoice.BillingCategory == BillingCategoryType.Misc)
              {
                try
                {
                  /* Set billing period as of invoice future period*/
                  Logger.Info("Set billing period as of invoice future period : Start");
                  billingPeriod = CalendarManager.GetBillingPeriod(clearingHouseEnum, miscUatpInvoice.BillingYear, miscUatpInvoice.BillingMonth, miscUatpInvoice.BillingPeriod); ;
                  Logger.Info("Set billing period as of invoice future period : End");
                }
                catch 
                {
                  var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Invoice Billing Period", string.Format("{00}{1:00}{2:00}", miscUatpInvoice.BillingYear.ToString().Substring(2), miscUatpInvoice.BillingMonth, miscUatpInvoice.BillingPeriod), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidBillingPeriod, ErrorStatus.X, 0, 0);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
                }
                 
                // Get miscellaneousConfiguration of the Issuing Organization
                issuingOrganizationId = MemberManager.GetMemberId(issuingOrgMemberCode);
                var miscellaneousConfiguration = MemberManager.GetMiscellaneousConfiguration(issuingOrganizationId);

                // Check whether futuresubmittion is allowed to user or not
                if (miscellaneousConfiguration != null && miscellaneousConfiguration.IsFutureBillingSubmissionsAllowed)
                {
                  miscUatpInvoice.IsFutureSubmission = true;
                  miscUatpInvoice.ValidationStatus = InvoiceValidationStatus.FutureSubmission;
                  miscUatpInvoice.ValidationStatusId = (int)InvoiceValidationStatus.FutureSubmission;
                }
                else
                {
                  var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, "Billing Date", string.Format("{0}{1}{2}", miscUatpInvoice.BillingYear.ToString().PadLeft(2, '0'), miscUatpInvoice.BillingMonth.ToString().PadLeft(2, '0'), miscUatpInvoice.BillingPeriod.ToString().PadLeft(2, '0')), miscUatpInvoice, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidBillingPeriod, ErrorStatus.X);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
                }
              }
              else if (ReferenceManager.IsValidSmiForLateSubmission(miscUatpInvoice.SettlementMethodId) && IsLateSubmission(miscUatpInvoice, fileSubmissionDate, clearingHouseEnum, billingPeriod))
                {
                    miscUatpInvoice.ValidationStatus = InvoiceValidationStatus.ErrorPeriod;
                    miscUatpInvoice.ValidationStatusId = (int)InvoiceValidationStatus.ErrorPeriod;
                }
                else
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Invoice Billing Period", string.Format("{00}{1:00}{2:00}", miscUatpInvoice.BillingYear.ToString().Substring(2), miscUatpInvoice.BillingMonth, miscUatpInvoice.BillingPeriod), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidBillingPeriod, ErrorStatus.X, 0, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }
            #endregion CMP626           
                       

            //SCP237693 - QF-081 SIS Reports
            //fix:: If invalid period error came in file then not include invoice date errors. Invoice date error will only be included if billing period was correctly provided.
            //CMP626 : Validate Invoice date
            if (!ValidateParsedInvoiceDate(miscUatpInvoice.InvoiceDate,billingPeriod))
            {
              if (!IsLateSubmission(miscUatpInvoice, miscUatpInvoice.InvoiceDate, clearingHouseEnum, CurrOrPrevbillingPeriod))
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Invoice Date", miscUatpInvoice.InvoiceDate.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidInvoiceDate, ErrorStatus.X, 0, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }

            // Validation for ChargeCategory
            if (miscUatpInvoice.ChargeCategory == null)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Charge Category", string.Empty, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidChargeCategory, ErrorStatus.X, 0, 0);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            // Make sure billing and billed member are not the same.
            if (miscUatpInvoice.BilledMemberId == miscUatpInvoice.BillingMemberId && miscUatpInvoice.BilledMemberId != 0 && miscUatpInvoice.BillingMemberId != 0)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Same Billing and Billed Member", miscUatpInvoice.BilledMemberId + " And " + miscUatpInvoice.BillingMemberId.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.SameBillingAndBilledMember, ErrorStatus.X, 0, 0, true);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            ////Shambhu Thakur - Commented Digital Signature Validation
            // As per bugId 5013

            #region Code moved at the top as per CMP #624 requirement
            //// Get the details of the billing and billed member.
            //var billingMember = (miscUatpInvoice.BillingMemberId == 0 ? null : MemberManager.GetMember(miscUatpInvoice.BillingMemberId));
            //var billedMember = (miscUatpInvoice.BilledMemberId == 0 ? null : MemberManager.GetMember(miscUatpInvoice.BilledMemberId));

            //// Get Final Parent Details for SMI, Currency, Clearing House abd Suspended Flag validations
            //var billingFinalParent = (miscUatpInvoice.BillingMemberId == 0 ? null : MemberManager.GetMember(MemberManager.GetFinalParentDetails(miscUatpInvoice.BillingMemberId)));
            //var billedFinalParent = (miscUatpInvoice.BilledMemberId == 0 ? null : MemberManager.GetMember(MemberManager.GetFinalParentDetails(miscUatpInvoice.BilledMemberId)));

            //// Assign final parent to invoice
            //if (billingFinalParent != null && billingFinalParent.Id != billingMember.Id)
            //{
            //    miscUatpInvoice.BillingParentMemberId = billingFinalParent.Id;
            //}
            //if (billedFinalParent != null && billedFinalParent.Id != billedMember.Id)
            //{
            //    miscUatpInvoice.BilledParentMemberId = billedFinalParent.Id;
            //}
            #endregion

            ////Shambhu Thakur(08Sep11) - uncommented member profile digitalsignapplication check

            if (billingMember != null)
            {
                if (miscUatpInvoice.DigitalSignatureRequired.Equals(DigitalSignatureRequired.Yes))
                {
                    // Vinod Patil SCP  ID : 54893 - DS exception for UATP ( additional check for UATP) 
                    // In the Member Profile, a member can define an exception for UATP and say that DS shouldn’t apply for UATP invoices
                    // And profile element of the Billing Member for DS Applicable is not applicable, raise an error with Non-Correctable

                    UatpConfiguration uatpConfiguration = MemberManager.GetUATPConfiguration(billingMember.Id);

                    if ((!billingMember.DigitalSignApplication) ||
                        (miscUatpInvoice.BillingCategory == BillingCategoryType.Uatp && billingMember.DigitalSignApplication &&
                         uatpConfiguration.ISUatpInvIgnoreFromDsproc))
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                        exceptionDetailsList.Count() + 1,
                                                                                        fileSubmissionDate, miscUatpInvoice,
                                                                                        "Digital Signature Required",
                                                                                        miscUatpInvoice.
                                                                                            DigitalSignatureRequiredId
                                                                                            .ToString(), fileName,
                                                                                        ErrorLevels.ErrorLevelInvoice,
                                                                                        MiscUatpErrorCodes.
                                                                                            InvalidDigitalSignatureRequired,
                                                                                        ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                }
            }

            #region CMP496: Comment, this validation not covering all possible cases.
            /* 
      if (!string.IsNullOrWhiteSpace(miscUatpInvoice.BillingMemberLocationCode) && !string.IsNullOrWhiteSpace(miscUatpInvoice.BilledMemberLocationCode))
      {
         

          //Either both location should be provided or location information for both billing and billed member should be provided
          if (miscUatpInvoice.MemberLocationInformation.Count == 2 &&
              !string.IsNullOrWhiteSpace(miscUatpInvoice.BillingMemberLocationCode) &&
              !string.IsNullOrWhiteSpace(miscUatpInvoice.BilledMemberLocationCode))
          {
              isValidBillingLocationCombination = true;
          }

          //Either both location should be provided or location information for both billing and billed member should be provided
          if (miscUatpInvoice.MemberLocationInformation.Count == 2 &&
              string.IsNullOrWhiteSpace(miscUatpInvoice.BillingMemberLocationCode) &&
              string.IsNullOrWhiteSpace(miscUatpInvoice.BilledMemberLocationCode))
          {
              isValidBillingLocationCombination = true;
          }

          if (miscUatpInvoice.MemberLocationInformation.Count == 0 &&
              !string.IsNullOrWhiteSpace(miscUatpInvoice.BillingMemberLocationCode) &&
              !string.IsNullOrWhiteSpace(miscUatpInvoice.BilledMemberLocationCode))
          {
              isValidBillingLocationCombination = true;
          }

          if (miscUatpInvoice.MemberLocationInformation.Count == 0 &&
              string.IsNullOrWhiteSpace(miscUatpInvoice.BillingMemberLocationCode) &&
              string.IsNullOrWhiteSpace(miscUatpInvoice.BilledMemberLocationCode))
          {
              isValidBillingLocationCombination = true;
          }

       }*/
            #endregion

            #region CMP496: 1) Adding New validation to cover all scenarios using Matrix.
            var validBillingBilledLocationCombination =
                   referenceDataRepository.IsValidBillingBilledCombination(miscUatpInvoice.MemberLocationInformation,
                                                                           miscUatpInvoice.SubmissionMethod,
                                                                           miscUatpInvoice.BillingMemberLocationCode,
                                                                           miscUatpInvoice.BilledMemberLocationCode,
                                                                           miscUatpInvoice.BillingReferenceDataSourceId,
                                                                           miscUatpInvoice.BilledReferenceDataSourceId);


            switch (validBillingBilledLocationCombination)
            {
                case ReferenceDataErrorType.General:
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                     exceptionDetailsList.Count() + 1,
                                                                                     fileSubmissionDate, miscUatpInvoice,
                                                                                     "Member Location Information",
                                                                                     string.Empty, fileName,
                                                                                     ErrorLevels.ErrorLevelInvoice,
                                                                                     MiscUatpErrorCodes.InvalidInvoiceMemberLocationInformation,
                                                                                     ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                        
                        break;
                    }
                case ReferenceDataErrorType.Specific:
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                    exceptionDetailsList.Count() + 1,
                                                                                    fileSubmissionDate, miscUatpInvoice,
                                                                                    "Billing Member Location Information",
                                                                                    string.Empty, fileName,
                                                                                    ErrorLevels.ErrorLevelInvoice,
                                                                                    ErrorCodes.InvalidInvoiceBillingMemberLocationInformation,
                                                                                    ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                        break;
                    }
                case ReferenceDataErrorType.Both:
                    {
                        var validationExceptionDetail1 = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                     exceptionDetailsList.Count() + 1,
                                                                                     fileSubmissionDate, miscUatpInvoice,
                                                                                     "Billing Member Location Information",
                                                                                     string.Empty, fileName,
                                                                                     ErrorLevels.ErrorLevelInvoice,
                                                                                     ErrorCodes.InvalidInvoiceBillingMemberLocationInformation,
                                                                                     ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail1);

                        var validationExceptionDetail2 = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                    exceptionDetailsList.Count() + 1,
                                                                                    fileSubmissionDate, miscUatpInvoice,
                                                                                    "Member Location Information",
                                                                                    string.Empty, fileName,
                                                                                    ErrorLevels.ErrorLevelInvoice,
                                                                                     MiscUatpErrorCodes.InvalidInvoiceMemberLocationInformation,
                                                                                    ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail2);
                       
                        isValid = false;
                        break;
                    }
            }
           
            #endregion

            // Country should be validated only if reference data are provided.
            foreach (var memberData in miscUatpInvoice.MemberLocationInformation)
            {
                if (memberData.IsBillingMember && miscUatpInvoice.BillingReferenceDataSourceId.Equals((int)ReferenceDataSource.Supplied))
                {
                    if (!ReferenceManager.IsValidCountryCode(miscUatpInvoice, memberData.CountryCode))
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Billing Member Location Country code", memberData.CountryCode, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidMemberLocationInformation, ErrorStatus.X, 0, 0, true);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                    else
                    {
                        //CMP496: 2) If country code is valid then get country name from mst_country master.
                        //This validation only work when reference data is provide in file or supplied
                        //Replace it with file's country name
                        var countryName = ReferenceManager.GetCountryNameByCode(memberData.CountryCode);
                        memberData.CountryName = countryName;
                    }

                    if (!String.IsNullOrEmpty(memberData.CountryCodeIcao))
                    {
                        if (!ReferenceManager.IsValidCountryIcaoCode(memberData.CountryCode))
                        {
                            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                            exceptionDetailsList.Count() + 1,
                                                                                            fileSubmissionDate, miscUatpInvoice,
                                                                                            "Billing Member Location Country code icao",
                                                                                            memberData.CountryCodeIcao, fileName,
                                                                                            ErrorLevels.ErrorLevelInvoice,
                                                                                            MiscUatpErrorCodes.
                                                                                                InvalidMemberLocationInformation,
                                                                                            ErrorStatus.X, 0, 0, true);
                            exceptionDetailsList.Add(validationExceptionDetail);
                            isValid = false;
                        }
                    }
                }
                else if (!memberData.IsBillingMember && miscUatpInvoice.BilledReferenceDataSourceId.Equals((int)ReferenceDataSource.Supplied))
                {
                    if (!ReferenceManager.IsValidCountryCode(miscUatpInvoice, memberData.CountryCode))
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Billed Member Location Country code", memberData.CountryCode, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidMemberLocationInformation, ErrorStatus.X, 0, 0, true);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                    else
                    {
                        //CMP496: 2) If country code is valid then get country name from mst_country master.
                        //This validation only work when reference data is provide in file or supplied
                        //Replace it with file's country name
                        var countryName = ReferenceManager.GetCountryNameByCode(memberData.CountryCode);
                        memberData.CountryName = countryName;
                    }

                    if (!String.IsNullOrEmpty(memberData.CountryCodeIcao))
                    {
                        if (!ReferenceManager.IsValidCountryIcaoCode(memberData.CountryCode))
                        {
                            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Billed Member Location Country code icao", memberData.CountryCodeIcao, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidMemberLocationInformation, ErrorStatus.X, 0, 0, true);
                            exceptionDetailsList.Add(validationExceptionDetail);
                            isValid = false;
                        }
                    }
                }

                #region CMP496: 3) Validate Billed reference data if supplied in file.
                //This code will only work if billed location code equal to UATP, MISC. 
                if (!memberData.IsBillingMember && miscUatpInvoice.BilledReferenceDataSourceId == (int)ReferenceDataSource.Supplied)
                {
                    exceptionDetailsList = referenceDataRepository.ReferenceDataValidation(exceptionDetailsList, miscUatpInvoice.SubmissionMethod, fileName,
                                                                                           fileSubmissionDate, null, null,
                                                                                           miscUatpInvoice);
                    Logger.Info("Reference Data Validated ");
                }
                #endregion
            }

            miscUatpInvoice.BillingReferenceDataSourceId = (int)ReferenceDataSource.Supplied;

            // If Billed or Billing member location code is valid then corresponding location information object is added to miscInvoice MemberLocationInformation.
            if (!string.IsNullOrEmpty(miscUatpInvoice.BillingMemberLocationCode))
            {
                // Make DB call only if location code information is not present in invoice MemberLocationInformation.
                if ((miscUatpInvoice.MemberLocationInformation.Count(location => location.IsBillingMember && location.MemberLocationCode != null && location.MemberLocationCode.Equals(miscUatpInvoice.BillingMemberLocationCode)) == 0)
                && !MemberManager.IsValidMemberLocation(miscUatpInvoice.BillingMemberLocationCode, miscUatpInvoice.BillingMemberId))
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                                    fileSubmissionDate, miscUatpInvoice,
                                                                                    "Billing Member Location Code",
                                                                                    miscUatpInvoice.BillingMemberLocationCode,
                                                                                    fileName, ErrorLevels.ErrorLevelInvoice,
                                                                                    ErrorCodes.InvalidBillingMemberLocation,
                                                                                    ErrorStatus.X, 0, 0, true);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }

            if (!string.IsNullOrEmpty(miscUatpInvoice.BilledMemberLocationCode))
            {
                if ((miscUatpInvoice.MemberLocationInformation.Count(location => location.IsBillingMember == false && location.MemberLocationCode != null && location.MemberLocationCode.Equals(miscUatpInvoice.BilledMemberLocationCode)) == 0)
                            && !MemberManager.IsValidMemberLocation(miscUatpInvoice.BilledMemberLocationCode, miscUatpInvoice.BilledMemberId))
                {
                    //Invalid Billed Member Location 
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                                    fileSubmissionDate, miscUatpInvoice,
                                                                                    "Billed Member Location Code",
                                                                                    miscUatpInvoice.BilledMemberLocationCode,
                                                                                    fileName, ErrorLevels.ErrorLevelInvoice,
                                                                                    ErrorCodes.InvalidBilledMemberLocation,
                                                                                    ErrorStatus.X, 0, 0, true);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
                else
                {
                    #region CMP496: 4) Populate billed member location code. Due to new scenario introduced.
                    //A new scenario introduce when billing location code not supplied and billing ref data, billed location code, billed ref data are passed.
                    //Due this billed member location code is not populated in member location information table's column.
                    if (miscUatpInvoice.BilledReferenceDataSourceId == (int)ReferenceDataSource.Supplied)
                    {
                        miscUatpInvoice.MemberLocationInformation = referenceDataRepository.PopulateBilledMemberLocationCode(miscUatpInvoice.MemberLocationInformation,
                                                                                   miscUatpInvoice.BilledMemberId,
                                                                                   miscUatpInvoice.BilledMemberLocationCode,
                                                                                   miscUatpInvoice.BilledReferenceDataSourceId,
                                                                                   true);
                    }
                    #endregion
                }
            }

            // UC-G3320- Annexure G3320-3.1- 7
            // Validation on presence of Bank details
            // Presence of Bank Details in any of the following fields in the Account Details node in the parent node ‘OtherOrganization’ will result in validation error
            // IBAN
            // SWIFT
            // Bank Code
            // Branch Code
            // Bank Account No
            // Bank Account Name
            // Currency Code

            // Change is done by Priya R., Since populating Bank Details is now cumpolsary
            //foreach (var validationExceptionDetail in from otherOrganization in miscUatpInvoice.OtherOrganizationInformations
            //                                          where (!String.IsNullOrEmpty(otherOrganization.Iban)) || (!String.IsNullOrEmpty(otherOrganization.Swift)) || (!String.IsNullOrEmpty(otherOrganization.BankCode)) || (!String.IsNullOrEmpty(otherOrganization.BranchCode)) || (!String.IsNullOrEmpty(otherOrganization.BankAccountNumber)) || (!String.IsNullOrEmpty(otherOrganization.BankAccountName)) || (otherOrganization.Currency != null)
            //                                          select CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Other Organization Account Details", string.Empty, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidOtherOrganizationAccountDetails, ErrorStatus.X, 0, 0))
            //{
            //  exceptionDetailsList.Add(validationExceptionDetail);
            //  isValid = false;
            //}

            // If clearance 
            if (miscUatpInvoice.BillingCurrency == null && billedMember != null && billingMember != null)
            {
                GetClearanceCurrency(billingMember, billedMember, miscUatpInvoice);
            }

            // This is mandatory field.
            if (miscUatpInvoice.ListingCurrency == null)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Listing Currency", miscUatpInvoice.ListingCurrencyDisplayText, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidBillingCurrency, ErrorStatus.X, 0, 0);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            if (miscUatpInvoice.BillingMemberId != 0 && miscUatpInvoice.BilledMemberId != 0)
            {
                BillingMember = billingMember;
                BilledMember = billedMember;

                if ((billedMember != null) && (billingMember != null))
                {
                    #region CMP #624: ICH Rewrite-New SMI X
                    //Validate Non-X SMI
                    if (miscUatpInvoice.SettlementMethodId != (int)SMI.IchSpecialAgreement)
                    {
                      if (!ValidateSettlementMethod(miscUatpInvoice, billingFinalParent, billedFinalParent))
                      {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Settlement Method", miscUatpInvoice.SettlementMethodDisplayText, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidSettlementMethod, ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                      }
                      else //CMP602
                      {
                        SetViewableByClearingHouse(miscUatpInvoice);
                      }
                    }

                    #endregion

                    // Update suspended flag according to ach/Ach configuration.
                    if (ValidateSuspendedFlag(miscUatpInvoice, billingFinalParent, billedFinalParent))
                    {
                        miscUatpInvoice.SuspendedInvoiceFlag = true;
                    }

                    // UC-G3320- Annexure G3320-3.1 -1
                    if (!isOnbehalf)
                    {
                        if (!ValidateBillingMembershipStatus(billingMember))
                        {
                            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Billing Member", miscUatpInvoice.BillingMember != null ? miscUatpInvoice.BillingMember.MemberCodeNumeric : string.Empty, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidBillingMemberStatus, ErrorStatus.X, 0, 0, true);
                            exceptionDetailsList.Add(validationExceptionDetail);
                            isValid = false;
                        }
                    }
                    else
                    {
                        var issuingMemberDetails = MemberManager.GetMemberDetails(issuingOrganizationId);

                        if (!ValidateBillingMembershipStatus(issuingMemberDetails))
                        {
                            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Isuuing Member For OnbehalfOf File", issuingOrgMemberCode, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidBillingMemberStatus, ErrorStatus.X, 0, 0, true);
                            exceptionDetailsList.Add(validationExceptionDetail);
                            isValid = false;
                        }
                    }

                    // UC-G3320- Annexure G3320-3.1 -2
                    if (!ValidateBilledMemberStatus(billedMember))
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Billed Member", miscUatpInvoice.BilledMember != null ? miscUatpInvoice.BilledMember.MemberCodeNumeric : string.Empty, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidBilledMemberStatus, ErrorStatus.X, 0, 0, true);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }

                    // Get Ich configurations for the billing and billed members.
                    billingMember.IchConfiguration = GetIchConfiguration(billingMember.Id);
                    billedMember.IchConfiguration = GetIchConfiguration(billedMember.Id);

                    //Fixed Issue Id : 65713 
                    // Retrieve Ich Configurations of the Parent members only.
                    if (billingFinalParent != null)
                        billingFinalParent.IchConfiguration = MemberManager.GetIchConfig(billingFinalParent.Id);
                    if (billedFinalParent != null)
                        billedFinalParent.IchConfiguration = GetIchConfiguration(billedFinalParent.Id);
                    
                    //Validation for Blocked Airline
                    var isCreditorBlocked = false;
                    var isDebitorBlocked = false;
                    var isCGrpBlocked = false;
                    var isDGrpBlocked = false;

                    //SCP164383: Blocking Rule Failed
                    //Desc: Hooking a call to centralized code for blocking rule validation
                    ValidationForBlockedAirline(miscUatpInvoice.BillingMemberId, miscUatpInvoice.BilledMemberId, (SMI)miscUatpInvoice.InvoiceSmi,
                            miscUatpInvoice.BillingCategory, out isCreditorBlocked, out isDebitorBlocked,
                            out isCGrpBlocked, out isDGrpBlocked);

                    // Blocked by Debtor
                    if (isDebitorBlocked)
                    {
                      //SCP164383: Blocking Rule Failed
                      //Desc: Centralized code for blocking rule validation.
                      var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, string.Empty, string.Empty, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidBillingFromMember, ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                    // Blocked by Creditor
                    if (isCreditorBlocked)
                    {
                      //SCP164383: Blocking Rule Failed
                      //Desc: Centralized code for blocking rule validation.
                      var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, string.Empty, string.Empty, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidBillingToMember, ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }

                    //Validate BlockBy Group Rule
                    if (isCGrpBlocked)
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, string.Empty, string.Empty, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidBillingToMemberGroup, ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }

                    if (isDGrpBlocked)
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, string.Empty, string.Empty, fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidBillingFromMemberGroup, ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                }

                if (miscUatpInvoice.BillingCurrency.HasValue && billingMember != null && billedMember != null)
                {
                    //CMP #553: ACH Requirement for Multiple Currency Handling-FRS-v1.1.doc
                    if(!ValidateBillingCurrency(miscUatpInvoice, billingFinalParent, billedFinalParent, true, exceptionDetailsList, fileName, fileSubmissionDate))
                    {
                        isValid = false;
                    }
                }
                // SCP177435 - EXCHANGE RATE 
                if (miscUatpInvoice.ListingCurrencyId.HasValue == true && billingMember != null && billedMember != null)
                {
                  if (!ValidateListingCurrency(miscUatpInvoice, billingFinalParent, billedFinalParent))
                  {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Clearance Currency", miscUatpInvoice.BillingCurrencyDisplayText, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidClearanceCurrency, ErrorStatus.X, 0, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                  }
                }
            }

            // BugId-3997 validation for organization designator.
            foreach (var meberLocationinfo in miscUatpInvoice.MemberLocationInformation)
            {
                if (meberLocationinfo.OrganizationDesignator != null)
                {
                    if (meberLocationinfo.IsBillingMember)
                    {
                        if (billingMember != null && !meberLocationinfo.OrganizationDesignator.Equals(billingMember.MemberCodeAlpha))
                        {
                            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "OrganizationDesignator", meberLocationinfo.OrganizationDesignator, fileName, ErrorLevels.ErrorLevelInvoice,
                                                                                            ErrorCodes.InvalidOrganizationDesignator, ErrorStatus.X, 0, 0, true);

                            exceptionDetailsList.Add(validationExceptionDetail);
                            isValid = false;
                        }
                    }
                    else
                    {
                        if (billedMember != null && !meberLocationinfo.OrganizationDesignator.Equals(billedMember.MemberCodeAlpha))
                        {
                            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "OrganizationDesignator", meberLocationinfo.OrganizationDesignator, fileName, ErrorLevels.ErrorLevelInvoice,
                                                                                              ErrorCodes.InvalidOrganizationDesignator, ErrorStatus.X, 0, 0, true);
                            exceptionDetailsList.Add(validationExceptionDetail);
                            isValid = false;
                        }
                    }
                }
            }

            // IsDetails
            if (!Enum.IsDefined(typeof(DigitalSignatureRequired), miscUatpInvoice.DigitalSignatureRequired))
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Digital Signature", string.Empty, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidDigitalSignatureFlag, ErrorStatus.X, 0, 0);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
            }

            // Invoice Summary.
            if (miscUatpInvoice.InvoiceSummary != null)
            {
                //  double summationTolerance = 0;
                //  double roundingTolerance = 0;
                if (miscUatpInvoice.Tolerance != null)
                {
                    summationTolerance = miscUatpInvoice.Tolerance.SummationTolerance;
                    roundingTolerance = miscUatpInvoice.Tolerance.RoundingTolerance;
                }
                // Min max amount check for normal invoice.
                if (miscUatpInvoice.InvoiceType == InvoiceType.Invoice)
                {
                    if (!ValidateInvoiceTotalAmount(miscUatpInvoice))
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Invoice Summary TotalAmount", miscUatpInvoice.InvoiceSummary.TotalAmount.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidMinMaxAmount, ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }

                }

                var lineItemTotalChargeAmountSum = miscUatpInvoice.LineItems.Sum(lineItem => lineItem.ChargeAmount);

                if (!CompareUtil.Compare(miscUatpInvoice.InvoiceSummary.TotalLineItemAmount, lineItemTotalChargeAmountSum, summationTolerance, Constants.MiscDecimalPlaces))
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Invoice Summary TotalLineItemAmount", miscUatpInvoice.InvoiceSummary.TotalLineItemAmount.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidTotalLineItemAmount, ErrorStatus.X, 0, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }

                // Total Add-On Charge Amount should be equal to sum of Calculated amount in Add-On Charges breakdown
                decimal invoiceAddOnChargeAmountSum = 0;
                if (miscUatpInvoice.AddOnCharges.Count > 0)
                {
                    // Validate Add on Charge 
                    foreach (var addoncharge in miscUatpInvoice.AddOnCharges)
                    {
                        ValidateParsedAddonCharge(addoncharge, exceptionDetailsList, fileName, miscUatpInvoice, fileSubmissionDate, ErrorLevels.ErrorLevelInvoice, roundingTolerance);
                    }

                    invoiceAddOnChargeAmountSum = miscUatpInvoice.AddOnCharges.Sum(addonCharge => addonCharge.Amount);
                }

                decimal lineItemTotalAddOnChargeAmountSum = 0;

                if (miscUatpInvoice.LineItems.Count > 0)
                    lineItemTotalAddOnChargeAmountSum = miscUatpInvoice.LineItems.Sum(lineItem => lineItem.TotalAddOnChargeAmount.HasValue ? lineItem.TotalAddOnChargeAmount.Value : 0);

                // TotalAddOnChargeAmount should be provided if AddOnCharges of invoice or lineItem level is provided
                if ((invoiceAddOnChargeAmountSum != 0 || lineItemTotalAddOnChargeAmountSum != 0) && miscUatpInvoice.InvoiceSummary.TotalAddOnChargeAmount == null)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                     exceptionDetailsList.Count() + 1,
                                                                                     fileSubmissionDate, miscUatpInvoice,
                                                                                     "Invoice Summary TotalAddonChargeAmount",
                                                                                     miscUatpInvoice.InvoiceSummary.
                                                                                       TotalAddOnChargeAmount.ToString(),
                                                                                     fileName, ErrorLevels.ErrorLevelInvoice,
                                                                                     MiscUatpErrorCodes.
                                                                                       InvalidTotalAddOnChargeAmount,
                                                                                     ErrorStatus.X, 0, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
                // TotalAddOnChargeAmount should be equal to sum of AddOnCharge amount in AddOnCharges of invoice + lineItem Total AddOnCharges amount
                else if (miscUatpInvoice.InvoiceSummary.TotalAddOnChargeAmount != null)
                {
                    if (
                      !CompareUtil.Compare(miscUatpInvoice.InvoiceSummary.TotalAddOnChargeAmount.Value,
                                           (invoiceAddOnChargeAmountSum + lineItemTotalAddOnChargeAmountSum), summationTolerance,
                                           Constants.MiscDecimalPlaces))
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                        exceptionDetailsList.Count() + 1,
                                                                                        fileSubmissionDate, miscUatpInvoice,
                                                                                        "Invoice Summary TotalAddonChargeAmount",
                                                                                        miscUatpInvoice.InvoiceSummary.
                                                                                          TotalAddOnChargeAmount.ToString(),
                                                                                        fileName, ErrorLevels.ErrorLevelInvoice,
                                                                                        MiscUatpErrorCodes.
                                                                                          InvalidTotalAddOnChargeAmount,
                                                                                        ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                }

                var totalAmount = miscUatpInvoice.InvoiceSummary.TotalLineItemAmount + (miscUatpInvoice.InvoiceSummary.TotalTaxAmount.HasValue ? miscUatpInvoice.InvoiceSummary.TotalTaxAmount.Value : 0) + (miscUatpInvoice.InvoiceSummary.TotalVatAmount.HasValue ? miscUatpInvoice.InvoiceSummary.TotalVatAmount.Value : 0) + (miscUatpInvoice.InvoiceSummary.TotalAddOnChargeAmount.HasValue ? miscUatpInvoice.InvoiceSummary.TotalAddOnChargeAmount.Value : 0);

                if (!CompareUtil.Compare(miscUatpInvoice.InvoiceSummary.TotalAmount, totalAmount, summationTolerance, Constants.MiscDecimalPlaces))
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Invoice Summary TotalAmount", miscUatpInvoice.InvoiceSummary.TotalAmount.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidTotalAmount, ErrorStatus.X, 0, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }

            // Credit Note validation
            // UC-G3320 3.1 10 For Credit note invoice the value of the fields 'Rejection Flag' and 'Correspondence Flag' should not be 'Y' 
            if (miscUatpInvoice.IsCreditNote && miscUatpInvoice.InvoiceType != InvoiceType.CreditNote)
            {
                var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Invoice Type",
                                          miscUatpInvoice.InvoiceType.ToString(),
                                          fileName, ErrorLevels.ErrorLevelInvoice,
                                          MiscUatpErrorCodes.InvalidInvoiceType, ErrorStatus.X, 0, 0);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;


            }

            //Dleleted commented code

            // IS WEB 9.2.1 b. For Credit Note	Rejection and Correspondence Section will not be shown.
            if (miscUatpInvoice.IsCreditNote)
            {
                if (miscUatpInvoice.RejectionStage != 0 || !string.IsNullOrEmpty(miscUatpInvoice.RejectedInvoiceNumber) || miscUatpInvoice.IsAuthorityToBill ||
                  miscUatpInvoice.CorrespondenceRefNo.HasValue)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Invoice Type",
                                             miscUatpInvoice.InvoiceType.ToString(),
                                             fileName, ErrorLevels.ErrorLevelInvoice,
                                             MiscUatpErrorCodes.InvalidRejectionCorrespondenceNodes, ErrorStatus.X, 0, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }

            var netAmount = Convert.ToDecimal(miscUatpInvoice.ExchangeRate) > 0 ? Convert.ToDecimal(miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency) * Convert.ToDecimal(miscUatpInvoice.ExchangeRate) : 0;

            // Update Amount in clearance currency
            if (miscUatpInvoice.InvoiceSummary != null && miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
            {
                if (Convert.ToDecimal(miscUatpInvoice.ExchangeRate) == 0)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate",
                                             miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.Value.ToString(),
                                             fileName, ErrorLevels.ErrorLevelInvoice,
                                             MiscUatpErrorCodes.ExchangeRateCannotBeZero, ErrorStatus.X, 0, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
                else if (miscUatpInvoice.Tolerance != null && !CompareUtil.Compare(miscUatpInvoice.InvoiceSummary.TotalAmount, Convert.ToDecimal(netAmount), miscUatpInvoice.Tolerance.RoundingTolerance, Constants.MiscDecimalPlaces))
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Total Amount In Clearance Currency",
                                             miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.Value.ToString(),
                                             fileName, ErrorLevels.ErrorLevelInvoice,
                                             MiscUatpErrorCodes.InvalidTotalAmountInClearanceCurrency, ErrorStatus.X, 0, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }
            else if (miscUatpInvoice.InvoiceSummary != null && miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && !miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
            {
                if (Convert.ToDecimal(miscUatpInvoice.ExchangeRate) == 0)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate",
                                             miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.Value.ToString(),
                                             fileName, ErrorLevels.ErrorLevelInvoice,
                                             MiscUatpErrorCodes.ExchangeRateIsMadatoryForAmountInClearanceCurrency, ErrorStatus.X, 0, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }
            //Validate Exchange rate for SMI X
            if (miscUatpInvoice.SettlementMethodId == (int)SMI.IchSpecialAgreement)
            {
              ValidateExchangeRateForSmiX(miscUatpInvoice, exceptionDetailsList, fileName, fileSubmissionDate);
            }
            else
            { //CMP#648:Clearance Information in MISC Invoice PDFs
              if (miscUatpInvoice.ListingCurrencyId != null && miscUatpInvoice.BillingCurrency != null)
              {
                //Validate Non-X SMI Exchange Rate
                // Exchange Rate Validation.
                ValidateParsedExchangeRate(miscUatpInvoice,
                                           originalInvoice,
                                           linkedRm1Invoice,
                                           linkedRm2Invoice,
                                           linkedCorrespondence,
                                           exceptionDetailsList,
                                           fileName,
                                           fileSubmissionDate,
                                           clearingHouseEnum,
                                           billingPeriod);
              }
            }
          // If Exchange Rate is not found Or <= Zero. Mark invoice as Error Non-corractable. Except for SMI treat as bilateral
            //Author : Vinod Patil
            // SCPID : 50966
            /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
            if (!ReferenceManager.IsSmiLikeBilateral(miscUatpInvoice.SettlementMethodId, false))
            {
                if (Convert.ToDecimal(miscUatpInvoice.ExchangeRate) <= 0 && miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                    exceptionDetailsList.Count() + 1,
                                                                                    fileSubmissionDate,
                                                                                    miscUatpInvoice, "Exchange Rate",
                                                                                    Convert.ToString(
                                                                                        miscUatpInvoice.ExchangeRate),
                                                                                    fileName,
                                                                                    ErrorLevels.ErrorLevelInvoice,
                                                                                    MiscUatpErrorCodes.
                                                                                        InvalidExchangeRate,
                                                                                    ErrorStatus.X, 0, 0, true);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }
            BillingPeriod currentBillingPeriod;

            // Validation exclusively for Misc Type Invoice
            // Validation for CityAirport as it is auto complete field in UI Is-Web specification 9.1.1.5)
            if (!string.IsNullOrEmpty(miscUatpInvoice.LocationCode) && !IsValidCityAirportCode(miscUatpInvoice.LocationCode))
            {
                if (!IsValidUnlocCode(miscUatpInvoice.LocationCode))
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "InvoiceHeader LocationCode", miscUatpInvoice.LocationCode, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidIataAirPrtCityLocationCode, ErrorStatus.X, 0, 0, true);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }

            if (!string.IsNullOrEmpty(miscUatpInvoice.LocationCodeIcao))
            {
                var referenceManager = Ioc.Resolve<IReferenceManager>(typeof(IReferenceManager));
                if (!referenceManager.IsValidLocationIcaoCode(miscUatpInvoice.LocationCodeIcao))
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "InvoiceHeader LocationCodeIcao", miscUatpInvoice.LocationCodeIcao, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidLocationCodeIcao, ErrorStatus.X, 0, 0, true);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }

            if (miscUatpInvoice.PaymentDetail != null)
            {
                if (miscUatpInvoice.PaymentDetail.DiscountPercent > 100)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Payment Discount Percent", Convert.ToString(miscUatpInvoice.PaymentDetail.DiscountPercent), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidPaymentDiscountPercent, ErrorStatus.X, 0, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }

                if (miscUatpInvoice.PaymentDetail.DiscountDueDays > 366)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Payment Discount DueDays", Convert.ToString(miscUatpInvoice.PaymentDetail.DiscountDueDays), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidPaymentDiscountDueDays, ErrorStatus.X, 0, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }

                if (miscUatpInvoice.PaymentDetail.NetDueDays > 366)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Payment Discount NetDueDays", Convert.ToString(miscUatpInvoice.PaymentDetail.NetDueDays), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidPaymentNetDueDays, ErrorStatus.X, 0, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }

            try
            {
                currentBillingPeriod = CalendarManager.GetCurrentBillingPeriod();
            }
            catch (ISCalendarDataNotFoundException)
            {
                currentBillingPeriod = CalendarManager.GetLastClosedBillingPeriod(fileSubmissionDate, clearingHouseEnum);
            }

            //CMP#624 : Code review changes => Rejection code validation moved in "ValidateRejectionInvoiceDetails" method 
            #region InvoiceType.RejectionInvoice
            ////Rejection Invoice
            //if (miscUatpInvoice.InvoiceType == InvoiceType.RejectionInvoice)
            //{

            //   //SCP126263:SRM 4.5 - Admin Alert - Parsing and Validation failure 055
            //    /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH or Bilateral */
            //    if ((miscUatpInvoice.SettlementMethodId == (int)SMI.Ich || ReferenceManager.IsSmiLikeBilateral(miscUatpInvoice.SettlementMethodId, true)) && miscUatpInvoice.RejectionStage > 1)
            //    {
            //        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Rejection Stage", miscUatpInvoice.RejectionStage.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidRejectionStage, ErrorStatus.X, 0, 0);
            //        exceptionDetailsList.Add(validationExceptionDetail);
            //        isValid = false;
            //    }

            //    //SCP126263:SRM 4.5 - Admin Alert - Parsing and Validation failure 055
            //    if ((miscUatpInvoice.SettlementMethodId == (int)SMI.Ach || miscUatpInvoice.SettlementMethodId == (int)SMI.AchUsingIataRules || miscUatpInvoice.SettlementMethodId == (int)SMI.AdjustmentDueToProtest))
            //    {
            //        //if ((miscUatpInvoice.RejectionStage != 1) && (miscUatpInvoice.RejectionStage != 2))
            //        if (miscUatpInvoice.RejectionStage > 2)
            //        {
            //            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Rejection Stage",
            //                                                                            miscUatpInvoice.RejectionStage.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidRejectionStage, ErrorStatus.X, 0, 0);
            //            exceptionDetailsList.Add(validationExceptionDetail);
            //            isValid = false;
            //        }
            //    }

            //    //SCP126263:SRM 4.5 - Admin Alert - Parsing and Validation failure 055
            //    if (string.IsNullOrWhiteSpace(miscUatpInvoice.RejectedInvoiceNumber))
            //    {
            //      var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Rejection Invoice Number",
            //                                                                       string.Empty,fileName, ErrorLevels.ErrorLevelInvoice,MiscUatpErrorCodes.RejectionInvoiceNumberMandatory, ErrorStatus.X, 0, 0);
            //      exceptionDetailsList.Add(validationExceptionDetail);
            //      isValid = false;
            //    }

            //    if (miscUatpInvoice.RejectionStage == 0)
            //    {
            //        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Rejection Stage",
            //                                                                           miscUatpInvoice.RejectionStage.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidRejectionDetailStage, ErrorStatus.X, 0, 0);
            //        exceptionDetailsList.Add(validationExceptionDetail);
            //        isValid = false;
            //    }
                
            //    var linkedInvoice = GetMUInvoicePriviousTransanction(miscUatpInvoice);

            //    bool isMemberMigrated = false;

            //    // Ignore any validations related to billing history if System Parameter is true.
            //    if (!SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod)
            //    {
            //      // Linking process validation if Rejection Flag is "Y".
            //      // Billed member has migrated but could not find the data of the rejected invoice, throw an error.
            //      if (IsMemberMigrated(miscUatpInvoice))
            //      {
            //        isMemberMigrated = true;
            //      }
            //    }
                
            //    if (linkedInvoice == null)
            //    {
            //        if (isMemberMigrated)
            //        {
            //            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Rejection Invoice Number", miscUatpInvoice.RejectedInvoiceNumber, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.RejectionInvoiceNumberNotExist, ErrorStatus.C, 0, 0, islinkingError: true);
            //            exceptionDetailsList.Add(validationExceptionDetail);
            //            isValid = false;
            //        }
            //        else
            //        {

            //            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Rejection Invoice Number", miscUatpInvoice.RejectedInvoiceNumber, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.RejectionInvoiceNumberNotExist, ErrorStatus.W, 0, 0);
            //            exceptionDetailsList.Add(validationExceptionDetail);
            //            isValid = false;
            //        }

            //    }
            //    else
            //    {
            //        //SCP0000:Impact on MISC/UATP rejection linking due to purging
            //        if (IsLinkedInvoicePurged(linkedInvoice))
            //        {
            //          var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Rejection Invoice Number", miscUatpInvoice.RejectedInvoiceNumber, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.RejectionInvoiceNumberNotExist, ErrorStatus.C, 0, 0, islinkingError: true);
            //          exceptionDetailsList.Add(validationExceptionDetail);
            //          isValid = false;
            //        }
            //        //CMP#624 : New Validation #8:SMI Match Check for MISC/UATP Rejection Invoices
            //        else if (!ValidateSmiAfterLinking(miscUatpInvoice.SettlementMethodId, linkedInvoice.SettlementMethodId))
            //        {
            //          if (miscUatpInvoice.SettlementMethodId == (int)SMI.IchSpecialAgreement)
            //          {
            //            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
            //                                                                            exceptionDetailsList.Count() + 1,
            //                                                                            fileSubmissionDate,
            //                                                                            miscUatpInvoice,
            //                                                                            "Rejection Invoice Number",
            //                                                                            miscUatpInvoice.RejectedInvoiceNumber,
            //                                                                            fileName,
            //                                                                            ErrorLevels.ErrorLevelInvoice,
            //                                                                            MiscUatpErrorCodes.MuRejctionInvoiceSmiCheckForSmiX,
            //                                                                            ErrorStatus.X,
            //                                                                            0,
            //                                                                            0,
            //                                                                            islinkingError: false);
            //            exceptionDetailsList.Add(validationExceptionDetail);
            //            isValid = false;
            //          }
            //          else
            //          {
            //            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
            //                                                                            exceptionDetailsList.Count() + 1,
            //                                                                            fileSubmissionDate,
            //                                                                            miscUatpInvoice,
            //                                                                            "Rejection Invoice Number",
            //                                                                            miscUatpInvoice.RejectedInvoiceNumber,
            //                                                                            fileName,
            //                                                                            ErrorLevels.ErrorLevelInvoice,
            //                                                                            MiscUatpErrorCodes.MuRejctionInvoiceSmiCheckForSmiOtherThanX,
            //                                                                            ErrorStatus.X,
            //                                                                            0,
            //                                                                            0,
            //                                                                            islinkingError: false);
            //            exceptionDetailsList.Add(validationExceptionDetail);
            //            isValid = false;
            //          }

            //        }

            //        if (miscUatpInvoice.InvoiceType == InvoiceType.RejectionInvoice && linkedInvoice.InvoiceTypeId == (int)InvoiceType.CorrespondenceInvoice)
            //        {
            //            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice,
            //                                                                            "Rejection Invoice number", miscUatpInvoice.RejectedInvoiceNumber,
            //                                                                            fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.CorrespondenceInvoiceCannotBeRejected,
            //                                                                            ErrorStatus.X);
            //            exceptionDetailsList.Add(validationExceptionDetail);
            //            isValid = false;
            //        }

            //        if (miscUatpInvoice.RejectionStage == 1 && linkedInvoice.InvoiceTypeId == (int)InvoiceType.RejectionInvoice)
            //        {
            //            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice,
            //                                                                            "Rejection Invoice number", miscUatpInvoice.RejectedInvoiceNumber,
            //                                                                            fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidRejectionStageOfCurrentRejection,
            //                                                                            ErrorStatus.X);
            //            exceptionDetailsList.Add(validationExceptionDetail);
            //            isValid = false;
            //        }

            //        if (miscUatpInvoice.RejectionStage == 2)
            //        {
            //            if (linkedInvoice.InvoiceTypeId != (int)InvoiceType.RejectionInvoice || linkedInvoice.RejectionStage != 1)
            //            {
            //                var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice,
            //                                                                                "Rejection Invoice number", miscUatpInvoice.RejectedInvoiceNumber,
            //                                                                                fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvoiceIsNotRejectedInvoice,
            //                                                                                ErrorStatus.X);
            //                exceptionDetailsList.Add(validationExceptionDetail);
            //                isValid = false;
            //            }
            //        }
            //        // Check for invoices with other status.
            //        //SCPID : 117317 - question about same invoice No, check settlement year as well.
            //        //SCP251726: Two reject invoices for same original invoice number
            //        var invoiceCount = MiscUatpInvoiceRepository.GetCount(invoice =>invoice.RejectedInvoiceNumber.ToUpper() == miscUatpInvoice.RejectedInvoiceNumber.ToUpper() && 
            //                                        invoice.BillingMemberId == miscUatpInvoice.BillingMemberId && invoice.BilledMemberId == miscUatpInvoice.BilledMemberId && 
            //                                        invoice.BillingCategoryId == miscUatpInvoice.BillingCategoryId && invoice.SettlementYear == miscUatpInvoice.SettlementYear
            //                                        && !(invoice.InvoiceStatusId == (int)InvoiceStatusType.ErrorNonCorrectable && invoice.ValidationStatusId == (int)InvoiceValidationStatus.Failed));
            //        if (invoiceCount > 0)
            //        {
            //            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice,
            //                                                                           "Rejection Invoice number", miscUatpInvoice.RejectedInvoiceNumber,
            //                                                                           fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvoiceAlreadyRejected,
            //                                                                           ErrorStatus.X);
            //            exceptionDetailsList.Add(validationExceptionDetail);
            //            isValid = false;
            //        }
            //    }

            //    if (miscUatpInvoice.InvoiceSummary != null)
            //    {
            //        var transactionType = miscUatpInvoice.RejectionStage == 1 ? TransactionType.MiscRejection1 : TransactionType.MiscRejection2;
            //        MaxAcceptableAmount maxAcceptableAmount;
            //        MinAcceptableAmount minAcceptableAmount;

            //        var clearingHouse = ReferenceManager.GetClearingHouseFromSMI(miscUatpInvoice.SettlementMethodId);

            //        maxAcceptableAmount = GetMaxAcceptableAmount(miscUatpInvoice, clearingHouse, transactionType);

            //        if (maxAcceptableAmount != null && !ReferenceManager.IsValidNetAmount(Convert.ToDouble(miscUatpInvoice.InvoiceSummary.TotalAmount), transactionType, miscUatpInvoice.ListingCurrencyId, miscUatpInvoice, iMaxAcceptableAmount: maxAcceptableAmount, applicableMinimumField: ApplicableMinimumField.TotalAmount))
            //        {
            //            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Total Amount", miscUatpInvoice.InvoiceSummary.TotalAmount.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidTotalAmountOutsideLimit, ErrorStatus.X, 0, 0);
            //            exceptionDetailsList.Add(validationExceptionDetail);
            //            isValid = false;
            //        }

            //        minAcceptableAmount = GetMinAcceptableAmounts(miscUatpInvoice, clearingHouse, transactionType).FirstOrDefault();

            //        if (minAcceptableAmount != null && !ReferenceManager.IsValidNetAmount(Convert.ToDouble(miscUatpInvoice.InvoiceSummary.TotalAmount), transactionType, miscUatpInvoice.ListingCurrencyId, miscUatpInvoice, iMinAcceptableAmount: minAcceptableAmount, applicableMinimumField: ApplicableMinimumField.TotalAmount))
            //        {
            //            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Total Amount", miscUatpInvoice.InvoiceSummary.TotalAmount.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidTotalAmountOutsideLimit, ErrorStatus.X, 0, 0);
            //            exceptionDetailsList.Add(validationExceptionDetail);
            //            isValid = false;
            //        }

            //    }
            //     //SCP126263: SRM 4.5 - Admin Alert - Parsing and Validation failure 055
            //    // Validate settlement period; must be smaller than current period

            //    if (miscUatpInvoice.SettlementPeriod > 0 && miscUatpInvoice.SettlementMonth > 0 && miscUatpInvoice.SettlementYear > 0)
            //    {
            //      DateTime settlementDate;

            //      var cultureInfo = new CultureInfo("en-US");
            //      cultureInfo.Calendar.TwoDigitYearMax = 2099;
            //      const string billingDateFormat = "yyyyMMdd";

            //      DateTime.TryParseExact(string.Format("{0}{1}{2}", miscUatpInvoice.SettlementYear, miscUatpInvoice.SettlementMonth.ToString("00"), miscUatpInvoice.SettlementPeriod.ToString("00")), billingDateFormat, cultureInfo, DateTimeStyles.None, out settlementDate);
                  
            //      int settlementMethodId = linkedInvoice != null? linkedInvoice.SettlementMethodId: miscUatpInvoice.SettlementMethodId;

            //     var currentBillingP =  CalendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ReferenceManager.GetClearingHouseToFetchCurrentBillingPeriod(settlementMethodId));

            //      var settlementMonthPeriod = new BillingPeriod
            //      {
            //        Period = settlementDate.Day,
            //        Month = settlementDate.Month,
            //        Year = settlementDate.Year
            //      };

            //      miscUatpInvoice.ValidationDate = settlementDate;

            //      if (settlementMonthPeriod > currentBillingP)
            //      {
            //        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Rejection Invoice Settlement Period", string.Format("{0}-{1}-{2}", miscUatpInvoice.SettlementYear.ToString().PadLeft(2, '0'), miscUatpInvoice.SettlementMonth.ToString().PadLeft(2, '0'), miscUatpInvoice.SettlementPeriod.ToString().PadLeft(2, '0')),
            //                                                                        fileName, ErrorLevels.ErrorLevelInvoice,MiscUatpErrorCodes.InvalidSettlementMonthPeriod, ErrorStatus.X, 0, 0);
            //        exceptionDetailsList.Add(validationExceptionDetail);
            //        isValid = false;
            //      }
            //      if (DateTime.TryParseExact(string.Format("{0}{1}{2}", miscUatpInvoice.SettlementYear, miscUatpInvoice.SettlementMonth.ToString("00"), miscUatpInvoice.SettlementPeriod.ToString("00")), billingDateFormat, cultureInfo, DateTimeStyles.None, out settlementDate))
            //      {
            //        // Transaction out side time limit validation. UC-G3320-3.2
            //        if (IsTransactionOutSideTimeLimit(miscUatpInvoice))
            //        {
            //          miscUatpInvoice.IsValidationFlag += string.IsNullOrEmpty(miscUatpInvoice.IsValidationFlag)? TimeLimitFlag: ValidationFlagDelimeter + TimeLimitFlag;
            //        }
            //      }
            //    }
            //    else
            //    {
            //        //SCP126263:SRM 4.5 - Admin Alert - Parsing and Validation failure 055
            //      var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Rejection Invoice Settlement Period", string.Format("{0}-{1}-{2}", miscUatpInvoice.SettlementYear.ToString().PadLeft(2, '0'), miscUatpInvoice.SettlementMonth.ToString().PadLeft(2, '0'), miscUatpInvoice.SettlementPeriod.ToString().PadLeft(2, '0')),
            //                                                                      fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.RejectionSettlementPeriodMandatory, ErrorStatus.X, 0, 0);
            //      exceptionDetailsList.Add(validationExceptionDetail);
            //      isValid = false;
            //    }
                
            //    // Invoice number and rejected invoice number can not be same. 
            //    if (miscUatpInvoice.InvoiceNumber.ToLower().Equals(miscUatpInvoice.RejectedInvoiceNumber))
            //    {
            //      var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Rejection Invoice Number", miscUatpInvoice.RejectedInvoiceNumber, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidRejectionInvoiceNumber, ErrorStatus.X, 0, 0);
            //      exceptionDetailsList.Add(validationExceptionDetail);
            //      isValid = false;
            //    }

            //}
            

            #endregion

            if (miscUatpInvoice.InvoiceType == InvoiceType.RejectionInvoice)
            {
              if (!ValidateRejectionInvoiceDetails(miscUatpInvoice, originalInvoice, linkedRm1Invoice, exceptionDetailsList, fileName, fileSubmissionDate))
              {
                isValid = false;
              }
            }

          //CMP#624 : Code review changes => Correspondence code validation moved in "ValidateCorrespondenceInvoiceDetails" method 
            #region InvoiceType.CorrespondenceInvoice

            
            ////Correspondence invoice
            //if (miscUatpInvoice.InvoiceType == InvoiceType.CorrespondenceInvoice)
            //{
            //    if (!string.IsNullOrEmpty(miscUatpInvoice.CorrespondenceRefNo.ToString()))
            //    {
            //        // fetch correspondence to validate Correspondence Invoice. 
            //        //SCP199693:create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202] 

            //        var miscCorrespondence =
            //          MiscCorrespondenceRepository.GetCorrespondenceWithInvoice(
            //            miscCorrs =>
            //            miscCorrs.CorrespondenceNumber == miscUatpInvoice.CorrespondenceRefNo &&
            //            (((miscCorrs.CorrespondenceStatusId == (int)CorrespondenceStatus.Open || miscCorrs.CorrespondenceStatusId == (int)CorrespondenceStatus.Expired) 
            //            /*&& miscCorrs.CorrespondenceSubStatusId == (int)CorrespondenceSubStatus.Responded*/) ||
            //             miscCorrs.CorrespondenceStatusId == (int)CorrespondenceStatus.Closed)).OrderByDescending(miscCorr2 => miscCorr2.CorrespondenceStage).FirstOrDefault();

            //        if (miscCorrespondence == null)
            //        {
            //            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Correspondence Ref No", miscUatpInvoice.CorrespondenceRefNo.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvoiceNotExistForCorrespondence, ErrorStatus.X, 0, 0, islinkingError: true);
            //            exceptionDetailsList.Add(validationExceptionDetail);
            //            isValid = false;
            //        }
            //        else
            //        {

            //            var rejectedInvoice = miscCorrespondence.Invoice;

            //            if (rejectedInvoice == null)
            //            {
            //                var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Correspondence Ref No", miscUatpInvoice.RejectedInvoiceNumber, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvoiceNotExistForCorrespondence, ErrorStatus.C, 0, 0, islinkingError: true);
            //                exceptionDetailsList.Add(validationExceptionDetail);
            //                isValid = false;
            //            }
            //            else if (rejectedInvoice.InvoiceNumber.ToUpper() != miscUatpInvoice.RejectedInvoiceNumber.ToUpper())
            //            {
            //                var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Correspondence Ref No", miscUatpInvoice.RejectedInvoiceNumber, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvoiceNotExistForCorrespondence, ErrorStatus.C, 0, 0, islinkingError: true);
            //                exceptionDetailsList.Add(validationExceptionDetail);
            //                isValid = false;
            //            }
            //            else
            //            {
            //              //CMP#624 :  New Validation #9:SMI Match Check for MISC/UATP Correspondence Invoices 
            //              if (!ValidateSmiAfterLinking(miscUatpInvoice.SettlementMethodId, rejectedInvoice.SettlementMethodId))
            //              {
            //                if (miscUatpInvoice.SettlementMethodId == (int)SMI.IchSpecialAgreement)
            //                {
            //                  var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
            //                                                                                  exceptionDetailsList.Count() + 1,
            //                                                                                  fileSubmissionDate,
            //                                                                                  miscUatpInvoice,
            //                                                                                  "Correspondence Ref No",
            //                                                                                  miscUatpInvoice.RejectedInvoiceNumber,
            //                                                                                  fileName,
            //                                                                                  ErrorLevels.ErrorLevelInvoice,
            //                                                                                  MiscUatpErrorCodes.MuCorrespondenceInvoiceLinkingCheckForSmiX,
            //                                                                                  ErrorStatus.X,
            //                                                                                  0,
            //                                                                                  0,
            //                                                                                  islinkingError: false);
            //                  exceptionDetailsList.Add(validationExceptionDetail);
            //                  isValid = false;
            //                }
            //                else
            //                {
            //                  var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
            //                                                                                  exceptionDetailsList.Count() + 1,
            //                                                                                  fileSubmissionDate,
            //                                                                                  miscUatpInvoice,
            //                                                                                  "Correspondence Ref No",
            //                                                                                  miscUatpInvoice.RejectedInvoiceNumber,
            //                                                                                  fileName,
            //                                                                                  ErrorLevels.ErrorLevelInvoice,
            //                                                                                  MiscUatpErrorCodes.MuCorrespondenceInvoiceLinkingCheckForSmiOtherThanX,
            //                                                                                  ErrorStatus.X,
            //                                                                                  0,
            //                                                                                  0,
            //                                                                                  islinkingError: false);
            //                  exceptionDetailsList.Add(validationExceptionDetail);
            //                  isValid = false;
            //                }

            //              }
            //                // if correspondence is in closed state.
            //              if (miscCorrespondence.CorrespondenceStatus == CorrespondenceStatus.Closed)
            //              {
            //                var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Correspondence Status",
            //                                                                              Convert.ToString(miscCorrespondence.CorrespondenceStatus),
            //                                                                              fileName,
            //                                                                              ErrorLevels.ErrorLevelInvoice,
            //                                                                              MiscUatpErrorCodes.CorrRefNoClosed,
            //                                                                              ErrorStatus.X);
            //                exceptionDetailsList.Add(validationExceptionDetail);
            //                isValid = false;
            //              }
            //              else
            //              {
            //                // check if Correspondence Invoice is already created for the correspondence ref number.
            //                if (!CheckDuplicateCorrespondenceInvoice(miscUatpInvoice, false))
            //                {
            //                  // continue if Correspondence Invoice does not exists for the correspondence ref number.
            //                  if (miscUatpInvoice.IsAuthorityToBill &&  miscCorrespondence.CorrespondenceStatus != CorrespondenceStatus.Closed)
            //                  {
            //                    if (!(miscCorrespondence.CorrespondenceStatus == CorrespondenceStatus.Open &&              miscCorrespondence.AuthorityToBill))
            //                    {
            //                      var validationExceptionDetail =
            //                        CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Correspondence Number", miscCorrespondence.CorrespondenceNumber.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes. InvalidCorrespondenceStatusAuthorityToBill, ErrorStatus.X);
            //                      exceptionDetailsList.Add(validationExceptionDetail);
            //                      isValid = false;
            //                    }
            //                    else
            //                    {
            //                      if (miscCorrespondence.FromMemberId != miscUatpInvoice.BilledMemberId ||
            //                          miscCorrespondence.ToMemberId != miscUatpInvoice.BillingMemberId)
            //                      {
            //                        var validationExceptionDetail =
            //                          CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Correspondence Number", miscCorrespondence.CorrespondenceNumber.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidCorrespondenceFromTo, ErrorStatus.X);
            //                        exceptionDetailsList.Add(validationExceptionDetail);
            //                        isValid = false;
            //                      }
            //                    }
            //                  }
            //                  else
            //                  {
            //                    // if correspondence is in Expired state.
            //                    if (miscCorrespondence.CorrespondenceStatus == CorrespondenceStatus.Expired)
            //                    {
            //                      if (miscCorrespondence.ToMemberId != miscUatpInvoice.BilledMemberId ||
            //                          miscCorrespondence.FromMemberId != miscUatpInvoice.BillingMemberId)
            //                      {
            //                        var validationExceptionDetail =
            //                          CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Correspondence Number", miscCorrespondence.CorrespondenceNumber.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidCorrespondenceFromTo, ErrorStatus.X);
            //                        exceptionDetailsList.Add(validationExceptionDetail);
            //                        isValid = false;
            //                      }
            //                    }
            //                    else
            //                    {
            //                      var validationExceptionDetail =
            //                        CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Correspondence Number", miscCorrespondence.CorrespondenceNumber.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.CorrespondenceNotExpired, ErrorStatus.X);
            //                      exceptionDetailsList.Add(validationExceptionDetail);
            //                      isValid = false;
            //                    }
            //                  }

            //                  // Transaction out side time limit validation. UC-G3320-3.1 13-e
            //                  // CMP 318 - Effective From-To Date implementation.

            //                  miscCorrespondence.Invoice.ValidationDate =
            //                    new DateTime(miscCorrespondence.Invoice.SettlementYear, miscCorrespondence.Invoice.SettlementMonth, miscCorrespondence.Invoice.SettlementPeriod);

            //                  if (IsTransactionOutSideTimeLimit(miscCorrespondence.Invoice))
            //                  {
            //                    miscUatpInvoice.IsValidationFlag +=
            //                      string.IsNullOrEmpty(miscUatpInvoice.IsValidationFlag)
            //                        ? TimeLimitFlag
            //                        : ValidationFlagDelimeter + TimeLimitFlag;
            //                  }

            //                  //SCP219674 : InvalidAmountToBeSettled Validation
            //                  #region Old Code For Validatation of CorrespondenceAmounttobeSettled : To be remove
            //                 /* if (miscUatpInvoice.InvoiceSummary != null &&
            //                      (!ReferenceManager.IsSmiLikeBilateral(miscUatpInvoice.SettlementMethodId)))
            //                  {
            //                    decimal invoiceAmountToBeCompared = 0;

            //                    if (miscUatpInvoice.ListingCurrencyId == miscCorrespondence.CurrencyId)
            //                    {
            //                      invoiceAmountToBeCompared = miscUatpInvoice.InvoiceSummary.TotalAmount;
            //                      if (
            //                        !CompareUtil.Compare(miscCorrespondence.AmountToBeSettled, invoiceAmountToBeCompared,
            //                                             roundingTolerance, Constants.MiscDecimalPlaces))
            //                      {
            //                        var validationExceptionDetail =
            //                          CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Invoice TotalAmount", miscUatpInvoice.InvoiceSummary.TotalAmount.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidCorrspondenceAmountSettled, ErrorStatus.X, 0, 0, false,
            //                                                          miscCorrespondence.AmountToBeSettled.ToString());
            //                        exceptionDetailsList.Add(validationExceptionDetail);
            //                        isValid = false;
            //                      }
            //                    }
            //                    else if (miscCorrespondence.CurrencyId == miscUatpInvoice.BillingCurrencyId)
            //                    {
            //                      if (miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency != null)
            //                      {
            //                        invoiceAmountToBeCompared =
            //                          miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.Value;

            //                        if (
            //                          !CompareUtil.Compare(miscCorrespondence.AmountToBeSettled, invoiceAmountToBeCompared, roundingTolerance, Constants.MiscDecimalPlaces))
            //                        {
            //                          var validationExceptionDetail =
            //                            CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Invoice Total Amount In Clearance Currency", miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.Value.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidCorrspondenceAmountSettled, ErrorStatus.X, 0, 0, false,
            //                                                            miscCorrespondence.AmountToBeSettled.ToString());
            //                          exceptionDetailsList.Add(validationExceptionDetail);
            //                          isValid = false;
            //                        }
            //                      }
            //                    }
            //                    else
            //                    {

            //                      var originalInvoice = GetOriginalInvoice(miscUatpInvoice, miscCorrespondence);

            //                      double exchangeRate = 0;
            //                      if (originalInvoice != null && miscUatpInvoice.BillingCurrencyId != null &&
            //                          miscCorrespondence.CurrencyId != null)
            //                      {
            //                        exchangeRate =
            //                          ReferenceManager.GetExchangeRate(miscUatpInvoice.BillingCurrencyId.Value,
            //                                                           (BillingCurrency)
            //                                                           miscCorrespondence.CurrencyId.Value,
            //                                                           originalInvoice.BillingYear,
            //                                                           originalInvoice.BillingMonth);
            //                      }

            //                      var amountInClearanceCurrency = exchangeRate > 0
            //                                                        ? miscCorrespondence.AmountToBeSettled*
            //                                                          Convert.ToDecimal(exchangeRate)
            //                                                        : miscCorrespondence.AmountToBeSettled;
            //                      invoiceAmountToBeCompared = amountInClearanceCurrency;

            //                      if (miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency != null)
            //                      {
            //                        if (
            //                          !CompareUtil.Compare(
            //                            miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.Value,
            //                            invoiceAmountToBeCompared, roundingTolerance, Constants.MiscDecimalPlaces))
            //                        {
            //                          var validationExceptionDetail =
            //                            CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Total Amount In Clearance Currency", miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidCorrspondenceAmountSettled, ErrorStatus.X, 0, 0, false,
            //                                                            ConvertUtil.Round(invoiceAmountToBeCompared, 3).
            //                                                              ToString());
            //                          exceptionDetailsList.Add(validationExceptionDetail);
            //                          isValid = false;
            //                        }
            //                      }
            //                    }
            //                  } */
            //                  #endregion
            //                  #region New Code for Validatation of CorrespondenceAmounttobeSettled
            //                  if (!GetIsValidCorrAmountToBeSettled(miscUatpInvoice, miscCorrespondence))
            //                  {
            //                    var validationExceptionDetail =
            //                      CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
            //                                                      exceptionDetailsList.Count() + 1, fileSubmissionDate,
            //                                                      miscUatpInvoice, "Total Amount In Clearance Currency",
            //                                                      miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.ToString(), fileName,
            //                                                      ErrorLevels.ErrorLevelInvoice,
            //                                                      MiscUatpErrorCodes.InvalidCorrspondenceAmountSettled,
            //                                                      ErrorStatus.X, 0, 0, false);
            //                    exceptionDetailsList.Add(validationExceptionDetail);
            //                    isValid = false;
            //                  }
            //                  #endregion
            //                }
            //                  // Error if Correspondence Invoice is already created for the correspondence ref number.
            //                else
            //                {
            //                  var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Correspondence Invoice number", miscUatpInvoice.RejectedInvoiceNumber, fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.DuplicateInvoiceForCorrespondenceRefNo, ErrorStatus.X);
            //                  exceptionDetailsList.Add(validationExceptionDetail);
            //                  isValid = false;
            //                }
            //              }
            //                rejectedInvoice.ValidationDate = new DateTime(rejectedInvoice.SettlementYear, rejectedInvoice.SettlementMonth, rejectedInvoice.SettlementPeriod);
            //                // Transaction out side time limit validation. UC-G3320-3.1 13-e
            //                if (IsTransactionOutSideTimeLimit(rejectedInvoice))
            //                {
            //                    miscUatpInvoice.IsValidationFlag = string.IsNullOrEmpty(miscUatpInvoice.IsValidationFlag) ? TimeLimitFlag : "";
            //                }
            //            }
            //        }
            //    }

            //    if (miscUatpInvoice.InvoiceSummary != null)
            //    {
            //        if (miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue)
            //        {
            //            if (!ReferenceManager.IsValidNetAmount(Convert.ToDouble(miscUatpInvoice.InvoiceSummary.TotalAmount),
            //                                                 TransactionType.MiscCorrespondence,
            //                                                 miscUatpInvoice.ListingCurrencyId,
            //                                                 miscUatpInvoice, applicableMinimumField: ApplicableMinimumField.TotalAmount))
            //            {
            //                var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate,
            //                                                                                miscUatpInvoice,
            //                                                                                "Total Amount",
            //                                                                                miscUatpInvoice.InvoiceSummary.TotalAmount.ToString(),
            //                                                                                fileName,
            //                                                                                ErrorLevels.ErrorLevelInvoice,
            //                                                                                MiscUatpErrorCodes.InvalidTotalAmountOutsideLimit,
            //                                                                                ErrorStatus.X);
            //                exceptionDetailsList.Add(validationExceptionDetail);
            //                isValid = false;
            //            }
            //        }
            //    }
            //}
            #endregion

            if (miscUatpInvoice.InvoiceType == InvoiceType.CorrespondenceInvoice)
            {
              if (!ValidateCorrespondenceInvoiceDetails(miscUatpInvoice, originalInvoice, linkedRm1Invoice, linkedRm2Invoice, linkedCorrespondence, exceptionDetailsList, fileName, fileSubmissionDate))
              {
                isValid = false;
              }
            }

          if (miscUatpInvoice.InvoiceSummary != null)
            {
                decimal invoiceTaxAmountSum = 0;
                decimal invoiceVatAmountSum = 0;
                if (miscUatpInvoice.TaxBreakdown.Count > 0)
                {
                    //Vat-tax Validations
                    foreach (var tax in miscUatpInvoice.TaxBreakdown)
                    {
                        if (tax.Type.ToLower() == "vat")
                        {
                            ValidateParsedTax(tax, exceptionDetailsList, fileName, miscUatpInvoice, fileSubmissionDate, ErrorLevels.ErrorLevelInvoiceSummaryVat, roundingTolerance);
                        }
                        else
                        {
                            ValidateParsedTax(tax, exceptionDetailsList, fileName, miscUatpInvoice, fileSubmissionDate, ErrorLevels.ErrorLevelInvoiceSummaryTax, roundingTolerance);
                        }
                    }

                    invoiceTaxAmountSum = miscUatpInvoice.TaxBreakdown.Where(tax => tax.Type.ToUpper() == "TAX" && tax.CalculatedAmount.HasValue).Sum(invoiceTax => invoiceTax.CalculatedAmount.Value);
                    invoiceVatAmountSum = miscUatpInvoice.TaxBreakdown.Where(tax => tax.Type.ToUpper() == "VAT" && tax.CalculatedAmount.HasValue).Sum(invoiceTax => invoiceTax.CalculatedAmount.Value);
                }
                decimal lineItemTotalTaxAmountSum = 0;
                decimal lineItemTotalVatAmountSum = 0;

                if (miscUatpInvoice.LineItems.Count > 0)
                {
                    lineItemTotalTaxAmountSum = miscUatpInvoice.LineItems.Sum(lineItem => lineItem.TotalTaxAmount.HasValue ? lineItem.TotalTaxAmount.Value : 0);
                    lineItemTotalVatAmountSum = miscUatpInvoice.LineItems.Sum(lineItem => lineItem.TotalVatAmount.HasValue ? lineItem.TotalVatAmount.Value : 0);
                }

                // TotalTaxAmount should be provided if TaxBreakdown of invoice or lineItem level is provided
                if ((invoiceTaxAmountSum != 0 || lineItemTotalTaxAmountSum != 0) && miscUatpInvoice.InvoiceSummary.TotalTaxAmount == null)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                    exceptionDetailsList.Count() + 1,
                                                                                    fileSubmissionDate, miscUatpInvoice,
                                                                                    "Invoice Summary TotalTaxAmount",
                                                                                    miscUatpInvoice.InvoiceSummary.
                                                                                      TotalTaxAmount.ToString(), fileName,
                                                                                    ErrorLevels.ErrorLevelInvoice,
                                                                                    MiscUatpErrorCodes.InvalidTotalTaxAmount,
                                                                                    ErrorStatus.X, 0, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
                // TotalTaxAmount should be equal to sum of Calculated amount in TaxBreakdown of invoice + lineItem total tax amount
                else if (miscUatpInvoice.InvoiceSummary.TotalTaxAmount != null)
                {
                    if (!CompareUtil.Compare(miscUatpInvoice.InvoiceSummary.TotalTaxAmount.Value,
                                           (invoiceTaxAmountSum + lineItemTotalTaxAmountSum), summationTolerance,
                                           Constants.MiscDecimalPlaces))
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                        exceptionDetailsList.Count() + 1,
                                                                                        fileSubmissionDate, miscUatpInvoice,
                                                                                        "Invoice Summary TotalTaxAmount",
                                                                                        miscUatpInvoice.InvoiceSummary.
                                                                                          TotalTaxAmount.ToString(), fileName,
                                                                                        ErrorLevels.ErrorLevelInvoice,
                                                                                        MiscUatpErrorCodes.InvalidTotalTaxAmount,
                                                                                        ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                }

                // TotalVatAmount should be provided if VatBreakdown of invoice or lineItem level is provided
                if ((invoiceVatAmountSum != 0 || lineItemTotalVatAmountSum != 0) && miscUatpInvoice.InvoiceSummary.TotalVatAmount == null)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                    exceptionDetailsList.Count() + 1,
                                                                                    fileSubmissionDate, miscUatpInvoice,
                                                                                    "Invoice Summary TotalVatAmount",
                                                                                    miscUatpInvoice.InvoiceSummary.
                                                                                      TotalVatAmount.ToString(), fileName,
                                                                                    ErrorLevels.ErrorLevelInvoice,
                                                                                    MiscUatpErrorCodes.InvalidTotalVatAmount,
                                                                                    ErrorStatus.X, 0, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
                // TotalVatAmount should be equal to sum of Calculated amount in VatBreakdown of invoice + lineItem total tax amount 
                else if (miscUatpInvoice.InvoiceSummary.TotalVatAmount != null)
                {
                    if (!CompareUtil.Compare(miscUatpInvoice.InvoiceSummary.TotalVatAmount.Value,
                                           invoiceVatAmountSum + lineItemTotalVatAmountSum, summationTolerance,
                                           Constants.MiscDecimalPlaces))
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                        exceptionDetailsList.Count() + 1,
                                                                                        fileSubmissionDate, miscUatpInvoice,
                                                                                        "Invoice Summary TotalVatAmount",
                                                                                        miscUatpInvoice.InvoiceSummary.
                                                                                          TotalVatAmount.ToString(), fileName,
                                                                                        ErrorLevels.ErrorLevelInvoice,
                                                                                        MiscUatpErrorCodes.InvalidTotalVatAmount,
                                                                                        ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                }

                var totalInvoiceAmount = GetInvoiceTotalAmount(miscUatpInvoice);
                if (miscUatpInvoice.InvoiceType == InvoiceType.CreditNote)
                {
                    if (totalInvoiceAmount >= 0)
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Total Amount", totalInvoiceAmount.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvalidCreditNoteTotalAmount, ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                }
                else
                {
                    // for all other invoices check if Total amount is greater than or equal to zero.
                    // otherwise validate for min max limits as per transaction type.
                    if (totalInvoiceAmount < 0)
                    {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Total Amount", totalInvoiceAmount.ToString(), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvoiceTotalAmountNegative, ErrorStatus.X, 0, 0);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    }
                }
            }

            // Validate Linearity and Duplicates of the Line Item Number 
            // Is web Spec - 9.1.4.2-a 
            if (miscUatpInvoice.LineItems.Count > 0)
            {
                var distinctLineItemCount = miscUatpInvoice.LineItems.Select(rec => rec.LineItemNumber).Distinct().ToList().Count;

                if (distinctLineItemCount != miscUatpInvoice.LineItems.Count)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Line Item Number", string.Empty,
                                                                           fileName, ErrorLevels.ErrorLevelLineItem,
                                                                           MiscUatpErrorCodes.DuplicateLineItemNumber, ErrorStatus.X, 0, 0);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                }
            }
            
            // CMP-539: Validate the duplicates of the LineItemdetails DetailNumber in LineItem
            // Get the distinct line items 
            var distinctLineItems = miscUatpInvoice.LineItems.GroupBy(rec => rec.LineItemNumber).Distinct().ToList().Select(item => item.ElementAt(0)).ToList();
            foreach (var distinctLineItem in distinctLineItems)
            {
                var lineItemDetailComparer = new LineItemDetailsComparer();
                //Compare the distinct count with LineItemDetails count
                if (distinctLineItem.LineItemDetails.Distinct(lineItemDetailComparer).Count() !=
                    distinctLineItem.LineItemDetails.Count())
                {
                    // for each duplicate lineitem detail within lineitem , add exception
                    foreach (var equalLineITemDetail in lineItemDetailComparer.EqualLineItemDetails)
                    {
                        //Add Exception   
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                        exceptionDetailsList.Count() + 1,
                                                                                        fileSubmissionDate, miscUatpInvoice,
                                                                                        "Line Item Detail Number",
                                                                                        equalLineITemDetail.DetailNumber.ToString(),
                                                                                        fileName,
                                                                                        ErrorLevels.ErrorLevelLineItemDetail,
                                                                                        MiscUatpErrorCodes.
                                                                                            DuplicateLineItemDetailNumber,
                                                                                        ErrorStatus.X,
                                                                                        equalLineITemDetail.LineItemNumber,
                                                                                        equalLineITemDetail.DetailNumber);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                    } //For each end

                } //If end

            } // line item foreach end

            // Get whole field Charge Code mapping list 
            // Here onwards in the logic this memory copy of list will be used - No db calls 
            _fieldChargeCodeMappings = FieldChargeCodeMappingRepository.GetAll().ToList();

            _fieldMetaDatas = FieldMetaDataRepository.GetAll().ToList();

            // Get whole Field Meta Data List
            // Here onwards in the logic this memory copy of list will be used - No db calls

            foreach (var lineItem in miscUatpInvoice.LineItems)
            {
                if (isOnbehalf)
                {
                    if (onBehalfOfMemberList != null)
                    {
                        if (onBehalfOfMemberList.Count(type => type.TransmitterCode == issuingOrgMemberCode && type.ChargeCategoryId == miscUatpInvoice.ChargeCategoryId && type.ChargeCodeId == lineItem.ChargeCodeId) <= 0)
                        {
                            //SCPID : 305889 - Onbehalf of files stuck  
                            if (lineItem.ChargeCode != null)
                            {
                                var validationExceptionDetail = CreateValidationExceptionDetail(lineItem.Id.Value(),
                                                                                                exceptionDetailsList.
                                                                                                    Count() + 1,
                                                                                                fileSubmissionDate,
                                                                                                miscUatpInvoice,
                                                                                                "ChargeCategory - ChargeCode",
                                                                                                miscUatpInvoice.
                                                                                                    ChargeCategoryDisplayName +
                                                                                                "-" +
                                                                                                lineItem.ChargeCode.Name,
                                                                                                fileName,
                                                                                                ErrorLevels.
                                                                                                    ErrorLevelLineItem,
                                                                                                MiscUatpErrorCodes.
                                                                                                    InvalidChargeCateGoryAndCodeForOnBehalfFile,
                                                                                                ErrorStatus.X,
                                                                                                lineItem.LineItemNumber,
                                                                                                0);
                                exceptionDetailsList.Add(validationExceptionDetail);
                                isValid = false;
                            }
                        }
                    }
                }
                ValidateParsedLineItem(lineItem, exceptionDetailsList, fileName, miscUatpInvoice, fileSubmissionDate);
            }

            //SCP340919: Incorrect validation completed time in dashboard report
            miscUatpInvoice.ValidationDate = DateTime.UtcNow;

            ////SCP0000: PURGING AND SET EXPIRY DATE (Remove real time set expiry)
            //if (miscUatpInvoice.InvoiceType != InvoiceType.CorrespondenceInvoice)
            //{
            //  Model.Enums.TransactionType currentTransactionType;
            //  // Get Expiry period for invoice and update it for purging.
            //  miscUatpInvoice.ExpiryDatePeriod = GetExpiryDatePeriod(miscUatpInvoice, billingPeriod, out currentTransactionType);
            //}
          return isValid;
        }

        protected virtual DateTime GetExpiryDatePeriod(MiscUatpInvoice miscUatpInvoice, BillingPeriod? billingPeriod, out TransactionType currentTransactionType)
        {
            // Should be implemented in derived classes.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates the parsed line item status.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
        /// <param name="exceptionDetailsList">The exception details list.</param>
        private void UpdateParsedLineItemStatus(LineItem lineItem, InvoiceBase miscUatpInvoice, IEnumerable<IsValidationExceptionDetail> exceptionDetailsList)
        {
            // Update status of transactions - LineItem
            // UC-G-3320 Basic Flow 0.2  
            var validationErrorCountAtLineItem = exceptionDetailsList.Count(rec => rec.InvoiceNumber == miscUatpInvoice.InvoiceNumber && rec.LineItemOrBatchNo == lineItem.LineItemNumber);
            if (validationErrorCountAtLineItem == 0)
            {
                lineItem.LineItemStatus = InvoiceStatusType.ReadyForBilling;
            }
            else if (exceptionDetailsList.Count(rec => rec.InvoiceNumber == miscUatpInvoice.InvoiceNumber && rec.LineItemOrBatchNo == lineItem.LineItemNumber && rec.ErrorStatus == Enum.GetName(typeof(ErrorStatus), ErrorStatus.C)) > 0)
            {
                lineItem.LineItemStatus = InvoiceStatusType.ErrorCorrectable;
            }
            else if (exceptionDetailsList.Count(rec => rec.InvoiceNumber == miscUatpInvoice.InvoiceNumber && rec.LineItemOrBatchNo == lineItem.LineItemNumber && rec.ErrorStatus == Enum.GetName(typeof(ErrorStatus), ErrorStatus.X)) > 0)
            {
                lineItem.LineItemStatus = InvoiceStatusType.ErrorNonCorrectable;
            }
        }

        /// <summary>
        /// Updates the parsed invoice status.
        /// </summary>
        /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
        /// <param name="exceptionDetailsList">The exception details list.</param>
        /// <param name="billingMember">The billing member.</param>
        protected void UpdateParsedInvoiceStatus(MiscUatpInvoice miscUatpInvoice, IList<IsValidationExceptionDetail> exceptionDetailsList, DateTime fileSubmissionDate, string fileName)
        {

            // Update status of Invoice
            // UC-G-3320 Basic Flow 0.2
            double exchangeRate = 0;


            //fixed for issue id.5713
            miscUatpInvoice.SettlementFileStatus = InvoiceProcessStatus.NotSet;


            // MiscUatpInvoice.InvoiceStatus = InvoiceStatusType.ValidationCompleted;

            if (exceptionDetailsList.Count(rec => rec.InvoiceNumber == miscUatpInvoice.InvoiceNumber && Convert.ToInt32(rec.ErrorStatus) != (int)ErrorStatus.W) == 0)
            {

                //TotalAmountInClearanceCurrency is already updated in ValidateParsedExchangeRate method
                //if (exchangeRate != 0)
                //{
                //  miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.BillingAmount / Convert.ToDecimal(exchangeRate);
                //}

                if (miscUatpInvoice.ValidationStatus == InvoiceValidationStatus.ErrorPeriod)
                {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Billing Date", string.Format("{00}{1:00}{2:00}", miscUatpInvoice.BillingYear.ToString().Substring(2), miscUatpInvoice.BillingMonth, miscUatpInvoice.BillingPeriod), fileName, ErrorLevels.ErrorLevelInvoice, MiscUatpErrorCodes.InvoiceValidForLateSubmission, ErrorStatus.X);
                    exceptionDetailsList.Add(validationExceptionDetail);

                    // Update status of Invoice as OnHold, invoice status will be updated from SP as ReadyForBilling.
                    miscUatpInvoice.InvoiceStatusId = (int)InvoiceStatusType.ErrorNonCorrectable;
                }
                else
                {
                  if (miscUatpInvoice.ValidationStatus != InvoiceValidationStatus.FutureSubmission)
                  {
                    miscUatpInvoice.ValidationStatus = InvoiceValidationStatus.Completed;
                    miscUatpInvoice.ValidationStatusId = (int)InvoiceValidationStatus.Completed;                    

                    var billingMember = MemberManager.GetMember(miscUatpInvoice.BillingMemberId);
                    var billedMember = MemberManager.GetMember(miscUatpInvoice.BilledMemberId);


                    UpdateInvoiceDetails(miscUatpInvoice, billingMember, billedMember);
                  }
                  else if (miscUatpInvoice.ValidationStatus == InvoiceValidationStatus.FutureSubmission)
                  {
                    // Get Final Parent Details Clearing House
                    var billingFinalParent = MemberManager.GetMember(MemberManager.GetFinalParentDetails(miscUatpInvoice.BillingMemberId));
                    var billedFinalParent = MemberManager.GetMember(MemberManager.GetFinalParentDetails(miscUatpInvoice.BilledMemberId));

                    var clearingHouse = ReferenceManager.GetClearingHouseForInvoice(miscUatpInvoice, billingFinalParent, billedFinalParent);

                    // Update clearing house of invoice
                    miscUatpInvoice.ClearingHouse = clearingHouse;
                  }
                  // Update status of Invoice as OnHold, invoice status will be updated from SP as ReadyForBilling.
                  miscUatpInvoice.InvoiceStatusId = (int)InvoiceStatusType.OnHold;
                }

                //miscUatpInvoice.InvoiceStatus = InvoiceStatusType.OnHold;
            }
            else if (exceptionDetailsList.Count(rec => rec.InvoiceNumber == miscUatpInvoice.InvoiceNumber && Convert.ToInt32(rec.ErrorStatus) == (int)ErrorStatus.X) > 0)
            {
                miscUatpInvoice.InvoiceStatus = InvoiceStatusType.ErrorNonCorrectable;
                miscUatpInvoice.ValidationStatus = InvoiceValidationStatus.Failed;
                miscUatpInvoice.ValidationStatusId = (int)InvoiceValidationStatus.Failed;
                // Clear transaction data from invoice on Non-Correctable error.
                ClearInvoiceTransationData(miscUatpInvoice);
            }
            else if (exceptionDetailsList.Count(rec => rec.InvoiceNumber == miscUatpInvoice.InvoiceNumber && Convert.ToInt32(rec.ErrorStatus) == (int)ErrorStatus.C) > 0)
            {
                miscUatpInvoice.InvoiceStatus = InvoiceStatusType.ErrorCorrectable;
                miscUatpInvoice.ValidationStatus = InvoiceValidationStatus.Failed;
                miscUatpInvoice.ValidationStatusId = (int)InvoiceValidationStatus.Failed;

                var billingMember = MemberManager.GetMember(miscUatpInvoice.BillingMemberId);
                var billedMember = MemberManager.GetMember(miscUatpInvoice.BilledMemberId);

                UpdateInvoiceDetails(miscUatpInvoice, billingMember, billedMember);
            }

        }

        /// <summary>
        /// Update invoice if the invoice is ready for billing
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="billingMember"></param>
        /// <param name="billedMember"></param>
        public void UpdateInvoiceDetails(MiscUatpInvoice invoice, Member billingMember, Member billedMember)
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
                var requiredByBillingMember = false;
                var requiredByBilledMember = false;

                //if (billingMember != null && billingMember.DigitalSignApplication && (invoice.DigitalSignatureRequiredId == (int)DigitalSignatureRequired.Yes || invoice.DigitalSignatureRequiredId == (int)DigitalSignatureRequired.Default))
                // SCPID 28241 : Seperate out DSReq : Yes and Default in two different condition.
                if (billingMember != null && billingMember.DigitalSignApplication && (invoice.DigitalSignatureRequiredId == (int)DigitalSignatureRequired.Yes))
                {
                    requiredByBillingMember = true;
                } //Added  missing code to check DS Req for Country in case of billing for Default scenario
                else if (billingMember != null && billingMember.DigitalSignApplication && (invoice.DigitalSignatureRequiredId == (int)DigitalSignatureRequired.Default))
                {
                    if (IsDigitalSignatureRequiredForTheCountry(invoice, true))
                    {
                        requiredByBillingMember = true;
                    }
                }

                if (billedMember != null && billedMember.DigitalSignApplication)
                {
                    // Added missing code for to check DS Req for country check for billed point of view as well.
                    if (IsDigitalSignatureRequiredForTheCountry(invoice, false))
                    {
                        requiredByBilledMember = true;
                    }
                }


                // Update DsRequiredBy of invoice based on digital signature required by billing member and billed member.
                invoice.DsRequirdBy = GetDigitalSignatureRequiredBy(requiredByBillingMember, requiredByBilledMember);
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


        }

        /// <summary>
        /// Clears transaction data from MISC invoice.
        /// </summary>
        /// <param name="miscUatpInvoice"></param>
        private static void ClearInvoiceTransationData(MiscUatpInvoice miscUatpInvoice)
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
                miscUatpInvoice.LineItems.Clear();
                miscUatpInvoice.MemberContacts.Clear();
                miscUatpInvoice.OtherOrganizationInformations.Clear();
                miscUatpInvoice.MemberLocationInformation.Clear();
                miscUatpInvoice.TaxBreakdown.Clear();
                miscUatpInvoice.AdditionalDetails.Clear();
                miscUatpInvoice.AddOnCharges.Clear();
                miscUatpInvoice.Attachments.Clear();
                miscUatpInvoice.ValidationExceptionSummary.Clear();
            }
        }

        /// <summary>

        //SCP0000:Impact on MISC/UATP rejection linking due to purging
        /// <summary>
        /// Determines whether [is linked invoice purged] [the specified linked invoice].
        /// </summary>
        /// <param name="linkedInvoice">The linked invoice.</param>
        /// <returns>
        ///   <c>true</c> if [is linked invoice purged] [the specified linked invoice]; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsLinkedInvoicePurged(MiscUatpInvoice linkedInvoice)
        {
          bool isPurged = false;
          DateTime purgeDate = new DateTime(1973, 01, 01);
          // Linked invoice for Rejection misc. invoice stage 1, check in original misc. invoice 
          if (linkedInvoice != null && linkedInvoice.ExpiryDatePeriod != null)
          {
            if (linkedInvoice.ExpiryDatePeriod == purgeDate)
            {
              isPurged = true;
            }
          }
          return isPurged;
        }


        /// <summary>
        /// Gets the exchange rate for misc.
        /// </summary>
        /// <param name="listingCurrencyId">The listing currency id.</param>
        /// <param name="billingCurrency">The billing currency.</param>
        /// <param name="billingYear">The billing year.</param>
        /// <param name="billingMonth">The billing month.</param>
        /// <returns></returns>
        protected double GetExchangeRateForMisc(int listingCurrencyId, BillingCurrency billingCurrency, int billingYear, int billingMonth)
        {
            try
            {
                var exchangeRate = ReferenceManager.GetExchangeRate(listingCurrencyId, billingCurrency, billingYear, billingMonth);
                return exchangeRate;
            }
            catch (ISBusinessException ex)
            {
                return 0d;
            }
        }

        public long IsLocationCodePresent(string invoiceId)
        {
            var invoiceGuid = invoiceId.ToGuid();
            return LineItemRepository.GetCount(lineItem => lineItem.InvoiceId == invoiceGuid && !string.IsNullOrEmpty(lineItem.LocationCode));
        }

        public int GetMaxLineItemDetailNumber(Guid lineItemId)
        {
            return LineItemDetailRepository.GetMaxDetailNumber(lineItemId);
        }
        /// <summary>
        /// Fetch data for optional groups to populate optional field dropdown
        /// </summary>
        /// <param name="chargeCodeId"></param>
        /// <param name="chargeCodeTypeId"></param>
        /// <returns></returns>
        public List<DynamicGroupDetail> GetOptionalGroupDetails(int chargeCodeId, Nullable<int> chargeCodeTypeId)
        {
            return FieldMetaDataRepository.GetOptionalGroupMetadata(chargeCodeId, chargeCodeTypeId);
        }

        /// <summary>
        /// Gets the derived vat details for an Invoice.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <returns>List of derived vat details for the Invoice.</returns>
        public IList<MiscDerivedVatDetails> GetDerivedVatDetails(string invoiceId)
        {
            var invoiceIdGuid = invoiceId.ToGuid();
            return MiscUatpInvoiceRepository.GetDerivedVatDetails(invoiceIdGuid);
        }

        public int GetMaxLineItemNumber(Guid invoiceId)
        {
            return LineItemRepository.GetMaxLineItemNumber(invoiceId);
        }

        public MiscUatpInvoice GetOriginalInvoiceDetail(string rejectedInvoiceNumber, int billingMemberId)
        {
            // Replaced with LoadStrategy call
            var rejectedInvoice = MiscUatpInvoiceRepository.Single(invoiceNumber: rejectedInvoiceNumber, billingMemberId: billingMemberId);

            if (rejectedInvoice == null) return null;

            // DiplayText Retrieval from Table and setting it to required property - To be moved to sp
            rejectedInvoice.InvoiceStatusDisplayText = ReferenceManager.GetInvoiceStatusDisplayValue(rejectedInvoice.InvoiceStatusId);
            rejectedInvoice.SettlementMethodDisplayText = ReferenceManager.GetSettlementMethodDisplayValue(rejectedInvoice.SettlementMethodId);
            rejectedInvoice.SubmissionMethodDisplayText = ReferenceManager.GetDisplayValue(MiscGroups.FileSubmissionMethod, rejectedInvoice.SubmissionMethodId);

            if (rejectedInvoice.InvoiceType == InvoiceType.Invoice) return rejectedInvoice;

            if (rejectedInvoice.InvoiceType == InvoiceType.RejectionInvoice && rejectedInvoice.RejectionStage == 1) return GetOriginalInvoiceDetail(rejectedInvoice.RejectedInvoiceNumber, rejectedInvoice.BilledMemberId);

            return null;
        }

        /// <summary>
        /// Checks whether invoices are blocked due to some pending processes
        /// </summary>
        /// <param name="muInvoiceBases"></param>
        /// <returns></returns>
        public bool ValidateMiscUatpInvoices(IEnumerable<InvoiceBase> muInvoiceBases)
        {
            return (from muInvoiceBase in muInvoiceBases
                    where
                      muInvoiceBase.InvoiceStatus == InvoiceStatusType.ReadyForBilling || muInvoiceBase.InvoiceStatus == InvoiceStatusType.Claimed
                    select muInvoiceBase).Count() <= 0;
        }

        /// <summary>
        /// Determines whether transaction is out side time limit for specified invoice].
        /// </summary>
        /// <param name="invoice">The invoice.</param>
        /// <returns>
        /// true if transaction in not out side time limit for the specified invoice; otherwise, false.
        /// </returns>
        public bool IsTransactionOutSideTimeLimit(MiscUatpInvoice invoice)
        {
            TransactionType transactionType = 0;

            switch (invoice.RejectionStage)
            {
                case (int)RejectionStage.StageOne:
                    transactionType = invoice.BillingCategoryId == (int)BillingCategoryType.Uatp ? TransactionType.UatpRejection1 : TransactionType.MiscRejection1;
                    break;
                case (int)RejectionStage.StageTwo:
                    transactionType = invoice.BillingCategoryId == (int)BillingCategoryType.Uatp ? TransactionType.UatpRejection2 : TransactionType.MiscRejection2;
                    break;
            }
            //CMP#624 : 2.10 - Change#6 : Time Limits
            /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
            return (!ReferenceManager.IsSmiLikeBilateral(invoice.SettlementMethodId, false))
                         ? !ReferenceManager.IsTransactionInTimeLimitMethodA(transactionType, invoice.SettlementMethodId, invoice)
                         : !ReferenceManager.IsTransactionInTimeLimitMethodA2(transactionType, Convert.ToInt32(SMI.Bilateral), invoice);
        }

        /// <summary>
        /// Get Billing History Search Result
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <returns></returns>
        public List<MiscUatpInvoice> GetBillingHistoryAuditTrail(string invoiceId, int memberId = 0)
        {
            var invoiceIdTrailList = MiscInvoiceRepository.GetBillingHistoryAuditTrail(invoiceId);

            // Replaced with LoadStrategy GetSingleInvoiceTrail call
           // return invoiceIdTrailList.Select(trail => MiscInvoiceRepository.GetSingleInvoiceTrail(trail.Id)).ToList();
            // return invoiceIdTrailList.Select(trail => MiscInvoiceRepository.GetSingleInvoiceTrail(invoiceObject => invoiceObject.Id == trail.Id)).ToList();

            /*SCP160932: Correspondence No? 01800000100
              Desc: Code added to remove specific correspondences from invoice details fetched from DB, for audit trail purposes.
              Hence correspondences having sub-status as Received, Saved and ReadyForSubmit will not appear on audit trail.
              Date: 05-Aug-2013*/
            var invoiceList = invoiceIdTrailList.Select(trail => MiscInvoiceRepository.GetSingleInvoiceTrail(trail.Id)).ToList();

            foreach (var invoice in invoiceList)
            {
              if (invoice.Correspondences != null)
              {
                invoice.Correspondences.RemoveAll(
                    muCorr => muCorr.CorrespondenceSubStatus == CorrespondenceSubStatus.Received ||
                              muCorr.CorrespondenceSubStatus == CorrespondenceSubStatus.Saved ||
                              muCorr.CorrespondenceSubStatus == CorrespondenceSubStatus.ReadyForSubmit);
              }
            }

            /* In case of payable view remove all non presented invoices. */
            //if (isPayableView)
            //{
            /* SCP 250695: Correspondence Invoice raised is in Ready for Billing status and is visible to both the airline on Audit-trail. 
             * Desc: Any invoice whose status is not presented and current member (looking audit trail) is billed/payable is removed from the trail.
             */
            invoiceList.RemoveAll(inv => inv.InvoiceStatus != InvoiceStatusType.Presented && memberId == inv.BilledMemberId);

            return invoiceList;
        }

        //CMP508:Audit Trail Download with Supporting Documents
        /// <summary>
        /// Get billing history audit trail pdf
        /// </summary>
        /// <param name="invoiceId">invoice id</param>
        /// <returns>Audit Trail PDF</returns>
        public AuditTrailPdf GetBillingHistoryAuditTrailPdf(string invoiceId)
        {
            var invoiceList = GetBillingHistoryAuditTrail(invoiceId);
            var auditTrailPdf = new AuditTrailPdf();

            foreach (var miscUatpInvoice in invoiceList)
            {
                switch (miscUatpInvoice.InvoiceTypeId)
                {
                    case (int)InvoiceType.Invoice:
                        auditTrailPdf.OriginalInvoice = miscUatpInvoice;
                        break;
                    case (int)InvoiceType.CreditNote:
                        auditTrailPdf.OriginalInvoice = miscUatpInvoice;
                        break;
                    case (int)InvoiceType.RejectionInvoice:
                        auditTrailPdf.RejectionInvoiceList.Add(miscUatpInvoice);
                        break;
                    case (int)InvoiceType.CorrespondenceInvoice:
                        auditTrailPdf.CorrespondenceInvoice = miscUatpInvoice;
                        break;
                }
            }
            return auditTrailPdf;
        }

        /// <summary>
        /// Method to generate Misc Billing history Audit trail PDF
        /// </summary>
        /// <param name="auditTrail">Audit trail object on which pdf is to be generated</param>
        /// <param name="currentMemberId">Current session member</param>
        /// <param name="areaName"> Current AreaName</param>
        /// <returns>Audit trail Html string</returns>
        public string GenerateMiscBillingHistoryAuditTrailPdf(AuditTrailPdf auditTrail, int currentMemberId, string areaName)
        {
            _templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));

            //CMP508:Audit Trail Download with Supporting Documents
            VelocityContext context = GetVelocityContext(auditTrail, currentMemberId, areaName);

            // Generate Audit trail html string using .vm file and NVelocity context
            var reportContent = _templatedTextGenerator.GenerateEmbeddedTemplatedText(MiscBillingHistoryAuditTrailTemplateResourceName, context);
            // return Audit trail html string
            return reportContent;
        }

        //CMP508:Audit Trail Download with Supporting Documents
        /// <summary>
        /// Returns Html string for audit trail with supporting docs assigned with their folder numbers
        /// </summary>
        /// <param name="auditTrail">audit trail for which html is to be genereated</param>
        /// <param name="suppDocs">out parameter for Supp Docs</param>
        /// <returns>Html for audit trail</returns>
        public string GenerateMisUatpcBillingHistoryAuditTrailPackage(AuditTrailPdf auditTrail, int currentMemberId, string areaName, out Dictionary<Attachment, int> suppDocs)
        {
            _templatedTextGenerator = Ioc.Resolve<ITemplatedTextGenerator>(typeof(ITemplatedTextGenerator));

            //CMP508:Audit Trail Download with Supporting Documents
            VelocityContext context = GetVelocityContext(auditTrail, currentMemberId, areaName, true);

            // Generate Audit trail html string using .vm file and NVelocity context
            var reportContent = _templatedTextGenerator.GenerateEmbeddedTemplatedText(MiscBillingHistoryAuditTrailTemplateResourceName, context);

            //CMP508:Audit Trail Download with Supporting Documents
            suppDocs = (Dictionary<Attachment, int>)context.Get("suppDocs");
            // return Audit trail html string
            return reportContent;
        }

        //CMP508:Audit Trail Download with Supporting Documents
        /// <summary>
        /// Get velocity context for misc/uatp audit trail
        /// </summary>
        /// <param name="auditTrail">audit trail</param>
        /// <param name="downloadPackage">true if request is for package download</param>
        /// <returns>Velocity context for misc/uatp audit trail</returns>
        private VelocityContext GetVelocityContext(AuditTrailPdf auditTrail, int currentMemberId, string areaName, bool downloadPackage = false  )
        {
            var orderedLineItemsList = new List<LineItem>().AsEnumerable();
            var thirdStageRejectionInvoiceCorrespondenceList = new List<MiscCorrespondence>().AsEnumerable();
            var orderedRejectionInvoiceList = new List<MiscUatpInvoice>().AsEnumerable();

            if (auditTrail.CorrespondenceInvoice != null)
            {
                // Ordered Line Item list
                orderedLineItemsList = auditTrail.CorrespondenceInvoice.LineItems.OrderBy(lineitem => lineitem.LineItemNumber);
            }

            if (auditTrail.RejectionInvoiceList.Count > 0)
            {
                if (auditTrail.CorrespondenceInvoice != null)
                {
                    // Get Correspondence list from third stage Rejection invoice details
                    thirdStageRejectionInvoiceCorrespondenceList = auditTrail.RejectionInvoiceList.Where(inv => inv.InvoiceNumber == auditTrail.CorrespondenceInvoice.RejectedInvoiceNumber).OrderByDescending(invoice => invoice.RejectionStage).ToList()[0].Correspondences.OrderByDescending(corr => corr.CorrespondenceStage);
                }
                else
                {
                    // Get Correspondence list from third stage Rejection invoice details
                    thirdStageRejectionInvoiceCorrespondenceList = auditTrail.RejectionInvoiceList.OrderByDescending(invoice => invoice.RejectionStage).ToList()[0].Correspondences.OrderByDescending(corr => corr.CorrespondenceStage);
                }

                // Get Ordered rejection invoice list
                orderedRejectionInvoiceList = auditTrail.RejectionInvoiceList.OrderByDescending(invoice => invoice.RejectionStage).ToList();
            }

            // Instantiate VelocityContext
            var context = new VelocityContext();
            // Instantiate NonSamplingInvoiceManager
            var miscInvoiceManager = new MiscInvoiceManager();

            context.Put("areaName", areaName);
            context.Put("auditTrail", auditTrail);
            context.Put("orderedLineItemsList", orderedLineItemsList);
            context.Put("thirdStageRejectionInvoiceCorrespondenceList", thirdStageRejectionInvoiceCorrespondenceList);
            context.Put("currentMemberId", currentMemberId);
            context.Put("orderedRejectionInvoiceList", orderedRejectionInvoiceList);
            context.Put("miscInvoiceManager", miscInvoiceManager);
            //CMP508:Audit Trail Download with Supporting Documents
            context.Put("downloadPackage", downloadPackage);
            Dictionary<Attachment, int> suppDocs = new Dictionary<Attachment, int>();
            context.Put("suppDocs", suppDocs);

            return context;
        }

        /// <summary>
        /// Get Billing History Search Result
        /// </summary>
        /// <param name="invoiceCriteria"></param>
        /// <returns></returns>
        public IQueryable<MiscBillingHistorySearchResult> GetBillingHistorySearchResult(InvoiceSearchCriteria invoiceCriteria, int billingCategoryId)
        {
            // Get Final Parent Details for SMI,Clearing House validations
            var billingFinalParent = MemberManager.GetFinalParentDetails(invoiceCriteria.BillingMemberId);
            var billedFinalParent = MemberManager.GetFinalParentDetails(invoiceCriteria.BilledMemberId);
            var settlementMethodId = (int)ReferenceManager.GetDefaultSettlementMethodForMembers(billingFinalParent, billedFinalParent, billingCategoryId);

            var invoice = MiscInvoiceRepository.GetBillingHistorySearchResult(invoiceCriteria, billingCategoryId);
            foreach (var result in invoice)
            {
                result.ClearingHouse = ReferenceManager.GetClearingHouseFromSMI(settlementMethodId);
                // If Clearing house is 'B' then treat it as 'I'
                if (result.ClearingHouse.Equals("B"))
                {
                    result.ClearingHouse = "I";
                }

                //CMP #655: IS-WEB Display per Location ID
                result.DisplayMemberLocation = invoiceCriteria.BillingTypeId == 2 ? result.BillingMemberLocation : result.BilledMemberLocation;
            }
            return invoice.AsQueryable();
        }

        /// <summary>
        /// Gets the billing history correspondence search result.
        /// </summary>
        /// <param name="corrCriteria">The correspondence criteria.</param>
        /// <returns></returns>
        public IQueryable<MiscBillingHistorySearchResult> GetBillingHistoryCorrSearchResult(CorrespondenceSearchCriteria corrCriteria, int billingCategoryId)
        {
            var correspondences = MiscInvoiceRepository.GetBillingHistoryCorrSearchResult(corrCriteria, billingCategoryId);
            return correspondences.AsQueryable();
        }

        /// <summary>
        /// Gets the correspondence search result for Corr trail report.
        /// </summary>
        /// <param name="corrCriteria">The correspondence criteria.</param>
        /// <param name="billingCategoryId"></param>
        /// <returns></returns>
        public IQueryable<CorrespondenceTrailSearchResult> GetCorrespondenceTrailSearchResult(CorrespondenceTrailSearchCriteria corrCriteria, int billingCategoryId)
        {
            var correspondences = MiscInvoiceRepository.GetCorrespondenceTrailSearchResult(corrCriteria, billingCategoryId);
            return correspondences.AsQueryable();
        }

        /// <summary>
        /// Validates the correspondence time limit.
        /// </summary>
        /// <param name="invoiceId">The invoice id.</param>
        /// <param name="correspondenceStatusId">The correspondence status id.</param>
        /// <param name="authorityToBill">if set to true [authority to bill].</param>
        /// <param name="correspondenceDate">The correspondence date.</param>
        /// <returns>
        /// true if [is valid correspondence time limit] [the specified invoice id]; otherwise, false.
        /// </returns>
        public bool IsCorrespondenceOutSideTimeLimit(string invoiceId, int correspondenceStatusId, bool authorityToBill, DateTime correspondenceDate)
        {
            var isOutsideTimeLimit = false;
            var invoiceHeader = GetInvoiceDetail(invoiceId);
            var transactionType = TransactionType.MiscCorrespondence;

            if (correspondenceStatusId == (int)CorrespondenceStatus.Expired)
            {
                transactionType = TransactionType.MiscCorrInvoiceDueToExpiry;
            }
            else if (correspondenceStatusId == (int)CorrespondenceStatus.Open && authorityToBill)
            {
                transactionType = TransactionType.MiscCorrInvoiceDueToAuthorityToBill;
            }
            //CMP#624 : 2.10 - Change#6 : Time Limits
            /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH */
            isOutsideTimeLimit = (!ReferenceManager.IsSmiLikeBilateral(invoiceHeader.SettlementMethodId, false))
                                   ? !ReferenceManager.IsTransactionInTimeLimitMethodD(transactionType, invoiceHeader.SettlementMethodId, correspondenceDate)
                                   : !ReferenceManager.IsTransactionInTimeLimitMethodD1(transactionType, Convert.ToInt32(SMI.Bilateral), correspondenceDate);


            return isOutsideTimeLimit;
        }

        /// <summary>
        /// This will validate the errors on the error correction screen
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="newValue"></param>
        /// <param name="exceptionCode"></param>
        /// <returns></returns>
        public int ValidateForErrorCorrection(string newValue, string exceptionCode, Guid? entityId = null)
        {
            int result = 0;

            return result;
        }

        /// <summary>
        /// Validate location code, it should exist either invoice level or line item level. 
        /// </summary>
        /// <param name="lineItems"></param>
        /// <param name="invHeaderLocation"></param>
        /// <param name="invoiceId"></param>
        public void ValidateLocationCode(LineItem lineItem, string invHeaderLocation = null, string invoiceId = null)
        {
            MiscUatpInvoice invHeader = null;

            if (lineItem != null)
            {
                if (invoiceId != null)
                {
                    invHeader = GetInvoiceHeader(invoiceId);
                }

                invHeaderLocation = invHeader != null ? invHeader.LocationCode : invHeaderLocation;

                if (string.IsNullOrEmpty(invHeaderLocation) && string.IsNullOrEmpty(lineItem.LocationCode))
                {
                    throw new ISBusinessException(MiscUatpErrorCodes.LocationCodeRequiredAtInvoiceOrLineItem);
                }
                else if (!string.IsNullOrEmpty(invHeaderLocation) && !string.IsNullOrEmpty(lineItem.LocationCode))
                {
                    lineItem.LocationCode = string.Empty;
                    throw new ISBusinessException(MiscUatpErrorCodes.LocationCodeRequiredAtLineItem);
                }
            }
        }

        /// <summary>
        /// Issue with Charge Category and Charge Code on IS-WEB  while saving Line Item and Invoice Header in separate browser tab.
        /// 327666 - AIATSL - Query about SIS charge category/code for invoice MA68123027
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="invoiceId"></param>
        /// <param name="invHeaderChargeCategory"></param>
        public void ValidateChargeCategory(LineItem lineItem, string invoiceId, int? invHeaderChargeCategory)
        {
          if (lineItem != null )
          {
            var invHeader = invoiceId != null ? GetInvoiceHeader(invoiceId) : GetInvoiceHeader(lineItem.InvoiceId.ToString());
                      
            //Check when Header saved
            if (invHeaderChargeCategory != null && invHeaderChargeCategory != invHeader.ChargeCategoryId)
            {
              throw new ISBusinessException(MiscUatpErrorCodes.ChargeCategoryMismatchWithChargeCodeHeaderLevel);
            }
            
            // Check when Line Item saved
            if (invHeaderChargeCategory == null)
            {
              var releventChargeCode = GetChargeCode(lineItem.ChargeCodeId);
              if (releventChargeCode.ChargeCategoryId != invHeader.ChargeCategoryId)
              {
                throw new ISBusinessException(MiscUatpErrorCodes.ChargeCategoryMismatchWithChargeCodeLineItemLevel);
              }
            }
          }
        }

    /// <summary>
    /// SCP277476: Validate MISC TAX, VAT and Add on Charge Total amount against its breakdown total
    /// </summary>
    /// <param name="transactionId"></param>
    /// <param name="transactionType"></param>
    /// <param name="transactionLevelTotalTaxAmount"></param>
    /// <param name="sumofTaxTotalBrdown"></param>
    /// <param name="sumofVatTotalBrdown"></param>
    /// <param name="transactionLevelTotalVatAmount"></param>
    /// <param name="transactionLevelTotalAddOnCharge"></param>
    /// <param name="sumofAddOnTotalBrdown"></param>
    /// <returns></returns>
    public string ValidateMiscInvoiceBreakDownCaptured(Guid transactionId, int transactionType, decimal transactionLevelTotalTaxAmount, decimal sumofTaxTotalBrdown, decimal sumofVatTotalBrdown, decimal transactionLevelTotalVatAmount,
                                                             decimal transactionLevelTotalAddOnCharge, decimal sumofAddOnTotalBrdown)
    {

      return MiscUatpInvoiceRepository.ValidateMiscInvoiceBreakDownCaptured(transactionId, transactionType,
                                                                            ConvertUtil.Round(transactionLevelTotalTaxAmount, Constants.MiscDecimalPlaces),
                                                                            ConvertUtil.Round(sumofTaxTotalBrdown, Constants.MiscDecimalPlaces),
                                                                            ConvertUtil.Round(transactionLevelTotalVatAmount, Constants.MiscDecimalPlaces),
                                                                            ConvertUtil.Round(sumofVatTotalBrdown, Constants.MiscDecimalPlaces),
                                                                            ConvertUtil.Round(transactionLevelTotalAddOnCharge, Constants.MiscDecimalPlaces),
                                                                            ConvertUtil.Round(sumofAddOnTotalBrdown, Constants.MiscDecimalPlaces));
    }

    /// <summary>
    /// CMP288
    /// get invoice type by invoice descriptions
    /// </summary>
    /// <param name="invDesc">invoice description</param>
    /// <returns>invoice type</returns>
    public string LookupTemplateType(string invDesc)
    {
      return MiscUatpInvoiceRepository.LookupTemplateType(invDesc);
    }

    //CMP288: Invoice members location add for invoice preview.
    /// <summary>
    /// This method use to add memberlocation information for preview purpose only.
    /// </summary>
    /// <param name="invoice"></param>
    public void UpdateInvoiceMemberLocationInfo(InvoiceBase invoice)
    {
      // Adds member location information.
      UpdateMemberLocationInformation(invoice);
    }

    //CMP626 :  Separate method to check for future submission also.
    private bool ValidateBillingPeriodForMisc(MiscUatpInvoice invoice)
    {
      var clearingHouse = ReferenceManager.GetClearingHouseToFetchCurrentBillingPeriod(invoice.SettlementMethodId);
      //SCP321997: Sun Splash Aviation SH-361
      var currentBillingPeriod = CalendarManager.GetCurrentPeriodIfOpenOrPreviousAsCurrent(clearingHouse);

      if (invoice.InvoiceBillingPeriod > currentBillingPeriod)
      {
        // Check if future period is allowed for Misc.
        if (invoice.BillingCategory == BillingCategoryType.Misc)
        {
          var miscellaneousConfiguration = MemberManager.GetMiscellaneousConfiguration(invoice.BillingMemberId);
          // It can be equal to a future period if the IS calendar’s “Submissions Open (Future dated submissions)” date of that future period is equal to or less than the system date.
          if (miscellaneousConfiguration != null && miscellaneousConfiguration.IsFutureBillingSubmissionsAllowed)
          {
              //SCP#442250: MISC IS-WEB invoice status :validated future submission"
              if (invoice.InvoiceStatusId == (int)InvoiceStatusType.ReadyForSubmission)
              {
                  invoice.IsFutureSubmission = true;
              }
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
    /// Validates the rejection invoice details.
    /// </summary>
    /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
    /// <param name="originalInvoice">The original invoice.</param>
    /// <param name="linkedRm1Invoice">The linked RM1 invoice.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="fileSubmissionDate">The file submission date.</param>
    /// <returns></returns>
    public bool ValidateRejectionInvoiceDetails(MiscUatpInvoice miscUatpInvoice,
                                                      MiscUatpInvoice originalInvoice,
                                                      MiscUatpInvoice linkedRm1Invoice,
                                                      IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate)
    {
      bool isValid = true;
      //CMP#624 : Code review changes => Rejection code validation moved in "ValidateLinkingDetails" method 
      #region InvoiceType.RejectionInvoice

      //Rejection Invoice

      //SCP126263:SRM 4.5 - Admin Alert - Parsing and Validation failure 055
      /* CMP #624: ICH Rewrite-New SMI X, Here SMI X is expected to behave like ICH or Bilateral */
      if ((miscUatpInvoice.SettlementMethodId == (int) SMI.Ich || ReferenceManager.IsSmiLikeBilateral(miscUatpInvoice.SettlementMethodId, true)) && miscUatpInvoice.RejectionStage > 1)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                        exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        miscUatpInvoice,
                                                                        "Rejection Stage",
                                                                        miscUatpInvoice.RejectionStage.ToString(),
                                                                        fileName,
                                                                        ErrorLevels.ErrorLevelInvoice,
                                                                        MiscUatpErrorCodes.InvalidRejectionStage,
                                                                        ErrorStatus.X,
                                                                        0,
                                                                        0);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      //SCP126263:SRM 4.5 - Admin Alert - Parsing and Validation failure 055
      if ((miscUatpInvoice.SettlementMethodId == (int) SMI.Ach || miscUatpInvoice.SettlementMethodId == (int) SMI.AchUsingIataRules ||
           miscUatpInvoice.SettlementMethodId == (int) SMI.AdjustmentDueToProtest))
      {
        //if ((miscUatpInvoice.RejectionStage != 1) && (miscUatpInvoice.RejectionStage != 2))
        if (miscUatpInvoice.RejectionStage > 2)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                          exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          miscUatpInvoice,
                                                                          "Rejection Stage",
                                                                          miscUatpInvoice.RejectionStage.ToString(),
                                                                          fileName,
                                                                          ErrorLevels.ErrorLevelInvoice,
                                                                          MiscUatpErrorCodes.InvalidRejectionStage,
                                                                          ErrorStatus.X,
                                                                          0,
                                                                          0);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
      }

      //SCP126263:SRM 4.5 - Admin Alert - Parsing and Validation failure 055
      if (string.IsNullOrWhiteSpace(miscUatpInvoice.RejectedInvoiceNumber))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                        exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        miscUatpInvoice,
                                                                        "Rejection Invoice Number",
                                                                        string.Empty,
                                                                        fileName,
                                                                        ErrorLevels.ErrorLevelInvoice,
                                                                        MiscUatpErrorCodes.RejectionInvoiceNumberMandatory,
                                                                        ErrorStatus.X,
                                                                        0,
                                                                        0);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      if (miscUatpInvoice.RejectionStage == 0)
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                        exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        miscUatpInvoice,
                                                                        "Rejection Stage",
                                                                        miscUatpInvoice.RejectionStage.ToString(),
                                                                        fileName,
                                                                        ErrorLevels.ErrorLevelInvoice,
                                                                        MiscUatpErrorCodes.InvalidRejectionDetailStage,
                                                                        ErrorStatus.X,
                                                                        0,
                                                                        0);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      MiscUatpInvoice linkedInvoice = null; //GetMUInvoicePriviousTransanction(miscUatpInvoice);
      if (miscUatpInvoice.RejectionStage == 2)
      {
        linkedInvoice = linkedRm1Invoice;
      }
      else if (miscUatpInvoice.RejectionStage == 1)
      {
        linkedInvoice = originalInvoice;
      }

      bool isMemberMigrated = false;

      if (!SystemParameters.Instance.General.IgnoreValidationOnMigrationPeriod)
      {
          // Linking process validation if Rejection Flag is "Y".
          // Billed member has migrated but could not find the data of the rejected invoice, throw an error.
          if (IsMemberMigrated(miscUatpInvoice))
          {
              isMemberMigrated = true;
          }
      }

      if (linkedInvoice == null)
      {
          if (isMemberMigrated)
          {
              var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                              exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              miscUatpInvoice,
                                                                              "Rejection Invoice Number",
                                                                              miscUatpInvoice.RejectedInvoiceNumber,
                                                                              fileName,
                                                                              ErrorLevels.ErrorLevelInvoice,
                                                                              MiscUatpErrorCodes.RejectionInvoiceNumberNotExist,
                                                                              ErrorStatus.C,
                                                                              0,
                                                                              0,
                                                                              islinkingError: true);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
          }
          else
          {

              var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                              exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              miscUatpInvoice,
                                                                              "Rejection Invoice Number",
                                                                              miscUatpInvoice.RejectedInvoiceNumber,
                                                                              fileName,
                                                                              ErrorLevels.ErrorLevelInvoice,
                                                                              MiscUatpErrorCodes.RejectionInvoiceNumberNotExist,
                                                                              ErrorStatus.W,
                                                                              0,
                                                                              0);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;

              #region CMP#678: Time Limit Validation on Last Stage MISC Rejections

              if (miscUatpInvoice.BillingCategoryId == (int)BillingCategoryType.Misc)
              {
                  var yourInvDetail = new MiscUatpInvoice();
                  yourInvDetail.BillingYear = miscUatpInvoice.SettlementYear;
                  yourInvDetail.BillingMonth = miscUatpInvoice.SettlementMonth;
                  yourInvDetail.BillingPeriod = miscUatpInvoice.SettlementPeriod;
                  /*Input File Validation*/
                  var rmInTimeLimtMsg = ValidateMiscLastStageRmForTimeLimit(yourInvDetail,
                                                                            miscUatpInvoice,
                                                                            RmValidationType.InputFile,
                                                                            fileName: fileName,
                                                                            fileSubmissionDate: fileSubmissionDate,
                                                                            exceptionDetailsList: exceptionDetailsList);
                  if (!string.IsNullOrWhiteSpace(rmInTimeLimtMsg)) isValid = false;
              }

              #endregion
          }
      }
      else
      {
          //SCP0000:Impact on MISC/UATP rejection linking due to purging
          if (IsLinkedInvoicePurged(linkedInvoice))
          {
              var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                              exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              miscUatpInvoice,
                                                                              "Rejection Invoice Number",
                                                                              miscUatpInvoice.RejectedInvoiceNumber,
                                                                              fileName,
                                                                              ErrorLevels.ErrorLevelInvoice,
                                                                              MiscUatpErrorCodes.RejectionInvoiceNumberNotExist,
                                                                              ErrorStatus.C,
                                                                              0,
                                                                              0,
                                                                              islinkingError: true);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
          }
          //CMP#624 : New Validation #8:SMI Match Check for MISC/UATP Rejection Invoices
          else if (miscUatpInvoice.SettlementMethodId != (int)SMI.IchSpecialAgreement && !ValidateSmiAfterLinking(miscUatpInvoice.SettlementMethodId, linkedInvoice.SettlementMethodId))
          {
              //if (miscUatpInvoice.SettlementMethodId == (int) SMI.IchSpecialAgreement)
              //{
              //  var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
              //                                                                  exceptionDetailsList.Count() + 1,
              //                                                                  fileSubmissionDate,
              //                                                                  miscUatpInvoice,
              //                                                                  "Rejection Invoice Number",
              //                                                                  miscUatpInvoice.RejectedInvoiceNumber,
              //                                                                  fileName,
              //                                                                  ErrorLevels.ErrorLevelInvoice,
              //                                                                  MiscUatpErrorCodes.MuRejctionInvoiceSmiCheckForSmiX,
              //                                                                  ErrorStatus.X,
              //                                                                  0,
              //                                                                  0,
              //                                                                  islinkingError: false);
              //  exceptionDetailsList.Add(validationExceptionDetail);
              //  isValid = false;
              //}
              //else
              //{
              var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                              exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              miscUatpInvoice,
                                                                              "Rejection Invoice Number",
                                                                              miscUatpInvoice.RejectedInvoiceNumber,
                                                                              fileName,
                                                                              ErrorLevels.ErrorLevelInvoice,
                                                                              MiscUatpErrorCodes.MuRejctionInvoiceSmiCheckForSmiOtherThanX,
                                                                              ErrorStatus.X,
                                                                              0,
                                                                              0,
                                                                              islinkingError: false);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
              //}

          }


          #region CMP#678: Time Limit Validation on Last Stage MISC Rejections
          if (miscUatpInvoice.BillingCategoryId == (int)BillingCategoryType.Misc)
          {
              /*Input File Validation*/
              var rmInTimeLimtMsg = ValidateMiscLastStageRmForTimeLimit(linkedInvoice,
                                                                        miscUatpInvoice,
                                                                        RmValidationType.InputFile,
                                                                        fileName: fileName,
                                                                        fileSubmissionDate: fileSubmissionDate,
                                                                        exceptionDetailsList: exceptionDetailsList);
              if (!string.IsNullOrWhiteSpace(rmInTimeLimtMsg)) isValid = false;
          }

          #endregion

          if (miscUatpInvoice.InvoiceType == InvoiceType.RejectionInvoice && linkedInvoice.InvoiceTypeId == (int)InvoiceType.CorrespondenceInvoice)
          {
              var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                              exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              miscUatpInvoice,
                                                                              "Rejection Invoice number",
                                                                              miscUatpInvoice.RejectedInvoiceNumber,
                                                                              fileName,
                                                                              ErrorLevels.ErrorLevelInvoice,
                                                                              MiscUatpErrorCodes.CorrespondenceInvoiceCannotBeRejected,
                                                                              ErrorStatus.X);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
          }

          if (miscUatpInvoice.RejectionStage == 1 && linkedInvoice.InvoiceTypeId == (int)InvoiceType.RejectionInvoice)
          {
              var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                              exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              miscUatpInvoice,
                                                                              "Rejection Invoice number",
                                                                              miscUatpInvoice.RejectedInvoiceNumber,
                                                                              fileName,
                                                                              ErrorLevels.ErrorLevelInvoice,
                                                                              MiscUatpErrorCodes.InvalidRejectionStageOfCurrentRejection,
                                                                              ErrorStatus.X);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
          }

          if (miscUatpInvoice.RejectionStage == 2)
          {
              if (linkedInvoice.InvoiceTypeId != (int)InvoiceType.RejectionInvoice || linkedInvoice.RejectionStage != 1)
              {
                  var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                  exceptionDetailsList.Count() + 1,
                                                                                  fileSubmissionDate,
                                                                                  miscUatpInvoice,
                                                                                  "Rejection Invoice number",
                                                                                  miscUatpInvoice.RejectedInvoiceNumber,
                                                                                  fileName,
                                                                                  ErrorLevels.ErrorLevelInvoice,
                                                                                  MiscUatpErrorCodes.InvoiceIsNotRejectedInvoice,
                                                                                  ErrorStatus.X);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
              }
          }
          // Check for invoices with other status.
          //SCPID : 117317 - question about same invoice No, check settlement year as well.
          //SCP251726: Two reject invoices for same original invoice number
          var invoiceCount =
            MiscUatpInvoiceRepository.GetCount(
              invoice =>
              invoice.RejectedInvoiceNumber.ToUpper() == miscUatpInvoice.RejectedInvoiceNumber.ToUpper() && invoice.BillingMemberId == miscUatpInvoice.BillingMemberId &&
              invoice.BilledMemberId == miscUatpInvoice.BilledMemberId && invoice.BillingCategoryId == miscUatpInvoice.BillingCategoryId && invoice.SettlementYear == miscUatpInvoice.SettlementYear &&
              !(invoice.InvoiceStatusId == (int)InvoiceStatusType.ErrorNonCorrectable && invoice.ValidationStatusId == (int)InvoiceValidationStatus.Failed));
          if (invoiceCount > 0)
          {
              var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                              exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              miscUatpInvoice,
                                                                              "Rejection Invoice number",
                                                                              miscUatpInvoice.RejectedInvoiceNumber,
                                                                              fileName,
                                                                              ErrorLevels.ErrorLevelInvoice,
                                                                              MiscUatpErrorCodes.InvoiceAlreadyRejected,
                                                                              ErrorStatus.X);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
          }
      }

      if (miscUatpInvoice.InvoiceSummary != null)
      {
        var transactionType = miscUatpInvoice.RejectionStage == 1 ? TransactionType.MiscRejection1 : TransactionType.MiscRejection2;
        MaxAcceptableAmount maxAcceptableAmount;
        MinAcceptableAmount minAcceptableAmount;

        var clearingHouse = ReferenceManager.GetClearingHouseFromSMI(miscUatpInvoice.SettlementMethodId);

        maxAcceptableAmount = GetMaxAcceptableAmount(miscUatpInvoice, clearingHouse, transactionType);

        if (maxAcceptableAmount != null &&
            !ReferenceManager.IsValidNetAmount(Convert.ToDouble(miscUatpInvoice.InvoiceSummary.TotalAmount),
                                               transactionType,
                                               miscUatpInvoice.ListingCurrencyId,
                                               miscUatpInvoice,
                                               iMaxAcceptableAmount: maxAcceptableAmount,
                                               applicableMinimumField: ApplicableMinimumField.TotalAmount))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                          exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          miscUatpInvoice,
                                                                          "Total Amount",
                                                                          miscUatpInvoice.InvoiceSummary.TotalAmount.ToString(),
                                                                          fileName,
                                                                          ErrorLevels.ErrorLevelInvoice,
                                                                          MiscUatpErrorCodes.InvalidTotalAmountOutsideLimit,
                                                                          ErrorStatus.X,
                                                                          0,
                                                                          0);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

        minAcceptableAmount = GetMinAcceptableAmounts(miscUatpInvoice, clearingHouse, transactionType).FirstOrDefault();

        if (minAcceptableAmount != null &&
            !ReferenceManager.IsValidNetAmount(Convert.ToDouble(miscUatpInvoice.InvoiceSummary.TotalAmount),
                                               transactionType,
                                               miscUatpInvoice.ListingCurrencyId,
                                               miscUatpInvoice,
                                               iMinAcceptableAmount: minAcceptableAmount,
                                               applicableMinimumField: ApplicableMinimumField.TotalAmount))
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                          exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          miscUatpInvoice,
                                                                          "Total Amount",
                                                                          miscUatpInvoice.InvoiceSummary.TotalAmount.ToString(),
                                                                          fileName,
                                                                          ErrorLevels.ErrorLevelInvoice,
                                                                          MiscUatpErrorCodes.InvalidTotalAmountOutsideLimit,
                                                                          ErrorStatus.X,
                                                                          0,
                                                                          0);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }

      }
      //SCP126263: SRM 4.5 - Admin Alert - Parsing and Validation failure 055
      // Validate settlement period; must be smaller than current period

      if (miscUatpInvoice.SettlementPeriod > 0 && miscUatpInvoice.SettlementMonth > 0 && miscUatpInvoice.SettlementYear > 0)
      {
        DateTime settlementDate;

        var cultureInfo = new CultureInfo("en-US");
        cultureInfo.Calendar.TwoDigitYearMax = 2099;
        const string billingDateFormat = "yyyyMMdd";

        DateTime.TryParseExact(string.Format("{0}{1}{2}", miscUatpInvoice.SettlementYear, miscUatpInvoice.SettlementMonth.ToString("00"), miscUatpInvoice.SettlementPeriod.ToString("00")),
                               billingDateFormat,
                               cultureInfo,
                               DateTimeStyles.None,
                               out settlementDate);

        int settlementMethodId = linkedInvoice != null ? linkedInvoice.SettlementMethodId : miscUatpInvoice.SettlementMethodId;

        var currentBillingP = CalendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(ReferenceManager.GetClearingHouseToFetchCurrentBillingPeriod(settlementMethodId));

        var settlementMonthPeriod = new BillingPeriod { Period = settlementDate.Day, Month = settlementDate.Month, Year = settlementDate.Year };

        miscUatpInvoice.ValidationDate = settlementDate;

        if (settlementMonthPeriod > currentBillingP)
        {
          var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                          exceptionDetailsList.Count() + 1,
                                                                          fileSubmissionDate,
                                                                          miscUatpInvoice,
                                                                          "Rejection Invoice Settlement Period",
                                                                          string.Format("{0}-{1}-{2}",
                                                                                        miscUatpInvoice.SettlementYear.ToString().PadLeft(2, '0'),
                                                                                        miscUatpInvoice.SettlementMonth.ToString().PadLeft(2, '0'),
                                                                                        miscUatpInvoice.SettlementPeriod.ToString().PadLeft(2, '0')),
                                                                          fileName,
                                                                          ErrorLevels.ErrorLevelInvoice,
                                                                          MiscUatpErrorCodes.InvalidSettlementMonthPeriod,
                                                                          ErrorStatus.X,
                                                                          0,
                                                                          0);
          exceptionDetailsList.Add(validationExceptionDetail);
          isValid = false;
        }
        if (DateTime.TryParseExact(string.Format("{0}{1}{2}", miscUatpInvoice.SettlementYear, miscUatpInvoice.SettlementMonth.ToString("00"), miscUatpInvoice.SettlementPeriod.ToString("00")),
                                   billingDateFormat,
                                   cultureInfo,
                                   DateTimeStyles.None,
                                   out settlementDate))
        {
          // Transaction out side time limit validation. UC-G3320-3.2
          if (IsTransactionOutSideTimeLimit(miscUatpInvoice))
          {
            miscUatpInvoice.IsValidationFlag += string.IsNullOrEmpty(miscUatpInvoice.IsValidationFlag) ? TimeLimitFlag : ValidationFlagDelimeter + TimeLimitFlag;
          }
        }
      }
      else
      {
        //SCP126263:SRM 4.5 - Admin Alert - Parsing and Validation failure 055
        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                        exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        miscUatpInvoice,
                                                                        "Rejection Invoice Settlement Period",
                                                                        string.Format("{0}-{1}-{2}",
                                                                                      miscUatpInvoice.SettlementYear.ToString().PadLeft(2, '0'),
                                                                                      miscUatpInvoice.SettlementMonth.ToString().PadLeft(2, '0'),
                                                                                      miscUatpInvoice.SettlementPeriod.ToString().PadLeft(2, '0')),
                                                                        fileName,
                                                                        ErrorLevels.ErrorLevelInvoice,
                                                                        MiscUatpErrorCodes.RejectionSettlementPeriodMandatory,
                                                                        ErrorStatus.X,
                                                                        0,
                                                                        0);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }

      // Invoice number and rejected invoice number can not be same. 
      if (miscUatpInvoice.InvoiceNumber.ToLower().Equals(miscUatpInvoice.RejectedInvoiceNumber))
      {
        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                        exceptionDetailsList.Count() + 1,
                                                                        fileSubmissionDate,
                                                                        miscUatpInvoice,
                                                                        "Rejection Invoice Number",
                                                                        miscUatpInvoice.RejectedInvoiceNumber,
                                                                        fileName,
                                                                        ErrorLevels.ErrorLevelInvoice,
                                                                        MiscUatpErrorCodes.InvalidRejectionInvoiceNumber,
                                                                        ErrorStatus.X,
                                                                        0,
                                                                        0);
        exceptionDetailsList.Add(validationExceptionDetail);
        isValid = false;
      }




      #endregion

      return isValid;
    }

      /// <summary>
    /// Validates the correspondence invoice details.
    /// </summary>
    /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
    /// <param name="originalInvoice">The original invoice.</param>
    /// <param name="linkedRm1Invoice">The linked RM1 invoice.</param>
    /// <param name="linkedRm2Invoice">The linked RM2 invoice.</param>
    /// <param name="linkedCorrespondence">The linked correspondence.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="fileSubmissionDate">The file submission date.</param>
    /// <returns></returns>
    public bool ValidateCorrespondenceInvoiceDetails(MiscUatpInvoice miscUatpInvoice,
                                                    MiscUatpInvoice originalInvoice,
                                                    MiscUatpInvoice linkedRm1Invoice,
                                                    MiscUatpInvoice linkedRm2Invoice,
                                                    MiscCorrespondence linkedCorrespondence,
                                                    IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate)
      {
        bool isValid = true;
        //CMP#624 : Code review changes => Rejection code validation moved in "ValidateLinkingDetails" method 

        #region InvoiceType.CorrespondenceInvoice


        //Correspondence invoice

        if (!string.IsNullOrEmpty(miscUatpInvoice.CorrespondenceRefNo.ToString()))
        {
          // fetch correspondence to validate Correspondence Invoice. 
          //SCP199693:create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202] 

          var miscCorrespondence = linkedCorrespondence;
          //MiscCorrespondenceRepository.GetCorrespondenceWithInvoice(
          //  miscCorrs =>
          //  miscCorrs.CorrespondenceNumber == miscUatpInvoice.CorrespondenceRefNo &&
          //  (((miscCorrs.CorrespondenceStatusId == (int)CorrespondenceStatus.Open || miscCorrs.CorrespondenceStatusId == (int)CorrespondenceStatus.Expired) 
          //  /*&& miscCorrs.CorrespondenceSubStatusId == (int)CorrespondenceSubStatus.Responded*/) ||
          //   miscCorrs.CorrespondenceStatusId == (int)CorrespondenceStatus.Closed)).OrderByDescending(miscCorr2 => miscCorr2.CorrespondenceStage).FirstOrDefault();

          if (miscCorrespondence == null)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate,
                                                                            miscUatpInvoice,
                                                                            "Correspondence Ref No",
                                                                            miscUatpInvoice.CorrespondenceRefNo.ToString(),
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelInvoice,
                                                                            MiscUatpErrorCodes.InvoiceNotExistForCorrespondence,
                                                                            ErrorStatus.X,
                                                                            0,
                                                                            0,
                                                                            islinkingError: true);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          else
          {

            var rejectedInvoice = miscCorrespondence.Invoice;

            if (rejectedInvoice == null)
            { //SCP337178: Correspondence Invoice - AVIANCA (AV-134)
              var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                              exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              miscUatpInvoice,
                                                                              "Correspondence Ref No",
                                                                              miscUatpInvoice.CorrespondenceRefNo.ToString(),
                                                                              fileName,
                                                                              ErrorLevels.ErrorLevelInvoice,
                                                                              MiscUatpErrorCodes.InvoiceNotExistForCorrespondence,
                                                                              ErrorStatus.C,
                                                                              0,
                                                                              0,
                                                                              islinkingError: true);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
            else if (rejectedInvoice.InvoiceNumber.ToUpper() != miscUatpInvoice.RejectedInvoiceNumber.ToUpper())
            { //SCP337178: Correspondence Invoice - AVIANCA (AV-134)
              var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                              exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              miscUatpInvoice,
                                                                              "Correspondence Ref No",
                                                                              miscUatpInvoice.CorrespondenceRefNo.ToString(),
                                                                              fileName,
                                                                              ErrorLevels.ErrorLevelInvoice,
                                                                              MiscUatpErrorCodes.InvoiceNotExistForCorrespondence,
                                                                              ErrorStatus.C,
                                                                              0,
                                                                              0,
                                                                              islinkingError: true);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
          //SCP348090: Correspondence Invoice Authority to Bill
           // else
          //  {
              //CMP#624 :  New Validation #9:SMI Match Check for MISC/UATP Correspondence Invoices 
              if (miscUatpInvoice.SettlementMethodId != (int) SMI.IchSpecialAgreement && !ValidateSmiAfterLinking(miscUatpInvoice.SettlementMethodId, rejectedInvoice.SettlementMethodId))
              {
                //if (miscUatpInvoice.SettlementMethodId == (int) SMI.IchSpecialAgreement)
                //{
                //  var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                //                                                                  exceptionDetailsList.Count() + 1,
                //                                                                  fileSubmissionDate,
                //                                                                  miscUatpInvoice,
                //                                                                  "Correspondence Ref No",
                //                                                                  miscUatpInvoice.RejectedInvoiceNumber,
                //                                                                  fileName,
                //                                                                  ErrorLevels.ErrorLevelInvoice,
                //                                                                  MiscUatpErrorCodes.MuCorrespondenceInvoiceLinkingCheckForSmiX,
                //                                                                  ErrorStatus.X,
                //                                                                  0,
                //                                                                  0,
                //                                                                  islinkingError: false);
                //  exceptionDetailsList.Add(validationExceptionDetail);
                //  isValid = false;
                //}
                //else
                //{
                  var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                  exceptionDetailsList.Count() + 1,
                                                                                  fileSubmissionDate,
                                                                                  miscUatpInvoice,
                                                                                  "Correspondence Ref No",
                                                                                  miscUatpInvoice.RejectedInvoiceNumber,
                                                                                  fileName,
                                                                                  ErrorLevels.ErrorLevelInvoice,
                                                                                  MiscUatpErrorCodes.MuCorrespondenceInvoiceLinkingCheckForSmiOtherThanX,
                                                                                  ErrorStatus.X,
                                                                                  0,
                                                                                  0,
                                                                                  islinkingError: false);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
                //}

              }
              // if correspondence is in closed state.
              if (miscCorrespondence.CorrespondenceStatus == CorrespondenceStatus.Closed)
              {
                var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                exceptionDetailsList.Count() + 1,
                                                                                fileSubmissionDate,
                                                                                miscUatpInvoice,
                                                                                "Correspondence Status",
                                                                                Convert.ToString(miscCorrespondence.CorrespondenceStatus),
                                                                                fileName,
                                                                                ErrorLevels.ErrorLevelInvoice,
                                                                                MiscUatpErrorCodes.CorrRefNoClosed,
                                                                                ErrorStatus.X);
                exceptionDetailsList.Add(validationExceptionDetail);
                isValid = false;
              }
              else
              {
                // check if Correspondence Invoice is already created for the correspondence ref number.
                if (!CheckDuplicateCorrespondenceInvoice(miscUatpInvoice, false))
                {
                  // continue if Correspondence Invoice does not exists for the correspondence ref number.
                  if (miscUatpInvoice.IsAuthorityToBill && miscCorrespondence.CorrespondenceStatus != CorrespondenceStatus.Closed)
                  {
                    if (!(miscCorrespondence.CorrespondenceStatus == CorrespondenceStatus.Open && miscCorrespondence.AuthorityToBill))
                    {
                      var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                      exceptionDetailsList.Count() + 1,
                                                                                      fileSubmissionDate,
                                                                                      miscUatpInvoice,
                                                                                      "Correspondence Number",
                                                                                      miscCorrespondence.CorrespondenceNumber.ToString(),
                                                                                      fileName,
                                                                                      ErrorLevels.ErrorLevelInvoice,
                                                                                      MiscUatpErrorCodes.InvalidCorrespondenceStatusAuthorityToBill,
                                                                                      ErrorStatus.X);
                      exceptionDetailsList.Add(validationExceptionDetail);
                      isValid = false;
                    }
                    else
                    {
                      if (miscCorrespondence.FromMemberId != miscUatpInvoice.BilledMemberId || miscCorrespondence.ToMemberId != miscUatpInvoice.BillingMemberId)
                      {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                        exceptionDetailsList.Count() + 1,
                                                                                        fileSubmissionDate,
                                                                                        miscUatpInvoice,
                                                                                        "Correspondence Number",
                                                                                        miscCorrespondence.CorrespondenceNumber.ToString(),
                                                                                        fileName,
                                                                                        ErrorLevels.ErrorLevelInvoice,
                                                                                        MiscUatpErrorCodes.InvalidCorrespondenceFromTo,
                                                                                        ErrorStatus.X);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                      }
                    }
                  }
                  else
                  {
                    // if correspondence is in Expired state.
                    if (miscCorrespondence.CorrespondenceStatus == CorrespondenceStatus.Expired)
                    {
                      if (miscCorrespondence.ToMemberId != miscUatpInvoice.BilledMemberId || miscCorrespondence.FromMemberId != miscUatpInvoice.BillingMemberId)
                      {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                        exceptionDetailsList.Count() + 1,
                                                                                        fileSubmissionDate,
                                                                                        miscUatpInvoice,
                                                                                        "Correspondence Number",
                                                                                        miscCorrespondence.CorrespondenceNumber.ToString(),
                                                                                        fileName,
                                                                                        ErrorLevels.ErrorLevelInvoice,
                                                                                        MiscUatpErrorCodes.InvalidCorrespondenceFromTo,
                                                                                        ErrorStatus.X);
                        exceptionDetailsList.Add(validationExceptionDetail);
                        isValid = false;
                      }
                    }
                    else
                    {
                      var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                      exceptionDetailsList.Count() + 1,
                                                                                      fileSubmissionDate,
                                                                                      miscUatpInvoice,
                                                                                      "Correspondence Number",
                                                                                      miscCorrespondence.CorrespondenceNumber.ToString(),
                                                                                      fileName,
                                                                                      ErrorLevels.ErrorLevelInvoice,
                                                                                      MiscUatpErrorCodes.CorrespondenceNotExpired,
                                                                                      ErrorStatus.X);
                      exceptionDetailsList.Add(validationExceptionDetail);
                      isValid = false;
                    }
                  }

                  // Transaction out side time limit validation. UC-G3320-3.1 13-e
                  // CMP 318 - Effective From-To Date implementation.

                  miscCorrespondence.Invoice.ValidationDate = new DateTime(miscCorrespondence.Invoice.SettlementYear,
                                                                           miscCorrespondence.Invoice.SettlementMonth,
                                                                           miscCorrespondence.Invoice.SettlementPeriod);

                  if (IsTransactionOutSideTimeLimit(miscCorrespondence.Invoice))
                  {
                    miscUatpInvoice.IsValidationFlag += string.IsNullOrEmpty(miscUatpInvoice.IsValidationFlag) ? TimeLimitFlag : ValidationFlagDelimeter + TimeLimitFlag;
                  }

                  //SCP219674 : InvalidAmountToBeSettled Validation

                  #region New Code for Validatation of CorrespondenceAmounttobeSettled

                  if (!GetIsValidCorrAmountToBeSettled(miscUatpInvoice, miscCorrespondence))
                  {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                    exceptionDetailsList.Count() + 1,
                                                                                    fileSubmissionDate,
                                                                                    miscUatpInvoice,
                                                                                    "Total Amount In Clearance Currency",
                                                                                    miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.ToString(),
                                                                                    fileName,
                                                                                    ErrorLevels.ErrorLevelInvoice,
                                                                                    MiscUatpErrorCodes.InvalidCorrspondenceAmountSettled,
                                                                                    ErrorStatus.X,
                                                                                    0,
                                                                                    0,
                                                                                    false);
                    exceptionDetailsList.Add(validationExceptionDetail);
                    isValid = false;
                  }

                  #endregion
                }
                  // Error if Correspondence Invoice is already created for the correspondence ref number.
                else
                {
                  var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                                  exceptionDetailsList.Count() + 1,
                                                                                  fileSubmissionDate,
                                                                                  miscUatpInvoice,
                                                                                  "Correspondence Invoice number",
                                                                                  miscUatpInvoice.RejectedInvoiceNumber,
                                                                                  fileName,
                                                                                  ErrorLevels.ErrorLevelInvoice,
                                                                                  MiscUatpErrorCodes.DuplicateInvoiceForCorrespondenceRefNo,
                                                                                  ErrorStatus.X);
                  exceptionDetailsList.Add(validationExceptionDetail);
                  isValid = false;
                }
              }
              rejectedInvoice.ValidationDate = new DateTime(rejectedInvoice.SettlementYear, rejectedInvoice.SettlementMonth, rejectedInvoice.SettlementPeriod);
              // Transaction out side time limit validation. UC-G3320-3.1 13-e
              if (IsTransactionOutSideTimeLimit(rejectedInvoice))
              {
                miscUatpInvoice.IsValidationFlag = string.IsNullOrEmpty(miscUatpInvoice.IsValidationFlag) ? TimeLimitFlag : "";
              }
          //  }
          }
        }
        else
        {
            //SCP485960: Request to re-interface MXMLT-62920160501 file to be uploaded in SAP UNIX Server
            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate,
                                                                            miscUatpInvoice,
                                                                            "CorrespondenceDetails",
                                                                            String.Empty,
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelInvoice,
                                                                            miscUatpInvoice.BillingCategory ==
                                                                            BillingCategoryType.Misc
                                                                                ? MiscUatpErrorCodes.
                                                                                      CorrespondenceDetailsNotProvidedForMiscCorrInvoice
                                                                                : UatpErrorCodes.
                                                                                      CorrespondenceDetailsNotProvidedForUatpCorrInvoice,
                                                                            ErrorStatus.X);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
        }

          if (miscUatpInvoice.InvoiceSummary != null)
        {
          if (miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue)
          {
            if (
              !ReferenceManager.IsValidNetAmount(Convert.ToDouble(miscUatpInvoice.InvoiceSummary.TotalAmount),
                                                 TransactionType.MiscCorrespondence,
                                                 miscUatpInvoice.ListingCurrencyId,
                                                 miscUatpInvoice,
                                                 applicableMinimumField: ApplicableMinimumField.TotalAmount))
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                              exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              miscUatpInvoice,
                                                                              "Total Amount",
                                                                              miscUatpInvoice.InvoiceSummary.TotalAmount.ToString(),
                                                                              fileName,
                                                                              ErrorLevels.ErrorLevelInvoice,
                                                                              MiscUatpErrorCodes.InvalidTotalAmountOutsideLimit,
                                                                              ErrorStatus.X);
              exceptionDetailsList.Add(validationExceptionDetail);
              isValid = false;
            }
          }
        }

        #endregion

        return isValid;
      }

      #region Latest Copy of ValidateParsedExchangeRate from Dev 1.5 Branch (SCP280744)

    /* Changes related to SCP280744: MISC UATP Exchange Rate population/validation during error correction. */

    /// <summary>
    /// Validates the parsed exchange rate.
    /// </summary>
    /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
    /// <param name="linkedoriginalInvoice">The linkedoriginal invoice.</param>
    /// <param name="linkedRm1Invoice">The linked RM1 invoice.</param>
    /// <param name="linkedRm2Invoice">The linked RM2 invoice.</param>
    /// <param name="linkedCorrespondence">The linked correspondence.</param>
    /// <param name="exceptionDetailsList">The exception details list.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="fileSubmissionDate">The file submission date.</param>
    /// <param name="clearingHouseEnum">The clearing house enum.</param>
    /// <param name="currentBillingPeriod">The current billing period.</param>
    private void ValidateParsedExchangeRate(MiscUatpInvoice miscUatpInvoice,
                                                      MiscUatpInvoice linkedOriginalInvoice,
                                                      MiscUatpInvoice linkedRm1Invoice,
                                                      MiscUatpInvoice linkedRm2Invoice,
                                                      MiscCorrespondence linkedCorrespondence, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate, ClearingHouse clearingHouseEnum, BillingPeriod currentBillingPeriod)
    {
      //var currentBillingPeriod = CalendarManager.GetCurrentBillingPeriod();
      #region Original/Credit Note Invoice


      if (miscUatpInvoice.InvoiceType == InvoiceType.Invoice)
      {
        if (!ReferenceManager.IsSmiLikeBilateral(miscUatpInvoice.SettlementMethodId, true))
        {
          if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
          {
            if (miscUatpInvoice.ListingCurrencyId == miscUatpInvoice.BillingCurrencyId && Convert.ToDecimal(miscUatpInvoice.ExchangeRate) == 1)
            {
              // do not require any processing
            }
            else if (miscUatpInvoice.ListingCurrencyId == miscUatpInvoice.BillingCurrencyId && Convert.ToDecimal(miscUatpInvoice.ExchangeRate) != 1)
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              miscUatpInvoice,
                                                                              "Exchange Rate",
                                                                              Convert.ToString(miscUatpInvoice.ExchangeRate),
                                                                              fileName,
                                                                              ErrorLevels.ErrorLevelInvoice,
                                                                              ErrorCodes.InvalidListingToBillingRateForSameCurrencies,
                                                                              ErrorStatus.X,
                                                                              0,
                                                                              0);
              exceptionDetailsList.Add(validationExceptionDetail);
            }
            else if (!IsValidExchangeRate(miscUatpInvoice, exceptionDetailsList))
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              miscUatpInvoice,
                                                                              "Exchange Rate",
                                                                              Convert.ToString(miscUatpInvoice.ExchangeRate),
                                                                              fileName,
                                                                              ErrorLevels.ErrorLevelInvoice,
                                                                              ErrorCodes.InvalidListingToBillingRate,
                                                                              ErrorStatus.X,
                                                                              0,
                                                                              0);
              exceptionDetailsList.Add(validationExceptionDetail);
            }
          }
          else // Exchange Rate not given in input file.
          {
            if (miscUatpInvoice.ListingCurrencyId != null)
            {
              if (miscUatpInvoice.BillingCurrency != null)
              {
                var exchangeRate = GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value, miscUatpInvoice.BillingCurrency.Value, currentBillingPeriod.Year, currentBillingPeriod.Month);
                miscUatpInvoice.ExchangeRate = Convert.ToDecimal(exchangeRate);
              }
            }
          }

          // Update Amount in clearance currency
          if (miscUatpInvoice.InvoiceSummary != null && !miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate.HasValue && miscUatpInvoice.ExchangeRate.Value > 0)
          {
            miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate.Value;
          }
        }
      }

      else if (miscUatpInvoice.InvoiceType == InvoiceType.CreditNote)
      {
        // Exchange rate for credit note.
        decimal exchangeRate = 0;
        if (!ReferenceManager.IsSmiLikeBilateral(miscUatpInvoice.SettlementMethodId, true))
        {

          if (miscUatpInvoice.ListingCurrencyId != null)
          {
            if (miscUatpInvoice.BillingCurrency != null)
            {
              exchangeRate =
                Convert.ToDecimal(GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value, miscUatpInvoice.BillingCurrency.Value, currentBillingPeriod.Year, currentBillingPeriod.Month));
            }
          }
          if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
          {

            if (miscUatpInvoice.ListingCurrencyId == miscUatpInvoice.BillingCurrencyId && Convert.ToDecimal(miscUatpInvoice.ExchangeRate) != 1)
            {
              var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1,
                                                                              fileSubmissionDate,
                                                                              miscUatpInvoice,
                                                                              "Exchange Rate",
                                                                              Convert.ToString(miscUatpInvoice.ExchangeRate),
                                                                              fileName,
                                                                              ErrorLevels.ErrorLevelInvoice,
                                                                              ErrorCodes.InvalidListingToBillingRateForSameCurrencies,
                                                                              ErrorStatus.X,
                                                                              0,
                                                                              0);
              exceptionDetailsList.Add(validationExceptionDetail);
            }

            miscUatpInvoice.IsValidationFlag = ExchangeRateValidationFlag;
          }
          else
          {
            miscUatpInvoice.ExchangeRate = Convert.ToDecimal(exchangeRate);
            miscUatpInvoice.IsValidationFlag = ExchangeRateValidationFlag;
          }

          // Update Amount in clearance currency
          if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && Convert.ToDecimal(miscUatpInvoice.ExchangeRate) > 0)
          {
            miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate.Value;
          }
        }
      }
      #endregion

      #region RejectionInvoice
      else if (miscUatpInvoice.InvoiceType == InvoiceType.RejectionInvoice)
      {
        MiscUatpInvoice originalInvoice = linkedOriginalInvoice;
        MiscUatpInvoice rejectedInvoice1 = linkedRm1Invoice;

        if (miscUatpInvoice.RejectionStage == 1)
        {
          //Fetch original invoice
          //originalInvoice = MiscUatpInvoiceRepository.Single(invoiceNumber: miscUatpInvoice.RejectedInvoiceNumber, billingMemberId: miscUatpInvoice.BilledMemberId, billedMemberId: miscUatpInvoice.BillingMemberId,
          //billingCategoryId: miscUatpInvoice.BillingCategoryId, invoiceStatusId: (int)InvoiceStatusType.Presented, billingPeriod: miscUatpInvoice.SettlementPeriod, billingMonth: miscUatpInvoice.SettlementMonth, billingYear: miscUatpInvoice.SettlementYear);

          // If original invoice found , then use its billing month and year for exchange rate fetch.
          if (originalInvoice != null)
          {
            if (miscUatpInvoice.ListingCurrencyId != null && miscUatpInvoice.BillingCurrency != null)
            {

              var exchangeRate =
                Convert.ToDecimal(GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value, miscUatpInvoice.BillingCurrency.Value, originalInvoice.BillingYear, originalInvoice.BillingMonth));

              // Rejection invoice found and exchange rate given in input file.
              if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
              {//CMP#648: Convert Exchange rate into nullable field.
                if (!CompareUtil.Compare(exchangeRate, miscUatpInvoice.ExchangeRate.HasValue? miscUatpInvoice.ExchangeRate.Value:0.0M, 0D, Constants.ExchangeRateDecimalPlaces))
                {
                  //miscUatpInvoice.IsValidationFlag = ExchangeRateValidationFlag;

                  var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate",
                                                                                Convert.ToString(miscUatpInvoice.ExchangeRate.HasValue ? miscUatpInvoice.ExchangeRate.Value : (decimal?)null),
                                                                                fileName,
                                                                                ErrorLevels.ErrorLevelInvoice,
                                                                                ErrorCodes.InvalidListingToBillingRate,
                                                                                ErrorStatus.X);
                  exceptionDetailsList.Add(validationExceptionDetail);
                }
                else
                {
                  // Update Amount in clearance currency
                  if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate.HasValue && miscUatpInvoice.ExchangeRate.Value > 0)
                  {
                    miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate.Value;
                  }
                }
              }
              else // Exchange rate not given in input file.
              {
                miscUatpInvoice.ExchangeRate = exchangeRate;

                // Update Amount in clearance currency
                if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate.HasValue && miscUatpInvoice.ExchangeRate.Value > 0)
                {
                  miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate.Value;
                }
              }
            }
          }
          else  // If original invoice not found.
          {
            // Use settlement year , month in rejection details.
            if (miscUatpInvoice.ListingCurrencyId != null && miscUatpInvoice.BillingCurrency != null)
            {
              #region SCP# 280744 : MISC UATP Exchange Rate population/validation during error

              /* Check if billed member is migrated */
              bool isBilledMemberMigrated = IsMemberMigrated(miscUatpInvoice);
              /* if isBilledMemberMigrated = true this means => Billed member is migrated, 
               * so linking check is not to be bypassed by the system. Hence this leads to Error-Correctable*/
              if (isBilledMemberMigrated)
              {
                /* The system will NOT validate the Exchange Rate and Amount in Currency of Clearance
                 * Validation of Exchange Rate and/or Amount in Currency of Clearance will happen upon Error Correction */
              }
              else
              {
                /* If linking fails but linking is bypassed by the system (due to migration by the Billed Member)
                 * The system will derive and populate the Applicable Exchange Rate and/or Amount in Currency of Clearance */
                // Rejection invoice not found and exchange rate given in input file.
                if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
                {
                  switch (miscUatpInvoice.InvoiceSmi)
                  {
                    /* 1.Applicable for: a.Billing Category: MISC/UATP b.Submission Method: IS-XML SMIs: I, A or M */
                    case SMI.AchUsingIataRules:
                    case SMI.Ach:
                    case SMI.Ich:
                      var exchangeRate =
                          Convert.ToDecimal(
                              GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value,
                                                     miscUatpInvoice.BillingCurrency.Value,
                                                     miscUatpInvoice.SettlementYear,
                                                     miscUatpInvoice.SettlementMonth));
                      /* Validating Exchange Rate */
                      //CMP#648: Convert Exchange rate into nullable field.
                      if (!CompareUtil.Compare(exchangeRate, miscUatpInvoice.ExchangeRate.HasValue ? miscUatpInvoice.ExchangeRate.Value : 0.0M, 0D, Constants.ExchangeRateDecimalPlaces))
                      {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate", Convert.ToString(miscUatpInvoice.ExchangeRate), fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidListingToBillingRate, ErrorStatus.X);
                        exceptionDetailsList.Add(validationExceptionDetail);
                      }
                      else
                      {
                        // Update Amount in clearance currency
                        if (
                            !miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.
                                 HasValue && miscUatpInvoice.ExchangeRate > 0)
                        {
                          miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency =
                              miscUatpInvoice.InvoiceSummary.TotalAmount /
                              miscUatpInvoice.ExchangeRate.Value;
                        }
                      }
                      break;
                    /* Below commented code is only for logical completion */
                    //case SMI.AdjustmentDueToProtest:
                    //    /* SMI R is only for credit note invoice, not expected to appear for rejection invoice and/or correspondence invoice */
                    //    break;
                    //case SMI.Bilateral:
                    //    break;
                    ///* In case of bilateral SMI both miscUatpInvoice.ListingCurrencyId and miscUatpInvoice.BillingCurrency 
                    // * can not be provided. So this code path is not reachable/executed. 
                    // * Also exchange rate is not suppose to be validated for bilateral invoice */
                    default:
                      /* Logical completion - Do nothing - future added SMIs (behaving like bilateral) will fall in this case. */
                      break;
                  }
                } //End of if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile) 
                else
                {
                  var exchangeRate =
                      Convert.ToDecimal(
                          GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value,
                                                 miscUatpInvoice.BillingCurrency.Value,
                                                 miscUatpInvoice.SettlementYear,
                                                 miscUatpInvoice.SettlementMonth));
                  switch (miscUatpInvoice.InvoiceSmi)
                  {
                    /* 1.Applicable for: a.Billing Category: MISC/UATP b.Submission Method: IS-XML SMIs: I, A or M */
                    case SMI.AchUsingIataRules:
                    case SMI.Ach:
                    case SMI.Ich:
                      /* Populating Exchange Rate */
                      miscUatpInvoice.ExchangeRate = exchangeRate;
                      if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate.HasValue && miscUatpInvoice.ExchangeRate > 0)
                      {
                        miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate.Value;
                      }
                      break;
                    /* Below commented code is only for logical completion */
                    //case SMI.AdjustmentDueToProtest:
                    //    /* SMI R is only for credit note invoice, not expected to appear for rejection invoice and/or correspondence invoice */
                    //    break;
                    //case SMI.Bilateral:
                    //    break;
                    ///* In case of bilateral SMI both miscUatpInvoice.ListingCurrencyId and miscUatpInvoice.BillingCurrency 
                    // * can not be provided. So this code path is not reachable/executed. 
                    // * Also exchange rate is not suppose to be validated for bilateral invoice */
                    default:
                      /* Logical completion - Do nothing - future added SMIs (behaving like bilateral) will fall in this case. */
                      break;
                  }
                }
              }// End of if(isBilledMemberMigrated)

              #endregion


            }
          }
        }
        else if (miscUatpInvoice.RejectionStage == 2)  // If rejection stage is 2 
        {
          rejectedInvoice1 = linkedRm1Invoice;

          if (rejectedInvoice1 != null)
          {
            originalInvoice = linkedOriginalInvoice;

            // If original invoice found , then use its billing month and year for exchange rate fetch
            if (originalInvoice != null)
            {
              var exchangeRate =
                Convert.ToDecimal(GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value, miscUatpInvoice.BillingCurrency.Value, originalInvoice.BillingYear, originalInvoice.BillingMonth));

              // Rejection invoice found and exchange rate given.
              if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
              {
                //CMP#648: Convert Exchange rate into nullable field.
                if (!CompareUtil.Compare(exchangeRate, miscUatpInvoice.ExchangeRate.HasValue ? miscUatpInvoice.ExchangeRate.Value : 0.0M, 0D, Constants.ExchangeRateDecimalPlaces))
                {
                  var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate",
                                                                                Convert.ToString(miscUatpInvoice.ExchangeRate),
                                                                                fileName,
                                                                                ErrorLevels.ErrorLevelInvoice,
                                                                                ErrorCodes.InvalidListingToBillingRate,
                                                                                ErrorStatus.X);
                  exceptionDetailsList.Add(validationExceptionDetail);
                }
                else
                {
                  // Update Amount in clearance currency
                  if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate.HasValue && miscUatpInvoice.ExchangeRate.Value > 0)
                  {
                    miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate.Value;
                  }
                }
              }
              else
              {
                miscUatpInvoice.ExchangeRate = exchangeRate;

                // Update Amount in clearance currency
                if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate.HasValue && miscUatpInvoice.ExchangeRate.Value > 0)
                {
                  miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate.Value;
                }
              }
            }
            else //// If original invoice  not found 
            {
              if (miscUatpInvoice.ListingCurrencyId != null && miscUatpInvoice.BillingCurrency != null)
              {
                #region SCP# 280744 : MISC UATP Exchange Rate population/validation during error
                
                  /* If linking fails but linking is bypassed by the system (due to migration by the Billed Member)
                   * The system will derive and populate the Applicable Exchange Rate and/or Amount in Currency of Clearance */
                  // Rejection invoice not found and exchange rate given in input file.
                  if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
                  {
                      switch (miscUatpInvoice.InvoiceSmi)
                      {
                          /* 1.Applicable for: a.Billing Category: MISC/UATP b.Submission Method: IS-XML SMIs: I, A or M */
                          case SMI.AchUsingIataRules:
                          case SMI.Ach:
                          case SMI.Ich:
                              var exchangeRate =
                                  Convert.ToDecimal(
                                      GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value,
                                                             miscUatpInvoice.BillingCurrency.Value,
                                                             miscUatpInvoice.BillingYear,
                                                             miscUatpInvoice.BillingMonth));
                              /* Validating Exchange Rate */
                              //CMP#648: Convert Exchange rate into nullable field.
                              if (!CompareUtil.Compare(exchangeRate, miscUatpInvoice.ExchangeRate.HasValue ? miscUatpInvoice.ExchangeRate.Value : 0.0M, 0D, Constants.ExchangeRateDecimalPlaces))
                              {
                                var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate", Convert.ToString(miscUatpInvoice.ExchangeRate.HasValue ? miscUatpInvoice.ExchangeRate.Value : (decimal?)null), fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidListingToBillingRate, ErrorStatus.X);
                                  exceptionDetailsList.Add(validationExceptionDetail);
                              }
                              else
                              {
                                  // Update Amount in clearance currency
                                  if (
                                      !miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.
                                           HasValue && miscUatpInvoice.ExchangeRate.HasValue && miscUatpInvoice.ExchangeRate.Value > 0)
                                  {
                                      miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency =
                                          miscUatpInvoice.InvoiceSummary.TotalAmount /
                                          miscUatpInvoice.ExchangeRate.Value;
                                  }
                              }
                              break;
                          /* Below commented code is only for logical completion */
                          //case SMI.AdjustmentDueToProtest:
                          //    /* SMI R is only for credit note invoice, not expected to appear for rejection invoice and/or correspondence invoice */
                          //    break;
                          //case SMI.Bilateral:
                          //    break;
                          ///* In case of bilateral SMI both miscUatpInvoice.ListingCurrencyId and miscUatpInvoice.BillingCurrency 
                          // * can not be provided. So this code path is not reachable/executed. 
                          // * Also exchange rate is not suppose to be validated for bilateral invoice */
                          default:
                              /* Logical completion - Do nothing - future added SMIs (behaving like bilateral) will fall in this case. */
                              break;
                      }
                  } //End of if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile) 
                  else
                  {
                      var exchangeRate =
                          Convert.ToDecimal(
                              GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value,
                                                     miscUatpInvoice.BillingCurrency.Value,
                                                     miscUatpInvoice.BillingYear,
                                                     miscUatpInvoice.BillingMonth));
                      switch (miscUatpInvoice.InvoiceSmi)
                      {
                              /* 1.Applicable for: a.Billing Category: MISC/UATP b.Submission Method: IS-XML SMIs: I, A or M */
                          case SMI.AchUsingIataRules:
                          case SMI.Ach:
                          case SMI.Ich:
                              /* Populating Exchange Rate */
                              miscUatpInvoice.ExchangeRate = exchangeRate;
                              if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue &&
                                  miscUatpInvoice.ExchangeRate.HasValue && miscUatpInvoice.ExchangeRate.Value > 0)
                              {
                                  miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency =
                                      miscUatpInvoice.InvoiceSummary.TotalAmount/miscUatpInvoice.ExchangeRate.Value;
                              }
                              break;
                              /* Below commented code is only for logical completion */
                              //case SMI.AdjustmentDueToProtest:
                              //    /* SMI R is only for credit note invoice, not expected to appear for rejection invoice and/or correspondence invoice */
                              //    break;
                              //case SMI.Bilateral:
                              //    break;
                              ///* In case of bilateral SMI both miscUatpInvoice.ListingCurrencyId and miscUatpInvoice.BillingCurrency 
                              // * can not be provided. So this code path is not reachable/executed. 
                              // * Also exchange rate is not suppose to be validated for bilateral invoice */
                          default:
                              /* Logical completion - Do nothing - future added SMIs (behaving like bilateral) will fall in this case. */
                              break;
                      }
                  }

                  #endregion
              }
            }
          }
          else // R1 invoice not found , then use current fdr exchange rate will be fetched.
          {
            if (miscUatpInvoice.ListingCurrencyId != null && miscUatpInvoice.BillingCurrency != null)
            {
              #region SCP# 280744 : MISC UATP Exchange Rate population/validation during error

              /* Check if billed member is migrated */
              bool isBilledMemberMigrated = IsMemberMigrated(miscUatpInvoice);
              /* if isBilledMemberMigrated = true this means => Billed member is migrated, 
               * so linking check is not to be bypassed by the system. Hence this leads to Error-Correctable*/
              if (isBilledMemberMigrated)
              {
                /* The system will NOT validate the Exchange Rate and Amount in Currency of Clearance
                 * Validation of Exchange Rate and/or Amount in Currency of Clearance will happen upon Error Correction */
              }
              else
              {
                /* If linking fails but linking is bypassed by the system (due to migration by the Billed Member)
                 * The system will derive and populate the Applicable Exchange Rate and/or Amount in Currency of Clearance */
                // Rejection invoice not found and exchange rate given in input file.
                if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
                {
                  switch (miscUatpInvoice.InvoiceSmi)
                  {
                    /* 1.Applicable for: a.Billing Category: MISC/UATP b.Submission Method: IS-XML SMIs: I, A or M */
                    case SMI.AchUsingIataRules:
                    case SMI.Ach:
                    case SMI.Ich:
                      var exchangeRate =
                          Convert.ToDecimal(
                              GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value,
                                                     miscUatpInvoice.BillingCurrency.Value,
                                                     miscUatpInvoice.BillingYear,
                                                     miscUatpInvoice.BillingMonth));
                      /* Validating Exchange Rate */
                      //CMP#648: Convert Exchange rate into nullable field.
                      if (!CompareUtil.Compare(exchangeRate, miscUatpInvoice.ExchangeRate.HasValue ? miscUatpInvoice.ExchangeRate.Value : 0.0M, 0D, Constants.ExchangeRateDecimalPlaces))
                      {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate", Convert.ToString(miscUatpInvoice.ExchangeRate), fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidListingToBillingRate, ErrorStatus.X);
                        exceptionDetailsList.Add(validationExceptionDetail);
                      }
                      else
                      {
                        // Update Amount in clearance currency
                        if (
                            !miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.
                                 HasValue && miscUatpInvoice.ExchangeRate.HasValue && miscUatpInvoice.ExchangeRate.Value > 0)
                        {
                          miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency =
                              miscUatpInvoice.InvoiceSummary.TotalAmount /
                              miscUatpInvoice.ExchangeRate.Value;
                        }
                      }
                      break;
                    /* Below commented code is only for logical completion */
                    //case SMI.AdjustmentDueToProtest:
                    //    /* SMI R is only for credit note invoice, not expected to appear for rejection invoice and/or correspondence invoice */
                    //    break;
                    //case SMI.Bilateral:
                    //    break;
                    ///* In case of bilateral SMI both miscUatpInvoice.ListingCurrencyId and miscUatpInvoice.BillingCurrency 
                    // * can not be provided. So this code path is not reachable/executed. 
                    // * Also exchange rate is not suppose to be validated for bilateral invoice */
                    default:
                      /* Logical completion - Do nothing - future added SMIs (behaving like bilateral) will fall in this case. */
                      break;
                  }
                } //End of if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile) 
                else
                {
                  var exchangeRate =
                      Convert.ToDecimal(
                          GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value,
                                                 miscUatpInvoice.BillingCurrency.Value,
                                                 miscUatpInvoice.BillingYear,
                                                 miscUatpInvoice.BillingMonth));
                  switch (miscUatpInvoice.InvoiceSmi)
                  {
                    /* 1.Applicable for: a.Billing Category: MISC/UATP b.Submission Method: IS-XML SMIs: I, A or M */
                    case SMI.AchUsingIataRules:
                    case SMI.Ach:
                    case SMI.Ich:
                      /* Populating Exchange Rate */
                      miscUatpInvoice.ExchangeRate = exchangeRate;
                      if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && miscUatpInvoice.ExchangeRate.HasValue && miscUatpInvoice.ExchangeRate.Value > 0)
                      {
                        miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate.Value;
                      }
                      break;
                    /* Below commented code is only for logical completion */
                    //case SMI.AdjustmentDueToProtest:
                    //    /* SMI R is only for credit note invoice, not expected to appear for rejection invoice and/or correspondence invoice */
                    //    break;
                    //case SMI.Bilateral:
                    //    break;
                    ///* In case of bilateral SMI both miscUatpInvoice.ListingCurrencyId and miscUatpInvoice.BillingCurrency 
                    // * can not be provided. So this code path is not reachable/executed. 
                    // * Also exchange rate is not suppose to be validated for bilateral invoice */
                    default:
                      /* Logical completion - Do nothing - future added SMIs (behaving like bilateral) will fall in this case. */
                      break;
                  }
                }
              }// End of if(isBilledMemberMigrated)

              #endregion
            }
          }
        }
      }

      #endregion

      #region CorrespondenceInvoice
      else if (miscUatpInvoice.InvoiceType == InvoiceType.CorrespondenceInvoice && miscUatpInvoice.CorrespondenceRefNo != null)
      {
        MiscUatpInvoice originalInvoice;
        MiscUatpInvoice rejectedInvoice1;

        var miscCorrespondence = linkedCorrespondence;

        if (miscCorrespondence != null)
        {
          rejectedInvoice1 = miscCorrespondence.Invoice;

          //SCP#371970 - SIS: ICH Settlement Error - SIS Production
          //Desc: Comparison between below two values has to be case insensitive.
          //1. rejection invoice number provided as input in Misc Correspondence invoice and 
          //2. Invoice number of invoice linked to respective correspondence number.
          if (!string.Equals(rejectedInvoice1.InvoiceNumber, miscUatpInvoice.RejectedInvoiceNumber, StringComparison.CurrentCultureIgnoreCase))
          { 
              /* Linking Details provided in File are incorrect and so exchange rate validation should be bypassed. */
              return;
          }

          if (rejectedInvoice1.RejectionStage == 1)
          {
            //Fetch original invoice
            originalInvoice = linkedOriginalInvoice;

            // If original invoice found , then use its billing month and year for exchange rate fetch.
            if (originalInvoice != null)
            {
              var exchangeRate = Convert.ToDecimal(GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value, miscUatpInvoice.BillingCurrency.Value, originalInvoice.BillingYear, originalInvoice.BillingMonth));

              // Rejection invoice found and exchange rate given in input file.
              if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
              {//CMP#648: Convert Exchange rate into nullable field.
                if (!CompareUtil.Compare(exchangeRate, miscUatpInvoice.ExchangeRate.HasValue ? miscUatpInvoice.ExchangeRate.Value : 0.0M, 0D, Constants.ExchangeRateDecimalPlaces))
                {

                  var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate",
                                                                                  Convert.ToString(miscUatpInvoice.ExchangeRate),
                                                                                  fileName,
                                                                                  ErrorLevels.ErrorLevelInvoice,
                                                                                  ErrorCodes.InvalidListingToBillingRate,
                                                                                  ErrorStatus.X);
                  exceptionDetailsList.Add(validationExceptionDetail);
                }
                else
                {
                  // Update Amount in clearance currency
                  if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && Convert.ToDecimal(miscUatpInvoice.ExchangeRate) != 0)
                  {
                    miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency =
                      miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate.Value;
                  }
                }
              }
              else // Exchange rate not given in input file.
              {
                miscUatpInvoice.ExchangeRate = exchangeRate;

                // Update Amount in clearance currency
                if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && Convert.ToDecimal(miscUatpInvoice.ExchangeRate) != 0)
                {
                  miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate.Value;
                }
              }
            }
            else // If original invoice not found.
            {
              #region SCP# 280744 : MISC UATP Exchange Rate population/validation during error

              /* Check if billed member is migrated */
              bool isBilledMemberMigrated = IsMemberMigrated(miscUatpInvoice);
              /* if isBilledMemberMigrated = true this means => Billed member is migrated, 
               * so linking check is not to be bypassed by the system. Hence this leads to Error-Correctable*/
              if (isBilledMemberMigrated)
              {
                /* The system will NOT validate the Exchange Rate and Amount in Currency of Clearance
                 * Validation of Exchange Rate and/or Amount in Currency of Clearance will happen upon Error Correction */
              }
              else
              {
                /* If linking fails but linking is bypassed by the system (due to migration by the Billed Member)
                 * The system will derive and populate the Applicable Exchange Rate and/or Amount in Currency of Clearance */
                // Rejection invoice not found and exchange rate given in input file.
                if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
                {
                  switch (miscUatpInvoice.InvoiceSmi)
                  {
                    /* 1.Applicable for: a.Billing Category: MISC/UATP b.Submission Method: IS-XML SMIs: I, A or M */
                    case SMI.AchUsingIataRules:
                    case SMI.Ach:
                    case SMI.Ich:
                      var exchangeRate =
                          Convert.ToDecimal(
                              GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value,
                                                     miscUatpInvoice.BillingCurrency.Value,
                                                     rejectedInvoice1.SettlementYear,
                                                     rejectedInvoice1.SettlementMonth));
                      /* Validating Exchange Rate */
                      //CMP#648: Convert Exchange rate into nullable field.
                      if (!CompareUtil.Compare(exchangeRate, miscUatpInvoice.ExchangeRate.HasValue ? miscUatpInvoice.ExchangeRate.Value : 0.0M, 0D, Constants.ExchangeRateDecimalPlaces))
                      {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate", Convert.ToString(miscUatpInvoice.ExchangeRate), fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidListingToBillingRate, ErrorStatus.X);
                        exceptionDetailsList.Add(validationExceptionDetail);
                      }
                      else
                      {
                        // Update Amount in clearance currency
                        if (
                            !miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.
                                 HasValue && Convert.ToDecimal(miscUatpInvoice.ExchangeRate) != 0)
                        {
                          miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency =
                              miscUatpInvoice.InvoiceSummary.TotalAmount /
                              miscUatpInvoice.ExchangeRate.Value;
                        }
                      }
                      break;
                    /* Below commented code is only for logical completion */
                    //case SMI.AdjustmentDueToProtest:
                    //    /* SMI R is only for credit note invoice, not expected to appear for rejection invoice and/or correspondence invoice */
                    //    break;
                    //case SMI.Bilateral:
                    //    break;
                    ///* In case of bilateral SMI both miscUatpInvoice.ListingCurrencyId and miscUatpInvoice.BillingCurrency 
                    // * can not be provided. So this code path is not reachable/executed. 
                    // * Also exchange rate is not suppose to be validated for bilateral invoice */
                    default:
                      /* Logical completion - Do nothing - future added SMIs (behaving like bilateral) will fall in this case. */
                      break;
                  }
                } //End of if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile) 
                else
                {
                  var exchangeRate =
                      Convert.ToDecimal(GetExchangeRateForMisc(
                          miscUatpInvoice.ListingCurrencyId.Value,
                          miscUatpInvoice.BillingCurrency.Value,
                          rejectedInvoice1.SettlementYear,
                          rejectedInvoice1.SettlementMonth));
                  switch (miscUatpInvoice.InvoiceSmi)
                  {
                    /* 1.Applicable for: a.Billing Category: MISC/UATP b.Submission Method: IS-XML SMIs: I, A or M */
                    case SMI.AchUsingIataRules:
                    case SMI.Ach:
                    case SMI.Ich:
                      /* Populating Exchange Rate */
                      miscUatpInvoice.ExchangeRate = exchangeRate;
                      if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && Convert.ToDecimal(miscUatpInvoice.ExchangeRate) > 0)
                      {
                        miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate.Value;
                      }
                      break;
                    /* Below commented code is only for logical completion */
                    //case SMI.AdjustmentDueToProtest:
                    //    /* SMI R is only for credit note invoice, not expected to appear for rejection invoice and/or correspondence invoice */
                    //    break;
                    //case SMI.Bilateral:
                    //    break;
                    ///* In case of bilateral SMI both miscUatpInvoice.ListingCurrencyId and miscUatpInvoice.BillingCurrency 
                    // * can not be provided. So this code path is not reachable/executed. 
                    // * Also exchange rate is not suppose to be validated for bilateral invoice */
                    default:
                      /* Logical completion - Do nothing - future added SMIs (behaving like bilateral) will fall in this case. */
                      break;
                  }
                }
              }// End of if(isBilledMemberMigrated)

              #endregion
            }
          }
          else if (rejectedInvoice1.RejectionStage == 2) // If rejection stage is 2 
          {
            // Get rejected invoice from invoice number from rejection details.
            MiscUatpInvoice rejectionInvoiceStageOne;
            rejectionInvoiceStageOne = linkedRm1Invoice;

            if (rejectionInvoiceStageOne != null)
            {
              //Fetch original invoice
              originalInvoice = linkedOriginalInvoice;

              // If original invoice found , then use its billing month and year for exchange rate fetch
              if (originalInvoice != null)
              {

                var exchangeRate = Convert.ToDecimal(GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value, miscUatpInvoice.BillingCurrency.Value, originalInvoice.BillingYear, originalInvoice.BillingMonth));

                // Rejection invoice found and exchange rate given.
                if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
                {
                  //CMP#648: Convert Exchange rate into nullable field.
                  if (!CompareUtil.Compare(exchangeRate, miscUatpInvoice.ExchangeRate.HasValue ? miscUatpInvoice.ExchangeRate.Value : 0.0M, 0D, Constants.ExchangeRateDecimalPlaces))
                  {
                    var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate",
                                                                                  Convert.ToString(miscUatpInvoice.ExchangeRate),
                                                                                  fileName,
                                                                                  ErrorLevels.ErrorLevelInvoice,
                                                                                  ErrorCodes.InvalidListingToBillingRate,
                                                                                  ErrorStatus.X);
                    exceptionDetailsList.Add(validationExceptionDetail);
                  }
                  else
                  {
                    // Update Amount in clearance currency
                    if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && Convert.ToDecimal(miscUatpInvoice.ExchangeRate) != 0)
                    {
                      miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency =
                        miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate.Value;
                    }
                  }
                }
                else
                {
                  miscUatpInvoice.ExchangeRate = exchangeRate;

                  // Update Amount in clearance currency
                  if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && Convert.ToDecimal(miscUatpInvoice.ExchangeRate) > 0)
                  {
                    miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate.Value;
                  }
                }
              }
              else //// If original invoice not found 
              {
                #region SCP# 280744 : MISC UATP Exchange Rate population/validation during error

                
                  /* If linking fails but linking is bypassed by the system (due to migration by the Billed Member)
                   * The system will derive and populate the Applicable Exchange Rate and/or Amount in Currency of Clearance */
                  // Rejection invoice not found and exchange rate given in input file.
                  if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
                  {
                      switch (miscUatpInvoice.InvoiceSmi)
                      {
                          /* 1.Applicable for: a.Billing Category: MISC/UATP b.Submission Method: IS-XML SMIs: I, A or M */
                          case SMI.AchUsingIataRules:
                          case SMI.Ach:
                          case SMI.Ich:
                              var exchangeRate =
                                  Convert.ToDecimal(
                                      GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value,
                                                             miscUatpInvoice.BillingCurrency.Value,
                                                             miscUatpInvoice.BillingYear,
                                                             miscUatpInvoice.BillingMonth));
                              /* Validating Exchange Rate */
                              //CMP#648: Convert Exchange rate into nullable field.
                              if (!CompareUtil.Compare(exchangeRate, miscUatpInvoice.ExchangeRate.HasValue ? miscUatpInvoice.ExchangeRate.Value : 0.0M, 0D, Constants.ExchangeRateDecimalPlaces))
                              {
                                  var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate", Convert.ToString(miscUatpInvoice.ExchangeRate), fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidListingToBillingRate, ErrorStatus.X);
                                  exceptionDetailsList.Add(validationExceptionDetail);
                              }
                              else
                              {
                                  // Update Amount in clearance currency
                                  if (
                                      !miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.
                                           HasValue && Convert.ToDecimal(miscUatpInvoice.ExchangeRate) > 0)
                                  {
                                      miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency =
                                          miscUatpInvoice.InvoiceSummary.TotalAmount /
                                          miscUatpInvoice.ExchangeRate.Value;
                                  }
                              }
                              break;
                          /* Below commented code is only for logical completion */
                          //case SMI.AdjustmentDueToProtest:
                          //    /* SMI R is only for credit note invoice, not expected to appear for rejection invoice and/or correspondence invoice */
                          //    break;
                          //case SMI.Bilateral:
                          //    break;
                          ///* In case of bilateral SMI both miscUatpInvoice.ListingCurrencyId and miscUatpInvoice.BillingCurrency 
                          // * can not be provided. So this code path is not reachable/executed. 
                          // * Also exchange rate is not suppose to be validated for bilateral invoice */
                          default:
                              /* Logical completion - Do nothing - future added SMIs (behaving like bilateral) will fall in this case. */
                              break;
                      }
                  } //End of if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile) 
                  else
                  {
                      var exchangeRate =
                          Convert.ToDecimal(
                              GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value,
                                                     miscUatpInvoice.BillingCurrency.Value,
                                                     miscUatpInvoice.BillingYear,
                                                     miscUatpInvoice.BillingMonth));
                      switch (miscUatpInvoice.InvoiceSmi)
                      {
                              /* 1.Applicable for: a.Billing Category: MISC/UATP b.Submission Method: IS-XML SMIs: I, A or M */
                          case SMI.AchUsingIataRules:
                          case SMI.Ach:
                          case SMI.Ich:
                              /* Populating Exchange Rate */
                              miscUatpInvoice.ExchangeRate = exchangeRate;
                              if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue &&
                                  Convert.ToDecimal(miscUatpInvoice.ExchangeRate.Value) > 0)
                              {
                                  miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency =
                                      miscUatpInvoice.InvoiceSummary.TotalAmount/miscUatpInvoice.ExchangeRate.Value;
                              }
                              break;
                              /* Below commented code is only for logical completion */
                              //case SMI.AdjustmentDueToProtest:
                              //    /* SMI R is only for credit note invoice, not expected to appear for rejection invoice and/or correspondence invoice */
                              //    break;
                              //case SMI.Bilateral:
                              //    break;
                              ///* In case of bilateral SMI both miscUatpInvoice.ListingCurrencyId and miscUatpInvoice.BillingCurrency 
                              // * can not be provided. So this code path is not reachable/executed. 
                              // * Also exchange rate is not suppose to be validated for bilateral invoice */
                          default:
                              /* Logical completion - Do nothing - future added SMIs (behaving like bilateral) will fall in this case. */
                              break;
                      }
                  }

                  #endregion
              }
            }
            else // R1 invoice not found , then use current fdr exchange rate will be fetched.
            {
              #region SCP# 280744 : MISC UATP Exchange Rate population/validation during error

              /* Check if billed member is migrated */
              bool isBilledMemberMigrated = IsMemberMigrated(miscUatpInvoice);
              /* if isBilledMemberMigrated = true this means => Billed member is migrated, 
               * so linking check is not to be bypassed by the system. Hence this leads to Error-Correctable*/
              if (isBilledMemberMigrated)
              {
                /* The system will NOT validate the Exchange Rate and Amount in Currency of Clearance
                 * Validation of Exchange Rate and/or Amount in Currency of Clearance will happen upon Error Correction */
              }
              else
              {
                /* If linking fails but linking is bypassed by the system (due to migration by the Billed Member)
                 * The system will derive and populate the Applicable Exchange Rate and/or Amount in Currency of Clearance */
                // Rejection invoice not found and exchange rate given in input file.
                if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile)
                {
                  switch (miscUatpInvoice.InvoiceSmi)
                  {
                    /* 1.Applicable for: a.Billing Category: MISC/UATP b.Submission Method: IS-XML SMIs: I, A or M */
                    case SMI.AchUsingIataRules:
                    case SMI.Ach:
                    case SMI.Ich:
                      var exchangeRate =
                          Convert.ToDecimal(
                              GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value,
                                                     miscUatpInvoice.BillingCurrency.Value,
                                                     miscUatpInvoice.BillingYear,
                                                     miscUatpInvoice.BillingMonth));
                      /* Validating Exchange Rate */
                      //CMP#648: Convert Exchange rate into nullable field.
                      if (!CompareUtil.Compare(exchangeRate, miscUatpInvoice.ExchangeRate.HasValue ? miscUatpInvoice.ExchangeRate.Value : 0.0M, 0D, Constants.ExchangeRateDecimalPlaces))
                      {
                        var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(), exceptionDetailsList.Count() + 1, fileSubmissionDate, miscUatpInvoice, "Exchange Rate", Convert.ToString(miscUatpInvoice.ExchangeRate), fileName, ErrorLevels.ErrorLevelInvoice, ErrorCodes.InvalidListingToBillingRate, ErrorStatus.X);
                        exceptionDetailsList.Add(validationExceptionDetail);
                      }
                      else
                      {
                        // Update Amount in clearance currency
                        if (
                            !miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.
                                 HasValue && Convert.ToDecimal(miscUatpInvoice.ExchangeRate) > 0)
                        {
                          miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency =
                              miscUatpInvoice.InvoiceSummary.TotalAmount /
                              miscUatpInvoice.ExchangeRate.Value;
                        }
                      }
                      break;
                    /* Below commented code is only for logical completion */
                    //case SMI.AdjustmentDueToProtest:
                    //    /* SMI R is only for credit note invoice, not expected to appear for rejection invoice and/or correspondence invoice */
                    //    break;
                    //case SMI.Bilateral:
                    //    break;
                    ///* In case of bilateral SMI both miscUatpInvoice.ListingCurrencyId and miscUatpInvoice.BillingCurrency 
                    // * can not be provided. So this code path is not reachable/executed. 
                    // * Also exchange rate is not suppose to be validated for bilateral invoice */
                    default:
                      /* Logical completion - Do nothing - future added SMIs (behaving like bilateral) will fall in this case. */
                      break;
                  }
                } //End of if (miscUatpInvoice.IsExchangeRateProvidedInXmlFile) 
                else
                {
                  var exchangeRate =
                      Convert.ToDecimal(
                          GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value,
                                                 miscUatpInvoice.BillingCurrency.Value,
                                                 miscUatpInvoice.BillingYear,
                                                 miscUatpInvoice.BillingMonth));
                  switch (miscUatpInvoice.InvoiceSmi)
                  {
                    /* 1.Applicable for: a.Billing Category: MISC/UATP b.Submission Method: IS-XML SMIs: I, A or M */
                    case SMI.AchUsingIataRules:
                    case SMI.Ach:
                    case SMI.Ich:
                      /* Populating Exchange Rate */
                      miscUatpInvoice.ExchangeRate = exchangeRate;
                      if (!miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && Convert.ToDecimal(miscUatpInvoice.ExchangeRate) != 0)
                      {
                        miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate.Value;
                      }
                      break;
                    /* Below commented code is only for logical completion */
                    //case SMI.AdjustmentDueToProtest:
                    //    /* SMI R is only for credit note invoice, not expected to appear for rejection invoice and/or correspondence invoice */
                    //    break;
                    //case SMI.Bilateral:
                    //    break;
                    ///* In case of bilateral SMI both miscUatpInvoice.ListingCurrencyId and miscUatpInvoice.BillingCurrency 
                    // * can not be provided. So this code path is not reachable/executed. 
                    // * Also exchange rate is not suppose to be validated for bilateral invoice */
                    default:
                      /* Logical completion - Do nothing - future added SMIs (behaving like bilateral) will fall in this case. */
                      break;
                  }
                }
              }// End of if(isBilledMemberMigrated)

              #endregion
            }
          }
        }
      }
      #endregion
    }

    /// <summary>
    /// SCP280744: MISC UATP Exchange Rate population/validation during error correction
    /// SCP321993: FW ICH Settlement Error - SIS Production
    /// </summary>
    /// <param name="miscUatpInvoice"></param>
    /// <param name="updatedExRate"> Updated Exchange Rate (SCP321993: FW ICH Settlement Error - SIS Production) </param>
    /// <param name="updatedClearanceAmt"> Updated Clearance Amount (SCP321993: FW ICH Settlement Error - SIS Production) </param>
    public string ExchangeRateValidationsOnErrorCorrection(MiscUatpInvoice miscUatpInvoice, out decimal? updatedExRate, out decimal? updatedClearanceAmt)
    {
      updatedExRate = null;
      updatedClearanceAmt = null;
        /* For Logical Completion */
        //if (miscUatpInvoice.InvoiceType == InvoiceType.Invoice || miscUatpInvoice.InvoiceType == InvoiceType.CreditNote)
        //{

        //}
      //CMP#624 Get all linked invoices 
        MiscUatpInvoice linkedRejectedInvoice1;
        MiscUatpInvoice linkedRejectedInvoice2;
        MiscCorrespondence linkedCorrespondence;
        MiscUatpInvoice linkedOriginalInvoice = GetLinkedMUOriginalInvoice(miscUatpInvoice, out linkedRejectedInvoice1, out linkedRejectedInvoice2, out linkedCorrespondence);

        if (miscUatpInvoice.InvoiceType == InvoiceType.RejectionInvoice)
        {
            MiscUatpInvoice originalInvoice;
            MiscUatpInvoice rejectedInvoice1;

            switch (miscUatpInvoice.RejectionStage)
            {
                case 1:
                     originalInvoice = linkedOriginalInvoice;

                    if (originalInvoice != null)
                    {
                        #region SCP# 280744 : MISC UATP Exchange Rate population/validation during error

                        var exchangeRateFromFDRMaster =
                                    Convert.ToDecimal(GetExchangeRateForMisc(
                                        miscUatpInvoice.ListingCurrencyId.Value,
                                        miscUatpInvoice.BillingCurrency.Value,
                                        originalInvoice.BillingYear,
                                        originalInvoice.BillingMonth));

                        return ValidateRejInvAndCorresInvOnErrorCorrection(miscUatpInvoice, exchangeRateFromFDRMaster, out updatedExRate, out updatedClearanceAmt);

                        #endregion
                    }
                    else
                    {
                        /* Which exchange rate to pick for validation/derivation */
                        var exchangeRateFromFDRMaster =
                                        Convert.ToDecimal(
                                            GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value,
                                                                   miscUatpInvoice.BillingCurrency.Value,
                                                                   miscUatpInvoice.SettlementYear,
                                                                   miscUatpInvoice.SettlementMonth));

                        return ValidateRejInvAndCorresInvOnErrorCorrection(miscUatpInvoice, exchangeRateFromFDRMaster, out updatedExRate, out updatedClearanceAmt);
                    }
                    return "12";
                    break;
                case 2:
                    rejectedInvoice1 = linkedRejectedInvoice1;
                    if (rejectedInvoice1 != null)
                    {
                        //Fetch original invoice
                        originalInvoice = linkedOriginalInvoice;

                        // If original invoice found , then use its billing month and year for exchange rate fetch
                        if (originalInvoice != null)
                        {
                            #region SCP# 280744 : MISC UATP Exchange Rate population/validation during error

                            var exchangeRateFromFDRMaster =
                                    Convert.ToDecimal(GetExchangeRateForMisc(
                                        miscUatpInvoice.ListingCurrencyId.Value,
                                        miscUatpInvoice.BillingCurrency.Value,
                                        originalInvoice.BillingYear,
                                        originalInvoice.BillingMonth));

                            return ValidateRejInvAndCorresInvOnErrorCorrection(miscUatpInvoice, exchangeRateFromFDRMaster, out updatedExRate, out updatedClearanceAmt);

                            #endregion

                        }
                        else
                        {
                            /* Which exchange rate to pick for validation/derivation */
                            var exchangeRateFromFDRMaster =
                              Convert.ToDecimal(GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value,
                                                                       miscUatpInvoice.BillingCurrency.Value,
                                                                       miscUatpInvoice.BillingYear,
                                                                       miscUatpInvoice.BillingMonth));

                            return ValidateRejInvAndCorresInvOnErrorCorrection(miscUatpInvoice, exchangeRateFromFDRMaster, out updatedExRate, out updatedClearanceAmt);
                        }
                    }
                    else
                    {
                        /* Which exchange rate to pick for validation/derivation */
                        var exchangeRateFromFDRMaster =
                                        Convert.ToDecimal(
                                            GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value,
                                                                   miscUatpInvoice.BillingCurrency.Value,
                                                                   miscUatpInvoice.BillingYear,
                                                                   miscUatpInvoice.BillingMonth));

                        return ValidateRejInvAndCorresInvOnErrorCorrection(miscUatpInvoice, exchangeRateFromFDRMaster, out updatedExRate, out updatedClearanceAmt);
                    }
                    break;
            }
            /* By default return false */
            return "12";
        }
        else if (miscUatpInvoice.InvoiceType == InvoiceType.CorrespondenceInvoice && miscUatpInvoice.CorrespondenceRefNo != null)
        {
            MiscUatpInvoice originalInvoice;
            MiscUatpInvoice rejectedInvoice;

            var miscCorrespondence = linkedCorrespondence;

            if (miscCorrespondence != null)
            {
                rejectedInvoice = miscCorrespondence.Invoice;

                if (rejectedInvoice != null)
                {
                    switch (rejectedInvoice.RejectionStage)
                    {
                        case 1:
                            originalInvoice = linkedOriginalInvoice;
                            if (originalInvoice != null)
                            {
                                #region SCP# 280744 : MISC UATP Exchange Rate population/validation during error

                                var exchangeRateFromFDRMaster =
                                        Convert.ToDecimal(GetExchangeRateForMisc(
                                            miscUatpInvoice.ListingCurrencyId.Value,
                                            miscUatpInvoice.BillingCurrency.Value,
                                            originalInvoice.BillingYear,
                                            originalInvoice.BillingMonth));

                                return ValidateRejInvAndCorresInvOnErrorCorrection(miscUatpInvoice, exchangeRateFromFDRMaster, out updatedExRate, out updatedClearanceAmt);

                                #endregion
                            }
                            else
                            {
                                /* Which exchange rate to pick for validation/derivation */
                                var exchangeRateFromFDRMaster =
                                    Convert.ToDecimal(GetExchangeRateForMisc(
                                        miscUatpInvoice.ListingCurrencyId.Value,
                                        miscUatpInvoice.BillingCurrency.Value,
                                        rejectedInvoice.SettlementYear,
                                        rejectedInvoice.SettlementMonth));

                                return ValidateRejInvAndCorresInvOnErrorCorrection(miscUatpInvoice, exchangeRateFromFDRMaster, out updatedExRate, out updatedClearanceAmt);
                            }
                            break;
                        case 2:
                            // Get rejected invoice from invoice number from rejection details.
                            var rejectionInvoiceStageOne = linkedRejectedInvoice1;

                            if (rejectionInvoiceStageOne != null)
                            {
                                //Fetch original invoice
                                originalInvoice = linkedOriginalInvoice;

                                // If original invoice found , then use its billing month and year for exchange rate fetch
                                if (originalInvoice != null)
                                {
                                    #region SCP# 280744 : MISC UATP Exchange Rate population/validation during error

                                    var exchangeRateFromFDRMaster =
                                        Convert.ToDecimal(GetExchangeRateForMisc(
                                            miscUatpInvoice.ListingCurrencyId.Value,
                                            miscUatpInvoice.BillingCurrency.Value,
                                            originalInvoice.BillingYear,
                                            originalInvoice.BillingMonth));

                                    return ValidateRejInvAndCorresInvOnErrorCorrection(miscUatpInvoice, exchangeRateFromFDRMaster, out updatedExRate, out updatedClearanceAmt);

                                    #endregion

                                }
                                else
                                {
                                    /* Which exchange rate to pick for validation/derivation */
                                    var exchangeRateFromFDRMaster =
                                        Convert.ToDecimal(GetExchangeRateForMisc(
                                            miscUatpInvoice.ListingCurrencyId.Value,
                                            miscUatpInvoice.BillingCurrency.Value,
                                            miscUatpInvoice.BillingYear,
                                            miscUatpInvoice.BillingMonth));

                                    return ValidateRejInvAndCorresInvOnErrorCorrection(miscUatpInvoice, exchangeRateFromFDRMaster, out updatedExRate, out updatedClearanceAmt);
                                }
                            }
                            else
                            {
                                /* Which exchange rate to pick for validation/derivation */
                                var exchangeRateFromFDRMaster =
                                                Convert.ToDecimal(
                                                    GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value,
                                                                           miscUatpInvoice.BillingCurrency.Value,
                                                                           miscUatpInvoice.BillingYear,
                                                                           miscUatpInvoice.BillingMonth));

                                return ValidateRejInvAndCorresInvOnErrorCorrection(miscUatpInvoice, exchangeRateFromFDRMaster, out updatedExRate, out updatedClearanceAmt);
                            }
                            break;
                    }
                }
                else
                {
                    /* Which exchange rate to pick for validation/derivation */
                    var exchangeRateFromFDRMaster =
                                    Convert.ToDecimal(
                                        GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value,
                                                               miscUatpInvoice.BillingCurrency.Value,
                                                               miscUatpInvoice.BillingYear,
                                                               miscUatpInvoice.BillingMonth));

                    return ValidateRejInvAndCorresInvOnErrorCorrection(miscUatpInvoice, exchangeRateFromFDRMaster, out updatedExRate, out updatedClearanceAmt);
                }
            }
        }

        /* By default return false */
        return "12";
    }

    public string ValidateRejInvAndCorresInvOnErrorCorrection(MiscUatpInvoice miscUatpInvoice, decimal exchangeRateFromFDRMaster, out decimal? updatedExRate, out decimal? updatedClearanceAmt)
    {
      updatedExRate = null;
      updatedClearanceAmt = null;
      
      //var exchangeRateFromFDRMaster =
        //    Convert.ToDecimal(GetExchangeRateForMisc(miscUatpInvoice.ListingCurrencyId.Value,
        //                                             miscUatpInvoice.BillingCurrency.Value,
        //                                             originalInvoice.BillingYear,
        //                                             originalInvoice.BillingMonth));

        /* Step R2.a  If Exchange Rate was already provided in the file */
        if (Convert.ToDecimal(miscUatpInvoice.ExchangeRate) != 0)
        {
            /* Validate Exchange Rate */
           //CMP#648: Convert Exchange rate into nullable field.
          if (!CompareUtil.Compare(exchangeRateFromFDRMaster, miscUatpInvoice.ExchangeRate.HasValue ? miscUatpInvoice.ExchangeRate.Value : 0.0M, 0D, Constants.ExchangeRateDecimalPlaces))
            {
                /* Error Message : Linking failed because an Exchange Rate was already provided for this Invoice 
                 * and it does not match with the Applicable Exchange Rate based on updated linking information provided*/
                //throw business exception
                return "10";
            }
            else
            {
                /* Step R3.a  If Amount in Currency of Clearance was already provided in the file*/
                if (miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency != null
                    && miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue
                    && miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.Value != 0)
                {
                    /* Determine ‘Applicable Amount in Currency of Clearance’ by dividing the Invoice Amount by the Exchange Rate (round to 3 decimals) */
                    //miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
                  //CMP#648: Convert Exchange rate into nullable field.
                  decimal DerivedTotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / (miscUatpInvoice.ExchangeRate.HasValue ? miscUatpInvoice.ExchangeRate.Value : 0.0M);

                    /* Compare */
                    if (!CompareUtil.Compare(DerivedTotalAmountInClearanceCurrency, miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.Value, 3D, Constants.MiscDecimalPlaces))
                    {
                        /* Error Message : Linking failed because an Amount in Currency of Clearance was already provided for this Invoice 
                         * and it does not match with the Applicable Amount in Currency of Clearance using the Applicable Exchange Rate 
                         * based on updated linking information provided */
                        //throw business exception
                        return "11";
                    }
                    /* For logical completion - everyting is OK */
                    else
                    {
                        return "Success";
                    }
                }
                /* 15.Step R3.b  If Amount in Currency of Clearance was NOT provided in the file */
                else
                {
                    /* Determine ‘Applicable Amount in Currency of Clearance’ by dividing the Invoice Amount by the Exchange Rate (round to 3 decimals) */
                    //miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
                  updatedExRate = miscUatpInvoice.ExchangeRate;
                  updatedClearanceAmt = Convert.ToDecimal(miscUatpInvoice.ExchangeRate) > 0 ?miscUatpInvoice.InvoiceSummary.TotalAmount/miscUatpInvoice.ExchangeRate.Value:0;
                  
                  return "Success";
                }
            } //End of if (!CompareUtil.Compare(exchangeRateFromFDRMaster, miscUatpInvoice.ExchangeRate, 0D, Constants.ExchangeRateDecimalPlaces))

        }//End of if (miscUatpInvoice.ExchangeRate == 0)
        /* Step R2.b  If Exchange Rate was NOT provided in the file */
        else
        {
            /* Derive Exchange Rate */
            //miscUatpInvoice.ExchangeRate = exchangeRateFromFDRMaster;
            updatedExRate = exchangeRateFromFDRMaster;

            /* Step R3.a  If Amount in Currency of Clearance was already provided in the file*/
            if (miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency != null
                && miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue
                && miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.Value != 0)
            {
                /* Determine ‘Applicable Amount in Currency of Clearance’ by dividing the Invoice Amount by the Exchange Rate (round to 3 decimals) */
                //miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
              //CMP#648: Convert Exchange rate into nullable field.
              decimal DerivedTotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / (miscUatpInvoice.ExchangeRate.HasValue ? miscUatpInvoice.ExchangeRate.Value : 0.0M);

                /* Compare */
                if (!CompareUtil.Compare(DerivedTotalAmountInClearanceCurrency, miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.Value, 3D, Constants.MiscDecimalPlaces))
                {
                    /* Error Message : Linking failed because an Amount in Currency of Clearance was already provided for this Invoice 
                     * and it does not match with the Applicable Amount in Currency of Clearance using the Applicable Exchange Rate 
                     * based on updated linking information provided */
                    //throw business exception
                    return "11";
                }
                /* For logical completion - everyting is OK */
                else
                {
                  return "Success";
                }
            }
            /* 15.Step R3.b  If Amount in Currency of Clearance was NOT provided in the file */
            else
            {
                /* Determine ‘Applicable Amount in Currency of Clearance’ by dividing the Invoice Amount by the Exchange Rate (round to 3 decimals) */
                // miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate;
                updatedExRate = exchangeRateFromFDRMaster;
                updatedClearanceAmt = miscUatpInvoice.InvoiceSummary.TotalAmount / updatedExRate;
                return "Success";
            }
        }
    }

    #endregion

      /// <summary>
      /// Validates the exchange rate for smi X.
      /// </summary>
      /// <param name="miscUatpInvoice">The misc uatp invoice.</param>
      /// <param name="exceptionDetailsList">The exception details list.</param>
      /// <param name="fileName">Name of the file.</param>
      /// <param name="fileSubmissionDate">The file submission date.</param>
      /// <returns></returns>
      public bool ValidateExchangeRateForSmiX(MiscUatpInvoice miscUatpInvoice, IList<IsValidationExceptionDetail> exceptionDetailsList, string fileName, DateTime fileSubmissionDate)
      {
        bool isValid = true;
        /* CMP #624: Validate exchange rate for SMi X */
        if (miscUatpInvoice.SettlementMethodId == (int) SMI.IchSpecialAgreement)
        {
          if (miscUatpInvoice.ListingCurrencyId == miscUatpInvoice.BillingCurrencyId && Convert.ToDecimal(miscUatpInvoice.ExchangeRate) == 1)
          {
            // do not require any processing
          }
          else if (miscUatpInvoice.ListingCurrencyId == miscUatpInvoice.BillingCurrencyId && Convert.ToDecimal(miscUatpInvoice.ExchangeRate) != 1)
          {
            var validationExceptionDetail = CreateValidationExceptionDetail(miscUatpInvoice.Id.Value(),
                                                                            exceptionDetailsList.Count() + 1,
                                                                            fileSubmissionDate,
                                                                            miscUatpInvoice,
                                                                            "Exchange Rate",
                                                                            Convert.ToString(miscUatpInvoice.ExchangeRate),
                                                                            fileName,
                                                                            ErrorLevels.ErrorLevelInvoice,
                                                                            ErrorCodes.InvalidListingToBillingRateForSameCurrencies,
                                                                            ErrorStatus.X,
                                                                            0,
                                                                            0);
            exceptionDetailsList.Add(validationExceptionDetail);
            isValid = false;
          }
          // Update Amount in clearance currency
          if (miscUatpInvoice.InvoiceSummary != null && !miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency.HasValue && Convert.ToDecimal(miscUatpInvoice.ExchangeRate) > 0)
          {
            miscUatpInvoice.InvoiceSummary.TotalAmountInClearanceCurrency = miscUatpInvoice.InvoiceSummary.TotalAmount / miscUatpInvoice.ExchangeRate.Value;
          }

        }
        return isValid;
      }

      /// <summary>
      /// CMP#648: function will be used to validation exchange rate,clearance currency and TotalAmount.
      /// </summary>
      /// <param name="invoice"></param>
      /// <param name="errorMsg"></param>
      /// <returns></returns>
      public MiscUatpInvoice ValidateIswebMiscInvExchangeRate(MiscUatpInvoice invoice, out string errorMsg)
      {
        var currencyOfClearance = invoice.BillingCurrencyId;
        var currencyOfBilling = invoice.ListingCurrencyId;
        var exchangeRate = invoice.ListingToBillingRate;
        errorMsg = string.Empty;
        //Table 2: Permissible combination of Clearance Information fields and related validations
        //1. currencyOfClearance = No valid value selected, i.e. ‘Please Select’ is selected; exchangeRate =	Not captured (blanks/null); validation result = Pass
        if (!currencyOfClearance.HasValue && !exchangeRate.HasValue)
        {
          invoice.InvoiceSummary.TotalAmountInClearanceCurrency = null;
          invoice.ExchangeRate = invoice.ListingToBillingRate = null;
          invoice.BillingCurrencyId = null;
          errorMsg = string.Empty;
        }
        //2. currencyOfClearance = No valid value selected, i.e. ‘Please Select’ is selected; exchangeRate =	captured; validation result = Fail
        else if (!currencyOfClearance.HasValue && exchangeRate.Value >= 0)
        {
          invoice.InvoiceSummary.TotalAmountInClearanceCurrency = null;
          invoice.BillingCurrencyId = null;
          errorMsg = MiscErrorCodes.ExchangeRateCannotBeDefined;
        }

        //3. currencyOfClearance = Value is same as ‘Currency of Billing’ OR Value is different from ‘Currency of Billing’; exchangeRate =	Not captured; validation result = Pass
        else if (currencyOfClearance.HasValue && currencyOfBilling.HasValue && !exchangeRate.HasValue)
        {
          invoice.InvoiceSummary.TotalAmountInClearanceCurrency = null;
          invoice.ExchangeRate = invoice.ListingToBillingRate = null;
          errorMsg = string.Empty;
        }

        //4. currencyOfClearance = Value is same as ‘Currency of Billing’; exchangeRate = Value is captured, but is other than 1.00000 (or equivalent of 1); validation result = Fail
        else if (currencyOfClearance.HasValue && currencyOfBilling.HasValue &&
            currencyOfClearance.Value == currencyOfBilling.Value)
        {
          if (exchangeRate.Value != 1)
          {
              invoice.InvoiceSummary.TotalAmountInClearanceCurrency = null;
              errorMsg = MiscErrorCodes.ExchangeRateShouldBeEqualTo1;
          }
        }

        //6. currencyOfClearance = Value is different as ‘Currency of Billing’; exchangeRate = An invalid value is captured; validation result = Fail
        else if (currencyOfClearance.HasValue && currencyOfBilling.HasValue &&
            currencyOfClearance.Value != currencyOfBilling.Value)
        {
          if (exchangeRate.Value <= 0 || !Regex.IsMatch(exchangeRate.Value.ToString(), @"^[0-9]\d{0,11}(\.\d{0,5})?%?$"))
          {
            invoice.InvoiceSummary.TotalAmountInClearanceCurrency = null;
            errorMsg = MiscErrorCodes.InvalidExchangeRateSupplied;
          }
        }

        //7. currencyOfClearance = Value is different as ‘Currency of Billing’; exchangeRate =	An invalid value is captured; validation result = Fail
        else if (currencyOfClearance.HasValue && currencyOfBilling.HasValue &&
            currencyOfClearance.Value != currencyOfBilling.Value)
        {
          errorMsg = string.Empty;
        }

        return invoice;
      }

      /// <summary>
      /// SCP#417067: Validations for Notes and Legal text
      /// </summary>
      /// <param name="invoice">MiscUatp Invoice</param>
      /// <param name="errorMsg">Error Message out parameter</param>
      /// <returns> Eerror code</returns>
      public MiscUatpInvoice ValidateIswebMiscInvHeaderNoteDescription(MiscUatpInvoice invoice, out string errorMsg)
      {
          errorMsg = string.Empty;

          if (invoice.AdditionalDetails.Where(additionalDetail => additionalDetail.AdditionalDetailType == AdditionalDetailType.Note && additionalDetail.Description.Length > 500).Any())
          {
              errorMsg = MiscErrorCodes.MaxCharLimitExceedsForNoteDescription;
          }
          return invoice;
      }
    
      /// <summary>
      /// CMP #678: Time Limit Validation on Last Stage MISC Rejections
      /// </summary>
      /// <param name="yourInvoice">Invoice being rejected</param>
      /// <param name="rejectionInvoice">Invoice will create on rejected invoice</param>
      /// <param name="fileName">File Name</param>
      /// <param name="fileSubmissionDate">File Submission Date</param>
      /// <param name="exceptionDetailsList">Exception Detail List</param>
      /// <returns></returns>
      public string ValidateMiscLastStageRmForTimeLimit(MiscUatpInvoice yourInvoice, MiscUatpInvoice rejectionInvoice, RmValidationType validationType, string fileName = null, DateTime? fileSubmissionDate = null, IList<IsValidationExceptionDetail> exceptionDetailsList = null)
      {
          Logger.Info("Time Limit validation Start");

          bool isValid = false;
          int settlementMethodId = 0;
          var transactionType = TransactionType.MiscRejection1;
          string errorDescription = string.Empty;
          DateTime yourBillingDate = new DateTime();
          DateTime invoiceBillingDate = new DateTime();
          const string billingDateFormat = "yyyyMMdd";


          /*
           * When the Rejection Invoice being processed/validated is billed using SMI “I” or “X”  ‘Settlement Method’ “I” should be matched in the master
             When the Rejection Invoice being processed/validated is billed using SMI "M"  'Settlement Method' "M" should be matched in the master
             When the Rejection Invoice being processed/validated is billed using a Bilateral SMI (other than "X")  'Settlement Method' "B" should be matched in the master
             When the Rejection Invoice being processed/validated is billed using SMI “A”  ‘Settlement Method’ “A” should be matched in the master
           */

          if (rejectionInvoice != null)
          {
              if (rejectionInvoice.SettlementMethodId == (int) SMI.Ach && rejectionInvoice.RejectionStage == 1)
              {
                  return string.Empty;
              }

              if (rejectionInvoice.SettlementMethodId == (int) SMI.Ich ||
                  rejectionInvoice.SettlementMethodId == (int) SMI.IchSpecialAgreement)
              {
                  settlementMethodId = (int) SMI.Ich;
              }
              else if (rejectionInvoice.SettlementMethodId == (int)SMI.AchUsingIataRules)
              {
                  settlementMethodId = (int) SMI.AchUsingIataRules;
              }
              else if (ReferenceManager.IsSmiLikeBilateral(rejectionInvoice.SettlementMethodId, false))
              {
                  settlementMethodId = (int)SMI.Bilateral;
              }
              else if (rejectionInvoice.SettlementMethodId == (int)SMI.Ach)
              {
                  settlementMethodId = (int)SMI.Ach;
                  transactionType = TransactionType.MiscRejection2;
              }
          }

          switch (validationType)
          {
              case RmValidationType.BillingHistory:
              case RmValidationType.IsWebPayableInvoice:
                  {
                      if (!GetLastRmBillingPeriod(yourInvoice.BillingYear, yourInvoice.BillingMonth,
                                                  yourInvoice.BillingPeriod, billingDateFormat,
                                                  out yourBillingDate))
                          return string.Empty;



                      int expectedSmi;
                      ClearingHouse expectedClearingHouse;
                      GetExpectedSmiAndClearingHouseForMiscLastStageRm(yourInvoice, out expectedSmi,
                                                                       out expectedClearingHouse);
                      Logger.InfoFormat("Expected SMI for Time Limit [{0}]", expectedSmi);
                      Logger.InfoFormat("Expected CH for Time Limit [{0}]", expectedClearingHouse);

                      if (expectedSmi == (int)SMI.Ach && yourInvoice.RejectionStage < 1)
                      {
                          return string.Empty;
                      }

                      if (expectedSmi == (int)SMI.Ach && yourInvoice.RejectionStage == 1)
                      {
                          transactionType = TransactionType.MiscRejection2;
                      }

                      settlementMethodId = expectedSmi;
                      var currBillingPeriod =
                          CalendarManager.GetCurrentPeriodIfOpenOrNextAsCurrent(expectedClearingHouse);

                      if (!GetLastRmBillingPeriod(currBillingPeriod.Year, currBillingPeriod.Month,
                                                  currBillingPeriod.Period, billingDateFormat,
                                                  out invoiceBillingDate))
                          return string.Empty;

                  }

                  break;

              default:
                  if (yourInvoice != null && rejectionInvoice != null)
                  {
                      if (!GetLastRmBillingPeriod(yourInvoice.BillingYear, yourInvoice.BillingMonth,
                                                  yourInvoice.BillingPeriod, billingDateFormat,
                                                  out yourBillingDate))
                          return string.Empty;

                      if (!GetLastRmBillingPeriod(rejectionInvoice.BillingYear, rejectionInvoice.BillingMonth,
                                                  rejectionInvoice.BillingPeriod, billingDateFormat,
                                                  out invoiceBillingDate))
                          return string.Empty;
                  }
                  break;
          }

          if (invoiceBillingDate != new DateTime() && yourBillingDate != new DateTime())
          {
              string billingPeriodyymmpp = string.Empty;
              string billingPeriodyyyyMmmPx = string.Empty;

              var currentBillingPeriodyymmpp = string.Format("{0}{1}{2}",
                                                             invoiceBillingDate.Year.ToString("0000").Substring(
                                                                 2, 2),
                                                             invoiceBillingDate.Month.ToString("00"),
                                                             invoiceBillingDate.Day.ToString("00"));

              var timeLimit = GetTransactionTimeLimitForMiscLastStageRm(transactionType, settlementMethodId,
                                                                        yourBillingDate);

              if (timeLimit != null)
              {
                  var invoiceBillingPeriod = new BillingPeriod(invoiceBillingDate.Year, invoiceBillingDate.Month,
                                                               invoiceBillingDate.Day);

                  var startDate = new DateTime(yourBillingDate.Year, yourBillingDate.Month, 1);
                  var endDate = startDate.AddMonths(timeLimit.Limit);

                  var endBillingPeriod = new BillingPeriod(endDate.Year, endDate.Month, 4);

                  billingPeriodyymmpp = string.Format("{0}{1}04", endDate.ToString("yy"),
                                                      endDate.Month.ToString("00"));

                  billingPeriodyyyyMmmPx = string.Format("{1} {0} P4", endDate.Year.ToString("0000"),
                                                         endDate.ToString("MMM"));

                  isValid = (invoiceBillingPeriod <= endBillingPeriod);

                  if (!isValid)
                  {
                      /*Validation Failed*/

                      switch (validationType)
                      {
                          case RmValidationType.IsWebStandAlone:
                              {
                                  //Unable to save Rejection Invoice as it has been billed beyond the Applicable Time Limit - <Mmm YYYY Px> 
                                  errorDescription =
                                      Messages.ResourceManager.GetString(
                                          MiscUatpErrorCodes.RejInvTimeLimitValidationFailed);
                                  if (!string.IsNullOrEmpty(errorDescription))
                                  {
                                      errorDescription = string.Format(errorDescription, billingPeriodyyyyMmmPx);
                                      return errorDescription;
                                  }
                              }
                              break;
                          case RmValidationType.ErrorCorrectionScreen:
                              {
                                  //Unable to perform linking error correction as this Rejection Invoice has been billed beyond the Applicable Time Limit - <Mmm YYYY Px>.
                                  errorDescription =
                                      Messages.ResourceManager.GetString(
                                          MiscUatpErrorCodes.ErrorCorrectionRejInvTimeLimitValidationFailed);
                                  if (!string.IsNullOrEmpty(errorDescription))
                                  {
                                      errorDescription = string.Format(errorDescription, billingPeriodyyyyMmmPx);
                                      return errorDescription;
                                  }
                              }
                              break;

                          case RmValidationType.BillingHistory:
                          case RmValidationType.IsWebPayableInvoice:
                              {
                                  //Unable to initiate rejection as the Applicable Time Limit - <Mmm YYYY Px> has passed. In case Late Submission is open for period <Mmm YYYY Px>, the last stage rejection invoice against this invoice may be captured from an invoice of that Billing Period.
                                  errorDescription =
                                      Messages.ResourceManager.GetString(
                                          MiscUatpErrorCodes.BillingHistoryRejInvTimeLimitValidationFailed);
                                  if (!string.IsNullOrEmpty(errorDescription))
                                  {
                                      errorDescription = string.Format(errorDescription, billingPeriodyyyyMmmPx);
                                      return errorDescription;
                                  }
                              }
                              break;
                          case RmValidationType.InputFile:
                              {
                                  //Unable to initiate rejection as the Applicable Time Limit - <Mmm YYYY Px> has passed. In case Late Submission is open for period <Mmm YYYY Px>, the last stage rejection invoice against this invoice may be captured from an invoice of that Billing Period.
                                  errorDescription =
                                      Messages.ResourceManager.GetString(
                                          MiscUatpErrorCodes.ParsedRejInvTimeLimitValidationFailed);
                                  if (!string.IsNullOrEmpty(errorDescription))
                                  {
                                      errorDescription = string.Format(errorDescription, billingPeriodyymmpp);
                                      var validationExceptionDetail =
                                          IsValidationExceptionMiscLastStageRmDetail(rejectionInvoice.Id.Value(),
                                                                                     exceptionDetailsList.Count() +
                                                                                     1,
                                                                                     fileSubmissionDate.Value,
                                                                                     rejectionInvoice,
                                                                                     fileName,
                                                                                     "Settlement Month Period",
                                                                                     currentBillingPeriodyymmpp,
                                                                                     "Invoice",
                                                                                     MiscUatpErrorCodes.
                                                                                         ParsedRejInvTimeLimitValidationFailed,
                                                                                     errorDescription
                                              );
                                      exceptionDetailsList.Add(validationExceptionDetail);
                                      return errorDescription;
                                  }
                              }
                              break;
                      }
                  }
              }
              else
              {
                  /*Time Limit Record Not Found*/
                  switch (validationType)
                  {
                      case RmValidationType.IsWebStandAlone:
                          {
                              //Unable to save Rejection Invoice as the system failed to retrieve Time Limit data from the master while calculating the Applicable Time Limit. Please contact SIS Help Desk for resolution.
                              errorDescription =
                                  Messages.ResourceManager.GetString(
                                      MiscUatpErrorCodes.RejInvTimeLimitRecNotFound);
                              if (!string.IsNullOrEmpty(errorDescription))
                              {
                                  errorDescription = string.Format(errorDescription, billingPeriodyyyyMmmPx);
                                  return errorDescription;
                              }
                          }
                          break;

                      case RmValidationType.ErrorCorrectionScreen:
                          {
                              // Unable to perform linking error correction as the system failed to retrieve Time Limit data from the master while calculating Applicable Time Limit. Please contact SIS Help Desk for resolution.
                              errorDescription =
                                  Messages.ResourceManager.GetString(
                                      MiscUatpErrorCodes.ErrorCorrectionRejInvTimeLimitRecNotFound);
                              if (!string.IsNullOrEmpty(errorDescription))
                              {
                                  errorDescription = string.Format(errorDescription, billingPeriodyyyyMmmPx);
                                  return errorDescription;
                              }
                          }
                          break;

                      case RmValidationType.BillingHistory:
                      case RmValidationType.IsWebPayableInvoice:
                          {
                              //Unable to initiate rejection as the system failed to retrieve Time Limit data from the master while calculating Applicable Time Limit. Please contact SIS Help Desk for resolution.
                              errorDescription =
                                  Messages.ResourceManager.GetString(
                                      MiscUatpErrorCodes.BillingHistoryRejInvTimeLimitRecNotFound);
                              if (!string.IsNullOrEmpty(errorDescription))
                              {
                                  errorDescription = string.Format(errorDescription, billingPeriodyyyyMmmPx);
                                  return errorDescription;
                              }
                          }
                          break;
                      case RmValidationType.InputFile:
                          {
                              //The system failed to retrieve Time Limit data from the master while calculating Applicable Time Limit. Please contact SIS Help Desk for resolution.
                              errorDescription =
                                  Messages.ResourceManager.GetString(
                                      MiscUatpErrorCodes.ParsedRejInvTimeLimitRecNotFound);
                              if (!string.IsNullOrEmpty(errorDescription))
                              {
                                  errorDescription = string.Format(errorDescription, billingPeriodyymmpp);
                                  var validationExceptionDetail =
                                      IsValidationExceptionMiscLastStageRmDetail(rejectionInvoice.Id.Value(),
                                                                                 exceptionDetailsList.Count() + 1,
                                                                                 fileSubmissionDate.Value,
                                                                                 rejectionInvoice,
                                                                                 fileName,
                                                                                 "Settlement Month Period",
                                                                                 currentBillingPeriodyymmpp,
                                                                                 "Invoice",
                                                                                 MiscUatpErrorCodes.
                                                                                     ParsedRejInvTimeLimitRecNotFound,
                                                                                 errorDescription
                                          );
                                  exceptionDetailsList.Add(validationExceptionDetail);
                                  return errorDescription;
                              }
                          }
                          break;
                  }
              }
          }
          return string.Empty;
      }

        /// <summary>
      /// CMP#678: Time Limit Validation on Last Stage MISC Rejections
      /// </summary>
      /// <param name="billingYear">Billing Period Info</param>
      /// <param name="billingMonth">Billing Period Info</param>
      /// <param name="billingPeriod">Billing Period Info</param>
      /// <param name="dateFormat">Date Format</param>
      /// <param name="billingDate">Billing Date</param>
      /// <returns></returns>
        private bool GetLastRmBillingPeriod(int billingYear, int billingMonth, int billingPeriod, string dateFormat , out DateTime billingDate)
        {
             var cultureInfo = new CultureInfo("en-US");
              cultureInfo.Calendar.TwoDigitYearMax = 2099;
           return DateTime.TryParseExact(
                string.Format("{0}{1}{2}",
                              billingYear.ToString("0000"),
                              billingMonth.ToString("00"),
                              billingPeriod.ToString("00")),
                dateFormat,
                cultureInfo,
                DateTimeStyles.None,
                out billingDate);
        }

        /// <summary>
        /// CMP #678: Time Limit Validation on Last Stage MISC Rejections
        /// </summary>
        /// <param name="transactionType">Transaction Type</param>
        /// <param name="settlementMethodId">Settlement</param>
        /// <param name="rejectedInvBillingPeriod">Rejected Invoice Billing Period</param>
        /// <returns></returns>
        private TimeLimit GetTransactionTimeLimitForMiscLastStageRm(Model.Enums.TransactionType transactionType, int settlementMethodId, DateTime rejectedInvBillingPeriod)
        {
            if (settlementMethodId == (int) SMI.IchSpecialAgreement)
            {
                settlementMethodId = (int) SMI.Ich;
            }
            if (ReferenceManager.IsSmiLikeBilateral(settlementMethodId, false))
            {
                settlementMethodId = (int) SMI.Bilateral;
            }

            var timeLimits =
                TimeLimitRepository.Get(
                    rec =>
                    rec.IsActive && rec.TransactionTypeId == (int) transactionType &&
                    rec.SettlementMethodId == settlementMethodId && rejectedInvBillingPeriod >= rec.EffectiveFromPeriod &&
                    rejectedInvBillingPeriod <= rec.EffectiveToPeriod).ToList();

            if (timeLimits.Count > 1)
            {
                return null;
            }

            return timeLimits.FirstOrDefault();

        }

      /// <summary>
      /// Gets the expected smi and clearing house for stage three rm.
      /// </summary>
      /// <param name="yourInvoice">Rejected invoice.</param>
      /// <param name="expectedSmi">The expected smi.</param>
      /// <param name="expectedClearingHouse">The expected clearing house.</param>
      public void GetExpectedSmiAndClearingHouseForMiscLastStageRm(MiscUatpInvoice yourInvoice, out int expectedSmi, out ClearingHouse expectedClearingHouse)
      {
          var billingMember = yourInvoice.BilledMember;
          if (billingMember == null)
          {
              billingMember = MemberManager.GetMember(yourInvoice.BilledMemberId);
          }
          expectedSmi = yourInvoice.SettlementMethodId;
          expectedClearingHouse = ClearingHouse.Ich;
          // 1. A	Any	Any	A	ACH
          if (yourInvoice.SettlementMethodId == (int)SMI.Ach)
          {
              expectedSmi = (int)SMI.Ach;
              expectedClearingHouse = ClearingHouse.Ach;
          }
          //2.	I	‘Live’or ‘Suspended’	Any	I	ICH
          else if (yourInvoice.SettlementMethodId == (int)SMI.Ich && (billingMember.IchMemberStatusId == (int)IchMemberShipStatus.Live || billingMember.IchMemberStatusId == (int)IchMemberShipStatus.Suspended))
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
          else if (ReferenceManager.IsSmiLikeBilateral(yourInvoice.SettlementMethodId, false))
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
    }
}
