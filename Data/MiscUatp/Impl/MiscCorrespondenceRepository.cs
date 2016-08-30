using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Devart.Data.Oracle;
using Iata.IS.Core;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MemberProfile.Impl;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Enums;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.MiscUatp;
using Microsoft.Data.Extensions;
using Iata.IS.Model.Common;

namespace Iata.IS.Data.MiscUatp.Impl
{
    public class MiscCorrespondenceRepository : Repository<MiscCorrespondence>, IMiscCorrespondenceRepository
    {
        public override MiscCorrespondence Single(System.Linq.Expressions.Expression<Func<MiscCorrespondence, bool>> where)
        {
            throw new NotImplementedException("Use overloaded Single instead.");
        }

        public override IQueryable<MiscCorrespondence> Get(System.Linq.Expressions.Expression<Func<MiscCorrespondence, bool>> where)
        {
            throw new NotImplementedException("Use overloaded Get method instead.");
        }

        public IQueryable<MiscCorrespondence> GetCorr(Expression<Func<MiscCorrespondence, bool>> where)
        {
            var miscCorrespondence = EntityObjectSet.Include("Attachments")
                                .Include("ToMember")
                                 .Include("FromMember")
                                 .Include("CorrespondenceOwner")
                                 .Include("Currency").Where(where);

            return miscCorrespondence;
        }

        // Review: Needs to be removed.
        public MiscCorrespondence GetCouponWithAllDetails(Guid correspondenceId)
        {
            var miscCorrespondence = EntityObjectSet
                               .Include("Attachments").SingleOrDefault(i => i.Id == correspondenceId);

            return miscCorrespondence;
        }

        public IQueryable<MiscCorrespondence> GetCorrespondenceWithInvoice(System.Linq.Expressions.Expression<Func<MiscCorrespondence, bool>> where)
        {
            return EntityObjectSet.Include("Invoice").Where(where);
        }

        /// <summary>
        /// Updates the correspondence status.
        /// </summary>
        public List<ExpiredCorrespondence> UpdateCorrespondenceStatus(BillingCategoryType billingCategoryType, BillingPeriod billingPeriod, int _oornThreshold, int _oernThreshold, int _eornThreshold, int _eoryThreshold, int _eorybThreshold)
        {
          var parameters = new ObjectParameter[7];
          parameters[0] = new ObjectParameter(MiscInvoiceRepositoryConstants.BillingCategoryIdParameterName, typeof(int))
          {
            Value = (int)billingCategoryType
          };
          parameters[1] = new ObjectParameter(MiscInvoiceRepositoryConstants.BillingPeriodParameterName, typeof(string))
          {
            Value = string.Format("{0}-{1}-{2}", billingPeriod.Period.ToString().PadLeft(2, '0'),
                                                  billingPeriod.Month.ToString().PadLeft(2, '0'), billingPeriod.Year.ToString())
          };

          parameters[2] = new ObjectParameter("OORN_THRESHOLD", typeof(int)) { Value = _oornThreshold };
          parameters[3] = new ObjectParameter("OERN_THRESHOLD", typeof(int)) { Value = _oernThreshold };
          parameters[4] = new ObjectParameter("EORN_THRESHOLD", typeof(int)) { Value = _eornThreshold };
          parameters[5] = new ObjectParameter("EORY_THRESHOLD", typeof(int)) { Value = _eoryThreshold };
          parameters[6] = new ObjectParameter("EORYB_THRESHOLD", typeof(int)) { Value = _eorybThreshold };

          //   var parameter = new ObjectParameter(MiscInvoiceRepositoryConstants.BillingCategoryIdParameterName, typeof(int)){Value = (int)billingCategoryType};
          var list = ExecuteStoredFunction<ExpiredCorrespondence>(MiscInvoiceRepositoryConstants.UpdateCorrespondenceStatusFunctionName, parameters);
          return list.ToList();
        }

