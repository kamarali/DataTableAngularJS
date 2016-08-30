using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Common
{
  public class AvailableVatGrid: GridBase
  {
    public AvailableVatGrid(string gridId, string dataUrl)
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
                                   GridColumnHelper.HiddenKeyColumn("RowNumber"),
                                   GridColumnHelper.TextColumn("Identifier","VAT Identifier",80),
                                   GridColumnHelper.TextColumn("VatLabel","VAT Label",70),
                                   GridColumnHelper.TextColumn("VatText","VAT Text",200),
                                   GridColumnHelper.AmountColumn("VatBaseAmount","VAT Base Amount"),
                                   GridColumnHelper.PercentColumn("VatPercentage","VAT Percentage"),
                                   GridColumnHelper.AmountColumn("VatCalculatedAmount","VAT Calculated Amount",3),
                            };
      }
    }
    }
}