using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
  public class CargoCorrespondenceHistoryGrid : GridBase
  {
    public CargoCorrespondenceHistoryGrid(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }

    /// <summary>
    /// Initializes columns for grid
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn>
                            {
                                  GridColumnHelper.HiddenKeyColumn("Id"),
                                  GridColumnHelper.NumberColumn("CorrespondenceStage","Stage No."),
                                  GridColumnHelper.DateColumn("CorrespondenceDate", "Correspondence Date"),
                                  GridColumnHelper.AmountColumn("AmountToBeSettled", "Correspondence Amount", 3),
                                  GridColumnHelper.NumberColumn("NoOfAttachments","No. of Attachments"),
                                  
                            };

        var formatter = new CustomFormatter() { FormatFunction = string.Format("{0}_OpenGivenCorrespondenceRecord", _grid.ID) };
        _grid.SortSettings.InitialSortColumn = "CorrespondenceStage";
        _grid.Columns.Find(column => column.DataField == "CorrespondenceStage").Formatter = formatter;
      }
    }
  }
}