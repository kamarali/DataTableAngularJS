using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using Devart.Data.Oracle;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MemberProfile.Impl;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Pax.Enums;
using Iata.IS.Model.Pax.Sampling;
using Microsoft.Data.Extensions;
using Iata.IS.Model.Common;

namespace Iata.IS.Data.Pax.Impl
{
    public class SamplingFormCRepository : Repository<SamplingFormC>, ISamplingFormCRepository
    {

        public override SamplingFormC Single(Expression<System.Func<SamplingFormC, bool>> where)
        {
            return EntityObjectSet.Include("ListingCurrency").Include("ProvisionalBillingMember").SingleOrDefault(where);
        }

        /// <summary>
        /// Gets the sampling form C details.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public IQueryable<SamplingFormC> GetSamplingFormCDetails(Expression<Func<SamplingFormC, bool>> where)
        {
            return EntityObjectSet.Include("SamplingFormCDetails").Include("ListingCurrency").Include("ProvisionalBillingMember").Include("FromMember").Where(where);
        }

        public IList<SamplingFormCSourceTotal> GetSamplingFormCSourceTotalList(int provisionalBillingMonth, int provisionalBillingYear, int fromMemberId, int invoiceStatusId, int provisionalBillingMemberId, int? listingCurrencyId)
        {
            var parameters = new ObjectParameter[6];

            parameters[0] = new ObjectParameter(SamplingFormCRepositoryConstants.ProvisionalBillingYearParameterName, typeof(int)) { Value = provisionalBillingYear };
            parameters[1] = new ObjectParameter(SamplingFormCRepositoryConstants.ProvisionalBillingMonthParameterName, typeof(int)) { Value = provisionalBillingMonth };
            parameters[2] = new ObjectParameter(SamplingFormCRepositoryConstants.ProvisionalBillingMemberIdParameterName, typeof(int)) { Value = provisionalBillingMemberId };
            parameters[3] = new ObjectParameter(SamplingFormCRepositoryConstants.FromMemberIdParameterName, typeof(int)) { Value = fromMemberId };
            parameters[4] = new ObjectParameter(SamplingFormCRepositoryConstants.InvoiceStatusIdParameterName, typeof(int)) { Value = invoiceStatusId };
            parameters[5] = new ObjectParameter(SamplingFormCRepositoryConstants.ListingCurrencyIdParameterName, typeof(int?)) { Value = listingCurrencyId };

            var sourceTotals = ExecuteStoredFunction<SamplingFormCSourceTotal>(SamplingFormCRepositoryConstants.GetFormCSourceTotalListFunctionName, parameters);

            return sourceTotals.ToList();
        }

        public IList<SamplingFormCResultSet> GetSamplingFormCList(int provisionalBillingMonth, int provisionalBillingYear, int fromMemberId, int invoiceStatusId, int provisionalBillingMemberId)
        {
            var parameters = new ObjectParameter[5];

            parameters[0] = new ObjectParameter(SamplingFormCRepositoryConstants.ProvisionalBillingMonthParameterName, typeof(int)) { Value = provisionalBillingMonth };
            parameters[1] = new ObjectParameter(SamplingFormCRepositoryConstants.ProvisionalBillingYearParameterName, typeof(int)) { Value = provisionalBillingYear };
            parameters[2] = new ObjectParameter(SamplingFormCRepositoryConstants.ProvisionalBillingMemberIdParameterName, typeof(int)) { Value = provisionalBillingMemberId };
            parameters[3] = new ObjectParameter(SamplingFormCRepositoryConstants.FromMemberIdParameterName, typeof(int)) { Value = fromMemberId };
            parameters[4] = new ObjectParameter(SamplingFormCRepositoryConstants.InvoiceStatusIdParameterName, typeof(int)) { Value = invoiceStatusId };

            var samplingFormCList = ExecuteStoredFunction<SamplingFormCResultSet>(SamplingFormCRepositoryConstants.GetFormCListFunctionName, parameters);

            return samplingFormCList.ToList();
        }

        /// <summary>
        /// Following method will update FormC SourceCode total value
        /// </summary>
        /// <param name="formCId">FormC ID whose SourceCode total to be updated</param>
        public void UpdateFormCSourceCodeTotal(Guid formCId)
        {
            // Define parameters
            var parameters = new ObjectParameter[1];
            // Add FormCId as parameter
            parameters[0] = new ObjectParameter(SamplingFormCRepositoryConstants.FormCIdParameterName, typeof(Guid)) { Value = formCId };
            // Execute stored procedure
            ExecuteStoredProcedure(SamplingFormCRepositoryConstants.UpdateFormCSourceCodeTotal, parameters);
        }

