
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Cargo.Payables;
using Iata.IS.Model.SupportingDocuments;
using Iata.IS.Model.Enums;
using System;
using Iata.IS.Model.Cargo.Common;

namespace Iata.IS.Data.SupportingDocuments.Impl
{
  /// <summary>
  /// Repository for supporting document related data processing
  /// </summary>
    public class CargoSupportingDocumentRepository : Repository<UnlinkedSupportingDocument>, ICargoSupportingDocumentRepository
    {

        #region Public Methods
        #region "OldCode"
        /*
        /// <summary>
      /// Get the records for given search criteria.
      /// </summary>
      /// <param name="recordSearchCriteria">The record search criteria.</param>
      /// <returns></returns>
        public List<SupportingDocumentRecord> GetSupportingDocumentRecords(RecordSearchCriteria recordSearchCriteria)
        {
            var parameters = new ObjectParameter[10];

            parameters[0] = new ObjectParameter(SupportingDocumentsConstants.BillingMemberId, typeof(int)) { Value = recordSearchCriteria.BillingMemberId };
            parameters[1] = new ObjectParameter(SupportingDocumentsConstants.BillingMonth, typeof(int)) { Value = recordSearchCriteria.ClearanceMonth };
            parameters[2] = new ObjectParameter(SupportingDocumentsConstants.BillingPeriod, typeof(int)) { Value = recordSearchCriteria.ClearancePeriod };
            parameters[3] = new ObjectParameter(SupportingDocumentsConstants.BilledMemberId, typeof(int)) { Value = recordSearchCriteria.BilledMemberId };
            parameters[5] = new ObjectParameter(SupportingDocumentsConstants.InvoiceNumber, typeof(string)) { Value = recordSearchCriteria.InvoiceNumber };
            parameters[4] = new ObjectParameter(SupportingDocumentsConstants.BillingCategory, typeof(int)) { Value = recordSearchCriteria.BillingCategory };
            
            parameters[6] = new ObjectParameter(SupportingDocumentsConstants.BatchSequenceNumber, typeof(int)) { Value = recordSearchCriteria.BatchNumber };
            parameters[7] = new ObjectParameter(SupportingDocumentsConstants.BatchRecordSequenceNumber, typeof(int)) { Value = recordSearchCriteria.SequenceNumber };
            if (recordSearchCriteria.BreakdownSerialNumber == null)
            {
                parameters[8] = new ObjectParameter(SupportingDocumentsConstants.BreakdownSerialNumber, 0);
            }
            else
            {
                parameters[8] = new ObjectParameter(SupportingDocumentsConstants.BreakdownSerialNumber, typeof(int)) { Value = recordSearchCriteria.BreakdownSerialNumber };
            }
            parameters[9] = new ObjectParameter(SupportingDocumentsConstants.ChargeCategory, typeof(int)) { Value = recordSearchCriteria.ChargeCategoryId };

            var recordDetails = ExecuteStoredFunction<SupportingDocumentRecord>(SupportingDocumentsConstants.GetRecordsFunctionName, parameters);

            return recordDetails.ToList();
        }

        /// <summary>
        /// Gets the unlinked supporting documents.
        /// </summary>
        /// <param name="recordSearchCriteria">The record search criteria.</param>
        /// <returns></returns>
        public List<UnlinkedSupportingDocumentEx> GetUnlinkedSupportingDocuments(RecordSearchCriteria recordSearchCriteria)
        {
            var parameters = new ObjectParameter[12];

            parameters[0] = new ObjectParameter(SupportingDocumentsConstants.BillingMemberUnlinked, typeof(int)) { Value = recordSearchCriteria.BillingMemberId };
            parameters[1] = new ObjectParameter(SupportingDocumentsConstants.BillingMonthUnlinked, typeof(int?)) { Value = recordSearchCriteria.ClearanceMonth };
            parameters[2] = new ObjectParameter(SupportingDocumentsConstants.BillingPeriodUnlinked, typeof(int?)) { Value = recordSearchCriteria.ClearancePeriod };
            parameters[3] = new ObjectParameter(SupportingDocumentsConstants.BilledMemberUnlinked, typeof(int?)) { Value = recordSearchCriteria.BilledMemberId };
            parameters[4] = new ObjectParameter(SupportingDocumentsConstants.BillingCategoryUnlinked, typeof(int?)) { Value = recordSearchCriteria.BillingCategory };
            parameters[5] = new ObjectParameter(SupportingDocumentsConstants.InvoiceNumberUnlinked, typeof(string)) { Value = recordSearchCriteria.InvoiceNumber };
            parameters[6] = new ObjectParameter(SupportingDocumentsConstants.BatchSequenceNumberUnlinked, typeof(int?)) { Value = recordSearchCriteria.BatchNumber };
            parameters[7] = new ObjectParameter(SupportingDocumentsConstants.RecordSequenceNumberUnlinked, typeof(int?)) { Value = recordSearchCriteria.SequenceNumber };
            parameters[8] = new ObjectParameter(SupportingDocumentsConstants.BreakdownSerialNumberUnlinked, typeof(int?)) { Value = recordSearchCriteria.BreakdownSerialNumber };
            parameters[9] = new ObjectParameter(SupportingDocumentsConstants.BillingYearUnlinked, typeof(int?)) { Value = recordSearchCriteria.BillingYear };
            parameters[10] = new ObjectParameter(SupportingDocumentsConstants.SubmissionDate, typeof(DateTime?)) { Value = recordSearchCriteria.SubmissionDate };
            parameters[11] = new ObjectParameter(SupportingDocumentsConstants.OriginalFileName, typeof(string)) { Value = recordSearchCriteria.OriginalFileName };

            var recordDetails = ExecuteStoredFunction<UnlinkedSupportingDocumentEx>(SupportingDocumentsConstants.GetUnlinkedSupportingDocumentsFunctionName, parameters);

            return recordDetails.ToList();

        }

        /// <summary>
        /// Updates the number of attachments field for the given record.
        /// </summary>
        /// <param name="recordId">The record id.</param>
        /// <param name="recordType">Type of the record.</param>
        public void UpdateNumberOfAttachments(Guid recordId, RecordType recordType)
        {
            var parameters = new ObjectParameter[2];

            parameters[0] = new ObjectParameter(SupportingDocumentsConstants.RecordTypeId, typeof(int))
            {
                Value = (int)recordType
            };
            parameters[1] = new ObjectParameter(SupportingDocumentsConstants.RecordId, typeof(Guid))
            {
                Value = recordId
            };

            ExecuteStoredProcedure(SupportingDocumentsConstants.UpdateNumberOfAttachmentsFunctionName, parameters);
        }

        /// <summary>
        /// Gets the supporting document search result.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>

        public IList<SupportingDocSearchResult> GetSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria)
        {
            var parameters = new ObjectParameter[16];

            parameters[0] = new ObjectParameter(SupportingDocumentsConstants.BillingYearParameterName, typeof(int)) { Value = criteria.BillingYear };
            parameters[1] = new ObjectParameter(SupportingDocumentsConstants.BillingMonthParameterName, typeof(int)) { Value = criteria.BillingMonth };
            parameters[2] = new ObjectParameter(SupportingDocumentsConstants.BillingPeriodParameterName, typeof(int)) { Value = criteria.BillingPeriod };
            parameters[3] = new ObjectParameter(SupportingDocumentsConstants.BillingMemberIdParameterName, typeof(int)) { Value = criteria.BillingMemberId };
            parameters[4] = new ObjectParameter(SupportingDocumentsConstants.BilledMemberIdParameterName, typeof(int)) { Value = criteria.BilledMemberId };
            parameters[5] = new ObjectParameter(SupportingDocumentsConstants.InvoiceTypeParameterName, typeof(int)) { Value = criteria.SupportingDocumentTypeId };
            parameters[6] = new ObjectParameter(SupportingDocumentsConstants.InvoiceNoParameterName, typeof(string)) { Value = criteria.InvoiceNumber };
            parameters[7] = new ObjectParameter(SupportingDocumentsConstants.SourceCodeParameterName, typeof(int?)) { Value = criteria.SourceCodeId };
            parameters[8] = new ObjectParameter(SupportingDocumentsConstants.BatchNoParameterName, typeof(int?)) { Value = criteria.BatchSequenceNumber };

            parameters[9] = new ObjectParameter(SupportingDocumentsConstants.SequenceNoParameterName, typeof(int?)) { Value = criteria.RecordSequenceWithinBatch };
            parameters[10] = new ObjectParameter(SupportingDocumentsConstants.RMBMCMNumberParameterName, typeof(string)) { Value = criteria.RMBMCMNumber };
            parameters[11] = new ObjectParameter(SupportingDocumentsConstants.DocNoParameterName, typeof(long?)) { Value = criteria.TicketDocNumber };
            parameters[12] = new ObjectParameter(SupportingDocumentsConstants.CouponNoParameterName, typeof(int?)) { Value = criteria.CouponNumber };
            parameters[13] = new ObjectParameter(SupportingDocumentsConstants.AttachmentIndOrigParameterName, typeof(int)) { Value = criteria.AttachmentIndicatorOriginal };
            parameters[14] = new ObjectParameter(SupportingDocumentsConstants.IsMismatchCaseParameterName, typeof(int)) { Value = criteria.IsMismatchCases == true ? 1 : 0 };
            parameters[15] = new ObjectParameter(SupportingDocumentsConstants.CutOffEventNameParameterName, typeof(string)) { Value = criteria.CutOffDateEventName };

            var searchResult = ExecuteStoredFunction<SupportingDocSearchResult>(SupportingDocumentsConstants.GetSupportingDocumentSearchResultFunctionName, parameters);

            return searchResult.ToList();
        }
        */
        #endregion
        public IList<CargoSupportingDocSearchResult> GetCargoSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria)
         { 
            //03.10.11
            var parameters = new ObjectParameter[14];

            parameters[0] = new ObjectParameter(SupportingDocumentsConstants.BillingYearParameterName, typeof(int)) { Value = criteria.BillingYear };
            parameters[1] = new ObjectParameter(SupportingDocumentsConstants.BillingMonthParameterName, typeof(int)) { Value = criteria.BillingMonth };
            parameters[2] = new ObjectParameter(SupportingDocumentsConstants.BillingPeriodParameterName, typeof(int)) { Value = criteria.BillingPeriod };
            parameters[3] = new ObjectParameter(SupportingDocumentsConstants.BillingMemberIdParameterName, typeof(int)) { Value = criteria.BillingMemberId };
            parameters[4] = new ObjectParameter(SupportingDocumentsConstants.BilledMemberIdParameterName, typeof(int)) { Value = criteria.BilledMemberId };
            parameters[5] = new ObjectParameter(SupportingDocumentsConstants.InvoiceTypeParameterName, typeof(int)) { Value = criteria.SupportingDocumentTypeId };
            parameters[6] = new ObjectParameter(SupportingDocumentsConstants.InvoiceNoParameterName, typeof(string)) { Value = criteria.InvoiceNumber };
            parameters[7] = new ObjectParameter(SupportingDocumentsConstants.CGOBillingCodeParameterName, typeof(int)) { Value = criteria.BillingCode };
            parameters[8] = new ObjectParameter(SupportingDocumentsConstants.BatchNoParameterName, typeof(int?)) { Value = criteria.BatchSequenceNumber };

            parameters[9] = new ObjectParameter(SupportingDocumentsConstants.SequenceNoParameterName, typeof(int?)) { Value = criteria.RecordSequenceWithinBatch };
            parameters[10] = new ObjectParameter(SupportingDocumentsConstants.RMBMCMNumberParameterName, typeof(string)) { Value = criteria.RMBMCMNumber };

            parameters[11] = new ObjectParameter(SupportingDocumentsConstants.CGOAWBSerialNumberParameterName, typeof(int)) { Value = criteria.AWBSerialNumber };
           // parameters[12] = new ObjectParameter(SupportingDocumentsConstants.CouponNoParameterName, typeof(int?)) { Value = criteria.CouponNumber };
            parameters[12] = new ObjectParameter(SupportingDocumentsConstants.AttachmentIndOrigParameterName, typeof(int)) { Value = criteria.AttachmentIndicatorOriginal };
            parameters[13] = new ObjectParameter(SupportingDocumentsConstants.IsMismatchCaseParameterName, typeof(int)) { Value = criteria.IsMismatchCases == true ? 1 : 0 };
            //parameters[15] = new ObjectParameter(SupportingDocumentsConstants.CutOffEventNameParameterName, typeof(string)) { Value = criteria.CutOffDateEventName };

            var searchResult = ExecuteStoredFunction<CargoSupportingDocSearchResult>(SupportingDocumentsConstants.GetCargoSupportingDocumentSearchResultFunctionName, parameters);

            return searchResult.ToList();
        }

