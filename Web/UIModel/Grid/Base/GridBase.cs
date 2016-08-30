using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Iata.IS.AdminSystem;
using Iata.IS.Core.Configuration;
using Iata.IS.Web.Util;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Base
{
  /// <summary>
  /// Wrapper over JQGrid.
  /// </summary>
  public abstract class GridBase
  {
    protected JQGrid _grid;
    protected bool _isGridViewOnly;
    protected bool _isCheckBoxRequired;
    private readonly int _pageSizeSelected;
    private readonly string _currentPageGridId;
    private readonly int _currentPageSelected;
    protected readonly bool _isDisplayChargeCategory;
    protected readonly bool _isdisplayDailyDeliveryStatusCol;
    protected readonly bool _isDisplayChargeCategoryType;
    protected readonly bool _isDisplayRejectionReasonCode;
    protected readonly int MemberNumericCodeColoumnWidth = 150;//CMP#596 Update grid column as per Table 3, Table 9,Table 13 and Table 21

    protected GridBase(string gridId, string dataUrl,int? currentPageNo, int? currentPageSize, bool isGridViewOnly = false, bool isCheckBoxRequired = false, bool isDisplayChargeCategory = false, bool isDisplayChargeCategoryType = false,bool isDisplayRejectionReasonCode=false)
    {
        _isGridViewOnly = isGridViewOnly;
        _isCheckBoxRequired = isCheckBoxRequired;
        _isDisplayChargeCategory = isDisplayChargeCategory;
        _isDisplayChargeCategoryType = isDisplayChargeCategoryType;
        _grid = new JQGrid {ID = gridId};
        _pageSizeSelected = currentPageSize ?? SessionUtil.PageSizeSelected;
        _currentPageGridId = SessionUtil.CurrentPageGridId;
        _currentPageSelected = currentPageNo ?? SessionUtil.CurrentPageSelected;
        _isDisplayRejectionReasonCode = isDisplayRejectionReasonCode;
        AlternateRowBackground = true;
        // Set Default Page size for Grid in Application Variable for First time

        if (string.IsNullOrEmpty(GlobalVariables.PageSizeOptions))
        {
            GlobalVariables.PageSizeOptions = SystemParameters.Instance.UIParameters.PageSizeOptions;
        }
        
        PageSizeOptions = GlobalVariables.PageSizeOptions;
 
        // Set Default Page size for Grid in Application Variable for First time
        if (GlobalVariables.DefaultPageSize == 0)
        {
            GlobalVariables.DefaultPageSize = SystemParameters.Instance.UIParameters.DefaultPageSize;
        }

        // Get the default page size from session if present, else fetch it from the configuration.
        DefaultPageSize = _pageSizeSelected != 0 ? _pageSizeSelected : GlobalVariables.DefaultPageSize;
        ShowRefreshButton = false;

        //get the current page index from JQGrid navigator text box for maintain current page status by clicking the edit button.
        var currentPageIndex = _currentPageSelected;
        if (currentPageIndex > 0)
        {
             CurrentPageNumber = currentPageIndex;
        }
       
        // Assign the data url using which data will be fetched.
        _grid.DataUrl = dataUrl;

        // Disable the search toolbar.
        _grid.ToolBarSettings.ShowSearchToolBar = false;

        // Set the appearance.
        _grid.ColumnReordering = false;
        _grid.AppearanceSettings.ShrinkToFit = true;
        _grid.Width = Unit.Percentage(100);
       
        // This is used to determine if the gridContainer CSS class is required or not. adjustGridContainer function is defined in site.js
        _grid.ClientSideEvents.GridInitialized = "adjustGridContainer";

        // Initialize the columns required for the grid.
        InitializeColumns();
    }


    protected GridBase(string gridId, string dataUrl, bool isGridViewOnly = false, bool isCheckBoxRequired = false, bool isDisplayChargeCategory = false, bool isDisplayChargeCategoryType = false, bool isdisplayDailyDeliveryStatusCol = false, bool isDisplayRejectionReasonCode = false)
    {
      _isGridViewOnly = isGridViewOnly;
      _isCheckBoxRequired = isCheckBoxRequired;
        _isDisplayChargeCategory = isDisplayChargeCategory;
      _isDisplayChargeCategoryType = isDisplayChargeCategoryType;
      _isdisplayDailyDeliveryStatusCol = isdisplayDailyDeliveryStatusCol;
      _grid = new JQGrid { ID = gridId };
      _pageSizeSelected = SessionUtil.PageSizeSelected;
      _currentPageGridId = SessionUtil.CurrentPageGridId;
      _currentPageSelected = SessionUtil.CurrentPageSelected;
      _isDisplayRejectionReasonCode = isDisplayRejectionReasonCode;
      AlternateRowBackground = true;
      // Set Default Page size for Grid in Application Variable for First time

      if (string.IsNullOrEmpty(GlobalVariables.PageSizeOptions))
      {
        GlobalVariables.PageSizeOptions = SystemParameters.Instance.UIParameters.PageSizeOptions;
      }
      PageSizeOptions = GlobalVariables.PageSizeOptions;
      if (gridId == ControlIdConstants.ArchiveRetrivalJobDetailsGridControl)
      {
          PageSizeOptions = "[5,10,20,100,300,500]";
          _grid.Height = Unit.Pixel(400);
      }
      else
      {
          PageSizeOptions = GlobalVariables.PageSizeOptions;
      }

      // Set Default Page size for Grid in Application Variable for First time

      if (GlobalVariables.DefaultPageSize ==0)
      {
        GlobalVariables.DefaultPageSize = SystemParameters.Instance.UIParameters.DefaultPageSize;
      }
      
      // Get the default page size from session if present, else fetch it from the configuration.
      DefaultPageSize = _pageSizeSelected != 0 ? _pageSizeSelected : GlobalVariables.DefaultPageSize;

        ShowRefreshButton = false;

      //get the current page index from JQGrid navigator text box for maintain current page status by clicking the edit button.
      if (_currentPageGridId == gridId)
      {
        var currentPageIndex = _currentPageSelected;
        if (currentPageIndex > 0)
        {
          CurrentPageNumber = currentPageIndex;
        }
      }
      else
      {
        SessionUtil.CurrentPageSelected = 1;
        SessionUtil.CurrentPageGridId = gridId;
      }

      // Assign the data url using which data will be fetched.
      _grid.DataUrl = dataUrl;

      // Disable the search toolbar.
      _grid.ToolBarSettings.ShowSearchToolBar = false;

      // Set the appearance.
      _grid.ColumnReordering = false;
      _grid.AppearanceSettings.ShrinkToFit = true;
      _grid.Width = Unit.Percentage(100);

      // This is used to determine if the gridContainer CSS class is required or not. adjustGridContainer function is defined in site.js
      _grid.ClientSideEvents.GridInitialized = "adjustGridContainer";

      // Initialize the columns required for the grid.
      InitializeColumns();
    }

    /// <summary>
    /// The url that is used to fetch data for the grid.
    /// </summary>
    public string DataUrl
    {
      get
      {
        return _grid.DataUrl;
      }
      set
      {
        _grid.DataUrl = value;
      }
    }

    /// <summary>
    /// Renders the grid with alternate row background colors.
    /// </summary>
    public bool AlternateRowBackground
    {
      get
      {
        return _grid.AppearanceSettings.AlternateRowBackground;
      }

      set
      {
        _grid.AppearanceSettings.AlternateRowBackground = value;
      }
    }

    /// <summary>
    /// Returns Grid instance 
    /// </summary>
    public JQGrid Instance
    {
      get
      {
        return _grid;
      }
    }

    /// <summary>
    /// Comma separated page size options enclosed in square brackets. E.g. [5,10, 15, 20]
    /// </summary>
    public string PageSizeOptions
    {
      get
      {
        return _grid.PagerSettings.PageSizeOptions;
      }

      set
      {
        _grid.PagerSettings.PageSizeOptions = value;
      }
    }

    /// <summary>
    /// Put the value on navigator text box for maintain current page of JQGrid whene edit and click navigator icon of JQGrid.
    ///  </summary>
    public int CurrentPageNumber
    {
      get
      {
        return _grid.PagerSettings.CurrentPage;
      }

      set
      {
        _grid.PagerSettings.CurrentPage = value;
      }
    }

    /// <summary>
    /// Default page size to be displayed on the grid.
    /// </summary>
    public int DefaultPageSize
    {
      get
      {
        return _grid.PagerSettings.PageSize;
      }

      set
      {
        _grid.PagerSettings.PageSize = value;
      }
    }

    /// <summary>
    /// Hide/display footer
    /// </summary>
    public bool ShowFooter
    {
      get
      {
        return _grid.AppearanceSettings.ShowFooter;
      }

      set
      {
        _grid.AppearanceSettings.ShowFooter = value;
      }
    }

    /// <summary>
    /// Shows the refresh button.
    /// </summary>
    public bool ShowRefreshButton
    {
      get
      {
        return _grid.ToolBarSettings.ShowRefreshButton;
      }

      set
      {
        _grid.ToolBarSettings.ShowRefreshButton = value;
      }
    }

    /// <summary>
    /// Columns will be adjusted as per content to fit in total width of grid
    /// </summary>
    public bool ShrinkToFit
    {
      get
      {
        return _grid.AppearanceSettings.ShrinkToFit;
      }

      set
      {
        _grid.AppearanceSettings.ShrinkToFit = value;
      }
    }

    /// <summary>
    /// Width of the grid in pixel.
    /// </summary>
    //public int WidthInPixel { get; set; }


    /// <summary>
    /// SCP000: AntiXSS Fix IS WEB
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public JsonResult DataBind<T>(IQueryable<T> data)
    {
        if (data == null)
        {
            return null;
        }
        return Instance.DataBind(data.ToSanitizeStringField());
    }

    public JsonResult DataBind<T>(IList<T> data)
    {
        if (data == null)
        {
            return null;
        }
        return Instance.DataBind(data.ToSanitizeStringField());
    }

    public JsonResult DataBind<T>(IEnumerable<T> data)
    {
        if (data == null)
        {
            return null;
        }
        return Instance.DataBind(data.ToSanitizeStringField());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public JsonResult DataBind(object data)
    {
      if (data == null)
      {
        return null;
      }

      return Instance.DataBind(data);
    }

    protected abstract void InitializeColumns();
  }
}
