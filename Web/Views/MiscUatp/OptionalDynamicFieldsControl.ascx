<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Iata.IS.Model.MiscUatp.Common.FieldMetaData>>" %>
<%@ Import Namespace="Iata.IS.Model.MiscUatp.Enums" %>
<div class="fieldContainer horizontalFlowFor4FieldsPerLine">
  <h2>
    Optional Information</h2>
  <div class="topLine">
  </div>
</div>
<div class="fieldContainer horizontalFlowFor4FieldsPerLine" id="divOptionalGroup">
<div>
    <%
      var scriptForgroup = string.Empty;
      foreach (var fieldMetaData in Model)
      {
        var grpScript = string.Empty;
        if (fieldMetaData.FieldType == FieldType.Group)
        {%>
    <%: Html.DynamicFieldGroup(fieldMetaData, Url.Action("GetDynamicFieldAutocompleteData", "Data", new { area = "" }), Url.Content("~/Content/Images/"), "FieldTemplates", out grpScript)%>
    <%
}
    scriptForgroup += grpScript;
  } %>
    <script type="text/javascript">
    $(document).ready(function () {
    <%: scriptForgroup %>
    });
    </script>
    </div>
</div>

