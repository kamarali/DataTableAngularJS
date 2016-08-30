using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
  public class RejectionMemoAwbGrid : GridBase
  {
    public RejectionMemoAwbGrid(string gridId, string dataUrl)
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
                              /* PLEASE CHANGE IN SCRIPTHELPER IF SEQUENCE OF COLUMNS IS CHANGED.*/
                              GridColumnHelper.ActionColumn("Id", 90),
                              GridColumnHelper.SortableTextColumn("AwbBillingCode","Billing Code", 115),
                              GridColumnHelper.NumberColumn("AwbIssueingAirline","Issuing Airline", dataFormatString: "{0:000}"),
                              GridColumnHelper.NumberColumn("AwbSerialNumberCheckDigit", "AWB Serial No. & Check Digit", 100),
                              GridColumnHelper.DateColumn("AwbDate","AWB Issue Date", 80),
                              GridColumnHelper.SortableTextColumn("ConsignmentOriginId","Consignment Origin",80),
                              GridColumnHelper.SortableTextColumn("ConsignmentDestinationId","Consignment Dest.",80),
                              GridColumnHelper.SortableTextColumn("CarriageFromId","Carriage From",70),
                              GridColumnHelper.SortableTextColumn("CarriageToId","Carriage To",70),
                              GridColumnHelper.DateColumn("TransferDate","Transfer Date"),
                              GridColumnHelper.AmountColumn("WeightChargeDiff","Weight Charges", 3),
                              GridColumnHelper.AmountColumn("ValuationChargeDiff","Valuation Charges", 3),
                              GridColumnHelper.AmountColumn("OtherChargeDiff","Other Charges", 3),
                              GridColumnHelper.AmountColumn("IscAmountDifference","ISC Amount", 3),
                              GridColumnHelper.AmountColumn("VatAmountDifference","VAT Amount", 3),
                              GridColumnHelper.AmountColumn("NetRejectAmount","Net Reject Amount", 3),
                              GridColumnHelper.SortableTextColumn("CurrencyAdjustmentIndicator","Currency Adjustment Ind.",120),
                              GridColumnHelper.NumberColumn("BilledWeight","Billed Weight"),
                              GridColumnHelper.SortableTextColumn("KgLbIndicator","KG/LB Ind.", 60),
                              GridColumnHelper.SortableTextColumn("ProvisionalReqSpa","Proviso/Req/SPA Ind.", 60),
                              GridColumnHelper.NumberColumn("ProratePercentage","Prorate %", isSortable: true),
                              GridColumnHelper.SortableTextColumn("PartShipmentIndicator","Part Shipment Indicator", 90),
                              GridColumnHelper.SortableTextColumn("CcaIndicator","CCA Indicator", 60),
                              GridColumnHelper.HiddenColumn("BdSerialNumber")
                            };
      
        var formatter = new CustomFormatter { FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID) };
        _grid.SortSettings.InitialSortColumn = "BdSerialNumber";
        _grid.SortSettings.InitialSortDirection = SortDirection.Asc;
        _grid.Columns.Find(column => column.DataField == "Id").Formatter = formatter;
        _grid.Columns.Find(column => column.DataField == "CcaIndicator").Formatter = new CustomFormatter { FormatFunction = "SetCcaIndicator" };
        _grid.Columns.Find(column => column.DataField == "AwbBillingCode").Formatter = new CustomFormatter { FormatFunction = "SetBillingCode" };
      }
    }

  }
}
