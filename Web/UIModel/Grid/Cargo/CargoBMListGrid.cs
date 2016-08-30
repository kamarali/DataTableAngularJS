using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
    public class CargoBMListGrid : GridBase
    {
        public CargoBMListGrid(string gridId, string dataUrl)
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
                              GridColumnHelper.ActionColumn("Id",90),
                              GridColumnHelper.SortableTextColumn("BatchSequenceNumber","Batch Seq Number", 80),
                              GridColumnHelper.SortableTextColumn("RecordSequenceWithinBatch", "Record Seq within Batch", 80),
                              GridColumnHelper.SortableTextColumn("BillingMemoNumber","Billing Memo Number", 80),
                              GridColumnHelper.AmountColumn("BilledTotalWeightCharge","Total Weight Charge", 3),
                              GridColumnHelper.AmountColumn("BilledTotalValuationAmount","Total Valuation Charge", 3),
                              GridColumnHelper.AmountColumn("BilledTotalOtherChargeAmount","Total Other Charge", 3),
                              GridColumnHelper.AmountColumn("BilledTotalIscAmount","Total ISC Amount", 3),
                              GridColumnHelper.AmountColumn("BilledTotalVatAmount","Total VAT Amount", 3),                                             
                              //GridColumnHelper.TextColumn("CurrencyOfListing","Currency Of Listing",80),
                              GridColumnHelper.AmountColumn("NetBilledAmount","Total Net Amount", 3),                                             
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
