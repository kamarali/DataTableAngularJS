<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.Common.AchCurrencySetUp>" %>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div>
                        <label>
                            Currency of Clearance:
                        </label>
                        <%: Html.TextBoxFor(model => model.Name, new { @class = "alphaNumeric upperCase", @maxLength = 3 })%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
