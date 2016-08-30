using System.Collections.Generic;
using System.Web.UI.WebControls;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.FileViaWeb
{
  public class FileDownloadSearchModel
  {
    public JQGrid FileDownloadSearchGrid { get; set; }


    public FileDownloadSearchModel()
    {
      FileDownloadSearchGrid = new JQGrid
      {
        Columns = new List<JQGridColumn>()
                    {
                      new JQGridColumn {
                             DataField = "ID", 
                             PrimaryKey = true,
                             Editable = false,
                             Visible=false
                        },
                          new JQGridColumn { DataField = "FileDate", HeaderText="File Generated On",
                                           PrimaryKey = false,
                                            Width = 120 },    
                        new JQGridColumn { DataField = "BillingMonth", HeaderText="Billing Month",
                                           PrimaryKey = false,
                                            Width = 120 },                                    
                        new JQGridColumn { DataField = "BillingPeriod",  HeaderText="Billing Period",
                                           Width = 120},
                        new JQGridColumn { DataField = "Filename",  HeaderText="File Name",
                                           Width = 300
                                         },
                        new JQGridColumn { DataField = "FileType",  HeaderText="File Type",
                                        Width = 300 },
                        //CMP#622: MISC Outputs Split as per Location ID
                        new JQGridColumn { DataField = "MiscLocationCode",  HeaderText="Location ID",
                                           Width = 90},
                        new JQGridColumn { DataField = "Action", 
                                        Width = 80 }
                    },
       
        Width = Unit.Percentage(100),
      };
      FileDownloadSearchGrid.SortSettings.InitialSortColumn = "FileDate";
      FileDownloadSearchGrid.SortSettings.InitialSortDirection = Trirand.Web.Mvc.SortDirection.Desc;
      FileDownloadSearchGrid.ToolBarSettings.ShowRefreshButton = true;
    }

  }
}