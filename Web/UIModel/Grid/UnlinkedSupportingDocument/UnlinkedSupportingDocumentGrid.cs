using System.Collections.Generic;
using Iata.IS.Model.Enums;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.UnlinkedSupportingDocument
{
  public class UnlinkedSupportingDocumentGrid : GridBase
  {
    public UnlinkedSupportingDocumentGrid(string gridId, string dataUrl, BillingCategoryType billingCategoryType) : base(gridId, dataUrl)
    {
      // Call InitializeSourceGridColumns() method which will initialize SourceCode grid columns
      InitializeUnlinkedSupportingDocumentGridColumns(billingCategoryType);
    }

    /// <summary>
    /// Initializes columns for grid
    /// </summary>
    protected override void InitializeColumns()
    {
      // Note: Initialized SourceGrid columns in InitializeUnlinkedSupportingDocumentGridColumns() method as column count varies depending on BillingType, .i.e. two new columns are added if BillingType is Payables 
    }

    private void InitializeUnlinkedSupportingDocumentGridColumns(BillingCategoryType billingCategoryType)
    {
      if (_grid == null)
      {
        return;
      }

      var formatter = new CustomFormatter {
                                            FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID)
                                          };

      if (billingCategoryType.Equals(BillingCategoryType.Pax))
      {
        _grid.Columns = new List<JQGridColumn> {
                                                 GridColumnHelper.ActionColumn("Id", 90),
                                                 GridColumnHelper.TextColumn("IsFormC", "Form C (Y/N)", 90),
                                                 GridColumnHelper.TextColumn("BillingYearMonth", "Billing Year/Month", 160),
                                                 GridColumnHelper.TextColumn("PeriodNumber", "Billing Period", 140),
                                                 GridColumnHelper.TextColumn("BilledMemberName", "Billed Member", 170),
                                                 GridColumnHelper.TextColumn("InvoiceNumber", "Invoice Number", 120),
                                                 GridColumnHelper.TextColumn("BatchNumber", "Batch Number", 70),
                                                 GridColumnHelper.TextColumn("SequenceNumber", "Sequence Number", 100),
                                                 GridColumnHelper.TextColumn("CouponBreakdownSerialNumber", "Breakdown Serial No.", 100),
                                                 GridColumnHelper.TextColumn("OriginalFileName", "Supporting Document File Name", 180),
                                               };

        _grid.Columns[0].Formatter = formatter;
      }
      else if (billingCategoryType.Equals(BillingCategoryType.Misc) || billingCategoryType.Equals(BillingCategoryType.Uatp))
      {
        _grid.Columns = new List<JQGridColumn> {
                                                 GridColumnHelper.ActionColumn("Id", 90),
                                                 GridColumnHelper.TextColumn("BillingYearMonth", "Billing Year/Month", 160),
                                                 GridColumnHelper.TextColumn("PeriodNumber", "Billing Period", 140),
                                                 GridColumnHelper.TextColumn("BilledMemberName", "Billed Member", 170),
                                                 GridColumnHelper.TextColumn("InvoiceNumber", "Invoice Number", 120),
                                                 //GridColumnHelper.TextColumn("ChargeCategory", "Charge Category", 120),
                                                 GridColumnHelper.TextColumn("OriginalFileName", "Supporting Document File Name", 180),
                                               };

        _grid.Columns[0].Formatter = formatter;
      }
      else  if (billingCategoryType.Equals(BillingCategoryType.Cgo))
      {
          _grid.Columns = new List<JQGridColumn> {
                                                 GridColumnHelper.ActionColumn("Id", 90),
                                                 GridColumnHelper.TextColumn("BillingYearMonth", "Billing Year/Month", 160),
                                                 GridColumnHelper.TextColumn("PeriodNumber", "Billing Period", 140),
                                                 GridColumnHelper.TextColumn("BilledMemberName", "Billed Member", 170),
                                                 GridColumnHelper.TextColumn("InvoiceNumber", "Invoice Number", 120),
                                                 GridColumnHelper.TextColumn("BatchNumber", "Batch Number", 70),
                                                 GridColumnHelper.TextColumn("SequenceNumber", "Sequence Number", 100),
                                                 GridColumnHelper.TextColumn("CouponBreakdownSerialNumber", "Breakdown Serial No.", 100),
                                                 GridColumnHelper.TextColumn("OriginalFileName", "Supporting Document File Name", 180),
                                               };

          _grid.Columns[0].Formatter = formatter;
      }
    }
  }
}