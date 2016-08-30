using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Iata.IS.Model.AchRecapSheet
{
    //public class AchRecapSheetDetail
    //{

    //  public string ClearancePeriod { get; set; }

    //  public string DebtorClearingHouse { get; set; }

    //  public string CreditorAccountCode { get; set; }

    //  public string CreditorDesignator { get; set; }

    //  public string DebtorAccountCode { get; set; }

    //  public string DebtorDesignator { get; set; }

    //  public decimal PaxAmtInUSD { get; set; }

    //  public decimal PaxAmtInCAD { get; set; }

    //  public decimal CgoAmtInUSD { get; set; }

    //  public decimal CgoAmtInCAD { get; set; }

    //  public decimal UATPAmtInUSD { get; set; }

    //  public decimal UATPAmtInCAD { get; set; }

    //  public decimal MiscAmtInUSD { get; set; }

    //  public decimal MiscAmtInCAD { get; set; }

    //  public string PaxAmtInUSDSign 
    //  { 
    //    get
    //    {
    //      return (PaxAmtInUSD >= 0 ? "+" : "-");
    //    }
    //  }

    //  public string PaxAmtInCADSign
    //  {
    //    get
    //    {
    //      return (PaxAmtInCAD >= 0 ? "+" : "-");
    //    }
    //  }

    //  public string CgoAmtInUSDSign
    //  {
    //    get
    //    {
    //      return (CgoAmtInUSD >= 0 ? "+" : "-");
    //    }
    //  }

    //  public string CgoAmtInCADSign
    //  {
    //    get
    //    {
    //      return (CgoAmtInCAD >= 0 ? "+" : "-");
    //    }
    //  }

    //  public string UATPAmtInUSDSign
    //  {
    //    get
    //    {
    //      return (UATPAmtInUSD >= 0 ? "+" : "-");
    //    }
    //  }

    //  public string UATPAmtInCADSign
    //  {
    //    get
    //    {
    //      return (UATPAmtInCAD >= 0 ? "+" : "-");
    //    }
    //  }

    //  public string MiscAmtInUSDSign
    //  {
    //    get
    //    {
    //      return (MiscAmtInUSD >= 0 ? "+" : "-");
    //    }
    //  }

    //  public string MiscAmtInCADSign
    //  {
    //    get
    //    {
    //      return (MiscAmtInCAD >= 0 ? "+" : "-");
    //    }
    //  }

    //  //public Guid InvoiceId { get; set; }

    //}

    public class AchRecapSheetDetail
    {
        public string ClearancePeriod { get; set; }

        public string DebtorClearingHouse { get; set; }

        public string CreditorAccountCode { get; set; }

        public string CreditorDesignator { get; set; }

        public string DebtorAccountCode { get; set; }

        public string DebtorDesignator { get; set; }

        public string CurrencyCode { get; set; }

        public decimal PaxAmt { get; set; }

        public decimal CgoAmt { get; set; }

        public decimal UatpAmt { get; set; }

        public decimal MiscAmt { get; set; }
    }

    [XmlRoot(ElementName = "ACHSettlementTransmissionRecapSummary")]
    public class AchSettlementTransmissionRecapSummary
    {
        public AchRecapSummaryHeader Header { get; set; }

        [XmlArrayItemAttribute("Detail", IsNullable = false)]
        public List<AchRecapSummaryDetail> Details { get; set; }
    }

    public class AchRecapSummaryHeader
    {
        public string FileCreationDate { get; set; }
      
        public ulong HashTotalAmount { get; set; }

        public int TotalNoOfDetailRecords { get; set; }
    }

    public class AchRecapSummaryDetail
    {
        public string ClearancePeriod { get; set; }
       
        public AchRecapSummaryDetailCreditorMember CreditorMember { get; set; }
     
        public AchRecapSummaryDetailDebtorMember DebtorMember { get; set; }
      
        public string BillingCategory { get; set; }
        
        public string ClearanceCurrencyCode { get; set; }
        
        public decimal TotalAmountInClearanceCurrency { get; set; }
        
    }

    public class AchRecapSummaryDetailCreditorMember
    {
        public string Designator { get; set; }
      
        public string Code { get; set; }  
        
    }

    public class AchRecapSummaryDetailDebtorMember
    {
        public string Designator { get; set; }

        public string Code { get; set; }

        public string DebtorMemberClearingHouse { get; set; }
    }
}