        public IList<SamplingFormCResultSet> GetSamplingFormCPayablesList(int provisionalBillingMonth, int provisionalBillingYear, int toMemberId, int provisionalBilledMemberId)
        {
            var parameters = new ObjectParameter[4];

            parameters[0] = new ObjectParameter(SamplingFormCRepositoryConstants.ProvisionalBillingMonthParameterName, typeof(int)) { Value = provisionalBillingMonth };
            parameters[1] = new ObjectParameter(SamplingFormCRepositoryConstants.ProvisionalBillingYearParameterName, typeof(int)) { Value = provisionalBillingYear };
            parameters[2] = new ObjectParameter(SamplingFormCRepositoryConstants.ProvisionalBilledMemberIdParameterName, typeof(int)) { Value = provisionalBilledMemberId };
            parameters[3] = new ObjectParameter(SamplingFormCRepositoryConstants.ToMemberIdParameterName, typeof(int)) { Value = toMemberId };

            var samplingFormCList = ExecuteStoredFunction<SamplingFormCResultSet>(SamplingFormCRepositoryConstants.GetFormCPayablesListFunctionName, parameters);

            return samplingFormCList.ToList();
        }

        /// <summary>
        /// Get sampling form C details
        /// </summary>
        /// <param name="provisionalBillingMonth">The Provisional billing month</param>
        /// <param name="provisionalBillingYear">The provisional billing year</param>
        /// <param name="fromMemberId">The From member id</param>
        /// <param name="invoiceStatusIds">The invoice status id</param>
        /// <param name="provisionalBillingMemberId">The provisional billing member id</param>
        /// <param name="listingCurrencyCodeNum">The Listing Currency Code Num id</param>
        /// <returns>List of samplingFormC</returns>
        public IList<SamplingFormC> GetSamplingFormCDetails(int? provisionalBillingMonth = null, int? provisionalBillingYear = null, int? fromMemberId = null, string invoiceStatusIds = null, int? provisionalBillingMemberId = null, int? listingCurrencyCodeNum = null)
        {
            var entities = new string[] 
                        { 
                            LoadStrategy.Entities.SamplingFormC, LoadStrategy.Entities.SamplingFormCDetails, 
                            LoadStrategy.Entities.SamplingFormCListingCurrency, LoadStrategy.Entities.ProvisionalBillingMember, 
                            LoadStrategy.Entities.SamplingFormCFromMember, LoadStrategy.Entities.SamplingFormCRecordAttachment
                        };
            return GetSamplingFormCLS(new LoadStrategy(string.Join(",", entities)), null,
                                      provisionalBillingMonth,
                                      provisionalBillingYear,
                                      fromMemberId,
                                      invoiceStatusIds,
                                      provisionalBillingMemberId,
                                      listingCurrencyCodeNum);
        }

        /// <summary>
        ///  Gets the Sampling form C Data having processing completed on date as input parameter
        /// </summary>
        /// <param name="processingCompletedOnDateTime">processingCompletedOnDateTime</param>
        /// <param name="provisionalBillingMemberId">provisionalBillingMemberId</param>
        /// <param name="formCAttachmentCompletedOnDateInd">formCAttachmentCompletedOnDateInd is used if u want to fetch the attachments attached in the given Processing Completed on DateTime</param>
        /// <param name="invoiceStatusId"></param>
        /// <returns>List of Sampling form C</returns>
        public IList<SamplingFormC> GetSamplingFormCDataForOutputGeneration(DateTime processingCompletedOnDateTime, int provisionalBillingMemberId, int? formCAttachmentCompletedOnDateInd = null, string invoiceStatusId = null)
        {
            var entities = new string[] 
                        { 
                            LoadStrategy.Entities.SamplingFormC, LoadStrategy.Entities.SamplingFormCDetails, 
                            LoadStrategy.Entities.SamplingFormCListingCurrency, LoadStrategy.Entities.ProvisionalBillingMember, 
                            LoadStrategy.Entities.SamplingFormCFromMember,LoadStrategy.Entities.SamplingFormCSourceCodeTotal
                        };
            return GetSamplingFormCLS(new LoadStrategy(string.Join(",", entities)), processingCompletedOnDateTime: processingCompletedOnDateTime, formCAttachmentCompletedOnDateInd: formCAttachmentCompletedOnDateInd, provisionalBillingMemberId: provisionalBillingMemberId, invoiceStatusId: invoiceStatusId);
        }

        /// <summary>
        /// Returns the list of Form Cs of provisionalBillingMember in the period 
        /// </summary>
        /// <param name="processingCompletedOnDateTime">processingCompletedOnDateTime</param>
        /// <param name="provisionalBillingMemberId">provisionalBillingMemberId</param>
        /// <returns></returns>
        public List<SamplingFormC> GetOnlySamplingFormcForOutputfileGeneration(DateTime processingCompletedOnDateTime, int provisionalBillingMemberId)
        {
            return EntityObjectSet.Where(i => i.ProcessingCompletedOn == processingCompletedOnDateTime && i.ProvisionalBillingMemberId == provisionalBillingMemberId && i.InvoiceStatusId == (int)InvoiceStatusType.ProcessingComplete).ToList();
        }