        /// <summary>
        /// Get single Correspondence record
        /// </summary>
        /// <param name="correspondenceId">The Correspondence Id</param>
        /// <param name="correspondenceNumber">The Correspondence Number</param>
        /// <param name="correspondenceStage">The Correspondence Stage</param>
        /// <returns>Correspondence record, if exists</returns>
        public MiscCorrespondence Single(Guid? correspondenceId, long? correspondenceNumber, int? correspondenceStage)
        {
            var entities = new string[] 
                        { 
                            LoadStrategy.Entities.Correspondence, LoadStrategy.Entities.CorrespondenceAttachment,
                            LoadStrategy.Entities.CorrespondencesToMember, LoadStrategy.Entities.CorrespondencesFromMember,
                            LoadStrategy.Entities.CorrespondenceCurrency,LoadStrategy.Entities.AttachmentUploadedbyUser,
                            LoadStrategy.Entities.CorrespondenceOwnerInfo
                        };

            List<MiscCorrespondence> miscCorrespondences = GetCorrespondenceLS(new LoadStrategy(string.Join(",", entities)),
                                   correspondenceId: correspondenceId,
                                   correspondenceNumber: correspondenceNumber,
                                   correspondenceStage: correspondenceStage);
            if (miscCorrespondences == null || miscCorrespondences.Count == 0) return null;
            else if (miscCorrespondences.Count > 1) throw new ApplicationException("Multiple records found");
            else return miscCorrespondences[0];
        }

        public long GetInitialCorrespondenceNumber(int memberId)
        {
            /* CMP #596: LENGTH OF MEMBER ACCOUNTING CODE TO BE INCREASED TO 12. 
             * DESC: WITH THIS CMP, FOR EVERY MISC CORRESPONDENCE, 4000 WILL ALWAYS 
             * BE USED FOR THE FIRST 4 POSITIONS OF THE 11 DIGIT NUMBER. 
             * REF: FRS SECTION 3.5 POINT 7.*/
            var parameters = new ObjectParameter[1];

            parameters[0] = new ObjectParameter(MiscCorrespondenceRepositoryConstants.ResultParameterName, typeof(long));

            ExecuteStoredProcedure(MiscCorrespondenceRepositoryConstants.GetCorresRefNoMethodName, parameters);

            return long.Parse(parameters[0].Value.ToString());

        }
        /// <summary>
        /// Get filtered list of Correspondence records
        /// </summary>
        /// <param name="correspondenceNumber">The Correspondence Number</param>
        /// <param name="correspondenceStatusId">The Correspondence Status</param>
        /// <param name="authorityToBill">Authority To Bill</param>
        /// <param name="invoiceId">The Invoice</param>
        /// <param name="fromMemberId">The from member</param>
        /// <param name="correspondenceStage">The Correspondence Stage</param>
        /// <param name="correspondenceSubStatusId">The Correspondence sub status</param>
        /// <returns>List of filtered MiscCorrespondence records</returns>
        public List<MiscCorrespondence> Get(long? correspondenceNumber, int? correspondenceStatusId, bool? authorityToBill, Guid? invoiceId, int? fromMemberId, int? correspondenceStage, int? correspondenceSubStatusId)
        {
            var entities = new string[] 
                        { 
                            LoadStrategy.Entities.Correspondence, LoadStrategy.Entities.CorrespondenceAttachment,
                            LoadStrategy.Entities.CorrespondencesToMember, LoadStrategy.Entities.CorrespondencesFromMember,
                            LoadStrategy.Entities.CorrespondenceCurrency
                        };
            return GetCorrespondenceLS(new LoadStrategy(string.Join(",", entities)),
                                       correspondenceNumber: correspondenceNumber,
                                       correspondenceStatusId: correspondenceStatusId,
                                       authorityToBill: authorityToBill,
                                       invoiceId: invoiceId,
                                       fromMemberId: fromMemberId,
                                       correspondenceStage: correspondenceStage,
                                       correspondenceSubStatusId: correspondenceSubStatusId);
        }

