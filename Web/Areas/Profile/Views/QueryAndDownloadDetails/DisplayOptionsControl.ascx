<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.QueryAndDownloadDetails>" %>
<%@ Import Namespace="Iata.IS.Model.MemberProfile" %>
<script type="text/javascript">
  $(document).ready(function () {

    //Initialize sort information grid
    setSortGrid();

    //If you want to move selected item from availableFields to selectedFields
    $("#add").click(function () {
      $("#availableFields option:selected").appendTo("#selectedFields");
    });

    //If you want to move all item from availableFields to selectedFields
    $("#addAll").click(function () {
      $("#availableFields option").appendTo("#selectedFields");
    });

    //If you want to remove selected item from selectedFields to availableFields
    $("#remove").click(function () {
      $("#selectedFields option:selected").each(function () {
        $(this).appendTo("#availableFields");
        var selrowId = $(this).val();
        try {
          var sortRow = $("#sortGrid tr[id='" + selrowId + "']");
          if (sortRow != null) {
            $("#sortGrid").delRowData(selrowId);
          }
        }
        catch (e) {
          alert(e);
        }
      });
    });

    //If you want to remove all items from selectedFields to availableFields
    $("#removeAll").click(function () {
      $("#selectedFields option").appendTo("#availableFields");
      $("#sortGrid").clearGridData();
    });

  });
</script>
<h2>
  Display Options</h2>
<div>
  <div class="fieldContainer verticalFlow">
    <div class="twoColumn">
      <div class="twoColumn">
        <div>
          <label for="availableFields">
            Available Fields</label>
          <%:Html.ListBox("availableFields", (MultiSelectList)ViewData["ProfileMetadataAvailableFields"], new { size = "10", @class = "oneColumn disableTemp", style = "width:150px;" })%></div>
      </div>
      <div style="width: 12%;">
        <br />
        <br />
        <div>
          <input type="button" class="shiftButton disableTemp" value=">" id="add" />
        </div>
        <div>
          <input type="button" class="shiftButton disableTemp" value=">>" id="addAll" />
        </div>
        <div>
          <input type="button" class="shiftButton disableTemp" value="<" id="remove" />
        </div>
        <div>
          <input type="button" class="shiftButton disableTemp" value="<<" id="removeAll" />
        </div>
      </div>
      <div class="twoColumn">
        <div>
          <label for="selectedFields">
            Selected Fields</label>
          <%:Html.ListBox("selectedFields", new MultiSelectList(new ContactType[0]), new { size = "10", @class = "oneColumn disableTemp", style = "width:150px;"})%></div>
      </div>
    </div>
    <div id="sortDiv" class="halfWidthColumn">
      <div style="width: 20%">
        <br />
        <br />
        <br />
        <div>
          <input type="button" class="secondaryButton twoColumnWidth disableTemp" value="Include" onclick="addRow('sortGrid')" />
        </div>
        <div>
          <input type="button" class="secondaryButton twoColumnWidth disableTemp" value="Exclude" onclick="deleteRow('sortGrid')" />
        </div>
      </div>
      <div style="width: 80%;">
        <%Html.RenderPartial("SortOnControl");%>
      </div>
    </div>
    <div class="clear">
    </div>
  </div>
  <div class="buttonContainer">
    <input type="button" class="primaryButton" value="Generate Report" id="btnDownloadReport" />
    <input id="selectedMetaList" name="selectedMetaList" type="hidden" value="" />
    <input id="sortIds" name="sortIds" type="hidden" value="" />
    <input id="sortOrder" name="sortOrder" type="hidden" value="" />
    <input id="reportType" name="reportType" type="hidden" value="0" />
    <input id="allowToDownloadCSVFile" name="allowToDownloadCSVFile" type="hidden" value="<%: ViewData["AllowContactDetailsDownload"] %>" />
  </div>
</div>
