using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Profile
{
  /// <summary>
  /// Grid for Profile Update list
  /// </summary>
  public class ProfileUpdateReport:GridBase
  {
    public ProfileUpdateReport(string gridId, string dataUrl) : base(gridId, dataUrl)
    {
    }

    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn>
                            {
                              GridColumnHelper.TextColumn("ElementName","Element Name",70),
                              GridColumnHelper.TextColumn("OldVAlue", "Old Value",50),
                              GridColumnHelper.TextColumn("NewVAlue","New Value",50),
                              GridColumnHelper.DateColumn("ChangeEffectiveOn","ChangeEffectiveOn"),
                            };


      }
    }
  }
}