using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;
namespace Iata.IS.Web.UIModel.Grid.SystemMonitor
{
    public class OutpuFileSearchGrid : GridBase
    {
        public OutpuFileSearchGrid(string gridId, string dataUrl)
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
                            //TFS#9972 :Firefox: v47 :System Monitor:Error on Resend output file
                            GridColumnHelper.HiddenColumn("FileId",isPrimaryKey:true),
                            GridColumnHelper.TextColumn("FileName","File Name",250,true),
                            GridColumnHelper.TextColumn("FileMemberName","Member",160,true),
                            //CMP#622: MISC Outputs Split as per Location IDs
                            GridColumnHelper.TextColumn("MiscLocationCode","Location ID",90, true),
                            GridColumnHelper.DateTimeColumn("FileDate","File Date",90,true),
                            GridColumnHelper.TextColumn("ProvisionalBillingYear","Billing Year",60,true),
                            GridColumnHelper.TextColumn("ProvisionalBillingMonth","Billing Month",60,true),
                            GridColumnHelper.TextColumn("ProvisionalBillingPeriod","Billing Period",60,true),
                            GridColumnHelper.TextColumn("FileStatus","File Status",150,true),
                            GridColumnHelper.TextColumn("FileId","FTP Log",60,true),
                            GridColumnHelper.TextColumn("IsPurged","Purged ?",60,true),
                           };
            }
            
            //var clientSideEvents = new ClientSideEvents { RowSelect = "RowSelectEvent" };
            //_grid.ClientSideEvents = clientSideEvents;

            var formatFileName = new CustomFormatter { FormatFunction = "formatFileName" };

            _grid.Columns[1].Formatter = formatFileName;

            
            var formatter = new CustomFormatter { FormatFunction = "formatViewFTPLog" };
            /* CMP#622: Grid Column Index updated in order to get correct formatter applied. */
            _grid.Columns[9].Formatter = formatter;

            var formatterIsPurged = new CustomFormatter { FormatFunction = "formatIspurged" };
            /* CMP#622: Grid Column Index updated in order to get correct formatter applied. */
            _grid.Columns[10].Formatter = formatterIsPurged;


            _grid.SortSettings.InitialSortColumn = "FileDate";
            _grid.SortSettings.InitialSortDirection = SortDirection.Desc;

        }// end InitializeColumns()  
    }
}