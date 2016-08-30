using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Iata.IS.Business.Common;
using Iata.IS.Core.DI;

namespace Iata.IS.Web.Util.ExtensionHelpers
{
  public static class TextBoxExtensions
  {
    public static MvcHtmlString FlightDateTextBoxFor(this HtmlHelper html, string controlName, int? flightDay, int? flightMonth)
    {
      var flightDate = string.Empty;

      if (flightDay.HasValue && flightMonth.HasValue)
      {
        flightDate = string.Format("{0}-{1}", flightDay.Value.ToString("00"), flightMonth.Value.ToString("00"));
      }

      return html.TextBox(controlName, flightDate, new { maxLength = 5 });
    }

    public static MvcHtmlString AttachmentIndicatorTextBox(this HtmlHelper html, string controlName, int attachmentIndicator, object htmlAttributes = null)
    {
      if (htmlAttributes == null)
      {
        htmlAttributes = new { @readOnly = true, @class = "ignoredirty" };
      }
      //var value = attachmentIndicator == false ? "No" : "Yes";
        
        string value = "No";
        
        switch (attachmentIndicator)
        {
            case 0:
                value = "No";
                break;
            case 1:
                value = "Yes";
                break;
            case 2:
                value = "Pending";
                break;
        } 
        
      return html.TextBox(controlName, value, htmlAttributes);
    }

    public static MvcHtmlString AttachmentIndicatorTextBox(this HtmlHelper html, string controlName, bool attachmentIndicator, object htmlAttributes = null)
    {
      if (htmlAttributes == null)
      {
        htmlAttributes = new { @readOnly = true, @class = "ignoredirty" };
      }
      var value = attachmentIndicator == false ? "No" : "Yes";

      return html.TextBox(controlName, value, htmlAttributes);
    }

    public static MvcHtmlString OrgNameTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, bool isBilling = true)
    {

        var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
        var result = metadata.Model != null ? metadata.Model.ToString() : string.Empty;
        const string orgNameDelimiter = "!!!";

        string value = result.Replace(orgNameDelimiter, string.Empty);

        object htmlAttributes = null;
        htmlAttributes = isBilling ? new {maxLength = 100, @class = "largeTextField populated", Value = value} : new { maxLength = 100, @class = "largeTextField", Value = value };
       
        return htmlHelper.TextBoxFor(expression, htmlAttributes);
    }

    /// <summary>
    /// This function is used to create text box for charge category.
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlName"></param>
    /// <param name="chargeCategoryId"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    public static MvcHtmlString ChargeCategoryTextBox(this HtmlHelper html, string controlName, int chargeCategoryId, object htmlAttributes = null)
    {
      IChargeCategoryManager chargeCategoryManager = Ioc.Resolve<IChargeCategoryManager>(typeof(IChargeCategoryManager));
      var chargeCategory = chargeCategoryManager.GetChargeCategoryDetails(chargeCategoryId);
      Ioc.Release(chargeCategoryManager);

     return html.TextBox(controlName, chargeCategory.Name, htmlAttributes);
    }

    /// <summary>
    /// This function is used to create text box for charge category.
    /// </summary>
    /// <param name="html"></param>
    /// <param name="controlName"></param>
    /// <param name="chargeCategoryId"></param>
    /// <param name="htmlAttributes"></param>
    /// <returns></returns>
    //CMP #636: Standard Update Mobilization
    public static MvcHtmlString ChargeCodeTextBox(this HtmlHelper html, string controlName, int chargeCodeId, object htmlAttributes = null)
    {
      IChargeCodeManager chargeCodeManager = Ioc.Resolve<IChargeCodeManager>(typeof(IChargeCodeManager));
      var chargeCode = chargeCodeManager.GetChargeCodeDetails(chargeCodeId);
      Ioc.Release(chargeCodeManager);

      return html.TextBox(controlName, chargeCode.Name, htmlAttributes);
    }
  }
}