using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Core;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Common;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MemberProfile.Impl;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Pax;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Pax.Impl
{
  public class PaxCorrespondenceRepository : Repository<Correspondence>, IPaxCorrespondenceRepository
  {
    public override Correspondence Single(System.Linq.Expressions.Expression<Func<Correspondence, bool>> where)
    {
      throw new NotImplementedException("Use overloaded Single instead.");
    }

    public override IQueryable<Correspondence> Get(System.Linq.Expressions.Expression<Func<Correspondence, bool>> where)
    {
      return EntityObjectSet
        .Include("ToMember").Include("FromMember")
        .Where(where);
    }

    public IQueryable<Correspondence> GetCorr(System.Linq.Expressions.Expression<Func<Correspondence, bool>> where)
    {
      var paxCorrespondence = EntityObjectSet.Include("Attachments")
                          .Include("ToMember")
                           .Include("FromMember")
                           .Include("CorrespondenceOwner")
                           .Include("Currency").Where(where);

      return paxCorrespondence;
    }


    public IQueryable<Correspondence> GetCorrespondenceWithInvoice(System.Linq.Expressions.Expression<Func<Correspondence, bool>> where)
    {
      return EntityObjectSet.Include("Invoice").Where(where);
    }

    public List<ExpiredCorrespondence> UpdateCorrespondenceStatus(BillingPeriod billingPeriod, int _oornThreshold, int _oernThreshold, int _eornThreshold, int _eoryThreshold, int _eorybThreshold)
    {
      var parameters = new ObjectParameter[6];
      parameters[0] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.BillingPeriodParameterName, typeof(string))
      {
        Value = string.Format("{0}-{1}-{2}", billingPeriod.Period.ToString().PadLeft(2, '0'), billingPeriod.Month.ToString().PadLeft(2, '0'), billingPeriod.Year.ToString())
      };

      parameters[1] = new ObjectParameter("OORN_THRESHOLD", typeof(int)) { Value = _oornThreshold };
      parameters[2] = new ObjectParameter("OERN_THRESHOLD", typeof(int)) { Value = _oernThreshold };
      parameters[3] = new ObjectParameter("EORN_THRESHOLD", typeof(int)) { Value = _eornThreshold };
      parameters[4] = new ObjectParameter("EORY_THRESHOLD", typeof(int)) { Value = _eoryThreshold };
      parameters[5] = new ObjectParameter("EORYB_THRESHOLD", typeof(int)) { Value = _eorybThreshold };

      var list = ExecuteStoredFunction<ExpiredCorrespondence>(PaxCorrespondenceRepositoryConstants.UpdatePaxCorrespondenceMethodName, parameters);
      return list.ToList();
    }

    public static List<Correspondence> LoadAuditEntities(ObjectSet<Correspondence> objectSet, LoadStrategyResult loadStrategyResult, Action<Correspondence> link, string entity)
    {
      if (link == null)
        link = new Action<Correspondence>(c => { });
      List<Correspondence> correspondence = new List<Correspondence>();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.PaxEntities.Correspondence))
      {
        // first result set includes the category
        foreach (var c in
          new PaxMaterializers().PaxInvoiceCorrespondenceAuditMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach<Correspondence>(link)
          )
        {
          correspondence.Add(c);
        }
        reader.Close();
      }

      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CorrespondenceAttachment))
      {
        PaxCorrespondenceAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<CorrespondenceAttachment>(), loadStrategyResult, null);
      }

      return correspondence;
    }
    /// <summary>
    /// Get single Correspondence record
    /// </summary>
    /// <param name="correspondenceId">The Correspondence Id</param>
    /// <param name="correspondenceNumber">The Correspondence Number</param>
    /// <param name="correspondenceStage">The Correspondence Stage</param>
    /// <returns>Correspondence record, if exists</returns>
    public Correspondence Single(Guid? correspondenceId = null, long ? correspondenceNumber = null, int? correspondenceStage = null)
    {
      var entities = new string[]
                       {
                         LoadStrategy.Entities.Correspondence, LoadStrategy.Entities.CorrespondenceAttachment, LoadStrategy.Entities.CorrespondencesToMember,
                         LoadStrategy.Entities.CorrespondencesFromMember, LoadStrategy.Entities.CorrespondenceCurrency,LoadStrategy.Entities.AttachmentUploadedbyUser,
                         LoadStrategy.Entities.CorrespondenceOwnerInfo
                       };

      List<Correspondence> correspondences = GetCorrespondenceLS(new LoadStrategy(string.Join(",", entities)),
                                                                 correspondenceId: correspondenceId,
                                                                 correspondenceNumber: correspondenceNumber,
                                                                 correspondenceStage: correspondenceStage);
      if (correspondences == null || correspondences.Count == 0) return null;
      else if (correspondences.Count > 1) throw new ApplicationException("Multiple records found");
      else return correspondences[0];
    }

    //SCP210204: IS-WEB Outage (Added new method to improve performance of pax correspondence)
    /// <summary>
    /// Gets the correspondence and attachment.
    /// </summary>
    /// <param name="correspondenceId">The correspondence id.</param>
    /// <returns></returns>
    public Correspondence GetCorrespondenceAndAttachment(Guid correspondenceId)
    {
      var entities = new string[]
                       {
                         LoadStrategy.Entities.Correspondence, LoadStrategy.Entities.CorrespondenceAttachment
                       };

      List<Correspondence> correspondences = GetCorrespondenceLS(new LoadStrategy(string.Join(",", entities)),
                                                                 correspondenceId: correspondenceId);
      if (correspondences == null || correspondences.Count == 0) return null;
      else if (correspondences.Count > 1) throw new ApplicationException("Multiple records found");
      else return correspondences[0];
    }
    public long GetInitialCorrespondenceNumber(int memberId)
    {
        var parameters = new ObjectParameter[2];

        parameters[0] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.MemberIdParameterName, typeof(int)) { Value = memberId };

        parameters[1] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.ResultParameterName, typeof (long));

        ExecuteStoredProcedure(PaxCorrespondenceRepositoryConstants.GetCorresRefNoMethodName, parameters);

        return long.Parse(parameters[1].Value.ToString());

    }


    private List<Correspondence> GetCorrespondenceLS(LoadStrategy loadStrategy, Guid? correspondenceId = null, long ? correspondenceNumber = null, int? correspondenceStage = null)
      {
          if (correspondenceId == null && correspondenceNumber == null && correspondenceStage == null )
          {
            throw new ArgumentNullException("loadStrategy");
          }

      return ExecuteLoadsSP(SisStoredProcedures.GetCorrespondence,
                            loadStrategy,
                            new[]
                              {
                                new OracleParameter(PaxCorrespondenceRepositoryConstants.CorrespondenceId, correspondenceId.HasValue ? ConvertUtil.ConvertGuidToString(correspondenceId.Value) : null),
                                new OracleParameter(PaxCorrespondenceRepositoryConstants.CorrespondenceNumber, correspondenceNumber),
                                new OracleParameter(PaxCorrespondenceRepositoryConstants.CorrespondenceStage, correspondenceStage)
                              },
                            r => FetchRecord(r));
      }

      private List<Correspondence> FetchRecord(LoadStrategyResult loadStrategyResult)
      {
          List<Correspondence> correspondences = new List<Correspondence>();
          if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.Correspondence))
          {
              correspondences = PaxCorrespondenceRepository.LoadEntities(this.EntityObjectSet, loadStrategyResult, null);
          }
          return correspondences;
      }

      /// <summary>
      /// This will load list of Correspondence objects
      /// </summary>
      /// <param name="objectSet"></param>
      /// <param name="loadStrategyResult"></param>
      /// <param name="link"></param>
      /// <returns></returns>
      public static List<Correspondence> LoadEntities(ObjectSet<Correspondence> objectSet, LoadStrategyResult loadStrategyResult, Action<Correspondence> link)
      {
        if (link == null) link = new Action<Correspondence>(c => { });

        var correspondences = new List<Correspondence>();

        using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.Correspondence))
        {

          correspondences = new PaxMaterializers().PaxCorrespondenceMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList(); 
          reader.Close();
        }
        if (correspondences.Count > 0)
        {
          if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CorrespondencesFromMember))
          {
            MemberRepository.LoadEntities(objectSet.Context.CreateObjectSet<Member>(), loadStrategyResult, null, LoadStrategy.Entities.CorrespondencesFromMember);
          }

          if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CorrespondencesToMember))
          {
            MemberRepository.LoadEntities(objectSet.Context.CreateObjectSet<Member>(), loadStrategyResult, null, LoadStrategy.Entities.CorrespondencesToMember);
          }


          if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CorrespondenceAttachment))
          {
            PaxCorrespondenceAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<CorrespondenceAttachment>(), loadStrategyResult, null);
          }

          if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CorrespondenceCurrency))
          {
            CurrencyRepository.LoadEntities(objectSet.Context.CreateObjectSet<Currency>(), loadStrategyResult, null, LoadStrategy.Entities.CorrespondenceCurrency);
          }

          //Load Correspondence owner information
          if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CorrespondenceOwnerInfo))
          {
            UserRepository.LoadEntities(objectSet.Context.CreateObjectSet<User>(), loadStrategyResult, null, LoadStrategy.Entities.CorrespondenceOwnerInfo);
          }
        }

        return correspondences;
      }

      public IQueryable<Correspondence> GetCorrespondenceWithAttachment(System.Linq.Expressions.Expression<Func<Correspondence, bool>> where)
      {
        return EntityObjectSet
        .Include("Attachments")
        .Where(where);
      }

      public IQueryable<Correspondence> GetCorrespondenceForTraiReport(System.Linq.Expressions.Expression<Func<Correspondence, bool>> where)
      {
        return EntityObjectSet
        .Include("Attachments")
        .Include("FromMember")
        .Include("ToMember")
        .Include("Currency")
        .Where(where);
      }

      /*SCP# 120094 - Pax Correspondence Download- Download All button 
        Desc: Comma seperated field in queue had size limit of 4000, now it is updated (to CLOB) to accomodate value of any length.
        Method below is to enqueue correspondences.
        Date: 10-May-2013*/
      public void EnqueueCorrespondencesForDownloadReport(int memberId, int userId, String correspondenceNumbers, String downloadUrl)
      {
          try
          {
              var parameters = new ObjectParameter[4];
              parameters[0] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.MemberIdParameterNameForEnqueDownloadRequest, typeof(int))
              {
                  Value = memberId
              };
              parameters[1] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.UserIdParameterNameForEnqueDownloadRequest, typeof(int))
              {
                  Value = userId
              };
              parameters[2] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.CorrespondenceIdsParameterNameForEnqueDownloadRequest, typeof(String))
              {
                  Value = correspondenceNumbers
              };
              parameters[3] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.DownloadUrlForEnqueDownloadRequest, typeof(String))
              {
                  Value = downloadUrl
              };

              ExecuteStoredProcedure(PaxCorrespondenceRepositoryConstants.EnqueueCorrespondenceReportForDownloadFunctionName, parameters);
          }
          catch (Exception)
          {
              
              throw;
          }
      }

      //SCP106534: ISWEB No-02350000768 
      //Desc: Calling SP to create corr.
      //Date: 20/06/2013
      public int CreateCorrespondence(ref Correspondence correspondenceBaseRecord)
      {
          int returnValue = -1;

          try
          {
              string corrAttachmentNetGuid = "";
              foreach (CorrespondenceAttachment attachment in correspondenceBaseRecord.Attachments)
              {
                  corrAttachmentNetGuid = corrAttachmentNetGuid + attachment.Id.ToString() + ",";
              }

              if (corrAttachmentNetGuid.Length != 0)
                  corrAttachmentNetGuid = corrAttachmentNetGuid.Substring(0, corrAttachmentNetGuid.Length - 1);

              /* input */
              /* CMP#657: Retention of Additional Email Addresses in Correspondences
                 Adding code to get email ids from initiator and non-initiator and removing
                 additional email field*/
              var parameters = new ObjectParameter[32];
              parameters[0] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.CORRESPONDENCE_NO_I, typeof(int)) { Value = correspondenceBaseRecord.CorrespondenceNumber };
              parameters[1] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.CORRESPONDENCE_DATE_I, typeof(DateTime)) { Value = correspondenceBaseRecord.CorrespondenceDate };
              parameters[2] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.CORRESPONDENCE_STAGE_I, typeof(int)) { Value = correspondenceBaseRecord.CorrespondenceStage };
              parameters[3] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.FROM_MEMBER_ID_I, typeof(int)) { Value = correspondenceBaseRecord.FromMemberId };
              parameters[4] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.TO_MEMBER_ID_I, typeof(int)) { Value = correspondenceBaseRecord.ToMemberId };
              parameters[5] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.TO_EMAILID_I, typeof(string)) { Value = correspondenceBaseRecord.ToEmailId };
              parameters[6] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.AMOUNT_TO_SETTLED_I, typeof(decimal)) { Value = correspondenceBaseRecord.AmountToBeSettled };
              parameters[7] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.OUR_REFERENCE_I, typeof(string)) { Value = correspondenceBaseRecord.OurReference };
              parameters[8] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.YOUR_REFERENCE_I, typeof(string)) { Value = correspondenceBaseRecord.YourReference };
              parameters[9] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.SUBJECT_I, typeof(string)) { Value = correspondenceBaseRecord.Subject };
              parameters[10] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.CORRESPONDENCE_STATUS_I, typeof(int)) { Value = correspondenceBaseRecord.CorrespondenceStatusId };
              parameters[11] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.AUTHORITY_TO_BILL_I, typeof(int)) { Value = correspondenceBaseRecord.AuthorityToBill };
              parameters[12] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.NO_OF_DAYS_TO_EXPIRE_I, typeof(int)) { Value = correspondenceBaseRecord.NumberOfDaysToExpire };
              parameters[13] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.LAST_UPDATED_BY_I, typeof(int)) { Value = correspondenceBaseRecord.LastUpdatedBy };
              parameters[14] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.LAST_UPDATED_ON_I, typeof(DateTime)) { Value = correspondenceBaseRecord.LastUpdatedOn };
              parameters[15] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.FROM_EMAILID_I, typeof(string)) { Value = correspondenceBaseRecord.FromEmailId };
              parameters[16] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.CURRENCY_CODE_NUM_I, typeof(int)) { Value = correspondenceBaseRecord.CurrencyId };
              parameters[17] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.CORRESPONDENCE_DETAILS_I, typeof(string)) { Value = correspondenceBaseRecord.CorrespondenceDetails };
              parameters[18] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.CORRESPONDENCE_OWNER_ID_I, typeof(int)) { Value = correspondenceBaseRecord.CorrespondenceOwnerId };
              parameters[19] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.CORRESPONDENCE_SUB_STATUS_I, typeof(int)) { Value = correspondenceBaseRecord.CorrespondenceSubStatusId };
              parameters[20] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.ADDITIONAL_EMAIL_INITIATOR, typeof(string)) { Value = correspondenceBaseRecord.AdditionalEmailInitiator };
              parameters[21] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.ADDITIONAL_EMAIL_NON_INITIATOR, typeof(string)) { Value = correspondenceBaseRecord.AdditionalEmailNonInitiator };
              parameters[22] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.INVOICE_ID_I, typeof(Guid)) { Value = correspondenceBaseRecord.InvoiceId };
              parameters[23] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.CORRESPONDENCE_SENT_ON_I, typeof(DateTime)) { Value = correspondenceBaseRecord.CorrespondenceSentOnDate };
              parameters[24] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.EXPIRY_DATE_I, typeof(DateTime)) { Value = correspondenceBaseRecord.ExpiryDate };
              parameters[25] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.EXPIRY_DATEPERIOD_I, typeof(DateTime)) { Value = correspondenceBaseRecord.ExpiryDatePeriod };
              parameters[26] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.BM_EXPIRY_PERIOD_I, typeof(DateTime)) { Value = correspondenceBaseRecord.BMExpiryPeriod };
              parameters[27] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.PAX_RM_IDS_I, typeof(string)) { Value = GetRMsOracleGuids(correspondenceBaseRecord.RejectionMemoIds) };
              parameters[28] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.PAX_CORR_ATTACHMENT_IDS_I, typeof(string)) { Value = GetRMsOracleGuids(corrAttachmentNetGuid) };
              parameters[29] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.CORRESPONDENCE_ID_O, typeof(Guid));
              parameters[30] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.OPERATION_STATUS_INDICATOR_O, typeof(int));
              //CMP526 - Passenger Correspondence Identifiable by Source Code
              parameters[31] = new ObjectParameter(PaxCorrespondenceRepositoryConstants.SOURCE_CODE_I, typeof (int))
                                   {Value = correspondenceBaseRecord.SourceCode};

              /* call sp */
              ExecuteStoredProcedure(PaxCorrespondenceRepositoryConstants.ProcCreatePaxCorrespondenceSP, parameters);

              /* output */
              //-1 => Internal DB Exception
              //0  => Success (this is when CORRESPONDENCE_ID_O will have a value)
              //1  => RM already has corr linked to it.
              //2  => Problem updating RM
              //3  => ANYTHING BUT STAGE 1 CORR ALREADY EXISTS.
              if (parameters[30].Value != null)
              {
                  if (int.TryParse(parameters[30].Value.ToString(), out returnValue))
                  {
                    //SCP210204: IS-WEB Outage (QA Issue Fix)
                    if (returnValue != -1 && returnValue != 4)
                      {
                          correspondenceBaseRecord.Id = (Guid)parameters[29].Value;
                      }
                  }
              }
          }
          catch (Exception ex)
          {
              returnValue = -1;
              throw;
          }
          return returnValue;
      }

      //SCP106534: ISWEB No-02350000768 
      //Desc: Method convert the .Net GUIDs to Oracle GUIDs.
      //Date: 20/06/2013
      private string GetRMsOracleGuids(string rejectionMemoIds)
      {
          if (!string.IsNullOrWhiteSpace(rejectionMemoIds))
          {
              string RMsOracleGuids = "";
              var sRejectedMemos = rejectionMemoIds.Split(',');

              foreach (string sRejectedMemo in sRejectedMemos)
              {
                  if (!string.IsNullOrWhiteSpace(sRejectedMemo))
                      RMsOracleGuids += ConvertUtil.ConvertNetGuidToOracleGuid(sRejectedMemo) + ",";
              }

              if (RMsOracleGuids.Contains(","))
              {
                  RMsOracleGuids = RMsOracleGuids.Remove(RMsOracleGuids.Length - 1);
              }

              return RMsOracleGuids;
          }

          return "";
      }

      /// <summary>
      /// Gets Only Correspondence from database Using Load Strategy.
      /// </summary>
      /// <param name="correspondenceId"></param>
      /// <param name="correspondenceNumber"></param>
      /// <param name="correspondenceStage"></param>
      /// <returns></returns>
      public Correspondence GetOnlyCorrespondenceUsingLoadStrategy(Guid? correspondenceId = null, long? correspondenceNumber = null, int? correspondenceStage = null)
      {
          var entities = new string[]
                       {
                         LoadStrategy.Entities.Correspondence
                       };

          List<Correspondence> correspondences = GetCorrespondenceLS(new LoadStrategy(string.Join(",", entities)),
                                                                     correspondenceId: correspondenceId,
                                                                     correspondenceNumber: correspondenceNumber,
                                                                     correspondenceStage: correspondenceStage);
          if (correspondences == null || correspondences.Count == 0) return null;
          else if (correspondences.Count > 1) throw new ApplicationException("Multiple records found");
          else return correspondences[0];
      }

      public IQueryable<Correspondence> GetLastRespondedCorrespondene(System.Linq.Expressions.Expression<Func<Correspondence, bool>> where)
      {
          return EntityObjectSet.Where(where);
      }

      public Correspondence GetFirstCorrespondence(Guid? correspondenceId = null)
      {
          return ExecuteLoadsSP(SisStoredProcedures.GetFirstCorrespondence,
                            new LoadStrategy(""), 
                            new[]
                              {
                                new OracleParameter(PaxCorrespondenceRepositoryConstants.CorrespondenceId, correspondenceId.HasValue ? 
                                    ConvertUtil.ConvertGuidToString(correspondenceId.Value) : null)
                              },
                            r => FetchRecord(r)).FirstOrDefault();
      }
  }
}
