<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.SettlementMethod>" %>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMax">
        <div class="solidBox">
            <div class="fieldContainer horizontalMediumFlow">
                <div>
                    <div>
                        <label>
                           Settlement Method Name :</label>
                        <%: Html.TextBoxFor(model => model.Name, new { @id = "Name", @maxlength=1 })%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>

