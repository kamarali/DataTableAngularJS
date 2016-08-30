using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Profile
{
    public class BlocksbyGroupExceptions : GridBase
    {
        public BlocksbyGroupExceptions(string gridId, string dataUrl)
            : base(gridId, dataUrl)
        {
        }

        protected override void InitializeColumns()
        {
            if (_grid != null)
            {
                _grid.Columns = new List<JQGridColumn>
                                    {
                                        GridColumnHelper.ActionColumn("ExceptionMemberId",50),
                                        GridColumnHelper.TextColumn("DisplayMemberCode", "Member Code", MemberNumericCodeColoumnWidth), // CMP#596 Update grid column as per Table 3, Table 9,Table 13 and Table 21  
                                        GridColumnHelper.TextColumn("DisplayMemberCommercialName", "Member Name", 250),
                                       
                                    };
                var formatter = new CustomFormatter
                {
                    FormatFunction = string.Format("{0}_DeleteRecord", _grid.ID)
                };

                _grid.Columns[0].Formatter = formatter;

            }
        }
    }
}