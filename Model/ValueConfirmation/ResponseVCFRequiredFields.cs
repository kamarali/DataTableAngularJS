using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileHelpers;

namespace Iata.IS.Model.ValueConfirmation
{
  [FixedLengthRecord] 
 public class ResponseVCFRequiredFields
  {
    //file header field
    [FieldIgnored]
    public string Sour;
    [FieldIgnored]
    public string VcfKey;
    [FieldIgnored]
    public string RecordCount;

    // arln pair field
    [FieldIgnored]
    public string Vast;
     [FieldIgnored]
    public string RecordSequenceNo;

    [FieldIgnored]
    public string GroupHeaderLineNo;
     [FieldIgnored]
    public int TotalRecords;
     [FieldIgnored]
    public int TotalArlnSeparatorRecords;

    [FieldFixedLength(2)]
    public string Blank1;
    [FieldFixedLength(10)]
    public string Blank2;

    // to identify cuopon uniqly
    
    [FieldFixedLength(4)] 
    public string BillingAirline;
    [FieldFixedLength(4)] 
    public string BilledAirline ;
    [FieldFixedLength(10)] 
    public string InvoiceNumber ;
    [FieldFixedLength(6)] 
    public string BillingDate ;
    [FieldFixedLength(2)] 
    public string PeriodNumber ;
    [FieldFixedLength(5)] 
    public string BatchSequenceNumber ;
    [FieldFixedLength(5)] 
    public string RecordSequencewithinBatch ;

    // blank fields to skip initial fields in coupon record
    [FieldFixedLength(2)]
    public string Blank10;
    [FieldFixedLength(4)]
    public string Blank11;
    [FieldFixedLength(11)]
    public string Blank12;

    [FieldFixedLength(1)]
    public string Blank13;
    [FieldFixedLength(1)]
    public string Blank14;
    [FieldFixedLength(1)]
    public string Blank15;

    [FieldFixedLength(3)]
    public string Blank16;
    [FieldFixedLength(3)]
    public string Blank17;
    [FieldFixedLength(16)]
    public string Blank18;

    [FieldFixedLength(2)]
    public string Blank19;
    [FieldFixedLength(11)]
    public string Blank20;
    [FieldFixedLength(5)]
    public string Blank21;

    [FieldFixedLength(9)]
    public string Blank22;
    [FieldFixedLength(4)]
    public string Blank23;
    [FieldFixedLength(11)]
    public string Blank24;

    [FieldFixedLength(11)]
    public string Blank25;
    [FieldFixedLength(1)]
    public string Blank26;
    [FieldFixedLength(2)]
    public string Blank27;

    [FieldFixedLength(40)]
    [FieldTrim(TrimMode.Right)]
    public string CouponId;

    [FieldFixedLength(22)]
    public string Filler3;
  
    // Coupon level record, to update in pax coupon record
    [FieldFixedLength(1)] 
    public string PMIValidated ;
    [FieldFixedLength(2)] 
    public string AgreementIndicatorValidated ;
    [FieldFixedLength(2)] 
    public string NfpReasionCodeValidated ;

    [FieldFixedLength(3)] 
    public string ProrateMethodology ;
    [FieldFixedLength(2)] 
    public string MonthofSale ;
    [FieldFixedLength(2)] 
    public string YearofSale ;

    [FieldFixedLength(11)]
    public string BilledProrateAmount ;
    [FieldFixedLength(11)]
    public string ProrateAmount ;
    //[FieldFixedLength(3)] 
   // public string ProrateAmountBaseCurrency ;
    [FieldFixedLength(3)] 
    public string BaseCurrencyofDPRO ;

    [FieldFixedLength(11)] 
    public string BilledTotalTaxAmount ;
    [FieldFixedLength(11)] 
    public string TotalTaxAmount ;
    [FieldFixedLength(3)] 
    public string PublishedTaxAmountCurrency1 ;

    [FieldFixedLength(3)] 
    public string PublishedTaxAmountCurrency2 ;
    [FieldFixedLength(3)] 
    public string PublishedTaxAmountCurrency3 ;
    [FieldFixedLength(3)] 
    public string PublishedTaxAmountCurrency4 ;

    [FieldFixedLength(5)] 
    public string IscPercentage ;
    [FieldFixedLength(11)] 
    public string BilledHandlingFeeAmount ;
    [FieldFixedLength(11)] 
    public string HandlingFeeAmount ;

    [FieldFixedLength(3)] 
    public string HandlingFeeBaseCurrency ;
    [FieldFixedLength(4)] 
    public string UATPPercentage ;
    [FieldFixedLength(25)]
    public string Filler;
    [FieldFixedLength(1)] 
    public string ReasonCode ;
    [FieldFixedLength(61)]
    public string Filler1;

  }
}
