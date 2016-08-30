using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Reports.ValidationSummary;

namespace Iata.IS.Data.Reports.ValidationSummary.Impl
{
    class PaxValidationSummary : Repository<InvoiceBase> , IPaxValidationSummary
    {
        public List<PassengerValidationSummary> GetPaxValidationSummary(int clearanceMonth,int billingPeriod, string fileName , DateTime billingSubmissiondate, string memberCode , string category)
        {
            var parameters = new ObjectParameter[6];

            parameters[0] = new ObjectParameter(PaxValidationSummaryConstants.BillingMonth, clearanceMonth );
            parameters[1] = new ObjectParameter(PaxValidationSummaryConstants.BillingPeriod, billingPeriod);
            parameters[2] = new ObjectParameter(PaxValidationSummaryConstants.FileName,  fileName );
            parameters[3] = new ObjectParameter(PaxValidationSummaryConstants.BillingSubDtae,billingSubmissiondate );
            parameters[4] = new ObjectParameter(PaxValidationSummaryConstants.MemberCode, memberCode);
            parameters[5] = new ObjectParameter(PaxValidationSummaryConstants.Category, category);

           var list = ExecuteStoredFunction<PassengerValidationSummary>(PaxValidationSummaryConstants.GetPaxValidationSummary, parameters);
           
            
            return list.ToList();
        }
    }
}
