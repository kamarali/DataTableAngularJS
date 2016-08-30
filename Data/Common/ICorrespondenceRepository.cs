using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Data.Common
{
  public interface ICorrespondenceRepository
  { 
    
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
     int[] CanCorrespondenceClose(int billingcategoryId, Guid correspondenceId);

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
    int CloseCorrespondence(string correspondenceNo, string correspondenceStage, string correspondenceStatus,
                            string correspondenceSubStatus, int scenarioNo, int billingCategoryId,
                            string acceptanceComment, int acceptanceUserId, DateTime acceptanceDateTime);

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
    bool IsCorrespondenceEligibleForReply(int billingcategoryId, Guid correspondenceId, int loggedInUserId, int loggedInMemberId, int draftPermissionId);

  }
}
