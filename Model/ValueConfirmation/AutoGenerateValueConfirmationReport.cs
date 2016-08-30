using System;
namespace Iata.IS.Model.ValueConfirmation
{
  public class AutoGenerateValueConfirmationReport
  {
    public string BillingAirline { get; set; }
    public string BillingAirlineNumber { get; set; }
    public int BillingAirlineMonth { get; set; }
    public int BillingAirlinePeriod { get; set; }
    public int BillingAirlineYear { get; set; }
    public Guid InvoiceNo { get; set; }

    public string BilliedAirline { get; set; }
    public string BilledAirlineNumber { get; set; }
    public int MonthOfSale { get; set; }
    public int YearOfSale { get; set; }
    public string IssuingAirline { get; set; }

    public int DocumentNumber { get; set; }
    public int CouponNumber { get; set; }
    public string OriginalPMI { get; set; }
    public string ValidatedPMI { get; set; }
    public string AggrementIndicatorSupplied { get; set; }

    public string AggrementIndicatorValidated { get; set; }
    public string ProrateMethodologySupplied { get; set; }
    public string ProrateMethodologyValidated { get; set; }
    public string NfpReasonCodeSupplied { get; set; }
    public string NfpReasonCodeValidated { get; set; }

    public decimal BilledAmtUsdAtpco { get; set; }
    //TO DO
    public decimal ProrateAmtAtpco { get; set; }
    public string ProrateAmtBaseCurAtpco { get; set; }
    public string ProrateAmtBaseCurSupplied { get; set; }

    public decimal BilledTotalTaxAmtUsdAtpco { get; set; }
    public decimal TotalTaxAmountAtpco { get; set; }

    public string PublTaxAmtCurrency1Atpco { get; set; }
    public string PublTaxAmtCurrency2Atpco { get; set; }
    public string PublTaxAmtCurrency3Atpco { get; set; }
    public string PublTaxAmtCurrency4Atpco { get; set; }
    public decimal InterlineServChargePer { get; set; }

    public int IscFeePercentageAtpco { get; set; }
    public decimal BilledHandlFeeAmtUsdAtpco { get; set; }
    public decimal HandlingFeeAmtAtpco { get; set; }
    public string HandlingFeeBaseCurAtpco { get; set; }
    public decimal UatpPercentage { get; set; }
    public int UatpPercentageAtpco { get; set; }

    public string AtpcoReasonCode { get; set; } 
     
  }
}
