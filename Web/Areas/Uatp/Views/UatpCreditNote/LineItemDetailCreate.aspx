<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Web.Areas.Misc.Models.LineItemDetailViewModel>" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: UATP :: Receivables :: Create Line Item Detail
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Line Item Detail</h1>
  <div>
    <% Html.RenderPartial("~/Views/MiscUatp/InvoiceHeaderInfoControl.ascx", Model.LineItemDetail.LineItem.Invoice); %>
  </div>
   <div>
    <% Html.RenderPartial("~/Views/MiscUatp/ReadonlyLineItemHeaderControl.ascx", Model.LineItemDetail.LineItem); %>
  </div>
  <% using (Html.BeginForm(null, null, FormMethod.Post, new { id = "LineItemDetailForm", @class = "validCharacters" }))
     {  %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <% Html.RenderPartial("~/Views/MiscUatp/LineItemDetailControl.ascx", Model); %>
  </div>
  <div class="buttonContainer">
    <% if(Model.LineItemDetail.LineItem.LineItemDetails.Count > 0){%>
    <input class="secondaryButton" value="First" onclick = "javascript:location.href='<%: Url.Action("LineItemDetailEdit", "UatpCreditNote", new { invoiceId = Model.LineItemDetail.LineItem.InvoiceId, lineItemId = Model.LineItemDetail.LineItem.Id, lineItemDetailId = Model.LineItemDetail.LineItem.NavigationDetails.FirstId }) %>'" type="button"/>
    <input class="secondaryButton" value="Previous" onclick = "javascript:location.href='<%: Url.Action("LineItemDetailEdit", "UatpCreditNote", new { invoiceId = Model.LineItemDetail.LineItem.InvoiceId, lineItemId = Model.LineItemDetail.LineItem.Id, lineItemDetailId = Model.LineItemDetail.LineItem.NavigationDetails.LastId }) %>'" type="button"/>
    <%} else
    {%>
    <input class="secondaryButton" value="First" disabled = "disabled" type="button"/>
    <input class="secondaryButton" value="Previous" type="button" disabled ="disabled" />
    <%
    }%>
  </div>
   <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save And Add" id="Save" onclick="javascript:return changeAction('<%: Url.Action("LineItemDetailCreate","UatpCreditNote") %>')"  />
    <input class="primaryButton ignoredirty" type="submit" value="Save And Duplicate" id="SaveDuplicate" onclick="javascript:return changeAction('<%: Url.Action("LineItemDetailDuplicate","UatpCreditNote") %>')" />
    <input class="primaryButton ignoredirty" type="submit" value="Save And Back" id="SaveReturn" onclick="javascript:return changeAction('<%: Url.Action("LineItemDetailReturn","UatpCreditNote") %>')" />
    <% if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
    { %>
      <%: Html.LinkButton("Back", Url.Action("LineItemView", "UatpCreditNote"))%>
    <%} else {%>
      <%: Html.LinkButton("Back", Url.Action("LineItemEdit", "UatpCreditNote"))%>
    <%} %>
  </div>
  <%} %>
  <div id="FieldTemplates" class="hidden">
  </div>
  <div id="TaxBreakdown" class="hidden">
    <% Html.RenderPartial("~/Views/MiscUatp/LineItemDetailTaxControl.ascx", Model.LineItemDetail.TaxBreakdown);%>
  </div>
  <div id="VATBreakdown" class="hidden">
    <% Html.RenderPartial("~/Views/MiscUatp/LineItemDetailVATControl.ascx", Model.LineItemDetail.TaxBreakdown);%>
  </div>
  <div id="AddChargeBreakdown" class="hidden">
    <% Html.RenderPartial("~/Views/MiscUatp/LineItemDetailAddChargeControl.ascx", Model.LineItemDetail.AddOnCharges);%>
  </div>
  <div class="clear">
  </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/FieldCloner.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/LineItemDetail.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Uatp/TaxBreakdown.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Uatp/VATBreakdown.js")%>"></script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/AddCharge.js")%>"></script>
  <script type="text/javascript">
    $(document).ready(function () {
      initializeParentForm('LineItemDetailForm');
      CheckServiceDateOnCreateMode();
      InitializeTaxGrid('<%= new JavaScriptSerializer().Serialize(Model.LineItemDetail.TaxBreakdown.Where(i => i.Type == TaxType.Tax))%>');
      InitializeVatGrid('<%= new JavaScriptSerializer().Serialize(Model.LineItemDetail.TaxBreakdown.Where(i => i.Type == TaxType.VAT))%>');
    
      InitializeAddChargeGrid('<%= new JavaScriptSerializer().Serialize(Model.LineItemDetail.AddOnCharges)%>', true);
     <%if(ViewData[ViewDataConstants.IsExceptionOccurred]  == null || ((bool)ViewData[ViewDataConstants.IsExceptionOccurred]  != true))
       {
         if (ViewData[ViewDataConstants.PageMode] == PageMode.Create)
         {%>
      ClearDefaultValuesOnCreateMode(<%:ViewData[ViewDataConstants.RetainLineItemDetailStartDate] ?? 0%>);<%
         }
       }%>
        
      HideOptionalFieldsOnPageRender();
      //for credit note only
      $('#UnitPrice').attr('watermark','negativeFourDecimalPlaces');
      $('#ChargeAmount').attr('watermark','negativeAmount');
      SetMiscPageWaterMark();
      InitialiseLineItemDetail('<%: Url.Action("GetGroupHtml", "Data", new { area = ""})%>', '<%: Url.Action("GetSubdivisionCodesByCountryCode","Data", new { area = ""}) %>');
       <% if(ViewData[ViewDataConstants.IsExceptionOccurred]  == null || ((bool)ViewData[ViewDataConstants.IsExceptionOccurred]  != true))
         {
           if (ViewData[ViewDataConstants.PageMode] == PageMode.Clone)
           {%>
      DefaultValuesOnCloneMode();<%
           }
         }%>
          // UOM code should be disabled and defaulted to EA for UATP. (UATP Web Review Issue)
    $('#UomCodeId').attr('disabled', true);
    });
</script>
</asp:Content>
