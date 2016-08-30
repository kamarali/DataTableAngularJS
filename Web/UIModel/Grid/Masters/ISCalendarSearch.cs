using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Masters
{
    public class ISCalendarSearch : GridBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ISCalendarSearch"/> class.
        /// </summary>
        /// <param name="gridId">The grid id.</param>
        /// <param name="dataUrl">The data URL.</param>
        public ISCalendarSearch(string gridId, string dataUrl)
            : base(gridId, dataUrl)
        {
        }

        /// <summary>
        /// Initializes the columns.
        /// </summary>
        protected override void InitializeColumns()
        {
            if (_grid != null)
            {
                _grid.Columns = new List<JQGridColumn>
                                    {
                                        GridColumnHelper.ActionColumn("Id",80),
                                        GridColumnHelper.HiddenColumn("IsActive"),
                                        GridColumnHelper.SortableTextColumn("Name", "Event Name", 150),
                                        GridColumnHelper.SortableTextColumn("Month", "Month", 70),
                                        GridColumnHelper.SortableTextColumn("Year", "Year", 70),
                                        GridColumnHelper.SortableTextColumn("Period", "Period", 70),
                                        GridColumnHelper.TextColumn("EventDateTime", "Event Date Time", 150),
                                        GridColumnHelper.SortableTextColumn("EventCategory", "Event Category", 150),
                                        GridColumnHelper.SortableTextColumn("IsActive", "Active", 115),
                                        GridColumnHelper.SortableTextColumn("LastUpdatedOn", "Last Updated On", 280),
                                    }; 
                _grid.SortSettings.InitialSortColumn = "LastUpdatedOn";
                _grid.SortSettings.InitialSortDirection = SortDirection.Desc;
                var formatter = new CustomFormatter
                {
                    FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID)
                };

                _grid.Columns[0].Formatter = formatter;

            }
        }
    }
}