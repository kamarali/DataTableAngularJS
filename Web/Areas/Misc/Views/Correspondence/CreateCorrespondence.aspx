<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.MiscCorrespondence>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Miscellaneous :: Correspondence
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Correspondence</h1>
  <%
    using (Html.BeginForm(null, null, FormMethod.Post, new { id = ControlIdConstants.CorrespondenceHeader }))
    {%>
    <%: Html.AntiForgeryToken() %>
  <div>
    <h2>
      Create Correspondence</h2>
    <%
      Html.RenderPartial("CreateCorrespondenceControl", Model);%>
    <div class="buttonContainer">
      <input type="submit" value="Save" class="primaryButton ignoredirty" id="SaveCorrespondence" />
      <input type="submit" value="Send" class="primaryButton ignoredirty" id="SendCorrespondence" />
      <input type="submit" value="Ready To Submit" class="primaryButton ignoredirty" id="SaveCorrespondenceAsReadyToSubmit" />
      <input type="button" id="UploadAttachment" value="Attachments" class="primaryButton ignoredirty" />
      
    </div>
    <%
    }%>
    <%--CMP 527: Show close buttons on predefine conditions--%>
   <%if (Convert.ToBoolean(ViewData[ViewDataConstants.CorrespondenceCanClose]))
              {
                  using (Html.BeginForm("CloseCorrespondence", "Correspondence", new { area = "Misc", correspondenceId = Model.Id, scenarioId = Convert.ToInt32(ViewData[ViewDataConstants.CorrespondeneClosedScenario]) }, FormMethod.Post, new { id = "frmCloseCorrespondence" }))
                {%>
                   <%: Html.AntiForgeryToken() %>
                   <input class="primaryButton ignoredirty" onclick="return closeCorrespondence('<%:Model.Id%>','<%:Convert.ToInt16(ViewData[ViewDataConstants.CorrespondeneClosedScenario])%>');" type="submit" value="Close Correspondence" />

                   <%:Html.LinkButton("Back", Url.Action("Index", "BillingHistory", new { back = true }))%>
                <%
    }}%>
    <div class="clear">
    </div>
  </div>
  <%--  CMP527: Show comments if closed by initatior--%>
   <div id="CloseCorrespondence" class="hidden">
    <% using (Html.BeginForm("CloseCorrespondence", "Correspondence", new { area = "Misc", correspondenceId = Model.Id, invoiceId = Model.InvoiceId, scenarioId = Convert.ToInt32(ViewData[ViewDataConstants.CorrespondeneClosedScenario]) }, FormMethod.Post, new { id = "frmCloseCorrespondence" }))
       {%> 
       <%: Html.AntiForgeryToken() %>
      
       <%  Html.RenderPartial("~/Views/Correspondence/AcceptanceComments.ascx", Model);%>
    <%  }%>
  </div>
  <div class="hidden" id="divAttachment">
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
  </div>  
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    // Set billing type from Viewdata
    billingType = '<%: ViewData[ViewDataConstants.BillingType] %>';
  </script>
  <script src="<%:Url.Content("~/Scripts/Misc/MiscCorrespondence.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/jquery.uploadify.v2.1.0.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/swfobject.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/Pax/AttachmentsBreakdown.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/multifile_compressed.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/jquery.blockUI.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/correspondence/Correspondence.js")%>" type="text/javascript"></script>
 
  <script type="text/javascript">
    var saveAction, sendAction;
    $(document).ready(function () {            
      saveAction = '<%:Url.Action("CreateCorrespondence", "Correspondence", new { invoiceId = Model.Invoice.Id })%>';
      readyToSubmit = '<%:Url.Action("ReadyToSubmitCorrespondence", "Correspondence", new { invoiceId = Model.Invoice.Id })%>';
      sendAction = '<%:Url.Action("CreateAndSendCorrespondence", "Correspondence", new { invoiceId = Model.Invoice.Id })%>';
      InitialiseCorrespondenceHeader();
      InitializeAttachmentGrid(<%=new JavaScriptSerializer().Serialize(Model.Attachments)%>, '<%:new FileAttachmentHelper().GetValidFileExtention(Model.ToMemberId, BillingCategoryType.Misc)%>', '<%:Url.Action("CorrespondenceAttachmentDownload", "Correspondence")%>', '<%:Url.Content("~/Content/Images/busy.gif")%>');
      $("#SaveCorrespondence").bind("click", SaveCorrespondence);
      $("#SendCorrespondence").bind("click", ReplyCorrespondence);
      $("#SaveCorrespondenceAsReadyToSubmit").bind("click", ReadyToSubmitCorrespondence);
    <%
      if (ViewData[ViewDataConstants.CorrespondenceInitiator] != null && Convert.ToBoolean(ViewData[ViewDataConstants.CorrespondenceInitiator].ToString()))
      {%>
      $('#AuthorityToBill').attr('disabled', true);
    <%}%>

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

      return "<a target='_blank' href='<%: Url.Action("View","MiscInvoice", new{ invoiceId = ViewData[ViewDataConstants.InvoiceId].ToString()}) %>'>" + cellValue + "</a>";
    }

    function CorrespondenceHistoryGrid_OpenGivenCorrespondenceRecord(cellValue, options, rowObject) {
      return "<a target='_blank' href='<%:Url.Action("ViewLinkedCorrespondence", "Correspondence")%>/" + rowObject.Id + "'>" + cellValue + "</a>";
    }
  </script>
</asp:Content>
