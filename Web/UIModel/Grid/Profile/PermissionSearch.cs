using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Profile
{
    public class PermissionSearch :  GridBase 
    {

        public PermissionSearch(string gridId, string dataUrl)
            : base(gridId, dataUrl)
        { 
        }

        protected override void InitializeColumns()
        {
            if (_grid != null)
            {
                _grid.Columns = new List<JQGridColumn>
                                    {
                                        GridColumnHelper.ActionColumn("Id",80),
                                        GridColumnHelper.TextColumn("TemplateName", "Template Name", 350),
                                        GridColumnHelper.TextColumn("CategoryName", "Category Name", 350),

                                    };

                _grid.SortSettings.InitialSortColumn = "LastUpdatedOn";
                _grid.SortSettings.InitialSortDirection = SortDirection.Desc;
                var formatter = new CustomFormatter { FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID) };

                // Grid Checkbox column
                _grid.MultiSelect = true;
                _grid.MultiSelectMode = MultiSelectMode.SelectOnCheckBoxClickOnly;
                _grid.Columns[0].Formatter = formatter;

            }
        }


    }
}