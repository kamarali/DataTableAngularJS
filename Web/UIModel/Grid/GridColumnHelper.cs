using Iata.IS.Web.Util;
using Trirand.Web.Mvc;
using System;

namespace Iata.IS.Web.UIModel.Grid
{
  public class GridColumnHelper
  {
    /// <summary>
    /// Alignment for Number Columns
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="heading">The heading.</param>
    /// <param name="width">The width.</param>
    /// <param name="dataFormatString">The data format string.</param>
    /// <param name="isSortable">if set to <c>true</c> [is sortable].</param>
    /// <returns></returns>
    public static JQGridColumn NumberColumn(string name, string heading, int width = 90, string dataFormatString = null, bool isSortable = false)
    {
      return new JQGridColumn
      {
        DataField = name,
        Editable = false,
        Width = width,
        HeaderText = heading,
        TextAlign = TextAlign.Right,
        Sortable = isSortable,
        DataFormatString = dataFormatString
      };

    }

    /// <summary>
    /// Custom column
    /// </summary>
    public static JQGridColumn CustomColumn(string name, string heading, int width, TextAlign textAlign, JQGridColumnFormatter formatter = null, bool isVisible = true, string dataFormatString = null, bool isSortable = false)
    {
      var column = new JQGridColumn
                            {
                              DataField = name,
                              Editable = true,
                              Width = width,
                              HeaderText = heading,
                              TextAlign = textAlign,
                              Sortable = isSortable,
                              Visible = isVisible,
                              DataFormatString = dataFormatString
                            };

      if (formatter != null)
        column.Formatter = formatter;

      return column;
    }

    public static JQGridColumn ActionColumn(string name, int width, bool isViewOnly = false, string heading = "Actions", TextAlign textAlignment = TextAlign.Left, bool isPrimaryKey = true)
    {
      return new JQGridColumn
               {
                 DataField = name,
                 Editable = true,
                 Width = width,
                 HeaderText = heading,
                 TextAlign = textAlignment,
                 Sortable = false,
                 PrimaryKey = isPrimaryKey,
                 Visible = !isViewOnly
               };
    }

    /// <summary>
    /// Alignment for text Columns
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="heading">The heading.</param>
    /// <param name="width">The width.</param>
    /// <param name="isSortable">if set to <c>true</c> [is sortable].</param>
    /// <returns></returns>
    public static JQGridColumn TextColumn(string name, string heading, int width, bool isSortable = false)
    {
      return new JQGridColumn
      {
        DataField = name,
        Editable = false,
        HeaderText = heading,
        TextAlign = TextAlign.Left,
        Width = width,
        Sortable = isSortable
      };
    }

    /// <summary>
    /// Returns an instance of the currency column based on the parameters passed in.
    /// </summary>
    public static JQGridColumn AmountColumn(string name, string heading, int decimalPlaces = 2, int width = 90, bool isSortable = false)
    {
      return new JQGridColumn
      {
        DataField = name,
        Editable = false,
        Width = width,
        HeaderText = heading,
        TextAlign = TextAlign.Right,
        Formatter = new NumberFormatter { DecimalPlaces = decimalPlaces, DecimalSeparator = ".", ThousandsSeparator = "," },
        Sortable = isSortable
      };
    }

    /// <summary>
    /// Returns an instance of the currency column based on the parameters passed in.
    /// </summary>
    public static JQGridColumn PercentColumn(string name, string heading, int width = 90, bool isSortable = false)
    {
      return new JQGridColumn
      {
        DataField = name,
        Editable = false,
        Width = width,
        HeaderText = heading,
        TextAlign = TextAlign.Right,
        Sortable = isSortable,
        Formatter = new NumberFormatter { DecimalPlaces = 3 }
      };
    }

    internal static JQGridColumn DateColumn(string name, string heading, int width = 110)
    {
      return new JQGridColumn
      {
        DataField = name,
        Editable = false,
        Width = width,
        DataFormatString = FormatConstants.GridColumnDateFormat,
        HeaderText = heading,
        TextAlign = TextAlign.Left
      };
    }

