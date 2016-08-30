using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.MU
{
  public class DerivedVatGrid : GridBase
  {
    public DerivedVatGrid(string gridId, string dataUrl)
      : base(gridId, dataUrl)
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
                          GridColumnHelper.NumberColumn("SerialNo","Sr. No.",50),
                          GridColumnHelper.TextColumn("Description","Description",300),
                          GridColumnHelper.TextColumn("SubType","VAT SubType",90),
                          GridColumnHelper.AmountColumn("Amount","VAT Base Amount"),
                          GridColumnHelper.AmountColumn("Percentage","VAT Percent",3),
                          GridColumnHelper.AmountColumn("CalculatedAmount","VAT Calculated Amount"),
                          GridColumnHelper.TextColumn("CategoryCode","VAT Category",90),
                        };
      
    }
  }
}