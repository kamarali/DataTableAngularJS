<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.BlockingRule>" %>
<script type="text/javascript">

    $(document).ready(function () {
        $('#ichMemberText').focus();
        var MenuType = '<%: ViewData["clearingHouse"]%>';

        if (MenuType == 'ICH') {
           // CMP-670-ICH Blocks for ACH-Only Members-FRS-v1.1.doc
            registerAutocomplete('ichMemberText', 'MemberId', '<%:Url.Action("GetMemberListForIchOrAch", "Data", new { area = "",menuType="bothAchIch" })%>', 0, true, null);
        } else if (MenuType == 'ACH') {
            registerAutocomplete('ichMemberText', 'MemberId', '<%:Url.Action("GetMemberListForIchOrAch", "Data", new { area = "", menuType="bothAchIch"})%>', 0, true, null);
        }

        $("#searchBlockingRule").change(function () {

            $("#Id").val($("#searchBlockingRule").val());
        });
    });
</script>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                Desc: Non layout related IS-WEB screen changes.
                Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
        <div class="solidBox" style="width: 150%">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div>
                        <label>
                            Member Code:</label>
                        <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                                Desc: Non layout related IS-WEB screen changes.
                                Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
                        <%: Html.TextBoxFor(model => model.MemberText, new { @id = "ichMemberText", @Class = "autocComplete textboxWidth" })%>
                        <%: Html.HiddenFor(model => model.MemberId, new { @id = "MemberId" })%>
                    </div>
                    <div>
                        <label>
                            Blocking Rule:</label>
                        <%: Html.TextBoxFor(model => model.RuleName, new { @id = "searchBlockingRule" })%>                        
                    </div>
                    <div>
                       <%-- <label>
                            Description:</label>
                        <%: Html.TextAreaFor(model => model.Description, 3, 60, new { @id = "searchDescription" })%>--%>
                        <%:Html.HiddenFor(model=>model.Id)%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
