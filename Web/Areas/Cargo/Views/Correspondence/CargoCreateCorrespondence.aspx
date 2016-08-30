<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Cargo.CargoCorrespondence>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Business.Security.Permissions.Cargo" %>
<%@ Import Namespace="Iata.IS.Business.Security.Permissions.Cargo.BillingHistoryAndCorrespondence" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Cargo :: Correspondence
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Correspondence</h1>
  <% using (Html.BeginForm("CargoCreateCorrespondence", "Correspondence", new { invoiceId = ViewData[ViewDataConstants.InvoiceId] }, FormMethod.Post, new { id = ControlIdConstants.CorrespondenceHeader }))
     {  %>
     <%: Html.AntiForgeryToken() %>
  <div>
    <h2>
      Create Correspondence</h2>
     <%-- <%: Html.Hidden("invoiceId", new{ value=ViewData[ViewDataConstants.InvoiceId] }) %>--%>
    <%Html.RenderPartial("CargoCreateCorrespondenceControl", Model); %>
    <div class="buttonContainer">
      <%
        if (Html.IsAuthorized(BillingHistoryAndCorrespondence.DraftCorrespondence))
        {
      %>
      <input type="submit" value="Save" class="primaryButton ignoredirty" id="CorrespondenceSave" />
      <%
        }
      %>
      <%
        if (Html.IsAuthorized(BillingHistoryAndCorrespondence.SendCorrespondence))
        {
      %>
      <input class="primaryButton ignoredirty" id="SendCorrespondence" type="submit" value="Send" />
      <%
        }
      %>
      <%
        if (Html.IsAuthorized(BillingHistoryAndCorrespondence.DraftCorrespondence))
        {
      %>
      <input class="primaryButton ignoredirty" id="SaveCorrespondenceAsReadyToSubmit" type="submit" value="Ready To Submit" />
      <%
        }
      %>
      <input class="primaryButton ignoredirty" id="attachmentBreakdown" onclick="return openAttachment();" type="button" value="Attachments" />
      <%: Html.LinkButton("Back", Url.Action("Index", "BillingHistory", new { back = true }))%>
    </div>
    <%
      }%>
    <div class="clear">
    </div>
  </div>
  <div class="hidden" id="divAttachment">
    <%
      Html.RenderPartial("CargoCorrespondenceAttachmentControl", Model);%>
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
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%: Url.Content("~/Scripts/Cargo/CargoCorrespondence.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.uploadify.v2.1.0.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/swfobject.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/Cargo/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/multifile_compressed.js") %>" type="text/javascript"></script>
  <script src="<%: Url.Content("~/Scripts/jquery.blockUI.js") %>" type="text/javascript"></script>

  <script type="text/javascript">
		var saveAction, sendAction;
		$(document).ready(function () {    
			saveAction = '<%: Url.Action("CargoCreateCorrespondence","Correspondence", new {invoiceId = Request.RequestContext.RouteData.Values["invoiceId"]}) %>';
			readyToSubmit = '<%: Url.Action("ReadyToSubmitCorrespondence","Correspondence", new {invoiceId = ViewData[ViewDataConstants.InvoiceId], transactionId = Model.Id}) %>';
			sendAction = '<%: Url.Action("CreateAndSendCorrespondence","Correspondence", new {invoiceId = ViewData[ViewDataConstants.InvoiceId]}) %>';
			InitialiseCorrespondenceHeader();
			InitializeAttachmentGrid(<%= new JavaScriptSerializer().Serialize(Model.Attachments) %>, '<%: new FileAttachmentHelper().GetValidFileExtention(Model.ToMemberId, BillingCategoryType.Cgo) %>', '<%: Url.Action("CorrespondenceAttachmentDownload","Correspondence") %>', '<%:Url.Content("~/Content/Images/busy.gif")%>');
			$("#SaveCorrespondence").bind("click", SaveCorrespondence);
			$("#SendCorrespondence").bind("click", ReplyCorrespondence);
			$("#SaveCorrespondenceAsReadyToSubmit").bind("click", ReadyToSubmitCorrespondence);
		  <%
		  if(ViewData[ViewDataConstants.CorrespondenceInitiator] != null && Convert.ToBoolean(ViewData[ViewDataConstants.CorrespondenceInitiator].ToString()))
		  {
		  %>
		  $('#AuthorityToBill').attr('disabled', true);
		  <%
		  }
		  %>

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
			return "<a target='_blank' href='<%: Url.Action("ViewLinkedCorrespondence","Correspondence") %>/" + rowObject.Id + "'>" + cellValue + "</a>";
		}
  </script>
</asp:Content>
