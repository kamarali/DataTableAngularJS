<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Sampling.SamplingFormEDetail>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>



<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: <%: ViewData[ViewDataConstants.BillingType].ToString() %> :: <%:ViewData[ViewDataConstants.PageMode].ToString()%> Form E
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <%:ViewData[ViewDataConstants.PageMode].ToString()%> Form E</h1>
     <input type="hidden" id="FormBAmmount" value='<%: ViewData["FormAbTotal"] %>' />
  <div>
    <% Html.RenderPartial("SamplingInvoiceHeaderInfoControl", Model.Invoice); %>
  </div>
  <% using (Html.BeginForm("FormEEdit", "FormDE", new { transactionId = Model.Id }, FormMethod.Post, new { id = "formE", @class = "validCharacters" }))
     {%>
     <%: Html.AntiForgeryToken() %>
  <div>
    <%
       Html.RenderPartial("FormEControl", Model);%>
  </div>
  <div class="buttonContainer">
    <input class="primaryButton ignoredirty" type="submit" value="Save" id="Save"/>
    <%
       if (ViewData[ViewDataConstants.PageMode] != null && ViewData[ViewDataConstants.PageMode].ToString() == PageMode.View)
       {%>
    <input class="secondaryButton" type="button" value="Back" onclick="javascript:location.href = '<%:Url.Action("View",
                                      "FormDEPayables",
                                      new
                                        {
                                          invoiceId = Model.Id
                                        })%>'" />
    <%
       }
       else
       {%>
          <%: Html.LinkButton("Back", Url.Action("Edit", "FormDE", new { invoiceId = Model.Id }))%>
    <%
       }
     }%>
  </div>
  <div class="buttonContainer">
      <%
          // TODO: Need to figure out which invoice status should be check here
        if ((Model.Invoice.InvoiceStatus == InvoiceStatusType.Open || Model.Invoice.InvoiceStatus == InvoiceStatusType.ValidationError) && Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Pax.Receivables.SampleFormDE.Validate))// || Model.Invoice.InvoiceStatus == InvoiceStatus.ReadyForValidation || Model.Invoice.InvoiceStatus == InvoiceStatus.PendingForCorrections)
    {
      using (Html.BeginForm("ValidateInvoice", "FormDE", FormMethod.Post))
      {
%>
    <%: Html.AntiForgeryToken() %>
    <input class="primaryButton" type="submit" value="Validate Invoice" />    
    <%
      }
    }
      if (Model.Invoice.InvoiceStatus == InvoiceStatusType.ReadyForSubmission && Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Pax.Receivables.SampleFormDE.Submit))
      {
        using (Html.BeginForm("Submit", "FormDE", FormMethod.Post))
        {%>
        <%: Html.AntiForgeryToken() %>
    <input class="primaryButton" type="submit" value="Submit Invoice" />
    <%
        }
      }%>
  </div>
  <div class="clear">
  </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
 
  <script type="text/javascript">
    $(document).ready(function () {
      SetPageToViewMode(<%: ((string)ViewData[ViewDataConstants.PageMode] == PageMode.View).ToString().ToLower()%>);
      /* CMP #596: Length of Member Accounting Code to be Increased to 12 
        Desc: Issuing Airlines auto-complete should exclude pure numeric values having size 5 or greater
        Ref: FRS Section 3.4 Table 17 Row 7 */
      registerAutocomplete('TicketIssuingAirline', 'TicketIssuingAirline', '<%:Url.Action("GetTicketIssuingAirlineListForPaxCgo", "Data", new { area = "" })%>', 0, true, null);
      registerAutocomplete('ReasonCode', 'ReasonCode', '<%:Url.Action("GetReasonCodeList", "Data", new { area="" })%>', 0, true, null, '', '<%:Convert.ToInt32(TransactionType.SamplingFormD)%>');
      
      var SMIAch = '<%: Model.Invoice.InvoiceSmi == SMI.Ach %>';
      setGlobalVariables(SMIAch,'<%: Model.Invoice.IsFormABViaIS %>','<%: Model.Invoice.IsFormCViaIS %>');
      
      InializeLinking();
    });
  </script>
   <script src='<%:Url.Content("~/Scripts/Pax/SamplingFormE.js")%>' type="text/javascript"></script>  
</asp:Content>
