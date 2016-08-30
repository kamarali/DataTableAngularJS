using System;
using System.Collections.Generic;
using Iata.IS.Model.AchRecapSheet;
using Iata.IS.Model.Calendar;

namespace Iata.IS.Data.ACHRecapSheet
{
  public interface IACHRecapSheetRepository
  {

      List<AchRecapSheetDetail> GetACHRecapSheetData(BillingPeriod billingPeriod, int achRecapSheetRegenerationFleg);
    
      void UpdateInvoiceACHRecapSheetStatus(BillingPeriod billingPeriod, string invoiceRecapSheetStatus);

      void UpdateACHRecapSheetData(BillingPeriod billingPeriod, string FileName, string FileLocation, int ACHMemberId);
      
  }
}
