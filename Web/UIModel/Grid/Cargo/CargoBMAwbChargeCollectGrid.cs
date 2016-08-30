using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
  public class CargoBMAwbChargeCollectGrid : GridBase
  {
      public CargoBMAwbChargeCollectGrid(string gridId, string dataUrl)
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
                              GridColumnHelper.SortableTextColumn("AwbIssueingAirline","Issueing Airline", 80),
                              GridColumnHelper.SortableTextColumn("AwbSerialNumber", "AWB Serial No.", 80),
                              GridColumnHelper.SortableTextColumn("AwbCheckDigit","Check Digit", 80),
                              GridColumnHelper.DateTimeColumn("AwbDate","Issueing Date",90),
                              GridColumnHelper.SortableTextColumn("ConsignmentOriginId","Consignment Origin",90),
                              GridColumnHelper.SortableTextColumn("ConsignmentDestinationId","Consignment Dest",90),
                              GridColumnHelper.SortableTextColumn("CarriageFromId","Carriage From",90),
                              GridColumnHelper.SortableTextColumn("CarriageToId","Carriage To",90),                                           
                              GridColumnHelper.DateTimeColumn("TransferDate","Carriage Date"),
                              GridColumnHelper.AmountColumn("BilledWeightCharge","Weight Charge", 3),                                             
                              GridColumnHelper.AmountColumn("BilledValuationCharge","Valuation Charge", 3),
                              GridColumnHelper.AmountColumn("BilledOtherCharge","Other Charge", 3),
                              GridColumnHelper.AmountColumn("BilledAmtSubToIsc","Weight Charge Sub ISC", 3, 100),
                              GridColumnHelper.AmountColumn("BilledIscAmount","ISC Amt", 3, 70),
                              GridColumnHelper.AmountColumn("BilledVatAmount","VAT Amt", 3, 70),
                              
                            };


        var formatter = new CustomFormatter
        {
            FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID)
        };
        _grid.Columns[0].Formatter = formatter;
      }
    }

  }
}
