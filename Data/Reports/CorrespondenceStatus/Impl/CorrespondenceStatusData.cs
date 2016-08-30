using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.Enums;
using Iata.IS.Model.Reports.CorrespondenceStatus;

namespace Iata.IS.Data.Reports.CorrespondenceStatus.Impl
{
    /// <summary>
    /// Call stored procedure which returns correspondence status report status
    /// </summary>
    public class CorrespondenceStatusData : Repository<InvoiceBase> , ICorrespondenceStatusData
    {
        //CMP526 - Passenger Correspondence Identifiable by Source Code
        public List<CorrespondenceStatusModel> GetCorrespondenceDetails(DateTime fromDate, DateTime toDate, Int64 refrenceNo, int initiatingMember, int frommemberId, int tommemberId, int loginMemberId, int isAuthorize, int corrStatus, int corrSubStatus, int corrStage, int expiryDays, int chargeCategory, string category, int sourceCode)
        {
            if (category == Convert.ToString((int)BillingCategoryType.Pax))
            {

                //CMP526 - Passenger Correspondence Identifiable by Source Code
                var parameters = new ObjectParameter[14];

                parameters[0] = new ObjectParameter(CorrespondenceStatusConstant.FromDate, fromDate);
                parameters[1] = new ObjectParameter(CorrespondenceStatusConstant.ToDate, toDate);
                parameters[2] = new ObjectParameter(CorrespondenceStatusConstant.CorrRefNo, refrenceNo);
                parameters[3] = new ObjectParameter(CorrespondenceStatusConstant.InitMemberCode, initiatingMember);
                parameters[4] = new ObjectParameter(CorrespondenceStatusConstant.FromMemberCode, frommemberId);
                parameters[5] = new ObjectParameter(CorrespondenceStatusConstant.ToMemberCode, tommemberId);
                parameters[6] = new ObjectParameter(CorrespondenceStatusConstant.LoginMemberId, loginMemberId);
                parameters[7] = new ObjectParameter(CorrespondenceStatusConstant.AuthorityToBill, isAuthorize);
                parameters[8] = new ObjectParameter(CorrespondenceStatusConstant.CorrStatus, corrStatus);
                parameters[9] = new ObjectParameter(CorrespondenceStatusConstant.CorrSubStatus, corrSubStatus);
                parameters[10] = new ObjectParameter(CorrespondenceStatusConstant.CorrStage, corrStage);
                parameters[11] = new ObjectParameter(CorrespondenceStatusConstant.ExpiryDays, expiryDays);
                parameters[12] = new ObjectParameter(CorrespondenceStatusConstant.ChargeCategory, chargeCategory);
                //CMP526 - Passenger Correspondence Identifiable by Source Code
                parameters[13] = new ObjectParameter(CorrespondenceStatusConstant.PaxSourceCode, sourceCode);

                var list =ExecuteStoredFunction<CorrespondenceStatusModel>(CorrespondenceStatusConstant.PaxCorrspondenceStatus,parameters);
                return list.ToList();
            }
            else if (category == Convert.ToString((int)BillingCategoryType.Cgo))
            {

                var parameters = new ObjectParameter[13];

                parameters[0] = new ObjectParameter(CorrespondenceStatusConstant.FromDate, fromDate);
                parameters[1] = new ObjectParameter(CorrespondenceStatusConstant.ToDate, toDate);
                parameters[2] = new ObjectParameter(CorrespondenceStatusConstant.CorrRefNo, refrenceNo);
                parameters[3] = new ObjectParameter(CorrespondenceStatusConstant.InitMemberCode, initiatingMember);
                parameters[4] = new ObjectParameter(CorrespondenceStatusConstant.FromMemberCode, frommemberId);
                parameters[5] = new ObjectParameter(CorrespondenceStatusConstant.ToMemberCode, tommemberId);
                parameters[6] = new ObjectParameter(CorrespondenceStatusConstant.LoginMemberId, loginMemberId);
                parameters[7] = new ObjectParameter(CorrespondenceStatusConstant.AuthorityToBill, isAuthorize);
                parameters[8] = new ObjectParameter(CorrespondenceStatusConstant.CorrStatus, corrStatus);
                parameters[9] = new ObjectParameter(CorrespondenceStatusConstant.CorrSubStatus, corrSubStatus);
                parameters[10] = new ObjectParameter(CorrespondenceStatusConstant.CorrStage, corrStage);
                parameters[11] = new ObjectParameter(CorrespondenceStatusConstant.ExpiryDays, expiryDays);
                parameters[12] = new ObjectParameter(CorrespondenceStatusConstant.ChargeCategory, chargeCategory);

                var list = ExecuteStoredFunction<CorrespondenceStatusModel>(CorrespondenceStatusConstant.CgoCorrspondenceStatus, parameters);
                return list.ToList();
            }
            else
            {
                var parameters = new ObjectParameter[13];

                parameters[0] = new ObjectParameter(CorrespondenceStatusConstant.FromDate, fromDate);
                parameters[1] = new ObjectParameter(CorrespondenceStatusConstant.ToDate, toDate);
                parameters[2] = new ObjectParameter(CorrespondenceStatusConstant.CorrRefNo, refrenceNo);
                parameters[3] = new ObjectParameter(CorrespondenceStatusConstant.InitMemberCode, initiatingMember);
                parameters[4] = new ObjectParameter(CorrespondenceStatusConstant.FromMemberCode, frommemberId);
                parameters[5] = new ObjectParameter(CorrespondenceStatusConstant.ToMemberCode, tommemberId);
                parameters[6] = new ObjectParameter(CorrespondenceStatusConstant.LoginMemberId, loginMemberId);
                parameters[7] = new ObjectParameter(CorrespondenceStatusConstant.AuthorityToBill, isAuthorize);
                parameters[8] = new ObjectParameter(CorrespondenceStatusConstant.CorrStatus, corrStatus);
                parameters[9] = new ObjectParameter(CorrespondenceStatusConstant.CorrSubStatus, corrSubStatus);
                parameters[10] = new ObjectParameter(CorrespondenceStatusConstant.CorrStage, corrStage);
                parameters[11] = new ObjectParameter(CorrespondenceStatusConstant.ExpiryDays, expiryDays);
                parameters[12] = new ObjectParameter(CorrespondenceStatusConstant.ChargeCategory, chargeCategory);

                var list =ExecuteStoredFunction<CorrespondenceStatusModel>(CorrespondenceStatusConstant.MiscCorrespondenceStatus, parameters);
                return list.ToList();
            }

           
        }// end GetCorrespondenceDetails();
    }// End CorrespondenceStatusData class;
}// End namespace