      /*  public IList<PayableSupportingDocSearchResult> GetPayableSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria)
        {
          var parameters = new ObjectParameter[15];

          parameters[0] = new ObjectParameter(SupportingDocumentsConstants.PayableBillingYearParameterName, typeof(int)) { Value = criteria.BillingYear };
          parameters[1] = new ObjectParameter(SupportingDocumentsConstants.PayableBillingMonthParameterName, typeof(int)) { Value = criteria.BillingMonth };
          parameters[2] = new ObjectParameter(SupportingDocumentsConstants.PayableBillingPeriodParameterName, typeof(int)) { Value = criteria.BillingPeriod };
          parameters[3] = new ObjectParameter(SupportingDocumentsConstants.PayableBillingMemberIdParameterName, typeof(int)) { Value = criteria.BillingMemberId };
          parameters[4] = new ObjectParameter(SupportingDocumentsConstants.PayableBilledMemberIdParameterName, typeof(int)) { Value = criteria.BilledMemberId };
          parameters[5] = new ObjectParameter(SupportingDocumentsConstants.PayableInvoiceTypeParameterName, typeof(int)) { Value = criteria.SupportingDocumentTypeId };
          parameters[6] = new ObjectParameter(SupportingDocumentsConstants.PayableInvoiceNoParameterName, typeof(string)) { Value = criteria.InvoiceNumber };
          parameters[7] = new ObjectParameter(SupportingDocumentsConstants.PayableSourceCodeParameterName, typeof(int?)) { Value = criteria.SourceCodeId };
          parameters[8] = new ObjectParameter(SupportingDocumentsConstants.PayableBatchNoParameterName, typeof(int?)) { Value = criteria.BatchSequenceNumber };

          parameters[9] = new ObjectParameter(SupportingDocumentsConstants.PayableSequenceNoParameterName, typeof(int?)) { Value = criteria.RecordSequenceWithinBatch };
          parameters[10] = new ObjectParameter(SupportingDocumentsConstants.PayableRMBMCMNumberParameterName, typeof(string)) { Value = criteria.RMBMCMNumber };
          parameters[11] = new ObjectParameter(SupportingDocumentsConstants.PayableDocNoParameterName, typeof(long?)) { Value = criteria.TicketDocNumber };
          parameters[12] = new ObjectParameter(SupportingDocumentsConstants.PayableCouponNoParameterName, typeof(int?)) { Value = criteria.CouponNumber };
          parameters[13] = new ObjectParameter(SupportingDocumentsConstants.PayableAttachmentIndValidParameterName, typeof(int)) { Value = criteria.AttachmentIndicatorValidated };
          parameters[14] = new ObjectParameter(SupportingDocumentsConstants.PayableChargeCodeParameterName, typeof(int)) { Value = criteria.ChargeCategoryId };

          var searchResult = ExecuteStoredFunction<PayableSupportingDocSearchResult>(SupportingDocumentsConstants.GetPayableSupportingDocumentSearchResultFunctionName, parameters);

          return searchResult.ToList();
        }
        */
        #endregion