        public List<SamplingFormC> SystemMonitorGetOnlySamplingFormcForOutputfileGeneration(DateTime processingCompletedOnDateTime, int provisionalBillingMemberId)
        {
            return EntityObjectSet.Where(i => i.ProcessingCompletedOn == processingCompletedOnDateTime && i.ProvisionalBillingMemberId == provisionalBillingMemberId).ToList();
        }


        /// <summary>
        /// Updates the multiple form C status.
        /// </summary>
        /// <param name="formCIds">The form C ids.</param>
        /// <param name="invoiceStatus">The invoice status.</param>
        public void UpdateFormCInvoiceStatus(string formCIds, InvoiceStatusType invoiceStatus)
        {
            var parameters = new ObjectParameter[2];
            parameters[0] = new ObjectParameter(SamplingFormCRepositoryConstants.FormCIdsParameterName, typeof(string))
            {
                Value = formCIds
            };
            parameters[1] = new ObjectParameter(SamplingFormCRepositoryConstants.InvoiceStatusIdParameterName, typeof(int))
            {
                Value = (int)invoiceStatus
            };

            ExecuteStoredProcedure(SamplingFormCRepositoryConstants.UpdateFormCStatusFunctionName, parameters);

        }

        /// <summary>
        /// Get list of form C 
        /// </summary>
        /// <param name="processingCompletedOnDateTime">processingCompletedOnDateTime</param>
        /// <param name="formCAttachmentCompletedOnDateInd">if equal to 1 it will compare processingCompletedOnDateTime with Form C Attachment Completed On Date</param>
        /// <returns></returns>
        public List<SamplingFormC> GetSamplingFormCOfflineCollectionData(DateTime processingCompletedOnDateTime, int? formCAttachmentCompletedOnDateInd = null)
        {
            var entities = new string[] 
                        { 
                            LoadStrategy.Entities.SamplingFormC, LoadStrategy.Entities.SamplingFormCDetails, LoadStrategy.Entities.SamplingFormCRecordAttachment,
                            LoadStrategy.Entities.SamplingFormCFromMember, LoadStrategy.Entities.ProvisionalBillingMember,LoadStrategy.Entities.SamplingFormCFromMember
                        };
            return GetSamplingFormCLS(new LoadStrategy(string.Join(",", entities)), processingCompletedOnDateTime: processingCompletedOnDateTime, formCAttachmentCompletedOnDateInd: formCAttachmentCompletedOnDateInd);
        }

        /// <summary>
        /// Load Data to generate Form C Offline Archive
        /// </summary>
        /// <param name="processingCompletedOnDateTime">processingCompletedOnDateTime</param>
        /// <param name="formCAttachmentCompletedOnDateInd">formCAttachmentCompletedOnDateInd</param>
        /// <param name="memberId">memberId</param>
        /// <param name="isProvisional">If set then member is Provisional member else member is From member</param>
        /// <returns></returns>
        public List<SamplingFormC> GetFormCDataForOfflineArchive(DateTime processingCompletedOnDateTime, int? formCAttachmentCompletedOnDateInd = null, int? memberId = null, bool isProvisional = false)
        {
            var entities = new string[] 
                        { 
                            LoadStrategy.Entities.SamplingFormC, LoadStrategy.Entities.SamplingFormCDetails, LoadStrategy.Entities.SamplingFormCRecordAttachment,
                            LoadStrategy.Entities.SamplingFormCFromMember, LoadStrategy.Entities.ProvisionalBillingMember,LoadStrategy.Entities.SamplingFormCFromMember
                        };
            if (memberId == null)
                return GetSamplingFormCLS(new LoadStrategy(string.Join(",", entities)), processingCompletedOnDateTime: processingCompletedOnDateTime, formCAttachmentCompletedOnDateInd: formCAttachmentCompletedOnDateInd);
            else
            {
                if (isProvisional)
                    return GetSamplingFormCLS(new LoadStrategy(string.Join(",", entities)), processingCompletedOnDateTime: processingCompletedOnDateTime, formCAttachmentCompletedOnDateInd: formCAttachmentCompletedOnDateInd, provisionalBillingMemberId: memberId);
                else
                {
                    return GetSamplingFormCLS(new LoadStrategy(string.Join(",", entities)), processingCompletedOnDateTime: processingCompletedOnDateTime, formCAttachmentCompletedOnDateInd: formCAttachmentCompletedOnDateInd, fromMemberId: memberId);
                }
            }
        }

