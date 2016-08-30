<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.RejectionMemo>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Passenger :: Correspondence
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Create Correspondence</h1>
  <div>
    <%Html.RenderPartial("CreateCorrespondenceControl", Model.Correspondence); %>
    <div class="buttonContainer">
      <input type="submit" value="Save" class="primaryButton" />
      <input type="submit" value="Reply/Send" class="primaryButton" />
      <% using (Html.BeginForm("AttachmentCapture", "Shared", new { area = "Shared" }, FormMethod.Post, new { style = "display:inline" }))
         { %>
      <input type="submit" value="Upload Attachment" class="primaryButton" />
      <% } %>
      <input type="submit" value="Back" class="secondaryButton" onclick="history.go(-1)" />
    </div>
    <div class="clear">
    </div>
  </div> 
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
