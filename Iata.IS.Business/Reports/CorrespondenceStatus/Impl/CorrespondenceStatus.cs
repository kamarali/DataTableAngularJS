﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Data.Reports.CorrespondenceStatus;
using Iata.IS.Model.Reports.CorrespondenceStatus;

namespace Iata.IS.Business.Reports.CorrespondenceStatus.Impl
{
    /// <summary>
    /// This class is implemented for returns correspondence status
    /// </summary>
   public class CorrespondenceStatus : ICorrespondenceStatus
    {
       /// <summary>
       /// Get and set property of interface IcorrespondenceStatusData
       /// </summary>
       public ICorrespondenceStatusData IcorrespondenceStatusData { get; set; }

       /// <summary>
       /// Set property of Correspondence status interface
       /// </summary>
       /// <param name="icorrespondenceStatus">Interface IcorrepondenceStatusData</param>
       public CorrespondenceStatus(ICorrespondenceStatusData icorrespondenceStatus)
       {
           IcorrespondenceStatusData = icorrespondenceStatus;
       }// End CorrespondenceStatus

       /// <summary>
       /// Return Correspondence status of the members
       /// </summary>
       /// <param name="fromDate">Status of correspondence from the current date</param>
       /// <param name="toDate">Status of correspondence till to date</param>
       /// <param name="refrenceNo">Refrence no of correspondence</param>
       /// <param name="initiatingMember">Initiating member id</param>
       /// <param name="frommemberId">Id of the from member</param>
       /// <param name="tommemberId">Id of the to member</param>
       /// <param name="loginMemberId">Login member id</param>
       /// <param name="isAuthorize">Value of the authorize</param>
       /// <param name="corrStatus">Correspondence status</param>
       /// <param name="corrSubStatus">Correspondence sub status</param>
       /// <param name="corrStage">Correspondance stage</param>
       /// <param name="expiryDays">Expiry date</param>
       /// <param name="chargeCategory">Charge category</param>
       /// <param name="category">category</param>
       /// <param name="sourceCode">sourceCode</param>
       /// <returns> List of correspondence</returns>
       //CMP526 - Passenger Correspondence Identifiable by Source Code
       public List<CorrespondenceStatusModel> GetCorrespondenceDetails(DateTime fromDate, DateTime toDate, Int64 refrenceNo, int initiatingMember, int frommemberId, int tommemberId, int loginMemberId, int isAuthorize, int corrStatus, int corrSubStatus, int corrStage, int expiryDays, int chargeCategory, string category, int sourceCode)
       {
           // Getcorrespondencedetails from data
           return IcorrespondenceStatusData.GetCorrespondenceDetails(fromDate, toDate, refrenceNo, initiatingMember,
                                                              frommemberId, tommemberId, loginMemberId, isAuthorize,
                                                              corrStatus, corrSubStatus, corrStage, expiryDays,chargeCategory, category, sourceCode);
       }// End GetCorrespondenceDetails
      
    }// End CorrespondenceStatus class
}// end namespace
