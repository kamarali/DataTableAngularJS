using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.LegalArchive
{
    public class ArchiveRetrivalJobDetailsGrid : GridBase
    {
        public ArchiveRetrivalJobDetailsGrid(string gridId, string dataUrl, int? currentPageNo, int? currentPageSize)
            : base(gridId, dataUrl, currentPageNo, currentPageSize)
        {
        }

        protected override void InitializeColumns()
        {
            if (_grid != null)
            {
                _grid.Columns = new List<JQGridColumn>
                                    {
                                        new JQGridColumn
                                            {
                                                DataField = "ID",
                                                PrimaryKey = true,
                                                Editable = false,
                                                Visible = false
                                            },
                                        GridColumnHelper.SortableTextColumn("Action", "Actions", 80),
                                        GridColumnHelper.SortableTextColumn("Type", "Type", 90),
                                        GridColumnHelper.SortableTextColumn("MemberText", "Member", MemberNumericCodeColoumnWidth), // CMP#596 Update grid column as per Table 3, Table 9,Table 13 and Table 21  
                                        GridColumnHelper.SortableTextColumn("BillingCategoryText", "Billing Category",
                                                                            150),
                                        GridColumnHelper.SortableTextColumn("InvoiceNumber", "Invoice Number", 100),
                                        GridColumnHelper.DateColumn("InvoiceDate", "Invoice Date", 100),
                                        GridColumnHelper.NumberColumn("BillingYear", "Billing Year", 80,isSortable:true),
                                        GridColumnHelper.SortableTextColumn("BillingMonthText", "Billing Month", 80),
                                        GridColumnHelper.NumberColumn("BillingPeriod", "Billing Period", 80, isSortable:true),
                                        
                                        GridColumnHelper.SortableTextColumn("MiscLocationCode", "Location (MISC Only)", 70), //CMP #666: MISC Legal Archiving Per Location ID

                                        GridColumnHelper.SortableTextColumn("BillingLocation",
                                                                            "Billing Location Country", 110),
                                        GridColumnHelper.SortableTextColumn("BilledLocation", "Billed Location Country",
                                                                            110),
                                        GridColumnHelper.SortableTextColumn("SettlementText", "Settlement Method", 80),
                                        GridColumnHelper.HiddenColumn("IsFileExist")
                                    };

                _grid.PagerSettings.PageSizeOptions = "[5,10,50,100,300,500]";
                _grid.Height = Unit.Pixel(300);
                var formatter = new CustomFormatter() { FormatFunction = "DownloadZipRecord" };

                _grid.Columns.Find(column => column.DataField == "Action").Formatter = formatter;  
            }
        }
    }
}