        public IList<CargoPayableSupportingDocSearchResult> GetCargoPayableSupportingDocumentSearchResult(SupportingDocSearchCriteria criteria)
        {
            var parameters = new ObjectParameter[13];
           
            parameters[0] = new ObjectParameter(SupportingDocumentsConstants.CgoPayableBillingYearParameterName, typeof(int)) { Value = criteria.BillingYear };
            parameters[1] = new ObjectParameter(SupportingDocumentsConstants.CgoPayableBillingMonthParameterName, typeof(int)) { Value = criteria.BillingMonth };
            parameters[2] = new ObjectParameter(SupportingDocumentsConstants.CgoPayableBillingPeriodParameterName, typeof(int)) { Value = criteria.BillingPeriod };
            parameters[3] = new ObjectParameter(SupportingDocumentsConstants.CgoPayableBilledMemberIdParameterName, typeof(int)) { Value = criteria.BilledMemberId };
            parameters[4] = new ObjectParameter(SupportingDocumentsConstants.CgoPayableBillingMemberIdParameterName, typeof(int)) { Value = criteria.BillingMemberId };
            parameters[5] = new ObjectParameter(SupportingDocumentsConstants.CgoPayableInvoiceTypeParameterName, typeof(int)) { Value = criteria.SupportingDocumentTypeId };
            parameters[6] = new ObjectParameter(SupportingDocumentsConstants.CgoPayableInvoiceNoParameterName, typeof(string)) { Value = criteria.InvoiceNumber };

            parameters[7] = new ObjectParameter(SupportingDocumentsConstants.CgoPayableBatchNoParameterName, typeof(int?)) { Value = criteria.BatchSequenceNumber };
            parameters[8] = new ObjectParameter(SupportingDocumentsConstants.CgoPayableSequenceNoParameterName, typeof(int?)) { Value = criteria.RecordSequenceWithinBatch };
            parameters[9] = new ObjectParameter(SupportingDocumentsConstants.CgoPayableRMBMCMNumberParameterName, typeof(string)) { Value = criteria.RMBMCMNumber };
            parameters[10] = new ObjectParameter(SupportingDocumentsConstants.CgoPayableBillingCodeParameterName, typeof(int)) { Value = criteria.BillingCode };
            parameters[11] = new ObjectParameter(SupportingDocumentsConstants.CgoPayableAWBSerialNumberParameterName, typeof(int)) { Value = criteria.AWBSerialNumber };
            parameters[12] = new ObjectParameter(SupportingDocumentsConstants.CgoPayableAttachmentIndOriginParameterName, typeof(int)) { Value = criteria.AttachmentIndicatorValidated };
            
            var searchResult = ExecuteStoredFunction<CargoPayableSupportingDocSearchResult>(SupportingDocumentsConstants.GetCargoPayableSupportingDocumentSearchResultFunctionName, parameters);
            
            return searchResult.ToList();
        }
    }
}