    /// <summary>
    /// Create new date time column for
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="heading">The heading.</param>
    /// <param name="width">new parameter added for ISCalender Grid header is too long</param>
    /// <param name="isSortable">if set to <c>true</c> [is sortable].</param>
    /// <returns></returns>
    internal static JQGridColumn DateTimeColumn(string name, string heading, int width = 110, bool isSortable = false)
    {
      return new JQGridColumn
      {
        DataField = name,
        Editable = false,
        Width = width,
        DataFormatString = FormatConstants.GridColumnDateTimeFormat,
        HeaderText = heading,
        TextAlign = TextAlign.Left,
        Sortable = isSortable
      };
    }

    internal static JQGridColumn HiddenKeyColumn(string name)
    {
      return new JQGridColumn
      {
        DataField = name,
        Editable = false,
        Visible = false,
        PrimaryKey = true
      };
    }

    internal static JQGridColumn HiddenColumn(string name,bool isPrimaryKey = false)
    {
      return new JQGridColumn
      {
        DataField = name,
        Editable = false,
        Visible = false,
        PrimaryKey = isPrimaryKey
      };
    }


    /// <summary>
    /// Returns an instance of the Exchange rate column based on the parameters passed in.
    /// </summary>
    public static JQGridColumn ExchangeRateColumn(string name, string heading, int width = 90, bool isSortable = false)
    {
      return new JQGridColumn
      {
        DataField = name,
        Editable = false,
        Width = width,
        HeaderText = heading,
        TextAlign = TextAlign.Right,
        Sortable = isSortable,
        Formatter = new NumberFormatter { DecimalPlaces = 5 }
      };
    }

    /// <summary>
    /// Returns an instance of the Color Coded Status column.
    /// </summary>
    public static JQGridColumn ColorCodedStatusColumn(string name, string heading, int width)
    {
      return new JQGridColumn
      {
        DataField = name,
        Editable = false,
        Width = width,
        HeaderText = heading,
        TextAlign = TextAlign.Center,
        Sortable = true
      };
    }

    /// <summary>
    /// Following method is used for sortable Text columns 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="heading"></param>
    /// <param name="width"></param>
    /// <returns></returns>
    public static JQGridColumn SortableTextColumn(string name, string heading, int width)
    {
      return new JQGridColumn
      {
        DataField = name,
        Editable = false,
        HeaderText = heading,
        TextAlign = TextAlign.Left,
        Width = width,
        Sortable = true
      };
    }

    /// <summary>
    /// Create new date with full year time column for
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="heading">The heading.</param>
    /// <param name="width">new parameter added for ISCalender Grid header is too long</param>
    /// <param name="isSortable">if set to <c>true</c> [is sortable].</param>
    /// <returns></returns>
    internal static JQGridColumn DateFullYearTimeColumn(string name, string heading, int width = 110, bool isSortable = false)
    {
        return new JQGridColumn
        {
            DataField = name,
            Editable = false,
            Width = width,
            DataFormatString = FormatConstants.GridColumnDateFullYearTimeFormat,
            HeaderText = heading,
            TextAlign = TextAlign.Left,
            Sortable = isSortable
        };
    }

    //Added following for fixing Spira issue IN:005629, for showing date in yyMMMpp format, where pp is period
    //Roleback issue id :6237, for showing date in ppMMMyyyy format, where pp is period
    /// <summary>
    /// Create new date with ppMMMyyyy, where pp is period
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="heading">The heading.</param>
    /// <param name="width">new parameter added for ISCalender Grid header is too long</param>
    /// <param name="isSortable">if set to <c>true</c> [is sortable].</param>
    /// <returns></returns>
    internal static JQGridColumn DateFullYearFormat(string name, string heading, int width = 110, bool isSortable = false)
    {
        return new JQGridColumn
        {
            DataField = name,
            Editable = false,
            Width = width,
            DataFormatString = FormatConstants.GridColumnDateFullYearFormat,
            HeaderText = heading,
            TextAlign = TextAlign.Left,
            Sortable = isSortable
        };
    }
    
  }
}
