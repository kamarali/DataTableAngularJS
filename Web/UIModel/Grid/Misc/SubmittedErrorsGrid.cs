using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Misc
{
  public class SubmittedErrorsGrid : GridBase
  {
    public SubmittedErrorsGrid(string gridId, string dataUrl, bool isGridViewOnly = false)
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
      // CMP #636: Standard Update Mobilization.
      //_grid.ToolBarSettings.ToolBarPosition = ToolBarPosition.Hidden;
      _grid.Columns = new List<JQGridColumn>
                        {
                          GridColumnHelper.TextColumn("ErrorCode","Code",100),
                          GridColumnHelper.TextColumn("ErrorDescription","Description",500)
                        };
    }
  }
}