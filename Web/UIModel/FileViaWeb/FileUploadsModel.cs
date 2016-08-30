using System.Collections.Generic;
using System.Web.UI.WebControls;
using Iata.IS.Web.Util;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.FileViaWeb
{
  public class FileUploadsModel
  {
    public JQGrid FileUploadGrid { get; set; }

    public FileUploadsModel()
    {
      FileUploadGrid = new JQGrid
      {
        Columns = new List<JQGridColumn>(){
                        new JQGridColumn { HeaderText = "File Name",
                                        DataField = "FileName", 
                                        PrimaryKey = false,
                                        Width = 300 },
                        new JQGridColumn { HeaderText = "File Type",
                                        DataField = "FileType", 
                                        PrimaryKey = false,
                                        Width = 200 },
                        new JQGridColumn {HeaderText = "Received Date",
                                        DataField = "ReceivedDate", 
                                        PrimaryKey = false,
                                        Width = 150 },
                       new JQGridColumn {HeaderText = "Billing Period-Month-Year",
                                        DataField = "BillingPeriod", 
                                        PrimaryKey = false,
                                        Width = 150 },
                        new JQGridColumn { HeaderText = "Status",
                                        DataField = "FileLogStatus", 
                                        Width = 150 },
                        /* CMP #675: Progress Status Bar for Processing of Billing Data Files. Desc: New Columns Added. */
                        new JQGridColumn { DataField = "FileProgressStatus",
                                        Editable = true, Width = 80,
                                        HeaderText = "File Progress Status",
                                        TextAlign = Trirand.Web.Mvc.TextAlign.Center,
                                        Sortable = false, Visible = true },
                        new JQGridColumn { DataField = "IsFileLogId",
                                        Editable = false, Visible = false}
                    },
        Width = Unit.Percentage(100),
      };
      FileUploadGrid.SortSettings.InitialSortColumn = "ReceivedDate";
      FileUploadGrid.SortSettings.InitialSortDirection = Trirand.Web.Mvc.SortDirection.Desc;
      FileUploadGrid.ToolBarSettings.ShowRefreshButton = true;
      /* CMP #675: Progress Status Bar for Processing of Billing Data Files. Desc: Formatter Added for New Column. */
      FileUploadGrid.Columns.Find(column => column.DataField == "FileProgressStatus").Formatter =
                          new CustomFormatter
                          {
                              FormatFunction =
                                  string.Format(
                                      "{0}_GenerateProgressBarAction",
                                      ControlIdConstants.UploadFileStatusGrid)
                          };
    }
  }
}