using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Devart.Data.Oracle;
using Iata.IS.Core;
using Iata.IS.Model.Calendar;
using Iata.IS.Model.Cargo;
using Iata.IS.Model.Common;
using Iata.IS.Data.Common.Impl;
using Iata.IS.Data.Impl;
using Iata.IS.Data.MemberProfile.Impl;
using Iata.IS.Model.MemberProfile;
using Microsoft.Data.Extensions;

namespace Iata.IS.Data.Cargo.Impl
{
  public class CargoCorrespondenceRepository : Repository<CargoCorrespondence>, ICargoCorrespondenceRepository
  {
    public override CargoCorrespondence Single(System.Linq.Expressions.Expression<Func<CargoCorrespondence, bool>> where)
    {
      throw new NotImplementedException("Use overloaded Single instead.");
    }

    /* public override IQueryable<CargoCorrespondence> Get(System.Linq.Expressions.Expression<Func<CargoCorrespondence, bool>> where)
    {
      return EntityObjectSet
        .Include("ToMember").Include("FromMember")
        .Where(where);
    } */

    public IQueryable<CargoCorrespondence> GetCorr(System.Linq.Expressions.Expression<Func<CargoCorrespondence, bool>> where)
    {
      var cargoCorrespondence = EntityObjectSet.Include("Attachments")
                          .Include("ToMember")
                           .Include("FromMember")
                           .Include("CorrespondenceOwner")
                           .Include("Currency").Where(where);

      return cargoCorrespondence;
    }
    public IQueryable<CargoCorrespondence> GetCorrespondenceWithInvoice(System.Linq.Expressions.Expression<Func<CargoCorrespondence, bool>> where)
    {
      return EntityObjectSet.Include("Invoice").Where(where);
    }

    public List<ExpiredCorrespondence> UpdateCorrespondenceStatus(BillingPeriod billingPeriod, int _oornThreshold, int _oernThreshold, int _eornThreshold, int _eoryThreshold, int _eorybThreshold)
    {
      var parameters = new ObjectParameter[6];
      parameters[0] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.BillingPeriodParameterName, typeof(string))
      {
        Value = string.Format("{0}-{1}-{2}", billingPeriod.Period.ToString().PadLeft(2, '0'), billingPeriod.Month.ToString().PadLeft(2, '0'), billingPeriod.Year.ToString())
      };

      parameters[1] = new ObjectParameter("OORN_THRESHOLD", typeof(int)) { Value = _oornThreshold };
      parameters[2] = new ObjectParameter("OERN_THRESHOLD", typeof(int)) { Value = _oernThreshold };
      parameters[3] = new ObjectParameter("EORN_THRESHOLD", typeof(int)) { Value = _eornThreshold };
      parameters[4] = new ObjectParameter("EORY_THRESHOLD", typeof(int)) { Value = _eoryThreshold };
      parameters[5] = new ObjectParameter("EORYB_THRESHOLD", typeof(int)) { Value = _eorybThreshold };


      var list = ExecuteStoredFunction<ExpiredCorrespondence>(CargoCorrespondenceRepositoryConstants.UpdateCargoCorrespondenceMethodName, parameters);
      return list.ToList();
    }

