using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Pax
{
  public class BillingMemoGrid : GridBase
  {
    public BillingMemoGrid(string gridId, string dataUrl)
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
                              /* Column index used in ScriptHelper: GeneratePaxPayablesGridViewRejectScript(). Please take care while updating indices. */
                              GridColumnHelper.ActionColumn("Id",90),
                                             GridColumnHelper.SortableTextColumn("SourceCodeId","Source Code", 80),
                                             GridColumnHelper.SortableTextColumn("BatchSequenceNumber","Batch No.", 80),
                                             GridColumnHelper.SortableTextColumn("RecordSequenceWithinBatch","Sequence No.", 80),
                                             GridColumnHelper.TextColumn("BillingMemoNumber","Billing Memo Number",90),                                             
                                             GridColumnHelper.AmountColumn("TotalGrossAmountBilled","Gross Fare Value"),
                                             GridColumnHelper.AmountColumn("TotalIscAmountBilled","ISC Amt."),
                                             GridColumnHelper.CustomColumn("TotalOtherCommissionAmount","Other Comm. Amt.",120,
                                             TextAlign.Right,new NumberFormatter(){DecimalPlaces = 2, DecimalSeparator = ".", ThousandsSeparator = ","}),
                                             GridColumnHelper.AmountColumn("TotalUatpAmountBilled","UATP Amount"),
                                             GridColumnHelper.AmountColumn("TotalHandlingFeeBilled","Handling Fee Amt."),
                                             GridColumnHelper.AmountColumn("TaxAmountBilled","Tax Amt."),
                                             GridColumnHelper.AmountColumn("TotalVatAmountBilled","VAT Amt."),
                                             GridColumnHelper.AmountColumn("NetAmountBilled","Net Billed Amt."),
                                             GridColumnHelper.TextColumn("ReasonCode","Reason Code",70),
                                             GridColumnHelper.TextColumn("ReasonCodeDescription","Reason Description",120)
                            };


        var formatter = new CustomFormatter
                        {
                          FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID)
                        };
        _grid.Columns[0].Formatter = formatter;
        _grid.SortSettings.InitialSortColumn = "BatchSequenceNumber,RecordSequenceWithinBatch";
        _grid.SortSettings.InitialSortDirection = SortDirection.Asc;

      }
    }
  }
}