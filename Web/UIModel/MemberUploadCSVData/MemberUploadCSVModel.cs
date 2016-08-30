using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Iata.IS.Web.Util;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.MemberUploadCSVData
{
    /// <summary>
    /// CMP #608: Load Member Profile - CSV Option
    /// </summary>
    public class MemberUploadCSVModel
    {
        public JQGrid MemberUploadCSVGrid { get; set; }

        public MemberUploadCSVModel()
        {
            MemberUploadCSVGrid = new JQGrid
          {
              Columns = new List<JQGridColumn>(){
            new JQGridColumn { HeaderText = "File Uploaded On (EST/EDT)",
                                        DataField = "FileUploadDate", 
                                        PrimaryKey = true,
                                        Width = 200,
                                        DataFormatString = FormatConstants.GridColumnDateTimeFormat },
                       new JQGridColumn {HeaderText = "Uploaded By",
                                        DataField = "UploadedBy", 
                                        PrimaryKey = false,
                                        Width = 100,
                                        Sortable = true},
                       new JQGridColumn {HeaderText = "Status",
                                        DataField = "Status", 
                                        PrimaryKey = false,
                                        Width = 100,
                                        Sortable = true},
                        new JQGridColumn { HeaderText = "File Name",
                                        DataField = "FileName", 
                                        PrimaryKey = false,
                                        Width = 300,
                                        Sortable = true},
                        new JQGridColumn { HeaderText = "Action",
                                            DataField = "DownloadFileId", 
                                            Width = 50,
                                            Sortable = false}
                    },
              Width = Unit.Percentage(100),
          };
            MemberUploadCSVGrid.SortSettings.InitialSortColumn = "FileUploadDate";
            MemberUploadCSVGrid.SortSettings.InitialSortDirection = Trirand.Web.Mvc.SortDirection.Desc;
            MemberUploadCSVGrid.ToolBarSettings.ShowRefreshButton = true;
        }
    }
}