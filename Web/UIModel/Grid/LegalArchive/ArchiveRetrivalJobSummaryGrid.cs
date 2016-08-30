using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;
using System.Web.UI.WebControls;
using SortDirection = Trirand.Web.Mvc.SortDirection;

namespace Iata.IS.Web.UIModel.Grid.LegalArchive
{
    public class ArchiveRetrivalJobSummaryGrid: GridBase
    {
        public ArchiveRetrivalJobSummaryGrid(string gridId, string dataUrl,int? currentPageNo, int? currentPageSize)
            : base(gridId, dataUrl, currentPageNo, currentPageSize)
        {
        }
        /// <summary>
        /// Initializes columns for grid
        /// </summary>
        protected override void InitializeColumns()
        {
            if (_grid == null)
            {
                return;
            }

            _grid.Columns = new List<JQGridColumn>
                            {
                              //TFS#9956:IE:Version 11- Unexpected error in View Retrieved Invoice.
                              GridColumnHelper.HiddenColumn("Id",isPrimaryKey:true),
                              GridColumnHelper.SortableTextColumn("RequestedBy", "Requested By",80),
                              GridColumnHelper.SortableTextColumn("RequestedOn", "Requested On",80),
                              GridColumnHelper.SortableTextColumn("JobId","Job Id",100),
                              GridColumnHelper.SortableTextColumn("JobStatus","Job Status",80),
                              GridColumnHelper.SortableTextColumn("InvoiceNumber", "Invoice Number",120),
                              GridColumnHelper.SortableTextColumn("Type","Type",80),
                              GridColumnHelper.NumberColumn("BillingYear","Billing Year",80,isSortable:true),
                              GridColumnHelper.SortableTextColumn("BillingMonthText","Billing Month",80),
                              GridColumnHelper.NumberColumn("BillingPeriodText","Billing Period",80,isSortable:true),
                              GridColumnHelper.SortableTextColumn("Member","Member",MemberNumericCodeColoumnWidth), // CMP#596 Update grid column as per Table 3, Table 9,Table 13 and Table 21  
                              GridColumnHelper.SortableTextColumn("BillingCategoryText","Billing Category",150),
                              GridColumnHelper.SortableTextColumn("MiscLocationCodes","Locations (MISC Only) ",70), //CMP #666: MISC Legal Archiving Per Location ID
                              GridColumnHelper.SortableTextColumn("BillingLocation","Billing Location Country",150),
                              GridColumnHelper.SortableTextColumn("BilledingLocation","Billed Location Country",150),
                              GridColumnHelper.SortableTextColumn("SettlementText","Settlement Method",100),
                            };

            var clientSideEvents = new ClientSideEvents { RowSelect = "DisplayDetails"};
            _grid.Width = Unit.Percentage(100);
            _grid.ClientSideEvents = clientSideEvents;
            _grid.SortSettings.InitialSortColumn = "RequestedOn";
            _grid.SortSettings.InitialSortDirection = SortDirection.Desc;
        }
    }
}