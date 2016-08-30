using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
  public class AwbChargeCollectGrid : GridBase
  {
    public AwbChargeCollectGrid(string gridId, string dataUrl, bool isCheckBoxRequired = false)
            : base(gridId, dataUrl, isCheckBoxRequired: isCheckBoxRequired)
        {
        }
    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn>
                          {//this is old file pls update it - Nishikant
                            GridColumnHelper.ActionColumn("Id", 70),
                            //GridColumnHelper.SortableTextColumn("SourceCodeId", "Source Code", 100),
                            GridColumnHelper.SortableTextColumn("BatchSequenceNumber", "Batch No.", 50),
                            GridColumnHelper.SortableTextColumn("RecordSequenceWithinBatch", "Seq. No.", 50),
                            GridColumnHelper.TextColumn("AwbIssueingAirline", "Iss. Arln.", 70),
                            GridColumnHelper.NumberColumn("AwbSerialNumberCheckDigit", "AWB Serial No.", 80),
                            //GridColumnHelper.NumberColumn("AwbCheckDigit", "Check Digit", 50),
                            GridColumnHelper.DateColumn("AwbDate", "AWB Date", 80),
                            GridColumnHelper.TextColumn("ConsignmentOriginId", "Consgn. Origin", 60),
                            GridColumnHelper.TextColumn("ConsignmentDestinationId", "Consgn. Dest.",60),
                            GridColumnHelper.TextColumn("CarriageFromId", "Carriage From", 70),
                            GridColumnHelper.TextColumn("CarriageToId", "Carriage To", 70),
                            GridColumnHelper.DateColumn("DateOfCarriage", "Date Of Carriage", 70),
                            GridColumnHelper.AmountColumn("WeightCharges", "Weight Charges",3),
                            GridColumnHelper.AmountColumn("ValuationCharges", "Valuation Charges",3),
                            GridColumnHelper.AmountColumn("OtherCharges", "Other Charges", 3),
                            GridColumnHelper.AmountColumn("AmountSubjectToIsc", "Amount Sub To ISC", 3),
                            GridColumnHelper.PercentColumn("IscPer", "ISC %",60),
                            GridColumnHelper.AmountColumn("IscAmount", "ISC Amount", 3),
                            GridColumnHelper.AmountColumn("VatAmount", "VAT Amount", 3),
                            GridColumnHelper.AmountColumn("AwbTotalAmount", "AWB Total Amount", 3),
                            GridColumnHelper.TextColumn("CurrencyAdjustmentIndicator", "Curr. Adj. Ind.", 70),
                            GridColumnHelper.NumberColumn("BilledWeight", "Billed Weight"),
                            GridColumnHelper.SortableTextColumn("KgLbIndicator","KG/LB Ind.", 40),
                            GridColumnHelper.TextColumn("ProvisoReqSpa", "Proviso/Req/Spa",100),
                            GridColumnHelper.NumberColumn("ProratePer", "Prorate %", isSortable: true),
                             GridColumnHelper.TextColumn("PartShipmentIndicator", "Part Shipment", 60),
                              GridColumnHelper.TextColumn("CcaIndicator", "CCA?", 40),
                          };


        var formatter = new CustomFormatter
        {
          FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID)
        };
        _grid.Columns[0].Formatter = formatter;
        _grid.Columns.Find(column => column.DataField == "CcaIndicator").Formatter = new CustomFormatter { FormatFunction = "SetCcaIndicator" };
        _grid.Columns.Find(column => column.DataField == "AwbSerialNumberCheckDigit").Formatter = new CustomFormatter { FormatFunction = "SetAwbSerialNumber" };
        _grid.MultiSelect = _isCheckBoxRequired;
        _grid.MultiSelectMode = MultiSelectMode.SelectOnCheckBoxClickOnly;

        _grid.SortSettings.InitialSortColumn = "BatchSequenceNumber,RecordSequenceWithinBatch";
        _grid.SortSettings.InitialSortDirection = SortDirection.Asc;

        // If rejection is allowed, only then register the client side event. Rejection is not allowed for Form A/B coupons
        // and for non-sampling coupons when billing type is Receivables.
        if (_isCheckBoxRequired)
          _grid.ClientSideEvents = new ClientSideEvents
          {
            RowSelect = "SetRejectAccess"
          };
      }
    }
  }
}
