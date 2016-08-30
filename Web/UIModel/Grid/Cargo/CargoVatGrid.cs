using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
  public class CargoVatGrid : GridBase
  {
    public CargoVatGrid(string gridId, string dataUrl, bool isGridViewOnly = false)
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
                          GridColumnHelper.ActionColumn("Id", 30, _isGridViewOnly),
                          GridColumnHelper.TextColumn("Identifier","VAT Identifier",80),
                          GridColumnHelper.TextColumn("VatLabel","VAT Label",70),
                          GridColumnHelper.TextColumn("VatText","VAT Text",200),
                          GridColumnHelper.AmountColumn("VatBaseAmount","VAT Base Amount", 3),
                          GridColumnHelper.PercentColumn("VatPercentage", "VAT Percentage"),
                          GridColumnHelper.AmountColumn("VatCalculatedAmount","VAT Calculated Amount", 3),
                        };
      var formatter = new CustomFormatter
                        {
                          FormatFunction = string.Format("{0}_DeleteRecord", _grid.ID)
                        };
      _grid.Columns[0].Formatter = formatter;

      var nullablePerFormatter = new CustomFormatter
                                   {
                                     FormatFunction = "DisplayNullablePercentFormatter"
                                   };
      _grid.Columns[5].Formatter = nullablePerFormatter;

      var nullableFormatter = new CustomFormatter
                                {
                                  FormatFunction = "DisplayNullableAmountFormatter"
                                };
      _grid.Columns[6].Formatter = nullableFormatter;
    }
  }
}