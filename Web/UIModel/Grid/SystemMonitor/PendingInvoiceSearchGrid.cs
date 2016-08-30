using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;
namespace Iata.IS.Web.UIModel.Grid.SystemMonitor
{
    public class PendingInvoiceSearchGrid : GridBase
    {
      public PendingInvoiceSearchGrid(string gridId, string dataUrl)
            : base(gridId, dataUrl)
        {

        }
        protected override void InitializeColumns()
        {
            if (_grid != null)
            {
                // Following code is used to add multiselect checkbox in Jqgrid header row.
                _grid.MultiSelectMode = MultiSelectMode.SelectOnCheckBoxClickOnly;
                _grid.MultiSelect = true;

                _grid.Columns = new List<JQGridColumn>
                          {
                            GridColumnHelper.HiddenColumn("InvoiceId",isPrimaryKey:true),
                            GridColumnHelper.TextColumn("InvoiceType","Invoice Type",100,true),
                            GridColumnHelper.TextColumn("BillingCategory","Billing Category",100,true),
                            GridColumnHelper.TextColumn("InvoiceNumber","Invoice Number",100,true),
                            GridColumnHelper.TextColumn("BillingMemberAlphaNumericName","Billing Member",250,true),
                            GridColumnHelper.TextColumn("BilledMemberAlphaNumericName","Billed Member",250,true),
                            GridColumnHelper.TextColumn("InvoiceStatus","Invoice Status",100,true),
                         };
            }
            
            //var clientSideEvents = new ClientSideEvents { RowSelect = "RowSelectEvent" };
            //_grid.ClientSideEvents = clientSideEvents;

            

        }// end InitializeColumns()  
    }
}