using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Masters
{
    public class SubDivisionSearch : GridBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubDivisionSearch"/> class.
        /// </summary>
        /// <param name="gridId">The grid id.</param>
        /// <param name="dataUrl">The data URL.</param>
        public SubDivisionSearch(string gridId, string dataUrl)
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
                                        GridColumnHelper.HiddenColumn("CountryId"),
                                        GridColumnHelper.SortableTextColumn("Id", "Sub Division Code", 180),
                                        GridColumnHelper.SortableTextColumn("Name", "Sub Division Name", 180),
                                        GridColumnHelper.SortableTextColumn("CountryName", "Country Name", 150),
                                        GridColumnHelper.SortableTextColumn("IsActive", "Active", 115),
                                        GridColumnHelper.DateTimeColumn("LastUpdatedOn", "Last Updated On", 200,true),
                                       
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