using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
  public class CargoAvailableVatGrid: GridBase
  {
    public CargoAvailableVatGrid(string gridId, string dataUrl)
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
                                   GridColumnHelper.TextColumn("VatIdentifierText","VAT Identifier",80),
                                   GridColumnHelper.TextColumn("VatLabel","VAT Label",70),
                                   GridColumnHelper.TextColumn("VatText","VAT Text",200),
                                   GridColumnHelper.AmountColumn("VatBaseAmount","VAT Base Amount", 3),
                                   GridColumnHelper.PercentColumn("VatPercentage","VAT Percentage"),
                                   GridColumnHelper.AmountColumn("VatCalculatedAmount","VAT Calculated Amount", 3),
                                   GridColumnHelper.AmountColumn("OtVatCalculatedAmount","VAT Amount from Other Charge", 3),
                            };
      }
    }
    }
}