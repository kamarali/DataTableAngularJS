using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using Iata.IS.Data.Impl;
using Iata.IS.Model.Base;
using Iata.IS.Model.MemberProfile;
using Iata.IS.Model.Reports.ValidationDetails;

namespace Iata.IS.Data.Reports.ValidationDetails.Impl
{
   public class ValidationDetailsDataImpl : Repository<InvoiceBase> , IValidationDetailsData
    {
        public List<ValidationDetailsModel> GetValidationDetails(int clearanceMonth, int billingPeriod, string fileName, DateTime billingSubmissiondate, string BilledOrganisation, string InvoiceNo, string category, string memberCode)
        {
            var parameters = new ObjectParameter[8];

            parameters[0] = new ObjectParameter(ValidationDetailsDataImplConstants.BillingMonth, clearanceMonth);
            parameters[1] = new ObjectParameter(ValidationDetailsDataImplConstants.BillingPeriod, billingPeriod);
            parameters[2] = new ObjectParameter(ValidationDetailsDataImplConstants.FileName, fileName);
            parameters[3] = new ObjectParameter(ValidationDetailsDataImplConstants.BillingSubDtae, billingSubmissiondate);
            parameters[4] = new ObjectParameter(ValidationDetailsDataImplConstants.BilledOrganisation, BilledOrganisation);
            parameters[5] = new ObjectParameter(ValidationDetailsDataImplConstants.InvoiceNumber, InvoiceNo);
            parameters[6] = new ObjectParameter(ValidationDetailsDataImplConstants.Category, category);
            parameters[7] = new ObjectParameter(ValidationDetailsDataImplConstants.MemberCode, memberCode);


            var list = ExecuteStoredFunction<ValidationDetailsModel>(ValidationDetailsDataImplConstants.GetValidationDetails, parameters);
           
            
            return list.ToList();
        }
    }
}
