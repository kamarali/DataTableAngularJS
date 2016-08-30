using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Pax
{
  /// <summary>
  /// Grid for Form D list
  /// </summary>
  public class SamplingFormDGrid:GridBase
  {
    public SamplingFormDGrid(string gridId, string dataUrl, bool isCheckBoxRequired = false)
      : base(gridId, dataUrl, isCheckBoxRequired: isCheckBoxRequired)
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
                              GridColumnHelper.TextColumn("ProvisionalInvoiceNumber", "Prov. Inv. No.",70),
                              GridColumnHelper.SortableTextColumn("BatchNumberOfProvisionalInvoice", "Prov. Inv. Batch No.", 90),
                              GridColumnHelper.SortableTextColumn("RecordSeqNumberOfProvisionalInvoice","Prov. Inv. Seq. No.",90),
                              GridColumnHelper.NumberColumn("TicketIssuingAirline","Tkt. Issuing Airline"),
                              GridColumnHelper.NumberColumn("TicketDocNumber","Tkt. No."),
                              GridColumnHelper.NumberColumn("CouponNumber","Coupon No."),
                              GridColumnHelper.AmountColumn("EvaluatedGrossAmount","Gross Fare Value"),
                              GridColumnHelper.PercentColumn("IscPercent", "ISC Rate", 60),
                              GridColumnHelper.AmountColumn("IscAmount","ISC Amount"),
                              GridColumnHelper.PercentColumn("OtherCommissionPercent", "Other Comm. Rate", 80),
                              GridColumnHelper.AmountColumn("OtherCommissionAmount","Other Comm. Amount"),
                              GridColumnHelper.PercentColumn("UatpPercent", "UATP Rate", 60),
                              GridColumnHelper.AmountColumn("UatpAmount","UATP Amount"),                                             
                              GridColumnHelper.AmountColumn("HandlingFeeAmount","Handling Fee Amt."),
                              GridColumnHelper.AmountColumn("TaxAmount","Tax Amt."),                                             
                              GridColumnHelper.AmountColumn("VatAmount","VAT Amt."),
                              GridColumnHelper.AmountColumn("EvaluatedNetAmount","Coupon Total Amt.")
                            };


        var formatter = new CustomFormatter { FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID) };
        _grid.Columns[0].Formatter = formatter;
        _grid.MultiSelect = _isCheckBoxRequired;
        _grid.ClientSideEvents = new ClientSideEvents
        {
          RowSelect = "SetRejectAccess"
        };
      }
    }
  }
}