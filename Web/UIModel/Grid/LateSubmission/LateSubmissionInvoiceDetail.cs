using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.LateSubmission
{
    public class LateSubmissionInvoiceDetail : GridBase
    {
        public LateSubmissionInvoiceDetail(string gridId, string dataUrl)
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
                              GridColumnHelper.SortableTextColumn("InvoiceNo", "Invoice Number",225),
                              GridColumnHelper.SortableTextColumn("Category", "Billed Category",150),
                              GridColumnHelper.SortableTextColumn("BilledAirline","Billed Airline",220),                             
                              GridColumnHelper.AmountColumn("Amount","Amount",3),
                              GridColumnHelper.SortableTextColumn("Currency", "Currency",150),
                              GridColumnHelper.SortableTextColumn("Status","Status",110),
                            };
                _grid.Columns[4].Sortable=true;
                _grid.Width = 1070;
            }

        }
    }
}
