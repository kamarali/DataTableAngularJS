using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.MU
{
  public class ContactGrid : GridBase
  {
    public ContactGrid(string gridId, string dataUrl, bool isGridViewOnly = false)
      : base(gridId, dataUrl, isGridViewOnly)
    {
    }

    /// <summary>
    /// Initializes columns for grid
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid == null)
        return;

      _grid.Columns = new List<JQGridColumn>
                        {
                          GridColumnHelper.ActionColumn("Id", 60, _isGridViewOnly),
                          GridColumnHelper.TextColumn("Type","Contact Type",50),
                          GridColumnHelper.TextColumn("Value","Contact Value",100),
                          GridColumnHelper.TextColumn("Description","Description",100)
                        };
      var formatter = new CustomFormatter();
      formatter.FormatFunction = string.Format("{0}_DeleteRecord", _grid.ID);
      _grid.Columns[0].Formatter = formatter;
    }
  }
}