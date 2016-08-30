<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Web.Areas.Misc.Models.LineItemDetailViewModel>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Miscellaneous :: Receivables ::
  <%:ViewData[ViewDataConstants.PageMode] %>
  Line Item Detail
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%:ViewData[ViewDataConstants.PageMode] %>
    Line Item Detail</h1>
  <div>
    <% Html.RenderPartial("~/Views/MiscUatp/InvoiceHeaderInfoControl.ascx", Model.LineItemDetail.LineItem.Invoice); %>
  </div>
  <div>
    <% Html.RenderPartial("~/Views/MiscUatp/ReadonlyLineItemHeaderControl.ascx", Model.LineItemDetail.LineItem); %>
  </div>
  <% using (Html.BeginForm("LineItemDetailEdit", "MiscCreditNote", FormMethod.Post, new { id = "LineItemDetailForm", @class = "validCharacters" }))
     {  %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("~/Views/MiscUatp/LineItemDetailControl.ascx", Model); %>
  </div>
<div class="buttonContainer">
    <%
      var actionToRedirect = "LineItemDetailEdit";
      if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
        actionToRedirect = "LineItemDetailView";
    %>
    <% if (Model.LineItemDetail.Id != Model.LineItemDetail.NavigationDetails.FirstId)
       {%>
    <input class="secondaryButton" value="First" onclick="javascript:location.href='<%: Url.Action(actionToRedirect, "MiscCreditNote", new { invoiceId = Model.LineItemDetail.LineItem.InvoiceId, lineItemId = Model.LineItemDetail.LineItemId, lineItemDetailId = Model.LineItemDetail.NavigationDetails.FirstId }) %>'"
      type="button" />
    <input class="secondaryButton" value="Previous" onclick="javascript:location.href='<%: Url.Action(actionToRedirect, "MiscCreditNote", new { invoiceId = Model.LineItemDetail.LineItem.InvoiceId, lineItemId = Model.LineItemDetail.LineItemId, lineItemDetailId = Model.LineItemDetail.NavigationDetails.PreviousId }) %>'"
      type="button" />
    <%}
       else
       {%>
    <input class="secondaryButton" value="First" disabled="disabled" type="button" />
    <input class="secondaryButton" value="Previous" type="button" disabled="disabled" />
    <%   
      }%>
    <% if (Model.LineItemDetail.Id != Model.LineItemDetail.NavigationDetails.LastId)
       {%>
    <input class="secondaryButton" value="Next" onclick="javascript:location.href='<%: Url.Action(actionToRedirect, "MiscCreditNote", new { invoiceId = Model.LineItemDetail.LineItem.InvoiceId, lineItemId = Model.LineItemDetail.LineItemId, lineItemDetailId = Model.LineItemDetail.NavigationDetails.NextId }) %>'"
      type="button" />
    <input class="secondaryButton" value="Last" onclick="javascript:location.href='<%: Url.Action(actionToRedirect, "MiscCreditNote", new { invoiceId = Model.LineItemDetail.LineItem.InvoiceId, lineItemId = Model.LineItemDetail.LineItemId, lineItemDetailId = Model.LineItemDetail.NavigationDetails.LastId }) %>'"
      type="button" />
    <%}
       else
       {%>
    <input class="secondaryButton" value="Next" disabled="disabled" type="button" />
    <input class="secondaryButton" value="Last" disabled="disabled" type="button" />
    <%
      }%>
  </div>
  <!--SIS_SCR_REPORT_23_jun-2016_2 : Value_Shadowing Change Request[""] to Request.Form[""] -->
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save And Add" id="Save" onclick="javascript:return changeAction('<%: Url.Action("LineItemDetailEdit","MiscCreditNote", new { lineItemDetailId = Model.LineItemDetail.Id.ToString() }) %>'); return false;" />
    <input class="primaryButton ignoredirty" type="submit" value="Save And Duplicate" id="SaveDuplicate"
      onclick="javascript:return changeAction('<%: Url.Action("LineItemDetailClone","MiscCreditNote", new { lineItemDetailId = Model.LineItemDetail.Id.ToString() })%>')" />
    <input class="primaryButton ignoredirty" type="submit" value="Save And Back" id="SaveReturn"
      onclick="javascript:return changeAction('<%: Url.Action("LineItemDetailEditAndReturn","MiscCreditNote", new { lineItemDetailId = Model.LineItemDetail.Id.ToString() }) %>');" />
    <% if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
       {
         if (Request.Form["searchType"] != null && Request.Form["searchType"] == "p")
         {
           %>
           <%:Html.LinkButton("Back", Url.Action("LineItemView", "MiscCreditNote", new { searchType = "p" }))%>
           <%
         }
         else
         {
%>
      <%:Html.LinkButton("Back", Url.Action("LineItemView", "MiscCreditNote"))%>
    <%
         }
       }
       else
       {%>
      <%: Html.LinkButton("Back", Url.Action("LineItemEdit", "MiscCreditNote"))%>
    <%} %>
  </div>
  <%} %>
  <div id="FieldTemplates" class="hidden">
  </div>
  <div id="TaxBreakdown" class="hidden">
    <% Html.RenderPartial("~/Views/MiscUatp/LineItemDetailTaxControl.ascx", Model.LineItemDetail.TaxBreakdown);%>
  </div>
  <div id="VATBreakdown" class="hidden">
    <% Html.RenderPartial("~/Views/MiscUatp/LineItemDetailVATControl.ascx", Model.LineItemDetail.TaxBreakdown.Where(tax => tax.Type == "Vat").ToList());%>
  </div>
  <div id="AddChargeBreakdown" class="hidden">
    <% Html.RenderPartial("~/Views/MiscUatp/LineItemDetailAddChargeControl.ascx", Model.LineItemDetail.AddOnCharges);%>
  </div>
  <div class="clear">
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/FieldCloner.js")%>"></script>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/Misc/LineItemDetail.js") %>"></script>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/Misc/TaxBreakdown.js") %>"></script>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/Misc/VATBreakdown.js") %>"></script>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/Misc/AddCharge.js") %>"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      initializeParentForm('LineItemDetailForm');
      SetPageToViewModeEx(<%: ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>,'#LineItemDetailForm');
      CheckServiceDateOnCreateMode();
      InitializeTaxGrid(<%= new JavaScriptSerializer().Serialize(Model.LineItemDetail.TaxBreakdown.Where(i=>i.Type==TaxType.Tax)) %>);
      InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.LineItemDetail.TaxBreakdown.Where(i=>i.Type == TaxType.VAT)) %>);
      
      InitializeAddChargeGrid('<%= new JavaScriptSerializer().Serialize(Model.LineItemDetail.AddOnCharges) %>', true);      
      if(<%: ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>)
       $(".linkImage").hide();
      HideOptionalFieldsOnPageRender();

      //for credit note only
      $('#UnitPrice').attr('watermark','negativeFourDecimalPlaces');
      $('#ChargeAmount').attr('watermark','negativeAmount');
      SetMiscPageWaterMark();
      InitialiseLineItemDetail('<%: Url.Action("GetGroupHtml", "Data", new { area = ""})%>', '<%: Url.Action("GetSubdivisionCodesByCountryCode","Data", new { area = ""}) %>');
    });
  </script>
</asp:Content>
