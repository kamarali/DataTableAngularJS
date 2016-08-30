using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Masters
{
    public class AchCurrencySetUpSearch : GridBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AircraftTypeSearch"/> class.
        /// </summary>
        /// <param name="gridId">The grid id.</param>
        /// <param name="dataUrl">The data URL.</param>
        //CMP #553: ACH Requirement for Multiple Currency Handling.
        public AchCurrencySetUpSearch(string gridId, string dataUrl)
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
                                        GridColumnHelper.SortableTextColumn("CurrencyCode", "Currency Code", 150),
                                        GridColumnHelper.SortableTextColumn("CurrencyName", "Currency Name", 250),
                                        GridColumnHelper.SortableTextColumn("IsActive", "Active", 115),
                                        GridColumnHelper.DateTimeColumn("LastUpdatedOn", "Last Updated On", 280),
                                       
                                    };
                _grid.SortSettings.InitialSortColumn = "CurrencyCode";
                _grid.SortSettings.InitialSortDirection = SortDirection.Asc;

                var formatter = new CustomFormatter
                {
                    FormatFunction = string.Format("{0}_DeleteRecord", _grid.ID)
                };

                _grid.Columns[0].Formatter = formatter;

            }
        }
    }
}