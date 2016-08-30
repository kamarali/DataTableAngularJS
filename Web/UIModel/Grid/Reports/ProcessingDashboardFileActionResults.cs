using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Reports
{
  public class ProcessingDashboardFileActionResults: GridBase
  {
    public ProcessingDashboardFileActionResults(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }

    /// <summary>
    /// Following method is used to initialize Columns of File Action Status Results grid on ProcessingDashboardFileActionResultsControl control
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid != null)
      {

        _grid.Columns = new List<JQGridColumn>
                            {
                              GridColumnHelper.HiddenColumn("FileId",isPrimaryKey:true),
                              GridColumnHelper.HiddenColumn("BillingMemberId"),
                              GridColumnHelper.TextColumn("BillingMemberCode","Billing Member Code",90),
                              GridColumnHelper.TextColumn("BillingMemberName","Billing Member Name",100),
                              GridColumnHelper.TextColumn("FileName","File Name",120),
                              GridColumnHelper.TextColumn("FormatedReceivedInIsDate","Received in IS",100),
                              GridColumnHelper.TextColumn("NumberOfInvoices","Total No. Of Invoices",80),
                              GridColumnHelper.TextColumn("NumberOfActions","Action on Invoices",80),
                              GridColumnHelper.TextColumn("NumberOfAlreadyRequested","Late Sub. Already Requested",100),
                              GridColumnHelper.TextColumn("NumberOfSettlementFilesCreated","Settlement File Sent",80),
                              GridColumnHelper.TextColumn("NumberOfValidationErrors","Validation Error Invoices",80),
                              GridColumnHelper.TextColumn("NumberOfFormCs","Form Cs",80)
                            };
        _grid.ToolBarSettings.ToolBarPosition = ToolBarPosition.Hidden;
        foreach (var colm in _grid.Columns)
        {
            colm.Resizable = false;
        }
      }
     
    }// end InitializeColumns()  
  }// end ProcessingDashboardInvoiceActionResults class
}// end namespace