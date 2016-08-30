<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>

<script type="text/javascript">
    $(document).ready(function () {


        $('#futureMergerFromValue').removeAttr('disabled');
        $('#MemberTextFutureValue').removeAttr('disabled');

        $('#currentMergerFromValue').watermark(_periodFormat);
        $('#futureMergerFromValue').watermark(_periodFormat);

        $('#FutureIsMergedValue').change(function () {
            if ($(this).prop('checked') == false) {
                $('#futureMergerFromValue').val('');
                $('#MemberTextFutureValue').val('');
                $('#MemberIdFutureValue').val('');
                $('#FutureIsMergedValue').prop('checked', false);
                $('#FutureIsMergedValue').val(false);
                $('#futureMergerFromValue').attr('disabled', 'disabled');
                $('#MemberTextFutureValue').attr('disabled', 'disabled');
            }
            else {
                $('#futureMergerFromValue').removeAttr('disabled');
                $('#MemberTextFutureValue').removeAttr('disabled');
            }
        });


        registerAutocomplete('AutoSubdivisionFutureValue', 'AutoSubdivisionFutureValue', '<%:Url.Action("GetSubdivisionNameList", "Data", new { area = "" })%>', 0, true, null, '', '', '#DefaultLocation_CountryId');
        registerAutocomplete('AutoSponsoredbyTextFutureValue', 'AutoSponsoredIdFutureValue', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);
        registerAutocomplete('AutoAggregatedbyTextFutureValue', 'AutoAggregatedIdFutureValue', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null);

        registerAutocomplete('MemberTextFutureValue', 'MemberIdFutureValue', '<%:Url.Action("GetNonMergedMemberList", "Data", new { area = "",selectedMemberId= Model })%>', 0, true, null);
    });
</script>
<div class="solidBox dataEntry" id="futureValueDialog">
  <div class="fieldContainer verticalFlow">
    <div class="oneColumn">
      <div class="textField">
        <label>Current Value:</label>
        <%: Html.TextBox("currentTextValue", "", new { disabled = "disabled" })%>
      </div>
      <div class="textField">
        <label>New Value:</label>
        <%: Html.TextBox("futureTextValue", "")%>
      </div>
      <div class="textAreaField">
        <label>Current Value:</label>
        <%: Html.TextArea("currentTextAreaValue", "", new { disabled = "disabled" })%>
      </div>
      <div class="textAreaField">
        <label>New Value:</label>
        <%: Html.TextArea("futureTextAreaValue", "")%>
      </div>
      <div class="checkboxField">
        <label>Current Value:</label>
        <%: Html.CheckBox("currentCheckboxValue", false, new { disabled = "disabled" })%>
      </div>
      <div class="checkboxField">
        <label>New Value:</label>
        <%: Html.CheckBox("checkboxFutureValue", false)%>
      </div>
      <!--This div is used for displaying sub division field as autocomplete field-->
       <div class="AutoSubdivisionField">
        <label>Current Value:</label>
        <%: Html.TextBox("currentAutoSubdivisionValue", false, new { disabled = "disabled" })%>
      </div>
      <div class="AutoSubdivisionField">
        <label>New Value:</label>
        <%: Html.TextBox("AutoSubdivisionFutureValue", false)%>
      </div>
      <!--div for displaying sub division autocomplete fields ends-->

      <!--This div is used for displaying member list as autocomplete field-->
       <div class="AutoSponsoredField">
        <label>Current Value:</label>
        <%: Html.TextBox("currentAutoSponsoredbyTextValue", false, new { disabled = "disabled" })%>
      </div>
      <div class="AutoSponsoredField">
        <label>New Value:</label>
        <%: Html.TextBox("AutoSponsoredbyTextFutureValue", false)%>
        <%: Html.Hidden("AutoSponsoredIdFutureValue", false)%>
      </div>
      <!--div for displaying member list as autocomplete fields ends-->

       <!--This div is used for displaying agrregator member list as autocomplete field-->
       <div class="AutoAggregatedField">
        <label>Current Value:</label>
        <%: Html.TextBox("currentAutoAggregatedbyTextValue", false, new { disabled = "disabled" })%>
      </div>
      <div class="AutoAggregatedField">
        <label>New Value:</label>
        <%: Html.TextBox("AutoAggregatedbyTextFutureValue", false)%>
        <%: Html.Hidden("AutoAggregatedIdFutureValue", false)%>
      </div>
      <!--div for displaying agrregator member list as autocomplete fields ends-->

      <!--This div is used for displaying Is Merged Information-->
       <div class="checkboxIsMerged">
        <label>Is Merged (Current Value):</label>
        <%: Html.CheckBox("currentIsMergedValue", false, new { disabled = "disabled" })%>
      </div>
      <div class="checkboxIsMerged">
        <label>Is Merged (New Value):</label>
        <%: Html.CheckBox("FutureIsMergedValue", false)%>
      </div>
      <!--This div is used for displaying Is Merged Information end-->

      <!--This div is used for displaying Merger Effective From-->
        <div class="textFieldMergerFrom">
        <label>Merger Effective Period (Current Value):</label>
        <%: Html.TextBox("currentMergerFromValue", "", new { disabled = "disabled" })%>
      </div>
        <div class="textFieldMergerFrom">
        <label>Merger Effective Period (New Value):</label>
        <%: Html.TextBox("futureMergerFromValue", "")%>
      </div>
      <!--This div is used for displaying Merger Effective From ends-->

      <!--This div is used for displaying All Member list as autocomplete field-->
       <div class="MemberListField">
        <label>Parent Member (Current Value):</label>
        <%: Html.TextBox("MemberTextValue", false, new { disabled = "disabled", @class = "autocComplete largeTextField ac_input" })%>       
      </div>
      <div class="MemberListField">
        <label>Parent Member (New Value):</label>
        <%: Html.TextBox("MemberTextFutureValue",false, new { @class= "autocComplete largeTextField ac_input"})%>
        <%: Html.Hidden("MemberIdFutureValue", false)%>
      </div>
      <!--div for displaying agrregator member list as autocomplete fields ends-->

      <!--CMP#689: Flexible CH Activation Options Start-->
      <!--These div is used for displaying radio buttons-->
      <div class="radioButton">
        <label><%: Html.RadioButton("changeTimingRB", "1", true)%> Immediate</label>        
      </div>
      <div class="radioButton">
        <label><%: Html.RadioButton("changeTimingRB", "2", false)%> Future Period</label>        
      </div>
      <!--This div is used for displaying secondary title-->
      <div id="secondryTitle" class="ui-dialog-titlebar ui-widget-header">
        <label>Please Specify Effective Future Period:</label>
       </div>
      <!--CMP#689: Flexible CH Activation Options ends-->
      
      <!--This div is used for displaying dropdowns-->
       <div class="dropdownField">
        <label class="dropdownCurrentValue">Current Value:</label>
        
      </div>
      <div class="dropdownField">
        <label class="dropdownNewValue">New Value:</label>
        
      </div>
      <!--div for displaying agrregator member list as autocomplete fields ends-->
      <div id="futurePeriodContainer">
        <label>Future Period:</label>
        <%: Html.TextBox("futurePeriod", "")%>
      </div>
      <div id="futureDateContainer">
        <label>Future Date:</label>
        <%: Html.TextBox("futureDate", "", new { @class = "datePicker", @readOnly = true })%>
      </div>
    </div>
  </div>
  <div class="clear">
  
  </div>
</div>
