using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.MUPayables
{
    /// <summary>
    /// This class is used to initialize grid for misc payable attachment.
    /// CMP-665-User Related Enhancements-FRS-v1.2 [Sec 2.9: IS-WEB MISC Payables Invoice Search Screen]
    /// </summary>
    public class MiscPayableAttachmentGrid : GridBase
    {
        public MiscPayableAttachmentGrid(string gridId, string dataUrl)
            : base(gridId, dataUrl)
        {
        }

        /// <summary>
        /// Initializes columns for grid
        /// </summary>
        protected override void InitializeColumns()
        {
            if (_grid != null)
            {
                _grid.Columns = new List<JQGridColumn>
                          {
                            GridColumnHelper.ActionColumn("Id", 50),
                            GridColumnHelper.TextColumn("SerialNo", "Sr. No.", 90),
                            GridColumnHelper.TextColumn("OriginalFileName", "File Name", 300),
                            GridColumnHelper.CustomColumn("FileSizeInKb", "File Size (KB)", 90, TextAlign.Left, dataFormatString : "{0:N2}"),
                          };


                _grid.SortSettings.InitialSortColumn = "SerialNo";
                _grid.SortSettings.InitialSortDirection = SortDirection.Asc;


                _grid.Columns[0].Visible = false;

                var linkFormatter = new CustomFormatter
                                        {
                                            FormatFunction = "GetLinkForMiscPayableFileName"
                                        };
                _grid.Columns[2].Formatter = linkFormatter;
            }
        }

    }
}