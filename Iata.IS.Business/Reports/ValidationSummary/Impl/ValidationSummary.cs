using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iata.IS.Business.MemberProfile;
using Iata.IS.Data.Reports.ValidationSummary;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Data.MemberProfile;
using Iata.IS.Model.Reports.ValidationSummary;


namespace Iata.IS.Business.Reports.ValidationSummary.Impl
{
    class ValidationSummary : IValidationSummary
    {
        private IPaxValidationSummary PaxValidationSummary { get; set; }

        public ValidationSummary(IPaxValidationSummary paxValidationSummary)
        {
            PaxValidationSummary = paxValidationSummary;
        }

        public List<PassengerValidationSummary> GetPaxValidationSummary(int clearanceMonth,int billingPeriod, string fileName , DateTime billingSubmissiondate, string memberCode , string category)
        {
            return PaxValidationSummary.GetPaxValidationSummary(clearanceMonth,billingPeriod,
                                                                 fileName, billingSubmissiondate,
                                                                 memberCode, category);
        }
    }
}
