using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Masters
{
    public class UomCodeSearch : GridBase
    {
        public UomCodeSearch(string gridId, string dataUrl)
            : base(gridId, dataUrl)
        {
        }

        protected override void InitializeColumns()
        {
            if (_grid != null)
            {
                _grid.Columns = new List<JQGridColumn>
                                    {
                                        GridColumnHelper.ActionColumn("Key",80),
                                        GridColumnHelper.HiddenColumn("IsActive"),
                                        GridColumnHelper.SortableTextColumn("Id", "UOM Code", 115),
                                        GridColumnHelper.SortableTextColumn("UomType", "UOM Code Type", 115),
                                        GridColumnHelper.SortableTextColumn("Description", "Description", 300),
                                        GridColumnHelper.SortableTextColumn("IsActive", "Active", 115),
                                       GridColumnHelper.DateTimeColumn("LastUpdatedOn", "Last Updated On", 200,true),
                                       
                                    };
                _grid.SortSettings.InitialSortColumn = "LastUpdatedOn";
                _grid.SortSettings.InitialSortDirection = SortDirection.Desc;
                _grid.Width = 990;
                var formatter = new CustomFormatter
                {
                    FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID)
                };

                _grid.Columns[0].Formatter = formatter;

            }
        }
    }
}