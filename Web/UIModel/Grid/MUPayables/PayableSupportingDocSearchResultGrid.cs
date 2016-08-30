using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.MUPayables
{
  public class PayableSupportingDocSearchResultGrid : GridBase
  {
    public PayableSupportingDocSearchResultGrid(string gridId, string dataUrl, bool isUatp)
      : base(gridId, dataUrl,false,false,isUatp)
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
                            GridColumnHelper.ActionColumn("InvoiceId", 50),
                            GridColumnHelper.TextColumn("BillingPeriod", "Billing Period", 100,true),
                            GridColumnHelper.TextColumn("BillingMemberText", "Billing Member", MemberNumericCodeColoumnWidth,true),// CMP#596 Update grid column as per Table 3, Table 9,Table 13 and Table 21
                            GridColumnHelper.TextColumn("InvoiceNumber", "Invoice No.", 90,true),
                            _isDisplayChargeCategory ? GridColumnHelper.TextColumn("ChargeCategory", "Charge Category", 150,true) : GridColumnHelper.HiddenColumn("ChargeCategory"),
                            GridColumnHelper.TextColumn("AttachmentIndicatorValidatedText", "Attachment Indicator Validated", 120,true),
                            GridColumnHelper.NumberColumn("AttachmentNumber", "No. of Attachments", 100,isSortable:true),

                          };


        CustomFormatter formatter = new CustomFormatter();
        formatter.FormatFunction = string.Format("{0}_UploadMiscAttachment", _grid.ID);
        _grid.Columns[0].Formatter = formatter;
      }
    }
  }
}
