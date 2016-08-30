using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Pax
{
  public class CreditMemoCouponGrid : GridBase
  {
    public CreditMemoCouponGrid(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {

    }

    /// <summary>
    /// Initializes columns for grid
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid == null) return;
      _grid.Columns = new List<JQGridColumn> {
                                               GridColumnHelper.ActionColumn("Id", 90),
                                               GridColumnHelper.NumberColumn("TicketOrFimIssuingAirline", "Tkt. Issuing Airline",90,"{0:D3}"),
                                               //SCP139115 - In view invoice screen, ticket number is not a sortable field
                                               GridColumnHelper.NumberColumn("TicketDocOrFimNumber", "Tkt./Doc. No.",isSortable:true),
                                               GridColumnHelper.NumberColumn("TicketOrFimCouponNumber", "Coupon No."),
                                               GridColumnHelper.TextColumn("FromToAirport", "From-To", 80),
                                               GridColumnHelper.AmountColumn("GrossAmountCredited", "Gross Fare Value"),
                                               GridColumnHelper.AmountColumn("IscAmountBilled", "ISC Amt."),
                                               GridColumnHelper.AmountColumn("OtherCommissionBilled", "Other Comm. Amt."),
                                               GridColumnHelper.AmountColumn("UatpAmountBilled", "UATP Amt."),
                                               GridColumnHelper.AmountColumn("HandlingFeeAmount", "Handling Fee Amt."),
                                               GridColumnHelper.AmountColumn("TaxAmount", "Tax Amt."),
                                               GridColumnHelper.AmountColumn("VatAmount", "VAT Amt."),
                                               GridColumnHelper.AmountColumn("NetAmountCredited", "Net Credited Amt."),

                                             };

      var formatter = new CustomFormatter() { FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID) };
      _grid.Columns[0].Formatter = formatter;
    }
  }
}