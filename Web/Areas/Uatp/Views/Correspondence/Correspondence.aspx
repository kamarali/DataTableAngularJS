<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.MiscCorrespondence>" %>

<%@ Import Namespace="Iata.IS.Business.Security.Permissions.UATP.DownloadCorrespondences" %>

<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Uatp :: Correspondence
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Correspondence</h1>
    <div>
  <%
    using (Html.BeginForm(null, null, FormMethod.Post, new { id = ControlIdConstants.CorrespondenceHeader }))
    {%>
    <%
      Html.RenderPartial("CorrespondenceControl", Model);%>
    <div class="buttonContainer">
      <%
          //CMP573 User to be Able to Reply to a Correspondence by Clicking the URL in a Correspondence Alert
         if (Convert.ToBoolean(ViewData[ViewDataConstants.IsCorrespondenceEligibleForReply]))
        {
      %>
      <input type="button" value="Reply" class="primaryButton ignoredirty" id="ReplyCorrespondence"
                onclick="window.location.href='<%:Url.Action("ReplyCorrespondence", "Correspondence", new {invoiceId = Model.InvoiceId,transactionId = Model.Id})%>';" />
       <%
        }
      
      %>
      <%--SCP#447047: Correspondences --%>
      <%--<% if (Html.IsAuthorized(DownloadCorrespondences.PDF))
        {%>--%>
      <input type="button" value="Download PDF" class="primaryButton ignoredirty" onclick="javascript:window.open('<%:Url.Action("DownloadCorrespondence", "Correspondence", new { invoiceId = Model.InvoiceId, transactionId = Model.Id })%>');" />
      <%--<%}%>--%>
      <input type="button" id="UploadAttachment" value="Attachments" class="primaryButton ignoredirty" />
      
     <%--CMP 527: Show close buttons on predefine conditions--%>
     <%if (Convert.ToBoolean(ViewData[ViewDataConstants.CorrespondenceCanClose]))
              {%>
                   <input class="primaryButton ignoredirty" onclick="return closeCorrespondence('<%:Model.Id%>','<%:Convert.ToInt16(ViewData[ViewDataConstants.CorrespondeneClosedScenario])%>');" type="button" value="Close Correspondence" />
                <%
            }%>
            <%:Html.LinkButton("Back", Url.Action("Index", "BillingHistory", new { back = true }))%>
    <%
    }%>
    </div>
    <div class="clear">
    </div>
  </div>
  <div id="divAttachment" class="hidden">
    <%
      Html.RenderPartial("CorrespondenceAttachmentControl", Model);%>
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
     <%--  CMP527: Show comments if closed by initatior--%>
   <div id="CloseCorrespondence" class="hidden">
    <% using (Html.BeginForm("CloseCorrespondence", "Correspondence", new { area = "UATP", correspondenceId = Model.Id, invoiceId = Model.InvoiceId, correspondenceStage = Model.CorrespondenceStage, correspondenceStatus = Model.CorrespondenceStatusId, correspondenceSubStatus = Model.CorrespondenceSubStatusId == (int)CorrespondenceSubStatus.Received ? (int)CorrespondenceSubStatus.Responded : Model.CorrespondenceSubStatusId, scenarioId = Convert.ToInt32(ViewData[ViewDataConstants.CorrespondeneClosedScenario]) }, FormMethod.Post, new { id = "frmCloseCorrespondence" }))
       {%>
       <%: Html.AntiForgeryToken() %>
       <%  
         Html.RenderPartial("~/Views/Correspondence/AcceptanceComments.ascx", Model);
    }%>
  </div>
  </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Misc/MiscCorrespondence.js")%>"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/multifile_compressed.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/jquery.blockUI.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/correspondence/Correspondence.js")%>" type="text/javascript"></script>
 
  <script type="text/javascript">        
    $(document).ready(function () {
      InitialiseCorrespondenceHeader();          
      $isOnView = true;                   
      InitializeAttachmentGrid(<%=new JavaScriptSerializer().Serialize(Model.Attachments)%>, '<%:new FileAttachmentHelper().GetValidFileExtention(Model.ToMemberId, BillingCategoryType.Uatp)%>', '<%:Url.Action("CorrespondenceAttachmentDownload", "Correspondence")%>', '<%:Url.Content("~/Content/Images/busy.gif")%>');
    });

    function CorrespondenceRejectionsGrid_OpenGivenRejectionRecord(cellValue, options, rowObject) {
         return "<a target='_blank' href='<%: Url.Action("View","UatpInvoice", new{ invoiceId = ViewData[ViewDataConstants.InvoiceId].ToString()}) %>'>" + cellValue + "</a>";
    }
    
    function CorrespondenceHistoryGrid_OpenGivenCorrespondenceRecord(cellValue, options, rowObject) {
      return "<a target=_ blank href=<%=Url.Action("ViewLinkedCorrespondence", "Correspondence")%>/" + rowObject.Id + ">" + cellValue + "</a>";
    }
  </script>
</asp:Content>
