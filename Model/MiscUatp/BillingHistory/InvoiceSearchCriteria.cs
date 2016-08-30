using System;
using Iata.IS.Model.Base;

namespace Iata.IS.Model.MiscUatp.BillingHistory
{
  [Serializable]
  public class InvoiceSearchCriteria : InvoiceSearchCriteriaBase
  {
    public int ChargeCategoryId { get; set; }

      //CMP #655: IS-WEB Display per Location ID
        public string MemberLocation { get; set; }
  }
}