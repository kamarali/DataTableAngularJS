using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Pax;

namespace Iata.IS.Data.Common.Impl
{
  public class CorrespondenceRepository : Repository<Correspondence>, ICorrespondenceRepository
  {

    private const string ParamBillingCategoryId = "BILLING_CATEGORY_ID_I";
    private const string ParamCorrespondenceId = "CORRESPONDENCE_ID_I";
    private const string ParamCanClose = "CAN_CLOSE_O";
    private const string ParamSenarioIdOut = "SCENARIO_ID_O";
    private const string ParamCorrespondenceNo = "CORRESPONDENCE_NO_I";
    private const string ParamCorrespondenceStage = "CORRESPONDENCE_STAGE_I";
    private const string ParamCorrespondenceStatus = "CORRESPONDENCE_STATUS_I";
    private const string ParamCorrespondenceSubStatus = "CORRESPONDENCE_SUB_STATUS_I";
    private const string ParamSenarioId = "SCENARIO_ID_I";
    private const string ParamAcceptanceComment = "ACCEPTANCE_COMMENTS_I";
    private const string ParamAcceptanceDate = "ACCEPTANCE_DATE_I";
    private const string ParamAcceptanceUserId = "ACCEPTANCE_USER_ID_I";
    private const string ParamIsClosed = "IS_CLOSED_O";
    private const string CanCorrespondenceCloseProc = "CanCorrespondenceClose";
    private const string CloseCorrespondenceProc = "CloseCorrespondence";

    /// <summary>
    /// CMP 573
    /// User to be Able to Reply to a Correspondence by Clicking the URL in a Correspondence Alert.
    /// </summary>
    private const string ParamLoggedInUserId = "LOGGED_IN_USER_ID_I";
    private const string ParamLoggedInMemberId = "LOGGED_IN_MEMBER_ID_I";
    private const string ParamDraftPermissionId = "DRAFTPREMISSION_ID_I";
    private const string ParamIsEligibleForReply = "IS_ELIGIBLE_FOR_REPLY_O";
    private const string CorrespondenceEligibleForReply = "IsCorrespondenceEligibleForReply";

    
    /// <summary>
    /// CMP 527
    /// Check can correspondence close or not.
    /// </summary>
    /// <param name="billingcategoryId">billing category id.</param>
    /// <param name="correspondenceId">Correspondence id</param>
    /// <returns>
    /// true: if correspondence can able to close
    /// false: if correspondece can not close.
    /// </returns>
    public int[] CanCorrespondenceClose(int billingcategoryId, Guid correspondenceId)
    {
      try
      {
        var parameters = new ObjectParameter[4];
        parameters[0] = new ObjectParameter(ParamBillingCategoryId, typeof (int))
                          {
                            Value = billingcategoryId
                          };
        parameters[1] = new ObjectParameter(ParamCorrespondenceId, typeof (Guid))
                          {
                            Value = correspondenceId
                          };
        parameters[2] = new ObjectParameter(ParamCanClose, typeof(bool));

        parameters[3] = new ObjectParameter(ParamSenarioIdOut, typeof(int));

        ExecuteStoredProcedure(CanCorrespondenceCloseProc, parameters);

        var result = new int[2];

        result[(int)CorrespondenceCloseStatus.CorrespondenceCanClose] = Convert.ToInt16(parameters[2].Value);

        result[(int)CorrespondenceCloseStatus.CorrespondenceCloseScenario] = Convert.ToInt16(parameters[3].Value);

        return result;
      }
      catch (Exception)
      {
        return new int[2]{0,0};
      }
    }

