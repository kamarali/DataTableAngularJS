using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Masters
{
    public class LeadPeriodSearch : GridBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LeadPeriodSearch"/> class.
        /// </summary>
        /// <param name="gridId">The grid id.</param>
        /// <param name="dataUrl">The data URL.</param>
        public LeadPeriodSearch(string gridId, string dataUrl)
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
                                        GridColumnHelper.DateFullYearTimeColumn("EffectiveFromPeriod", "Effective From Period", 150,true),
                                        GridColumnHelper.DateFullYearTimeColumn("EffectiveToPeriod", "Effective To Period", 150,true),
                                        GridColumnHelper.SortableTextColumn("Period", "Lead Period", 115),
                                        GridColumnHelper.SortableTextColumn("ClearingHouse", "Clearing House", 115),
                                        GridColumnHelper.SortableTextColumn("BillingCategoryValue", "Billing Category Name", 115),
                                        GridColumnHelper.SortableTextColumn("IsActive", "Active", 115),
                                        GridColumnHelper.DateFullYearTimeColumn("LastUpdatedOn", "Last Updated On", 200,true),
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