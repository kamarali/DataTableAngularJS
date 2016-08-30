using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Pax
{
  /// <summary>
  /// Grid for Rejection Memo Coupon Breakdown List
  /// </summary>
  public class RejectionMemoCouponBreakdownGrid : GridBase
  {
    public RejectionMemoCouponBreakdownGrid(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }

    /// <summary>
    /// Initializes columns for grid
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid == null) return;
      _grid.Columns = new List<JQGridColumn>
                        { GridColumnHelper.ActionColumn("Id",90),
                          GridColumnHelper.NumberColumn("TicketOrFimIssuingAirline","Tkt. Issuing Airline", dataFormatString: "{0:D3}"),
                          //SCP139115 - In view invoice screen, ticket number is not a sortable field
                          GridColumnHelper.NumberColumn("TicketDocOrFimNumber", "Tkt./Doc. Number",isSortable:true),
                          GridColumnHelper.NumberColumn("TicketOrFimCouponNumber","Coupon Number"),
                          GridColumnHelper.TextColumn("FromToAirport","From-To",80),
                          GridColumnHelper.AmountColumn("GrossAmountDifference","Gross Fare Value"),
                          GridColumnHelper.AmountColumn("IscDifference","ISC Amt."),
                          GridColumnHelper.AmountColumn("OtherCommissionDifference","Other Comm. Amt."),
                          GridColumnHelper.AmountColumn("UatpDifference","UATP Amt."),                                             
                          GridColumnHelper.AmountColumn("HandlingDifference","Handling Fee Amt."),
                          GridColumnHelper.AmountColumn("TaxAmountDifference","Tax Amt."),                                             
                          GridColumnHelper.AmountColumn("VatAmountDifference","VAT Amt."),
                          GridColumnHelper.AmountColumn("NetRejectAmount","Net Reject Amt.")
                        };

      var formatter = new CustomFormatter { FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID) };
      _grid.Columns[0].Formatter = formatter;
    }
  }
}