using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
    public class AttachmentResultGrid : GridBase
    {
        public AttachmentResultGrid(string gridId, string dataUrl)
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

                          GridColumnHelper.ActionColumn("Id", 50),
                          GridColumnHelper.NumberColumn("SerialNo", "Sr. No.", 90),
                          GridColumnHelper.TextColumn("OriginalFileName", "File Name", 300),
                          GridColumnHelper.NumberColumn("FileSizeInKb", "File Size (KB)", 90,"{0:N2}"),
                          GridColumnHelper.DateTimeColumn("LastUpdatedOnInString", "Upload/Link Timestamp", 140),
                          GridColumnHelper.TextColumn("UserName", "Uploaded By", 150),
                         
                        };

      _grid.SortSettings.InitialSortColumn = "SerialNo";
      _grid.SortSettings.InitialSortDirection = SortDirection.Asc;

      CustomFormatter formatter = new CustomFormatter();
      formatter.FormatFunction = string.Format("{0}_DeleteAttachment", _grid.ID);
      _grid.Columns[0].Formatter = formatter;

      CustomFormatter linkFormatter = new CustomFormatter();
      linkFormatter.FormatFunction = "GetLinkForSupportingDocFileName";
      _grid.Columns[2].Formatter = linkFormatter;
      
    }

    }
}