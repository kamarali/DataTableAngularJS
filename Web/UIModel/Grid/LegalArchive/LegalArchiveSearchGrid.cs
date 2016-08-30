using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.LegalArchive
{
    public class LegalArchiveSearchGrid : GridBase
    {
        public LegalArchiveSearchGrid(string gridId, string dataUrl)
            : base(gridId, dataUrl)
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
                         //TFS#9954:IE-Version 11- "Retrieve Selected" is not performing any action in "Legal Archive Retrieval".
                         GridColumnHelper.HiddenColumn("Id",isPrimaryKey:true),
                         GridColumnHelper.TextColumn("MemberText", "Member", MemberNumericCodeColoumnWidth, true), // CMP#596 Update grid column as per Table 3, Table 9,Table 13 and Table 21  
                         GridColumnHelper.TextColumn("BillingCategoryText", "Billing Category", 90, true),
                         GridColumnHelper.TextColumn("InvoiceNumber", "Invoice No.", 90, isSortable:true),
                         GridColumnHelper.TextColumn("InvoiceDate", "Invoice Date", 90, isSortable:false),
                         GridColumnHelper.NumberColumn("BillingYear", "Billing Year", 90, isSortable:true),
                         GridColumnHelper.NumberColumn("BillingMonthText", "Billing Month", 90, isSortable:true),
                         GridColumnHelper.TextColumn("BillingPeriod", "Billing Period", 120, true),
                         GridColumnHelper.TextColumn("LegalArchivalLocation", "Location (MISC Only)", 70, true), //CMP #666: MISC Legal Archiving Per Location ID
                         GridColumnHelper.TextColumn("BillingCountryCode", "Billing Location Country", 120, true),
                         GridColumnHelper.TextColumn("BilledCountryCode", "Billed Location Country", 120, true),
                         GridColumnHelper.TextColumn("SettlementMethodText", "Settlement Method", 120, true),
                         GridColumnHelper.TextColumn("ReceivablePayableIndicator", "Type", 100,true),
                        };

            // Grid Checkbox column
            _grid.MultiSelect = true;
            _grid.MultiSelectMode = MultiSelectMode.SelectOnCheckBoxClickOnly;
            
            var formatter = new CustomFormatter { FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID) };
            //_grid.Columns[0].Formatter = formatter;
        }
    }
}