<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Correspondence>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Business.Security.Permissions.Pax" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Pax :: Correspondence
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/PaxCorrespondence.js")%>"></script>
  <h1>
    Correspondence</h1>
    <div>
  <%
    using (
      Html.BeginForm("EditPaxCorrespondence",
                     "Correspondence",
                     new { invoiceId = Request.RequestContext.RouteData.Values["invoiceId"] },
                     FormMethod.Post,
                     new { id = ControlIdConstants.CorrespondenceHeader }))
    {%>
    <%: Html.AntiForgeryToken() %>
   <h2>
      Edit Correspondence</h2>
    <%
      Html.RenderPartial("PaxCreateCorrespondenceControl", Model);%>
    <div class="buttonContainer">
      <%
      if (Html.IsAuthorized(BillingHistoryAndCorrespondence.DraftCorrespondence) && Model.ExpiryDate >= DateTime.UtcNow.Date)
      {
      %>
      <input type="submit" value="Save" class="primaryButton ignoredirty" id="SaveCorrespondence" />
      <%
      }
      %>
      <%
      if (Html.IsAuthorized(BillingHistoryAndCorrespondence.SendCorrespondence) && Model.ExpiryDate >= DateTime.UtcNow.Date)
      {
      %>
      <input class="primaryButton ignoredirty" id="SendCorrespondence" type="submit" value="Send" />
      <%
      }
      %>
      <%
      if (Html.IsAuthorized(BillingHistoryAndCorrespondence.DraftCorrespondence) && Model.ExpiryDate >= DateTime.UtcNow.Date)
      {
      %>
      <input class="primaryButton ignoredirty" id="SaveCorrespondenceAsReadyToSubmit" type="submit" value="Ready To Submit" />
      <%
      }
      %>
      <%
      if (Model.ExpiryDate >= DateTime.UtcNow.Date)
      {%>
      <input class="primaryButton ignoredirty" id="UploadAttachment" onclick="return openAttachment();" type="button" value="Attachments" />
      <%
      }%>
        
        
        <%--CMP527: show close correspondence button on pre define conditions.--%>
        <%if (Convert.ToBoolean(ViewData[ViewDataConstants.CorrespondenceCanClose]))
            {%>
                <input class="primaryButton ignoredirty" onclick="return closeCorrespondence('<%:Model.Id%>','<%:Convert.ToInt16(ViewData[ViewDataConstants.CorrespondeneClosedScenario])%>');" type="submit" value="Close Correspondence" />
            <%
        }%>
        <%:Html.LinkButton("Back", Url.Action("Index", "BillingHistory", new { back = true }))%>
         <%
    }%>
    </div>
    <div class="clear">
    </div>
   <%--CMP527: show accpetance comments required textbox.--%>
    <div id="CloseCorrespondence" class="hidden">
       <% using (Html.BeginForm("CloseCorrespondence", "Correspondence", new { area = "Pax", correspondenceId = Model.Id, correspondenceStage = Model.CorrespondenceStage, correspondenceStatus = Model.CorrespondenceStatusId, correspondenceSubStatus = Model.CorrespondenceSubStatusId == (int)CorrespondenceSubStatus.Received ? (int)CorrespondenceSubStatus.Responded : Model.CorrespondenceSubStatusId, scenarioId = Convert.ToInt32(ViewData[ViewDataConstants.CorrespondeneClosedScenario]) }, FormMethod.Post, new { id = "frmCloseCorrespondence" }))
       { %>
              <%: Html.AntiForgeryToken() %>
         <% Html.RenderPartial("~/Views/Correspondence/AcceptanceComments.ascx", Model);
    }%>
    </div>
  <div class="hidden" id="divAttachment">
    <%
    Html.RenderPartial("PaxCorrespondenceAttachmentControl", Model);%>
  </div>
  <div>
    <div style="float: left;">
      <h2>
        Linked Rejections</h2>
      <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.CorrespondenceRejectionsGrid]);%>
    </div>
    <div style="float: left; padding-left: 20px;">
      <h2>
        Correspondence History</h2>
      <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.CorrespondenceHistoryGrid]);%>
    </div>
    <div class="clear">
    </div>
  </div>
  </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%:Url.Content("~/Scripts/Pax/PaxCorrespondence.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/multifile_compressed.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/jquery.blockUI.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/correspondence/Correspondence.js")%>" type="text/javascript"></script>
  <script type="text/javascript">   
    var saveAction, sendAction;                 
    $(document).ready(function () {          
      saveAction = '<%:Url.Action("EditPaxCorrespondence", "Correspondence", new { invoiceId = Request.RequestContext.RouteData.Values["invoiceId"], transactionId = Model.Id })%>';
      readyToSubmit = '<%:Url.Action("ReadyToSubmitCorrespondence", "Correspondence", new { invoiceId = Request.RequestContext.RouteData.Values["invoiceId"], transactionId = Model.Id })%>';
      sendAction = '<%:Url.Action("SendCorrespondence", "Correspondence", new { transactionId = Model.Id })%>';
      InitialiseCorrespondenceHeader();
      InitializeAttachmentGrid(<%=new JavaScriptSerializer().Serialize(Model.Attachments)%>, '<%:new FileAttachmentHelper().GetValidFileExtention(Model.ToMemberId, BillingCategoryType.Pax)%>', '<%:Url.Action("CorrespondenceAttachmentDownload", "Correspondence")%>', '<%:Url.Content("~/Content/Images/busy.gif")%>');
      $("#SaveCorrespondence").bind("click", SaveCorrespondence);
      $("#SendCorrespondence").bind("click", ReplyCorrespondence);
      $("#SaveCorrespondenceAsReadyToSubmit").bind("click", ReadyToSubmitCorrespondence);
    
    <%
      if (ViewData[ViewDataConstants.CorrespondenceInitiator] != null && Convert.ToBoolean(ViewData[ViewDataConstants.CorrespondenceInitiator].ToString()))
      {%>
      $('#AuthorityToBill').attr('disabled', true);
    <%
      }%>

      // While creating Correspondence, if RejectionInvoice has Settlement Method Indicator value as "Bilateral", enable "CurrencyId" dropdown
      <% if(ViewData[ViewDataConstants.IsSMILikeBilateral] != null && Convert.ToBoolean(ViewData[ViewDataConstants.IsSMILikeBilateral]) == true) {%>
         $('#CurrencyId').attr('disabled', false);
      <% }%>
    });

    function SaveCorrespondence() {
      //CMP #661: Popup Before Sending a Correspondence
      _isSendClicked = false;
      $("form").attr("action", saveAction);
      return true;
    }

    function ReplyCorrespondence() { 
      //CMP #661: Popup Before Sending a Correspondence 
      // set flag true here, which will further used to show confirmation popup after validation on submitHandler         
      _isSendClicked = true;          
      $("form").attr("action", sendAction);
      return true;
    }

    function ReadyToSubmitCorrespondence() {    
      //CMP #661: Popup Before Sending a Correspondence
       _isSendClicked = false;         
      $("form").attr("action", readyToSubmit);
      return true;
    }

   function CorrespondenceRejectionsGrid_OpenGivenRejectionRecord(cellValue, options, rowObject) {
		
      if(rowObject.BillingCode == 0) // non-sampling
      {
        return "<a target='_blank' href='<%: Url.Action("RMView","Invoice", new {invoiceId = ViewData[ViewDataConstants.InvoiceId].ToString()}) %>/" + rowObject.Id + "?fc=true'>" + cellValue + "</a>";
      }
      else if (rowObject.BillingCode == 6) // Form F
      {
        return "<a target='_blank' href='<%: Url.Action("RMView","FormF", new {invoiceId = ViewData[ViewDataConstants.InvoiceId].ToString()}) %>/" + rowObject.Id + "?fc=true'>" + cellValue + "</a>";
      }
      else if (rowObject.BillingCode == 7) // Form XF
      {
        return "<a target='_blank' href='<%: Url.Action("RMView","FormXF", new {invoiceId = ViewData[ViewDataConstants.InvoiceId].ToString()}) %>/" + rowObject.Id + "?fc=true'>" + cellValue + "</a>";
      }
		}

    function CorrespondenceHistoryGrid_OpenGivenCorrespondenceRecord(cellValue, options, rowObject) {
      return "<a target='_blank' href='<%: Url.Action("ViewLinkedCorrespondence", "Correspondence")%>/" + rowObject.Id + "'>" + cellValue + "</a>";
    }
  </script>
</asp:Content>
