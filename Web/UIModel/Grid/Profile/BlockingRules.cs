using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Profile
{
  public class BlockingRules : GridBase
  {
    public BlockingRules(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }

    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn>
                                    {
                                        GridColumnHelper.ActionColumn("Id",80),
                                        GridColumnHelper.TextColumn("MemberText", "Member Name", 300),
                                        GridColumnHelper.TextColumn("RuleName", "Blocking Rule", 115),
                                        GridColumnHelper.TextColumn("Description", "Description", 280),
                                       
                                    };
        var formatter = new CustomFormatter
        {
          FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID)
        };

        _grid.Columns[0].Formatter = formatter;

      }
    }
  }
}