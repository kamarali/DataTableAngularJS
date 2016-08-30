<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.MiscCorrespondence>" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Uatp :: Correspondence
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
      <%:Html.LinkButton("Back", Url.Action("Index", "BillingHistory", new { back = true }))%>
    </div>
    <%
    }%>
    <div class="clear">
    </div>
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

  <script type="text/javascript">
    var saveAction, sendAction;
    $(document).ready(function () {            
      saveAction = '<%:Url.Action("CreateCorrespondence", "Correspondence", new { invoiceId = Model.Invoice.Id })%>';
      readyToSubmit = '<%:Url.Action("ReadyToSubmitCorrespondence", "Correspondence", new { invoiceId = Model.Invoice.Id })%>';
      sendAction = '<%:Url.Action("CreateAndSendCorrespondence", "Correspondence", new { invoiceId = Model.Invoice.Id })%>';
      InitialiseCorrespondenceHeader();
      InitializeAttachmentGrid(<%=new JavaScriptSerializer().Serialize(Model.Attachments)%>, '<%:new FileAttachmentHelper().GetValidFileExtention(Model.ToMemberId, BillingCategoryType.Uatp)%>', '<%:Url.Action("CorrespondenceAttachmentDownload", "Correspondence")%>', '<%:Url.Content("~/Content/Images/busy.gif")%>');
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
      $("form").attr("action", saveAction);
      return true;
    }

    function ReplyCorrespondence() {           
      $("form").attr("action", sendAction);
      return true;
    }

    function ReadyToSubmitCorrespondence() {           
      $("form").attr("action", readyToSubmit);
      return true;
    }
         
    function CorrespondenceRejectionsGrid_OpenGivenRejectionRecord(cellValue, options, rowObject) {

      return "<a target='_blank' href='<%: Url.Action("View","UatpInvoice", new{ invoiceId = ViewData[ViewDataConstants.InvoiceId].ToString()}) %>'>" + cellValue + "</a>";
    }

    function CorrespondenceHistoryGrid_OpenGivenCorrespondenceRecord(cellValue, options, rowObject) {
      return "<a target='_blank' href='<%:Url.Action("ViewLinkedCorrespondence", "Correspondence")%>/" + rowObject.Id + "'>" + cellValue + "</a>";
    }
  </script>
</asp:Content>
