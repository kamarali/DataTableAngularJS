using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Masters
{
    public class SettlementMethodSearch:GridBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettlementMethodSearch"/> class.
        /// </summary>
        /// <param name="gridId">The grid id.</param>
        /// <param name="dataUrl">The data URL.</param>
        public SettlementMethodSearch(string gridId, string dataUrl)
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
                                        GridColumnHelper.ActionColumn("Id",100),
                                        GridColumnHelper.HiddenColumn("IsActive"),
                                        GridColumnHelper.SortableTextColumn("Name", "Settlement Method Name", 100),
                                        GridColumnHelper.SortableTextColumn("Description", "Description", 250),
                                        GridColumnHelper.SortableTextColumn("IsActive", "Active", 100),
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