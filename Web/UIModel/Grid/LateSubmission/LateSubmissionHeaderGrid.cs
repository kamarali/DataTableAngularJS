using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.LateSubmission
{
    public class LateSubmissionHeaderGrid : GridBase
    {
        public LateSubmissionHeaderGrid(string gridId, string dataUrl)
            : base(gridId, dataUrl)
        {
        }

        protected override void InitializeColumns()
        {
            if (_grid != null)
            {

                _grid.Columns = new List<JQGridColumn>
                            {
                              GridColumnHelper.HiddenColumn("MemberId",isPrimaryKey:true),
                              GridColumnHelper.SortableTextColumn("MemberName", "Member",MemberNumericCodeColoumnWidth), // CMP#596 Update grid column as per Table 3, Table 9,Table 13 and Table 21  
                              GridColumnHelper.SortableTextColumn("NoOfInvoices", "Count of Invoices",100),
                              GridColumnHelper.SortableTextColumn("FormattedPassengerBilling","Passenger Billing",195),
                              GridColumnHelper.SortableTextColumn("FormattedCargoBilling","Cargo Billing",195),
                              GridColumnHelper.SortableTextColumn("FormattedMiscBilling", "MISC Billing",195),
                              GridColumnHelper.SortableTextColumn("FormattedUatpBilling","UATP Billing",195),
                            };
                _grid.Width = 1070;
                var clientSideEvents = new ClientSideEvents { RowSelect = "DisplayDetails" };
                _grid.ClientSideEvents = clientSideEvents;
            }
        }
    }
}