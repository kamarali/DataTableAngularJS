using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Reports
{
  public class FileStatusLateSubmissionWarning:GridBase
  {
    public FileStatusLateSubmissionWarning(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }

    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn>
                            {
                              GridColumnHelper.TextColumn("InvoiceNo", "Invoice No",100),
                              GridColumnHelper.TextColumn("FormatedInvoiceDate", "Invoice Date",130),
                              GridColumnHelper.TextColumn("BilledMemberCode","Billed Member Code",120),
                              GridColumnHelper.TextColumn("BilledMemberName","Billed Member Name",170),
                              GridColumnHelper.TextColumn("InvoiceAmount", "Invoice Amount",100),
                              GridColumnHelper.TextColumn("InvoiceCurrency","Invoice Currency",70),
                              GridColumnHelper.TextColumn("ValidationStatus", "Validation Status",100),
                            };
      }
    }
  }
}