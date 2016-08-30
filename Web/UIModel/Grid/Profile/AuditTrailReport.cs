using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Profile
{
  public class AuditTrailReport:GridBase
  {
    public AuditTrailReport(string gridId, string dataUrl) : base(gridId, dataUrl)
    {
    }

    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn>
                            {
                              GridColumnHelper.TextColumn("Id", "Audit Id",150),
                              GridColumnHelper.TextColumn("DisplayBilateralMember", "Bilateral Partner",150),
                              GridColumnHelper.TextColumn("DisplayGroup","Group",150),
                              GridColumnHelper.TextColumn("ElementName","Element Name",170),
                              GridColumnHelper.TextColumn("OldVAlue", "Old Value",150),
                              GridColumnHelper.TextColumn("NewVAlue","New Value",150),
                              GridColumnHelper.TextColumn("ChangeApprovedByUserId", "Approved By",150),
                              GridColumnHelper.TextColumn("NewVAlue","New Value",50),
                              GridColumnHelper.DateColumn("LastUpdatedOn", "Changed On"),
                              GridColumnHelper.TextColumn("LastUpdatedBy","Changed By",50),
                              GridColumnHelper.TextColumn("DisplayActionType","Action",50),
                              GridColumnHelper.DateColumn("ChangeEffectiveOn","ChangeEffectiveOn"),
                            };

      }
    }
  }
}