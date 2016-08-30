using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;


namespace Iata.IS.Web.UIModel.Grid.Masters
{
    public class LanguageSearch : GridBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageSearch"/> class.
        /// </summary>
        /// <param name="gridId">The grid id.</param>
        /// <param name="dataUrl">The data URL.</param>
        public LanguageSearch(string gridId, string dataUrl)
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
                                        GridColumnHelper.ActionColumn("Language_Code",80),
                                        GridColumnHelper.SortableTextColumn("Language_Code", "LANGUAGE CODE", 80),
                                        GridColumnHelper.SortableTextColumn("Language_Desc", "LANGUAGE DESCRIPTION", 150),
                                        GridColumnHelper.SortableTextColumn("IsReqForHelp", "Applicable For Help",115),
                                        GridColumnHelper.SortableTextColumn("IsReqForPdf", "Applicable For PDF",115),
                                        GridColumnHelper.DateTimeColumn("LastUpdatedOn", "LAST UPDATED ON", 200,true),
                                       
                                    };

                _grid.SortSettings.InitialSortColumn = "LastUpdatedOn";
                _grid.SortSettings.InitialSortDirection = SortDirection.Desc;
                _grid.Width = 890;
                var formatter = new CustomFormatter
                {
                    FormatFunction = string.Format("{0}_EditRecord", _grid.ID)
                };

                _grid.Columns[0].Formatter = formatter;

            }
        }
    }
}

