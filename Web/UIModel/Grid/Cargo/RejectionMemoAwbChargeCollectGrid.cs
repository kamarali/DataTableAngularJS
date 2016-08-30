using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
  public class RejectionMemoAwbChargeCollectGrid : GridBase
  {
    public RejectionMemoAwbChargeCollectGrid(string gridId, string dataUrl)
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
                              GridColumnHelper.TextColumn("AwbDate","Issueing Date",90),
                              GridColumnHelper.AmountColumn("ConsignmentOriginId","Consignment Origin", 3),
                              GridColumnHelper.AmountColumn("ConsignmentDestinationId","Consignment Dest", 3),
                              GridColumnHelper.AmountColumn("CarriageFromId","Carriage From", 3),
                              GridColumnHelper.AmountColumn("CarriageToId","Carriage To", 3),                                             
                              GridColumnHelper.AmountColumn("TransferDate","Carriage Date", 3),
                              GridColumnHelper.AmountColumn("AcceptedWeightCharge","Weight Charge", 3),                                             
                              GridColumnHelper.AmountColumn("AcceptedValuationCharge","Valuation Charge", 3),
                              GridColumnHelper.AmountColumn("AcceptedOtherCharge","Other Charge", 3),
                              GridColumnHelper.TextColumn("AcceptedAmtSubToIsc","Weight Charge Sub ISC",100),
                              GridColumnHelper.TextColumn("AcceptedIscAmount","ISC Amt",100),
                              GridColumnHelper.TextColumn("AcceptedVatAmount","VAT Amt",100),
                              
                            };


      //  var formatter = new CustomFormatter { FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID) };
       // _grid.Columns[0].Formatter = formatter;
      }
    }

  }
}
