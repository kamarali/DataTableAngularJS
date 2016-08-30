using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
  public class CargoUnappliedVat: GridBase
  {
    public CargoUnappliedVat(string gridId, string dataUrl)
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
        // Chnaged the name of column From 'Vat Amount' to 'Amount' #3856
        _grid.Columns = new List<JQGridColumn>
                            {
                                   GridColumnHelper.HiddenKeyColumn("RowNumber"),
                                   GridColumnHelper.TextColumn("VatIdentifierText","VAT Identifier",200),
                                   GridColumnHelper.AmountColumn("NonAppliedAmount","Amount", 3, 200),
                                 
                            };
      }
    }
  }
}