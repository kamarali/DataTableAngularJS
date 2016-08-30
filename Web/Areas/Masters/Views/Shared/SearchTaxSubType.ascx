<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.Common.TaxSubType>" %>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div class="editor-label">
                        <label>
                            Tax Sub Type:
                        </label>
                        <%: Html.TextBoxFor(model => model.SubType, new { @class = "alphabetsOnly upperCase", @maxLength = 20 })%>
                    </div>
                    <div class="editor-label">
                        <label>
                            Tax Type:
                        </label>
                        <%: Html.TextBoxFor(model => model.Type, new { @class = "alphabet upperCase", @maxLength = 1 })%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
