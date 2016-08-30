using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Pax
{
  public class FormCCouponGrid : GridBase
  {
    public FormCCouponGrid(string gridId, string dataUrl)
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
                        {
                       new JQGridColumn
                                             {
                                                DataField="Id",
                                                Width=55,
                                                HeaderText = "Actions",
                                                TextAlign = TextAlign.Left,                                                
                                             },
                                             GridColumnHelper.TextColumn("ProvisionalInvoiceNumber","Prov. Inv. No.",70),
                                             GridColumnHelper.CustomColumn("BatchNumberOfProvisionalInvoice","Prov. Inv. Batch No.",70,TextAlign.Right),
                                             GridColumnHelper.CustomColumn("RecordSeqNumberOfProvisionalInvoice","Prov. Inv. Seq. No.",70,TextAlign.Right),
                                            GridColumnHelper.NumberColumn("TicketIssuingAirline","Tkt. Issuing Airline", dataFormatString: "{0:000}"),
                                            GridColumnHelper.NumberColumn("DocumentNumber","Tkt./Doc. Number"),
                                            GridColumnHelper.CustomColumn("CouponNumber","Coupon Number",50,TextAlign.Right),
                                            GridColumnHelper.TextColumn("ETicketIndicator", "E-Ticket Indicator", 60),
                                            GridColumnHelper.AmountColumn("GrossAmountAlf","Gross Amount/ALF"),
                                            GridColumnHelper.TextColumn("ReasonCode","Reason Code",70),
                                            GridColumnHelper.TextColumn("ReasonCodeDescription","Reason Description",190)
                        };
      var formatter = new CustomFormatter() { FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID) };
      _grid.Columns[0].Formatter = formatter;      
    }
  }
}