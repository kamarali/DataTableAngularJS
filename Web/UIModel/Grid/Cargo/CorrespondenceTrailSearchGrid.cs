using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Iata.IS.Web.Util;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
  public class CorrespondenceTrailSearchGrid : GridBase
  {
    public CorrespondenceTrailSearchGrid(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }

    /// <summary>
    /// Initializes columns for grid
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid == null)
      {
        return;
      }
   
      _grid.Columns = new List<JQGridColumn> 
                        { 
                          GridColumnHelper.TextColumn("TransactionNumber", "Correspondence Ref No.", 120, true),
                          GridColumnHelper.TextColumn("TransactionDate", "Correspondence Date", 100, true),
                          GridColumnHelper.TextColumn("MemberCode", "Member Code", MemberNumericCodeColoumnWidth, true), // CMP#596 Update grid column as per Table 3, Table 9,Table 13 and Table 21  
                          GridColumnHelper.TextColumn("DisplayCorrespondenceStatus", "Correspondence Status",100, true),
                          GridColumnHelper.TextColumn("DisplayCorrespondenceSubStatus","Correspondence Sub Status",100, true),
                          GridColumnHelper.NumberColumn("NoOfDaysToExpire","Number Of Days To Expire", isSortable: true),
                          GridColumnHelper.TextColumn("AuthorityToBill","Authority To Bill",60, true),
                          GridColumnHelper.TextColumn("TotalNetAmount", "Transaction Amount",120, true),
                        
                        };


      _grid.Columns.Find(column => column.DataField == "AuthorityToBill").Formatter = new CustomFormatter { FormatFunction = "SetAuthorityToBill" };

      _grid.MultiSelect = true;

      _grid.MultiSelectMode = MultiSelectMode.SelectOnCheckBoxClickOnly;

      _grid.ClientSideEvents = new ClientSideEvents
      {
        RowSelect = "GetSelectedRecordId"
      };
    }
  }
}
