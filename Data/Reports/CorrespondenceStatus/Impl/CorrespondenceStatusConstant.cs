using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iata.IS.Data.Reports.CorrespondenceStatus.Impl
{
    /// <summary>
    /// This class is used to hold all the constants for the correspondence status
    /// </summary>
   public class CorrespondenceStatusConstant
    {
        public const string FromDate = "FROM_DATE_I";
        public const string ToDate = "TO_DATE_I";
        public const string CorrRefNo = "CORS_REFRENCE_NO";
        public const string InitMemberCode = "CORS_INIT_MEMBERCODE_I";
        public const string FromMemberCode = "FROM_MEMBER_ID_I";
        public const string ToMemberCode = "TO_MEMBER_ID_I";
        public const string LoginMemberId = "LOGIN_MEMBER_ID_I";
        public const string AuthorityToBill = "AUTHORITYTO_BILL_I";
        public const string CorrStatus = "CORR_STATUS_I";
        public const string CorrSubStatus = "CORR_SUB_STATUS_I";
        public const string CorrStage = "CORR_STAGE_I";
        public const string ExpiryDays = "CORR_EXPIRY_DAYS_I";
        public const string ChargeCategory = "CORR_CHARGE_CATEGORY_I";
        public const string MiscCorrespondenceStatus = "GetMiscCorrespondenceStatus";
        public const string PaxCorrspondenceStatus = "GetCorrespondenceStatus";
        public const string CgoCorrspondenceStatus = "GetCGOCorrespondenceStatus";
        //CMP526 - Passenger Correspondence Identifiable by Source Code
        public const string PaxSourceCode = "SOURCE_CODE_I";

    }// End CorrespondenceStatusConstant
}// End namespace
