<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Correspondence>" %>

<%@ Import Namespace="Iata.IS.Business.Security.Permissions.Pax.DownloadCorrespondences" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Business.Security.Permissions.Pax" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Pax :: Correspondence
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    View Correspondence</h1>
  <%
    using (Html.BeginForm(null, null, FormMethod.Post, new {id = ControlIdConstants.CorrespondenceHeader}))
    {%>
    <%
      Html.RenderPartial("PaxCorrespondenceControl", Model);%>
    <div class="buttonContainer">
      <%
        //CMP573 User to be Able to Reply to a Correspondence by Clicking the URL in a Correspondence Alert
        if (Convert.ToBoolean(ViewData[ViewDataConstants.IsCorrespondenceEligibleForReply]))
        {
%>
       <input type="button" value="Reply" class="primaryButton ignoredirty" id="ReplyCorrespondence"
                onclick="window.location.href='<%:Url.Action("ReplyCorrespondence", "Correspondence", new {transactionId = Model.Id})%>';" />
             
           
      <%
      }%>
       <%--SCP#447047: Correspondences --%>
       <%--<% if (Html.IsAuthorized(DownloadCorrespondences.PDF))
        {%>--%>
      <input class="primaryButton ignoredirty" onclick="javascript:window.open('<%:Url.Action("DownloadCorrespondence", "Correspondence", new { invoiceId = Model.InvoiceId, transactionId = Model.Id })%>');" type="button" value="Download PDF" />      
      <%--<%}%>--%>
     <%-- TFS#9995 : Mozilla:V46 - Attachment button color remain Orange after clicking on Save Button- Create Correspondence Screen.--%>
      <input class="primaryButton ignoredirty" id="UploadAttachment" onclick="return openAttachment();" type="submit" value="Attachments" />
     
    
   
     <%--CMP527: show close correspondence button on pre define conditions.--%>
      <%if (Convert.ToBoolean(ViewData[ViewDataConstants.CorrespondenceCanClose]))
              {%>
                   <%-- SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202] --%>
                   <input class="primaryButton ignoredirty" onclick="return CheckIfBMExistsThenCloseCorrespondence('<%: Model.CorrespondenceNumber%>', '<%:Model.Id%>', '<%: Model.AuthorityToBill%>', '<%: Url.Action("CheckIfBMExistsOnReply", "Correspondence", new { area = "Pax" }) %>', '<%:Convert.ToInt16(ViewData[ViewDataConstants.CorrespondeneClosedScenario])%>')" type="button" value="Close Correspondence" />
            <%
            } %>
            <%:Html.LinkButton("Back", Url.Action("Index", "BillingHistory", new { back = true }))%>
      <%}%>
    </div>
    <div class="clear">
    </div>
  <%--CMP527: show accpetance comments required textbox.--%>
    <div id="CloseCorrespondence" class="hidden">
       <% using (Html.BeginForm("CloseCorrespondence", "Correspondence", new { area = "Pax", correspondenceId = Model.Id, correspondenceStage = Model.CorrespondenceStage, correspondenceStatus = Model.CorrespondenceStatusId, correspondenceSubStatus = Model.CorrespondenceSubStatusId == (int)CorrespondenceSubStatus.Received ? (int)CorrespondenceSubStatus.Responded : Model.CorrespondenceSubStatusId, scenarioId = Convert.ToInt32(ViewData[ViewDataConstants.CorrespondeneClosedScenario]) }, FormMethod.Post, new { id = "frmCloseCorrespondence" }))
       {%>
        <%: Html.AntiForgeryToken() %>
        <%
           Html.RenderPartial("~/Views/Correspondence/AcceptanceComments.ascx", Model);%>
    <%
       }%>
    </div>
  <div id="divAttachment" class="hidden">
    <%
    Html.RenderPartial("PaxCorrespondenceAttachmentControl", Model);%>
  </div>
  <div style="width:1000px;">
    <div style="float: left;">
      <h2>
        Linked Rejections</h2>
      <%
    Html.RenderPartial("GridControl", ViewData[ViewDataConstants.CorrespondenceRejectionsGrid]);%>
    </div>

    <div style="float: left; padding-left: 20px;">
      <h2>
        Correspondence History</h2> 
       
     <div >
      <%
    Html.RenderPartial("GridControl", ViewData[ViewDataConstants.CorrespondenceHistoryGrid]);%>
    </div>
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
  <script src="<%:Url.Content("~/Scripts/Pax/PaxCorrespondence.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/multifile_compressed.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/jquery.blockUI.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/correspondence/Correspondence.js")%>" type="text/javascript"></script>

  <script type="text/javascript">        
		$(document).ready(function () {
			InitialiseCorrespondenceHeader();          
			$isOnView = true;                   
			InitializeAttachmentGrid(<%=new JavaScriptSerializer().Serialize(Model.Attachments)%>, '<%:new FileAttachmentHelper().GetValidFileExtention(Model.ToMemberId, BillingCategoryType.Pax)%>', '<%:Url.Action("CorrespondenceAttachmentDownload", "Correspondence")%>', '<%:Url.Content("~/Content/Images/busy.gif")%>');
           
          $("#CorrespondenceHistoryGrid").setGridWidth(480);
          
               
		});

      
		
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
