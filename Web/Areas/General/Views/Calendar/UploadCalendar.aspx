<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  Upload Calendar Data
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    ICH & ACH Calendar</h1>
  <h2>
    Upload Calendar Data</h2>
  <form id="ajaxUploadForm" action="<%: Url.Action("UploadCalendar", "Calendar", new { area = "General"})%>" method="post"
  enctype="multipart/form-data">
  <%: Html.AntiForgeryToken() %>
  <div>
    <label id="lblMultiFileUpload" class="labelAttachment fieldContainer">
      File to Upload:</label> <span>
        <input id="file_element" type="file" name="file" /></span></div>
  
  <div class="verticalFlow">
    <div class="fieldContainer">
      <label>
        File with header row:</label>
      <input id="withHeader" value="True" type="radio" title="With Header" name="headerFlag" />
    </div>
    <div class="fieldContainer">
      <label>
        File without header row:</label>
      <input id="withoutHeader" value="False" type="radio" title="Without Header" name="headerFlag" />
    </div>
  </div>
  <div class="clear">
    <span>Note: The calendar data will not be imported till next billing period.</span>
  </div>
  <div class="buttonContainer">
    <input id="ajaxUploadButton" type="submit" value="Submit" class="primaryButton" />
    <input type="submit" value="Regenerate Triggers" class="primaryButton ignoredirty" id="RegenerateTriggers" onclick="javascript:return changeAction('<%:Url.Action("RegenerateTriggers", "Calendar", new { area="General" })%>')" />
  </div>
  <div id="files_list">
  </div>
  </form>
  <%Html.RenderPartial("ValidationErrorDetailsControl"); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    $(document).ready(function () {

      $("#ajaxUploadButton").click(function () {
        var headerFlag = $("input:radio[name='headerFlag']:checked");
        if (headerFlag.length <= 0) {
          alert('Please select header row option.'); return false;
        }
        return true;
      });
    });
  </script>
</asp:Content>
