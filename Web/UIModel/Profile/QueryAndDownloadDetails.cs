using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using SIS.Web.UIModels;
using Trirand.Web.Mvc;
using System.Linq;

namespace Iata.IS.Web.UIModel.Profile
{
  public class QueryAndDownloadDetails : GridBase
  {
    private JQGrid _queryNDownloadResultGrid;

    public override JQGrid GridInstance
    {
      get
      {
        if (_queryNDownloadResultGrid == null)
        {
          _queryNDownloadResultGrid = new JQGrid
          {
            Columns = new List<JQGridColumn>
                                           {
                                             new JQGridColumn
                                             {
                                               DataField = "SortOn",
                                               // always set PrimaryKey for Add,Edit,Delete operations
                                               // if not set, the first column will be assumed as primary key
                                               PrimaryKey = true,
                                               Editable = false,
                                               Width = 200,
                                               HeaderText = "Sort On"
                                             },
                                             new JQGridColumn
                                             {
                                               DataField = "SortingOrder",
                                               Editable = true,
                                               Width = 170,   
                                               TextAlign= Trirand.Web.Mvc.TextAlign.Left,
                                               HeaderText = "Order",        

                                             },        
                                           },
            
          };
        }

        _queryNDownloadResultGrid.Columns.Find(c => c.DataField == "SortingOrder").Formatter = new CustomFormatter { FormatFunction = "formatDropdown", UnFormatFunction = "unformatDropdown" };
        return _queryNDownloadResultGrid;
      }
    }

    public override void SetUpGrid()
    {

      // ID is a mandatory field. Must by unique if you have several grids on one page.
      GridInstance.ID = "QueryNDownloadDetaildDisplayGrid";

      // Setting the DataUrl to an action (method) in the controller is required.
      // This action will return the data needed by the grid

      GridInstance.DataUrl = ActionMethodUrl;

      //UrlHelper 
      // show the search toolbar
      GridInstance.AppearanceSettings.ShowFooter = false;
      GridInstance.ToolBarSettings.ShowRefreshButton = false;
      GridInstance.ToolBarSettings.ShowSearchToolBar = false;
      GridInstance.AppearanceSettings.ShrinkToFit = true;
      GridInstance.PagerSettings.ScrollBarPaging = true;
      GridInstance.AppearanceSettings.ScrollBarOffset = 18;
      GridInstance.Width = Unit.Percentage(100);
    }

    /// <summary>
    /// This method is called when the grid requests data.
    /// </summary>
    public override JsonResult FetchGridData()
    {

      SetUpGrid();

      List<SortOnList> list = new List<SortOnList>();
      list.Add(new SortOnList() { SortOn = "Location ID", SortingOrder = "test5" });
      list.Add(new SortOnList() { SortOn = "Country", SortingOrder = "test6" });
      list.Add(new SortOnList() { SortOn = "City", SortingOrder = "test7" });
      list.Add(new SortOnList() { SortOn = "Designator", SortingOrder = "test1" });
      list.Add(new SortOnList() { SortOn = "Prefix", SortingOrder = "test2" });
      list.Add(new SortOnList() { SortOn = "Commercial Name", SortingOrder = "test3" });
      list.Add(new SortOnList() { SortOn = "Legal Name", SortingOrder = "test4" });


      return GridInstance.DataBind(list.AsQueryable());
    }

    public QueryAndDownloadDetails(string url)
      : base(url)
    {
      SetUpGrid();
    }
  }
  public class SortOnList
  {
    public string SortOn { get; set; }
    public string SortingOrder { get; set; }

  }
}