   /// <summary>
   /// This is use to close correspondence 
   /// </summary>
   /// <param name="correspondenceNo">correspondence number</param>
    /// <param name="correspondenceStage">correspondence stage</param>
    /// <param name="correspondenceStatus">correspondence status</param>
    /// <param name="correspondenceSubStatus">correspondence sub status</param>
   /// <param name="scenarioNo">scenario number - 1/2/3/4/5/6/7</param>
   //SCENARIO 1: STAGE = 1,   STATUS = OPEN     AND  SUB-STATUS = SAVED
   //SCENARIO 2: STAGE = 1,   STATUS = OPEN     AND  SUB-STATUS = READY FOR SUBMIT
   //SCENARIO 3: STAGE = 1,   STATUS = EXPIRED  AND  SUB-STATUS = RESPONDED
   //SCENARIO 4: STAGE = >=2, STATUS = OPEN     AND  SUB-STATUS = RECEVIED/RESPONDED
   //SCENARIO 5: STAGE = >2,  STATUS = OPEN     AND  SUB-STATUS = SAVED
   //SCENARIO 6: STAGE = >2,  STATUS = OPEN     AND  SUB-STATUS = READY FOR SUBMIT
   //SCENARIO 7: STAGE = >2,  STATUS = EXPIRED  AND  SUB-STATUS = RESPONDED 
   /// <param name="billingCategoryId">billing category id</param>
   /// <param name="acceptanceComment">comments</param>
   /// <param name="acceptanceUserId">user id</param>
   /// <param name="acceptanceDateTime">datetime</param>
   /// <returns></returns>
    public int CloseCorrespondence(string correspondenceNo, string correspondenceStage, string correspondenceStatus, string correspondenceSubStatus, int scenarioNo, int billingCategoryId, string acceptanceComment, int acceptanceUserId, DateTime acceptanceDateTime)
    {
      try
      {
        var parameters = new ObjectParameter[10];
        parameters[0] = new ObjectParameter(ParamCorrespondenceNo, typeof(string))
        {
          Value = correspondenceNo
        };
        parameters[1] = new ObjectParameter(ParamCorrespondenceStage, typeof(int))
        {
          Value = correspondenceStage
        };
        parameters[2] = new ObjectParameter(ParamCorrespondenceStatus, typeof(int))
        {
          Value = correspondenceStatus
        };
        parameters[3] = new ObjectParameter(ParamCorrespondenceSubStatus, typeof(int))
        {
          Value = correspondenceSubStatus
        };
        parameters[4] = new ObjectParameter(ParamSenarioId, typeof(int))
        {
          Value = scenarioNo
        };
        parameters[5] = new ObjectParameter(ParamBillingCategoryId, typeof(int))
        {
          Value = billingCategoryId
        };
        parameters[6] = new ObjectParameter(ParamAcceptanceComment, typeof(string))
        {
          Value = acceptanceComment
        };
        parameters[7] = new ObjectParameter(ParamAcceptanceDate, typeof(DateTime))
        {
          Value = acceptanceDateTime
        };
        parameters[8] = new ObjectParameter(ParamAcceptanceUserId, typeof(int))
        {
          Value = acceptanceUserId
        };
       
        parameters[9] = new ObjectParameter(ParamIsClosed, typeof(bool));

        ExecuteStoredProcedure(CloseCorrespondenceProc, parameters);


        return Convert.ToInt16(parameters[9].Value);

      }
      catch (Exception)
      {
        return 0;
      }
    }

    /// <summary>
    /// CMP 573
    /// User to be Able to Reply to a Correspondence by Clicking the URL in a Correspondence Alert.
    /// </summary>
    /// <param name="billingcategoryId">billing category id.</param>
    /// <param name="correspondenceId">Correspondence id</param>
    /// <param name="loggedInUserId">CurrentlyLoggedInUser id</param>
    /// <param name="loggedInMemberId">CurrentlyLoggedInMember id</param>
    /// <param name="draftPermissionId">Draft Permission id</param>
    /// <returns>
    /// true: if correspondence is eligible for reply
    /// false: if correspondence is not eligible for reply
    /// </returns>
    public bool IsCorrespondenceEligibleForReply(int billingcategoryId, Guid correspondenceId, int loggedInUserId, int loggedInMemberId, int draftPermissionId)
    {
      try
      {
        var parameters = new ObjectParameter[6];

        parameters[0] = new ObjectParameter(ParamCorrespondenceId, typeof (Guid))
                          {
                            Value = correspondenceId
                          };

        parameters[1] = new ObjectParameter(ParamBillingCategoryId, typeof (int))
                          {
                            Value = billingcategoryId
                          };

        parameters[2] = new ObjectParameter(ParamLoggedInUserId, typeof (int))
                          {
                            Value = loggedInUserId
                          };
        parameters[3] = new ObjectParameter(ParamLoggedInMemberId, typeof (int))
                          {
                            Value = loggedInMemberId
                          };

        parameters[4] = new ObjectParameter(ParamDraftPermissionId, typeof (int))
                          {
                            Value = draftPermissionId
                          };

        parameters[5] = new ObjectParameter(ParamIsEligibleForReply, typeof (int));


        ExecuteStoredProcedure(CorrespondenceEligibleForReply, parameters);

        return Convert.ToBoolean(parameters[5].Value);

      }
      catch (Exception)
      {
        return false;
      }
    }

  }
}
