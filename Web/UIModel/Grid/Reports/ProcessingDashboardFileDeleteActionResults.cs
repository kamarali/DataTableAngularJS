using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Reports
{
    public class ProcessingDashboardFileDeleteActionResults : GridBase
    {
        public ProcessingDashboardFileDeleteActionResults(string gridId, string dataUrl) : base(gridId, dataUrl)
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
                              GridColumnHelper.HiddenColumn("FileId",isPrimaryKey:true),
                              GridColumnHelper.HiddenColumn("BillingMemberId"),
                              GridColumnHelper.TextColumn("BillingMemberCode","Billing Member Code",100),
                              GridColumnHelper.TextColumn("BillingMemberName","Billing Member Name",100),
                              GridColumnHelper.TextColumn("FileName","File Name",200),
                              GridColumnHelper.TextColumn("FormatedReceivedInIsDate","Received in IS",100),
                              GridColumnHelper.TextColumn("DeleteStatus","Status",100),
                               GridColumnHelper.TextColumn("TotalInvoices","Total No of Invoices/Form C",100),
                              GridColumnHelper.TextColumn("TotalInvoiceInError","Total No of Invalid Invoices/Form C Deleted",150)
                            };
                _grid.ToolBarSettings.ToolBarPosition = ToolBarPosition.Hidden;
                foreach (var colm in _grid.Columns)
                {
                    colm.Resizable = false;
                }
            }// end if()

        }// end InitializeColumns() 
    }// end ProcessingDashboardFileDeleteActionResults class
}// end namespace