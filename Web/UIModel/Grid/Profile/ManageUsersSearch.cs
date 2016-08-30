using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Profile
{
  public class ManageUsersSearch : GridBase 
  {
    public ManageUsersSearch(string gridId, string dataUrl)
            : base(gridId, dataUrl)
        { 
        }

    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn>
                                    {
                                        GridColumnHelper.HiddenKeyColumn("ID"),
                                        GridColumnHelper.ActionColumn("Actions", 90),
                                        GridColumnHelper.TextColumn("FirstName", "First Name", 250,true),
                                        GridColumnHelper.TextColumn("LastName", "Last Name", 250,true),
                                        GridColumnHelper.TextColumn("EmailAddress", "Email Address", 300,true),
                                        GridColumnHelper.TextColumn("SuperUser", "Super User", 100,true),

                                    };

        _grid.SortSettings.InitialSortColumn = "FirstName";
        _grid.SortSettings.InitialSortDirection = SortDirection.Asc;
      //  var formatter = new CustomFormatter { FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID) };

        // Grid Checkbox column
       // _grid.MultiSelect = true;
       // _grid.MultiSelectMode = MultiSelectMode.SelectOnCheckBoxClickOnly;
        //_grid.Columns[0].Formatter = formatter;

      }
    }

  }
}