using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Profile
{
  public class Contacts : GridBase
  {
    public Contacts(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }

    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn>
                                {
                                  GridColumnHelper.ActionColumn("Id", 80),
                                        GridColumnHelper.TextColumn("FirstName", "First Name", 120),
                                         GridColumnHelper.TextColumn("LastName", "Last Name", 120),
                                        GridColumnHelper.TextColumn("EmailAddress", "Email ID", 180),
                                       GridColumnHelper.TextColumn("StaffId", "Staff ID", 120),
                                       
                                    };
        var clientSideEvents = new ClientSideEvents { RowSelect = "DisplayDetails", GridInitialized = "SelectFirstRow" };
        _grid.ClientSideEvents = clientSideEvents;
        var formatter = new CustomFormatter
        {
          FormatFunction = string.Format("{0}_DeleteRecord", _grid.ID)
        };

        _grid.Columns[0].Formatter = formatter;
      }
    }
  }

}