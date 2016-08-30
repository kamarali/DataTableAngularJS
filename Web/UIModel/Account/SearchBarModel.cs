using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Trirand.Web.Mvc;
using System.Web.UI.WebControls;

namespace SIS.Web.UIModels.Account
{
  public class SearchBarModel
  {
    /// <summary>
    /// 
    /// </summary>
    public JQGrid OrdersGrid { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Status { get; set; }

    public int StatusId { get; set; }

    public string ViewMode { get; set; }

    public int RecordCount { get; set; }

    public string MemberName { get; set; }

    public int MemberId { get; set; }

    public int UserCategoryId { get; set; }


    /// <summary>
    /// 
    /// </summary>
    public SearchBarModel()
    {
      OrdersGrid = new JQGrid
      {
        Columns = new List<JQGridColumn>()
                    {  
                         new JQGridColumn {
                             DataField = "ID", 
                             PrimaryKey = true,
                             Editable = false,
                             Visible=false
                        },
                        new JQGridColumn {DataField = "Actions", 
                                        Width = 230},
                        new JQGridColumn {HeaderText = "First Name", 
                                        DataField = "FirstName", 
                                        Width = 300 },                                    
                        new JQGridColumn {HeaderText = "Last Name", 
                                        DataField = "LastName", 
                                        Width = 300},
                        new JQGridColumn {HeaderText = "Email Address",
                                        DataField = "EmailAddress", 
                                        Width = 400, 
                                        },
                        new JQGridColumn {HeaderText = "Super User",
                                        DataField = "SuperUser", 
                                        Width = 200, 
                                        },
                        // CMP#668: Archival of IS-WEB Users and Removal from Screens
                        new JQGridColumn {HeaderText = "Is Archived",
                                        DataField = "IsArchived",
                                        Width = 200,
                                        }
                    },
        Width = Unit.Pixel(740),
        ID = "UserSearchGrid",
      };
      OrdersGrid.ToolBarSettings.ShowRefreshButton = true;
    }

  }
}