<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Data.ManageSystemParameter.ManageSystemParameterModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  ManageSystemParameter
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript" src="<%:Url.Content("~/Scripts/SystemParameter.js")%>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    Manage System Parameter</h1>
  <div>
    <h2>
      Search Criteria</h2>
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>
          <div>
            <label>
              System Parameter Groups:</label>
            <%:Html.GetSystemParameterGroups(model => model.SystemId, new { Style = "width:250px;" })%>
          </div>
        </div>
      </div>
      <div class="clear">
      </div>
    </div>
    <div>
      <label>
      </label>
      <div class="buttonContainer">
      <input type="button" class="primaryButton" value="Search" onclick="ShowEditSystemParameter();" />
      </div>
    </div>
  </div>
  <div id="divEditSystemParameter" class="ichBlkCred-dialog">
  </div>
  <script type="text/javascript">
      $(document).ready(function () {
          $('#SystemId').focus();
          if ('<%:ViewData["SystemParameterKey"]%>' != '' && '<%:ViewData["SystemParameterKey"]%>' != null) {

              showClientSuccessMessage("Record successfully added.");
              $("#SystemId").val('<%:ViewData["SystemParameterKey"]%>'.toString());
              ShowEditSystemParameter("updated");
         }   
      });

    function ShowEditSystemParameter(status) {
      if (status != "updated")
        $('#clientSuccessMessageContainer').hide();
      var SystemParamGroup = $("#SystemId option:selected").val();
      var SystemParamDisaplyName = $("#SystemId option:selected").html();

      if (SystemParamGroup == '' || SystemParamGroup == null) {
        $("#divEditSystemParameter").html('');
        $('#clientSuccessMessageContainer').hide();
        alert("Please select SystemParameter");
      }
      else {
        // Execute UpdateBlockingMember action which will update values in database
        $.ajax({
          type: "POST",
          url: '<%: Url.Action("GetSystemParameterValue", "ManageSystemParameter", new { area = "ISOps"}) %>',
          data: { SystemParamGroup: SystemParamGroup, disaplyName: SystemParamDisaplyName },
          success: function (response) {
            $("#divEditSystemParameter").html('');
            $("#divEditSystemParameter").html(response);
            initSystemVariablesValidations("^/d*$");
          },
          error: function (xhr, textStatus, errorThrown) {

            alert('An error occurred! ' + errorThrown);
          }
        });
      }
      return false;
    }

    function CancelProcessing() {
      $("#divEditSystemParameter").html('');
      $('#clientSuccessMessageContainer').hide();
      $("#SystemId").val('');
    }
  </script>
</asp:Content>
