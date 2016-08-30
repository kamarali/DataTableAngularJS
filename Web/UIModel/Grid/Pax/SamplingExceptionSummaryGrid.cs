using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Pax
{
    public class SamplingExceptionSummaryGrid : GridBase
    {
        public SamplingExceptionSummaryGrid(string gridId, string dataUrl, bool isDisplayChargeCategory)
            : base(gridId, dataUrl, isDisplayChargeCategory: isDisplayChargeCategory)
        {

        }

        protected override void InitializeColumns()
        {
            if (_grid != null)
            {
                _grid.Columns = new List<JQGridColumn>
                                {
                                  GridColumnHelper.HiddenColumn("Id",isPrimaryKey:true),
                                  GridColumnHelper.SortableTextColumn("ExceptionCode", "Exception Code", 100),
                                  GridColumnHelper.SortableTextColumn("ErrorDescription", "Error Description", 250),

                                  GridColumnHelper.SortableTextColumn("MemberCode","Provisional Billing Member",100),
                                  GridColumnHelper.SortableTextColumn("FileName","File Name",250),
                                  GridColumnHelper.SortableTextColumn("ErrorCount","Error Count",70),
                                 // _isDisplayChargeCategory ? GridColumnHelper.TextColumn("ChargeCategory","Charge Category",70):GridColumnHelper.HiddenColumn("ChargeCategory"),
                                  
                                  GridColumnHelper.SortableTextColumn("BatchUpdateAllowed","Batch Update Allowed",120),
                                  GridColumnHelper.HiddenColumn("InvoiceID"),
                                  GridColumnHelper.HiddenColumn("BilledMemberId"),
                                  GridColumnHelper.HiddenColumn("BillingMemberId"),
                              };

                var clientSideEvents = new ClientSideEvents { RowSelect = "SamplingDisplayDetails", GridInitialized = "SamplingSelectFirstRow" };
                _grid.ClientSideEvents = clientSideEvents;

            }
        }
    }
}