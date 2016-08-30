<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Web.Areas.Misc.Models.LineItemDetailViewModel>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Miscellaneous :: <%:ViewData[ViewDataConstants.BillingType] %> ::
  <%if (ViewData[ViewDataConstants.PageMode].ToString() == "Clone")
    {%> Edit <%}
    else
    {%>
  <%:ViewData[ViewDataConstants.PageMode]%>
  <%}%>
  Line Item Detail
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
   <%if (ViewData[ViewDataConstants.PageMode].ToString() == "Clone")
    {%> Edit <%}
    else
    {%>
  <%:ViewData[ViewDataConstants.PageMode]%>
  <%}%>
    Line Item Detail</h1>
  <div>
    <% Html.RenderPartial("~/Views/MiscUatp/InvoiceHeaderInfoControl.ascx", Model.LineItemDetail.LineItem.Invoice); %>
  </div>
  <div>
    <% Html.RenderPartial("~/Views/MiscUatp/ReadonlyLineItemHeaderControl.ascx", Model.LineItemDetail.LineItem); %>
  </div>
  <% using (Html.BeginForm("LineItemDetailEdit", "MiscInvoice", FormMethod.Post, new { id = "LineItemDetailForm", @class = "validCharacters" }))
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
    <input class="secondaryButton" value="First" id="First" onclick="javascript:location.href='<%: Url.Action(actionToRedirect, "MiscPayInvoice", new { invoiceId = Model.LineItemDetail.LineItem.InvoiceId, lineItemId = Model.LineItemDetail.LineItemId, lineItemDetailId = Model.LineItemDetail.NavigationDetails.FirstId }) %>'"
      type="button" />
    <input class="secondaryButton" value="Previous" id="Previous" onclick="javascript:location.href='<%: Url.Action(actionToRedirect, "MiscPayInvoice", new { invoiceId = Model.LineItemDetail.LineItem.InvoiceId, lineItemId = Model.LineItemDetail.LineItemId, lineItemDetailId = Model.LineItemDetail.NavigationDetails.PreviousId }) %>'"
      type="button" />
    <%}
       else
       {%>
    <input class="secondaryButton" value="First" id="First" disabled="disabled" type="button" />
    <input class="secondaryButton" value="Previous" id="Previous" type="button" disabled="disabled" />
    <%   
      }%>
    <% if (Model.LineItemDetail.Id != Model.LineItemDetail.NavigationDetails.LastId)
       {%>
    <input class="secondaryButton" value="Next" id="Next" onclick="javascript:location.href='<%: Url.Action(actionToRedirect, "MiscPayInvoice", new { invoiceId = Model.LineItemDetail.LineItem.InvoiceId, lineItemId = Model.LineItemDetail.LineItemId, lineItemDetailId = Model.LineItemDetail.NavigationDetails.NextId }) %>'"
      type="button" />
    <input class="secondaryButton" value="Last" id="Last" onclick="javascript:location.href='<%: Url.Action(actionToRedirect, "MiscPayInvoice", new { invoiceId = Model.LineItemDetail.LineItem.InvoiceId, lineItemId = Model.LineItemDetail.LineItemId, lineItemDetailId = Model.LineItemDetail.NavigationDetails.LastId }) %>'"
      type="button" />
    <%}
       else
       {%>
    <input class="secondaryButton" value="Next" id="Next" disabled="disabled" type="button" />
    <input class="secondaryButton" value="Last" id="Last" disabled="disabled" type="button" />
    <%
      }%>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save And Add" id="Save" onclick="javascript:return changeAction('<%: Url.Action("LineItemDetailEdit","MiscInvoice", new { lineItemDetailId = Model.LineItemDetail.Id.ToString() }) %>'); return false;" />
    <input class="primaryButton ignoredirty" type="submit" value="Save And Duplicate" id="SaveDuplicate"
      onclick="javascript:return changeAction('<%: Url.Action("LineItemDetailClone","MiscInvoice", new { lineItemDetailId = Model.LineItemDetail.Id.ToString() })%>')" />
    <input class="primaryButton ignoredirty" type="submit" value="Save And Back" id="SaveReturn"
      onclick="javascript:return changeAction('<%: Url.Action("LineItemDetailEditAndReturn","MiscInvoice", new { lineItemDetailId = Model.LineItemDetail.Id.ToString() }) %>');" />
    
    <% if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
        {
          if (Request.Form["searchType"] != null && Request.Form["searchType"] == "p")
          {
        %>
                   <%:Html.LinkButton("Back", Url.Action("LineItemView", "MiscPayInvoice", new {searchType = "p"}))%>
                   <%
          }
          else
          {
        %>
              <%:Html.LinkButton("Back", Url.Action("LineItemView", "MiscPayInvoice"))%>
            <%
          }
        }
        else
       {%>
      <%: Html.LinkButton("Back", Url.Action("LineItemEdit", "MiscInvoice"))%>
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
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/Misc/FieldCloner.js")%>"></script>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/Misc/LineItemDetail.js") %>"></script>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/Misc/TaxBreakdown.js") %>"></script>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/Misc/VATBreakdown.js") %>"></script>
  <script type="text/javascript" src="<%: Url.Content("~/Scripts/Misc/AddCharge.js") %>"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      initializeParentForm('LineItemDetailForm');
      // Set variable to true if PageMode is "View"
      $isOnView = <%:(ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View ? true : false ).ToString().ToLower() %>;
      SetPageToViewModeEx(<%: ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>, '#LineItemDetailForm');
      CheckServiceDateOnCreateMode();
      InitializeTaxGrid(<%= new JavaScriptSerializer().Serialize(Model.LineItemDetail.TaxBreakdown.Where(i=>i.Type==TaxType.Tax)) %>);
      InitializeVatGrid(<%= new JavaScriptSerializer().Serialize(Model.LineItemDetail.TaxBreakdown.Where(i=>i.Type ==TaxType.VAT)) %>);
      
      InitializeAddChargeGrid('<%= new JavaScriptSerializer().Serialize(Model.LineItemDetail.AddOnCharges) %>', true);
      SetMiscPageWaterMark();
      InitialiseLineItemDetail('<%: Url.Action("GetGroupHtml", "Data", new { area = ""})%>', '<%: Url.Action("GetSubdivisionCodesByCountryCode","Data", new { area = ""}) %>');
      if(<%: ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>)
       $(".linkImage").hide();

      HideOptionalFieldsOnPageRender();      
    });
  </script>
</asp:Content>
