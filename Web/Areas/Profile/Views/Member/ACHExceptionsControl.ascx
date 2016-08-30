<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.AchConfiguration>" %>
<!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
        Desc: Non layout related IS-WEB screen changes.
        Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
<div style="width: 120%">
  <div class="fieldContainer verticalFlow">
    <div class="twoColumnWidth">
      <div>
        <%:Html.Hidden("BillingCategory")%>
        <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                Desc: Non layout related IS-WEB screen changes.
                Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
        <%: Html.TextBoxFor(ach => ach.DiaplayMemberText, new { @id = "exceptionMemberText", @class = "autocComplete textboxWidth" })%>
        <%: Html.TextBoxFor( ach => ach.MemberId,new{@id="exceptionMemberId" , @class="hidden"})%>
        <input id="hiddenpaxMemberIdAdd" type="hidden" />
        <input id="hiddenpaxMemberIdRemove" type="hidden" />
        <input id="hiddencgoMemberIdAdd" type="hidden" />
        <input id="hiddencgoMemberIdRemove" type="hidden" />
        <input id="hiddenmiscMemberIdAdd" type="hidden" />
        <input id="hiddenmiscMemberIdRemove" type="hidden" />
        <input id="hiddenuatpMemberIdAdd" type="hidden" />
        <input id="hiddenuatpMemberIdRemove" type="hidden" />
        <input id="hiddenSelfId" type="hidden" value="<%:Session["SelectedMemberId"]%>" />
      </div>
    </div>
    <div class="oneColumnWidth">
      <div class="buttonContainer">
        <input class="secondaryButton" id="addExceptionMember" value="Add" type="button"
          onclick="AddMember();" />
      </div>
    </div>
  </div>
  <div class="fieldContainer verticalFlow">
    <div class="twoColumnWidth">
      <div>
        <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                Desc: Non layout related IS-WEB screen changes.
                Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
        <%:Html.ListBox("exceptionMemberspax", (MultiSelectList)ViewData["paxexceptionMemberList"], new { size = "8", @class = "listboxWidth" })%>
        <%:Html.ListBox("exceptionMemberscgo", (MultiSelectList)ViewData["cgoexceptionMemberList"], new { size = "8", @class = "listboxWidth" })%>
        <%:Html.ListBox("exceptionMembersmisc", (MultiSelectList)ViewData["miscexceptionMemberList"], new { size = "8", @class = "listboxWidth" })%>
        <%:Html.ListBox("exceptionMembersuatp", (MultiSelectList)ViewData["uatpexceptionMemberList"], new { size = "8", @class = "listboxWidth" })%>
      </div>
      <div id="paxExceptionsdiv">
        <label class="hidden" id="paxExcLabel">
          Future Period:</label>
        <%:Html.TextBoxFor(achModel => achModel.PaxExceptionFuturePeriod, new { @class = "hidden" })%>
      </div>
      <div id="cgoExceptionsdiv">
        <label class="hidden" id="cgoExcLabel">
          Future Period:</label>
        <%:Html.TextBoxFor(achModel => achModel.CgoExceptionFuturePeriod, new { @class = "hidden" })%>
      </div>
      <div id="miscExceptionsdiv">
        <label class="hidden" id="miscExcLabel">
          Future Period:</label>
        <%:Html.TextBoxFor(achModel => achModel.MiscExceptionFuturePeriod, new { @class = "hidden" })%>
      </div>
      <div id="uatpExceptionsdiv">
        <label class="hidden" id="uatpExcLabel">
          Future Period:</label>
        <%:Html.TextBoxFor(achModel => achModel.UatpExceptionFuturePeriod, new { @class = "hidden" })%>
      </div>
    </div>
    <div class="oneColumnWidth">
      <div class="buttonContainer">
        <input class="secondaryButton" id="removeExceptionMember" value="Remove" type="button"
          onclick="DeleteMember();" />
      </div>
    </div>
  </div>
</div>