        public List<SamplingFormC> GetSamplingFormCLS(LoadStrategy loadStrategy, string formCId = null, int? provisionalBillingMonth = null, int? provisionalBillingYear = null, int? fromMemberId = null, string invoiceStatusId = null, int? provisionalBillingMemberId = null, int? listingCurrencyCodeNum = null, DateTime? processingCompletedOnDateTime = null, int? formCAttachmentCompletedOnDateInd = null)
        {
            if (provisionalBillingMonth == null && provisionalBillingYear == null && fromMemberId == null && invoiceStatusId == null && provisionalBillingMemberId == null && listingCurrencyCodeNum == null && processingCompletedOnDateTime == null) throw new ArgumentNullException("All parameters are missing");
            return base.ExecuteLoadsSP(SisStoredProcedures.GetSamplingFormC,
                                       loadStrategy,
                                       new OracleParameter[]
                                   {
                                     new OracleParameter(SamplingFormCRepositoryConstants.FormCIdParameterName, formCId),
                                     new OracleParameter(SamplingFormCRepositoryConstants.ProvisionalBillingMemberIdParameterName, provisionalBillingMemberId),
                                     new OracleParameter(SamplingFormCRepositoryConstants.ProvisionalBillingMonthParameterName, provisionalBillingMonth),
                                     new OracleParameter(SamplingFormCRepositoryConstants.ProvisionalBillingYearParameterName, provisionalBillingYear),
                                     new OracleParameter(SamplingFormCRepositoryConstants.InvoiceStatusIdsParameterName, invoiceStatusId),
                                     new OracleParameter(SamplingFormCRepositoryConstants.FromMemberIdParameterName, fromMemberId),
                                     new OracleParameter(SamplingFormCRepositoryConstants.ListingCurrencyCodeNumParamaterName, listingCurrencyCodeNum),
                                     new OracleParameter(SamplingFormCRepositoryConstants.FormCDeatilIdParameterName, null),
                                     new OracleParameter(SamplingFormCRepositoryConstants.ProcessingCompletedOnDateTimeParameterName,processingCompletedOnDateTime), 
                                     new OracleParameter(SamplingFormCRepositoryConstants.FormCProcessingCompletedOnDateTimeParameterName,formCAttachmentCompletedOnDateInd), 
                                   },
                                       r => this.FetchRecord(r));
        }

        private List<SamplingFormC> FetchRecord(LoadStrategyResult loadStrategyResult)
        {
            List<SamplingFormC> samplingFormCs = new List<SamplingFormC>();
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormC))
            {
                samplingFormCs = SamplingFormCRepository.LoadEntities(this.EntityObjectSet, loadStrategyResult, null);
            }
            return samplingFormCs;
        }

        /// <summary>
        /// Load the given object set with entities from the Load Strategy Result.
        /// The task of loading child entities is delegated to the appropriate repository that handles the child entities.
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<SamplingFormC> LoadEntities(ObjectSet<SamplingFormC> objectSet, LoadStrategyResult loadStrategyResult, Action<SamplingFormC> link)
        {
            if (link == null)
                link = new Action<SamplingFormC>(c => { });

            var samplingFormCs = new List<SamplingFormC>();

            using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.SamplingFormC))
            {
                samplingFormCs = new PaxMaterializers().SamplingFormCMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
                if (!reader.IsClosed)
                    reader.Close();
            }

            if (samplingFormCs.Count > 0)
            {
                if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormCDetails))
                {
                    SamplingFormCRecordRepository.LoadEntities(objectSet.Context.CreateObjectSet<SamplingFormCRecord>(), loadStrategyResult, sfcr => sfcr.SamplingFormC = samplingFormCs.Find(sfc => sfc.Id == sfcr.SamplingFormCId), LoadStrategy.Entities.SamplingFormCDetails);
                    //The fetched child records should use the Parent entities.
                }
                if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormCListingCurrency))
                {
                    CurrencyRepository.LoadEntities(objectSet.Context.CreateObjectSet<Currency>(), loadStrategyResult, null, LoadStrategy.Entities.SamplingFormCListingCurrency);
                }
                if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.ProvisionalBillingMember))
                {
                    MemberRepository.LoadEntities(objectSet.Context.CreateObjectSet<Member>(), loadStrategyResult, null, LoadStrategy.Entities.ProvisionalBillingMember);
                }
                if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormCFromMember))
                {
                    MemberRepository.LoadEntities(objectSet.Context.CreateObjectSet<Member>(), loadStrategyResult, null, LoadStrategy.Entities.SamplingFormCFromMember);
                }
                if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.SamplingFormCSourceCodeTotal))
                {
                    SamplingFormCSamplingFormCSourceCodeTotalRepository.LoadEntities(objectSet.Context.CreateObjectSet<SamplingFormCSourceCodeTotal>(), loadStrategyResult, null);
                }
            }

            return samplingFormCs;
        }
    }
}
