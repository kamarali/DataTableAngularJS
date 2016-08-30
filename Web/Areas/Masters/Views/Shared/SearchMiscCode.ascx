<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.MiscCode>" %>
<div>
    <h2>
        Search Criteria</h2>
    <div >
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div style = "width:210px" >
                        <label>
                            Miscellaneous Code Group:
                        </label>
                        <%: Html.MiscCodeGroupDropdownListFor(model => model.Group, new {style = "width:200px" })%>
                       </div>
                    <div>
                        <label>
                            Miscellaneous Code:
                        </label>
                        <%: Html.TextBoxFor(model => model.Name, new { @class = "alphaNumeric upperCase", @maxLength = 50 })%>
                        <%: Html.ValidationMessageFor(model => model.Name)%>
                    </div>
                    <div style="width:400px;">
                        <label>
                            Description:
                        </label>
                        <%: Html.TextBoxFor(model => model.Description, new { @id = "Description", @maxLength = 255 })%>                        
                        <%: Html.ValidationMessageFor(model => model.Description)%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
