using System;
using System.Collections.Generic;
using Iata.IS.Data.Reports.ValidationDetails;
using Iata.IS.Model.Reports.ValidationDetails;


namespace Iata.IS.Business.Reports.ValidationDetails.Impl
{
  class ValidationDetailsImpl : IValidationDetails
  {
    private IValidationDetailsData ValidationDetailsParam { get; set; }

    public ValidationDetailsImpl(IValidationDetailsData validationDetailsData)
    {
      ValidationDetailsParam = validationDetailsData;
    }

        public List<ValidationDetailsModel> GetValidationDetails(int clearanceMonth, int billingPeriod,  string fileName, DateTime billingSubmissiondate,  string BilledOrganisation , string InvoiceNo , string category,  string memberCode)
        {
            return ValidationDetailsParam.GetValidationDetails(clearanceMonth, billingPeriod,
                                                                fileName, billingSubmissiondate,
                                                                 BilledOrganisation , InvoiceNo , category , memberCode);
        }
    }
  }