    public static List<CargoCorrespondence> LoadAuditEntities(ObjectSet<CargoCorrespondence> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoCorrespondence> link, string entity)
    {
      if (link == null)
        link = new Action<CargoCorrespondence>(c => { });
      List<CargoCorrespondence> correspondence = new List<CargoCorrespondence>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.PaxEntities.Correspondence))
      {
        // first result set includes the category
        foreach (var c in
          cargoMaterializers.CargoInvoiceCorrespondenceAuditMaterializer
            .Materialize(reader)
            .Bind(objectSet)
            .ForEach<CargoCorrespondence>(link)
          )
        {
          correspondence.Add(c);
        }
        reader.Close();
      }

      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.CorrespondenceAttachment))
      {
        CargoCorrespondenceAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<CargoCorrespondenceAttachment>(), loadStrategyResult, null);
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
    public CargoCorrespondence Single(Guid? correspondenceId = null, long? correspondenceNumber = null, int? correspondenceStage = null)
    {
      var entities = new string[]
                       {
                         LoadStrategy.Entities.Correspondence, LoadStrategy.Entities.CorrespondenceAttachment, LoadStrategy.Entities.CorrespondencesToMember,
                         LoadStrategy.Entities.CorrespondencesFromMember, LoadStrategy.Entities.CorrespondenceCurrency,LoadStrategy.Entities.AttachmentUploadedbyUser,
                         LoadStrategy.Entities.CorrespondenceOwnerInfo
                       };

      List<CargoCorrespondence> correspondences = GetCorrespondenceLS(new LoadStrategy(string.Join(",", entities)),
                                                                 correspondenceId: correspondenceId,
                                                                 correspondenceNumber: correspondenceNumber,
                                                                 correspondenceStage: correspondenceStage);
      if (correspondences == null || correspondences.Count == 0) return null;
      else if (correspondences.Count > 1) throw new ApplicationException("Multiple records found");
      else return correspondences[0];
    }

    public long GetInitialCorrespondenceNumber(int memberId)
    {
        var parameters = new ObjectParameter[2];

        parameters[0] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.MemberIdParameterName, typeof(int)) { Value = memberId };

        parameters[1] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.ResultParameterName, typeof(long));

        ExecuteStoredProcedure(CargoCorrespondenceRepositoryConstants.GetCorresRefNoMethodName, parameters);

        return long.Parse(parameters[1].Value.ToString());

    }


    private List<CargoCorrespondence> GetCorrespondenceLS(LoadStrategy loadStrategy, Guid? correspondenceId = null, long? correspondenceNumber = null, int? correspondenceStage = null)
    {
      if (correspondenceId == null && correspondenceNumber == null && correspondenceStage == null)
      {
        throw new ArgumentNullException("loadStrategy");
      }

      return ExecuteLoadsSP(SisStoredProcedures.GetCargoCorrespondence,
                        loadStrategy,
                        new[]
                            {
                              new OracleParameter(CargoCorrespondenceRepositoryConstants.CorrespondenceId, correspondenceId.HasValue ? ConvertUtil.ConvertGuidToString(correspondenceId.Value) : null),
                              new OracleParameter(CargoCorrespondenceRepositoryConstants.CorrespondenceNumber, correspondenceNumber),
                              new OracleParameter(CargoCorrespondenceRepositoryConstants.CorrespondenceStage, correspondenceStage)
                            },
                        r => FetchRecord(r));
    }

    private List<CargoCorrespondence> FetchRecord(LoadStrategyResult loadStrategyResult)
    {
      List<CargoCorrespondence> correspondences = new List<CargoCorrespondence>();
      if (loadStrategyResult.IsLoaded(LoadStrategy.Entities.Correspondence))
      {
        correspondences = CargoCorrespondenceRepository.LoadEntities(this.EntityObjectSet, loadStrategyResult, null);
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
    public static List<CargoCorrespondence> LoadEntities(ObjectSet<CargoCorrespondence> objectSet, LoadStrategyResult loadStrategyResult, Action<CargoCorrespondence> link)
    {
      if (link == null) link = new Action<CargoCorrespondence>(c => { });

      var correspondences = new List<CargoCorrespondence>();
      var cargoMaterializers = new CargoMaterializers();
      using (OracleDataReader reader = loadStrategyResult.GetReader(LoadStrategy.Entities.Correspondence))
      {

        correspondences = cargoMaterializers.CargoCorrespondenceMaterializer.Materialize(reader).Bind(objectSet).ForEach(link).ToList();
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
          CargoCorrespondenceAttachmentRepository.LoadEntities(objectSet.Context.CreateObjectSet<CargoCorrespondenceAttachment>(), loadStrategyResult, null);
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

    public IQueryable<CargoCorrespondence> GetCorrespondenceWithAttachment(System.Linq.Expressions.Expression<Func<CargoCorrespondence, bool>> where)
    {
      return EntityObjectSet
      .Include("Attachments")
      .Where(where);
    }

    public IQueryable<CargoCorrespondence> GetCorrespondenceForTraiReport(System.Linq.Expressions.Expression<Func<CargoCorrespondence, bool>> where)
    {
      return EntityObjectSet
      .Include("Attachments")
      .Include("FromMember")
      .Include("ToMember")
      .Include("Currency")
      .Where(where);
    }

    //SCP106534: ISWEB No-02350000768 
    //Desc: Calling SP to create corr.
    //Date: 20/06/2013
    public int CreateCorrespondence(ref CargoCorrespondence correspondenceRecord)
    {
        int returnValue = -1;

        try
        {
            string corrAttachmentNetGuid = "";
            foreach (CargoCorrespondenceAttachment attachment in correspondenceRecord.Attachments)
            {
                corrAttachmentNetGuid = corrAttachmentNetGuid + attachment.Id.ToString() + ",";
            }

            if (corrAttachmentNetGuid.Length != 0)
                corrAttachmentNetGuid = corrAttachmentNetGuid.Substring(0, corrAttachmentNetGuid.Length - 1);

            /* input */
            /* CMP#657: Retention of Additional Email Addresses in Correspondences
                 Adding code to get email ids from initiator and non-initiator and removing
                 additional email field*/
            var parameters = new ObjectParameter[31];
            parameters[0] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.CORRESPONDENCE_NO_I, typeof(int)) { Value = correspondenceRecord.CorrespondenceNumber };
            parameters[1] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.CORRESPONDENCE_DATE_I, typeof(DateTime)) { Value = correspondenceRecord.CorrespondenceDate };
            parameters[2] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.CORRESPONDENCE_STAGE_I, typeof(int)) { Value = correspondenceRecord.CorrespondenceStage };
            parameters[3] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.FROM_MEMBER_ID_I, typeof(int)) { Value = correspondenceRecord.FromMemberId };
            parameters[4] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.TO_MEMBER_ID_I, typeof(int)) { Value = correspondenceRecord.ToMemberId };
            parameters[5] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.TO_EMAILID_I, typeof(string)) { Value = correspondenceRecord.ToEmailId };
            parameters[6] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.AMOUNT_TO_SETTLED_I, typeof(decimal)) { Value = correspondenceRecord.AmountToBeSettled };
            parameters[7] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.OUR_REFERENCE_I, typeof(string)) { Value = correspondenceRecord.OurReference };
            parameters[8] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.YOUR_REFERENCE_I, typeof(string)) { Value = correspondenceRecord.YourReference };
            parameters[9] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.SUBJECT_I, typeof(string)) { Value = correspondenceRecord.Subject };
            parameters[10] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.CORRESPONDENCE_STATUS_I, typeof(int)) { Value = correspondenceRecord.CorrespondenceStatusId };
            parameters[11] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.AUTHORITY_TO_BILL_I, typeof(int)) { Value = correspondenceRecord.AuthorityToBill };
            parameters[12] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.NO_OF_DAYS_TO_EXPIRE_I, typeof(int)) { Value = correspondenceRecord.NumberOfDaysToExpire };
            parameters[13] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.LAST_UPDATED_BY_I, typeof(int)) { Value = correspondenceRecord.LastUpdatedBy };
            parameters[14] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.LAST_UPDATED_ON_I, typeof(DateTime)) { Value = correspondenceRecord.LastUpdatedOn };
            parameters[15] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.FROM_EMAILID_I, typeof(string)) { Value = correspondenceRecord.FromEmailId };
            parameters[16] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.CURRENCY_CODE_NUM_I, typeof(int)) { Value = correspondenceRecord.CurrencyId };
            parameters[17] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.CORRESPONDENCE_DETAILS_I, typeof(string)) { Value = correspondenceRecord.CorrespondenceDetails };
            parameters[18] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.CORRESPONDENCE_OWNER_ID_I, typeof(int)) { Value = correspondenceRecord.CorrespondenceOwnerId };
            parameters[19] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.CORRESPONDENCE_SUB_STATUS_I, typeof(int)) { Value = correspondenceRecord.CorrespondenceSubStatusId };
            parameters[20] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.ADDITIONAL_EMAIL_INITIATOR, typeof(string)) { Value = correspondenceRecord.AdditionalEmailInitiator };
            parameters[21] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.ADDITIONAL_EMAIL_NON_INITIATOR, typeof(string)) { Value = correspondenceRecord.AdditionalEmailNonInitiator };
            parameters[22] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.INVOICE_ID_I, typeof(Guid)) { Value = correspondenceRecord.InvoiceId };
            parameters[23] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.CORRESPONDENCE_SENT_ON_I, typeof(DateTime)) { Value = correspondenceRecord.CorrespondenceSentOnDate };
            parameters[24] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.EXPIRY_DATE_I, typeof(DateTime)) { Value = correspondenceRecord.ExpiryDate };
            parameters[25] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.EXPIRY_DATEPERIOD_I, typeof(DateTime)) { Value = correspondenceRecord.ExpiryDatePeriod };
            parameters[26] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.BM_EXPIRY_PERIOD_I, typeof(DateTime)) { Value = correspondenceRecord.BMExpiryPeriod };
            parameters[27] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.CGO_RM_IDS_I, typeof(string)) { Value = GetRMsOracleGuids(correspondenceRecord.RejectionMemoIds) };
            parameters[28] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.CGO_CORR_ATTACHMENT_IDS_I, typeof(string)) { Value = GetRMsOracleGuids(corrAttachmentNetGuid) };
            parameters[29] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.CORRESPONDENCE_ID_O, typeof(Guid));
            parameters[30] = new ObjectParameter(CargoCorrespondenceRepositoryConstants.OPERATION_STATUS_INDICATOR_O, typeof(int));

            /* call sp */
            ExecuteStoredProcedure(CargoCorrespondenceRepositoryConstants.ProcCreateCGOCorrespondenceSP, parameters);

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
                    if (returnValue != -1)
                    {
                        correspondenceRecord.Id = (Guid)parameters[29].Value;
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
    public CargoCorrespondence GetOnlyCorrespondenceUsingLoadStrategy(Guid? correspondenceId = null, long? correspondenceNumber = null, int? correspondenceStage = null)
    {
        var entities = new string[]
                       {
                         LoadStrategy.Entities.Correspondence
                       };

        List<CargoCorrespondence> correspondences = GetCorrespondenceLS(new LoadStrategy(string.Join(",", entities)),
                                                                   correspondenceId: correspondenceId,
                                                                   correspondenceNumber: correspondenceNumber,
                                                                   correspondenceStage: correspondenceStage);
        if (correspondences == null || correspondences.Count == 0) return null;
        else if (correspondences.Count > 1) throw new ApplicationException("Multiple records found");
        else return correspondences[0];
    }

    public IQueryable<CargoCorrespondence> GetLastRespondedCorrespondene(System.Linq.Expressions.Expression<Func<CargoCorrespondence, bool>> where)
    {
        return EntityObjectSet.Where(where);
    }
  }
}
