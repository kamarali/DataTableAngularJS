using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Reports
{
  public class ProcessingDashboardInvoiceActionResults: GridBase
  {
    public ProcessingDashboardInvoiceActionResults(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }

    /// <summary>
    /// Following method is used to initialize Columns of Invoice Action Status Results grid on ProcessingDashboardInvoiceActionResultsControl control
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid != null)
      {

        _grid.Columns = new List<JQGridColumn>
                            {
                              GridColumnHelper.HiddenColumn("InvoiceId",isPrimaryKey:true ),
                              GridColumnHelper.HiddenColumn("BillingMemberId"),
                              GridColumnHelper.TextColumn("BillingMemberCode","Billing Member Code",90),
                              GridColumnHelper.TextColumn("BillingMemberName","Billing Member Name",156),
                              GridColumnHelper.TextColumn("InvoiceNo","Invoice No.",100),
                              GridColumnHelper.TextColumn("ActionStatus","Status",150)
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