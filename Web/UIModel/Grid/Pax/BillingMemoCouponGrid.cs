using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Pax
{
  public class BillingMemoCouponGrid : GridBase
  {
    public BillingMemoCouponGrid(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }

    /// <summary>
    /// Initializes columns for grid
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn>
                            {
                              GridColumnHelper.ActionColumn("Id",90),
                                             GridColumnHelper.NumberColumn("TicketOrFimIssuingAirline","Tkt. Issuing Arln.", 90,"{0:D3}"),
                                             //SCP139115 - In view invoice screen, ticket number is not a sortable field
                                             GridColumnHelper.NumberColumn("TicketDocOrFimNumber","Ticket/Doc. No.",isSortable:true),
                                             GridColumnHelper.NumberColumn("TicketOrFimCouponNumber","Coupon No."),
                             //                GridColumnHelper.TextColumn("FromToAirport","From-To",80),
                                              // TODO: From-to Column
                                             GridColumnHelper.TextColumn("FromAirportOfCoupon","From",80),
                                             GridColumnHelper.TextColumn("ToAirportOfCoupon","To",80),
                                             GridColumnHelper.AmountColumn("GrossAmountBilled","Gross Fare Value"),
                                             GridColumnHelper.AmountColumn("IscAmountBilled","ISC Amt."),
                                             GridColumnHelper.CustomColumn("OtherCommissionBilled","Other Comm. Amt.",120,TextAlign.Right, new NumberFormatter(){DecimalPlaces = 2, DecimalSeparator = ".", ThousandsSeparator = ","}),
                                             GridColumnHelper.AmountColumn("UatpAmountBilled","UATP Amt."),
                                             GridColumnHelper.AmountColumn("HandlingFeeAmount","Handling Fee Amt."),
                                             GridColumnHelper.AmountColumn("TaxAmount","Tax Amt."),                                             
                                             GridColumnHelper.AmountColumn("VatAmount","VAT Amt."),
                                             GridColumnHelper.AmountColumn("NetAmountBilled","Net Billed Amt."),

                            };
        
        
        CustomFormatter formatter = new CustomFormatter();
        formatter.FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID);
        _grid.Columns[0].Formatter = formatter;
      }
    }
  }
}