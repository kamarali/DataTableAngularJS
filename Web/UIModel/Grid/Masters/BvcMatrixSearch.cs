using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;
namespace Iata.IS.Web.UIModel.Grid.Masters
{
    public class BvcMatrixSearch : GridBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BvcMatrixSearch"/> class.
        /// </summary>
        /// <param name="gridId">The grid id.</param>
        /// <param name="dataUrl">The data URL.</param>
        public BvcMatrixSearch(string gridId, string dataUrl)
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
                                        GridColumnHelper.HiddenColumn("IsActive"),
                                        GridColumnHelper.SortableTextColumn("ValidatedPmi", "Validated PMI", 80),
                                        GridColumnHelper.SortableTextColumn("EffectiveFrom", "Effective From", 115),
                                        GridColumnHelper.SortableTextColumn("EffectiveTo", "Effective To", 115),
                                        GridColumnHelper.SortableTextColumn("IsFareAmount", "Fare Amount", 80),
                                        GridColumnHelper.SortableTextColumn("IsHfAmount", "HF Amount", 80),
                                        GridColumnHelper.SortableTextColumn("IsTaxAmount", "Tax Amount", 80),
                                        GridColumnHelper.SortableTextColumn("IsIscPercentage", "ISC Percentage", 80),
                                        GridColumnHelper.SortableTextColumn("IsUatpPercentage", "UATP Percentage", 80),
                                        GridColumnHelper.SortableTextColumn("IsActive", "Active", 115),
                                         GridColumnHelper.DateTimeColumn("LastUpdatedOn", "Last Updated On", 150,true),
                                       
                                    };
        _grid.SortSettings.InitialSortColumn = "LastUpdatedOn";
        _grid.SortSettings.InitialSortDirection = SortDirection.Desc;
        _grid.Width = 890;
        var formatter = new CustomFormatter
        {
          FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID)
        };

        _grid.Columns[0].Formatter = formatter;

      }
    }
    }
}