using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Masters
{
  public class ChargeCodeTypeNameSetUpSearch : GridBase
  {
       /// <summary>
    /// Initializes a new instance of the <see cref="ChargeCodeTypeNameSetUpSearch"/> class.
    /// </summary>
    /// <param name="gridId">The grid id.</param>
    /// <param name="dataUrl">The data URL.</param>
    public ChargeCodeTypeNameSetUpSearch(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }

    /// <summary>
    /// Initializes the columns.
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn>
                                    {
                                        GridColumnHelper.ActionColumn("Id",80),
                                        //TFS#9976 :Firefox: v47 : Master maintenance:Incorrect Activate ,Deactivate action buttons are displayed for the records in Charge Code Type Requirement Setup and Charge Code Type Name Setup masters
                                        GridColumnHelper.HiddenColumn("IsActive"),
                                        GridColumnHelper.TextColumn("ChargeCategory", "Charge Category", 200,true),
                                        GridColumnHelper.TextColumn("ChargeCode", "Charge Code", 200,true),
                                        GridColumnHelper.TextColumn("ChargeCodeTypeName", "Charge Code Type Name", 300,true),
                                        //TFS#9976 :Firefox: v47 : Master maintenance:Incorrect Activate ,Deactivate action buttons are displayed for the records in Charge Code Type Requirement Setup and Charge Code Type Name Setup masters
                                        GridColumnHelper.TextColumn("IsActive", "Active", 115, true),
                                        GridColumnHelper.DateTimeColumn("LastUpdatedOn", "Last Updated On", 280, true),
                                    };
        _grid.SortSettings.InitialSortColumn = "ChargeCategory";
        _grid.SortSettings.InitialSortDirection = SortDirection.Asc;
        var formatter = new CustomFormatter
        {
          FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID)
        };

        _grid.Columns[0].Formatter = formatter;

      }
    }
  }
}