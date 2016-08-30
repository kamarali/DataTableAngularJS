<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.ContactType>" %>
<script type="text/javascript" language="javascript">    $(document).ready(function () { $('#TypeId').focus(); });</script>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div>
                        <label>
                           Contact Type:</label>
                        <%: Html.TypeOfContactTypeDropdownListFor(model => model.TypeId)%>
                    </div>
                    <div>
                        <label>
                           Contact Type Group:</label>
                        <%: Html.ContactTypeGroupDropdownListFor(model => model.GroupId)%>
                    </div>
                    <div>
                        <label>
                           Contact Type Sub Group:</label>
                        <%: Html.ContactTypeSubGroupDropdownList(model => model.SubGroupId)%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
