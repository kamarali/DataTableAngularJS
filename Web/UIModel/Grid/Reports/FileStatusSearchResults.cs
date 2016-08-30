using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Iata.IS.Web.Util;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Reports
{
  public class FileStatusSearchResults : GridBase
  {
    public FileStatusSearchResults(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }

    /// <summary>
    /// Following method is used to initialize Columns of FileSearchResults grid on FileStatusSearchResultControl.ascx
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
          // Following code is used to add multiselect checkbox in Jqgrid header row.
          _grid.MultiSelectMode = MultiSelectMode.SelectOnCheckBoxClickOnly;
          _grid.MultiSelect = true;


          _grid.Columns = new List<JQGridColumn>
                              {
                                  GridColumnHelper.HiddenColumn("IsFileLogId",isPrimaryKey:true),
                                  GridColumnHelper.HiddenColumn("NumberOfInvoicesInFile"),
                                  GridColumnHelper.HiddenColumn("NumberOfValidInvoicesInFile"),
                                  GridColumnHelper.HiddenColumn("NumberOfInvalidInvoicesInFile"),
                                  GridColumnHelper.HiddenColumn("FileStatusId"),
                                  GridColumnHelper.HiddenColumn("FileFormatId"),
                                  GridColumnHelper.HiddenColumn("RejectOnValidationFailure"),
                                  GridColumnHelper.DateTimeColumn("FileGeneratedDate", "File Generated Date", 130, true),
                                  GridColumnHelper.SortableTextColumn("FileName", "File Name", 230),
                                  GridColumnHelper.SortableTextColumn("BillingCategory", "Billing Category", 90),
                                  GridColumnHelper.SortableTextColumn("BillingMemberCode", "Billing Member",
                                                                      MemberNumericCodeColoumnWidth),
                                  // CMP#596 Update grid column as per Table 3, Table 9,Table 13 and Table 21  
                                  GridColumnHelper.SortableTextColumn("BillingMemberName", "Billing Member Name", 120),
                                  GridColumnHelper.SortableTextColumn("FileFormat", "File Format", 80),
                                  GridColumnHelper.DateTimeColumn("ReceivedbyIS", "Received by IS", 130, true),
                                  GridColumnHelper.SortableTextColumn("FileStatus", "File Status", 140),
                                  GridColumnHelper.HiddenColumn("InvoicesInPeriodError"),
                                  /* CMP #675: Progress Status Bar for Processing of Billing Data Files. 
                                   * Desc: New column Added. */
                                  GridColumnHelper.ActionColumn("FileProgressStatus", 80, heading: "File Progress Status", textAlignment: TextAlign.Center,isPrimaryKey:false)
                            };


      }
      _grid.Columns.Find(c => c.DataField == "FileStatus").Formatter = new CustomFormatter
      {
        FormatFunction = "formatFileStatus"
      };

      _grid.SortSettings.InitialSortColumn = "ReceivedByIS";
      _grid.SortSettings.InitialSortDirection = SortDirection.Desc;

      /* CMP #675: Progress Status Bar for Processing of Billing Data Files. 
       * Desc: Formatter Added for New Column. */
      _grid.Columns.Find(column => column.DataField == "FileProgressStatus").Formatter = new CustomFormatter
                                                                                               {
                                                                                                   FormatFunction =
                                                                                                       string.Format(
                                                                                                           "{0}_GenerateProgressBarAction",
                                                                                                           ControlIdConstants
                                                                                                               .
                                                                                                               PDFileStatusGrid)
                                                                                               };

    }// end InitializeColumns()  
  }// end FileStatusSearchResults class
}// end namespace