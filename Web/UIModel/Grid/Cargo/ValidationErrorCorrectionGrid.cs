using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
    public class ValidationErrorCorrectionGrid : GridBase
    {
        public ValidationErrorCorrectionGrid(string gridId, string dataUrl)
            : base(gridId, dataUrl)
        {
        }
        protected override void InitializeColumns()
        {
          if (_grid != null)
          {
            _grid.Columns = new List<JQGridColumn>
                                    {
                                        GridColumnHelper.HiddenColumn("Id",isPrimaryKey:true),
                                        GridColumnHelper.SortableTextColumn("ExceptionCode","Exception Code",150),
                                        GridColumnHelper.SortableTextColumn("ErrorDescription", "Error Description", 250),
                                        GridColumnHelper.SortableTextColumn("FileName","File Name",250),
                                        GridColumnHelper.SortableTextColumn("MemberCode","Billed Member",100),
                                        GridColumnHelper.SortableTextColumn("InvoiceNo","Invoice Number",150),
                                        GridColumnHelper.SortableTextColumn("ErrorCount","Error Count",80),
                                        GridColumnHelper.SortableTextColumn("BatchUpdateAllowed","Batch Update Allowed",100),
                                        GridColumnHelper.HiddenColumn("InvoiceID"),
                                        GridColumnHelper.HiddenColumn("BilledMemberId"),
                                        GridColumnHelper.HiddenColumn("BillingMemberId"),
                                    };

            _grid.SortSettings.InitialSortColumn = "FileName";
            _grid.SortSettings.InitialSortDirection = SortDirection.Asc;

            var clientSideEvents = new ClientSideEvents { RowSelect = "DisplayDetails", GridInitialized = "SelectFirstRow" };
            //var clientSideEvents = new ClientSideEvents { GridInitialized = "SelectFirstRow" };
            _grid.ClientSideEvents = clientSideEvents;
              
          }
        }
    }
}