        private List<MiscCorrespondence> GetCorrespondenceLS(LoadStrategy loadStrategy, Guid? correspondenceId = null, long? correspondenceNumber = null, int? correspondenceStatusId = null, bool? authorityToBill = null, Guid? invoiceId = null, int? fromMemberId = null, int? correspondenceStage = null, int? correspondenceSubStatusId = null)
        {
            if (correspondenceId == null && correspondenceNumber == null && correspondenceStatusId == null && authorityToBill == null && invoiceId == null && fromMemberId == null &&
                correspondenceStage == null && correspondenceSubStatusId == null) throw new ArgumentNullException("All  filter parameters are missing");
            return base.ExecuteLoadsSP(SisStoredProcedures.GetMiscCorrespondence,
                                       loadStrategy,
                                       new[]
                                         {

                                             new OracleParameter(MiscCorrespondenceRepositoryConstants.CorrespondenceId,
                                                                 correspondenceId.HasValue ? ConvertUtil.ConvertGuidToString(correspondenceId.Value) : null),
                                             new OracleParameter(MiscCorrespondenceRepositoryConstants.CorrespondenceNumber, correspondenceNumber),
                                             new OracleParameter(MiscCorrespondenceRepositoryConstants.CorrespondenceStage, correspondenceStage),
                                             new OracleParameter(MiscCorrespondenceRepositoryConstants.InvoiceId,
                                                                 invoiceId.HasValue ? ConvertUtil.ConvertGuidToString(invoiceId.Value) : null),
                                             new OracleParameter(MiscCorrespondenceRepositoryConstants.FromMemberId, fromMemberId),
                                             new OracleParameter(MiscCorrespondenceRepositoryConstants.CorrespondenceStatus, correspondenceStatusId),
                                             new OracleParameter(MiscCorrespondenceRepositoryConstants.CorrespondenceSubStatus, correspondenceSubStatusId),
                                             new OracleParameter(MiscCorrespondenceRepositoryConstants.AuthorityToBill, (authorityToBill.HasValue ? (int?) (authorityToBill.Value ? 1 : 0) : null))
                                         },
                                       r => this.FetchRecord(r));
        }
        private List<MiscCorrespondence> FetchRecord(LoadStrategyResult loadStrategyResult)
        {
            List<MiscCorrespondence> miscCorrespondenceRecords = new List<MiscCorrespondence>();
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.Correspondence))
            {
                miscCorrespondenceRecords = MiscCorrespondenceRepository.LoadEntities(this.EntityObjectSet, loadStrategyResult, null);
            }
            return miscCorrespondenceRecords;
        }

        /// <summary>
        /// This will load list of MiscCorrespondence objects
        /// </summary>
        /// <param name="objectSet"></param>
        /// <param name="loadStrategyResult"></param>
        /// <param name="link"></param>
        /// <returns></returns>
        public static List<MiscCorrespondence> LoadEntities(ObjectSet<MiscCorrespondence> objectSet, LoadStrategyResult loadStrategyResult, Action<MiscCorrespondence> link)
        {
            if (link == null)
                link = new Action<MiscCorrespondence>(c => { });

            var miscCorrespondences = new List<MiscCorrespondence>();

      
      var muMaterializers = new MuMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.Correspondence))
      {
          
        // first result set includes the category
        miscCorrespondences = muMaterializers.MiscCorrespondenceMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
        reader.Close();
      }

            //Load Misc CorrespondencesFromMember list
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CorrespondencesFromMember) && miscCorrespondences.Count != 0)
            {
                MemberRepository.LoadEntities(objectSet.Context.CreateObjectSet<Member>(), loadStrategyResult, null, LoadStrategy.Entities.CorrespondencesFromMember);
            }

            //Load Misc CorrespondencesToMember list
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CorrespondencesToMember) && miscCorrespondences.Count != 0)
            {
                MemberRepository.LoadEntities(objectSet.Context.CreateObjectSet<Member>(), loadStrategyResult, null, LoadStrategy.Entities.CorrespondencesToMember);
            }

            //Load MiscUatpCorrespondenceAttachment
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CorrespondenceAttachment) && miscCorrespondences.Count != 0)
            {
                MiscUatpCorrespondenceAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<MiscUatpCorrespondenceAttachment>(), loadStrategyResult, null);
            }

            //Load MiscUatpCorrespondenceCurrency
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CorrespondenceCurrency) && miscCorrespondences.Count != 0)
            {
                CurrencyRepository.LoadEntities(objectSet.Context.CreateObjectSet<Currency>(), loadStrategyResult, null, LoadStrategy.Entities.CorrespondenceCurrency);
            }

            //Load Correspondence owner information
            if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CorrespondenceOwnerInfo) && miscCorrespondences.Count != 0)
            {
                UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>(), loadStrategyResult, null, LoadStrategy.Entities.CorrespondenceOwnerInfo);
            }

            return miscCorrespondences;
        }
    

    public IQueryable<MiscCorrespondence> GetCorrespondenceForTraiReport(System.Linq.Expressions.Expression<Func<MiscCorrespondence, bool>> where)
    {
      return EntityObjectSet
      .Include("Attachments")
      .Include("FromMember")
      .Include("ToMember")
      .Include("Currency")
      .Where(where);
    }

    public IQueryable<MiscCorrespondence> GetLastRespondedCorrespondene(System.Linq.Expressions.Expression<Func<MiscCorrespondence, bool>> where)
    {
        return EntityObjectSet.Where(where);
    }

  }
}
